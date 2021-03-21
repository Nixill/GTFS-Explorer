// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your Javascript code.

var connection = new signalR.HubConnectionBuilder().withUrl("/eventsHub").build();

async function start() {
	try {
		await connection.start();
		console.log("SignalR Connected.");
	} catch (err) {
		console.log(err);
		setTimeout(start, 5000);
	}
};

connection.onclose(start);
start();

connection.on("select-new-file-response", (response) => {
	//0 = Yes
	if (response == 0) {
		window.location.href = "/Index";
	}
});

connection.on("loading-file", () => {
	$('#loader').css('display', 'block');
	$('#fader').css('display', 'block');
	$('#loader-wrapper').css('display', 'block');
});

(function () {
	const { ipcRenderer } = require("electron");

	document.getElementById("selectFile").addEventListener("click", (e) => {
		//To prevent open dialog to open since we'll be using Electron's
		e.preventDefault();

		//Send event to Main Ipc
		ipcRenderer.send("select-new-gtfs-file");
	});
}());