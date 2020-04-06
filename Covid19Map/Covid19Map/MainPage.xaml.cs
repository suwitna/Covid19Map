using Covid19Map.MenuItems;
using Covid19Map.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Covid19Map.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MainPage : MasterDetailPage
    {
        public MainPage()
        {
            NavigationPage.SetHasNavigationBar(this, false);
            InitializeComponent();
            MasterPage.ListView.ItemSelected += ListView_ItemSelected;

            MasterPageItem item = new MasterPageItem();
            //item.TargetType = typeof(CovidMapListPage);
            // item.Title = "ข้อมูลพิกัด Covid-19";

            item.TargetType = typeof(CovidMapThaiViewPage);
            item.Title = "ข้อมูลจากกรมควบคุมโรค";
            item.Icon = "list_b.png";

            var page = (Page)Activator.CreateInstance(item.TargetType);
            page.Title = item.Title;
            page.IconImageSource = item.Icon;

            Detail = new NavigationPage(page);
            IsPresented = false;

            MasterPage.ListView.SelectedItem = null;
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

        private async void ListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            var item = e.SelectedItem as MainPageMasterMenuItem;
            if (item == null)
                return;

            List<Page> li = Navigation.NavigationStack.ToList();
            if (li.Count > 0) 
            {
                Page last = li.ElementAt(li.Count - 1);
                Navigation.RemovePage(last);
            }

            if (item.Id == 5)
            {
                Device.BeginInvokeOnMainThread(async () =>
                {
                    var result = await this.DisplayAlert(null, "ต้องการออกจากระบบ?", "ตกลง", "ยกเลิก");

                    if (result)
                    {
                        Application.Current.Properties.Clear();
                        var page = (Page)Activator.CreateInstance(typeof(LoginPage));

                        Detail = new NavigationPage(page);
                        IsPresented = false;

                        MasterPage.ListView.SelectedItem = null;
                    }
                    else
                    {
                        MasterPage.ListView.SelectedItem = null;
                    }
                });
            }
            else
            {
                var page = (Page)Activator.CreateInstance(item.TargetType);

                page.Title = item.Title;

                var currentPage = ((NavigationPage)((MasterDetailPage)Application.Current.MainPage).Detail).RootPage;
                if (page.GetType() != currentPage.GetType())
                { 
                    Detail = new NavigationPage(page);
                }
                IsPresented = false;
                MasterPage.ListView.SelectedItem = null;
            }
        }
    }
}