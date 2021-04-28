using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using GTFS.Entities;
using GTFS_Explorer.BackEnd.SignalR;
using GTFS_Explorer.Core.Interfaces;
using GTFS_Explorer.Core.Interfaces.RepoInterfaces;
using GTFS_Explorer.Core.Models.Structs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.SignalR;

namespace GTFS_Explorer.FrontEnd.Pages.MainPages
{
    public class SystemMapModel : PageModel
    {
        private readonly IRoutesRepository _routesRepository;
        private readonly ISystemMapBuilderService _systemMapBuilderService;
        private readonly IHubContext<EventsHub> _hubContext;

		public SystemMapModel(
			IRoutesRepository routesRepository,
			ISystemMapBuilderService systemMapBuilderService, 
            IHubContext<EventsHub> hubContext)
		{
			_routesRepository = routesRepository;
			_systemMapBuilderService = systemMapBuilderService;
            _hubContext = hubContext;
        }

		public List<Route> Routes { get; set; } = new List<Route>();
		public IEnumerable<Tuple<Tuple<Color, Color>, IEnumerable<Coordinate>>> ShapeLines { get; set; }

		public async Task OnGetAsync()
        {
            await _hubContext.Clients.All.SendAsync("loading-file");
            InitRoutes();
            ShapeLines = _systemMapBuilderService.GetRouteShapeLines();
        }

        private void InitRoutes()
        {
            var routes = _routesRepository.GetAllRoutes();
            foreach (var agency in routes.Keys)
            {
                List<Route> agencyRouteList;
                routes.TryGetValue(agency, out agencyRouteList);
                Routes.AddRange(agencyRouteList);
            }
        }
    }
}