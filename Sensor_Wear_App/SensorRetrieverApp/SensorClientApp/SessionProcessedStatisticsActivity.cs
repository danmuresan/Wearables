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
        private TextView m_maxCorrelationTextView;
        private TextView m_avgCorrelationTextView;
        private TextView m_minStdDevTextView;
        private TextView m_avgStdDevTextView;
        private TextView m_minVarianceTextView;
        private TextView m_avgVarianceTextView;
        private TextView m_pitchAngleTextView;
        private TextView m_rollAngleTextView;
        private TextView m_yawAngleTextView;
        private TextView m_avgShotDuration;
        private TextView m_minShotDuration;
        private TextView m_maxShotDuration;
        private TextView m_avgShotAggressivity;
        private TextView m_minShotAggressivity;
        private TextView m_maxShotAggressivity;
        private Button m_computeDurationBtn;
        private Button m_computeAnglesBtn;
        private ProgressDialog m_progress;

        private StorageManager m_storageManager;
        private int m_sessionIndexStart;
        private int m_sessionIndexEnd;
        private double m_totalAvgVelocity;
        private double m_minAvgVelocity;
        private double m_maxAvgVelocity;
        private int m_shotsCount;
        private List<List<Acceleration>> m_shotsAccelerationsList;
        private List<SimilarityMetrics> m_allSimilarityMetrics;

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
            m_avgCorrelationTextView = FindViewById<TextView>(Resource.Id.AvgCorrelationCoefficient);
            m_maxCorrelationTextView = FindViewById<TextView>(Resource.Id.MaxCorrelationCoefficient);
            m_minStdDevTextView = FindViewById<TextView>(Resource.Id.MinStdDevDiff);
            m_minVarianceTextView = FindViewById<TextView>(Resource.Id.MinVarianceDiff);
            m_avgStdDevTextView = FindViewById<TextView>(Resource.Id.AvgStdDevDiff);
            m_avgVarianceTextView = FindViewById<TextView>(Resource.Id.AvgVarianceDiff);
            m_computeAnglesBtn = FindViewById<Button>(Resource.Id.ComputeAnglesBtn);
            m_pitchAngleTextView = FindViewById<TextView>(Resource.Id.PitchAngle);
            m_rollAngleTextView = FindViewById<TextView>(Resource.Id.RollAngle);
            m_yawAngleTextView = FindViewById<TextView>(Resource.Id.YawAngle);
            m_avgShotDuration = FindViewById<TextView>(Resource.Id.AvgDuration);
            m_minShotDuration = FindViewById<TextView>(Resource.Id.MinDuration);
            m_maxShotDuration = FindViewById<TextView>(Resource.Id.MaxDuration);
            m_avgShotAggressivity = FindViewById<TextView>(Resource.Id.AvgAgresivity);
            m_minShotAggressivity = FindViewById<TextView>(Resource.Id.MinAgresivity);
            m_maxShotAggressivity = FindViewById<TextView>(Resource.Id.MaxAgresivity);
            m_computeDurationBtn = FindViewById<Button>(Resource.Id.ComputeDurationBtn);

            m_correlationMetricBtn.Click += OnCorrelationMetricBtnClick;
            m_computeAnglesBtn.Click += OnComputeAnglesBtnClick;
            m_computeDurationBtn.Click += OnComputeDurationBtnClick;
            m_allSimilarityMetrics = new List<SimilarityMetrics>();

            InitializeProgressDialog();
            await LoadDataAsync();
        }

        private async void OnComputeDurationBtnClick(object sender, EventArgs e)
        {
            InitializeProgressDialog();
            int maxDurationCount = 0;
            int minDurationCount = 1000;
            int sumDurations = 0;
            int avgDuration = 0;
            int minAggressivity = int.MaxValue;
            int maxAggressivity = 0;
            int sumAggressivity = 0;
            int avgAggressivity = 0;

            await Task.Run(() => {
                foreach (var shot in m_shotsAccelerationsList)
                {
                    int shotDurationCount = 0;

                    // filter with Window Filter
                    var windowFilteredShot = DataOperationsUtil.ApplyWindowFilterOnAccelerationBatch(shot);

                    // apply threshold
                    var windowFilteredXAxis = windowFilteredShot.Select(x => x.X).ToList();
                    var windowFilteredYAxis = windowFilteredShot.Select(y => y.Y).ToList();
                    var windowFilteredZAxis = windowFilteredShot.Select(z => z.Z).ToList();

                    var medianValX = DataOperationsUtil.GetAvgValueRaw(windowFilteredXAxis);
                    var medianValY = DataOperationsUtil.GetAvgValueRaw(windowFilteredYAxis);
                    var medianValZ = DataOperationsUtil.GetAvgValueRaw(windowFilteredZAxis);

                    var maxValX = DataOperationsUtil.GetMaxValueRaw(windowFilteredXAxis);
                    var maxValY = DataOperationsUtil.GetMaxValueRaw(windowFilteredYAxis);
                    var maxValZ = DataOperationsUtil.GetMaxValueRaw(windowFilteredZAxis);
                    var aggressivityValForCurrentShot = (int)(maxValX + maxValY + maxValZ) / 3;

                    for (int i = 0; i < windowFilteredXAxis.Count(); i++)
                    {
                        if (windowFilteredXAxis[i] > medianValX && windowFilteredYAxis[i] > medianValY && windowFilteredZAxis[i] > medianValZ)
                        {
                            shotDurationCount++;
                        }
                    }

                    if (shotDurationCount > maxDurationCount)
                    {
                        maxDurationCount = shotDurationCount;
                    }

                    if (shotDurationCount < minDurationCount)
                    {
                        minDurationCount = shotDurationCount;
                    }

                    if (aggressivityValForCurrentShot > maxAggressivity)
                    {
                        maxAggressivity = aggressivityValForCurrentShot;
                    }

                    if (aggressivityValForCurrentShot < minAggressivity)
                    {
                        minAggressivity = aggressivityValForCurrentShot;
                    }

                    sumDurations += shotDurationCount;
                    sumAggressivity += aggressivityValForCurrentShot;
                }

                avgDuration = sumDurations / m_shotsAccelerationsList.Count();
                avgAggressivity = sumAggressivity / m_shotsAccelerationsList.Count();
            });

            const int millisPerSample = 7;
            const double secondsPerSample = (millisPerSample / 1000.0);

            m_maxShotDuration.Text = string.Format(m_maxShotDuration.Text, Math.Round(maxDurationCount * secondsPerSample , 3));
            m_minShotDuration.Text = string.Format(m_minShotDuration.Text, Math.Round(minDurationCount * secondsPerSample, 3));
            m_avgShotDuration.Text = string.Format(m_avgShotDuration.Text, Math.Round(avgDuration * secondsPerSample, 3));
            m_maxShotAggressivity.Text = string.Format(m_maxShotAggressivity.Text, maxAggressivity);
            m_minShotAggressivity.Text = string.Format(m_minShotAggressivity.Text, minAggressivity);
            m_avgShotAggressivity.Text = string.Format(m_avgShotAggressivity.Text, avgAggressivity);
            m_minShotDuration.Visibility = Android.Views.ViewStates.Visible;
            m_maxShotDuration.Visibility = Android.Views.ViewStates.Visible;
            m_avgShotDuration.Visibility = Android.Views.ViewStates.Visible;
            m_maxShotAggressivity.Visibility = Android.Views.ViewStates.Visible;
            m_minShotAggressivity.Visibility = Android.Views.ViewStates.Visible;
            m_avgShotAggressivity.Visibility = Android.Views.ViewStates.Visible;

            HideProgressDialog();
        }

        private async void OnComputeAnglesBtnClick(object sender, EventArgs e)
        {
            double pitchAlphaAngle = 0;
            double rollBetaAngle = 0;
            double yawGammaAngle = 0;

            InitializeProgressDialog();
            Toast.MakeText(this, "Feature not yet available...", ToastLength.Long).Show();
            //await Task.Run(() => {

            //    // Todo...

            //});
            HideProgressDialog();
        }

        private void InitializeProgressDialog()
        {
            m_progress = new ProgressDialog(this);
            m_progress.Indeterminate = true;
            m_progress.SetProgressStyle(ProgressDialogStyle.Spinner);

            m_progress.SetMessage("Processing data. Please wait a bit...");
            m_progress.SetCancelable(false);
            m_progress.Show();
        }

        private void HideProgressDialog()
        {
            m_progress.Hide();
        }

        private async void OnCorrelationMetricBtnClick(object sender, EventArgs e)
        {
            InitializeProgressDialog();
            double sumCorr = 0;
            double maxCorr = 0;
            double sumStdDevDiff = 0;
            double minStdDevDiff = int.MaxValue;
            double sumVarianceDiff = 0;
            double minVarianceDiff = int.MaxValue;
            int stdDevCount = 0;

            await Task.Run(() =>
            {
                for (int shotIdx = 0; shotIdx < m_shotsAccelerationsList.Count() - 2; shotIdx++)
                {
                    var first = m_shotsAccelerationsList[shotIdx];
                    var second = m_shotsAccelerationsList[shotIdx + 1];

                    var firstNormalized = DataFilteringHelper.NormalizeAccelerationBatch(first).ToList();
                    var secondNormalized = DataFilteringHelper.NormalizeAccelerationBatch(second).ToList();

                    SimilarityMetrics similarityMetrics = new SimilarityMetrics(firstNormalized, secondNormalized);
                    similarityMetrics.ComputeCrossCorrelationCoefficient();

                    // ?? Experiment with different thresholds
                    if (similarityMetrics.CorrelationCoefficient >= 0.4)
                    {
                        // for those shots that kinda resemble at places, compute other metrics as well
                        similarityMetrics.ComputeStandardDeviationDifference();
                        similarityMetrics.ComputeVarianceDifference();

                        var allAxisAvgStdDev = (similarityMetrics.StandardDeviationDifference_XAxis + similarityMetrics.StandardDeviationDifference_YAxis + similarityMetrics.StandardDeviationDifference_ZAxis) / 3.0;
                        var allAxisVarianceDiff = (similarityMetrics.VarianceDifference_XAxis + similarityMetrics.VarianceDifference_YAxis + similarityMetrics.VarianceDifference_ZAxis) / 3.0;
                        sumStdDevDiff += allAxisAvgStdDev;
                        sumVarianceDiff += allAxisVarianceDiff;
                        if (allAxisAvgStdDev < minStdDevDiff)
                        {
                            minStdDevDiff = allAxisAvgStdDev;
                        }
                        if (allAxisVarianceDiff < minVarianceDiff)
                        {
                            minVarianceDiff = allAxisVarianceDiff;
                        }

                        stdDevCount++;

                    }

                    // save off for future use ??
                    m_allSimilarityMetrics.Add(similarityMetrics);
                    sumCorr += similarityMetrics.CorrelationCoefficient;

                    if (similarityMetrics.CorrelationCoefficient > maxCorr)
                    {
                        maxCorr = similarityMetrics.CorrelationCoefficient;
                    }
                }
            });

            double avgCorr = sumCorr / (m_shotsAccelerationsList.Count() - 2);
            double avgStdDevDiff = sumStdDevDiff / stdDevCount;
            double avgVarianceDiff = sumVarianceDiff / stdDevCount;

            m_avgCorrelationTextView.Text = string.Format(m_avgCorrelationTextView.Text, Math.Round(avgCorr, 2));
            m_maxCorrelationTextView.Text = string.Format(m_maxCorrelationTextView.Text, Math.Round(maxCorr, 2));
            m_avgStdDevTextView.Text = string.Format(m_avgStdDevTextView.Text, Math.Round(avgStdDevDiff, 2));
            m_minStdDevTextView.Text = string.Format(m_minStdDevTextView.Text, Math.Round(minStdDevDiff, 2));
            m_avgVarianceTextView.Text = string.Format(m_avgVarianceTextView.Text, Math.Round(avgVarianceDiff, 2));
            m_minVarianceTextView.Text = string.Format(m_minVarianceTextView.Text, Math.Round(minVarianceDiff, 2));

            m_avgCorrelationTextView.Visibility = Android.Views.ViewStates.Visible;
            m_maxCorrelationTextView.Visibility = Android.Views.ViewStates.Visible;
            m_avgStdDevTextView.Visibility = Android.Views.ViewStates.Visible;
            m_avgVarianceTextView.Visibility = Android.Views.ViewStates.Visible;
            m_minStdDevTextView.Visibility = Android.Views.ViewStates.Visible;
            m_minVarianceTextView.Visibility = Android.Views.ViewStates.Visible;

            HideProgressDialog();
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
            HideProgressDialog();
        }
    }
}