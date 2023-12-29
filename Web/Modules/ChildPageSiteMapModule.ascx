﻿<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ChildPageSiteMapModule.ascx.cs" Inherits="mojoPortal.Web.UI.ChildPageSiteMapModule" %>
<portal:OuterWrapperPanel ID="pnlOuterWrap" runat="server" SkinID="childpagesitemapmodule">
	<portal:InnerWrapperPanel ID="pnlInnerWrap" runat="server" CssClass="panelwrapper childpagesitemapmodule">
		<portal:ModuleTitleControl id="Title1" runat="server" EnableViewState="false" />
		<portal:OuterBodyPanel ID="pnlOuterBody" runat="server">
			<portal:InnerBodyPanel ID="pnlInnerBody" runat="server" CssClass="modulecontent">
				<portal:ChildPageMenu id="ChildPageMenu1" runat="server" CssClass="txtnormal" ForceDisplay="true" />
			</portal:InnerBodyPanel>
			<portal:EmptyPanel id="divFooter" runat="server" CssClass="modulefooter" SkinID="modulefooter" />
		</portal:OuterBodyPanel>
	</portal:InnerWrapperPanel>
</portal:OuterWrapperPanel>
