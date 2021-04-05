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

namespace GTFS_Explorer.BackEnd.Repositiories
{
    public class RoutesRepository : IRoutesRepository
    {
        private readonly GTFSFeedReader _feedReader;
        private readonly ITimepointRepository _timepointRepository;
        private readonly IScheduleBuilder _scheduleBuilder;

		public RoutesRepository(
            GTFSFeedReader feedReader, 
            ITimepointRepository timepointRepository, 
            IScheduleBuilder scheduleBuilder)
		{
			_feedReader = feedReader;
			_timepointRepository = timepointRepository;
			_scheduleBuilder = scheduleBuilder;
		}

		/// <summary>
		/// Returns a <c>Dictioary</c> containing all the routes in the feed,
		/// separated by agency.
		/// </summary>
		public Dictionary<Agency, List<Route>> GetAllRoutes()
        {
            GeneratorDictionary<Agency, List<Route>> dict =
                new GeneratorDictionary<Agency, List<Route>>(
                    new FuncGenerator<Agency, List<Route>>(x => new List<Route>()));

            var agencies = _feedReader.Feed.Agencies;

            foreach (Route route in _feedReader.Feed.Routes)
            {
                dict[agencies.Get(route.AgencyId)].Add(route);
            }

            return dict;
        }

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

        public Grid<string> GetSchedule(string route, DirectionType? dir, string serviceId, TimepointStrategy strat)
        {
            var stops = _scheduleBuilder.GetScheduleHeader(route, dir, strat);
            var times = _scheduleBuilder.GetSortTimes(route, dir, stops);
            Tuple<List<string>, List<Tuple<string, Dictionary<string, TimeOfDay>>>> sched = 
                _scheduleBuilder.GetSchedule(route, dir, serviceId, stops, times);
            return GridifySchedule(sched);
        }

        public Grid<string> GetSchedule(string routeID, DirectionType? dir, string serviceId) =>
            GetSchedule(routeID, dir, serviceId, _timepointRepository.GetTimepointStrategy());

        public Grid<string> GridifySchedule(Tuple<List<string>, List<Tuple<string, Dictionary<string, TimeOfDay>>>> sched)
        {
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
                            string timeString = 
                                NodaTime.LocalTime
                                .FromSecondsSinceMidnight(time)
                                .ToString("HH:mm:ss", DateTimeFormatInfo.InvariantInfo);
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