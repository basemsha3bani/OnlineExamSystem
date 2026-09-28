using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace DataRepository.GateWay
{
    internal class ContextGateway
    {
        internal static DbConext dbConext;
        internal static IDbContextTransaction _transaction;
        public static void CreateDatabaseTransaction()
        {
            GetContextInstance();
            _transaction = dbConext.Database.BeginTransaction();
        }

        internal static void GetContextInstance()
        {
            if (dbConext == null)
            {
                dbConext = new DbConext();
            }
            //return dbConext;
        }

        public static void Rollback()
        {
            _transaction.Rollback();
        }

        public static void Dispose()
        {
            _transaction.Dispose();
        }

        public static void Commit()
        {
            _transaction.Commit();
        }
    }
   internal class ContextGateway<TModelRepository>: ContextGateway where TModelRepository : class
    {
       

      

        private ContextGateway() { }

        internal static void Add(IRepository repository) 
        {
            dbConext.Entry(repository).State = EntityState.Added;

            dbConext.SaveChanges();
        }
        internal static void Add(IEnumerable<IRepository> repository)
        {
            dbConext.AddRange(repository);

            dbConext.SaveChanges();
        }
        internal static void Edit(IRepository repository)
        {

            dbConext.Entry(repository).State = EntityState.Modified;

            dbConext.SaveChanges();



        }

        internal static void Edit(IEnumerable<IRepository> repository)
        {

            dbConext.UpdateRange(repository);

            dbConext.SaveChanges();



        }


        internal static void Edit(IRepository repository, IRepository withnewvalues)
        {
            dbConext.Entry(repository).State = EntityState.Detached;

            dbConext.Entry(withnewvalues).State = EntityState.Modified;
            dbConext.SaveChanges();

        }

        internal static void Delete(IRepository repository)
        {
            dbConext.Entry(repository).State = EntityState.Deleted;

            dbConext.SaveChanges();
        }

        // Helper: verify the include expression is a simple member access (or boxed member)
        private static bool IsMemberAccess(Expression expression)
        {
            if (expression == null) return false;

            // Accept MemberExpression or UnaryExpression (boxing) whose operand is MemberExpression
            if (expression is MemberExpression) return true;

            if (expression is UnaryExpression unary && unary.Operand is MemberExpression) return true;

            return false;
        }

        private static IQueryable<TModelRepository> ApplyIncludes(IQueryable<TModelRepository> query, params Expression<Func<TModelRepository, object>>[] includeProperties)
        {
            if (includeProperties == null || includeProperties.Length == 0) return query;

            foreach (var include in includeProperties)
            {
                if (include == null) continue;

                if (!IsMemberAccess(include.Body))
                    throw new InvalidOperationException($"Invalid Include expression: '{include}'. Include must be a property access (e.g. 't => t.Navigation'). Projections (new {{ ... }}) are not supported.");

                query = query.Include(include);
            }

            return query;
        }

        internal static TModelRepository GetById(Expression<Func<TModelRepository, bool>> predicate, params Expression<Func<TModelRepository, object>>[] includeProperties)
        {
            IQueryable<TModelRepository> query = dbConext.Set<TModelRepository>().AsNoTracking();

            if (predicate != null)
                query = query.Where(predicate);

            query = ApplyIncludes(query, includeProperties);

            return query.FirstOrDefault();
        }

        internal static List<TModelRepository> List(Expression<Func<TModelRepository, bool>> predicate = null, params Expression<Func<TModelRepository, object>>[] includeProperties)   
        {
            IQueryable<TModelRepository> query = dbConext.Set<TModelRepository>().AsNoTracking();

            if (predicate != null)
                query = query.Where(predicate);

            query = ApplyIncludes(query, includeProperties);

            return query.ToList();
        }
      



      
    }
}
