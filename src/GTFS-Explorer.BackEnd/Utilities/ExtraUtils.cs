using System.Text.RegularExpressions;
using GTFS.Entities;
using NodaTime;
using NodaTime.Text;

namespace GTFS_Explorer.BackEnd.Utilities
{
    public static class ExtraUtils
    {
        public static Duration DurationFromTimeOfDay(TimeOfDay time)
        {
            return Duration.FromSeconds(time.TotalSeconds);
        }

        private static Regex TimeFormat = new Regex(@"(\d+):(\d\d):(\d\d)");

        public static string RemoveSeconds(string before)
        {
            var match = TimeFormat.Match(before);
            string hr = match.Groups[1].Value;
            string min = match.Groups[2].Value;
            return $"{hr}:{min}";
        }
    }
}