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


using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;
using System.Xml.Linq;
using System.Net;
using System.IO;

namespace Bitboxx.DNNModules.BBNews.Components
{
    public class TwitterApi11
    {
        public string TokenKey { get; set; }
        public string TokenSecret { get; set; }
        public string ConsumerKey { get; set; }
        public string ConsumerSecret { get; set; }

        private class User
        {
            public string name { get; set; }
            public string screen_name { get; set; }
            public string profile_image_url { get; set; }
            public string url { get; set; }
        }

        private class Tweet
        {
            public string created_at { get; set; }
            public string id_str { get; set; }
            public string text { get; set; }
            public User user { get; set; }
        }

        private class Tweets
        {
            public List<Tweet> statuses { get; set; }
        }

        public TwitterApi11 (string tokenKey, string tokenSecret, string consumerKey, string consumerSecret)
        {
            TokenKey = tokenKey;
            TokenSecret = tokenSecret;
            ConsumerKey = consumerKey;
            ConsumerSecret = consumerSecret;
        }

        public List<NewsInfo> GetUserTimeLine(string twitterUsername, int tweetsCount)
        {
            // Other OAuth connection/authentication variables
            string oAuthVersion = "1.0";
            string oAuthSignatureMethod = "HMAC-SHA1";
            string oAuthNonce = Convert.ToBase64String(new ASCIIEncoding().GetBytes(DateTime.Now.Ticks.ToString()));
            TimeSpan timeSpan = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            string oAuthTimestamp = Convert.ToInt64(timeSpan.TotalSeconds).ToString();
            string resourceUrl = "https://api.twitter.com/1.1/statuses/user_timeline.json";

            // Generate OAuth signature. Note that Twitter is very particular about the format of this string. Even reordering the variables
            // will cause authentication errors.

            var baseFormat = "count={7}&oauth_consumer_key={0}&oauth_nonce={1}&oauth_signature_method={2}" +
                             "&oauth_timestamp={3}&oauth_token={4}&oauth_version={5}&screen_name={6}";

            var baseString = string.Format(baseFormat,
                ConsumerKey,
                oAuthNonce,
                oAuthSignatureMethod,
                oAuthTimestamp,
                TokenKey,
                oAuthVersion,
                Uri.EscapeDataString(twitterUsername),
                Uri.EscapeDataString(tweetsCount.ToString())
                );

            baseString = string.Concat("GET&", Uri.EscapeDataString(resourceUrl), "&", Uri.EscapeDataString(baseString));

            // Generate an OAuth signature using the baseString
            var compositeKey = string.Concat(Uri.EscapeDataString(ConsumerSecret), "&", Uri.EscapeDataString(TokenSecret));
            string oAuthSignature;
            using (HMACSHA1 hasher = new HMACSHA1(ASCIIEncoding.ASCII.GetBytes(compositeKey)))
            {
                oAuthSignature = Convert.ToBase64String(hasher.ComputeHash(ASCIIEncoding.ASCII.GetBytes(baseString)));
            }

            // Now build the Authentication header. Again, Twitter is very particular about the format. Do not reorder variables.
            var headerFormat = "OAuth oauth_nonce=\"{0}\", oauth_signature_method=\"{1}\", " +
                               "oauth_timestamp=\"{2}\", oauth_consumer_key=\"{3}\", " +
                               "oauth_token=\"{4}\", oauth_signature=\"{5}\", " +
                               "oauth_version=\"{6}\"";

            var authHeader = string.Format(headerFormat,
                Uri.EscapeDataString(oAuthNonce),
                Uri.EscapeDataString(oAuthSignatureMethod),
                Uri.EscapeDataString(oAuthTimestamp),
                Uri.EscapeDataString(ConsumerKey),
                Uri.EscapeDataString(TokenKey),
                Uri.EscapeDataString(oAuthSignature),
                Uri.EscapeDataString(oAuthVersion)
                );

            // Now build the actual request

            ServicePointManager.Expect100Continue = false;
            var postBody = string.Format("screen_name={0}&count={1}", Uri.EscapeDataString(twitterUsername), Uri.EscapeDataString(tweetsCount.ToString()));
            resourceUrl += "?" + postBody;
            HttpWebRequest request = (HttpWebRequest) WebRequest.Create(resourceUrl);
            request.Headers.Add("Authorization", authHeader);
            request.Method = "GET";
            request.ContentType = "application/x-www-form-urlencoded";

            // Retrieve the response data and deserialize the JSON data to a list of Tweet objects
            WebResponse response = request.GetResponse();
            string responseData = new StreamReader(response.GetResponseStream()).ReadToEnd();

            List<dynamic> rawTweets = new JavaScriptSerializer().Deserialize<List<dynamic>>(responseData);
            List<Tweet> userTweets = new JavaScriptSerializer().Deserialize<List<Tweet>>(responseData);

            return TweetsToNews(userTweets, rawTweets);
        }

        public List<NewsInfo> SearchTweets(string searchTerm, int tweetsCount)
        {
            // Other OAuth connection/authentication variables
            string oAuthVersion = "1.0";
            string oAuthSignatureMethod = "HMAC-SHA1";
            string oAuthNonce = Convert.ToBase64String(new ASCIIEncoding().GetBytes(DateTime.Now.Ticks.ToString()));
            TimeSpan timeSpan = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            string oAuthTimestamp = Convert.ToInt64(timeSpan.TotalSeconds).ToString();
            string resourceUrl = "https://api.twitter.com/1.1/search/tweets.json";

            // Generate OAuth signature. Note that Twitter is very particular about the format of this string. Even reordering the variables
            // will cause authentication errors.

            var baseFormat = "count={7}&oauth_consumer_key={0}&oauth_nonce={1}&oauth_signature_method={2}" +
                             "&oauth_timestamp={3}&oauth_token={4}&oauth_version={5}&q={6}";

            var baseString = string.Format(baseFormat,
                ConsumerKey,
                oAuthNonce,
                oAuthSignatureMethod,
                oAuthTimestamp,
                TokenKey,
                oAuthVersion,
                Uri.EscapeDataString(searchTerm),
                Uri.EscapeDataString(tweetsCount.ToString())
                );

            baseString = string.Concat("GET&", Uri.EscapeDataString(resourceUrl), "&", Uri.EscapeDataString(baseString));

            // Generate an OAuth signature using the baseString
            var compositeKey = string.Concat(Uri.EscapeDataString(ConsumerSecret), "&", Uri.EscapeDataString(TokenSecret));
            string oAuthSignature;
            using (HMACSHA1 hasher = new HMACSHA1(ASCIIEncoding.ASCII.GetBytes(compositeKey)))
            {
                oAuthSignature = Convert.ToBase64String(hasher.ComputeHash(ASCIIEncoding.ASCII.GetBytes(baseString)));
            }

            // Now build the Authentication header. Again, Twitter is very particular about the format. Do not reorder variables.
            var headerFormat = "OAuth oauth_nonce=\"{0}\", oauth_signature_method=\"{1}\", " +
                               "oauth_timestamp=\"{2}\", oauth_consumer_key=\"{3}\", " +
                               "oauth_token=\"{4}\", oauth_signature=\"{5}\", " +
                               "oauth_version=\"{6}\"";

            var authHeader = string.Format(headerFormat,
                Uri.EscapeDataString(oAuthNonce),
                Uri.EscapeDataString(oAuthSignatureMethod),
                Uri.EscapeDataString(oAuthTimestamp),
                Uri.EscapeDataString(ConsumerKey),
                Uri.EscapeDataString(TokenKey),
                Uri.EscapeDataString(oAuthSignature),
                Uri.EscapeDataString(oAuthVersion)
                );

            // Now build the actual request

            ServicePointManager.Expect100Continue = false;
            var postBody = string.Format("q={0}&count={1}", Uri.EscapeDataString(searchTerm), Uri.EscapeDataString(tweetsCount.ToString()));
            resourceUrl += "?" + postBody;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(resourceUrl);
            request.Headers.Add("Authorization", authHeader);
            request.Method = "GET";
            request.ContentType = "application/x-www-form-urlencoded";

            // Retrieve the response data and deserialize the JSON data to a list of Tweet objects
            WebResponse response = request.GetResponse();
            string responseData = new StreamReader(response.GetResponseStream()).ReadToEnd();

            List<dynamic> rawTweets = new List<dynamic>(new JavaScriptSerializer().Deserialize<dynamic>(responseData)["statuses"]);
            Tweets userTweets = new JavaScriptSerializer().Deserialize<Tweets>(responseData);

            return TweetsToNews(userTweets.statuses,rawTweets);
        }

        private List<NewsInfo> TweetsToNews(List<Tweet> tweets, List<dynamic> rawTweets )
        {
            List<NewsInfo> news = new List<NewsInfo>();
            int j = 0;
            foreach (Tweet userTweet in tweets)
            {
                NewsInfo tweet = new NewsInfo();
                tweet.Author = userTweet.user.name + "|" + userTweet.user.url + "||" + userTweet.user.screen_name;
                tweet.GUID = userTweet.id_str;
                tweet.Image = userTweet.user.profile_image_url;
                tweet.Summary = "";
                tweet.Link = "https://twitter.com/" + userTweet.user.screen_name;
                tweet.Pubdate = ParseDateTime(userTweet.created_at);
                tweet.News = HttpUtility.HtmlDecode(userTweet.text);
                tweet.Internal = false;
                tweet.Hide = false;
                tweet.Title = HttpUtility.HtmlDecode(userTweet.text);
                tweet.MetaData = new JavaScriptSerializer().Serialize(rawTweets[j]);
                string title = tweet.Title;

                List<string> urls = new List<string>();

                int i = 0;
                while (title.IndexOf("http://t.co") > -1 && i < 100)
                {
                    i++;
                    int pos = title.IndexOf("http://t.co");
                    string url = title.Substring(pos);
                    int posEnde = url.IndexOf(" ");
                    if (posEnde > -1)
                        url =  url.Substring(0, posEnde).Trim();
                    urls.Add(url);
                    title = title.Replace(url, "");
                }

                i = 0;
                while (title.IndexOf("https://t.co") > -1 && i < 100)
                {
                    i++;
                    int pos = title.IndexOf("https://t.co");
                    string url = title.Substring(pos);
                    int posEnde = url.IndexOf(" ");
                    if (posEnde > -1)
                        url = url.Substring(0, posEnde).Trim();
                    urls.Add(url);
                    title = title.Replace(url, "");
                }
                if (urls.Any())
                    tweet.Link = urls[0];

                foreach (string url in urls)
                {
                    if (!url.EndsWith("…"))
                        tweet.News = tweet.News.Replace(url, "<a href=\"" + url + "\">" + url + "</a>");
                }

                news.Add(tweet);
                j++;
            }
            return (from l in news where l.News != String.Empty orderby l.Pubdate descending select l).ToList();
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
