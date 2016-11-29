<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="EditFeeds.ascx.cs" Inherits="Bitboxx.DNNModules.BBNews.EditFeeds" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.UI.WebControls" Assembly="DotNetNuke.Web" %>
<%@ Register TagPrefix="dnnd" Namespace="DotNetNuke.Web.UI.WebControls" Assembly="DotNetNuke.Web.Deprecated" %>
<div class="dnnForm dnnBBNewsEdit dnnClear">
	<div class="dnnFormMessage dnnFormInfo dnnClear">
		<asp:Label ID="lblIntro" runat="server" ResourceKey="Intro" />
	</div>
	<asp:Panel runat="server" ID="pnlSearch" Visible="true">
		<div class="dnnFormItem">
			<asp:DataGrid ID="grdFeeds" runat="server" AllowSorting="True" DataKeyNames="FeedId"
				AutoGenerateColumns="False" 
				BorderStyle="None" 
				GridLines="None"
				Width="98%" 
				CssClass="dnnGrid" 
				onitemcommand="grdFeeds_ItemCommand">
				<ItemStyle CssClass="dnnGridItem" horizontalalign="Left" />
				<AlternatingItemStyle cssclass="dnnGridAltItem" />
				<Edititemstyle cssclass="dnnFormInput" />
				<SelectedItemStyle cssclass="dnnFormError" />
				<FooterStyle cssclass="dnnGridFooter" />
				<PagerStyle cssclass="dnnGridPager" />
				<HeaderStyle CssClass="dnnGridHeader" />
				<Columns>
					<asp:templatecolumn>
						<itemtemplate>
							<asp:ImageButton ID="cmdEdit" runat="server" CommandName="Edit" CommandArgument='<%# DataBinder.Eval(Container.DataItem,"FeedId") %>' IconKey="Edit"/>
						</itemtemplate>
					</asp:templatecolumn>
					<asp:templatecolumn>
						<itemtemplate>
							<asp:ImageButton ID="ImageButton1" runat="server" CommandName="Delete" CommandArgument='<%# DataBinder.Eval(Container.DataItem,"FeedId") %>' IconKey="Delete"/>
						</itemtemplate>
					</asp:templatecolumn>
                    <asp:BoundColumn  DataField="FeedID" HeaderText="FeedID" />
					<asp:BoundColumn  DataField="FeedName" HeaderText="FeedName" />
					<asp:TemplateColumn  HeaderText="FeedType">
						<itemtemplate>
							<asp:Label runat="server"  ID="lblFeedTypeName" Text='<%# GetfeedType(DataBinder.Eval(Container.DataItem,"FeedType")) %>' />
						</itemtemplate>
					</asp:TemplateColumn>
					<asp:BoundColumn  DataField="LastRetrieve" HeaderText="LastRetrieve" DataFormatString=""/>
					<asp:templatecolumn HeaderText="Active">
						<itemtemplate>
							<dnn:DnnImage Runat="server" ID="Image1" IconKey="Checked" Visible='<%# DataBinder.Eval(Container.DataItem,"Enabled") %>' />
							<dnn:DnnImage Runat="server" ID="Image2" IconKey="Unchecked" Visible='<%# !(bool)DataBinder.Eval(Container.DataItem,"Enabled") %>' />
						</ItemTemplate>
					</asp:templatecolumn>
				</Columns>
			</asp:DataGrid>
		</div>
	</asp:Panel>
	<hr/>
	<asp:Panel runat="server" ID="pnlEdit" Visible="False">
		<fieldset>
			<div class="dnnFormItem">
				<dnn:Label id="lblFeedName" runat="server"  controlname="txtFeedName" suffix=":" />
				<asp:TextBox id="txtFeedName" runat="server" CssClass="dnnFormInput dnnFormRequired" />
				<asp:RequiredFieldValidator ID="valFeedname" CssClass="dnnFormMessage dnnFormError"  ControlToValidate="txtFeedName" ResourceKey="valFeedName.Error" runat="server"/>
				<asp:HiddenField runat="server" ID="hidFeedId"/>
			</div>
			<div class="dnnFormItem">
				<dnn:Label ID="lblFeedType" runat="server" controlname="cboFeedType" suffix=":" />
				<asp:DropDownList runat="server" ID="cboFeedType" CssClass="dnnFormInput dnnFormRequired" AutoPostBack="True" OnSelectedIndexChanged="cboFeedType_SelectedIndexChanged">
					<asp:ListItem ResourceKey="FeedTypeNone.Text" Value="0"/>
					<asp:ListItem ResourceKey="FeedTypeTwitterSearch.Text" Value="1"/>
					<asp:ListItem ResourceKey="FeedTypeTwitterTimeline.Text" Value="3"/>
					<asp:ListItem ResourceKey="FeedTypeRss.Text" Value="2"/>
				</asp:DropDownList>
			</div>
			<asp:Panel runat="server" ID="pnlFeedUrl" Visible="False">
				<div class="dnnFormItem">
					<dnn:Label id="lblFeedUrl" runat="server" controlname="txtFeedUrl" suffix=":" />
					<asp:TextBox id="txtFeedUrl" runat="server" CssClass="dnnFormInput dnnFormRequired" />
					<asp:RequiredFieldValidator ID="valFeedUrl" CssClass="dnnFormMessage dnnFormError"  ControlToValidate="txtFeedUrl" ResourceKey="valFeedUrl.Error" runat="server"/>
				</div>
			</asp:Panel>
			<div class="dnnFormItem">
				<dnn:Label ID="lblRetrieveInterval" runat="server"  controlname="txtRetrieveInterval" suffix=":" />
				<asp:TextBox ID="txtRetrieveInterval" runat="server" CssClass="dnnFormInput"  />
				<asp:RangeValidator ID="valRetrieveIntervalValid" runat="server" CssClass="dnnFormMessage dnnFormError" MinimumValue="0" MaximumValue="86400" ControlToValidate="txtRetrieveInterval" ResourceKey="valRetrieveIntervalValid.Error" />
			</div>
			<div class="dnnFormItem">
				<dnn:Label ID="lblTryInterval" runat="server"  controlname="txtTryInterval" suffix=":" />
				<asp:TextBox ID="txtTryInterval" runat="server" CssClass="dnnFormInput" />
				<asp:RangeValidator ID="valTryIntervalValid" runat="server" CssClass="dnnFormMessage dnnFormError" MinimumValue="0" MaximumValue="86400" ControlToValidate="txtTryInterval" ResourceKey="valTryIntervalValid.Error" />
			</div>
			<div class="dnnFormItem">
				<dnn:Label ID="lblReorgInterval" runat="server"  controlname="txtReorgInterval" suffix=":" />
				<asp:TextBox ID="txtReorgInterval" runat="server" CssClass="dnnFormInput"  />
				<asp:RangeValidator ID="valReorgIntervalValid" runat="server" CssClass="dnnFormMessage dnnFormError" MinimumValue="0" MaximumValue="365" ControlToValidate="txtReorgInterval" ResourceKey="valReorgIntervalValid.Error" />
			</div>
			<div class="dnnFormItem">
				<dnn:Label ID="lblEnabled" runat="server"  controlname="chkEnabled" suffix=":" />
				<asp:CheckBox ID="chkEnabled" runat="server" CssClass="dnnFormInput" />
			</div>
			<div class="dnnFormItem">
				<dnn:Label ID="lblLastRetrieve" runat="server"  controlname="txtLastRetrieve" suffix=":" />
				<asp:TextBox ID="txtLastRetrieve" runat="server" CssClass="dnnFormInput" Enabled="False" />
			</div>
			<div class="dnnFormItem">
				<dnn:Label ID="lblLastTry" runat="server" controlname="txtLastTry" suffix=":" />
				<asp:TextBox ID="txtLastTry" runat="server" CssClass="dnnFormInput" Enabled="False" />
			</div>
			<asp:Panel runat="server" ID="pnlCredentials" Visible="False">
				<div class="dnnFormItem">
					<dnn:Label ID="lblUserName" runat="server"  controlname="txtUserName" suffix=":" />
					<asp:TextBox ID="txtUserName" runat="server" CssClass="dnnFormInput"  />
					<asp:RequiredFieldValidator ID="valUserName" CssClass="dnnFormMessage dnnFormError"  ControlToValidate="txtUserName" ResourceKey="valUserName.Error" runat="server" />
				</div>
				<div class="dnnFormItem">
					<dnn:Label ID="lblPassword" runat="server" controlname="txtPassword" suffix=":" />
					<asp:TextBox ID="txtPassword" runat="server" CssClass="dnnFormInput"  />
					<asp:RequiredFieldValidator ID="valPassword" CssClass="dnnFormMessage dnnFormError"  ControlToValidate="txtPassword" ResourceKey="valPassword.Error" runat="server" />
				</div>
			</asp:Panel>
		</fieldset>
	</asp:Panel>
	<ul class="dnnActions dnnClear">
		<li><asp:LinkButton ID="cmdNew" runat="server" class="dnnPrimaryAction" ResourceKey="cmdNew" Visible="True" onclick="cmdNew_Click" CausesValidation="False"/></li>
		<li><asp:LinkButton ID="cmdSave" runat="server" CssClass="dnnPrimaryAction" ResourceKey="cmdSave" Visible="False" onclick="cmdSave_Click" /></li>
		<li><asp:LinkButton ID="cmdCancel" runat="server" CssClass="dnnSecondaryAction" ResourceKey="cmdCancel" Visible="False" onclick="cmdCancel_Click" CausesValidation="False"/></li>
	</ul>
</div>