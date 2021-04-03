using GTFS.Entities;
using System;
using System.Collections.Generic;

namespace GTFS_Explorer.Core.Interfaces.RepoInterfaces
{
	public interface ICalendarTesterRepository
	{
		string GetDescription(string serviceId, Tuple<DateTime, DateTime> dateRange);
		Tuple<DateTime, DateTime> GetFeedDateRange();
		Tuple<bool, bool> RangeExceedsCalendar(Calendar cal, DateTime start, DateTime end);
		string EnglishList<T>(List<T> items, Func<T, string> toString);
	}
}