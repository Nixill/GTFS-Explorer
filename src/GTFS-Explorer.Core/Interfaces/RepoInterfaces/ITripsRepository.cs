using GTFS.Entities;
using GTFS.Entities.Enumerations;
using System.Collections.Generic;

namespace GTFS_Explorer.Core.Interfaces.RepoInterfaces
{
    public interface ITripsRepository
    {
        IEnumerable<Trip> GetAllTripsOfRoute(string routeId);
        IEnumerable<DirectionType?> GetDirectionsOfRoute(string routeId);
    }
}