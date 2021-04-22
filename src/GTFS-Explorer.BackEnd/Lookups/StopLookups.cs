using System;
using System.Collections.Generic;
using System.Linq;
using GTFS;
using GTFS.Entities;
using NodaTime;

namespace GTFS_Explorer.BackEnd.Lookups
{
    public static class StopLookups
    {
        public static IEnumerable<Route> GetRoutesServingStop(GTFSFeed feed, string stopId)
        {
            var routeEnumerable =
                from stopTime in feed.StopTimes
                where stopTime.StopId == stopId
                join trip in feed.Trips on stopTime.TripId equals trip.Id
                join route in feed.Routes on trip.RouteId equals route.Id
                select route;

            return routeEnumerable.Distinct();
        }

        public static IEnumerable<StopTime> GetStopSchedule(GTFSFeed feed, string stopId, string routeId, LocalDate day)
        {
            var services = RouteLookups.ServicesOn(feed, day);

            var tripsForRoute = feed.Trips
                .Where(x => x.RouteId == routeId && services.Item1.Contains(x.ServiceId))
                .Select(x => x.Id);

            var stopTimesForRoute = feed.StopTimes
                .Where(x => x.StopId == stopId && tripsForRoute.Contains(x.TripId));

            return stopTimesForRoute;
        }
    }
}