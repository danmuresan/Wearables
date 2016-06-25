using Android.Content;
using Android.Gms.Common.Apis;
using Android.Gms.Wearable;
using System;
using Android.OS;
using Android.Gms.Common;
using Commons.Constants;
using Android.Util;

namespace SensorRetrieverApp.Helpers
{
    public class CommunicationManager : Java.Lang.Object, GoogleApiClient.IConnectionCallbacks, GoogleApiClient.IOnConnectionFailedListener/*, IDataApiDataListener*/
    {
        private GoogleApiClient m_googleApiClient;

        public CommunicationManager(Context ctx)
        {
            m_googleApiClient = new GoogleApiClient.Builder(ctx)
                .AddApi(WearableClass.API)
                .AddConnectionCallbacks(this)
                .AddOnConnectionFailedListener(this)
                .Build();

            m_googleApiClient.Connect();
        }

        public void OnConnected(Bundle p0)
        {
            Log.Debug("LISTENER", "Google Api Client connected successfully!");
        }

        public void OnConnectionFailed(ConnectionResult p0)
        {
            Log.Debug("LISTENER", "Google Api Client did not successfully connect!");
        }

        public void OnConnectionSuspended(int p0)
        {
            Log.Debug("LISTENER", "Google Api Client connection suspended!");
        }

        public void OnDataChanged(DataEventBuffer dataEvents)
        {
        }

        public bool SendDataRequest(string dataAsJson)
        {
            Log.Debug("SENDER", "Preparing data request for sending...");

            PutDataMapRequest dataMap = PutDataMapRequest.Create(Constants.AccDataPath);
            var data = dataMap.DataMap;
            data.PutString(Constants.AccDataTag, dataAsJson);
            data.PutLong(Constants.TimeStampTag, DateTime.Now.Ticks);
            PutDataRequest request = dataMap.AsPutDataRequest();
            request.SetUrgent();

            try
            {
                var pendingResult = WearableClass.DataApi.PutDataItem(m_googleApiClient, request);
                //WearableClass.DataApi.AddListener(m_googleApiClient, this);
                return true;
            }
            catch (Exception ex)
            {
                Log.Debug("SENDER", ex.ToString());
                return false;
            }
        }
    }
}