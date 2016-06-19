
namespace Commons.Filters
{
    public static class FilterFactory
    {
        public static IFilterOperation GetFilterByType(FilterType type)
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
                default:
                    return null;
            }
        }
    }
}