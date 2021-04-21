using GTFS.Entities;
using GTFS_Explorer.BackEnd.Lookups;
using GTFS_Explorer.BackEnd.Readers;
using GTFS_Explorer.Core.Interfaces;
using GTFS_Explorer.Core.Models.Structs;
using System;
using System.Collections.Generic;
using System.Text;

namespace GTFS_Explorer.BackEnd.Builders
{
	public class TripMapBuilderService : ITripMapBuilderService
	{
		private readonly GTFSFeedReader _reader;

		public TripMapBuilderService(GTFSFeedReader reader)
		{
			_reader = reader;
		}

		public List<Coordinate> GetAllTripShapes(Trip trip)
			=> TripMapBuilder.GetAllTripShapes(_reader.Feed, trip);
	}
}