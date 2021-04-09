using System;
using System.Collections.Generic;
using System.Linq;
using GTFS;
using GTFS.Entities;
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

      // Mapping of all line segments to draw in the map
      Dictionary<Coordinate, HashSet<Coordinate>> segments =
        new GeneratorDictionary<Coordinate, HashSet<Coordinate>>(
          new FuncGenerator<Coordinate, HashSet<Coordinate>>(x => new HashSet<Coordinate>()));

      // Now get all the a-to-b connections available
      foreach (string shapeID in shapeIDs)
      {
        Coordinate? last = null;
        var shapePoints = feed.Shapes.Where(x => x.Id == shapeID).OrderBy(x => x.Sequence).Select(x => new Coordinate(x.Latitude, x.Longitude));
        foreach (Coordinate now in shapePoints)
        {
          if (last.HasValue)
          {
            segments[last.Value].Add(now);
          }
        }
      }

      foreach (Trip slTrip in shapeless)
      {
        Coordinate? last = null;
        var shapePoints = feed.StopTimes
          .Where(x => x.TripId == slTrip.Id)
          .OrderBy(x => x.StopSequence)
          .Select(x => feed.Stops.Get(x.StopId))
          .Select(x => new Coordinate(x.Latitude, x.Longitude));
        foreach (Coordinate now in shapePoints)
        {
          if (last.HasValue)
          {
            segments[last.Value].Add(now);
          }
        }
      }

      // Now string them back into lines
      List<List<Coordinate>> loops = new List<List<Coordinate>>();
      Dictionary<Coordinate, List<Coordinate>> routePieces = new Dictionary<Coordinate, List<Coordinate>>();

      while (segments.Any())
      {
        List<Coordinate> routeLine = new List<Coordinate>();
        Coordinate now = segments.Keys.First();
        bool forward = true;
        if (routePieces.ContainsKey(now))
        {
          routeLine = routePieces[now];
          routePieces.Remove(now);
          if (routeLine[0] == now)
          {
            forward = false;
          }
        }
        else
        {
          routePieces.Add(now, routeLine);
          routeLine.Add(now);
        }

        while (true)
        {
          if (!segments.ContainsKey(now)) break;
          HashSet<Coordinate> nexts = segments[now];
          Coordinate next = nexts.First();
          nexts.Remove(next);
          if (nexts.Count == 0) segments.Remove(now);
          now = next;
          routeLine.Add(now);
        }

        if (routePieces.ContainsKey(now))
        {
          // there are two pieces with a common endpoint that we need to merge
          List<Coordinate> routeLine2 = routePieces[now];
          bool forward2 = routeLine2[0] == now;
          Coordinate otherEnd = (forward) ? routeLine2[-1] : routeLine2[0];
          // first make sure we're not looking at the exact same list (i.e. a loop)
          if (otherEnd == now)
          {
            // oh, we are? remove it from dict, add it to loops
            routePieces.Remove(now);
            loops.Add(routeLine);
          }
          else
          {
            if (!forward) routeLine.Reverse();
            if (!forward2) routeLine2.Reverse();
            routeLine2.RemoveAt(0);
            routeLine.AddRange(routeLine2);
            routePieces[otherEnd] = routeLine;
          }
        }
      }

      foreach (var piece in routePieces.Values)
      {
        loops.Add(piece);
      }

      return loops;
    }
  }

  public struct Coordinate
  {
    public double Latitude;
    public double Longitude;
    public Coordinate(double lat, double lon)
    {
      Latitude = lat;
      Longitude = lon;
    }
    public override int GetHashCode()
    {
      return Latitude.GetHashCode() ^ Longitude.GetHashCode();
    }
    public override bool Equals(object obj)
    {
      if (!(obj is Coordinate right)) return false;
      return (Latitude == right.Latitude && Longitude == right.Longitude);
    }
    public static bool operator ==(Coordinate left, Coordinate right) => left.Equals(right);
    public static bool operator !=(Coordinate left, Coordinate right) => !(left.Equals(right));
  }
}