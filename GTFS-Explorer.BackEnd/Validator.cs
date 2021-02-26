using System;
using System.IO;
using System.IO.Compression;
using System.Linq;

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

                if (file.GetEntry("agency.txt") == null) return new Tuple<bool, string>(false, "No agency.txt file");
                if (file.GetEntry("stops.txt") == null) return new Tuple<bool, string>(false, "No stops.txt file");
                if (file.GetEntry("routes.txt") == null) return new Tuple<bool, string>(false, "No routes.txt file");
                if (file.GetEntry("trips.txt") == null) return new Tuple<bool, string>(false, "No trips.txt file");
                if (file.GetEntry("stop_times.txt") == null) return new Tuple<bool, string>(false, "No stop_times.txt file");
                if ((file.GetEntry("calendar.txt") == null) || file.GetEntry("calendar_dates.txt") == null)
                        return new Tuple<bool, string>(false, "No calendar.txt or calendar_dates.txt file");

                return new Tuple<bool, string>(true, "");
            }
            catch (InvalidDataException)
            {
                return new Tuple<bool, string>(false, "Not a zip file");
            }
            catch (Exception)
            {
                return new Tuple<bool, string>(false, "Unknown error occurred");
            }
        }
    }
}