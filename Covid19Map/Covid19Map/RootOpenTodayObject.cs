using System;
using System.Collections.Generic;
using System.Text;

namespace Covid19Map
{
    class RootOpenTodayObject
    {
        public int Confirmed { get; set; }
        public int Hospitalized { get; set; }
        public int Deaths { get; set; }
        public int NewConfirmed { get; set; }
        public int Recovered { get; set; }
        public int NewRecovered { get; set; }
        public int NewHospitalized { get; set; }
        public int NewDeaths { get; set; }
        public string UpdateDate { get; set; }
        public string Source { get; set; }
        public string DevBy { get; set; }
        public string SeverBy { get; set; }


    }
}
