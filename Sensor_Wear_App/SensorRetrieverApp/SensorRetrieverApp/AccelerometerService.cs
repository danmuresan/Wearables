using Android.App;
using Android.Content;
using Android.Hardware;
using Android.Runtime;
using SensorRetrieverApp.Helpers;
using Commons.Models;

namespace SensorRetrieverApp
{
    public class AccelerationService : IntentService, ISensorEventListener
    {
        private SensorManager m_sensorManager;
        private AccelerationManager m_accManager;

        protected override void OnHandleIntent(Intent intent)
        {
            // TODO: ...

            m_accManager = new AccelerationManager(this);
            m_sensorManager = (SensorManager)GetSystemService(SensorService);
            m_sensorManager.RegisterListener(this, m_sensorManager.GetDefaultSensor(SensorType.Accelerometer), SensorDelay.Fastest);
        }

        public void OnAccuracyChanged(Sensor sensor, [GeneratedEnum] SensorStatus accuracy)
        {
        }

        public void OnSensorChanged(SensorEvent e)
        {
            if (e.Sensor.Type == SensorType.Accelerometer)
            {
                var x = e.Values[0];
                var y = e.Values[1];
                var z = e.Values[2];
                var acc = new Acceleration(x, y, z);
                m_accManager.RegisterItemAsync(acc);
                //UpdateUi(acc);
            }
        }
    }
}