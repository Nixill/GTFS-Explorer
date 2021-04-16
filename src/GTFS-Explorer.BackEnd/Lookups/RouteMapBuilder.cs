using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using GTFS;
using GTFS.Entities;
using GTFS_Explorer.Core.Models.Structs;
using Nixill.Collections;

namespace GTFS_Explorer.BackEnd.Lookups
{
    public static class RouteMapBuilder
    {
        public static List<List<Coordinate>> GetShapes(GTFSFeed feed, string route)
        {
            var trips = feed.Trips.Where(x => x.RouteId == route);

            var shapeIDs = trips.Select(x => x.ShapeId).Distinct();
            var shapeless = trips.Where(x => x.ShapeId == null);

            List<List<Coordinate>> ret = new List<List<Coordinate>>();

            // First draw all the shapes
            foreach (string shapeID in shapeIDs)
            {
                var shapePoints = feed.Shapes.Where(x => x.Id == shapeID).OrderBy(x => x.Sequence).Select(x => new Coordinate(x.Latitude, x.Longitude));
                List<Coordinate> line = new List<Coordinate>();
                foreach (Coordinate now in shapePoints)
                {
                    line.Add(now);
                }
                ret.Add(line);
            }

            // Then connect-the-dots on shapeless trips
            foreach (Trip slTrip in shapeless)
            {
                var shapePoints = feed.StopTimes
                  .Where(x => x.TripId == slTrip.Id)
                  .OrderBy(x => x.StopSequence)
                  .Select(x => feed.Stops.Get(x.StopId))
                  .Select(x => new Coordinate(x.Latitude, x.Longitude));
                List<Coordinate> line = new List<Coordinate>();
                foreach (Coordinate now in shapePoints)
                {
                    line.Add(now);
                }
                ret.Add(line);
            }

            // And return it
            return ret;
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

        public static List<Tuple<Stop, bool>> GetStops(GTFSFeed feed, string routeID)
        {
            return feed.Trips
              .Where(x => x.RouteId == routeID)
              .SelectMany(x =>
                feed.StopTimes
                  .Where(y => y.TripId == x.Id)
                  .Select(y => new Tuple<Stop, bool>(feed.Stops.Get(y.StopId), y.TimepointType == GTFS.Entities.Enumerations.TimePointType.Exact))
              ).Distinct().ToList();
        }
    }

    //public readonly struct Coordinate
    //{
    //  public readonly double Latitude;
    //  public readonly double Longitude;
    //  public Coordinate(double lat, double lon)
    //  {
    //    Latitude = lat;
    //    Longitude = lon;
    //  }
    //  public override int GetHashCode()
    //  {
    //    return Latitude.GetHashCode() ^ Longitude.GetHashCode();
    //  }
    //  public override bool Equals(object obj)
    //  {
    //    if (!(obj is Coordinate right)) return false;
    //    return (Latitude == right.Latitude && Longitude == right.Longitude);
    //  }
    //  public static bool operator ==(Coordinate left, Coordinate right) => left.Equals(right);
    //  public static bool operator !=(Coordinate left, Coordinate right) => !(left.Equals(right));
    //}
}