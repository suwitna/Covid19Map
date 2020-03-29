using System.Collections.Generic;
using Xamarin.Forms.Maps;

namespace Covid19Map
{
    public class CustomMap : Map
    {
        public List<CustomPin> CustomPins { get; set; }
    }
}
