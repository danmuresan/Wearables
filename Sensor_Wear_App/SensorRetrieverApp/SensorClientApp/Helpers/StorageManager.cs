using Android.App;
using Android.Content;
using Android.Preferences;

namespace SensorClientApp.Helpers
{
    public class StorageManager
    {
        public const string DefaultDataTag = "DataBatch";

        private readonly ISharedPreferences m_sharedPrefs;
        private readonly ISharedPreferencesEditor m_sharedPrefsEditor;

        public StorageManager()
        {
            m_sharedPrefs = PreferenceManager.GetDefaultSharedPreferences(Application.Context);
            m_sharedPrefsEditor = m_sharedPrefs.Edit();
        }

        public void SaveSerializedData(string data, int? dataCount = null)
        {
            var dataTag = dataCount != null ? $"{DefaultDataTag}_{dataCount.ToString()}" : DefaultDataTag;
            m_sharedPrefsEditor.PutString(dataTag, data);
            m_sharedPrefsEditor.Apply();
        }

        public string RetrieveSerializedDate(int? dataCount = null)
        {
            var dataTag = dataCount != null ? $"{DefaultDataTag}_{dataCount.ToString()}" : DefaultDataTag;
            return m_sharedPrefs.GetString(dataTag, string.Empty);
        }
    }
}