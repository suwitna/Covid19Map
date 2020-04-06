using Covid19Map.Model;
using Plugin.Geolocator;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Maps;
using Xamarin.Forms.Xaml;
using XamForms.Controls;

namespace Covid19Map.View
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MapTrackingPage : ContentPage
    {
        FirebaseHelper firebaseHelper = new FirebaseHelper();
        double zoomMeters = 5000;
        double latitude = 17.3773698;
        double longitude = 104.7608508;
        string login = "suwit";
        public MapTrackingPage()
        {
            InitializeComponent();
            popupLoadingView.IsVisible = true;
            activityIndicator.IsRunning = true;

            btnRefresh.Clicked += BtnRefresh_Clicked;
            MyMap.MapClicked += MyMap_MapClicked;
            calendar.DateClicked += Calendar_DateClicked;
            calendar.StartDate = DateTime.Now;
            //calendar.SelectedDate = DateTime.Now;
            int i = 0;
            MessagingCenter.Subscribe<object, string>(this, "UpdateLabel", (s, e) =>
            {
                Device.BeginInvokeOnMainThread(async  () =>
                {
                    titleName.Text = i + "=>" + e;
                    //await AddTrackingLocation();
                    //titleName.Text = "ประวัติการเดินทาง";
                    i++;
                });
            });

            Device.StartTimer(TimeSpan.FromSeconds(1), () =>
            {
                Task.Factory.StartNew(async () =>
                {
                    await GetMapTrackingByLoginName(login);
                    Device.BeginInvokeOnMainThread(async () =>
                    {
                        //await AddTrackingLocation();
                        popupLoadingView.IsVisible = false;
                        activityIndicator.IsRunning = false;
                    });

                });
                return false;
            });
            /*
            Device.StartTimer(TimeSpan.FromMinutes(10), () =>
            {
                Task.Factory.StartNew(async () =>
                {
                    Device.BeginInvokeOnMainThread(async () =>
                    {
                        await AddTrackingLocation();
                    });

                });
                return false;
            });
            */
        }

        private async void Calendar_DateClicked(object sender, XamForms.Controls.DateTimeEventArgs e)
        {
            var dateSelect = calendar.SelectedDate;
            calendar.SelectedBackgroundColor = Color.Accent;
            await GetMapTrackingByDate(login,(DateTime) dateSelect);
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

        protected async override void OnAppearing()
        {
            base.OnAppearing();
        }
        private async Task AddTrackingLocation()
        {
            var locator = CrossGeolocator.Current;
            locator.DesiredAccuracy = 20;

            var position = await locator.GetPositionAsync(TimeSpan.FromTicks(10000));

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
            tracking.LoginName = login;
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
        }

        private async Task RetreiveLocation()
        {
            var locator = CrossGeolocator.Current;
            locator.DesiredAccuracy = 20;

            var position = await locator.GetPositionAsync(TimeSpan.FromTicks(10000));

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
            MyMap.Pins.Clear();

            Geocoder geoCoder = new Geocoder();
            var possibleAddresses = await geoCoder.GetAddressesForPositionAsync(new Position(latitude, longitude));
            var placemarks = await Geocoding.GetPlacemarksAsync(latitude, longitude);
            string city = "";
            string address = "";

            foreach (var item in placemarks)
            {
                city = item.ToString();
                break;
            }

            foreach (var item in possibleAddresses)
            {
                address = item.ToString();
                break;
            }

            var allTracking = await firebaseHelper.GetAllMapTracking();
            if (allTracking != null)
            {
                foreach (var item in allTracking)
                {
                    Position pos = new Position(item.Latitude, item.Longitude);

                    CustomPin pin = new CustomPin
                    {
                        Type = PinType.Place,
                        Position = pos,
                        Label = item.LoginName,
                        Address = item.AdressName,
                    };

                    MyMap.Pins.Add(pin);
                }
            }

            MyMap.MoveToRegion(MapSpan.FromCenterAndRadius(new Position(latitude, longitude)
                           , Distance.FromMeters(zoomMeters)));

        }

        private async Task GetMapTrackingByLoginName(string name)
        {
            var locator = CrossGeolocator.Current;
            locator.DesiredAccuracy = 20;

            var position = await locator.GetPositionAsync(TimeSpan.FromTicks(10000));

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
            MyMap.Pins.Clear();

            var allTracking = await firebaseHelper.GetMapTrackingByLoginName(name);
            
            if (allTracking != null)
            {
                
                List<SpecialDate> evens = new List<SpecialDate>();
                DateTime prevDate = new DateTime();
                foreach (var item in allTracking)
                {
                    Position pos = new Position(item.Latitude, item.Longitude);

                    CustomPin pin = new CustomPin
                    {
                        Type = PinType.Place,
                        Position = pos,
                        Label = item.LoginName.ToUpper(),
                        Address = item.CreateDate.ToString("dd/MM/yyyy HH:mm:ss", new CultureInfo("th-TH")),
                    };

                    MyMap.Pins.Add(pin);

                    //date event
                    if(item.CreateDate != null)
                    {
                        if (prevDate.Date.ToString("dd-MM-yyyy") != item.CreateDate.ToString("dd-MM-yyyy"))
                        {
                            SpecialDate even = new SpecialDate(item.CreateDate)
                            {
                                Selectable = true,
                                BackgroundPattern = new BackgroundPattern(1)
                                {
                                    Pattern = new List<Pattern>
                                    {
                                        new Pattern { WidthPercent = 1f, HightPercent = 0.6f },
                                        //new Pattern{ WidthPercent = 1f, HightPercent = 0.4f, Color = Color.Transparent, Text = "พบข้อมูล", TextColor=Color.Black, TextSize=11, TextAlign=TextAlign.Middle},
                                        new Pattern{ WidthPercent = 1f, HightPercent = 0.4f, Text = "พบข้อมูล", TextColor=Color.Green, TextSize=8, TextAlign=TextAlign.Middle},
                                    }
                                }
                            };
                            evens.Add(even);
                            prevDate = item.CreateDate;

                        }
                    }
                    
                }
                if (evens != null && evens.Count > 0) 
                {
                    calendar.SpecialDates = evens;
                }
            }

            MyMap.MoveToRegion(MapSpan.FromCenterAndRadius(new Position(latitude, longitude)
                           , Distance.FromMeters(zoomMeters)));

            calendar.SelectedDate = DateTime.Now;
            calendar.SelectedBackgroundColor = Color.Accent;
        }

        private async Task GetMapTrackingByDate(string name, DateTime date)
        {
            var locator = CrossGeolocator.Current;
            locator.DesiredAccuracy = 20;

            var position = await locator.GetPositionAsync(TimeSpan.FromTicks(10000));

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
            MyMap.Pins.Clear();

            var allTracking = await firebaseHelper.GetMapTrackingByDate(name, date);
            if (allTracking != null)
            {
                foreach (var item in allTracking)
                {
                    Position pos = new Position(item.Latitude, item.Longitude);

                    CustomPin pin = new CustomPin
                    {
                        Type = PinType.Place,
                        Position = pos,
                        Label = item.LoginName.ToUpper(),
                        Address = item.CreateDate.ToString("dd/MM/yyyy HH:mm:ss", new CultureInfo("th-TH")),
                    };

                    MyMap.Pins.Add(pin);
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