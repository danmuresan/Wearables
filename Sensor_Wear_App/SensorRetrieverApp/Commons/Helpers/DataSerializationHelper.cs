using Android.Util;
using Commons.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Commons.Helpers
{
    public static class DataSerializationHelper
    {
        public static string GetJsonFromObject<T>(this T dataObject) where T : IDataModel
        {
            return JsonConvert.SerializeObject(dataObject);
        }

        public static T GetObjectFromJson<T>(this string dataAsJson) where T : IDataModel
        {
            return JsonConvert.DeserializeObject<T>(dataAsJson);
        }

        public static IEnumerable<T> GetObjectListFromJson<T>(this string dataAsJson) where T : IDataModel
        {
            IEnumerable<T> deserializedObject;
            try
            {
                deserializedObject = JsonConvert.DeserializeObject<IEnumerable<T>>(dataAsJson);
            }
            catch (Exception ex)
            {
                Log.Error("SERIALIZER", ex.ToString());
                return Enumerable.Empty<T>();
            }

            return deserializedObject;
        }
    }
}