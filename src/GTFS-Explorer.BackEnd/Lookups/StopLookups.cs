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

        public static IEnumerable<Stop> GetNearbyStops(GTFSFeed feed, string stopId)
        {
            Stop stop = feed.Stops.Get(stopId);

            // Stops within one kilometer
            double lonWidth = GIS.DegreesInOneKm(stop.Latitude);
            double minLon = stop.Longitude - lonWidth;
            double maxLon = stop.Longitude + lonWidth;
            double minLat = stop.Latitude - 0.00888888889;
            double maxLat = stop.Latitude + 0.00888888889;

            var stopsWithin1km = feed.Stops.Where(st =>
            {
                if (
                    st.Latitude < minLon ||
                    st.Latitude > maxLon ||
                    st.Longitude < minLon ||
                    st.Longitude > maxLon
                ) return false;

                if (GIS.MetersBetween(st.Latitude, st.Longitude, stop.Latitude, stop.Longitude) > 1000) return false;

                return true;
            });

            // Adjacent stops on lines that serve it
            var adjacentStops = GetAdjacentStops(feed, stopId).Distinct().Select(id => feed.Stops.Get(id));

            // And output
            return stopsWithin1km.Union(adjacentStops).Distinct();
        }

        private static IEnumerable<string> GetAdjacentStops(GTFSFeed feed, string stopId)
        {
            var orderedStopTimes = feed.StopTimes.OrderBy(stm => new Tuple<string, uint>(stm.TripId, stm.StopSequence));
            StopTime lastTime = null;

            foreach (StopTime time in orderedStopTimes)
            {
                if (lastTime != null)
                {
                    if (lastTime.TripId == time.TripId && lastTime.StopId == stopId) yield return time.StopId;
                }
            }
        }
    }
}