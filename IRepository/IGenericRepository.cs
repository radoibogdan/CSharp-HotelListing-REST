using HotelListing.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using X.PagedList;

namespace HotelListing.IRepository
{
    // T = prepared to take any type of class
    public interface IGenericRepository<T> where T : class
    {
        // Get All records (Task - Async programming)
        Task<IList<T>> GetAll(
            Expression<Func<T, bool>> expression = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
            List<string> includes = null
        );
        // Get All using params (pagenumber and pagesize) for paging
        Task<IPagedList<T>> GetPagedList(RequestParams requestParams, List<string> includes = null);
        // Get record
        Task<T> Get(Expression<Func<T, bool>> expression,List<string> includes = null);
        // Insert record
        Task Insert(T entity);
        // Insert multiple records
        Task InsertRange(IEnumerable<T> entities);
        // Delete record
        Task Delete(int id);
        // Delete multiple records
        void DeleteRange(IEnumerable<T> entities);
        // Update record
        void Update(T entity);
    }
}
