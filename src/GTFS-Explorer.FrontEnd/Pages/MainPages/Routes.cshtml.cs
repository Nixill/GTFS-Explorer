using System.Collections.Generic;
using GTFS.Entities;
using GTFS_Explorer.Core.Interfaces.RepoInterfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GTFS_Explorer.FrontEnd.Pages.MainPages
{
    public class RoutesModel : PageModel
    {
        private readonly IRoutesRepository _routesRepository;

        public List<Route> Routes { get; set; } = new List<Route>();

        public RoutesModel(IRoutesRepository routesRepository)
        {
            _routesRepository = routesRepository;
            GetRoutes();
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        private void GetRoutes()
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