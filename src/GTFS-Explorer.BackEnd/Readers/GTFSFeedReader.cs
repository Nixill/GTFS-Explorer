using ElectronNET.API;
using GTFS;
using GTFS.IO;
using GTFS_Explorer.BackEnd.Extensions;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using System.IO.Compression;
using System.Linq;

namespace GTFS_Explorer.BackEnd.Readers
{
    public class GTFSFeedReader
    {
        public GTFSFeed Feed;

        public GTFSFeedReader(IWebHostEnvironment env)
        {
            var dir = Electron.App.GetGTFSFileDir(env);
            string[] files = Directory.GetFiles(dir);
            var file = Path.GetFileNameWithoutExtension(files[0]);
            var destDir = Directory.CreateDirectory(Path.Combine(dir, file));

            if(!Directory.EnumerateFileSystemEntries(destDir.FullName).Any())
            {
                ZipFile.ExtractToDirectory(files[0], destDir.FullName);
            }

            var directoryInfo = new DirectoryInfo(destDir.FullName);
            var gtfsDirectorySource = new GTFSDirectorySource(directoryInfo);
            var reader = new GTFSReader<GTFSFeed>();
            Feed = reader.Read(gtfsDirectorySource);
        }
    }
}