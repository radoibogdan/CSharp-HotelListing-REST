using HotelListing.Configurations.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// Bridge between our classes and our actual database
namespace HotelListing.Data
{
    // IdentityDbContext - take advantage of Identity services, install Identity.EntityFrameworkCore
    // By default uses IdentityUser, but we have ApiUser
    public class DatabaseContext : IdentityDbContext<ApiUser>
    {
        // Define Constructor
        public DatabaseContext(DbContextOptions options) : base(options)
        {}

        // SeedData (ch 11) FakeData
        // List out what the db should know about when it is generated
        // Countries = the name that the db will use
        public DbSet<Country> Countries { get; set; }
        public DbSet<Hotel> Hotels { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            // IdentityDbContext = base
            base.OnModelCreating(builder);

            // Fake data for Hotel
            builder.ApplyConfiguration(new HotelConfiguration());
            // Fake data for Country
            builder.ApplyConfiguration(new CountryConfiguration());
            // Role Configuration
            builder.ApplyConfiguration(new RoleConfiguration());
        }
    }
}
