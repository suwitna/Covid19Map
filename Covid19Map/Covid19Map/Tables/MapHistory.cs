using System;

namespace Covid19Map.Tables
{
    public class MapHistory
    {
        [SQLite.PrimaryKey, SQLite.AutoIncrement]
        public Guid HistoryId { get; set; }
        public string LoginName { get; set; }
        public string Accuracy { get; set; }
        public string GeoLatitude { get; set; }
        public string GeoLongitude { get; set; }
        public string PinType { get; set; }
        public string PinLabel { get; set; }
        public string PinAddress { get; set; }
        public DateTime SaveTime { get; set; }
    }
}
