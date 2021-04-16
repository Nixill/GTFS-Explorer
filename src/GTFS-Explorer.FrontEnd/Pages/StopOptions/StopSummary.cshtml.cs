using System.Collections.Generic;
using System.Threading.Tasks;
using GTFS.Entities;
using GTFS_Explorer.BackEnd.SignalR;
using GTFS_Explorer.Core.Interfaces.RepoInterfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.SignalR;

namespace GTFS_Explorer.FrontEnd.Pages.StopOptions
{
    public class StopSummaryModel : PageModel
    {
        private readonly IRoutesRepository _routesRepository;
		private readonly IHubContext<EventsHub> _hubContext;
		private readonly IStopsRepository _stopsRepository;

		public StopSummaryModel(
			IRoutesRepository routesRepository,
			IHubContext<EventsHub> hubContext, 
			IStopsRepository stopsRepository)
		{
			_routesRepository = routesRepository;
			_hubContext = hubContext;
			_stopsRepository = stopsRepository;
		}

		public string StopId { get; set; }
		public Stop Stop { get; set; }
		public List<Route> RoutesServingStop { get; set; }

		public async Task OnGetAsync(string stopId)
        {
			StopId = stopId;
			Stop = _stopsRepository.GetStopById(StopId);
			await _hubContext.Clients.All.SendAsync("loading-file");
			RoutesServingStop = _routesRepository.GetRoutesServingStop(stopId);
		}
    }
}