using Newtonsoft.Json;
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
        [Required]
        public double? XAxisAcceleration { get; set; }

        [JsonProperty("y")]
        [Required]
        public double? YAxisAcceleration { get; set; }

        [JsonProperty("z")]
        [Required]
        public double? ZAxisAcceleration { get; set; }

        [Required]
        public virtual AccelerationBatch Batch { get; set; }
    }
}
