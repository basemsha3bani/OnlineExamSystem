using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace DataRepository.GateWay
{
    // All gateways in a request share the injected, scoped DbContext.
    // Writes are staged until the coordinating operation calls SaveChanges.
    public class ContextGateway<T> where T : class
    {
        private readonly DbConext context;
        public ContextGateway(DbConext context) { this.context = context; }
        public IQueryable<T> Query => context.Set<T>();
        public void Add(T entity) => context.Set<T>().Add(entity);
        public void Delete(T entity) => context.Set<T>().Remove(entity);
        public int SaveChanges() => context.SaveChanges();
        public IDbContextTransaction BeginTransaction() => context.Database.BeginTransaction();
        public T GetById(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes)
            => Include(Query, includes).FirstOrDefault(predicate);
        public List<T> List(Expression<Func<T, bool>> predicate = null, params Expression<Func<T, object>>[] includes)
        {
            var query = Include(Query.AsNoTracking(), includes);
            return (predicate == null ? query : query.Where(predicate)).ToList();
        }
        private IQueryable<T> Include(IQueryable<T> query, Expression<Func<T, object>>[] includes)
        {
            foreach (var include in includes ?? Array.Empty<Expression<Func<T, object>>>())
                query = query.Include(include);
            return query;
        }
    }
}
