using Commons.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Commons.Filters
{
    public class WindowedLengthFilter : IFilterOperation
    {
        public double FilterOrder { get; set; }

        public WindowedLengthFilter(int filterOrder = 50)
        {
            // filter order is the window size of the moving window for this filter
            FilterOrder = filterOrder;
        }

        public IEnumerable<double> ApplyFilter(IEnumerable<double> inputData)
        {
            var inputDataArray = inputData.ToArray();
            var outputLength = inputData.Count() - (int)FilterOrder;

            double[] filteredData = new double[outputLength];
            for (int i = 0; i < outputLength; i++)
            {
                double currentLength = 0;
                for (int j = i; j < i + FilterOrder; j++)
                {
                    currentLength += DataOperationsUtil.GetEuclideanLength(j + 1, j, inputDataArray[j + 1], inputDataArray[j]);
                }

                filteredData[i] = currentLength;
            }

            return filteredData;
        }
    }
}