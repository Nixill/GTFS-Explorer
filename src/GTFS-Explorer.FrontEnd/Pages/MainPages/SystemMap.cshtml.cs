using System.Collections.Generic;
using GTFS.Entities;
using GTFS_Explorer.Core.Interfaces;
using GTFS_Explorer.Core.Interfaces.RepoInterfaces;
using GTFS_Explorer.Core.Models.Structs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GTFS_Explorer.FrontEnd.Pages.MainPages
{
    public class SystemMapModel : PageModel
    {
        private readonly IRoutesRepository _routesRepository;
        private readonly IRouteMapBuilderService _routeMapBuilderService;

		public SystemMapModel(
			IRoutesRepository routesRepository,
			IRouteMapBuilderService routeMapBuilderService)
		{
			_routesRepository = routesRepository;
			_routeMapBuilderService = routeMapBuilderService;
            InitRoutes();
		}

		public List<Route> Routes { get; set; } = new List<Route>();
        public List<List<Coordinate>> RouteShapes { get; set; } = new List<List<Coordinate>>();

        public void OnGet()
        {
            List<List<List<Coordinate>>> allShapes = new List<List<List<Coordinate>>>();

			foreach (var route in Routes)
			{
                allShapes.Add(_routeMapBuilderService.GetShapes(route.Id));
            }
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