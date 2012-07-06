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
using System.IO;
using System.Text;
using System.Threading;
using System.Web.UI.WebControls;
using Bitboxx.DNNModules.BBNews.Components;
using DotNetNuke.Entities.Users;
using DotNetNuke.Framework;
using DotNetNuke.Services.Localization;

namespace Bitboxx.DNNModules.BBNews.Controls
{
	public partial class TemplateControl : UserControlBase
	{
		private string _cssClass = "Normal";
		private int _width = 150;
		private string _localResourceFile;
		private string _key;
		private bool _isSuperUser;

		public string Value
		{
			get
			{
				if (ViewState["Value"] != null)
					return (string)ViewState["Value"];
				return "default";
			}
			set
			{
				ViewState["Value"] = value;
				ddlTemplate.SelectedValue = value;
				ShowThumb();
			}
		}
		public string CssClass
		{
			get { return _cssClass; }
			set
			{
				_cssClass = value;
				pnlView.CssClass = value;
			}
		}
		public int Width
		{
			get { return _width; }
			set 
			{ 
				_width = value;
				pnlView.Width = value;
				imgThumb.Width = new Unit(_width);
				ddlTemplate.Width = new Unit(_width);
			}
		}
		public string EditMode
		{
			get
			{
				if (ViewState["EditMode"] != null)
					return (string)ViewState["EditMode"];
				return "view";
			}
			set
			{
				string mode = value.ToLower();
				ViewState["EditMode"] = mode;
				pnlEdit.Visible = (mode != "view");
				pnlView.Visible = (mode == "view");
				pnlNewTemplate.Visible = (mode == "new");
				pnlEditTemplate.Visible = (mode == "edit");
			}
		}
		public string Key
		{
			get { return _key; }
			set { _key = value; }
		}
		
		private string TemplatePath
		{
			get
			{
				return MapPath("~" + TemplateSourceDirectory + @"\..\Templates\" + Key + "\\");
			}
		}

		
		protected void Page_Load(object sender, EventArgs e)
		{
			_localResourceFile = Localization.GetResourceFile(TemplateControl, TemplateControl.GetType().BaseType.Name + ".ascx");

			cmdNew.Text = Localization.GetString("cmdNew.Text", _localResourceFile);
			cmdEdit.Text = Localization.GetString("cmdEdit.Text", _localResourceFile);
			cmdSave.Text = Localization.GetString("cmdSave.Text", _localResourceFile);
			cmdCancelEdit.Text = Localization.GetString("cmdCancelEdit.Text", _localResourceFile);
			rblMode.Items[0].Text = Localization.GetString("rblMode0.Text", _localResourceFile);
			rblMode.Items[1].Text = Localization.GetString("rblMode1.Text", _localResourceFile);
			lblMode.Text = Localization.GetString("lblMode.Text", _localResourceFile);
			lblLanguage.Text = Localization.GetString("lblLanguage.Text", _localResourceFile);
			lblFileCap.Text = Localization.GetString("lblFileCap.Text", _localResourceFile);
			lblName.Text = Localization.GetString("lblName.Text", _localResourceFile);
			valNameRequired.Text = Localization.GetString("valNamerequired.Error", _localResourceFile);
			ltrHelp.Text = GetHelpText();

			string fallbackLanguage = PortalSettings.DefaultLanguage;

			if (!IsPostBack)
			{
				ReadTemplateList();

				UserInfo objUser = UserController.GetCurrentUserInfo();
				_isSuperUser = (objUser != null && objUser.IsSuperUser);
				pnlMode.Visible = _isSuperUser;
				rblMode.SelectedValue = (_isSuperUser ? "0" : "1");

				Localization.LoadCultureDropDownList(ddlLanguage, CultureDropDownTypes.NativeName, ((PageBase)Page).PageCulture.Name);
				ddlLanguage.Visible = (ddlLanguage.Items.Count > 1);
				ddlLanguage.SelectedValue = fallbackLanguage;

				LoadTemplateFile();
			}
		}

		protected void ddlTemplate_SelectedIndexChanged(object sender, EventArgs e)
		{
			Value = ddlTemplate.SelectedValue;
		}
		protected void ddlLanguage_SelectedIndexChanged(object sender, EventArgs e)
		{
			LoadTemplateFile();
		}
		protected void rblMode_SelectedIndexChanged(object sender, EventArgs e)
		{
			LoadTemplateFile();
		}
		protected void cmdNew_Click(object sender, EventArgs e)
		{
			EditMode = "new";
			txtTemplate.Text = "";
			txtName.Text = "";
			lblFile.Text = "";
		}

		protected void cmdEdit_Click(object sender, EventArgs e)
		{
			LoadTemplateFile();
			EditMode = "edit";
		}

		protected void cmdSave_Click(object sender, EventArgs e)
		{
			SaveTemplateFile();
			ReadTemplateList();
			if (EditMode == "new")
			{
				ddlTemplate.SelectedValue = txtName.Text.Trim();
				ddlTemplate_SelectedIndexChanged(ddlTemplate, new EventArgs());
			}
			else
			{
				if (ddlTemplate.Items.FindByValue(Value)!= null)
				{
					ddlTemplate.SelectedValue = Value;
					ddlTemplate_SelectedIndexChanged(ddlTemplate, new EventArgs());
				}
			}
			EditMode = "view";
		}

		protected void cmdCancelEdit_Click(object sender, EventArgs e)
		{
			EditMode = "view";
		}

		private void ReadTemplateList()
		{
			string[] files = Directory.GetFiles(TemplatePath, "*.htm");

			// We assemble all the filenames without language codes, portal infos and file extensions
			// and filter out all duplicates

			List<string> lst = new List<string>();
			foreach (string file in files)
			{
				FileInfo fi = new FileInfo(file);
				string normFile = fi.Name.Substring(0, fi.Name.IndexOf('.'));
				if (!lst.Contains(normFile) && normFile != "default")
					lst.Add(normFile);
			}

			if (lst.Count > 0)
			{
				if (!lst.Contains(Value))
				{
					Value = lst[0];
				}
				ddlTemplate.DataSource = lst;
				ddlTemplate.DataBind();
			}
			else
			{
				ddlTemplate.Visible = false;
			}
			cmdEdit.Visible = (lst.Count > 0);
		}

		private void ShowThumb()
		{
			string imageFile = TemplatePath + Value + ".jpg";
			if (File.Exists(imageFile))
				imgThumb.ImageUrl = AppRelativeTemplateSourceDirectory + @"../Templates/" + Key + "/" + Value + ".jpg";
			else
				imgThumb.ImageUrl = "";
		}
		private void LoadTemplateFile()
		{
			if (EditMode == "new")
				return;
			string currentLanguage = System.Threading.Thread.CurrentThread.CurrentUICulture.Name;
			string fallbackLanguage = PortalSettings.DefaultLanguage;
			string templateFile = ddlTemplate.SelectedValue;
			string defaultFile = Path.Combine(TemplatePath, templateFile + ".htm");
			string defaultLangFile = Path.Combine(TemplatePath, templateFile + "." + ddlLanguage.SelectedValue + ".htm");

			if (ddlLanguage.SelectedValue != fallbackLanguage)
				templateFile += "." + ddlLanguage.SelectedValue;
			if (rblMode.SelectedValue == "1")
				templateFile += ".Portal-" + PortalSettings.PortalId;
			templateFile += ".htm";
			lblFile.Text = templateFile;
			templateFile = Path.Combine(TemplatePath, templateFile);

			if (File.Exists(templateFile))
				txtTemplate.Text = File.ReadAllText(templateFile);
			else if (File.Exists(defaultLangFile))
				txtTemplate.Text = File.ReadAllText(defaultLangFile);
			else if (File.Exists(defaultFile))
				txtTemplate.Text = File.ReadAllText(defaultFile);
		}

		private void SaveTemplateFile()
		{
			string currentLanguage = System.Threading.Thread.CurrentThread.CurrentUICulture.Name;
			string fallbackLanguage = PortalSettings.DefaultLanguage;
			string templateFile;

			if (EditMode == "new")
				templateFile = txtName.Text.Trim();
			else
				templateFile = ddlTemplate.SelectedValue;

			string thumbFile = Path.Combine(TemplatePath, templateFile + ".jpg");

			if (ddlLanguage.SelectedValue != fallbackLanguage)
				templateFile += "." + ddlLanguage.SelectedValue;

			if (rblMode.SelectedValue == "1")
				templateFile += ".Portal-" + PortalSettings.PortalId;

			templateFile += ".htm";
			lblFile.Text = templateFile;
			templateFile = Path.Combine(TemplatePath, templateFile);

			File.WriteAllText(templateFile, txtTemplate.Text);
			CreateThumb(thumbFile);
			DotNetNuke.Common.Utilities.DataCache.ClearPortalCache(PortalSettings.PortalId, false);
		}

		private void CreateThumb(string thumbFile)
		{
			NewsInfo demoNews = new NewsInfo();
			demoNews.Author = "Author";
			demoNews.GUID = Guid.NewGuid().ToString();
			BBNewsController controller = new BBNewsController();
			var feeds = controller.GetFeeds(PortalSettings.PortalId);
			demoNews.FeedId = feeds[0].FeedId;
			demoNews.Hide = false;
			demoNews.Image = "http://placehold.it/140x100";
			demoNews.Internal = false;
			demoNews.Link = "http://www.bitboxx.net";
			demoNews.Pubdate = DateTime.Now;
			demoNews.Title = "Title of the news";
			demoNews.Summary = "Li Europan lingues es membres del sam familie. Lor separat existentie es un myth. Por scientie, musica, sport etc, litot Europa usa li sam vocabular. Li lingues differe solmen in li grammatica, li pronunciation e li plu commun vocabules. Omnicos directe al desirabilite de un nov lingua franca: On refusa continuar payar custosi traductores.";
			demoNews.News = "<h1>The Sound of Drums</h1> <p>Heh-haa! Super squeaky bum time! Saving the world with meals on wheels. Father Christmas. Santa Claus. Or as I've always known him: Jeff. You've swallowed a planet! It's a fez. I wear a fez now. Fezes are cool. All I've got to do is pass as an ordinary human being. Simple. What could possibly go wrong?</p> <h2>Flesh and Stone</h2> <p>They're not aliens, they're Earth&hellip;liens! You hate me; you want to kill me! Well, go on! Kill me! KILL ME! You know when grown-ups tell you 'everything's going to be fine' and you think they're probably lying to make you feel better? No&hellip; It's a thing; it's like a plan, but with more greatness. They're not aliens, they're Earth&hellip;liens! Annihilate? No. No violence. I won't stand for it. Not now, not ever, do you understand me?! I'm the Doctor, the Oncoming Storm - and you basically meant beat them in a football match, didn't you?</p> <ul> <li>I hate yogurt. It's just stuff with bits in.</li> <li>You hit me with a cricket bat.</li> <li>I'm the Doctor, I'm worse than everyone's aunt. *catches himself* And that is not how I'm introducing myself.</li> </ul> <h3>New Earth</h3> <p>I hate yogurt. It's just stuff with bits in. I am the last of my species, and I know how that weighs on the heart so don't lie to me! It's a fez. I wear a fez now. Fezes are cool. I am the last of my species, and I know how that weighs on the heart so don't lie to me!</p> <h4>The Beast Below</h4> <p>You know when grown-ups tell you 'everything's going to be fine' and you think they're probably lying to make you feel better? Aw, you're all Mr. Grumpy Face today. You hit me with a cricket bat.</p> <ol> <li>It's art! A statement on modern society, 'Oh Ain't Modern Society Awful?'!</li> <li>I'm the Doctor. Well, they call me the Doctor. I don't know why. I call me the Doctor too. I still don't know why.</li> <li>Sorry, checking all the water in this area; there's an escaped fish.</li> <li>Heh-haa! Super squeaky bum time!</li> </ol> <h5>The Lazarus Experiment</h5> <p>I'm the Doctor. Well, they call me the Doctor. I don't know why. I call me the Doctor too. I still don't know why. I'm the Doctor, I'm worse than everyone's aunt. *catches himself* And that is not how I'm introducing myself. You hit me with a cricket bat. Saving the world with meals on wheels. You've swallowed a planet! I'm the Doctor, I'm worse than everyone's aunt. *catches himself* And that is not how I'm introducing myself.</p> ";
			BBNewsTokenReplace tokenReplace = new BBNewsTokenReplace(demoNews);
			string html = tokenReplace.ReplaceBBNewsTokens(txtTemplate.Text);

			StringBuilder sb = new StringBuilder();
			sb.AppendLine("<html><head>");
			sb.AppendLine("<link href=\"file:///" + Server.MapPath("~/Portals/_default/default.css") + "\" rel=\"stylesheet\" type=\"text/css\" />");
			sb.AppendLine("<link href=\"file:///" + Server.MapPath("~/DesktopModules/BBNews/module.css") + "\" rel=\"stylesheet\" type=\"text/css\" />");
			sb.AppendLine("<link href=\"file:///" + Server.MapPath(PortalSettings.ActiveTab.SkinPath + "skin.css") + "\" rel=\"stylesheet\" type=\"text/css\" />");
			sb.AppendLine("<link href=\"file:///" + Server.MapPath(PortalSettings.HomeDirectory + "portal.css") + "\" rel=\"stylesheet\" type=\"text/css\" />");
			sb.AppendLine("</head>");
			sb.AppendLine("<body>");
			sb.Append(html);
			sb.AppendLine("</body></html>");

			AutoResetEvent resultEvent = new AutoResetEvent(false);
			IEBrowser browser = new IEBrowser(false, sb.ToString(), thumbFile, resultEvent);
			EventWaitHandle.WaitAll(new AutoResetEvent[] { resultEvent });
		}

		public string GetTemplate(string name)
		{
			string currentLanguage = System.Threading.Thread.CurrentThread.CurrentUICulture.Name;

			string portalLangFile = Path.Combine(TemplatePath,name + "." + currentLanguage + ".Portal-" + PortalSettings.PortalId.ToString() + ".htm");
			string langFile = Path.Combine(TemplatePath, name + "." + currentLanguage + ".htm");
			string portalFile = Path.Combine(TemplatePath, name + ".Portal-" + PortalSettings.PortalId.ToString() + ".htm");
			string neutralFile = Path.Combine(TemplatePath, name + ".htm");
			string defaultFile = Path.Combine(TemplatePath, "default.htm");

			if (File.Exists(portalLangFile))
				return File.ReadAllText(portalLangFile);
			else if (File.Exists(langFile))
				return File.ReadAllText(langFile);
			else if (File.Exists(portalFile))
				return File.ReadAllText(portalFile);
			else if (File.Exists(neutralFile))
				return File.ReadAllText(neutralFile);
			else if (File.Exists(defaultFile))
				return File.ReadAllText(defaultFile);
			return "";
		}

		public void SaveTemplateFile(string fileName, string template)
		{
			string currentLanguage = System.Threading.Thread.CurrentThread.CurrentUICulture.Name;
			string fallbackLanguage = PortalSettings.DefaultLanguage;
			string templateFile = Path.Combine(TemplatePath, fileName + ".htm");
			string thumbFile = Path.Combine(TemplatePath, fileName + ".jpg");

			File.WriteAllText(templateFile, template);
			CreateThumb(thumbFile);
			DotNetNuke.Common.Utilities.DataCache.ClearPortalCache(PortalSettings.PortalId, false);
		}

		private string GetHelpText()
		{
			string currentLanguage = System.Threading.Thread.CurrentThread.CurrentUICulture.Name;

			string langFile = Path.Combine(TemplatePath, "tokens." + currentLanguage + ".htp");
			string neutralFile = Path.Combine(TemplatePath, "tokens.htp");

			if (File.Exists(langFile))
				return File.ReadAllText(langFile);
			else if (File.Exists(neutralFile))
				return File.ReadAllText(neutralFile);
			return "";

		}
	}
}