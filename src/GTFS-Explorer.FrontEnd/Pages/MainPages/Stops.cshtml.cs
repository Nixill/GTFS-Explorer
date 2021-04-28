using GTFS.Entities;
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
        }

        public List<Stop> Stops { get; set; } = new List<Stop>();

        public void OnGet()
        {
            Stops = _stopsRepository.GetStopList();
        }
    }
}