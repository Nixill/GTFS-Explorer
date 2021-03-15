using GTFS;
using GTFS.Entities;
using System.Collections.Generic;

namespace GTFS_Explorer.Core.Interfaces.RepoInterfaces
{
    public interface IRoutesRepository
    {
        Dictionary<Agency, List<Route>> GetAllRoutes();
        Route GetRouteById(string id);
        List<Route> GetRoutesList();
    }
}