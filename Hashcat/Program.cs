using BLL.Infrastructure;
using BLL.Services;
using Domain.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using WebHashcat.Configurations;
using WebHashcat.SignalR;
using System.Security.Claims;
using Azure.Identity;
using Microsoft.AspNetCore.Mvc;
//using SignalRAuthenticationSample.Data;
//using SignalRAuthenticationSample.Hubs;
//using SignalRAuthenticationSample;

var builder = WebApplication.CreateBuilder(args);

var keyVaultEndpoint = new Uri(Environment.GetEnvironmentVariable("VaultUri"));
builder.Configuration.AddAzureKeyVault(keyVaultEndpoint, new DefaultAzureCredential());

// Add services to the container.
builder.Services.AddTransient<CurrencyService>();
builder.Services.AddTransient<UserService>();
builder.Services.AddTransient<LookupTableService>();

builder.Services.AddControllersWithViews();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.ConfigureJWT(builder.Configuration);
builder.Services.ConfigureSwagger();
//builder.Services.AuthenticationCookie();

//builder.Services.Configure<CookieAuthenticationOptions>(options =>
//{
//    options.Cookie.Name = "AuthCookie";
//    options.Cookie.HttpOnly = true;
//    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
//    options.Cookie.SameSite = SameSiteMode.Strict;
//    options.ExpireTimeSpan = TimeSpan.FromDays(1);
//    options.SlidingExpiration = false;
//    options.LoginPath = "/Login";
//});

//builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddCookie(options =>
//{
//    options.Cookie.Name = "AuthCookie";
//    options.Cookie.HttpOnly = true;
//    options.Cookie.SecurePolicy = CookieSecurePolicy.None; //заменить на always
//    options.Cookie.SameSite = SameSiteMode.Strict;
//    options.ExpireTimeSpan = TimeSpan.FromDays(1);
//    options.SlidingExpiration = false;
//    options.LoginPath = "/Login";
//});

builder.Services.AddSignalR();
//builder.Services.AddAuthentication().AddIdentityServerJwt();
//builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<IPostConfigureOptions<JwtBearerOptions>, ConfigureJwtBearerOptions>());
builder.Services.AddSingleton<IUserIdProvider, CustomUserIdProvider>();

builder.Services.Configure<SendGridEmailSenderOptions>(option =>
{
    option.ApiKey = builder.Configuration["ExternalProviders:SendGrid:SENDGRID_API_KEY"];
    option.SenderName = builder.Configuration["ExternalProviders:SendGrid:SenderName"];
    option.SenderEmail = builder.Configuration["ExternalProviders:SendGrid:SenderEmail"];
});

ConfigurationBll.Configure(builder.Services, builder.Configuration);

builder.Services.AddRazorPages();

builder.Services.AddHttpClient();
builder.Services.AddMvc(options => { options.Filters.Add(new RequireHttpsAttribute()); });

builder.Services.AddHsts(options =>
{
    options.Preload = true;
    options.IncludeSubDomains = true;
    options.MaxAge = TimeSpan.FromDays(365);
});

builder.Services.AddAntiforgery(options => options.HeaderName = "X-XSRF-TOKEN");

//builder.Services.AddDistributedRedisCache(option =>
//{
//    option.Configuration = builder.Configuration["LocalRedisConnectionString"];
//});
builder.Services.AddDistributedRedisCache(option =>
{
    option.Configuration = builder.Configuration["CacheConnection"];
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.Use(async (context, next) =>
{
    context.Response.Headers.Add("X-Xss-Protection", "1; mode=block");
    context.Response.Headers.Add("X-Frame-Options", "SAMEORIGIN");
    await next();
});

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseCookiePolicy();
app.UseAuthorization();
app.UseAuthentication();

app.MapControllers();
app.MapRazorPages();

//app.UseCors(builder =>
//{
//    builder.WithOrigins("https://localhost:7149")
//        .AllowAnyHeader()
//        .WithMethods("GET", "POST")
//        .AllowCredentials();
//});

//app.MapHub<CabinetHub>("/Cabinet");

app.MapControllerRoute("areas", "{area:exists}/{controller=Home}/{action=Index}");

app.MapControllerRoute("default", "{controller=Home}/{action=Index}");

app.Run();
