using Covid19Map.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Plugin.Geolocator;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Maps;
using Xamarin.Forms.Xaml;

namespace Covid19Map.View
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CovidAtNKPPage : ContentPage
    {

        double zoomMeters = 5000;
        double latitude = 17.3773698;
        double longitude = 104.7608508;

        public CovidAtNKPPage()
        {
            InitializeComponent();
            var currentPage = ((NavigationPage)((MasterDetailPage)Application.Current.MainPage).Detail).RootPage;

            if (currentPage.GetType() == typeof(CovidAtNKPPage))
                return;

            popupLoadingView.IsVisible = true;
            activityIndicator.IsRunning = true;

            btnRefresh.Clicked += BtnRefresh_Clicked;
            MyMap.MapClicked += MyMap_MapClicked;

            Device.StartTimer(TimeSpan.FromSeconds(5), () => {
                popupLoadingView.IsVisible = false;
                activityIndicator.IsRunning = false;
                return false;
            });
        }

        private void MyMap_MapClicked(object sender, MapClickedEventArgs e)
        {
            latitude = e.Position.Latitude;
            longitude = e.Position.Longitude;

            MyMap.MoveToRegion(MapSpan.FromCenterAndRadius(new Position(latitude, longitude)
                           , Distance.FromMeters(zoomMeters)));
        }

        private async void BtnRefresh_Clicked(object sender, EventArgs e)
        {
            latitude = 17.3773698;
            longitude = 104.7608508;
            await RetreiveLocation();
        }

        public static async Task RefreshDataAsync()
        {
            var uri = new Uri("https://covid19.th-stat.com/api/open/area");

            var client = new HttpClient();
            var content = await client.GetStringAsync(uri);

            RootObject json = JsonConvert.DeserializeObject<RootObject>(content);
            /*
            var uri = new Uri("https://covid19.th-stat.com/api/open/area");
            HttpClient myClient = new HttpClient();

            var response = await myClient.GetAsync(uri);
            //CovidDataList ObjCovidList = new CovidDataList();
            if (response.IsSuccessStatusCode)
            {
                var settings = new JsonSerializerSettings() { ContractResolver = new CamelCasePropertyNamesContractResolver() };
                var content = await response.Content.ReadAsStringAsync();
                RootObject json = JsonConvert.DeserializeObject<RootObject>(content, settings);

                Console.WriteLine("");
            }
            */
        }

        protected async override void OnAppearing()
        {
            //Task.Run(async () => await RetreiveData());
           // Task.Run(async () => await RetreiveLocation());

            base.OnAppearing();
            await RetreiveLocation();
        }
        private async Task RetreiveData()
        {
            FirebaseHelper firebaseHelper = new FirebaseHelper();
            var allPersons = await firebaseHelper.GetAllCovidMap();

            List<CustomPin> list = new List<CustomPin>();
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
                list.Add(pin);

                
                MyMap.Pins.Add(pin);
            }
            //MyMap.CustomPins = new List<CustomPin>();
            //MyMap.CustomPins = list;
            MyMap.MoveToRegion(MapSpan.FromCenterAndRadius(new Position(latitude, longitude)
                           , Distance.FromMeters(zoomMeters)));
        }

        private async Task RetreiveLocation()
        {

            if (MyMap.VisibleRegion != null)
            {
                Distance level = MyMap.VisibleRegion.Radius;
                zoomMeters = level.Meters;

                latitude = MyMap.VisibleRegion.Center.Latitude;
                longitude = MyMap.VisibleRegion.Center.Longitude;
            }
            else
            {
                zoomMeters = 5000;
                latitude = 17.3773698;
                longitude = 104.7608508;
            }
            MyMap.Pins.Clear();
            FirebaseHelper firebaseHelper = new FirebaseHelper();
            var allPersons = await firebaseHelper.GetAllCovidMap();
            //MyMap.Circles = new List<CustomCircle>();

            if (allPersons != null)
            {
                //MyMap.CustomPins = new List<CustomPin>();
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
                    MyMap.CustomPins.Add(pin);
                    MyMap.Pins.Add(pin);
                    /*
                    CustomCircle circle = new CustomCircle
                    {
                        Position = pos,
                        Radius = 500,
                    };
                    MyMap.Circles.Add(circle);
                    */
                }
            }

            MyMap.MoveToRegion(MapSpan.FromCenterAndRadius(new Position(latitude, longitude)
                           , Distance.FromMeters(zoomMeters)));

        }

        private async Task LocationCircle()
        {

                var username = Application.Current.Properties["USER_NAME"] as string;

                var locator = CrossGeolocator.Current;
                locator.DesiredAccuracy = 20;
                
                var position = await locator.GetPositionAsync(TimeSpan.FromTicks(10000));
                Position pos = new Position(position.Latitude, position.Longitude);

                //MyMap.Circles = new List<CustomCircle>();
                /*
                CustomCircle circle= new CustomCircle
                {
                    Position = pos,
                    Radius = 500,
                };
                MyMap.Circles.Add(circle);
                */
        }

        protected override bool OnBackButtonPressed()
        {
            Device.BeginInvokeOnMainThread(async () => {
                var result = await DisplayAlert("ออกจากแอพพลิเคชั่น", "ท่านกำลังออกจากระบบ โปรดยืนยัน?", "ตกลง", "ยกเลิก");
                if (result)
                {
                    // await this.Navigation.PopAsync(); // or anything else
                    if (Device.RuntimePlatform == Device.Android)
                    {
                        Android.OS.Process.KillProcess(Android.OS.Process.MyPid());
                    }
                }
            });

            return true;
        }

        
    }
}