using Covid19Map.Views;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Covid19Map
{
    public partial class App : Application
    {
        public App()
        {
            NavigationPage.SetHasNavigationBar(this, false);
            InitializeComponent();
            MainPage = new NavigationPage(new LoginPage());
            //MainPage = new NavigationPage(new LocationTrackerPage());
            /*
            InitializeComponent();

            MainPage = new MainPage();
            */
           
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
