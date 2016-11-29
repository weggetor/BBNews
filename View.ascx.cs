/*
' Copyright (c) 2015  bitboxx solutions
'  All rights reserved.
' 
' THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED
' TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL
' THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF
' CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
' DEALINGS IN THE SOFTWARE.
' 
*/

using System.Collections;
using System.Linq;
using System.Resources;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Users;
using DotNetNuke.Framework.JavaScriptLibraries;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Web.Client.ClientResourceManagement;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.ServiceModel.Syndication;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;
using Bitboxx.DNNModules.BBNews.Components;
using Bitboxx.DNNModules.BBNews.Models;
using DotNetNuke.Application;
using DotNetNuke.Entities.Modules.Actions;
using DotNetNuke.Services.Localization;
using DotNetNuke.UI.Utilities;
using Newtonsoft.Json;
using Globals = DotNetNuke.Common.Globals;

namespace Bitboxx.DNNModules.BBNews
{
    [DNNtc.PackageProperties("Bitboxx.BBNews", 1, "Bitboxx BBNews", "A flexible RSS/News reader, provider and aggregator", "bbnews.png", "Torsten Weggen", "bitboxx solutions", "http://www.bitboxx.net", "info@bitboxx.net", true)]
    [DNNtc.ModuleProperties("Bitboxx.BBNews", "Bitboxx BBNews", -1)]
    [DNNtc.ModuleDependencies(DNNtc.ModuleDependency.CoreVersion, "08.00.00")]
    [DNNtc.ModuleControlProperties("", "Bitboxx.BBNews", DNNtc.ControlType.View, "", true, false)]
    public partial class View : PortalModuleBase, IActionable
    {
        protected string ModuleProperties
        {
            get
            {
                UserInfo currentUser = UserController.GetCurrentUserInfo();

                Dictionary<string, string> resources;
                using (var rsxr = new ResXResourceReader(MapPath(LocalResourceFile + ".ascx.resx")))
                {
                    resources = rsxr.OfType<DictionaryEntry>()
                        .ToDictionary(
                            entry => entry.Key.ToString().Replace(".", "_"),
                            entry => LocalizeString(entry.Key.ToString()));

                }

                List<string> languages = new List<string>();
                LocaleController lc = new LocaleController();
                Dictionary<string, Locale> loc = lc.GetLocales(PortalId);
                foreach (KeyValuePair<string, Locale> item in loc)
                {
                    string cultureCode = item.Value.Culture.Name;
                    languages.Add(cultureCode);
                }

                dynamic properties = new ExpandoObject();
                properties.Resources = resources;
                properties.Settings = Settings;
                properties.ApplicationPath = Globals.ApplicationPath;
                properties.IsEditable = IsEditable;
                properties.EditMode = EditMode;
                properties.IsAdmin = currentUser == null ? false : currentUser.IsInRole(PortalSettings.AdministratorRoleName);
                properties.ModuleId = ModuleId;
                properties.PortalId = PortalId;
                properties.UserId = UserId;
                properties.HomeDirectory = PortalSettings.HomeDirectory.Substring(1);
                properties.ModuleDirectory = this.ControlPath;
                properties.PortalLanguages = languages;
                properties.CurrentLanguage = System.Threading.Thread.CurrentThread.CurrentCulture.Name;

                return ClientAPI.GetSafeJSString(JsonConvert.SerializeObject(properties));
            }
        }

        protected string ModuleDirectory
        {
            get
            {
                string moduleDirectory = "/" + this.ControlPath;
                return moduleDirectory.Substring(0, moduleDirectory.LastIndexOf('/') + 1);
            }
        }


        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                DotNetNuke.Framework.ServicesFramework.Instance.RequestAjaxScriptSupport();
                DotNetNuke.Framework.ServicesFramework.Instance.RequestAjaxAntiForgerySupport();
            }
            catch (Exception exc) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        protected void Page_Prerender(object sender, EventArgs e)
        {
            if (Request["feed"] != null)
            {
                string format = Request["feed"].ToLower().Trim();
                string feedUrl = Globals.NavigateURL(TabId, "", "feed=" + Request["feed"]);
                string alternateUrl = Globals.NavigateURL(TabId);

                string appUrl = string.Format("{0}://{1}{2}{3}",
                                    Request.Url.Scheme,
                                    Request.Url.Host,
                                    Request.Url.Port == 80
                                        ? string.Empty
                                        : ":" + Request.Url.Port,
                                    Request.ApplicationPath);
                string newsPage = (string) Settings["NewsPage"];

                int categoryId = Convert.ToInt32(Settings["CategoryID"] ?? "0");
                SyndicationFeed feed = new BBNewsController().CreateFeed(categoryId, feedUrl,alternateUrl,appUrl,newsPage);
                Response.Clear();
                Response.ContentType = "text/xml";
                Response.Charset = "utf-8";
                XmlWriterSettings settings = new XmlWriterSettings();
                settings.Indent = true;
                using (XmlWriter writer = XmlWriter.Create(Response.OutputStream, settings))
                {
                    switch (format)
                    {
                        case "atom":
                            Atom10FeedFormatter atom10Formatter = new Atom10FeedFormatter(feed);
                            atom10Formatter.WriteTo(writer);
                            break;
                        case "rss":
                        default:
                            Rss20FeedFormatter rss20Formatter = new Rss20FeedFormatter(feed);
                            rss20Formatter.WriteTo(writer);
                            break;

                    }
                    writer.Flush();
                }
                Response.Flush();
                Response.End();
            }
            else
            {

                string titleLabelName = DotNetNukeContext.Current.Application.Version.Major < 6 ? "lblTitle" : "titleLabel";
                Control ctl = Globals.FindControlRecursiveDown(this.ContainerControl, titleLabelName);

                if ((ctl != null))
                {
                    int newsId = Convert.ToInt32(Request["newsid"] ?? "-1");
                    NewsInfo news = DbController.Instance.GetNews(newsId);

                    if (Settings["ShowTitle"] != null && Convert.ToBoolean((string) Settings["ShowTitle"]) && news != null)
                    {
                        // We can set the title of our module
                        ((Label) ctl).Text = news.Title;
                    }
                    if (Settings["ShowRss"] != null)
                    {
                        int position = Convert.ToInt32(Settings["ShowRss"]);
                        Control rssIconCtrl = ParseControl("<a href=\"" + Globals.NavigateURL(TabId, "", "feed=rss") +
                                                           "\"><img src=\"" + Page.ResolveUrl("~/images/action_rss.gif") + "\" style=\"vertical-align:middle; padding-right:5px;\" /></a>");
                        switch (position)
                        {
                            case 1:
                                ctl.Parent.Controls.AddAt(0, rssIconCtrl);
                                break;
                            case 2:
                                ctl.Parent.Controls.Add(rssIconCtrl);
                                break;
                        }

                    }
                }
            }
        }


        #region Implementation of IActionable

            /// ----------------------------------------------------------------------------- 
            /// <summary> 
            /// Registers the module actions required for interfacing with the portal framework 
            /// </summary> 
            /// <value></value> 
            /// <returns></returns> 
            /// <remarks></remarks> 
            /// <history> 
            /// </history> 
            /// ----------------------------------------------------------------------------- 
        public ModuleActionCollection ModuleActions
        {
            get
            {
                ModuleActionCollection Actions = new ModuleActionCollection();
                Actions.Add(
                    GetNextActionID(),
                    Localization.GetString(ModuleActionType.EditContent, this.LocalResourceFile),
                    ModuleActionType.EditContent,
                    "",
                    "Edit.gif",
                    EditUrl(),
                    false,
                    DotNetNuke.Security.SecurityAccessLevel.Edit,
                    true,
                    false);
                return Actions;
            }
        }


        #endregion

    }
}