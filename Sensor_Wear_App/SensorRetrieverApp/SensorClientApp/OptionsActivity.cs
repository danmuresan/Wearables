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

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            m_storageManager = new Helpers.StorageManager();
            SetContentView(Resource.Layout.OptionsLayout);

            m_clearOptionBtn = FindViewById<Button>(Resource.Id.ClearDataButton);

            m_clearOptionBtn.Click += OnClearOptionBtnClick;
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