<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="EditScheduler.ascx.cs" Inherits="Bitboxx.DNNModules.BBNews.EditScheduler" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<div class="dnnForm dnnBBNewsEdit dnnClear">
	<div class="dnnFormMessage dnnFormInfo">
		<asp:Label ID="lblIntro" runat="server" ResourceKey="Intro" />
	</div>
</div>
<div class="dnnForm dnnBBNewsEdit dnnClear" id="dnnEditSchedule">
    <fieldset>
        <div class="dnnFormItem">
            <dnn:Label ID="plFriendlyName" runat="server" ControlName="txtFriendlyName" />
            <asp:TextBox ID="txtFriendlyName" runat="server" />
        </div>
        <div class="dnnFormItem">
            <dnn:Label ID="plEnabled" runat="server" ControlName="chkEnabled" />
            <asp:CheckBox ID="chkEnabled" runat="server" />
        </div>
        <div class="dnnFormItem">
            <dnn:Label ID="plTimeLapse" runat="server" ControlName="txtTimeLapse" />
            <div>
				<asp:TextBox ID="txtTimeLapse" runat="server" MaxLength="10" style="width:50px"/>
				<asp:DropDownList ID="ddlTimeLapseMeasurement" runat="server">
					<asp:ListItem resourcekey="Seconds" Value="s" />
					<asp:ListItem resourcekey="Minutes" Value="m" />
					<asp:ListItem resourcekey="Hours" Value="h" />
					<asp:ListItem resourcekey="Days" Value="d" />
				</asp:DropDownList>
			</div>
        </div>
        <div class="dnnFormItem">
            <dnn:Label ID="plRetryTimeLapse" runat="server" ControlName="txtRetryTimeLapse" />
            <div>
				<asp:TextBox ID="txtRetryTimeLapse" runat="server" MaxLength="10" style="width:50px"/>
				<asp:DropDownList ID="ddlRetryTimeLapseMeasurement" runat="server">
				   <asp:ListItem resourcekey="Seconds" Value="s" />
					<asp:ListItem resourcekey="Minutes" Value="m" />
					<asp:ListItem resourcekey="Hours" Value="h" />
					<asp:ListItem resourcekey="Days" Value="d" />
				</asp:DropDownList>
			</div>
        </div>
        <div class="dnnFormItem">
            <dnn:Label ID="plRetainHistoryNum" runat="server" ControlName="ddlRetainHistoryNum" />
            <asp:DropDownList ID="ddlRetainHistoryNum" runat="server">
                <asp:ListItem Value="0" resourcekey="None" />
                <asp:ListItem Value="1">1</asp:ListItem>
                <asp:ListItem Value="5">5</asp:ListItem>
                <asp:ListItem Value="10">10</asp:ListItem>
                <asp:ListItem Value="25">25</asp:ListItem>
                <asp:ListItem Value="50">50</asp:ListItem>
                <asp:ListItem Value="100">100</asp:ListItem>
                <asp:ListItem Value="250">250</asp:ListItem>
                <asp:ListItem Value="500">500</asp:ListItem>
                <asp:ListItem Value="-1" resourcekey="All" />
            </asp:DropDownList>
        </div>
    </fieldset>
    <ul class="dnnActions dnnClear">
    	<li><asp:LinkButton id="cmdUpdate" runat="server" CssClass="dnnPrimaryAction" resourcekey="cmdUpdate" OnClick="OnUpdateClick"/></li>
        <li><asp:LinkButton id="cmdRun" runat="server" CssClass="dnnSecondaryAction" resourcekey="cmdRun" Causesvalidation="False" OnClick="OnRunClick" /></li>
        <li><asp:HyperLink id="cmdCancel" runat="server" CssClass="dnnSecondaryAction" resourcekey="cmdCancel" /></li>
    </ul>
</div>
