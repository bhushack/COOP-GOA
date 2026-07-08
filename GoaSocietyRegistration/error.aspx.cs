using GoaSocietyRegistration.Development;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace GoaSocietyRegistration
{
    public partial class error : System.Web.UI.Page
    {
        string ipaddress = Utility.getIP();
        string macaddress = Utility.GetMACAddress(Utility.getIP());
        //string macaddress = Utility.GetMACAddress();
        protected void Page_Load(object sender, EventArgs e)
        {
            //if (Session["common_logout"] != null)
            //{
            //    string abc = Session["common_logout"].ToString();
            //    if (abc.Contains("@"))
            //    {
            //        //admin
            //        Utility.logout(Convert.ToInt64(Session["loginsession_count"]));
            //    }
            //    else
            //    {
            //        Utility.logout_user(Convert.ToInt64(Session["loginsession_count"]));
            //    }

            //}

            //CreateLogFiles Err = new CreateLogFiles();
            //Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), "Error Encountered " + (Request.UrlReferrer.ToString() == null ? "Null url" : Request.UrlReferrer.ToString()), "Page_Load" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath) + ipaddress);
            HttpContext.Current.Response.AddHeader("Cache-Control", "no-cache, no-store, must-revalidate");
            HttpContext.Current.Response.AddHeader("Pragma", "no-cache");
            HttpContext.Current.Response.AddHeader("Expires", "0");
            Response.Redirect("~/Default.aspx");
          
        }
        private void RecordUserAction(string action, string description, string status)
        {
            /*Audit trail*/
            int count = 0;
            do
            {
                System.Web.HttpBrowserCapabilities browser = Request.Browser;
                UsersAuditTrails trail = new UsersAuditTrails();
                string uri = HttpContext.Current.Request.Url.AbsoluteUri;
                if (Session["login_id"] != null)
                {
                    trail.user_login_id = Session["login_id"].ToString();
                    trail.loggedin_status = "Y";
                }
                else
                {

                    trail.user_login_id = "null";
                    trail.loggedin_status = "N";
                }
                trail.browser_session_id = HttpContext.Current.Request.Cookies[GlobalVars.AntiXsrfTokenKey] == null ? HttpContext.Current.Session.SessionID : HttpContext.Current.Request.Cookies[GlobalVars.AntiXsrfTokenKey].Value;
                trail.user_session_id = Session["loginsession"] != null ? Session["loginsession"].ToString() : "null";
                trail.referrer = uri != null ? uri.ToString().Length > 100 ? uri.ToString().Substring(0, 100) : uri.ToString() : string.Empty;
                trail.accessed_module = "Society";
                trail.action_performed = action;
                trail.action_description = description.Length > 200 ? description.Substring(0, 200) : description;
                trail.action_status = status;
                trail.tracked_datetime = DateTime.Now;
                trail.loggedin_ip = ipaddress;
                trail.loggedin_mac = macaddress;
                trail.browser_name = browser.Browser;
                trail.browser_version = browser.Version;
                string strUserAgent = Request.UserAgent.ToString();
                if (strUserAgent != null)
                {
                    if (Request.Browser.IsMobileDevice == true || strUserAgent.Contains("iphone") ||
                        strUserAgent.Contains("blackberry") || strUserAgent.Contains("mobile") ||
                        strUserAgent.Contains("windows ce") || strUserAgent.Contains("opera mini") ||
                        strUserAgent.Contains("palm"))
                    {
                        trail.device_type = strUserAgent;
                    }
                    else
                    {
                        trail.device_type = "Laptop/ Desktop";
                    }

                }
                count = SaveAuditTrail(trail);
            } while (count == 0);
            /*Audit trail*/
        }
        public int SaveAuditTrail(UsersAuditTrails trial)
        {
            NpgsqlConnection conn = new NpgsqlConnection();
            NpgsqlCommand cmd = new NpgsqlCommand();
            conn.ConnectionString = ConfigurationManager.ConnectionStrings["connect"].ConnectionString;
            cmd.Connection = conn;
            try
            {
                conn.Open();
                string query = "INSERT INTO user_audit_trail( user_login_id,browser_session_id, user_session_id, loggedin_status, referrer, accessed_module,";
                query = query + " action_performed, action_description, action_status, ipaddress, macaddress,  tracked_datetime, browser_name,browser_version,device_type ) VALUES (@user_login_id,@browser_session_id, ";
                query = query + " @user_session_id, @loggedin_status, @referrer, @accessed_module, @action_performed, @action_description, @action_status, @ipaddress, @macaddress,";
                query = query + "  @tracked_datetime,@browser_name,@browser_version,@device_type)";
                cmd.CommandText = query;
                cmd.Parameters.AddWithValue("@user_login_id", trial.user_login_id);
                cmd.Parameters.AddWithValue("@browser_session_id", trial.browser_session_id);
                cmd.Parameters.AddWithValue("@user_session_id", trial.user_session_id);
                cmd.Parameters.AddWithValue("@loggedin_status", trial.loggedin_status);
                cmd.Parameters.AddWithValue("@referrer", trial.referrer);
                cmd.Parameters.AddWithValue("@accessed_module", trial.accessed_module);
                cmd.Parameters.AddWithValue("@action_performed", trial.action_performed);
                cmd.Parameters.AddWithValue("@action_description", trial.action_description);
                cmd.Parameters.AddWithValue("@action_status", trial.action_status);
                cmd.Parameters.AddWithValue("@ipaddress", trial.loggedin_ip);
                cmd.Parameters.AddWithValue("@macaddress", trial.loggedin_mac);
                cmd.Parameters.AddWithValue("@tracked_datetime", trial.tracked_datetime);
                cmd.Parameters.AddWithValue("@browser_name", trial.browser_name);
                cmd.Parameters.AddWithValue("@browser_version", trial.browser_version);
                cmd.Parameters.AddWithValue("@device_type", trial.device_type);
                cmd.ExecuteNonQuery();
                return 1;
            }
            catch (NpgsqlException ex)
            {
                return 0;
            }
            finally
            {
                conn.Close();
            }

        }
        protected void Button1_Click(object sender, EventArgs e)
        {
            int value = Utility.logout(Convert.ToInt64(Session["loginsession_count"]));
            if (value == 1)
            {
                Session.Clear();
                Session.Abandon();
                Session.RemoveAll();
                Response.Cookies.Clear();
                //RecordUserAction("Button1_Click", "All Session cleared.Logout", "S");
                Response.Redirect("~/Default.aspx");
            }
            else
            {
               // RecordUserAction("Button1_Click", "Exception Redirected to Login", "S");
                Response.Write("<script language='javascript'>alert(' Error While logout....Try after sometime ')</script>");
                Response.Redirect("~/Default.aspx");
            }
        }
    }
}