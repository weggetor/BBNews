<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="TemplateControl.ascx.cs" Inherits="Bitboxx.DNNModules.BBNews.Controls.TemplateControl" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<asp:Panel runat="server" ID="pnlView">
   <div>
   		<asp:DropDownList runat="server" ID="ddlTemplate" onselectedindexchanged="ddlTemplate_SelectedIndexChanged"  AutoPostBack="True" />
   </div>
   <div><asp:Image ID="imgThumb" runat="server" ImageAlign="Top"/></div>
   <div style="float:left;padding-right:30px;"><asp:LinkButton ID="cmdNew" runat="server" resourcekey="cmdNew" OnClick="cmdNew_Click"/></div>
   <div style="float:left"><asp:LinkButton ID="cmdEdit" runat="server" resourcekey="cmdEdit" OnClick="cmdEdit_Click"/></div>
</asp:Panel>
<asp:Panel runat="server" ID="pnlEdit" Visible="False">
	<asp:Panel runat="server" ID="pnlMode" CssClass="dnnFormInput">
		<asp:Panel runat="server" ID="pnlNewTemplate" CssClass="dnnFormItem">
			<dnn:Label runat="server" id="lblName" controlname="txtName" suffix=":" />
			<asp:TextBox ID="txtName" runat="server" CssClass="dnnFormInput" />
			<asp:RequiredFieldValidator ID="valNameRequired" ControlToValidate="txtName" runat="server" Resourcekey="valNameRequired.Error"/>
		</asp:Panel>
		<asp:Panel runat="server" ID="pnlEditTemplate" CssClass="dnnFormItem">
			<div class="dnnFormItem">
				<dnn:Label runat="server" id="lblMode" controlname="rblMode" suffix=":" />
				<asp:RadioButtonList ID="rblMode" runat="server" CssClass="dnnFormRadioButtons" RepeatDirection="Horizontal" 
					AutoPostBack="True" OnSelectedIndexChanged="rblMode_SelectedIndexChanged" style="min-width: 200px;">
					<asp:ListItem resourcekey="rblMode0" Value="0" />
					<asp:ListItem resourcekey="rblMode1" Value="1" />
				</asp:RadioButtonList>
			</div>
		
			<div class="dnnFormItem">
				<dnn:Label runat="server" id="lblLanguage" controlname="ddlLanguage" suffix=":" />
				<asp:DropDownList runat="server" ID="ddlLanguage" AutoPostBack="True" OnSelectedIndexChanged="ddlLanguage_SelectedIndexChanged"/>
			</div>
			<div class="dnnFormItem">
				<dnn:Label runat="server" id="lblFileCap" controlname="lblFile" suffix=":" />
				<asp:Label runat="server" ID="lblFile" />
			</div>
		</asp:Panel>
		<div class="dnnFormItem">
			<dnn:Label runat="server" id="lblDummy" />
			<asp:textbox id="txtTemplate" CssClass="dnnFormInput" width="390" columns="30" textmode="MultiLine" rows="10" maxlength="3000" runat="server" />
		</div>
		<div class="dnnFormItem">
			<div style="float:left;padding-right:30px;"><asp:LinkButton ID="cmdSave" runat="server" resourcekey="cmdSave" OnClick="cmdSave_Click" CausesValidation="True"/></div>
			<div style="float:left"><asp:LinkButton ID="cmdCancelEdit" runat="server" resourcekey="cmdCancelEdit" OnClick="cmdCancelEdit_Click" CausesValidation="False"/></div>
		</div>
		<div class="dnnFormMessage dnnFormInfo dnnClear">
			<asp:Label runat="server" ID="ltrHelp" />
		</div>
	</asp:Panel>
	
</asp:Panel>
