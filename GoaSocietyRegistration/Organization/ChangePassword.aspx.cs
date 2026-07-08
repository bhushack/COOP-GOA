using GoaSocietyRegistration.Development;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace GoaSocietyRegistration.Organization
{
    public partial class ChangePassword : System.Web.UI.Page
    {

        Insert ins = new Insert();
        Validate val = new Validate();
        Development.SessionManage session = new SessionManage();
        string ipaddress = Utility.getIP();
        string macaddress = Utility.GetMACAddress(Utility.getIP());
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Check4Tampering() || Page.RouteData.RouteHandler == null)
            {
                //RecordUserAction("Page_Load", "Access request failed. Tampered session", "F");
                session.__AbandonAdmin(Request, Response);
            }
            else
            {
                HttpContext.Current.Response.AddHeader("Cache-Control", "no-cache, no-store, must-revalidate");
                HttpContext.Current.Response.AddHeader("Pragma", "no-cache");
                HttpContext.Current.Response.AddHeader("Expires", "0");
                //RecordUserAction("Page_Load", "Page Load Successful", "S");
                Label1.Text = Server.HtmlEncode(Session["firstname"].ToString());
                if (Session["Enc_Random"] == null)
                {
                    var random = Encryption.Encrypt.GenerateRandomSaltAES();
                    var randomiv = Encryption.Encrypt.GenerateRandomIvAES();
                    Session["Enc_Random"] = random;
                    Session["Enc_Vector"] = randomiv;
                }
                if (Session["Sess_rndNo"] == null)
                {
                    //Random random = new Random();
                    //Session["Sess_rndNo"] = random.Next(10000, 100000).ToString() + random.Next(10000, 100000).ToString() + random.Next(100000, 1000000).ToString();
                    //string abc = Session["Sess_rndNo"].ToString();


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

        protected void TxtBxPassword_TextChanged(object sender, EventArgs e)
        {
            if (Check4Tampering() || Page.RouteData.RouteHandler == null)
            {
                //RecordUserAction("Page_Load", "Access request failed. Tampered session", "F");
                SessionManage session = new SessionManage();
                session.__AbandonAdmin(Request, Response);
            }
            else
            {
                if (Session["firstname"] != null)
                {
                    if (TxtBxPassword.Text.Length > 0)
                    {
                        //var currentpassword = AES_algorithm.DecryptStringAES(HdCurrentPassword.Value, Session["Enc_Random"].ToString(), Session["Enc_Vector"].ToString());
                        var userHash = HdShaCurrentPass.Value.Trim();
                        //if (val.validateData(currentpassword, val.password_policy))
                        //{
                        NpgsqlConnection conn = new NpgsqlConnection();
                        NpgsqlCommand cmd = new NpgsqlCommand();
                        conn.ConnectionString = ConfigurationManager.ConnectionStrings["connect"].ConnectionString;
                        cmd.Connection = conn;
                        try
                        {
                            int a = 0;
                            conn.Open();
                            string query = "SELECT user_password,usermobileno from esociety.admin_table where username=@username and active='Y'";
                            cmd.CommandText = query;
                            cmd.Parameters.AddWithValue("@username", Session["firstname"].ToString());
                            NpgsqlDataReader rd = cmd.ExecuteReader();
                            if (rd.Read())
                            {

                                ViewState["user_password"] = Server.HtmlEncode(rd["user_password"].ToString());
                                string user_password = Server.HtmlEncode(rd["user_password"].ToString());
                                ViewState["usermobileno"] = Server.HtmlEncode(rd["usermobileno"].ToString());
                                if (user_password == null || user_password == "")
                                {
                                    Response.Write("<script language='javascript'>alert(' Please Generate your password first. ')</script>");
                                }
                                else
                                {

                                    var checkHash = Encryption.Encrypt.SHA512(Session["Sess_rndNo"].ToString() + user_password);
                                    if (checkHash == userHash)
                                    {
                                        a = 1;
                                        TxtbxNewPassword.Enabled = true;
                                        TxtBxConfirmPassowrd.Enabled = true;
                                        TxtBxPassword.Enabled = false;
                                        TxtBxPassword.Text = "*************";
                                    }
                                    else
                                    {
                                        Response.Write("<script language='javascript'>alert('Credentails doesnot match')</script>");
                                        //Login Failed
                                    }
                                }
                            }
                            else
                            {
                                TxtbxNewPassword.Enabled = false;
                                TxtBxConfirmPassowrd.Enabled = false;
                            }
                            rd.Close();
                        }
                        catch (NpgsqlException ex)
                        {
                            CreateLogFiles Err = new CreateLogFiles();
                            Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "TxtBxPassword_TextChanged" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
                            RecordUserAction("Update Password", "Password NOT CHANGED", "Failed", "NA", 2);
                        }
                        finally
                        {
                            conn.Close();
                        }
                        //}

                    }
                    else
                    {
                        Response.Write("<script language='javascript'>alert('Please enter password')</script>");
                        //Login Failed
                    }
                }

                else
                {
                    CreateLogFiles Err = new CreateLogFiles();
                    Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), "session null", "TxtBxPassword_TextChanged" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
                    RecordUserAction("Update Password", "Password not chnaged due to Null Session ", "Failed", "NA", 2);

                }
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
        protected bool checkoldpassword(string newpassword)
        {
            bool exists = true;
            int count = 0;
            if (Check4Tampering() || Page.RouteData.RouteHandler == null)
            {
                SessionManage session = new SessionManage();
                session.__AbandonAdmin(Request, Response);
            }
            else
            {
                if (Session["firstname"] != null)
                {

                    NpgsqlConnection conn = new NpgsqlConnection();
                    NpgsqlCommand cmd = new NpgsqlCommand();
                    conn.ConnectionString = ConfigurationManager.ConnectionStrings["connect"].ConnectionString;
                    cmd.Connection = conn;
                    try
                    {
                        int a = 0;
                        conn.Open();
                        string query = "SELECT user_password FROM (Select user_password from esociety.usertable_history where username = @username";
                        query = query + " order by password_updateat desc limit 2 ) user_password union SELECT user_password FROM esociety.admin_table where username = @username";
                        cmd.CommandText = query;
                        cmd.Parameters.AddWithValue("@username", Session["firstname"].ToString());
                        NpgsqlDataReader rd = cmd.ExecuteReader();
                        while (rd.Read())
                        {
                            string old_db__password = Server.HtmlEncode(rd["user_password"].ToString());
                            if (old_db__password == newpassword)
                            {
                                count = count + 1;
                            }
                        }
                        rd.Close();
                        if (count >= 1)
                        {
                            exists = true;
                        }
                        else
                        {
                            exists = false;
                        }
                    }
                    catch (NpgsqlException ex)
                    {
                        exists = true;
                        CreateLogFiles Err = new CreateLogFiles();
                        Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "checkoldpassword" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
                    }
                    finally
                    {
                        conn.Close();
                    }

                }
                else
                {
                    exists = true;
                    CreateLogFiles Err = new CreateLogFiles();
                    Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), "session null", "checkoldpassword" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
                    RecordUserAction("Read Old Password", "checkoldpassword", "Failed", "NA", 2);
                }
            }
            return exists;
        }
        protected void BtnChangePassword_Click(object sender, EventArgs e)
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
                    // RecordUserAction("submit_password_Click", "Event request failed. Tampered session", "F");
                    Response.Redirect("~/OrganizationLogin");//check redirecrtion is ok or not
                }
                else
                {
                    //RecordUserAction("BtnChangePassword_Click", "Change Password Clicked", "S");
                    if (Session["firstname"] != null)
                    {
                        //var nwpasword = AES_algorithm.DecryptStringAES(HDPassword.Value, Session["Enc_Random"].ToString(), Session["Enc_Vector"].ToString());
                        //var cnfpassword = AES_algorithm.DecryptStringAES(HDCnfPassword.Value, Session["Enc_Random"].ToString(), Session["Enc_Vector"].ToString());
                        var nwpasswordhash = HDShaPass.Value.Trim();
                        var cnfpasswordhash = HDShaCnfPass.Value.Trim();
                        if (TxtbxNewPassword.Text.Length > 0 && TxtBxConfirmPassowrd.Text.Length > 0)
                        {
                            
                            // if ((nwpasswordhash == cnfpasswordhash))//&& !checkoldpassword(nwpasswordhash))
                           // {
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
                                            RecordUserAction("btnLogin_Click", "Event request failed. Tampered session", "F", "NA", 1);
                                            return;
                                        }
                                        else
                                        {
                                            var password = nwpasswordhash;
                                            NpgsqlConnection conn = new NpgsqlConnection();
                                            NpgsqlCommand cmd = new NpgsqlCommand();
                                            conn.ConnectionString = ConfigurationManager.ConnectionStrings["connect"].ConnectionString;
                                            cmd.Connection = conn;
                                            conn.Open();
                                            NpgsqlTransaction trans = conn.BeginTransaction();
                                            try
                                            {
                                                cmd.Parameters.Clear();
                                                string update_password = "UPDATE esociety.admin_table SET user_password=@user_password,reset_password=true,last_reset_on=CURRENT_DATE  WHERE username=@username";
                                                cmd.CommandText = update_password;
                                                cmd.Parameters.AddWithValue("@user_password", password);
                                                // cmd.Parameters.AddWithValue("@salt", salt);
                                                cmd.Parameters.AddWithValue("@username", Session["firstname"].ToString());
                                                cmd.ExecuteNonQuery();
                                                cmd.Parameters.Clear();
                                                string insert_history = "INSERT INTO esociety.usertable_history(username, mobileno, user_password,  password_updateat, password_updated_by,";
                                                insert_history = insert_history + " ipaddress, macaddress) VALUES(@username, @mobileno, @user_password,  current_timestamp,";
                                                insert_history = insert_history + " @password_updated_by,@ipaddress, @macaddress)";
                                                cmd.CommandText = insert_history;
                                                cmd.Parameters.AddWithValue("@username", Session["firstname"].ToString());
                                                cmd.Parameters.AddWithValue("@mobileno", ViewState["usermobileno"].ToString());
                                                cmd.Parameters.AddWithValue("@user_password", ViewState["user_password"].ToString());
                                                cmd.Parameters.AddWithValue("@password_updated_by", Session["userfirstname"].ToString());
                                                cmd.Parameters.AddWithValue("@ipaddress", ipaddress);
                                                cmd.Parameters.AddWithValue("@macaddress", macaddress);
                                                cmd.ExecuteNonQuery();
                                                trans.Commit();
                                                Label11.ForeColor = System.Drawing.Color.Green;
                                                Label11.Text = "Password Changed Successfully!!";
                                                RecordUserAction("Update Password", "Password Changed Successfully", "Success", "NA", 1);
                                                Response.Write("<script language='javascript'>alert('Password updated succesfully')</script>");
                                                int value = Utility.logout(Convert.ToInt64(Session["loginsessioncount"]));//chnages session value
                                                if (value == 1)
                                                {
                                                    Session.Clear();
                                                    Session.Abandon();
                                                    Session.RemoveAll();
                                                    Response.Cookies.Clear();
                                                    Session.Remove("Enc_Random");
                                                    Session.Remove("Enc_Vector");
                                                    session.__SanitiseAdmin();
                                                    Response.Redirect("~/OrganizationLogin.aspx");
                                                }
                                                else
                                                {
                                                    RecordUserAction("Update Password", "Logout Failed", "Failed", "NA", 1);
                                                    Response.Write("<script language='javascript'>alert(' Error While logout....Try after sometime ')</script>");
                                                    Response.Redirect("~/Organization/Dashboard.aspx");
                                                }
                                            }
                                            catch (NpgsqlException ex)
                                            {
                                                trans.Rollback();
                                                Label11.ForeColor = System.Drawing.Color.Red;
                                                RecordUserAction("BtnChangePassword_Click", ex.Message, "Failed", "NA", 2);
                                                Label11.Text = "Password Changing Failed";
                                                CreateLogFiles Err = new CreateLogFiles();
                                                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "BtnChangePassword_Click" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
                                            }
                                            finally
                                            {
                                                conn.Close();
                                            }
                                        }
                                    }
                                    else
                                    {
                                        TxtbxNewPassword.Focus();
                                        Label11.ForeColor = System.Drawing.Color.Red;
                                        Label11.Text = "It Seems You Have Entered Same Password As Old Password!   Please set new password which is not same as old password.";
                                        // RecordUserAction("BtnChangePassword_Click", "Old and New Passwords Not matching", "F");
                                    }

                                }
                                else
                                {
                                    Response.Write("<script language='javascript'>alert('Please enter password')</script>");
                                }
                           // }
                            //else
                            //{
                            //    //error messag erelated to password that is same as old passowrd
                            //    RecordUserAction("Update Password", "Password Changed Failed", "Failure", "NA", 1);
                            //    Response.Write("<script language='javascript'>alert('It Seems You Have Entered Same Password As Old Password!   Please set new password which is not same as old password. of Password Mismatch')</script>");
                            //}
                        }
                    }

                }
            }
        }
    }
}