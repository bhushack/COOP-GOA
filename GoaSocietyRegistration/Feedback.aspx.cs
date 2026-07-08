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


namespace GoaSocietyRegistration
{
    public partial class Feedback : System.Web.UI.Page
    {
        Insert ins = new Insert();
        Validate val = new Validate();
        string ipaddress = Utility.getIP();
        string macaddress = Utility.GetMACAddress(Utility.getIP());

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Check4Tampering() || Page.RouteData.RouteHandler == null)
            {
                SessionManage session = new SessionManage();
                session.__AbandonAdmin(Request, Response);
            }
            else
            {
                HttpContext.Current.Response.AddHeader("Cache-Control", "no-cache, no-store, must-revalidate");
                HttpContext.Current.Response.AddHeader("Pragma", "no-cache");
                HttpContext.Current.Response.AddHeader("Expires", "0");
                if (!IsPostBack)
                {
                    string key = Utility.GenerateRandomString(10, "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789#$%&*()");
                    var random = Encryption.Encrypt.GenerateRandomSaltAES();
                    var randomiv = Encryption.Encrypt.GenerateRandomIvAES();
                    Session["Enc_Random"] = random;
                    Session["Enc_Vector"] = randomiv;
                    string strcapcha = Utility.GenerateRandomString(6, "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789");
                    ImgCaptcha.ImageUrl = "Development/GenerateCaptcha.aspx?" + DateTime.Now.Ticks.ToString() + "&capstr=" + Server.HtmlEncode(strcapcha.ToString());
                    Session["captcha"] = strcapcha;                   
                }
            }
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
                else
                {
                    sessHackedCheck = true;
                }
            }
            return sessHackedCheck;
        }
        protected void callErrorModal(string msg)
        {
            refreshCaptcha();
            Label10.Text = Server.HtmlEncode(msg);
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "none", "<script> $(window).on('load', function () { $('#feedbackerror').modal({ backdrop: 'static' });});</script>", false);
        }

        protected void LkBtnSubmit_Click(object sender, EventArgs e)
        {
            string temp_mobiles = null, temp_emails = null;
            if (HDmobile.Value != null && HDmobile.Value != "")
            {
                temp_mobiles = Encryption.Encrypt.DecryptStringAES(HDmobile.Value.ToString(), Session["Enc_Random"].ToString(), Session["Enc_Vector"].ToString());
            }
            else
            {

                lberror.Text = "Please enter Mobile No!";
            }
            if (HDemail.Value != null && HDemail.Value != "")
            {
                temp_emails = Encryption.Encrypt.DecryptStringAES(HDemail.Value.ToString(), Session["Enc_Random"].ToString(), Session["Enc_Vector"].ToString());
            }
            else
            {

                lberror.Text = "Please enter Email ID!";
            }
            try
            {
                if (TxtBxName.Text == "" || TxtBxName.Text == null)
                {
                    callErrorModal("Name is Blank!!!");
                }
                else if (!val.validateData(TxtBxName.Text, val.name))
                {
                    callErrorModal("Name is Invalid");
                }
                else if (temp_emails == "" || temp_emails == null)
                {
                    callErrorModal("Email is Blank!!!");
                }
                else if (!val.validateData(temp_emails, val.emailformatregex))
                {
                    callErrorModal("Email is Invalid!!!");
                }
                else if (temp_mobiles == "" || temp_mobiles == null)
                {
                    callErrorModal("Mobile is Blank!!!");
                }
                else if (!(temp_mobiles.Length == 10))
                {
                    callErrorModal("Mobile is Invalid!!!");
                }
                else if (!val.validateData(temp_mobiles, val.mobile_regex))
                {
                    callErrorModal("Mobile is Invalid!!!");
                }
                else if (TxtFeedback.Text == "" || TxtFeedback.Text == null)
                {
                    callErrorModal("Feedback is Blank!!!");
                }
                else if (!val.validateData(TxtFeedback.Text, val.address))
                {
                    callErrorModal("Feedback should have characters!!!");
                }
                else
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
                            conn.Open();
                            try
                            {
                                string query = "INSERT INTO esociety.feedback(feedback_name, feedback_mobile, feedback_email, feedback, ipaddress, created_at)	VALUES";
                                query = query + "(@feedback_name, @feedback_mobile, @feedback_email, @feedback, @ipaddress, CURRENT_TIMESTAMP)";
                                cmd.CommandText = query;
                                cmd.Parameters.AddWithValue("@feedback_name", TxtBxName.Text);
                                cmd.Parameters.AddWithValue("@feedback_mobile", Encrypt.Encryptt(temp_mobiles));
                                cmd.Parameters.AddWithValue("@feedback_email", temp_emails);
                                cmd.Parameters.AddWithValue("@feedback", TxtFeedback.Text);
                                cmd.Parameters.AddWithValue("@ipaddress", ipaddress);
                                cmd.ExecuteNonQuery();
                                Label2.Text = Server.HtmlEncode("Feedback is Saved!!");
                                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "none", "<script> $(window).on('load', function () { $('#redirectmodal').modal({ backdrop: 'static' });});</script>", false);

                            }
                            catch (Exception ex)
                            {
                                CreateLogFiles Err = new CreateLogFiles();
                                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "LkBtnSubmit_Click" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath) + " from " + ipaddress);
                            }
                            finally
                            {
                                conn.Close();
                            }
                        }
                        else
                        {
                            txtbxcaptcha.Text = string.Empty;
                            Response.Write("<script language='javascript'>alert('Wrong Captcha')</script>");
                            refreshCaptcha();
                        }
                    }
                    else {

                        refreshCaptcha();                     
                        Response.Write("<script language='javascript'>alert('Please Enter Correct Captcha')</script>");
                    }
                }

            }
            catch (Exception ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "LkBtnSubmit_Click" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath) + " from " + ipaddress);
            }
        }

        protected void LkBtnCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect("Feedback.aspx");
        }

        protected void lkrefreshCaptcha_Click(object sender, EventArgs e)
        {
            refreshCaptcha();
        }
        public void refreshCaptcha()
        {
            try
            {
                string strcapcha = Utility.GenerateRandomString(6, "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789");
                ImgCaptcha.ImageUrl = "Development/GenerateCaptcha.aspx?" + DateTime.Now.Ticks.ToString() + "&capstr=" + Server.HtmlEncode(strcapcha.ToString());
                Session["captcha"] = strcapcha;
            }
            catch (Exception ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "refreshCaptcha()" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
            }

        }

        protected void LinkButton1_Click(object sender, EventArgs e)
        {
            Response.Redirect("Feedback.aspx");
        }
    }
}