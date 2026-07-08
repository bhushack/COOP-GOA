using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GoaSocietyRegistration.Development
{
    public class OnlinePaymentDetails
    {
        public long onlinepayment_id { get; set; }
        public string echallanreq_xml { get; set; }
        public string echallan_no { get; set; }
        public Nullable<System.DateTime> eChallanGeneratedOn { get; set; }
        public string nicencdata { get; set; }
        public string paystat_response_xml { get; set; }
        public string status { get; set; }
        public Nullable<decimal> total_amt { get; set; }
        public string bank_ref_no { get; set; }
        public string bank_rcvd_date { get; set; }
        public string treasury_rcvd_date { get; set; }
        public Nullable<bool> active { get; set; }
        public Nullable<long> updated_by { get; set; }
        public System.DateTime updated_on { get; set; }
        public string loggedin_ip { get; set; }
        public string loggedin_mac { get; set; }
    }
}