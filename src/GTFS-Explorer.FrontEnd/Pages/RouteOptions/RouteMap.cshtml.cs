using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using GTFS.Entities;
using GTFS_Explorer.BackEnd.SignalR;
using GTFS_Explorer.Core.Interfaces;
using GTFS_Explorer.Core.Interfaces.RepoInterfaces;
using GTFS_Explorer.Core.Models.Structs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.SignalR;

namespace GTFS_Explorer.FrontEnd.Pages.RouteOptions
{
    public class RouteMapModel : PageModel
    {
        private readonly IRoutesRepository _routesRepository;
        private readonly IRouteMapBuilderService _routeMapBuilderService;
        private readonly IHubContext<EventsHub> _hubContext;

        public RouteMapModel(
            IRoutesRepository routesRepository,
            IRouteMapBuilderService routeMapBuilderService,
            IHubContext<EventsHub> hubContext)
        {
            _routesRepository = routesRepository;
            _routeMapBuilderService = routeMapBuilderService;
            _hubContext = hubContext;
        }

        public string RouteId { get; set; }
        public Route Route { get; set; }
        public List<List<Coordinate>> RouteShapes { get; set; }
        public Tuple<Color, Color> RouteColors { get; set; }
        public List<Tuple<Stop, bool>> RouteStops { get; set; }

        public async Task OnGetAsync(string routeId)
        {
            RouteId = routeId;
            Route = _routesRepository.GetRouteById(RouteId);
            await _hubContext.Clients.All.SendAsync("loading-file");

            RouteShapes = _routeMapBuilderService.GetShapes(routeId);
            RouteColors = _routeMapBuilderService.GetRouteColors(Route);
            RouteStops = _routeMapBuilderService.GetStops(routeId);
        }
    }
}