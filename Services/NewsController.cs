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

using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using Bitboxx.DNNModules.BBNews.Components;
using Bitboxx.DNNModules.BBNews.Models;
using DotNetNuke.Common;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Security;
using DotNetNuke.Web.Api;

namespace Bitboxx.DNNModules.BBNews.Services
{
    [SupportedModules("Bitboxx.BBNews")]
    public class NewsController : DnnApiController
    {
        // <summary>
        // API that loads all news
        // </summary>
        // <returns></returns>
        [HttpGet]
        [ValidateAntiForgeryToken]
        [ActionName("list")]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.View)]
        public HttpResponseMessage GetNews(int portalId, int categoryId, int topN, DateTime startDate, DateTime endDate, int pageNum, int pageSize, bool includeHidden, string search)
        {
            try
            {
                var moduleInfo = new ModuleController().GetModule(ActiveModule.ModuleID, ActiveModule.TabID);
                var moduleSettings = moduleInfo.TabModuleSettings;
                string templateName = (string)moduleSettings["TemplateName"];
                string template = BBNewsController.GetTemplate(templateName, "News");

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
                template = compatibleTemplate.ToString();

                var allNews = DbController.Instance.GetNews(portalId, categoryId, topN, startDate, endDate, pageNum, pageSize, includeHidden, search);
                int tabId = Convert.ToInt32(ActiveModule.TabModuleSettings["NewsPage"]);
                if (tabId == 0)
                    tabId = ActiveModule.TabID;
                foreach (NewsInfo news in allNews)
                {
                    BBNewsTokenReplace tokenReplace = new BBNewsTokenReplace(news);
                    if (news.Internal)
                    {
                        if (tabId == ActiveModule.TabID)
                            news.Link += "#/newsid/" + news.NewsID.ToString();
                        else
                            news.Link = Globals.NavigateURL(tabId, "", "#/newsId=" + news.NewsID.ToString());
                    }
                    news.Html = tokenReplace.ReplaceBBNewsTokens(template);
                }
                int newsCount = DbController.Instance.GetNewsCount(portalId, categoryId, topN, startDate, endDate, includeHidden, search);

                NewsDto result = new NewsDto() {Data = allNews, TotalCount = newsCount};
                return Request.CreateResponse(HttpStatusCode.OK, result);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
            }
        }

        // <summary>
        // API that loads all news
        // </summary>
        // <returns></returns>
        [HttpGet]
        [ValidateAntiForgeryToken]
        [ActionName("get")]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.View)]
        public HttpResponseMessage GetSingleNews(int newsId)
        {
            try
            {
                var moduleInfo = new ModuleController().GetModule(ActiveModule.ModuleID, ActiveModule.TabID);
                var moduleSettings = moduleInfo.TabModuleSettings;
                string templateName = (string)moduleSettings["TemplateNameSingle"];
                string template = BBNewsController.GetTemplate(templateName, "News");

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
                template = compatibleTemplate.ToString();

                NewsInfo news = DbController.Instance.GetNews(newsId);
                
                BBNewsTokenReplace tokenReplace = new BBNewsTokenReplace(news);
                news.Html = tokenReplace.ReplaceBBNewsTokens(template);
                
                return Request.CreateResponse(HttpStatusCode.OK, news);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
            }
        }


    }

    public class NewsDto
    {
        public int TotalCount { get; set; }
        public IEnumerable<NewsInfo> Data { get; set; }
    }
}