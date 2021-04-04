using System;
using System.Collections.Generic;
using System.Globalization;
using GTFS;
using GTFS.Entities;
using GTFS.Entities.Enumerations;
using Nixill.Collections.Grid;
using Nixill.GTFS;

namespace GTFS_Explorer.BackEnd.Lookups
{
  public static class RouteLookups
  {
    /// <summary>
    ///   <para>Returns a transit schedule in grid form.</para>
    /// </summary>
    /// <remarks>
    ///   <para>The returned schedule will have stop IDs along the first
    ///     row, trip IDs in the first column, and times of when that trip
    ///     reaches that stop in the rest of the grid.</para>
    /// </remarks>
    /// <param name="feed">The GTFS feed to use.</param>
    /// <param name="route">The ID of the route to retrieve.</param>
    /// <param name="dir">
    ///   Which direction of the route should be scheduled.
    /// </param>
    /// <param name="serviceId">The ID of the day to retrieve.</param>
    public static Grid<string> GetSchedule(GTFSFeed feed, string route, DirectionType? dir, string serviceId) =>
      GetSchedule(feed, route, dir, serviceId, TimepointFinder.GetTimepointStrategy(feed));

    /// <summary>
    ///   <para>Returns a transit schedule in grid form.</para>
    /// </summary>
    /// <remarks>
    ///   <para>The returned schedule will have stop IDs along the first
    ///     row, trip IDs in the first column, and times of when that trip
    ///     reaches that stop in the rest of the grid.</para>
    /// </remarks>
    /// <param name="feed">The GTFS feed to use.</param>
    /// <param name="route">The ID of the route to retrieve.</param>
    /// <param name="dir">
    ///   Which direction of the route should be scheduled.
    /// </param>
    /// <param name="serviceId">The ID of the day to retrieve.</param>
    /// <param name="strat">
    ///   The timepoint strategy, a value returned by
    ///   <c>TimepointFinder.GetTimepointStrategy()</c>.
    /// </param>
    public static Grid<string> GetSchedule(GTFSFeed feed, string route, DirectionType? dir, string serviceId, TimepointStrategy strat)
    {
      var stops = ScheduleBuilder.GetScheduleHeader(feed, route, dir, strat);
      var times = ScheduleBuilder.GetSortTimes(feed, route, dir, stops);
      var sched = ScheduleBuilder.GetSchedule(feed, route, dir, serviceId, stops, times);
      return GridifySchedule(sched);
    }

    /// <summary>
    ///   <para>Returns a transit schedule in grid form.</para>
    /// </summary>
    /// <remarks>
    ///   <para>The returned schedule will have stop IDs along the first
    ///     row, trip IDs in the first column, and times of when that trip
    ///     reaches that stop in the rest of the grid.</para>
    /// </remarks>
    /// <param name="feed">The GTFS feed to use.</param>
    /// <param name="route">The ID of the route to retrieve.</param>
    /// <param name="dir">
    ///   Which direction of the route should be scheduled.
    /// </param>
    /// <param name="serviceId">The ID of the day to retrieve.</param>
    /// <param name="stopOrder">The order of stops to use.</param>
    /// <param name="sortTimes">The sort times of the trips.</param>
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