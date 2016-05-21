using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Commons.Models
{
    public class AccelerationBatch : IDataModel
    {
        [JsonProperty("Accelerations")]
        public IList<Acceleration> Accelerations { get; set; }

        [JsonProperty("TimeStamp")]
        public DateTime TimeStamp { get; set; }

        public AccelerationBatch()
        {
            Accelerations = new List<Acceleration>();
            TimeStamp = DateTime.Now;
        }
    }
}