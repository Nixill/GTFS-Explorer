using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GTFS.Entities;
using GTFS.Entities.Enumerations;
using GTFS_Explorer.BackEnd.SignalR;
using GTFS_Explorer.Core.Interfaces.RepoInterfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.SignalR;

namespace GTFS_Explorer.FrontEnd.Pages.RouteOptions
{
    public class StopListModel : PageModel
    {
        private readonly IRoutesRepository _routesRepository;
        private readonly ITripsRepository _tripsRepository;
        private readonly IHubContext<EventsHub> _hubContext;

		public StopListModel(
			IRoutesRepository routesRepository,
			ITripsRepository tripsRepository, 
            IHubContext<EventsHub> hubContext)
		{
			_routesRepository = routesRepository;
			_tripsRepository = tripsRepository;
			_hubContext = hubContext;
		}

		public string RouteId { get; set; }
        public Route Route { get; set; }
		public IEnumerable<Tuple<Stop, bool>> StopsFromRoute { get; set; }

        [BindProperty(SupportsGet = true)] //By default OneDirection
        public string RouteDirection { get; set; } = DirectionType.OneDirection.ToString();
        public IEnumerable<DirectionType?> DirectionsOfRoute { get; set; }

		public async Task OnGet(string routeId, string routeDirection)
        {
            RouteId = routeId;
            Route = _routesRepository.GetRouteById(RouteId);

            if (!string.IsNullOrEmpty(routeDirection))
                RouteDirection = routeDirection;

            DirectionsOfRoute = _tripsRepository.GetDirectionsOfRoute(RouteId);

            DirectionType direction;
            if (DirectionsOfRoute.Count() == 1)
                direction = (DirectionType)DirectionsOfRoute.First();
            else
                Enum.TryParse(RouteDirection, out direction);

            await _hubContext.Clients.All.SendAsync("loading-file");

            StopsFromRoute = _routesRepository.GetRouteStops(RouteId, direction);
        }

        public IActionResult OnPost(string routeDirection)
        {
            return RedirectToPage(new
            {
                routeId = RouteId,
                routeDirection
            });
        }
    }
}