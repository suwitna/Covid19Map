using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Plugin.Geolocator;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Maps;
using Covid19Map.Model;
using Android.Util;

namespace Covid19Map.Droid
{
    [Service(Label= "BackgroundService")]
    class BackgroundService : Service
    {
        int counter = 0;
        bool isRunnningTimer = true;
        public override void OnCreate()
        {
            base.OnCreate();
        }

        public override void OnDestroy()
        {
            Log.Debug("BackgroundService", "OnDestroy");
            StopSelf();
            counter = 0;
            isRunnningTimer = false;
            base.OnDestroy();
            /*
            Log.Debug("BackgroundService", "SendBroadcast(broadcastIntent)");
            Intent broadcastIntent = new Intent(this, typeof(BackgroundReceiver));
            SendBroadcast(broadcastIntent);
            */
        }
        public override IBinder OnBind(Intent intent)
        {
            return null;
        }

        public override StartCommandResult OnStartCommand(Intent intent, StartCommandFlags flags, int startId)
        {
            Device.StartTimer(TimeSpan.FromSeconds(30), () =>
            {
                Device.BeginInvokeOnMainThread(async () =>
                {
                    await AddTrackingLocation();
                    MessagingCenter.Send<string>(counter.ToString(), "counterValue");
                    counter += 1;
                });
                
                return isRunnningTimer;
            });
            return StartCommandResult.Sticky;
        }

        private async Task AddTrackingLocation()
        {
            Log.Debug("BackgroundService", "AddTrackingLocation");
            Log.Debug("BackgroundService", "Start ... ");
            FirebaseHelper firebaseHelper = new FirebaseHelper();
            string seviceId = System.Guid.NewGuid().ToString();
            var android_id = Android.Provider.Settings.Secure.GetString(ContentResolver, Android.Provider.Settings.Secure.AndroidId);

            double zoomMeters = 5000;
            double latitude = 17.3773698;
            double longitude = 104.7608508;

            var locator = CrossGeolocator.Current;
            locator.DesiredAccuracy = 20;

            var position = await locator.GetPositionAsync(TimeSpan.FromTicks(10000));
            CustomMap MyMap = new CustomMap();
            if (MyMap.VisibleRegion != null)
            {
                Distance level = MyMap.VisibleRegion.Radius;
                zoomMeters = level.Meters;

                latitude = position.Latitude;
                longitude = position.Longitude;
            }
            else
            {
                zoomMeters = 5000;
                latitude = 17.3773698;
                longitude = 104.7608508;
            }

            Geocoder geoCoder = new Geocoder();
            var possibleAddresses = await geoCoder.GetAddressesForPositionAsync(new Position(latitude, longitude));
            var placemarks = await Geocoding.GetPlacemarksAsync(latitude, longitude);
            string district = "";
            string province = "";
            string country = "";
            string postalcode = "";

            string address = "";

            foreach (var item in placemarks)
            {
                district = item.SubLocality != null ? item.SubLocality : item.SubAdminArea;
                province = item.AdminArea;
                country = item.CountryName;
                postalcode = item.PostalCode;

                break;
            }

            foreach (var item in possibleAddresses)
            {
                address = item.ToString();
                break;
            }

            MapTracking tracking = new MapTracking();
            tracking.LoginName = android_id;
            tracking.AdressName = address;
            tracking.DistrictName = district;
            tracking.ProvinceName = province;
            tracking.CountryName = country;
            tracking.PostalCode = postalcode;
            tracking.Latitude = latitude;
            tracking.Longitude = longitude;
            tracking.IsActive = "Y";
            tracking.CreateDate = DateTime.Now;
            tracking.UpdateDate = DateTime.Now;

            await firebaseHelper.AddMapTracking(tracking);

            Log.Debug("BackgroundService", "Done ... ");
        }
    }
}