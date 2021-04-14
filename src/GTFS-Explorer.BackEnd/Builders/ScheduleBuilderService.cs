using GTFS.Entities;
using GTFS.Entities.Enumerations;
using GTFS_Explorer.BackEnd.Readers;
using GTFS_Explorer.Core.Enums;
using GTFS_Explorer.Core.Interfaces;
using GTFS_Explorer.Core.Interfaces.RepoInterfaces;
using Nixill.GTFS;
using System;
using System.Collections.Generic;

namespace GTFS_Explorer.BackEnd.Builders
{
	public class ScheduleBuilderService : IScheduleBuilderService
	{
		private readonly GTFSFeedReader _reader;
		private readonly ITimepointRepository _timepointFinder;

		public ScheduleBuilderService(
			GTFSFeedReader reader,
			ITimepointRepository timepointFinder)
		{
			_reader = reader;
			_timepointFinder = timepointFinder;
		}

		public List<string> GetScheduleHeader(string route, DirectionType? dir) =>
			GetScheduleHeader(route, dir, _timepointFinder.GetTimepointStrategy());

		public List<string> GetScheduleHeader(string route, DirectionType? dir, TimepointStrategy strat)
			=> ScheduleBuilder.GetScheduleHeader(_reader.Feed, route, dir, strat);

		public Dictionary<string, int> GetSortTimes(string route, DirectionType? dir, List<string> timepoints)
			=> ScheduleBuilder.GetSortTimes(_reader.Feed, route, dir, timepoints);

		public Tuple<List<string>, List<Tuple<string, Dictionary<string, TimeOfDay>>>>
			GetSchedule(
				string route,
				DirectionType? dir,
				List<string> serviceIds,
				List<string> stopOrder,
				Dictionary<string, int> sortTimes)
			=> ScheduleBuilder.GetSchedule(_reader.Feed, route, dir, serviceIds, stopOrder, sortTimes);

		public Tuple<List<string>, List<Tuple<string, Dictionary<string, TimeOfDay>>>> GetSchedule(string route, DirectionType? dir, List<string> serviceIds) =>
		  GetSchedule(route, dir, serviceIds, _timepointFinder.GetTimepointStrategy());

		public Tuple<List<string>, List<Tuple<string, Dictionary<string, TimeOfDay>>>>
			GetSchedule(string route, DirectionType? dir, List<string> serviceIds, TimepointStrategy strat)
			=> ScheduleBuilder.GetSchedule(_reader.Feed, route, dir, serviceIds, strat);
	}
}