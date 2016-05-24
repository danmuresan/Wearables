using Commons.Models;
using System;

namespace SensorClientApp.Helpers
{
    public class IncomingDataEventArgs : EventArgs
    {
        public IDataModel Data { get; }

        public string DataAsJson { get; }

        public IncomingDataEventArgs(IDataModel model, string dataAsJson = null)
        {
            Data = model;
            DataAsJson = dataAsJson ?? string.Empty;
        }
    }
}