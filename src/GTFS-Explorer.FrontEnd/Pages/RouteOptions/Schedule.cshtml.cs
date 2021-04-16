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
using Nixill.Collections.Grid;
using NodaTime.Text;

namespace GTFS_Explorer.FrontEnd.Pages.RouteOptions
{
    public class ScheduleModel : PageModel
    {
        private readonly IRoutesRepository _routesRepository;
        private readonly ITimepointRepository _timepointRepository;
        private readonly IStopsRepository _stopsRepository;
        private readonly ITripsRepository _tripsRepository;
        private readonly IHubContext<EventsHub> _hubContext;

        public ScheduleModel(
            IRoutesRepository routesRepository,
            ITimepointRepository timepointRepository,
            IStopsRepository stopsRepository,
            ITripsRepository tripsRepository,
            IHubContext<EventsHub> hubContext)
        {
            _routesRepository = routesRepository;
            _timepointRepository = timepointRepository;
            _stopsRepository = stopsRepository;
            _tripsRepository = tripsRepository;
            _hubContext = hubContext;
        }

        [BindProperty(SupportsGet = true)] //By default today's date
        public DateTime ScheduleDate { get; set; } = DateTime.Now;

        [BindProperty(SupportsGet = true)] //By default OneDirection
        public string RouteDirection { get; set; } = DirectionType.OneDirection.ToString();

        [BindProperty]
        public string RouteId { get; set; }

        public Grid<string> Schedule { get; set; }
        public List<Stop> Stops { get; set; } = new List<Stop>();
        public IEnumerable<DirectionType?> DirectionsOfRoute { get; set; }
        public bool IsGenericSchedule { get; set; }

        public async Task OnGetAsync(
            string routeId,
            DateTime? scheduleDate,
            string routeDirection)
        {
            RouteId = routeId;

            if (scheduleDate.HasValue)
                ScheduleDate = (DateTime)scheduleDate;
            if (!string.IsNullOrEmpty(routeDirection))
                RouteDirection = routeDirection;

            var dateResult = LocalDatePattern.Iso.Parse(ScheduleDate.ToString("yyyy-MM-dd"));
            DirectionsOfRoute = _tripsRepository.GetDirectionsOfRoute(RouteId);
            DirectionType direction;
            if (DirectionsOfRoute.Count() == 1)
                direction = (DirectionType)DirectionsOfRoute.First();
            else
                Enum.TryParse(RouteDirection, out direction);

            await _hubContext.Clients.All.SendAsync("loading-file");

            var services = _routesRepository.ServicesOn(dateResult.Value);
            IsGenericSchedule = services.Item2;

            Schedule = _routesRepository.GetSchedule(
                routeId, direction,
                services.Item1,
                _timepointRepository.GetTimepointStrategy());

            GridLine<string> stopIds = (GridLine<string>)Schedule.GetRow(0);
            foreach (string stopId in stopIds.Skip(1))
            {
                Stops.Add(_stopsRepository.GetStopById(stopId));
            }
        }

        public IActionResult OnPostCreateSchedule(string routeDirection)
        {
            return RedirectToPage(new
            {
                routeId = RouteId,
                scheduleDate = ScheduleDate.ToString("yyyy-MM-dd"),
                routeDirection
            });
        }
    }
}