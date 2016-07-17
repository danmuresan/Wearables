
namespace Commons.Filters
{
    public static class FilterFactory
    {
        public static IFilterOperation GetFilterByType(FilterType type, double? filterOrder = null)
        {
            switch(type)
            {
                case FilterType.RollingAverageLowPass:
                    return new RollingAverageFilter();
                case FilterType.DerivativeFilter:
                    return new DerivativeFilter();
                case FilterType.MaxPeaksFilter:
                    return new MaximumPeaksFilter();
                case FilterType.MinPeaksFilter:
                    return new MinimumPeaksFilter();
                case FilterType.NullOutIrrelevantFilter:
                    return new NullOutIrrelevanciesFilter();
                case FilterType.WindowedLengthFilter:
                    return filterOrder == null ? new WindowedLengthFilter() : new WindowedLengthFilter((int)filterOrder);
                default:
                    return null;
            }
        }
    }
}