using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms.Maps;

namespace Covid19Map
{
    public class CustomPin : Pin
    {
        public string Name { get; set; }
        public string Url { get; set; }
    }
}
