using Android.Content;
using Android.Util;
using Commons.Constants;
using Commons.Filters;
using Commons.Helpers;
using Commons.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace SensorClientApp.Helpers
{
    public class DataExportManager
    {
        private readonly StorageManager m_storageManager;

        public DataExportManager(Context context)
        {
            m_storageManager = new StorageManager();
        }

        public async Task<bool> ExportLogsAsync()
        {
            // TODO: custom logger which actually builds out a string with the stack trace (common for both apps)
            return false;
        }

        /// <summary>
        /// Exports all not previously exported data
        /// Adds processing options such as filters which are applied successively
        /// </summary>
        public async Task<bool> ExportAllRawAsync(List<FilterType> filtersByType)
        {
            bool result = false;
            string csvX = string.Empty, csvY = string.Empty, csvZ = string.Empty;
            await Task.Run(async () =>
            {
                foreach (var item in m_storageManager.RetrieveAllUnexportedSerializedData<AccelerationBatch>())
                {
                    // apply a low pass filter for the retreived acceleration batches
                    var filteredItem = FilterRawAccelerationBatch(item, filtersByType);
                    var csvAccelerationBatch = filteredItem.ToCsv();
                    csvX += csvAccelerationBatch[0];
                    csvY += csvAccelerationBatch[1];
                    csvZ += csvAccelerationBatch[2];
                }

                result = await WriteAccelerationBatchToCsvFileAsync(csvX, csvY, csvZ);
            });

            return result;
        }

        private async Task<bool> WriteAccelerationBatchToCsvFileAsync(string csvX, string csvY, string csvZ, string fileName = null)
        {
            bool result;
            string fileNameXAxis = $"{Constants.XAxisCsvFileSuffix}_{fileName ?? DateTime.Now.ToString(Constants.CustomShortDateFormat)}.csv";
            string fileNameYAxis = $"{Constants.YAxisCsvFileSuffix}_{fileName ?? DateTime.Now.ToString(Constants.CustomShortDateFormat)}.csv";
            string fileNameZAxis = $"{Constants.ZAxisCsvFileSuffix}_{fileName ?? DateTime.Now.ToString(Constants.CustomShortDateFormat)}.csv";

            if (string.IsNullOrEmpty(csvX) && string.IsNullOrEmpty(csvY) && string.IsNullOrEmpty(csvZ))
            {
                return false;
            }

            try
            {
                Task xWriteTask = FileManipulationHelper.WriteToFileAsync(fileNameXAxis, csvX);
                Task yWriteTask = FileManipulationHelper.WriteToFileAsync(fileNameYAxis, csvY);
                Task zWriteTask = FileManipulationHelper.WriteToFileAsync(fileNameZAxis, csvZ);
                await Task.WhenAll(new List<Task> { xWriteTask, yWriteTask, zWriteTask });
                m_storageManager.SaveExportIndex(m_storageManager.RetrieveDataIndex());

                result = true;
            }
            catch (Exception ex)
            {
                Log.Error("PROCESSOR", ex.ToString());
                result = false;
            }

            return result;
        }

        private AccelerationBatch FilterRawAccelerationBatch(AccelerationBatch item, List<FilterType> filtersByType)
        {
            foreach (var filterType in filtersByType)
            {
                item = item.FilterAccelerationBatch(filterType);
            }

            return item;
        }

        /// <summary>
        /// Exports all not previously exported data and throws out data not corresponding to a shot based on a custom alg
        /// Adds processing options such as filters which are applied successively
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
        public async Task<List<bool>> ExportPerShotDataAsync()
        {
            var allUnexportedData = m_storageManager.RetrieveAllUnexportedSerializedData<AccelerationBatch>();
            string csvX = string.Empty;
            string csvY = string.Empty;
            string csvZ = string.Empty;
            List<bool> shotsExportedSuccessfully = new List<bool>();

            await Task.Run(async () =>
            {

                foreach (var unexportedDataItem in allUnexportedData)
                {
                    var filteredItem = FilterRawAccelerationBatch(unexportedDataItem, new List<FilterType> { FilterType.RollingAverageLowPass });

                    var xFilteredValues = filteredItem.Accelerations.Select(x => x.X);
                    var yFilteredValues = filteredItem.Accelerations.Select(y => y.Y);

                    var maxPeaksFilter = FilterFactory.GetFilterByType(FilterType.MaxPeaksFilter);
                    var minPeaksFilter = FilterFactory.GetFilterByType(FilterType.MinPeaksFilter);

                    var xAxisMaxPeaks = maxPeaksFilter.ApplyFilter(xFilteredValues);
                    var xAxisMinPeaks = minPeaksFilter.ApplyFilter(xFilteredValues);

                    maxPeaksFilter.FilterOrder = 0.7;
                    minPeaksFilter.FilterOrder = 0.7;

                    var yAxisMaxPeaks = maxPeaksFilter.ApplyFilter(yFilteredValues);
                    var yAxisMinPeaks = minPeaksFilter.ApplyFilter(yFilteredValues);

                    // center around x-axis max value peak if we have other peaks in the area
                    var peaksToCenterAround = xAxisMaxPeaks.ToArray();
                    const int windowLength = 550;

                    for (int i = 0; i < peaksToCenterAround.Length; i++)
                    {
                        if (peaksToCenterAround[i] != 0)
                        {
                            // check other peaks
                            int peaksInWindow = 0;
                            var startOfWindow = i - windowLength / 2;
                            if (startOfWindow < 0)
                            {
                                startOfWindow = 0;
                            }

                            var endOfWindow = i + windowLength / 2;
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
                                // TODO: (we have a good window, export it)

                                var accelerationsAroundPeak = filteredItem.Accelerations.Skip(startOfWindow).Take(endOfWindow - startOfWindow).ToList();
                                var csvAccelerationBatch = accelerationsAroundPeak.ToCsv();
                                csvX = csvAccelerationBatch[0];
                                csvY = csvAccelerationBatch[1];
                                csvZ = csvAccelerationBatch[2];

                                var res = await WriteAccelerationBatchToCsvFileAsync(csvX, csvY, csvZ, $"shot_{i}");
                                shotsExportedSuccessfully.Add(res);
                            }
                        }
                    }
                }
            });

            return shotsExportedSuccessfully;
        }
    }
}