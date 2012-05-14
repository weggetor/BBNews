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
using System.Data;
using System.Globalization;
using System.Text.RegularExpressions;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Users;
using DotNetNuke.Services.Tokens;

namespace Bitboxx.DNNModules.BBNews.Components
{
	[Serializable]
	public class NewsInfo : IHydratable, IPropertyAccess
	{
		public NewsInfo()
		{
			NewsId = 0;
			FeedId = 0;
			Title = "";
			Summary = "";
			Author = "";
			News = "";
			Link = "";
			Image = "";
			GUID = "";
			Pubdate = DateTime.Now;
			Hide = false;
			Internal = false;
		}
		public Int32 NewsId { get; set; }
		public Int32 FeedId { get; set; }
		public string Title { get; set; }
		public string Summary { get; set; }
		public string Author { get; set; }
		public string News { get; set; }
		public string Link { get; set; }
		public string Image { get; set; }
		public string GUID { get; set; }
		public DateTime Pubdate { get; set; }
		public Boolean Hide { get; set; }
		public Boolean Internal { get; set; }

		#region Implementation of IHydratable

		public void Fill(IDataReader dr)
		{
			NewsId = Null.SetNullInteger(dr["NewsId"]);
			FeedId = Null.SetNullInteger(dr["FeedId"]);
			Title = Null.SetNullString(dr["Title"]);
			Summary = Null.SetNullString(dr["Summary"]);
			Author = Null.SetNullString(dr["Author"]);
			News = Null.SetNullString(dr["News"]);
			Link = Null.SetNullString(dr["Link"]);
			Image = Null.SetNullString(dr["Image"]);
			GUID = Null.SetNullString(dr["GUID"]);
			Pubdate = Null.SetNullDateTime(dr["Pubdate"]);
			Hide = Null.SetNullBoolean(dr["Hide"]);
			Internal = Null.SetNullBoolean(dr["Internal"]);
		}

		public int KeyID
		{
			get { return NewsId; }
			set { NewsId = value; }
		}

		#endregion

		#region Implementation of IPropertyAccess

		public string GetProperty(string propertyName, string format, CultureInfo formatProvider, UserInfo accessingUser, Scope accessLevel, ref bool propertyNotFound)
		{
			string[] authInfo;
			FeedInfo feed = new BBNewsController().GetFeed(FeedId);
			propertyNotFound = false;
			switch (propertyName.ToLower())
			{
				case "newsid":
					return NewsId.ToString(String.IsNullOrEmpty(format) ? "g" : format, formatProvider);
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
					if (!String.IsNullOrEmpty(Link))
					{
						Uri url = new Uri(Link);
						return PropertyAccess.FormatString(url.Host,format);
					}
					return "";
				case "favicon":
					if (feed.FeedUrl != String.Empty)
					{
						Uri urlfav = new Uri(feed.FeedUrl);
						return "<img src=\"http://" + urlfav.Host + 
						       "/favicon.ico\"" +
						       (String.IsNullOrEmpty(format) ? "" : " width=\"" + format + "\"") +
						       " style=\"vertical-align: middle;\" />";
					}
					return "";
				case "feedid":
					return FeedId.ToString(String.IsNullOrEmpty(format) ? "g" : format, formatProvider);
				case "feedname":
					return PropertyAccess.FormatString(feed.FeedName, format);
				case "feedurl":
					return PropertyAccess.FormatString(feed.FeedUrl, format);
				default:
					propertyNotFound = true;
					return String.Empty;
			}
		}

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

		#endregion
	}

}