﻿using mojoPortal.Core.Configuration;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace mojoPortal.Web.Controls.DatePicker
{
	public class AirDatepicker : TextBox
	{


		protected override void OnPreRender(EventArgs e)
		{
			base.OnPreRender(e);

			if (HttpContext.Current == null) { return; }
			if (HttpContext.Current.Request == null) { return; }

			LangCode = GetSupportedLangCode(CultureInfo.CurrentCulture.Name, CultureInfo.CurrentCulture.TwoLetterISOLanguageName);

			SetupScript();
		}

		private string GetSupportedLangCode(string cultureName, string twoLetterISOLanguageName)
		{
			string localeFolderPath = Page.Server.MapPath(LocalePath);

			DirectoryInfo dir = new DirectoryInfo(localeFolderPath);
			var files = new List<FileInfo>();
			if (dir.Exists)
			{
				files = dir.GetFiles("*.js", SearchOption.TopDirectoryOnly).ToList();
			}
			
			if (files.Any(f => f.Name.Replace(".js", string.Empty) == cultureName))
			{
				return cultureName;
			}
			else if (files.Any(f => f.Name.Replace(".js", string.Empty) == twoLetterISOLanguageName))
			{
				return twoLetterISOLanguageName;
			}
			else
			{
				return "en";
			}
		}

		private void SetupScript()
		{
			ScriptManager.RegisterClientScriptBlock(this, GetType(), "airDate", 
$@"
<link rel=""stylesheet"" href=""{ResolveUrl(StylePath)}""/>
<script src=""{ResolveUrl(ScriptPath)}""></script>
<script>
window.airDatepickerExt = {{
	selectDateOpts: {{updateTime:true,silent:true}},
	nowButton: {{
		content: 'Now',
		onClick: (dp) => {{
			const date = new Date();
			dp.selectDate(date,{{updateTime:true}});
			dp.setViewDate(date);
		}},
	}}, //end nowButton
	todayButton: {{
		content: 'Today',
		onClick: (dp) => {{
			const date = new Date();
			dp.selectDate(date);
			dp.setViewDate(date);
		}},
    }}, //end todayButton
	pickers: {{}}
}};
</script>
", false);

			string relatedPickerScript = string.Empty; //will be populated if RelatedPickerControl has value
	
			if (!string.IsNullOrWhiteSpace(RelatedPickerControl))
			{
				string relatedPickerBaseScript = $@"<script>
(function(){{
	function ensureDateRange(dateRange){{
		if (dateRange.startInstance.lastSelectedDate.valueOf() > dateRange.endInstance.lastSelectedDate.valueOf()) {{
			if (dateRange.caller === 'start') {{
				dateRange.endInstance.selectDate(incrementHours(dateRange.startInstance.lastSelectedDate));
			}}
			if (dateRange.caller === 'end') {{
				dateRange.startInstance.selectDate(decrementHours(dateRange.endInstance.lastSelectedDate));
			}}
		}}
	}}

	function incrementHours(date) {{
		const result = new Date(date);
		result.setMinutes(result.getMinutes() + 30);
		return result;
	}}

	function decrementHours(date) {{
		const result = new Date(date);
		result.setMinutes(result.getMinutes() - 30);
		return result;
	}}

	window.airDatepickerExt.relatedEnsureRange = ensureDateRange;
}})();
</script>";

				string caller = "start";
				string startInstance = ClientID;
				string endInstance = RelatedPickerControl;

				if (RelatedPickerRelation == RelatedPickerRelation.Start)
				{
					// RelatedPickerRelation.Start means the RelatedPickerControl is the "start date", so this picker is the "end date"
					caller = "end";
					startInstance = RelatedPickerControl;
					endInstance = ClientID;
				}

				relatedPickerScript = $@"onSelect: function(date, formattedDate, instance) {{
	window.airDatepickerExt.relatedEnsureRange({{
		caller: '{caller}',
		startInstance: window.airDatepickerExt.pickers.{startInstance},
		endInstance: window.airDatepickerExt.pickers.{endInstance}
	}});
}}";
				ScriptManager.RegisterStartupScript(this, GetType(), "relatedPickerBaseScript", relatedPickerBaseScript, false);
			}

			//string airdateScriptBase = $@"<script>window.airDatepickerSelectDateOpts = {{updateTime:true,silent:true}};</script>";
			//ScriptManager.RegisterStartupScript(this, GetType(), "airdateScriptBase", airdateScriptBase, false);

			DateTime thisDateTime = DateTime.Now;
			if (!string.IsNullOrWhiteSpace(Text))
			{
				thisDateTime = DateTime.Parse(Text);
			}

			string airdateScriptSingleton = $@"<script type=""module"">
import {ClientID}_thelocale from '{ResolveUrl(LocalePath + LangCode + ".js")}';
window.airDatepickerExt.pickers.{ClientID} = new AirDatepicker('#{ClientID}', {{
	locale: {ClientID}_thelocale,
	timepicker: {ShowTime.ToString().ToLower()},
	buttons: [window.airDatepickerExt.todayButton,{(ShowTime ? "window.airDatepickerExt.nowButton," : string.Empty)}'clear'],
	keyboardNav: {KeyboardNav.ToString().ToLower()},
	{relatedPickerScript} 
}});

{(string.IsNullOrWhiteSpace(Text) ? string.Empty : $@"window.airDatepickerExt.pickers.{ClientID}.selectDate(new Date(
	{thisDateTime.Year},
	{thisDateTime.Month - 1},
	{thisDateTime.Day},
	{thisDateTime.Hour},
	{thisDateTime.Minute}
),	
window.airDatepickerExt.selectDateOpts);")}
</script>";
			ScriptManager.RegisterStartupScript(this, GetType(), UniqueID + "airdateScriptSingleton", airdateScriptSingleton, false);
		}

		public string ScriptPath { get; set; } = ConfigHelper.GetStringProperty("AirdateScriptPath", "~/ClientScript/air-datepicker/air-datepicker.js");
		public string LocalePath { get; set; } = ConfigHelper.GetStringProperty("AirdateLocalePath", "~/ClientScript/air-datepicker/locale/");
		public string StylePath { get; set; } = ConfigHelper.GetStringProperty("AirdateStylePath", "~/ClientScript/air-datepicker/air-datepicker.css");

		/// <summary>
		/// Disables (true) or enables (false) the timepicker. Can be set when initialising (first creating) the datepicker.
		/// </summary>
		public bool ShowTime { get; set; } = false;
		/// <summary>
		/// if true the control will be rendered only as a time picker with not datepicker
		/// </summary>
		public bool ShowTimeOnly { get; set; } = false;
		/// <summary>
		/// 12 or 24
		/// </summary>
		public string ClockHours { get; set; } = "12";

		public bool AutoLocalize { get; set; } = true;

		public string LangCode { get; set; } = "en";

		/// <summary>
		/// this allows localizing the Done button in the time picker
		/// </summary>
		public string DoneLabel { get; set; } = "Done";

		/// <summary>
		/// this allows localizing the Hour label in the time picker
		/// </summary>
		public string HourLabel { get; set; } = "Hour";

		/// <summary>
		/// this allows localizing the Minute label in the time picker
		/// </summary>
		public string MinuteLabel { get; set; } = "Minute";

		/// <summary>
		/// this allows localizing the AM label in the time picker
		/// </summary>
		public string AmDesignator { get; set; } = "AM";

		/// <summary>
		/// this allows localizing the PM label in the time picker
		/// </summary>
		public string PmDesignator { get; set; } = "PM";
		/// <summary>
		/// The URL for the popup button image. If set, buttonText becomes the alt value and is not directly displayed.
		/// </summary>
		public string ButtonImage { get; set; } = string.Empty;

		/// <summary>
		/// Set to true to place an image after the field to use as the trigger without it appearing on a button.
		/// </summary>
		public bool ButtonImageOnly { get; set; } = false;
		/// <summary>
		/// A function to calculate the week of the year for a given date. 
		/// The default implementation uses the ISO 8601 definition: weeks start on a Monday; 
		/// the first week of the year contains the first Thursday of the year.
		/// </summary>
		public string CalculateWeek { get; set; } = string.Empty;

		// *** enabled by ghalib ghniem Aug-14-2011 ChangeMonth: bool ,ChangeYear: bool, YearRange: string c-10:c+10
		/// <summary>
		/// Allows you to change the month by selecting from a drop-down list. You can enable this feature by setting the attribute to true.
		/// </summary>
		public bool ChangeMonth { get; set; } = false;

		/// <summary>
		/// Allows you to change the year by selecting from a drop-down list. 
		/// You can enable this feature by setting the attribute to true. 
		/// Use the yearRange option to control which years are made available for selection.
		/// </summary>
		public bool ChangeYear { get; set; } = false;
		/// <summary>
		/// Control the range of years displayed in the year drop-down: either relative to today's year (-nn:+nn), relative to the currently selected year (c-nn:c+nn), 
		/// absolute (nnnn:nnnn), or combinations of these formats (nnnn:-nn). 
		/// Note that this option only affects what appears in the drop-down, 
		/// to restrict which dates may be selected use the minDate and/or maxDate options.
		/// </summary>
		public string YearRange { get; set; } = "c-10:c+10";

		/// <summary>
		/// When true entry in the input field is constrained to those characters allowed by the current dateFormat.
		/// </summary>
		public bool ConstrainInput { get; set; } = false;
		/// <summary>
		/// Set the first day of the week: Sunday is 0, Monday is 1, ... 
		/// This attribute is one of the regionalisation attributes.
		/// -1 use default don't set it by script
		/// </summary>
		public int FirstDay { get; set; } = -1;
		/// <summary>
		/// Have the datepicker appear automatically when the field receives focus ('focus'), appear only when a button is clicked ('button'), or appear when either event takes place ('both').
		/// </summary>
		public string ShowOn { get; set; } = "button";
		/// <summary>
		/// When true a column is added to show the week of the year. The calculateWeek option determines how the week of the year is calculated. You may also want to change the firstDay option.
		/// </summary>
		public bool ShowWeek { get; set; } = false;

		public string TimeCssClass { get; set; } = "timepicker";

		public string RelatedPickerControl { get; set; }
		public RelatedPickerRelation RelatedPickerRelation { get; set; } = RelatedPickerRelation.Start;
		public bool KeyboardNav { get; set; }
		//private int stepMonths = 1;
		///// <summary>
		///// Set how many months to move when clicking the Previous/Next links.
		///// </summary>
		//public int StepMonths
		//{
		//    get { return stepMonths; }
		//    set { stepMonths = value; }
		//}

		//private string weekHeader = "Wk";
		///// <summary>
		///// The text to display for the week of the year column heading. This attribute is one of the regionalisation attributes. Use showWeek to display this column.
		///// </summary>
		//public string WeekHeader
		//{
		//    get { return weekHeader; }
		//    set { weekHeader = value; }
		//}

		//private string yearSuffix = string.Empty;
		///// <summary>
		///// Additional text to display after the year in the month headers. This attribute is one of the regionalisation attributes.
		///// </summary>
		//public string YearSuffix
		//{
		//    get { return yearSuffix; }
		//    set { yearSuffix = value; }
		//}


	}
}
