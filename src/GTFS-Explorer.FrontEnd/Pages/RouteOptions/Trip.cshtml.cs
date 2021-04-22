using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using GTFS.Entities;
using GTFS.Entities.Enumerations;
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
    private readonly IRouteMapBuilderService _routeMapBuilderService;
    private readonly IRoutesRepository _routesRepository;
    private readonly ITripMapBuilderService _tripMapBuilderService;

    public TripModel(
        ITripsRepository tripsRepository,
        IHubContext<EventsHub> hubContext,
        IStopsRepository stopsRepository,
        IRouteMapBuilderService routeMapBuilderService,
        IRoutesRepository routesRepository,
        ITripMapBuilderService tripMapBuilderService)
    {
      _tripsRepository = tripsRepository;
      _hubContext = hubContext;
      _stopsRepository = stopsRepository;
      _routeMapBuilderService = routeMapBuilderService;
      _routesRepository = routesRepository;
      _tripMapBuilderService = tripMapBuilderService;
    }

    [BindProperty(SupportsGet = true)]
    public DateTime TripDate { get; set; } = DateTime.Now;
    public Route Route { get; set; }
    public string RouteId { get; set; }
    public string TripId { get; set; }
    public Trip Trip { get; set; }
    public List<Tuple<Stop, TimeOfDay?, TimeOfDay?, bool, PickupType, DropOffType>> TripStops { get; set; }
    public List<Coordinate> TripShapes { get; set; }
    public Tuple<Color, Color> TripColors { get; set; }

    public async Task OnGetAsync(string routeId, string tripId, DateTime? tripDate)
    {
      if (tripDate.HasValue)
        TripDate = (DateTime)tripDate;

      await _hubContext.Clients.All.SendAsync("loading-file");

      RouteId = routeId;
      TripId = tripId;
      Route = _routesRepository.GetRouteById(routeId);
      Trip = _tripsRepository.GetTripById(TripId);
      TripStops = _stopsRepository.GetStopsFromTrip(TripId);
      TripShapes = _tripMapBuilderService.GetAllTripShapes(Trip);
      TripColors = _routeMapBuilderService.GetRouteColors(Route);
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

    public string PickupDropoffRule(PickupType pick, DropOffType drop)
    => (((int)pick) * 4 + ((int)drop)) switch
    {
      1 => "Pickup only",
      2 => "Call agency for drop-off",
      3 => "Ask driver for drop-off",
      4 => "Drop-off only",
      5 => "Timing only",
      6 => "No pickup, call agency for drop-off",
      7 => "No pickup, ask driver for drop-off",
      8 => "Call agency for pickup",
      9 => "No drop-off, call agency for pickup",
      10 => "Call agency",
      11 => "Ask driver for drop-off, call agency for pickup",
      12 => "Ask driver for pickup",
      13 => "No drop-off, ask driver for pickup",
      14 => "Call agency for drop-off, ask driver for pickup",
      15 => "Ask the driver",
      _ => ""
    };
  }
}