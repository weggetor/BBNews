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
                    DeleteNews2(-1);
                }
                catch { } // ignore
            }
        }

        #region News

        public IEnumerable<News2Info> GetNews2(int portalId, int categoryId, int topN, DateTime startDate, DateTime endDate, int pageNum, int pageSize, bool includeHidden, string search)
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
                return context.ExecuteQuery<News2Info>(CommandType.Text, sqlCmd, portalId, categoryId, startDate, endDate);
            }
        }

        public int GetNewsCount2(int portalId, int categoryId, int topN, DateTime startDate, DateTime endDate, bool includeHidden, String search)
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

        public News2Info GetNews2(int newsId)
        {
            using (IDataContext context = DataContext.Instance())
            {
                string sqlCmd = "SELECT * FROM {databaseOwner}[{objectQualifier}" + Prefix + "News] WHERE NewsId = @0";
                return context.ExecuteSingleOrDefault<News2Info>(CommandType.Text, sqlCmd, newsId);
            }
        }

        public IEnumerable<News2Info> GetNews2()
        {
            using (IDataContext context = DataContext.Instance())
            {
                string sqlCmd = "SELECT * FROM {databaseOwner}[{objectQualifier}" + Prefix + "News]";
                return context.ExecuteQuery<News2Info>(CommandType.Text, sqlCmd);
            }
        }

        public void ReorgNews2(int feedId)
        {
            using (IDataContext context = DataContext.Instance())
            {
                Feed2Info feed = GetFeed2(feedId);

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

        public void SaveNewsByGuid2(News2Info news)
        {
            using (IDataContext context = DataContext.Instance())
            {
                var repository = context.GetRepository<News2Info>();
                News2Info dbNews = repository.Find("WHERE GUID = @0 AND FeedId = @1", news.GUID, news.FeedID).FirstOrDefault();
                if (dbNews == null)
                {
                    repository.Insert(news);
                }

            }
        }

        public void SaveNewsById2(News2Info news)
        {
            using (IDataContext context = DataContext.Instance())
            {
                var repository = context.GetRepository<News2Info>();
                News2Info dbNews = repository.Find("NewsId = @0", news.NewsID).FirstOrDefault();
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

        public void DeleteNews2(int newsId)
        {
            using (IDataContext context = DataContext.Instance())
            {
                var repository = context.GetRepository<News2Info>();
                News2Info dbNews = new News2Info() { NewsID = newsId };
                repository.Delete(dbNews);
            }
        }

        #endregion

        #region Category

        public Category2Info GetCategory2(int categoryId)
        {
            using (IDataContext context = DataContext.Instance())
            {
                var repository = context.GetRepository<Category2Info>();
                return repository.GetById(categoryId);
            }
        }

        public IEnumerable<Category2Info> GetCategories2()
        {
            using (IDataContext context = DataContext.Instance())
            {
                var repository = context.GetRepository<Category2Info>();
                return repository.Get();
            }
        }

        public IEnumerable<Category2Info> GetCategories2(int portalId)
        {
            using (IDataContext context = DataContext.Instance())
            {
                var repository = context.GetRepository<Category2Info>();
                return repository.Find("WHERE PortalId = @0", portalId);
            }
        }

        public void SaveCategory2(Category2Info category)
        {
            using (IDataContext context = DataContext.Instance())
            {
                var repository = context.GetRepository<Category2Info>();
                Category2Info dbCat = GetCategory2(category.CategoryID);
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

        public void DeleteCategory2(int categoryId)
        {
            using (IDataContext context = DataContext.Instance())
            {
                var repository = context.GetRepository<Category2Info>();
                Category2Info dbCat = new Category2Info() { CategoryID = categoryId };
                repository.Delete(dbCat);
            }
        }

        #endregion

        #region Feed

        public Feed2Info GetFeed2(int feedId)
        {
            using (IDataContext context = DataContext.Instance())
            {
                var repository = context.GetRepository<Feed2Info>();
                return repository.GetById(feedId);
            }
        }

        public IEnumerable<Feed2Info> GetFeeds2()
        {
            using (IDataContext context = DataContext.Instance())
            {
                var repository = context.GetRepository<Feed2Info>();
                return repository.Get();
            }
        }

        public IEnumerable<Feed2Info> GetFeeds2(int portalId)
        {
            using (IDataContext context = DataContext.Instance())
            {
                var repository = context.GetRepository<Feed2Info>();
                return repository.Find("WHERE PortalId = @0", portalId);
            }
        }

        public IEnumerable<Feed2Info> GetCategoryFeeds2(int categoryId)
        {
            using (IDataContext context = DataContext.Instance())
            {
                string sqlCmd = "SELECT feeds.* " +
                                " FROM {databaseOwner}[{objectQualifier}" + Prefix + "Feed] feeds " +
                                " INNER JOIN {databaseOwner}[{objectQualifier}" + Prefix + "CategoryFeeds] cf on f.FeedId = cf.FeedId" +
                                " WHERE cf.CategoryId = @0";
                return context.ExecuteQuery<Feed2Info>(CommandType.Text, sqlCmd, categoryId);
            }
        }

        public void SaveFeed2(Feed2Info feed)
        {
            using (IDataContext context = DataContext.Instance())
            {
                var repository = context.GetRepository<Feed2Info>();
                Feed2Info dbFeed = GetFeed2(feed.FeedID);
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

        public void DeleteFeed2(int feedId)
        {
            using (IDataContext context = DataContext.Instance())
            {
                var repository = context.GetRepository<Feed2Info>();
                Feed2Info dbFeed = new Feed2Info() { FeedID = feedId };
                repository.Delete(dbFeed);
            }
        }

        #endregion

        #region CategoryFeeds

        public void AddCategoryFeed2(int categoryId, int feedId)
        {
            RemoveCategoryFeed2(categoryId, feedId);

            using (IDataContext context = DataContext.Instance())
            {
                var repository = context.GetRepository<CategoryFeeds2Info>();
                CategoryFeeds2Info catFeed = new CategoryFeeds2Info() { CategoryID = categoryId, FeedID = feedId };
                repository.Insert(catFeed);
            }
        }

        public void RemoveCategoryFeed2(int categoryId, int feedId)
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