using System;
using System.Collections.Generic;
using ElectronNET.API;
using GTFS.Entities;
using GTFS.Entities.Enumerations;
using GTFS_Explorer.BackEnd.Repositiories;
using GTFS_Explorer.Core.Interfaces.RepoInterfaces;
using GTFS_Explorer.Core.Models.Structs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NodaTime.Text;

namespace GTFS_Explorer.FrontEnd.Pages.RouteOptions
{
    public class RouteSummaryModel : PageModel
    {
        private readonly IRoutesRepository _routesRepository;

		public RouteSummaryModel(IRoutesRepository routesRepository)
        {
            _routesRepository = routesRepository;
        }

        //From URL:
        [BindProperty]
        public string RouteId { get; set; }

        [BindProperty]
        public RouteStats RouteStats { get; set; }

        [BindProperty(SupportsGet = true)]
        public DateTime ServicesDate { get; set; }

        public Route Route { get; set; }

        public void OnGet(string routeId, DateTime? servicesDate)
        {
            if (servicesDate.HasValue)
                ServicesDate = (DateTime)servicesDate;

            var dateResult = LocalDatePattern.Iso.Parse(ServicesDate.ToString("yyyy-MM-dd"));
            RouteId = routeId;
            Route = _routesRepository.GetRouteById(RouteId);

            var statsDict = _routesRepository.GetRouteStats(dateResult.Value, routeId);

			foreach (var item in statsDict.Keys)
			{

			}

            Electron.IpcMain.On("open-route-link", async (args) =>
            {
                await Electron.Shell.OpenExternalAsync(Route.Url);
            });
        }

        public IActionResult OnPostRouteStats()
        {
            return RedirectToPage(new
            {
                routeId = RouteId,
                scheduleDate = ServicesDate.ToString("yyyy-MM-dd")
            });
        }
    }
}