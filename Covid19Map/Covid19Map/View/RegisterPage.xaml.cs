
using Covid19Map.Tables;
using Plugin.Geolocator;
using SQLite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Covid19Map.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RegisterPage : ContentPage
    {
        public RegisterPage()
        {
            NavigationPage.SetHasNavigationBar(this, true);
            InitializeComponent();
        }

        private async void Button_Clicked(object sender, EventArgs e)
        {
            if (EntryUserName.Text == null)
            {
                //await this.DisplayAlert(null, "กรุณาระบุชื่อผู้ใช้งาน", null, "ตกลง");
                EntryUserName.PlaceholderColor = Color.FromHex("#ffb3ba");
                EntryUserName.Placeholder = "กรุณาระบุชื่อผู้ใช้งาน";
                EntryUserName.Focus();
                return;
            }

            if (EntryUserPassword.Text == null)
            {
                //await this.DisplayAlert(null, "กรุณาระบุรหัสผ่าน", null, "ตกลง");
                EntryUserPassword.PlaceholderColor = Color.FromHex("#ffb3ba");
                EntryUserPassword.Placeholder = "กรุณาระบุรหัสผ่าน";
                EntryUserPassword.Focus();
                return;
            }

            var dbpath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "UserDatabase.db");
            var db = new SQLiteConnection(dbpath);
            db.CreateTable<RegUserTable>();

            var locator = CrossGeolocator.Current;
            locator.DesiredAccuracy = 20;

            var position = await locator.GetPositionAsync(TimeSpan.FromTicks(10000));
            string strLat = position.Latitude.ToString();
            string strLon = position.Longitude.ToString();

            var item = new RegUserTable()
            {
                UserName = EntryUserName.Text,
                Password = EntryUserPassword.Text,
                Latitude = strLat,
                Longitude = strLon,
            };

            var syn = new SynMapHistory()
            {
                UserName = EntryUserName.Text,
                SynFlag = "Y",
                SynMapTime = DateTime.Now,
                UpdateTime = DateTime.Now,
            };
            db.CreateTable<SynMapHistory>();

            db.Insert(item);
            db.Insert(syn);
            Device.BeginInvokeOnMainThread(async () => {
                var result = await this.DisplayAlert(null, "สมัครสมาชิกสำเร็จ!", null, "ตกลง");

                if (!result)
                {
                    await Navigation.PushAsync(new LoginPage());
                }
            });
        }

        async void Cancel_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new LoginPage());
        }
    }
}