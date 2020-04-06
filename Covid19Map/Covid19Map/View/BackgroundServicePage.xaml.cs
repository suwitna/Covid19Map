using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Covid19Map.View
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class BackgroundServicePage : ContentPage
    {
        public BackgroundServicePage()
        {
            InitializeComponent();
            StartService.Clicked += StartService_Clicked;
            StopService.Clicked += StopService_Clicked;

            StartService.IsEnabled = true;
            StopService.IsEnabled = false;
            MessagingCenter.Unsubscribe<string>(this, "counterValue");
            MessagingCenter.Subscribe<string>(this, "counterValue", (value) =>
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    counter.Text = value;
                });
            });
        }



        private void StartService_Clicked(object sender, EventArgs e)
        {
            MessagingCenter.Send<string>("1", "myService");
            StartService.IsEnabled = false;
            StopService.IsEnabled = true;
        }

        private void StopService_Clicked(object sender, EventArgs e)
        {
            MessagingCenter.Send<string>("0", "myService");
            StartService.IsEnabled = true;
            StopService.IsEnabled = false;
        }
    }
}