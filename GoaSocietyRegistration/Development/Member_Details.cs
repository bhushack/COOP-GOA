using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GoaSocietyRegistration.Development
{
    public class Member_Details
    {
        public string app_id { get; set; }
        public Int64 member_id { get; set; }
        public string fname { get; set; }
        public string designtaion { get; set; }
        public int design { get; set; }
        public string occupation { get; set; }      
        public int occupatid { get; set; }
        public string address { get; set; }
        public string proofname { get; set; }
        public int proofid { get; set; }
        public string mangcomm { get; set; }
        public string created_at { get; set; }
        public string created_by { get; set; }       
        public string ipaddress { get; set; }
        public string macaddress { get; set; }
        public string active { get; set; }
        public string document_mongoentry { get; set; }
        public string doc_id { get; set; }
        public string proof_document_no { get; set; }
        public int salutation_id { get; set; }
        public string salutation { get; set; }
        public string gender { get; set; }
        public int age { get; set; }
        public string designtaion_others { get; set; }
        public string occupation_others { get; set; }

        public string remarks { get; set; }

        public string dateofadmission { get; set; }


    }
}