using GTFS;
using GTFS.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GTFS_Explorer.BackEnd.Lookups
{
	public class TripLookups
	{
        /// <summary>
        /// Finds stop times with their respective Stop of any trip
        /// </summary>
        /// <param name="feed">The feed to read from</param>
        /// <param name="tripId">The trip id you want to get the stops from</param>
        /// <returns>A list of tuples where Item1 is the arrival time of the stop
        ///     and Item2 is the Stop</returns>
        public static List<Tuple<TimeOfDay?, Stop>> GetStopsFromTrip(GTFSFeed feed, string tripId)
        {
            var stopTimes = from stopTime in feed.StopTimes
                            where stopTime.TripId == tripId
                            join stop in feed.Stops on stopTime.StopId equals stop.Id
                            select new Tuple<TimeOfDay?, Stop>(stopTime.ArrivalTime, stop);

            return stopTimes.ToList();
        }
    }
}