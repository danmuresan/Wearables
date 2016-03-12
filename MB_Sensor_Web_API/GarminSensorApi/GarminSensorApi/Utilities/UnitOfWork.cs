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
            Context.Database.CreateIfNotExists();
            Context.Database.Connection.Open();
        }

        public void Dispose()
        {
            Context.Database.Connection.Close();
            Context.Dispose();
        }

        public void SaveChanges()
        {
            Context.SaveChanges();
        }
    }
}
