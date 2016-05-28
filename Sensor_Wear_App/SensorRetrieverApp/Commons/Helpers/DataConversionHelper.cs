using System.Collections.Generic;
using Commons.Models;
using System.Linq;

namespace Commons.Helpers
{
    public static class DataConversionHelper
    {
        public static string ToCsv<T>(this IEnumerable<T> data) where T : IDataModel
        {
            return string.Join(",", data);
        }

        public static string[] ToCsv(this AccelerationBatch accBatch)
        {
            var xAccs = accBatch.Accelerations.Select(a => a.X).ToList();
            var yAccs = accBatch.Accelerations.Select(a => a.Y).ToList();
            var zAccs = accBatch.Accelerations.Select(a => a.Z).ToList();

            string[] csvAccs = new string[3];
            csvAccs[0] = string.Join(",", xAccs);
            csvAccs[1] = string.Join(",", yAccs);
            csvAccs[2] = string.Join(",", zAccs);

            return csvAccs;
        }
    }
}