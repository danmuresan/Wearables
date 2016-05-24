using SensorClientApp.Services;
using System;
using System.Threading;

namespace SensorClientApp.Helpers
{
    public abstract class DataProcessor : IDisposable
    {
        public const int ConcatBufferConst = 5;

        private IDataListener m_wearListenerService;

        protected Timer m_timeoutTimer;

        public DataProcessor(IDataListener listener)
        {
            m_wearListenerService = listener;
            m_wearListenerService.NewDataArrived += OnDataArrived;
        }

        protected abstract void OnDataArrived(object sender, IncomingDataEventArgs e);

        public void Dispose()
        {
            m_wearListenerService.NewDataArrived -= OnDataArrived;
        }
    }
}