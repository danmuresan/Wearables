using System;
using Android.App;
using Android.OS;
using Android.Widget;
using SensorClientApp.Helpers;
using System.Collections.Generic;
using Commons.Filters;

namespace SensorClientApp
{
    [Activity(Label = "DataProcessorActivity")]
    public class DataProcessorActivity : Activity
    {
        private Button m_exportToCsvBtn;
        private DataExportManager m_dataExportManager;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            SetContentView(Resource.Layout.SimpleProcessorLayout);

            m_exportToCsvBtn = FindViewById<Button>(Resource.Id.ExportCsvButton);

            m_exportToCsvBtn.Click += OnExportToCsvClick;

            m_dataExportManager = new DataExportManager(this);
        }

        private void OnExportToCsvClick(object sender, EventArgs e)
        {
            var filters = new List<FilterType> { FilterType.RollingAverageLowPass };
            new AlertDialog.Builder(this)
                .SetTitle("Export options")
                .SetMessage("Choose how you want to export your data?")
                .SetPositiveButton("Export all (apply low-pass filter on raw data and export it entirely) ?", async (o, ev) => {
                    var result = await m_dataExportManager.ExportAllRawAsync(filters);
                    if (result)
                    {
                        Toast.MakeText(this, "CSV export successful!", ToastLength.Long).Show();
                    }
                    else
                    {
                        Toast.MakeText(this, "CSV export failed!", ToastLength.Long).Show();
                    }
                })
                .SetNegativeButton("Export single shot only (filter out data we don't really need) ?", async (o, ev) => {
                    await m_dataExportManager.ExportPerShotDataAsync();
                    Toast.MakeText(this, "Nothing to do for now!", ToastLength.Long).Show();
                })
                .Show();
        }
    }
}