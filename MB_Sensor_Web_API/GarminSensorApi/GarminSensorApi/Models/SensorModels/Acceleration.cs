using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GarminSensorApi.Models.SensorModels
{
    public class Acceleration : IDataTableModel
    {
        public Acceleration()
        {
        }

        [Key]
        public long? Id { get; set; }

        [JsonProperty("x")]
        public double? XAxisAcceleration { get; set; }

        [JsonProperty("y")]
        public double? YAxisAcceleration { get; set; }

        [JsonProperty("z")]
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
