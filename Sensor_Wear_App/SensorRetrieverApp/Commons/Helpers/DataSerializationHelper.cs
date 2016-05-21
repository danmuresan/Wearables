using Commons.Models;
using Newtonsoft.Json;

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
    }
}