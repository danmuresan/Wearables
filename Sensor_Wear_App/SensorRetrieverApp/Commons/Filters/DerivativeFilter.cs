using System;
using System.Collections.Generic;
using System.Linq;

namespace Commons.Filters
{
    public class DerivativeFilter : IFilterOperation
    {
        public DerivativeFilter(double order = 0)
        {
            FilterOrder = order;
        }

        public double FilterOrder { get; set; }

        public IEnumerable<double> ApplyFilter(IEnumerable<double> inputData)
        {
            var dataAsArray = inputData.ToArray();
            for (int i = 0; i < dataAsArray.Length - 1; i++)
            {
                dataAsArray[i] = dataAsArray[i + 1] - dataAsArray[i];
            }

            // pad out last index
            dataAsArray[dataAsArray.Length - 1] = 0;

            return dataAsArray;
        }
    }
}