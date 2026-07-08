using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GoaSocietyRegistration.Organization.Digital.Design
{
    public class APIResponse
    {
        public int ErrorCode { get; set; }
        public string ErrorMessage { get; set; }
        public string Result { get; set; }
    }
}