using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Xml;
using Bitboxx.DNNModules.BBNews.Components;
using DotNetNuke.ComponentModel.DataAnnotations;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Users;
using DotNetNuke.Services.Tokens;
using Newtonsoft.Json.Linq;

namespace Bitboxx.DNNModules.BBNews.Models
{
    public partial class NewsInfo:IPropertyAccess
    {
        [ReadOnlyColumn]
        public string Html { get; set; }
        
        #region Implementation of IPropertyAccess

        public string GetProperty(string propertyName, string format, CultureInfo formatProvider, UserInfo accessingUser, Scope accessLevel, ref bool propertyNotFound)
        {
            string[] authInfo;
            FeedInfo feed = DbController.Instance.GetFeed(FeedID);
            propertyNotFound = false;
            switch (propertyName.ToLower())
            {
                case "newsid":
                    return NewsID.ToString(String.IsNullOrEmpty(format) ? "g" : format, formatProvider);
                case "title":
                    return (String.IsNullOrEmpty(format)) ? Title : ShortenString(Title, Convert.ToInt32(format));
                case "titlelink":
                    return "<a href=\"" + Link + "\"" +
                        (Internal == false ? " target=\"_blank\">" : ">") +
                        (String.IsNullOrEmpty(format) ? Title : ShortenString(Title, Convert.ToInt32(format))) +
                        "</a>";
                case "link":
                    return PropertyAccess.FormatString(Link, format);
                case "summary":
                    return (String.IsNullOrEmpty(format)) ? Summary : ShortenString(Summary, Convert.ToInt32(format));
                case "summarynohtml":
                    string summary = Regex.Replace(Summary, "<.*?>", string.Empty);
                    return (String.IsNullOrEmpty(format)) ? summary : ShortenString(summary, Convert.ToInt32(format));
                case "author":
                    return PropertyAccess.FormatString(Author, format);
                case "authorname":
                    authInfo = Author.Split('|');
                    if (authInfo.Length > 0)
                        return PropertyAccess.FormatString(authInfo[0], format);
                    else
                        return PropertyAccess.FormatString(Author, format);
                case "authorurl":
                    authInfo = Author.Split('|');
                    if (authInfo.Length > 1)
                        return PropertyAccess.FormatString(authInfo[1], format);
                    return String.Empty;
                case "authoremail":
                    authInfo = Author.Split('|');
                    if (authInfo.Length > 3)
                        return PropertyAccess.FormatString(authInfo[2], format);
                    return String.Empty;
                case "authornick":
                    authInfo = Author.Split('|');
                    if (authInfo.Length > 3)
                        return PropertyAccess.FormatString(authInfo[3], format);
                    return String.Empty;
                case "news":
                    return (String.IsNullOrEmpty(format)) ? News : ShortenString(News, Convert.ToInt32(format));
                case "image":
                    return "<img src=\"" + Image + "\" alt=\"" + Title + "\">";
                case "imagelink":
                    return PropertyAccess.FormatString(Image, format);
                case "pubdate":
                    return Pubdate.ToString(String.IsNullOrEmpty(format) ? "d" : format, formatProvider);
                case "source":
                    Uri url;
                    if (!String.IsNullOrEmpty(Link))
                    {
                        try
                        {
                            url = new Uri(Link);
                            return PropertyAccess.FormatString(url.Host, format);
                        }
                        catch (Exception)
                        {
                            return "";
                        }
                    }
                    else
                    {
                        url = new Uri("http://" + PortalSettings.Current.PortalAlias.HTTPAlias);
                        return PropertyAccess.FormatString(url.Host, format);
                    }
                case "favicon":
                    Uri urlFav;
                    if (feed.FeedUrl != String.Empty)
                    {
                        urlFav = new Uri(feed.FeedUrl);
                    }
                    else
                    {
                        urlFav = new Uri("http://" + PortalSettings.Current.PortalAlias.HTTPAlias);
                    }
                    return "<img src=\"http://" + urlFav.Host +
                               "/favicon.ico\"" +
                               (String.IsNullOrEmpty(format) ? "" : " width=\"" + format + "\"") +
                               " style=\"vertical-align: middle;\" />";

                case "feedid":
                    return FeedID.ToString(String.IsNullOrEmpty(format) ? "g" : format, formatProvider);
                case "feedname":
                    return PropertyAccess.FormatString(feed.FeedName, format);
                case "feedurl":
                    return PropertyAccess.FormatString(feed.FeedUrl, format);
                case "meta":
                    return GetMetaData(format);
                default:
                    propertyNotFound = true;
                    return String.Empty;
            }
        }

        [ReadOnlyColumn]
        public CacheLevel Cacheability
        {
            get { return CacheLevel.fullyCacheable; }
        }

        #endregion

        #region private Helper methods

        private string ShortenString(string FullString, int MaxLength)
        {
            if (FullString.Length <= MaxLength)
                return FullString;
            string shortenString = FullString.Substring(0, MaxLength);
            while (shortenString.EndsWith(" ") == false && shortenString.Length > 0)
            {
                shortenString = shortenString.Substring(0, shortenString.Length - 1);
            }
            return shortenString.Trim() + " ...";
        }

        private string GetMetaData(string key)
        {
            try
            {
                var jsonData = JObject.Parse(MetaData);
                return jsonData.SelectToken(key, false).ToString();
            }
            catch (Exception)
            {
                try
                {
                    XmlDocument doc = new XmlDocument();
                    doc.LoadXml(MetaData);
                    XmlNode node = doc.SelectSingleNode("/root/row[translate(key, 'ABCDEFGHIJKLMNOPQRSTUVWXYZ', 'abcdefghijklmnopqrstuvwxyz')='" + key.ToLower() + "']/val");
                    if (node != null)
                        return node.InnerText;
                }
                catch (Exception)
                {
                }
            }
            return "";
        }

        #endregion
    }
}