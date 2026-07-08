using GoaSocietyRegistration.Development;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace GoaSocietyRegistration
{
    public partial class Society : System.Web.UI.MasterPage
    {
        private string _antiXsrfTokenValue;
        string ipaddress = Utility.getIP();
        string macaddress = Utility.GetMACAddress(Utility.getIP());
        Insert insrt = new Insert();
        protected void Page_Init(object sender, EventArgs e)
        {
            //First, check for the existence of the Anti-XSS cookie
            var requestCookie = Request.Cookies[GlobalVars.AntiXsrfTokenKey];
            Guid requestCookieGuidValue;

            //If the CSRF cookie is found, parse the token from the cookie.
            //Then, set the global page variable and view state user key.
            //The global variable will be used to validate that it matches in the view state form field in the Page.PreLoad method.
            if (requestCookie != null && Guid.TryParse(requestCookie.Value, out requestCookieGuidValue))
            {
                //Set the global token variable so the cookie value can be
                //validated against the value in the view state form field in
                //the Page.PreLoad method.
                _antiXsrfTokenValue = requestCookie.Value;

                //Set the view state user key, which will be validated by the
                //framework during each request
                Page.ViewStateUserKey = _antiXsrfTokenValue;
            }
            //If the CSRF cookie is not found, then this is a new session.
            else
            {
                //Generate a new Anti-XSRF token
                _antiXsrfTokenValue = Guid.NewGuid().ToString("N");

                //Set the view state user key, which will be validated by the
                //framework during each request
                Page.ViewStateUserKey = _antiXsrfTokenValue;

                //Create the non-persistent CSRF cookie
                var responseCookie = new HttpCookie(GlobalVars.AntiXsrfTokenKey)
                {
                    //Set the HttpOnly property to prevent the cookie from
                    //being accessed by client side script
                    HttpOnly = true,

                    //Add the Anti-XSRF token to the cookie value
                    Value = _antiXsrfTokenValue
                };

                //If we are using SSL, the cookie should be set to secure to
                //prevent it from being sent over HTTP connections
                if (FormsAuthentication.RequireSSL && Request.IsSecureConnection)
                    responseCookie.Secure = true;

                //Add the CSRF cookie to the response
                Response.Cookies.Set(responseCookie);
            }

            Page.PreLoad += master_Page_PreLoad;
        }

        protected void master_Page_PreLoad(object sender, EventArgs e)
        {
            //During the initial page load, add the Anti-XSRF token and user
            //name to the ViewState
            if (!IsPostBack)
            {
                //Set Anti-XSRF token
                ViewState[GlobalVars.AntiXsrfTokenKey] = Page.ViewStateUserKey;

                //If a user name is assigned, set the user name
                ViewState[GlobalVars.AntiXsrfUserNameKey] = Context.User.Identity.Name ?? String.Empty;
            }
            //During all subsequent post backs to the page, the token value from
            //the cookie should be validated against the token in the view state
            //form field. Additionally user name should be compared to the
            //authenticated users name
            else
            {
                //Validate the Anti-XSRF token
                if ((string)ViewState[GlobalVars.AntiXsrfTokenKey] != _antiXsrfTokenValue
                || (string)ViewState[GlobalVars.AntiXsrfUserNameKey] != (Context.User.Identity.Name ?? String.Empty))
                {
                    Response.Redirect("LoginModule.aspx");
                }
            }
        }
        public string setloginName(string token)
        {
            string name = "";
            NpgsqlConnection conn = new NpgsqlConnection();
            NpgsqlCommand cmd = new NpgsqlCommand();
            conn.ConnectionString = ConfigurationManager.ConnectionStrings["connect"].ConnectionString;
            cmd.Connection = conn;
            try
            {
                conn.Open();
                string query1 = "SELECT user_firstname FROM esociety.usertable where user_loginname=@sid";
                cmd.CommandText = query1;
                cmd.Parameters.AddWithValue("@sid", token);
                NpgsqlDataReader rd = cmd.ExecuteReader();
                if (rd.Read())
                {
                    name = Server.HtmlEncode(rd["user_firstname"].ToString());
                }
                rd.Close();
                RecordUserAction("SetLoginName", "Success", "S");
            }
            catch (NpgsqlException ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "setloginName()" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
                Response.Write("<script language='javascript'>alert('" + "Exection Error" + "')</script>");
                RecordUserAction("SetLoginName", ex.Message, "F");
                name = "";
            }
            finally
            {
                conn.Close();
            }
            return name;
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Check4Tampering() || Page.RouteData.RouteHandler == null)
            {
                RecordUserAction("Page_Load", "Access request failed. Tampered session", "F");
                SessionManage session = new SessionManage();
                session.__Abandon(Request, Response);
            }
            else
            {
                HttpContext.Current.Response.AddHeader("Cache-Control", "no-cache, no-store, must-revalidate");
                HttpContext.Current.Response.AddHeader("Pragma", "no-cache");
                HttpContext.Current.Response.AddHeader("Expires", "0");
                int status = getApplnStatus(Server.HtmlEncode(Session["login_id"].ToString()));

                int renewalstatus = Utility.checkifrenewal(Session["AppID"].ToString());
                if (renewalstatus == 2)
                {
                    schedule1.Visible = true;
                }
                else
                {
                    schedule1.Visible = false;
                }
                Label1.Text = setloginName(Session["login_id"].ToString());

                if(status  == 11 || status == 12)
                {
                    nav_certcopy.Visible = true;
                }
            }
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
                    trail.user_login_id = Server.HtmlEncode(Session["login_id"].ToString());
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
                trail.accessed_module = "Master User";
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
        private bool Check4Tampering()
        {
            bool sessHackedCheck = false;
            if (Request.UrlReferrer != null)
            {
                Uri uri = new Uri(Request.UrlReferrer.ToString());
                string referer = uri.Scheme + Uri.SchemeDelimiter + uri.Host;
                if (!GlobalVars.AntiXsrfRefererHeaderList.Contains(referer))
                    sessHackedCheck = true;
            }
            if (!sessHackedCheck)
            {
                if (Context.Session != null && Session["login_id"] != null && Session["DoTAuthTok"] != null && Request.Cookies["DoTAuthTok"] != null)
                {
                    if (!(Session["DoTAuthTok"].ToString()).Equals(Request.Cookies["DoTAuthTok"].Value))
                        sessHackedCheck = true;
                }
                else
                    sessHackedCheck = true;
            }
            return sessHackedCheck;
        }
        protected void lklogout_Click(object sender, EventArgs e)
        {
            try
            {
                int value = Utility.logout_user(Convert.ToInt64(Session["loginsession_count"]));
                if (value == 1)
                {
                    Session.Clear();
                    Session.Abandon();
                    Session.RemoveAll();
                    Response.Cookies.Clear();

                    Response.Redirect("LoginModule.aspx");
                }
                else
                {
                    Response.Write("<script language='javascript'>alert(' Error While logout....Try after sometime ')</script>");
                    Response.Redirect("Dashboard.aspx");
                }
            }
            catch (Exception ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "lklogout_Click" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
            }            
        }

        protected int getApplnStatus(string token)
        {
            NpgsqlConnection conn = new NpgsqlConnection();
            NpgsqlCommand cmd = new NpgsqlCommand();
            conn.ConnectionString = ConfigurationManager.ConnectionStrings["connect"].ConnectionString;
            cmd.Connection = conn;
            int status = 0;
            try
            {
                conn.Open();
                string query = "SELECT status_id FROM esociety.status_table where login_id=@login";
                cmd.CommandText = query;
                cmd.Parameters.AddWithValue("@login", token);
                NpgsqlDataReader rd = cmd.ExecuteReader();
                if (rd.Read())
                {
                    status = (int)rd["status_id"];
                }
                else
                {
                    status = 0;
                }
                rd.Close();
            }
            catch (Exception ex)
            {
                status = 0;
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "getApplnStatus" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
            }
            finally
            {
                conn.Close();
            }
            return status;
        }

        protected void btnHidden_Click(object sender, EventArgs e)
        {
            Session.Clear();
            Session.Abandon();
            Session.RemoveAll();
            Response.Cookies.Clear();
        }
    }
}