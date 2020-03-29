using System;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Maps;
using Xamarin.Forms.Xaml;

namespace Covid19Map.View
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CovidAtNKPPage : ContentPage
    {
        FirebaseHelper firebaseHelper = new FirebaseHelper();
        double zoomMeters = 5000;
        double latitude = 17.3773698;
        double longitude = 104.7608508;

        public CovidAtNKPPage()
        {
            InitializeComponent();
            btnRefresh.Clicked += BtnRefresh_Clicked;
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
            await RetreiveLocation();
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

            MyMap.MoveToRegion(MapSpan.FromCenterAndRadius(new Position(latitude, longitude)
                        , Distance.FromMeters(zoomMeters)));

            var allPersons = await firebaseHelper.GetAllCovidMap();
            foreach (var item in allPersons)
            {
                Position pos = new Position(Convert.ToDouble(item.Latitude), Convert.ToDouble(item.Longitude));

                CustomPin pin = new CustomPin
                {
                    Type = PinType.SearchResult,
                    Position = pos,
                    Label = item.PinLabel,
                    Address = item.PinAddress,
                };
                MyMap.Pins.Add(pin);
            }
 
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