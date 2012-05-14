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
using System.Collections.Generic;
using Bitboxx.DNNModules.BBNews.Components;
using DotNetNuke.Services.Scheduling;

namespace Bitboxx.DNNModules.BBNews
{
	public class BBNewsScheduler : SchedulerClient
	{
		public BBNewsScheduler(ScheduleHistoryItem oItem)
			: base()
		{
			this.ScheduleHistoryItem = oItem;
		}

		public override void DoWork()
		{
			try
			{
				//Mark as progressing
				this.Progressing();
		
				BBNewsController controller = new BBNewsController();

				// Check feeds
				this.ScheduleHistoryItem.AddLogNote("Start Task: <br/ >");
				List<FeedInfo> AllFeeds = controller.GetFeeds();
				foreach (FeedInfo feed in AllFeeds)
				{
					if (feed.Enabled && (feed.LastRetrieve == DateTime.MinValue ||
						(DateTime.Now > feed.LastRetrieve + new TimeSpan(0, 0, feed.RetrieveInterval) &&
						 DateTime.Now > feed.LastTry + new TimeSpan(0, 0, feed.TryInterval))))
						controller.ReadFeed(feed.FeedId);
					
					controller.ReorgNews(feed.FeedId);
				}

				//Note
				this.ScheduleHistoryItem.AddLogNote("Task Completed:");
				
				//Show success
				this.ScheduleHistoryItem.Succeeded = true;
			}
			catch (Exception ex)
			{
				this.ScheduleHistoryItem.Succeeded = false;
				this.ScheduleHistoryItem.AddLogNote("Exception: "+ ex.Message);
				this.Errored(ref ex);
				DotNetNuke.Services.Exceptions.Exceptions.LogException(ex);
			}
		}
	}
}