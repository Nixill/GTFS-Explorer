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

        public GTFSFeedReader(IWebHostEnvironment env)
        {
            var dir = Electron.App.GetGTFSFileDir(env);
            string[] files = Directory.GetFiles(dir);

            if (files != null)
            {
                var filePath = Electron.App.GetGTFSFilePath(env, files[0]);

                var reader = new GTFSReader<GTFSFeed>();
                Feed = reader.ReadFeed(filePath);
            }
        }
    }
}