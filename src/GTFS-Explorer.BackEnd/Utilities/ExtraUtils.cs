using System.Globalization;
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

        public static string TimeOfDayToString(TimeOfDay timeOfDay)
        {
            int time = timeOfDay.TotalSeconds;
            return NodaTime.LocalTime.FromSecondsSinceMidnight(time).ToString("HH:mm", DateTimeFormatInfo.InvariantInfo);
        }

        public static string DurationToTimeOfDay(Duration dur)
        {
            string timePart = (dur.Minus(Duration.FromDays((int)(dur.TotalDays)))).ToString("HH:mm", DateTimeFormatInfo.InvariantInfo);
            string dayPart = "";
            if (dur.TotalDays >= 2)
            {
                dayPart = $" (+{dur.TotalDays} days)";
            }
            else if (dur.TotalDays >= 1)
            {
                dayPart = " (the next day)";
            }
            return timePart + dayPart;
        }

        public static string DurationToLengthString(Duration dur)
        {
            int duration = (int)dur.TotalSeconds;

            int secs, mins, hrs;

            if (duration < 300)
            {
                secs = duration % 60;
            }
            else
            {
                secs = 0;
                duration += 30; // For rounding
            }

            duration /= 60;

            mins = duration % 60;
            hrs = duration / 60;

            string durDesc = "";

            if (hrs > 1)
            {
                durDesc += $" {hrs} hours";
            }
            else if (hrs == 1)
            {
                durDesc += " 1 hour";
            }

            if (mins > 1)
            {
                durDesc += $" {mins} minutes";
            }
            else if (mins == 1)
            {
                durDesc += " 1 minute";
            }

            if (secs > 1)
            {
                durDesc += $" {secs} seconds";
            }
            else if (secs == 1)
            {
                durDesc += " 1 second";
            }

            return durDesc[1..];
        }
    }
}