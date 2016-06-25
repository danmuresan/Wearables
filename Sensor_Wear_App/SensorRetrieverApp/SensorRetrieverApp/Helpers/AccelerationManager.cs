using Android.Widget;
using Android.Content;
using System.Threading.Tasks;
using Commons.Models;
using Commons.Helpers;
using Commons.Constants;

namespace SensorRetrieverApp.Helpers
{
    public class AccelerationManager
    {
        private readonly Context m_ctx;
        private int m_currentIndex = 0;
        private AccelerationBatch m_accBatch;
        private CommunicationManager m_commManager;

        public AccelerationManager(Context ctx)
        {
            m_ctx = ctx;
            m_commManager = new CommunicationManager(ctx);
        }

        public async Task RegisterItemAsync(Acceleration acc)
        {
            if (m_currentIndex == 0)
            {
                m_accBatch = new AccelerationBatch();
            }

            if (m_currentIndex >= Constants.BufferOverflowLength)
            {
                await TrySendDataBatch(m_accBatch);
                m_accBatch = new AccelerationBatch();
                m_currentIndex = 0;
            }

            m_accBatch.Accelerations.Add(acc);
            m_currentIndex++;
        }

        private async Task TrySendDataBatch(AccelerationBatch accBatch)
        {
            var result = m_commManager.SendDataRequest(accBatch.GetJsonFromObject());
            //await Task.Run(() => m_commManager.SendDataRequest(accBatch.GetJsonFromObject()));

            if (result)
            {
                Toast.MakeText(m_ctx, "Data batch sent successfully!", ToastLength.Short).Show();
            }
            else
            {
                Toast.MakeText(m_ctx, "Data batch failed to send!", ToastLength.Short).Show();
            }
        }
    }
}