using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System.IO;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using GTFS_Explorer.BackEnd.Utilities;
using ElectronNET.API;
using GTFS_Explorer.BackEnd.Extensions;

namespace GTFS_Explorer.FrontEnd.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly IWebHostEnvironment _environment;

        public Tuple<bool, string> isValidFile { get; set; } 
            = new Tuple<bool, string>(false, "");

        [BindProperty]
        public IFormFile UploadedFile { get; set; }

        public IndexModel(ILogger<IndexModel> logger, IWebHostEnvironment env)
        {
            _logger = logger;
            _environment = env;
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (UploadedFile == null)
            {
                isValidFile = new Tuple<bool, string>(false, "No file uploaded!");
                return Page();
            }

            string file = Electron.App.GetGTFSFilePath(_environment, UploadedFile.FileName);
            
            using (var stream = new FileStream(file, FileMode.Create))
            {
                await UploadedFile.CopyToAsync(stream);
            }

            isValidFile = Validator.IsValidGTFS(file);
            if (!isValidFile.Item1 && UploadedFile != null)
            {
                System.IO.File.Delete(file);
                return null;
            }

            return RedirectToPage("/MainPages/Selection");
        }
    }
}