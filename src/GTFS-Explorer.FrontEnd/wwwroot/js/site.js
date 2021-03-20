// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your Javascript code.

(function () {
	const { ipcRenderer } = require("electron");

	document.getElementById("selectfile").addEventListener("click", (e) => {
		//To prevent open dialog to open since we'll be using Electron's
		e.preventDefault();

		//Send event to Main Ipc
		ipcRenderer.send("select-new-gtfs-file");
	});

	//Listen event from Electron.IpcMain
	//ipcRenderer.on("redirect-to-selection", (event, arg) => {
	//	window.location.href = "/MainPages/Selection";
	//});
}());