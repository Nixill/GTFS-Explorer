using GTFS;
using GTFS.Entities;
using GTFS_Explorer.Core.Models.Structs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GTFS_Explorer.BackEnd.Lookups
{
	public class TripMapBuilder
	{
		public static List<Coordinate> GetAllTripShapes(GTFSFeed feed, Trip trip)
		{
			//Keep it basic for now:
			var shapes = from shape in feed.Shapes
						 where shape.Id == trip.ShapeId
						 select shape;

			var coordinates = new List<Coordinate>();
			foreach (var shape in shapes)
			{
				coordinates.Add(new Coordinate(shape.Latitude, shape.Longitude));
			}

			return coordinates;
		}
	}
}