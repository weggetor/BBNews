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
using System.Web.UI.WebControls;
using Bitboxx.DNNModules.BBNews.Components;
using DotNetNuke.Common;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Services.Exceptions;

namespace Bitboxx.DNNModules.BBNews
{

	/// ----------------------------------------------------------------------------- 
	/// <summary> 
	/// The Settings class manages Module Settings 
	/// </summary> 
	/// <remarks> 
	/// </remarks> 
	/// <history> 
	/// </history> 
	/// ----------------------------------------------------------------------------- 
	[DNNtc.ModuleControlProperties("Settings", "Configure settings", DNNtc.ControlType.Edit, "", true, true)]
	partial class Settings : ModuleSettingsBase
	{
		private BBNewsController _controller;
		// private ScheduleItem _scheduleItem = null;
		public BBNewsController Controller
		{
			get
			{
				if (_controller == null)
					_controller = new BBNewsController();
				return _controller;
			}
		}
		#region "Base Method Implementations"

		public override void LoadSettings()
		{
			try
			{
				if (!IsPostBack)
				{
					List<CategoryInfo> cats = Controller.GetCategories(PortalId);
					cboCategory.Items.Add(new ListItem("(Select Category)", "0"));
					foreach (CategoryInfo cat in cats)
					{
						cboCategory.Items.Add(new ListItem(cat.CategoryName, cat.CategoryId.ToString()));
					}

					//cboPubDateFormat.Items.Add(new ListItem(string.Format("{0:d}",DateTime.Now),"d"));
					//cboPubDateFormat.Items.Add(new ListItem(string.Format("{0:D}",DateTime.Now),"D"));
					//cboPubDateFormat.Items.Add(new ListItem(string.Format("{0:t}",DateTime.Now),"t"));
					//cboPubDateFormat.Items.Add(new ListItem(string.Format("{0:T}",DateTime.Now),"T"));
					//cboPubDateFormat.Items.Add(new ListItem(string.Format("{0:f}",DateTime.Now),"f"));
					//cboPubDateFormat.Items.Add(new ListItem(string.Format("{0:F}",DateTime.Now),"F"));
					//cboPubDateFormat.Items.Add(new ListItem(string.Format("{0:g}",DateTime.Now),"g"));
					//cboPubDateFormat.Items.Add(new ListItem(string.Format("{0:G}",DateTime.Now),"G"));
					//cboPubDateFormat.Items.Add(new ListItem(string.Format("{0:M}",DateTime.Now),"M"));
					//cboPubDateFormat.Items.Add(new ListItem(string.Format("{0:r}",DateTime.Now),"r"));
					//cboPubDateFormat.Items.Add(new ListItem(string.Format("{0:s}",DateTime.Now),"s"));
					//cboPubDateFormat.Items.Add(new ListItem(string.Format("{0:u}",DateTime.Now),"u"));
					//cboPubDateFormat.Items.Add(new ListItem(string.Format("{0:U}",DateTime.Now),"U"));
					//cboPubDateFormat.Items.Add(new ListItem(string.Format("{0:Y}",DateTime.Now),"Y"));

					if (TabModuleSettings["ShowTitle"] != null)
						chkShowTitle.Checked = Convert.ToBoolean((string)TabModuleSettings["ShowTitle"]);

					if (TabModuleSettings["NewsPage"] != null)
						urlSelectNewsPage.Url = (string)TabModuleSettings["NewsPage"];
					else
						urlSelectNewsPage.Url = TabId.ToString();

					if (TabModuleSettings["TemplateName"] != null)
						selTemplate.Value = (string)TabModuleSettings["TemplateName"];

					if (TabModuleSettings["RowsPerPage"] != null)
						txtRowsPerPage.Text = (string)TabModuleSettings["RowsPerPage"];

					if (TabModuleSettings["NewsInRow"] != null)
						txtNewsInRow.Text = (string)TabModuleSettings["NewsInRow"];

					if (TabModuleSettings["CategoryID"] != null)
						cboCategory.SelectedValue = (string)TabModuleSettings["CategoryID"];

					if (TabModuleSettings["TopN"] != null)
						txtTopN.Text = (string)TabModuleSettings["TopN"];

					if (TabModuleSettings["StartDate"] != null)
					{
						string[] startDateParts = ((string)TabModuleSettings["StartDate"]).Split(',');
						dtpStartDate.SelectedDate = new DateTime(Convert.ToInt32(startDateParts[0]), Convert.ToInt32(startDateParts[1]), Convert.ToInt32(startDateParts[2]));
					}

					if (TabModuleSettings["EndDate"] != null)
					{
						string[] endDateParts = ((string)TabModuleSettings["EndDate"]).Split(',');
						dtpEndDate.SelectedDate = new DateTime(Convert.ToInt32(endDateParts[0]), Convert.ToInt32(endDateParts[1]), Convert.ToInt32(endDateParts[2]));
					}
					if (TabModuleSettings["ShowRss"] != null && (string)TabModuleSettings["ShowRss"] != String.Empty)
						rblRss.SelectedValue = (string)TabModuleSettings["ShowRss"];
					else
						rblRss.SelectedValue = "0";

					if (TabModuleSettings["MarqueeDirection"] != null)
						cboMarqueeDirection.SelectedValue = (string)TabModuleSettings["MarqueeDirection"];
					else
						cboMarqueeDirection.SelectedIndex = 0;

					if (TabModuleSettings["MarqueeScrollAmount"] != null)
						txtMarqueeScrollAmount.Text = (string)TabModuleSettings["MarqueeScrollAmount"];
					else
						txtMarqueeScrollAmount.Text = "6";

					if (TabModuleSettings["MarqueeScrollDelay"] != null)
						txtMarqueeScrollDelay.Text = (string)TabModuleSettings["MarqueeScrollDelay"];
					else
						txtMarqueeScrollDelay.Text = "85";

					if (TabModuleSettings["MarqueeAlternate"] != null)
						chkMarqueeAlternate.Checked = Convert.ToBoolean(TabModuleSettings["MarqueeAlternate"]);
					else
						chkMarqueeAlternate.Checked = false;


					if (TabModuleSettings["View"] != null)
						rblView.SelectedValue = (string)TabModuleSettings["View"];
					else
						rblView.SelectedIndex = 0;
					rblView_SelectedIndexChanged(this, new EventArgs());

				}
			}
			catch (Exception exc)
			{
				//Module failed to load 
				Exceptions.ProcessModuleLoadException(this, exc);
			}
		}

		/// ----------------------------------------------------------------------------- 
		/// <summary> 
		/// UpdateSettings saves the modified settings to the Database 
		/// </summary> 
		/// <remarks> 
		/// </remarks> 
		/// <history> 
		/// </history> 
		/// ----------------------------------------------------------------------------- 
		public override void UpdateSettings()
		{
			try
			{
				ModuleController objModules = new ModuleController();

				objModules.UpdateTabModuleSetting(TabModuleId, "ShowTitle", chkShowTitle.Checked.ToString());
				objModules.UpdateTabModuleSetting(TabModuleId, "NewsPage", urlSelectNewsPage.Url);
				objModules.UpdateTabModuleSetting(TabModuleId, "TemplateName", selTemplate.Value);
				objModules.UpdateTabModuleSetting(TabModuleId, "NewsInRow", txtNewsInRow.Text);
				objModules.UpdateTabModuleSetting(TabModuleId, "RowsPerPage", txtRowsPerPage.Text);
				objModules.UpdateTabModuleSetting(TabModuleId, "TopN", txtTopN.Text);
				if (dtpStartDate.SelectedDate == null)
					objModules.DeleteTabModuleSetting(TabModuleId, "StartDate");
				else
				{
					DateTime startDate = (DateTime)dtpStartDate.SelectedDate;
					objModules.UpdateTabModuleSetting(TabModuleId, "StartDate", 
							startDate.Year.ToString() + "," + startDate.Month.ToString() + "," + startDate.Day.ToString());
				}
				if (dtpEndDate.SelectedDate == null)
					objModules.DeleteTabModuleSetting(TabModuleId, "EndDate");
				else
				{
					DateTime endDate = (DateTime)dtpEndDate.SelectedDate;
					objModules.UpdateTabModuleSetting(TabModuleId, "EndDate",
							endDate.Year.ToString() + "," + endDate.Month.ToString() + "," + endDate.Day.ToString());
				}

				objModules.UpdateTabModuleSetting(TabModuleId, "CategoryID", cboCategory.SelectedValue);
				objModules.UpdateTabModuleSetting(TabModuleId, "View", rblView.SelectedValue);
				objModules.UpdateTabModuleSetting(TabModuleId, "ShowRss", rblRss.SelectedValue);
				objModules.UpdateTabModuleSetting(TabModuleId, "MarqueeDirection", cboMarqueeDirection.SelectedValue);
				objModules.UpdateTabModuleSetting(TabModuleId, "MarqueeScrollAmount", txtMarqueeScrollAmount.Text);
				objModules.UpdateTabModuleSetting(TabModuleId, "MarqueeScrollDelay", txtMarqueeScrollDelay.Text);
				objModules.UpdateTabModuleSetting(TabModuleId, "MarqueeAlternate", chkMarqueeAlternate.Checked.ToString());

			}

			catch (Exception exc)
			{
				//Module failed to load 
				Exceptions.ProcessModuleLoadException(this, exc);
			}
		}

		#endregion

		protected void rblView_SelectedIndexChanged(object sender, EventArgs e)
		{
			pnlCategory.Visible = true;
			pnlMulti.Visible = (rblView.SelectedIndex != 2);
			pnlNewsRows.Visible = (rblView.SelectedIndex == 0);
			pnlSelectPage.Visible = (rblView.SelectedIndex != 2);
			pnlShowTitle.Visible = (rblView.SelectedIndex == 2);
			pnlMarquee.Visible = (rblView.SelectedIndex == 1);
		}

	}
}

