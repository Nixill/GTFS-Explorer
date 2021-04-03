using GTFS.Entities;
using GTFS.Entities.Enumerations;
using GTFS_Explorer.Core.Enums;
using System.Collections.Generic;
using Nixill.Collections.Grid;

namespace GTFS_Explorer.Core.Interfaces.RepoInterfaces
{
    public interface IRoutesRepository
    {
        Dictionary<Agency, List<Route>> GetAllRoutes();
        Route GetRouteById(string id);
        List<Route> GetRoutesList();
        Grid<string> GetSchedule(string route, DirectionType? dir, string serviceId, TimepointStrategy strat);
    }
}