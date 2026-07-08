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
    public partial class PendingApplications : System.Web.UI.Page
    {
        Insert ins = new Insert();
        System.Globalization.CultureInfo french = new System.Globalization.CultureInfo("fr-FR");
        string ipaddress = Utility.getIP();
        string macaddress = Utility.GetMACAddress(Utility.getIP());
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
                HttpContext.Current.Response.AddHeader("Cache-Control", "no-cache, no-store, must-revalidate");
                HttpContext.Current.Response.AddHeader("Pragma", "no-cache");
                HttpContext.Current.Response.AddHeader("Expires", "0");
                if (!IsPostBack)
                {
                Utility.FillDistrictSoc(ddlDistrict);
                
                if (Convert.ToInt32(Session["ROLE"]) == 3)
                {
                    ddlDistrict.SelectedValue = Session["DistrictID"].ToString();
                    ddlDistrict.Enabled = false;
                }
                   
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


        protected void permission_Click(object sender, EventArgs e)
        {
            Response.Redirect("Dashboard.aspx");
        }

        protected void callErrorModal(string msg)
        {
            Label4.Text = Server.HtmlEncode(msg);
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "none", "<script> $(window).on('load', function () { $('#myerrorModal').modal({ backdrop: 'static' });});</script>", false);
        }

        protected void SearchButton_Click(object sender, EventArgs e)
        {
            try
            {             
              
                if (TxtBxFromDate.Text == "" && TxtBxToDate.Text == "")
                {
                    callErrorModal("Please Enter From and To date"); print_table.Visible = false; printbtn.Visible = false;
                }
                else if (TxtBxFromDate.Text == "")
                {
                    callErrorModal("Please Enter From Date"); print_table.Visible = false; printbtn.Visible = false;
                }
                else if (TxtBxToDate.Text == "")
                {
                    callErrorModal("Please Enter To Date"); print_table.Visible = false; printbtn.Visible = false;
                }
                else if (ddlDistrict.SelectedValue == "-1")
                {
                    callErrorModal("Please Select District"); print_table.Visible = false; printbtn.Visible = false;
                }
                else
                {
                    DateTime d1 = Convert.ToDateTime(TxtBxFromDate.Text, french);
                    DateTime d2 = Convert.ToDateTime(TxtBxToDate.Text, french);

                    if (d1 > d2)
                    {
                        callErrorModal("From Date should be earlier than To Date"); print_table.Visible = false; printbtn.Visible = false;
                    }
                    else
                    {
                        print_table.Visible = true; printbtn.Visible = true;

                        loadNewCases();

                        loadObsCases();

                    }
                }
            }
            catch (Exception ex)
            {
                print_table.Visible = false; printbtn.Visible = false;
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "SearchButton_Click" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath) + " from " + ipaddress);
            }
        }


        protected void loadNewCases()
        {//new case
            NpgsqlConnection conn = new NpgsqlConnection();
            NpgsqlCommand cmd = new NpgsqlCommand();
            conn.ConnectionString = ConfigurationManager.ConnectionStrings["connect"].ConnectionString;
            cmd.Connection = conn;
            try
            {
                conn.Open();
                string query1 = "SELECT esociety.status_table.app_id, status_table.login_id,  society.socname, applicant_details.new_or_renewal, status_table.status_id, status_table.application_submission_time from esociety.status_table inner join esociety.society";
                query1 = query1 + " on esociety.status_table.app_id = esociety.society.app_id inner join esociety.applicant_details on esociety.society.app_id=esociety.applicant_details.app_id where (status_table.status_id=3 or status_table.status_id=4) and Date(esociety.status_table.application_submission_time)";
                query1 = query1 + " between @submit_fromdate and @submit_todate and society.socdistrict = @socdistrict order by status_table.application_submission_time DESC";
                              
                cmd.CommandText = query1;
                cmd.Parameters.AddWithValue("@socdistrict", Convert.ToInt32(ddlDistrict.SelectedValue));
                cmd.Parameters.AddWithValue("@submit_fromdate", Convert.ToDateTime(TxtBxFromDate.Text, french).Date);
                cmd.Parameters.AddWithValue("@submit_todate", Convert.ToDateTime(TxtBxToDate.Text, french).Date);
                using (NpgsqlDataAdapter dr = new NpgsqlDataAdapter(cmd))
                {
                    DataTable dt = new DataTable();
                    dr.Fill(dt);
                    gvnewappln.DataSource = dt;
                    gvnewappln.DataBind();
                }
                RecordUserAction("Read", "New Applications loaded in Gridview", "S", "NA", 1);

            }
            catch (NpgsqlException ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "loadNewCases()" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath) + " from " + ipaddress);
                RecordUserAction("Read", "Error while loading New Applications in Gridview", "F", "NA", 1);

            }
            finally
            {
                conn.Close();
            }
        }

        protected void loadObsCases()
        {//new case
            NpgsqlConnection conn = new NpgsqlConnection();
            NpgsqlCommand cmd = new NpgsqlCommand();
            conn.ConnectionString = ConfigurationManager.ConnectionStrings["connect"].ConnectionString;
            cmd.Connection = conn;
            try
            {
                conn.Open();
                string query1 = "SELECT esociety.status_table.app_id, status_table.login_id,  society.socname, applicant_details.new_or_renewal, status_table.status_id, status_table.application_obs_submission_time from esociety.status_table inner join esociety.society";
                query1 = query1 + " on esociety.status_table.app_id = esociety.society.app_id inner join esociety.applicant_details on esociety.society.app_id=esociety.applicant_details.app_id where (status_table.status_id=6 or status_table.status_id=7) and Date(esociety.status_table.application_obs_submission_time)";
                query1 = query1 + " between @submit_fromdate and @submit_todate and society.socdistrict = @socdistrict order by status_table.application_obs_submission_time DESC";

                cmd.CommandText = query1;
                cmd.Parameters.AddWithValue("@socdistrict", Convert.ToInt32(ddlDistrict.SelectedValue));
                cmd.Parameters.AddWithValue("@submit_fromdate", Convert.ToDateTime(TxtBxFromDate.Text, french).Date);
                cmd.Parameters.AddWithValue("@submit_todate", Convert.ToDateTime(TxtBxToDate.Text, french).Date);
                using (NpgsqlDataAdapter dr = new NpgsqlDataAdapter(cmd))
                {
                    DataTable dt = new DataTable();
                    dr.Fill(dt);
                    gvobsappln.DataSource = dt;
                    gvobsappln.DataBind();
                }
                RecordUserAction("Read", "Obs Applications loaded in Gridview", "S", "NA", 1);

            }
            catch (NpgsqlException ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "loadObsCases()" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath) + " from " + ipaddress);
                RecordUserAction("Read", "Exception while loading Obs Applications in Gridview", "F", "NA", 1);
            }
            finally
            {
                conn.Close();
            }
        }

            
        protected void gvnewappln_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType.Equals(DataControlRowType.DataRow))
                {
                    if (e.Row.Cells[4].Text.Contains("1"))
                    {
                        string str = e.Row.Cells[4].Text;
                        str = "New";
                        e.Row.Cells[4].Text = str;
                        
                    }
                    else if (e.Row.Cells[4].Text.Contains("2"))
                    {
                        string str = e.Row.Cells[4].Text;
                        str = "Renewal";
                        e.Row.Cells[4].Text = str;
                      
                    }
                    else
                    {
                        string str = e.Row.Cells[4].Text;
                        str = "NA";
                        e.Row.Cells[4].Text = str;
                    }


                    if (e.Row.Cells[5].Text.Contains("3"))
                    {
                        string str = e.Row.Cells[5].Text;
                        str = "Dealing Hand";
                        e.Row.Cells[5].Text = str;
                        //e.Row.Cells[7].Text = "New";
                    }
                    else if (e.Row.Cells[5].Text.Contains("4"))
                    {
                        string str = e.Row.Cells[5].Text;
                        str = "District Registrar";
                        e.Row.Cells[5].Text = str;
                        //e.Row.Cells[7].Text = "New";
                    }
                    else
                    {
                        string str = e.Row.Cells[5].Text;
                        str = "NA";
                        e.Row.Cells[5].Text = str;
                    }

                    
                }
            }
            catch (Exception ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "gvnewappln_RowDataBound" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath) + " from " + ipaddress);
            }
        }

       

        protected void gvobsappln_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType.Equals(DataControlRowType.DataRow))
                {
                    if (e.Row.Cells[4].Text.Contains("1"))
                    {
                        string str = e.Row.Cells[4].Text;
                        str = "New";
                        e.Row.Cells[4].Text = str;

                    }
                    else if (e.Row.Cells[4].Text.Contains("2"))
                    {
                        string str = e.Row.Cells[4].Text;
                        str = "Renewal";
                        e.Row.Cells[4].Text = str;

                    }
                    else
                    {
                        string str = e.Row.Cells[4].Text;
                        str = "NA";
                        e.Row.Cells[4].Text = str;
                    }


                    if (e.Row.Cells[5].Text.Contains("6"))
                    {
                        string str = e.Row.Cells[5].Text;
                        str = "Dealing Hand";
                        e.Row.Cells[5].Text = str;
                        //e.Row.Cells[7].Text = "New";
                    }
                    else if (e.Row.Cells[5].Text.Contains("7"))
                    {
                        string str = e.Row.Cells[5].Text;
                        str = "District Registrar";
                        e.Row.Cells[5].Text = str;
                        //e.Row.Cells[7].Text = "New";
                    }
                    else
                    {
                        string str = e.Row.Cells[5].Text;
                        str = "NA";
                        e.Row.Cells[5].Text = str;
                    }


                }
            }
            catch (Exception ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "gvobsappln_RowDataBound" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath) + " from " + ipaddress);
            }

        }

        protected void gvobsremarks_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                string text = e.Row.Cells[1].Text;
                string[] Result = text.Split('|');
                int m = e.Row.RowIndex;
                if (e.Row.RowIndex >= 0)
                {

                    // e.Row.Cells[1].Text = "<b>Society Obs :</b> " + Server.HtmlEncode(Result[0]) + "<br />" +
                    //"<b>Members Obs :</b> " + Server.HtmlEncode(Result[1]);

                     e.Row.Cells[1].Text =  Server.HtmlEncode(Result[1]);

                }


            }
            catch (Exception ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "gvhistory_RowDataBound" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
            }
        }

        protected void LkViewDetails_Click(object sender, EventArgs e)
        {

            try
            {
                LinkButton btn = (LinkButton)sender;
                GridViewRow row = (GridViewRow)btn.NamingContainer;
                string app_id = ((Label)gvobsappln.Rows[row.RowIndex].FindControl("LkobsAppID")).Text;

                if (app_id != null || app_id != "")
                {
                    loadgridsubmittedtime(app_id);
                    loadgridremarkshistory(app_id);
                   
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "none", "<script> $(window).on('load', function () { $('#remarkshistory').modal({ backdrop: 'static' });});</script>", false);
                }
                else
                {
                    Label69.ForeColor = System.Drawing.Color.Red;
                    Label69.Text = "Failed..Try Again ..";
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "none", "<script> $(window).on('load', function () { $('#MyErrorModal').modal({ backdrop: 'static' });});</script>", false);

                }
            }
            catch (Exception ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "LkViewDetails_Click " + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath) + " from " + ipaddress);

            }
        }


        protected void loadgridsubmittedtime(string app_id)
        {
            NpgsqlConnection conn = new NpgsqlConnection();
            NpgsqlCommand cmd = new NpgsqlCommand();
            conn.ConnectionString = ConfigurationManager.ConnectionStrings["connect"].ConnectionString;
            cmd.Connection = conn;
            try
            {
                string query = "SELECT submitted_at FROM esociety.application_submission_history where app_id = @app_id";
                cmd.CommandText = query;
                cmd.Parameters.AddWithValue("@app_id", Convert.ToInt64(app_id));
                using (NpgsqlDataAdapter dr = new NpgsqlDataAdapter(cmd))
                {
                    DataTable dt = new DataTable();
                    dr.Fill(dt);
                    gvobssubmittime.DataSource = dt;
                    gvobssubmittime.DataBind();
                }

            }
            catch (NpgsqlException ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "loadgridsubmittedtime()" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath) + " from " + ipaddress);

                // RecordUserAction("Read", "Function Load", "Failed", "NA", 2);
                Response.Write("<script language='javascript'>alert('" + "Error while loading...." + "')</script>");
            }
            finally
            {
                conn.Close();
            }
        }

        protected void loadgridremarkshistory(string app_id)
        {
            NpgsqlConnection conn = new NpgsqlConnection();
            NpgsqlCommand cmd = new NpgsqlCommand();
            conn.ConnectionString = ConfigurationManager.ConnectionStrings["connect"].ConnectionString;
            cmd.Connection = conn;
            try
            {
                string query = "SELECT  app_id, observation_by_dh, remarks_sendobservation, submit_time_remarkssendobservation FROM esociety.remarks_table";
                query = query + " where app_id = @app_id order by submit_time_remarkssendobservation ASC";
                cmd.CommandText = query;
                cmd.Parameters.AddWithValue("@app_id", Convert.ToInt64(app_id));
                using (NpgsqlDataAdapter dr = new NpgsqlDataAdapter(cmd))
                {
                    DataTable dt = new DataTable();
                    dr.Fill(dt);
                    gvobsremarks.DataSource = dt;
                    gvobsremarks.DataBind();
                }

            }
            catch (NpgsqlException ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "loadgridremarkshistory()" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath) + " from " + ipaddress);

                // RecordUserAction("Read", "Function Load", "Failed", "NA", 2);
                Response.Write("<script language='javascript'>alert('" + "Error while loading...." + "')</script>");
            }
            finally
            {
                conn.Close();
            }
        }


    }
}