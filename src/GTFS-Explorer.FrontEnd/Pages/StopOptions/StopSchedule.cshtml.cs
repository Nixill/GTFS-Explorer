using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GTFS.Entities;
using GTFS_Explorer.BackEnd.SignalR;
using GTFS_Explorer.Core.Interfaces.RepoInterfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.SignalR;
using NodaTime.Text;

namespace GTFS_Explorer.FrontEnd.Pages.StopOptions
{
    public class StopScheduleModel : PageModel
    {
        public readonly IStopsRepository _stopsRepository;
        private readonly IHubContext<EventsHub> _hubContext;
        private readonly IRoutesRepository _routesRepository;

		public StopScheduleModel(
			IStopsRepository stopsRepository,
			IHubContext<EventsHub> hubContext, 
            IRoutesRepository routesRepository)
		{
			_stopsRepository = stopsRepository;
			_hubContext = hubContext;
			_routesRepository = routesRepository;
		}

		public Dictionary<Route, List<StopTime>> StopSchedule { get; set; }
		public string StopId { get; set; }
        public DateTime ScheduleDate { get; set; } = DateTime.Now;
		public IEnumerable<Route> RoutesServingStop { get; set; }

		public async Task OnGet(string stopId, DateTime? scheduleDate)
        {
            if (scheduleDate.HasValue)
                ScheduleDate = (DateTime)scheduleDate;

            StopId = stopId;
            var dateResult = LocalDatePattern.Iso.Parse(ScheduleDate.ToString("yyyy-MM-dd"));

            await _hubContext.Clients.All.SendAsync("loading-file");
            RoutesServingStop = _routesRepository.GetRoutesServingStop(StopId);
            StopSchedule = _stopsRepository.GetStopSchedule(StopId, dateResult.Value);
        }

        public IActionResult OnPost()
		{
            return RedirectToPage(new 
            { 
                stopId = StopId, 
                scheduleDate = ScheduleDate.ToString("yyyy-MM-dd")
            });
		}
    }
}