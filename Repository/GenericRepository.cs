using HotelListing.Data;
using HotelListing.IRepository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace HotelListing.Repository
{
    // Inherits from IGenericRepository
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly DatabaseContext _context;
        private readonly DbSet<T> _db; 

        public GenericRepository(DatabaseContext context)
        {
            _context = context;
            _db = _context.Set<T>(); // give me a set of whatever T is
        }

        public async Task Delete(int id)
        {
            var entity = await _db.FindAsync(id);
            _db.Remove(entity);
        }

        public void DeleteRange(IEnumerable<T> entities)
        {
            _db.RemoveRange(entities);
        }

        public async Task<T> Get(Expression<Func<T, bool>> expression, List<string> includes = null)
        {
            // Get all the record in the table
            IQueryable<T> query = _db;
            // For each ForeignKey in includes, get the record corresponding - sql join
            // ex: Country in includes => gets hotel and its corresponding country columns
            if (includes != null)
            {
                foreach (var includeProperty in includes)
                {
                    query = query.Include(includeProperty);
                }
            }
            // AsNoTracking = don't modify the record in the db
            return await query.AsNoTracking().FirstOrDefaultAsync(expression);
        }

        public async Task<IList<T>> GetAll(Expression<Func<T, bool>> expression = null, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, List<string> includes = null)
        {
            // Get all the record in the table
            IQueryable<T> query = _db;
            
            // Ex: "WHERE countryId = 2"
            if (expression != null)
            {
                query = query.Where(expression);
            }
            
            // For each ForeignKey in includes, get the record corresponding - sql join
            // ex: Country in includes => gets hotel and its corresponding country columns
            if (includes != null)
            {
                foreach (var includeProperty in includes)
                {
                    query = query.Include(includeProperty);
                }
            }

            // SQL Order by
            if (orderBy != null)
            {
                query = orderBy(query);
            }

            // AsNoTracking = don't modify the record in the db
            return await query.AsNoTracking().ToListAsync();
        }

        public async Task Insert(T entity)
        {
            await _db.AddAsync(entity);
        }

        public async Task InsertRange(IEnumerable<T> entities)
        {
            await _db.AddRangeAsync(entities);
        }

        public void Update(T entity)
        {
            // 2 step process, check if record already exists in the db and is different then update
            _db.Attach(entity);
            _context.Entry(entity).State = EntityState.Modified;
        }
    }
}
