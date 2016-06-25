using System;
using Android.App;
using Android.Hardware;
using Android.OS;
using SensorRetrieverApp.Helpers;
using Android.Widget;
using System.Diagnostics;
using Commons.Models;
using Android.Support.V4.Content;
using Android.Content;
using Commons.Helpers;
using System.Threading.Tasks;
using System.Threading;

namespace SensorRetrieverApp
{
    [Activity(Label = "SensorActivity")]
    public class SensorActivity : Activity
    {
        private Stopwatch m_stopWatch;
        private AccelerometerUiBroadcastReceiver m_broadcastReceiver;
        private Intent m_serviceIntent;

        internal TextView XAxisTextView { get; private set; }
        internal TextView YAxisTextView { get; private set; }
        internal TextView ZAxisTextView { get; private set; }
        internal TextView ElapsedTextView { get; private set; }
        internal Button StopSessionBtn { get; private set; }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            m_broadcastReceiver = new AccelerometerUiBroadcastReceiver(this);
            m_serviceIntent = new Intent(this, typeof(SensorRetrieverService));

            // Create your application here
            SetContentView(Resource.Layout.SensorView);

            XAxisTextView = FindViewById<TextView>(Resource.Id.acc_x);
            YAxisTextView = FindViewById<TextView>(Resource.Id.acc_y);
            ZAxisTextView = FindViewById<TextView>(Resource.Id.acc_z);
            ElapsedTextView = FindViewById<TextView>(Resource.Id.elapsed_time);
            StopSessionBtn = FindViewById<Button>(Resource.Id.stop_session_btn);
            StopSessionBtn.Click += OnStopSessionBtnClick;

            m_stopWatch = new Stopwatch();
        }

        protected async override void OnStart()
        {
            base.OnStart();

            // start the service and ui updater
            Thread t = new Thread(() => {
                StartService(m_serviceIntent);
            });
            t.Start();

            await Task.Delay(2000);

            LocalBroadcastManager.GetInstance(this).RegisterReceiver((m_broadcastReceiver),
                new IntentFilter(Commons.Constants.Constants.AccelerationUiUpdateResult)
            );

            m_stopWatch.Start();
        }

        protected override void OnStop()
        {
            LocalBroadcastManager.GetInstance(this).UnregisterReceiver(m_broadcastReceiver);
            base.OnStop();
        }

        private void OnStopSessionBtnClick(object sender, EventArgs e)
        {
            m_stopWatch.Stop();
            StopService(m_serviceIntent);
            Finish();
        }

        internal void UpdateUi(Acceleration acc)
        {
            XAxisTextView.Text = acc.X.ToString("N2");
            YAxisTextView.Text = acc.Y.ToString("N2");
            ZAxisTextView.Text = acc.Z.ToString("N2");
            ElapsedTextView.Text = $"{(int)m_stopWatch.Elapsed.TotalSeconds} s";
        }
    }

    internal class AccelerometerUiBroadcastReceiver : BroadcastReceiver
    {
        private readonly SensorActivity m_callingActivity;

        public AccelerometerUiBroadcastReceiver(Context caller)
        {
            m_callingActivity = (SensorActivity)caller;
        }

        public override void OnReceive(Context context, Intent intent)
        {
            var serializedLastAcc = intent.GetStringExtra(Commons.Constants.Constants.AccelerationUiUpdateMessage);
            var lastAcc = serializedLastAcc?.GetObjectFromJson<Acceleration>() ?? new Acceleration(0, 0, 0);
            m_callingActivity.UpdateUi(lastAcc);
        }
    }
}