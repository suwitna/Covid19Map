using System;
using System.Collections.Generic;
using System.Text;

namespace Covid19Map.Model
{
    public class MapTracking
    {
        public string RowKey { get; set; }
        public string LoginName { get; set; }
        public string AdressName { get; set; }
        public string DistrictName { get; set; }
        public string ProvinceName { get; set; }
        public string CountryName { get; set; }
        public string PostalCode { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string IsActive { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }
    }
}
