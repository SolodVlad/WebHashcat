﻿using DLL.Context;
using Domain.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Renci.SshNet;
using System.Configuration;
using System.Diagnostics;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Timers;
using WebHashcat.Models;
using Timer = System.Timers.Timer;

namespace WebHashcat.Hubs
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class BalanceHub : Hub
    {
        private readonly UserManager<User> _userManager;
        private readonly IHubContext<BalanceHub> _hubContext;
        private readonly IConfiguration _configuration;

        private readonly decimal _defCost = 0.01M;
        private readonly Timer _timer;
        private string userName;

        public BalanceHub(IConfiguration configuration, HubContextAccessor hubContextAccessor)
        {
            _timer = new Timer(1000);
            _timer.Elapsed += PaymentWithdrawalTimer;
            _timer.AutoReset = true;

            _hubContext = hubContextAccessor.GetHubContext();
            _configuration = configuration;
        }

        public void StartPaymentWithdrawal()
        {
            userName = Context.UserIdentifier;
            _timer.Start();
        }

        public void StopPaymentWithdrawal()
        {
            _timer.Stop();
            _timer.Dispose();
        }

        private async void PaymentWithdrawalTimer(object sender, ElapsedEventArgs e)
        {
            var optionsBuilder = new DbContextOptionsBuilder<WebHashcatDbContext>();
            optionsBuilder.UseSqlServer(_configuration.GetValue<string>("DbConnection"));

            var userManager = new UserManager<User>(new UserStore<User>(new WebHashcatDbContext(optionsBuilder.Options)), null, null, null, null, null, null, null, null
                );

            var user = await userManager.FindByNameAsync(userName);
            user.Balance -= _defCost;
            
            userManager.UpdateAsync(user).Wait();

            await _hubContext.Clients.User(userName).SendAsync("paymentWithdrawal", user.Balance);

            if (user.Balance == 0) 
                StopPaymentWithdrawal();
        }
    }
}
