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

            builder.Entity<Country>().HasData(
                new Country
                {
                    Id = 1,
                    Name = "Jamaica",
                    ShortName = "JM"
                },
                new Country
                {
                    Id = 2,
                    Name = "France",
                    ShortName = "FR"
                },
                new Country
                {
                    Id = 3,
                    Name = "Romania",
                    ShortName = "RO"
                }
            );
            builder.Entity<Hotel>().HasData(
                new Hotel
                {
                    Id = 1,
                    Name = "Sandals Resort and Spa",
                    Address = "Negril",
                    CountryId = 1,
                    Rating = 4.5
                },
                new Hotel
                {
                    Id = 2,
                    Name = "Ibis",
                    Address = "Dani",
                    CountryId = 2,
                    Rating = 4.2
                },
                new Hotel
                {
                    Id = 3,
                    Name = "Grand Paladium",
                    Address = "George Town",
                    CountryId = 3,
                    Rating = 3.7
                }
            );
        }
    }
}
