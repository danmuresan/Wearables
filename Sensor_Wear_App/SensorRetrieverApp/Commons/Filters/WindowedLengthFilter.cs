using Commons.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Commons.Filters
{
    public class WindowedLengthFilter : IFilterOperation
    {
        public double FilterOrder { get; set; }

        public WindowedLengthFilter(int filterOrder = 100)
        {
            // filter order is the window size of the moving window for this filter
            FilterOrder = filterOrder;
        }

        public IEnumerable<double> ApplyFilter(IEnumerable<double> inputData)
        {
            var inputDataArray = inputData.ToArray();
            var inputLength = inputData.Count();

            double[] filteredData = new double[inputData.Count()];
            for (int i = 0; i < inputLength; i++)
            {
                double currentLength = 0;
                for (int j = 1; j < i + FilterOrder; j++)
                {
                    currentLength += DataOperationsUtil.GetEuclideanLength(j + 1, j, inputDataArray[j + 1], inputDataArray[j]);
                }

                filteredData[i] = currentLength;
            }

            return filteredData;
        }
    }
}