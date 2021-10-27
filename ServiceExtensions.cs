using AspNetCoreRateLimit;
using HotelListing.Data;
using HotelListing.Models;
using Marvin.Cache.Headers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HotelListing
{
    public static class ServiceExtensions
    {
        public static void ConfigureIdentity(this IServiceCollection services)
        {
            // Return IdentityBuilder
            // Handle user interaction (password min length etc.)
            var builder = services.AddIdentityCore<ApiUser>(q => q.User.RequireUniqueEmail = true);
            // Creates new IdentityBuilder based on builder
            builder = new IdentityBuilder(builder.UserType, typeof(IdentityRole), services);
            // Which db it needs to interact with, for the Idetity Services to append = DbContext
            builder.AddEntityFrameworkStores<DatabaseContext>().AddDefaultTokenProviders();
        }

        // Iconfiguration gives access to appsettings.json file
        public static void ConfigureJWT(this IServiceCollection services, IConfiguration configuration)
        {
            var jwtSettings = configuration.GetSection("Jwt");
            var key = Environment.GetEnvironmentVariable("KEY");
            services
                .AddAuthentication(o => {
                    // Default config, when someone tries to auth check for bearer token
                    o.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    o.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(o => {
                    o.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = false,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = jwtSettings.GetSection("Issuer").Value,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key))
                    };
                });
        }

        // Exception Handler
        public static void ConfigureExceptionHandler(this IApplicationBuilder app)
        {
            // overide default exception handler
            app.UseExceptionHandler(error => {
                // context = controller that is passing down information
                error.Run(async context => {
                    context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                    context.Response.ContentType = "application/json";
                    var contextFeature = context.Features.Get<IExceptionHandlerFeature>();
                    if (contextFeature != null)
                    {
                        Log.Error($"Something went wrong in the {contextFeature.Error}");
                        // Error = Created in Models/Error
                        await context.Response.WriteAsync(new Error
                        {
                            StatusCode = context.Response.StatusCode,
                            Message = "Internal server error. Please try again later."
                        }.ToString());
                    }
                });
            });
        }

        // Versioning - Microsoft.AspNetCore.Mvc.Versioning
        public static void ConfigureVersioning(this IServiceCollection services)
        {
            services.AddApiVersioning(opt =>
            {
                // Header in our response = This is the version you are using
                opt.ReportApiVersions = true;
                // Set Default value for version. If client doesn't specify the version he want => he will get v1.0
                opt.AssumeDefaultVersionWhenUnspecified = true;
                opt.DefaultApiVersion = new ApiVersion(1, 0); // v1.0
                // Client needs to specify in Request Headers the api-version
                opt.ApiVersionReader = new HeaderApiVersionReader("api-version");
            });
        }

        // Caching - using Marvin Cache Headers
        public static void ConfigureHttpCacheHeaders(this IServiceCollection services)
        {
            services.AddResponseCaching();
            services.AddHttpCacheHeaders(
                (expirationOpt) =>
                {
                    expirationOpt.MaxAge = 120;
                    expirationOpt.CacheLocation = CacheLocation.Private;
                },
                (validatateOpt) =>
                {
                    // VIP: Once the data changes the cache expires
                    validatateOpt.MustRevalidate = true;
                }
            );
        }

        //// Rate Limiting / Throttling 
        public static void ConfigureRateLimiting(this IServiceCollection services)
        {
            var rateLimitRules = new List<RateLimitRule>
            {
                // All endpoints => 1 hit in 5sec
               new RateLimitRule
                {
                    Endpoint = "*",
                    Limit= 1,
                    Period = "5s"
                }
            };
            services.Configure<IpRateLimitOptions>(opt =>
            {
                opt.GeneralRules = rateLimitRules;
            });
            services.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>();
            services.AddSingleton<IIpPolicyStore, MemoryCacheIpPolicyStore>();
            services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
        }
    }
}
