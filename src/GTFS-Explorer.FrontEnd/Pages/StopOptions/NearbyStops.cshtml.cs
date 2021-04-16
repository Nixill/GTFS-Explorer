using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GTFS_Explorer.FrontEnd.Pages.StopOptions
{
    public class NearbyStopsModel : PageModel
    {
        public NearbyStopsModel()
        {

        }

        public string StopId { get; set; }

        public void OnGet(string stopId)
        {
            StopId = stopId;
        }
    }
}