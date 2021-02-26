using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using GTFSExplorer.Backend;
using System.IO;
using System;
using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;
using ElectronNET.API;
using ElectronNET.API.Entities;
using Microsoft.AspNetCore.Hosting;

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

        public async Task OnPostAsync()
        {
            isValidFile = await IsValidFile();
            if (!isValidFile.Item1 && UploadedFile != null) 
            {
                string targetFileName = $"{_environment.WebRootPath}\\tempFiles\\{UploadedFile.FileName}";
                System.IO.File.Delete(targetFileName);
            }
        }

        protected async Task<Tuple<bool, string>> IsValidFile()
        {
            if (UploadedFile == null)
                return new Tuple<bool, string>(false, "No file uploaded!");

            /*If running electron: yourPath\GTFS-Explorer.FrontEnd\obj\Host\bin\wwwroot\tempFiles*/
            /*If running browser: yourPath\GTFS-Explorer.FrontEnd\wwwroot\tempFiles*/
            string targetFileName = $"{_environment.WebRootPath}\\tempFiles\\{UploadedFile.FileName}";
            using (var stream = new FileStream(targetFileName, FileMode.Create))
            {
                await UploadedFile.CopyToAsync(stream);
            }

            return Validator.IsValidGTFS(targetFileName);
        }
    }
}
