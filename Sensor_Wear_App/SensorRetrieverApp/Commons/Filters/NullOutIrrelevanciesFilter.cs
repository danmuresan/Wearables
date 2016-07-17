using System;
using System.Collections.Generic;

namespace Commons.Filters
{
    public class NullOutIrrelevanciesFilter : IFilterOperation
    {
        private readonly IFilterOperation m_windowLengthFilter;

        public double FilterOrder { get; set; }

        public NullOutIrrelevanciesFilter()
        {
            m_windowLengthFilter = FilterFactory.GetFilterByType(FilterType.WindowedLengthFilter);
        }

        /// <summary>
        /// At places where there is no movement (which we may be interested in), we should simplify the signal
        /// by nulling out those values below some adaptive threshold value
        /// </summary>
        public IEnumerable<double> ApplyFilter(IEnumerable<double> inputData)
        {
            // TODO: ...
            return null;
        }
    }
}