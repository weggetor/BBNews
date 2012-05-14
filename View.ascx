<%@ Control Language="C#" Inherits="Bitboxx.DNNModules.BBNews.View"  AutoEventWireup="true" CodeBehind="View.ascx.cs" %>
<asp:MultiView ID="MultiView1" runat="server" ActiveViewIndex="0">
	<asp:View ID="ViewTable" runat="server">
		<asp:ListView ID="lstNews" runat="server" GroupItemCount="1" 
			onitemcreated="lstNews_ItemCreated" 
			onselectedindexchanging="lstNews_SelectedIndexChanging"
			DataKeyNames="NewsId">
			<Layouttemplate>
				<table id="Table1" runat="server" style="width:100%" border="0" cellpadding="0" cellspacing="0">
					<tr runat="server" id="groupPlaceholder" />
				</table>
			</Layouttemplate>
			<GroupTemplate>
				<tr id="Tr1" runat="server"><td id="itemPlaceholder" /></tr>
			</GroupTemplate>
			<ItemTemplate>
				<td id="newsCell" runat="server" style="vertical-align:top">
					 <asp:PlaceHolder ID="newsPlaceHolder" runat="server" />
				</td>
			</ItemTemplate>
		</asp:ListView>
		<asp:DataPager ID="Pager" runat="server" PageSize="6" CssClass="Normal" PagedControlID="lstNews" onprerender="Pager_PreRender">                       
			<Fields>
				<asp:TemplatePagerField>
					<PagerTemplate>
						<asp:Label ID="lblPage" runat="server" ResourceKey="lblPage.Text"></asp:Label>
						<asp:Label runat="server" ID="CurrentPageLabel" Text="<%# Container.TotalRowCount>0 ? (Container.StartRowIndex / Container.PageSize) + 1 : 0 %>" />
						<asp:Label ID="lblOfPage" runat="server" ResourceKey="lblOfPage.Text"></asp:Label>
						<asp:Label runat="server" ID="TotalPagesLabel" Text="<%# Math.Ceiling ((double)Container.TotalRowCount / Container.PageSize) %>" />
						(<asp:Label runat="server" ID="TotalItemsLabel" Text="<%# Container.TotalRowCount%>" /><asp:Label ID="lblProduct" runat="server" ResourceKey="lblProduct.Text"></asp:Label>):
					</PagerTemplate>
				</asp:TemplatePagerField>
				<asp:NumericPagerField ButtonCount="10" />
			</Fields>
		</asp:DataPager>
	</asp:View>
	<asp:View ID="ViewMarquee" runat="server">
		<asp:Literal ID="ltrMarquee" runat="server" Mode="PassThrough"/>
	</asp:View>
	<asp:View ID="ViewDetail" runat="server">
		<asp:Placeholder ID="plhDetail" runat="server" />
	</asp:View>
</asp:MultiView>

