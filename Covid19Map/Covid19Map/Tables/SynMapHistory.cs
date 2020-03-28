using System;

namespace Covid19Map.Tables
{
    public class SynMapHistory
    {
        [SQLite.PrimaryKey, SQLite.AutoIncrement]
        public Guid SynMapId { get; set; }
        public string UserName { get; set; }
        public string SynFlag { get; set; }
        public DateTime SynMapTime { get; set; }
        public DateTime UpdateTime { get; set; }
    }
}
