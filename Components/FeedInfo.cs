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
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Modules;

namespace Bitboxx.DNNModules.BBNews.Components
{
	[Serializable]
	public class FeedInfo: IHydratable
	{
		public FeedInfo()
		{
			FeedId = 0;
			PortalId = 0;
			FeedUrl = "";
			FeedName = "";
			FeedType = 0;
			LastRetrieve = DateTime.Now;
			LastTry = DateTime.Now;
			RetrieveInterval = 120;
			TryInterval = 20;
			Enabled = true;
			ReorgInterval = 0;
			UserName = "";
			Password = "";

		}
		public int FeedId { get; set; }
		public string FeedName { get; set; }
		public string FeedUrl { get; set; }
		public int FeedType { get; set; }
		public int PortalId { get; set; }
		public DateTime LastRetrieve { get; set; }
		public DateTime LastTry { get; set; }
		public int RetrieveInterval { get; set; }
		public int TryInterval { get; set; }
		public bool Enabled { get; set; }
		public Int32 ReorgInterval { get; set; }
		public string UserName { get; set; }
		public string Password { get; set; }

		#region Implementation of IHydratable

		public void Fill(IDataReader dr)
		{
			FeedId = Null.SetNullInteger(dr["FeedId"]);
			FeedName = Null.SetNullString(dr["FeedName"]);
			FeedUrl = Null.SetNullString(dr["FeedUrl"]);
			FeedType = Null.SetNullInteger(dr["FeedType"]);
			PortalId = Null.SetNullInteger(dr["PortalId"]);
			LastRetrieve = Null.SetNullDateTime(dr["LastRetrieve"]);
			LastTry = Null.SetNullDateTime(dr["LastTry"]);
			RetrieveInterval = Null.SetNullInteger(dr["RetrieveInterval"]);
			TryInterval = Null.SetNullInteger(dr["TryInterval"]);
			ReorgInterval = Null.SetNullInteger(dr["ReorgInterval"]);
			Enabled = Null.SetNullBoolean(dr["Enabled"]);
			UserName = Null.SetNullString(dr["UserName"]);
			Password = Null.SetNullString(dr["Password"]);
		}

		public int KeyID
		{
			get { return FeedId; }
			set { FeedId = value; }
		}

		#endregion
	}

}