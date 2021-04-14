using ElectronNET.API;
using GTFS.Entities;
using GTFS_Explorer.BackEnd.Readers;
using GTFS_Explorer.BackEnd.SignalR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.SignalR;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace GTFS_Explorer.FrontEnd.Pages.MainPages
{
    public class SelectionModel : PageModel
    {
        private readonly GTFSFeedReader _reader;

		public SelectionModel(GTFSFeedReader reader)
		{
			_reader = reader;
		}

		[BindProperty]
		public FeedInfo FeedInfo { get; set; }

		public void OnGet()
        {
			FeedInfo = _reader.Feed.GetFeedInfo();
			if (AreFeedInfoPropsNull(FeedInfo))
				FeedInfo = null;
			else
			{
				if (!string.IsNullOrEmpty(FeedInfo.StartDate) &&
			   !string.IsNullOrEmpty(FeedInfo.EndDate))
				{
					FeedInfo.StartDate =
						Regex.Replace(FeedInfo.StartDate, @"^(....)(..)(..)$", "$1-$2-$3");
					FeedInfo.EndDate =
						Regex.Replace(FeedInfo.EndDate, @"^(....)(..)(..)$", "$1-$2-$3");
				}

				Electron.IpcMain.On("open-publisher-url", async (args) =>
				{
					await Electron.Shell.OpenExternalAsync(FeedInfo.PublisherUrl);
				});
			}
		}

		private bool AreFeedInfoPropsNull(FeedInfo feedInfo)
		{
			return feedInfo.GetType().GetProperties()
				.Where(p => p.PropertyType == typeof(string))
				.Select(p => (string)p.GetValue(feedInfo))
				.Any(value => string.IsNullOrEmpty(value));
		}
    }
}