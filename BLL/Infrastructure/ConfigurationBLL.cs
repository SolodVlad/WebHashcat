using BLL.Services;
using DLL.Context;
using DLL.Repository;
using Domain.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BLL.Infrastructure
{
    public static class ConfigurationBll
    {
        public static void Configure(this IServiceCollection serviceCollection, IConfiguration configuration)
        {
            serviceCollection.AddDbContext<HashWorkDbContext>(opt => opt.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

            serviceCollection.AddTransient<LookupTableRepository>();
            serviceCollection.AddTransient<CurrencyRepository>();
            serviceCollection.AddTransient<UserRepository>();
            serviceCollection.AddTransient<IEmailSender, SendGridEmailService>();

            serviceCollection.AddIdentity<User, IdentityRole>(opt =>
            {
                opt.SignIn.RequireConfirmedEmail = true;
            }).AddEntityFrameworkStores<HashWorkDbContext>()
              .AddDefaultTokenProviders();
        }
    }
}
