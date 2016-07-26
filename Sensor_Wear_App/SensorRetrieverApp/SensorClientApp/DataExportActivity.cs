using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using SensorClientApp.Helpers;
using System.Threading.Tasks;
using Commons.Filters;

namespace SensorClientApp
{
    [Activity(Label = "DataExportActivity")]
    public class DataExportActivity : Activity
    {
        private DataExportManager m_dataExportManager;

        private Button m_exportAllBtn;
        private Button m_exportShotsRawBtn;
        private Button m_exportWindowedShotsBtn;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            SetContentView(Resource.Layout.DataExportLayout);
            m_dataExportManager = new DataExportManager(this);

            m_exportAllBtn = FindViewById<Button>(Resource.Id.ExportAllBtn);
            m_exportShotsRawBtn = FindViewById<Button>(Resource.Id.ExportShotsRawBtn);
            m_exportWindowedShotsBtn = FindViewById<Button>(Resource.Id.ExportWindowedShotsBtn);


            m_exportAllBtn.Click += OnExportAllClick;
            m_exportShotsRawBtn.Click += OnExportShotsRawClick;
            m_exportWindowedShotsBtn.Click += OnExportWindowedShotsClick;

        }

        private void OnExportWindowedShotsClick(object sender, EventArgs e)
        {
            ShowConfirmationMessage(async () =>
            {
                var results = await m_dataExportManager.ExportPerShotWindowFilteredDataAsync();

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
            });
        }

        private void OnExportShotsRawClick(object sender, EventArgs e)
        {
            ShowConfirmationMessage(async () =>
            {
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
            });
        }

        private void OnExportAllClick(object sender, EventArgs e)
        {
            var filters = new List<FilterType> { FilterType.RollingAverageLowPass };

            ShowConfirmationMessage(async () =>
            {
                var result = await m_dataExportManager.ExportAllRawAsync(filters);
                if (result)
                {
                    Toast.MakeText(this, "CSV export successful!", ToastLength.Long).Show();
                }
                else
                {
                    Toast.MakeText(this, "CSV export failed!", ToastLength.Long).Show();
                }
            });
        }

        private void ShowConfirmationMessage(Func<Task> positiveAction)
        {
            new AlertDialog.Builder(this)
                .SetTitle("Export?")
                .SetMessage("Exported data will be marked as exported")
                .SetPositiveButton("Ok", async (o, ev) => await positiveAction())
                .SetNegativeButton("Cancel", (o, ev) => {  })
                .Show();
        }
    }
}