using System;
using System.Collections.Generic;
using System.Text;

namespace Covid19Map.Model
{
    class Covid
    {
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public string PinLabel { get; set; }
        public string PinAddress { get; set; }
        public string FoundDate { get; set; }
        public string IsActive { get; set; }
        public DateTime UpdateDate { get; set; }
    }
}
