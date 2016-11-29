using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlTypes;
using System.Linq;
using System.Web;
using Bitboxx.DNNModules.BBNews.Models;
using DotNetNuke.Data;

namespace Bitboxx.DNNModules.BBNews.Components
{
    public class DbController
    {
        /// <summary>
        /// The _instance
        /// </summary>
        private static DbController _instance;
        /// <summary>
        /// Gets the instance.
        /// </summary>
        /// <value>The instance.</value>
        public static DbController Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new DbController();
                }
                return _instance;
            }
        }

        private const string Prefix = "BBNews_";

        /// <summary>
        /// Clears the cache.
        /// </summary>

        public void ClearCache(int moduleId)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                try
                {
                    // Setup fictitious item to delete (just to clear the scope cache)
                    DeleteNews(-1);
                }
                catch { } // ignore
            }
        }

        #region News

        public IEnumerable<NewsInfo> GetNews(int portalId, int categoryId, int topN, DateTime startDate, DateTime endDate, int pageNum, int pageSize, bool includeHidden, string search)
        {
            // parameters: PortalId = 0, CategoryId = 1, StartDate = 2 , EndDate = 3,

            using (IDataContext context = DataContext.Instance())
            {
                string sqlCmd;
                if (pageNum > 0 && pageSize > 0)
                {
                    string where = (includeHidden ? " 1=1" : " News.Hide = 0");

                    if (categoryId > 0)
                    {
                        where += " AND News.FeedId IN (SELECT FeedId FROM {databaseOwner}[{objectQualifier}" + Prefix + "CategoryFeeds] CatFeeds" +
                                 " INNER JOIN  {databaseOwner}[{objectQualifier}" + Prefix + "Category] Cat ON CatFeeds.CategoryId = Cat.CategoryId" +
                                 " WHERE Cat.CategoryId = @1";
                        if (portalId > -1)
                        {
                            where += " AND Cat.PortalID = @0 ";
                        }
                        where += ")";
                    }
                    if (startDate > (DateTime)SqlDateTime.MinValue && startDate <= (DateTime)SqlDateTime.MaxValue)
                    {
                        where += " AND News.Pubdate >= @2";
                    }
                    if (endDate < (DateTime)SqlDateTime.MaxValue && endDate >= (DateTime)SqlDateTime.MinValue)
                    {
                        where += " AND News.Pubdate <= @3";
                    }
                    if (!String.IsNullOrEmpty(search))
                    {
                        where += " AND (News.Title LIKE '%" + search + "%' OR News.Summary LIKE '%" + search +
                                 "%' OR News.News LIKE '%" + search + "%' )";
                    }

                    string topClause = (topN > 0 ? "TOP " + topN.ToString() : "");
                    sqlCmd =
                        ";WITH NewsRN AS ( SELECT ROW_NUMBER() OVER (ORDER BY News.PubDate DESC, News.NewsId DESC) AS RowNum, News.NewsId FROM " +
                        "{databaseOwner}[{objectQualifier}" + Prefix + "News] News WHERE " + where + ")" +
                        "SELECT " + topClause + " News.*, NewsRN.RowNum FROM {databaseOwner}[{objectQualifier}" + Prefix + "News] News" +
                        " INNER JOIN NewsRN ON NewsRN.NewsId = News.NewsId" +
                        " WHERE " + String.Format(" RowNum BETWEEN {0} AND {1}", (pageNum - 1) * pageSize + 1, pageNum * pageSize) +
                        " ORDER BY News.Pubdate DESC, News.NewsId DESC";

                }
                else
                {
                    string topClause = (topN > 0 ? "TOP " + topN.ToString() : "");

                    sqlCmd = "SELECT DISTINCT " + topClause + " * FROM {databaseOwner}[{objectQualifier}" + Prefix + "News] News";

                    string where = (includeHidden ? " 1=1" : " News.Hide = 0");
                    if (categoryId > 0)
                    {
                        where += " AND News.FeedId IN " +
                            "(SELECT FeedId FROM {databaseOwner}[{objectQualifier}" + Prefix + "CategoryFeeds] CatFeeds" +
                            " INNER JOIN {databaseOwner}[{objectQualifier}" + Prefix + "Category] Cat ON CatFeeds.CategoryId = Cat.CategoryId" +
                            " WHERE Cat.CategoryId = @1";
                        if (portalId > -1)
                        {
                            where += " AND Cat.PortalID = @0 ";
                        }
                        where += ")";
                    }
                    if (startDate > (DateTime)SqlDateTime.MinValue && startDate <= (DateTime)SqlDateTime.MaxValue)
                    {
                        where += " AND News.Pubdate >= @2";
                    }
                    if (endDate < (DateTime)SqlDateTime.MaxValue && endDate >= (DateTime)SqlDateTime.MinValue)
                    {
                        where += " AND News.Pubdate <= @3";
                    }
                    if (!String.IsNullOrEmpty(search))
                    {
                        where += " AND (News.Title LIKE '%" + search + "%' OR News.Summary LIKE '%" + search +
                                 "%' OR News.News LIKE '%" + search + "%' )";
                    }

                    sqlCmd += " WHERE " + where + " ORDER BY Pubdate DESC, News.NewsId DESC";
                }
                return context.ExecuteQuery<NewsInfo>(CommandType.Text, sqlCmd, portalId, categoryId, startDate, endDate);
            }
        }

        public int GetNewsCount(int portalId, int categoryId, int topN, DateTime startDate, DateTime endDate, bool includeHidden, String search)
        {
            using (IDataContext context = DataContext.Instance())
            {

                string topClause = (topN > 0 ? "TOP " + topN.ToString() : "");
                string sqlCmd = ";with tmp as (SELECT " + topClause + " * FROM {databaseOwner}[{objectQualifier}" + Prefix + "News] News";

                string where = (includeHidden ? " 1=1" : " News.Hide = 0");

                if (categoryId > 0)
                {
                    where += " AND News.FeedId IN (SELECT FeedId FROM {databaseOwner}[{objectQualifier}" + Prefix + "CategoryFeeds] CatFeeds" +
                             " INNER JOIN {databaseOwner}[{objectQualifier}" + Prefix + "Category] Cat ON CatFeeds.CategoryId = Cat.CategoryId" +
                             " WHERE Cat.CategoryId = @1";
                    if (portalId > -1)
                    {
                        where += " AND Cat.PortalID = @0 ";
                    }
                    where += ")";
                }
                if (startDate > (DateTime)SqlDateTime.MinValue && startDate <= (DateTime)SqlDateTime.MaxValue)
                {
                    where += " AND News.Pubdate >= @2";
                }
                if (endDate < (DateTime)SqlDateTime.MaxValue && endDate >= (DateTime)SqlDateTime.MinValue)
                {
                    where += " AND News.Pubdate <= @3";
                }
                if (!String.IsNullOrEmpty(search))
                {
                    where += " AND (News.Title LIKE '%" + search + "%' OR News.Summary LIKE '%" + search +
                             "%' OR News.News LIKE '%" + search + "%' )";
                }
                sqlCmd += " WHERE " + where + ") SELECT COUNT(*) FROM tmp";
                return context.ExecuteScalar<int>(CommandType.Text, sqlCmd, portalId, categoryId, startDate, endDate);
            }
        }

        public NewsInfo GetNews(int newsId)
        {
            using (IDataContext context = DataContext.Instance())
            {
                string sqlCmd = "SELECT * FROM {databaseOwner}[{objectQualifier}" + Prefix + "News] WHERE NewsId = @0";
                return context.ExecuteSingleOrDefault<NewsInfo>(CommandType.Text, sqlCmd, newsId);
            }
        }

        public IEnumerable<NewsInfo> GetNews()
        {
            using (IDataContext context = DataContext.Instance())
            {
                string sqlCmd = "SELECT * FROM {databaseOwner}[{objectQualifier}" + Prefix + "News]";
                return context.ExecuteQuery<NewsInfo>(CommandType.Text, sqlCmd);
            }
        }

        public void ReorgNews(int feedId)
        {
            using (IDataContext context = DataContext.Instance())
            {
                FeedInfo feed = GetFeed(feedId);

                int reorgInterval = feed.ReorgInterval;

                if (reorgInterval > 0)
                {
                    string sqlCmd = "DELETE FROM {databaseOwner}[{objectQualifier}" + Prefix + "News]" +
                        " WHERE FeedId = @0" +
                        " AND PubDate < @1";

                    DateTime firstPubDateLeft = DateTime.Now - new TimeSpan(reorgInterval, 0, 0, 0);
                    context.Execute(CommandType.Text, sqlCmd, feedId, firstPubDateLeft);
                }
            }
        }

        public void SaveNewsByGuid(NewsInfo news)
        {
            using (IDataContext context = DataContext.Instance())
            {
                var repository = context.GetRepository<NewsInfo>();
                NewsInfo dbNews = repository.Find("WHERE GUID = @0 AND FeedId = @1", news.GUID, news.FeedID).FirstOrDefault();
                if (dbNews == null)
                {
                    repository.Insert(news);
                }

            }
        }

        public void SaveNewsById(NewsInfo news)
        {
            using (IDataContext context = DataContext.Instance())
            {
                var repository = context.GetRepository<NewsInfo>();
                NewsInfo dbNews = repository.Find("WHERE NewsId = @0", news.NewsID).FirstOrDefault();
                if (dbNews == null)
                {
                    repository.Insert(news);
                }
                else
                {
                    repository.Update(news);
                }

            }
        }

        public void DeleteNews(int newsId)
        {
            using (IDataContext context = DataContext.Instance())
            {
                var repository = context.GetRepository<NewsInfo>();
                NewsInfo dbNews = new NewsInfo() { NewsID = newsId };
                repository.Delete(dbNews);
            }
        }

        #endregion

        #region Category

        public CategoryInfo GetCategory(int categoryId)
        {
            using (IDataContext context = DataContext.Instance())
            {
                var repository = context.GetRepository<CategoryInfo>();
                return repository.GetById(categoryId);
            }
        }

        public IEnumerable<CategoryInfo> GetCategories()
        {
            using (IDataContext context = DataContext.Instance())
            {
                var repository = context.GetRepository<CategoryInfo>();
                return repository.Get();
            }
        }

        public IEnumerable<CategoryInfo> GetCategories(int portalId)
        {
            using (IDataContext context = DataContext.Instance())
            {
                var repository = context.GetRepository<CategoryInfo>();
                return repository.Find("WHERE PortalId = @0", portalId);
            }
        }

        public void SaveCategory(CategoryInfo category)
        {
            using (IDataContext context = DataContext.Instance())
            {
                var repository = context.GetRepository<CategoryInfo>();
                CategoryInfo dbCat = GetCategory(category.CategoryID);
                if (dbCat == null)
                {
                    repository.Insert(category);
                }
                else
                {
                    repository.Update(category);
                }
            }
        }

        public void DeleteCategory(int categoryId)
        {
            using (IDataContext context = DataContext.Instance())
            {
                var repository = context.GetRepository<CategoryInfo>();
                CategoryInfo dbCat = new CategoryInfo() { CategoryID = categoryId };
                repository.Delete(dbCat);
            }
        }

        #endregion

        #region Feed

        public FeedInfo GetFeed(int feedId)
        {
            using (IDataContext context = DataContext.Instance())
            {
                var repository = context.GetRepository<FeedInfo>();
                return repository.GetById(feedId);
            }
        }

        public IEnumerable<FeedInfo> GetFeeds()
        {
            using (IDataContext context = DataContext.Instance())
            {
                var repository = context.GetRepository<FeedInfo>();
                return repository.Get();
            }
        }

        public IEnumerable<FeedInfo> GetFeeds(int portalId)
        {
            using (IDataContext context = DataContext.Instance())
            {
                var repository = context.GetRepository<FeedInfo>();
                return repository.Find("WHERE PortalId = @0", portalId);
            }
        }

        public IEnumerable<FeedInfo> GetCategoryFeeds(int categoryId)
        {
            using (IDataContext context = DataContext.Instance())
            {
                string sqlCmd = "SELECT f.* " +
                                " FROM {databaseOwner}[{objectQualifier}" + Prefix + "Feed] f " +
                                " INNER JOIN {databaseOwner}[{objectQualifier}" + Prefix + "CategoryFeeds] cf on f.FeedId = cf.FeedId" +
                                " WHERE cf.CategoryId = @0";
                return context.ExecuteQuery<FeedInfo>(CommandType.Text, sqlCmd, categoryId);
            }
        }

        public void SaveFeed(FeedInfo feed)
        {
            using (IDataContext context = DataContext.Instance())
            {
                var repository = context.GetRepository<FeedInfo>();
                FeedInfo dbFeed = GetFeed(feed.FeedID);
                if (dbFeed == null)
                {
                    repository.Insert(feed);
                }
                else
                {
                    repository.Update(feed);
                }
            }
        }

        public void DeleteFeed(int feedId)
        {
            using (IDataContext context = DataContext.Instance())
            {
                var repository = context.GetRepository<FeedInfo>();
                FeedInfo dbFeed = new FeedInfo() { FeedID = feedId };
                repository.Delete(dbFeed);
            }
        }

        #endregion

        #region CategoryFeeds

        public void AddCategoryFeed(int categoryId, int feedId)
        {
            RemoveCategoryFeed(categoryId, feedId);

            using (IDataContext context = DataContext.Instance())
            {
                string sqlCmd = "INSERT INTO {databaseOwner}[{objectQualifier}" + Prefix + "CategoryFeeds]" +
                                " (CategoryID,FeedID) VALUES (@0,@1)";
                context.Execute(CommandType.Text, sqlCmd,categoryId,feedId);
            }
        }

        public void RemoveCategoryFeed(int categoryId, int feedId)
        {
            using (IDataContext context = DataContext.Instance())
            {
                string sqlCmd = "DELETE FROM {databaseOwner}[{objectQualifier}" + Prefix + "CategoryFeeds]" +
                            " WHERE CategoryId = @0 AND FeedId = @1";
                context.Execute(CommandType.Text, sqlCmd, categoryId, feedId);
            }
        }

        #endregion

    }
}