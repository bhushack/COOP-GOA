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
    public partial class VerifyCertificate : System.Web.UI.Page
    {
        Insert insrt = new Insert();
        SessionManage session = new SessionManage();
        System.Globalization.CultureInfo french = new System.Globalization.CultureInfo("fr-FR");
        Validate val = new Validate();
        Insert ins = new Insert();
        string ipaddress = Utility.getIP();
        string macaddress = Utility.GetMACAddress(Utility.getIP());
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Check4Tampering())
            {
                RecordUserAction("Page_Load", "Access request failed. Tampered session", "F");
                session.__Abandon(Request, Response);
            }
            else
            { 
                HttpContext.Current.Response.AddHeader("Cache-Control", "no-cache, no-store, must-revalidate");
                HttpContext.Current.Response.AddHeader("Pragma", "no-cache");
                HttpContext.Current.Response.AddHeader("Expires", "0");
                RecordUserAction("Applicant Page Onload", "Applicant Page Load Success", "S"); 
            }
        }
        private bool Check4Tampering()
        {
            bool sessHackedCheck = false;
            if (Request.UrlReferrer != null && GlobalVars.AntiPageRequest.Contains(Path.GetFileName(Page.AppRelativeVirtualPath)))
            {
                Uri uri = new Uri(Request.UrlReferrer.ToString());
                string referrer = uri.Scheme + Uri.SchemeDelimiter + uri.Host;
                if (!GlobalVars.AntiXsrfRefererHeaderList.Contains(referrer))
                    sessHackedCheck = true;
            }
            else
            {
                sessHackedCheck = true;
            }
            return sessHackedCheck;
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

                    trail.user_login_id = DBNull.Value.ToString();
                    trail.loggedin_status = "N";
                }
                trail.browser_session_id = HttpContext.Current.Request.Cookies[GlobalVars.AntiXsrfTokenKey] == null ? HttpContext.Current.Session.SessionID : HttpContext.Current.Request.Cookies[GlobalVars.AntiXsrfTokenKey].Value;
                trail.user_session_id = Session["loginsession"] != null ? Session["loginsession"].ToString() : "null";
                trail.referrer = uri != null ? uri.ToString().Length > 100 ? uri.ToString().Substring(0, 100) : uri.ToString() : string.Empty;
                trail.accessed_module = Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath);
                trail.action_performed = action;
                trail.action_description = description.Length > 200 ? description.Substring(0, 200) : description;
                trail.action_status = status;
                trail.tracked_datetime = DateTime.Now;
                trail.loggedin_ip = ipaddress;
                trail.loggedin_mac = macaddress;
                trail.browser_name = browser.Browser;
                trail.browser_version = browser.Version;
                string strUserAgent = Request.UserAgent.ToString().ToLower();
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
                count = insrt.SaveAuditTrail(trail);
            } while (count == 0);
            /*Audit trail*/
        }
        public void callErrorMsg(string msg)
        {
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('\"" + msg + "\')", true);
        }
        protected void BtnSearch_Click(object sender, EventArgs e)
        {
            if (ddlDepartment.SelectedValue == "-1")
            {
                callErrorMsg("Please select department");
            }
            else if (ddlService.SelectedValue == "-1")
            {
                callErrorMsg("Please select service");
            }
            else if (ddlDistrict.SelectedValue == "-1")
            {
                callErrorMsg("Please select district");
            }
            else if(TxtBxCertiifcate.Text =="" || TxtBxCertiifcate.Text == null)
            {
                callErrorMsg("Certiifacte no is Blank");
            }
            else if(!val.validateData(TxtBxCertiifcate.Text, val.Regno))
            {
                callErrorMsg("Invalid Certiifacte no");
            }
            else
            {
                NpgsqlConnection conn = new NpgsqlConnection();
                NpgsqlCommand cmd = new NpgsqlCommand();
                conn.ConnectionString = ConfigurationManager.ConnectionStrings["connect"].ConnectionString;
                cmd.Connection = conn;
                conn.Open();
                NpgsqlTransaction myTrans = conn.BeginTransaction();
                cmd.Transaction = myTrans;
                try
                {
                    cmd.Parameters.Clear();
                    string query = "SELECT socregid,socname,concat(socaddr,' ',pincode)as soc_address,date(regdate) as reg_date,date(regdate::date + INTERVAL '5 years') AS validity,";
                    query = query + " (SELECT date(submitted_at) FROM esociety.application_submission_history where application_submission_history.app_id = society.app_id";
                    query = query + " order by submitted_at desc limit 1) as application_date FROM esociety.society where socdistrict = @socdistrict and socregid = @socregid";
                    cmd.CommandText = query;
                    cmd.Parameters.AddWithValue("@socdistrict", Convert.ToInt16(ddlDistrict.SelectedValue));
                    cmd.Parameters.AddWithValue("@socregid", TxtBxCertiifcate.Text.Trim());
                    NpgsqlDataReader rd = cmd.ExecuteReader();
                    if (rd.Read())
                    {
                        TextBox2.Text = rd["socname"].ToString().Trim();
                        TextBox3.Text = rd["soc_address"].ToString().Trim();  
                        TextBox4.Text = Convert.ToDateTime(rd["application_date"], french).Date.ToString().Trim();
                        TextBox5.Text = rd["socregid"].ToString().Trim();
                        TextBox6.Text = Convert.ToDateTime(rd["reg_date"], french).Date.ToString().Trim(); //rd["reg_date"].ToString().Trim();
                        TextBox7.Text = Convert.ToDateTime(rd["validity"], french).Date.ToString().Trim(); //rd["validity"].ToString().Trim();
                        data.Visible = true;
                    }
                    else
                    {
                        data.Visible = false;
                    }
                    rd.Close();
                }
                catch (NpgsqlException ex)
                {
                    CreateLogFiles Err = new CreateLogFiles();
                    Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "BtnSearch_Click()" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
                    myTrans.Rollback(); 
                }
                finally
                {
                    conn.Close();
                }
            }
        }
    }
}