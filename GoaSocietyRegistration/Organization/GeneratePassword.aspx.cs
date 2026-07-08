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

namespace GoaSocietyRegistration.Organization
{
    public partial class GeneratePassword : System.Web.UI.Page
    {
        Validate val = new Validate();
        SessionManage session = new SessionManage();
        protected void Page_Load(object sender, EventArgs e)
        {
            HttpContext.Current.Response.AddHeader("Cache-Control", "no-cache, no-store, must-revalidate");
            HttpContext.Current.Response.AddHeader("Pragma", "no-cache");
            HttpContext.Current.Response.AddHeader("Expires", "0");
            if (!IsPostBack)
            {
                Session["login_id"] = null;
                string strcapcha = Utility.GenerateRandomString(6, "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789");
                image1.ImageUrl = "../Development/GenerateCaptcha.aspx?" + DateTime.Now.Ticks.ToString() + "&capstr=" + Server.HtmlEncode(strcapcha.ToString());
                Session["captcha"] = strcapcha;
                // string captcha = Utility.GenerateRandomString(8, "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789");
                // image.ImageUrl= "../Development/admincaptcha.aspx?" + DateTime.Now.Ticks.ToString() + "&capstr=" + Server.HtmlEncode(captcha.ToString());
                // Session["adminCaptcha"] = captcha;
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
        protected int setPassword(string password, string username)
        {

            int a = 0;

            if (Check4Tampering() || Page.RouteData.RouteHandler == null)
            {
                //RecordUserAction("setPasswordfunction", "Event request failed. Tampered session", "F");
                session.__Abandon(Request, Response);
                a = 0;
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
                    string update_passowrd = "UPDATE esociety.admin_table SET user_password=@user_password, passwordgenerated=2 WHERE username=@username";
                    cmd.CommandText = update_passowrd;
                    //HashSalt hashSalt = GenerateSaltedHash(64, TxtBxConfirmPass.Text);
                    cmd.Parameters.AddWithValue("@user_password", password);//apply aes algo
                    //cmd.Parameters.AddWithValue("@salt", salt);
                    cmd.Parameters.AddWithValue("@username", username);
                    cmd.ExecuteNonQuery();
                    a = 1;
                }
                catch (NpgsqlException ex)
                {
                    a = 0;
                    CreateLogFiles Err = new CreateLogFiles();
                    Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "setPassword()" + " " + System.IO.Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
                    
                }
                finally
                {
                    conn.Close();
                }
            }
            return a;
        }
        protected void LkBtnSearch_Click(object sender, EventArgs e)
        {

            if(TextBxCaptcha.Text== Session["captcha"].ToString())
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
                        //RecordUserAction("LinkButton1_Click", "Event request failed. Tampered session", "F");
                        Response.Redirect(ResolveUrl("~/Default"));
                    }
                    else
                    {
                        var username = Sanitize.InputText(TextBox1.Text);
                        if (TextBox1.Text != "" || TextBox1.Text != null)
                        {
                            Session["user"] = username;
                            if (!val.validateData(username, val.emailformatregex))
                            {
                                TextBox1.Focus();
                                TextBox1.Text = "";
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
                                    string query = "SELECT concat(userfirstname, usermiddlename, userlastname) as name, username, useremailid, usermobileno,passwordgenerated FROM esociety.admin_table";
                                    query = query + " where username=@username";
                                    cmd.CommandText = query;
                                    cmd.Parameters.AddWithValue("@username", username);
                                    NpgsqlDataReader rd = cmd.ExecuteReader();
                                    if (rd.Read())
                                    {
                                        if (Convert.ToInt32(rd["passwordgenerated"].ToString()) == 1)
                                        {
                                            TextBox1.Enabled = false;
                                            showdata.Visible = true;
                                            LbName.Text = Server.HtmlEncode(rd["name"].ToString());
                                            string mobilenmask = Server.HtmlEncode(Encryption.Encrypt.Decrypt(rd["usermobileno"].ToString()));
                                            LbMobileNo.Text = Server.HtmlEncode(Utility.MaskMobile(mobilenmask, 0, "XXXXXX"));

                                            LkBtnSearch.Visible = false;
                                            Label1.Text = "";
                                        }
                                        else if (Convert.ToInt32(rd["passwordgenerated"].ToString()) == 2)
                                        {
                                            showdata.Visible = false;
                                            Label1.Text = "Password for this User-ID is already generated!!!";
                                            // TextBox1.Text = "";
                                        }
                                    }
                                    else
                                    {
                                        showdata.Visible = false;
                                        Label1.Text = "User-ID Invalid or Wrong";
                                        TextBox1.Text = "";
                                    }
                                    rd.Close();
                                }
                                catch (NpgsqlException ex)
                                {
                                    TextBox1.Text = "";
                                    CreateLogFiles Err = new CreateLogFiles();
                                    Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "LkBtnSearch_Click" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));

                                }
                                finally
                                {
                                    conn.Close();
                                }
                            }
                        }
                        else
                        {
                            TextBox1.Focus();
                            TextBox1.Text = "";
                        }
                    }
                }
            }
            else
            {
                Label1.Text = "Wrong Captcha entered";
            }            
        }

        protected void resetbutton_Click(object sender, EventArgs e)
        {
            try
            {
                TextBox1.Visible = true;
                TextBox1.Enabled = true;
                LkBtnSearch.Visible = true;
                TextBox1.Text = "";
                Label1.Text = "";
                showdata.Visible = false;
            }
            catch (Exception ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "resetbutton_Click" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
            }
        }
        
        protected void submit_password_Click(object sender, EventArgs e)
        {
            try
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
                        //RecordUserAction("submit_password_Click", "Event request failed. Tampered session", "F");
                        Response.Redirect(ResolveUrl("~/Default"));
                    }
                    else
                    {


                        var nwpasswordhash = HDShaPass.Value.Trim();
                        var cnfpasswordhash = HDShaCnfPass.Value.Trim();
                        if (!(String.IsNullOrEmpty(nwpasswordhash) && String.IsNullOrEmpty(cnfpasswordhash)))
                        {
                            if (nwpasswordhash != cnfpasswordhash)
                            {
                                TxtBxConfirmPass.Focus();
                                Label1.Text = "Password and confirm password doesnot match";

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
                                            //RecordUserAction("btnLogin_Click", "Event request failed. Tampered session", "F");
                                            return;
                                        }
                                        else
                                        {
                                            //HashSalt hashs = new HashSalt();
                                            // hashs = GenerateSaltedHash(64, txtPassword.Text);
                                            //var password = hashs.Hash.ToString();
                                            //var salt = hashs.Salt.ToString();
                                            var password = nwpasswordhash;
                                            int a = setPassword(password, Session["user"].ToString());
                                            if (a == 1)
                                            {
                                                Label2.Text = "Password set Successfully. Proceed to Login";
                                                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "none", "<script> $(window).on('load', function () { $('#myModal').modal({ backdrop: 'static' });});</script>", false);

                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    //RecordUserAction("submit_password_Click", "Event failed. Failed login attempt", "F");
                                    session.__GetNewASP_CookieAdmin(Request, Response);
                                    return;
                                }
                            }
                        }

                        else
                        {
                            txtPassword.Focus();
                            Label1.Text = "Password fields cannot be empty.";
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "submit_password_Click" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
            }
        }

        protected void redirect_to_login_Click(object sender, EventArgs e)
        {
            session.__SanitiseAdmin();
            Response.Redirect("~/OrganizationLogin.aspx");

        }
        public void refreshCaptcha()
        {
            try
            {
                TextBxCaptcha.Text = "";
                string strcapcha = Utility.GenerateRandomString(6, "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789");
                image1.ImageUrl = "../Development/GenerateCaptcha.aspx?" + DateTime.Now.Ticks.ToString() + "&capstr=" + Server.HtmlEncode(strcapcha.ToString());
                Session["captcha"] = strcapcha;
            }
            catch (Exception ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "refreshCaptcha()" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
            }
        }
        protected void LkBtnCaptchaRefresh_Click(object sender, EventArgs e)
        {
            refreshCaptcha();
        }
    }
}