using System;
using System.Data.Entity;
using GarminSensorApi.Db;

namespace GarminSensorApi.Utilities
{
    public class UnitOfWork : IUnitOfWork
    {
        public DbContext Context { get; private set; }

        public UnitOfWork()
        {
            Context = new SensorDataContext();
        }

        public void Dispose()
        {
            Context.Dispose();
        }

        public void SaveChanges()
        {
            Context.SaveChanges();
        }
    }
}
