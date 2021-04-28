using System;
using System.Collections.Generic;
using System.Linq;
using GTFS;
using GTFS.Entities;
using GTFS_Explorer.BackEnd.Extensions;
using Nixill.Collections;
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
                    st.Latitude < minLat ||
                    st.Latitude > maxLat ||
                    st.Longitude < minLon ||
                    st.Longitude > maxLon
                ) return false;

                if (GIS.MetersBetween(st.Latitude, st.Longitude, stop.Latitude, stop.Longitude) > 1000) return false;

                return true;
            }).OrderBy(st => GIS.MetersBetween(st.Latitude, st.Longitude, stop.Latitude, stop.Longitude)).Skip(1).Take(10);

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
                if (lastTime != null && lastTime.TripId == time.TripId)
                {
                    if (lastTime.StopId == stopId) yield return time.StopId;
                    if (time.StopId == stopId) yield return lastTime.StopId;
                }

                lastTime = time;
            }
        }

        public static Dictionary<Route, List<StopTime>> GetStopSchedule(GTFSFeed feed, string stopId, LocalDate date)
        {
            Tuple<List<string>, bool> services = RouteLookups.ServicesOn(feed, date);

            GeneratorDictionary<string, Route> routes = new GeneratorDictionary<string, Route>(
                new FuncGenerator<string, Route>(x => feed.Routes.Get(feed.Trips.Get(x).RouteId))
            );

            var orderedStopTimes = feed.StopTimes
                .Where(x => x.StopId == stopId && services.Item1.Contains(feed.Trips.Get(x.TripId).ServiceId))
                .OrderBy(stm => new Tuple<string, TimeOfDay>(
                    routes[stm.TripId].Id,
                    stm.ArrivalTime.HasValue ? stm.ArrivalTime.Value : TimeOfDay.FromTotalSeconds(1000000)));

            GeneratorDictionary<Route, List<StopTime>> times = new GeneratorDictionary<Route, List<StopTime>>(
                new FuncGenerator<Route, List<StopTime>>(x => new List<StopTime>())
            );

            foreach (var time in orderedStopTimes)
            {
                times[routes[time.TripId]].Add(time);
            }

            return times;
        }
    }
}