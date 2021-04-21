using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GTFS.Entities;
using GTFS_Explorer.BackEnd.SignalR;
using GTFS_Explorer.Core.Interfaces;
using GTFS_Explorer.Core.Interfaces.RepoInterfaces;
using GTFS_Explorer.Core.Models.Structs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.SignalR;

namespace GTFS_Explorer.FrontEnd.Pages.RouteOptions
{
    public class TripModel : PageModel
    {
        private readonly ITripsRepository _tripsRepository;
        private readonly IHubContext<EventsHub> _hubContext;
        private readonly IStopsRepository _stopsRepository;
        private readonly ITripMapBuilderService _tripMapBuilderService;

		public TripModel(
			ITripsRepository tripsRepository,
			IHubContext<EventsHub> hubContext,
			IStopsRepository stopsRepository, 
			ITripMapBuilderService tripMapBuilderService)
		{
			_tripsRepository = tripsRepository;
			_hubContext = hubContext;
			_stopsRepository = stopsRepository;
			_tripMapBuilderService = tripMapBuilderService;
		}

		[BindProperty(SupportsGet = true)]
        public DateTime TripDate { get; set; } = DateTime.Now;
        public string RouteId { get; set; }
		public string TripId { get; set; }
		public Trip Trip { get; set; }
		public List<Tuple<TimeOfDay?, Stop>> TripStops { get; set; }
		public List<Coordinate> TripShapes { get; set; }

		public async Task OnGetAsync(string routeId, string tripId, DateTime? tripDate)
        {
            if (tripDate.HasValue)
                TripDate = (DateTime)tripDate;

            await _hubContext.Clients.All.SendAsync("loading-file");

            RouteId = routeId;
            TripId = tripId;
            Trip = _tripsRepository.GetTripById(TripId);
            TripStops = _stopsRepository.GetStopsFromTrip(TripId);
			TripShapes = _tripMapBuilderService.GetAllTripShapes(Trip);
		}

  //      public IActionResult OnPost()
		//{
  //          return RedirectToPage(new
  //          {
  //              routeId = RouteId,
  //              tripId = TripId,
  //              tripDate = TripDate.ToString("yyyy-MM-dd")
  //          });
		//}
    }
}