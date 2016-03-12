using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GarminSensorApi.Models.SensorModels
{
    public class Acceleration : IDataTableModel
    {
        [Key]
        public long? Id { get; set; }
        public double? XAxisAcceleration { get; set; }
        public double? YAxisAcceleration { get; set; }
        public double? ZAxisAcceleration { get; set; }
    }

    public class AccelerationBatch : IDataTableModel
    {
        [Key]
        public long? Id { get; set; }
        public IList<Acceleration> AccelerationList { get; set; }
        public DateTime? TimeStamp { get; set; }
    }
}
