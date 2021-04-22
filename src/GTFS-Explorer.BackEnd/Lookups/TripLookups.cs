using GTFS;
using GTFS.Entities;
using GTFS.Entities.Enumerations;
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
        public static List<Tuple<Stop, TimeOfDay?, TimeOfDay?, bool, PickupType, DropOffType>> GetStopsFromTrip(GTFSFeed feed, string tripId)
        {
            var stopTimes = feed.StopTimes
                .Where(x => x.TripId == tripId)
                .OrderBy(x => x.StopSequence)
                .Select(x =>
                {
                    var stop = feed.Stops.Get(x.StopId);
                    var arrTime = x.ArrivalTime;
                    var depTime = x.DepartureTime;
                    bool isTimepoint = (x.TimepointType == TimePointType.Exact || (x.TimepointType == TimePointType.None && x.ArrivalTime != null));
                    var pickup = x.PickupType ?? PickupType.Regular;
                    var dropoff = x.DropOffType ?? DropOffType.Regular;

                    return new Tuple<Stop, TimeOfDay?, TimeOfDay?, bool, PickupType, DropOffType>(
                        stop, arrTime, depTime, isTimepoint, pickup, dropoff
                    );
                });

            return stopTimes.ToList();
        }
    }
}