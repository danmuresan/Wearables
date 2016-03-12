using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace GarminSensorApi.Utilities
{
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        private readonly IUnitOfWork m_unitOfWork;

        public Repository() : this(new UnitOfWork())
        {
        }

        public Repository(IUnitOfWork unitOfWork)
        {
            m_unitOfWork = unitOfWork ?? new UnitOfWork();
        }

        public IEnumerable<TEntity> GetAll()
        {
            try
            {
                return m_unitOfWork.Context.Set<TEntity>().ToList();
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
                return m_unitOfWork.Context.Set<TEntity>().Find(id);
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
                return m_unitOfWork.Context.Set<TEntity>().Find(entity);
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
                m_unitOfWork.Context.Set<TEntity>().Add(entity);
                m_unitOfWork.SaveChanges();
                return true;
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
                m_unitOfWork.Context.Set<TEntity>().Attach(entity);
                m_unitOfWork.Context.Entry(entity).State = EntityState.Modified;
                m_unitOfWork.SaveChanges();
                return true;
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
                var entityToRemove = Get(entity);
                m_unitOfWork.Context.Set<TEntity>().Remove(entityToRemove);
                m_unitOfWork.SaveChanges();
                return true;
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
                var entityToRemove = GetById(id);
                m_unitOfWork.Context.Set<TEntity>().Remove(entityToRemove);
                m_unitOfWork.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
                return false;
            }
        }

    }
}
