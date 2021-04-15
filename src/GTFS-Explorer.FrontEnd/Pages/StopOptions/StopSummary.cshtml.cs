using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GTFS.Entities;
using GTFS_Explorer.Core.Interfaces.RepoInterfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GTFS_Explorer.FrontEnd.Pages.StopOptions
{
    public class StopSummaryModel : PageModel
    {
        private readonly IStopsRepository _stopsRepository;

		public StopSummaryModel(IStopsRepository stopsRepository)
		{
			_stopsRepository = stopsRepository;
		}

		public string StopId { get; set; }
		public Stop Stop { get; set; }

		public void OnGet(string stopId)
        {
			StopId = stopId;
			Stop = _stopsRepository.GetStopById(StopId);
		}
    }
}