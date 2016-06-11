using Commons.Filters;
using Commons.Models;
using System;
using System.Linq;

namespace Commons.Helpers
{
    public static class DataFilteringHelper
    {
        public static AccelerationBatch FilterAccelerationBatch(this AccelerationBatch accBatch, FilterType type)
        {
            if (accBatch == null)
            {
                throw new ArgumentNullException(nameof(accBatch));
            }

            var accelerations = accBatch.Accelerations;
            var xAxisAccs = accelerations.Select(x => x.X).ToArray();
            var yAxisAccs = accelerations.Select(y => y.Y).ToArray();
            var zAxisAccs = accelerations.Select(z => z.Z).ToArray();

            var filter = FilterFactory.GetFilterByType(type);

            var filteredX = filter.ApplyFilter(xAxisAccs).ToList();
            var filteredY = filter.ApplyFilter(yAxisAccs).ToList();
            var filteredZ = filter.ApplyFilter(zAxisAccs).ToList();

            for (int i = 0; i < accelerations.Count; i++)
            {
                accelerations[i] = new Acceleration(filteredX[i], filteredY[i], filteredZ[i]);
            }

            return new AccelerationBatch
            {
                Accelerations = accelerations,
                TimeStamp = accBatch.TimeStamp
            };
        }
    }
}