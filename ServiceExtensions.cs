using HotelListing.Data;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
    }
}
