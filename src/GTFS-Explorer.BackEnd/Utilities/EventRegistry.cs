using ElectronNET.API;
using ElectronNET.API.Entities;
using GTFS_Explorer.BackEnd.Extensions;
using GTFS_Explorer.BackEnd.SignalR;
using GTFS_Explorer.Core.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace GTFS_Explorer.BackEnd.Utilities
{
	public class EventRegistry : IEventRegistry
	{
		private readonly IWebHostEnvironment _env;
		private readonly IHubContext<EventsHub> _hubContext;

		public EventRegistry(
			IWebHostEnvironment env, 
			IHubContext<EventsHub> hubContext)
		{
			_env = env;
			_hubContext = hubContext;
		}

		/// <summary>
		/// Registers all custom electron events we define in this class
		/// </summary>
		public void RegisterEvents()
		{
			BeforeQuit();
			SetOnClickSelectNewFile();
		}

		private void BeforeQuit()
		{
			Electron.App.BeforeQuit += (x) =>
			{
				Electron.App.DeleteGTFSFileDir(_env);
				return Task.CompletedTask;
			};
		}

		private void SetOnClickSelectNewFile()
		{
			Electron.IpcMain.On("select-new-gtfs-file", async (args) =>
			{
				var result = await Electron.Dialog.ShowMessageBoxAsync(new MessageBoxOptions(
					"This will redirect you to the main page. You will loose access to the current GTFS file " +
					"unless you load it again, would you like to proceed?")
				{
					Type = MessageBoxType.warning,
					Title = "Open New GTFS File",
					Buttons = new string[] { "Yes", "No" }
				});

				//result.Response: 0 = Yes, 1 = No

				//Send this response to client using SignalR
				//in order to redirect there

				await _hubContext.Clients.All.SendAsync(
					"select-new-file-response", result.Response);
			});
		}

		//Old
		//private void SetOnClickSelectNewFile(IWebHostEnvironment env)
		//{
		//	Electron.IpcMain.On("select-new-gtfs-file", async (args) =>
		//	{
		//		BrowserWindow mainWindow = Electron.WindowManager.BrowserWindows.First();

		//		var filesSelected = await Electron.Dialog.ShowOpenDialogAsync(mainWindow, new OpenDialogOptions
		//		{
		//			Title = "Open GTFS File",
		//			DefaultPath = await Electron.App.GetPathAsync(PathName.Downloads),
		//			Filters = new FileFilter[]
		//			{
		//				new FileFilter { Name = "GTFS", Extensions = new string[] { "gtfs", "zip", "rar" } }
		//			}
		//		});

		//		var isValidFile = Validator.IsValidGTFS(filesSelected[0]);

		//		if (!isValidFile.Item1)
		//		{
		//			await Electron.Dialog.ShowMessageBoxAsync(isValidFile.Item2);
		//			return;
		//		}

		//		Electron.App.DeleteGTFSFileDir(env);
		//		var pathToSave = Electron.App.GetGTFSFilePath(env, Path.GetFileName(filesSelected[0]));

		//		File.Copy(filesSelected[0], pathToSave);

		//		//TODO: Figure our something else to redirect...
		//		//Electron.IpcMain.Send(mainWindow, "redirect-to-selection", null);
		//	});
		//}
	}
}