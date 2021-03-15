using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GTFS.Entities;
using GTFS_Explorer.Core.Interfaces.RepoInterfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GTFS_Explorer.FrontEnd.Pages.MainPages
{
    public class RoutesModel : PageModel
    {
        private readonly IRoutesRepository _routesRepository;

        public List<Route> Routes = new List<Route>();

        public RoutesModel(IRoutesRepository routesRepository)
        {
            _routesRepository = routesRepository;
            Routes = _routesRepository.GetRoutesList();
        }

        public void OnGet()
        {
            //Routes = await Task.Run(_routesRepository.GetRoutesList);
        }
    }
}