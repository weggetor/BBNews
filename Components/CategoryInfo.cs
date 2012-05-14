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
using System.Globalization;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Users;
using DotNetNuke.Services.Tokens;


namespace Bitboxx.DNNModules.BBNews.Components
{
	[Serializable]
	public class CategoryInfo:IHydratable,IPropertyAccess
	{
		public CategoryInfo()
		{
			CategoryId = 0;
			PortalId = -1;
			CategoryName = "";
			CategoryDescription = "";

		}
		public Int32 CategoryId { get; set; }
		public Int32 PortalId { get; set; }
		public string CategoryName { get; set; }
		public string CategoryDescription { get; set; }

		#region Implementation of IHydratable

		public void Fill(IDataReader dr)
		{
			CategoryId = Null.SetNullInteger(dr["CategoryId"]);
			CategoryName = Null.SetNullString(dr["CategoryName"]);
			CategoryDescription = Null.SetNullString(dr["CategoryDescription"]);
			PortalId = Null.SetNullInteger(dr["PortalId"]);
		}

		public int KeyID
		{
			get { return CategoryId; }
			set { CategoryId = value; }
		}

		#endregion

		#region Implementation of IPropertyAccess

		public string GetProperty(string propertyName, string format, CultureInfo formatProvider, UserInfo accessingUser, Scope accessLevel, ref bool propertyNotFound)
		{
			propertyNotFound = false;
			switch (propertyName.ToLower())
			{
				case "categoryid":
					return CategoryId.ToString(String.IsNullOrEmpty(format) ? "g" : format, formatProvider);
				case "categoryname":
					return PropertyAccess.FormatString(CategoryName, format);
				case "categorydescription":
					return PropertyAccess.FormatString(CategoryDescription, format);
				case "portalid":
					return PortalId.ToString(String.IsNullOrEmpty(format) ? "g" : format, formatProvider);
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
	}
}