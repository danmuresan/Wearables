using System;
using System.Collections.Generic;
using System.Linq;

namespace Commons.Filters
{
    /// <summary>
    /// Low pass fiter used to smooth out possible short term variations due to noise
    /// </summary>
    public class RollingAverageFilter : IFilterOperation
    {
        private const int DefaultFilterMagnitude = 5;
        private int? m_filterMagnitude = null;

        private void CheckFilterData(IEnumerable<double> dataBatch, int filterOrder)
        {
            if (dataBatch == null)
            {
                throw new ArgumentNullException(nameof(dataBatch));
            }
            if (filterOrder % 2 != 0)
            {
                throw new ArgumentException(nameof(filterOrder), "Filter order must be an odd number");
            }
            if (filterOrder < 3 && filterOrder > 9)
            {
                throw new ArgumentOutOfRangeException(nameof(filterOrder), "Range between 3 and 9");
            }
        }

        public void SetFilterMagnitude(int filterMagnitude)
        {
            m_filterMagnitude = filterMagnitude;
        }

        public IEnumerable<double> ApplyFilter(IEnumerable<double> dataBatch)
        {
            var filterOrder = m_filterMagnitude ?? DefaultFilterMagnitude;
            CheckFilterData(dataBatch, filterOrder);

            int startIndex = (filterOrder / 2);
            var dataArray = dataBatch.ToArray();
            var outputArray = new double[dataArray.Length];

            // populate start and end of the output array (we can't use the same array for output as it will alter values on the go)
            for (int i = 0; i < startIndex; i++)
            {
                outputArray[i] = dataArray[i];
            }

            for (int i = dataArray.Length - 1; i > dataArray.Length - startIndex; i--)
            {
                outputArray[i] = dataArray[i];
            }

            for (int i = startIndex; i < dataArray.Length - startIndex; i++)
            {
                double sum = 0;
                for (int j = i - startIndex; j <= i + startIndex; j++)
                {
                    sum += dataArray[j];
                }

                outputArray[i] = sum / filterOrder;
            }

            return dataArray.ToList();
        }
    }
}