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
using System.Data;
using System.Data.SqlClient;
using Microsoft.ApplicationBlocks.Data;

using DotNetNuke.Common.Utilities;
using DotNetNuke.Framework.Providers;
using System.Data.SqlTypes;

namespace Bitboxx.DNNModules.BBNews
{

	/// ----------------------------------------------------------------------------- 
	/// <summary> 
	/// SQL Server implementation of the abstract DataProvider class 
	/// </summary> 
	/// <remarks> 
	/// </remarks> 
	/// <history> 
	/// </history> 
	/// ----------------------------------------------------------------------------- 
	public class SqlDataProvider : DataProvider
	{


		#region "Private Members"

		private const string ProviderType = "data";
		private const string ModuleQualifier = "BBNews_";

		private ProviderConfiguration _providerConfiguration = ProviderConfiguration.GetProviderConfiguration(ProviderType);
		private string _connectionString;
		private string _providerPath;
		private string _objectQualifier;
		private string _databaseOwner;

		#endregion

		#region "Constructors"

		public SqlDataProvider()
		{

			// Read the configuration specific information for this provider 
			Provider objProvider = (Provider)_providerConfiguration.Providers[_providerConfiguration.DefaultProvider];

			// Read the attributes for this provider 

			//Get Connection string from web.config 
			_connectionString = Config.GetConnectionString();

			if (_connectionString == "")
			{
				// Use connection string specified in provider 
				_connectionString = objProvider.Attributes["connectionString"];
			}

			_providerPath = objProvider.Attributes["providerPath"];

			_objectQualifier = objProvider.Attributes["objectQualifier"];
			if (_objectQualifier != "" & _objectQualifier.EndsWith("_") == false)
			{
				_objectQualifier += "_";
			}

			_databaseOwner = objProvider.Attributes["databaseOwner"];
			if (_databaseOwner != "" & _databaseOwner.EndsWith(".") == false)
			{
				_databaseOwner += ".";
			}

		}

		#endregion

		#region "Properties"
		public string ConnectionString
		{
			get { return _connectionString; }
		}
		public string ProviderPath
		{
			get { return _providerPath; }
		}
		public string ObjectQualifier
		{
			get { return _objectQualifier; }
		}
		public string DatabaseOwner
		{
			get { return _databaseOwner; }
		}
		public string Prefix
		{
			get { return _databaseOwner + _objectQualifier + ModuleQualifier; }
		}
		#endregion

		#region "Private Methods"

		private string GetFullyQualifiedName(string name)
		{
			return DatabaseOwner + ObjectQualifier + ModuleQualifier + name;
		}
		private object GetNull(object Field)
		{
			return DotNetNuke.Common.Utilities.Null.GetNull(Field, DBNull.Value);
		}

		#endregion

		#region "Public Methods"

		public override int GetNewsCount(int PortalId, int CategoryId, int TopN, DateTime StartDate, DateTime EndDate, bool includeHidden, string search)
		{
			string topClause = (TopN > 0 ? "TOP " + TopN.ToString() : "");
			string selCmd = "with tmp as (SELECT " + topClause + " * FROM " + GetFullyQualifiedName("News") + " News" ;

			List<SqlParameter> sqlParams = new List<SqlParameter>();
			string where = (includeHidden ? " 1=1" : " News.Hide = 0");
			
			if (CategoryId > 0)
			{
				where += " AND News.FeedId IN (SELECT FeedId FROM " + GetFullyQualifiedName("CategoryFeeds") + " CatFeeds" +
				         " INNER JOIN " + GetFullyQualifiedName("Category") + " Cat ON CatFeeds.CategoryId = Cat.CategoryId" +
				         " WHERE Cat.CategoryId = @CategoryId";
				if (PortalId > -1)
				{
					where += " AND Cat.PortalID = @PortalId ";
					sqlParams.Add(new SqlParameter("PortalId", PortalId));
				}
				sqlParams.Add(new SqlParameter("CategoryId", CategoryId));
				where += ")";
			}
			if (StartDate > (DateTime)SqlDateTime.MinValue && StartDate <= (DateTime)SqlDateTime.MaxValue)
			{
				where += " AND News.Pubdate >= @StartDate";
				sqlParams.Add(new SqlParameter("StartDate", StartDate));
			}
			if (EndDate < (DateTime)SqlDateTime.MaxValue && EndDate >= (DateTime)SqlDateTime.MinValue)
			{
				where += " AND News.Pubdate <= @EndDate";
				sqlParams.Add(new SqlParameter("EndDate", EndDate));
			}
			if (!String.IsNullOrEmpty(search))
			{
				where += " AND (News.Title LIKE '%" + search + "%' OR News.Summary LIKE '%" + search +
						 "%' OR News.News LIKE '%" + search + "%' )";
			}
			selCmd += " WHERE " + where +") SELECT COUNT(*) FROM tmp";

			return (int)SqlHelper.ExecuteScalar(ConnectionString, CommandType.Text, selCmd, sqlParams.ToArray());
		}
		
		public override IDataReader GetNews(int PortalId, int CategoryId, int TopN, DateTime StartDate, DateTime EndDate, int pageNum, int pageSize, bool includeHidden, string search)
		{
			if (pageNum > 0 && pageSize > 0)
			{
				List<SqlParameter> sqlParams = new List<SqlParameter>();
				string where = (includeHidden ? " 1=1" : " News.Hide = 0");

				if (CategoryId > 0)
				{
					where += " AND News.FeedId IN (SELECT FeedId FROM " + GetFullyQualifiedName("CategoryFeeds") + " CatFeeds" +
							 " INNER JOIN " + GetFullyQualifiedName("Category") + " Cat ON CatFeeds.CategoryId = Cat.CategoryId" +
							 " WHERE Cat.CategoryId = @CategoryId";
					if (PortalId > -1)
					{
						where += " AND Cat.PortalID = @PortalId ";
						sqlParams.Add(new SqlParameter("PortalId", PortalId));
					}
					sqlParams.Add(new SqlParameter("CategoryId", CategoryId));
					where += ")";
				}
				if (StartDate > (DateTime) SqlDateTime.MinValue && StartDate <= (DateTime) SqlDateTime.MaxValue)
				{
					where += " AND News.Pubdate >= @StartDate";
					sqlParams.Add(new SqlParameter("StartDate", StartDate));
				}
				if (EndDate < (DateTime) SqlDateTime.MaxValue && EndDate >= (DateTime) SqlDateTime.MinValue)
				{
					where += " AND News.Pubdate <= @EndDate";
					sqlParams.Add(new SqlParameter("EndDate", EndDate));
				}
				if (!String.IsNullOrEmpty(search))
				{
					where += " AND (News.Title LIKE '%" + search + "%' OR News.Summary LIKE '%" + search +
							 "%' OR News.News LIKE '%" + search + "%' )";
				}

				string topClause = (TopN > 0 ? "TOP " + TopN.ToString() : "");
				string selCmd =
					"WITH NewsRN AS ( SELECT ROW_NUMBER() OVER (ORDER BY News.PubDate DESC, News.NewsId DESC) AS RowNum, News.NewsId FROM " +
					GetFullyQualifiedName("News") + " News WHERE " + where +")"+
					"SELECT " + topClause + " News.*, NewsRN.RowNum FROM " + GetFullyQualifiedName("News") + " News" +
					" INNER JOIN NewsRN ON NewsRN.NewsId = News.NewsId" +
					" WHERE " + String.Format(" RowNum BETWEEN {0} AND {1}", (pageNum - 1) * pageSize + 1, pageNum * pageSize) +
					" ORDER BY News.Pubdate DESC, News.NewsId DESC";

				return (IDataReader) SqlHelper.ExecuteReader(ConnectionString, CommandType.Text, selCmd, sqlParams.ToArray());
			}
			else
			{
				string topClause = (TopN > 0 ? "TOP " + TopN.ToString() : "");

				string selCmd = "SELECT DISTINCT " + topClause + " * FROM " + GetFullyQualifiedName("News") + " News";

				List<SqlParameter> sqlParams = new List<SqlParameter>();
				string where = (includeHidden ? " 1=1" : " News.Hide = 0");
				if (CategoryId > 0)
				{
					where += " AND News.FeedId IN "+
						"(SELECT FeedId FROM " + GetFullyQualifiedName("CategoryFeeds") + " CatFeeds" +
						" INNER JOIN " + GetFullyQualifiedName("Category") + " Cat ON CatFeeds.CategoryId = Cat.CategoryId" +
						" WHERE Cat.CategoryId = @CategoryId";
					if (PortalId > -1)
					{
						where += " AND Cat.PortalID = @PortalId ";
						sqlParams.Add(new SqlParameter("PortalId", PortalId));
					}
					sqlParams.Add(new SqlParameter("CategoryId", CategoryId));
					where += ")";
				}
				if (StartDate > (DateTime)SqlDateTime.MinValue && StartDate <= (DateTime)SqlDateTime.MaxValue)
				{
					where += " AND News.Pubdate >= @StartDate";
					sqlParams.Add(new SqlParameter("StartDate", StartDate));
				}
				if (EndDate < (DateTime)SqlDateTime.MaxValue && EndDate >= (DateTime)SqlDateTime.MinValue)
				{
					where += " AND News.Pubdate <= @EndDate";
					sqlParams.Add(new SqlParameter("EndDate", EndDate));
				}
				if (!String.IsNullOrEmpty(search))
				{
					where += " AND (News.Title LIKE '%" + search + "%' OR News.Summary LIKE '%" + search +
							 "%' OR News.News LIKE '%" + search + "%' )";
				}

				selCmd += " WHERE " + where + " ORDER BY Pubdate DESC, News.NewsId DESC";
				return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, CommandType.Text, selCmd, sqlParams.ToArray());
			}
		}
		public override IDataReader GetNews(int NewsId)
		{
			string selCmd = "SELECT * FROM " + GetFullyQualifiedName("News") +
				" WHERE NewsId = @NewsId";
			SqlParameter SqlParams = new SqlParameter("NewsId", NewsId);
			return SqlHelper.ExecuteReader(ConnectionString, CommandType.Text, selCmd, SqlParams);
		}
		public override IDataReader GetNews()
		{
			string selCmd = "SELECT * FROM " + GetFullyQualifiedName("News");
			return SqlHelper.ExecuteReader(ConnectionString, CommandType.Text, selCmd);
		}

		public override void ReorgNews(int FeedId)
		{
			string selCmd = "SELECT ReorgInterval FROM " + GetFullyQualifiedName("Feed") +
				" WHERE FeedId = @FeedId";
			
			SqlParameter[] SqlParams = new SqlParameter[] {
				   new SqlParameter("FeedId",FeedId)};
			
			int reorgInterval = (int)SqlHelper.ExecuteScalar(ConnectionString, CommandType.Text, selCmd, SqlParams);

			if (reorgInterval > 0)
			{
				string delCmd = "DELETE FROM " + GetFullyQualifiedName("News") +
					" WHERE FeedId = @FeedId"+
					" AND PubDate < @ReorgTime";
				
				SqlParams = new SqlParameter[] {
				   new SqlParameter("FeedId",FeedId),
				   new SqlParameter("ReorgTime",DateTime.Now - new TimeSpan(reorgInterval,0,0,0))};

				SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, delCmd, SqlParams);
			}
		}
		public override void SaveNewsByGuid(Components.NewsInfo News)
		{
			SqlParameter[] SqlParams;

			SqlParams = new SqlParameter[] {
			   new SqlParameter("FeedId",News.FeedId),
			   new SqlParameter("GUID",News.GUID)};

			string selCmd = "SELECT NewsId FROM " + GetFullyQualifiedName("News") +
				" where GUID = @GUID AND FeedId = @FeedId";

			object newsId = SqlHelper.ExecuteScalar(ConnectionString, CommandType.Text, selCmd, SqlParams);

			if (newsId == null)
			{
				SqlParams = new SqlParameter[]
				            	{
				            		new SqlParameter("FeedId", News.FeedId),
				            		new SqlParameter("Title", News.Title),
				            		new SqlParameter("Summary", News.Summary),
				            		new SqlParameter("Author", News.Author),
				            		new SqlParameter("News", News.News),
				            		new SqlParameter("Link", News.Link),
				            		new SqlParameter("Image", News.Image),
				            		new SqlParameter("GUID", News.GUID),
				            		new SqlParameter("Pubdate", News.Pubdate),
				            		new SqlParameter("Hide", News.Hide),
				            		new SqlParameter("Internal", News.Internal),
                                    new SqlParameter("MetaData", News.MetaData),
                                };

				string insCmd = "INSERT INTO " + GetFullyQualifiedName("News") +
				   " (FeedId,Title,Summary,Author,News,Link,Image,GUID,Pubdate,Hide,Internal,MetaData)" +
				   " VALUES " +
				   " (@FeedId,@Title,@Summary,@Author,@News,@Link,@Image,@GUID,@Pubdate,@Hide,@Internal,@MetaData)";

				int result = SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, insCmd, SqlParams);
			}

		}
		public override void SaveNewsById(Components.NewsInfo News)
		{
			SqlParameter[] SqlParams;

			string selCmd = "SELECT NewsId FROM " + GetFullyQualifiedName("News") +
				" where NewsId = @NewsId";

			object newsId = SqlHelper.ExecuteScalar(ConnectionString, CommandType.Text, selCmd, new SqlParameter("NewsId", News.NewsId));

			SqlParams = new SqlParameter[]
				            	{
				            		new SqlParameter("FeedId", News.FeedId),
				            		new SqlParameter("Title", News.Title),
				            		new SqlParameter("Summary", News.Summary),
				            		new SqlParameter("Author", News.Author),
				            		new SqlParameter("News", News.News),
				            		new SqlParameter("Link", News.Link),
				            		new SqlParameter("Image", News.Image),
				            		new SqlParameter("GUID", News.GUID),
				            		new SqlParameter("Pubdate", News.Pubdate),
				            		new SqlParameter("Hide", News.Hide),
				            		new SqlParameter("Internal", News.Internal)
				            	};

			if (newsId == DBNull.Value || newsId == null)
			{

				string insCmd = "INSERT INTO " + GetFullyQualifiedName("News") +
				   " (FeedId,Title,Summary,Author,News,Link,Image,GUID,Pubdate,Hide,Internal)" +
				   " VALUES " +
				   " (@FeedId,@Title,@Summary,@Author,@News,@Link,@Image,@GUID,@Pubdate,@Hide,@Internal)";

				SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, insCmd, SqlParams);
			}
			else
			{
				string updCmd = "UPDATE " + GetFullyQualifiedName("News") + " SET " +
					" FeedId = @FeedId," +
					" Title = @Title," +
					" Summary = @Summary," +
					" Author = @Author," +
					" News = @News," +
					" Link = @Link," +
					" Image = @Image," +
					" GUID = @GUID," +
					" Pubdate = @Pubdate," +
					" Hide = @Hide," +
					" Internal = @Internal" +
					" WHERE NewsId = " + newsId.ToString();

				SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, updCmd, SqlParams);

			}

		}
		public override void DeleteNews(int NewsId)
		{
			string delCmd = "DELETE FROM " + GetFullyQualifiedName("News")  +
				" WHERE NewsId = @NewsId";
			SqlParameter SqlParams = new SqlParameter("NewsId", NewsId);
			SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, delCmd, SqlParams);
		}

		public override IDataReader GetCategory(int categoryId)
		{
			string selCmd = "SELECT * FROM " + GetFullyQualifiedName("Category") + " WHERE CategoryId = @CategoryId";
			return (IDataReader) SqlHelper.ExecuteReader(ConnectionString, CommandType.Text, selCmd, new SqlParameter("CategoryId", categoryId));
		}
		public override IDataReader GetCategories()
		{
			string selCmd = "SELECT * FROM " + GetFullyQualifiedName("Category");
			return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, CommandType.Text, selCmd);
		}

		public override IDataReader GetCategories(int PortalId)
		{
			string selCmd = "SELECT * FROM " + GetFullyQualifiedName("Category") + 
				" WHERE PortalID = @PortalId";
			SqlParameter SqlParam = new SqlParameter("PortalId", PortalId);
			return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, CommandType.Text, selCmd, SqlParam);
		}
		public override void SaveCategory(Components.CategoryInfo category)
		{
			SqlParameter[] SqlParams;

			SqlParams = new SqlParameter[] {
			   new SqlParameter("CategoryId",category.CategoryId)};

			string selCmd = "SELECT CategoryId FROM " + GetFullyQualifiedName("Category") +
				" where CategoryId = @CategoryId";

			object categoryId = SqlHelper.ExecuteScalar(ConnectionString, CommandType.Text, selCmd, SqlParams);

			SqlParams = new SqlParameter[] {
			   new SqlParameter("CategoryId",category.CategoryId),
			   new SqlParameter("PortalId",category.PortalId),
			   new SqlParameter("CategoryName",category.CategoryName),
			   new SqlParameter("CategoryDescription",category.CategoryDescription)
			};

			if (categoryId == DBNull.Value || categoryId == null)
			{
				string insCmd = "INSERT INTO " + GetFullyQualifiedName("Category") +
				   " (PortalId,CategoryName,CategoryDescription)" +
				   " VALUES " +
				   " (@PortalId,@CategoryName,@CategoryDescription)";

				SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, insCmd, SqlParams);
			}
			else
			{
				string updCmd = "UPDATE " + GetFullyQualifiedName("Category") + " SET " +
					" PortalId = @PortalId," +
					" CategoryName = @CategoryName," +
					" CategoryDescription = @CategoryDescription" +
					" WHERE CategoryId = @CategoryId";
				SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, updCmd, SqlParams);
			}
		}
		public override void DeleteCategory(int CategoryId)
		{
			string delCmd = "DELETE FROM " +GetFullyQualifiedName("Category") + 
				" WHERE CategoryId = @CategoryId";
			SqlParameter SqlParams = new SqlParameter("CategoryID", CategoryId);
			SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, delCmd, SqlParams);
		}

		public override IDataReader GetFeed(int FeedId)
		{
			string selCmd = "SELECT * FROM " + GetFullyQualifiedName("Feed") + 
				" WHERE FeedId = @FeedId" +
				" ORDER BY FeedName ASC";
			SqlParameter SqlParam = new SqlParameter("FeedId", FeedId);
			return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, CommandType.Text, selCmd, SqlParam);

		}
		public override IDataReader GetFeeds()
		{
			return SqlHelper.ExecuteReader(ConnectionString, CommandType.Text, "SELECT * FROM " + GetFullyQualifiedName("Feed"));
		}
		public override IDataReader GetFeeds(int portalId)
		{
			string sqlCmd = "SELECT * FROM " + GetFullyQualifiedName("Feed") +
			                " WHERE PortalId = @PortalId";
			return SqlHelper.ExecuteReader(ConnectionString, CommandType.Text,sqlCmd, new SqlParameter("PortalId",portalId));
		}
		public override void SaveFeed(Components.FeedInfo Feed)
		{
			SqlParameter[] SqlParams;

			SqlParams = new SqlParameter[] {
			   new SqlParameter("FeedID",Feed.FeedId)};

			string selCmd = "SELECT FeedId FROM " + GetFullyQualifiedName("Feed") +
				" where FeedID = @FeedID";

			object feedId = SqlHelper.ExecuteScalar(ConnectionString, CommandType.Text, selCmd, SqlParams);

			SqlParams = new SqlParameter[] {
				   new SqlParameter("FeedId",Feed.FeedId),
				   new SqlParameter("PortalId",Feed.PortalId),
				   new SqlParameter("FeedUrl",Feed.FeedUrl),
				   new SqlParameter("FeedName",Feed.FeedName),
				   new SqlParameter("FeedType",Feed.FeedType),
				   new SqlParameter("LastRetrieve",Null.GetNull(Feed.LastRetrieve,DBNull.Value)),
				   new SqlParameter("LastTry",Null.GetNull(Feed.LastTry,DBNull.Value)),
				   new SqlParameter("RetrieveInterval",Feed.RetrieveInterval),
				   new SqlParameter("TryInterval",Feed.TryInterval),
				   new SqlParameter("Enabled",Feed.Enabled),
				   new SqlParameter("ReorgInterval",Feed.ReorgInterval),
				   new SqlParameter("UserName",Feed.UserName),
				   new SqlParameter("Password",Feed.Password),
			};

			if (feedId == DBNull.Value || feedId == null)
			{
				string insCmd = "INSERT INTO " + GetFullyQualifiedName("Feed") +
				   " (PortalId,FeedUrl,FeedName,FeedType,LastRetrieve,LastTry,RetrieveInterval,TryInterval,Enabled, ReorgInterval,UserName,Password)" +
				   " VALUES " +
				   " (@PortalId,@FeedUrl,@FeedName,@FeedType,@LastRetrieve,@LastTry,@RetrieveInterval,@TryInterval,@Enabled,@ReorgInterval,@UserName,@Password)";

				SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, insCmd, SqlParams);			
			}
			else
			{
				string updCmd = "UPDATE " + GetFullyQualifiedName("Feed") + " SET " +
				" PortalId = @PortalId," +
				" FeedUrl = @FeedUrl," +
				" FeedName = @FeedName," +
				" FeedType = @FeedType," +
				" LastRetrieve = @LastRetrieve," +
				" LastTry = @LastTry," +
				" RetrieveInterval = @RetrieveInterval," +
				" TryInterval = @TryInterval," +
				" Enabled = @Enabled," +
				" ReorgInterval = @ReorgInterval," +
				" UserName = @UserName," +
				" Password = @Password" +
				" WHERE FeedID = @FeedID";

				SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, updCmd, SqlParams);
			}
		}
		public override void DeleteFeed(int FeedId)
		{
			string delCmd = "DELETE FROM " + GetFullyQualifiedName("Feed") +
				" WHERE FeedID = @FeedId";
			SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, delCmd, new SqlParameter("FeedId",FeedId));
		}

		public override IDataReader GetCategoryFeeds(int categoryId)
		{
			string sqlCmd = "SELECT * FROM " + GetFullyQualifiedName("Feed") + " f " + 
				" INNER JOIN " + GetFullyQualifiedName("CategoryFeeds") + " cf on f.FeedId = cf.FeedId" +
							" WHERE cf.CategoryId = @CategoryId";
			return SqlHelper.ExecuteReader(ConnectionString, CommandType.Text, sqlCmd, new SqlParameter("CategoryId", categoryId));
		}

		public override void AddCategoryFeed(int categoryId, int feedId)
		{
			string sqlCmd = "SELECT Count(*) FROM " + GetFullyQualifiedName("CategoryFeeds") +
			                " WHERE CategoryId = @CategoryId AND FeedId = @FeedId";
			SqlParameter[] sqlParams = new SqlParameter[]
			                           	{
			                           		new SqlParameter("CategoryId", categoryId), 
											new SqlParameter("FeedId", feedId)
			                           	};
			int cnt = (int) SqlHelper.ExecuteScalar(ConnectionString, CommandType.Text, sqlCmd, sqlParams);
			if (cnt == 0)
			{
				sqlCmd = "INSERT INTO " + GetFullyQualifiedName("CategoryFeeds") +
				         "(Categoryid,FeedId) VALUES (@CategoryId,@FeedId)";
				SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, sqlCmd, sqlParams);
			}
		}
		public override void RemoveCategoryFeed(int categoryId, int feedId)
		{
			string sqlCmd = "DELETE FROM " + GetFullyQualifiedName("CategoryFeeds") +
							" WHERE CategoryId = @CategoryId AND FeedId = @FeedId";
			SqlParameter[] sqlParams = new SqlParameter[]
			                           	{
			                           		new SqlParameter("CategoryId", categoryId), 
											new SqlParameter("FeedId", feedId)
			                           	};
			SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, sqlCmd, sqlParams);
		}
		#endregion

	}
}