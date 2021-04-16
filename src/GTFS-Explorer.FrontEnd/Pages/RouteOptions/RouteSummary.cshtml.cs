using System;
using System.Collections.Generic;
using System.Globalization;
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
using NodaTime;
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
        public DateTime ServicesDate { get; set; } = DateTime.Now;

        public Dictionary<DirectionType?, RouteStats> StatsDictionary { get; set; }
        public Route Route { get; set; }

        public string DurationToTimeOfDay(Duration dur)
        {
            string timePart = (dur.Minus(Duration.FromDays((int)(dur.TotalDays)))).ToString("HH:mm", DateTimeFormatInfo.InvariantInfo);
            string dayPart = "";
            if (dur.TotalDays >= 2)
            {
                dayPart = $" (+{dur.TotalDays} days)";
            }
            else if (dur.TotalDays >= 1)
            {
                dayPart = " (the next day)";
            }
            return timePart + dayPart;
        }

        public string DurationToLengthString(Duration dur)
        {
            int duration = (int)dur.TotalSeconds;

            int secs, mins, hrs;

            if (duration < 300)
            {
                secs = duration % 60;
            }
            else
            {
                secs = 0;
                duration += 30; // For rounding
            }

            duration /= 60;

            mins = duration % 60;
            hrs = duration / 60;

            string durDesc = "";

            if (hrs > 1)
            {
                durDesc += $" {hrs} hours";
            }
            else if (hrs == 1)
            {
                durDesc += " 1 hour";
            }

            if (mins > 1)
            {
                durDesc += $" {mins} minutes";
            }
            else if (mins == 1)
            {
                durDesc += " 1 minute";
            }

            if (secs > 1)
            {
                durDesc += $" {secs} seconds";
            }
            else if (secs == 1)
            {
                durDesc += " 1 second";
            }

            return durDesc[1..];
        }

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