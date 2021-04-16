using GTFS.Entities;
using GTFS_Explorer.Core.Models.Structs;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace GTFS_Explorer.Core.Interfaces
{
    public interface IRouteMapBuilderService
    {
        public List<List<Coordinate>> GetShapes(string route);
        public Tuple<Color, Color> GetRouteColors(Route route);
        public List<Tuple<Stop, bool>> GetStops(string routeID);
    }
}