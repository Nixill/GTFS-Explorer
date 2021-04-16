using GTFS.Entities;
using GTFS.Entities.Enumerations;
using GTFS_Explorer.BackEnd.Readers;
using GTFS_Explorer.Core.Enums;
using GTFS_Explorer.Core.Interfaces.RepoInterfaces;
using Nixill.GTFS;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GTFS_Explorer.BackEnd.Repositiories
{
    public class TimepointRepository : ITimepointRepository
    {
        private readonly GTFSFeedReader _reader;

        public TimepointRepository(GTFSFeedReader reader)
        {
            _reader = reader;
        }

        public TimepointStrategy GetTimepointStrategy()
            => TimepointFinder.GetTimepointStrategy(_reader.Feed);

        public IEnumerable<Tuple<string, bool>> DataTimepoints(string route, DirectionType? dir, bool strict = false)
            => TimepointFinder.DataTimepoints(_reader.Feed, route, dir, strict);

        public IEnumerable<Tuple<string, bool>> DataTimepoints(Route route, DirectionType? dir, bool strict = false)
          => DataTimepoints(route.Id, dir, strict);

        public IEnumerable<string> FirstStops(string route, DirectionType? dir)
            => TimepointFinder.FirstStops(_reader.Feed, route, dir);

        public IEnumerable<string> FirstStops(Route route, DirectionType? dir)
          => FirstStops(route.Id, dir);

        public IEnumerable<string> LastStops(string route, DirectionType? dir)
            => TimepointFinder.LastStops(_reader.Feed, route, dir);

        public IEnumerable<string> LastStops(Route route, DirectionType? dir)
          => LastStops(route.Id, dir);

        public IEnumerable<Tuple<string, string>> FirstAndLastStopPairs(string route, DirectionType? dir)
            => TimepointFinder.FirstAndLastStopPairs(_reader.Feed, route, dir);

        public IEnumerable<Tuple<string, string>> FirstAndLastStopPairs(Route route, DirectionType? dir)
          => FirstAndLastStopPairs(route.Id, dir);

        public IEnumerable<string> FirstAndLastStopList(string route, DirectionType? dir)
            => TimepointFinder.FirstAndLastStopList(_reader.Feed, route, dir);

        public IEnumerable<string> FirstAndLastStopList(Route route, DirectionType? dir)
          => FirstAndLastStopList(route.Id, dir);
    }
}