using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GoaSocietyRegistration.Organization.Digital.Design
{
    public class CardData
    {
        public string Name { get; set; }
        public string publickey { get; set; }
        public string publicketString { get; set; }

        public string rawdataString { get; set; }
        public string serialnumberString { get; set; }
        public string type { get; set; }

        public string Issuer { get; set; }
        public string IssuerName { get; set; }
        public DateTime NotAfter { get; set; }

        public DateTime NotBefore { get; set; }
        public string publickey_ { get; set; }
        public string version { get; set; }
    }
}