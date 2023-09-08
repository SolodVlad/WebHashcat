using Domain.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Timers;
using Timer = System.Timers.Timer;

namespace WebHashcat.Hubs
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class BalanceHub : Hub
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly UserManager<User> _userManager;
        private User user;

        private readonly double _defCost = 0.1;
        private readonly Timer _timer;

        public BalanceHub(UserManager<User> userManager, IServiceProvider serviceProvider)
        {
            _userManager = userManager;
            _timer = new Timer(1000);
            _timer.Elapsed += PaymentWithdrawalTimer;
            _timer.AutoReset = true;
            _serviceProvider = serviceProvider;
        }

        public async Task StartPaymentWithdrawal()
        {
            user = await _userManager.FindByNameAsync(Context.UserIdentifier);

            _timer.Start();
        }

        public void StopPaymentWithdrawal()
        {
            _timer.Stop();
            _timer.Dispose();
        }

        private async void PaymentWithdrawalTimer(object sender, ElapsedEventArgs e)
        {
            user.Balance -= _defCost;
            await _userManager.UpdateAsync(user);
            
            await Clients.User(Context.UserIdentifier).SendAsync("paymentWithdrawal", user.Balance);
        }
    }
}
