using System;

namespace Covid19Map.Tables
{
    public class RegUserTable
    {
        [SQLite.PrimaryKey, SQLite.AutoIncrement]
        public Guid UserId { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }

    }
}
