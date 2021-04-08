using NodaTime;
using System.Collections.Generic;

namespace GTFS_Explorer.Core.Models.Structs
{
	public struct RouteStats
	{
        // Line 1
        public Duration StartTime;
        public Duration EndTime;
        // Line 2
        public List<string> StartStops;
        // Line 3
        public List<string> EndStops;
        // Line 4
        public Duration ShortestTrip;
        public Duration LongestTrip;
        public Duration AverageTrip;
        public int TotalTrips;
    }
}