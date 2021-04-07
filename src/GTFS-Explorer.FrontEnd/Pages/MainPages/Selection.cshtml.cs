using ElectronNET.API;
using GTFS.Entities;
using GTFS_Explorer.BackEnd.Readers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.RegularExpressions;

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

			if(!string.IsNullOrEmpty(FeedInfo.StartDate) &&
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
}