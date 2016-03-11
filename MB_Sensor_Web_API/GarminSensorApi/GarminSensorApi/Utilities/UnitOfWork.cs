using System.Data.Linq;

namespace GarminSensorApi.Utilities
{
    public class UnitOfWork : IUnitOfWork
    {
        public DataContext Context { get; }

        public UnitOfWork()
        {
            // TODO: ...
        }

        public void Dispose()
        {
            throw new System.NotImplementedException();
        }

        public void SaveChanges()
        {
            throw new System.NotImplementedException();
        }
    }
}
