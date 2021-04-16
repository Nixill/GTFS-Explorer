using System;
using System.Collections.Generic;
using System.Linq;
using GTFS;
using GTFS.Entities;
using GTFS.Entities.Enumerations;
using GTFS_Explorer.Core.Models.Structs;
using Nixill.Collections;
using NodaTime;

namespace GTFS_Explorer.BackEnd.Lookups
{
    public static class Lists
    {
        /// <summary>
        /// Returns a <c>Dictioary</c> containing all the routes in the feed,
        /// separated by agency.
        /// </summary>
        /// <param name="feed">The GTFS feed to use.</param>
        public static Dictionary<Agency, List<Route>> GetAllRoutes(GTFSFeed feed)
        {
            GeneratorDictionary<Agency, List<Route>> dict = new GeneratorDictionary<Agency, List<Route>>(new FuncGenerator<Agency, List<Route>>(x => new List<Route>()));

            var agencies = feed.Agencies;

            foreach (Route route in feed.Routes)
            {
                if (route.Id != null && route.Id != "")
                {
                    if (route.AgencyId == null) route.AgencyId = "";
                    dict[agencies.Get(route.AgencyId)].Add(route);
                }
            }

            foreach (List<Route> list in dict.Values)
            {
                list.Sort((l, r) =>
                {
                    return (l.LongName ?? l.ShortName ?? l.Id).CompareTo(r.LongName ?? r.ShortName ?? r.Id);
                });
            }

            return dict;
        }

        /// <summary>
        /// Returns a <c>Dictionary</c> containing all the stops in the feed,
        /// along with how major a stop it is.
        /// </summary>
        /// <param name="feed">The GTFS feed to use.</param>
        public static List<Stop> GetAllStops(GTFSFeed feed)
        {
            List<Stop> list = new List<Stop>();

            var stops = feed.Stops;

            // First, we need to add platforms with no parent station
            list.AddRange(
              from stop in stops
              where stop.LocationType == LocationType.Stop && stop.ParentStation == null
              select stop
            );

            // Second, add whole stations
            list.AddRange(
              from stop in stops
              where stop.LocationType == LocationType.Station
              select stop
            );

            return list;
        }

        public static DateRange GetServiceRange(GTFSFeed feed)
        {
            List<DateRange> ranges = new List<DateRange>();

            // Range from feed.Calendars
            if (feed.Calendars.Any())
            {
                LocalDate min = LocalDate.MaxIsoValue;
                LocalDate max = LocalDate.MinIsoValue;

                foreach (Calendar cal in feed.Calendars)
                {
                    min = LocalDate.Min(LocalDate.FromDateTime(cal.StartDate), min);
                    max = LocalDate.Max(LocalDate.FromDateTime(cal.EndDate), max);
                }

                ranges.Add(new DateRange(min, max));
            }

            // Range from feed.CalendarDates
            if (feed.CalendarDates.Any())
            {
                LocalDate min = LocalDate.MaxIsoValue;
                LocalDate max = LocalDate.MinIsoValue;

                foreach (CalendarDate cal in feed.CalendarDates.Where(x => x.ExceptionType == ExceptionType.Added))
                {
                    min = LocalDate.Min(LocalDate.FromDateTime(cal.Date), min);
                    max = LocalDate.Max(LocalDate.FromDateTime(cal.Date), max);
                }

                ranges.Add(new DateRange(min, max));
            }

            if (ranges.Count == 2)
            {
                return ranges[0] + ranges[1];
            }
            else
            {
                return ranges[0];
            }
        }
    }
}
