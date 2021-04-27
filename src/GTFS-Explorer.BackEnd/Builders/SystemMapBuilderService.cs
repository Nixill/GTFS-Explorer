using GTFS.Entities;
using GTFS_Explorer.BackEnd.Lookups;
using GTFS_Explorer.BackEnd.Readers;
using GTFS_Explorer.Core.Interfaces;
using GTFS_Explorer.Core.Models.Structs;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace GTFS_Explorer.BackEnd.Builders
{
	public class SystemMapBuilderService : ISystemMapBuilderService
	{
		private readonly GTFSFeedReader _reader;

		public SystemMapBuilderService(GTFSFeedReader reader)
		{
			_reader = reader;
		}

		public bool DoShapelessTripsExist()
			=> SystemMapBuilder.DoShapelessTripsExist(_reader.Feed);

		public IEnumerable<Stop> GetAllStops()
			=> SystemMapBuilder.GetAllStops(_reader.Feed);

		public IEnumerable<Tuple<Tuple<Color, Color>, IEnumerable<Coordinate>>> GetRouteShapeLines()
			=> SystemMapBuilder.GetRouteShapeLines(_reader.Feed);

		public IEnumerable<Tuple<Tuple<Color, Color>, IEnumerable<Coordinate>>> GetRouteStopPatternLines()
			=> SystemMapBuilder.GetRouteStopPatternLines(_reader.Feed);
	}
}