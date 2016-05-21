using Android.App;
using Android.Gms.Wearable;
using Android.Gms.Common.Data;
using Commons.Constants;
using Commons.Models;
using Commons.Helpers;
using Android.Widget;

namespace SensorClientApp.Services
{
    public class WearListenerService : WearableListenerService
    {
        public override void OnDataChanged(DataEventBuffer dataEvents)
        {
            base.OnDataChanged(dataEvents);

            var events = FreezableUtils.FreezeIterable(dataEvents);
            foreach (IDataEvent ev in events)
            {
                var uri = ev.DataItem.Uri;
                var path = uri != null ? uri.Path : null;

                if (Constants.AccDataPath == path)
                {
                    var map = DataMapItem.FromDataItem(ev.DataItem).DataMap;
                    var dataAsString = map.GetString(Constants.AccDataTag);
                    AccelerationBatch data = dataAsString.GetObjectFromJson<AccelerationBatch>();
                    Toast.MakeText(Application.Context, $"Retrieved {data.Accelerations.Count} more objects...", ToastLength.Short);
                }
            }
        }
    }

    //public class WearListenerService : WearableListenerService, GoogleApiClient.IConnectionCallbacks, GoogleApiClient.IOnConnectionFailedListener
    //{
    //    private const string WearAccDataTag = "/accelerations";

    //    private GoogleApiClient m_googleClientApi;
    //    private Timer m_dataRequestTimer;

    //    public void OnConnected(Bundle connectionHint)
    //    {
    //    }

    //    private void PollForNewData(object state)
    //    {
    //        if (m_googleClientApi.IsConnected)
    //        {
    //            var putDataMapRequest = PutDataMapRequest.Create(WearAccDataTag);
    //            var dataMap = putDataMapRequest.DataMap;

    //        }
    //    }

    //    public void OnConnectionFailed(ConnectionResult result)
    //    {
    //    }

    //    public void OnConnectionSuspended(int cause)
    //    {
    //    }

    //    protected override void OnHandleIntent(Intent intent)
    //    {
    //        // TODO...
    //    }

    //    public override void OnStart(Intent intent, int startId)
    //    {
    //        base.OnStart(intent, startId);

    //        m_googleClientApi = new GoogleApiClient.Builder(this)
    //            .AddConnectionCallbacks(this)
    //            .AddOnConnectionFailedListener(this)
    //            .AddApi(WearableClass.API)
    //            .Build();

    //        m_googleClientApi.Connect();
    //    }

    //    public override void OnDestroy()
    //    {
    //        base.OnDestroy();
    //        m_googleClientApi.Disconnect();
    //    }
    //}
}