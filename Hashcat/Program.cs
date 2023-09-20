using BLL.Infrastructure;
using BLL.Services;
using Domain.Models;
using WebHashcat.Configurations;
using Azure.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using WebHashcat.Areas.Cabinet.Services;
using WebHashcat.Areas.Cabinet.Hubs;

var builder = WebApplication.CreateBuilder(args);

var keyVaultEndpoint = new Uri(Environment.GetEnvironmentVariable("VaultUri"));
builder.Configuration.AddAzureKeyVault(keyVaultEndpoint, new DefaultAzureCredential());

// Add services to the container.
builder.Services.AddTransient<LookupTableService>();

builder.Services.AddControllersWithViews();

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.ConfigureJWT(builder.Configuration);
builder.Services.ConfigureSwagger();

builder.Services.Configure<SendGridEmailSenderOptions>(option =>
{
    option.ApiKey = builder.Configuration.GetValue<string>("SENDGRID-API-KEY");
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

builder.Services.AddDistributedRedisCache(option =>
{
    option.Configuration = builder.Configuration["CacheConnection"];
});

builder.Services.AddHttpContextAccessor();

builder.Services.AddSignalR().AddAzureSignalR();
builder.Services.Configure<HubOptions>(options =>
{
    options.ClientTimeoutInterval = TimeSpan.FromDays(1);
    options.MaximumParallelInvocationsPerClient = 10;
});

builder.Services.AddSingleton<HubContextAccessor>();

builder.Services.AddSingleton<IUserIdProvider, CustomUserIdProvider>();

builder.Services.AddApplicationInsightsTelemetry(builder.Configuration["APPLICATIONINSIGHTS_CONNECTION_STRING"]);

//builder.Services.AddSingleton<UserBalanceManager>();
builder.Services.AddSingleton<ShellStreamService>();

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

app.UseFileServer();

app.MapHub<HashcatHub>("/hubs/hashcat");

app.MapHub<BalanceHub>("/hubs/balance");

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseCookiePolicy();
app.UseAuthorization();
app.UseAuthentication();

app.MapControllers();
app.MapRazorPages();

app.MapControllerRoute("areas", "{area:exists}/{controller=Home}/{action=Index}");

app.MapControllerRoute("default", "{controller=Home}/{action=Index}");

app.Run();