using System;
using System.Collections.Generic;
using System.Globalization;
using GTFS;
using GTFS.Entities;
using GTFS.Entities.Enumerations;
using GTFS_Explorer.Core.Enums;
using Nixill.Collections.Grid;
using Nixill.GTFS;

namespace GTFS_Explorer.BackEnd.Lookups
{
  public static class RouteLookups
  {
    public static Grid<string> GetSchedule(GTFSFeed feed, string routeID, DirectionType? dir, string serviceId) =>
      GetSchedule(feed, routeID, dir, serviceId, TimepointFinder.GetTimepointStrategy(feed));

    public static Grid<string> GetSchedule(GTFSFeed feed, string route, DirectionType? dir, string serviceId, Nixill.GTFS.TimepointStrategy strat)
    {
      var stops = ScheduleBuilder.GetScheduleHeader(feed, route, dir, strat);
      var times = ScheduleBuilder.GetSortTimes(feed, route, dir, stops);
      var sched = ScheduleBuilder.GetSchedule(feed, route, dir, serviceId, stops, times);
      return GridifySchedule(sched);
    }

    public static Grid<string> GetSchedule(GTFSFeed feed, string route, DirectionType? dir, string serviceId, List<string> stopOrder, Dictionary<string, int> sortTimes) =>
      GridifySchedule(ScheduleBuilder.GetSchedule(feed, route, dir, serviceId, stopOrder, sortTimes));

    public static Grid<String> GridifySchedule(Tuple<List<string>, List<Tuple<string, Dictionary<string, TimeOfDay>>>> sched)
    {
      Grid<string> ret = new Grid<string>(sched.Item1.Count + 1, 0);

      // Add the stops row
      var stops = sched.Item1;
      stops.Insert(0, "");
      ret.AddRow(stops);

      // Now add the trips
      var trips = sched.Item2;
      foreach (var trip in trips)
      {
        List<string> tripRow = new List<string>();
        // Add the times at each stop
        foreach (string stop in stops)
        {
          if (stop == "")
          {
            tripRow.Add(trip.Item1);
          }
          else
          {
            if (trip.Item2.ContainsKey(stop))
            {
              int time = trip.Item2[stop].TotalSeconds;
              string timeString = NodaTime.LocalTime.FromSecondsSinceMidnight(time).ToString("HH:mm:ss", DateTimeFormatInfo.InvariantInfo);
              tripRow.Add(timeString);
            }
            else
            {
              tripRow.Add("");
            }
          }
        }

        ret.AddRow(tripRow);
      }

      return ret;
    }
  }
}