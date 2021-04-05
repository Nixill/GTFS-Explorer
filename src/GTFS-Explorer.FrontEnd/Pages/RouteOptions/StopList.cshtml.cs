using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GTFS.Entities;
using GTFS_Explorer.Core.Interfaces.RepoInterfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GTFS_Explorer.FrontEnd.Pages.RouteOptions
{
    public class StopListModel : PageModel
    {
        private readonly IRoutesRepository _routesRepository;

        public StopListModel(IRoutesRepository routesRepository)
        {
            _routesRepository = routesRepository;
        }

        public string RouteId { get; set; }
        public Route Route { get; set; }

        public void OnGet(string routeId)
        {
            RouteId = routeId;
            Route = _routesRepository.GetRouteById(RouteId);
        }
    }
}