using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using SensorClientApp.Services;
using Android.Util;

namespace SensorClientApp
{
    [Activity(Label = "SensorClientApp", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        private bool m_isServiceRunning = false;
        private Button m_startStopBtn;
        private Button m_processBtn;
        private Button m_optionsBtn;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            // Get our button from the layout resource,
            // and attach an event to it
            m_startStopBtn = FindViewById<Button>(Resource.Id.MyButton);
            m_processBtn = FindViewById<Button>(Resource.Id.ProcessButton);
            m_optionsBtn = FindViewById<Button>(Resource.Id.OptionsButtonn);

            m_startStopBtn.Click += OnStartSessionClick;
            m_processBtn.Click += OnProcessBtnClick;
            m_optionsBtn.Click += OnOptionsBtnClick;
        }

        private void OnOptionsBtnClick(object sender, EventArgs e)
        {
            // cleanup service just in case it's still running
            if (m_isServiceRunning)
            {
                ToggleListenerService();
            }

            // start another activity
            Intent intent = new Intent(this, typeof(OptionsActivity));
            StartActivity(intent);
        }

        private void OnProcessBtnClick(object sender, EventArgs e)
        {
            // cleanup service just in case it's still running
            if (m_isServiceRunning)
            {
                ToggleListenerService();
            }

            // start another activity
            Intent intent = new Intent(this, typeof(DataProcessorActivity));
            StartActivity(intent);
        }

        private void OnStartSessionClick(object sender, EventArgs e)
        {
            ToggleListenerService();
        }

        private void ToggleListenerService()
        {
            Intent serviceIntent = new Intent(this, typeof(WearListenerService));

            if (!m_isServiceRunning)
            {
                m_startStopBtn.Text = Resources.GetString(Resource.String.StopSession);
                try
                {
                    StartService(serviceIntent);
                    Toast.MakeText(this, "Starting up background listener service...", ToastLength.Long).Show();
                    m_isServiceRunning = true;
                }
                catch (Exception ex)
                {
                    Toast.MakeText(this, "Couldn't set up wear listener service for some reason!", ToastLength.Long).Show();
                    Log.Debug("MAIN", ex.ToString());
                }
            }
            else
            {
                m_startStopBtn.Text = Resources.GetString(Resource.String.StartSession);
                try
                {
                    StopService(serviceIntent);
                    Toast.MakeText(this, "Stopping background listener service...", ToastLength.Long).Show();
                    m_isServiceRunning = false;
                }
                catch (Exception ex)
                {
                    Toast.MakeText(this, "Couldn't propperly terminate wear listener service for some reason!", ToastLength.Long).Show();
                    Log.Debug("MAIN", ex.ToString());
                }
            }
        }
    }
}

