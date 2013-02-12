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
using System.Data.SqlTypes;
using System.IO;
using System.Net;
using System.ServiceModel.Syndication;
using System.Text;
using System.Xml;

using Bitboxx.DNNModules.BBNews.Components;

using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Modules;
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
	public class BBNewsController : PortalModuleBase //, ISearchable, IPortable
	{

		#region "Public Data Methods"

		public List<NewsInfo> GetNews(int PortalId, int CategoryId, int TopN, DateTime StartDate, DateTime EndDate, int pageNum, int pageSize, bool includeHidden, string search)
		{
			return CBO.FillCollection<NewsInfo>(DataProvider.Instance().GetNews(PortalId, CategoryId, TopN, StartDate, EndDate, pageNum, pageSize,includeHidden,search));
		}
		public int GetNewsCount(int PortalID, int CategoryID, int TopN, DateTime StartDate, DateTime EndDate, bool includeHidden, String search)
		{
			return DataProvider.Instance().GetNewsCount(PortalID, CategoryID, TopN, StartDate, EndDate, includeHidden, search);
		}
		public NewsInfo GetNews(int NewsId)
		{
			return CBO.FillObject<NewsInfo>(DataProvider.Instance().GetNews(NewsId));
		}
		public List<NewsInfo> GetNews()
		{
			return CBO.FillCollection<NewsInfo>(DataProvider.Instance().GetNews());
		}
		public void ReorgNews(int FeedId)
		{
			DataProvider.Instance().ReorgNews(FeedId);
		}
		public void SaveNewsByGuid(NewsInfo News)
		{
			DataProvider.Instance().SaveNewsByGuid(News);
		}
		public void SaveNewsById(NewsInfo News)
		{
			DataProvider.Instance().SaveNewsById(News);
		}
		public void DeleteNews(int NewsId)
		{
			DataProvider.Instance().DeleteNews(NewsId);
		}
		public CategoryInfo GetCategory(int categoryId)
		{
			return CBO.FillObject<CategoryInfo>(DataProvider.Instance().GetCategory(categoryId));
		}
		public List<CategoryInfo> GetCategories()
		{
			return CBO.FillCollection<CategoryInfo>(DataProvider.Instance().GetCategories());
		}
		public List<CategoryInfo> GetCategories(int PortalId)
		{
			return CBO.FillCollection<CategoryInfo>(DataProvider.Instance().GetCategories(PortalId));
		}
		public void SaveCategory(CategoryInfo Category)
		{
			DataProvider.Instance().SaveCategory(Category);
		}
		public void DeleteCategory(int CategoryId)
		{
			DataProvider.Instance().DeleteCategory(CategoryId);
		}

		public FeedInfo GetFeed(int FeedId)
		{
			return CBO.FillObject<FeedInfo>(DataProvider.Instance().GetFeed(FeedId));
		}
		public List<FeedInfo> GetFeeds()
		{
			return CBO.FillCollection<FeedInfo>(DataProvider.Instance().GetFeeds());
		}
		public List<FeedInfo> GetFeeds(int portalId)
		{
			return CBO.FillCollection<FeedInfo>(DataProvider.Instance().GetFeeds(portalId));
		}
		public void SaveFeed(FeedInfo Feed)
		{
			DataProvider.Instance().SaveFeed(Feed);
		}
		public void DeleteFeed(int FeedId)
		{
			DataProvider.Instance().DeleteFeed(FeedId);
		}
		public List<FeedInfo>  GetCategoryFeeds(int categoryId)
		{
			return CBO.FillCollection<FeedInfo>(DataProvider.Instance().GetCategoryFeeds(categoryId));
		}
		public void AddCategoryFeed(int categoryId, int feedId)
		{
			DataProvider.Instance().AddCategoryFeed(categoryId, feedId);
		}
		public void RemoveCategoryFeed(int categoryId, int feedId)
		{
			DataProvider.Instance().RemoveCategoryFeed(categoryId, feedId);
		}
		#endregion

		#region "Public Helper Methods"
		
		public void ReadFeed(int FeedId)
		{
			FeedInfo feedInfo = this.GetFeed(FeedId);
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

			try
			{
				switch (feedInfo.FeedType)
				{
					case 0: // None
						return;
					case 1: // Twitter Search
						string url = String.Format("http://search.twitter.com/search.atom?q={0}", feedInfo.FeedUrl);

						wrq = (HttpWebRequest)WebRequest.Create(url);
						if (ProxyServer != string.Empty)
							wrq.Proxy = new WebProxy(ProxyServer + (ProxyPort != "-1" ? ":" + ProxyPort : ""));

						if (ProxyUserName != string.Empty)
							wrq.Proxy.Credentials = new NetworkCredential(ProxyUserName, ProxyPassword);

						// Set UserAgent to avoid prohibited (403) answer
						wrq.UserAgent = "Mozilla/5.0 (Windows NT 6.2; WOW64; rv:15.0) Gecko/20100101 Firefox/15.0.1";

						wrp = wrq.GetResponse();
						rssStream = wrp.GetResponseStream();

						settings = new XmlReaderSettings();

						rssReader = XmlReader.Create(rssStream,settings);

						feed = SyndicationFeed.Load(rssReader);

						foreach (var feedItem in feed.Items)
						{
							NewsInfo news = new NewsInfo();
							news.News = "";
							news.Internal = false;
							news.Title = (feedItem.Title != null ? feedItem.Title.Text : "");
							news.Summary = feedItem.Content != null ? ((TextSyndicationContent)feedItem.Content).Text : "";
							//if (feedInfo.StripHtml)
							//    news.Summary = StripHTML(news.Summary);
							news.Link = (feedItem.Links.Count > 0 ? feedItem.Links[0].Uri.OriginalString : "");
							news.Author = "";
							if (feedItem.Authors.Count > 0)
							{
								string name = feedItem.Authors[0].Name ?? " ";
								string nick = "";
								if (name.IndexOf('(') > -1)
								{
									nick = name.Substring(0, name.IndexOf('(') - 1);
									name = name.Substring(name.IndexOf('(') + 1);
									name = name.Substring(0, name.Length - 1);
								}
								string uri = feedItem.Authors[0].Uri ?? " ";
								string email = feedItem.Authors[0].Email ?? " ";
							
								news.Author = name + "|" + uri + "|" + email + "|" + nick;
							}

							news.Image = (feedItem.Links.Count > 1 ? feedItem.Links[1].Uri.OriginalString : "");

							DateTime pubDate = (feedItem.PublishDate.LocalDateTime != DateTime.MinValue ? feedItem.PublishDate.LocalDateTime : (DateTime)SqlDateTime.MinValue);
							DateTime lastUpdated = (feedItem.LastUpdatedTime.LocalDateTime != DateTime.MinValue ? feedItem.LastUpdatedTime.LocalDateTime : (DateTime)SqlDateTime.MinValue);
							news.Pubdate = (pubDate > lastUpdated ? pubDate : lastUpdated);
							news.Pubdate = (news.Pubdate < (DateTime)SqlDateTime.MinValue ? (DateTime)SqlDateTime.MinValue : news.Pubdate);

							if (feedItem.Id != null)
								news.GUID = feedItem.Id;
							else
								news.GUID = string.Format("{0:yyyyMMddHHmmss}", news.Pubdate) +
									news.Title.ToUpper().Substring(0, Math.Min(news.Title.Length, 20));
							news.FeedId = feedInfo.FeedId;
							this.SaveNewsByGuid(news);
						}
						feedInfo.LastRetrieve = DateTime.Now;
						this.SaveFeed(feedInfo);
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

							rssReader = XmlReader.Create(rssStream, settings);
							feed = SyndicationFeed.Load(rssReader);
						}
						catch (IOException)
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
									catch (IOException iox)
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

							rssReader = System.Xml.XmlReader.Create(new StringReader(xml));
							feed = SyndicationFeed.Load(rssReader);
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
							news.Pubdate = (news.Pubdate < (DateTime)SqlDateTime.MinValue ? (DateTime)SqlDateTime.MinValue : news.Pubdate);
							news.Internal = false;

							if (feedItem.Id != null)
								news.GUID = feedItem.Id;
							else
								news.GUID = string.Format("{0:yyyyMMddHHmmss}", news.Pubdate) +
									news.Title.ToUpper().Substring(0, Math.Min(news.Title.Length, 20));
							news.FeedId = feedInfo.FeedId;
							this.SaveNewsByGuid(news);
						}
						feedInfo.LastRetrieve = DateTime.Now;
						this.SaveFeed(feedInfo);
						break;

					case 3: 
						break;
				}
			}
			catch (Exception ex)
			{
				EventLogController objEventLog = new EventLogController();
				objEventLog.AddLog("Feed Read Error (" + feedInfo.FeedId.ToString() + ":" + feedInfo.FeedUrl + ") ", ex.ToString(), PortalSettings, -1, EventLogController.EventLogType.ADMIN_ALERT);
				feedInfo.LastTry = DateTime.Now;
				this.SaveFeed(feedInfo);
			}
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

			//List<BBNewsInfo> colBBNewss = GetBBNewss(ModInfo.ModuleID);
			//foreach (BBNewsInfo objBBNews in colBBNewss)
			//{
			//    SearchItemInfo SearchItem = new SearchItemInfo(ModInfo.ModuleTitle, objBBNews.Content, objBBNews.CreatedByUser, objBBNews.CreatedDate, ModInfo.ModuleID, objBBNews.ItemId.ToString(), objBBNews.Content, "ItemId=" + objBBNews.ItemId.ToString());
			//    SearchItemCollection.Add(SearchItem);
			//}

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