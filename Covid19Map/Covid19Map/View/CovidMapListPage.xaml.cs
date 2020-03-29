using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Covid19Map.View
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CovidMapListPage : ContentPage
    {
        FirebaseHelper firebaseHelper = new FirebaseHelper();
        public CovidMapListPage()
        {
            InitializeComponent();
            btnAddCovidMap.Clicked += BtnAddCovidMap_Clicked;
            btnViewCovidMap.Clicked += BtnViewCovidMap_Clicked;
        }

        private async void BtnViewCovidMap_Clicked(object sender, EventArgs e)
        {
            /*
            try
            {
                var request = new GeolocationRequest(GeolocationAccuracy.Medium);
                var location = await Geolocation.GetLocationAsync(request);
                //var location = await Geolocation.GetLastKnownLocationAsync();

                if (location != null)
                {
                    Console.WriteLine($"Latitude: {location.Latitude}, Longitude: {location.Longitude}, Altitude: {location.Altitude}");
                    await Navigation.PushAsync(new CovidMapViewPage());
                }
            }
            catch (FeatureNotSupportedException fnsEx)
            {
                // Handle not supported on device exception
            }
            catch (FeatureNotEnabledException fneEx)
            {
                // Handle not enabled on device exception
                await this.DisplayAlert(null, "กรุณาเปิด Location Service ก่อนสมัครใช้งาน", null, "ตกลง");
            }
            catch (PermissionException pEx)
            {
                // Handle permission exception
            }
            catch (Exception ex)
            {
                // Unable to get location
            }
            */
            await Navigation.PushAsync(new CovidMapViewPage());
        }

        protected async override void OnAppearing()
        {

            base.OnAppearing();
            var allCovidMap = await firebaseHelper.GetAllCovidMap();
            lstCovidMap.ItemsSource = allCovidMap;
        }

        private async void BtnAddCovidMap_Clicked(object sender, EventArgs e)
        {
            /*
            try
            {
                var request = new GeolocationRequest(GeolocationAccuracy.Medium);
                var location = await Geolocation.GetLocationAsync(request);
                //var location = await Geolocation.GetLastKnownLocationAsync();

                if (location != null)
                {
                    Console.WriteLine($"Latitude: {location.Latitude}, Longitude: {location.Longitude}, Altitude: {location.Altitude}");
                    await Navigation.PushAsync(new CovidMapAddPage());
                }
            }
            catch (FeatureNotSupportedException fnsEx)
            {
                // Handle not supported on device exception
            }
            catch (FeatureNotEnabledException fneEx)
            {
                // Handle not enabled on device exception
                await this.DisplayAlert(null, "กรุณาเปิด Location Service ก่อนสมัครใช้งาน", null, "ตกลง");
            }
            catch (PermissionException pEx)
            {
                // Handle permission exception
            }
            catch (Exception ex)
            {
                // Unable to get location
            }
            */
            await Navigation.PushAsync(new CovidMapAddPage());
        }
    }
}