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
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.ServiceModel.Syndication;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Xml;
using System.Xml.Linq;
using Bitboxx.DNNModules.BBNews.Components;
using DotNetNuke.Application;
using DotNetNuke.Common;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Modules.Actions;
using DotNetNuke.Entities.Users;
using DotNetNuke.Security;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.Localization;
using DotNetNuke.UI.Skins.Controls;

namespace Bitboxx.DNNModules.BBNews
{
    [DNNtc.PackageProperties("Bitboxx.BBNews", 1, "Bitboxx BBNews", "A flexible RSS/News reader, provider and aggregator", "bbnews.png", "Torsten Weggen", "bitboxx solutions", "http://www.bitboxx.net", "info@bitboxx.net", true)]
    [DNNtc.ModuleProperties("Bitboxx.BBNews", "Bitboxx BBNews", -1)]
    [DNNtc.ModuleDependencies(DNNtc.ModuleDependency.CoreVersion, "06.01.00")]
	[DNNtc.ModuleControlProperties("", "Bitboxx.BBNews", DNNtc.ControlType.View,"", true, false)]
	partial class View : PortalModuleBase,IActionable
	{
		#region Private Members
		
		private BBNewsController _controller;
		private bool _isConfigured = false;
		private int _newsInRow = 0;
		private int _rowsPerPage = 0;
		private NewsInfo News = null;
		private NewsInfo _theNews = null;

		#endregion

		#region Properties
		
		public int CategoryId
		{
			get
			{
				int _categoryId = -1;
				if (Settings["CategoryID"] != null)
					Int32.TryParse((string)Settings["CategoryID"], out _categoryId);
				return _categoryId;
			}
		}
		public int ViewIndex
		{
			get
			{
				int _viewIndex = 0;
				if (Settings["View"] != null)
					Int32.TryParse((string)Settings["View"], out _viewIndex);
				return _viewIndex;
			}
		}
		public int TopN
		{
			get
			{
				int _topN = 0;
				if (Settings["TopN"] != null)
					Int32.TryParse((string)Settings["TopN"], out _topN);
				return _topN;
			}
		}
		public DateTime StartDate
		{
			get
			{
				DateTime _startDate = (DateTime)SqlDateTime.MinValue;
				if (Settings["StartDate"] != null)
				{
					string[] startDateParts = ((string)Settings["StartDate"]).Split(',');
					_startDate = new DateTime(Convert.ToInt32(startDateParts[0]), Convert.ToInt32(startDateParts[1]), Convert.ToInt32(startDateParts[2]));
				}
				return _startDate;
			}
		}
		public DateTime EndDate
		{
			get
			{
				DateTime _endDate = (DateTime)SqlDateTime.MaxValue;
				if (Settings["EndDate"] != null)
				{
					string[] endDateParts = ((string)Settings["EndDate"]).Split(',');
					_endDate = new DateTime(Convert.ToInt32(endDateParts[0]), Convert.ToInt32(endDateParts[1]), Convert.ToInt32(endDateParts[2]));
				}
				return _endDate;
			}
		}
		public BBNewsController Controller
		{
			get
			{
				if (_controller == null)
					_controller = new BBNewsController();
				return _controller;
			}
		}
		public List<NewsInfo> AllNews
		{
			get
			{
				if (ViewState["News"] != null)
					return (List<NewsInfo>)ViewState["News"];
				else
					return Controller.GetNews(PortalId, CategoryId, TopN, StartDate, EndDate,-1,-1,false,"");
					
			}
			set
			{
				ViewState["News"] = value;
			}
		}
		public NewsInfo TheNews
		{
			get
			{
				if (_theNews == null) 
				{
					if (!String.IsNullOrEmpty(Request["newsid"]))
					{
						int newsId = -1;
						Int32.TryParse(Request["newsid"], out newsId);
						_theNews = Controller.GetNews(newsId);
					}
					else
					{
						var theNews = Controller.GetNews(PortalId, CategoryId, 1, StartDate, EndDate, -1, -1, false, "");
						if (theNews.Count > 0)
							_theNews = theNews[0];
					}
				}
				return _theNews;
			}
		}
		public string Template
		{
			get
			{
				if (Settings["TemplateName"] != null)
				{
					var ctrl = LoadControl("controls\\TemplateControl.ascx") as Controls.TemplateControl;
					ctrl.Key = "News";
					return ctrl.GetTemplate((string)Settings["TemplateName"]);
				}
				
				if (Settings["Template"] != null)
				{
					return (string)Settings["Template"];
				}
				return "";
			}
		}
		
		#endregion

		#region "Event Handlers"

		protected void Page_Init(object sender, System.EventArgs e)
		{
			UserInfo user = UserController.GetCurrentUserInfo();
			if (user.IsInRole("Administrator") && IsEditable)
				MultiView1.Visible = true;

            if (Settings["NewsInRow"] != null)
		    {
		        _newsInRow = 1;
		        _rowsPerPage = 5;
                if (ViewIndex == 0 || ViewIndex == 3)
		        {
		            Int32.TryParse((string) Settings["NewsInRow"], out _newsInRow);
		            Int32.TryParse((string) Settings["RowsPerPage"], out _rowsPerPage);
		            lstNews.GroupItemCount = _newsInRow;
                    lstBsNews.GroupItemCount = _newsInRow;
                    Pager.PageSize = _newsInRow*_rowsPerPage;
                    PagerBs.PageSize = _newsInRow * _rowsPerPage;
                }
		        MultiView1.ActiveViewIndex = ViewIndex;
		        _isConfigured = true;
		    }
		    else
		    {
		        string message = Localization.GetString("Configure.Message", this.LocalResourceFile);
		        DotNetNuke.UI.Skins.Skin.AddModuleMessage(this, message + " (NewsInRow)", ModuleMessage.ModuleMessageType.YellowWarning);
		    }
		    if (Settings["TemplateName"] == null && Settings["Template"] != null )
			{
				var ctrl = LoadControl("controls\\TemplateControl.ascx") as Controls.TemplateControl;
				ctrl.Key = "News";
				ctrl.SaveTemplateFile("Module_" + ModuleId.ToString(), (string) Settings["Template"]);
				
				string message = Localization.GetString("Configure.Message", this.LocalResourceFile);
                DotNetNuke.UI.Skins.Skin.AddModuleMessage(this, message + " (Template)", ModuleMessage.ModuleMessageType.YellowWarning);
			}

		    if (Settings["CategoryID"] == null)
		    {
                string message = Localization.GetString("Configure.Message", this.LocalResourceFile);
                DotNetNuke.UI.Skins.Skin.AddModuleMessage(this, message + " (Category)", ModuleMessage.ModuleMessageType.YellowWarning);
                _isConfigured = false;
		    }
		    if (!_isConfigured)
		    {
		        Pager.Visible = false;
		        PagerBs.Visible = false;
		    }
		}
		
		protected void Page_Load(object sender, System.EventArgs e)
		{
			try
			{
				if (_isConfigured)
				{
                    if (Request["feed"] == null)
					{
						switch (ViewIndex)
						{
							case 0: // Table
								lstNews.DataSource = AllNews;
								lstNews.DataBind();
								Pager.Visible = (AllNews.Count > _rowsPerPage*_newsInRow);
								break;
                            case 3: // Bootstrap 3
                                lstBsNews.DataSource = AllNews;
                                lstBsNews.DataBind();
                                PagerBs.Visible = (AllNews.Count > _rowsPerPage * _newsInRow);
                                break;
                            case 1: // Marquee
								StringBuilder sb = new StringBuilder();
								sb.Append("<marquee class=\"bbnews-marquee\"");

								if (Settings["MarqueeDirection"] != null)
								{
									string direction = (string) Settings["MarqueeDirection"];
									switch (direction)
									{
										case "1":
											sb.Append(" direction=\"right\"");
											break;
										case "2":
											sb.Append(" direction=\"up\"");
											break;
										case "3":
											sb.Append(" direction=\"down\"");
											break;
									}
								}
								if (Settings["MarqueeScrollAmount"] != null)
									sb.Append(" scrollamount=\"" + (string) Settings["MarqueeScrollAmount"] + "\"");
								if (Settings["MarqueeScrollDelay"] != null)
									sb.Append(" scrolldelay=\"" + (string) Settings["MarqueeScrollDelay"] + "\"");
								if (Settings["MarqueeAlternate"] != null && Convert.ToBoolean(Settings["MarqueeAlternate"]))
									sb.Append(" behaviour=\"alternate\"");
								sb.Append(">");
								for (int i = 0; i < AllNews.Count; i++)
								{
									NewsInfo news = AllNews[i];

									if (news.Internal && Settings["NewsPage"] != null)
									{
										int newsTabId = Convert.ToInt32((string)Settings["NewsPage"]);
										news.Link = Globals.NavigateURL(newsTabId, "", "newsid=" + news.NewsId.ToString());
									}

									PortalSecurity ps = new PortalSecurity();
									string newstext = ProcessTokens(news, Template);
									string newstextSec = ps.InputFilter(newstext, PortalSecurity.FilterFlag.NoScripting);
									Control ctrl = ParseControl(newstextSec);

									StringBuilder cb = new StringBuilder();
									StringWriter tw = new StringWriter(cb);
									HtmlTextWriter hw = new HtmlTextWriter(tw);

									ctrl.RenderControl(hw);
									sb.Append(cb.ToString());

								}
								sb.Append("</marquee>");
								ltrMarquee.Text = sb.ToString();
								Pager.Visible = false;
						        PagerBs.Visible = false;
								break;
							case 2: // Details
								if (TheNews != null)
								{
									if (TheNews.Internal && Settings["NewsPage"] != null)
									{
										int newsTabId = Convert.ToInt32((string) Settings["NewsPage"]);
										TheNews.Link = Globals.NavigateURL(newsTabId, "", "newsid=" + TheNews.NewsId.ToString());
									}
									PortalSecurity ps = new PortalSecurity();
									string newstext = ProcessTokens(TheNews, Template);
									string newstextSec = ps.InputFilter(newstext, PortalSecurity.FilterFlag.NoScripting);
									Control ctrl = ParseControl(newstextSec);

									plhDetail.Controls.Add(ctrl);
								}
								break;
						}
					}
				}
			}

			catch (Exception exc)
			{
				//Module failed to load 
				Exceptions.ProcessModuleLoadException(this, exc);
			}
		}

		protected void Pager_PreRender(object sender, EventArgs e)
		{
			lstNews.DataSource = AllNews;
			lstNews.DataBind();
		    lstBsNews.DataSource = AllNews;
            lstBsNews.DataBind();
			Pager.Visible = (AllNews.Count > _rowsPerPage * _newsInRow);
            PagerBs.Visible = (AllNews.Count > _rowsPerPage * _newsInRow);

        }
		protected void Page_Prerender(object sender, EventArgs e)
		{
		    if (Request["feed"] != null)
		    {
		        string format = Request["feed"].ToLower().Trim();
		        SyndicationFeed feed = CreateFeed();
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
		            if (Settings["ShowTitle"] != null && Convert.ToBoolean((string) Settings["ShowTitle"]) && TheNews != null)
		            {
		                // We can set the title of our module
		                ((Label) ctl).Text = TheNews.Title;
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
		protected void lstNews_ItemCreated(object sender, ListViewItemEventArgs e)
		{
			if (_isConfigured)
			{
				ListView lv = sender as ListView;
				ListViewDataItem item = e.Item as ListViewDataItem;
				News = item.DataItem as NewsInfo;
				if (News != null)
				{
					HtmlTableCell td = e.Item.FindControl("newsCell") as HtmlTableCell;
					if (td != null)
						td.Width = Convert.ToInt32(100 / _newsInRow) + " %";
					
					PlaceHolder ph = e.Item.FindControl("newsPlaceholder") as PlaceHolder;
					
					// if newslink is empty we should generate an internal link
					if (News.Internal && Settings["NewsPage"] != null)
					{
						int newsTabId = Convert.ToInt32((string)Settings["NewsPage"]);
						News.Link = Globals.NavigateURL(newsTabId, "", "newsid="+ News.NewsId.ToString());
					}

					PortalSecurity ps = new PortalSecurity();
					string newstext = ProcessTokens(News, Template);
					string newstextSec = ps.InputFilter(newstext, PortalSecurity.FilterFlag.NoScripting);
					Control ctrl = ParseControl(newstextSec);
					ph.Controls.Add(ctrl);
				}
			}
			else
			{
				Pager.Visible = false;
                PagerBs.Visible = false;
                lstNews.Visible = false;
			}
		}

		protected void lstNews_SelectedIndexChanging(object sender, ListViewSelectEventArgs e)
		{
			LinkButton btn = sender as LinkButton;
			int newsId = (int)lstNews.DataKeys[e.NewSelectedIndex].Value;

			Response.Redirect(Globals.NavigateURL(TabId, "", "nid=" + newsId.ToString()));
		}

        protected void lstBsNews_SelectedIndexChanging(object sender, ListViewSelectEventArgs e)
        {
            LinkButton btn = sender as LinkButton;
            int newsId = (int)lstBsNews.DataKeys[e.NewSelectedIndex].Value;

            Response.Redirect(Globals.NavigateURL(TabId, "", "nid=" + newsId.ToString()));
        }

        protected void lstBsNews_ItemCreated(object sender, ListViewItemEventArgs e)
        {
            if (_isConfigured)
            {
                ListView lv = sender as ListView;
                ListViewDataItem item = e.Item as ListViewDataItem;
                News = item.DataItem as NewsInfo;
                if (News != null)
                {
                    HtmlGenericControl newsDiv = e.Item.FindControl("newsDiv") as HtmlGenericControl;

                    int cols = 12 / _newsInRow;
                    if (newsDiv != null)
                        newsDiv.Attributes["class"] = "col-md-" + cols.ToString();
                    
                    PlaceHolder ph = e.Item.FindControl("newsPlaceholder") as PlaceHolder;

                    // if newslink is empty we should generate an internal link
                    if (News.Internal && Settings["NewsPage"] != null)
                    {
                        int newsTabId = Convert.ToInt32((string)Settings["NewsPage"]);
                        News.Link = Globals.NavigateURL(newsTabId, "", "newsid=" + News.NewsId.ToString());
                    }

                    PortalSecurity ps = new PortalSecurity();
                    string newstext = ProcessTokens(News, Template);
                    string newstextSec = ps.InputFilter(newstext, PortalSecurity.FilterFlag.NoScripting);
                    Control ctrl = ParseControl(newstextSec);
                    ph.Controls.Add(ctrl);
                }
            }
            else
            {
                Pager.Visible = false;
                PagerBs.Visible = false;
                lstNews.Visible = false;
                lstBsNews.Visible = false;
            }
        }

        #endregion


        #region Helper Methods

        private Control FindControlRecursive(Control rootControl, string controlID)
		{
			if (rootControl.ID == controlID)
				return rootControl;

			foreach (Control controlToSearch in rootControl.Controls)
			{
				Control controlToReturn = FindControlRecursive(controlToSearch, controlID);
				if (controlToReturn != null)
					return controlToReturn;
			}
			return null;
		}

		private SyndicationFeed CreateFeed()
		{
			CategoryInfo category = Controller.GetCategory(CategoryId);
			if (category != null)
			{
                string feedUrl = Globals.NavigateURL(TabId, "", "feed=" + Request["feed"]);
			    string alternateUrl = Globals.NavigateURL(TabId);

                string appUrl = string.Format("{0}://{1}{2}{3}",
                                    Request.Url.Scheme,
                                    Request.Url.Host,
                                    Request.Url.Port == 80
                                        ? string.Empty
                                        : ":" + Request.Url.Port,
                                    Request.ApplicationPath);

                SyndicationFeed feed = new SyndicationFeed(category.CategoryName, category.CategoryDescription, new Uri(alternateUrl));
                // set the feed ID to the main URL of your Website
			    feed.Id = feedUrl;
                feed.BaseUri = new Uri(feedUrl);
                feed.Title = new TextSyndicationContent(category.CategoryName);
                feed.Description = new TextSyndicationContent(category.CategoryDescription);
                feed.LastUpdatedTime = new DateTimeOffset(DateTime.Now);
			    feed.Generator = "bitboxx bbnews for DNN";
                if (!string.IsNullOrEmpty(PortalSettings.LogoFile))
                    feed.ImageUrl = new Uri(appUrl + PortalSettings.HomeDirectory + PortalSettings.LogoFile);
                

                // Add the URL that will link to your published feed when it's done
                SyndicationLink link = new SyndicationLink(new Uri(feedUrl));
                link.RelationshipType = "self";
                link.MediaType = "text/html";
                link.Title = category.CategoryName;
                feed.Links.Add(link);

                List<SyndicationItem> items = new List<SyndicationItem>();
				foreach (NewsInfo news in AllNews.Take(10).OrderByDescending(n => n.Pubdate))
				{
					if (news.Internal && Settings["NewsPage"] != null)
					{
						int newsTabId = Convert.ToInt32((string)Settings["NewsPage"]);
						news.Link = Globals.NavigateURL(newsTabId, "", "newsid=" + news.NewsId.ToString());
					}

				    try
				    {
				        Uri newsLink = new Uri(news.Link);
                        SyndicationItem item = new SyndicationItem(news.Title, news.Summary,newsLink, news.GUID, news.Pubdate);
                        item.Id = news.GUID;

                        // Add the URL for the item as a link
                        link = new SyndicationLink(new Uri(news.Link));
                        item.Links.Add(link);
                        
                        // Fill some properties for the item
                        item.LastUpdatedTime = news.Pubdate;
                        item.PublishDate = news.Pubdate;

                        if (!string.IsNullOrEmpty(news.Image))
                        {
                            try
                            {
                                Uri imgUrl = new Uri(news.Image);
                                item.Links.Add(SyndicationLink.CreateMediaEnclosureLink(imgUrl, "image/" + news.Image.Substring(news.Image.LastIndexOf('.') + 1), 0));
                            }
                            catch (Exception)
                            {
                            }
                        }

                        // Fill the item content            
                        TextSyndicationContent content = new TextSyndicationContent(news.Summary, TextSyndicationContentKind.Plaintext);
                        item.Content = content;
                        items.Add(item);
                    }
                    catch (Exception)
				    {
				    }

 				}
				feed.Items = items;
				return feed;
			}
			return null;
		}



		/// <summary>
		/// Processes the tokens.
		/// </summary>
		/// <param name="faqItem">The FAQ item.</param>
		/// <param name="template">The template.</param>
		/// <returns>Answers in which all tokens are processed</returns>
		public string ProcessTokens(NewsInfo news, string template)
		{
			// For compability issues we need to convert old tokens to new tokens (sigh...)
			StringBuilder compatibleTemplate = new StringBuilder(template);
			compatibleTemplate.Replace("[TITLE]", "[BBNEWS:TITLE]");
			compatibleTemplate.Replace("[LINK]", "[BBNEWS:LINK]");
			compatibleTemplate.Replace("[TITLELINK]", "[BBNEWS:TITLELINK]");
			compatibleTemplate.Replace("[DESCRIPTION]", "[BBNEWS:DESCRIPTION]");
			compatibleTemplate.Replace("[AUTHOR]", "[BBNEWS:AUTHOR]");
			compatibleTemplate.Replace("[NEWS]", "[BBNEWS:NEWS]");
			compatibleTemplate.Replace("[IMAGE]", "[BBNEWS:IMAGE]");
			compatibleTemplate.Replace("[PUBDATE]", "[BBNEWS:PUBDATE]");
			compatibleTemplate.Replace("[SOURCE]", "[BBNEWS:SOURCE]");

			// Now we can call TokenReplace
			BBNewsTokenReplace tokenReplace = new BBNewsTokenReplace(news);
			return tokenReplace.ReplaceBBNewsTokens(template);
		}

		#endregion

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