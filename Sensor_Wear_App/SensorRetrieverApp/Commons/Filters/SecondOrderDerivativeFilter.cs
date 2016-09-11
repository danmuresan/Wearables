using System;
using System.Collections.Generic;
using System.Linq;

namespace Commons.Filters
{
    public class SecondOrderDerivativeFilter : IFilterOperation
    {
        public double FilterOrder { get; set; }

        public IEnumerable<double> ApplyFilter(IEnumerable<double> inputData)
        {
            var dataAsArray = inputData.ToArray();
            List<double> outputList = new List<double>();

            for (int i = 0; i < dataAsArray.Length - 3; i++)
            {
                // formula we use based on matrix multiplication: [1 -2 1] * [a1 a2 a3]
                var secondDerivForCurrentPoint = dataAsArray[i] + (-2) * dataAsArray[i + 1] + dataAsArray[i + 2];
                outputList.Add(secondDerivForCurrentPoint);
            }

            return outputList;
        }
    }
}