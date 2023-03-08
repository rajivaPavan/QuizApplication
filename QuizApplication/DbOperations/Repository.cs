using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using QuizApplication.DbContext;

namespace QuizApplication.DbOperations
{
    public interface IRepository<T> where T : class
    {
        Task<T> GetByIdAsync(int id);
        /// Returns a single entity by a given predicate.
        Task<T> GetAsync(Expression<Func<T, bool>> predicate, IEnumerable<string> includes = null);

        /// Returns a multiple entities by a given predicate.
        Task<List<T>> FindAsync(Expression<Func<T, bool>> predicate, IEnumerable<string> includes = null);

        /// Retrieves all entities in the repository.
        Task<List<T>> GetAllAsync(IEnumerable<string> includes = null);

        ///Retrieves all entities in the repository based on the predicate.
        Task<List<T>> GetAllAsync(Expression<Func<T, bool>> predicate, IEnumerable<string> includes = null);
        
        Task<T> AddAsync(T entity);
        Task<T> UpdateAsync(T entity);
        Task DeleteAsync(T entity);
    }
    
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly AppDbContext _context;
        private readonly DbSet<T> _entities;
        private readonly IQueryable<T> _queryable;

        public Repository(AppDbContext context)
        {
            _context = context;
            _entities = _context.Set<T>();
            _queryable = _entities.AsQueryable();
            _queryable = BuildQueryable(_queryable, GetComplexProperties().Select(p => p.Name));
        }

        public async Task<T> GetByIdAsync(int id)
        {
            return await _entities.FindAsync(id);
        }
        
        public async Task<T> GetAsync(Expression<Func<T, bool>> predicate, IEnumerable<string> includes = null)
        {
            
            return await _queryable.SingleOrDefaultAsync(predicate);
        }

        public async Task<List<T>> FindAsync(Expression<Func<T, bool>> predicate, IEnumerable<string> includes = null)
        {
            return await _queryable.Where(predicate).ToListAsync();
        }

        public async Task<List<T>> GetAllAsync(IEnumerable<string> includes = null)
        {
            return await _queryable.ToListAsync();
        }

        public async Task<List<T>> GetAllAsync(Expression<Func<T, bool>> predicate, IEnumerable<string> includes = null)
        {
            return await _queryable.Where(predicate).ToListAsync();
        }

        public async Task<T> AddAsync(T entity)
        {
            await _entities.AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<T> UpdateAsync(T entity)
        {
            _entities.Update(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task DeleteAsync(T entity)
        {
            _entities.Remove(entity);
            await _context.SaveChangesAsync();
        }
        
        public static IQueryable<T> BuildQueryable(IQueryable<T> queryable, IEnumerable<string> includes)
        {
            return includes == null
                ? queryable
                : includes.Aggregate(queryable, (current, include) => current.Include(include));
        }

        public static IEnumerable<PropertyInfo> GetComplexProperties()
        {
            // Get all the properties of T
            var properties = typeof(T).GetProperties();

            return (from property in properties
                let propertyType = property.PropertyType
                where propertyType.IsClass && propertyType != typeof(string)
                select property).ToList();
        }
    }

}