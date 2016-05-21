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
        public DbSet<HeartRateBatch> HeartRateBatches { get; set; }

        public SensorDataContext() : base(Constants.DbConnectionName)
        {
            Database.SetInitializer(new DropCreateDatabaseIfModelChanges<SensorDataContext>());
            Configuration.ProxyCreationEnabled = false;
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Acceleration>().HasRequired(a => a.Batch)
                .WithMany(x => x.Accelerations)
                .HasForeignKey(a => a.Batch);
        }
    }
}