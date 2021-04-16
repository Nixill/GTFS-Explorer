using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace GTFS_Explorer.BackEnd.SignalR
{
    public class EventsHub : Hub
    {
        public async Task SendUserSelectNewFileResponse(string response)
        {
            await Clients.All.SendAsync("select-new-file-response", response);
        }
    }
}