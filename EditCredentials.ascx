<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="EditCredentials.ascx.cs" Inherits="Bitboxx.DNNModules.BBNews.EditCredentials" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<div class="dnnForm dnnBBNewsEdit dnnClear">
	<div class="dnnFormMessage dnnFormInfo">
		<asp:Label ID="lblIntro" runat="server" ResourceKey="Intro" />
	</div>
</div>
<div class="dnnForm dnnBBNewsEdit dnnClear" id="dnnEditSchedule">
    <fieldset>
        <div class="dnnFormItem">
            <dnn:Label ID="lblTwitterConsumerKey" runat="server" ControlName="txtTwitterConsumerKey" />
            <asp:TextBox ID="txtTwitterConsumerKey" runat="server" />
        </div>
        <div class="dnnFormItem">
            <dnn:Label ID="lblTwitterConsumerSecret" runat="server" ControlName="txtTwitterConsumerSecret" />
            <asp:TextBox ID="txtTwitterConsumerSecret" runat="server" />
        </div>
        <div class="dnnFormItem">
            <dnn:Label ID="lblTwitterAccessToken" runat="server" ControlName="txtTwitterAccessToken" />
            <asp:TextBox ID="txtTwitterAccessToken" runat="server" />
        </div>
        <div class="dnnFormItem">
            <dnn:Label ID="lblTwitterAccessTokenSecret" runat="server" ControlName="txtTwitterAccessTokenSecret" />
            <asp:TextBox ID="txtTwitterAccessTokenSecret" runat="server" />
        </div>
    </fieldset>
    <ul class="dnnActions dnnClear">
    	<li><asp:LinkButton id="cmdUpdate" runat="server" CssClass="dnnPrimaryAction" resourcekey="cmdUpdate" OnClick="cmdUpdate_OnClick"/></li>
    </ul>
</div>
