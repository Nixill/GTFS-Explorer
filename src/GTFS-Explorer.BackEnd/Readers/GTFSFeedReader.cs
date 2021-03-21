using ElectronNET.API;
using GTFS;
using GTFS_Explorer.BackEnd.Extensions;
using Microsoft.AspNetCore.Hosting;
using System.IO;

namespace GTFS_Explorer.BackEnd.Readers
{
    public class GTFSFeedReader
    {
        public GTFSFeed Feed;
        private readonly IWebHostEnvironment _env;

        public GTFSFeedReader(IWebHostEnvironment env)
        {
            _env = env;
        }

        public void ReadFeed()
		{
            var dir = Electron.App.CreateGTFSFileDir(_env);
            string[] files = Directory.GetFiles(dir);

            if (files != null)
            {
                var filePath = Electron.App.GetGTFSFilePath(_env, files[0]);

                var reader = new GTFSReader<GTFSFeed>();
                Feed = reader.ReadFeed(filePath);
            }
        }
    }
}