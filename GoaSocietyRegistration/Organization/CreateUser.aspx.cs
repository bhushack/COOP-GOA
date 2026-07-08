using GoaSocietyRegistration.Development;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Encryption;

namespace GoaSocietyRegistration.Organization
{
    public partial class CreateUser : System.Web.UI.Page
    {
        Validate val = new Validate();
        Insert ins = new Insert();
        string ipaddress = Utility.getIP();
        string macaddress = Utility.GetMACAddress(Utility.getIP());
        protected void Page_Load(object sender, EventArgs e)
        {

            if (Check4Tampering() || Page.RouteData.RouteHandler == null)
            {
                RecordUserAction("Page_Load", "Access request failed. Tampered session", "Failed", "NA", 1);
                SessionManage session = new SessionManage();
                session.__AbandonAdmin(Request, Response);
            }
            else if (checkroles())
            {
                Label5.Text = "Permission Denied. Please Contact to your SRO.";
                Label5.ForeColor = System.Drawing.Color.Red;
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "none", "<script> $(window).on('load', function () { $('#permission_error_modal').modal({ backdrop: 'static' });});</script>", false);

            }
            else
            {
                HttpContext.Current.Response.AddHeader("Cache-Control", "no-cache, no-store, must-revalidate");
                HttpContext.Current.Response.AddHeader("Pragma", "no-cache");
                HttpContext.Current.Response.AddHeader("Expires", "0");
                bool value = Utility.getPasswordReset(Session["firstname"].ToString().Trim());
                if (value)
                {
                    if (Session["firstname"] != null)
                    {
                        if (!IsPostBack)
                        {
                            fillDesignation();
                        }
                    }
                    else
                    {
                        Response.Redirect("~/OrganizationLogin.aspx");
                    }
                }
                else
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('As per new Security Guidelines, you are requested to reset the password');window.location ='ChangePassword.aspx';", true);
                }
                
            }
        }
        private bool checkroles()
        {
            bool role_hacked = false;
            List<int> AllowedRoles = new List<int> { 1, 2, 3 };
            if (Context.Session != null && !AllowedRoles.Contains(Convert.ToInt32(Session["ROLE"])))
                role_hacked = true;
            return role_hacked;
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
                trail.loggedin_ip = ipaddress;
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
                count = ins.SaveOrganizationAuditTrail(trail);
            } while (count == 0);
            /*Audit trail*/


        }
        private bool Check4Tampering()
        {

            string referer = null;
            bool sessHackedCheck = false, pagesessionhack = false;
            if (Request.UrlReferrer == null)
            {
                sessHackedCheck = true;
            }
            else
            {
                //HOST NAME check
                if (Request.UrlReferrer != null)
                {
                    Uri uri = new Uri(Request.UrlReferrer.ToString());
                    referer = uri.Scheme + Uri.SchemeDelimiter + uri.Host;
                    if (!GlobalVars.AntiXsrfRefererHeaderList.Contains(referer))
                        sessHackedCheck = true;
                }
                //PAGE REFERRER White Listing
                if (!GlobalVars.AntiPageRequest.Contains(Path.GetFileName(Page.AppRelativeVirtualPath)))
                {
                    pagesessionhack = true;
                }
                if (!(sessHackedCheck || pagesessionhack))
                {
                    //Session checking
                    if (Context.Session != null && Session["firstname"] != null && Session["DoTAuthTokAdmin"] != null && Request.Cookies["DoTAuthTokAdmin"] != null)
                    {
                        //Double Authentication Cookie
                        if (!Session["DoTAuthTokAdmin"].ToString().Equals(Request.Cookies["DoTAuthTokAdmin"].Value))
                        {
                            sessHackedCheck = true;
                        }
                        else
                        {
                            //HttpCookie user_cookie = new HttpCookie("DoTAuthTokAdmin");
                            //user_cookie.HttpOnly = true;
                            //HttpContext.Current.Session["DoTAuthTokAdmin"] = Guid.NewGuid().ToString();
                            //user_cookie.Value = HttpContext.Current.Session["DoTAuthTokAdmin"].ToString();
                            //HttpContext.Current.Response.Cookies.Add(user_cookie);
                            //CSRF token
                            string currentPageName = Request.Url.Segments[Request.Url.Segments.Length - 1];
                            string previousPageName = Request.UrlReferrer.Segments[Request.UrlReferrer.Segments.Length - 1];
                            if (currentPageName == previousPageName)
                            {
                                if (HttpContext.Current.Request.HttpMethod == "POST" && (HttpContext.Current.Session["_csrfToken"].Equals(HiddenField1.Value)))
                                {
                                    HttpContext.Current.Session["_csrfToken"] = Guid.NewGuid().ToString();
                                    HiddenField1.Value = HttpContext.Current.Session["_csrfToken"].ToString();
                                    sessHackedCheck = false;
                                }
                                else if (HttpContext.Current.Request.HttpMethod == "GET")
                                {
                                    HttpContext.Current.Session["_csrfToken"] = Guid.NewGuid().ToString();
                                    HiddenField1.Value = HttpContext.Current.Session["_csrfToken"].ToString();
                                    sessHackedCheck = false;
                                }
                                else
                                {

                                    sessHackedCheck = true;
                                }
                            }
                            else
                            {
                                HttpContext.Current.Session["_csrfToken"] = Guid.NewGuid().ToString();
                                HiddenField1.Value = HttpContext.Current.Session["_csrfToken"].ToString();
                                sessHackedCheck = false;
                            }

                        }
                    }
                    else
                        sessHackedCheck = true;
                }
                else
                {
                    sessHackedCheck = true;
                }
            }
            return sessHackedCheck;
        }
        private void fillDesignation()
        {
            NpgsqlConnection conn = new NpgsqlConnection();
            NpgsqlCommand cmd = new NpgsqlCommand();
            conn.ConnectionString = ConfigurationManager.ConnectionStrings["connect"].ConnectionString;
            cmd.Connection = conn;
            NpgsqlDataAdapter adapter;
            DataSet ds;

            try
            {

                if (Session["ROLE"] != null)
                {
                    string query = "";
                    if (Convert.ToInt32(Session["ROLE"].ToString()) == 1)
                    {
                        query = "select rolename,role_id from esociety.mst_role where role_id!=1";
                    }
                    else if (Convert.ToInt32(Session["ROLE"].ToString()) == 2)
                    {
                        query = "select rolename,role_id from esociety.mst_role where role_id!=1 and role_id!=2";
                    }
                    else if (Convert.ToInt32(Session["ROLE"].ToString()) == 3)
                    {
                        query = "select rolename,role_id from esociety.mst_role where role_id!=1 and role_id!=2 and role_id!=3";
                    }
                    cmd = new NpgsqlCommand(query, conn);
                    conn.Open();
                    adapter = new NpgsqlDataAdapter(cmd);
                    ds = new DataSet();
                    adapter.Fill(ds, "esociety.mst_role");
                    ddlDesignationtype.DataSource = ds.Tables[0];
                    ddlDesignationtype.DataTextField = "rolename";
                    ddlDesignationtype.DataValueField = "role_id";
                    ddlDesignationtype.DataBind();
                    ddlDesignationtype.Items.Insert(0, new ListItem("-- Select --", "-1"));
                    cmd.Dispose();
                    adapter.Dispose();
                    ds = null;
                }
                else
                {
                    CreateLogFiles Err = new CreateLogFiles();
                    Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), "Session null", "fillDesignation");
                    label1.Visible = true;
                    lberror.ForeColor = System.Drawing.Color.Red;
                    lberror.Text = "Error While Loading.....Try  Again";
                }

            }
            catch (NpgsqlException ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "fillDesignation()"+ " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
                label1.Visible = true;
                lberror.ForeColor = System.Drawing.Color.Red;
                lberror.Text = "Error While Loading.....Try Loading Again";
            }
            finally
            {
                conn.Close();
            }
        }

        protected void lkbtnsubmit_Click(object sender, EventArgs e)
        {
            SessionManage session = new SessionManage();
            Page.Validate();
            if (HttpContext.Current.Request.HttpMethod == "POST" && Page.IsValid)
            {
                bool sessHackedCheck = false;
                if (Request.UrlReferrer != null)
                {
                    Uri uri = new Uri(Request.UrlReferrer.ToString());
                    string referrer = uri.Scheme + Uri.SchemeDelimiter + uri.Host;
                    if (!GlobalVars.AntiXsrfRefererHeaderList.Contains(referrer))
                        sessHackedCheck = true;
                }
                if (sessHackedCheck)
                {
                    session.__GetNewASP_CookieAdmin(Request, Response);
                    RecordUserAction("submit_password_Click", "Event request failed. Tampered session", "Failed", "NA", 1);
                    Response.Redirect("~/OrganizationLogin.aspx");
                }
                else
                {
                    if (Session["firstname"] != null)
                    {
                        string uname = Session["firstname"].ToString();
                        if (TxtBxFirstname.Text == "" || TxtBxFirstname.Text == null)
                        {
                            TxtBxFirstname.Focus();
                            label1.Visible = true;
                            lberror.Text = "First Name is Blank";
                        }
                        else if (!val.validateData(TxtBxFirstname.Text, val.name))
                        {
                            TxtBxFirstname.Focus();
                            label1.Visible = true;
                            lberror.Text = "First Name is Invalid";
                        }
                        else if (!val.validateData(TxtBxMiddleName.Text, val.name))
                        {
                            TxtBxMiddleName.Focus();
                            label1.Visible = true;
                            lberror.Text = "Middle Name is Invalid";
                        }
                        else if (TxtbxLastName.Text == null || TxtbxLastName.Text == "")
                        {
                            TxtbxLastName.Focus();
                            label1.Visible = true;
                            lberror.Text = "Last Name is Invalid";
                        }
                        else if (!val.validateData(TxtbxLastName.Text, val.name))
                        {
                            TxtbxLastName.Focus();
                            label1.Visible = true;
                            lberror.Text = "Last Name is Invalid";
                        }
                        else if (TxtBxEmailid.Text == null || TxtBxEmailid.Text == "")
                        {
                            TxtBxEmailid.Focus();
                            label1.Visible = true;
                            lberror.Text = "Email is Blank";
                        }
                        else if (!val.validateData(TxtBxEmailid.Text, val.emailformatregex))
                        {
                            TxtBxEmailid.Focus();
                            label1.Visible = true;
                            lberror.Text = "Email is Invalid";
                        }
                        else if (TxtBxMobileNo.Text == "" || TxtBxMobileNo.Text == null)
                        {
                            TxtBxMobileNo.Focus();
                            label1.Visible = true;
                            lberror.Text = "Mobile is Blank";
                        }
                        else if (!val.validateData(TxtBxMobileNo.Text, val.mobile_regex))
                        {
                            TxtBxMobileNo.Focus();
                            label1.Visible = true;
                            lberror.Text = "Mobile is Invalid";
                        }
                        else if (Convert.ToInt32(ddlDesignationtype.SelectedValue) == -1)
                        {
                            ddlDesignationtype.Focus();
                            label1.Visible = true;
                            lberror.Text = "Designation is Not Selected";
                        }
                        else
                        {

                            session.__ReInitialiseAuthCookieAdmin(Response);
                            if (Request.UrlReferrer != null)
                            {
                                Uri uri = new Uri(Request.UrlReferrer.ToString());
                                string referer = uri.Scheme + Uri.SchemeDelimiter + uri.Host;
                                if ((Session["DoTAuthTokAdmin"] != null && Response.Cookies["DoTAuthTokAdmin"] != null)
                                || !GlobalVars.AntiXsrfRefererHeaderList.Contains(referer))
                                {
                                    if (!Session["DoTAuthTokAdmin"].ToString().Equals(Response.Cookies["DoTAuthTokAdmin"].Value))
                                    {
                                        session.__GetNewASP_CookieAdmin(Request, Response);
                                        RecordUserAction("btnLogin_Click", "Event request failed. Tampered session", "Failed", "NA", 1);
                                        return;
                                    }
                                    else
                                    {
                                        NpgsqlConnection conn = new NpgsqlConnection();
                                        NpgsqlCommand cmd = new NpgsqlCommand();
                                        conn.ConnectionString = ConfigurationManager.ConnectionStrings["connect"].ConnectionString;
                                        cmd.Connection = conn;
                                        try
                                        {
                                            conn.Open();
                                            string query = "INSERT INTO esociety.admin_table(username, userfirstname, usermiddlename, userlastname, useremailid, usermobileno, user_designation,";
                                            query = query + " created_by, created_at, ipaddress, macaddress, active, role_id, passwordgenerated,district_id) VALUES(@username, @userfirstname, @usermiddlename, @userlastname, @useremailid, @usermobileno,";
                                            query = query + " @user_designation, @created_by, current_timestamp, @ipaddress,@macaddress, 'Y', @role_id,@passwordgenerated,@district_id)";
                                            cmd.CommandText = query;
                                            cmd.Parameters.AddWithValue("@username", TxtBxEmailid.Text);
                                            cmd.Parameters.AddWithValue("@userfirstname", TxtBxFirstname.Text);
                                            cmd.Parameters.AddWithValue("@usermiddlename", (TxtBxMiddleName.Text != "") ? TxtBxMiddleName.Text : " ");
                                            cmd.Parameters.AddWithValue("@userlastname", TxtbxLastName.Text);
                                            cmd.Parameters.AddWithValue("@useremailid", TxtBxEmailid.Text);
                                            cmd.Parameters.AddWithValue("@usermobileno", Encrypt.Encryptt(TxtBxMobileNo.Text));
                                            cmd.Parameters.AddWithValue("@user_designation", ddlDesignationtype.SelectedItem.Text);
                                            cmd.Parameters.AddWithValue("@created_by", uname);
                                            cmd.Parameters.AddWithValue("@ipaddress", ipaddress);
                                            cmd.Parameters.AddWithValue("@macaddress", macaddress);
                                            cmd.Parameters.AddWithValue("@role_id", Convert.ToInt32(ddlDesignationtype.SelectedValue));
                                            cmd.Parameters.AddWithValue("@passwordgenerated", 1);
                                            cmd.Parameters.AddWithValue("@district_id", Convert.ToInt32(Session["DistrictID"].ToString()));
                                            cmd.ExecuteNonQuery();
                                            RecordUserAction("Create", "User created Successfully.", "Success", "NA", 1);
                                            Response.Write("<script language='javascript'>alert('User Created')</script>");

                                            SendSMS sms = new SendSMS();
                                            sms.send_sms_dep_user(TxtBxMobileNo.Text);
                                            //modal will come here ankur
                                            lkbtnsubmit.Enabled = false;
                                        }
                                        catch (NpgsqlException ex)
                                        {
                                            RecordUserAction("Create", "User created Failed.", "Failed", "NA", 1);
                                            CreateLogFiles Err = new CreateLogFiles();
                                            Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "lkbtnsubmit_Click" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
                                        } 
                                        finally
                                        {
                                            conn.Close();
                                        }

                                    }
                                }
                            }
                            else
                            {
                                //RecordUserAction("lkbtnsubmit_Click", "Event failed. Failed login attempt", "F");
                                session.__GetNewASP_CookieAdmin(Request, Response);
                                return;
                            }
                        }
                    }
                }
            }
            else
            {
                //RecordUserAction("lkbtnsubmit_Click", "Event failed. Method is not post", "F");
                session.__GetNewASP_CookieAdmin(Request, Response);
                return;
            }
        }

        protected void permission_Click(object sender, EventArgs e)
        {

            Response.Redirect("Dashboard.aspx");

        }
    }
}