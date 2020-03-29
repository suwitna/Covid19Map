using Covid19Map.Tables;
using Covid19Map.View;
using SQLite;
using System;
using System.IO;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Covid19Map.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LoginPage : ContentPage
    {
        public LoginPage()
        {
            NavigationPage.SetHasNavigationBar(this, false);
            InitializeComponent();
            Application.Current.Properties.Clear();
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

        async void ForgotPassword_Clicked(object sender, EventArgs e)
        {
            ///await Navigation.PushAsync(new PasswordPage());
        }
        async void Signup_Clicked(object sender, EventArgs e)
        {
            try
            {
                var request = new GeolocationRequest(GeolocationAccuracy.Medium);
                var location = await Geolocation.GetLocationAsync(request);
                //var location = await Geolocation.GetLastKnownLocationAsync();

                if (location != null)
                {
                    Console.WriteLine($"Latitude: {location.Latitude}, Longitude: {location.Longitude}, Altitude: {location.Altitude}");
                    await Navigation.PushAsync(new RegisterPage());
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
        }

        async void Login_Clicked(object sender, EventArgs e)
        {
            var dbpath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "UserDatabase.db");
            var db = new SQLiteConnection(dbpath);
            db.CreateTable<RegUserTable>();
            var myquery = db.Table<RegUserTable>().Where(u => u.UserName.Equals(EntryUser.Text) && u.Password.Equals(EntryPassword.Text)).FirstOrDefault();

            if (myquery != null)
            {
                Application.Current.Properties.Add("USER_NAME", myquery.UserName);
                Application.Current.Properties.Add("USER_LATITUDE", myquery.Latitude);
                Application.Current.Properties.Add("USER_LONGITUDE", myquery.Longitude);
                App.Current.MainPage = new MainPage();
                //App.Current.MainPage = new LocationTrackerPage();
            }
            else
            {
                Device.BeginInvokeOnMainThread(async () => {
                    if (EntryUser.Text == null)
                    {
                        //await this.DisplayAlert(null, "กรุณาระบุชื่อผู้ใช้งาน", null, "ตกลง");
                        EntryUser.PlaceholderColor = Color.FromHex("#ffb3ba");
                        EntryUser.Placeholder = "กรุณาระบุชื่อผู้ใช้งาน";
                        EntryUser.Focus();
                        return;
                    }

                    if (EntryPassword.Text == null)
                    {
                        //await this.DisplayAlert(null, "กรุณาระบุรหัสผ่าน", null, "ตกลง");
                        EntryPassword.PlaceholderColor = Color.FromHex("#ffb3ba");
                        EntryPassword.Placeholder = "กรุณาระบุรหัสผ่าน";
                        EntryPassword.Focus();
                        return;
                    }

                    var result = await this.DisplayAlert(null, "ข้อผิดพลาด! ไม่พบข้อมูลผู้ใช้งาน\nกรุณาตรวจสอบชื่อผู้ใช้งานและรหัสผ่าน", null, "ตกลง");

                    if (!result)
                    {
                        EntryUser.Focus();
                    }
                });
            }
        }



    }
}