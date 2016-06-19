using System;
using System.Collections.Generic;
using System.Linq;

namespace Commons.Helpers
{
    public static class DataOperationsUtil
    {
        public static double GetMaxValueRaw(this IEnumerable<double> dataBatch)
        {
            if (dataBatch == null)
            {
                throw new ArgumentNullException(nameof(dataBatch));
            }

            return dataBatch.Max();
        }

        public static double GetMinValueRaw(this IEnumerable<double> dataBatch)
        {
            if (dataBatch == null)
            {
                throw new ArgumentNullException(nameof(dataBatch));
            }

            return dataBatch.Min();
        }

        public static double GetAvgValueRaw(this IEnumerable<double> dataBatch)
        {
            if (dataBatch == null)
            {
                throw new ArgumentNullException(nameof(dataBatch));
            }

            return dataBatch.Average();
        }

        public static MinMaxAverageValues GetMinMaxAvgValuesRaw(this IEnumerable<double> dataBatch)
        {
            if (dataBatch == null)
            {
                throw new ArgumentNullException(nameof(dataBatch));
            }

            var dataAsArray = dataBatch.ToArray();
            double min = int.MaxValue;
            double max = int.MinValue;
            double sum = 0;

            for (int i = 0; i < dataAsArray.Length; i++)
            {
                if (dataAsArray[i] < min)
                {
                    min = dataAsArray[i];
                }

                if (dataAsArray[i] > max)
                {
                    max = dataAsArray[i];
                }

                sum += dataAsArray[i];
            }

            return new MinMaxAverageValues(min, max, sum / dataAsArray.Length);
        }
    }

    public struct MinMaxAverageValues
    {
        public double Min { get; }
        public double Max { get; }
        public double Avg { get; }

        public MinMaxAverageValues(double min, double max, double avg)
        {
            Min = min;
            Max = max;
            Avg = avg;
        }
    }
}