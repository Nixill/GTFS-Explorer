using GTFS.Entities;
using GTFS.Entities.Enumerations;
using GTFS_Explorer.BackEnd.Lookups;
using GTFS_Explorer.BackEnd.Readers;
using GTFS_Explorer.Core.Interfaces.RepoInterfaces;
using Nixill.GTFS;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GTFS_Explorer.BackEnd.Repositiories
{
    public class StopsRepository : IStopsRepository
    {
        private readonly GTFSFeedReader _reader;

        public StopsRepository(GTFSFeedReader reader)
        {
            _reader = reader;
        }

        /// <summary>
        /// Returns a <c>List</c> containing all the stops in the feed
        /// </summary>
        public List<Stop> GetAllStops() => Lists.GetAllStops(_reader.Feed);

        public List<Stop> GetStopList()
        {
            return _reader.Feed.Stops.ToList();
        }

        /// <summary>
        /// Gets the stop by Id
        /// </summary>
        /// <param name="stopId"></param>
        /// <returns>The stop found</returns>
        public Stop GetStopById(string stopId)
        {
            return _reader.Feed.Stops.Get(stopId);
        }

        /// <summary>
        /// Lists the stops along a route, in order.
        /// </summary>
        /// <param name="route">The ID of the route to get stops for.</param>
        /// <param name="direction">
        /// The direction of trips to get. Must exactly match the direction
        /// provided in GTFS; <c>null</c> here does not match "any direction"
        /// in the data.
        /// </param>
        public IEnumerable<Stop> GetStopOrder(string route, DirectionType? direction = null)
            => StopLister.GetStopOrder(_reader.Feed, route, direction);

        /// <summary>
        /// Lists the stops along a route, in order.
        /// </summary>
        /// <param name="route">The Route to get stops for.</param>
        /// <param name="direction">
        /// The direction of trips to get. Must exactly match the direction
        /// provided in GTFS; <c>null</c> here does not match "any direction"
        /// in the data.
        /// </param>
        public IEnumerable<Stop> GetStopOrder(Route route, DirectionType? direction = null)
            => GetStopOrder(route.Id, direction);
    }
}