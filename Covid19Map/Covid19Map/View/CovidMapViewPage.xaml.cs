using Plugin.Geolocator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Maps;
using Xamarin.Forms.Xaml;

namespace Covid19Map.View
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CovidMapViewPage : ContentPage
    {
        FirebaseHelper firebaseHelper = new FirebaseHelper();
        bool onInit = false;
        double zoomMeters = 3000;

        public CovidMapViewPage()
        {
            InitializeComponent();
            if (Application.Current.Properties.ContainsKey("USER_NAME"))
            {
                var strLatitude = Application.Current.Properties["USER_LATITUDE"] as string;
                var strLongitude = Application.Current.Properties["USER_LATITUDE"] as string;
                MyMap.MoveToRegion(MapSpan.FromCenterAndRadius(new Position(Convert.ToDouble(strLatitude), Convert.ToDouble(strLongitude))
                                 , Distance.FromMeters(zoomMeters)));
            }

            Device.StartTimer(TimeSpan.FromSeconds(0.01), () =>
            {
                Task.Factory.StartNew(async () =>
                {
                    Device.BeginInvokeOnMainThread(async () =>
                    {
                        await RetreiveLocation();
                    });

                });
                return false;
            });
        }

        
        protected async override void OnAppearing()
        {

            base.OnAppearing();
            var allPersons = await firebaseHelper.GetAllCovidMap();
        }
        private async Task RetreiveLocation()
        {
            if (Application.Current.Properties.ContainsKey("USER_NAME"))
            {
                var username = Application.Current.Properties["USER_NAME"] as string;

                var locator = CrossGeolocator.Current;
                locator.DesiredAccuracy = 20;

                var position = await locator.GetPositionAsync(TimeSpan.FromTicks(10000));

               

                if (MyMap.VisibleRegion != null)
                {
                    Distance level = MyMap.VisibleRegion.Radius;
                    zoomMeters = level.Meters;
                }
                else
                {
                    zoomMeters = 2000;
                }
                MyMap.Pins.Clear();

                MyMap.MoveToRegion(MapSpan.FromCenterAndRadius(new Position(position.Latitude, position.Longitude)
                                  , Distance.FromMeters(zoomMeters)));

                var allPersons = await firebaseHelper.GetAllCovidMap();
                foreach (var item in allPersons)
                {
                    Position pos = new Position(Convert.ToDouble(item.Latitude), Convert.ToDouble(item.Longitude));

                    CustomPin pin = new CustomPin
                    {
                        Type = PinType.Place,
                        Position = pos,
                        Label = item.PinLabel,
                        Address = item.PinAddress,
                    };
                    MyMap.Pins.Add(pin);
                }
            }
        }
    }
}