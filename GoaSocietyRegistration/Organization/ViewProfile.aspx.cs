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

namespace GoaSocietyRegistration.Organization
{
    public partial class ViewProfile : System.Web.UI.Page
    {
        System.Globalization.CultureInfo french = new System.Globalization.CultureInfo("fr-FR");
        
        Validate val = new Validate();
        Insert ins = new Insert();
        string ipaddress = Utility.getIP();
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Check4Tampering() || Page.RouteData.RouteHandler == null)
            {
                RecordUserAction("Load", "Tampered Session on Page_Load", "Failed", "NA", 2);
                SessionManage session = new SessionManage();
                session.__AbandonAdmin(Request, Response);
            }
            else
            {
                if (Session["firstname"] != null)
                {
                    HttpContext.Current.Response.AddHeader("Cache-Control", "no-cache, no-store, must-revalidate");
                    HttpContext.Current.Response.AddHeader("Pragma", "no-cache");
                    HttpContext.Current.Response.AddHeader("Expires", "0");

                    if (!IsPostBack)
                    {
                        Session["edit_verifyprofile"] = 0;
                        RecordUserAction("Read", "Page Load Successfull", "Success", "NA", 1);
                        SetDesignation();
                        string loginid = Session["firstname"].ToString();
                        Label1.Text = Session["firstname"].ToString();
                    }
                    if (Convert.ToInt32(Session["edit_verifyprofile"].ToString()) == 0)
                    {
                        setData(Session["firstname"].ToString());
                    }
                }
                else
                {
                    RecordUserAction("Read", "Page Load Failed Null", "Failed", "NA", 1);
                    Response.Redirect("~/OrganizationLogin.aspx");
                }
            }
        }
        protected void setData(string username)
        {
            NpgsqlConnection conn = new NpgsqlConnection();
            NpgsqlCommand cmd = new NpgsqlCommand();
            conn.ConnectionString = ConfigurationManager.ConnectionStrings["connect"].ConnectionString;
            cmd.Connection = conn;
            try
            {
                conn.Open();
                string query = "SELECT username, userfirstname, usermiddlename, userlastname, useremailid, user_designation, role_id, district_id, usermobileno";
                query = query + " FROM esociety.admin_table where username= @username";
                cmd.CommandText = query;
                cmd.Parameters.AddWithValue("@username", username);
                NpgsqlDataReader rd = cmd.ExecuteReader();
                if (rd.Read())
                {
                    TxtBxFirstName.Text = Server.HtmlEncode(rd["userfirstname"].ToString());
                    TxtBxMiddlename.Text = Server.HtmlEncode(rd["usermiddlename"].ToString());
                    TxtBxLastName.Text = Server.HtmlEncode(rd["userlastname"].ToString());
                    //if (rd["dateofbirth"].ToString() == "" || rd["dateofbirth"].ToString() == null)
                    //{
                    //    TxtBxDobs.Text = "";
                    //}
                    //else
                    //{
                    //    System.Globalization.CultureInfo french = new System.Globalization.CultureInfo("fr-FR");
                    //    var dob = Server.HtmlEncode(rd["dateofbirth"].ToString());
                    //    DateTime date = Convert.ToDateTime(dob, french);
                    //    TxtBxDobs.Text = date.Date.ToString("dd/MM/yyyy");
                    //    //TxtBxDobs.Text = dt.Date.ToString();
                    //}

                    ddldesgination.SelectedValue = Server.HtmlEncode(rd["role_id"].ToString());
                    TxtBxMobile.Text = Server.HtmlEncode(Encryption.Encrypt.Decrypt(rd["usermobileno"].ToString()));
                    TxtBxFirstName.Enabled = false;
                    TxtBxMiddlename.Enabled = false;
                    TxtBxLastName.Enabled = false;
                   // TxtBxDobs.Enabled = false;
                    TxtBxMobile.Enabled = false;
                }
                else
                {
                    Response.Redirect("Dashboard.aspx");
                }
                rd.Close();
            }
            catch (NpgsqlException ex)
            {
                RecordUserAction("Read", "Db Function Load ", "Failed", "NA", 2);
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "setData()" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath) + " from " + ipaddress);
            }
            finally
            {
                conn.Close();
            }
        }
        protected void SetDesignation()
        {
            NpgsqlConnection conn = new NpgsqlConnection();
            NpgsqlCommand cmd = new NpgsqlCommand();
            conn.ConnectionString = ConfigurationManager.ConnectionStrings["connect"].ConnectionString;
            cmd.Connection = conn;
            NpgsqlDataAdapter adapter;
            DataSet ds;
            try
            {
                conn.Open();
                string query = "select rolename,role_id from esociety.mst_role";
                cmd = new NpgsqlCommand(query, conn);
                adapter = new NpgsqlDataAdapter(cmd);
                ds = new DataSet();
                adapter.Fill(ds, "esociety.mst_role");
                ddldesgination.DataSource = ds.Tables[0];
                ddldesgination.DataTextField = "rolename";
                ddldesgination.DataValueField = "role_id";
                ddldesgination.DataBind();
                ddldesgination.Items.Insert(0, new ListItem("-- Select --", "-1"));
                cmd.Dispose();
                adapter.Dispose();
                ds = null;

            }
            catch (NpgsqlException ex)
            {
                RecordUserAction("Read", "Db Function Load ", "Failed", "NA", 2);
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "SetDesignation()" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath) + " from " + ipaddress);
            }
            finally
            {
                conn.Close();
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

        protected void btn_edit_Click(object sender, EventArgs e)
        {
            try
            {
                RecordUserAction("Update", "btn_edit_Click Button Cliked", "Success", "NA", 1);
                Session["edit_verifyprofile"] = 1;
                TxtBxFirstName.Enabled = true;
                TxtBxMiddlename.Enabled = true;
                TxtBxLastName.Enabled = true;
                //TxtBxDobs.Enabled = true;
                TxtBxMobile.Enabled = true;
                btn_edit.Visible = false;
                btnUpdate.Visible = true;
                btn_cancel.Visible = true;
            }
            catch (Exception ex)
            {
                RecordUserAction("Update", "btn_edit_Click Button Cliked", "Failed", "NA", 1);
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "btn_edit_Click" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath) + " from " + ipaddress);
            }
        }

        protected void btn_cancel_Click(object sender, EventArgs e)
        {
            try
            {
                RecordUserAction("Update", "btn_cancel_Click Button Cliked", "Success", "NA", 1);
                Session["edit_verifyprofile"] = 0;
                TxtBxFirstName.Enabled = false;
                TxtBxMiddlename.Enabled = false;
                TxtBxLastName.Enabled = false;
               // TxtBxDobs.Enabled = false;
                ddldesgination.Enabled = false;
                TxtBxMobile.Enabled = false;
                btn_edit.Visible = true;
                btnUpdate.Visible = false;
                btn_cancel.Visible = false;
            }
            catch (Exception ex)
            {
                RecordUserAction("Update", "btn_cancel_Click Button Cliked", "Failed", "NA", 1);
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "btn_cancel_Click" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath) + " from " + ipaddress);
            }
        }

        protected void btnUpdate_Click(object sender, EventArgs e)
        {
            RecordUserAction("Update", "btnUpdate_Click Button Cliked", "Success", "NA", 1);
            Lberror.Visible = true;
            if (TxtBxFirstName.Text == "" || TxtBxFirstName.Text == null)
            {
                Lberror.Visible = true;
                Lberror.Text = "First Name is Blank";
                TxtBxFirstName.Focus();
            }
            else if (!val.validateData(TxtBxFirstName.Text, val.name))
            {
                Lberror.Visible = true;
                Lberror.Text = "First Name is Invalid";
                TxtBxFirstName.Focus();
            }
            else if (!val.validateData(TxtBxMiddlename.Text, val.name))
            {
                Lberror.Visible = true;
                Lberror.Text = "Middle Name is Invalid";
                TxtBxMiddlename.Focus();
            }
            else if (TxtBxLastName.Text == "" || TxtBxLastName.Text == null)
            {
                Lberror.Visible = true;
                Lberror.Text = "Last Name is Invalid";
                TxtBxLastName.Focus();
            }
            //else if (!val.validateData(TxtBxDobs.Text, val.dateregex))
            //{
            //    Lberror.Visible = true;
            //    Lberror.Text = "Date of Birth is Invalid";
            //    TxtBxDobs.Focus();
            //}
            else if (TxtBxMobile.Text == "" || TxtBxMobile.Text == null)
            {
                Lberror.Visible = true;
                Lberror.Text = "Mobile No is Blank";
                TxtBxMobile.Focus();
            }
            else if (!val.validateData(TxtBxMobile.Text, val.mobile_regex))
            {
                Lberror.Visible = true;
                Lberror.Text = "Mobile No is Invalid";
                TxtBxMobile.Focus();
            }
            else if (!(TxtBxMobile.MaxLength == 10))
            {
                Lberror.Visible = true;
                Lberror.Text = "Mobile No is not of 10 digit";
                TxtBxMobile.Focus();
            }
            else
            {
                Lberror.Visible = false;
                NpgsqlConnection conn = new NpgsqlConnection();
                NpgsqlCommand cmd = new NpgsqlCommand();
                conn.ConnectionString = ConfigurationManager.ConnectionStrings["connect"].ConnectionString;
                cmd.Connection = conn;
                conn.Open();
                NpgsqlTransaction trans = conn.BeginTransaction();
                try
                {
                    cmd.Parameters.Clear();
                    string query = "SELECT username, userfirstname, usermiddlename, userlastname, useremailid, user_designation, created_by, created_at, ipaddress,";
                    query = query + " macaddress, active, role_id, district_id, passwordgenerated, user_password, usermobileno FROM esociety.admin_table";
                    query = query + " where username= @username ";
                    cmd.CommandText = query;
                    cmd.Parameters.AddWithValue("@username", Session["firstname"].ToString());
                    NpgsqlDataReader rd = cmd.ExecuteReader();
                    if (rd.Read())
                    {
                        ViewState["username"] = rd["username"].ToString();
                        ViewState["userfirstname"] = rd["userfirstname"].ToString();
                        ViewState["usermiddlename"] = rd["usermiddlename"].ToString();
                        ViewState["userlastname"] = rd["userlastname"].ToString();
                        ViewState["useremailid"] = rd["useremailid"].ToString();
                        ViewState["user_designation"] = rd["user_designation"].ToString();
                        ViewState["created_by"] = rd["created_by"].ToString();
                        ViewState["created_at"] = (DateTime)rd["created_at"];
                        ViewState["ipaddress"] = rd["ipaddress"].ToString();
                        ViewState["macaddress"] = rd["macaddress"].ToString();
                        ViewState["active"] = rd["active"].ToString();
                        ViewState["role_id"] = (int)rd["role_id"];
                        ViewState["district_id"] = (int)rd["district_id"];
                        ViewState["passwordgenerated"] = (int)rd["passwordgenerated"];
                        ViewState["user_password"] = rd["user_password"].ToString();
                        ViewState["usermobileno"] = rd["usermobileno"].ToString();
                    }
                    else
                    {
                        trans.Rollback();
                    }
                    rd.Close();
                    cmd.Parameters.Clear();
                    string query_ins_history = "INSERT INTO esociety.usertable_history( username, mobileno, user_password, password_updateat,";
                    query_ins_history = query_ins_history + " password_updated_by, ipaddress, macaddress, action_done) 	VALUES (@username, @mobileno,";
                    query_ins_history = query_ins_history + " @user_password, CURRENT_TIMESTAMP , @password_updated_by, @ipaddress, @macaddress, @action_done)";
                    cmd.CommandText = query_ins_history;
                    cmd.Parameters.AddWithValue("@username", Session["firstname"].ToString());
                    cmd.Parameters.AddWithValue("@mobileno", ViewState["usermobileno"].ToString());
                    cmd.Parameters.AddWithValue("@user_password", ViewState["user_password"].ToString());
                    cmd.Parameters.AddWithValue("@password_updated_by", Session["firstname"].ToString());
                    cmd.Parameters.AddWithValue("@ipaddress", ViewState["ipaddress"]);
                    cmd.Parameters.AddWithValue("@macaddress", ViewState["macaddress"]);
                    cmd.Parameters.AddWithValue("@action_done", "Profile Update");
                    cmd.ExecuteNonQuery();
                    cmd.Parameters.Clear();
                    string query_update = "UPDATE esociety.admin_table SET userfirstname =@userfirstname, usermiddlename =@usermiddlename, userlastname =@userlastname,";
                    query_update = query_update + " usermobileno =@usermobileno WHERE username = @username";
                    cmd.CommandText = query_update;
                    cmd.Parameters.AddWithValue("@userfirstname", Server.HtmlEncode(TxtBxFirstName.Text));
                    cmd.Parameters.AddWithValue("@usermiddlename", TxtBxMiddlename.Text);
                    cmd.Parameters.AddWithValue("@userlastname", Server.HtmlEncode(TxtBxLastName.Text));
                    cmd.Parameters.AddWithValue("@usermobileno", Encryption.Encrypt.Encryptt(TxtBxMobile.Text));
                    //cmd.Parameters.AddWithValue("@dateofbirth", Convert.ToDateTime(TxtBxDobs.Text).Date);
                    cmd.Parameters.AddWithValue("@username", Session["firstname"].ToString());
                    cmd.ExecuteNonQuery();
                    trans.Commit();
                    Label8.Text = "Profile Saved!!!!";
                    RecordUserAction("Update", "btnUpdate_Click profile saved successfull", "Success", "NA", 1);
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "none", "<script> $(window).on('load', function () { $('#ConfirmationModal').modal({ backdrop: 'static' });});</script>", false);
                    setData(Session["firstname"].ToString());
                    
                }
                catch (NpgsqlException ex)
                {
                    RecordUserAction("Update", "btnUpdate_Click profile save failed", "Failed", "NA", 1);
                    CreateLogFiles Err = new CreateLogFiles();
                    Err.ErrorLog(HttpContext.Current.Server.MapPath("~/Logs/ErrorLog"), ex.Message + "  " + ex.StackTrace, "btnUpdate_Click" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath) + " from " + ipaddress);
                    Response.Write("<script language='javascript'>alert('Failed. Please try again....')</script>");
                    trans.Rollback();
                }
                finally
                {
                    conn.Close();
                }
            }
        }

        protected void redirect_Click(object sender, EventArgs e)
        {
            Response.Redirect("ViewProfile.aspx");
        }
    }
}