using GTFS.Entities;
using GTFS_Explorer.Core.Interfaces.RepoInterfaces;
using System.Collections.Generic;
using Nixill.Collections;
using System.Linq;
using GTFS_Explorer.BackEnd.Readers;
using Nixill.Collections.Grid;
using GTFS.Entities.Enumerations;
using GTFS_Explorer.Core.Enums;
using GTFS_Explorer.Core.Interfaces;
using System;
using System.Globalization;
using NodaTime;
using GTFS_Explorer.Core.Models.Structs;
using GTFS_Explorer.BackEnd.Lookups;

namespace GTFS_Explorer.BackEnd.Repositiories
{
  public class RoutesRepository : IRoutesRepository
  {
    private readonly GTFSFeedReader _feedReader;
    private readonly ITimepointRepository _timepointRepository;
    private readonly IScheduleBuilder _scheduleBuilder;
    private readonly IAgencyRepository _agencyRepository;

    public RoutesRepository(
      GTFSFeedReader feedReader,
      ITimepointRepository timepointRepository,
      IScheduleBuilder scheduleBuilder,
            IAgencyRepository agencyRepository)
    {
      _feedReader = feedReader;
      _timepointRepository = timepointRepository;
      _scheduleBuilder = scheduleBuilder;
      _agencyRepository = agencyRepository;
    }

    /// <summary>
    /// Returns a <c>Dictioary</c> containing all the routes in the feed,
    /// separated by agency.
    /// </summary>
    public Dictionary<Agency, List<Route>> GetAllRoutes() =>
          Lists.GetAllRoutes(_feedReader.Feed);

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
    /// <param name="serviceIds">The IDs of the days to retrieve.</param>
    public Grid<string> GetSchedule(string route, DirectionType? dir, List<string> serviceIds) =>
      GetSchedule(route, dir, serviceIds, _timepointRepository.GetTimepointStrategy());

    /// <summary>
    ///   <para>Returns a transit schedule in grid form.</para>
    /// </summary>
    /// <remarks>
    ///   <para>The returned schedule will have stop IDs along the first
    ///     row, trip IDs in the first column, and times of when that trip
    ///     reaches that stop in the rest of the grid.</para>
    ///   <para>This overload is deprecated; <c>serviceId</c> should be
    ///     replaced with <c>new List&lt;string&gt; { serviceId }</c>.
    ///   </para>
    /// </remarks>
    /// <param name="feed">The GTFS feed to use.</param>
    /// <param name="route">The ID of the route to retrieve.</param>
    /// <param name="dir">
    ///   Which direction of the route should be scheduled.
    /// </param>
    /// <param name="serviceId">The ID of the service to retrieve.</param>
    [Obsolete("Change `serviceId` to `new List<string> { serviceId }`.")]
    public Grid<string> GetSchedule(string route, DirectionType? dir, string serviceId) =>
      GetSchedule(route, dir, new List<string> { serviceId }, _timepointRepository.GetTimepointStrategy());

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
    /// <param name="date">The date to retrieve.</param>
    public Grid<string> GetSchedule(string route, DirectionType? dir, LocalDate date) =>
        GetSchedule(route, dir, ServicesOn(date));

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
    /// <param name="serviceIds">The IDs of the days to retrieve.</param>
    /// <param name="strat">
    ///   The timepoint strategy, a value returned by
    ///   <c>TimepointFinder.GetTimepointStrategy()</c>.
    /// </param>
    public Grid<string> GetSchedule(string route, DirectionType? dir, List<string> serviceIds, TimepointStrategy strat)
    {
      var stops = _scheduleBuilder.GetScheduleHeader(route, dir, strat);
      var times = _scheduleBuilder.GetSortTimes(route, dir, stops);
      var sched = _scheduleBuilder.GetSchedule(route, dir, serviceIds, stops, times);
      return GridifySchedule(sched);
    }

    /// <summary>
    ///   <para>Returns a transit schedule in grid form.</para>
    /// </summary>
    /// <remarks>
    ///   <para>The returned schedule will have stop IDs along the first
    ///     row, trip IDs in the first column, and times of when that trip
    ///     reaches that stop in the rest of the grid.</para>
    ///   <para>This overload is deprecated; <c>serviceId</c> should be
    ///     replaced with <c>new List&lt;string&gt; { serviceId }</c>.
    ///   </para>
    /// </remarks>
    /// <param name="feed">The GTFS feed to use.</param>
    /// <param name="route">The ID of the route to retrieve.</param>
    /// <param name="dir">
    ///   Which direction of the route should be scheduled.
    /// </param>
    /// <param name="serviceId">The ID of the service to retrieve.</param>
    /// <param name="strat">
    ///   The timepoint strategy, a value returned by
    ///   <c>TimepointFinder.GetTimepointStrategy()</c>.
    /// </param>
    [Obsolete("Change `serviceId` to `new List<string> { serviceId }`.")]
    public Grid<string> GetSchedule(string route, DirectionType? dir, string serviceId, TimepointStrategy strat) =>
      GetSchedule(route, dir, new List<string> { serviceId }, strat);

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
    /// <param name="date">The date to retrieve.</param>
    /// <param name="strat">
    ///   The timepoint strategy, a value returned by
    ///   <c>TimepointFinder.GetTimepointStrategy()</c>.
    /// </param>
    public Grid<string> GetSchedule(string route, DirectionType? dir, LocalDate date, TimepointStrategy strat) =>
      GetSchedule(route, dir, ServicesOn(date), strat);

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
    /// <param name="serviceIds">The IDs of the days to retrieve.</param>
    /// <param name="stopOrder">The order of stops to use.</param>
    /// <param name="sortTimes">The sort times of the trips.</param>
    public Grid<string> GetSchedule(string route, DirectionType? dir, List<string> serviceIds, List<string> stopOrder, Dictionary<string, int> sortTimes) =>
      GridifySchedule(_scheduleBuilder.GetSchedule(route, dir, serviceIds, stopOrder, sortTimes));

    /// <summary>
    ///   <para>Returns a transit schedule in grid form.</para>
    /// </summary>
    /// <remarks>
    ///   <para>The returned schedule will have stop IDs along the first
    ///     row, trip IDs in the first column, and times of when that trip
    ///     reaches that stop in the rest of the grid.</para>
    ///   <para>This overload is deprecated; <c>serviceId</c> should be
    ///     replaced with <c>new List&lt;string&gt; { serviceId }</c>.
    ///   </para>
    /// </remarks>
    /// <param name="feed">The GTFS feed to use.</param>
    /// <param name="route">The ID of the route to retrieve.</param>
    /// <param name="dir">
    ///   Which direction of the route should be scheduled.
    /// </param>
    /// <param name="serviceId">The ID of the service to retrieve.</param>
    /// <param name="stopOrder">The order of stops to use.</param>
    /// <param name="sortTimes">The sort times of the trips.</param>
    [Obsolete("Change `serviceId` to `new List<string> { serviceId }`.")]
    public Grid<string> GetSchedule(string route, DirectionType? dir, string serviceId, List<string> stopOrder, Dictionary<string, int> sortTimes) =>
      GridifySchedule(_scheduleBuilder.GetSchedule(route, dir, new List<string> { serviceId }, stopOrder, sortTimes));

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
    /// <param name="date">The date to retrieve.</param>
    /// <param name="stopOrder">The order of stops to use.</param>
    /// <param name="sortTimes">The sort times of the trips.</param>
    public Grid<string> GetSchedule(string route, DirectionType? dir, LocalDate date, List<string> stopOrder, Dictionary<string, int> sortTimes) =>
      GridifySchedule(_scheduleBuilder.GetSchedule(route, dir, ServicesOn(date), stopOrder, sortTimes));

    private Grid<string> GridifySchedule(Tuple<List<string>, List<Tuple<string, Dictionary<string, TimeOfDay>>>> sched)
    {
      //Grid<string> ret = new Grid<string>(sched.Item1.Count + 1, 0);
      Grid<string> ret = new Grid<string>();

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

    /// <summary>
    ///   Returns whether or not the Route has any trips or any stop
    ///   times.
    /// </summary>
    /// <param name="feed">The GTFS feed to check.</param>
    /// <param name="string">The ID of the route to check.</param>
    public bool HasAnyService(string route)
    {
      // First see if it has any trips
      var trips =
        from trip in _feedReader.Feed.Trips
        where trip.RouteId == route
        select trip;

      // If so, let's see if any of them have any stop times
      foreach (Trip theTrip in trips)
      {
        var stopTimes =
          from time in _feedReader.Feed.StopTimes
          where time.TripId == theTrip.Id
          select time;

        if (stopTimes.Any()) return true;
      }

      return false;
    }

    /// <summary>
    ///   Returns a <c>List</c> of all the service IDs active on a given
    ///   day.
    /// </summary>
    /// <remarks>
    ///   If no services run on a given day, an empty list is returned.
    /// </remarks>
    /// <param name="feed">The GTFS feed to check.</param>
    /// <param name="date">The date to check.</param>
    public List<string> ServicesOn(LocalDate date)
    {
      DateTime dt = date.ToDateTimeUnspecified();

      var ret = from cal in _feedReader.Feed.Calendars
                where cal.StartDate <= dt
                  && cal.EndDate >= dt
                  && date.DayOfWeek switch
                  {
                    IsoDayOfWeek.Monday => cal.Monday,
                    IsoDayOfWeek.Tuesday => cal.Tuesday,
                    IsoDayOfWeek.Wednesday => cal.Wednesday,
                    IsoDayOfWeek.Thursday => cal.Thursday,
                    IsoDayOfWeek.Friday => cal.Friday,
                    IsoDayOfWeek.Saturday => cal.Saturday,
                    IsoDayOfWeek.Sunday => cal.Sunday,
                    _ => false
                  }
                select cal.ServiceId;

      ret = ret.Except(from cald in _feedReader.Feed.CalendarDates
                       where cald.Date == dt && cald.ExceptionType == ExceptionType.Removed
                       select cald.ServiceId);

      ret = ret.Union(from cald in _feedReader.Feed.CalendarDates
                      where cald.Date == dt && cald.ExceptionType == ExceptionType.Added
                      select cald.ServiceId);

      return ret.Distinct().ToList();
    }

    public Dictionary<DirectionType?, RouteStats> GetRouteStats(LocalDate date, string route)
    {
      Dictionary<DirectionType?, RouteStats> ret = new Dictionary<DirectionType?, RouteStats>();

      var services = ServicesOn(date);
      var allTrips = _feedReader.Feed.Trips.Where(x => x.RouteId == route && services.Contains(x.ServiceId));
      var directions = allTrips.Select(x => x.Direction).Distinct();

      foreach (DirectionType? dir in directions)
      {
        var dirTrips = allTrips.Where(x => x.Direction == dir);

        var rs = new RouteStats()
        {
          StartStops = new List<string>(),
          EndStops = new List<string>(),
          TotalTrips = 0,
          AverageTrip = Duration.FromSeconds(0),
          StartTime = Duration.FromSeconds(0),
          EndTime = Duration.MaxValue,
          ShortestTrip = Duration.FromSeconds(0),
          LongestTrip = Duration.MaxValue
        };

        foreach (Trip t in dirTrips)
        {
          var stops = _feedReader.Feed.StopTimes.Where(x => x.TripId == t.Id).OrderBy(x => x.ArrivalTime);
          var firstStop = stops.First();
          var lastStop = stops.Last();

          var firstTime = Duration.FromSeconds(firstStop.DepartureTime.Value.TotalSeconds);
          var lastTime = Duration.FromSeconds(lastStop.ArrivalTime.Value.TotalSeconds);
          var length = lastTime - firstTime;

          if (rs.StartTime > firstTime) rs.StartTime = firstTime;
          if (rs.EndTime < lastTime) rs.EndTime = lastTime;
          if (rs.ShortestTrip > length) rs.ShortestTrip = length;
          if (rs.LongestTrip < length) rs.LongestTrip = length;

          rs.AverageTrip += length;
          rs.TotalTrips += 1;
        }

        rs.AverageTrip /= rs.TotalTrips;
        ret[dir] = rs;
      }

      return ret;
    }
  }
}