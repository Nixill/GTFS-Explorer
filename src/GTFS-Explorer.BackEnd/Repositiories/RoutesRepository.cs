using GTFS;
using GTFS.Entities;
using GTFS_Explorer.Core.Interfaces.RepoInterfaces;
using System.Collections.Generic;
using Nixill.Collections;
using System.Linq;

namespace GTFS_Explorer.BackEnd.Readers
{
    public class RoutesRepository : IRoutesRepository
    {
        private readonly GTFSFeedReader _feedReader;

        public RoutesRepository(GTFSFeedReader feedReader)
        {
            _feedReader = feedReader;
        }

        public Dictionary<Agency, List<Route>> GetAllRoutes()
        {
            GeneratorDictionary<Agency, List<Route>> dict =
                new GeneratorDictionary<Agency, List<Route>>(
                    new FuncGenerator<Agency, List<Route>>(x => new List<Route>()));

            var agencies = _feedReader.Feed.Agencies;

            foreach (Route route in _feedReader.Feed.Routes)
            {
                dict[agencies.Get(route.AgencyId)].Add(route);
            }

            return dict;
        }

        public Route GetRoute(int id)
        {
            return _feedReader.Feed.Routes.Get(id);
        }

        public List<Route> GetRoutesList()
        {
            return _feedReader.Feed.Routes.ToList();
        }
    }
}