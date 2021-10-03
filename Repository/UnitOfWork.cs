using HotelListing.Controllers.Data;
using HotelListing.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HotelListing.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly DatabaseContext _context;
        private IGenericRepository<Country> _countries;
        private IGenericRepository<Hotel> _hotels;
        public UnitOfWork(DatabaseContext context)
        {
            _context = context;
        }
        // ??= if _countries is empty => new => else return _countries
        public IGenericRepository<Country> Countries => _countries ??= new GenericRepository<Country>(_context);

        public IGenericRepository<Hotel> Hotels => _hotels ??= new GenericRepository<Hotel>(_context);

        // When operations are done free up the memory
        public void Dispose()
        {
            _context.Dispose(); // Kill the connection to the db
            GC.SuppressFinalize(this); // GarbageCollector
        }

        public async Task Save()
        {
            await _context.SaveChangesAsync();
        }
    }
}
