using SensorClientApp.Services;
using Android.Util;
using System;

namespace SensorClientApp.Helpers
{
    public sealed class SerializedDataProcessor : DataProcessor
    {
        private string m_incomingDataBatches = "[";
        private int m_index = 0;
        private int m_dataCycle;
        private readonly StorageManager m_storageManager;

        public SerializedDataProcessor(IDataListener listener) : base(listener)
        {
            m_storageManager = new StorageManager();
            m_dataCycle = m_storageManager.RetrieveDataIndex();
        }

        protected override void OnDataArrived(object sender, IncomingDataEventArgs e)
        {
            base.OnDataArrived(sender, e);
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

        protected override void OnTimeout(object sender, EventArgs e)
        {
            base.OnTimeout(sender, e);
            SaveGatheredDataToStorage();
        }
    }
}