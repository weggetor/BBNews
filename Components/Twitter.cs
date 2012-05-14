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
using System.Linq;
using System.Web;
using System.Xml.Linq;
using System.Net;
using System.IO;


// http://search.twitter.com/search.atom?q=dnn&rpp=5 mal checken !

namespace Bitboxx.DNNModules.BBNews.Components
{
    public class Twitter
    {
        public string Username { get; set; }
        public string Password { get; set; }
        
        public Twitter (string username, string password)
        {
            Username = username;
            Password = password;
        }

        
        private string PerformRequest(string method, string url)
        {
            if (Username == string.Empty || Password == string.Empty)
                throw new Exception("Credentials needed to resolve information!");

            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
            request.Method = method;
            request.Credentials = new NetworkCredential(Username, Password);
            WebResponse response = request.GetResponse();
            StreamReader reader = new StreamReader(response.GetResponseStream());
            string responseString = reader.ReadToEnd();
            reader.Close();
            return responseString;
        }

        private string Post(string url)
        {
            return PerformRequest("POST", url);
        }

        private string Get(string url)
        {
            return PerformRequest("GET", url);
        }
        
        public List<NewsInfo> GetUserTimeLine(string user)
        {

            string url = string.Format("http://twitter.com/statuses/user_timeline/{0}.xml", user);
            string response = Get(url);

            XDocument document = XDocument.Parse(response, LoadOptions.None);

            var query = from e in document.Root.Descendants("status")
                        select new NewsInfo
                       {
                            Author = e.Element("user").Element("name").Value,
							GUID = e.Element("id").Value,
							Image = e.Element("user").Element("profile_image_url").Value,
							Summary = "",
							Link = e.Element("user").Element("url").Value,
							Pubdate = ParseDateTime(e.Element("created_at").Value),
							News = HttpUtility.HtmlDecode(e.Element("text").Value),
							Internal = false,
							Hide = false,
							Title = HttpUtility.HtmlDecode(e.Element("text").Value)
                        };

            List<NewsInfo> news = (from n in query
                                      where n.News != ""
                                      orderby n.Pubdate descending
                                      select n).ToList();
			foreach (NewsInfo item in news)
			{
				int pos = item.Title.IndexOf("http://bit.ly");
				if (pos > 0)
				{
					item.Link = item.Title.Substring(pos);
					item.Title = item.Title.Substring(0, pos - 1);
				}
			}
			
			return news;
        }

        private DateTime ParseDateTime(string date)
        {
            string dayOfWeek = date.Substring(0, 3).Trim();
            string month = date.Substring(4, 3).Trim();
            string dayInMonth = date.Substring(8, 2).Trim();
            string time = date.Substring(11, 9).Trim();
            string offset = date.Substring(20, 5).Trim();
            string year = date.Substring(25, 5).Trim();
            string dateTime = string.Format("{0}-{1}-{2} {3}", dayInMonth, month, year, time);
            DateTime ret = DateTime.Parse(dateTime);
            return ret;
        }
    }
}
