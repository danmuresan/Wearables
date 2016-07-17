using System.Collections.Generic;

namespace Commons.Filters
{
    public interface IFilterOperation
    {
        double FilterOrder { get; set; }
        IEnumerable<double> ApplyFilter(IEnumerable<double> inputData);
    }

    public enum FilterType
    {
        RollingAverageLowPass,
        DerivativeFilter,
        MaxPeaksFilter,
        MinPeaksFilter,
        NullOutIrrelevantFilter,
        WindowedLengthFilter
    }
}