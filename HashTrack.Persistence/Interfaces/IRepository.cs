using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace HashTrack.Persistence.Interfaces
{
    public interface IRepository<T> where T : class
    {
        IEnumerable<T> Get(Expression<Func<T, bool>> predicate);
        T GetSingle(Expression<Func<T, bool>> predicate);
        IEnumerable<T> GetAll();
        T GetById(int id);
        void Insert(T entity);
        void Upsert(T entity, Func<T, bool> predicate);
        void Delete(T entity);
        void Save();
    }
}