using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using GTFS.Entities;
using GTFS_Explorer.Core.Interfaces;
using GTFS_Explorer.Core.Interfaces.RepoInterfaces;
using GTFS_Explorer.Core.Models.Structs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GTFS_Explorer.FrontEnd.Pages.RouteOptions
{
  public class RouteMapModel : PageModel
  {
    private readonly IRoutesRepository _routesRepository;
    private readonly IRouteMapBuilderService _routeMapBuilderService;

    public RouteMapModel(
            IRoutesRepository routesRepository,
            IRouteMapBuilderService routeMapBuilderService)
    {
      _routesRepository = routesRepository;
      _routeMapBuilderService = routeMapBuilderService;
    }

    public string RouteId { get; set; }
    public Route Route { get; set; }
    public List<List<Coordinate>> RouteShapes { get; set; }
    public Tuple<Color, Color> RouteColors { get; set; }
    public List<Stop> RouteStops { get; set; }

    public void OnGet(string routeId)
    {
      RouteId = routeId;
      Route = _routesRepository.GetRouteById(RouteId);
      RouteShapes = _routeMapBuilderService.GetShapes(routeId);
      RouteColors = _routeMapBuilderService.GetRouteColors(Route);
      RouteStops = _routeMapBuilderService.GetStops(routeId);
    }
  }
}