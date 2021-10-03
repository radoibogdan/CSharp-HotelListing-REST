using HotelListing.Controllers.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HotelListing.IRepository
{
    // Register for every variation of the generic repository relative to the class T
    public interface IUnitOfWork : IDisposable
    {
        IGenericRepository<Country> Countries { get; }
        IGenericRepository<Hotel> Hotels { get; }
        // Save to db all the changes
        Task Save();
    }
}
