using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace DataRepository.GateWay
{
    public class RepositoryGateWay<TModelRepository> where TModelRepository : class
    {
        DbConext dbConext = new DbConext();



        internal void Add(IRepository repository)
        {
            dbConext.Entry(repository).State = EntityState.Added;

            dbConext.SaveChanges();
        }
        internal void Edit(IRepository repository)
        {

            dbConext.Entry(repository).State = EntityState.Modified;

            dbConext.SaveChanges();



        }



        internal void Edit(IRepository repository, IRepository withnewvalues)
        {
            dbConext.Entry(repository).State = EntityState.Detached;

            dbConext.Entry(withnewvalues).State = EntityState.Modified;
            dbConext.SaveChanges();

        }

        internal void Delete(IRepository repository)
        {
            dbConext.Entry(repository).State = EntityState.Deleted;

            dbConext.SaveChanges();
        }

        internal TModelRepository GetById(Expression<Func<TModelRepository, bool>> predicate)
        {
            return dbConext.Set<TModelRepository>().Where(predicate).FirstOrDefault();


        }

        internal List<TModelRepository> List(Expression<Func<TModelRepository, bool>> predicate = null, params Expression<Func<TModelRepository, object>>[] includeProperties)
        {
           
            if(predicate==null)
            {
                return (includeProperties.Aggregate
             (dbConext.Set<TModelRepository>(), (current, includeProperty) => (DbSet<TModelRepository>)current.Include(includeProperty)).ToList());
            }

            return (includeProperties.Aggregate
               (dbConext.Set<TModelRepository>().Where(predicate), (current, includeProperty) => current.Include(includeProperty)).ToList());
        }


       
       






    }

}
