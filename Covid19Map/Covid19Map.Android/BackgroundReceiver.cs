using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.App;
using Android.Support.V4.Content;
using Android.Util;
using Android.Views;
using Android.Widget;
using Xamarin.Forms;

namespace Covid19Map.Droid
{
    [BroadcastReceiver]
    public class BackgroundReceiver : BroadcastReceiver
    {
        public override void OnReceive(Context context, Intent intent)
        {
            PowerManager pm = (PowerManager)context.GetSystemService(Context.PowerService);
            PowerManager.WakeLock wakeLock = pm.NewWakeLock(WakeLockFlags.Partial, "BackgroundService");
            wakeLock.Acquire();
            //MessagingCenter.Send<object, string>(this, "UpdateLabel", "เพิ่มประวัติการเดินทาง...");

            Log.Info("Received intent!", "Received");
            Log.Debug("Class:BackgroundReceiver", "Method:OnReceive", 0, "Else Called", System.Diagnostics.TraceEventType.Information);
            /*
            if (Build.VERSION.SdkInt >= Build.VERSION_CODES.O)
            {
                context.StartForegroundService(new Intent(context, typeof(BackgroundService)));
            }
            else
            {
                context.StartService(new Intent(context, typeof(BackgroundService)));
            }
            */
            Intent serviceIntent = new Intent(context, typeof(BackgroundService));
            context.StartService(serviceIntent);

            wakeLock.Release();
        }
    }
}