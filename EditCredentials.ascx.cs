#region copyright

// bitboxx - http://www.bitboxx.net
// Copyright (c) 2014 
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
using System.Web.UI;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Services.Localization;
using DotNetNuke.Services.Scheduling;
using DotNetNuke.UI.Skins;
using DotNetNuke.UI.Skins.Controls;

namespace Bitboxx.DNNModules.BBNews
{
	public partial class EditCredentials : PortalModuleBase
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
            txtTwitterAccessToken.Text = PortalController.GetPortalSetting("BB_TwitterToken", PortalId, "");
            txtTwitterAccessTokenSecret.Text = PortalController.GetPortalSetting("BB_TwitterTokenSecret", PortalId, "");
            txtTwitterConsumerKey.Text = PortalController.GetPortalSetting("BB_TwitterConsumerKey", PortalId, "");
            txtTwitterConsumerSecret.Text = PortalController.GetPortalSetting("BB_TwitterConsumerSecret", PortalId, "");
 		}

	    protected void cmdUpdate_OnClick(object sender, EventArgs e)
	    {
            PortalController.UpdatePortalSetting(PortalId, "BB_TwitterToken",txtTwitterAccessToken.Text.Trim());
            PortalController.UpdatePortalSetting(PortalId, "BB_TwitterTokenSecret", txtTwitterAccessTokenSecret.Text.Trim());
            PortalController.UpdatePortalSetting(PortalId, "BB_TwitterConsumerKey", txtTwitterConsumerKey.Text.Trim());
            PortalController.UpdatePortalSetting(PortalId, "BB_TwitterConsumerSecret", txtTwitterConsumerSecret.Text.Trim());
	    }
	}
}