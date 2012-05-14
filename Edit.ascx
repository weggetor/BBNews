<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Edit.ascx.cs" Inherits="Bitboxx.DNNModules.BBNews.Edit" %>
<%@ Register TagPrefix="dnn" Assembly="DotNetNuke" Namespace="DotNetNuke.UI.WebControls"%>
<script language="javascript" type="text/javascript">
	/*globals jQuery, window, Sys */
	(function ($, Sys) {
		function setUpBBNewsTabs() {
			$('#dnnBBNewsEdit').dnnTabs();
		}

		$(document).ready(function () {
			setUpBBNewsTabs();
			Sys.WebForms.PageRequestManager.getInstance().add_endRequest(function () {
				setUpBBNewsTabs();
			});
		});
	} (jQuery, window.Sys));
</script>

<div class="dnnForm dnnBBNewsEdit dnnClear" id="dnnBBNewsEdit">
	<ul class="dnnAdminTabNav dnnClear" id="">
		<li><a href="#bbEditCategories"><%=LocalizeString("bbEditCategories")%></a></li>
        <li><a href="#bbEditFeeds"><%=LocalizeString("bbEditFeeds")%></a></li>
		<li><a href="#bbManageCategoryFeeds"><%=LocalizeString("bbManageCategoryFeeds")%></a></li>
		<li><a href="#bbEditNews"><%=LocalizeString("bbEditNews")%></a></li>
		<li><a href="#bbEditScheduler"><%=LocalizeString("bbEditScheduler")%></a></li>
	</ul>
	<div id="bbEditCategories" class="bbEditCategories dnnClear">
		<asp:PlaceHolder runat="server" ID="plCategories"></asp:PlaceHolder>
	</div>
	<div id="bbEditFeeds" class="bbEditFeeds dnnClear">
	   <asp:PlaceHolder runat="server" ID="plFeeds"></asp:PlaceHolder>
	</div>
	<div id="bbManageCategoryFeeds" class="bbManageCategoryFeeds dnnClear">
		<asp:PlaceHolder runat="server" ID="plCategoryFeeds"></asp:PlaceHolder>
	</div>
	<div id="bbEditNews" class="bbEditNews dnnClear">
		<asp:PlaceHolder runat="server" ID="plNews"></asp:PlaceHolder>
	</div>
	<div id="bbEditScheduler" class="bbEditNews dnnClear">
		<asp:PlaceHolder runat="server" ID="plScheduler"></asp:PlaceHolder>
	</div>
</div>