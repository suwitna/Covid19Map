using Covid19Map.MenuItems;
using Covid19Map.Model;
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
    public partial class CovidMapAddPage : ContentPage
    {
        FirebaseHelper firebaseHelper = new FirebaseHelper();
        bool onInit = false;
        double zoomMeters = 5000;
        double latitude = 0;
        double longitude = 0;
        public CovidMapAddPage()
        {
            InitializeComponent();
            MyMap.MapClicked += MyMap_MapClicked;
            ToolbarSave.Clicked += ToolbarSave_Clicked;
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

        private async void ToolbarSave_Clicked(object sender, EventArgs e)
        {
            var result = await DisplayAlert(null, "บันทึกข้อมูลจุดที่พบผู้ป่วย COVID-19?", "บันทึก", "ยกเลิก");
            if (result)
            {
                SaveCovid19Map();
                List<Page> li = Navigation.NavigationStack.ToList();
                Page last = li.ElementAt(li.Count - 1);
                Navigation.RemovePage(last);

                return;
            }
            else
            {
                //Cancel
                return;
            }
        }

        private void SaveCovid19Map()
        {
            CovidMap covid = new CovidMap();
            covid.Latitude = latitude.ToString();
            covid.Longitude = longitude.ToString();
            covid.PinLabel = txtLabel.Text;
            covid.PinAddress = txtAddress.Text;
            covid.FoundDate = FoundDate.Date.ToString("dd/MM/yyyy");
            covid.IsActive = "Y";
            covid.CreateDate = DateTime.Now;
            covid.UpdateDate = DateTime.Now;

            AddCovidMap(covid);
        }

        private async void AddCovidMap(CovidMap covid)
        {
            await firebaseHelper.AddCovidMap(covid);
            //await DisplayAlert("Success", "Person Added Successfully", "OK");
            Console.WriteLine("Map history added Successfully");
            // var allPersons = await firebaseHelper.GetAllPersons();
            // lstPersons.ItemsSource = allPersons;
        }

        private void MyMap_MapClicked(object sender, MapClickedEventArgs e)
        {
            MyMap.Pins.Clear();

            MyMap.Pins.Add(new Pin
            {
                Label = "จุดที่พบผู้ป่วย COVID-19",
                Position = new Position(e.Position.Latitude, e.Position.Longitude)
            });

            latitude = e.Position.Latitude;
            longitude = e.Position.Longitude;

            txtLat.Text = "พิกัด Latitude : " + e.Position.Latitude.ToString();
            txtLong.Text = "พิกัด Longitude : " + e.Position.Longitude.ToString();
        }

        private async Task RetreiveLocation()
        {
            if (Application.Current.Properties.ContainsKey("USER_NAME"))
            {
                var username = Application.Current.Properties["USER_NAME"] as string;

                var locator = CrossGeolocator.Current;
                locator.DesiredAccuracy = 20;

                var position = await locator.GetPositionAsync(TimeSpan.FromTicks(10000));
                txtLat.Text = "พิกัด Latitude : " + position.Latitude.ToString();
                txtLong.Text = "พิกัด Longitude : " + position.Longitude.ToString();

                latitude = position.Latitude;
                longitude = position.Longitude;

                Position pos = new Position(position.Latitude, position.Longitude);
                DateTime datetime = DateTime.Now;
                CustomPin pin = new CustomPin
                {
                    Type = PinType.Place,
                    Position = pos,
                    Label = "ข้อมูลพิกัดล่าสุด(" + username + ")",
                    Address = "พิกัด: " + position.Latitude.ToString() + ", " + position.Longitude.ToString(),
                    Name = username,
                    Url = "www.google.co.th"
                };
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
                MyMap.Pins.Add(pin);
            }
        }
    }
}