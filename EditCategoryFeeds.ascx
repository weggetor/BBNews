<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="EditCategoryFeeds.ascx.cs" Inherits="Bitboxx.DNNModules.BBNews.EditCategoryFeeds" %>
<%@ Register TagPrefix="dnn" Assembly="DotNetNuke" Namespace="DotNetNuke.UI.WebControls" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %> 

<div class="dnnForm dnnBBNewsEditCategoryFeeds dnnClear">
	<div class="dnnFormMessage dnnFormInfo">
		<asp:Label ID="lblIntro" runat="server" ResourceKey="Intro" />
	</div>
	<div class="dnnFormItem">
		<dnn:Label id="lblCategories" runat="server" controlname="ddlCategories" suffix=":" />
		<asp:DropDownList runat="server" ID="ddlCategories" 
		OnSelectedIndexChanged="ddlCategories_SelectedIndexChanged"
		DataTextField="CategoryName"
		DataValueField="CategoryId" 
		AutoPostBack="True" />
	</div>
	<div class="dnnFormItem">
		<dnn:Label id="lblFeeds" runat="server" controlname="ctlFeeds" suffix=":" />
		<dnn:DualListBox id="ctlFeeds" runat="server" 
		   DataValueField="FeedID" 
		   DataTextField="FeedName" 
		   AddKey="AddFeed" 
		   RemoveKey="RemoveFeed" 
		   AddAllKey="AddAllFeeds" 
		   RemoveAllKey="RemoveAllFeeds" >
		  <AvailableListBoxStyle Height="130px" Width="225px" />
		  <HeaderStyle />
		  <SelectedListBoxStyle Height="130px" Width="225px"  />
		</dnn:DualListBox>
	</div>
</div>
