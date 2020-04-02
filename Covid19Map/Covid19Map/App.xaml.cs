using Covid19Map.View;
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
            //Verion Login
            //MainPage = new NavigationPage(new LoginPage());

            //Verion report location
            //MainPage = new NavigationPage(new LocationTrackerPage());

            //version Nakhonphanom
            // MainPage = new NavigationPage(new CovidAtNKPPage());
            /*
            InitializeComponent();

            MainPage = new MainPage();
            */

            //Thailand
            MainPage = new NavigationPage(new CovidMapThaiViewPage()); 

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
