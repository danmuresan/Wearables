using Android.App;
using Android.Content;
using Android.Preferences;
using Android.Util;
using Commons.Helpers;
using Commons.Models;
using Java.Lang;
using System.Collections.Generic;

namespace SensorClientApp.Helpers
{
    public class StorageManager
    {
        public const string DefaultDataTag = "DataBatch";
        public const string SharedPrefsIndexTag = "LastSavedIndex";

        private readonly ISharedPreferences m_sharedPrefs;
        private readonly ISharedPreferencesEditor m_sharedPrefsEditor;

        public StorageManager()
        {
            m_sharedPrefs = PreferenceManager.GetDefaultSharedPreferences(Application.Context);
            m_sharedPrefsEditor = m_sharedPrefs.Edit();
        }

        public void SaveNumber(int number, string tag = SharedPrefsIndexTag)
        {
            m_sharedPrefsEditor.PutInt(tag, number);
            m_sharedPrefsEditor.Apply();
        }

        public int RetrieveNumber(string tag = SharedPrefsIndexTag)
        {
            return m_sharedPrefs.GetInt(tag, 0);
        }

        public void SaveSerializedData(string data, int? dataCount = null)
        {
            var dataTag = dataCount != null ? $"{DefaultDataTag}_{dataCount.ToString()}" : DefaultDataTag;
            m_sharedPrefsEditor.PutString(dataTag, data);
            m_sharedPrefsEditor.Apply();
        }

        public string RetrieveSerializedData(string tag = DefaultDataTag, int? dataCount = null)
        {
            var dataTag = dataCount != null ? $"{DefaultDataTag}_{dataCount.ToString()}" : tag;
            return m_sharedPrefs.GetString(dataTag, string.Empty);
        }

        public List<T> RetrieveAllSerializedData<T>() where T : IDataModel
        {
            List<T> retrievedItems = new List<T>();
            var lastIndex = RetrieveNumber();
            for (int i = 0; i < lastIndex; i++)
            {
                try
                {
                    var serializedDataItem = RetrieveSerializedData(dataCount: i);
                    var dataItemBatch = serializedDataItem.GetObjectListFromJson<T>();
                    foreach (var item in dataItemBatch)
                    {
                        retrievedItems.Add(item);
                    }
                }
                catch (Exception ex)
                {
                    Log.Error("PROCESSOR", ex.ToString());
                }
            }

            return retrievedItems;
        }
    }
}