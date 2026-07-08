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
    public partial class ApproveSociety : System.Web.UI.Page
    {
        string ipaddress = Utility.getIP();
        Insert ins = new Insert();
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Check4Tampering() || Page.RouteData.RouteHandler == null)
            {
                // RecordUserAction("Page_Load", "Access request failed. Tampered session", "F");
                SessionManage session = new SessionManage();
                session.__AbandonAdmin(Request, Response);
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
                        societyApproval();
                        societyApprovalObs();
                        societyApproval_renewal();
                        societyApprovalObs_renewal();
                    }
                }
                else
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('As per new Security Guidelines, you are requested to reset the password');window.location ='ChangePassword.aspx';", true);
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
        protected void societyApprovalObs()
        {
            NpgsqlConnection conn = new NpgsqlConnection();
            NpgsqlCommand cmd = new NpgsqlCommand();
            conn.ConnectionString = ConfigurationManager.ConnectionStrings["connect"].ConnectionString;
            cmd.Connection = conn;
            try
            {
                string query = "";
                if (Convert.ToInt32(Session["ROLE"]) == 3)
                {
                    query = "select applicant_details.app_id,applicant_details.applicant_name,mst_memberdesignation.\"DesignationName\",";
                    query = query + " society.socname,mst_societytype.societytype,esociety.status_table.status_id,status_table.application_submission_time from esociety.applicant_details";
                    query = query + " INNER JOIN esociety.society on applicant_details.app_id = society.app_id";
                    query = query + " INNER JOIN esociety.mst_memberdesignation on applicant_details.applicant_designation = mst_memberdesignation.\"DesignationID\"";
                    query = query + " INNER JOIN esociety.mst_societytype on mst_societytype.societyid = society.soctype";
                    query = query + " INNER JOIN esociety.status_table on esociety.status_table.app_id = esociety.applicant_details.app_id";
                    query = query + " INNER JOIN esociety.mst_district on esociety.mst_district.\"DistrictID\" =esociety.society.socdistrict";
                    query = query + " where (esociety.status_table.status_id = 7 or  esociety.status_table.status_id = 6) and applicant_details.new_or_renewal =1 and society.socdistrict=@district order by status_table.application_submission_time ASC";
                }
                else if (Convert.ToInt32(Session["ROLE"]) == 4)
                {
                    query = "select applicant_details.app_id,applicant_details.applicant_name,mst_memberdesignation.\"DesignationName\",";
                    query = query + " society.socname,mst_societytype.societytype,esociety.status_table.status_id,status_table.application_submission_time from esociety.applicant_details";
                    query = query + " INNER JOIN esociety.society on applicant_details.app_id = society.app_id";
                    query = query + " INNER JOIN esociety.mst_memberdesignation on applicant_details.applicant_designation = mst_memberdesignation.\"DesignationID\"";
                    query = query + " INNER JOIN esociety.mst_societytype on mst_societytype.societyid = society.soctype";
                    query = query + " INNER JOIN esociety.status_table on esociety.status_table.app_id = esociety.applicant_details.app_id";
                    query = query + " INNER JOIN esociety.mst_district on esociety.mst_district.\"DistrictID\" =esociety.society.socdistrict";
                    query = query + " where esociety.status_table.status_id = 6  and applicant_details.new_or_renewal =1 and society.socdistrict=@district order by status_table.application_submission_time ASC";

                }
                conn.Open();
                cmd.CommandText = query;
                cmd.Parameters.AddWithValue("@district", Convert.ToInt32(Session["DistrictID"].ToString()));
                NpgsqlDataReader dr = cmd.ExecuteReader();
                grvobservation_society.DataSource = dr;
                grvobservation_society.DataBind();
                Lbcount.Text = grvobservation_society.Rows.Count.ToString();
                dr.Close();
            }
            catch (NpgsqlException ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "societyApprovalObs()"+" " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
                //RecordUserAction("MemberDetails_Page_Load", "Exception at Member Data", "F");
                Response.Write("<script language='javascript'>alert('" + "Exception at Member Data" + "')</script>");
            }
            finally
            {
                conn.Close();
            }
        }
        protected void societyApproval()
        {
            NpgsqlConnection conn = new NpgsqlConnection();
            NpgsqlCommand cmd = new NpgsqlCommand();
            conn.ConnectionString = ConfigurationManager.ConnectionStrings["connect"].ConnectionString;
            cmd.Connection = conn;
            try
            {
                string query = "";
                if (Convert.ToInt32(Session["ROLE"]) == 3)
                {
                    query = "select applicant_details.app_id,applicant_details.applicant_name,mst_memberdesignation.\"DesignationName\",";
                    query = query + " society.socname,mst_societytype.societytype,esociety.status_table.status_id,status_table.application_submission_time from esociety.applicant_details";
                    query = query + " INNER JOIN esociety.society on applicant_details.app_id = society.app_id";
                    query = query + " INNER JOIN esociety.mst_memberdesignation on applicant_details.applicant_designation = mst_memberdesignation.\"DesignationID\"";
                    query = query + " INNER JOIN esociety.mst_societytype on mst_societytype.societyid = society.soctype";
                    query = query + " INNER JOIN esociety.status_table on esociety.status_table.app_id = esociety.applicant_details.app_id";
                    query = query + " INNER JOIN esociety.mst_district on esociety.mst_district.\"DistrictID\" =esociety.society.socdistrict";
                    query = query + " where (esociety.status_table.status_id = 3 or esociety.status_table.status_id = 4) and applicant_details.new_or_renewal =1 and society.socdistrict=@district order by status_table.application_submission_time ASC";
                }
                else if (Convert.ToInt32(Session["ROLE"]) == 4)
                {
                    query = "select applicant_details.app_id,applicant_details.applicant_name,mst_memberdesignation.\"DesignationName\",";
                    query = query + " society.socname,mst_societytype.societytype,esociety.status_table.status_id,status_table.application_submission_time from esociety.applicant_details";
                    query = query + " INNER JOIN esociety.society on applicant_details.app_id = society.app_id";
                    query = query + " INNER JOIN esociety.mst_memberdesignation on applicant_details.applicant_designation = mst_memberdesignation.\"DesignationID\"";
                    query = query + " INNER JOIN esociety.mst_societytype on mst_societytype.societyid = society.soctype";
                    query = query + " INNER JOIN esociety.status_table on esociety.status_table.app_id = esociety.applicant_details.app_id";
                    query = query + " INNER JOIN esociety.mst_district on esociety.mst_district.\"DistrictID\" = esociety.society.socdistrict";
                    query = query + " where esociety.status_table.status_id = 3 and applicant_details.new_or_renewal =1  and society.socdistrict=@district order by status_table.application_submission_time ASC";

                }
                conn.Open();
                cmd.CommandText = query;
                cmd.Parameters.AddWithValue("@district", Convert.ToInt32(Session["DistrictID"].ToString()));
                NpgsqlDataReader dr = cmd.ExecuteReader();
                grvApplicantDetails.DataSource = dr;
                grvApplicantDetails.DataBind();
                Label3.Text = grvApplicantDetails.Rows.Count.ToString();
                dr.Close();
            }
            catch (NpgsqlException ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "societyApproval()"+ " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
                RecordUserAction("Read", " Read Society comes for Approval", "F","NA",1);
                Response.Write("<script language='javascript'>alert('" + "Exception at Member Data" + "')</script>");
            }
            finally
            {
                conn.Close();
            }
        }
        protected void societyApprovalObs_renewal()   // renewal applications submitted after observation
        {
            NpgsqlConnection conn = new NpgsqlConnection();
            NpgsqlCommand cmd = new NpgsqlCommand();
            conn.ConnectionString = ConfigurationManager.ConnectionStrings["connect"].ConnectionString;
            cmd.Connection = conn;
            try
            {
                string query = "";
                if (Convert.ToInt32(Session["ROLE"]) == 3)
                {
                    query = "select applicant_details.app_id,applicant_details.applicant_name,mst_memberdesignation.\"DesignationName\",";
                    query = query + " society.socname,mst_societytype.societytype,esociety.status_table.status_id,status_table.application_submission_time from esociety.applicant_details";
                    query = query + " INNER JOIN esociety.society on applicant_details.app_id = society.app_id";
                    query = query + " INNER JOIN esociety.mst_memberdesignation on applicant_details.applicant_designation = mst_memberdesignation.\"DesignationID\"";
                    query = query + " INNER JOIN esociety.mst_societytype on mst_societytype.societyid = society.soctype";
                    query = query + " INNER JOIN esociety.status_table on esociety.status_table.app_id = esociety.applicant_details.app_id";
                    query = query + " INNER JOIN esociety.mst_district on esociety.mst_district.\"DistrictID\" =esociety.society.socdistrict";
                    query = query + " where (esociety.status_table.status_id = 7 or  esociety.status_table.status_id = 6) and applicant_details.new_or_renewal =2 and society.socdistrict=@district order by status_table.application_submission_time ASC";
                }
                else if (Convert.ToInt32(Session["ROLE"]) == 4)
                {
                    query = "select applicant_details.app_id,applicant_details.applicant_name,mst_memberdesignation.\"DesignationName\",";
                    query = query + " society.socname,mst_societytype.societytype,esociety.status_table.status_id,status_table.application_submission_time from esociety.applicant_details";
                    query = query + " INNER JOIN esociety.society on applicant_details.app_id = society.app_id";
                    query = query + " INNER JOIN esociety.mst_memberdesignation on applicant_details.applicant_designation = mst_memberdesignation.\"DesignationID\"";
                    query = query + " INNER JOIN esociety.mst_societytype on mst_societytype.societyid = society.soctype";
                    query = query + " INNER JOIN esociety.status_table on esociety.status_table.app_id = esociety.applicant_details.app_id";
                    query = query + " INNER JOIN esociety.mst_district on esociety.mst_district.\"DistrictID\" =esociety.society.socdistrict";
                    query = query + " where esociety.status_table.status_id = 6  and applicant_details.new_or_renewal =2 and society.socdistrict=@district order by status_table.application_submission_time ASC";

                }
                conn.Open();
                cmd.CommandText = query;
                cmd.Parameters.AddWithValue("@district", Convert.ToInt32(Session["DistrictID"].ToString()));
                NpgsqlDataReader dr = cmd.ExecuteReader();
                grvrenewal_obs.DataSource = dr;
                grvrenewal_obs.DataBind();
                lbobsrenew_count.Text = grvrenewal_obs.Rows.Count.ToString();
                dr.Close();
            }
            catch (NpgsqlException ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "societyApprovalObs_renewal()" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
                //RecordUserAction("MemberDetails_Page_Load", "Exception at Member Data", "F");
                Response.Write("<script language='javascript'>alert('" + "Exception at Member Data" + "')</script>");
            }
            finally
            {
                conn.Close();
            }
        }
        protected void societyApproval_renewal()
        {
            NpgsqlConnection conn = new NpgsqlConnection();
            NpgsqlCommand cmd = new NpgsqlCommand();
            conn.ConnectionString = ConfigurationManager.ConnectionStrings["connect"].ConnectionString;
            cmd.Connection = conn;
            try
            {
                string query = "";
                if (Convert.ToInt32(Session["ROLE"]) == 3)
                {
                    query = "select applicant_details.app_id,applicant_details.applicant_name,mst_memberdesignation.\"DesignationName\",";
                    query = query + " society.socname,mst_societytype.societytype,esociety.status_table.status_id,status_table.application_submission_time from esociety.applicant_details";
                    query = query + " INNER JOIN esociety.society on applicant_details.app_id = society.app_id";
                    query = query + " INNER JOIN esociety.mst_memberdesignation on applicant_details.applicant_designation = mst_memberdesignation.\"DesignationID\"";
                    query = query + " INNER JOIN esociety.mst_societytype on mst_societytype.societyid = society.soctype";
                    query = query + " INNER JOIN esociety.status_table on esociety.status_table.app_id = esociety.applicant_details.app_id";
                    query = query + " INNER JOIN esociety.mst_district on esociety.mst_district.\"DistrictID\" =esociety.society.socdistrict";
                    query = query + " where (esociety.status_table.status_id = 3 or esociety.status_table.status_id = 4) and applicant_details.new_or_renewal =2 and society.socdistrict=@district order by status_table.application_submission_time ASC";
                }
                else if (Convert.ToInt32(Session["ROLE"]) == 4)
                {
                    query = "select applicant_details.app_id,applicant_details.applicant_name,mst_memberdesignation.\"DesignationName\",";
                    query = query + " society.socname,mst_societytype.societytype,esociety.status_table.status_id,status_table.application_submission_time from esociety.applicant_details";
                    query = query + " INNER JOIN esociety.society on applicant_details.app_id = society.app_id";
                    query = query + " INNER JOIN esociety.mst_memberdesignation on applicant_details.applicant_designation = mst_memberdesignation.\"DesignationID\"";
                    query = query + " INNER JOIN esociety.mst_societytype on mst_societytype.societyid = society.soctype";
                    query = query + " INNER JOIN esociety.status_table on esociety.status_table.app_id = esociety.applicant_details.app_id";
                    query = query + " INNER JOIN esociety.mst_district on esociety.mst_district.\"DistrictID\" = esociety.society.socdistrict";
                    query = query + " where esociety.status_table.status_id = 3 and applicant_details.new_or_renewal =2  and society.socdistrict=@district order by status_table.application_submission_time ASC";

                }
                conn.Open();
                cmd.CommandText = query;
                cmd.Parameters.AddWithValue("@district", Convert.ToInt32(Session["DistrictID"].ToString()));
                NpgsqlDataReader dr = cmd.ExecuteReader();
                grvRenewalApplications.DataSource = dr;
                grvRenewalApplications.DataBind();
                Lbrenew_count.Text = grvRenewalApplications.Rows.Count.ToString();
                dr.Close();
            }
            catch (NpgsqlException ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "societyApproval()" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
                RecordUserAction("Read", " Read Society comes for Approval", "F", "NA", 1);
                Response.Write("<script language='javascript'>alert('" + "Exception at Member Data" + "')</script>");
            }
            finally
            {
                conn.Close();
            }
        } // renewal applications submitted first time
        
        protected void ENameLinkBtn_Click(object sender, EventArgs e)
        {
            try
            {
                LinkButton btn = (LinkButton)sender;
                GridViewRow row = (GridViewRow)btn.NamingContainer;
                string index = ((Label)grvApplicantDetails.Rows[row.RowIndex].FindControl("lblRowNum")).Text;
                string app_id = ((Label)grvApplicantDetails.Rows[row.RowIndex].FindControl("LbApp_id")).Text;
                Session["app_id"] = app_id;
                if (app_id == "" || app_id == null)
                {
                    RecordUserAction("Verification_New", "Application ID is Null", "Failed", app_id, 2);
                }
                else
                {
                    //Response.Redirect("VerifySociety.aspx");
                    //uncomment last paragraph and uncomment upper line
                    if (Convert.ToInt32(index) == 1)
                    {
                        RecordUserAction("Verification_New", "Application open for Verfication", "Success", app_id, 1);
                        Response.Redirect("VerifySociety.aspx");
                     }
                    else
                    {
                        RecordUserAction("Verification_New", "Cicked Application not listed on Serail No 1", "Failed", app_id, 1);
                        Response.Write("<script language='javascript'>alert('" + "You can verify application only in Serial." + "')</script>");
                    }
                }
            }
            catch (Exception ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "ENameLinkBtn_Click" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
            }
        }

        protected void ENameLinkBtn_obs_Click(object sender, EventArgs e)
        {
            try
            {
                LinkButton btn = (LinkButton)sender;
                GridViewRow row = (GridViewRow)btn.NamingContainer;
                string index = ((Label)grvobservation_society.Rows[row.RowIndex].FindControl("lblRowNum_obs")).Text;
                string app_id = ((Label)grvobservation_society.Rows[row.RowIndex].FindControl("LbApp_id_obs")).Text;
                Session["app_id"] = app_id;
                if (app_id == "" || app_id == null)
                {
                    RecordUserAction("Verification_Obs", "Application ID is Null", "Failed", app_id, 2);
                }
                else
                {
                    // Response.Redirect("VerifySociety.aspx");
                    //uncomment last paragraph and uncomment upper line
                    if (Convert.ToInt32(index) == 1)
                    {
                        RecordUserAction("Verification_Obs", "Application open for Verfication", "Success", app_id, 1);
                        Response.Redirect("VerifySociety.aspx");
                    }
                    else
                    {
                        RecordUserAction("Verification_Obs", "Cicked Application not listed on Serail No 1", "Failed", app_id, 1);
                        Response.Write("<script language='javascript'>alert('" + "You can verify application only in Serial." + "')</script>");
                    }
                }
            }
            catch (Exception ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "ENameLinkBtn_obs_Click" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
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

        protected void ENameLinkBtnRenewal_Click(object sender, EventArgs e)
        {

            try
            {
                LinkButton btn = (LinkButton)sender;
                GridViewRow row = (GridViewRow)btn.NamingContainer;
                string index = ((Label)grvRenewalApplications.Rows[row.RowIndex].FindControl("lblRowNum")).Text;
                string app_id = ((Label)grvRenewalApplications.Rows[row.RowIndex].FindControl("LbApp_id")).Text;
                Session["app_id"] = app_id;
                if (app_id == "" || app_id == null)
                {
                    RecordUserAction("Verification_New", "Application ID is Null", "Failed", app_id, 2);
                }
                else
                {
                    //Response.Redirect("VerifySociety.aspx");
                    //uncomment last paragraph and uncomment upper line
                    if (Convert.ToInt32(index) == 1)
                    {
                        RecordUserAction("Verification_Renewal", "Application open for Verfication", "Success", app_id, 1);
                        Response.Redirect("VerifySociety.aspx");
                    }
                    else
                    {
                        RecordUserAction("Verification_Renewal", "Clicked Application not listed on Serail No 1", "Failed", app_id, 1);
                        Response.Write("<script language='javascript'>alert('" + "You can verify application only in Serial." + "')</script>");
                    }
                }
            }
            catch (Exception ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "ENameLinkBtn_ClENameLinkBtnRenewal_Clickick" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
            }
        }

        protected void ENameLinkBtnrenewal_obs_Click(object sender, EventArgs e)
        {
            try
            {
                LinkButton btn = (LinkButton)sender;
                GridViewRow row = (GridViewRow)btn.NamingContainer;
                string index = ((Label)grvrenewal_obs.Rows[row.RowIndex].FindControl("lblRowNum_obs")).Text;
                string app_id = ((Label)grvrenewal_obs.Rows[row.RowIndex].FindControl("LbApp_id_obs")).Text;
                Session["app_id"] = app_id;
                if (app_id == "" || app_id == null)
                {
                    RecordUserAction("Verification_Obs", "Application ID is Null", "Failed", app_id, 2);
                }
                else
                {
                    
                    if (Convert.ToInt32(index) == 1)
                    {
                        RecordUserAction("Verification_Obs_renewal", "Application open for Verfication", "Success", app_id, 1);
                        Response.Redirect("VerifySociety.aspx");
                    }
                    else
                    {
                        RecordUserAction("Verification_Obs_renewal", "Clicked Application not listed on Serail No 1", "Failed", app_id, 1);
                        Response.Write("<script language='javascript'>alert('" + "You can verify application only in Serial." + "')</script>");
                    }
                }
            }
            catch (Exception ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "ENameLinkBtnrenewal_obs_Click" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
            }
        }
    }
}