using System;
using System.Collections.Generic;
using System.Linq;
using GTFS.Entities;
using GTFS.Entities.Enumerations;
using GTFS_Explorer.Core.Interfaces.RepoInterfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
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

		public ScheduleModel(
			IRoutesRepository routesRepository,
			ITimepointRepository timepointRepository,
			IStopsRepository stopsRepository, 
            ITripsRepository tripsRepository)
		{
			_routesRepository = routesRepository;
			_timepointRepository = timepointRepository;
			_stopsRepository = stopsRepository;
			_tripsRepository = tripsRepository;
		}

		[BindProperty(SupportsGet = true)] //By default today's date
        public DateTime ScheduleDate { get; set; } = DateTime.UtcNow;

        [BindProperty(SupportsGet = true)] //By default OneDirection
        public string RouteDirection { get; set; } = DirectionType.OneDirection.ToString();

        [BindProperty]
        public string RouteId { get; set; }

		public Grid<string> Schedule { get; set; }
        public List<Stop> Stops { get; set; } = new List<Stop>();
        public IEnumerable<DirectionType?> DirectionsOfRoute { get; set; } 

        public void OnGet(
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
            Enum.TryParse(RouteDirection, out DirectionType direction);

            Schedule = _routesRepository.GetSchedule(
                routeId, direction, 
                _routesRepository.ServicesOn(dateResult.Value), 
                _timepointRepository.GetTimepointStrategy());

            GridLine<string> stopIds = (GridLine<string>)Schedule.GetRow(0);
            foreach (string stopId in stopIds.Skip(1))
			{
                Stops.Add(_stopsRepository.GetStopById(stopId));
			}
        }

        public IActionResult OnPostCreateSchedule(string routeDirection)
		{
            return RedirectToPage(new { 
                routeId = RouteId, 
                scheduleDate = ScheduleDate.ToString("yyyy-MM-dd"),
				routeDirection
			});
		}
    }
}