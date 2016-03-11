using System;
using System.Data.Linq;

namespace GarminSensorApi.Utilities
{
    public interface IUnitOfWork : IDisposable
    {
        DataContext Context { get; }

        void SaveChanges();
    }
}
