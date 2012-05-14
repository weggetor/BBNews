<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="EditNews.ascx.cs" Inherits="Bitboxx.DNNModules.BBNews.EditNews" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<%@ Register TagPrefix="dnn" TagName="TextEditor" Src="~/controls/TextEditor.ascx" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.UI.WebControls" Assembly="DotNetNuke.Web" %>

	<div class="dnnFormMessage dnnFormInfo dnnClear">
		<asp:Label ID="lblIntro" runat="server" ResourceKey="Intro" />
	</div>

<div class="dnnForm dnnBBNewsEdit dnnClear">
	<asp:Panel runat="server" ID="pnlSearch" Visible="True">
		<div class="dnnFormItem">
			<dnn:Label runat="server" ID="lblSearchText" />
			<asp:TextBox runat="server" ID="txtSearch"></asp:TextBox>
		</div>
		<div class="dnnFormItem">
			<dnn:Label runat="server" ID="lblSearchCategory" />
			<asp:DropDownList runat="server" ID="ddlSearchCategory" DataTextField="CategoryName" DataValueField="CategoryId" />
		</div>
		<div class="dnnFormItem">
			<dnn:Label runat="server" ID="lblSearchDateStart"/>
			<dnn:DnnDatePicker runat="server" ID="dpDateStart"/>
		</div>
		<div class="dnnFormItem">
			<dnn:Label runat="server" ID="lblSearchDateEnd"/>
			<dnn:DnnDatePicker runat="server" ID="dpDateEnd"/>
		</div>
		<div class="dnnFormItem">
			<dnn:Label runat="server" ID="lblSearch"/>
			<asp:LinkButton runat="server" ID="cmdSearch" CssClass="dnnPrimaryAction" OnClick="cmdSearch_Click" Text="Search"/>
		</div>
		<div class="dnnFormItem">
			<asp:DataGrid ID="grdNews" runat="server" 
				AllowSorting="True" 
				AllowPaging="True"
				PageSize="15"
				AllowCustomPaging="True"
				DataKeyNames="NewsId"
				AutoGenerateColumns="False" 
				BorderStyle="None" 
				GridLines="None"
				Width="98%" 
				CssClass="dnnGrid" 
				onitemcommand="grdNews_ItemCommand"
				OnItemCreated="grdNews_ItemCreated"
				OnPageIndexChanged="grdNews_PageIndexChanged">
				<ItemStyle CssClass="dnnGridItem" horizontalalign="Left" />
				<AlternatingItemStyle cssclass="dnnGridAltItem" />
				<Edititemstyle cssclass="dnnFormInput" />
				<SelectedItemStyle cssclass="dnnFormError" />
				<FooterStyle cssclass="dnnGridFooter" />
				<PagerStyle cssclass="dnnGridPager" Mode="NumericPages" />
				<HeaderStyle CssClass="dnnGridHeader" />
				<Columns>
					<asp:templatecolumn>
						<itemtemplate>
							<asp:ImageButton ID="cmdEdit" runat="server" CommandName="Edit" CommandArgument='<%# DataBinder.Eval(Container.DataItem,"NewsId") %>' IconKey="Edit"/>
						</itemtemplate>
					</asp:templatecolumn>
					<asp:templatecolumn>
						<itemtemplate>
							<asp:ImageButton ID="cmdDelete" runat="server" CommandName="Delete" CommandArgument='<%# DataBinder.Eval(Container.DataItem,"NewsId") %>' IconKey="Delete"/>
						</itemtemplate>
					</asp:templatecolumn>
					<asp:TemplateColumn  HeaderText="Title">
						<itemtemplate>
							<asp:Label runat="server"  ID="lblTitle" Text='<%# GetNewsTitle(DataBinder.Eval(Container.DataItem,"Title"),65) %>' />
						</itemtemplate>
					</asp:TemplateColumn>
					<asp:TemplateColumn  HeaderText="PubDate">
						<itemtemplate>
							<asp:Label runat="server"  ID="lblPubDate" Text='<%# DataBinder.Eval(Container.DataItem,"Pubdate") %>' />
						</itemtemplate>
					</asp:TemplateColumn>
					<asp:TemplateColumn  HeaderText="FeedName">
						<itemtemplate>
							<asp:Label runat="server"  ID="lblFeed" Text='<%# GetFeedName(DataBinder.Eval(Container.DataItem,"FeedId")) %>' />
						</itemtemplate>
					</asp:TemplateColumn>
					<asp:templatecolumn HeaderText="Hide">
						<itemtemplate>
							<dnn:DnnImage Runat="server" ID="Image1" IconKey="Checked" Visible='<%# DataBinder.Eval(Container.DataItem,"Hide") %>' />
							<dnn:DnnImage Runat="server" ID="Image2" IconKey="Unchecked" Visible='<%# !(bool)DataBinder.Eval(Container.DataItem,"Hide") %>' />
						</ItemTemplate>
					</asp:templatecolumn>
				</Columns>
			</asp:DataGrid>
		</div>
	</asp:Panel>
	<asp:Panel runat="server" ID="pnlEdit" Visible="False">
		<fieldset>
			<div class="dnnFormItem">
				<dnn:Label ID="lblHide" runat="server"  controlname="chkHide" suffix=":" />
				<asp:CheckBox runat="server" ID="chkHide" CssClass="dnnFormCheckbox"/>
			</div>
			<div class="dnnFormItem">
				<dnn:Label ID="lblInternal" runat="server" controlname="chkInternal" suffix=":" />
				<asp:CheckBox runat="server" ID="chkInternal" CssClass="dnnFormCheckbox"/>
			</div>
			<div class="dnnFormItem">
				<dnn:Label ID="lblFeed" runat="server"  controlname="ddlFeed" suffix=":" />
				<asp:DropDownList runat="server" ID="ddlFeed" CssClass="dnnFormInput dnnFormRequired" />
			</div>
			<div class="dnnFormItem">
				<dnn:Label id="lblTitle" runat="server" controlname="txtTitle" suffix=":" />
				<asp:TextBox id="txtTitle" runat="server" CssClass="dnnFormInput dnnFormRequired"  MaxLength="250"  TextMode="MultiLine" Width="550" Height="40" />
				<asp:RequiredFieldValidator ID="valTitle" CssClass="dnnFormMessage dnnFormError"  ControlToValidate="txtTitle" ResourceKey="valTitle.Error" runat="server"/>
				<asp:HiddenField runat="server" ID="hidNewsId"/>
				<asp:HiddenField runat="server" ID="hidGuid"/>
			</div>
			<div class="dnnFormItem">
				<dnn:Label id="lblPubDate" runat="server"  controlname="dpPubDate" suffix=":" />
				<dnn:DnnDatePicker runat="server" ID="dpPubDate" CssClass="dnnFormInput" />
				<dnn:DnnTimePicker runat="server" ID="tpPubDate" CssClass="dnnFormItem"  DateInput="dpPubDate"/>
			</div>
			<div class="dnnFormItem">
				<dnn:Label ID="lblAuthor" runat="server" controlname="txtAuthor" suffix=":" />
				<fieldset>
					<div class="dnnFormItem">
						<dnn:Label runat="server" id="lblAuthorName"  controlname="txtAuthorName" suffix=":" />
						<asp:TextBox runat="server" id="txtAuthorName" CssClass="dnnFormInput" Columns="40" />
					</div>
					<div class="dnnFormItem">
						<dnn:Label runat="server" id="lblAuthorUrl"  controlname="txtAuthorUrl" suffix=":" />
						<asp:TextBox runat="server" id="txtAuthorUrl" CssClass="dnnFormInput" Columns="40" />
					</div>
					<div class="dnnFormItem">
						<dnn:Label runat="server" id="lblAuthorEmail"  controlname="txtAuthorEmail" suffix=":" />
						<asp:TextBox runat="server" id="txtAuthorEmail" CssClass="dnnFormInput" Columns="40" />
					</div>
					<div class="dnnFormItem">
						<dnn:Label runat="server" id="lblAuthorNick"  controlname="txtAuthorNick" suffix=":" />
						<asp:TextBox runat="server" id="txtAuthorNick" CssClass="dnnFormInput" Columns="40" />
					</div>
				</fieldset>
			</div>
			<div class="dnnFormItem">
				<dnn:Label ID="lblLink" runat="server"  controlname="txtLink" suffix=":" />
				<asp:TextBox runat="server" id="txtLink" CssClass="dnnFormInput" Columns="80" MaxLength="500" />
			</div>
			<div class="dnnFormItem">
				<dnn:Label ID="lblImage" runat="server"  controlname="txtImage" suffix=":" />
				<asp:TextBox runat="server" id="txtImage" CssClass="dnnFormInput" Columns="80" MaxLength="500" />
			</div>
			<div class="dnnFormItem">
				<dnn:Label ID="lblSummary" runat="server"  controlname="txtSummary" suffix=":" />
				<div class="dnnRight">
					<dnn:texteditor ID="txtSummary" runat="server" TextRenderMode="Raw" HtmlEncode="False" defaultmode="Rich"  choosemode="False" chooserender="False" Width="550" Height="350"/>
				</div>
			</div>
			<div class="dnnFormItem">
				<dnn:Label ID="lblNews" runat="server"  controlname="txtNews" suffix=":" />
				<div class="dnnRight">
					<dnn:texteditor ID="txtNews" runat="server" TextRenderMode="Raw" HtmlEncode="False" defaultmode="Rich"  choosemode="False" chooserender="False" Width="550" Height="350"/>
				</div>
			</div>
		</fieldset>
	</asp:Panel>
	<ul class="dnnActions dnnClear">
		<li><asp:LinkButton ID="cmdNew" runat="server" CssClass="dnnPrimaryAction" ResourceKey="cmdNew" Visible="True" onclick="cmdNew_Click" CausesValidation="False"/></li>
		<li><asp:LinkButton ID="cmdSave" runat="server" CssClass="dnnPrimaryAction" ResourceKey="cmdSave" Visible="False" onclick="cmdSave_Click" /></li>
		<li><asp:LinkButton ID="cmdCancel" runat="server" CssClass="dnnSecondaryAction" ResourceKey="cmdCancel" Visible="False" onclick="cmdCancel_Click" CausesValidation="False"/></li>
	</ul>

</div>