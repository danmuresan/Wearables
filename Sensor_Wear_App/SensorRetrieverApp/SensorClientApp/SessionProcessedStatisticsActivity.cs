using System.Threading.Tasks;
using Android.App;
using Android.OS;
using Android.Widget;
using SensorClientApp.Helpers;
using Commons.Models;
using Commons.Helpers;
using System;

namespace SensorClientApp
{
    [Activity(Label = "SessionProcessedStatisticsActivity")]
    public class SessionProcessedStatisticsActivity : Activity
    {
        private TextView m_totalAvgSpeedTextView;
        private TextView m_maxAvgSpeedTextView;
        private TextView m_minAvgSpeedTextView;
        private TextView m_shotsCountTextView;

        private StorageManager m_storageManager;
        private int m_sessionIndexStart;
        private int m_sessionIndexEnd;
        private double m_totalAvgVelocity;
        private double m_minAvgVelocity;
        private double m_maxAvgVelocity;
        private int m_shotsCount;

        protected async override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            m_storageManager = new StorageManager();

            // Create your application here
            SetContentView(Resource.Layout.ProcessedStatsLayout);

            if (!int.TryParse(Intent.GetStringExtra(Commons.Constants.Constants.SessionIndexStartTag), out m_sessionIndexStart) ||
                !int.TryParse(Intent.GetStringExtra(Commons.Constants.Constants.SessionIndexEndTag), out m_sessionIndexEnd))
            {
                Toast.MakeText(this, $"Failed to retrieve data for processing...", ToastLength.Long).Show();
            }

            m_totalAvgSpeedTextView = FindViewById<TextView>(Resource.Id.TotalAvgVelocity);
            m_maxAvgSpeedTextView = FindViewById<TextView>(Resource.Id.MaxAvgVelocity);
            m_minAvgSpeedTextView = FindViewById<TextView>(Resource.Id.MinAvgVelocity);
            m_shotsCountTextView = FindViewById<TextView>(Resource.Id.ShotsCount);

            await LoadDataAsync();
        }

        private async Task LoadDataAsync()
        {
            await Task.Run(() =>
            {
                var serializedDataBatch = m_storageManager.RetrieveSerializedDataBatch<AccelerationBatch>(m_sessionIndexStart, m_sessionIndexEnd);
                var accelerations = serializedDataBatch.FlattenAccelerationBatches();

                var shotsAccelerationList = DataOperationsUtil.ApplyShotsExtractionAlgorithm(accelerations);

                double minVelocity = int.MaxValue;
                double maxVelocity = 0;
                double velocitySum = 0;

                foreach (var shotAcc in shotsAccelerationList)
                {
                    var avgVelocity = DataOperationsUtil.GetAvgVelocityForAccelerationBatch(shotAcc);
                    if (avgVelocity > maxVelocity)
                    {
                        maxVelocity = avgVelocity;
                    }

                    if (avgVelocity < minVelocity)
                    {
                        minVelocity = avgVelocity;
                    }

                    velocitySum += avgVelocity;
                }

                m_shotsCount = shotsAccelerationList.Count;
                m_totalAvgVelocity = Math.Round(velocitySum / m_shotsCount, 2, MidpointRounding.AwayFromZero);
                m_minAvgVelocity = minVelocity;
                m_maxAvgVelocity = maxVelocity;
            });

            UpdateView();
        }

        private void UpdateView()
        {
            m_shotsCountTextView.Text = string.Format(m_shotsCountTextView.Text, m_shotsCount);
            m_totalAvgSpeedTextView.Text = string.Format(m_totalAvgSpeedTextView.Text, m_totalAvgVelocity);
            m_maxAvgSpeedTextView.Text = string.Format(m_maxAvgSpeedTextView.Text, m_maxAvgVelocity);
            m_minAvgSpeedTextView.Text = string.Format(m_minAvgSpeedTextView.Text, m_minAvgVelocity);
        }
    }
}