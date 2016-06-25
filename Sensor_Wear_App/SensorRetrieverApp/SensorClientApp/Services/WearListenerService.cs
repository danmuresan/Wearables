using Android.Gms.Wearable;
using Commons.Constants;
using Commons.Models;
using Commons.Helpers;
using Android.Content;
using Android.Gms.Common.Apis;
using Android.Gms.Common;
using System;
using Android.OS;
using SensorClientApp.Helpers;
using Android.Util;
using Android.Widget;
using Android.App;
using System.Threading;

namespace SensorClientApp.Services
{
    [Service]
    public class WearListenerService : WearableListenerService, GoogleApiClient.IConnectionCallbacks, GoogleApiClient.IOnConnectionFailedListener, IDataListener
    {
        private const int TimeoutInSeconds = 40;

        private GoogleApiClient m_googleApiClient;
        private DataProcessor m_dataProcessor;
        private Timer m_timeoutTimer;
        private Handler m_dispatcher;

        public event EventHandler<IncomingDataEventArgs> NewDataArrived;
        public event EventHandler ClientTimedOut;

        public override void OnStart(Intent intent, int startId)
        {
            base.OnStart(intent, startId);
            m_dispatcher = new Handler();

            m_googleApiClient = new GoogleApiClient.Builder(this)
                .AddApi(WearableClass.API)
                .AddConnectionCallbacks(this)
                .AddOnConnectionFailedListener(this)
                .Build();

            if (!m_googleApiClient.IsConnected)
            {
                m_googleApiClient.Connect();
            }

            // TODO: change this when actually ready for server implementation
            m_dataProcessor = new SerializedDataProcessor(this);
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            m_googleApiClient.Disconnect();
            m_dataProcessor.Dispose();
            m_timeoutTimer.Dispose();
            m_timeoutTimer = null;
        }

        public override void OnDataChanged(DataEventBuffer dataEvents)
        {
            base.OnDataChanged(dataEvents);
            Log.Debug("LISTENER", "New data batch arrived");
            Toast.MakeText(this, "Processing arrived data...", ToastLength.Short).Show();

            foreach (IDataEvent ev in dataEvents)
            {
                var uri = ev.DataItem.Uri;
                var path = uri != null ? uri.Path : null;

                if (Constants.AccDataPath == path)
                {
                    var map = DataMapItem.FromDataItem(ev.DataItem).DataMap;
                    var dataAsString = map.GetString(Constants.AccDataTag);
                    AccelerationBatch data = dataAsString.GetObjectFromJson<AccelerationBatch>();

                    m_timeoutTimer.Change(TimeSpan.FromSeconds(TimeoutInSeconds), Timeout.InfiniteTimeSpan);
                    NewDataArrived?.Invoke(this, new IncomingDataEventArgs(data, dataAsString));
                }
            }
        }

        public void OnConnectionFailed(ConnectionResult result)
        {
            Log.Debug("LISTENER", "Google Api Client did not successfully connect!");
            Toast.MakeText(this, "Client did not successfully connect!", ToastLength.Long).Show();
        }

        public void OnConnected(Bundle connectionHint)
        {
            Log.Debug("LISTENER", "Google Api Client connected successfully!");
            Toast.MakeText(this, "Client connected successfully!", ToastLength.Long).Show();
            WearableClass.DataApi.AddListener(m_googleApiClient, this);
            m_timeoutTimer = new Timer(OnTimeout, null, TimeSpan.FromSeconds(TimeoutInSeconds), Timeout.InfiniteTimeSpan);
        }

        public void OnConnectionSuspended(int cause)
        {
            Log.Debug("LISTENER", "Google Api Client connection suspended!");
            Toast.MakeText(this, "Client connection suspended!", ToastLength.Long).Show();
        }

        private void OnTimeout(object state)
        {
            Log.Debug("LISTENER", "Client timed out.");

            m_dispatcher.Post(() => Toast.MakeText(this, "Client seems to have timed out (hasn't sent any request in some time). We suggest you to stop and restart the listener service or check the sending device!", ToastLength.Long).Show());
            m_timeoutTimer.Change(TimeSpan.FromSeconds(5), Timeout.InfiniteTimeSpan);
            ClientTimedOut?.Invoke(this, EventArgs.Empty);
        }
    }
}