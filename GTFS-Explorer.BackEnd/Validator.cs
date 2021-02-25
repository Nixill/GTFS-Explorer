using System;
using System.IO.Compression;

namespace GTFSExplorer.Backend
{
    public class Validator
    {
        public static Tuple<bool, string> IsValidGTFS(string filePath)
        {
            try
            {
                using ZipArchive file = ZipFile.OpenRead(filePath);

                var files = file.Entries;

                if (!files.Contains("agency.txt")) return new Tuple<bool, string>(false, "No agency.txt file");
                if (!files.Contains("stops.txt")) return new Tuple<bool, string>(false, "No stops.txt file");
                if (!files.Contains("routes.txt")) return new Tuple<bool, string>(false, "No routes.txt file");
                if (!files.Contains("trips.txt")) return new Tuple<bool, string>(false, "No trips.txt file");
                if (!files.Contains("stop_times.txt")) return new Tuple<bool, string>(false, "No stop_times.txt file");
                if (!(files.Contains("calendar.txt") || files.Contains("calendar_dates.txt"))) return new Tuple<bool, string>(false, "No calendar.txt or calendar_dates.txt file");

                return new Tuple<bool, string>(true, "");
            }
            catch (InvalidDataException ex)
            {
                return new Tuple<bool, string>(false, "Not a zip file");
            }
            catch (Exception ex)
            {
                return new Tuple<bool, string>(false, "Unknown error occurred");
            }
        }
    }
}