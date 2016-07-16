using System;
using Android.App;
using Android.OS;
using Android.Widget;
using SensorClientApp.Helpers;
using System.Collections.Generic;
using Commons.Filters;
using System.Linq;
using Android.Content;

namespace SensorClientApp
{
    [Activity(Label = "DataProcessorActivity")]
    public class DataProcessorActivity : Activity
    {
        private Button m_exportToCsvBtn;
        private Button m_processDataBtn;
        private Spinner m_processDataIndexSpinnerStart;
        private Spinner m_processDataIndexSpinnerEnd;
        private TextView m_totalSessionsCollectedTextView;

        private int m_totalSessions;
        private DataExportManager m_dataExportManager;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            SetContentView(Resource.Layout.SimpleProcessorLayout);
            m_dataExportManager = new DataExportManager(this);

            m_exportToCsvBtn = FindViewById<Button>(Resource.Id.ExportCsvButton);
            m_processDataBtn = FindViewById<Button>(Resource.Id.ProcessSampleButton);
            m_processDataIndexSpinnerStart = FindViewById<Spinner>(Resource.Id.DataIndexSpinnerStart);
            m_processDataIndexSpinnerEnd = FindViewById<Spinner>(Resource.Id.DataIndexSpinnerEnd);

            m_totalSessionsCollectedTextView = FindViewById<TextView>(Resource.Id.TotalSessionsCollected);
            m_totalSessions = m_dataExportManager.GetTotalCollectedSamplesCount();

            m_totalSessionsCollectedTextView.Text = string.Format(m_totalSessionsCollectedTextView.Text, m_totalSessions);
            m_exportToCsvBtn.Click += OnExportToCsvClick;
            m_processDataBtn.Click += OnProcessDataBtnClick;
            SetupSpinners();
        }

        private void SetupSpinners()
        {
            var items = new List<int>();
            for (int i = 1; i <= m_totalSessions; i++)
            {
                items.Add(i);
            }

            ArrayAdapter<int> spinnerAdapter = new ArrayAdapter<int>(this, Resource.Layout.SpinnerItem, Resource.Id.SpinnerTextView, items);
            m_processDataIndexSpinnerStart.Adapter = spinnerAdapter;
            m_processDataIndexSpinnerEnd.Adapter = spinnerAdapter;
        }

        private void OnProcessDataBtnClick(object sender, EventArgs e)
        {
            var selectedIndexStart = m_processDataIndexSpinnerStart.SelectedItem.ToString();
            var selectedIndexEnd = m_processDataIndexSpinnerEnd.SelectedItem.ToString();

            if (int.Parse(selectedIndexStart) > int.Parse(selectedIndexEnd))
            {
                Toast.MakeText(this, $"Starting index has to be lower than ending index!", ToastLength.Short).Show();
                return;
            }

            Intent statsIntent = new Intent(this, typeof(SessionProcessedStatisticsActivity));
            statsIntent.PutExtra(Commons.Constants.Constants.SessionIndexStartTag, selectedIndexStart);
            statsIntent.PutExtra(Commons.Constants.Constants.SessionIndexEndTag, selectedIndexEnd);

            Toast.MakeText(this, $"Processing data for selected session... (from {selectedIndexStart} to {selectedIndexEnd})", ToastLength.Long).Show();

            StartActivity(statsIntent);
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
                    var results = await m_dataExportManager.ExportPerShotDataAsync();

                    if (!results.Any())
                    {
                        Toast.MakeText(this, "No shot detected and exported!", ToastLength.Long).Show();
                    }
                    else if (results.Any(x => !x))
                    {
                        if (results.All(x => !x))
                        {
                            Toast.MakeText(this, "Exports for all shots failed!", ToastLength.Long).Show();
                        }
                        else
                        {
                            Toast.MakeText(this, "Not all shots were exported successfully (some failed)!", ToastLength.Long).Show();
                        }
                    }
                    else
                    {
                        Toast.MakeText(this, $"All shots exported successfully {results.Count} !", ToastLength.Long).Show();
                    }
                })
                .Show();
        }
    }
}