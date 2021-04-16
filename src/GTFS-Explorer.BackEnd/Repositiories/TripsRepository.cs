using GTFS.Entities;
using GTFS.Entities.Enumerations;
using GTFS_Explorer.BackEnd.Readers;
using GTFS_Explorer.Core.Interfaces.RepoInterfaces;
using System.Collections.Generic;
using System.Linq;

namespace GTFS_Explorer.BackEnd.Repositiories
{
    public class TripsRepository : ITripsRepository
    {
        private readonly GTFSFeedReader _reader;

        public TripsRepository(GTFSFeedReader reader)
        {
            _reader = reader;
        }

        public IEnumerable<DirectionType?> GetDirectionsOfRoute(string routeId)
        {
            var allTrips = GetAllTripsOfRoute(routeId);
            return allTrips.Select(x => x.Direction).Distinct();
        }

        public IEnumerable<Trip> GetAllTripsOfRoute(string routeId)
        {
            return _reader.Feed.Trips.Where(x => x.RouteId == routeId);
        }
    }
}