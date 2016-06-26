using System;
using Android.App;
using Android.OS;
using Android.Widget;
using Android.Content;
using SensorClientApp.Helpers;

namespace SensorClientApp
{
    [Activity(Label = "OptionsActivity")]
    public class OptionsActivity : Activity
    {
        private StorageManager m_storageManager;
        private DataExportManager m_dataExportManager;
        private Button m_clearOptionBtn;
        private Button m_resetCollectedDataIndexBtn;
        private Button m_sendLogsBtn;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            m_storageManager = new StorageManager();
            m_dataExportManager = new DataExportManager(this);
            SetContentView(Resource.Layout.OptionsLayout);

            m_clearOptionBtn = FindViewById<Button>(Resource.Id.ClearDataButton);
            m_resetCollectedDataIndexBtn = FindViewById<Button>(Resource.Id.ResetCollectedDataIndex);
            m_sendLogsBtn = FindViewById<Button>(Resource.Id.SendLogsButton);

            m_clearOptionBtn.Click += OnClearOptionBtnClick;
            m_resetCollectedDataIndexBtn.Click += OnResetIndexClick;
            m_sendLogsBtn.Click += OnSendLogsClick;
        }

        private async void OnSendLogsClick(object sender, EventArgs e)
        {
            var logsExported = await m_dataExportManager.ExportLogsAsync();
            if (logsExported)
            {
                Toast.MakeText(this, "Logs exported successfully!", ToastLength.Long).Show();
            }
            else
            {
                Toast.MakeText(this, "Log export failed!", ToastLength.Long).Show();
            }
        }

        private void OnResetIndexClick(object sender, EventArgs e)
        {
            new AlertDialog.Builder(this)
                .SetTitle("Mark all data as unexported / unprocessed ?")
                .SetMessage("Are you sure you want to mark all existing data as not previously exported or processed?")
                .SetPositiveButton(Resource.String.Yes, (o, ev) => {
                    m_storageManager.ResetExportIndex();
                    Toast.MakeText(this, "Index reseted successfully!", ToastLength.Long).Show();
                })
                .SetNegativeButton(Resource.String.No, (IDialogInterfaceOnClickListener)null)
                .Show();
        }

        private void OnClearOptionBtnClick(object sender, EventArgs e)
        {
            new AlertDialog.Builder(this)
                .SetTitle("Delete all data")
                .SetMessage("Are you sure you want to wipe out all previously gathered data?")
                .SetPositiveButton(Resource.String.Yes, (o, ev) => {
                    var cleared = m_storageManager.ClearAllData();
                    if (cleared) Toast.MakeText(this, "Successfully cleared all data!", ToastLength.Long).Show();
                    else Toast.MakeText(this, "Failed to clear the data!", ToastLength.Long).Show();
                })
                .SetNegativeButton(Resource.String.No, (IDialogInterfaceOnClickListener)null)
                .Show();
        }
    }
}