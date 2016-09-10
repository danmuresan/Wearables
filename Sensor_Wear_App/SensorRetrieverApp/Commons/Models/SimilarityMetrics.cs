using Commons.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Commons.Models
{
    public class SimilarityMetrics
    {
        private readonly IList<Acceleration> m_first;
        private readonly IList<Acceleration> m_second;

        public double CorrelationCoefficient { get; private set; }

        public double StandardDeviationDifference_XAxis { get; private set; }
        public double StandardDeviationDifference_YAxis { get; private set; }
        public double StandardDeviationDifference_ZAxis { get; private set; }

        public double VarianceDifference_XAxis { get; private set; }
        public double VarianceDifference_YAxis { get; private set; }
        public double VarianceDifference_ZAxis { get; private set; }

        public SimilarityMetrics(IEnumerable<Acceleration> first, IEnumerable<Acceleration> second)
        {
            m_first = first.ToList();
            m_second = second.ToList();
        }

        public void ComputeStandardDeviationDifference()
        {
            var stdDevFirst_XAxis = DataOperationsUtil.GetStandardDeviation(m_first.Select(x => x.X));
            var stdDevSecond_XAxis = DataOperationsUtil.GetStandardDeviation(m_second.Select(x => x.X));

            var stdDevFirst_YAxis = DataOperationsUtil.GetStandardDeviation(m_first.Select(y => y.Y));
            var stdDevSecond_YAxis = DataOperationsUtil.GetStandardDeviation(m_second.Select(y => y.Y));

            var stdDevFirst_ZAxis = DataOperationsUtil.GetStandardDeviation(m_first.Select(z => z.Z));
            var stdDevSecond_ZAxis = DataOperationsUtil.GetStandardDeviation(m_second.Select(z => z.Z));

            StandardDeviationDifference_XAxis = Math.Abs(stdDevFirst_XAxis - stdDevSecond_XAxis);
            StandardDeviationDifference_YAxis = Math.Abs(stdDevFirst_YAxis - stdDevSecond_YAxis);
            StandardDeviationDifference_ZAxis = Math.Abs(stdDevFirst_ZAxis - stdDevSecond_ZAxis);
        }

        public void ComputeVarianceDifference()
        {
            var varianceFirst_XAxis = DataOperationsUtil.GetVariance(m_first.Select(x => x.X));
            var varianceSecond_XAxis = DataOperationsUtil.GetVariance(m_second.Select(x => x.X));

            var varianceFirst_YAxis = DataOperationsUtil.GetVariance(m_first.Select(y => y.Y));
            var varianceSecond_YAxis = DataOperationsUtil.GetVariance(m_second.Select(y => y.Y));

            var varianceFirst_ZAxis = DataOperationsUtil.GetVariance(m_first.Select(z => z.Z));
            var varianceSecond_ZAxis = DataOperationsUtil.GetVariance(m_second.Select(z => z.Z));

            VarianceDifference_XAxis = Math.Abs(varianceFirst_XAxis - varianceSecond_XAxis);
            VarianceDifference_YAxis = Math.Abs(varianceFirst_YAxis - varianceSecond_YAxis);
            VarianceDifference_ZAxis = Math.Abs(varianceFirst_ZAxis - varianceSecond_ZAxis);
        }

        public void ComputeCrossCorrelationCoefficient()
        {
            CorrelationCoefficient = DataOperationsUtil.GetAvgCrossCorrelation(m_first, m_second);
        }

        public void ComputeAllMetrics()
        {
            ComputeCrossCorrelationCoefficient();
            ComputeStandardDeviationDifference();
            ComputeVarianceDifference();
        }
    }
}
