using System;
using System.Collections.Generic;
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

    public static Grid<string> GetSchedule(GTFSFeed feed, string route, DirectionType? dir, string serviceId, TimepointStrategy strat)
    {
      var stops = ScheduleBuilder.GetScheduleHeader(feed, route, dir, strat);
      var times = ScheduleBuilder.GetSortTimes(feed, route, dir, stops);
      Tuple<List<string>, List<Tuple<string, Dictionary<string, TimeOfDay>>>> sched = ScheduleBuilder.GetSchedule(feed, route, dir, serviceId, stops, times);
      return GridifySchedule(sched);
    }


  }
}