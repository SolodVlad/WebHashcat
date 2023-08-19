using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

namespace WebHashcat.Configurations
{
    public static class ServiceCollectionConfiguration
    {
        //public static AuthenticationBuilder AddCookie(this AuthenticationBuilder builder, Action<CookieAuthenticationOptions> configureOptions)
        //{
        //    builder.Services.ConfigureApplicationCookie(options =>
        //    {
        //        options.Cookie.Name = "AuthCookie";
        //        options.Cookie.HttpOnly = true;
        //        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        //        options.Cookie.SameSite = SameSiteMode.Strict;
        //        options.ExpireTimeSpan = TimeSpan.FromDays(1);
        //        options.SlidingExpiration = false;
        //        options.LoginPath = "/Login";
        //    });
        //    return builder;
        //}

        public static void ConfigureJWT(this IServiceCollection services, IConfiguration configuration)
        {
            var jwtConfig = configuration.GetSection("JWT");
            var secretKey = configuration.GetValue<string>("JWTSecret");
            services.AddAuthentication(opt =>
            {
                opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtConfig["validIssuer"],
                    ValidAudience = jwtConfig["validAudience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
                };
                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        context.Token = context.Request.Cookies["AuthCookie"];
                        return Task.CompletedTask;
                    }
                };
            })
            .AddCookie(options =>
            {
                options.Cookie.Name = "AuthCookie";
                options.Cookie.HttpOnly = true;
                options.Cookie.SecurePolicy = CookieSecurePolicy.None; //заменить на always
                options.Cookie.SameSite = SameSiteMode.Strict;
                options.ExpireTimeSpan = TimeSpan.FromDays(1);
                options.SlidingExpiration = false;
                options.LoginPath = "/Login";
            });
        }

        public static void ConfigureSwagger(this IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Web Hashcat API",
                    Version = "v1",
                    Description = "Web Hashcat API Services.",
                    Contact = new OpenApiContact
                    {
                        Name = "Solodovnik Vladislav."
                    },
                });
                c.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "JWT Authorization header using the Bearer scheme."
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });
            });
        }
    }
}
