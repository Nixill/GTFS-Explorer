using GTFS.Entities;
using GTFS_Explorer.Core.Interfaces.RepoInterfaces;
using System.Collections.Generic;
using Nixill.Collections;
using System.Linq;
using GTFS_Explorer.BackEnd.Readers;

namespace GTFS_Explorer.BackEnd.Repositiories
{
    public class RoutesRepository : IRoutesRepository
    {
        private readonly GTFSFeedReader _feedReader;

        public RoutesRepository(GTFSFeedReader feedReader)
        {
            _feedReader = feedReader;
        }

        /// <summary>
        /// Returns a <c>Dictioary</c> containing all the routes in the feed,
        /// separated by agency.
        /// </summary>
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

        /// <summary>
        /// Finds the Route entity by Id
        /// </summary>
        /// <param name="id">Id of the route</param>
        /// <returns>The Route entity found</returns>
        public Route GetRouteById(string id)
        {
            return _feedReader.Feed.Routes.Get(id);
        }

        public List<Route> GetRoutesList()
        {
            return _feedReader.Feed.Routes.ToList();
        }
    }
}