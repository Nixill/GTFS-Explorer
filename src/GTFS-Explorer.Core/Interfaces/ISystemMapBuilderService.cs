using GTFS.Entities;
using GTFS_Explorer.Core.Models.Structs;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace GTFS_Explorer.Core.Interfaces
{
	public interface ISystemMapBuilderService
	{
		bool DoShapelessTripsExist();
		IEnumerable<Tuple<Tuple<Color, Color>, IEnumerable<Coordinate>>> GetRouteShapeLines();
		IEnumerable<Tuple<Tuple<Color, Color>, IEnumerable<Coordinate>>> GetRouteStopPatternLines();
		IEnumerable<Stop> GetAllStops();
	}
}