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


// Some blog info about Scheduler
// http://kemmis.info/blog/archive/2008/05/20/programmatically-add-delete-and-update-scheduled-tasks.aspx


using System;
using System.Collections;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Services.Localization;
using DotNetNuke.Services.Scheduling;
using DotNetNuke.UI.Skins;
using DotNetNuke.UI.Skins.Controls;

namespace Bitboxx.DNNModules.BBNews
{
	public partial class EditScheduler : PortalModuleBase
	{
		#region Private Members

		private BBNewsController _controller;
		private ScheduleItem _scheduleItem;	

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

		#endregion

		
		protected void Page_Load(object sender, EventArgs e)
		{
			ArrayList scheduleItems = SchedulingProvider.Instance().GetSchedule();

			// find schedule item with matching TypeFullName
			foreach (object item in scheduleItems)
			{
			    if (((ScheduleItem)item).TypeFullName == "Bitboxx.DNNModules.BBNews.BBNewsScheduler, Bitboxx.DNNModules.BBNews")
			    {
			        _scheduleItem = (ScheduleItem)item;
			        break;
			    }
			}
			// If we have no scheduler Task we create one!
			if (_scheduleItem == null)
			{
			    _scheduleItem = new ScheduleItem();
			    _scheduleItem.FriendlyName = "Bitboxx BBNews Feed Schedule";
			    _scheduleItem.TypeFullName = "Bitboxx.DNNModules.BBNews.BBNewsScheduler, Bitboxx.DNNModules.BBNews";
			    _scheduleItem.TimeLapse = 1; // execution frequency 
			    _scheduleItem.TimeLapseMeasurement = "m"; // "s" for seconds, "m" for minutes, "h" for hours, "d" for days
			    _scheduleItem.RetryTimeLapse = 10; // retry frequency
			    _scheduleItem.RetryTimeLapseMeasurement = "s"; // "s" for seconds, "m" for minutes, "h" for hours, "d" for days
			    _scheduleItem.RetainHistoryNum = 50;
			    _scheduleItem.AttachToEvent = "None"; //for instance "APPLICATION_START" or "None"
			    _scheduleItem.CatchUpEnabled = false;
			    _scheduleItem.Enabled = true;
			    _scheduleItem.ObjectDependencies = ""; //for example "SiteLog,Users,UsersOnline"
			    _scheduleItem.Servers = ""; // (Optional)
			    _scheduleItem.ScheduleID = SchedulingProvider.Instance().AddSchedule(_scheduleItem);
			}

			txtFriendlyName.Text = _scheduleItem.FriendlyName;
			chkEnabled.Checked = _scheduleItem.Enabled;
			if (_scheduleItem.TimeLapse == Null.NullInteger)
			{
				txtTimeLapse.Text = "";
			}
			else
			{
				txtTimeLapse.Text = Convert.ToString(_scheduleItem.TimeLapse);
			}
			if (ddlTimeLapseMeasurement.Items.FindByValue(_scheduleItem.TimeLapseMeasurement) != null)
			{
				ddlTimeLapseMeasurement.Items.FindByValue(_scheduleItem.TimeLapseMeasurement).Selected = true;
			}
			if (_scheduleItem.RetryTimeLapse == Null.NullInteger)
			{
				txtRetryTimeLapse.Text = "";
			}
			else
			{
				txtRetryTimeLapse.Text = Convert.ToString(_scheduleItem.RetryTimeLapse);
			}
			if (ddlRetryTimeLapseMeasurement.Items.FindByValue(_scheduleItem.RetryTimeLapseMeasurement) != null)
			{
				ddlRetryTimeLapseMeasurement.Items.FindByValue(_scheduleItem.RetryTimeLapseMeasurement).Selected = true;
			}
			if (ddlRetainHistoryNum.Items.FindByValue(Convert.ToString(_scheduleItem.RetainHistoryNum)) != null)
			{
				ddlRetainHistoryNum.Items.FindByValue(Convert.ToString(_scheduleItem.RetainHistoryNum)).Selected = true;
			}
			else
			{
				ddlRetainHistoryNum.Items.Add(_scheduleItem.RetainHistoryNum.ToString());
				ddlRetainHistoryNum.Items.FindByValue(Convert.ToString(_scheduleItem.RetainHistoryNum)).Selected = true;
			}

		}
		protected void OnRunClick(object sender, EventArgs e)
		{
			SchedulingProvider.Instance().RunScheduleItemNow(_scheduleItem);
			var strMessage = Localization.GetString("RunNow", LocalResourceFile);
			Skin.AddModuleMessage(this, strMessage, ModuleMessage.ModuleMessageType.GreenSuccess);
		}

		/// <summary>
		/// cmdUpdate_Click runs when the Update Button is clicked
		/// </summary>
		/// <remarks>
		/// </remarks>
		/// <history>
		/// [cnurse]	9/28/2004	Updated to reflect design changes for Help, 508 support and localisation
		/// </history>
		protected void OnUpdateClick(Object sender, EventArgs e)
		{
			_scheduleItem.FriendlyName = txtFriendlyName.Text;
			if (String.IsNullOrEmpty(txtTimeLapse.Text) || txtTimeLapse.Text == "0" || txtTimeLapse.Text == "-1")
			{
				_scheduleItem.TimeLapse = Null.NullInteger;
			}
			else
			{
				_scheduleItem.TimeLapse = Convert.ToInt32(txtTimeLapse.Text);
			}
			_scheduleItem.TimeLapseMeasurement = ddlTimeLapseMeasurement.SelectedItem.Value;

			if (String.IsNullOrEmpty(txtRetryTimeLapse.Text) || txtRetryTimeLapse.Text == "0" || txtRetryTimeLapse.Text == "-1")
			{
				_scheduleItem.RetryTimeLapse = Null.NullInteger;
			}
			else
			{
				_scheduleItem.RetryTimeLapse = Convert.ToInt32(txtRetryTimeLapse.Text);
			}
			_scheduleItem.RetryTimeLapseMeasurement = ddlRetryTimeLapseMeasurement.SelectedItem.Value;
			_scheduleItem.RetainHistoryNum = Convert.ToInt32(ddlRetainHistoryNum.SelectedItem.Value);
			_scheduleItem.Enabled = chkEnabled.Checked;
			
			
			SchedulingProvider.Instance().UpdateSchedule(_scheduleItem);
			
			var strMessage = Localization.GetString("UpdateSuccess", LocalResourceFile);
			Skin.AddModuleMessage(this, strMessage, ModuleMessage.ModuleMessageType.GreenSuccess);
			//Response.Redirect(Globals.NavigateURL(), true);
		}

	}
}