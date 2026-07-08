using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GoaSocietyRegistration;

namespace GoaSocietyRegistration.Development
{
    public class UsersAuditTrails
    {

      public string app_id { get; set; }
        public string user_login_id { get; set; }
        public string admin_login_id { get; set; }
        public string browser_session_id { get; set; }
        public string user_session_id { get; set; }
        public string admin_session_id { get; set; }
        public string loggedin_status { get; set; }
        public string referrer { get; set; }
        public string accessed_module { get; set; }
        public string action_performed { get; set; }
        public string action_description { get; set; }
        public string action_status { get; set; }
        public System.DateTime tracked_datetime { get; set; }
        public string loggedin_ip { get; set; }
        public string loggedin_mac { get; set; }
        public string browser_name { get; set; }
        public int is_crud { get; set; }
        public string browser_version { get; set; }
        public string device_type { get; set; }
        public string admin_login_name { get; set; }
        
    }
}
