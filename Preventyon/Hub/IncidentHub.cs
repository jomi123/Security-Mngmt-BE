using Microsoft.AspNetCore.SignalR;
using Preventyon.Models;
using Preventyon.Models.DTO.Incidents;
using System.Threading.Tasks;

 

namespace Preventyon.Hubs
{
    public class IncidentHub : Hub
    {
        public async Task SendIncidentUpdate()
        {
            await Clients.All.SendAsync("ReceiveIncidentUpdate");
        }
        public async Task SendAdminUpdate()
        {
            await Clients.All.SendAsync("ReceiveAdminUpdate");
        }
    }
}