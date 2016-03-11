using System.Collections.Generic;

namespace GarminSensorApi.Utilities
{
    public interface IRepository<TEntity> where TEntity : class 
    {
        IEnumerable<TEntity> GetAll();

        TEntity GetById(int id);

        bool Add(TEntity entity);

        bool Update(TEntity entity);

        bool Delete(TEntity entity);

        bool DeleteById(int id);
    }
}