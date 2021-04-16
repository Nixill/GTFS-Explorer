using System.Collections.Generic;
using System.Linq;
using GTFS;
using GTFS.Entities;

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
    }
}