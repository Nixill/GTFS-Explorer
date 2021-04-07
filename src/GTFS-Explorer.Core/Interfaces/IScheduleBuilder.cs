using GTFS.Entities;
using GTFS.Entities.Enumerations;
using GTFS_Explorer.Core.Enums;
using System;
using System.Collections.Generic;

namespace GTFS_Explorer.Core.Interfaces
{
	public interface IScheduleBuilder
	{
		List<string> GetScheduleHeader(string route, DirectionType? dir, TimepointStrategy strat);
		Dictionary<string, int> GetSortTimes(string route, DirectionType? dir, List<string> timepoints);
		Tuple<List<string>, List<Tuple<string, Dictionary<string, TimeOfDay>>>> GetSchedule(
				string route,
				DirectionType? dir,
				List<string> serviceIds,
				List<string> stopOrder,
				Dictionary<string, int> sortTimes);
		Tuple<List<string>, List<Tuple<string, Dictionary<string, TimeOfDay>>>> GetSchedule(
				string route, 
				DirectionType? dir, 
				List<string> serviceIds, 
				TimepointStrategy strat);
	}
}