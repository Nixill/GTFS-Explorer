using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ElectronNET.API;
using GTFS.Entities;
using GTFS.Entities.Enumerations;
using GTFS_Explorer.BackEnd.SignalR;
using GTFS_Explorer.Core.Interfaces.RepoInterfaces;
using GTFS_Explorer.Core.Models.Structs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.SignalR;
using NodaTime.Text;

namespace GTFS_Explorer.FrontEnd.Pages.RouteOptions
{
    public class RouteSummaryModel : PageModel
    {
        private readonly IRoutesRepository _routesRepository;
        private readonly IHubContext<EventsHub> _hubContext;

        public RouteSummaryModel(
            IRoutesRepository routesRepository,
            IHubContext<EventsHub> hubContext)
        {
            _routesRepository = routesRepository;
            _hubContext = hubContext;
        }

        //From URL:
        [BindProperty]
        public string RouteId { get; set; }

        [BindProperty(SupportsGet = true)] //By default today's date
        public DateTime ServicesDate { get; set; } = DateTime.UtcNow;

        public Dictionary<DirectionType?, RouteStats> StatsDictionary { get; set; }
        public Route Route { get; set; }

        public async Task OnGetAsync(string routeId, DateTime? servicesDate)
        {
            if (servicesDate.HasValue)
                ServicesDate = (DateTime)servicesDate;

            var dateResult = LocalDatePattern.Iso.Parse(ServicesDate.ToString("yyyy-MM-dd"));
            RouteId = routeId;
            Route = _routesRepository.GetRouteById(RouteId);

            await _hubContext.Clients.All.SendAsync("loading-file");

            StatsDictionary = _routesRepository.GetRouteStats(dateResult.Value, routeId);

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
                servicesDate = ServicesDate.ToString("yyyy-MM-dd")
            });
        }
    }
}