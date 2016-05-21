using System;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Support.Wearable.Views;
using Android.Support.V4.App;
using Android.Support.V4.View;
using Java.Interop;
using Android.Views.Animations;

namespace SensorRetrieverWearApp
{
    [Activity(Label = "SensorRetrieverWearApp", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            var v = FindViewById<WatchViewStub>(Resource.Id.watch_view_stub);
            v.LayoutInflated += delegate
            {
                // Get our button from the layout resource,
                // and attach an event to it
                Button button = FindViewById<Button>(Resource.Id.myButton);

                int notificationId = 001;
                Intent viewIntent = new Intent(this, typeof(SensorActivity));
                PendingIntent viewPendingIntent =
                        PendingIntent.GetActivity(this, 0, viewIntent, 0);

                button.Click += (sender, args) =>
                {
                    var notification = new NotificationCompat.Builder(this)
                        .SetContentTitle("Session started")
                        .SetContentText("Registering sensor data...")
                        .SetSmallIcon(Android.Resource.Drawable.AlertLightFrame)
                        .SetContentIntent(viewPendingIntent)
                        .AddAction(Resource.Drawable.generic_confirmation_00163, "", viewPendingIntent);

                    var manager = NotificationManagerCompat.From(this);
                    manager.Notify(1, notification.Build());

                    StartActivity(viewIntent);
                };
            };
        }
    }
}


