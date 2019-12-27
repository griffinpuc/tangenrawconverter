using System;
using System.Collections.Generic;
using System.Text;

namespace JSONrawdataconvert
{
    class Data
    {
        public string dateTime { get; set; }
        public string assayName { get; set; }
        public string assayID { get; set; }
        public string assayLotNumber { get; set; }
        public string sampleId { get; set; }
        public string firmwareVersion { get; set; }
        public string instrumentUuid { get; set; }
        public string instrumentName { get; set; }
        public object[] scans { get; set; }


    }
}
