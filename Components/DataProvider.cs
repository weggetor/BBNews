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
using System.Data;
using Bitboxx.DNNModules.BBNews.Components;

namespace Bitboxx.DNNModules.BBNews
{

	/// ----------------------------------------------------------------------------- 
	/// <summary> 
	/// An abstract class for the data access layer 
	/// </summary> 
	/// <remarks> 
	/// </remarks> 
	/// <history> 
	/// </history> 
	/// ----------------------------------------------------------------------------- 
	public abstract class DataProvider
	{

		#region "Shared/Static Methods"

		/// <summary>
		/// singleton reference to the instantiated object 
		/// </summary>
		private static DataProvider objProvider = null;

		/// <summary>
		/// constructor
		/// </summary>
		static DataProvider()
		{
			CreateProvider();
		}

		/// <summary>
		/// dynamically create provider 
		/// </summary>
		private static void CreateProvider()
		{
			objProvider = (DataProvider)DotNetNuke.Framework.Reflection.CreateObject("data", "Bitboxx.DNNModules.BBNews", "");
		}

		/// <summary>
		/// return the provider 
		/// </summary>
		/// <returns></returns>
		public static DataProvider Instance()
		{
			return objProvider;
		}

		#endregion

		#region "Abstract methods"

		public abstract IDataReader GetNews(int PortalID, int CategoryID, int TopN, DateTime StartDate, DateTime EndDate, int pageNum, int pageSize, bool includeHidden, string search);
		public abstract int GetNewsCount(int PortalID, int CategoryID, int TopN, DateTime StartDate, DateTime EndDate, bool includeHidden, string search);
		public abstract IDataReader GetNews(int NewsId);
		public abstract IDataReader GetNews();

		public abstract void ReorgNews(int FeedId);
		public abstract void SaveNewsByGuid(NewsInfo News);
		public abstract void SaveNewsById(NewsInfo News);
		public abstract void DeleteNews(int NewsId);

		public abstract IDataReader GetCategory(int categoryId);
		public abstract IDataReader GetCategories();
		public abstract IDataReader GetCategories(int PortalId);
		public abstract void SaveCategory(CategoryInfo Category);
		public abstract void DeleteCategory(int CategoryId);

		public abstract IDataReader GetFeed(int Feedid);
		public abstract IDataReader GetFeeds();
		public abstract IDataReader GetFeeds(int portalId);
		public abstract void SaveFeed(FeedInfo Feed);
		public abstract void DeleteFeed(int FeedId);

		public abstract IDataReader GetCategoryFeeds(int categoryId);
		public abstract void AddCategoryFeed(int categoryId, int feedId);
		public abstract void RemoveCategoryFeed(int categoryId, int feedId);

		#endregion

	}
}