using Android.Content;
using Android.Gms.Common.Apis;
using Android.Gms.Wearable;
using System;
using Android.OS;
using Android.Gms.Common;
using Commons.Constants;

namespace SensorRetrieverApp.Helpers
{
    public class CommunicationManager : Java.Lang.Object, IGoogleApiClientConnectionCallbacks, IGoogleApiClientOnConnectionFailedListener
    {
        private IGoogleApiClient m_googleApiClient;

        public CommunicationManager(Context ctx)
        {
            m_googleApiClient = new GoogleApiClientBuilder(ctx)
                .AddApi(WearableClass.Api)
                .AddConnectionCallbacks(this)
                .AddOnConnectionFailedListener(this)
                .Build();

            m_googleApiClient.Connect();
        }

        public void OnConnected(Bundle p0)
        {
            //
        }


        public void OnConnectionFailed(ConnectionResult p0)
        {
            //
        }

        public void OnConnectionSuspended(int p0)
        {
            //
        }

        public bool SendDataRequest(string dataAsJson)
        {
            PutDataMapRequest dataMap = PutDataMapRequest.Create(Constants.AccDataPath);
            var data = dataMap.DataMap;
            data.PutString(Constants.AccDataTag, dataAsJson);
            PutDataRequest request = dataMap.AsPutDataRequest();

            try
            {
                var pendingResult = WearableClass.DataApi.PutDataItem(m_googleApiClient, request);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}