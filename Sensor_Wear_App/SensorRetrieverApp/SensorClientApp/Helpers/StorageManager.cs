using Android.App;
using Android.Content;
using Android.Preferences;
using Android.Util;
using Commons.Constants;
using Commons.Helpers;
using Commons.Models;
using System;
using System.Collections.Generic;

namespace SensorClientApp.Helpers
{
    public class StorageManager
    {
        private readonly ISharedPreferences m_sharedPrefs;
        private readonly ISharedPreferencesEditor m_sharedPrefsEditor;

        public StorageManager()
        {
            m_sharedPrefs = PreferenceManager.GetDefaultSharedPreferences(Application.Context);
            m_sharedPrefsEditor = m_sharedPrefs.Edit();
        }

        public void SaveNumber(int number, string tag)
        {
            m_sharedPrefsEditor.PutInt(tag, number);
            m_sharedPrefsEditor.Apply();
        }

        public int RetrieveNumber(string tag, int defaultValue = 0)
        {
            return m_sharedPrefs.GetInt(tag, defaultValue);
        }

        internal void SaveDataIndex(int number)
        {
            SaveNumber(number, Constants.SharedPrefsIndexTag);
        }

        internal int RetrieveDataIndex()
        {
            return RetrieveNumber(Constants.SharedPrefsIndexTag);
        }

        internal void SaveExportIndex(int number)
        {
            SaveNumber(number, Constants.LastExportIndexTag);
        }

        internal int RetrieveExportIndex()
        {
            return RetrieveNumber(Constants.LastExportIndexTag);
        }

        internal void ResetExportIndex()
        {
            SaveNumber(0, Constants.LastExportIndexTag);
        }

        public void SaveSerializedData(string data, int? dataCount = null)
        {
            var dataTag = dataCount != null ? $"{Constants.DefaultDataTag}_{dataCount.ToString()}" : Constants.DefaultDataTag;
            m_sharedPrefsEditor.PutString(dataTag, data);
            m_sharedPrefsEditor.Apply();
        }

        public string RetrieveSerializedData(string tag = Constants.DefaultDataTag, int? dataCount = null)
        {
            var dataTag = dataCount != null ? $"{Constants.DefaultDataTag}_{dataCount.ToString()}" : tag;
            return m_sharedPrefs.GetString(dataTag, string.Empty);
        }

        /// <summary>
        /// This gets the saved data which hasn't been exported yet (say to CSV format)
        /// ex: you export one set of data, you do another listening session and you've got the previous data as well as the new data
        /// => you want to export only the new data
        /// </summary>
        public List<T> RetrieveAllUnexportedSerializedData<T>() where T : IDataModel
        {
            List<T> retrievedItems = new List<T>();
            var lastIndex = RetrieveDataIndex();
            var lastExportIndex = RetrieveExportIndex();

            if (lastExportIndex > lastIndex)
            {
                // something's wrong, fallback to all items
                return RetrieveAllSerializedData<T>();
            }

            for (int i = lastExportIndex; i < lastIndex; i++)
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

        public List<T> RetrieveAllSerializedData<T>() where T : IDataModel
        {
            List<T> retrievedItems = new List<T>();
            var lastIndex = RetrieveDataIndex();
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
                    Log.Error("STORAGE", ex.ToString());
                }
            }

            return retrievedItems;
        }

        public bool ClearAllData()
        {
            try
            {
                m_sharedPrefsEditor.Clear().Commit();
                return true;
            }
            catch (Exception ex)
            {
                Log.Error("STORAGE", ex.ToString());
                return false;
            }
        }
    }
}