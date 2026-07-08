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
using WS_Encryption;

namespace GoaSocietyRegistration.Organization
{
    public partial class VerifyAmendment : System.Web.UI.Page
    {
        ByteEncryption.ByteEncryption obj_Byte_Encryption = new ByteEncryption.ByteEncryption();
        NICEncryption _encryption = new NICEncryption();
        Validate vs = new Validate();
        Insert ins = new Insert();
        string ipaddress = Utility.getIP();
        string macaddress = Utility.GetMACAddress(Utility.getIP());

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Check4Tampering() || Page.RouteData.RouteHandler == null)
            {
                RecordUserAction("Page_Load", "Access request failed. Tampered session", "F", "NA", 1);
                SessionManage session = new SessionManage();
                session.__AbandonAdmin(Request, Response);
            }
            else
            {
                HttpContext.Current.Response.AddHeader("Cache-Control", "no-cache, no-store, must-revalidate");
                HttpContext.Current.Response.AddHeader("Pragma", "no-cache");
                HttpContext.Current.Response.AddHeader("Expires", "0");
                Session["loadflag"] = "stop";
                Session["ObjectID"] = "aa";
                if (!IsPostBack)
                {
                    Session["flagAmenddocs"] = 1;

                }

                if (Session["SessionID"] != null || Session["firstname"] != null)
                {
                    if (Convert.ToInt32(Session["flagAmenddocs"].ToString()) == 1)
                    {
                        loaddocuments();
                    }


                }
                else
                {
                    Response.Redirect("~/LoginModule.aspx");
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

        public void loaddocuments()
        {
            int amendstatus = ins.getOtherServicesStatus(Session["app_id"].ToString());

            if(amendstatus == 2 || amendstatus == 4)
            {
                sendobservation.Visible = true;
                Amendaccepted.Visible = true;
            }

            Society_Details society = ins.FetchSociety(Session["app_id"].ToString(), 3, 3);

           
            value_society_name.Text = Sanitize.InputText(society.socname);
            value_society_address.Text = Sanitize.InputText(society.socaddr);
            value_registration_id.Text = Sanitize.InputText(society.socregid);
            value_registration_date.Text = Sanitize.InputText(society.regdate);

            getAdditionaldocs(Convert.ToInt64(Session["app_id"].ToString()));

        }

        protected void getAdditionaldocs(Int64 appid)
        {
            NpgsqlConnection conn = new NpgsqlConnection();
            NpgsqlCommand cmd = new NpgsqlCommand();
            conn.ConnectionString = ConfigurationManager.ConnectionStrings["connect"].ConnectionString;
            cmd.Connection = conn;
            try
            {
                conn.Open();
                string query = "SELECT object_id,docname from esociety.otherdoc_amendment where app_id=@appid";
                cmd.CommandText = query;
                cmd.Parameters.AddWithValue("@appid", appid);

                NpgsqlDataAdapter da1 = new NpgsqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                da1.Fill(ds);

                GridView_AddAmendDocs.DataSource = ds;
                GridView_AddAmendDocs.DataBind();
                
                da1.Dispose();
            }
            catch (NpgsqlException ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "getAdditionaldocs()" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath) + " from " + ipaddress);

                Response.Write("<script language='javascript'>alert('" + "Exception in Gridview" + "')</script>");
            }
            finally
            {
                conn.Close();
            }



           
        }


        public int getmyID(Int64 appid, int myid)
        {
            int exists = 0;
            NpgsqlConnection conn = new NpgsqlConnection();
            NpgsqlCommand cmd = new NpgsqlCommand();
            conn.ConnectionString = ConfigurationManager.ConnectionStrings["connect"].ConnectionString;
            cmd.Connection = conn;
            try
            {
                conn.Open();
                string query = "select object_id from esociety.docs_amendment where app_id=@appid and myid=@myid";
                cmd.CommandText = query;
                cmd.Parameters.AddWithValue("@appid", appid);
                cmd.Parameters.AddWithValue("@myid", myid);
                NpgsqlDataReader dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    string objectid = Server.HtmlEncode(dr["object_id"].ToString());
                    Session["ObjectID"] = objectid;
                    exists = 1;
                }
                else
                {
                    Session["loadflag"] = "stop";
                    exists = 0;
                }
                dr.Close();
            }
            catch (NpgsqlException ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "getmyID()" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
                Response.Write("<script language='javascript'>alert('" + "Not getting PDF ID" + "')</script>");
                exists = 0;
            }
            finally
            {

                conn.Close();
            }
            return exists;
        }

        protected void Amend_ImgBtnViewPdf1_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                if (Session["app_id"] != null)
                {
                    RecordUserAction("Read File", "View Amendment Pdf1 ", "Success", Session["app_id"].ToString(), 1);
                    getmyID(Convert.ToInt64(Session["app_id"].ToString()), 1);
                    Session["loadflag"] = "yes";
                    Session["myid"] = 1;
                    Session["collection"] = "Amendment Documents";
                    Session["docid"] = 2;
                }
                else
                {
                    Session["loadflag"] = "stop";
                }
            }
            catch (Exception ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "ImgBtnViewPdf2_Click" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
            }
        }

        protected void Amend_ImgBtnViewPdf3_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                if (Session["app_id"] != null)
                {
                    RecordUserAction("Read File", "View Amendment Pdf3 ", "Success", Session["app_id"].ToString(), 1);
                    getmyID(Convert.ToInt64(Session["app_id"].ToString()), 3);
                    Session["loadflag"] = "yes";
                    Session["myid"] = 3;
                    Session["collection"] = "Amendment Documents";
                    Session["docid"] = 2;
                }
                else
                {
                    Session["loadflag"] = "stop";
                }
            }
            catch (Exception ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "ImgBtnViewPdf2_Click" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
            }
        }

        protected void Amend_ImgBtnViewPdf2_Click(object sender, ImageClickEventArgs e)
        {

            try
            {
                if (Session["app_id"] != null)
                {
                    RecordUserAction("Read File", "View Amendment Pdf2 ", "Success", Session["app_id"].ToString(), 1);
                    getmyID(Convert.ToInt64(Session["app_id"].ToString()), 2);
                    Session["loadflag"] = "yes";
                    Session["myid"] = 2;
                    Session["collection"] = "Amendment Documents";
                    Session["docid"] = 2;
                }
                else
                {
                    Session["loadflag"] = "stop";
                }
            }
            catch (Exception ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "ImgBtnViewPdf2_Click" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
            }
        }

        protected void Amend_ImgBtnViewPdf4_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                if (Session["app_id"] != null)
                {
                    RecordUserAction("Read File", "View Amendment Pdf4 ", "Success", Session["app_id"].ToString(), 1);
                    getmyID(Convert.ToInt64(Session["app_id"].ToString()), 4);
                    Session["loadflag"] = "yes";
                    Session["myid"] = 4;
                    Session["collection"] = "Amendment Documents";
                    Session["docid"] = 2;
                }
                else
                {
                    Session["loadflag"] = "stop";
                }
            }
            catch (Exception ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "ImgBtnViewPdf2_Click" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
            }
        }

        protected void Amend_ImgBtnViewPdf5_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                if (Session["app_id"] != null)
                {
                    RecordUserAction("Read File", "View Amendment Pdf5 ", "Success", Session["app_id"].ToString(), 1);
                    getmyID(Convert.ToInt64(Session["app_id"].ToString()), 5);
                    Session["loadflag"] = "yes";
                    Session["myid"] = 5;
                    Session["collection"] = "Amendment Documents";
                    Session["docid"] = 2;
                }
                else
                {
                    Session["loadflag"] = "stop";
                }
            }
            catch (Exception ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "ImgBtnViewPdf2_Click" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
            }
        }

        protected void Amend_ImgBtnViewPdf6_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                if (Session["app_id"] != null)
                {
                    RecordUserAction("Read File", "View Amendment Pdf6 ", "Success", Session["app_id"].ToString(), 1);
                    getmyID(Convert.ToInt64(Session["app_id"].ToString()), 6);
                    Session["loadflag"] = "yes";
                    Session["myid"] = 6;
                    Session["collection"] = "Amendment Documents";
                    Session["docid"] = 2;
                }
                else
                {
                    Session["loadflag"] = "stop";
                }
            }
            catch (Exception ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "ImgBtnViewPdf2_Click" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
            }
        }

        protected void View_adddocs_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                ImageButton btn = (ImageButton)sender;
                GridViewRow row = (GridViewRow)btn.NamingContainer;
                HiddenField hdobid = row.FindControl("hfobjectID") as HiddenField;
                if (hdobid != null)
                {
                    string objectid = hdobid.Value;
                    if (objectid != null)
                    {
                        RecordUserAction("Read", "LbView_Click View pdf file from Additional Docs", "Success", Session["app_id"].ToString(), 2);
                        Session["ObjectID"] = objectid;
                        Session["loadflag"] = "yes";
                        Session["myid"] = 0;
                        Session["collection"] = "Amendment Documents";
                        Session["docid"] = 2;
                    }
                    else
                    {
                        Session["loadflag"] = "stop";
                    }
                }
            }
            catch (Exception ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "View_adddocs_Click" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
            }
        }

        protected void sendobservation_Click(object sender, EventArgs e)
        {
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "none", "<script> $(window).on('load', function () { $('#sendobs_confirmation_modal').modal({ backdrop: 'static' });});</script>", false);

        }

        protected void Amendaccepted_Click(object sender, EventArgs e)
        {
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "none", "<script> $(window).on('load', function () { $('#AmendAccepted').modal({ backdrop: 'static' });});</script>", false);

        }

        protected void dashboard_Click(object sender, EventArgs e)
        {
            try
            {
                RecordUserAction("Redirect", "dashboard_Click Redirect to Dashboard through navbar", "Success", Session["app_id"].ToString(), 2);
                Session["flagAmenddocs"] = 1;
                Response.Redirect("Dashboard.aspx");
            }
            catch (Exception ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "dashboard_Click" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
            }
        }

        protected void nav_documents_Click(object sender, EventArgs e)
        {
            try
            {
                RecordUserAction("Redirect", "nav_documents_Click Redirect to Documents through navbar", "Success", Session["app_id"].ToString(), 2);
                Session["flagAmenddocs"] = 0;               
              
                Documents.Visible = true;
                loaddocuments();
                //loadadditionaldocuments();
            }
            catch (Exception ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "nav_documents_Click" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
            }
        }
              

        protected void histobservation_Click(object sender, EventArgs e)
        {
            try
            {               
                if (Session["app_id"].ToString() != null || Session["app_id"].ToString() != "")
                {
                    loadGridView_historyobs(Session["app_id"].ToString());
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
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "histobservation_Click" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
            }

        }

        protected void view_applicant_logout_Click(object sender, EventArgs e)
        {
            try
            {
               
                int value = Utility.logout(Convert.ToInt64(Session["SessionID"]));
                if (value == 1)
                {
                    Session.Clear();
                    Session.Abandon();
                    Session.RemoveAll();
                    Response.Cookies.Clear();

                    Response.Redirect("~/OrganizationLogin.aspx");
                }
                else
                {
                    CreateLogFiles Err = new CreateLogFiles();
                    Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), "Failed", "view_applicant_logout_Click()" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
                   
                    Response.Write("<script language='javascript'>alert(' Error While logout....Try after sometime ')</script>");
                    Response.Redirect("~/Organization/Dashboard.aspx");
                }
            }
            catch (Exception ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "view_applicant_logout_Click" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
            }
        }

        protected void loadGridView_historyobs(string app_id)
        {
            if (Session["app_id"].ToString() != null || Session["app_id"].ToString() != "")
            {

                NpgsqlConnection conn = new NpgsqlConnection();
                NpgsqlCommand cmd = new NpgsqlCommand();
                conn.ConnectionString = ConfigurationManager.ConnectionStrings["connect"].ConnectionString;
                cmd.Connection = conn;
                try
                {
                    string query = "SELECT app_id, obsremarks_bydro,submittime_obsremarks FROM esociety.remarks_amendment";
                    query = query + " where app_id = @app_id order by submittime_obsremarks asc";
                    cmd.CommandText = query;
                    cmd.Parameters.AddWithValue("@app_id", Convert.ToInt64(Session["app_id"]));
                    using (NpgsqlDataAdapter dr = new NpgsqlDataAdapter(cmd))
                    {
                        DataTable dt = new DataTable();
                        dr.Fill(dt);
                        gvhistory.DataSource = dt;
                        gvhistory.DataBind();
                    }

                }
                catch (NpgsqlException ex)
                {
                    CreateLogFiles Err = new CreateLogFiles();
                    Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "loadGridView_historyobs()" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath) + " from " + ipaddress);

                    // RecordUserAction("Read", "Function Load", "Failed", "NA", 2);
                    Response.Write("<script language='javascript'>alert('" + "Error while loading...." + "')</script>");
                }
                finally
                {
                    conn.Close();
                }
            }
        }

        protected void send_obs_modal_confirm_button_Click(object sender, EventArgs e)
        {
            try
            {
                string remarks_mandatory = outputtextremarks.InnerText;

                int value = doValidation(remarks_mandatory.Trim());
                if (value == 1)
                {
                    if (Session["app_id"] != null)
                    {

                        int a = saveremarksbysro(1);
                        if (a == 1)
                        {
                            RecordUserAction("Update", "Amendment Application Sent for Observation", "Success", Session["app_id"].ToString(), 1);
                            Label65.ForeColor = System.Drawing.Color.Green;
                            Label65.Text = "Observation Send Successfully";
                            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "none", "<script> $(window).on('load', function () { $('#redirectModal').modal({ backdrop: 'static' });});</script>", false);

                        }
                        else
                        {
                            RecordUserAction("Update", "Ammendment Application not Accepted", "Failed", Session["app_id"].ToString(), 1);
                            Label69.ForeColor = System.Drawing.Color.Red;
                            Label69.Text = "Submission Failed..Try Again";
                            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "none", "<script> $(window).on('load', function () { $('#MyErrorModal').modal({ backdrop: 'static' });});</script>", false);

                        }
                    }
                    else
                    {

                        RecordUserAction("Update", "Send Observation Failed due to null session", "Failed", Session["app_id"].ToString(), 2);
                        Label69.ForeColor = System.Drawing.Color.Red;
                        Label69.Text = "Submission Failed..Try Again";
                        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "none", "<script> $(window).on('load', function () { $('#MyErrorModal').modal({ backdrop: 'static' });});</script>", false);
                    }



                }


               
            }
            catch (Exception ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "send_obs_modal_confirm_button_Click" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
            }
        }

        protected int doValidation(string str1)
        {
            int a = 0;
            try
            {
                if (str1 == null || str1 == "")
                {
                    a = 0;
                    Label69.ForeColor = System.Drawing.Color.Red;
                    Label69.Text = "Remarks is Blank!!!!";
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "none", "<script> $(window).on('load', function () { $('#MyErrorModal').modal({ backdrop: 'static' });});</script>", false);

                }
                else if (!vs.validateData(str1, vs.reamrks_validation))
                {
                    a = 0;
                    Label69.ForeColor = System.Drawing.Color.Red;
                    Label69.Text = "No Special Characters allowed in Remarks!!!";
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "none", "<script> $(window).on('load', function () { $('#MyErrorModal').modal({ backdrop: 'static' });});</script>", false);

                }
                else
                {
                    a = 1;
                }
            }
            catch (Exception ex)
            {
                a = 0;
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "doValidation()" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
            }
            return a;
        }

        protected void acceptapplicationmodalclick_Click(object sender, EventArgs e)
        {
            try
            {
                string remarks_mandatory = outputtextremarks.InnerText;                

                int value = doValidation(remarks_mandatory.Trim());
                if (value == 1)
                {

                    int a = saveremarksbysro(2);
                    if(a == 1)
                    {
                        RecordUserAction("Update", "Ammendment Application Accepted", "Success", Session["app_id"].ToString(), 1);
                        Label65.ForeColor = System.Drawing.Color.Green;
                        Label65.Text = "Application ID:" + Session["app_id"].ToString() + " is accepted for further processing.";
                        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "none", "<script> $(window).on('load', function () { $('#redirectModal').modal({ backdrop: 'static' });});</script>", false);

                    }
                    else
                    {
                        RecordUserAction("Update", "Ammendment Application not Accepted", "Failed", Session["app_id"].ToString(), 1);
                        Label69.ForeColor = System.Drawing.Color.Red;
                        Label69.Text = "Submission Failed..Try Again";
                        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "none", "<script> $(window).on('load', function () { $('#MyErrorModal').modal({ backdrop: 'static' });});</script>", false);

                    }



                }       
                       
                
            }
            catch (Exception ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "accecptapplicationmodalclick_Click" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
            }
        }

        protected void btnRedirect_Click(object sender, EventArgs e)
        {
            Response.Redirect("SocietyAmendment.aspx");
        }

        protected void View_adddocs_Click1(object sender, ImageClickEventArgs e)
        {
            try
            {
                ImageButton btn = (ImageButton)sender;
                GridViewRow row = (GridViewRow)btn.NamingContainer;
                HiddenField hdobid = row.FindControl("hfobjectID") as HiddenField;
                if (hdobid != null)
                {
                    string objectid = hdobid.Value;
                    if (objectid != null)
                    {
                        Session["ObjectID"] = objectid;
                        Session["loadflag"] = "yes";
                        Session["myid"] = 0;
                        Session["collection"] = "Amendment Documents";
                        Session["docid"] = 2;
                    }
                    else
                    {
                        Session["loadflag"] = "stop";
                    }
                }
            }
            catch (Exception ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "View_adddocs_Click" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
            }
        }

        protected void View_adddocs_Click2(object sender, ImageClickEventArgs e)
        {
            try
            {
                ImageButton btn = (ImageButton)sender;
                GridViewRow row = (GridViewRow)btn.NamingContainer;
                HiddenField hdobid = row.FindControl("hfobjectID") as HiddenField;
                if (hdobid != null)
                {
                    string objectid = hdobid.Value;
                    if (objectid != null)
                    {
                        Session["ObjectID"] = objectid;
                        Session["loadflag"] = "yes";
                        Session["myid"] = 0;
                        Session["collection"] = "Amendment Documents";
                        Session["docid"] = 2;
                    }
                    else
                    {
                        Session["loadflag"] = "stop";
                    }
                }
            }
            catch (Exception ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "View_adddocs_Click" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
            }
        }

        protected void LbView_Click(object sender, EventArgs e)
        {
            try
            {
                LinkButton btn = (LinkButton)sender;
                GridViewRow row = (GridViewRow)btn.NamingContainer;
                HiddenField hdobid = row.FindControl("hfobjectID") as HiddenField;
                if (hdobid != null)
                {
                    string objectid = hdobid.Value;
                    if (objectid != null)
                    {
                        Session["ObjectID"] = objectid;
                        Session["loadflag"] = "yes";
                        Session["myid"] = 0;
                        Session["collection"] = "Amendment Documents";
                        Session["docid"] = 2;
                    }
                    else
                    {
                        Session["loadflag"] = "stop";
                    }
                }
            }
            catch (Exception ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "View_adddocs_Click" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
            }

        }

        
        protected int saveremarksbysro(int index)  // 1 - observationbtnclicked   //2- accepted btn clicked
        {
            int retval = 0;
            string remarks = outputtextremarks.InnerText;
            string loginid = Utility.getUserLoginID(Session["app_id"].ToString());
            string submitime = getamendmentsubmittimebyuser();
            string user_fullname = Session["userfirstname"].ToString() + " " + Session["usermiddlename"].ToString() + " " + Session["userlastname"].ToString();

            NpgsqlConnection conn = new NpgsqlConnection();
            NpgsqlCommand cmd = new NpgsqlCommand();
            conn.ConnectionString = ConfigurationManager.ConnectionStrings["connect"].ConnectionString;
            cmd.Connection = conn;
            conn.Open();
            NpgsqlTransaction trans = conn.BeginTransaction();
            try
            {
                string ins_query = "";
                if(index == 1)
                {
                    ins_query = "INSERT INTO esociety.remarks_amendment(app_id,login_id,regdistrict,submittime_appl_byuser,obsremarks_bydro,submittime_obsremarks,updated_by,updated_at,ipaddress,updated_by_name)";
                    ins_query = ins_query + " VALUES(@app_id,@login_id,@regdistrict,@submittime_appl_byuser,@remarks,current_timestamp,@updated_by,current_timestamp,@ipaddress,@updated_by_name)";
                }
                else
                {
                    ins_query = "INSERT INTO esociety.remarks_amendment(app_id,login_id,regdistrict,submittime_appl_byuser,amendacceptedremarks,submittime_amendaccepted,updated_by,updated_at,ipaddress,updated_by_name)";
                    ins_query = ins_query + " VALUES(@app_id,@login_id,@regdistrict,@submittime_appl_byuser,@remarks,current_timestamp,@updated_by,current_timestamp,@ipaddress,@updated_by_name)";
                }
                
                cmd.CommandText = ins_query;
                cmd.Parameters.AddWithValue("@app_id", Convert.ToInt64(Session["app_id"].ToString()));
                cmd.Parameters.AddWithValue("@login_id", loginid);
                cmd.Parameters.AddWithValue("@regdistrict", Convert.ToInt32(Session["DistrictID"]));
                cmd.Parameters.AddWithValue("@submittime_appl_byuser",Convert.ToDateTime(submitime));
                cmd.Parameters.AddWithValue("@remarks", remarks);
                cmd.Parameters.AddWithValue("@updated_by", Session["firstname"].ToString());
                cmd.Parameters.AddWithValue("@updated_by_name", user_fullname);
                cmd.Parameters.AddWithValue("@ipaddress", ipaddress);
                cmd.ExecuteNonQuery();
                cmd.Parameters.Clear();

                string update_query = "";                
                update_query = "UPDATE esociety.status_amendment set amend_status=@amend_status, obsremarks_bydro=@obsremarks_bydro where app_id= @app_id";
                cmd.CommandText = update_query;
                if(index == 1)
                {
                    cmd.Parameters.AddWithValue("@amend_status",3);
                }
                else
                {
                    cmd.Parameters.AddWithValue("@amend_status", 5);
                }
                cmd.Parameters.AddWithValue("@obsremarks_bydro", remarks);
                cmd.Parameters.AddWithValue("@app_id", Convert.ToInt64(Session["app_id"].ToString()));
                cmd.ExecuteNonQuery();
                cmd.Parameters.Clear();

                trans.Commit();
                retval = 1;
                


            }
            catch (NpgsqlException ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(HttpContext.Current.Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "checked_by_sro_after_dh()----Insert.cs");
                RecordUserAction("Update", "Send Observation Failed", "Failed", Session["app_id"].ToString(), 1);
                trans.Rollback();
                retval = 0;
               
            }
            finally
            {
                conn.Close();
            }
            return retval;
        }
            
        protected string getamendmentsubmittimebyuser()
        {
            string submittime="";

            NpgsqlConnection conn = new NpgsqlConnection();
            NpgsqlCommand cmd = new NpgsqlCommand();
            conn.ConnectionString = ConfigurationManager.ConnectionStrings["connect"].ConnectionString;
            cmd.Connection = conn;
            try
            {
                conn.Open();
                string query = "select amend_status,amend_submittime,amend_obssubmittime from esociety.status_amendment where app_id=@appid";
                cmd.CommandText = query;
                cmd.Parameters.AddWithValue("@appid", Convert.ToInt64(Session["app_id"].ToString()));               
                NpgsqlDataReader dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    if(Convert.ToInt32(dr["amend_status"])==2)
                    {
                        submittime = dr["amend_submittime"].ToString();
                    }
                    else
                    {
                        submittime = dr["amend_obssubmittime"].ToString();
                    }
                }
                dr.Close();

            }
            catch (NpgsqlException ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "getmyID()" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
                Response.Write("<script language='javascript'>alert('" + "Not getting PDF ID" + "')</script>");
              
            }
            finally
            {

                conn.Close();
            }

            return submittime;
        }

        protected void nav_applications_Click(object sender, EventArgs e)
        {
            try
            {
                Session["flagAmenddocs"] = 1;
                Response.Redirect("SocietyAmendment.aspx");
            }
            catch (Exception ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "nav_applications_Click" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
            }
        }

        protected void gobacktosociety_Click(object sender, EventArgs e)
        {
            Response.Redirect("VerifySociety.aspx");
        }
    }
}