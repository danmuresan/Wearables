using System.Collections.Generic;

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
                default:
                    return null;
            }
        }
    }
}