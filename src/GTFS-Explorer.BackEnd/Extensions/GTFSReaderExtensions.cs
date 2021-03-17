using GTFS;
using GTFS.IO;
using GTFS_Explorer.BackEnd.Extensions.GTFSExtra;
using System;
using System.IO;

namespace GTFS_Explorer.BackEnd.Extensions
{
    public static class GTFSReaderExtensions
    {
        /// <summary>
        /// Reads a GTFS feed.
        /// From: https://github.com/itinero/GTFS/blob/3e975d9914196fff0433cb401dddf6e8b748e68b/src/GTFS/GTFSReaderExtensions.cs
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="path">The path, this has to be either a folder or a zip archive.</param>
        /// <param name="separator">A custom separator.</param>
        /// <typeparam name="T">The GTFS feed type.</typeparam>
        /// <returns>The GTFS feed.</returns>
        public static T Read<T>(this GTFSReader<T> reader, string path, char? separator = null) where T : IGTFSFeed, new()
        {
            if (path == null) throw new ArgumentNullException(nameof(path));

            if (Directory.Exists(path))
            {
                using (var source = new GTFSDirectorySource(path, (char)separator))
                {
                    return reader.Read<T>(source);
                }
            }
            else if (File.Exists(path) && path.ToLower().EndsWith(".zip"))
            {
                using (var source = new GTFSArchiveSource(File.OpenRead(path), separator))
                {
                    return reader.Read<T>(source);
                }
            }

            throw new ArgumentException("Could not open GTFS feed, directory or archive not found.", nameof(path));
        }
    }
}