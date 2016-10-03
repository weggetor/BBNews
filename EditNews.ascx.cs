#region copyright

// bitboxx - http://www.bitboxx.net
// Copyright (c) 2012 
// by bitboxx solutions Torsten Weggen
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
// documentation files (the "Software"), to deal in the Software without restriction, including without limitation 
// the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and 
// to permit persons to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or substantial portions 
// of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED
// TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL 
// THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF 
// CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
// DEALINGS IN THE SOFTWARE.

#endregion


using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using Bitboxx.DNNModules.BBNews.Components;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Services.Localization;

namespace Bitboxx.DNNModules.BBNews
{
	public partial class EditNews : PortalModuleBase
	{
		#region Private Members

		private BBNewsController _controller;
		private bool _inEditMode = false;

		#endregion

		#region Properties

		public BBNewsController Controller
		{
			get
			{
				if (_controller == null)
					_controller = new BBNewsController();
				return _controller;
			}
		}
		public bool InEditMode
		{
			get { return _inEditMode; }
			set
			{
				_inEditMode = value;
				pnlEdit.Visible = _inEditMode;
				pnlSearch.Visible = !_inEditMode;
				cmdNew.Visible = !_inEditMode;
				cmdSave.Visible = _inEditMode;
				cmdCancel.Visible = _inEditMode;
			}
		}
		public Control MainControl { get; set; }
        #endregion
        
        protected DotNetNuke.UI.UserControls.TextEditor txtSummary;
        protected DotNetNuke.UI.UserControls.TextEditor txtNews;

        protected void Page_Load(object sender, EventArgs e)
		{
			Localization.LocalizeDataGrid(ref grdNews, this.LocalResourceFile);

			List<FeedInfo> allFeeds = Controller.GetFeeds(PortalId);
			ddlFeed.Items.Clear();
			ddlFeed.Items.Add(new ListItem(Localization.GetString("SelectFeed.Text", this.LocalResourceFile), "-1"));
			ddlFeed.AppendDataBoundItems = true;
			ddlFeed.DataSource = allFeeds;
			ddlFeed.DataBind();
			ddlFeed.DataTextField = "FeedName";
			ddlFeed.DataValueField = "FeedId";
			ddlFeed.SelectedValue = "-1";

			if (!IsPostBack)
			{
				List<CategoryInfo> allCats = Controller.GetCategories(PortalId);
				ddlSearchCategory.Items.Clear();
				ddlSearchCategory.Items.Add(new ListItem(Localization.GetString("SelectCategory.Text", this.LocalResourceFile), "-1"));
				ddlSearchCategory.AppendDataBoundItems = true;
				ddlSearchCategory.DataSource = allCats;
				ddlSearchCategory.DataBind();
				ddlSearchCategory.DataTextField = "CategoryName";
				ddlSearchCategory.DataValueField = "CategoryId";
				ddlSearchCategory.SelectedValue = "-1";

				BindData();
			}
		}

		protected void grdNews_ItemCommand(object source, DataGridCommandEventArgs e)
		{
			int newsId;
			switch (e.CommandName)
			{
				case "Edit":
				    newsId = Convert.ToInt32(e.CommandArgument);
					NewsInfo news = Controller.GetNews(newsId);
					txtTitle.Text = news.Title;
					ddlFeed.SelectedValue = news.FeedId.ToString();
					txtSummary.Text = news.Summary;
					txtNews.Text = news.News;
					chkHide.Checked = news.Hide;
					chkInternal.Checked = news.Internal;
					if (news.Pubdate == DateTime.MinValue)
					{
						dpPubDate.SelectedDate = null;
						tpPubDate.SelectedDate = null;
					}
					else
					{
						dpPubDate.SelectedDate = news.Pubdate;
						tpPubDate.SelectedDate = news.Pubdate;
					}

					string[] author = news.Author.Split('|');
					if (author.Length == 4)
					{
						txtAuthorName.Text = author[0];
						txtAuthorUrl.Text = author[1];
						txtAuthorEmail.Text = author[2];
						txtAuthorNick.Text = author[3];
					}
					else
					{
						txtAuthorName.Text = news.Author;
						txtAuthorUrl.Text = "";
						txtAuthorEmail.Text = "";
						txtAuthorNick.Text = "";
					}
					
					txtImage.Text = news.Image;
					txtLink.Text = news.Link;
					hidNewsId.Value = news.NewsId.ToString();
					hidGuid.Value = news.GUID;
				    InEditMode = true;
				    break;

				case "Delete":
				    newsId = Convert.ToInt32(e.CommandArgument);
				    Controller.DeleteNews(newsId);
				    InEditMode = false;
					BindData();
				    break;
			}
		}

		protected void grdNews_ItemCreated(object sender, DataGridItemEventArgs e)
		{
			if (e.Item.ItemType == ListItemType.Pager)
			{
				Label lblPagerText = new Label();
				lblPagerText.Text = Localization.GetString("lblPagerText.Text", this.LocalResourceFile) + " ";
				lblPagerText.Font.Bold = true;
				if ( e.Item.Controls[0].FindControl("lblPagerText1" ) == null)
					e.Item.Controls[0].Controls.AddAt(0, lblPagerText);
			}
		}

		protected void grdNews_PageIndexChanged(object source, DataGridPageChangedEventArgs e)
		{
			grdNews.CurrentPageIndex = e.NewPageIndex;
			BindData();
		}

		protected void cmdNew_Click(object sender, EventArgs e)
		{
			txtTitle.Text = "";
			ddlFeed.SelectedValue = "-1";
			txtSummary.Text = "";
			txtNews.Text = "";
			chkHide.Checked = false;
			chkInternal.Checked = true;
			dpPubDate.SelectedDate = DateTime.Now;
			tpPubDate.SelectedDate = DateTime.Now;
			txtAuthorName.Text = "";
			txtAuthorUrl.Text = "";
			txtAuthorEmail.Text = "";
			txtAuthorNick.Text = "";
			txtImage.Text = "";
			txtLink.Text = "";
			hidNewsId.Value = "-1";
			hidGuid.Value = Guid.NewGuid().ToString();
			InEditMode = true;
			BindData();
		}

		protected void cmdSave_Click(object sender, EventArgs e)
		{
			NewsInfo news = new NewsInfo();
			news.NewsId = Convert.ToInt32(hidNewsId.Value);
			news.GUID = hidGuid.Value;
			news.Title = txtTitle.Text;
			news.FeedId = Convert.ToInt32(ddlFeed.SelectedValue);
			news.Summary = txtSummary.Text;
			news.News = txtNews.Text;
			news.Hide = chkHide.Checked;
			if (dpPubDate.SelectedDate == null || tpPubDate.SelectedDate == null)
				news.Pubdate = DateTime.MinValue;
			else
			{
				news.Pubdate = new DateTime(dpPubDate.SelectedDate.Value.Year,
					dpPubDate.SelectedDate.Value.Month,
					dpPubDate.SelectedDate.Value.Day,
					tpPubDate.SelectedDate.Value.Hour,
					tpPubDate.SelectedDate.Value.Minute,
					tpPubDate.SelectedDate.Value.Second);
			}
			news.Link = txtLink.Text;
			news.Image = txtImage.Text;
			news.Internal = chkInternal.Checked;
			news.Author = txtAuthorName.Text + "|" + txtAuthorUrl.Text + "|" + txtAuthorEmail.Text + "|" + txtAuthorNick.Text;

			Controller.SaveNewsById(news);
			InEditMode = false;
			BindData();
		}

		protected void cmdCancel_Click(object sender, EventArgs e)
		{
			InEditMode = false;
			BindData();
		}
		protected void cmdSearch_Click(object sender, EventArgs e)
		{
			InEditMode = false;
			grdNews.CurrentPageIndex = 0;
			BindData();
		}

		private void BindData()
		{
			int topN = -1;
			DateTime startDate = dpDateStart.SelectedDate == null ? new DateTime() : (DateTime) dpDateStart.SelectedDate;
			DateTime endDate = dpDateEnd.SelectedDate == null ? new DateTime() : (DateTime)dpDateEnd.SelectedDate; 
			String search = txtSearch.Text;
			int categoryId = Convert.ToInt32(ddlSearchCategory.SelectedValue);
			
			int pageNum = grdNews.CurrentPageIndex +1;
			int pageSize = 15;

			List<NewsInfo> allNews = Controller.GetNews(PortalId, categoryId, topN, startDate, endDate, pageNum, pageSize,true,search);
			grdNews.DataSource = allNews;
			grdNews.PageSize = pageSize;
			grdNews.VirtualItemCount = Controller.GetNewsCount(PortalId, categoryId, topN, startDate, endDate,true,search);
			grdNews.DataBind();
		}


		public string GetFeedName(object feedId)
		{
			FeedInfo feed = Controller.GetFeed((int) feedId);
			return feed.FeedName;
		}

		public string GetNewsTitle(object title, int maxLength)
		{
			string retVal = (string) title;
			if (retVal.Length > maxLength)
				return retVal.Substring(0, maxLength) + "...";
			return retVal;
		}

	}
}