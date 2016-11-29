#region copyright

// 
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
using System.Data;
using System.Data.SqlTypes;
using System.IO;
using System.Linq;
using System.Net;
using System.ServiceModel.Syndication;
using System.Text;
using System.Xml;

using Bitboxx.DNNModules.BBNews.Components;
using Bitboxx.DNNModules.BBNews.Models;
using DotNetNuke.Common;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Data;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Services.Log.EventLog;
using DotNetNuke.Services.Search;


namespace Bitboxx.DNNModules.BBNews
{

	/// ----------------------------------------------------------------------------- 
	/// <summary> 
	/// The Controller class for BBNews 
	/// </summary> 
	/// <remarks> 
	/// </remarks> 
	/// <history> 
	/// </history> 
	/// ----------------------------------------------------------------------------- 

	// [DNNtc.UpgradeEventMessage("01.01.01,04.00.02,04.01.00")]
	[DNNtc.BusinessControllerClass()]
	// public class BBNewsController : ISearchable, IPortable
	public class BBNewsController //: PortalModuleBase , ISearchable, IPortable
    {

		#region "Public Helper Methods"
		
		public void ReadFeed(int FeedId)
		{
			FeedInfo feedInfo = DbController.Instance.GetFeed(FeedId);
			string ProxyServer = DotNetNuke.Entities.Host.Host.ProxyServer;
			string ProxyPort = DotNetNuke.Entities.Host.Host.ProxyPort.ToString();
			string ProxyUserName = DotNetNuke.Entities.Host.Host.ProxyUsername;
			string ProxyPassword = DotNetNuke.Entities.Host.Host.ProxyPassword;
			HttpWebRequest wrq;
			WebResponse wrp;
			Stream rssStream;
			XmlReaderSettings settings;
			XmlReader rssReader;
			SyndicationFeed feed;

            string oAuthToken = DotNetNuke.Entities.Portals.PortalController.GetPortalSetting("BB_TwitterToken", feedInfo.PortalId, "");
            string oAuthTokenSecret = DotNetNuke.Entities.Portals.PortalController.GetPortalSetting("BB_TwitterTokenSecret", feedInfo.PortalId, "");
            string oAuthConsumerKey = DotNetNuke.Entities.Portals.PortalController.GetPortalSetting("BB_TwitterConsumerKey", feedInfo.PortalId, "");
            string oAuthConsumerSecret = DotNetNuke.Entities.Portals.PortalController.GetPortalSetting("BB_TwitterConsumerSecret", feedInfo.PortalId, "");
            
			try
			{
				switch (feedInfo.FeedType)
				{
					case 0: // None
						return;
					case 1: // Twitter Search
						
                        TwitterApi11 twitterApiSearch = new TwitterApi11(oAuthToken, oAuthTokenSecret, oAuthConsumerKey, oAuthConsumerSecret);
                        List<NewsInfo> newsListSearch = twitterApiSearch.SearchTweets(feedInfo.FeedUrl, 20);
                        foreach (NewsInfo news in newsListSearch)
                        {
                            news.FeedID = FeedId;
                            DbController.Instance.SaveNewsByGuid(news);
                        }
                        feedInfo.LastRetrieve = DateTime.Now;
                        DbController.Instance.SaveFeed(feedInfo);
                        break;

                    case 3: // Twitter Timeline

                        TwitterApi11 twitterApiUser = new TwitterApi11(oAuthToken, oAuthTokenSecret, oAuthConsumerKey, oAuthConsumerSecret);
                        List<NewsInfo> newsListUser = twitterApiUser.GetUserTimeLine(feedInfo.FeedUrl, 20);
                        foreach (NewsInfo news in newsListUser)
                        {
                            news.FeedID = FeedId;
                            DbController.Instance.SaveNewsByGuid(news);
                        }
                        feedInfo.LastRetrieve = DateTime.Now;
                        DbController.Instance.SaveFeed(feedInfo);
                        break;

					case 2: // RSS
						Uri baseUrl = new Uri(feedInfo.FeedUrl);
						try
						{
							wrq = (HttpWebRequest)WebRequest.Create(feedInfo.FeedUrl);

							if (ProxyServer != string.Empty)
								wrq.Proxy = new WebProxy(ProxyServer + (ProxyPort != "-1" ? ":" + ProxyPort : ""));

							if (ProxyUserName != string.Empty)
								wrq.Proxy.Credentials = new NetworkCredential(ProxyUserName, ProxyPassword);


							// Set UserAgent to avoid prohibited (403) answer
							wrq.UserAgent = "Mozilla/5.0 (Windows NT 6.2; WOW64; rv:15.0) Gecko/20100101 Firefox/15.0.1";

							wrp = wrq.GetResponse();
							rssStream = wrp.GetResponseStream();

							settings = new XmlReaderSettings();
							settings.ProhibitDtd = false;

							rssReader = XmlReader.Create(rssStream, settings);
							feed = SyndicationFeed.Load(rssReader);
						}
						catch (Exception ex)
						{
							// zweiter Versuch
							string xml = string.Empty;
							WebClient c = new WebClient();
							Stream s = c.OpenRead(feedInfo.FeedUrl);
							try
							{
								int n, bloc = 65535;
								byte[] bytes = new byte[bloc];
								do
								{
									try
									{
										n = s.Read(bytes, 0, bloc);
									}
									catch (IOException)
									{
										break;
									}

									// The end of the file is reached.
									if (n == 0)
										break; 
									xml += Encoding.UTF8.GetString(bytes, 0, n);
								} while (true);
							}
							finally
							{
								s.Close();
							}

				            // if xml is not valid lets try to repair
                            try
						    {
                                try
                                {
                                    rssReader = System.Xml.XmlReader.Create(new StringReader(xml));
                                    feed = SyndicationFeed.Load(rssReader);
                                }
                                catch (Exception)
                                {
                                    xml = xml.Replace("&nbsp;", "&amp;nbsp;")
                                        .Replace("&lt;", "&amp;lt;")
                                        .Replace("&gt;", "&amp;gt;")
                                        .Replace("&ndash;", "&amp;ndash;")
                                        .Replace("&ldquo;", "&amp;ldquo;")
                                        .Replace("&rdquo;", "&amp;rdquo;")
                                        .Replace("&rsquo;", "&amp;rsquo;");

                                    MemoryStream stream = new MemoryStream();
                                    StreamWriter writer = new StreamWriter(stream);
                                    writer.Write(xml);
                                    writer.Flush();
                                    stream.Position = 0;

                                    rssReader = new RssXmlReader(stream);
                                    feed = SyndicationFeed.Load(rssReader);
                                }
						    }
						    catch (Exception)
						    {
						        xml = AddCDATA(xml);
                                rssReader = System.Xml.XmlReader.Create(new StringReader(xml));
                                feed = SyndicationFeed.Load(rssReader);
						    }
    					}
						

						foreach (var feedItem in feed.Items)
						{
							NewsInfo news = new NewsInfo();
							news.News = "";
							news.Internal = false;
							news.Title = (feedItem.Title != null ? feedItem.Title.Text : "");
							news.Summary = (feedItem.Summary != null ? feedItem.Summary.Text : "");
							//if (feedInfo.StripHtml)
							//    news.Summary = StripHTML(news.Summary);
							news.Link = (feedItem.Links.Count > 0 ? feedItem.Links[0].Uri.OriginalString : "");
							if (!news.Link.ToLower().StartsWith("http"))
								news.Link = baseUrl.Scheme + "://" + baseUrl.Host + news.Link;
							news.Author = "";
							if (feedItem.Authors.Count > 0)
							{
								string name = feedItem.Authors[0].Name ?? " ";
								string uri = feedItem.Authors[0].Uri ?? " ";
								string email = feedItem.Authors[0].Email ?? " ";
								string nick = "";
								news.Author = name + "|" + uri + "|" + email + "|" + nick;
							}
							
							news.Image = (feedItem.Links.Count > 1 ? feedItem.Links[1].Uri.OriginalString : "");

							DateTime pubDate = (feedItem.PublishDate.LocalDateTime != DateTime.MinValue ? feedItem.PublishDate.LocalDateTime : (DateTime)SqlDateTime.MinValue);
							DateTime lastUpdated = (feedItem.LastUpdatedTime.LocalDateTime != DateTime.MinValue ? feedItem.LastUpdatedTime.LocalDateTime : (DateTime)SqlDateTime.MinValue);
							news.Pubdate = (pubDate > lastUpdated ? pubDate : lastUpdated);
							news.Pubdate = (news.Pubdate <= (DateTime)SqlDateTime.MinValue ? DateTime.Now : news.Pubdate);
							news.Internal = false;
						    news.MetaData = new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(feedItem);


                            if (feedItem.Id != null)
								news.GUID = feedItem.Id;
							else
								news.GUID = string.Format("{0:yyyyMMddHHmmss}", news.Pubdate) +
									news.Title.ToUpper().Substring(0, Math.Min(news.Title.Length, 20));
							news.FeedID = feedInfo.FeedID;

                            if (String.IsNullOrEmpty(news.News))
						        news.News = news.Summary;

                            DbController.Instance.SaveNewsByGuid(news);
						}
						feedInfo.LastRetrieve = DateTime.Now;
                        DbController.Instance.SaveFeed(feedInfo);
						break;

					
				}
			}
			catch (Exception ex)
			{
				EventLogController objEventLog = new EventLogController();
				objEventLog.AddLog("Feed Read Error (" + feedInfo.FeedID.ToString() + ":" + feedInfo.FeedUrl + ") ", ex.ToString(), PortalSettings.Current, -1, EventLogController.EventLogType.ADMIN_ALERT);
				feedInfo.LastTry = DateTime.Now;
                DbController.Instance.SaveFeed(feedInfo);
			}
		}

        public SyndicationFeed CreateFeed(int categoryId, string feedUrl, string alternateUrl, string appUrl, string newsPage)
        {
            CategoryInfo category = DbController.Instance.GetCategory(categoryId);
            if (category != null)
            {

                SyndicationFeed feed = new SyndicationFeed(category.CategoryName, category.CategoryDescription, new Uri(alternateUrl));
                // set the feed ID to the main URL of your Website
                feed.Id = feedUrl;
                feed.BaseUri = new Uri(feedUrl);
                feed.Title = new TextSyndicationContent(category.CategoryName);
                feed.Description = new TextSyndicationContent(category.CategoryDescription);
                feed.LastUpdatedTime = new DateTimeOffset(DateTime.Now);
                feed.Generator = "bitboxx bbnews for DNN";
                if (!string.IsNullOrEmpty(PortalSettings.Current.LogoFile))
                    feed.ImageUrl = new Uri(appUrl + PortalSettings.Current.HomeDirectory + PortalSettings.Current.LogoFile);


                // Add the URL that will link to your published feed when it's done
                SyndicationLink link = new SyndicationLink(new Uri(feedUrl));
                link.RelationshipType = "self";
                link.MediaType = "text/html";
                link.Title = category.CategoryName;
                feed.Links.Add(link);

                List<SyndicationItem> items = new List<SyndicationItem>();
                List<NewsInfo> catNews = DbController.Instance.GetNews(PortalSettings.Current.PortalId, categoryId, 10, new DateTime(1900, 1, 1), new DateTime(9999, 12, 31), 1, 10, false, "").ToList();
                foreach (NewsInfo news in catNews.OrderByDescending(n => n.Pubdate))
                {
                    if (news.Internal && newsPage != null)
                    {
                        int newsTabId = Convert.ToInt32(newsPage);
                        news.Link = Globals.NavigateURL(newsTabId, "", "newsid=" + news.NewsID.ToString());
                    }

                    try
                    {
                        Uri newsLink = new Uri(news.Link);
                        SyndicationItem item = new SyndicationItem(news.Title, news.Summary, newsLink, news.GUID, news.Pubdate);
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


        public string AddCDATA(string xml)
	    {
	        if (xml.IndexOf("<description>") > -1 && xml.IndexOf("<description><![CDATA[") == -1)
	        {
	            xml = xml.Replace("<description>", "<description><![CDATA[").Replace("</description>", "]]></description>");
	        }
	        return xml;
	    }

        public static string GetTemplate(string name, string key)
        {
            string templatePath = Path.Combine(Globals.ApplicationMapPath, "Desktopmodules/BBNews/Templates/" + key);
            ;
            string currentLanguage = System.Threading.Thread.CurrentThread.CurrentUICulture.Name;
            string portalLangFile = Path.Combine(templatePath, name + "." + currentLanguage + ".Portal-" + PortalSettings.Current.PortalId.ToString() + ".htm");
            string langFile = Path.Combine(templatePath, name + "." + currentLanguage + ".htm");
            string portalFile = Path.Combine(templatePath, name + ".Portal-" + PortalSettings.Current.PortalId.ToString() + ".htm");
            string neutralFile = Path.Combine(templatePath, name + ".htm");
            string defaultFile = Path.Combine(templatePath, "default.htm");

            if (File.Exists(portalLangFile))
                return File.ReadAllText(portalLangFile);
            else if (File.Exists(langFile))
                return File.ReadAllText(langFile);
            else if (File.Exists(portalFile))
                return File.ReadAllText(portalFile);
            else if (File.Exists(neutralFile))
                return File.ReadAllText(neutralFile);
            else if (File.Exists(defaultFile))
                return File.ReadAllText(defaultFile);
            return "";
        }


        #endregion

        #region "Optional Interfaces"

        /// ----------------------------------------------------------------------------- 
        /// <summary> 
        /// GetSearchItems implements the ISearchable Interface 
        /// </summary> 
        /// <remarks> 
        /// </remarks> 
        /// <param name="ModInfo">The ModuleInfo for the module to be Indexed</param> 
        /// <history> 
        /// </history> 
        /// ----------------------------------------------------------------------------- 
        public DotNetNuke.Services.Search.SearchItemInfoCollection GetSearchItems(ModuleInfo ModInfo)
		{

			SearchItemInfoCollection SearchItemCollection = new SearchItemInfoCollection();

            List<NewsInfo> colBBNewss = DbController.Instance.GetNews().ToList();
            foreach (NewsInfo objBBNews in colBBNewss)
            {
                SearchItemInfo SearchItem = new SearchItemInfo(ModInfo.ModuleTitle, objBBNews.Summary, -1, objBBNews.Pubdate, ModInfo.ModuleID, objBBNews.NewsID.ToString(), objBBNews.News, "NewsId=" + objBBNews.NewsID.ToString());
                SearchItemCollection.Add(SearchItem);
            }

            return SearchItemCollection;

		}

		/// ----------------------------------------------------------------------------- 
		/// <summary> 
		/// ExportModule implements the IPortable ExportModule Interface 
		/// </summary> 
		/// <remarks> 
		/// </remarks> 
		/// <param name="ModuleID">The Id of the module to be exported</param> 
		/// <history> 
		/// </history> 
		/// ----------------------------------------------------------------------------- 
		public string ExportModule(int ModuleID)
		{

			string strXML = "";

			//List<BBNewsInfo> colBBNewss = GetBBNewss(ModuleID);
			//if (colBBNewss.Count != 0)
			//{
			//    strXML += "<BBNewss>";
			//    foreach (BBNewsInfo objBBNews in colBBNewss)
			//    {
			//        strXML += "<BBNews>";
			//        strXML += "<content>" + XmlUtils.XMLEncode(objBBNews.Content) + "</content>";
			//        strXML += "</BBNews>";
			//    }
			//    strXML += "</BBNewss>";
			//}

			return strXML;

		}

		/// ----------------------------------------------------------------------------- 
		/// <summary> 
		/// ImportModule implements the IPortable ImportModule Interface 
		/// </summary> 
		/// <remarks> 
		/// </remarks> 
		/// <param name="ModuleID">The Id of the module to be imported</param> 
		/// <param name="Content">The content to be imported</param> 
		/// <param name="Version">The version of the module to be imported</param> 
		/// <param name="UserId">The Id of the user performing the import</param> 
		/// <history> 
		/// </history> 
		/// ----------------------------------------------------------------------------- 
		public void ImportModule(int ModuleID, string Content, string Version, int UserId)
		{

			//XmlNode xmlBBNewss = Globals.GetContent(Content, "BBNewss");
			//foreach (XmlNode xmlBBNews in xmlBBNewss.SelectNodes("BBNews"))
			//{
			//    BBNewsInfo objBBNews = new BBNewsInfo();
			//    objBBNews.ModuleId = ModuleID;
			//    objBBNews.Content = xmlBBNews.SelectSingleNode("content").InnerText;
			//    objBBNews.CreatedByUser = UserId;
			//    AddBBNews(objBBNews);
			//}

		}

		#endregion
	}
}