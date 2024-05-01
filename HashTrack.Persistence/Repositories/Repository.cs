using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using HashTrack.Core.Attributes;
using HashTrack.Core.Enums;
using HashTrack.Persistence.Interfaces;

namespace HashTrack.Persistence.Repositories
{
    [RegisterService(LifeCycle.Transient, typeof(IRepository<>), true)]
    public class Repository<T> : IRepository<T> where T : class
    {
        protected readonly DbContext _context;
        protected readonly DbSet<T> _dbSet;

        public Repository(DbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        public IEnumerable<T> Get(Expression<Func<T, bool>> predicate)
        {
            return _dbSet.AsNoTracking().Where(predicate);
        }

        public T GetSingle(Expression<Func<T, bool>> predicate)
        {
            return _dbSet.AsNoTracking().FirstOrDefault(predicate);
        }

        public IEnumerable<T> GetAll()
        {
            return _dbSet.AsNoTracking().ToList();
        }

        public T GetById(int id)
        {
            return _dbSet.Find(id);
        }

        public void Insert(T entity)
        {
            _dbSet.Add(entity);
        }

        public void Upsert(T entity, Func<T, bool> predicate)
        {
            var existingEntity = _dbSet.Local.FirstOrDefault(predicate) ?? _dbSet.FirstOrDefault(predicate);
            if (existingEntity != null)
                // The entity already exists in the context, or we load it from the database
                _context.Entry(existingEntity).CurrentValues.SetValues(entity);
            else
                // Entity is not tracked, so attach and set as modified
                _dbSet.Add(entity);
        }

        public void Delete(T entity)
        {
            if (_context.Entry(entity).State == EntityState.Detached) _dbSet.Attach(entity);
            _dbSet.Remove(entity);
        }

        public void Save()
        {
            _context.SaveChanges();
        }
    }
}