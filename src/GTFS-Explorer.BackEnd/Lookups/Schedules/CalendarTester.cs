using System.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using GTFS;
using GTFS.Entities;
using GTFS.Entities.Enumerations;

namespace Nixill.GTFS
{
  public static class CalendarTester
  {
    public static string GetDescription(GTFSFeed feed, string serviceId, Tuple<DateTime, DateTime> dateRange)
    {
      Calendar cal = feed.Calendars.Get(serviceId).FirstOrDefault();
      IEnumerable<CalendarDate> calDates = feed.CalendarDates.Where(x => x.ServiceId == serviceId);

      if (cal == null && !calDates.Any(x => x.ExceptionType == ExceptionType.Added)) return $"{serviceId} does not exist.";

      if (dateRange == null) dateRange = GetFeedDateRange(feed);

      StringBuilder output = new StringBuilder();

      if (cal != null && cal.Mask != 0)
      {
        output.Append($"Operates {DayMasks.Get(cal.Mask)}");

        var rangeOut = RangeExceedsCalendar(cal, dateRange.Item1, dateRange.Item2);

        if (rangeOut.Item1 && rangeOut.Item2) output.Append($", from {cal.StartDate.ToString("yyyy-MM-dd")} to {cal.EndDate.ToString("yyyy-MM-dd")}.");
        else if (rangeOut.Item1) output.Append($", starting {cal.StartDate.ToString("yyyy-MM-dd")}.");
        else if (rangeOut.Item2) output.Append($", until {cal.EndDate.ToString("yyyy-MM-dd")}.");
        else output.Append(".");

        var addedDates = calDates.Where(x => x.ExceptionType == ExceptionType.Added).Select(x => x.Date).ToList();
        var removedDates = calDates.Where(x => x.ExceptionType == ExceptionType.Removed).Select(x => x.Date).ToList();

        if (addedDates.Count > 0)
        {
          output.Append(" Also ");
          output.Append(EnglishList(addedDates, x => x.ToString("yyyy-MM-dd")));
          output.Append(".");
        }

        if (removedDates.Count > 0)
        {
          output.Append(" Excludes ");
          output.Append(EnglishList(removedDates, x => x.ToString("yyyy-MM-dd")));
          output.Append(".");
        }
      }
      else
      {
        var addedDates = calDates.Where(x => x.ExceptionType == ExceptionType.Added).Select(x => x.Date).ToList();
        output.Append("Operates ");
        output.Append(EnglishList(addedDates, x => x.ToString("yyyy-MM-dd")));
        output.Append(".");
      }

      return output.ToString();
    }

    public static Tuple<DateTime, DateTime> GetFeedDateRange(GTFSFeed feed)
    {
      FeedInfo info = feed.GetFeedInfo();

      if (info.StartDate != null && info.EndDate != null)
      {
        return new Tuple<DateTime, DateTime>(
          DateTime.ParseExact(info.StartDate, "yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture),
          DateTime.ParseExact(info.EndDate, "yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture));
      }

      DateTime? minStartLeft = null;
      DateTime? minStartRight = null;
      DateTime? maxEndLeft = null;
      DateTime? maxEndRight = null;

      DateTime minStart;
      DateTime maxEnd;

      if (feed.Calendars.Any())
      {
        minStartLeft = feed.Calendars.Min(x => x.StartDate);
        maxEndLeft = feed.Calendars.Max(x => x.EndDate);
      }

      if (feed.CalendarDates.Any())
      {
        minStartRight = feed.CalendarDates.Where(x => x.ExceptionType == ExceptionType.Added).Min(x => x.Date);
        maxEndRight = feed.CalendarDates.Where(x => x.ExceptionType == ExceptionType.Added).Max(x => x.Date);
      }

      if (minStartLeft.HasValue && minStartRight.HasValue)
      {
        if (minStartLeft > minStartRight) minStart = minStartRight.Value;
        else minStart = minStartLeft.Value;
      }
      else
      {
        if (minStartLeft.HasValue) minStart = minStartLeft.Value;
        else minStart = minStartRight.Value;
      }

      if (maxEndLeft.HasValue && maxEndRight.HasValue)
      {
        if (maxEndLeft > maxEndRight) maxEnd = maxEndLeft.Value;
        else maxEnd = maxEndLeft.Value;
      }
      else
      {
        if (maxEndLeft.HasValue) maxEnd = maxEndLeft.Value;
        else maxEnd = maxEndRight.Value;
      }

      return new Tuple<DateTime, DateTime>(minStart, maxEnd);
    }

    public static Tuple<bool, bool> RangeExceedsCalendar(Calendar cal, DateTime start, DateTime end)
    {
      bool startOut = false;
      bool endOut = false;

      // First figure out the start
      // Quick shortcuts
      if (start >= cal.StartDate) startOut = false;
      else if (start <= (cal.StartDate.AddDays(-7))) startOut = true;
      else
      {
        // The long way
        for (DateTime date = start; date < cal.StartDate; date = date.AddDays(1))
        {
          if (cal[date.DayOfWeek])
          {
            startOut = true;
            break;
          }
        }
      }

      // Now figure out the end
      // Quick shortcuts
      if (end <= cal.EndDate) endOut = false;
      else if (end >= (cal.StartDate.AddDays(7))) startOut = true;
      else
      {
        // The long way
        for (DateTime date = end; date > cal.EndDate; date = date.AddDays(-1))
        {
          if (cal[date.DayOfWeek])
          {
            endOut = true;
            break;
          }
        }
      }

      return new Tuple<bool, bool>(startOut, endOut);
    }

    public static string EnglishList<T>(List<T> items) => EnglishList(items, x => x.ToString());

    public static string EnglishList<T>(List<T> items, Func<T, string> toString)
    {
      if (items.Count == 0) return "";
      else if (items.Count == 1) return toString(items[0]);
      else if (items.Count == 2) return toString(items[0]) + " and " + toString(items[1]);
      else
      {
        StringBuilder ret = new StringBuilder();
        for (int i = 0; i < items.Count - 1; i++)
        {
          ret.Append(toString(items[i]));
          ret.Append(", ");
        }

        ret.Append("and ");
        ret.Append(toString(items[items.Count - 1]));

        return ret.ToString();
      }
    }
  }
}