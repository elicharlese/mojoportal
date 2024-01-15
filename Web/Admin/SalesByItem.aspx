﻿<%@ Page Language="C#" AutoEventWireup="false" MasterPageFile="~/App_MasterPages/layout.Master" CodeBehind="SalesByItem.aspx.cs" Inherits="mojoPortal.Web.AdminUI.SalesByItemPage" %>

<asp:Content ContentPlaceHolderID="leftContent" ID="MPLeftPane" runat="server" />
<asp:Content ContentPlaceHolderID="mainContent" ID="MPContent" runat="server">
<portal:AdminCrumbContainer ID="pnlAdminCrumbs" runat="server" CssClass="breadcrumbs">
<asp:HyperLink ID="lnkAdminMenu" runat="server" NavigateUrl="~/Admin/AdminMenu.aspx" /><portal:AdminCrumbSeparator id="AdminCrumbSeparator1" runat="server" Text="&nbsp;&gt;" EnableViewState="false" />
<asp:HyperLink ID="lnkCommerceReports" runat="server" NavigateUrl="~/Admin/SalesSummary.aspx" /><portal:AdminCrumbSeparator id="AdminCrumbSeparator2" runat="server" Text="&nbsp;&gt;" EnableViewState="false" />
<asp:HyperLink ID="lnkModuleReport" runat="server" NavigateUrl="~/Admin/SalesSummary.aspx" /><portal:AdminCrumbSeparator id="AdminCrumbSeparator3" runat="server" Text="&nbsp;&gt;" EnableViewState="false" />
<asp:HyperLink ID="lnkThisPage" runat="server" CssClass="selectedcrumb" />
</portal:AdminCrumbContainer>
<portal:OuterWrapperPanel ID="pnlOuterWrap" runat="server">

<portal:InnerWrapperPanel ID="pnlInnerWrap" runat="server" CssClass="panelwrapper salesbyitem ">
<portal:HeadingControl ID="heading" runat="server" />
<portal:OuterBodyPanel ID="pnlOuterBody" runat="server">
<portal:InnerBodyPanel ID="pnlInnerBody" runat="server" CssClass="modulecontent">
<strong><asp:Literal ID="litAllTimeRevenue" runat="server" /></strong>
<div>
<div >
     <%--<ajaxToolkit:BarChart ID="bcSales" runat="server" Visible="false" EnableViewState="false" SkinID="SalesByItem" ChartWidth="820" ChartHeight="300"></ajaxToolkit:BarChart>--%>
 <zgw:zedgraphweb id="zgSales" runat="server" RenderMode="ImageTag"
    Width="720" Height="300"></zgw:zedgraphweb>
</div>
<div class="floatpanel">
<div>&nbsp;</div>
<div class="floatpanel">
<mp:mojoGridView ID="grdSales" runat="server"
     AllowPaging="false"
     AllowSorting="false"
	 AutoGenerateColumns="false">
     <Columns>
		<asp:TemplateField>
			<ItemTemplate>
                <%# Eval("Y") %>
            </ItemTemplate>
		</asp:TemplateField>
		<asp:TemplateField>
			<ItemTemplate>
                <%# Eval("M") %>
            </ItemTemplate>
		</asp:TemplateField>
		<asp:TemplateField>
			<ItemTemplate>
                <%# string.Format(currencyCulture, "{0:c}", Convert.ToDecimal(Eval("Sales"))) %>
            </ItemTemplate>
		</asp:TemplateField>
        <asp:TemplateField>
			<ItemTemplate>
                <%# Eval("Units") %>
            </ItemTemplate>
		</asp:TemplateField>
		
</Columns>
<EmptyDataTemplate>
            <p class="nodata"><asp:Literal id="litempty" runat="server" Text="<%$ Resources:Resource, GridViewNoData %>" /></p>
    </EmptyDataTemplate>
</mp:mojoGridView>
<div>&nbsp;</div>
</div>

</div>

</div>

</portal:InnerBodyPanel>
</portal:OuterBodyPanel>

</portal:InnerWrapperPanel> 
</portal:OuterWrapperPanel>
</asp:Content>
<asp:Content ContentPlaceHolderID="rightContent" ID="MPRightPane" runat="server" />
<asp:Content ContentPlaceHolderID="pageEditContent" ID="MPPageEdit" runat="server" />
