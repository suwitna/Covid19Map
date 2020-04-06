using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Xamarin.Forms;

namespace Covid19Map.Droid
{
    [Service]
    public class PeriodicService : Service
    {
        public override void OnCreate()
        {
            base.OnCreate();
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
        }
        public override IBinder OnBind(Intent intent)
        {
            return null;
        }

        public override StartCommandResult OnStartCommand(Intent intent, StartCommandFlags flags, int startId)
        {
            MessagingCenter.Send<object, string>(this, "UpdateLabel", "บันทึกข้อมูลการเดินทางสำเร็จ!");
            return StartCommandResult.Sticky;
        }


    }
}