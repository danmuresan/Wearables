using Newtonsoft.Json;
using System.Collections.Generic;

namespace GarminSensorApi.Models.SensorModels
{
    public class AccelerationDataBatch
    {
        public AccelerationDataBatch()
        {
        }

        [JsonProperty("batch")]
        public List<Acceleration> Accelerations { get; set; }
    }
}