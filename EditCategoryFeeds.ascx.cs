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
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using Bitboxx.DNNModules.BBNews.Components;
using Bitboxx.DNNModules.BBNews.Models;
using DotNetNuke.Entities.Icons;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Services.Localization;

namespace Bitboxx.DNNModules.BBNews
{
	public partial class EditCategoryFeeds : PortalModuleBase
	{
		#region Private Members

		private BBNewsController _controller;

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
		public Control MainControl { get; set; } 
		#endregion

		protected void Page_Load(object sender, EventArgs e)
		{
			ctlFeeds.AddImageURL = IconController.IconURL("MoveNext");
			ctlFeeds.AddAllImageURL = IconController.IconURL("MoveLast");
			ctlFeeds.RemoveImageURL = IconController.IconURL("MovePrevious");
			ctlFeeds.RemoveAllImageURL = IconController.IconURL("MoveFirst");
			ctlFeeds.AddButtonClick += ctlFeeds_AddButtonClick;
			ctlFeeds.AddAllButtonClick += ctlFeeds_AddAllButtonClick;
			ctlFeeds.RemoveButtonClick += ctlFeeds_RemoveButtonClick;
			ctlFeeds.RemoveAllButtonClick += ctlFeeds_RemoveAllButtonClick;

			FillDdlCategories();
		}

		void ctlFeeds_RemoveAllButtonClick(object sender, EventArgs e)
		{
			int categoryId = Convert.ToInt32(ddlCategories.SelectedValue);
			foreach (FeedInfo feed in DbController.Instance.GetCategoryFeeds(categoryId))
			{
                DbController.Instance.RemoveCategoryFeed(categoryId, feed.FeedID);
			}
			BindData();
		}

		void ctlFeeds_RemoveButtonClick(object sender, DotNetNuke.UI.WebControls.DualListBoxEventArgs e)
		{
			if (e.Items != null)
			{
				int categoryId = Convert.ToInt32(ddlCategories.SelectedValue);
				foreach (string feed in e.Items)
				{
					int feedId = Convert.ToInt32(feed);
                    DbController.Instance.RemoveCategoryFeed(categoryId, feedId);
				}
			}
			BindData();
		}

		void ctlFeeds_AddAllButtonClick(object sender, EventArgs e)
		{
			int categoryId = Convert.ToInt32(ddlCategories.SelectedValue);
			foreach (FeedInfo feed in DbController.Instance.GetFeeds(PortalId))
			{
                DbController.Instance.AddCategoryFeed(categoryId, feed.FeedID);
			}
			BindData();
		}

		void ctlFeeds_AddButtonClick(object sender, DotNetNuke.UI.WebControls.DualListBoxEventArgs e)
		{
			if (e.Items != null)
			{
				int categoryId = Convert.ToInt32(ddlCategories.SelectedValue);
				foreach (string feed in e.Items)
				{
					int feedId = Convert.ToInt32(feed);
                    DbController.Instance.AddCategoryFeed(categoryId, feedId);
				}
			}
			BindData();
		}

		protected void ddlCategories_SelectedIndexChanged(object sender, EventArgs e)
		{
			BindData();
		}

		public void FillDdlCategories()
		{
			List<CategoryInfo> allCats = DbController.Instance.GetCategories(PortalId).ToList();
			ddlCategories.Items.Clear();
			ddlCategories.Items.Add(new ListItem(Localization.GetString("Select.Text", this.LocalResourceFile), "0"));
			ddlCategories.AppendDataBoundItems = true;
			ddlCategories.DataSource = allCats;
			ddlCategories.DataBind();
			ddlCategories.SelectedValue = "0";
			BindData();
		}

		private void BindData()
		{
			ctlFeeds.Visible = false;
			lblFeeds.Visible = false;
			if (ddlCategories.SelectedValue != null)
			{
				int categoryId = Convert.ToInt32(ddlCategories.SelectedValue);
				if (categoryId > 0)
				{
					List<FeedInfo> allFeeds = DbController.Instance.GetFeeds(PortalId).ToList();
					List<FeedInfo> selFeeds = DbController.Instance.GetCategoryFeeds(categoryId).ToList();

					foreach (FeedInfo feed in selFeeds)
					{
						foreach (FeedInfo allFeed in allFeeds)
						{
							if (allFeed.FeedID == feed.FeedID)
							{
								allFeeds.Remove(allFeed);
								break;
							}
						}
					}

					ctlFeeds.AvailableDataSource = allFeeds;
					ctlFeeds.SelectedDataSource = selFeeds;
					ctlFeeds.DataBind();
					ctlFeeds.Visible = true;
					lblFeeds.Visible = true;
				}
			}
		}
	}
}