using Newtonsoft.Json;
using System;

namespace Commons.Models
{
    public class Acceleration : IDataModel
    {
        [JsonProperty("X")]
        public double X { get; set; }

        [JsonProperty("Y")]
        public double Y { get; set; }

        [JsonProperty("Z")]
        public double Z { get; set; }

        public Acceleration(double x, double y, double z)
        {
            X = Math.Round(x, 3, MidpointRounding.AwayFromZero);
            Y = Math.Round(y, 3, MidpointRounding.AwayFromZero);
            Z = Math.Round(z, 3, MidpointRounding.AwayFromZero);
        }

        public override string ToString()
        {
            return $"{X}, {Y}, {Z}";
        }
    }
}