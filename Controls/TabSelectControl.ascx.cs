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
using System.Web.UI.WebControls;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Users;
using DotNetNuke.Framework;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Entities.Tabs;

namespace Bitboxx.DNNModules.BBNews.Controls
{

    public partial class TabSelectControl : UserControlBase
    {

        private int _ModuleID = -2;
        private PortalInfo _objPortal;
        private string _Url = "";
        private string _Width = "";
        protected DropDownList cboTabs;

        public int ModuleID
        {
            get
            {
                int returnValue;
                returnValue = Convert.ToInt32(ViewState["ModuleId"]);
                if (returnValue == -2)
                {
                    if (Request.QueryString["mid"] != null)
                    {
                        returnValue = int.Parse(Request.QueryString["mid"]);
                    }
                }
                return returnValue;
            }
            set
            {
                this._ModuleID = value;
            }
        }
        public string Url
        {
            get
            {
                string returnValue;
                returnValue = "";

                if (cboTabs.SelectedItem != null)
                {
                    returnValue = cboTabs.SelectedItem.Value;
                }
                return returnValue;
            }
            set
            {
                this._Url = value;
            }
        }
        public string Width
        {
            get
            {
                return Convert.ToString(this.ViewState["SkinControlWidth"]);
            }
            set
            {
                this._Width = value;
            }
        }

        public TabSelectControl()
        {
            Load += new EventHandler(this.Page_Load);

            this._ModuleID = -2;
            this._Url = "";
            this._Width = "";
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                PortalController objPortals = new PortalController();
                if (Request.QueryString["pid"] != null && (PortalSettings.ActiveTab.ParentId == PortalSettings.SuperTabId || UserController.GetCurrentUserInfo().IsSuperUser))
                {
                    _objPortal = objPortals.GetPortal(int.Parse(Request.QueryString["pid"]));
                }
                else
                {
                    _objPortal = objPortals.GetPortal(PortalSettings.PortalId);
                }

                if (!Page.IsPostBack)
                {
                    // set width of control
                    if (!String.IsNullOrEmpty(_Width))
                    {
                        cboTabs.Width = Unit.Parse(_Width);
                    }
                    // save persistent values
                    ViewState["ModuleId"] = Convert.ToString(_ModuleID);
                    ViewState["SkinControlWidth"] = _Width;

                    // load listitems
                    cboTabs.Items.Clear();

                    cboTabs.DataSource = TabController.GetPortalTabs(_objPortal.PortalID, -1, true, true, false, false);
                    cboTabs.DataBind();
                    if (cboTabs.Items.FindByValue(_Url) != null)
                    {
                        cboTabs.Items.FindByValue(_Url).Selected = true;
                    }
                }
            }
            catch (Exception exc) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }


     }
}
