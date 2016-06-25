using System;
using SensorClientApp.Helpers;

namespace SensorClientApp.Services
{
    public interface IDataListener
    {
        event EventHandler<IncomingDataEventArgs> NewDataArrived;
        event EventHandler ClientTimedOut;
    }
}