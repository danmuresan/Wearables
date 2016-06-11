using System.Collections.Generic;

namespace Commons.Filters
{
    public interface IFilterOperation
    {
        IEnumerable<double> ApplyFilter(IEnumerable<double> inputData);
    }

    public enum FilterType
    {
        RollingAverageLowPass
    }
}