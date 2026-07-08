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
    public partial class SocietyAmendment : System.Web.UI.Page
    {
        string ipaddress = Utility.getIP();
        string macaddress = Utility.GetMACAddress(Utility.getIP());       
        Insert ins = new Insert();
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
                HttpContext.Current.Response.AddHeader("Cache-Control", "no-cache, no-store, must-revalidate");
                HttpContext.Current.Response.AddHeader("Pragma", "no-cache");
                HttpContext.Current.Response.AddHeader("Expires", "0");
                loadamendapplication();
                loadamendapplication_observation();
                loadaceeptedamendment();
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

        public void loadamendapplication()
        {

            NpgsqlConnection conn = new NpgsqlConnection();
            NpgsqlCommand cmd = new NpgsqlCommand();
            conn.ConnectionString = ConfigurationManager.ConnectionStrings["connect"].ConnectionString;
            cmd.Connection = conn;
            try
            {
                conn.Open();
                string query = "SELECT status_amendment.app_id, society.socname, society.socregid, status_amendment.amend_submittime from esociety.status_amendment inner join esociety.society";
                query = query + " on esociety.status_amendment.app_id= esociety.society.app_id where amend_status=2 and society.socdistrict=@socdistrict order by status_amendment.amend_submittime ASC";
                cmd.CommandText = query;
                cmd.Parameters.AddWithValue("@socdistrict", Convert.ToInt32(Session["DistrictID"].ToString()));
                NpgsqlDataAdapter da = new NpgsqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                da.Fill(ds);
                gvamend.DataSource = ds;
                gvamend.DataBind();

                da.Dispose();

                Label1.Text = Server.HtmlEncode(gvamend.Rows.Count.ToString());
            }
            catch (NpgsqlException ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "bindgridview()" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath) + " from " + ipaddress);

                Response.Write("<script language='javascript'>alert('" + "Exception in Gridview" + "')</script>");
            }
            finally
            {
                conn.Close();
            }
        }

        public void loadamendapplication_observation()
        {

            NpgsqlConnection conn = new NpgsqlConnection();
            NpgsqlCommand cmd = new NpgsqlCommand();
            conn.ConnectionString = ConfigurationManager.ConnectionStrings["connect"].ConnectionString;
            cmd.Connection = conn;
            try
            {
                conn.Open();
                string query = "SELECT status_amendment.app_id, society.socname, society.socregid, society.regdate, status_amendment.amend_obssubmittime from esociety.status_amendment inner join esociety.society";
                query = query + " on esociety.status_amendment.app_id= esociety.society.app_id where amend_status=4 and society.socdistrict=@socdistrict order by status_amendment.amend_obssubmittime ASC";
                cmd.CommandText = query;
                cmd.Parameters.AddWithValue("@socdistrict", Convert.ToInt32(Session["DistrictID"].ToString()));
                NpgsqlDataAdapter da = new NpgsqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                da.Fill(ds);
                gvamendobservation.DataSource = ds;
                gvamendobservation.DataBind();

                da.Dispose();

                Label2.Text = Server.HtmlEncode(gvamendobservation.Rows.Count.ToString());
            }
            catch (NpgsqlException ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "bindgridview()" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath) + " from " + ipaddress);

                Response.Write("<script language='javascript'>alert('" + "Exception in Gridview" + "')</script>");
            }
            finally
            {
                conn.Close();
            }
        }

        public void loadaceeptedamendment()
        {

            NpgsqlConnection conn = new NpgsqlConnection();
            NpgsqlCommand cmd = new NpgsqlCommand();
            conn.ConnectionString = ConfigurationManager.ConnectionStrings["connect"].ConnectionString;
            cmd.Connection = conn;
            try
            {
                conn.Open();
                string query = "SELECT status_amendment.app_id,society.socname,society.socregid,society.regdate from esociety.status_amendment inner join esociety.society";
                query = query + " on esociety.status_amendment.app_id= esociety.society.app_id where amend_status=5 and society.socdistrict=@socdistrict";
                cmd.CommandText = query;
                cmd.Parameters.AddWithValue("@socdistrict", Convert.ToInt32(Session["DistrictID"].ToString()));
                NpgsqlDataAdapter da = new NpgsqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                da.Fill(ds);
                gvamendaccepted.DataSource = ds;
                gvamendaccepted.DataBind();

                da.Dispose();

                Label3.Text = Server.HtmlEncode(gvamendaccepted.Rows.Count.ToString());
            }
            catch (NpgsqlException ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "bindgridview()" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath) + " from " + ipaddress);

                Response.Write("<script language='javascript'>alert('" + "Exception in Gridview" + "')</script>");
            }
            finally
            {
                conn.Close();
            }
        }

        protected void gvamend_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            //try
            //{
            //    string amendstatus= ((Label)e.Row.FindControl("LbAmendStatus")).Text;
            //    string submittime = ((Label)e.Row.FindControl("LbAmendSubmittime")).Text;

            //    if (amendstatus == "2")
            //    {
            //        e.Row.Cells[6].Text = submittime;
            //    }

            //    //if (e.Row.RowType == DataControlRowType.DataRow)
            //    //{
            //    //    if (e.Row.Cells[6].Text.Equals("Service (Govt. Employee)"))
            //    //    {
            //    //        e.Row.BackColor = Color.Cyan;
            //    //    }
            //    //}
            //}
            //catch (Exception ex)
            //{
            //    CreateLogFiles Err = new CreateLogFiles();
            //    Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "gvamend_RowDataBound" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
            //}
        }

        protected void lkviewapplicant_Click(object sender, EventArgs e)
        {
            try
            {
                LinkButton btn = (LinkButton)sender;
                GridViewRow row = (GridViewRow)btn.NamingContainer;
                
                string app_id = ((Label)gvamend.Rows[row.RowIndex].FindControl("LbApp_id")).Text;
                Session["app_id"] = app_id;
                if (app_id == "" || app_id == null)
                {
                    RecordUserAction("Read", "Application ID is Null", "Failed", app_id, 2);
                }
                else
                { 
                        RecordUserAction("Read", "Amendment open for Verification", "Success", app_id, 1);
                        Response.Redirect("VerifyAmendment.aspx");                    
                }
            }
            catch (Exception ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "lkviewapplicant_Click" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
            }
        }

        protected void lkviewobsapplicant_Click(object sender, EventArgs e)
        {
            try
            {
                LinkButton btn = (LinkButton)sender;
                GridViewRow row = (GridViewRow)btn.NamingContainer;

                string app_id = ((Label)gvamendobservation.Rows[row.RowIndex].FindControl("LbApp_id")).Text;
                Session["app_id"] = app_id;
                if (app_id == "" || app_id == null)
                {
                    RecordUserAction("Read", "Application ID is Null", "Failed", app_id, 2);
                }
                else
                {
                    RecordUserAction("Read", "Amendment open for Verification", "Success", app_id, 1);
                    Response.Redirect("VerifyAmendment.aspx");
                }
            }
            catch (Exception ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "lkviewapplicant_Click" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
            }
        }

        protected void lkviewacceptedapplicant_Click(object sender, EventArgs e)
        {
            try
            {
                LinkButton btn = (LinkButton)sender;
                GridViewRow row = (GridViewRow)btn.NamingContainer;

                string app_id = ((Label)gvamendaccepted.Rows[row.RowIndex].FindControl("LbApp_id")).Text;
                Session["app_id"] = app_id;
                if (app_id == "" || app_id == null)
                {
                    RecordUserAction("Read", "Application ID is Null", "Failed", app_id, 2);
                }
                else
                {
                    RecordUserAction("Read", "Amendment open for Verification", "Success", app_id, 1);
                    Response.Redirect("VerifyAmendment.aspx");
                }
            }
            catch (Exception ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "lkviewapplicant_Click" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
            }
        }
    }
}