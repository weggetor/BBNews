<%@ Control Language="C#" AutoEventWireup="false" Inherits="Bitboxx.DNNModules.BBNews.Settings" Codebehind="Settings.ascx.cs" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<%@ Register TagPrefix="bb" TagName="TabSelectControl" Src="Controls/TabSelectControl.ascx" %> 
<%@ Register TagPrefix="bb" TagName="TemplateControl" Src="Controls/TemplateControl.ascx" %> 
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.UI.WebControls" Assembly="DotNetNuke.Web" %>
 
<div class="dnnForm dnnBBNewsSettings dnnClear">
    <div class="dnnFormItem">
		<dnn:label id="lblView" runat="server" controlname="rblView" suffix=":" />
		<asp:RadioButtonList ID="rblView" runat="server"  CssClass="dnnFormRadioButtons" RepeatDirection="Horizontal" onselectedindexchanged="rblView_SelectedIndexChanged" AutoPostBack="True">
			<asp:ListItem resourcekey="rblView0" Value="0" />
			<asp:ListItem resourcekey="rblView1" Value="1" />
			<asp:ListItem resourcekey="rblView2" Value="2" />
		</asp:RadioButtonList>
	</div>
	<asp:Panel ID="pnlMulti" runat="server">
		<div class="dnnFormItem">
			<dnn:label id="lblCategory" runat="server" controlname="cboCategory" suffix=":" />
			<asp:DropDownList ID="cboCategory" runat="server" CssClass="dnnFormInput"/>
		</div>
		<div class="dnnFormItem">
			<dnn:label id="lblTopN" runat="server" controlname="txtTopN" suffix=":" />
			<asp:textbox id="txtTopN" runat="server" Columns="3" CssClass="dnnFormInput dnnNumberInput" />
			<asp:RangeValidator	ID="RangeValidatorTopN" CssClass="dnnFormMessage dnnFormError" runat="server" Type="Integer" 
				ErrorMessage="0..99" MinimumValue="0" MaximumValue="99" ControlToValidate="txtTopN" />
		</div>
		<div class="dnnFormItem">
			<dnn:label id="lblStartDate" runat="server" controlname="dtpStartDate" suffix=":" />
			<dnn:DnnDatePicker ID="dtpStartDate" runat="server" CssClass="dnnFormInput" />
		</div>
		<div class="dnnFormItem">
			<dnn:label id="lblEndDate" runat="server" controlname="dtpEndDate" suffix=":" />
			<dnn:DnnDatePicker ID="dtpEndDate" runat="server" CssClass="dnnFormInput"/>
		</div>
		<div class="dnnFormItem">
			<dnn:label id="lblShowRss" runat="server" controlname="rblRss" suffix=":" />
			<asp:RadioButtonList ID="rblRss" runat="server" CssClass="dnnFormRadioButtons" RepeatDirection="Horizontal">
				<asp:ListItem resourcekey="rblRss0" Value="0" />
				<asp:ListItem resourcekey="rblRss1" Value="1" />
				<asp:ListItem resourcekey="rblRss2" Value="2" />
			</asp:RadioButtonList>
		</div>
	</asp:Panel>
	<asp:Panel ID="pnlShowTitle" runat="server">
		<div class="dnnFormItem">
			<dnn:label id="lblShowTitle" runat="server" controlname="chkShowTitle" suffix=":" />
			<asp:CheckBox ID="chkShowTitle" CssClass="dnnFormInput" runat="server" />
		</div>
	</asp:Panel>
	<asp:Panel ID="pnlSelectPage" runat="server">
		<div class="dnnFormItem">
			<dnn:label id="lblNewsPage" runat="server" controlname="urlSelectNewsPage" suffix=":" />
			<bb:TabSelectControl ID="urlSelectNewsPage" runat="server" Width="200" CssClass="dnnFormInput"/>
		</div>
	</asp:Panel>
	<asp:Panel ID="pnlNewsRows" runat="server">
		<div class="dnnFormItem">
			<dnn:label id="lblNewsInRow" runat="server" controlname="txtNewsInRow" suffix=":" />
			<asp:textbox id="txtNewsInRow" CssClass="dnnFormInput dnnNumberInput" columns="2" runat="server" />
			<asp:RangeValidator	ID="RangeValidatorNewsInRow"  CssClass="dnnFormMessage dnnFormError" runat="server"  Type="Integer" 
				ErrorMessage="1..10" MinimumValue="1" MaximumValue="10" ControlToValidate="txtNewsInRow" />&nbsp;
		</div>
		<div class="dnnFormItem">
			<dnn:label id="lblRowsPerPage" runat="server" controlname="txtRowsPerPage" suffix=":" />
			<asp:textbox id="txtRowsPerPage" CssClass="dnnFormInput dnnNumberInput" columns="2" runat="server" />
			<asp:RangeValidator	ID="RangeValidatorRowsPerPage" CssClass="dnnFormMessage dnnFormError" runat="server" Type="Integer" 
				ErrorMessage="1..99" MinimumValue="1" MaximumValue="99" ControlToValidate="txtRowsPerPage" />
		</div>
	</asp:Panel>
	<div class="dnnFormItem">
		<dnn:label id="lblSelectTemplate" runat="server" controlname="selTemplate" suffix=":" />
		<bb:TemplateControl runat="server" ID="selTemplate" Key="News" CssClass="dnnFormInput" Width="250"/>
	</div>
	<asp:Panel ID="pnlMarquee" runat="server">
		<div class="dnnFormItem">
			<dnn:label id="lblMarqueeDirection" runat="server" controlname="cboMarqueeDirection" suffix=":" />
			<asp:DropDownList ID="cboMarqueeDirection" CssClass="dnnFormInput" runat="server">
				<asp:ListItem resourcekey="cboMarqueeDirection0" Value="0" />
				<asp:ListItem resourcekey="cboMarqueeDirection1" Value="1" />
				<asp:ListItem resourcekey="cboMarqueeDirection2" Value="2" />
				<asp:ListItem resourcekey="cboMarqueeDirection3" Value="3" />
			</asp:DropDownList>
		</div>
		<div class="dnnFormItem">
			<dnn:label id="lblMarqueeScrollAmount" runat="server" controlname="txtMarqueeScrollAmount" suffix=":" />
			<asp:textbox ID="txtMarqueeScrollAmount" CssClass="dnnFormInput dnnNumberInput" Columns="4" runat="server" />
			<asp:RangeValidator	ID="RangeValidatorMarqueeScrollAmount" CssClass="dnnFormMessage dnnFormError" runat="server" Type="Integer" 
				ErrorMessage="1..100" MinimumValue="1" MaximumValue="100" ControlToValidate="txtMarqueeScrollAmount" />
		</div>
		<div class="dnnFormItem">
			<dnn:label id="lblMarqueeScrollDelay" runat="server" controlname="txtMarqueeScrollDelay" suffix=":" />
			<asp:textbox ID="txtMarqueeScrollDelay" CssClass="dnnFormInput dnnNumberInput" Columns="4" runat="server" />
			<asp:RangeValidator	ID="RangeValidatorMarqueeScrollDelay" CssClass="dnnFormMessage dnnFormError" runat="server" Type="Integer" 
				ErrorMessage="1..10000" MinimumValue="1" MaximumValue="10000" ControlToValidate="txtMarqueeScrollDelay" />
		</div>
		<div class="dnnFormItem">
			<dnn:label id="lblMarqueeAlternate" runat="server" controlname="chkMarqueeAlternate" suffix=":" />
			<asp:checkbox ID="chkMarqueeAlternate" CssClass="dnnFormInput" runat="server" />
		</div>
	</asp:Panel>
</div>

