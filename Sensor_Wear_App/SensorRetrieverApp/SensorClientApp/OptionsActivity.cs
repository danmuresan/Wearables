using System;
using Android.App;
using Android.OS;
using Android.Widget;
using Android.Content;

namespace SensorClientApp
{
    [Activity(Label = "OptionsActivity")]
    public class OptionsActivity : Activity
    {
        private Helpers.StorageManager m_storageManager;
        private Button m_clearOptionBtn;
        private Button m_resetCollectedDataIndex;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            m_storageManager = new Helpers.StorageManager();
            SetContentView(Resource.Layout.OptionsLayout);

            m_clearOptionBtn = FindViewById<Button>(Resource.Id.ClearDataButton);
            m_resetCollectedDataIndex = FindViewById<Button>(Resource.Id.ResetCollectedDataIndex);

            m_clearOptionBtn.Click += OnClearOptionBtnClick;
            m_resetCollectedDataIndex.Click += OnResetIndexClick;
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