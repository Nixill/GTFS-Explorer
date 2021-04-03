using GTFS.Entities;
using GTFS.Entities.Enumerations;
using GTFS_Explorer.BackEnd.Readers;
using GTFS_Explorer.Core.Enums;
using GTFS_Explorer.Core.Interfaces.RepoInterfaces;
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
        {
            var stopTimes =
              from timesUG in _reader.Feed.StopTimes
              group timesUG by new
              {
                  timesUG.TimepointType,
                  HasTime = (timesUG.ArrivalTime != null)
              } into times
              select new Tuple<TimePointType, bool>(
                times.First().TimepointType,
                times.First().ArrivalTime != null
              );

            if (stopTimes.Contains(new Tuple<TimePointType, bool>(TimePointType.Exact, true)))
            {
                return TimepointStrategy.SpecifiedTimepoints;
            }
            else if (stopTimes.Contains(new Tuple<TimePointType, bool>(TimePointType.None, false)))
            {
                return TimepointStrategy.NullTimepoints;
            }

            return TimepointStrategy.SelfSufficient;
        }

        public IEnumerable<Tuple<string, bool>> DataTimepoints(string route, DirectionType? dir, bool strict = false)
        {
            var routeStopTimes =
              from stopTimes in _reader.Feed.StopTimes
              join trips in _reader.Feed.Trips on stopTimes.TripId equals trips.Id
              where trips.RouteId == route
                && trips.Direction == dir
              select stopTimes;

            return
              from stopTimesUngrouped in routeStopTimes
              group stopTimesUngrouped by stopTimesUngrouped.StopId into stopTimes
              where stopTimes.Any(x => x.TimepointType == TimePointType.Exact || (!strict && x.TimepointType == TimePointType.None && x.ArrivalTime != null))
              select new Tuple<string, bool>(
                stopTimes.First().StopId,
                stopTimes.All(x => x.TimepointType == TimePointType.Exact || (!strict && x.TimepointType == TimePointType.None && x.ArrivalTime != null))
              );
        }

        public IEnumerable<Tuple<string, bool>> DataTimepoints(Route route, DirectionType? dir, bool strict = false)
          => DataTimepoints(route.Id, dir, strict);

        public IEnumerable<string> FirstStops(string route, DirectionType? dir)
        {
            return
              from stopTimesUG in _reader.Feed.StopTimes
              join trips in _reader.Feed.Trips on stopTimesUG.TripId equals trips.Id
              where trips.RouteId == route
                && trips.Direction == dir
              orderby stopTimesUG.StopSequence
              group stopTimesUG by stopTimesUG.TripId into stopTimes
              select stopTimes.First().StopId;
        }

        public IEnumerable<string> FirstStops(Route route, DirectionType? dir)
          => FirstStops(route.Id, dir);

        public IEnumerable<string> LastStops(string route, DirectionType? dir)
        {
            return
              from stopTimesUG in _reader.Feed.StopTimes
              join trips in _reader.Feed.Trips on stopTimesUG.TripId equals trips.Id
              where trips.RouteId == route
                && trips.Direction == dir
              orderby stopTimesUG.StopSequence
              group stopTimesUG by stopTimesUG.TripId into stopTimes
              select stopTimes.Last().StopId;
        }

        public IEnumerable<string> LastStops(Route route, DirectionType? dir)
          => LastStops(route.Id, dir);

        public IEnumerable<Tuple<string, string>> FirstAndLastStopPairs(string route, DirectionType? dir)
        {
            return
              from stopTimesUG in _reader.Feed.StopTimes
              join trips in _reader.Feed.Trips on stopTimesUG.TripId equals trips.Id
              where trips.RouteId == route
                && trips.Direction == dir
              orderby stopTimesUG.StopSequence
              group stopTimesUG by stopTimesUG.TripId into stopTimes
              select new Tuple<string, string>(
                stopTimes.First().StopId,
                stopTimes.Last().StopId
              );
        }

        public IEnumerable<Tuple<string, string>> FirstAndLastStopPairs(Route route, DirectionType? dir)
          => FirstAndLastStopPairs(route.Id, dir);

        public IEnumerable<string> FirstAndLastStopList(string route, DirectionType? dir)
        {
            List<string> ret = new List<string>();

            foreach (Tuple<string, string> pair in FirstAndLastStopPairs(route, dir))
            {
                ret.Add(pair.Item1);
                ret.Add(pair.Item2);
            }

            return ret.Distinct();
        }

        public IEnumerable<string> FirstAndLastStopList(Route route, DirectionType? dir)
          => FirstAndLastStopList(route.Id, dir);
    }
}