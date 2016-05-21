using System;

using Android.App;
using Android.OS;
using Android.Runtime;
using Android.Widget;
using Android.Hardware;
using SensorRetrieverWearApp.Managers;
using System.Diagnostics;
using Commons.Models;

namespace SensorRetrieverWearApp
{
    [Activity(Label = "SensorActivity")]
    public class SensorActivity : Activity, ISensorEventListener
    {
        private SensorManager m_sensorManager;
        private AccelerationManager m_accManager;
        private Stopwatch m_stopWatch;

        internal TextView XAxisTextView { get; private set; }
        internal TextView YAxisTextView { get; private set; }
        internal TextView ZAxisTextView { get; private set; }
        internal TextView ElapsedTextView { get; private set; }
        internal Button StopSessionBtn { get; private set; }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            SetContentView(Resource.Layout.SensorView);

            XAxisTextView = FindViewById<TextView>(Resource.Id.acc_x);
            YAxisTextView = FindViewById<TextView>(Resource.Id.acc_y);
            ZAxisTextView = FindViewById<TextView>(Resource.Id.acc_z);
            ElapsedTextView = FindViewById<TextView>(Resource.Id.elapsed_time);
            StopSessionBtn = FindViewById<Button>(Resource.Id.stop_session_btn);
            StopSessionBtn.Click += OnStopSessionBtnClick;

            m_accManager = new AccelerationManager(this);
            m_sensorManager = (SensorManager)GetSystemService(SensorService);
            m_sensorManager.RegisterListener(this, m_sensorManager.GetDefaultSensor(SensorType.Accelerometer), SensorDelay.Fastest);

            m_stopWatch = new Stopwatch();
            m_stopWatch.Start();
        }

        private void OnStopSessionBtnClick(object sender, EventArgs e)
        {
            m_stopWatch.Stop();
            m_sensorManager.UnregisterListener(this);
            Finish();
        }

        public void OnAccuracyChanged(Sensor sensor, [GeneratedEnum] SensorStatus accuracy)
        {
            // Do nothing
        }

        public async void OnSensorChanged(SensorEvent e)
        {
            if (e.Sensor.Type == SensorType.Accelerometer)
            {
                var x = e.Values[0];
                var y = e.Values[1];
                var z = e.Values[2];
                var acc = new Acceleration(x, y, z);
                await m_accManager.RegisterItemAsync(acc);
                UpdateUi(acc);
            }
        }

        private void UpdateUi(Acceleration acc)
        {
            XAxisTextView.Text = acc.X.ToString("N1");
            YAxisTextView.Text = acc.Y.ToString("N1");
            ZAxisTextView.Text = acc.Z.ToString("N1");
            ElapsedTextView.Text = $"{m_stopWatch.Elapsed.TotalSeconds} s";
        }
    }
}