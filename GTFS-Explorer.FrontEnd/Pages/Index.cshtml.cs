using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;

namespace GTFS_Explorer.FrontEnd.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        public bool isValidFile { get; set; } = true;

        [BindProperty]
        public IFormFile UploadedFile { get; set; }

        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }

        public void OnPost()
        {
            isValidFile = ValidateFile();
        }

        protected bool IsValidExtension()
        {
            if (UploadedFile == null)
                return false;

            string FileExt = Path.GetExtension(UploadedFile.FileName);
            FileExt = FileExt.ToLower();
            return FileExt == ".gtfs" || FileExt == ".zip";
        }

        protected bool ValidateFile()
        {
            if (!IsValidExtension())
                return false;

            //Add inside files of zip validation here
            //Ex: contains all GTFS required files, etc..

            return true;
        }
    }
}
