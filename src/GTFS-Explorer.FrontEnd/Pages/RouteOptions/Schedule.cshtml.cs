using System.Collections.Generic;
using System.Linq;
using GTFS.Entities;
using GTFS.Entities.Enumerations;
using GTFS_Explorer.BackEnd.Readers;
using GTFS_Explorer.Core.Interfaces.RepoInterfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Nixill.Collections.Grid;

namespace GTFS_Explorer.FrontEnd.Pages.RouteOptions
{
    public class ScheduleModel : PageModel
    {
        private readonly GTFSFeedReader _reader;
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

		public string RouteId { get; set; }
        public Route Route { get; set; }
		public Grid<string> Schedule { get; set; }
        public List<Stop> Stops { get; set; } = new List<Stop>();

		public void OnGet(string routeId)
        {
            RouteId = routeId;
            Route = _routesRepository.GetRouteById(routeId);
            Schedule = _routesRepository.GetSchedule(
                routeId, DirectionType.OneDirection, "1", 
                _timepointRepository.GetTimepointStrategy());

            GridLine<string> stopIds = (GridLine<string>)Schedule.GetRow(0);
            foreach (string stopId in stopIds.Skip(1))
			{
                Stops.Add(_stopsRepository.GetStopById(stopId));
			}
        }
    }
}