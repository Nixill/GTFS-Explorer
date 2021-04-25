using System;
using System.Collections.Generic;
using System.Drawing;
using GTFS;
using GTFS_Explorer.Core.Models.Structs;
using System.Linq;
using GTFS.Entities;
using GTFS.Entities.Enumerations;

namespace GTFS_Explorer.BackEnd.Lookups
{
    public class SystemMapBuilder
    {
        public bool DoShapelessTripsExist(GTFSFeed feed) =>
            feed.Trips.Any(tp => tp.ShapeId == null || tp.ShapeId == "");

        public IEnumerable<Tuple<Tuple<Color, Color>, IEnumerable<Coordinate>>> GetRouteShapeLines(GTFSFeed feed) =>
            feed.Routes.SelectMany(rt =>
                feed.Trips
                    .Where(tp => rt.Id == tp.RouteId && tp.ShapeId != null && tp.ShapeId != "")
                    .Select(tp => new Tuple<Tuple<Color, Color>, IEnumerable<Coordinate>>(
                        RouteMapBuilder.GetRouteColors(rt),
                        feed.Shapes
                            .Where(sh => tp.ShapeId == sh.Id)
                            .Distinct()
                            .OrderBy(sh => sh.Sequence)
                            .Select(sh => new Coordinate(sh.Latitude, sh.Longitude))
                    ))
            );

        public IEnumerable<Tuple<Tuple<Color, Color>, IEnumerable<Coordinate>>> GetRouteStopPatternLines(GTFSFeed feed) =>
            feed.Routes.SelectMany(rt =>
                feed.Trips
                    .Where(tp => rt.Id == tp.RouteId && (tp.ShapeId == null || tp.ShapeId == ""))
                    .Select(tp => new Tuple<Tuple<Color, Color>, IEnumerable<Coordinate>>(
                        RouteMapBuilder.GetRouteColors(rt),
                        feed.StopTimes
                            .Where(stm => tp.Id == stm.TripId)
                            .OrderBy(stm => stm.StopSequence)
                            .Select(stm => feed.Stops.Get(stm.StopId))
                            .Select(st => new Coordinate(st.Latitude, st.Longitude))
                    ))
            );

        public IEnumerable<Stop> GetAllStops(GTFSFeed feed) =>
            feed.Stops.Where(st =>
                (st.LocationType == LocationType.Stop && st.ParentStation == "")
                || st.LocationType == LocationType.Station
            );
    }
}