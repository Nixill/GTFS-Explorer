using GTFS.Entities;
using GTFS_Explorer.Core.Interfaces.RepoInterfaces;
using System.Collections.Generic;
using System.Linq;
using GTFS_Explorer.BackEnd.Readers;
using Nixill.Collections.Grid;
using GTFS.Entities.Enumerations;
using GTFS_Explorer.Core.Enums;
using GTFS_Explorer.Core.Interfaces;
using System;
using NodaTime;
using GTFS_Explorer.Core.Models.Structs;
using GTFS_Explorer.BackEnd.Lookups;

namespace GTFS_Explorer.BackEnd.Repositiories
{
	public class RoutesRepository : IRoutesRepository
	{
		private readonly GTFSFeedReader _reader;
		private readonly ITimepointRepository _timepointRepository;
		private readonly IScheduleBuilderService _scheduleBuilder;

		public RoutesRepository(
		  GTFSFeedReader feedReader,
		  ITimepointRepository timepointRepository,
		  IScheduleBuilderService scheduleBuilder)
		{
			_reader = feedReader;
			_timepointRepository = timepointRepository;
			_scheduleBuilder = scheduleBuilder;
		}

		/// <summary>
		/// Returns a <c>Dictioary</c> containing all the routes in the feed,
		/// separated by agency.
		/// </summary>
		public Dictionary<Agency, List<Route>> GetAllRoutes() =>
			  Lists.GetAllRoutes(_reader.Feed);

		/// <summary>
		/// Finds the Route entity by Id
		/// </summary>
		/// <param name="id">Id of the route</param>
		/// <returns>The Route entity found</returns>
		public Route GetRouteById(string id)
		{
			return _reader.Feed.Routes.Get(id);
		}

		public List<Route> GetRoutesList()
		{
			return _reader.Feed.Routes.ToList();
		}

		/// <summary>
		///   <para>Returns a transit schedule in grid form.</para>
		/// </summary>
		/// <remarks>
		///   <para>The returned schedule will have stop IDs along the first
		///     row, trip IDs in the first column, and times of when that trip
		///     reaches that stop in the rest of the grid.</para>
		/// </remarks>
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
			=> RouteLookups.GetSchedule(_reader.Feed, route, dir, serviceIds, strat);

		/// <summary>
		///   <para>Returns a transit schedule in grid form.</para>
		/// </summary>
		/// <remarks>
		///   <para>The returned schedule will have stop IDs along the first
		///     row, trip IDs in the first column, and times of when that trip
		///     reaches that stop in the rest of the grid.</para>
		/// </remarks>
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
		/// <param name="route">The ID of the route to retrieve.</param>
		/// <param name="dir">
		///   Which direction of the route should be scheduled.
		/// </param>
		/// <param name="serviceIds">The IDs of the days to retrieve.</param>
		/// <param name="stopOrder">The order of stops to use.</param>
		/// <param name="sortTimes">The sort times of the trips.</param>
		public Grid<string> GetSchedule(
			string route,
			DirectionType? dir,
			List<string> serviceIds,
			List<string> stopOrder,
			Dictionary<string, int> sortTimes)
			=> RouteLookups.GetSchedule(_reader.Feed, route, dir, serviceIds, stopOrder, sortTimes);

		/// <summary>
		///   <para>Returns a transit schedule in grid form.</para>
		/// </summary>
		/// <remarks>
		///   <para>The returned schedule will have stop IDs along the first
		///     row, trip IDs in the first column, and times of when that trip
		///     reaches that stop in the rest of the grid.</para>
		/// </remarks>
		/// <param name="route">The ID of the route to retrieve.</param>
		/// <param name="dir">
		///   Which direction of the route should be scheduled.
		/// </param>
		/// <param name="date">The date to retrieve.</param>
		/// <param name="stopOrder">The order of stops to use.</param>
		/// <param name="sortTimes">The sort times of the trips.</param>
		public Grid<string> GetSchedule(
			string route,
			DirectionType? dir,
			LocalDate date,
			List<string> stopOrder,
			Dictionary<string, int> sortTimes)
			=>  RouteLookups.GetSchedule(_reader.Feed, route, dir, date, stopOrder, sortTimes);

		/// <summary>
		///   Returns whether or not the Route has any trips or any stop
		///   times.
		/// </summary>
		/// <param name="route">The ID of the route to check.</param>
		public bool HasAnyService(string route) 
			=> RouteLookups.HasAnyService(_reader.Feed, route);

		/// <summary>
		///   Returns a <c>List</c> of all the service IDs active on a given
		///   day.
		/// </summary>
		/// <remarks>
		///   If no services run on a given day, an empty list is returned.
		/// </remarks>
		/// <param name="date">The date to check.</param>
		public List<string> ServicesOn(LocalDate date)
				=> RouteLookups.ServicesOn(_reader.Feed, date);

		public Dictionary<DirectionType?, RouteStats> GetRouteStats(LocalDate date, string route)
				=> RouteLookups.GetRouteStats(_reader.Feed, date, route);
	}
}