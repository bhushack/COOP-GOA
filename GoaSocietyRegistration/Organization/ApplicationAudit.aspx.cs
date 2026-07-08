using GoaSocietyRegistration.Development;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace GoaSocietyRegistration.Organization
{
    public partial class ApplicationAudit : System.Web.UI.Page
    {
        string ipaddress = Utility.getIP();
        Insert ins = new Insert();
        System.Globalization.CultureInfo french = new System.Globalization.CultureInfo("fr-FR");
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Check4Tampering() || Page.RouteData.RouteHandler == null)
            {

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
                if (!IsPostBack)
                {
                    SearchAuditLogs();
                    SearchLoginout();
                    FillUserId();
                }
                TabName.Value = Request.Form[TabName.UniqueID];
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
        private void SearchLoginout()
        {
            NpgsqlConnection conn = new NpgsqlConnection();
            conn.ConnectionString = ConfigurationManager.ConnectionStrings["connect"].ConnectionString;
            try
            {
                using (NpgsqlCommand cmd = new NpgsqlCommand())
                {
                    string query = "";
                    query = " SELECT user_loginname, user_logintime, user_logouttime, ipaddress, user_fullname FROM esociety.loginentries ";


                    conn.Open();
                    cmd.CommandText = query;
                    if (!string.IsNullOrEmpty(TextBox1.Text.Trim()) && DropDownList1.SelectedIndex == 0)
                    {
                        query += " where  Date(user_logintime) = @user_logintime ORDER BY user_logintime DESC";
                        cmd.Parameters.AddWithValue("@user_logintime", Convert.ToDateTime(TextBox1.Text, french).Date);

                    }
                    else if (!string.IsNullOrEmpty(TextBox1.Text.Trim()) && DropDownList1.SelectedIndex != -1)
                    {
                        query += " where Date(user_logintime) = @user_logintime and user_loginname = @user_loginname ORDER BY user_logintime DESC";
                        cmd.Parameters.AddWithValue("@user_logintime", Convert.ToDateTime(TextBox1.Text, french).Date);
                        cmd.Parameters.AddWithValue("@user_loginname", DropDownList1.SelectedItem.Text);

                    }
                    else if (DropDownList1.SelectedIndex != -1)
                    {
                        query += " where user_loginname = @user_loginname ORDER BY user_logintime DESC";
                        cmd.Parameters.AddWithValue("@user_loginname", DropDownList1.SelectedItem.Text);
                    }
                    else
                    {


                    }
                    cmd.CommandText = query;
                    cmd.Connection = conn;
                    using (NpgsqlDataAdapter dr = new NpgsqlDataAdapter(cmd))
                    {
                        DataTable dt = new DataTable();
                        dr.Fill(dt);
                        Gridview1.DataSource = dt;
                        Gridview1.DataBind();
                    }
                }


            }
            catch (NpgsqlException ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "SearchAuditLogs()" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
                RecordUserAction("Read", " Read SearchAuditLogs", "Failed", "NA", 1);
                Response.Write("<script language='javascript'>alert('" + "Exception at ApplicationAudit Data" + "')</script>");
            }
            finally
            {
                conn.Close();
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
        private void SearchAuditLogs()
        {
            NpgsqlConnection conn = new NpgsqlConnection();
            conn.ConnectionString = ConfigurationManager.ConnectionStrings["connect"].ConnectionString;
            try
            {
                using (NpgsqlCommand cmd = new NpgsqlCommand())
                {
                    string query = "";
                    query = " SELECT tracked_datetime, admin_login_id,app_id,  action_performed, action_description, accessed_module,  action_status, ipaddress FROM esociety.admin_audit_trail ";


                    conn.Open();
                    cmd.CommandText = query;
                    if (!string.IsNullOrEmpty(TxtBxDate.Text.Trim()) && ddlrole.SelectedIndex == 0)
                    {
                        query += " where (loggedin_status = @loggedin_status or loggedin_status = 'N' ) and Date(tracked_datetime) = @tracked_datetime ORDER BY tracked_datetime DESC";
                        cmd.Parameters.AddWithValue("@loggedin_status", 'Y');
                        cmd.Parameters.AddWithValue("@tracked_datetime", Convert.ToDateTime(TxtBxDate.Text, french).Date);

                    }
                    else if (!string.IsNullOrEmpty(TxtBxDate.Text.Trim()) && ddlrole.SelectedIndex != -1)
                    {
                        query += " where loggedin_status = @loggedin_status and Date(tracked_datetime) = @tracked_datetime and admin_login_id = @admin_login_id ORDER BY tracked_datetime DESC";
                        cmd.Parameters.AddWithValue("@loggedin_status", 'Y');
                        cmd.Parameters.AddWithValue("@tracked_datetime", Convert.ToDateTime(TxtBxDate.Text, french).Date);
                        cmd.Parameters.AddWithValue("@admin_login_id", ddlrole.SelectedItem.Text);

                    }
                    else
                    {
                        query += " where loggedin_status = @loggedin_status or loggedin_status = 'N' ORDER BY tracked_datetime DESC";
                        cmd.Parameters.AddWithValue("@loggedin_status", 'Y');

                    }
                    cmd.CommandText = query;
                    cmd.Connection = conn;
                    using (NpgsqlDataAdapter dr = new NpgsqlDataAdapter(cmd))
                    {
                        DataTable dt = new DataTable();
                        dr.Fill(dt);
                        GridviewAuditLogs.DataSource = dt;
                        GridviewAuditLogs.DataBind();
                    }
                }


            }
            catch (NpgsqlException ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "SearchAuditLogs()" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
                RecordUserAction("Read", " Read SearchAuditLogs", "Failed", "NA", 1);
                Response.Write("<script language='javascript'>alert('" + "Exception at ApplicationAudit Data" + "')</script>");
            }
            finally
            {
                conn.Close();
            }

        }
        public void FillUserId()
        {
            NpgsqlConnection connect = new NpgsqlConnection();
            NpgsqlCommand cmd = new NpgsqlCommand();
            connect.ConnectionString = ConfigurationManager.ConnectionStrings["connect"].ConnectionString;
            cmd.Connection = connect;
            NpgsqlDataAdapter adapter;
            DataSet ds;
            try
            {
                connect.Open();
                string query = "SELECT username, id FROM esociety.admin_table where role_id >= 2 ORDER BY role_id DESC";
                cmd.CommandText = query;
                adapter = new NpgsqlDataAdapter(cmd);
                ds = new DataSet();
                adapter.Fill(ds, "esociety.admin_table");
                ddlrole.DataSource = ds.Tables[0];
                DropDownList1.DataSource = ds.Tables[0];
                ddlrole.DataTextField = "username";
                ddlrole.DataValueField = "id";
                ddlrole.DataBind();
                ddlrole.Items.Insert(0, new System.Web.UI.WebControls.ListItem("-- Select --", "-1"));

                DropDownList1.DataTextField = "username";
                DropDownList1.DataValueField = "id";
                DropDownList1.DataBind();
                DropDownList1.Items.Insert(0, new System.Web.UI.WebControls.ListItem("-- Select --", "-1"));
                cmd.Dispose();
                adapter.Dispose();
                ds = null;
            }
            catch (NpgsqlException ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "SearchAuditLogs()" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));

                RecordUserAction("Read", "Read FillSocietyType", "Failed", "NA", 2);
                Response.Write("<script language='javascript'>alert('Soceity Type:" + "Society Dropdown Loading Error" + "')</script>");
            }
            finally
            {
                connect.Close();
            }
        }
        protected void seach_adudit_logs_Click(object sender, EventArgs e)
        {
            SearchAuditLogs();
            RecordUserAction("Read", " Read SearchAuditLogs", "Success", "NA", 1);
        }

        protected void permission_Click(object sender, EventArgs e)
        {
            Response.Redirect("Dashboard.aspx");
        }

        protected void GridviewAuditLogs_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType == DataControlRowType.Footer)
                {
                    e.Row.Cells[0].Text = "Page " + (GridviewAuditLogs.PageIndex + 1) + " of " + GridviewAuditLogs.PageCount;
                }
            }
            catch (Exception ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "GridviewAuditLogs_RowDataBound" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
            }
        }

        protected void GridviewAuditLogs_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            GridviewAuditLogs.PageIndex = e.NewPageIndex;
            SearchAuditLogs();
        }
        public override void VerifyRenderingInServerForm(Control control)
        {
            /* Verifies that the control is rendered */
        }
        protected void export_logs_excel_Click(object sender, EventArgs e)
        {
            RecordUserAction("Read", "Downloaded Excel of AuditLogs", "Success", "NA", 1);
            try
            {
                Response.Clear();
                Response.Buffer = true;
                Response.AddHeader("content-disposition", "attachment;filename=AuditLogs.xls");
                Response.Charset = "";
                Response.ContentType = "application/vnd.ms-excel";

                using (StringWriter sw = new StringWriter())
                {
                    HtmlTextWriter hw = new HtmlTextWriter(sw);

                    //To Export all pages
                    GridviewAuditLogs.AllowPaging = false;
                    this.SearchAuditLogs();

                    GridviewAuditLogs.HeaderRow.BackColor = System.Drawing.Color.White;
                    foreach (TableCell cell in GridviewAuditLogs.HeaderRow.Cells)
                    {
                        cell.BackColor = GridviewAuditLogs.HeaderStyle.BackColor;
                    }
                    foreach (GridViewRow row in GridviewAuditLogs.Rows)
                    {
                        row.BackColor = Color.White;
                        foreach (TableCell cell in row.Cells)
                        {
                            if (row.RowIndex % 2 == 0)
                            {
                                cell.BackColor = GridviewAuditLogs.AlternatingRowStyle.BackColor;
                            }
                            else
                            {
                                cell.BackColor = GridviewAuditLogs.RowStyle.BackColor;
                            }
                            cell.CssClass = "textmode";
                        }
                    }

                    GridviewAuditLogs.RenderControl(hw);

                    //style to format numbers to string
                    string style = @"<style> .textmode { } </style>";
                    Response.Write(style);
                    Response.Output.Write(sw.ToString());
                    Response.Flush();
                    Response.End();
                }
            }
            catch (Exception ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "export_logs_excel_Click" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
            }


        }

        protected void LinkButton1_Click(object sender, EventArgs e)
        {
            SearchLoginout();
            RecordUserAction("Read", " Read SearchloginoutLogs", "Success", "NA", 1);
        }

        protected void Gridview1_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType == DataControlRowType.Footer)
                {
                    e.Row.Cells[0].Text = "Page " + (Gridview1.PageIndex + 1) + " of " + Gridview1.PageCount;
                }
            }
            catch (Exception ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "Gridview1_RowDataBound" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
            }
        }

        protected void Gridview1_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            Gridview1.PageIndex = e.NewPageIndex;
            SearchLoginout();
        }

        protected void LinkButton2_Click(object sender, EventArgs e)
        {
            RecordUserAction("Read", "Downloaded Excel of AuditLogs", "Success", "NA", 1);
            try
            {
                Response.Clear();
                Response.Buffer = true;
                Response.AddHeader("content-disposition", "attachment;filename=AuditLogin.xls");
                Response.Charset = "";
                Response.ContentType = "application/vnd.ms-excel";

                using (StringWriter sw = new StringWriter())
                {
                    HtmlTextWriter hw = new HtmlTextWriter(sw);

                    //To Export all pages
                    Gridview1.AllowPaging = false;
                    this.SearchLoginout();

                    Gridview1.HeaderRow.BackColor = System.Drawing.Color.White;
                    foreach (TableCell cell in Gridview1.HeaderRow.Cells)
                    {
                        cell.BackColor = Gridview1.HeaderStyle.BackColor;
                    }
                    foreach (GridViewRow row in Gridview1.Rows)
                    {
                        row.BackColor = Color.White;
                        foreach (TableCell cell in row.Cells)
                        {
                            if (row.RowIndex % 2 == 0)
                            {
                                cell.BackColor = Gridview1.AlternatingRowStyle.BackColor;
                            }
                            else
                            {
                                cell.BackColor = Gridview1.RowStyle.BackColor;
                            }
                            cell.CssClass = "textmode";
                        }
                    }

                    Gridview1.RenderControl(hw);

                    //style to format numbers to string
                    string style = @"<style> .textmode { } </style>";
                    Response.Write(style);
                    Response.Output.Write(sw.ToString());
                    Response.Flush();
                    Response.End();
                }
            }
            catch (Exception ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "LinkButton2_Click" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
            }
        }
    }
}