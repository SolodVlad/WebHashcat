using WebHashcatAdminPanel.Configurations;
using Azure.Identity;
using Microsoft.AspNetCore.Mvc;
using BLL.Services;
using BLL.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

var keyVaultEndpoint = new Uri(Environment.GetEnvironmentVariable("VaultUri"));
builder.Configuration.AddAzureKeyVault(keyVaultEndpoint, new DefaultAzureCredential());

// Add services to the container.
builder.Services.AddTransient<LookupTableService>();
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddControllersWithViews();

builder.Services.ConfigureJWT(builder.Configuration);
builder.Services.ConfigureSwagger();

builder.Services.AddApplicationInsightsTelemetry(builder.Configuration["APPLICATIONINSIGHTS_CONNECTION_STRING"]);

builder.Services.AddDistributedRedisCache(option =>
{
    option.Configuration = builder.Configuration["CacheConnection"];
});

builder.Services.AddMvc(options => { options.Filters.Add(new RequireHttpsAttribute()); });

builder.Services.AddHsts(options =>
{
    options.Preload = true;
    options.IncludeSubDomains = true;
    options.MaxAge = TimeSpan.FromDays(365);
});

builder.Services.AddAntiforgery(options => options.HeaderName = "X-XSRF-TOKEN");

ConfigurationBll.Configure(builder.Services, builder.Configuration);
builder.Services.AddDistributedRedisCache(option =>
{
    option.Configuration = builder.Configuration["CacheConnection"];
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
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

app.UseAuthorization();
app.UseAuthentication();

app.MapControllerRoute("areas", "{area:exists}/{controller=Home}/{action=Index}");

app.MapControllerRoute("default", "{controller=Home}/{action=Index}");

//app.MapRazorPages();

app.Run();
