using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GarminSensorApi.Models.SensorModels
{
    public class HeartRate : IDataTableModel
    {
        [Key]
        public long? Id { get; set; }
        public int HeartRateValue { get; set; }
        public DateTime? TimeStamp { get; set; }
    }

    public class HeartRateBatch : IDataTableModel
    {
        [Key]
        public long? Id { get; set; }
        public IList<HeartRate> HeartRateValueList { get; set; }
        public DateTime? TimeStamp { get; set; }
    }
}
