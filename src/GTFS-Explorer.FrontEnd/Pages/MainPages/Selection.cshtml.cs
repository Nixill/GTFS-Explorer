using ElectronNET.API;
using GTFS.Entities;
using GTFS_Explorer.BackEnd.Readers;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Globalization;
using System.Linq;
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

        public FeedInfo FeedInfo { get; set; }
        public DateTime FeedStartDate { get; set; }
        public DateTime FeedEndDate { get; set; }

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
                    FeedStartDate =
                        DateTime.ParseExact(FeedInfo.StartDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                    FeedEndDate =
                        DateTime.ParseExact(FeedInfo.EndDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
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