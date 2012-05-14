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


using System.Drawing;
using System.Drawing.Imaging;
using System.Threading;
using System.Windows.Forms;

public class IEBrowser : ApplicationContext
{
    string File;
    string Html;
    AutoResetEvent ResultEvent;

    public IEBrowser(bool visible, string html, string file, AutoResetEvent resultEvent)
    {
        ResultEvent = resultEvent;
        Thread thrd = new Thread(new ThreadStart(
            delegate {
                Init(visible,html, file);
                System.Windows.Forms.Application.Run(this);
            })); 
        // set thread to STA state before starting
        thrd.SetApartmentState(ApartmentState.STA);
        thrd.Start(); 
    }

    private void Init(bool visible,string html, string file)
    {
        // create a WebBrowser control
        WebBrowser ieBrowser = new WebBrowser();
        ieBrowser.ScrollBarsEnabled = false;
        ieBrowser.ScriptErrorsSuppressed = true;
        
        // set WebBrowser event handle
        ieBrowser.DocumentCompleted += IEBrowser_DocumentCompleted;
 
        ieBrowser.Navigate("about:blank");
        File = file;
        Html = html;

    }

    // DocumentCompleted event handle
    void IEBrowser_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
    {
        try
        {
            WebBrowser browser = (WebBrowser)sender;
            HtmlDocument doc = browser.Document;
            doc.OpenNew(true);
            doc.Write(Html);
            browser.Width = doc.Body.ScrollRectangle.Width;
            browser.Height = doc.Body.ScrollRectangle.Height;

            Bitmap bitmap = new Bitmap(browser.Width, browser.Height);
            browser.DrawToBitmap(bitmap, new Rectangle(0, 0, browser.Width, browser.Height));
            browser.Dispose();

            bitmap.Save(File, ImageFormat.Jpeg);
        }
        catch (System.Exception)
        {
        }
        finally
        {
            ResultEvent.Set();
        }

    }
}