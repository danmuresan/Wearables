using System;
using System.Data.Entity;

namespace GarminSensorApi.Utilities
{
    public interface IUnitOfWork : IDisposable
    {
        DbContext Context { get; }

        void SaveChanges();
    }
}
