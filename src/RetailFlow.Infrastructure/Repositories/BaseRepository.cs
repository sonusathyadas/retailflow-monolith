using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using RetailFlow.Domain.Interfaces;
using RetailFlow.Infrastructure.Data;

namespace RetailFlow.Infrastructure.Repositories
{
    /// <summary>
    /// Generic EF6 repository implementation.
    /// </summary>
    public class BaseRepository<T> : IRepository<T> where T : class
    {
        protected readonly RetailFlowDbContext _context;
        protected readonly DbSet<T> _dbSet;

        public BaseRepository(RetailFlowDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _dbSet = context.Set<T>();
        }

        public T GetById(int id)
        {
            return _dbSet.Find(id);
        }

        public IEnumerable<T> GetAll()
        {
            return _dbSet.ToList();
        }

        public IEnumerable<T> Find(Expression<Func<T, bool>> predicate)
        {
            return _dbSet.Where(predicate).ToList();
        }

        public void Add(T entity)
        {
            _dbSet.Add(entity);
        }

        public void Update(T entity)
        {
            _context.Entry(entity).State = EntityState.Modified;
        }

        public void Remove(T entity)
        {
            _dbSet.Remove(entity);
        }

        public void SaveChanges()
        {
            _context.SaveChanges();
        }
    }
}
