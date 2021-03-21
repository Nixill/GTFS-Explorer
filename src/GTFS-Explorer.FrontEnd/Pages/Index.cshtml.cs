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
using System.Linq;
using ElectronNET.API.Entities;
using GTFS_Explorer.BackEnd.Readers;

namespace GTFS_Explorer.FrontEnd.Pages
{
	public class IndexModel : PageModel
	{
		private readonly ILogger<IndexModel> _logger;
		private readonly IWebHostEnvironment _environment;
		private readonly GTFSFeedReader _reader;

		public Tuple<bool, string> isValidFile { get; set; }
				= new Tuple<bool, string>(false, "");

		[BindProperty]
		public IFormFile UploadedFile { get; set; }

		public IndexModel(
			ILogger<IndexModel> logger, 
			IWebHostEnvironment env,
			GTFSFeedReader reader)
		{
			_logger = logger;
			_environment = env;
			_reader = reader;
		}

		public void OnGet()
		{
			Electron.App.DeleteGTFSFileDir(_environment);
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

			/* TODO here:
			 * Send message to render process using SignalR
			 * notifying that file is loading in order to 
			 * display a loading screen
			 */

			_reader.ReadFeed();
			return RedirectToPage("/MainPages/Selection");
		}
	}
}