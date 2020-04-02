using System;
using System.Collections.Generic;
using System.Text;

namespace Covid19Map
{
    class Province
    {
        public string id { get; set; }
        public string zip { get; set; }
        public string province { get; set; }
        public string district { get; set; }
        public double lat { get; set; }
        public double lng { get; set; }
    }
}
