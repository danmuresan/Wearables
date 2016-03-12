using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace GarminSensorApi.Utilities
{
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        public IEnumerable<TEntity> GetAll()
        {
            try
            {
                using (var unitOfWork = new UnitOfWork())
                {
                    return unitOfWork.Context.Set<TEntity>().ToList();                    
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
                return Enumerable.Empty<TEntity>();
            }
        }

        public TEntity GetById(int id)
        {
            try
            {
                using (var unitOfWork = new UnitOfWork())
                {
                    return unitOfWork.Context.Set<TEntity>().Find(id);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
                return null;
            }
        }

        public TEntity Get(TEntity entity)
        {
            try
            {
                using (var unitOfWork = new UnitOfWork())
                {
                    return unitOfWork.Context.Set<TEntity>().Find(entity);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
                return null;
            }
        }

        public bool Add(TEntity entity)
        {
            try
            {
                using (var unitOfWork = new UnitOfWork())
                {
                    unitOfWork.Context.Set<TEntity>().Add(entity);
                    unitOfWork.SaveChanges();
                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
                return false;
            }
        }

        public bool Update(TEntity entity)
        {
            try
            {
                using (var unitOfWork = new UnitOfWork())
                {
                    unitOfWork.Context.Set<TEntity>().Attach(entity);
                    unitOfWork.Context.Entry(entity).State = EntityState.Modified;
                    unitOfWork.SaveChanges();
                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
                return false;
            }
        }

        public bool Delete(TEntity entity)
        {
            try
            {
                using (var unitOfWork = new UnitOfWork())
                {
                    var entityToRemove = Get(entity);
                    unitOfWork.Context.Set<TEntity>().Remove(entityToRemove);
                    unitOfWork.SaveChanges();
                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
                return false;
            }
        }

        public bool DeleteById(int id)
        {
            try
            {
                using (var unitOfWork = new UnitOfWork())
                {
                    var entityToRemove = GetById(id);
                    unitOfWork.Context.Set<TEntity>().Remove(entityToRemove);
                    unitOfWork.SaveChanges();
                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
                return false;
            }
        }

    }
}
