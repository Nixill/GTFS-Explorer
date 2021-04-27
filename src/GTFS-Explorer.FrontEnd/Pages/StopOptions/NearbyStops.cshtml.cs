using System.Collections.Generic;
using System.Threading.Tasks;
using GTFS.Entities;
using GTFS_Explorer.BackEnd.SignalR;
using GTFS_Explorer.Core.Interfaces.RepoInterfaces;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.SignalR;

namespace GTFS_Explorer.FrontEnd.Pages.StopOptions
{
    public class NearbyStopsModel : PageModel
    {
        public readonly IStopsRepository _stopsRepository;
		private readonly IHubContext<EventsHub> _hubContext;

		public NearbyStopsModel(
			IStopsRepository stopsRepository, 
			IHubContext<EventsHub> hubContext)
		{
			_stopsRepository = stopsRepository;
			_hubContext = hubContext;
		}

		public string StopId { get; set; }
		public IEnumerable<Stop> NearbyStops { get; set; }

		public async Task OnGetAsync(string stopId)
        {
            StopId = stopId;
			await _hubContext.Clients.All.SendAsync("loading-file");
			NearbyStops = _stopsRepository.GetNearbyStops(StopId);
		}
    }
}