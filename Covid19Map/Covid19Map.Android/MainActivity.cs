using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Content;
using Xamarin.Forms;
using Android.Util;
using Android.Icu.Util;

namespace Covid19Map.Droid
{
    [Activity(Label = "Covid19Map", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(savedInstanceState);

            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
            global::Xamarin.FormsMaps.Init(this, savedInstanceState);
                                                                        //...


            Plugin.CurrentActivity.CrossCurrentActivity.Current.Activity = this;
            // CrossCurrentActivity.Current.Activity is still NULL

            LoadApplication(new App());

            //StartService(new Intent(this, typeof(PeriodicService)));
            /*
            var alarmIntent = new Intent(this, typeof(BackgroundReceiver));
            var pending = PendingIntent.GetBroadcast(this, 0, alarmIntent, PendingIntentFlags.UpdateCurrent);
            var alarmManager = GetSystemService(AlarmService).JavaCast<AlarmManager>();
            alarmManager.SetInexactRepeating(AlarmType.RtcWakeup, 1000 * 1, 1000 * 1, pending);
            
            MessagingCenter.Unsubscribe<string>(this, "myService");
            MessagingCenter.Subscribe<string>(this, "myService", (value) =>
            {
                if(value=="1")
                {
                    StartService(new Intent(this, typeof(BackgroundService)));
                }
                else
                {
                    StopService(new Intent(this, typeof(BackgroundService)));
                }
            });
            */
            //StartService(new Intent(this, typeof(BackgroundService)));
            //var intent = new Intent(this, typeof(BackgroundReceiver));
            //this.SendBroadcast(intent);
            //BackgroundReceiver catcher = new BackgroundReceiver();
            //RegisterReceiver(catcher, new IntentFilter(Intent.ActionTimeTick));

            //StartService(new Intent(this, typeof(BackgroundReceiver)));
            /*
            var alarmIntent = new Intent(this, typeof(BackgroundReceiver));
            bool isAlarm = (PendingIntent.GetBroadcast(this, 0, alarmIntent, PendingIntentFlags.NoCreate) != null);
            if(isAlarm == false)
            { 
                var pending = PendingIntent.GetBroadcast(this, 0, alarmIntent, 0);
                var alarmManager = GetSystemService(AlarmService).JavaCast<AlarmManager>();
                alarmManager.SetRepeating(AlarmType.ElapsedRealtimeWakeup, SystemClock.ElapsedRealtime(), 1800000, pending);
            }
            */
        }
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }
}