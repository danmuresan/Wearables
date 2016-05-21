using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GarminSensorApi.Models.SensorModels
{
    public class AccelerationBatch : IDataTableModel
    {
        public AccelerationBatch()
        {
        }

        [Key]
        public long? Id { get; set; }

        [JsonProperty("batch")]
        [Required]
        //[ForeignKey("Id")]
        public virtual ICollection<Acceleration> Accelerations { get; set; }

        [Required]
        public DateTime? TimeStamp { get; set; }
    }
}