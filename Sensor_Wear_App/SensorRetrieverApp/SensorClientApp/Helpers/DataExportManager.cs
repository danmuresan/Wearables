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

        /// <summary>
        /// Exports all not previously exported data
        /// Adds processing options such as filters which are applied successively
        /// </summary>
        public async Task<bool> ExportAllRawAsync(List<FilterType> filtersByType)
        {
            string csvX = string.Empty, csvY = string.Empty, csvZ = string.Empty;
            bool result = false;

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

                if (string.IsNullOrEmpty(csvX) && string.IsNullOrEmpty(csvY) && string.IsNullOrEmpty(csvZ))
                {
                    result = true;
                }

                try
                {
                    Task xWriteTask = FileManipulationHelper.WriteToFileAsync($"{Constants.XAxisCsvFileSuffix}_{DateTime.Now.ToString(Constants.CustomShortDateFormat)}.csv", csvX);
                    Task yWriteTask = FileManipulationHelper.WriteToFileAsync($"{Constants.YAxisCsvFileSuffix}_{DateTime.Now.ToString(Constants.CustomShortDateFormat)}.csv", csvY);
                    Task zWriteTask = FileManipulationHelper.WriteToFileAsync($"{Constants.ZAxisCsvFileSuffix}_{DateTime.Now.ToString(Constants.CustomShortDateFormat)}.csv", csvZ);
                    await Task.WhenAll(new List<Task> { xWriteTask, yWriteTask, zWriteTask });
                    m_storageManager.SaveExportIndex(m_storageManager.RetrieveDataIndex());

                    result = true;
                }
                catch (Exception ex)
                {
                    Log.Error("PROCESSOR", ex.ToString());
                    result = false;
                }
            });

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

        public async Task<bool> ExportPerShotDataAsync()
        {
            return true;
        }
    }
}