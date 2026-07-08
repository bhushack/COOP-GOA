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
    public partial class Search : System.Web.UI.Page
    {
        string ipaddress = Utility.getIP();
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
                if (!IsPostBack)
                {
                    Utility.FillDistrictSoc(ddldistrict);
                }
            }
            
        }

        private bool checkroles()
        {
            bool role_hacked = false;
            List<int> AllowedRoles = new List<int> { 1, 3, 4 };
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



        //public void FillDistrictSoc()
        //{
        //    NpgsqlConnection connect = new NpgsqlConnection();
        //    NpgsqlCommand cmd = new NpgsqlCommand();
        //    connect.ConnectionString = ConfigurationManager.ConnectionStrings["connect"].ConnectionString;
        //    cmd.Connection = connect;
        //    NpgsqlDataAdapter adapter;
        //    DataSet ds;
        //    try
        //    {
        //        connect.Open();
        //        string query = "SELECT \"DistrictName\", \"DistrictID\" FROM esociety.mst_district where \"DistrictID\" != 3";
        //        cmd.CommandText = query;
        //        adapter = new NpgsqlDataAdapter(cmd);
        //        ds = new DataSet();
        //        adapter.Fill(ds, "mst_district");
        //        ddldistrict.DataSource = ds.Tables[0];
        //        ddldistrict.DataTextField = "DistrictName";
        //        ddldistrict.DataValueField = "DistrictID";
        //        ddldistrict.DataBind();
        //        ddldistrict.Items.Insert(0, new ListItem("-- Select --", "-1"));
        //        cmd.Dispose();
        //        adapter.Dispose();
        //        ds = null;
        //    }
        //    catch (NpgsqlException ex)
        //    {
        //        CreateLogFiles Err = new CreateLogFiles();
        //        Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "FillDistrictSoc()" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
        //        //RecordUserAction("FillDistrictSoc", ex.Message, "F");
        //        Response.Write("<script language='javascript'>alert('District DropDown:" + "District dropdown not accessible." + "')</script>");//CHanGE THIS
        //    }
        //    finally
        //    {
        //        connect.Close();
        //    }
        //}
        protected void callErrorModal(string msg)
        {
            Label4.Text = Server.HtmlEncode(msg);
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "none", "<script> $(window).on('load', function () { $('#myerrorModal').modal({ backdrop: 'static' });});</script>", false);
        }
        protected void btnSearch_Click(object sender, EventArgs e)
        {
            if ((TxtBxLoginID.Text == null || TxtBxLoginID.Text == "") && (TxtBxAppID.Text == null || TxtBxAppID.Text == "") && (SocietyName.Text == null || SocietyName.Text == "") && ddldistrict.SelectedValue == "-1" && (TxtBxRegID.Text == null || TxtBxRegID.Text == ""))
            {
                callErrorModal("Search Criteria is not there");
            }
            else
            {
                int tokenid = 0, app_id = 0, societyname = 0, district = 0, reg_id = 0;
                try
                {
                    if (TxtBxLoginID.Text == "" || TxtBxLoginID.Text == null)
                    {
                        tokenid = 0;
                    }
                    else
                    {
                        tokenid = 1;
                    }
                    if (TxtBxAppID.Text == "" || TxtBxAppID.Text == null)
                    {
                        app_id = 0;
                    }
                    else
                    {
                        app_id = 1;
                    }
                    if (SocietyName.Text == "" || SocietyName.Text == null)
                    {
                        societyname = 0;
                    }
                    else
                    {
                        societyname = 1;
                    }
                    if (Convert.ToInt16(ddldistrict.SelectedValue) != -1)
                    {
                        district = 1;
                    }
                    else
                    {
                        district = 0; 
                    }
                    if (TxtBxRegID.Text == "" || TxtBxRegID.Text == null)
                    {
                        reg_id = 0;
                    }
                    else
                    {
                        reg_id = 1;
                    }
                    search(tokenid, app_id, societyname, district, reg_id);
                }
                catch (Exception ex)
                {
                    CreateLogFiles Err = new CreateLogFiles();
                    Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "btnSearch_Click" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath) + " from " + ipaddress);

                }
            }
        }
        protected void search(int token,int app_id,int societyname,int district, int reg_id)
        {
            NpgsqlConnection conn = new NpgsqlConnection();
            NpgsqlCommand cmd = new NpgsqlCommand();
            conn.ConnectionString = ConfigurationManager.ConnectionStrings["connect"].ConnectionString;
            cmd.Connection = conn;
            try
            {
                conn.Open();
                string query = "select esociety.applicant_details.login_id,esociety.applicant_details.app_id,esociety.society.socname,";
                query = query + " esociety.mst_district.\"DistrictName\",esociety.mst_societytype.societytype, esociety.mst_status.status_name as societystatus from esociety.applicant_details,";
                query = query + " esociety.society,esociety.mst_societytype,esociety.mst_district,esociety.mst_status,esociety.status_table where esociety.applicant_details.app_id = esociety.society.app_id";
                query = query + " and esociety.society.soctype = esociety.mst_societytype.societyid and esociety.mst_status.status_id = esociety.status_table.status_id and esociety.status_table.app_id = esociety.applicant_details.app_id";
                query = query + " and esociety.society.socdistrict = esociety.mst_district.\"DistrictID\" and status_table.status_id >=3";
                
                if (token == 1)
                {
                    query = query + " and esociety.applicant_details.login_id = @loginid";
                }
                if (app_id == 1)
                {
                    query = query + " and esociety.applicant_details.app_id = @app_id";
                }
                if (societyname == 1)
                {
                    query = query + " and (UPPER(esociety.society.socname)) like @socname";
                }
                if (district == 1)
                {
                    query = query + " and esociety.society.socdistrict = @district";
                }
                if(reg_id == 1)
                {
                    query = query + " and esociety.society.socregid = @socregid";
                }
                cmd.CommandText = query;
                if (token == 1)
                {
                    cmd.Parameters.AddWithValue("@loginid", TxtBxLoginID.Text.Trim());
                }
                if (app_id == 1)
                {
                    cmd.Parameters.AddWithValue("@app_id",Convert.ToInt64(TxtBxAppID.Text.Trim()));
                }
                if (societyname == 1)
                {
                    cmd.Parameters.AddWithValue("@socname", "%" + SocietyName.Text.ToUpper().Trim() + "%");
                }
                if (district == 1)
                {
                    cmd.Parameters.AddWithValue("@district", Convert.ToInt32(ddldistrict.SelectedValue));
                }
                if (reg_id == 1)
                {
                    cmd.Parameters.AddWithValue("@socregid", TxtBxRegID.Text.ToUpper().Trim());
                }
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "none", "<script> $(window).on('load', function () { $('#searchModal').modal({ backdrop: 'static' });});</script>", false);
                using (NpgsqlDataAdapter dr = new NpgsqlDataAdapter(cmd))
                {
                    DataTable dt = new DataTable();
                    dr.Fill(dt);
                    gdSearch.DataSource = dt;
                    gdSearch.DataBind();
                }
            }
            catch (NpgsqlException ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "search()" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath) + " from " + ipaddress);

            }
            finally
            {
                conn.Close();
            }
        }

        protected void LbView_Click(object sender, EventArgs e)
        {
            try
            {
                //RecordUserAction("Read", "LbViewverification_Click  Button Clicked", "Success", "NA", 1);
                LinkButton btn = (LinkButton)sender;
                GridViewRow row = (GridViewRow)btn.NamingContainer;
                string app_id = ((Label)gdSearch.Rows[row.RowIndex].FindControl("LbSearchApp_id")).Text;
                Session["app_id"] = app_id;
                if (app_id == "" || app_id == null)
                {
                    //RecordUserAction("Read", "LbViewverification_Click - Application ID is Blank", "Failed", app_id, 2);
                }
                else
                {
                    // RecordUserAction("Read", "LbViewverification_Click - Redirect to View Applicant", "Success", app_id, 1);
                    Response.Redirect("VerifySociety.aspx");
                }
            }
            catch (Exception ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "LbView_Click" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath) + " from " + ipaddress);
            }
        }
    }
    
}