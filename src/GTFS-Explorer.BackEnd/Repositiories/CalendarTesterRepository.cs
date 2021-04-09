using GTFS.Entities;
using GTFS_Explorer.BackEnd.Readers;
using GTFS_Explorer.Core.Interfaces.RepoInterfaces;
using Nixill.GTFS;
using System;
using System.Collections.Generic;

namespace GTFS_Explorer.BackEnd.Repositiories
{
    public class CalendarTesterRepository : ICalendarTesterRepository
    {
        private readonly GTFSFeedReader _reader;

		public CalendarTesterRepository(GTFSFeedReader reader)
		{
			_reader = reader;
		}

        public string GetDescription(string serviceId, Tuple<DateTime, DateTime> dateRange)
            => CalendarTester.GetDescription(_reader.Feed, serviceId, dateRange);

        public Tuple<DateTime, DateTime> GetFeedDateRange()
            => CalendarTester.GetFeedDateRange(_reader.Feed);

        public Tuple<bool, bool> RangeExceedsCalendar(Calendar cal, DateTime start, DateTime end)
            => CalendarTester.RangeExceedsCalendar(cal, start, end);

        public string EnglishList<T>(List<T> items) => EnglishList(items, x => x.ToString());

        public string EnglishList<T>(List<T> items, Func<T, string> toString)
            => CalendarTester.EnglishList(items, toString);
    }
}