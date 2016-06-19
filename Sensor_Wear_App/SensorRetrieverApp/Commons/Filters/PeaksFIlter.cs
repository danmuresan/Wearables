using Commons.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Commons.Filters
{
    public abstract class PeaksFilterBase : IFilterOperation
    {
        public PeaksFilterBase(double order = 0.8)
        {
            FilterOrder = order;
        }

        public double FilterOrder { get; set; }

        public IEnumerable<double> ApplyFilter(IEnumerable<double> inputData)
        {
            var minMaxAvgValues = inputData.GetMinMaxAvgValuesRaw();
            var allPeaks = GetAllPeaks(inputData.ToArray(), minMaxAvgValues);
            return SupressClosePeaks(allPeaks);
        }

        protected abstract double[] GetAllPeaks(double[] inputData, MinMaxAverageValues minMaxAvgValues);

        private IEnumerable<double> SupressClosePeaks(double[] allPeaks)
        {
            List<Tuple<int, double>> finalPeaks = new List<Tuple<int, double>>();
            const int supressLength = 250;

            int i = 0;
            while (i < allPeaks.Length)
            {
                if (allPeaks[i] != 0)
                {
                    finalPeaks.Add(new Tuple<int, double>(i, allPeaks[i]));
                    i += supressLength;
                }
                else
                {
                    i++;
                }
            }

            for (i = 0; i < allPeaks.Length; i++)
            {
                allPeaks[i] = 0;
            }

            for (i = 0; i < finalPeaks.Count(); i++)
            {
                allPeaks[finalPeaks[i].Item1] = finalPeaks[i].Item2;
            }

            return allPeaks;
        }
    }

    public class MaximumPeaksFilter : PeaksFilterBase
    {
        protected override double[] GetAllPeaks(double[] inputData, MinMaxAverageValues minMaxAvgValues)
        {
            List<double> maxPeaks = new List<double>();
            for (int i = 0; i < inputData.Length; i++)
            {
                if (inputData[i] >= FilterOrder * (Math.Abs(minMaxAvgValues.Max) - Math.Abs(minMaxAvgValues.Avg)))
                {
                    maxPeaks.Add(inputData[i]);
                }
                else
                {
                    maxPeaks.Add(0);
                }
            }

            return maxPeaks.ToArray();
        }
    }

    public class MinimumPeaksFilter : PeaksFilterBase
    {
        protected override double[] GetAllPeaks(double[] inputData, MinMaxAverageValues minMaxAvgValues)
        {
            List<double> minPeaks = new List<double>();
            for (int i = 0; i < inputData.Length; i++)
            {
                if (inputData[i] <= FilterOrder * (Math.Abs(minMaxAvgValues.Avg) - Math.Abs(minMaxAvgValues.Min)))
                {
                    minPeaks.Add(inputData[i]);
                }
                else
                {
                    minPeaks.Add(0);
                }
            }

            return minPeaks.ToArray();
        }
    }
}