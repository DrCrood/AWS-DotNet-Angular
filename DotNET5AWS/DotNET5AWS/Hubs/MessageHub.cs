using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;


namespace DotNET5AWS.Hubs
{
    public class MessageHub : Hub
    {
        public async Task SendMessage(string username, string message)
        {
            await Clients.Caller.SendAsync("messageReceived", username, message);
           // await Clients.All.SendAsync("messageReceived", username, message);
        }
    }
}
