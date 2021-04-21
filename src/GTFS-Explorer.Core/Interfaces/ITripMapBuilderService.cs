using GTFS.Entities;
using GTFS_Explorer.Core.Models.Structs;
using System.Collections.Generic;

namespace GTFS_Explorer.Core.Interfaces
{
	public interface ITripMapBuilderService
	{
		public List<Coordinate> GetAllTripShapes(Trip trip);
	}
}