using System;
using System.Collections.Generic;
using System.Linq;
using GTFS.Entities;
using GTFS.Entities.Enumerations;
using GTFS_Explorer.Core.Interfaces.RepoInterfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Nixill.Collections.Grid;
using NodaTime.Text;

namespace GTFS_Explorer.FrontEnd.Pages.RouteOptions
{
    public class ScheduleModel : PageModel
    {
        private readonly IRoutesRepository _routesRepository;
        private readonly ITimepointRepository _timepointRepository;
        private readonly IStopsRepository _stopsRepository;

		public ScheduleModel(
			IRoutesRepository routesRepository,
			ITimepointRepository timepointRepository,
			IStopsRepository stopsRepository)
		{
			_routesRepository = routesRepository;
			_timepointRepository = timepointRepository;
			_stopsRepository = stopsRepository;
		}

        [BindProperty(SupportsGet = true)] //By default today's date
        public DateTime ScheduleDate { get; set; } = DateTime.UtcNow;

        [BindProperty]
        public string RouteId { get; set; }

		public Grid<string> Schedule { get; set; }

        public List<Stop> Stops { get; set; } = new List<Stop>();

		public void OnGet(string routeId, DateTime? scheduleDate)
        {
            if (scheduleDate.HasValue)
                ScheduleDate = (DateTime)scheduleDate;

            var dateResult = LocalDatePattern.Iso.Parse(ScheduleDate.ToString("yyyy-MM-dd"));

            RouteId = routeId;
            Schedule = _routesRepository.GetSchedule(
                routeId, DirectionType.OneDirection, 
                _routesRepository.ServicesOn(dateResult.Value), 
                _timepointRepository.GetTimepointStrategy());

            GridLine<string> stopIds = (GridLine<string>)Schedule.GetRow(0);
            foreach (string stopId in stopIds.Skip(1))
			{
                Stops.Add(_stopsRepository.GetStopById(stopId));
			}
        }

        public IActionResult OnPostCreateSchedule()
		{
            return RedirectToPage(new { 
                routeId = RouteId, 
                scheduleDate = ScheduleDate.ToString("yyyy-MM-dd") 
            });
		}
    }
}