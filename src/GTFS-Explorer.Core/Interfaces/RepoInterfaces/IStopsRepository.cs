using GTFS.Entities;
using GTFS.Entities.Enumerations;
using System;
using System.Collections.Generic;

namespace GTFS_Explorer.Core.Interfaces.RepoInterfaces
{
    public interface IStopsRepository
    {
        List<Stop> GetAllStops();
        IEnumerable<Stop> GetStopOrder(string route, DirectionType? direction = null);
        Stop GetStopById(string stopId);
        List<Stop> GetStopList();
        List<Tuple<Stop, TimeOfDay?, TimeOfDay?, bool, PickupType, DropOffType>> GetStopsFromTrip(string tripId);
    }
}