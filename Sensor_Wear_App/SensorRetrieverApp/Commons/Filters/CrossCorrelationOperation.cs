using Commons.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Commons.Filters
{
    public class CrossCorrelationOperation
    {
        private double[] m_inputDataFirst;
        private double[] m_inputDataSecond;
        private int m_seriesCount;
        private int m_maxDelay;

        private double MeanFirst => DataOperationsUtil.GetAvgValueRaw(m_inputDataFirst);
        private double MeanSecond => DataOperationsUtil.GetAvgValueRaw(m_inputDataSecond);

        public CrossCorrelationOperation(IEnumerable<double> inputDataFirst, IEnumerable<double> inputDataSecond, int? maxDelay = null)
        {
            InitializeInputs(inputDataFirst, inputDataSecond);
        }

        public void UpdateInputDataSeries(IEnumerable<double> inputDataFirst, IEnumerable<double> inputDataSecond, int? maxDelay = null)
        {
            InitializeInputs(inputDataFirst, inputDataSecond);
        }

        private void InitializeInputs(IEnumerable<double> inputDataFirst, IEnumerable<double> inputDataSecond, int? maxDelay = null)
        {
            if (inputDataFirst == null)
            {
                throw new ArgumentNullException(nameof(inputDataFirst));
            }
            if (inputDataSecond == null)
            {
                throw new ArgumentNullException(nameof(inputDataSecond));
            }

            m_inputDataFirst = inputDataFirst.ToArray();
            m_inputDataSecond = inputDataSecond.ToArray();
            m_seriesCount = m_inputDataFirst.Count(); ;
            m_maxDelay = maxDelay ?? m_seriesCount;
        }

        public double ComputeCoeff()
        {
            var sum1 = m_inputDataFirst.Zip(m_inputDataSecond, (x1, y1) => (x1 - MeanFirst) * (y1 - MeanSecond)).Sum();

            var sumSqr1 = m_inputDataFirst.Sum(x => Math.Pow((x - MeanFirst), 2.0));
            var sumSqr2 = m_inputDataSecond.Sum(y => Math.Pow((y - MeanSecond), 2.0));

            var result = sum1 / Math.Sqrt(sumSqr1 * sumSqr2);

            return result;
        }

        public double ComputeCoeffMethod2()
        {
            double[] array_xy = new double[m_seriesCount];
            double[] array_xp2 = new double[m_seriesCount];
            double[] array_yp2 = new double[m_seriesCount];

            for (int i = 0; i < m_seriesCount; i++)
            {
                array_xy[i] = m_inputDataFirst[i] * m_inputDataSecond[i];
            }

            for (int i = 0; i < m_seriesCount; i++)
            {
                array_xp2[i] = Math.Pow(m_inputDataFirst[i], 2.0);
            }

            for (int i = 0; i < m_seriesCount; i++)
            {
                array_yp2[i] = Math.Pow(m_inputDataSecond[i], 2.0);
            }

            double sum_x = 0;
            double sum_y = 0;

            foreach (double n in m_inputDataFirst)
            {
                sum_x += n;
            }

            foreach (double n in m_inputDataSecond)
            {
                sum_y += n;
            }

            double sum_xy = 0;
            foreach (double n in array_xy)
            {
                sum_xy += n;
            }

            double sum_xpow2 = 0;
            foreach (double n in array_xp2)
            {
                sum_xpow2 += n;
            }

            double sum_ypow2 = 0;
            foreach (double n in array_yp2)
            {
                sum_ypow2 += n;
            }

            double Ex2 = Math.Pow(sum_x, 2.00);
            double Ey2 = Math.Pow(sum_y, 2.00);

            return (m_inputDataFirst.Length * sum_xy - sum_x * sum_y) /
                     Math.Sqrt((m_inputDataFirst.Length * sum_xpow2 - Ex2) * (m_inputDataFirst.Length * sum_ypow2 - Ey2));
        }

        //public IDictionary<int, double> Apply()
        //{
        //    /* Calculate the denominator */
        //    double sumFirst = 0;
        //    double sumSecond = 0;

        //    for (int i = 0; i < m_seriesCount; i++)
        //    {
        //        sumFirst += (m_inputDataFirst[i] - MeanFirst) * (m_inputDataFirst[i] - MeanFirst);
        //        sumSecond += (m_inputDataSecond[i] - MeanSecond) * (m_inputDataSecond[i] - MeanSecond);
        //    }

        //    double denominator = Math.Sqrt(sumFirst * sumSecond);

        //    double sumFirstSecond;
        //    double corrAtDelay;
        //    var correlation = new Dictionary<int, double>();

        //    /* Calculate the correlation series */
        //    for (int delay = -m_maxDelay; delay < m_maxDelay; delay++)
        //    {
        //        sumFirstSecond = 0;
        //        for (int i = 0; i < m_seriesCount; i++)
        //        {
        //            int j = i + delay;
        //            if (j < 0 || j >= m_seriesCount)
        //            {
        //                continue;
        //            }
        //            else
        //            {
        //                sumFirstSecond += (m_inputDataFirst[i] - MeanFirst) * (m_inputDataSecond[j] - MeanSecond);
        //            }
        //            /* Or should it be (?)
        //            if (j < 0 || j >= n)
        //               sxy += (x[i] - mx) * (-my);
        //            else
        //               sxy += (x[i] - mx) * (y[j] - my);
        //            */
        //        }

        //        /* r is the correlation coefficient at "delay" */
        //        corrAtDelay = sumFirstSecond / denominator;
        //        correlation.Add(delay, corrAtDelay);
        //    }

        //    return correlation;
        //}
    }
}