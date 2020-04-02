using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Plugin.Geolocator;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Schema;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Maps;
using Xamarin.Forms.PlatformConfiguration;
using Xamarin.Forms.Xaml;

namespace Covid19Map.View
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CovidMapThaiViewPage : ContentPage
    {
        FirebaseHelper firebaseHelper = new FirebaseHelper();
        bool onInit = false;
        double zoomMeters = 600000;
        double latitude = 13.7560243;
        double longitude = 100.4986793;
        public CovidMapThaiViewPage()
        {
            InitializeComponent();
            btnRefresh.Clicked += BtnRefresh_Clicked;

            popupLoadingView.IsVisible = true;
            activityIndicator.IsRunning = true;

            Device.StartTimer(TimeSpan.FromSeconds(7), () =>
            {
                Task.Factory.StartNew(async () =>
                {
                    Device.BeginInvokeOnMainThread(async () =>
                    {
                        //SyncProvinceList();
                        popupLoadingView.IsVisible = false;
                        activityIndicator.IsRunning = false;
                    });

                });
                return false;
            });
        }
        public async Task MapBuilding25()
        {
            var location = new Location(13.7920545, 100.6326222);
            var options = new MapLaunchOptions { Name = "Mika House" };

            await Xamarin.Essentials.Map.OpenAsync(location, options);
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

        private async void BtnRefresh_Clicked(object sender, EventArgs e)
        {
            //Task.Run(async () => await SyncCovidAnnounceData());
            SyncProvinceList();
        }

        protected async override void OnAppearing()
        {
            //SyncProvinceList();
            this.Appearing += Page_Appearing;
        }

        public async void Page_Appearing(object sender, EventArgs e)
        {
            this.Appearing -= Page_Appearing;
            await SyncProvinceList();
        }

        private async Task SyncCovidAnnounceData()
        {
            var uri = new Uri("https://covid19.th-stat.com/api/open/area");

            var client = new HttpClient();
            var content = await client.GetStringAsync(uri);
            // HttpClient client = new HttpClient();
            //Task<string> getStringTask = client.GetStringAsync(uri);
            //string content = await getStringTask;

            RootObject json = JsonConvert.DeserializeObject<RootObject>(content);
        }

        private async Task SyncProvinceList()
        {
            var current = Connectivity.NetworkAccess;

            if (current != NetworkAccess.Internet)
            {
                var result = await DisplayAlert(null, "ไม่สามารถเชื่อมต่ออินเตอร์เน็ตได้\nกรุณาตรวจสอบแล้วลองอีกครั้ง", null, "ตกลง");
                if (!result)
                {
                    // await this.Navigation.PopAsync(); // or anything else
                    if (Device.RuntimePlatform == Device.Android)
                    {
                        Android.OS.Process.KillProcess(Android.OS.Process.MyPid());
                    }
                }
                //await this.DisplayAlert(null, "ไม่สามารถเชื่อมต่ออินเตอร์เน็ตได้\nกรุณาตรวจสอบแล้วลองอีกครั้ง", null, "ตกลง");
                //return;
            }

            var uri = new Uri("https://covid19.th-stat.com/api/open/area");

            HttpClient client = new HttpClient();
            //Task<string> getStringTask = client.GetStringAsync(uri);
            //string content = await getStringTask;
            var content = await client.GetStringAsync(uri);
            RootObject jsonCovid = JsonConvert.DeserializeObject<RootObject>(content);

            uri = new Uri("https://covid19.th-stat.com/api/open/cases");
            //getStringTask = client.GetStringAsync(uri);
            //content = await getStringTask;
            content = await client.GetStringAsync(uri);
            RootOpenCaseObject jsonOpenCase = JsonConvert.DeserializeObject<RootOpenCaseObject>(content);

            uri = new Uri("https://covid19.th-stat.com/api/open/today");
            //getStringTask = client.GetStringAsync(uri);
            //content = await getStringTask;
            content = await client.GetStringAsync(uri);
            RootOpenTodayObject jsonOpenToday = JsonConvert.DeserializeObject<RootOpenTodayObject>(content);

            MyMap.Pins.Clear();
            var assambly = typeof(CovidMapThaiViewPage).GetTypeInfo().Assembly;
            Stream stream = assambly.GetManifestResourceStream("Covid19Map.thai.json");

            using (var reader = new StreamReader(stream))
            {
                var json = reader.ReadToEnd();
                List<Province> provinces = JsonConvert.DeserializeObject<List<Province>>(json);

                foreach (var item in provinces)
                {
                    string detail = "";
                    string openCase = "";

                    int intCase = 0;
                    int intMale = 0;
                    int intFemale = 0;
                    int intThai = 0;
                    int intNonThai = 0;
                    int intChinese = 0;

                    Position pos = new Position(item.lat, item.lng);
                    if (jsonCovid != null)
                    {
                        List<ProvinceData> Data = jsonCovid.Data;
                        if (Data != null)
                        {

                            int i = 1;
                            foreach (var data in Data)
                            {
                                if (item.province.Trim() == data.Province.Trim())
                                {
                                    string locDetail = data.Location.Trim();
                                    if (locDetail.Length > 0)
                                    {
                                        detail += i + ") " + data.Location.Trim();
                                        detail += "\n";
                                        i++;
                                    }
                                }
                            }
                        }
                    }

                    if (jsonOpenCase != null)
                    {
                        List<OpenCase> Data = jsonOpenCase.Data;
                        if (Data != null)
                        {
                            foreach (var data in Data)
                            {
                                if (data.Province != null)
                                {
                                    if (item.province.Trim() == data.Province.Trim())
                                    {
                                        intCase += 1;
                                        //"Male or Female
                                        if (data.GenderEn != null)
                                        {
                                            if (data.GenderEn.Trim().ToUpper() == "MALE")
                                            {
                                                intMale += 1;
                                            }
                                            else
                                            {
                                                intFemale += 1;
                                            }
                                        }


                                        //Thai
                                        if (data.NationEn != null)
                                        {
                                            if (data.NationEn.Trim().ToUpper() == "THAI")
                                            {
                                                intThai += 1;
                                            }
                                            else
                                            {
                                                intNonThai += 1;
                                            }

                                            //Chinese
                                            if (data.NationEn.Trim().ToUpper() == "CHINESE")
                                            {
                                                intChinese += 1;
                                            }
                                        }
                                    }
                                }
                            }
                            bool multiLine = false;

                            if (intCase > 0)
                            {
                                openCase += "ติดเชื้อสะสม: " + intCase;
                                multiLine = true;
                            }
                            if (intMale > 0)
                            {
                                if (multiLine == true)
                                {
                                    openCase += "\n";
                                }

                                openCase += "ชาย: " + intMale + " ราย";
                            }
                            if (intFemale > 0)
                            {
                                if (multiLine == true)
                                {
                                    openCase += "\n";
                                }
                                openCase += "หญิง: " + intFemale + " ราย";
                            }

                            if (intThai > 0)
                            {
                                if (multiLine == true)
                                {
                                    openCase += "\n";
                                }
                                openCase += "คนไทย: " + intThai + " ราย";
                            }
                            if (intNonThai > 0)
                            {
                                if (multiLine == true)
                                {
                                    openCase += "\n";
                                }
                                openCase += "คนต่างชาติ: " + intNonThai + " ราย";
                            }
                            if (intChinese > 0)
                            {
                                if (multiLine == true)
                                {
                                    openCase += "\n";
                                }
                                openCase += "คนจีน: " + intChinese + " ราย";
                            }
                            if (detail.Trim() != "")
                            {
                                openCase += "\n<<พื้นที่เตือนระวัง>>";
                            }

                        }
                    }

                    if (openCase != "")
                    {
                        CustomPin pin = new CustomPin
                        {
                            Type = PinType.Place,
                            Position = pos,
                            Label = item.province,
                            Address = openCase,
                        };
                        if (detail != "")
                        {
                            pin.Clicked += (object sender, EventArgs e) =>
                            {
                                var p = sender as Pin;
                                DisplayAlert("พื้นที่เตือนระวัง", detail.Trim(), "ตกลง");
                            };
                        }
                        MyMap.Pins.Add(pin);
                    }
                }
            }

            try
            {
                var locator = CrossGeolocator.Current;

                if (locator != null)
                {
                    locator.DesiredAccuracy = 20;
                    var position = await locator.GetPositionAsync(TimeSpan.FromTicks(10000));
                    MyMap.MoveToRegion(MapSpan.FromCenterAndRadius(new Position(position.Latitude, position.Longitude)
                                        , Distance.FromMeters(zoomMeters)));

                    //

                    Polygon polygon = new Polygon
                    {
                        StrokeWidth = 2,
                        StrokeColor = Color.FromHex("#F469B2"),
                        FillColor = Color.FromHex("#32A88980"),
                        Geopath = {
                            new Position(position.Latitude-0.005, position.Longitude+0.005),
                            new Position(position.Latitude+0.005, position.Longitude+0.005),
                            new Position(position.Latitude+0.005, position.Longitude-0.005),
                            new Position(position.Latitude-0.005, position.Longitude-0.005)
                        }
                    };

                    MyMap.MapElements.Add(polygon);
                    //
                }
            }
            catch (FeatureNotSupportedException fnsEx)
            {
                // Handle not supported on device exception
            }
            catch (FeatureNotEnabledException fneEx)
            {
                // Handle not enabled on device exception
                MyMap.MoveToRegion(MapSpan.FromCenterAndRadius(new Position(latitude, longitude)
                                        , Distance.FromMeters(zoomMeters)));
                await this.DisplayAlert("ข้อแนะนำ", "กรุณาเปิด Location Service เพื่อประสิทธิภาพในการใช้งานที่ดียิ่งขึ้น", null, "ตกลง");
            }
            catch (PermissionException pEx)
            {
                // Handle permission exception
            }
            catch (Exception ex)
            {
                // Unable to get location
                MyMap.MoveToRegion(MapSpan.FromCenterAndRadius(new Position(latitude, longitude)
                                        , Distance.FromMeters(zoomMeters)));
                await this.DisplayAlert("ข้อแนะนำ", "กรุณาเปิด Location Service เพื่อประสิทธิภาพในการใช้งานที่ดียิ่งขึ้น", null, "ตกลง");
            }

            if (jsonOpenToday != null)
            {

                string notiTitle = "";
                string notiDetail = "";
                notiTitle += "รายงานสถานการณ์ โควิด-19";
                notiTitle += "\n" + jsonOpenToday.UpdateDate;
                notiDetail += "ติดเชื้อสะสม     : " + jsonOpenToday.Confirmed + " ราย(เพิ่มขึ้น " + jsonOpenToday.NewConfirmed + ")";
                notiDetail += "\nหายแล้ว           : " + jsonOpenToday.Recovered + " ราย(เพิ่มขึ้น " + jsonOpenToday.NewRecovered + ")";
                notiDetail += "\nรักษาอยู่ใน รพ.: " + jsonOpenToday.Hospitalized + " ราย(เพิ่มขึ้น " + jsonOpenToday.NewHospitalized + ")";
                notiDetail += "\nเสียชีวิต            : " + jsonOpenToday.Deaths + " ราย(เพิ่มขึ้น " + jsonOpenToday.NewDeaths + ")";


                await this.DisplayAlert(notiTitle, notiDetail, null, "ตกลง");

            }
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
                }
                else
                {
                    zoomMeters = 2000;
                }
                MyMap.Pins.Clear();

                MyMap.MoveToRegion(MapSpan.FromCenterAndRadius(new Position(position.Latitude, position.Longitude)
                                  , Distance.FromMeters(zoomMeters)));
        }
    }
}