using System;
using System.Collections.Generic;
using System.Text;

namespace Covid19Map
{
    class RootObject
    {
        public List<ProvinceData> Data { get; set; }
        public string LastData { get; set; }
        public string Source { get; set; }
        public string DevBy { get; set; }
        public string SeverBy { get; set; }

    }
}
