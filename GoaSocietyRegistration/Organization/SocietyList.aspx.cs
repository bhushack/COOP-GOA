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
    public partial class SocietyList : System.Web.UI.Page
    {
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
            else if (checkroles() )
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

                bool value = Utility.getPasswordReset(Session["firstname"].ToString().Trim());
                if (value)
                {
                    if (!IsPostBack)
                    {
                        LoadAccepted();
                        LoadRejected();
                    }
                }
                else
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('As per new Security Guidelines, you are requested to reset the password');window.location ='ChangePassword.aspx';", true);
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
        protected void LoadAccepted()
        {
            NpgsqlConnection conn = new NpgsqlConnection();
            NpgsqlCommand cmd = new NpgsqlCommand();
            conn.ConnectionString = ConfigurationManager.ConnectionStrings["connect"].ConnectionString;
            cmd.Connection = conn;
            try
            {
                if (Convert.ToInt32(Session["ROLE"].ToString()) == 1 || Convert.ToInt32(Session["ROLE"].ToString()) == 2)
                {
                    string query = "";
                    query = "select applicant_details.app_id,applicant_details.applicant_name,mst_memberdesignation.\"DesignationName\",";
                    query = query + " society.socname,mst_societytype.societytype,esociety.status_table.status_id,status_table.application_submission_time,applicant_details.new_or_renewal from esociety.applicant_details";
                    query = query + " INNER JOIN esociety.society on applicant_details.app_id = society.app_id";
                    query = query + " INNER JOIN esociety.mst_memberdesignation on applicant_details.applicant_designation = mst_memberdesignation.\"DesignationID\"";
                    query = query + " INNER JOIN esociety.mst_societytype on mst_societytype.societyid = society.soctype";
                    query = query + " INNER JOIN esociety.status_table on esociety.status_table.app_id = esociety.applicant_details.app_id";
                    query = query + " INNER JOIN esociety.mst_district on esociety.mst_district.\"DistrictID\" =esociety.society.socdistrict";
                    query = query + " where (esociety.status_table.status_id >= 8 and esociety.status_table.status_id != 9) and (society.socdistrict= 551 or society.socdistrict=552) order by status_table.application_submission_time ASC";
                    conn.Open();
                    cmd.CommandText = query;
                    
                }
                else if (Convert.ToInt32(Session["ROLE"].ToString()) == 3 || Convert.ToInt32(Session["ROLE"].ToString()) == 4)
                {
                    string query = "";
                    query = "select applicant_details.app_id,applicant_details.applicant_name,mst_memberdesignation.\"DesignationName\",";
                    query = query + " society.socname,mst_societytype.societytype,esociety.status_table.status_id,status_table.application_submission_time,applicant_details.new_or_renewal from esociety.applicant_details";
                    query = query + " INNER JOIN esociety.society on applicant_details.app_id = society.app_id";
                    query = query + " INNER JOIN esociety.mst_memberdesignation on applicant_details.applicant_designation = mst_memberdesignation.\"DesignationID\"";
                    query = query + " INNER JOIN esociety.mst_societytype on mst_societytype.societyid = society.soctype";
                    query = query + " INNER JOIN esociety.status_table on esociety.status_table.app_id = esociety.applicant_details.app_id";
                    query = query + " INNER JOIN esociety.mst_district on esociety.mst_district.\"DistrictID\" =esociety.society.socdistrict";
                    query = query + " where (esociety.status_table.status_id >= 8 and esociety.status_table.status_id != 9) and society.socdistrict= @district order by status_table.application_submission_time ASC";
                    conn.Open();
                    cmd.CommandText = query;
                    cmd.Parameters.AddWithValue("@district", Convert.ToInt32(Session["DistrictID"].ToString()));


                }
              
                NpgsqlDataReader dr = cmd.ExecuteReader();
                GridviewAccepted.DataSource = dr;
                GridviewAccepted.DataBind();
                dr.Close();
                Label3.Text = Server.HtmlEncode(GridviewAccepted.Rows.Count.ToString());
            }
            catch (NpgsqlException ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "LoadAccepted()" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
                RecordUserAction("LoadAccepted", "Exception while loading Accepted gridview Data", "F","NA",1);               
            }
            finally
            {
                conn.Close();
            }
        }

        protected void LoadRejected()
        {
            NpgsqlConnection conn = new NpgsqlConnection();
            NpgsqlCommand cmd = new NpgsqlCommand();
            conn.ConnectionString = ConfigurationManager.ConnectionStrings["connect"].ConnectionString;
            cmd.Connection = conn;
            try
            {
                string query = "";
                query = "select applicant_details.app_id,applicant_details.applicant_name,mst_memberdesignation.\"DesignationName\",applicant_details.new_or_renewal,";
                query = query + " society.socname,mst_societytype.societytype,esociety.status_table.status_id,status_table.application_submission_time from esociety.applicant_details";
                query = query + " INNER JOIN esociety.society on applicant_details.app_id = society.app_id";
                query = query + " INNER JOIN esociety.mst_memberdesignation on applicant_details.applicant_designation = mst_memberdesignation.\"DesignationID\"";
                query = query + " INNER JOIN esociety.mst_societytype on mst_societytype.societyid = society.soctype";
                query = query + " INNER JOIN esociety.status_table on esociety.status_table.app_id = esociety.applicant_details.app_id";
                query = query + " INNER JOIN esociety.mst_district on esociety.mst_district.\"DistrictID\" =esociety.society.socdistrict";
                query = query + " where esociety.status_table.status_id = 9 and society.socdistrict= @district order by status_table.application_submission_time ASC";
                conn.Open();
                cmd.CommandText = query;
                cmd.Parameters.AddWithValue("@district", Convert.ToInt32(Session["DistrictID"].ToString()));
                NpgsqlDataReader dr = cmd.ExecuteReader();
                gridviewRejected.DataSource = dr;
                gridviewRejected.DataBind();
                dr.Close();
                Label4.Text = Server.HtmlEncode(gridviewRejected.Rows.Count.ToString());
            }
            catch (NpgsqlException ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "LoadRejected()" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
                RecordUserAction("LoadRejected", "Exception while loading Rejected gridview Data", "F", "NA", 1);
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
        protected void ENameLinkBtn_Click(object sender, EventArgs e)
        {
            try
            {
                LinkButton btn = (LinkButton)sender;
                GridViewRow row = (GridViewRow)btn.NamingContainer;
                string app_id = ((Label)GridviewAccepted.Rows[row.RowIndex].FindControl("LbApp_id")).Text;
                Session["app_id"] = app_id;
                if (app_id == "" || app_id == null)
                {
                    RecordUserAction("Read", "Application ID is Null", "Failed", app_id, 1);
                }
                else
                {
                    RecordUserAction("Read", "View Detials of Accepted Society", "Success", app_id, 1);
                    Response.Redirect("VerifySociety.aspx");
                }
            }
            catch (Exception ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "ENameLinkBtn_Click" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
            }
        }

        protected void LinkButton1_Click(object sender, EventArgs e)
        {
            try
            {
                LinkButton btn = (LinkButton)sender;
                GridViewRow row = (GridViewRow)btn.NamingContainer;
                string app_id = ((Label)gridviewRejected.Rows[row.RowIndex].FindControl("Label2")).Text;
                Session["app_id"] = app_id;
                if (app_id == "" || app_id == null)
                {
                    RecordUserAction("Read", "Application ID is Null", "Failed", app_id, 1);
                }
                else
                {
                    RecordUserAction("Read", "View Detials of Rejected Society", "Success", app_id, 1);
                    Response.Redirect("VerifySociety.aspx");
                }
            }
            catch (Exception ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "LinkButton1_Click" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
            }
        }

        protected void permission_Click(object sender, EventArgs e)
        {
            Response.Redirect("Dashboard.aspx");
        }

        protected void gridviewRejected_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType.Equals(DataControlRowType.DataRow))
                {
                    if (e.Row.Cells[6].Text.Contains("1"))
                    {
                        string str = e.Row.Cells[4].Text;
                        str = "New";
                        e.Row.Cells[6].Text = str;

                    }
                    else if (e.Row.Cells[6].Text.Contains("2"))
                    {
                        string str = e.Row.Cells[4].Text;
                        str = "Renewed";
                        e.Row.Cells[6].Text = str;

                    }
                    else
                    {
                        string str = e.Row.Cells[6].Text;
                        str = "NA";
                        e.Row.Cells[6].Text = str;
                    }


                }
            }
            catch (Exception ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "gridviewRejected_RowDataBound" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath) + " from " + ipaddress);
            }
        }

        protected void GridviewAccepted_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType.Equals(DataControlRowType.DataRow))
                {
                    if (e.Row.Cells[6].Text.Contains("1"))
                    {
                        string str = e.Row.Cells[4].Text;
                        str = "New";
                        e.Row.Cells[6].Text = str;

                    }
                    else if (e.Row.Cells[6].Text.Contains("2"))
                    {
                        string str = e.Row.Cells[4].Text;
                        str = "Renewed";
                        e.Row.Cells[6].Text = str;

                    }
                    else
                    {
                        string str = e.Row.Cells[6].Text;
                        str = "NA";
                        e.Row.Cells[6].Text = str;
                    }                                  


                }
            }
            catch (Exception ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "GridviewAccepted_RowDataBound" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath) + " from " + ipaddress);
            }
        }
    }
}