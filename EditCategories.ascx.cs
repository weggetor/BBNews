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
using System.Web.UI;
using System.Web.UI.WebControls;
using Bitboxx.DNNModules.BBNews.Components;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Services.Localization;

namespace Bitboxx.DNNModules.BBNews
{
	public partial class EditCategories : PortalModuleBase
	{
		#region Private Members

		private BBNewsController _controller;
		private bool _inEditMode = false;

		#endregion

		#region Properties

		public BBNewsController Controller
		{
			get
			{
				if (_controller == null)
					_controller = new BBNewsController();
				return _controller;
			}
		}
		public bool InEditMode
		{
			get { return _inEditMode; }
			set
			{
				_inEditMode = value;
				pnlEdit.Visible = _inEditMode;
				pnlSearch.Visible = !_inEditMode;
				cmdNew.Visible = !_inEditMode;
				cmdSave.Visible = _inEditMode;
				cmdCancel.Visible = _inEditMode;
			}
		}

		public Control MainControl { get; set; } 
		#endregion

		
		protected void Page_Load(object sender, EventArgs e)
		{
			Localization.LocalizeDataGrid(ref grdCategories, this.LocalResourceFile);
			if (!IsPostBack)
				BindData();
		}

		protected void grdCategories_ItemCommand(object source, DataGridCommandEventArgs e)
		{
			int categoryId = Convert.ToInt32(e.CommandArgument);
			switch (e.CommandName)
			{
				case "Edit":
					CategoryInfo cat = Controller.GetCategory(categoryId);
					txtCategoryName.Text = cat.CategoryName;
					txtCategoryDescription.Text = cat.CategoryDescription;
					hidCategoryId.Value = cat.CategoryId.ToString();
					InEditMode = true;
					BindData();
					break;
				case "Delete":
					Controller.DeleteCategory(categoryId);
					InEditMode = false;
					BindData();
					break;
			}
		}

		protected void cmdNew_Click(object sender, EventArgs e)
		{
			txtCategoryName.Text = "";
			txtCategoryDescription.Text = "";
			hidCategoryId.Value = "-1";
			InEditMode = true;
			BindData();
		}

		protected void cmdSave_Click(object sender, EventArgs e)
		{
			CategoryInfo cat = new CategoryInfo();
			cat.CategoryId = Convert.ToInt32(hidCategoryId.Value);
			cat.CategoryName = txtCategoryName.Text;
			cat.CategoryDescription = txtCategoryDescription.Text;
			cat.PortalId = PortalId;
			Controller.SaveCategory(cat);
			BindData();
			InEditMode = false;
			EditCategoryFeeds ctrl = (EditCategoryFeeds)((Edit) MainControl).SubModules["EditCategoryFeeds"];
			ctrl.FillDdlCategories();
		}

		protected void cmdCancel_Click(object sender, EventArgs e)
		{
			InEditMode = false;
			BindData();
		}

		private void BindData()
		{
			List<CategoryInfo> allcats = Controller.GetCategories(PortalId);
			grdCategories.DataSource = allcats;
			grdCategories.DataBind();
		}
	}
}