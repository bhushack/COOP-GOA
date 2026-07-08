using GoaSocietyRegistration.Development;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.Security;
using System.Web.Services;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace GoaSocietyRegistration
{

    [Serializable]
    public partial class LoginModule : System.Web.UI.Page
    {


        //public static string MyMethod(string name)
        //{
        //    name = HttpContext.Current.Server.HtmlEncode(name);
        //    return "Hello " + name;
        //}
        SendSMS sms = new SendSMS();
        private string _antiXsrfTokenValue;
        // static int flag = 0;
        string errortext = "";
        string ipaddress = Utility.getIP();
        string macaddress = Utility.GetMACAddress(Utility.getIP());
        //string macaddress = Utility.GetMACAddress();
        Validate _val = new Validate();

        SessionManage session = new SessionManage();
    
        protected void Page_Load(object sender, EventArgs e)
        {            
            if (Check4Tampering() || Page.RouteData.RouteHandler == null)
            {
                RecordUserAction("Page_Load", "Access request failed. Tampered session", "F");
                session.__Abandon(Request, Response);
            }
            else
            {
                HttpContext.Current.Response.AddHeader("Cache-Control", "no-cache, no-store, must-revalidate");
                HttpContext.Current.Response.AddHeader("Pragma", "no-cache");
                HttpContext.Current.Response.AddHeader("Expires", "0");
                if (!IsPostBack)
                {
                    Session["login_id"] = null;
                    string strcapcha = Utility.GenerateRandomString(6, "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789");
                    ImgCaptcha.ImageUrl = "../Development/GenerateCaptcha.aspx?" + DateTime.Now.Ticks.ToString() + "&capstr=" + Server.HtmlEncode(strcapcha.ToString());
                    Session["captcha"] = strcapcha;
                    // string captcha = Utility.GenerateRandomString(8, "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789");
                    // image.ImageUrl= "../Development/admincaptcha.aspx?" + DateTime.Now.Ticks.ToString() + "&capstr=" + Server.HtmlEncode(captcha.ToString());
                    // Session["adminCaptcha"] = captcha;
                }
                if (Session["Enc_Random"] == null)
                {
                    var random = Encryption.Encrypt.GenerateRandomSaltAES();
                    var randomiv = Encryption.Encrypt.GenerateRandomIvAES();
                    Session["Enc_Random"] = random;
                    Session["Enc_Vector"] = randomiv;
                }
                //if (flag == 1)
                //{
                //    Page.ClientScript.RegisterStartupScript(this.GetType(), " ", "submitPoll()", true);
                //}
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
                trail.user_session_id = Session["loginsession_count"] != null ? Session["loginsession_count"].ToString() : "null";
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
                string query = "INSERT INTO esociety.user_audit_trail( user_login_id,browser_session_id, user_session_id, loggedin_status, referrer, accessed_module,";
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
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "SaveAuditTrail()" + " " + System.IO.Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));

                return 0;
            }
            finally
            {
                conn.Close();
            }

        }

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

                RecordUserAction("getlogin_sess_count", ex.Message, "F");
                return 0;
            }
            finally
            {
                connect.Close();
            }
        }
        public Int32 getlogin_sess_count_user()
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
                string query = "select nextval('esociety.login_count_user')";
                cmd.CommandText = query;
                value = Convert.ToString(cmd.ExecuteScalar());
                int i = Convert.ToInt32(value);
                return i;
            }
            catch (Exception ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "getlogin_sess_count_user()" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));

                RecordUserAction("getlogin_sess_count_user", ex.Message, "F");
                return 0;
            }
            finally
            {
                connect.Close();
            }
        }
        public int setlogs()
        {
            if (Session["temp_loginsession"] != null)
            {
                Int64 loginsession = getlogin_sess_count_user();
                
                Session["loginsession_count"] = loginsession;
                NpgsqlConnection conn = new NpgsqlConnection();
                NpgsqlCommand cmd = new NpgsqlCommand();
                conn.ConnectionString = ConfigurationManager.ConnectionStrings["connect"].ConnectionString;
                cmd.Connection = conn;
                try
                {
                    conn.Open();
                    string query = "INSERT INTO esociety.loginentries_user(login_sess_count, user_loginname, user_logintime, user_logouttime, ipaddress, macaddress) VALUES(@login_sess_count, @user_loginname, current_timestamp, current_timestamp, @ipaddress, @macaddress)";
                    cmd.CommandText = query;
                    cmd.Parameters.AddWithValue("@login_sess_count", loginsession);
                    cmd.Parameters.AddWithValue("@user_loginname", Session["temp_loginsession"].ToString());
                    cmd.Parameters.AddWithValue("@ipaddress", ipaddress);
                    cmd.Parameters.AddWithValue("@macaddress", macaddress);                   
                    cmd.ExecuteNonQuery();
                    RecordUserAction("setlogs", "Login Session generated", "S");
                    return 1;

                }
                catch (NpgsqlException ex)
                {
                    CreateLogFiles Err = new CreateLogFiles();
                    Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "setlogs()" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));

                    RecordUserAction("setlogs", ex.Message, "F");
                    lbstatus.ForeColor = System.Drawing.Color.Red;
                    lbstatus.Text = "Login Failed";
                    return 0;
                }
                finally
                {
                    conn.Close();
                }
            }
            else {
                return 0;

            }

        }

        //protected void lkbtnorglogin_Click(object sender, EventArgs e)
        //{
        //    if(Session["adminCaptcha"].ToString()== TextBox1.Text)
        //    {
        //        string encrypt_password = Utility.Encryptdata(txtbxPassword.Text);
        //        NpgsqlConnection connect = new NpgsqlConnection();
        //        NpgsqlCommand cmd = new NpgsqlCommand();
        //        connect.ConnectionString = ConfigurationManager.ConnectionStrings["connect"].ConnectionString;
        //        cmd.Connection = connect;
        //        connect.Open();
        //        NpgsqlTransaction myTrans = connect.BeginTransaction();
        //        cmd.Transaction = myTrans;
        //        if (txtbxusername.Text == "" || txtbxusername.Text == null)
        //        {
        //            RecordUserAction("lkbtnorglogin_Click", "UserName is Blank", "F");
        //            Response.Write("<script language='javascript'>alert('UserName is Blank!!!')</script>");
        //        }
        //        else if (!_val.validateData(txtbxusername.Text, _val.emailformatregex))
        //        {
        //            RecordUserAction("lkbtnorglogin_Click", "Username Invalid", "F");
        //            Response.Write("<script language='javascript'>alert('UserName is Invalid!!!')</script>");
        //        }
        //        else if (txtbxPassword.Text == "" || txtbxPassword.Text == null)
        //        {
        //            RecordUserAction("lkbtnorglogin_Click", "Password is Blank", "F");
        //            Response.Write("<script language='javascript'>alert('Password is Blank!!!')</script>");
        //        }
        //        else
        //        {
        //            try
        //            {                        
        //                string query = "SELECT districtid,roleid,user_loginname,user_password,user_firstname,user_designation from esociety.usertable where user_loginname=@login and active='Y' and user_password=@password";
        //                cmd.CommandText = query;
        //                cmd.Parameters.AddWithValue("@login", txtbxusername.Text);
        //                cmd.Parameters.AddWithValue("@password", encrypt_password);
        //                NpgsqlDataReader dr = cmd.ExecuteReader();
        //                if (dr.Read())
        //                {

        //                    int roleid = Convert.ToInt32(Server.HtmlEncode(dr["roleid"].ToString()));
        //                    int districtid = Convert.ToInt32(Server.HtmlEncode(dr["districtid"].ToString()));
        //                    string firstname =  Server.HtmlEncode(dr["user_firstname"].ToString());
        //                    Session["designation"] = Server.HtmlEncode(dr["user_designation"].ToString());
        //                    //Response.Write("<script language='javascript'>alert(' Wrong Password or Username ')</script>");
        //                    Session["ROLE"] = roleid;
        //                    dr.Close();
        //                    cmd.Parameters.Clear();
        //                    Int64 loginsession = getlogin_sess_count();
        //                    cmd.CommandText = "INSERT INTO esociety.loginentries(login_sess_count, user_loginname, user_logintime, user_logouttime, ipaddress, macaddress) VALUES(@login_sess_count, @user_loginname, current_timestamp, current_timestamp, @ipaddress, @macaddress)";
        //                    cmd.Parameters.AddWithValue("@user_loginname", txtbxusername.Text);
        //                    cmd.Parameters.AddWithValue("@login_sess_count", loginsession);
        //                    cmd.Parameters.AddWithValue("@ipaddress", ipaddress);
        //                    cmd.Parameters.AddWithValue("@macaddress", macaddress);
        //                    cmd.ExecuteNonQuery();
        //                    myTrans.Commit();
        //                    Session["firstname"] = firstname;
        //                    Session["DistrictID"] = districtid;
        //                    Session["LoginName"] = txtbxusername.Text;
        //                    Session["SessionID"] = loginsession;
        //                    Response.Redirect("Admin/AdminDashboard.aspx");
        //                    RecordUserAction("lkbtnorglogin_Click", "Admin Logged in Successfully", "S");                           
        //                }
        //                else
        //                {
        //                    RecordUserAction("lkbtnorglogin_Click", "Password/Username mismatch", "F");
        //                    Response.Write("<script language='javascript'>alert(' Wrong Password or Username ')</script>");
        //                    refreshadminCaptcha();
        //                }

        //            }
        //            catch (NpgsqlException ex)
        //            {
        //                myTrans.Rollback();
        //                RecordUserAction("lkbtnorglogin_Click", ex.Message, "F");
        //                Response.Write("<script language='javascript'>alert('" + "Log in exception" + "')</script>");
        //                refreshadminCaptcha();
        //            }
        //            finally
        //            {
        //                connect.Close();
        //            }
        //        }
        //    }
        //    else
        //    {
        //        RecordUserAction("lkbtnorglogin_Click", "Wrong Captcha", "F");
        //        Response.Write("<script language='javascript'>alert('Wrong Captcha')</script>");
        //    }            
        //}
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
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "()" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
            }

            return newId;
        }
        protected void lkbtnsubmit_Click(object sender, EventArgs e)
        {
            try
            {
                if (Session["captcha"] != null)
                {
                    if (Session["captcha"].ToString() == txtbxcaptcha.Text)
                    {
                        if (Session["OTP"] != null && Session["temp_loginsession"] != null)
                        {
                            if (Session["OTP"].ToString() == txtenterotp.Text)
                            {
                                txtbxcaptcha.Text = string.Empty;
                                Session.Remove("captcha");
                                session.__ReInitialiseAuthCookie(Response);
                                if (Request.UrlReferrer != null)
                                {
                                    Uri uri = new Uri(Request.UrlReferrer.ToString());
                                    string referer = uri.Scheme + Uri.SchemeDelimiter + uri.Host;
                                    if ((Session["DoTAuthTok"] != null && Response.Cookies["DoTAuthTok"] != null)
                                    || !GlobalVars.AntiXsrfRefererHeaderList.Contains(referer))
                                    {
                                        if (!(Session["DoTAuthTok"].ToString()).Equals(Response.Cookies["DoTAuthTok"].Value))
                                        {
                                            session.__GetNewASP_Cookie(Request, Response);
                                            RecordUserAction("lkbtnsubmit_Click", "Event request failed. Tampered session", "F");
                                            ClearSensitiveInputs("Wrong login credentials !");
                                            return;
                                        }
                                        else
                                        {
                                            int value = setlogs();
                                            if (value == 1)
                                            {
                                                string OldID = Context.Session.SessionID;
                                                string newID = generateNewSessionId();
                                                Session["login_id"] = Session["temp_loginsession"];
                                                Session.Remove("OTP");
                                                Session["temp_loginsession"] = null;
                                                txtenterotp.Text = string.Empty;
                                                RecordUserAction("lkbtnsubmit_Click", "citizen login succes", "S");
                                                Response.Redirect("Dashboard.aspx");

                                            }
                                            else
                                            {
                                                RecordUserAction("lkbtnsubmit_Click", "citizen login Failed", "F");
                                                lbstatus.ForeColor = System.Drawing.Color.Red;
                                                lbstatus.Text = "Please Login Again";
                                            }
                                        }
                                    }
                                    else
                                    {
                                        RecordUserAction("lkbtnsubmit_Click", "Event failed. Failed login attempt", "F");
                                        ClearSensitiveInputs("Wrong login credentials !");
                                        refreshCaptcha();
                                        session.__GetNewASP_Cookie(Request, Response);
                                        return;
                                    }
                                }
                            }
                            else
                            {
                                refreshCaptcha();
                                lbstatus.ForeColor = System.Drawing.Color.Red;
                                RecordUserAction("lkbtnsubmit_Click", "citizen Entered wrong OTP", "F");
                                lbstatus.Text = "Invalid OTP. Please enter correct OTP. ";
                            }
                        }
                        else
                        {


                            refreshCaptcha();
                            RecordUserAction("Incorrect OTP", "Entered Wrong OTP", "F");
                            Response.Write("<script language='javascript'>alert('Please Enter Correct OTP')</script>");

                        }
                    }
                    else
                    {
                        refreshCaptcha();                    
                        RecordUserAction("Incorrect Captcha", "Entered Wrong Captcha", "F");
                        Response.Write("<script language='javascript'>alert('Please Enter Correct Captcha')</script>");
                    }
                }
                else
                {
                    refreshCaptcha();
                    RecordUserAction("Incorrect Captcha", "Entered Wrong Captcha", "F");
                    Response.Write("<script language='javascript'>alert('Please Enter Correct Captcha')</script>");
                }
            }
            catch (Exception ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "lkbtnsubmit_Click" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
            }            
        }
        private void ClearSensitiveInputs(string errorMsg)
        {
            txtbxcitiusername.Text = string.Empty;

            //Label5.Text = string.Empty; for modal


            txtbxcaptcha.Text = string.Empty;
        }

        protected void lkbtnresend_Click(object sender, EventArgs e)
        {
            try
            {
                if (txtbxcitiusername.Text == "" || txtbxcitiusername.Text == null)
                {

                    lbstatus.ForeColor = System.Drawing.Color.Red;
                    lbstatus.Text = "Username is blank";
                    refreshCaptcha();
                }
                else if (txtbxcaptcha.Text == "" || txtbxcaptcha.Text == null)
                {
                    lbstatus.ForeColor = System.Drawing.Color.Red;
                    lbstatus.Text = "Captcha is blank";
                    refreshCaptcha();
                }
                else if (Session["captcha"] != null)
                {

                    txtenterotp.Visible = false;
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "none", "<script> $(window).on('load', function () { $('#smsmodal').modal({ backdrop: 'show' });});</script>", false);

                }
            }
            catch (Exception ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "lkbtnresend_Click" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
            }
        }

        protected void lkbtnotp_Click(object sender, EventArgs e)
        {
            lbstatus.Text = "";
            if (Session["captcha"] != null)
            {
                if (txtbxcitiusername.Text == "" || txtbxcitiusername.Text == null)
                {

                    lbstatus.ForeColor = System.Drawing.Color.Red;
                    RecordUserAction("lkbtnotp_Click", "Username is blank", "F");
                    lbstatus.Text = "Username is blank";
                    refreshCaptcha();
                }
                else if (txtbxcaptcha.Text == "" || txtbxcaptcha.Text == null)
                {
                    lbstatus.ForeColor = System.Drawing.Color.Red;
                    RecordUserAction("lkbtnotp_Click", "No Captcha", "F");
                    lbstatus.Text = "Captcha is blank";
                    refreshCaptcha();
                }
                else
                {
                    if (Session["captcha"].ToString() == txtbxcaptcha.Text)
                    {
                        txtbxcaptcha.Text = string.Empty;
                        HttpContext.Current.Session.Remove("captcha");
                        NpgsqlConnection conn = new NpgsqlConnection();
                        NpgsqlCommand cmd = new NpgsqlCommand();
                        conn.ConnectionString = ConfigurationManager.ConnectionStrings["connect"].ConnectionString;
                        cmd.Connection = conn;
                        try
                        {
                            conn.Open();
                            string query = "select user_mobile from esociety.usertable where user_loginname=@sid and active = 'Y'";
                            cmd.CommandText = query;
                            cmd.Parameters.AddWithValue("@sid", txtbxcitiusername.Text.Trim());
                            NpgsqlDataReader rd = cmd.ExecuteReader();
                            if (rd.Read())
                            {
                                string mobileno = Server.HtmlEncode(rd["user_mobile"].ToString());
                               // string otp = "11111111";
                                RecordUserAction("lkbtnotp_Click", "otp sent to Applicant", "S");
                                SendSMS sms = new SendSMS();
                                string decrypt_mobile = Encryption.Encrypt.Decrypt(mobileno);
                             
                               string result =    sms.send_otp_sms(decrypt_mobile);
                                 string[] otp = result.Split('|');
                                rd.Close();
                              if (otp[0] != "0" && otp[1] == "OK")
                                  //if (otp != "0")
                                    {
                                      Session["OTP"] = otp[0];
                                 //Session["OTP"] = otp;
                                    lkbtnsubmit.Visible = true;
                                    txtenterotp.Visible = true;
                                    lkbtnresend.Visible = true;
                                    lkbtnotp.Visible = false;
                                    //flag = 1;
                                    //Page.ClientScript.RegisterStartupScript(this.GetType(), " ", "submitPolls()", true);
                                }
                                else
                                {
                                    lkbtnotp.Text = "Resend OTP";
                                }
                                Session["temp_loginsession"] = Server.HtmlEncode(txtbxcitiusername.Text.Trim());
                            }
                            else
                            {
                                lbstatus.Text = "Invalid Username or Captcha";
                                RecordUserAction("lkbtnotp_Click", "Username is wrong", "F");
                                Response.Write("<script language='javascript'>alert(' Invalid Username or Captcha ')</script>");

                            }
                            rd.Close();
                            refreshCaptcha();
                        }
                        catch (NpgsqlException ex)
                        {
                            CreateLogFiles Err = new CreateLogFiles();
                            Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "lkbtnotp_Click" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
                            RecordUserAction("lkbtnotp_Click", ex.Message, "F");
                            Response.Write("<script language='javascript'>alert('" + "Technical or Execution Error" + "')</script>");
                            refreshCaptcha();
                        }
                        finally
                        {
                            conn.Close();
                        }
                    }
                    else
                    {
                        lbstatus.Text = "Invalid Username or Captcha";
                        RecordUserAction("lkbtnotp_Click", "Wrong captcha", "F");
                        Response.Write("<script language='javascript'>alert('Invalid Username or Captcha')</script>");
                        refreshCaptcha();
                    }

                }
            }
            else
            {
                RecordUserAction("lkbtnotp_Click", "Session captcha null", "F");
                Response.Write("<script language='javascript'>alert(' Invalid Username or Captcha ')</script>");
                refreshCaptcha();
            }
        }

       
        public void refreshCaptcha()
        {
            try
            {
                txtbxcaptcha.Text = "";
                string strcapcha = Utility.GenerateRandomString(6, "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789");
                ImgCaptcha.ImageUrl = "../Development/GenerateCaptcha.aspx?" + DateTime.Now.Ticks.ToString() + "&capstr=" + Server.HtmlEncode(strcapcha.ToString());
                Session["captcha"] = strcapcha;
            }
            catch (Exception ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "refreshCaptcha()" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
            }            
        }

        protected void imagebtn_Click(object sender, ImageClickEventArgs e)
        {
            RecordUserAction("imagebtn_Click", "Clicked by SRO", "S");
            refreshCaptcha();
        }

        protected void smsresend_Click(object sender, EventArgs e)
        {
            if (Session["captcha"] != null)
            {
                if (Session["captcha"].ToString() == txtbxcaptcha.Text)
                {
                    txtbxcaptcha.Text = string.Empty;
                    Session.Remove("captcha");
                    NpgsqlConnection conn = new NpgsqlConnection();
                    NpgsqlCommand cmd = new NpgsqlCommand();
                    conn.ConnectionString = ConfigurationManager.ConnectionStrings["connect"].ConnectionString;
                    cmd.Connection = conn;
                    try
                    {
                        conn.Open();
                        string query = "select user_mobile from esociety.usertable where user_loginname=@sid";
                        cmd.CommandText = query;
                        cmd.Parameters.AddWithValue("@sid", txtbxcitiusername.Text);
                        NpgsqlDataReader rd = cmd.ExecuteReader();
                        if (rd.Read())
                        {
                            string mobileno = Server.HtmlEncode(rd["user_mobile"].ToString());
                            //string otp = "11111111";
                            RecordUserAction("lkbtnresend_Click", "otp sent to Applicant", "S");
                            SendSMS sms = new SendSMS();
                            string decrypt_mobile = Encryption.Encrypt.Decrypt(mobileno);
                            string result = sms.send_otp_sms(decrypt_mobile);
                            string[] otp = result.Split('|');
                            rd.Close();
                            if (otp[0] != "0" && otp[1] == "OK")
                            {
                                Session["OTP"] = otp[0];
                               // Session["OTP"] = "11111111";
                                lkbtnsubmit.Visible = true;
                                txtenterotp.Visible = true;
                                lkbtnresend.Visible = true;
                                lkbtnotp.Visible = false;
                                //flag = 1;
                                //Page.ClientScript.RegisterStartupScript(this.GetType(), " ", "submitPolls()", true);
                            }
                            else
                            {
                                lkbtnotp.Text = "Resend OTP";
                            }
                            Session["temp_loginsession"] = Server.HtmlEncode(txtbxcitiusername.Text);
                        }
                        else
                        {
                            lbstatus.Text = "Invalid Username";
                            RecordUserAction("lkbtnresend_Click", "Username is wrong", "F");
                            Response.Write("<script language='javascript'>alert(' Invalid Username ')</script>");

                        }
                        rd.Close();
                        refreshCaptcha(); 
                    }
                    catch (NpgsqlException ex)
                    {
                        CreateLogFiles Err = new CreateLogFiles();
                        Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "smsresend_Click" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
                        lbstatus.Text = "Invalid Username";
                        RecordUserAction("lkbtnresend_Click", ex.Message, "F");
                        Response.Write("<script language='javascript'>alert('" + "Error. Please try after some time" + "')</script>");
                        refreshCaptcha();
                    }
                    finally
                    {
                        conn.Close();
                    }
                }
                else
                {
                    refreshCaptcha();
                    lbstatus.Text = "Please enter correct captcha";
                }
            }
            else
            {
                RecordUserAction("lkbtnresend_Click", "Session captcha null", "F");
                Response.Write("<script language='javascript'>alert(' Invalid Username or Captcha ')</script>");
                refreshCaptcha();
            }
        }

        protected void lkrefreshCaptcha_Click(object sender, EventArgs e)
        {
            refreshCaptcha();
            RecordUserAction("btnrefresh_Click", "Clicked by Applicant", "S");
        }

        protected void LkforgotToken_Click(object sender, EventArgs e)
        {
            showModal();
            CaptchaGenerate();
            otptoken.Visible = false;
           
        }

        protected void showModal()
        {
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "none", "<script> $(window).on('load', function () { $('#forgottoken').modal({ backdrop: 'static' });});</script>", false);
        }


        protected void CaptchaGenerate()
        {
            TxtBxCapt.Text = "";
            string forgotcaptcha =  Utility.GenerateRandomString(6, "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789");
            ImgCaptcha1.ImageUrl = "../Development/GenerateCaptcha.aspx?" + DateTime.Now.Ticks.ToString() + "&capstr=" + Server.HtmlEncode(forgotcaptcha.ToString());
            HttpContext.Current.Session["captcha_token"] = forgotcaptcha;
        }

        protected void LinkButton4_Click(object sender, EventArgs e)
        {
            CaptchaGenerate(); showModal();
        }

        protected void LkGetOTP_Click(object sender, EventArgs e)
        {
            if (Session["captcha_token"] != null)
            {
                string forgot_temp_mobiles = null;
               
                if (HDForgotmobile.Value != null && HDForgotmobile.Value != "")
                {
                    forgot_temp_mobiles = Encryption.Encrypt.DecryptStringAES(HDForgotmobile.Value.ToString(), Session["Enc_Random"].ToString(), Session["Enc_Vector"].ToString());
                }

                //////////


                if (forgot_temp_mobiles == null || forgot_temp_mobiles == "")
                {
                    Label4.Text = "Please enter Mobile No!";
                    Label4.Visible = true;
                    Label4.ForeColor = System.Drawing.Color.Red;
                    showModal();

                }
                else if (!_val.validateData(forgot_temp_mobiles, _val.mobile_regex))
                {
                    Label4.Text = "Mobile no is not valid. Check Mobile no entered";
                    Label4.Visible = true;
                    Label4.ForeColor = System.Drawing.Color.Red;
                    showModal();
                }
                else if (TxtBxCapt.Text == "" || TxtBxCapt.Text == null)
                {
                    Label4.Text = "Enter Captcha";
                    Label4.Visible = true;
                    Label4.ForeColor = System.Drawing.Color.Red;
                    showModal();
                }
                else
                {
                    //TxtBxCaptcha.Enabled = false;
                    if (Session["captcha_token"].ToString() == TxtBxCapt.Text)
                    {

                        NpgsqlConnection conn = new NpgsqlConnection();
                        NpgsqlCommand cmd = new NpgsqlCommand();
                        conn.ConnectionString = ConfigurationManager.ConnectionStrings["connect"].ConnectionString;
                        cmd.Connection = conn;
                        try
                        {
                            conn.Open();
                            string query = "SELECT applicant_mobile_no,login_id FROM esociety.applicant_details where applicant_mobile_no=@applicant_mobile_no";
                            cmd.CommandText = query;
                            cmd.Parameters.AddWithValue("@applicant_mobile_no", Encryption.Encrypt.Encryptt(forgot_temp_mobiles));
                            using (NpgsqlDataAdapter dr = new NpgsqlDataAdapter(cmd))
                            {
                                DataTable dt = new DataTable();
                                dr.Fill(dt);
                                gridforgottoken.DataSource = dt;
                                gridforgottoken.DataBind();

                            }
                            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "none", "<script> $(window).on('load', function () { $('#forgottokenlist').modal({ backdrop: 'static' });});</script>", false);

                        }
                        catch (NpgsqlException ex)
                        {
                            CreateLogFiles Err = new CreateLogFiles();
                            Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "LkGetOTP_Click" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath) + " from " + ipaddress);

                        }
                        finally
                        {
                            conn.Close();
                        }
                    }
                    else
                    {
                        Label4.Text = "Invalid Captcha";
                        RecordUserAction("LkGetOTP_Click", "Wrong captcha", "F");
                        Response.Write("<script language='javascript'>alert('Invalid Captcha')</script>");
                        CaptchaGenerate();
                        showModal();
                    }
                }
            }
            else
            {
                CaptchaGenerate();
                showModal();
            }
        }

      
        protected void gridforgottoken_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType.Equals(DataControlRowType.DataRow))
                {
                    string x = e.Row.Cells[2].Text;
                    e.Row.Cells[2].Text = Encryption.Encrypt.Decrypt(x);
                }
            }
            catch (Exception ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "gridforgottoken_RowDataBound" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath) + " from " + ipaddress);
            }
        }

        protected void close_modal_Click(object sender, EventArgs e)
        {
            try
            {               
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "none", "<script> $(window).on('load', function () { $('#forgottokenlist').modal('hide');});</script>", false);
            }
            catch (Exception ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "close_modal_Click" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath) + " from " + ipaddress);

            }
        }
    }
}