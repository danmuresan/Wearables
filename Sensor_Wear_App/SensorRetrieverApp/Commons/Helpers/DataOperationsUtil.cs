using Commons.Filters;
using Commons.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Commons.Helpers
{
    public static class DataOperationsUtil
    {
        public static double GetMaxValueRaw(this IEnumerable<double> dataBatch)
        {
            if (dataBatch == null)
            {
                throw new ArgumentNullException(nameof(dataBatch));
            }

            return dataBatch.Max();
        }

        public static double GetMinValueRaw(this IEnumerable<double> dataBatch)
        {
            if (dataBatch == null)
            {
                throw new ArgumentNullException(nameof(dataBatch));
            }

            return dataBatch.Min();
        }

        public static double GetAvgValueRaw(this IEnumerable<double> dataBatch)
        {
            if (dataBatch == null)
            {
                throw new ArgumentNullException(nameof(dataBatch));
            }

            return dataBatch.Average();
        }

        public static MinMaxAverageValues GetMinMaxAvgValuesRaw(this IEnumerable<double> dataBatch)
        {
            if (dataBatch == null)
            {
                throw new ArgumentNullException(nameof(dataBatch));
            }

            var dataAsArray = dataBatch.ToArray();
            double min = int.MaxValue;
            double max = int.MinValue;
            double sum = 0;

            for (int i = 0; i < dataAsArray.Length; i++)
            {
                if (dataAsArray[i] < min)
                {
                    min = dataAsArray[i];
                }

                if (dataAsArray[i] > max)
                {
                    max = dataAsArray[i];
                }

                sum += dataAsArray[i];
            }

            return new MinMaxAverageValues(min, max, sum / dataAsArray.Length);
        }

        public static double GetStandardDeviation(this IEnumerable<double> values)
        {
            double avg = values.Average();
            return Math.Sqrt(values.Average(v => Math.Pow(v - avg, 2)));
        }

        public static double GetVariance(this IEnumerable<double> values)
        {
            double mean = values.GetAvgValueRaw();
            double variance = 0;
            var valuesArray = values.ToArray();

            for (int i = 0; i < valuesArray.Count(); i++)
            {
                variance += Math.Pow((valuesArray[i] - mean), 2);
            }

            int start = 0;
            int n = valuesArray.Count();
            if (start > 0)
            {
                n -= 1;
            }

            return variance / (n);
        }

        public static AccelerationAngles GetAccelerationAngles(this Acceleration acc, bool asDegrees = false)
        {
            var pitch = Math.Atan(acc.X / (Math.Sqrt(Math.Pow(acc.Y, 2) + Math.Pow(acc.Z, 2))));
            var roll = Math.Atan(acc.Y / (Math.Sqrt(Math.Pow(acc.X, 2) + Math.Pow(acc.Z, 2))));
            var yaw = Math.Atan((Math.Sqrt(Math.Pow(acc.X, 2) + Math.Pow(acc.Y, 2))) / acc.Z);

            if (!asDegrees)
            {
                // return radians vals
                return new AccelerationAngles(pitch, roll, yaw);
            }
            else
            {
                var pitchDegrees = DataConversionHelper.RadiansToDegrees(pitch);
                var rollDegrees = DataConversionHelper.RadiansToDegrees(roll);
                var yawDegrees = DataConversionHelper.RadiansToDegrees(yaw);

                return new AccelerationAngles(pitchDegrees, rollDegrees, yawDegrees);
            }
        }

        /// <summary>
        /// sqrt((x1 - x2)^2 + (y1 - y2)^2)
        /// </summary>
        /// <param name="x1"></param>
        /// <param name="x2"></param>
        /// <param name="y1"></param>
        /// <param name="y2"></param>
        /// <returns></returns>
        public static double GetEuclideanLength(double x1, double x2, double y1, double y2)
        {
            return Math.Sqrt(Math.Pow(x1 - x2, 2) + Math.Pow(y1 - y2, 2));
        }

        /// <summary>
        /// Compute speed based on integral of acceleration over time, starting with initial velocity at 0 (before a shot)
        /// v = v0 + a * dt
        /// which is the numerical integration of acceleration over time: v(t) = integral(a(t)dt)
        /// </summary>
        /// <param name="cce"></param>
        /// <returns></returns>
        private static double GetAvgVelocityForAccelerationAxis(List<double> accOnAxis, int sampleOffset = 10)
        {
            // for simplicity, we assume that before any shot, the velocity is at some point 0 (when taking the racket back for instance)
            // dt (delta time) = 10 samples interval
            double initialVelocity = 0; // v0
            double currentVelocity = 0; // v at time t
            const int sampleFrq = 200;
            double deltaTime = ((sampleOffset * 1000) / sampleFrq) / 1000.0; // based on 200 HZ sample frq, 10 samples come in at approx. 50 ms offsets => 0.05 seconds

            double totalSpeed = 0;
            int accCount = 0;
            for (int i = 0; i < accOnAxis.Count; i += sampleOffset)
            {
                currentVelocity = initialVelocity + accOnAxis[i] * deltaTime;
                totalSpeed += currentVelocity;
                initialVelocity = currentVelocity;
                accCount++;
            }

            // avg speed for current sample batch (meters / second)
            return currentVelocity;
        }

        public static double GetAvgVelocityForAccelerationBatch(List<Acceleration> accBatchSignal)
        {
            double avgVelocityXAxis = GetAvgVelocityForAccelerationAxis(accBatchSignal.Select(x => x.X).ToList());
            double avgVelocityYAxis = GetAvgVelocityForAccelerationAxis(accBatchSignal.Select(y => y.Y).ToList());
            double avgVelocityZAxis = GetAvgVelocityForAccelerationAxis(accBatchSignal.Select(z => z.Z).ToList());

            // compute the avg of total speed based on polar coordinates (per axis values)
            var velocityTotal = Math.Sqrt(Math.Pow(avgVelocityXAxis, 2) + Math.Pow(avgVelocityYAxis, 2) + Math.Pow(avgVelocityZAxis, 2));
            return Math.Round(Math.Abs(velocityTotal), 2, MidpointRounding.AwayFromZero);
        }

        public static double GetAvgCrossCorrelation(IList<Acceleration> accSeriesFirst, IList<Acceleration> accSeriesSecond)
        {
            var xAxisAccelerationsFirst = accSeriesFirst.Select(x => x.X).ToArray();
            var xAxisAccelerationsSecond = accSeriesSecond.Select(x => x.X).ToArray();
            var yAxisAccelerationsFirst = accSeriesFirst.Select(y => y.Y).ToArray();
            var yAxisAccelerationsSecond = accSeriesSecond.Select(y => y.Y).ToArray();
            var zAxisAccelerationsFirst = accSeriesFirst.Select(z => z.Z).ToArray();
            var zAxisAccelerationsSecond = accSeriesSecond.Select(z => z.Z).ToArray();

            var corrOp = new CrossCorrelationOperation(xAxisAccelerationsFirst, xAxisAccelerationsSecond);
            var corrCoefX = corrOp.ComputeCoeffMethod2();

            corrOp.UpdateInputDataSeries(yAxisAccelerationsFirst, yAxisAccelerationsSecond);
            var corrCoefY = corrOp.ComputeCoeffMethod2();

            corrOp.UpdateInputDataSeries(zAxisAccelerationsFirst, zAxisAccelerationsSecond);
            var corrCoefZ = corrOp.ComputeCoeffMethod2();

            var avgCorrScore = (Math.Abs(corrCoefX) + Math.Abs(corrCoefY) + Math.Abs(corrCoefZ)) / 3.0;

            return avgCorrScore;
        }

        public static IEnumerable<Acceleration> ApplyWindowFilterOnAccelerationBatch(IEnumerable<Acceleration> accelerations)
        {
            List<Acceleration> normalizedAccelerations = new List<Acceleration>();
            var windowedLengthFilter = FilterFactory.GetFilterByType(FilterType.WindowedLengthFilter);

            var normalizedXAxis = windowedLengthFilter.ApplyFilter(accelerations.Select(x => x.X)).ToList();
            var normalizedYAxis = windowedLengthFilter.ApplyFilter(accelerations.Select(y => y.Y)).ToList();
            var normalizedZAxis = windowedLengthFilter.ApplyFilter(accelerations.Select(z => z.Z)).ToList();

            for (int i = 0; i < normalizedXAxis.Count(); i++)
            {
                normalizedAccelerations.Add(new Acceleration(normalizedXAxis[i], normalizedYAxis[i], normalizedZAxis[i]));
            }

            return normalizedAccelerations;
        }

        /// <summary>
        /// Peak based, shot extraction algorithm as descibed below
        /// Takes a list of accelerations as input and produces a list of lists of accelerations (each subist representing a shot out of the original raw signal)
        /// </summary>
        /* 
         *  Compute mean of X_Axis
         *  Compute max and min of X_Axis

         *  Compute mean of Y_Axis
         *  Compute max and min of Y_Axis

         *  Skip check for Z_Axis
            --------------------------------------------------
         *  From observations:
            - take all x peaks for which: value >= 80% of ABS(MAX - AVG)
            - take all x peaks for which: value <= 80% of ABS(MIN - AVG)

            - supress close peaks(close to each other) - take one max / min only

            - take all y peaks for which same conditions hold with 70%

            - supress close peaks for Y

            - take a window of 500 samples and check for peaks within window(X and Y)

            - if conditions for peaks are met in the window, remove the peaks from the list and mark a window of 500 samples,
            centered around some peak as a new shot found

            - isolate and save off those positions in the raw acceleration array

            - continue checks until end */
        /// </summary>
        // TODO: Maybe add a safe check to not allow peaks of absolute values smaller than some threshold???
        public static List<List<Acceleration>> ApplyShotsExtractionAlgorithm(List<Acceleration> dataToProcess, int windowWidth = 550)
        {
            List<List<Acceleration>> output = new List<List<Acceleration>>();
            if (windowWidth < 200)
            {
                windowWidth = 200;
            }

            var xFilteredValues = dataToProcess.Select(x => x.X);
            var yFilteredValues = dataToProcess.Select(y => y.Y);

            var maxPeaksFilter = FilterFactory.GetFilterByType(FilterType.MaxPeaksFilter);
            var minPeaksFilter = FilterFactory.GetFilterByType(FilterType.MinPeaksFilter);

            var xAxisMaxPeaks = maxPeaksFilter.ApplyFilter(xFilteredValues);
            var xAxisMinPeaks = minPeaksFilter.ApplyFilter(xFilteredValues);

            // 80 % is the default, we need a smaller threshold here
            maxPeaksFilter.FilterOrder = 0.65;
            minPeaksFilter.FilterOrder = 0.65;

            var yAxisMaxPeaks = maxPeaksFilter.ApplyFilter(yFilteredValues);
            var yAxisMinPeaks = minPeaksFilter.ApplyFilter(yFilteredValues);

            // center around x-axis max value peak if we have other peaks in the area
            var peaksToCenterAround = xAxisMaxPeaks.ToArray();

            for (int i = 0; i < peaksToCenterAround.Length; i++)
            {
                if (peaksToCenterAround[i] != 0)
                {
                    // check other peaks
                    int peaksInWindow = 0;
                    var startOfWindow = i - windowWidth / 2;
                    if (startOfWindow < 0)
                    {
                        startOfWindow = 0;
                    }

                    var endOfWindow = i + windowWidth / 2;
                    if (endOfWindow > peaksToCenterAround.Length)
                    {
                        endOfWindow = peaksToCenterAround.Length;
                    }

                    for (int j = startOfWindow; j < endOfWindow; j++)
                    {
                        if (xAxisMinPeaks.ElementAt(j) != 0)
                        {
                            peaksInWindow++;
                        }

                        if (yAxisMaxPeaks.ElementAt(j) != 0)
                        {
                            peaksInWindow++;
                        }

                        if (yAxisMinPeaks.ElementAt(j) != 0)
                        {
                            peaksInWindow++;
                        }
                    }

                    if (peaksInWindow >= 3)
                    {
                        var accelerationsAroundPeak = dataToProcess.Skip(startOfWindow).Take(endOfWindow - startOfWindow).ToList();
                        output.Add(accelerationsAroundPeak);
                    }
                }
            }

            return output;
        }
    }

    public struct MinMaxAverageValues
    {
        public double Min { get; }
        public double Max { get; }
        public double Avg { get; }

        public MinMaxAverageValues(double min, double max, double avg)
        {
            Min = min;
            Max = max;
            Avg = avg;
        }
    }
}