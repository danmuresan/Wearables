using System;
using Android.App;
using Android.OS;
using Android.Widget;
using SensorClientApp.Helpers;
using Commons.Models;
using Commons.Helpers;
using System.Threading.Tasks;
using System.Collections.Generic;
using Android.Util;
using Commons.Constants;
using Commons.Filters;

namespace SensorClientApp
{
    [Activity(Label = "DataProcessorActivity")]
    public class DataProcessorActivity : Activity
    {
        private Button m_exportToCsvBtn;
        private StorageManager m_storageManager;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            SetContentView(Resource.Layout.SimpleProcessorLayout);

            m_exportToCsvBtn = FindViewById<Button>(Resource.Id.ExportCsvButton);

            m_exportToCsvBtn.Click += OnExportToCsvClick;

            m_storageManager = new StorageManager();
        }

        private async void OnExportToCsvClick(object sender, EventArgs e)
        {
            string csvX = string.Empty, csvY = string.Empty, csvZ = string.Empty;
            foreach (var item in m_storageManager.RetrieveAllUnexportedSerializedData<AccelerationBatch>())
            {
                // apply a low pass filter for the retreived acceleration batches
                var filteredItem = item.FilterAccelerationBatch(FilterType.RollingAverageLowPass);
                var csvAccelerationBatch = filteredItem.ToCsv();
                csvX += csvAccelerationBatch[0];
                csvY += csvAccelerationBatch[1];
                csvZ += csvAccelerationBatch[2];
            }

            try
            {
                Task xWriteTask = FileManipulationHelper.WriteToFileAsync($"{Constants.XAxisCsvFileSuffix}_{DateTime.Now.ToString(Constants.CustomShortDateFormat)}.csv", csvX);
                Task yWriteTask = FileManipulationHelper.WriteToFileAsync($"{Constants.YAxisCsvFileSuffix}_{DateTime.Now.ToString(Constants.CustomShortDateFormat)}.csv", csvY);
                Task zWriteTask = FileManipulationHelper.WriteToFileAsync($"{Constants.ZAxisCsvFileSuffix}_{DateTime.Now.ToString(Constants.CustomShortDateFormat)}.csv", csvZ);
                await Task.WhenAll(new List<Task> { xWriteTask, yWriteTask, zWriteTask });
                m_storageManager.SaveExportIndex(m_storageManager.RetrieveDataIndex());

                Toast.MakeText(this, "CSV export successful!", ToastLength.Long).Show();
            }
            catch (Exception ex)
            {
                Log.Error("PROCESSOR", ex.ToString());
                Toast.MakeText(this, "CSV export failed!", ToastLength.Long).Show();
            }
        }
    }
}