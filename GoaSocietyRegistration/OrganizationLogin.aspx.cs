using GoaSocietyRegistration.Development;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace GoaSocietyRegistration
{
    public partial class OrganizationLogin : System.Web.UI.Page
    {
        private string _antiXsrfTokenValue;
        string errortext = "";
        string ipaddress = Utility.getIP();
        string macaddress = Utility.GetMACAddress(Utility.getIP());
        Insert ins = new Insert();//
        //string macaddress = Utility.GetMACAddress();
        Validate _val = new Validate();


        protected void Page_Load(object sender, EventArgs e)
        {
            if (Check4Tampering() || Page.RouteData.RouteHandler == null)
            {
                //RecordUserAction("Page_Load", "Access request failed. Tampered session", "F");
                SessionManage session = new SessionManage();
                session.__AbandonAdmin(Request, Response);
            }
            else
            {
            
                if (!IsPostBack)
                {
                    //Response.Cache.SetExpires(DateTime.UtcNow.AddDays(-1));
                    //Response.Cache.SetCacheability(HttpCacheability.NoCache);
                    //Response.Cache.SetNoStore();
                    //Response.ClearHeaders();
                    HttpContext.Current.Response.AddHeader("Cache-Control", "no-cache, no-store, must-revalidate");
                    HttpContext.Current.Response.AddHeader("Pragma", "no-cache");
                    HttpContext.Current.Response.AddHeader("Expires", "0");
                    Session["firstname"] = null;
                    //here re changes are there
                    string captcha = Utility.GenerateRandomString(6, "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789");
                    image.ImageUrl = "Development/admincaptcha.aspx?" + DateTime.Now.Ticks.ToString() + "&capstr=" + Server.HtmlEncode(captcha.ToString());
                    Session["adminCaptchasoc"] = captcha;

                  

                }
                if (Session["Enc_Random"] == null)
                {
                    var random = Encryption.Encrypt.GenerateRandomSaltAES();
                    var randomiv = Encryption.Encrypt.GenerateRandomIvAES();
                    Session["Enc_Random"] = random;
                    Session["Enc_Vector"] = randomiv;
                }
                if (Session["Sess_rndNo"] == null)
                {
                     
                    byte[] rngBytes = new byte[4];
                    RandomNumberGenerator.Create().GetBytes(rngBytes);

                    string myInt = BitConverter.ToString(rngBytes);
                    Session["Sess_rndNo"] = myInt;
                    string abc = Session["Sess_rndNo"].ToString();
                }

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

                    trail.admin_login_id = txtbxusername.Text != "" ? txtbxusername.Text : "Invalid UserId";
                    trail.loggedin_status = "N";
                }
                trail.app_id = app_id == null ? "Null" : app_id;
                trail.browser_session_id = HttpContext.Current.Request.Cookies[GlobalVars.AntiXsrfTokenKey] == null ? HttpContext.Current.Session.SessionID : HttpContext.Current.Request.Cookies[GlobalVars.AntiXsrfTokenKey].Value;
                trail.admin_session_id = "NA";
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
                    trail.admin_login_name = trail.admin_login_id != "Invalid UserId" ? Utility.getAdminName(txtbxusername.Text.Trim()) : "Invalid Username";
                   
                }
               
               
                count = ins.SaveOrganizationAuditTrail(trail);
            } while (count == 0);
            /*Audit trail*/


        }
        //public int SaveAuditTrail(UsersAuditTrails trial)
        //{
        //    NpgsqlConnection conn = new NpgsqlConnection();
        //    NpgsqlCommand cmd = new NpgsqlCommand();
        //    conn.ConnectionString = ConfigurationManager.ConnectionStrings["connect"].ConnectionString;
        //    cmd.Connection = conn;
        //    try
        //    {
        //        conn.Open();
        //        string query = "INSERT INTO esociety.user_audit_trail( user_login_id,browser_session_id, user_session_id, loggedin_status, referrer, accessed_module,";
        //        query = query + " action_performed, action_description, action_status, ipaddress, macaddress,  tracked_datetime, browser_name,browser_version,device_type ) VALUES (@user_login_id,@browser_session_id, ";
        //        query = query + " @user_session_id, @loggedin_status, @referrer, @accessed_module, @action_performed, @action_description, @action_status, @ipaddress, @macaddress,";
        //        query = query + "  @tracked_datetime,@browser_name,@browser_version,@device_type)";
        //        cmd.CommandText = query;
        //        cmd.Parameters.AddWithValue("@user_login_id", trial.user_login_id);
        //        cmd.Parameters.AddWithValue("@browser_session_id", trial.browser_session_id);
        //        cmd.Parameters.AddWithValue("@user_session_id", trial.user_session_id);
        //        cmd.Parameters.AddWithValue("@loggedin_status", trial.loggedin_status);
        //        cmd.Parameters.AddWithValue("@referrer", trial.referrer);
        //        cmd.Parameters.AddWithValue("@accessed_module", trial.accessed_module);
        //        cmd.Parameters.AddWithValue("@action_performed", trial.action_performed);
        //        cmd.Parameters.AddWithValue("@action_description", trial.action_description);
        //        cmd.Parameters.AddWithValue("@action_status", trial.action_status);
        //        cmd.Parameters.AddWithValue("@ipaddress", trial.loggedin_ip);
        //        cmd.Parameters.AddWithValue("@macaddress", trial.loggedin_mac);
        //        cmd.Parameters.AddWithValue("@tracked_datetime", trial.tracked_datetime);
        //        cmd.Parameters.AddWithValue("@browser_name", trial.browser_name);
        //        cmd.Parameters.AddWithValue("@browser_version", trial.browser_version);
        //        cmd.Parameters.AddWithValue("@device_type", trial.device_type);
        //        cmd.ExecuteNonQuery();
        //        return 1;
        //    }
        //    catch (NpgsqlException ex)
        //    {
        //        return 0;
        //    }
        //    finally
        //    {
        //        conn.Close();
        //    }

        //}
        public Int32 getlogin_sess_count()
        {
            //keep track of login and logout and to update logout time based on this...
            NpgsqlConnection connect = new NpgsqlConnection();
            NpgsqlCommand cmd = new NpgsqlCommand();
            connect.ConnectionString = ConfigurationManager.ConnectionStrings["connect"].ConnectionString;
            cmd.Connection = connect;
            try
            {
                string value = "";
                connect.Open();
                cmd.Parameters.Clear();
                string query = "select nextval('esociety.login_count')";
                cmd.CommandText = query;
                value = Convert.ToString(cmd.ExecuteScalar());
                int i = Convert.ToInt32(value);
                return i;
            }
            catch (Exception ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "getlogin_sess_count()" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
                RecordUserAction("Create", "Create Session", "Failed", "NA", 2);
                ///RecordUserAction("getlogin_sess_count", ex.Message, "F");
                return 0;
            }
            finally
            {
                connect.Close();
            }
        }
        public void refreshadminCaptcha()
        {
            try
            {
                TextBox1.Text = ""; //here re changes are there
                string captcha = Utility.GenerateRandomString(6, "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789");
                image.ImageUrl = "Development/admincaptcha.aspx?" + DateTime.Now.Ticks.ToString() + "&capstr=" + Server.HtmlEncode(captcha.ToString());
                Session["adminCaptchasoc"] = captcha;
            }
            catch (Exception ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "refreshadminCaptcha()" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
            }
        }

        private void ClearSensitiveInputs(string errorMsg)
        {
            try
            {
                refreshadminCaptcha();
                txtbxusername.Text = string.Empty;
                txtbxPassword.Text = string.Empty;
                lberror.Text = errorMsg;
                HDShaPass.Value = string.Empty;
            }
            catch (Exception ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "ClearSensitiveInputs()" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
            }
        }
        private string generateNewSessionId()
        {
            System.Web.SessionState.SessionIDManager manager = new System.Web.SessionState.SessionIDManager();
            string oldId = manager.GetSessionID(Context);
            string newId = manager.CreateSessionID(Context);
            bool isAdd = false, isRedir = false;
            try
            {
                manager.SaveSessionID(Context, newId, out isRedir, out isAdd);
                HttpApplication ctx = (HttpApplication)HttpContext.Current.ApplicationInstance;
                HttpModuleCollection mods = ctx.Modules;
                System.Web.SessionState.SessionStateModule ssm = (SessionStateModule)mods.Get("Session");
                System.Reflection.FieldInfo[] fields = ssm.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Instance);
                SessionStateStoreProviderBase store = null;
                System.Reflection.FieldInfo rqIdField = null, rqLockIdField = null, rqStateNotFoundField = null;
                foreach (System.Reflection.FieldInfo field in fields)
                {
                    if (field.Name.Equals("_store")) store = (SessionStateStoreProviderBase)field.GetValue(ssm);
                    if (field.Name.Equals("_rqId")) rqIdField = field;
                    if (field.Name.Equals("_rqLockId")) rqLockIdField = field;
                    if (field.Name.Equals("_rqSessionStateNotFound")) rqStateNotFoundField = field;
                }
                object lockId = rqLockIdField.GetValue(ssm);
                if ((lockId != null) && (oldId != null)) store.ReleaseItemExclusive(Context, oldId, lockId);
                rqStateNotFoundField.SetValue(ssm, true);
                rqIdField.SetValue(ssm, newId);

            }
            catch (Exception ex)
            {
                newId = null;
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "generateNewSessionId()" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
            }


            return newId;
        }
        public int setlogs()
        {

            Int64 loginsession = getlogin_sess_count();
            string user_fullname = Session["userfirstname"].ToString() + " " + Session["usermiddlename"].ToString() + " " + Session["userlastname"].ToString();
            Session["loginsession_count"] = loginsession;
            Session["SessionID"] = loginsession;
            NpgsqlConnection conn = new NpgsqlConnection();
            NpgsqlCommand cmd = new NpgsqlCommand();
            conn.ConnectionString = ConfigurationManager.ConnectionStrings["connect"].ConnectionString;
            cmd.Connection = conn;
            try
            {
                conn.Open();
                cmd.CommandText = "INSERT INTO esociety.loginentries(login_sess_count, user_loginname, user_logintime, user_logouttime, ipaddress, macaddress, user_fullname) VALUES(@login_sess_count, @user_loginname, current_timestamp, current_timestamp, @ipaddress, @macaddress, @user_fullname)";
                cmd.Parameters.AddWithValue("@user_loginname", txtbxusername.Text);
                cmd.Parameters.AddWithValue("@login_sess_count", loginsession);
                cmd.Parameters.AddWithValue("@ipaddress", ipaddress);
                cmd.Parameters.AddWithValue("@macaddress", macaddress);
                cmd.Parameters.AddWithValue("@user_fullname", user_fullname);
                cmd.ExecuteNonQuery();
                //RecordUserAction("setlogs", "Login Session generated", "S");
                return 1;

            }
            catch (NpgsqlException ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "setlogs()" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
                //RecordUserAction("setlogs", ex.Message, "F");
                lberror.ForeColor = System.Drawing.Color.Red;
                lberror.Text = "Login Failed";
                return 0;
            }
            finally
            {
                conn.Close();
            }

        }
        protected void loginsuccessfull(string username)
        {
            string status = null;
            NpgsqlConnection conn = new NpgsqlConnection();
            NpgsqlCommand cmd = new NpgsqlCommand();
            conn.ConnectionString = ConfigurationManager.ConnectionStrings["connect"].ConnectionString;
            cmd.Connection = conn;
            try
            {
                conn.Open();
                string query = "SELECT district_id,role_id,username,active,userfirstname,usermiddlename, userlastname, user_designation from esociety.admin_table where username=@username and active='Y' ";
                cmd.CommandText = query;
                cmd.Parameters.AddWithValue("@username", username);
                NpgsqlDataReader rd = cmd.ExecuteReader();
                if (rd.Read())
                {
                    status = Server.HtmlEncode(rd["active"].ToString());
                    if (status == "Y")
                    {
                        Session["ROLE"] = Convert.ToInt32(Server.HtmlEncode(rd["role_id"].ToString()));
                        Session["firstname"] = Server.HtmlEncode(username);
                        Session["userfirstname"] = Server.HtmlEncode(rd["userfirstname"].ToString());
                        Session["usermiddlename"] = Server.HtmlEncode(rd["usermiddlename"].ToString());
                        Session["userlastname"] = Server.HtmlEncode(rd["userlastname"].ToString());
                        Session["designation"] = Server.HtmlEncode(rd["user_designation"].ToString());
                        Session["LoginName"] = Sanitize.InputText(txtbxusername.Text);
                        Session["DistrictID"] = Convert.ToInt32(Server.HtmlEncode(rd["district_id"].ToString()));
                        Session["org_pdf"] = Sanitize.InputText(Guid.NewGuid().ToString());
                        setlogs();
                        rd.Close();
                        RecordUserAction("Read", "Login Successfully", "Success", "NA", 1);
                        bool value = Utility.getPasswordReset(txtbxusername.Text.Trim());
                        if (value)
                        {
                            Response.Redirect("Organization/Dashboard.aspx");
                        }
                        else
                        {
                            Response.Redirect("Organization/ChangePassword.aspx");
                        }

                    }
                    else
                    {
                        refreshadminCaptcha();
                        RecordUserAction("Read", "Login -Account is Deactivated or Kindly Contact Administrator", "Success", "NA", 1);

                        // Label1.Visible = true;
                        lberror.Text = "User-ID or password is Invalid.";
                    }

                }
                else
                {
                    refreshadminCaptcha();
                    RecordUserAction("Read", "Login - Wrong Password Entered", "Failed", "NA", 1);
                    // Label1.Visible = true;
                    lberror.Text = "User-ID or password is Invalid";
                }
                rd.Close();

            }
            catch (NpgsqlException ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "loginsuccessfull()" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
                refreshadminCaptcha();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message, "btnLogin_Click");
                RecordUserAction("Read", "Login ErrorLogs", "Failed", "NA", 1);
            }
            finally
            {
                conn.Close();
            }
        }
        protected void lkbtnorglogin_Click(object sender, EventArgs e)
        {

            ////////////////////////////////////////
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
                    RecordUserAction("Read", "Login to Organization Login", "Failed", "NA", 1);
                    //RecordUserAction("lkbtnorglogin_Click", "Event request failed. Tampered session", "F");                    
                    Response.Redirect("~/Organization/login");
                }
                else
                {
                    if (txtbxusername.Text == "" || txtbxusername.Text == null)
                    {
                        refreshadminCaptcha();
                        RecordUserAction("Read", "LoginUser Name Blank", "Failed", "NA", 1);
                        Response.Write("<script language='javascript'>alert('User-ID or password is Invalid')</script>");
                    }
                    else if (!_val.validateData(txtbxusername.Text, _val.emailformatregex))
                    {
                        refreshadminCaptcha();
                        RecordUserAction("Read", "Login Invalid User Name", "Failed", "NA", 1);
                        Response.Write("<script language='javascript'>alert('User-ID or password is Invalid')</script>");
                    }
                    else
                    {
                        var userHash = HDShaPass.Value.Trim();
                       
                        if (Session["adminCaptchasoc"] != null)
                        {
                            try
                            {


                                if (Session["adminCaptchasoc"].ToString() == TextBox1.Text)
                                {
                           
                                    HttpContext.Current.Session.Remove("adminCaptchasoc");


                                    TextBox1.Text = "";
                                    RecordUserAction("Read", "Login - Entered Correct Captcha", "Success", "NA", 1);
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
                                                RecordUserAction("lkbtnorglogin_Click", "Event request failed. Tampered session", "Failed", "NA", 2);
                                                return;
                                            }
                                            else
                                            {
                                                NpgsqlConnection conn = new NpgsqlConnection();
                                                NpgsqlCommand cmd = new NpgsqlCommand();
                                                conn.ConnectionString = ConfigurationManager.ConnectionStrings["connect"].ConnectionString;
                                                cmd.Connection = conn;
                                                //string encrypt = BaseUtility.Encryptdata(TxtBxpassword.Text);
                                                try
                                                {
                                                    int a = 0;
                                                    conn.Open();
                                                    string query = "SELECT user_password,concat(userfirstname,' ',userlastname) as nameofdr from esociety.admin_table where username=@username and active='Y'";
                                                    cmd.CommandText = query;
                                                    cmd.Parameters.AddWithValue("@username", txtbxusername.Text.Trim());
                                                    NpgsqlDataReader rd = cmd.ExecuteReader();
                                                    if (rd.Read())
                                                    {
                                                        string user_password = Server.HtmlEncode(rd["user_password"].ToString());
                                                        // string salt = Server.HtmlEncode(rd["salt"].ToString());
                                                        if (user_password == null || user_password == "")
                                                        {
                                                            Response.Write("<script language='javascript'>alert('Please Generate your password first.')</script>");
                                                        }
                                                        else
                                                        {
                                                            var checkHash = Encryption.Encrypt.SHA512(Session["Sess_rndNo"].ToString() + user_password);
                                                            //bool isPasswordMatched = verifyPassword(TxtBxpassword.Text, hash_password, salt);
                                                            if (checkHash == userHash)
                                                            {
                                                                a = 1;
                                                                Session.Clear();
                                                                Session.Contents.RemoveAll();
                                                                //2----------------
                                                                string OldID = Context.Session.SessionID;
                                                                string newID = generateNewSessionId();
                                                                session.__ReInitialiseAuthCookieAdmin(Response);                                                                
                                                                Session["logginname"] = rd["nameofdr"].ToString();
                                                                Session.Remove("Enc_Random");
                                                                Session.Remove("Enc_Vector");
                                                                loginsuccessfull(txtbxusername.Text);
                                                                //Login Successfull
                                                            }
                                                            else
                                                            {
                                                                refreshadminCaptcha();
                                                                Response.Write("<script language='javascript'>alert('User-ID or password is Invalid')</script>");
                                                                ClearSensitiveInputs("User-ID or password is Invalid");
                                                                RecordUserAction("Read", "Login User-ID or password is Invalid", "Failed", "NA", 1);

                                                                //Login Failed
                                                            }
                                                        }
                                                    }
                                                    else
                                                    {
                                                        refreshadminCaptcha();
                                                        Response.Write("<script language='javascript'>alert('Invalid User Name or Password.')</script>");
                                                        RecordUserAction("Read", "Login - Invalid Username or Password", "Failed", "NA", 1);
                                                        ClearSensitiveInputs("User-ID or password is Invalid");


                                                    }
                                                    rd.Close();

                                                }
                                                catch (NpgsqlException ex)
                                                {
                                                    refreshadminCaptcha();
                                                    CreateLogFiles Err = new CreateLogFiles();
                                                    Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message, "lkbtnorglogin_Click");
                                                    RecordUserAction("Read", "Login - ErrorLogs", "Failed", "NA", 1);
                                                    ClearSensitiveInputs("User-ID or password is Invalid");

                                                }
                                                finally
                                                {
                                                    conn.Close();
                                                }
                                            }

                                        }
                                        else
                                        {
                                            refreshadminCaptcha();
                                            RecordUserAction("Read", "Login Event Failed", "Failed", "NA", 1);
                                            ClearSensitiveInputs("User-ID or password is Invalid");
                                            session.__GetNewASP_CookieAdmin(Request, Response);
                                            return;
                                        }
                                    }
                                }
                                else
                                {
                                    refreshadminCaptcha();
                                    ClearSensitiveInputs("Wrong Captcha !!");                                   
                                    RecordUserAction("Read", "Login - Entered InCorrect Captcha", "Failed", "NA", 1);
                                }

                            }
                            catch (Exception ex)
                            {
                                CreateLogFiles Err = new CreateLogFiles();
                                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "lkbtnorglogin_Click" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
                                refreshadminCaptcha();
                                ClearSensitiveInputs("Try Again !");
                                Label48.Text = "User-ID or password is Invalid";
                                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "none", "<script> $(window).on('load', function () { $('#errorModal').modal({ backdrop: 'static' });});</script>", false);
                            }
                        }
                        else
                        {

                            ClearSensitiveInputs("Wrong Captcha !");
                            CreateLogFiles Err = new CreateLogFiles();
                            Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), "outside single catpcha null it seems" + " " + "", "lkbtnorglogin_Click" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));

                            RecordUserAction("Read", "btnLogin_Click Event Captcha Null", "Failed", "NA", 1);
                        }
                    }

                }
            }
            else
            {

            }
        }



        protected void generate_pass_Click(object sender, EventArgs e)
        {
            try
            {
                if (Check4Tampering() || Page.RouteData.RouteHandler == null)
                {
                    //RecordUserAction("generate_pass_Click", "Event request failed. Tampered session", "F");
                    RecordUserAction("Redirect", "Redirect to Genrate Password", "Failed", "NA", 2);
                    SessionManage session = new SessionManage();
                    session.__AbandonAdmin(Request, Response);
                }
                else
                {
                    RecordUserAction("Redirect", "Redirect to Gnerate Password", "Success", "NA", 2);
                    Response.Redirect("Organization/GeneratePassword.aspx");
                }
            }
            catch (Exception ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "generate_pass_Click" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
            }
        }

        protected void redirection_Click(object sender, EventArgs e)
        {
            Response.Redirect("OrganizationLogin.aspx");
        }

        protected void lkrefreshCaptcha_Click(object sender, EventArgs e)
        {
            refreshadminCaptcha();
        }
    }
}