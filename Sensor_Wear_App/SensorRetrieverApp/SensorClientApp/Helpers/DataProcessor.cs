using Android.Util;
using SensorClientApp.Services;
using System;

namespace SensorClientApp.Helpers
{
    public abstract class DataProcessor : IDisposable
    {
        public const int ConcatBufferConst = 5;

        private IDataListener m_wearListenerService;

        public DataProcessor(IDataListener listener)
        {
            m_wearListenerService = listener;
            m_wearListenerService.NewDataArrived += OnDataArrived;
            m_wearListenerService.ClientTimedOut += OnTimeout;
        }
        
        protected virtual void OnDataArrived(object sender, IncomingDataEventArgs e)
        {
            Log.Debug("PROCESSOR", "Begin processing new data...");
        }

        protected virtual void OnTimeout(object sender, EventArgs e)
        {
            Log.Debug("PROCESSOR", "Timeout occurred, device hasn't sent us data in some time.");
        }

        public void Dispose()
        {
            m_wearListenerService.NewDataArrived -= OnDataArrived;
            m_wearListenerService.ClientTimedOut -= OnTimeout;
        }
    }
}