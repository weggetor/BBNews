<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="EditCategories.ascx.cs" Inherits="Bitboxx.DNNModules.BBNews.EditCategories" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<div class="dnnForm dnnBBNewsEdit dnnClear">
	<div class="dnnFormMessage dnnFormInfo">
		<asp:Label ID="lblIntro" runat="server" ResourceKey="Intro" />
	</div>
</div>
<div class="dnnForm dnnBBNewsEdit dnnClear">
	<asp:Panel runat="server" ID="pnlSearch" Visible="true">
		<div class="dnnFormItem">
			<asp:DataGrid ID="grdCategories" runat="server" AllowSorting="True" DataKeyNames="CategoryId"
				AutoGenerateColumns="False" 
				BorderStyle="None" 
				GridLines="None"
				Width="98%" 
				CssClass="dnnGrid" onitemcommand="grdCategories_ItemCommand">
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
							<asp:ImageButton ID="cmdEdit" runat="server" CommandName="Edit" CommandArgument='<%# DataBinder.Eval(Container.DataItem,"CategoryId") %>' IconKey="Edit"/>
						</itemtemplate>
					</asp:templatecolumn>
					<asp:templatecolumn>
						<itemtemplate>
							<asp:ImageButton runat="server" CommandName="Delete" CommandArgument='<%# DataBinder.Eval(Container.DataItem,"CategoryId") %>' IconKey="Delete"/>
						</itemtemplate>
					</asp:templatecolumn>
					<asp:BoundColumn  DataField="CategoryName" HeaderText="CategoryName" ItemStyle-Width="20%" />
					<asp:BoundColumn  DataField="CategoryDescription" HeaderText="CategoryDescription" ItemStyle-Width="78%" />
				</Columns>
			</asp:DataGrid>
		</div>
	</asp:Panel>
	<asp:Panel runat="server" ID="pnlEdit" Visible="False">
		<div class="dnnFormItem">
			<dnn:Label id="lblCategoryName" runat="server" controlname="txtCategoryName" suffix=":" />
			<asp:TextBox id="txtCategoryName" runat="server" Columns="30" CssClass="dnnFormInput" />
			<asp:HiddenField runat="server" ID="hidCategoryId"/>
		</div>
		<div class="dnnFormItem">
			<dnn:Label id="lblCategoryDescription" runat="server" controlname="txtCategoryDescription" suffix=":" />
			<asp:TextBox id="txtCategoryDescription" runat="server" Columns="30" Rows="6" CssClass="dnnFormInput" TextMode="MultiLine" />
			<asp:HiddenField runat="server" ID="HiddenField1"/>
		</div>
	</asp:Panel>
	<ul class="dnnActions dnnClear">
		<li><asp:LinkButton ID="cmdNew" runat="server" class="dnnPrimaryAction" ResourceKey="cmdNew" Visible="True" onclick="cmdNew_Click"/></li>
		<li><asp:LinkButton ID="cmdSave" runat="server" CssClass="dnnPrimaryAction" ResourceKey="cmdSave" Visible="False" onclick="cmdSave_Click" /></li>
		<li><asp:LinkButton ID="cmdCancel" runat="server" CssClass="dnnSecondaryAction" ResourceKey="cmdCancel" Visible="False" onclick="cmdCancel_Click"/></li>
	</ul>
</div>

