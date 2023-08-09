﻿using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace WebHashcat.SignalR
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class CabinetHub : Hub
    {
        public async Task Send(string message)
        {
            var userName = Context.User.Identity.Name;
            await Clients.All.SendAsync("Receive", message);
        }
    }
}