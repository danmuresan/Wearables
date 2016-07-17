using System.Threading.Tasks;
using Android.App;
using Android.OS;
using Android.Widget;
using SensorClientApp.Helpers;
using Commons.Models;
using Commons.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SensorClientApp
{
    [Activity(Label = "SessionProcessedStatisticsActivity")]
    public class SessionProcessedStatisticsActivity : Activity
    {
        private TextView m_totalAvgSpeedTextView;
        private TextView m_maxAvgSpeedTextView;
        private TextView m_minAvgSpeedTextView;
        private TextView m_shotsCountTextView;
        private Button m_correlationMetricBtn;

        private StorageManager m_storageManager;
        private int m_sessionIndexStart;
        private int m_sessionIndexEnd;
        private double m_totalAvgVelocity;
        private double m_minAvgVelocity;
        private double m_maxAvgVelocity;
        private int m_shotsCount;
        private List<List<Acceleration>> m_shotsAccelerationsList;

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
            m_correlationMetricBtn = FindViewById<Button>(Resource.Id.CorrelationMetricBtn);

            m_correlationMetricBtn.Click += OnCorrelationMetricBtnClick;
            await LoadDataAsync();
        }

        private void OnCorrelationMetricBtnClick(object sender, EventArgs e)
        {
            var first = m_shotsAccelerationsList[3];
            var second = m_shotsAccelerationsList[5];

            var crossCorrelationCoefficient = DataOperationsUtil.GetAvgCrossCorrelation(first, second);
            var stdDevFirst_XAxis = DataOperationsUtil.GetStandardDeviation(first.Select(x => x.X));
            var stdDevSecond_XAxis = DataOperationsUtil.GetStandardDeviation(second.Select(x => x.X));
            var varianceFirst_XAxis = DataOperationsUtil.GetVariance(first.Select(x => x.X));
            var varianceSecond_XAxis = DataOperationsUtil.GetVariance(second.Select(x => x.X));

            var stdDevFirst_YAxis = DataOperationsUtil.GetStandardDeviation(first.Select(y => y.Y));
            var stdDevSecond_YAxis = DataOperationsUtil.GetStandardDeviation(second.Select(y => y.Y));
            var varianceFirst_YAxis = DataOperationsUtil.GetVariance(first.Select(y => y.Y));
            var varianceSecond_YAxis = DataOperationsUtil.GetVariance(second.Select(y => y.Y));

            var stdDevFirst_ZAxis = DataOperationsUtil.GetStandardDeviation(first.Select(z => z.Z));
            var stdDevSecond_ZAxis = DataOperationsUtil.GetStandardDeviation(second.Select(z => z.Z));
            var varianceFirst_ZAxis = DataOperationsUtil.GetVariance(first.Select(z => z.Z));
            var varianceSecond_ZAxis = DataOperationsUtil.GetVariance(second.Select(z => z.Z));
        }

        private async Task LoadDataAsync()
        {
            await Task.Run(() =>
            {
                var serializedDataBatch = m_storageManager.RetrieveSerializedDataBatch<AccelerationBatch>(m_sessionIndexStart, m_sessionIndexEnd);
                var accelerations = serializedDataBatch.FlattenAccelerationBatches();

                m_shotsAccelerationsList = DataOperationsUtil.ApplyShotsExtractionAlgorithm(accelerations);

                double minVelocity = int.MaxValue;
                double maxVelocity = 0;
                double velocitySum = 0;

                foreach (var shotAcc in m_shotsAccelerationsList)
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

                m_shotsCount = m_shotsAccelerationsList.Count;
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