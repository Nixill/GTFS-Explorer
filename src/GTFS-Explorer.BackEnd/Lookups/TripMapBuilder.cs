using GTFS;
using GTFS.Entities;
using GTFS_Explorer.Core.Models.Structs;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace GTFS_Explorer.BackEnd.Lookups
{
    public class TripMapBuilder
    {
        public static List<Coordinate> GetAllTripShapes(GTFSFeed feed, Trip trip)
        {
            //Trips may not have shapes defined
            if (trip.ShapeId != null && trip.ShapeId != "")
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
            else
            {
                // Go by stops
                return feed.StopTimes
                    .Where(x => x.TripId == trip.Id)
                    .OrderBy(x => x.StopSequence)
                    .Select(x => feed.Stops.Get(x.StopId))
                    .Select(x => new Coordinate(x.Latitude, x.Longitude))
                    .ToList();
            }
        }

        public static Tuple<Color, Color> GetRouteColors(Route route)
        {
            Color primary, secondary;

            // Get primary color
            int? color = route.Color;
            if (color.HasValue) primary = Color.FromArgb(-16_777_216 | color.Value);
            else primary = Color.White;

            // Get secondary color
            color = route.TextColor;
            if (color.HasValue) secondary = Color.FromArgb(-16_777_216 | color.Value);
            else
            {
                float luma = (0.299f * primary.R + 0.587f * primary.G + 0.114f * primary.B) / 255;
                if (luma >= 0.5) secondary = Color.Black;
                else secondary = Color.White;
            }

            return new Tuple<Color, Color>(primary, secondary);
        }
    }
}