using Android.Content;
using Android.Widget;
using Commons.Helpers;
using Commons.Models;
using System.Threading.Tasks;

namespace SensorRetrieverWearApp.Managers
{
    public class AccelerationManager
    {
        public const short BufferOverflowLength = 1000;

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

            if (m_currentIndex >= BufferOverflowLength)
            {
                await TrySendBatchAsync(m_accBatch);
                m_accBatch = new AccelerationBatch();
                m_currentIndex = 0;
            }

            m_accBatch.Accelerations.Add(acc);
            m_currentIndex++;
        }

        private async Task TrySendBatchAsync(AccelerationBatch accBatch)
        {
            Toast.MakeText(m_ctx, "Sending data batch...", ToastLength.Long).Show();

            await Task.Factory.StartNew(
                () => m_commManager.SendDataRequest(accBatch.GetJsonFromObject())
                );
        }
    }
}