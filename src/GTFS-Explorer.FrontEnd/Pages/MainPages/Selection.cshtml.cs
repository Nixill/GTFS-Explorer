using GTFS_Explorer.BackEnd.Readers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GTFS_Explorer.FrontEnd.Pages.MainPages
{
    public class SelectionModel : PageModel
    {
        private readonly GTFSFeedReader _reader;
        private readonly IWebHostEnvironment _environment;

        public SelectionModel(IWebHostEnvironment environment, GTFSFeedReader reader)
        {
            _environment = environment;
            //To read the file when this page model is contructed
            //TODO: add a loading screen
            _reader = reader;
        }

        public void OnGet()
        {

        }
    }
}