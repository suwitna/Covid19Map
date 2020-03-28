using System;
using System.Collections.Generic;
using System.Text;

namespace Covid19Map.Tables
{
    class CovidMap
    {
        [SQLite.PrimaryKey, SQLite.AutoIncrement]
        public Guid CovidMapId { get; set; }
        public string GeoLatitude { get; set; }
        public string GeoLongitude { get; set; }
        public string PinType { get; set; }
        public string PinLabel { get; set; }
        public string PinAddress { get; set; }
        public string FoundTime { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }
    }
}
