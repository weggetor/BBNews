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
using DotNetNuke.Entities.Modules;
using DotNetNuke.Framework;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.Localization;

namespace Bitboxx.DNNModules.BBNews
{
	[DNNtc.ModuleControlProperties("Edit", "Bitboxx.BBNews Admin", DNNtc.ControlType.Edit, "", true, true)]
	public partial class Edit : PortalModuleBase
	{
		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);
			jQuery.RequestDnnPluginsRegistration();
		}

		protected void Page_Load(object sender, EventArgs e)
		{
			try
			{
				PortalModuleBase ctrlCategories = LoadControl("EditCategories.ascx") as PortalModuleBase;
				ctrlCategories.ModuleConfiguration = this.ModuleConfiguration;
				ctrlCategories.LocalResourceFile = Localization.GetResourceFile(ctrlCategories, ctrlCategories.GetType().BaseType.Name + ".ascx");
				plCategories.Controls.Add(ctrlCategories);

				PortalModuleBase ctrlFeeds = LoadControl("EditFeeds.ascx") as PortalModuleBase;
				ctrlFeeds.ModuleConfiguration = this.ModuleConfiguration;
				ctrlFeeds.LocalResourceFile = Localization.GetResourceFile(ctrlFeeds, ctrlFeeds.GetType().BaseType.Name + ".ascx");
				plFeeds.Controls.Add(ctrlFeeds);

				PortalModuleBase ctrlCategoryfeeds = LoadControl("EditCategoryFeeds.ascx") as PortalModuleBase;
				ctrlCategoryfeeds.ModuleConfiguration = this.ModuleConfiguration;
				ctrlCategoryfeeds.LocalResourceFile = Localization.GetResourceFile(ctrlCategoryfeeds, ctrlCategoryfeeds.GetType().BaseType.Name + ".ascx");
				plCategoryFeeds.Controls.Add(ctrlCategoryfeeds);

				PortalModuleBase ctrlNews = LoadControl("EditNews.ascx") as PortalModuleBase;
				ctrlNews.ModuleConfiguration = this.ModuleConfiguration;
				ctrlNews.LocalResourceFile = Localization.GetResourceFile(ctrlNews, ctrlNews.GetType().BaseType.Name + ".ascx");
				plNews.Controls.Add(ctrlNews);

				PortalModuleBase ctrlScheduler = LoadControl("EditScheduler.ascx") as PortalModuleBase;
				ctrlScheduler.ModuleConfiguration = this.ModuleConfiguration;
				ctrlScheduler.LocalResourceFile = Localization.GetResourceFile(ctrlNews, ctrlScheduler.GetType().BaseType.Name + ".ascx");
				plScheduler.Controls.Add(ctrlScheduler);
			}
			catch (Exception ex)
			{
				//Module failed to load 
				Exceptions.ProcessModuleLoadException(this, ex);
			}
		}
	}
}