using GTFS.Entities;
using GTFS.Entities.Enumerations;
using GTFS_Explorer.BackEnd.Readers;
using GTFS_Explorer.Core.Enums;
using GTFS_Explorer.Core.Interfaces;
using GTFS_Explorer.Core.Interfaces.RepoInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GTFS_Explorer.BackEnd.Schedules
{
	public class ScheduleBuilder : IScheduleBuilder
	{
        private readonly GTFSFeedReader _reader;
        private readonly ITimepointRepository _timepointFinder;
        private readonly IStopsRepository _stopsRepository;

		public ScheduleBuilder(
			GTFSFeedReader reader,
			ITimepointRepository timepointFinder, 
            IStopsRepository stopsRepository)
		{
			_reader = reader;
			_timepointFinder = timepointFinder;
			_stopsRepository = stopsRepository;
		}

		public List<string> GetScheduleHeader(string route, DirectionType? dir) =>
            GetScheduleHeader(route, dir, _timepointFinder.GetTimepointStrategy());

        public List<string> GetScheduleHeader(string route, DirectionType? dir, TimepointStrategy strat)
        {
            IEnumerable<string> timepointList;

            if (strat == TimepointStrategy.SpecifiedTimepoints)
            {
                timepointList = _timepointFinder.DataTimepoints(route, dir, true)
                                               .Select(x => x.Item1);
            }
            else if (strat == TimepointStrategy.NullTimepoints)
            {
                timepointList = _timepointFinder.DataTimepoints(route, dir, true)
                                               .Select(x => x.Item1);
            }
            else
            {
                timepointList = _timepointFinder.FirstAndLastStopList(route, dir);
            }

            var timepointsOrdered =
              from stops in _stopsRepository.GetStopOrder(route, dir)
              join timepoints in timepointList on stops.Id equals timepoints
              select stops.Id;

            return timepointsOrdered.ToList();
        }

        public Dictionary<string, int> GetSortTimes(string route, DirectionType? dir, List<string> timepoints)
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
                          from stopTimesUG in _reader.Feed.StopTimes
                          join trips in _reader.Feed.Trips on stopTimesUG.TripId equals trips.Id
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

        public Tuple<List<string>, List<Tuple<string, Dictionary<string, TimeOfDay>>>> 
            GetSchedule(string route, DirectionType? dir, string serviceId, List<string> stopOrder, Dictionary<string, int> sortTimes)
        {
            var filteredTrips =
              from trips in _reader.Feed.Trips
              where trips.RouteId == route && trips.Direction == dir && trips.ServiceId == serviceId
              select trips.Id;

            List<Tuple<string, Dictionary<string, TimeOfDay>, int>> tripSchedules = new List<Tuple<string, Dictionary<string, TimeOfDay>, int>>();

            foreach (string tripId in filteredTrips)
            {
                var tripTimes =
                  (from times in _reader.Feed.StopTimes
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

        public Tuple<List<string>, List<Tuple<string, Dictionary<string, TimeOfDay>>>> 
            GetSchedule(string route, DirectionType? dir, string serviceId) 
                => GetSchedule(route, dir, serviceId, _timepointFinder.GetTimepointStrategy());

        public Tuple<List<string>, List<Tuple<string, Dictionary<string, TimeOfDay>>>> 
            GetSchedule(string route, DirectionType? dir, string serviceId, TimepointStrategy strat)
        {
            var stops = GetScheduleHeader(route, dir, strat);
            var times = GetSortTimes(route, dir, stops);
            return GetSchedule(route, dir, serviceId, stops, times);
        }
    }
}