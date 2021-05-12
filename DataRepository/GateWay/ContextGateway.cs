using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace DataRepository.GateWay
{
   internal class ContextGateway<TModelRepository> where TModelRepository : class
    {
        private static DbConext dbConext;

        internal static void GetContextInstance()
        {
            if (dbConext == null)
            {
                dbConext = new DbConext();
            }
           //return dbConext;
        }

        private ContextGateway() { }

        internal static void Add(IRepository repository) 
        {
            dbConext.Entry(repository).State = EntityState.Added;

            dbConext.SaveChanges();
        }
        internal static void Edit(IRepository repository)
        {

            dbConext.Entry(repository).State = EntityState.Modified;

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

        internal static TModelRepository GetById(Expression<Func<TModelRepository, bool>> predicate)
        {
            return dbConext.Set<TModelRepository>().Where(predicate).FirstOrDefault();


        }

        internal static List<TModelRepository> List(Expression<Func<TModelRepository, bool>> predicate = null, params Expression<Func<TModelRepository, object>>[] includeProperties)
        {

            if (predicate == null)
            {
                return (includeProperties.Aggregate
             (dbConext.Set<TModelRepository>(), (current, includeProperty) => (DbSet<TModelRepository>)current.Include(includeProperty)).ToList());
            }

            return (includeProperties.Aggregate
               (dbConext.Set<TModelRepository>().Where(predicate), (current, includeProperty) => current.Include(includeProperty)).ToList());
        }
        private static IDbContextTransaction _transaction;



        public static void CreateDatabaseTransaction()
        {
            GetContextInstance();
            _transaction = dbConext.Database.BeginTransaction();
        }



        public static void Rollback()
        {
            _transaction.Rollback();
        }

        public static  void Dispose()
        {
            _transaction.Dispose();
        }

        public static void Commit()
        {
            _transaction.Commit();
        }
    }
}
