using GTFS.Entities;
using GTFS_Explorer.Core.Enums;
using GTFS_Explorer.Core.Interfaces.RepoInterfaces;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;

namespace GTFS_Explorer.FrontEnd.Pages.MainPages
{
    public class StopsModel : PageModel
    {
        private readonly IStopsRepository _stopsRepository;
        public StopsModel(IStopsRepository stopsRepository)
        {
            _stopsRepository = stopsRepository;
            Stops = _stopsRepository.GetAllStops();
        }

        //public Dictionary<Stop, StopMajority> StopsDictionary;
        public List<Stop> Stops;

        public void OnGet()
        {
        }
    }
}