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
    public partial class AdminUser : System.Web.UI.Page
    {
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
            else if (checkroles())
            {
                RecordUserAction("Load", "Unauthorized Access - Role", "Failed", "NA", 2);
                Label26.Text = "Permission Denied. Please Contact to your Admin.";
                Label26.ForeColor = System.Drawing.Color.Red;
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "none", "<script> $(window).on('load', function () { $('#permission_error_modal').modal({ backdrop: 'static' });});</script>", false);

            }
            else
            {
                if (Convert.ToInt32(Session["ROLE"].ToString()) == 1 || Convert.ToInt32(Session["ROLE"].ToString()) == 2)
                {
                    if (Session["firstname"] != null)
                    {

                        HttpContext.Current.Response.AddHeader("Cache-Control", "no-cache, no-store, must-revalidate");
                        HttpContext.Current.Response.AddHeader("Pragma", "no-cache");
                        HttpContext.Current.Response.AddHeader("Expires", "0");
                        bool value = Utility.getPasswordReset(Session["firstname"].ToString().Trim());
                        if (value)
                        {
                            if (!IsPostBack)
                            {

                                LoadUser();
                                RecordUserAction("Read", "Page Load Successfull", "Success", "NA", 1);
                            }
                        }
                        else
                        {
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('As per new Security Guidelines, you are requested to reset the password');window.location ='ChangePassword.aspx';", true);
                        }


                        
                    }
                }
                else
                {
                    //you are not allowed here
                }
            }
        }
        private bool checkroles()
        {
            bool role_hacked = false;
            List<int> AllowedRoles = new List<int> { 1, 2 };
            if (Context.Session != null && !AllowedRoles.Contains(Convert.ToInt32(Session["ROLE"])))
                role_hacked = true;
            return role_hacked;
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
        protected void LoadUser()
        {
            NpgsqlConnection conn = new NpgsqlConnection();
            NpgsqlCommand cmd = new NpgsqlCommand();
            conn.ConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["connect"].ConnectionString;
            cmd.Connection = conn;
            try
            {
                conn.Open();
                string query = "";
                if (Convert.ToInt32(Session["ROLE"].ToString()) == 1)
                {
                    query = "select concat(userfirstname,' ',usermiddlename,' ',userlastname) as user_name , username ,user_designation ,usermobileno ,district_id,active";
                    query = query + " FROM esociety.admin_table order by district_id";
                }
                else if (Convert.ToInt32(Session["ROLE"].ToString()) == 2)
                {
                    query = "select concat(userfirstname,' ',usermiddlename,' ',userlastname) as user_name , username ,user_designation ,usermobileno ,district_id, active";
                    query = query + " FROM esociety.admin_table where role_id >=3 order by district_id";
                }
                else
                {

                    Response.Redirect("Dashboard.aspx");
                }

                cmd.CommandText = query;
                using (NpgsqlDataAdapter dr = new NpgsqlDataAdapter(cmd))
                {
                    DataTable dt = new DataTable();
                    dr.Fill(dt);
                    GridViewUser.DataSource = dt;
                    GridViewUser.DataBind();
                }
                Label1.Text = GridViewUser.Rows.Count.ToString();
            }
            catch (NpgsqlException ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "LoadUser()" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath) + " from " + ipaddress);
            }
            finally
            {
                conn.Close();
            }
        }

        protected void GridViewUser_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType.Equals(DataControlRowType.DataRow))
                {
                    string y = e.Row.Cells[5].Text;
                    e.Row.Cells[5].Text = Encryption.Encrypt.Decrypt(y);
                }

                foreach (GridViewRow row in GridViewUser.Rows)
                {
                    string active = ((Label)GridViewUser.Rows[row.RowIndex].FindControl("Lbactive")).Text;
                    LinkButton activate = (LinkButton)row.FindControl("LbActivate");
                    LinkButton deactivate = (LinkButton)row.FindControl("LbDeactivate");
                    LinkButton reset = (LinkButton)row.FindControl("LbResetPassword");
                    //LinkButton transfer = (LinkButton)row.FindControl("LbTransfer");

                    if (active == "N")
                    {
                        TableCell statusCell = e.Row.Cells[10];
                        activate.Enabled = true;
                        deactivate.Enabled = false;
                        reset.Enabled = false;
                        if (e.Row.RowType.Equals(DataControlRowType.DataRow))
                        {
                            statusCell.Text = "Deactivated";
                        }
                    }
                    else if (active == "Y")
                    {
                        TableCell statusCell = e.Row.Cells[10];
                        activate.Enabled = false;
                        deactivate.Enabled = true;
                        reset.Enabled = true;
                        if (e.Row.RowType.Equals(DataControlRowType.DataRow))
                        {
                            statusCell.Text = "Active";
                        }

                    }
                    else if (active == "L")
                    {
                        activate.Enabled = true;
                        deactivate.Enabled = false;
                        reset.Enabled = false;
                        TableCell statusCell = e.Row.Cells[10];
                        if (e.Row.RowType.Equals(DataControlRowType.DataRow))
                        {
                            statusCell.Text = "Locked";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "GridViewUser_RowDataBound" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath) + " from " + ipaddress);
            }
        }

        protected void LbActivate_Click(object sender, EventArgs e)
        {
            try
            {
                RecordUserAction("Update", "LbActivate Click Modal Open", "Success", "NA", 1);
                LinkButton btn = (LinkButton)sender;
                GridViewRow row = (GridViewRow)btn.NamingContainer;
                string userid = ((Label)GridViewUser.Rows[row.RowIndex].FindControl("LbLoginName")).Text;
                Label3.Text = userid;
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "none", "<script> $(window).on('load', function () { $('#ModalActivate').modal({ backdrop: 'static' });});</script>", false);
            }
            catch (Exception ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "LbActivate_Click" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath) + " from " + ipaddress);
            }
        }

        protected void LbDeactivate_Click(object sender, EventArgs e)
        {
            try
            {
                LinkButton btn = (LinkButton)sender;
                GridViewRow row = (GridViewRow)btn.NamingContainer;
                string userid = ((Label)GridViewUser.Rows[row.RowIndex].FindControl("LbLoginName")).Text;
                Label11.Text = userid;
                RecordUserAction("Update", "LbDeactivate_Click Button Cliked", "Success", "NA", 1);
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "none", "<script> $(window).on('load', function () { $('#ModalDeactivate').modal({ backdrop: 'static' });});</script>", false);
            }
            catch (Exception ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "LbDeactivate_Click" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath) + " from " + ipaddress);
            }
        }

        protected void LbResetPassword_Click(object sender, EventArgs e)
        {
            try
            {

                LinkButton btn = (LinkButton)sender;
                GridViewRow row = (GridViewRow)btn.NamingContainer;
                string userid = ((Label)GridViewUser.Rows[row.RowIndex].FindControl("LbLoginName")).Text;
                Label16.Text = userid;
                RecordUserAction("Update", "LbResetPassword_Click Button Cliked", "Success", "NA", 1);
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "none", "<script> $(window).on('load', function () { $('#ModalReset').modal({ backdrop: 'static' });});</script>", false);

            }
            catch (Exception ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "LbResetPassword_Click" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath) + " from " + ipaddress);
            }
        }

        protected void LkUpdate_Click(object sender, EventArgs e)
        {
            try
            {

                if (TxtBxNewMobile.Text == "" || TxtBxNewMobile.Text == null)
                {
                    TxtBxNewMobile.Focus();
                    error.Text = "Mobile No is Blank";
                    error.Visible = true;
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "none", "<script> $(window).on('load', function () { $('#UpdateMobile').modal({ backdrop: 'static' });});</script>", false);
                }
                else if (!val.validateData(TxtBxNewMobile.Text, val.mobile_regex))
                {
                    TxtBxNewMobile.Focus();
                    error.Text = "Mobile No is Not Valid";
                    error.Visible = true;
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "none", "<script> $(window).on('load', function () { $('#UpdateMobile').modal({ backdrop: 'static' });});</script>", false);
                }
                else
                {
                    error.Visible = false; //usermobileno
                    int ac = UpdateUserHistory(Server.HtmlEncode(Label20.Text), "Mobile No update", 4);
                    if (ac == 1)
                    {
                        NpgsqlConnection conn = new NpgsqlConnection();
                        NpgsqlCommand cmd = new NpgsqlCommand();
                        conn.ConnectionString = ConfigurationManager.ConnectionStrings["connect"].ConnectionString;
                        cmd.Connection = conn;
                        try
                        {
                            conn.Open();
                            string query = "UPDATE esociety.admin_table SET usermobileno = @usermobileno WHERE username = @username";
                            cmd.CommandText = query;
                            cmd.Parameters.AddWithValue("@usermobileno", Encrypt.Encryptt(Server.HtmlEncode(TxtBxNewMobile.Text)));
                            cmd.Parameters.AddWithValue("@username", Server.HtmlEncode(Label20.Text));
                            cmd.ExecuteNonQuery();
                            Label7.Text = Label20.Text;
                            Label8.Text = "Mobile No changed Successfully!!!!";
                            RecordUserAction("Update", "LbMobile_Click updated successfully", "Success", "NA", 1);
                            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "none", "<script> $(window).on('load', function () { $('#ConfirmationModal').modal({ backdrop: 'static' });});</script>", false);

                        }
                        catch (NpgsqlException ex)
                        {

                            CreateLogFiles Err = new CreateLogFiles();
                            Err.ErrorLog(HttpContext.Current.Server.MapPath("~/Logs/ErrorLog"), ex.Message, "Lkconfirm_Click");
                            Response.Write("<script language='javascript'>alert('Failed, Please try again.')</script>");
                        }
                        finally
                        {
                            conn.Close();
                        }
                    }
                    else
                    {
                        Label7.Text = Label20.Text;
                        Label8.Text = "Error encountered.Please try again after sometime.";
                        RecordUserAction("Update", "LbMobile_Click updation failed", "Failed", "NA", 1);
                        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "none", "<script> $(window).on('load', function () { $('#ConfirmationModal').modal({ backdrop: 'static' });});</script>", false);
                    }
                    LoadUser();
                }
            }
            catch (Exception ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "LkUpdate_Click" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath) + " from " + ipaddress);
            }
        }

        protected void LkConfirm_Click(object sender, EventArgs e)
        {
            try
            {
                RecordUserAction("Update", "LkConfirm_Click Button Click", "Success", "NA", 1);
                if (Label3.Text != null)
                {
                    int a = UpdateUserHistory(Server.HtmlEncode(Label3.Text), "Account Activated", 1);
                    if (a == 1)
                    {
                        Label7.Text = Label3.Text;
                        Label8.Text = "Account Activated Successfully!!!!";
                        RecordUserAction("Update", "LbActivate_Click account activated successfull", "Success", "NA", 1);
                        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "none", "<script> $(window).on('load', function () { $('#ConfirmationModal').modal({ backdrop: 'static' });});</script>", false);

                    }
                    else
                    {
                        Label7.Text = Label3.Text;
                        Label8.Text = "Could not activate the account.Try again after sometimes";
                        RecordUserAction("Update", "LbActivate_Click account activated failed", "Failed", "NA", 1);
                        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "none", "<script> $(window).on('load', function () { $('#ConfirmationModal').modal({ backdrop: 'static' });});</script>", false);
                    }
                    LoadUser();
                }
            }
            catch (Exception ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "LkConfirm_Click" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath) + " from " + ipaddress);
            }
        }
        protected int UpdateUserHistory(string username, string reason, int action)///1--activate 2---deactivate 3---reset password 4----mobile no update
        {
            RecordUserAction("Update", "UpdateUserHistory", "Success", "NA", 1);
            int success = 0;
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
                cmd.Parameters.AddWithValue("@username", username);
                NpgsqlDataReader rd = cmd.ExecuteReader();
                if (rd.Read())
                {
                    success = 1;
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
                    success = 0;
                }
                rd.Close();
                cmd.Parameters.Clear();
                string query_ins_history = "INSERT INTO esociety.usertable_history( username, mobileno, user_password, password_updateat,";
                query_ins_history = query_ins_history + " password_updated_by, ipaddress, macaddress, action_done,old_status) 	VALUES (@username, @mobileno,";
                query_ins_history = query_ins_history + " @user_password, CURRENT_TIMESTAMP , @password_updated_by, @ipaddress, @macaddress, @action_done,@old_status)";
                cmd.CommandText = query_ins_history;
                cmd.Parameters.AddWithValue("@username", username);
                cmd.Parameters.AddWithValue("@mobileno", ViewState["usermobileno"].ToString());
                cmd.Parameters.AddWithValue("@user_password", ViewState["user_password"].ToString());
                cmd.Parameters.AddWithValue("@password_updated_by", Session["firstname"].ToString());
                cmd.Parameters.AddWithValue("@ipaddress", ViewState["ipaddress"]);
                cmd.Parameters.AddWithValue("@macaddress", ViewState["macaddress"]);
                cmd.Parameters.AddWithValue("@action_done", reason);
                cmd.Parameters.AddWithValue("@old_status", ViewState["active"].ToString());
                cmd.ExecuteNonQuery();
                cmd.Parameters.Clear();
                if (action == 1)
                {
                    string updatestatus = "UPDATE esociety.admin_table SET active = 'Y' WHERE username = @username";
                    cmd.CommandText = updatestatus;
                    cmd.Parameters.AddWithValue("@username", username);
                    cmd.ExecuteNonQuery();
                    trans.Commit();
                    success = 1;
                }
                else if (action == 2)
                {
                    string updatestatus = "UPDATE esociety.admin_table SET active = 'N' WHERE username = @username";
                    cmd.CommandText = updatestatus;
                    cmd.Parameters.AddWithValue("@username", username);
                    cmd.ExecuteNonQuery();
                    trans.Commit();
                    success = 1;
                }
                else if (action == 3)
                {
                    string updatestatus = "UPDATE esociety.admin_table SET user_password = @user_password , passwordgenerated = 1";
                    updatestatus = updatestatus + " WHERE username = @username";
                    cmd.CommandText = updatestatus;
                    cmd.Parameters.AddWithValue("@username", username);
                    cmd.Parameters.AddWithValue("@user_password", DBNull.Value);
                    cmd.ExecuteNonQuery();
                    trans.Commit();
                    success = 1;
                }
                else if (action == 4)
                {
                    trans.Commit();
                    success = 1;
                    RecordUserAction("Update", "UpdateUserHistory updated successfully", "Success", "NA", 1);
                }
                else
                {
                    trans.Rollback();
                    success = 0;
                    RecordUserAction("Update", "UpdateUserHistory updation failed", "Failed", "NA", 1);
                }
            }
            catch (NpgsqlException ex)
            {
                success = 0;
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(HttpContext.Current.Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "UpdateUserHistory()" + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath) + " from " + ipaddress);
                Response.Write("<script language='javascript'>alert('Error Encountered while deleting....')</script>");
                trans.Rollback();
            }
            finally
            {
                conn.Close();
            }
            return success;

        }
        protected void LkconfirmDeactivate_Click(object sender, EventArgs e)
        {
            try
            {
                if (Label11.Text != null)
                {
                    int a = UpdateUserHistory(Server.HtmlEncode(Label11.Text), "Account Deativated", 2);
                    if (a == 1)
                    {
                        Label7.Text = Label11.Text;
                        Label8.Text = "Account Deactivate Successfully!!!!";
                        RecordUserAction("Update", "LbDeactivate_Click account deactivated successfull", "Success", "NA", 1);
                        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "none", "<script> $(window).on('load', function () { $('#ConfirmationModal').modal({ backdrop: 'static' });});</script>", false);

                    }
                    else
                    {
                        Label7.Text = Label11.Text;
                        Label8.Text = "Could not deactivate the account.Please try again after sometime.";
                        RecordUserAction("Update", "LbDeactivate_Click account deactivated failed", "Failed", "NA", 1);
                        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "none", "<script> $(window).on('load', function () { $('#ConfirmationModal').modal({ backdrop: 'static' });});</script>", false);
                    }
                    LoadUser();
                }
            }
            catch (Exception ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "LkconfirmDeactivate_Click" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath) + " from " + ipaddress);
            }
        }

        protected void LkResetPassword_Click(object sender, EventArgs e)
        {

            try
            {
                if (Label16.Text != null)
                {
                    int a = UpdateUserHistory(Server.HtmlEncode(Label16.Text), "Account Deativated", 3);
                    if (a == 1)
                    {
                        Label7.Text = Label16.Text;
                        Label8.Text = "Password reset done";
                        RecordUserAction("Update", "LbResetPassword_Click account reset successfull", "Success", "NA", 1);
                        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "none", "<script> $(window).on('load', function () { $('#ConfirmationModal').modal({ backdrop: 'static' });});</script>", false);

                    }
                    else
                    {
                        Label7.Text = Label16.Text;
                        Label8.Text = "Password reset failed!!. Please try after some time.";
                        RecordUserAction("Update", "LbResetPassword_Click account reset failed", "Failed", "NA", 1);
                        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "none", "<script> $(window).on('load', function () { $('#ConfirmationModal').modal({ backdrop: 'static' });});</script>", false);
                    }
                    LoadUser();
                }
            }
            catch (Exception ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "LkResetPassword_Click" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath) + " from " + ipaddress);
            }
        }

        protected void permission_Click(object sender, EventArgs e)
        {
            RecordUserAction("Redirect", "Redirection to Dashboard", "Success", "NA", 2);
            Response.Redirect("Dashboard.aspx");
        }

        protected void LbMobile_Click(object sender, EventArgs e)
        {

            try
            {
                RecordUserAction("Update", "LbMobile_Click Button Clicked", "Success", "NA", 1);
                LinkButton btn = (LinkButton)sender;
                GridViewRow row = (GridViewRow)btn.NamingContainer;
                string userid = ((Label)GridViewUser.Rows[row.RowIndex].FindControl("LbLoginName")).Text;
                string mobile = ((Label)GridViewUser.Rows[row.RowIndex].FindControl("LbMobiles")).Text;
                Label20.Text = userid;
                Label23.Text = Encryption.Encrypt.Decrypt(mobile);
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "none", "<script> $(window).on('load', function () { $('#UpdateMobile').modal({ backdrop: 'static' });});</script>", false);

            }
            catch (Exception ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "LbMobile_Click" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath) + " from " + ipaddress);
            }
        }
      
        protected void GridViewUser_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            GridViewUser.PageIndex = e.NewPageIndex;
            LoadUser();
        }
    }
}