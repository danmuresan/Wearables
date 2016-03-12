using System.Data.Entity;
using GarminSensorApi.Models.SensorModels;
using GarminSensorApi.Utilities;

namespace GarminSensorApi.Db
{
    public class SensorDataContext : DbContext
    {
        public DbSet<Acceleration> Accelerations { get; set; }
        public DbSet<AccelerationBatch> AccelerationBatches { get; set; }
        public DbSet<HeartRate> HeartRates { get; set; }

        public SensorDataContext() : base(Constants.DbConnectionName)
        {
            Database.SetInitializer(new DropCreateDatabaseIfModelChanges<SensorDataContext>());
        }
    }
}