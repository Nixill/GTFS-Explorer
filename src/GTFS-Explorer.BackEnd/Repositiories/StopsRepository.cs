using GTFS.Entities;
using GTFS_Explorer.BackEnd.Readers;
using GTFS_Explorer.Core.Enums;
using GTFS_Explorer.Core.Interfaces.RepoInterfaces;
using System;
using System.Collections.Generic;
using System.Text;

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
        /// Returns a <c>Dictionary</c> containing all the stops in the feed,
        /// along with how major a stop it is.
        /// </summary>
        public Dictionary<Stop, StopMajority> GetAllStops()
        {
            Dictionary<Stop, StopMajority> dict = new Dictionary<Stop, StopMajority>();

            var stops = _reader.Feed.Stops;

            //For now to prevent errors
            return null;
        }
    }
}
