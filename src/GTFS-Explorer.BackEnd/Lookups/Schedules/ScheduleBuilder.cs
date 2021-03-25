using System.Linq;
using System.Collections.Generic;
using GTFS;
using GTFS.Entities;
using GTFS.Entities.Enumerations;
using System;

namespace Nixill.GTFS
{
  public static class ScheduleBuilder
  {
    public static List<string> GetScheduleHeader(GTFSFeed feed, string route, DirectionType? dir) =>
      GetScheduleHeader(feed, route, dir, TimepointFinder.GetTimepointStrategy(feed));

    public static List<string> GetScheduleHeader(GTFSFeed feed, string route, DirectionType? dir, TimepointStrategy strat)
    {
      IEnumerable<string> timepointList;

      if (strat == TimepointStrategy.SpecifiedTimepoints)
      {
        timepointList = TimepointFinder.DataTimepoints(feed, route, dir, true)
                                       .Select(x => x.Item1);
      }
      else if (strat == TimepointStrategy.NullTimepoints)
      {
        timepointList = TimepointFinder.DataTimepoints(feed, route, dir, true)
                                       .Select(x => x.Item1);
      }
      else
      {
        timepointList = TimepointFinder.FirstAndLastStopList(feed, route, dir);
      }

      var timepointsOrdered =
        from stops in StopLister.GetStopOrder(feed, route, dir)
        join timepoints in timepointList on stops.Id equals timepoints
        select stops.Id;

      return timepointsOrdered.ToList();
    }

    public static Dictionary<string, int> GetSortTimes(GTFSFeed feed, string route, DirectionType? dir, List<string> timepoints)
    {
      Dictionary<string, int> times = new Dictionary<string, int>();

      times[timepoints[0]] = 0;

      List<string> checkedTimes = new List<string>();

      bool failedLoop = true;

      while (times.Count < timepoints.Count)
      {
        foreach (string stopFrom in timepoints.Intersect(times.Keys).Except(checkedTimes))
        {
          failedLoop = false;

          foreach (string stopTo in timepoints.Except(times.Keys))
          {
            var differences =
              from stopTimesUG in feed.StopTimes
              join trips in feed.Trips on stopTimesUG.TripId equals trips.Id
              where trips.RouteId == route && trips.Direction == dir
              group stopTimesUG by stopTimesUG.TripId into stopTimes
              where stopTimes.Any(x => x.StopId == stopFrom) && stopTimes.Any(x => x.StopId == stopTo)
              select stopTimes.Where(x => x.StopId == stopTo).First().ArrivalTime.Value.TotalSeconds
                 - stopTimes.Where(x => x.StopId == stopFrom).First().ArrivalTime.Value.TotalSeconds;

            if (differences.Any())
            {
              times[stopTo] = (int)differences.Average() + times[stopFrom];
            }
          }

          checkedTimes.Add(stopFrom);
        }

        if (failedLoop)
        {
          times[timepoints.Except(checkedTimes).First()] = 0;
        }
      }

      return times;
    }

    public static Tuple<List<string>, List<Tuple<string, Dictionary<string, TimeOfDay>>>> GetSchedule(GTFSFeed feed, string route, DirectionType? dir, string serviceId, List<string> stopOrder, Dictionary<string, int> sortTimes)
    {
      var filteredTrips =
        from trips in feed.Trips
        where trips.RouteId == route && trips.Direction == dir && trips.ServiceId == serviceId
        select trips.Id;

      List<Tuple<string, Dictionary<string, TimeOfDay>, int>> tripSchedules = new List<Tuple<string, Dictionary<string, TimeOfDay>, int>>();

      foreach (string tripId in filteredTrips)
      {
        var tripTimes =
          (from times in feed.StopTimes
           where times.TripId == tripId && stopOrder.Contains(times.StopId)
           group times by new { times.TripId, times.StopId } into times2
           select new
           {
             times2.First().StopId,
             times2.First().DepartureTime
           }).ToDictionary(x => x.StopId, x => x.DepartureTime.Value);

        int sortTime = 0;

        foreach (string stopId in stopOrder)
        {
          if (tripTimes.ContainsKey(stopId))
          {
            sortTime = tripTimes[stopId].TotalSeconds - sortTimes[stopId];
            break;
          }
        }

        tripSchedules.Add(new Tuple<string, Dictionary<string, TimeOfDay>, int>(tripId, tripTimes, sortTime));
      }

      List<Tuple<string, Dictionary<string, TimeOfDay>>> allTrips =
        (from tripScheds in tripSchedules
         orderby tripScheds.Item3
         select new Tuple<string, Dictionary<string, TimeOfDay>>(tripScheds.Item1, tripScheds.Item2)).ToList();

      return new Tuple<List<string>, List<Tuple<string, Dictionary<string, TimeOfDay>>>>(stopOrder, allTrips);
    }

    public static Tuple<List<string>, List<Tuple<string, Dictionary<string, TimeOfDay>>>> GetSchedule(GTFSFeed feed, string route, DirectionType? dir, string serviceId) =>
      GetSchedule(feed, route, dir, serviceId, TimepointFinder.GetTimepointStrategy(feed));

    public static Tuple<List<string>, List<Tuple<string, Dictionary<string, TimeOfDay>>>> GetSchedule(GTFSFeed feed, string route, DirectionType? dir, string serviceId, TimepointStrategy strat)
    {
      var stops = GetScheduleHeader(feed, route, dir, strat);
      var times = GetSortTimes(feed, route, dir, stops);
      return GetSchedule(feed, route, dir, serviceId, stops, times);
    }
  }
}