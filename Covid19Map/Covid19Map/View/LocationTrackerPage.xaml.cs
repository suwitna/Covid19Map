﻿using Plugin.Geolocator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Maps;
using Xamarin.Forms.Xaml;
using Firebase.Database;
using Firebase.Database.Query;
using Covid19Map.Model;

namespace Covid19Map.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LocationTrackerPage : ContentPage
    {
        FirebaseHelper firebaseHelper = new FirebaseHelper();
        bool onInit = false;
        double zoomMeters = 5000;
        double latitude = 17.3773698;
        double longitude = 104.7608508;
        string login = "";

        public LocationTrackerPage()
        {
            InitializeComponent();
            if (Application.Current.Properties.ContainsKey("USER_NAME"))
            {
                var strLatitude = Application.Current.Properties["USER_LATITUDE"] as string;
                var strLongitude = Application.Current.Properties["USER_LATITUDE"] as string;
                
                login = Application.Current.Properties["USER_NAME"] as string;
                MyMap.MoveToRegion(MapSpan.FromCenterAndRadius(new Position(Convert.ToDouble(strLatitude), Convert.ToDouble(strLongitude))
                                 , Distance.FromMeters(zoomMeters)));
            }

            btnGetLocation.Clicked += BtnGetLocation_Clicked;
            //btnAdd.Clicked += BtnAdd_Clicked;
            //btnUpdate.Clicked += BtnUpdate_Clicked;
            Device.StartTimer(TimeSpan.FromSeconds(0.01), () =>
            {
                Task.Factory.StartNew(async () =>
                {
                    Device.BeginInvokeOnMainThread(async () =>
                    {
                        //await firebaseHelper.DeleteMapHistor();
                        await RetreiveLocation();
                    });

                });
                return false;
            });
            /*
            Device.StartTimer(TimeSpan.FromMinutes(30), () =>
            {
                Task.Factory.StartNew(async () =>
                {
                    Device.BeginInvokeOnMainThread(async () =>
                    {
                        await RetreiveLocation();
                    });

                });
                return true;
            });
            */
        }

        /*
        private async void BtnUpdate_Clicked(object sender, EventArgs e)
        {
            await firebaseHelper.UpdatePerson(1, "Mika");
            await DisplayAlert("Success", "Person Updated Successfully", "OK");
            var allPersons = await firebaseHelper.GetAllPersons();
            lstPersons.ItemsSource = allPersons;
        }

        private async void BtnAdd_Clicked(object sender, EventArgs e)
        {
            await firebaseHelper.AddPerson(1, "suwit");
            await DisplayAlert("Success", "Person Added Successfully", "OK");
            var allPersons = await firebaseHelper.GetAllPersons();
            lstPersons.ItemsSource = allPersons;
        }
        
        protected async override void OnAppearing()
        {

            base.OnAppearing();
            var allPersons = await firebaseHelper.GetAllPersons();
            lstPersons.ItemsSource = allPersons;
        }
        */
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
        protected async override void OnAppearing()
        {

            base.OnAppearing();
        }

        private async void BtnGetLocation_Clicked(object sender, EventArgs e)
        {
            Device.BeginInvokeOnMainThread(async () => {
                var result = await DisplayAlert("แจ้งพิกัด", "ต้องการแจ้งพิกัดปัจจุบันเข้าสู่ระบบ โปรดยืนยัน?", "ตกลง", "ยกเลิก");
                if (result)
                {
                    await RetreiveLocation();
                    await AddTrackingLocation();


                    popupSavingView.IsVisible = true;
                    Device.StartTimer(TimeSpan.FromSeconds(2), () => {
                        popupSavingView.IsVisible = false;
                        return false;
                    });

                }
            });
            
        }

        private async Task RetreiveLocation()
        {
            if (Application.Current.Properties.ContainsKey("USER_NAME"))
            {
                var username = Application.Current.Properties["USER_NAME"] as string;

                var locator = CrossGeolocator.Current;
                locator.DesiredAccuracy = 20;

                var position = await locator.GetPositionAsync(TimeSpan.FromTicks(10000));
                txtLat.Text = "Latitude : " + position.Latitude.ToString();
                txtLong.Text = "Longitude : " + position.Longitude.ToString();



                Position pos = new Position(position.Latitude, position.Longitude);
                DateTime datetime = DateTime.Now;
                Pin pin = new Pin
                {
                    Type = PinType.Place,
                    Position = pos,
                    Label = "ข้อมูลพิกัดล่าสุด(" + username + ")",
                    Address = "พิกัด: " + position.Latitude.ToString() + ", " + position.Longitude.ToString(),
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
                
                MapHistory history = new MapHistory();
                history.LoginName = username;
                history.Accuracy = zoomMeters.ToString();
                history.GeoLatitude = position.Latitude.ToString();
                history.GeoLongitude = position.Longitude.ToString();
                history.PinType = PinType.Place.ToString();
                history.PinLabel = pin.Label.ToString();
                history.PinAddress = pin.Address.ToString();
                history.SaveTime = datetime;

                AddMapHistory(history);
            }
        }

        private async void AddMapHistory(MapHistory history)
        {
            await firebaseHelper.AddMapHistory(history);
            //await DisplayAlert("Success", "Person Added Successfully", "OK");
            Console.WriteLine("Map history added Successfully");
            // var allPersons = await firebaseHelper.GetAllPersons();
            // lstPersons.ItemsSource = allPersons;
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
    }
}