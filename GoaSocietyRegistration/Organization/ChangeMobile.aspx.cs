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
    public partial class ChangeMobile : System.Web.UI.Page
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
                if (Session["firstname"] != null)
                {

                }
                else
                {
                    Response.Redirect("~/OrganizationLogin.aspx");
                }
            }
        }
        private bool checkroles()
        {
            bool role_hacked = false;
            List<int> AllowedRoles = new List<int> { 3 };
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
        protected void LkSearch_Click(object sender, EventArgs e)
        {
            label1.Visible = false;
            lberror.Visible = false;
            if (TxtBxLoginNumber.Text == "" || TxtBxLoginNumber.Text == null)
            {
                TxtBxLoginNumber.Focus();
                label1.Visible = true;
                lberror.Visible = true;
                lberror.Text = "Please enter Login ID";
            }
            else if (!val.validateData(TxtBxLoginNumber.Text, val.alpha_numericregex))
            {
                TxtBxLoginNumber.Focus();
                label1.Visible = true;
                lberror.Visible = true;
                lberror.Text = "Invalid Login ID";
            }
            else
            {
                showData.Visible = true;
                loadData(); TxtBxLoginNumber.Enabled = false;
            }
        }

        public void loadData()
        {
            NpgsqlConnection conn = new NpgsqlConnection();
            NpgsqlCommand cmd = new NpgsqlCommand();
            conn.ConnectionString = ConfigurationManager.ConnectionStrings["connect"].ConnectionString;
            cmd.Connection = conn;
            try
            {
                conn.Open();
                string query = "SELECT applicant_name,applicant_mobile_no FROM esociety.applicant_details where login_id = @login_id and active = 'Y'";
                cmd.CommandText = query;
                cmd.Parameters.AddWithValue("@login_id", TxtBxLoginNumber.Text.Trim());
                NpgsqlDataReader rd = cmd.ExecuteReader();
                if (rd.Read())
                {
                    TxtBxAppname.Text = rd["applicant_name"].ToString().Trim();
                    txtBxOldNumber.Text = Encryption.Encrypt.Decrypt(rd["applicant_mobile_no"].ToString().Trim());
                }
                else
                {
                    showData.Visible = false;
                }
                rd.Close();
            }
            catch (NpgsqlException ex)
            {
                RecordUserAction("Search", "Exception while searching Mobile no.", "Failed", "NA", 1);
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "loadData" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
            }
            finally
            {
                conn.Close();
            }
        }
        protected void permission_Click(object sender, EventArgs e)
        {
            Response.Redirect("Dashboard.aspx");
        }

        protected void lkUpdate_Click(object sender, EventArgs e)
        {
            label1.Visible = false;
            lberror.Visible = false;
            if (TxtbxNewNumber.Text == "" || TxtbxNewNumber.Text == null)
            {
                TxtbxNewNumber.Focus();
                label1.Visible = true;
                lberror.Visible = true;
                lberror.Text = "Please enter New Mobile Number";
            }
            else if (!val.validateData(TxtbxNewNumber.Text, val.mobile_regex))
            {
                TxtbxNewNumber.Focus();
                label1.Visible = true;
                lberror.Visible = true;
                lberror.Text = "Invalid New Mobile Number";
            }
            else
            {
                string old_number = txtBxOldNumber.Text;
                string new_number = Encryption.Encrypt.Encryptt(TxtbxNewNumber.Text);
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
                    string query = "INSERT INTO esociety.mobile_history(token_no, old_mobile, updated_at, update_from, update_by) VALUES (@token_no, @old_mobile,";
                    query = query + " CURRENT_TIMESTAMP, @update_from, @update_by)";
                    cmd.CommandText = query;
                    cmd.Parameters.AddWithValue("@token_no", TxtBxLoginNumber.Text.Trim());
                    cmd.Parameters.AddWithValue("@old_mobile", Encryption.Encrypt.Encryptt(old_number));
                    cmd.Parameters.AddWithValue("@update_from", ipaddress);
                    cmd.Parameters.AddWithValue("@update_by", Session["logginname"].ToString());
                    cmd.ExecuteNonQuery();
                    cmd.Parameters.Clear();
                    string upd_query = "UPDATE esociety.applicant_details set applicant_mobile_no=@applicant_mobile_no where login_id = @login_id and active = 'Y'";
                    cmd.CommandText = upd_query;
                    cmd.Parameters.AddWithValue("@applicant_mobile_no", new_number);
                    cmd.Parameters.AddWithValue("@login_id", TxtBxLoginNumber.Text.Trim());
                    cmd.ExecuteNonQuery();
                    cmd.Parameters.Clear();
                    string updusertable = "UPDATE esociety.usertable set user_mobile=@applicant_mobile_no WHERE user_loginname = @login_id and active = 'Y'";
                    cmd.CommandText = updusertable;
                    cmd.Parameters.AddWithValue("@applicant_mobile_no", new_number);
                    cmd.Parameters.AddWithValue("@login_id", TxtBxLoginNumber.Text.Trim());
                    cmd.ExecuteNonQuery();
                    cmd.Parameters.Clear();
                    myTrans.Commit();
                    Response.Write("<script language='javascript'>alert('" + "Mobile Number Updated" + "')</script>");
                    TxtBxLoginNumber.Enabled = true;
                    TxtBxLoginNumber.Text = "";
                    showData.Visible = false;
                    TxtBxAppname.Text = "";
                    txtBxOldNumber.Text = "";
                    TxtbxNewNumber.Text = "";
                }
                catch (NpgsqlException ex)
                {
                    CreateLogFiles Err = new CreateLogFiles();
                    Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "lkUpdate_Click" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));

                    myTrans.Rollback();
                    Response.Write("<script language='javascript'>alert('" + "Error while updating" + "')</script>");
                }
                finally
                {
                    conn.Close();
                }
            }
        }
    }
}