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
  public class RouteMapBuilderService : IRouteMapBuilderService
  {
    private readonly GTFSFeedReader _reader;

    public RouteMapBuilderService(GTFSFeedReader reader)
    {
      _reader = reader;
    }

    public List<List<Coordinate>> GetShapes(string route) =>
      RouteMapBuilder.GetShapes(_reader.Feed, route);

    public Tuple<Color, Color> GetRouteColors(Route route) =>
      RouteMapBuilder.GetRouteColors(route);

    public List<Tuple<Stop, bool>> GetStops(string routeID) =>
      RouteMapBuilder.GetStops(_reader.Feed, routeID);
  }
}