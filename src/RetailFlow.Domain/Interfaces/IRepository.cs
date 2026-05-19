using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace RetailFlow.Domain.Interfaces
{
    /// <summary>
    /// Generic repository interface for SQL-backed entities.
    /// </summary>
    public interface IRepository<T> where T : class
    {
        T GetById(int id);
        IEnumerable<T> GetAll();
        IEnumerable<T> Find(Expression<Func<T, bool>> predicate);
        void Add(T entity);
        void Update(T entity);
        void Remove(T entity);
        void SaveChanges();
    }
}
