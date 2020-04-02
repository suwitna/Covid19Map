using Covid19Map.MenuItems;
using Covid19Map.Tables;
using Covid19Map.View;
using Plugin.Media.Abstractions;
using SQLite;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using static Android.Preferences.PreferenceActivity;

namespace Covid19Map.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MainPageMaster : ContentPage
    {
        public ListView ListView;
        byte[] ImageBytes;

        public MainPageMaster()
        {
            InitializeComponent();

            BindingContext = new MainPageMasterViewModel();
            ListView = MenuItemsListView;
            LoadProfile();
        }

        public async void LoadProfile()
        {
            if (Application.Current.Properties.ContainsKey("USER_NAME"))
            {
                var username = Application.Current.Properties["USER_NAME"] as string;
                selectedImage.Source = "account.png";
                AccountName.Text = username;
                /*
               var username = Application.Current.Properties["USER_NAME"] as string;

               var dbpath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "UserDatabase.db");
               var db = new SQLiteConnection(dbpath);
               var myquery = db.Table<RegUserTable>().Where(u => u.UserName.Equals(username)).FirstOrDefault();

               if (myquery != null)
               {

               }

               var profile = db.Table<ProfileTable>().Where(u => u.UserName.Equals(username)).FirstOrDefault();
               if (profile != null)
               {
                   string fullName = "";

                   if (profile.FirstName != null)
                       fullName = profile.FirstName;

                   if (profile.LastName != null)
                       fullName += " " + profile.LastName;

                   AccountName.Text = fullName;

                   if (profile.Content != null)
                   {
                       ImageBytes = profile.Content;
                       Stream sm = BytesToStream(ImageBytes);
                       selectedImage.Source = ImageSource.FromStream(() => sm);
                   }

               }
               */
            }
        }

        public Stream BytesToStream(byte[] bytes)
        {
            Stream stream = new MemoryStream(bytes);
            return stream;
        }
        
        class MainPageMasterViewModel : INotifyPropertyChanged
        {
            public ObservableCollection<MainPageMasterMenuItem> MenuItems { get; set; }

            public MainPageMasterViewModel()
            {
                MenuItems = new ObservableCollection<MainPageMasterMenuItem>(new[]
                {
                    new MainPageMasterMenuItem { Id = 0, Title = "หน้าหลัก", IconSource="home_b.png", TargetType = typeof(LocationTrackerPage)},
                    new MainPageMasterMenuItem { Id = 1, Title = "ประวัติการเดินทาง", IconSource="list_b.png", TargetType = typeof(LocationTrackerPage)},
                    new MainPageMasterMenuItem { Id = 2, Title = "Covid-19 @ นครพนม ", IconSource="list_b.png", TargetType = typeof(CovidAtNKPPage)},
                    new MainPageMasterMenuItem { Id = 3, Title = "ข้อมูลพิกัด Covid-19", IconSource="setting_b.png", TargetType = typeof(CovidMapListPage)},
                    new MainPageMasterMenuItem { Id = 5, Title = "ออกจากระบบ", IconSource="logout_b.png"},
                });
            }

            #region INotifyPropertyChanged Implementation
            public event PropertyChangedEventHandler PropertyChanged;
            void OnPropertyChanged([CallerMemberName] string propertyName = "")
            {
                if (PropertyChanged == null)
                    return;

                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
            #endregion
        }

    }
}