using System.Collections.Generic;
using GTFS;
using GTFS.Entities;
using GTFS.Entities.Enumerations;
using System.Linq;
using System;

namespace Nixill.GTFS
{
  public static class TimepointFinder
  {
    public static TimepointStrategy GetTimepointStrategy(GTFSFeed feed)
    {
      var stopTimes =
        from timesUG in feed.StopTimes
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

    public static IEnumerable<Tuple<string, bool>> DataTimepoints(GTFSFeed feed, string route, DirectionType? dir, bool strict = false)
    {
      var routeStopTimes =
        from stopTimes in feed.StopTimes
        join trips in feed.Trips on stopTimes.TripId equals trips.Id
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

    public static IEnumerable<Tuple<string, bool>> DataTimepoints(GTFSFeed feed, Route route, DirectionType? dir, bool strict = false)
      => DataTimepoints(feed, route.Id, dir, strict);

    public static IEnumerable<string> FirstStops(GTFSFeed feed, string route, DirectionType? dir)
    {
      return
        from stopTimesUG in feed.StopTimes
        join trips in feed.Trips on stopTimesUG.TripId equals trips.Id
        where trips.RouteId == route
          && trips.Direction == dir
        orderby stopTimesUG.StopSequence
        group stopTimesUG by stopTimesUG.TripId into stopTimes
        select stopTimes.First().StopId;
    }

    public static IEnumerable<string> FirstStops(GTFSFeed feed, Route route, DirectionType? dir)
      => FirstStops(feed, route.Id, dir);

    public static IEnumerable<string> LastStops(GTFSFeed feed, string route, DirectionType? dir)
    {
      return
        from stopTimesUG in feed.StopTimes
        join trips in feed.Trips on stopTimesUG.TripId equals trips.Id
        where trips.RouteId == route
          && trips.Direction == dir
        orderby stopTimesUG.StopSequence
        group stopTimesUG by stopTimesUG.TripId into stopTimes
        select stopTimes.Last().StopId;
    }

    public static IEnumerable<string> LastStops(GTFSFeed feed, Route route, DirectionType? dir)
      => LastStops(feed, route.Id, dir);

    public static IEnumerable<Tuple<string, string>> FirstAndLastStopPairs(GTFSFeed feed, string route, DirectionType? dir)
    {
      return
        from stopTimesUG in feed.StopTimes
        join trips in feed.Trips on stopTimesUG.TripId equals trips.Id
        where trips.RouteId == route
          && trips.Direction == dir
        orderby stopTimesUG.StopSequence
        group stopTimesUG by stopTimesUG.TripId into stopTimes
        select new Tuple<string, string>(
          stopTimes.First().StopId,
          stopTimes.Last().StopId
        );
    }

    public static IEnumerable<Tuple<string, string>> FirstAndLastStopPairs(GTFSFeed feed, Route route, DirectionType? dir)
      => FirstAndLastStopPairs(feed, route.Id, dir);

    public static IEnumerable<string> FirstAndLastStopList(GTFSFeed feed, string route, DirectionType? dir)
    {
      List<string> ret = new List<string>();

      foreach (Tuple<string, string> pair in FirstAndLastStopPairs(feed, route, dir))
      {
        ret.Add(pair.Item1);
        ret.Add(pair.Item2);
      }

      return ret.Distinct();
    }

    public static IEnumerable<string> FirstAndLastStopList(GTFSFeed feed, Route route, DirectionType? dir)
      => FirstAndLastStopList(feed, route.Id, dir);
  }

  public enum TimepointStrategy
  {
    SelfSufficient,
    NullTimepoints,
    SpecifiedTimepoints
  }
}