using System;
using System.ComponentModel.DataAnnotations;

namespace GarminSensorApi.Models.SensorModels
{
    public class HeartRate
    {
        [Key]
        public long? Id { get; set; }
        public int HeartRateValue { get; set; }
        public DateTime? TimeStamp { get; set; }
    }
}
