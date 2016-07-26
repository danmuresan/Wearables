using Commons.Filters;
using Commons.Models;
using System;
using System.Collections.Generic;
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

        public static List<Acceleration> FlattenAccelerationBatches(this List<AccelerationBatch> accBatches)
        {
            List<Acceleration> accelerations = new List<Acceleration>();
            foreach (var accBatch in accBatches)
            {
                accelerations.AddRange(accBatch.Accelerations);
            }

            return accelerations;
        }

        /// <summary>
        /// First off, we filter out the accelerations with the window filter.
        /// We then begin the nulling out procedure - everything below an adaptive threshold of 20% is nulled
        /// consider the area of interest starting from the earliest non-nulled point up till the latest non-null point
        /// </summary>
        /// <param name="accelerations"></param>
        /// <returns></returns>
        public static IEnumerable<Acceleration> NormalizeAccelerationBatch(IEnumerable<Acceleration> accelerations)
        {
            var windowFiltered = DataOperationsUtil.ApplyWindowFilterOnAccelerationBatch(accelerations);
            var nullOutFilter = FilterFactory.GetFilterByType(FilterType.NullOutIrrelevantFilter);

            var nulledXAxis = nullOutFilter.ApplyFilter(windowFiltered.Select(x => x.X)).ToList();
            var nulledYAxis = nullOutFilter.ApplyFilter(windowFiltered.Select(y => y.Y)).ToList();
            var nulledZAxis = nullOutFilter.ApplyFilter(windowFiltered.Select(z => z.Z)).ToList();

            var firstNonNullOfX = nulledXAxis.FirstOrDefault(x => x > 0);
            var firstNonNullOfY = nulledYAxis.FirstOrDefault(y => y > 0);
            var firstNonNullOfZ = nulledZAxis.FirstOrDefault(z => z > 0);

            var lastNonNullOfX = nulledXAxis.LastOrDefault(x => x > 0);
            var lastNonNullOfY = nulledYAxis.LastOrDefault(y => y > 0);
            var lastNonNullOfZ = nulledZAxis.LastOrDefault(z => z > 0);

            var firstNonNullIndex = MinOfThree(nulledXAxis.IndexOf(firstNonNullOfX), nulledYAxis.IndexOf(firstNonNullOfY), nulledZAxis.IndexOf(firstNonNullOfZ));
            var lastNonNullIndex = MinOfThree(nulledXAxis.IndexOf(lastNonNullOfX), nulledYAxis.IndexOf(lastNonNullOfY), nulledZAxis.IndexOf(lastNonNullOfZ));

            List<Acceleration> output = new List<Acceleration>();
            for (int i = 0; i < accelerations.Count(); i++)
            {
                if (i < firstNonNullIndex)
                {
                    output.Add(new Acceleration(0, 0, 0));
                }
                else if (i > lastNonNullIndex)
                {
                    output.Add(new Acceleration(0, 0, 0));
                }
                else
                {
                    var currentAcc = accelerations.ElementAt(i);
                    output.Add(new Acceleration(currentAcc.X, currentAcc.Y, currentAcc.Z));
                }
            }

            return output;
        }

        private static double MinOfThree(double a, double b, double c)
        {
            return Math.Min(a, Math.Min(b, c));
        }

        private static double MaxOfThree(double a, double b, double c)
        {
            return Math.Max(a, Math.Max(b, c));
        }
    }
}