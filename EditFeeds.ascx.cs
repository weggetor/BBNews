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
using System.Data.SqlTypes;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using Bitboxx.DNNModules.BBNews.Components;
using Bitboxx.DNNModules.BBNews.Models;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Services.Localization;
using DotNetNuke.UI.Skins.Controls;

namespace Bitboxx.DNNModules.BBNews
{
	public partial class EditFeeds : PortalModuleBase
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

        protected DotNetNuke.UI.UserControls.LabelControl lblFeedUrl;

        protected void Page_Load(object sender, EventArgs e)
		{
			Localization.LocalizeDataGrid(ref grdFeeds, this.LocalResourceFile);
			if (!IsPostBack)
			{
				BindData();
			}
		}

		protected void grdFeeds_ItemCommand(object source, DataGridCommandEventArgs e)
		{
			int feedId = Convert.ToInt32(e.CommandArgument);
			switch (e.CommandName)
			{
				case "Edit":
					FeedInfo feed = DbController.Instance.GetFeed(feedId);
					txtFeedName.Text = feed.FeedName;
					txtFeedUrl.Text = feed.FeedUrl;
					cboFeedType.SelectedValue = feed.FeedType.ToString();
					txtLastRetrieve.Text = (feed.LastRetrieve == DateTime.MinValue ? "" : feed.LastRetrieve.ToString());
					txtLastTry.Text = (feed.LastTry == DateTime.MinValue ? "" : feed.LastTry.ToString());
					txtRetrieveInterval.Text = feed.RetrieveInterval.ToString();
					txtTryInterval.Text = feed.TryInterval.ToString();
					txtReorgInterval.Text = feed.ReorgInterval.ToString();
					chkEnabled.Checked = feed.Enabled;
					txtUserName.Text = feed.Username;
					txtPassword.Text = feed.Password;
					hidFeedId.Value = feed.FeedID.ToString();
					InEditMode = true;
					BindData();
					cboFeedType_SelectedIndexChanged(this, new EventArgs());
					break;
				case "Delete":
					try
					{
                        DbController.Instance.DeleteFeed(feedId);
					}
					catch (Exception)
					{
						string message = Localization.GetString("DeleteFeed.Error", this.LocalResourceFile);
						DotNetNuke.UI.Skins.Skin.AddModuleMessage(this, message, ModuleMessage.ModuleMessageType.YellowWarning);
					}
					
					InEditMode = false;
					BindData();
					break;
			}
		}

		protected void cmdNew_Click(object sender, EventArgs e)
		{
			txtFeedName.Text = "";
			txtFeedUrl.Text = "";
			cboFeedType.SelectedValue = "0";
			txtLastRetrieve.Text = "";
			txtLastTry.Text = "";
			txtRetrieveInterval.Text = "120";
			txtTryInterval.Text = "30";
			txtReorgInterval.Text = "31";
			chkEnabled.Checked = true;
			hidFeedId.Value = "-1";
			txtUserName.Text = "";
			txtPassword.Text = "";
			InEditMode = true;
			BindData();
		}

		protected void cmdSave_Click(object sender, EventArgs e)
		{
			FeedInfo feed = new FeedInfo();
			feed.FeedID = Convert.ToInt32(hidFeedId.Value);
			feed.FeedName = txtFeedName.Text;
			feed.FeedUrl = txtFeedUrl.Text;
			feed.FeedType = Convert.ToInt32(cboFeedType.SelectedValue);

		    DateTime time;
		    if (DateTime.TryParse(txtLastRetrieve.Text, out time))
		    {
		        feed.LastRetrieve = time;
		        feed.LastTry = time;
		    }
		    else
		    {
                feed.LastRetrieve = null;
                feed.LastTry = null;
            }
			
			feed.RetrieveInterval = Convert.ToInt32(txtRetrieveInterval.Text);
			feed.TryInterval = Convert.ToInt32(txtTryInterval.Text);
			feed.ReorgInterval = Convert.ToInt32(txtReorgInterval.Text);
			
			feed.Enabled = chkEnabled.Checked;
			feed.PortalId = PortalId;
			feed.Username = txtUserName.Text;
			feed.Password = txtPassword.Text;

            

            DbController.Instance.SaveFeed(feed);
			BindData();
			InEditMode = false;
		}

		protected void cmdCancel_Click(object sender, EventArgs e)
		{
			InEditMode = false;
			BindData();
		}

		protected void cboFeedType_SelectedIndexChanged(object sender, EventArgs e)
		{
			switch (cboFeedType.SelectedValue)
			{
				case "0": // None
					pnlFeedUrl.Visible = false;
					pnlCredentials.Visible = false;
					break;
				case "1": // Twitter Search
					pnlFeedUrl.Visible = true;
					pnlCredentials.Visible = false;
					lblFeedUrl.Text = Localization.GetString("lblFeedUrlTwitterSearch.Text", this.LocalResourceFile);
					break;
				case "2": // RSS
					pnlFeedUrl.Visible = true;
					pnlCredentials.Visible = false;
					lblFeedUrl.Text = Localization.GetString("lblFeedUrlRss.Text", this.LocalResourceFile);
					break;
				case "3": // Twitter Usertimeline
					pnlFeedUrl.Visible = true;
					pnlCredentials.Visible = false;
					lblFeedUrl.Text = Localization.GetString("lblFeedUrlTwitterTimeline.Text", this.LocalResourceFile);
					break;
			}
		}

		private void BindData()
		{
			List<FeedInfo> allFeeds = DbController.Instance.GetFeeds(PortalId).ToList();
			grdFeeds.DataSource = allFeeds;
			grdFeeds.DataBind();
		}
		public string GetfeedType(object feedType)
		{
			switch ((int)feedType)
			{
				case 0:
					return "none";
				case 1:
					return "twitter search";
				case 2:
					return "rss";
				case 3:
					return "twitter timeline";
				default:
					return "";
			}
		}
	}
}