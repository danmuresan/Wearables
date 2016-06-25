using Android.App;
using Android.Content;
using Android.Hardware;
using Android.Runtime;
using SensorRetrieverApp.Helpers;
using Commons.Models;
using System;
using Android.Support.V4.Content;
using Commons.Helpers;
using System.Threading;
using Android.Widget;
using Android.OS;

namespace SensorRetrieverApp
{
    [Service]
    public class SensorRetrieverService : Service, ISensorEventListener
    {
        private SensorManager m_sensorManager;
        private AccelerationManager m_accManager;
        private LocalBroadcastManager m_localBroadcastManager;
        private Acceleration m_lastAccelerationSample;
        private Timer m_updateUiTimer;

        public override void OnCreate()
        {
            Toast.MakeText(this, "Initializing sensor service...", ToastLength.Short).Show();

            m_accManager = new AccelerationManager(this);
            m_sensorManager = (SensorManager)GetSystemService(SensorService);
            m_sensorManager.RegisterListener(this, m_sensorManager.GetDefaultSensor(SensorType.Accelerometer), SensorDelay.Fastest);
            //m_sensorManager.RegisterListener(this, m_sensorManager.GetDefaultSensor(SensorType.Gyroscope), SensorDelay.Fastest);
        }

        public override void OnStart(Intent intent, int startId)
        {
            m_localBroadcastManager = LocalBroadcastManager.GetInstance(this);
            m_updateUiTimer = new Timer(OnTimerTick, null, TimeSpan.FromMilliseconds(600), TimeSpan.FromMilliseconds(600));
            Toast.MakeText(this, "Sensor service started!", ToastLength.Short).Show();
        }

        public override void OnDestroy()
        {
            m_updateUiTimer.Dispose();
            m_updateUiTimer = null;
            m_sensorManager.UnregisterListener(this);
            Toast.MakeText(this, "Sensor service stopped!", ToastLength.Short).Show();

            base.OnDestroy();
        }

        // Unless you provide binding for your service, you don't need to implement this
        // method, because the default implementation returns null. 
        public override IBinder OnBind(Intent intent)
        {
            return null;
        }

        private void OnTimerTick(object state)
        {
            BroadcastUpdateUi(m_lastAccelerationSample);
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
                m_lastAccelerationSample = new Acceleration(x, y, z);
                await m_accManager.RegisterItemAsync(m_lastAccelerationSample);
            }
            //else if (e.Sensor.Type == SensorType.Gyroscope)
            //{
            //    var xRoll = e.Values[0];
            //    var yPitch = e.Values[0];
            //    var zYawn = e.Values[0];
            //    var gyroReading = new Rotation(xRoll, yPitch, zYawn);
            //    // TODO ...
            //}
        }

        private void BroadcastUpdateUi(Acceleration acc)
        {
            Intent intent = new Intent(Commons.Constants.Constants.AccelerationUiUpdateResult);
            if (acc != null)
            {
                intent.PutExtra(Commons.Constants.Constants.AccelerationUiUpdateMessage, acc.GetJsonFromObject());
                m_localBroadcastManager.SendBroadcast(intent);
            }
        }
    }
}