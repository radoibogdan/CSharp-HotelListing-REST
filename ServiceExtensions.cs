using HotelListing.Data;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Diagnostics;
using Serilog;
using HotelListing.Models;

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
    }
}
