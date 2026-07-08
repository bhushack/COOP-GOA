using GoaSocietyRegistration.Development;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace GoaSocietyRegistration.Organization
{
    public partial class admin : System.Web.UI.MasterPage
    {
        Int32 districtid, loginsession;
        private string _antiXsrfTokenValue;
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
                    Response.Redirect("~/OrganizationLogin.aspx");
                }
            }
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            
            if (Check4Tampering() || Page.RouteData.RouteHandler == null)
            {
                // RecordUserAction("Page_Load", "Access request failed. Tampered session", "F");
                SessionManage session = new SessionManage();
                session.__AbandonAdmin(Request, Response);
            }
            else
            {
                HttpContext.Current.Response.AddHeader("Cache-Control", "no-cache, no-store, must-revalidate");
                HttpContext.Current.Response.AddHeader("Pragma", "no-cache");
                HttpContext.Current.Response.AddHeader("Expires", "0");
                if (Session["SessionID"] != null)
                {
                    lbusername.Text = Session["logginname"].ToString();// Session["firstname"].ToString();
                    userdesignation.Text = Session["designation"].ToString();
                    int role_id = (Int32)Session["ROLE"];
                    loginsession = Convert.ToInt32(Session["SessionID"]);
                    districtid = Convert.ToInt32(Session["DistrictID"]);
                    if (!this.IsPostBack)
                    {
                        //RecordUserAction("Page_Load", "Page Load Successful", "S");
                        DataTable dt = this.GetData(role_id);
                        PopulateMenu(dt);
                    }
                }
                else
                {
                    Response.Redirect("~/OrganizationLogin.aspx");
                }
            }
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
                if (Context.Session != null && Session["firstname"] != null && Session["DoTAuthTokAdmin"] != null && Request.Cookies["DoTAuthTokAdmin"] != null)
                {
                    if (!Session["DoTAuthTokAdmin"].ToString().Equals(Request.Cookies["DoTAuthTokAdmin"].Value))
                        sessHackedCheck = true;
                }
                else
                    sessHackedCheck = true;
            }
            return sessHackedCheck;
        }
        private void PopulateMenu(DataTable dt)
        {
            try
            {
                string currentPage = Path.GetFileName(Request.Url.AbsolutePath);

                foreach (DataRow row in dt.Rows)
                {
                    MenuItem menuItem = new MenuItem
                    {

                        Text = row["menu_name"].ToString(),
                        NavigateUrl = Sanitize.InputText(row["menu_url"].ToString()),
                        Selected = Sanitize.InputText(row["menu_url"].ToString()).EndsWith(currentPage, StringComparison.CurrentCultureIgnoreCase)
                    };
                    Menu1.Items.Add(menuItem);
                    //Menu1.Items.Add();
                }
            }
            catch (Exception ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(HttpContext.Current.Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "");
            }            
        }
        private DataTable GetData(int parentMenuId)
        {
            try
            {
                if (Session["SessionID"] != null)
                {
                    NpgsqlConnection conn = new NpgsqlConnection();
                    NpgsqlCommand cmd = new NpgsqlCommand();
                    conn.ConnectionString = ConfigurationManager.ConnectionStrings["connect"].ConnectionString;
                    cmd.Connection = conn;
                    string query = "SELECT concat(mst_menu.iconname,mst_menu.menu_name) as menu_name,mst_menu.menu_url FROM esociety.admin_table";
                    query = query + " JOIN esociety.menurole_link ON menurole_link.role_id = esociety.admin_table.role_id";
                    query = query + " JOIN esociety.mst_menu ON esociety.mst_menu.menu_id = esociety.menurole_link.menu_id";
                    query = query + " WHERE esociety.admin_table.username = @username AND esociety.menurole_link.active = 'Y' order by order_no";
                    DataTable dt = new DataTable();
                    cmd.CommandText = query;

                    cmd.Parameters.AddWithValue("@username", Session["LoginName"].ToString());
                    NpgsqlDataAdapter nda = new NpgsqlDataAdapter();
                    nda.SelectCommand = cmd;
                    nda.Fill(dt);
                    return dt;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(HttpContext.Current.Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "");
                return null;
            }           
        }

        private void RecordUserAction(string action, string description, string status, string app_id, int crud)
        {
            /*Audit trail*/
            int count = 0;
            do
            {
                System.Web.HttpBrowserCapabilities browser = Request.Browser;
                UsersAuditTrails trail = new UsersAuditTrails();
                string uri = HttpContext.Current.Request.Url.AbsoluteUri;
                if (Session["firstname"] != null)
                {
                    trail.admin_login_id = Session["firstname"].ToString();
                    trail.loggedin_status = "Y";
                }
                else
                {

                    trail.admin_login_id = "Invalid Token";
                    trail.loggedin_status = "N";
                }
                trail.app_id = app_id == null ? "Null" : app_id;
                trail.browser_session_id = HttpContext.Current.Request.Cookies[GlobalVars.AntiXsrfTokenKey] == null ? HttpContext.Current.Session.SessionID : HttpContext.Current.Request.Cookies[GlobalVars.AntiXsrfTokenKey].Value;
                trail.admin_session_id = Session["DoTAuthTokAdmin"].ToString() == null ? "Invalid Session" : Session["DoTAuthTokAdmin"].ToString();//login entries value
                trail.referrer = uri != null ? uri.ToString().Length > 100 ? uri.ToString().Substring(0, 100) : uri.ToString() : string.Empty;
                trail.accessed_module = Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath);
                trail.action_performed = action;
                trail.action_description = description.Length > 200 ? description.Substring(0, 200) : description;
                trail.action_status = status;
                trail.tracked_datetime = DateTime.Now;
                trail.loggedin_ip = Utility.getIP();
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
                trail.is_crud = crud;
                if (Session["userfirstname"] != null)
                {
                    trail.admin_login_name = Session["userfirstname"].ToString() + " " + Session["usermiddlename"].ToString() + " " + Session["userlastname"].ToString();

                }
                else
                {
                    trail.admin_login_name = "Invalid UserName";
                }
                Insert insrt = new Insert();
                count = insrt.SaveOrganizationAuditTrail(trail);
            } while (count == 0);
            /*Audit trail*/


        }

        protected void btnHidden_Click(object sender, EventArgs e)
        {
            Session.Clear();
            Session.Abandon();
            Session.RemoveAll();
            Response.Cookies.Clear();
        }

        protected void lkbtnLogout_Click(object sender, EventArgs e)
        {
            RecordUserAction("Read", " Read SearchAuditLogs", "Failed", "NA", 1);
            try
            {
                int value = Utility.logout(Convert.ToInt64(Session["SessionID"]));
                if (value == 1)
                {
                    Session.Clear();
                    Session.Abandon();
                    Session.RemoveAll();
                    Response.Cookies.Clear();
                    Response.Redirect("~/OrganizationLogin.aspx");
                }
                else
                {
                    Response.Write("<script language='javascript'>alert(' Error While logout....Try after sometime ')</script>");
                    Response.Redirect("Admin/AdminDashboard.aspx");
                }
            }
            catch (Exception ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(HttpContext.Current.Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "");
            }
            
        }
    }
}