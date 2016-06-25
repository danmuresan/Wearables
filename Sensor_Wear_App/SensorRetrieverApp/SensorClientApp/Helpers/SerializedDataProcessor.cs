using SensorClientApp.Services;
using Android.Util;
using System.Threading;
using System;

namespace SensorClientApp.Helpers
{
    public class SerializedDataProcessor : DataProcessor
    {
        private const int TimeoutInSeconds = 30;

        private string m_incomingDataBatches = "[";
        private int m_index = 0;
        private int m_dataCycle;
        private readonly StorageManager m_storageManager;

        public SerializedDataProcessor(IDataListener listener) : base(listener)
        {
            m_storageManager = new StorageManager();
            m_dataCycle = m_storageManager.RetrieveDataIndex();
            m_timeoutTimer = new Timer(OnTimeout, null, TimeSpan.FromSeconds(TimeoutInSeconds), Timeout.InfiniteTimeSpan);
        }

        protected override void OnDataArrived(object sender, IncomingDataEventArgs e)
        {
            Log.Debug("PROCESSOR", "Begin processing new data...");

            m_timeoutTimer.Change(TimeSpan.FromSeconds(TimeoutInSeconds), Timeout.InfiniteTimeSpan);

            if (m_index < ConcatBufferConst)
            {
                m_incomingDataBatches += e.DataAsJson;
                m_incomingDataBatches += ",";
                m_index++;
            }
            else
            {
                Log.Debug("PROCESSOR", "Trying to save data and reset counters...");
                SaveGatheredDataToStorage();
            }
        }

        private void SaveGatheredDataToStorage()
        {
            m_incomingDataBatches = m_incomingDataBatches.Substring(0, m_incomingDataBatches.Length - 1);
            m_incomingDataBatches += "]";
            
            if (m_incomingDataBatches.Length >= 2)
            {
                m_storageManager.SaveSerializedData(m_incomingDataBatches, m_dataCycle);
            }

            m_index = 0;
            m_incomingDataBatches = "[";
            m_dataCycle++;
            m_storageManager.SaveDataIndex(m_dataCycle);
        }

        private void OnTimeout(object state)
        {
            Log.Debug("PROCESSOR", "Timeout occurred, device hasn't sent us data in some time.");
            SaveGatheredDataToStorage();
        }
    }
}