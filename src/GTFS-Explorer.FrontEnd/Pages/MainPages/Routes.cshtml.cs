using System.Collections.Generic;
using GTFS.Entities;
using GTFS_Explorer.Core.Interfaces.RepoInterfaces;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GTFS_Explorer.FrontEnd.Pages.MainPages
{
    public class RoutesModel : PageModel
    {
        private readonly IRoutesRepository _routesRepository;

        public RoutesModel(IRoutesRepository routesRepository)
        {
            _routesRepository = routesRepository;
        }
        public List<Route> Routes { get; set; } = new List<Route>();

        public void OnGet()
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