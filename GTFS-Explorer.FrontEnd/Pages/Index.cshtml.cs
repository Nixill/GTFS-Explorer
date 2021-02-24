using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace GTFS_Explorer.FrontEnd.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        public bool isValidFile { get; set; } = false;

        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }

        public void OnPost()
        {
            ValidateFile();
        }

        private void ValidateFile()
        {
            //GTFS file validation here
            //(TODO: Change isValidFile accordingly^)
        }
    }
}
