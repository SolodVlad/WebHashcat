using Domain.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.VisualBasic;

namespace WebHashcat.Areas.Cabinet.Managers
{
    public class UserBalanceManager
    {
        private static UserBalanceManager _instance;
        private readonly UserManager<User> _userManager;
        private decimal _balance;

        private UserBalanceManager(UserManager<User> userManager)
        {
            _userManager = userManager;
            //_balance = GetBalanceFromDatabase(username);
        }

        public static UserBalanceManager GetInstance(UserManager<User> userManager) => _instance ??= new UserBalanceManager(userManager);

        public decimal GetBalance() => _balance;

        public async Task SetBalance(string username, decimal newBalance)
        {
            _balance = newBalance;
            await UpdateBalanceInDatabase(username, newBalance);
        }

        private async Task<decimal> GetBalanceFromDatabase(string username) => (await _userManager.FindByNameAsync(username)).Balance;

        private async Task UpdateBalanceInDatabase(string username, decimal newBalance)
        {
            var user = await _userManager.FindByNameAsync(username);
            user.Balance = newBalance;
            await _userManager.UpdateAsync(user);
        }
    }

}
