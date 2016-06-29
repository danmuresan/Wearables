using Android.Content;
using Android.Util;
using Commons.Constants;
using Commons.Filters;
using Commons.Helpers;
using Commons.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

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
        public async Task<List<bool>> ExportPerShotDataAsync()
        {
            var allUnexportedData = m_storageManager.RetrieveAllUnexportedSerializedData<AccelerationBatch>();
            string csvX = string.Empty;
            string csvY = string.Empty;
            string csvZ = string.Empty;
            List<bool> shotsExportedSuccessfully = new List<bool>();

            await Task.Run(async () =>
            {
                var dataToProcess = new List<Acceleration>();
                foreach (var unexportedDataItem in allUnexportedData)
                {
                    var filteredItem = FilterRawAccelerationBatch(unexportedDataItem, new List<FilterType> { FilterType.RollingAverageLowPass });
                    dataToProcess.AddRange(filteredItem.Accelerations);
                }

                var shotsList = DataOperationsUtil.ApplyShotsExtractionAlgorithm(dataToProcess);
                foreach (var shot in shotsList)
                {
                    var csvAccelerationBatch = shot.ToCsv();
                    csvX = csvAccelerationBatch[0];
                    csvY = csvAccelerationBatch[1];
                    csvZ = csvAccelerationBatch[2];

                    var res = await WriteAccelerationBatchToCsvFileAsync(csvX, csvY, csvZ, $"shot_{shotsList.IndexOf(shot)}");
                    shotsExportedSuccessfully.Add(res);
                }
            });

            return shotsExportedSuccessfully;
        }        
    }
}