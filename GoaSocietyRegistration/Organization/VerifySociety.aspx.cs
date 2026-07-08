using GoaSocietyRegistration.Development;
using MongoDB.Bson;
using MongoDB.Driver;
using Npgsql;
using System;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WS_Encryption;

namespace GoaSocietyRegistration.Admin
{
    public partial class VerifySoceity : System.Web.UI.Page
    {
        Validate _val = new Validate();
        Validate vs = new Validate();
        Insert ins = new Insert(); string ct = string.Empty;
        Int64 fileSizeFront = 0;
        byte[] documentBinary = new Byte[0];
        string ipaddress = Utility.getIP();
        string macaddress = Utility.GetMACAddress(Utility.getIP());
        //string macaddress = Utility.GetMACAddress();
        OtherDocuments doc = new OtherDocuments();
        NICEncryption _encryption = new NICEncryption();
        ByteEncryption.ByteEncryption obj_Byte_Encryption = new ByteEncryption.ByteEncryption();

        //static int flag4; for certificate not required as of now

        [DllImport(@"urlmon.dll", CharSet = CharSet.Auto)]
        private extern static UInt32 FindMimeFromData(UInt32 pBC, [MarshalAs(UnmanagedType.LPStr)] String pwzUrl, [MarshalAs(UnmanagedType.LPArray)] byte[] pBuffer,
       UInt32 cbSize, [MarshalAs(UnmanagedType.LPStr)] String pwzMimeProposed, UInt32 dwMimeFlags, out UInt32 ppwzMimeOut, UInt32 dwReserverd);

        Page_status_Check psc = new Page_status_Check();
        SendSMS sms = new SendSMS();
        string getsetsociety, getsetmembers;
        static int obs_id = 0;
        System.Globalization.CultureInfo french = new System.Globalization.CultureInfo("fr-FR");

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
                Int32 districtid, loginsession;
                if (!IsPostBack)
                {
                    Session["flagsociety"] = 1;
                    if(Utility.checkifrenewal(Session["app_id"].ToString()) ==2)
                    {
                        Renewallabel.Visible = true;
                        ViewState["renewal"] = 2;
                    }
                    else
                    {
                        ViewState["renewal"] = 1;
                        Renewallabel.Visible = false;
                    }

                }
                if (Session["SessionID"] != null || Session["firstname"] != null)
                {
                   
                    loginsession = Convert.ToInt32(Session["SessionID"]);
                    districtid = Convert.ToInt32(Session["DistrictID"]);
                    string firstname = Session["firstname"].ToString();
                    string appid = Session["app_id"].ToString();
                    value_application_id.Text = appid;
                    psc = ins.getPageStatus(Session["app_id"].ToString());
                    if (Convert.ToInt32(Session["flagsociety"].ToString()) == 1)
                    {
                        loadsociety();
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
        protected void loadsociety()
        {
            try
            {
                if (Session["app_id"] != null)
                {
                    FetchDetails data = new FetchDetails();
                    data = ins.LoadFullData(Session["app_id"].ToString());
                    value_applicant_name.Text = Server.HtmlEncode(data.applicant_name);
                    value_address.Text = Server.HtmlEncode(data.applicant_address) + ", " + Server.HtmlEncode(data.districtname);
                    value_mobileno.Text = Server.HtmlEncode(data.applicant_mobile_no);
                    if (Server.HtmlEncode(data.applicant_email) == "NA")
                    {
                        value_email.Text = "--";
                        value_email.ForeColor = System.Drawing.Color.Red;
                    }
                    else
                    {
                        value_email.Text = Server.HtmlEncode(Encryption.Encrypt.Decrypt(data.applicant_email));
                        value_email.ForeColor = System.Drawing.Color.Green;
                    }
                    //value_district.Text = Server.HtmlEncode(data.districtname);
                    value_desigantion.Text = Server.HtmlEncode(data.designationname);
                    value_society_type.Text = Server.HtmlEncode(data.societytype);
                    value_society_name.Text = Server.HtmlEncode(data.socname);
                    value_society_address.Text = Server.HtmlEncode(data.socaddr) + ", " + Server.HtmlEncode(data.soc_dname) + ", " + Server.HtmlEncode(data.soc_talukaname);
                    //value_society_district.Text = Server.HtmlEncode(data.soc_dname);
                    //value_taluka.Text = Server.HtmlEncode(data.soc_talukaname);
                    value_registration_fee.Text = "₹ " + Server.HtmlEncode(data.regfee);
                    value_processing_fee.Text = "₹ " + Server.HtmlEncode(data.processfee);
                    value_total_fee.Text = "₹ " + Server.HtmlEncode(data.totalfee);
                   

                    loadobjective();

                    if(Convert.ToInt32(ViewState["renewal"])==2)
                    {
                        tr_processfee.Visible = false;
                        tr_regfee.Visible = false;
                        tr_totalfee.Visible = false;
                    }
                }
                else
                {
                    //CreateLogFiles Err = new CreateLogFiles();
                    //Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), "Session null", "groom");
                }
                setobs_id(1);
            }
            catch (Exception ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "loadsociety()" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
            }
        }
        protected void loadmembers()
        {
            int exists = getmyID(Convert.ToInt64(Session["app_id"].ToString()), 7);
            if(exists == 1)
            {
                listexist.Text= "Members List Available!!!";
                ImgBtnViewPdf7.Enabled = true;
            }
            else
            {
                listexist.Text = "Members List Not Available!!!";
                ImgBtnViewPdf7.Enabled = false;
            }
            NpgsqlConnection conn = new NpgsqlConnection();
            NpgsqlCommand cmd = new NpgsqlCommand();
            conn.ConnectionString = ConfigurationManager.ConnectionStrings["connect"].ConnectionString;
            cmd.Connection = conn;
            conn.Open();
            NpgsqlTransaction myTrans = conn.BeginTransaction();
            cmd.Transaction = myTrans;
            try
            {
                
                string mem_query = "select fname,case when (design='8') then concat('Other : ' , designtaion_others) else designtaion end as designtaion,\"OccupationName\",address,proofname,mangcomm,doc_id,document_mongoentry,salutation,gender,age,member_id,occupation_others,remarks,dateofadmission";
                mem_query = mem_query + " from esociety.members inner join esociety.mst_occupation on esociety.mst_occupation.\"OccupationID\" = members.occupatid";
                mem_query = mem_query + " where app_id=@appid order by design ASC ";
                cmd.CommandText = mem_query;
                cmd.Parameters.AddWithValue("@appid", Convert.ToInt64(Session["app_id"].ToString()));
                NpgsqlDataReader rdr = cmd.ExecuteReader();
                grvMemberDetails.DataSource = rdr;
                grvMemberDetails.DataBind();
                value_total_members.Text = grvMemberDetails.Rows.Count.ToString();
               
                rdr.Close();

                string mangcomm_query = "select fname,case when (design='8') then concat('Other : ' , designtaion_others) else designtaion end as designtaion,\"OccupationName\",address,proofname,mangcomm,doc_id,document_mongoentry,salutation,gender,age,member_id,occupation_others,remarks,dateofadmission";
                mangcomm_query = mangcomm_query + " from esociety.members inner join esociety.mst_occupation on esociety.mst_occupation.\"OccupationID\" = members.occupatid";
                mangcomm_query = mangcomm_query + " where app_id=@appid and mangcomm='Yes' order by design ASC ";
                cmd.CommandText = mangcomm_query;
                cmd.Parameters.AddWithValue("@appid", Convert.ToInt64(Session["app_id"].ToString()));
                NpgsqlDataReader rdr1 = cmd.ExecuteReader();
                gv_mangcomm.DataSource = rdr1;
                gv_mangcomm.DataBind();
                value_mangcomm_members.Text = gv_mangcomm.Rows.Count.ToString();

                rdr1.Close(); 

                myTrans.Commit();
            }
            catch (NpgsqlException ex)
            {
                myTrans.Rollback();
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "loadmembers()" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
            }
            finally
            {
                conn.Close();
            }
            setobs_id(2);
        }
        public void loaddocuments()
        {
            if(Convert.ToInt32(ViewState["renewal"]) == 1)
            {
                regdoc1.Visible = true;
                regdoc2.Visible = true;
                regdoc3.Visible = true;
            }
            else if (Convert.ToInt32(ViewState["renewal"]) == 2)
            {
                renewdoc1.Visible = true;
                renewdoc2.Visible = true;
                renewdoc3.Visible = true;
                renewdoc4.Visible = true;
                renewdoc5.Visible = true;

                if (Convert.ToInt32(ViewState["renewal"]) == 2)
                {
                    tr_renewfeesheading.Visible = true;

                    tr_feescalc.Visible = true;

                }

            }

            Temp_table tmp_table = new Temp_table();
            //do here remarks condition            

            if (psc.status_id == 3 || psc.status_id == 6)
            {
                if (Convert.ToInt32(Session["ROLE"]) == 3)
                {
                    saveobservation.Visible = false;
                    sendobservation.Visible = false;
                    //reject.Visible = false;
                    accepted.Visible = false;
                    LkReject.Visible = false;
                    outputtextremarks.Visible = true;                   
                    pull_application_from_dh.Visible = true;
                }
                else if (Convert.ToInt32(Session["ROLE"]) == 4)
                {
                    sendobservation.Visible = false;
                    hidefordh.Visible = false;
                    remarks.Visible = false;
                    saveobservation.Visible = true;                    
                    outputtextremarks.Visible = false;
                }

                if (Convert.ToInt32(ViewState["renewal"]) == 2)
                {
                    tr_renewfeesheading.Visible = false;

                    tr_feescalc.Visible = false;

                }

            }
            //psc.status id 5 for user not for admin
            else if (psc.status_id == 4 || psc.status_id == 7)
            {
                if (Convert.ToInt32(Session["ROLE"]) == 3)
                {
                    
                    saveobservation.Visible = false;
                    accepted.Visible = true;
                    LkReject.Visible = true;
                    outputtextremarks.Visible = true;
                    if (psc.status_id == 4)
                    {
                        sendobservation.Visible = true;
                        //reject.Visible = false;
                        try
                        {
                            tmp_table = ins.getRemarksData(Session["app_id"].ToString());
                            string text = Server.HtmlEncode(tmp_table.observation_by_dh);
                            string[] Result = text.Split('|');
                           // outputtext.InnerText = Server.HtmlEncode(Result[0]);
                            outputtext1.InnerText = Server.HtmlEncode(Result[1]);
                           // outputtext.Disabled = true;
                            outputtext1.Disabled = true;

                        }
                        catch (IOException ex)
                        {
                            CreateLogFiles Err = new CreateLogFiles();
                            Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "loaddocuments()" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
                            //RecordUserAction("observation failed to load first time", ex.Message, "F");

                        }
                    }
                    else
                    {
                        try
                        {
                            tmp_table = ins.getRemarksData(Session["app_id"].ToString());
                            string text = Server.HtmlEncode(tmp_table.observation_by_dh);
                            string[] Result = text.Split('|');
                          //  outputtext.InnerText = Server.HtmlEncode(Result[0]);
                            outputtext1.InnerText = Server.HtmlEncode(Result[1]);
                           // outputtext.Disabled = true;
                            outputtext1.Disabled = true;

                            if (Utility.getObsCount(Session["app_id"].ToString()) <= 5)
                            {
                               // reject.Visible = false;
                                sendobservation.Visible = true;
                            }
                            else
                            {
                               // reject.Visible = true;
                                sendobservation.Visible = false;
                            }


                        }
                        catch (IOException ex)
                        {
                            CreateLogFiles Err = new CreateLogFiles();
                            Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "loaddocuments2()" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
                            //RecordUserAction("observation failed to load first time", ex.Message, "F");

                        }
                        //sendobservation.Visible = false;  // commented by sunidhi, implemented within try
                        //reject.Visible = true;
                    }
                  

                }
            }


            //complete all conditions here 

        }

        public void loadadditionaldocuments()
        {
            if (psc.status_id == 1 || psc.status_id == 2)
            {
                
            }
            else { getAdditionaldocs(Convert.ToInt64(Session["app_id"].ToString()), 0); }


            //if (psc.status_id == 3 || psc.status_id == 6 || psc.status_id == 4 || psc.status_id == 7)
            //{
            //    getAdditionaldocs(Convert.ToInt64(Session["app_id"].ToString()), 0);
            //}



        }

        //public void loadAmendDocuments()
        //{

        //    NpgsqlConnection conn = new NpgsqlConnection();
        //    NpgsqlCommand cmd = new NpgsqlCommand();
        //    conn.ConnectionString = ConfigurationManager.ConnectionStrings["connect"].ConnectionString;
        //    cmd.Connection = conn;
        //    try
        //    {
        //        conn.Open();
        //        string query = "SELECT object_id,docname from esociety.otherdoc_amendment where app_id=@appid";
        //        cmd.CommandText = query;
        //        cmd.Parameters.AddWithValue("@appid", appid);

        //        NpgsqlDataAdapter da1 = new NpgsqlDataAdapter(cmd);
        //        DataSet ds = new DataSet();
        //        da1.Fill(ds);

        //        GridView_AddAmendDocs.DataSource = ds;
        //        GridView_AddAmendDocs.DataBind();

        //        da1.Dispose();
        //    }
        //    catch (NpgsqlException ex)
        //    {
        //        CreateLogFiles Err = new CreateLogFiles();
        //        Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "getAdditionaldocs()" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath) + " from " + ipaddress);

        //        Response.Write("<script language='javascript'>alert('" + "Exception in Gridview" + "')</script>");
        //    }
        //    finally
        //    {
        //        conn.Close();
        //    }


        //}

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
                string query = "select object_id  from esociety.otherdoc_crossentry where app_id=@appid and myid=@myid";
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
        protected void view_applicant_logout_Click(object sender, EventArgs e)
        {
            try
            {
                // RecordUserAction("view_applicant_logout_Click", "Logout Button Clicked", "S");
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
                    //RecordUserAction("view_applicant_logout_Click", "Logout Button Clicked", "F");
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
        protected void goback_tomembers_Click(object sender, EventArgs e)
        {
            try
            {
                RecordUserAction("Redirect", "goback_tomembers_Click Redirect from Documents to Members Division", "Success", Session["app_id"].ToString(), 2);
                Session["flagsociety"] = 0;                
                Documents.Visible = false;
                Members.Visible = true;
                loadmembers();
            }
            catch (Exception ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "goback_tomembers_Click" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
            }
        }
        protected void btn_gotomembers_Click(object sender, EventArgs e)
        {
            try
            {
                RecordUserAction("Redirect", "btn_gotomembers_Click Redirect from Applicant/Society to Members Division", "Success", Session["app_id"].ToString(), 2);
                Applicant.Visible = false;
                Members.Visible = true;
                Employees.Visible = false;
                loadmembers();
                Session["flagsociety"] = 0;
            }
            catch (Exception ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "btn_gotomembers_Click" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
            }
        }
        protected void btn_gotosociety_Click(object sender, EventArgs e)
        {
            try
            {
                RecordUserAction("Redirect", "btn_gotosociety_Click Redirect from Members Division back to Applicant/Society", "Success", Session["app_id"].ToString(), 2);
                Members.Visible = false;
                Applicant.Visible = true;
                Employees.Visible = false;
                loadsociety();
                Session["flagsociety"] = 1;
            }
            catch (Exception ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "btn_gotosociety_Click" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
            }
        }
        protected void btn_gotodocument_Click(object sender, EventArgs e)
        {
            try
            {                
                RecordUserAction("Redirect", "btn_gotodocument_Click Redirect from Employees to Documents Division", "Success", Session["app_id"].ToString(), 2);
                Session["flagsociety"] = 0;
               // outputtext.InnerText = getsetsociety;
                outputtext1.InnerText = getsetmembers;
                Applicant.Visible = false;
                Members.Visible = false;
                Employees.Visible = false;
                Documents.Visible = true;               

                loaddocuments();
                loadadditionaldocuments();

                if (Convert.ToInt32(ViewState["renewal"]) == 2)
                {
                    fetchregdate();
                    fetchfees();                  
                }
            }
            catch (Exception ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "btn_gotodocument_Click" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
            }

        }
        protected void ImgBtnViewPdf4_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                if (Session["app_id"] != null)
                {
                    RecordUserAction("Read", "View  Pdf4 File", "Success", Session["app_id"].ToString(), 1);
                    getmyID(Convert.ToInt64(Session["app_id"].ToString()), 4);
                    Session["loadflag"] = "yes";
                    Session["myid"] = 4;
                    Session["collection"] = "Society Documents";
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
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "ImgBtnViewPdf4_Click" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
            }
        }
        protected void ImgBtnViewPdf3_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                if (Session["app_id"] != null)
                {
                    RecordUserAction("Read File", "View Pdf3 File  ", "Success", Session["app_id"].ToString(), 1);
                    getmyID(Convert.ToInt64(Session["app_id"].ToString()), 3);
                    Session["loadflag"] = "yes";
                    Session["myid"] = 3;
                    Session["collection"] = "Society Documents";
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
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "ImgBtnViewPdf3_Click" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
            }
        }
        protected void ImgBtnViewPdf2_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                if (Session["app_id"] != null)
                {
                    RecordUserAction("Read File", "View Pdf2 File  ", "Success", Session["app_id"].ToString(), 1);
                    getmyID(Convert.ToInt64(Session["app_id"].ToString()), 2);
                    Session["loadflag"] = "yes";
                    Session["myid"] = 2;
                    Session["collection"] = "Society Documents";
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
        protected void ImgBtnViewPdf1_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                if (Session["app_id"] != null)
                {
                    RecordUserAction("Read File", "View Pdf1 File  ", "Success", Session["app_id"].ToString(), 1);
                    getmyID(Convert.ToInt64(Session["app_id"].ToString()), 1);
                    Session["loadflag"] = "yes";
                    Session["myid"] = 1;
                    Session["collection"] = "Society Documents";
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
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "ImgBtnViewPdf1_Click" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
            }
        }
        protected void ImgBtnViewPdf5_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                if (Session["app_id"] != null)
                {
                    RecordUserAction("Read File", "View Pdf5 File  ", "Success", Session["app_id"].ToString(), 1);
                    getmyID(Convert.ToInt64(Session["app_id"].ToString()), 5);
                    Session["loadflag"] = "yes";
                    Session["myid"] = 5;
                    Session["collection"] = "Society Documents";
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
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "ImgBtnViewPdf5_Click" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
            }
        }
        protected void ImgBtnViewPdf6_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                if (Session["app_id"] != null)
                {
                    RecordUserAction("Read File", "View Pdf6 File  ", "Success", Session["app_id"].ToString(), 1);
                    getmyID(Convert.ToInt64(Session["app_id"].ToString()), 6);
                    Session["loadflag"] = "yes";
                    Session["myid"] = 6;
                    Session["collection"] = "Society Documents";
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
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "ImgBtnViewPdf6_Click" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
            }
        }

        protected void ImageButton3_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                RecordUserAction("Read File", "View Pdf of Society Additional Documents", "Success", Session["app_id"].ToString(), 1);
                ImageButton btn = (ImageButton)sender;
                GridViewRow row = (GridViewRow)btn.NamingContainer;
                HiddenField hdobid = row.FindControl("hfdmongodoc") as HiddenField;
                if (hdobid != null)
                {
                    string objectid = hdobid.Value;
                    if (objectid != null)
                    {
                        Session["ObjectID"] = objectid;
                        Session["loadflag"] = "yes";
                        Session["myid"] = 0;
                        Session["collection"] = "Members Document";
                        Session["docid"] = 1;
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
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "ImageButton3_Click" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
            }
        }

        protected void ImgBtnViewPdf7_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                if (Session["app_id"] != null)
                {
                    RecordUserAction("Read File", "View Members List File  ", "Success", Session["app_id"].ToString(), 1);
                    getmyID(Convert.ToInt64(Session["app_id"].ToString()), 7);
                    Session["loadflag"] = "yes";
                    Session["myid"] = 7;
                    Session["collection"] = "Society Documents";
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
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "ImgBtnViewPdf7_Click" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
            }
        }

        protected void nav_society_Click(object sender, EventArgs e)
        {
            try
            {
                RecordUserAction("Redirect", "nav_society_Click Redirect to Society Division through navbar", "Success", Session["app_id"].ToString(), 2);
                Session["flagsociety"] = 0;
                Applicant.Visible = true;
                Members.Visible = false;
                Documents.Visible = false;
                Employees.Visible = false;
                loadsociety();
            }
            catch (Exception ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "nav_society_Click" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
            }
        }
        protected void nav_members_Click(object sender, EventArgs e)
        {
            try
            {
                RecordUserAction("Redirect", "nav_members_Click Redirect to Members Division through navbar", "Success", Session["app_id"].ToString(), 2);
                Session["flagsociety"] = 0;
                Applicant.Visible = false;
                Members.Visible = true;
                Documents.Visible = false;
                Employees.Visible = false;
                loadmembers();
            }
            catch (Exception ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "nav_members_Click" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
            }
        }
        protected void nav_documents_Click(object sender, EventArgs e)
        {
            try
            {
                RecordUserAction("Redirect", "nav_documents_Click Redirect to Documents Division through navbar", "Success", Session["app_id"].ToString(), 2);
                Session["flagsociety"] = 0;
                //outputtext.InnerText = getsetsociety;
                outputtext1.InnerText = getsetmembers;
                Applicant.Visible = false;
                Members.Visible = false;
                Documents.Visible = true;
                Employees.Visible = false;
                loaddocuments();
                loadadditionaldocuments();

                if(Convert.ToInt32(ViewState["renewal"]) == 2)
                {
                    fetchregdate();
                    fetchfees();


                }
            }
            catch (Exception ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "nav_documents_Click" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
            }
        }
        protected void dashboard_Click(object sender, EventArgs e)
        {
            try
            {
                RecordUserAction("Redirect", "dashboard_Click Redirect to Dashbaord", "Success", Session["app_id"].ToString(), 2);
                Session["flagsociety"] = 1;
                Response.Redirect("Dashboard.aspx");
            }
            catch (Exception ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "dashboard_Click" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
            }
        }
        protected string getObservation(int oid)
        {
            string observationback = "";
            try
            {
                if (oid == 1)
                {
                    observationback = getsetsociety;
                }
                else if (oid == 2)
                {
                    observationback = getsetmembers;
                }

            }
            catch (Exception ex)
            {
                observationback = null;
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "getObservation()" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
            }
            return observationback;
        }
        protected void setObservation(string tempstring, int oid)
        {
            try
            {
                if (oid == 1)
                {
                    getsetsociety = getsetsociety + "\n" + tempstring;
                }
                if (oid == 2)
                {
                    getsetmembers = getsetmembers + "\n" + tempstring;
                }
            }
            catch (Exception ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "setObservation()" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
            }

        }
        protected void addobservationtotextarea_Click(object sender, EventArgs e)
        {
            try
            {
                int setid = getobs_id();
                var abc = Request.Form["inputtext"];
                setObservation(abc, setid);
                inputtext.InnerText = " ";
            }
            catch (Exception ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "addobservationtotextarea_Click" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
            }
        }
        public void setobs_id(int id)
        {
            obs_id = id;
        }
        protected void addtempobservation_Click(object sender, EventArgs e)
        {
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "none", "<script> $(window).on('load', function () { $('#errorModal').modal({ backdrop: 'static' });});</script>", false);

        }
        
        protected void reject_modal_confirm_button_Click(object sender, EventArgs e)
        {
            try
            {
                string remarks_mandatory = outputtextremarks.InnerText;
                int value = doValidation(remarks_mandatory.Trim(), "Remarks");
                if (value == 1)
                {
                    if (Session["app_id"] != null)
                    {
                        RecordUserAction("rejected_Click", "Application rejected button Click", "Success", Session["app_id"].ToString(), 1);
                        int status = 9;
                        int a = ins.checked_by_sro_after_dh(status, Session["app_id"].ToString(), remarks_mandatory,Session["firstname"].ToString());
                        if (a == 1)
                        {
                            RecordUserAction("Update", "Application rejected Successfully", "Success", Session["app_id"].ToString(), 1);
                            sms.rejected_send_sms_br_sro(getmobileno(Session["app_id"].ToString()));
                            Label65.ForeColor = System.Drawing.Color.Red;
                            Label65.Text = "Application ID:" + Server.HtmlEncode(Session["app_id"].ToString()) + " is rejected.";
                            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "none", "<script> $(window).on('load', function () { $('#redirectModal').modal({ backdrop: 'static' });});</script>", false);
                        }
                        else
                        {
                            RecordUserAction("Update", "Application rejected failed", "Failed", Session["app_id"].ToString(), 1);
                            Label69.ForeColor = System.Drawing.Color.Red;
                            Label69.Text = "Submission Failed..Try Again";
                            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "none", "<script> $(window).on('load', function () { $('#MyErrorModal').modal({ backdrop: 'static' });});</script>", false);

                        }
                    }
                    else
                    {
                        RecordUserAction("Update", "Reject Failed due to null session", "Failed", Session["app_id"].ToString(), 2);
                        Label69.ForeColor = System.Drawing.Color.Red;
                        Label69.Text = "Execution Error..Try Again";
                        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "none", "<script> $(window).on('load', function () { $('#MyErrorModal').modal({ backdrop: 'static' });});</script>", false);


                    }
                }
            }
            catch (Exception ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "reject_modal_confirm_button_Click" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
            }
        }

     
        protected void reject_Click(object sender, EventArgs e)
        {
            try
            {
                RecordUserAction("Update", "rejected_Click Application Modal Button Clicked", "Success", Session["app_id"].ToString(), 2);
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "none", "<script> $(window).on('load', function () { $('#reject_modal').modal({ backdrop: 'static' });});</script>", false);
            }
            catch (Exception ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "reject_Click" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
            }
        }
        protected int doValidation(string str1, string remarkstextbx)
        {
            int a = 0;
            try
            {
                if (str1 == null || str1 == "")
                {
                    a = 0;
                    Label69.ForeColor = System.Drawing.Color.Red;
                    Label69.Text = remarkstextbx + " is Blank!!!!";
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "none", "<script> $(window).on('load', function () { $('#MyErrorModal').modal({ backdrop: 'static' });});</script>", false);

                }
                else if (!vs.validateData(str1, vs.reamrks_validation))
                {
                    a = 0;
                    Label69.ForeColor = System.Drawing.Color.Red;
                    Label69.Text = "No Special Characters allowed in " + remarkstextbx + "!!!";
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


        protected void send_obs_modal_confirm_button_Click(object sender, EventArgs e)
        {
            try
            {
                //complete all conditions                  

                string remarks_mandatory = outputtextremarks.InnerText;
                int value = doValidation(remarks_mandatory.Trim(),"Remarks");
                if (value == 1)
                {
                    if (Session["app_id"] != null)
                    {
                        RecordUserAction("sendobservation_Click", "Send Observation Button Clicked", "Success", Session["app_id"].ToString(), 1);
                        int status = 5;

                        int a = ins.checked_by_sro_after_dh(status, Session["app_id"].ToString(), remarks_mandatory, Session["firstname"].ToString());
                        if (a == 1)
                        {
                            RecordUserAction("Update", "Send Observation Successfully", "Success", Session["app_id"].ToString(), 1);
                            sms.observation_send_sms_br_sro(getmobileno(Session["app_id"].ToString()));
                            Label65.ForeColor = System.Drawing.Color.Green;
                            Label65.Text = "Observation Send Successfully";
                            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "none", "<script> $(window).on('load', function () { $('#redirectModal').modal({ backdrop: 'static' });});</script>", false);
                        }
                        else
                        {
                            RecordUserAction("Update", "Send Observation Failed", "Failed", Session["app_id"].ToString(), 1);
                            Label69.ForeColor = System.Drawing.Color.Red;
                            Label69.Text = "Submission Failed..Try Again";
                            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "none", "<script> $(window).on('load', function () { $('#MyErrorModal').modal({ backdrop: 'static' });});</script>", false);

                        }
                    }
                    else
                    {

                        RecordUserAction("Update", "Send Observation Failed due to null session", "Failed", Session["app_id"].ToString(), 2);
                        Label69.ForeColor = System.Drawing.Color.Red;
                        Label69.Text = "Execution Error..Try Again";
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


        protected void sendobservation_Click(object sender, EventArgs e)
        {
            try
            {
                RecordUserAction("sendobservation_Click", "Send Observation Modal Button Clicked", "Success", Session["app_id"].ToString(), 2);
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "none", "<script> $(window).on('load', function () { $('#sendobs_confirmation_modal').modal({ backdrop: 'static' });});</script>", false);

            }
            catch (Exception ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "sendobservation_Click()" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
            }
        }
        protected void accepted_Click(object sender, EventArgs e)
        {
            try
            {
                RecordUserAction("Update", "accepted_Click Clicked on Accept Button", "Success", Session["app_id"].ToString(), 2);
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "none", "<script> $(window).on('load', function () { $('#AskForPayment').modal({ backdrop: 'static' });});</script>", false);

            }
            catch (Exception ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "accepted_Click()" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
            }

        }
        protected void save_obs_modal_confirm_button_Click(object sender, EventArgs e)
        {
            try
            {
                //code here to save into database
              
                //if (outputtext.InnerText == null || outputtext.InnerText == "")
                //{
                //    Label69.ForeColor = System.Drawing.Color.Red;
                //    Label69.Text = "Society Observation Remarks is Blank!!!!";
                //    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "none", "<script> $(window).on('load', function () { $('#MyErrorModal').modal({ backdrop: 'static' });});</script>", false);

                //}
                 if (outputtext1.InnerText == null || outputtext1.InnerText == "")
                {
                    Label69.ForeColor = System.Drawing.Color.Red;
                    Label69.Text = "Members Observation Remarks is Blank!!!!";
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "none", "<script> $(window).on('load', function () { $('#MyErrorModal').modal({ backdrop: 'static' });});</script>", false);

                }
                else
                {
                    RecordUserAction("LkObsSave_Click", "Save Observation Button Clicked", "Success", "NA", 1);
                    string obs_text = null;
                    //obs_text = outputtext.InnerText + "|" + outputtext1.InnerText;
                    obs_text = "" + "|" + outputtext1.InnerText;
                    if (obs_text == null || obs_text == "")
                    {
                        //obs_text = outputtext.InnerText + "|" + outputtext1.InnerText;
                        obs_text = "" + "|" + outputtext1.InnerText;
                    }
                    if (!vs.validateData(obs_text.Trim(), vs.observation_validation))
                    {
                        Label69.ForeColor = System.Drawing.Color.Red;
                        Label69.Text = "No Special Characters allowed in Observation !!";
                        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "none", "<script> $(window).on('load', function () { $('#MyErrorModal').modal({ backdrop: 'static' });});</script>", false);

                    }
                    else
                    {
                        if (Session["app_id"] != null)
                        {

                            int status = 0;
                            if (psc.status_id == 3)
                            {
                                status = 4;
                            }
                            else if (psc.status_id == 6)
                            {
                                status = 7;
                            }//applicant staus changes to dealing hand
                            int districtid = Convert.ToInt32(Session["DistrictID"]);
                            string uname = Session["firstname"].ToString();
                            int a = ins.save_status_dh(obs_text, status, Session["app_id"].ToString(), psc.status_id, districtid, uname);
                            if (a == 1)
                            {
                                RecordUserAction("Update", "Observation saved Successfully", "Success", Session["app_id"].ToString(), 1);
                                Label65.ForeColor = System.Drawing.Color.Green;
                                Label65.Text = "Observation Saved Successfully";
                                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "none", "<script> $(window).on('load', function () { $('#redirectModal').modal({ backdrop: 'static' });});</script>", false);
                            }
                            else
                            {
                                RecordUserAction("Update", "Observation Save Failed", "Failed", Session["app_id"].ToString(), 1);
                                Label69.ForeColor = System.Drawing.Color.Red;
                                Label69.Text = "Submission Failed..Try Again";
                                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "none", "<script> $(window).on('load', function () { $('#MyErrorModal').modal({ backdrop: 'static' });});</script>", false);

                            }
                        }
                        else
                        {

                            RecordUserAction("Update", "Save Observation Failed due to null session", "Failed", Session["app_id"].ToString(), 2);
                            Label69.ForeColor = System.Drawing.Color.Red;
                            Label69.Text = "Execution Error..Try Again";
                            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "none", "<script> $(window).on('load', function () { $('#MyErrorModal').modal({ backdrop: 'static' });});</script>", false);
                        }
                    }
                }
            
            }
            catch (Exception ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "save_obs_modal_confirm_button_Click" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
            }
        }

        protected void saveobservation_Click(object sender, EventArgs e)
        {
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "none", "<script> $(window).on('load', function () { $('#saveobs_confirmation_modal').modal({ backdrop: 'static' });});</script>", false);
        }
        protected void btnRedirect_Click(object sender, EventArgs e)
        {
            Response.Redirect("Dashboard.aspx");
        }


        protected void accecptapplicationmodalclick_Click(object sender, EventArgs e)
        {
            try
            {
                int isrenew = Convert.ToInt32(ViewState["renewal"]);
                string remarks_mandatory = outputtextremarks.InnerText;
                
                int value = doValidation(remarks_mandatory.Trim(), "Remarks");
                if (value == 1)
                {
                    if (Session["app_id"] != null)
                    {
                        if (isrenew == 1 || (isrenew == 2 && fetchfees() == 1))
                        {
                            RecordUserAction("accepted_Click", "Clicked on Accept Button", "Success", Session["app_id"].ToString(), 1);
                            int status = 8;//obs_id for accpet is 2
                            int a = ins.checked_by_sro_after_dh(status, Session["app_id"].ToString(), remarks_mandatory, Session["firstname"].ToString());
                            if (a == 1)
                            {
                                RecordUserAction("Update", "Application accepted Sucessfully", "Success", Session["app_id"].ToString(), 1);
                                sms.accepted_send_sms_br_sro(getmobileno(Session["app_id"].ToString()));
                                Label65.ForeColor = System.Drawing.Color.Green;
                                Label65.Text = "Application ID:" + Session["app_id"].ToString() + " is accepted for further processing.";
                                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "none", "<script> $(window).on('load', function () { $('#redirectModal').modal({ backdrop: 'static' });});</script>", false);
                            }
                            else
                            {
                                RecordUserAction("Update", "Application submission failed", "Failed", Session["app_id"].ToString(), 1);
                                Label69.ForeColor = System.Drawing.Color.Red;
                                Label69.Text = "Submission Failed..Try Again";
                                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "none", "<script> $(window).on('load', function () { $('#MyErrorModal').modal({ backdrop: 'static' });});</script>", false);
                            }

                            if(isrenew== 2)
                            {
                                updatefeesinsociety();
                            }
                        }
                        else
                        {
                            if(isrenew == 2 && fetchfees() == 0)
                            {
                                Label69.ForeColor = System.Drawing.Color.Red;
                                Label69.Text = "Fees calculation is incomplete";
                                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "none", "<script> $(window).on('load', function () { $('#MyErrorModal').modal({ backdrop: 'static' });});</script>", false);

                            }
                        }
                    }
                    else
                    {
                        RecordUserAction("lkbtnPayment_Click", "Accept Failed due to null session", "Failed", Session["app_id"].ToString(), 2);
                        Label69.ForeColor = System.Drawing.Color.Red;
                        Label69.Text = "Execution Error..Try Again";
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
       
        protected void oldobservation_Click(object sender, EventArgs e)
        {
            if (Session["app_id"] != null)
            {
                RecordUserAction("Read", "View Old Observation", "Sucess", Session["app_id"].ToString(), 1);
                Insert ins = new Insert();
                Temp_table tmp_table = new Temp_table();
                if (psc.status_id == 6 || psc.status_id == 7)//when to show old observation
                {
                    if (Convert.ToInt32(Session["ROLE"]) == 3)
                    {

                        try
                        {
                            tmp_table = ins.getRemarksData(Session["app_id"].ToString());
                            //string text = Server.HtmlEncode(tmp_table.observation_by_dh);
                            string text = getPreviousObsofDh(Session["app_id"].ToString());
                           // tmp_table = getObsData(Session["app_id"].ToString());
                            string remarks_text = Server.HtmlEncode(tmp_table.remarks_sendobservation);
                            string[] Result = text.Split('|');
                           // socmodal.Text = Server.HtmlEncode(Result[0]) != null ? Server.HtmlEncode(Result[0]) : "No Observation found";
                            membersmodal.Text = Server.HtmlEncode(Result[1]) != null ? Server.HtmlEncode(Result[1]) : "No Observation found";
                            remarksmodal.Text = Server.HtmlEncode(remarks_text);
                          //  socmodal.Enabled = false;
                            membersmodal.Enabled = false;
                            old_remarks_modal.Visible = true;
                            remarksmodal.Enabled = false;
                            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "none", "<script> $(window).on('load', function () { $('#showoldobsv').modal({ backdrop: 'static' });});</script>", false);
                        }
                        catch (IOException ex)
                        {
                            //RecordUserAction("observation failed to load first time", ex.Message, "F");
                            CreateLogFiles Err = new CreateLogFiles();
                            Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "oldobservation_Click" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));

                        }
                    }
                    else if (Convert.ToInt32(Session["ROLE"]) == 4)
                    {
                        try
                        {
                            tmp_table = ins.getRemarksData(Session["app_id"].ToString());
                            string text = Server.HtmlEncode(tmp_table.observation_by_dh);
                            string[] Result = text.Split('|');
                            //socmodal.Text = Server.HtmlEncode(Result[0]) != null ? Server.HtmlEncode(Result[0]) : "No Observation found";
                            membersmodal.Text = Server.HtmlEncode(Result[1]) != null ? Server.HtmlEncode(Result[1]) : "No Observation found";
                          //  socmodal.Enabled = false;
                            membersmodal.Enabled = false;
                            old_remarks_modal.Visible = false;
                            remarksmodal.Enabled = false;
                            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "none", "<script> $(window).on('load', function () { $('#showoldobsv').modal({ backdrop: 'static' });});</script>", false);
                        }
                        catch (IOException ex)
                        {
                            CreateLogFiles Err = new CreateLogFiles();
                            Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "oldobservation_Click" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
                            //RecordUserAction("observation failed to load first time", ex.Message, "F");

                        }
                    }
                    else
                    {
                        oldobservation.Enabled = false;
                    }
                }
                else
                {
                    oldobservation.Enabled = false;
                }
            }
        }

        protected void grvMemberDetails_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.Cells[6].Text.Equals("Service (Govt. Employee)"))
                {
                    e.Row.BackColor = Color.PapayaWhip;
                }

                if (e.Row.Cells[6].Text.Equals("Others"))
                {
                    string occupothers = ((Label)e.Row.FindControl("LbOccupOthers")).Text;
                    e.Row.Cells[6].Text = e.Row.Cells[6].Text + " : " + occupothers;
                }

               if(Convert.ToInt32(ViewState["renewal"]) == 1)
                {
                    e.Row.Cells[11].Visible = false;
                }
            }
            catch (Exception ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "grvMemberDetails_RowDataBound" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
            }
        }

        protected void pull_application_from_dh_Click(object sender, EventArgs e)
        {
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "none", "<script> $(window).on('load', function () { $('#pull_application_confirmation_modal').modal({ backdrop: 'static' });});</script>", false);
        }

        protected void modal_pull_application_button_Click(object sender, EventArgs e)
        {
            try
            {
                //code here to save into database
                RecordUserAction("modal_pull_application_button_Click", "Wants to pull application", "Success", "NA", 1);
                string obs_text = null;
                //obs_text = outputtext.InnerText + " Application pulled by DRO" + "|" + outputtext1.InnerText + "Application pulled by DRO";
                obs_text = "" + "" + "|" + outputtext1.InnerText + "Application pulled by DRO";
                if (obs_text == null || obs_text == "")
                {
                    //obs_text = outputtext.InnerText + "|" + outputtext1.InnerText;
                    obs_text = "" + "|" + outputtext1.InnerText;
                }
                if (!vs.validateData(obs_text.Trim(), vs.observation_validation))
                {
                    Label69.ForeColor = System.Drawing.Color.Red;
                    Label69.Text = "No Special Characters allowed in Observation !!";
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "none", "<script> $(window).on('load', function () { $('#MyErrorModal').modal({ backdrop: 'static' });});</script>", false);

                }
                else
                {
                    if (Session["app_id"] != null)
                    {

                        int status = 0;
                        if (psc.status_id == 3)
                        {
                            status = 4;
                        }
                        else if (psc.status_id == 6)
                        {
                            status = 7;
                        }//applicant staus changes to dealing hand
                        int districtid = Convert.ToInt32(Session["DistrictID"]);
                        string uname = Session["firstname"].ToString();
                        int a = ins.save_status_dh(obs_text, status, Session["app_id"].ToString(), psc.status_id, districtid,uname);
                        if (a == 1)
                        {
                            RecordUserAction("Pull", "Application Pulled Successfully", "Success", Session["app_id"].ToString(), 1);
                            Label65.ForeColor = System.Drawing.Color.Green;
                            Label65.Text = "Application pulled and Observation Saved Successfully";
                            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "none", "<script> $(window).on('load', function () { $('#redirectModal').modal({ backdrop: 'static' });});</script>", false);
                        }
                        else
                        {
                            RecordUserAction("Pull", "Application Pulled Failed", "Failed", Session["app_id"].ToString(), 1);
                            Label69.ForeColor = System.Drawing.Color.Red;
                            Label69.Text = "Submission Failed..Try Again";
                            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "none", "<script> $(window).on('load', function () { $('#MyErrorModal').modal({ backdrop: 'static' });});</script>", false);

                        }
                    }
                    else
                    {

                        RecordUserAction("modal_pull_application_button_Click", "Application Pulled Failed due to null session", "Failed", Session["app_id"].ToString(), 2);
                        Label69.ForeColor = System.Drawing.Color.Red;
                        Label69.Text = "Execution Error..Try Again";
                        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "none", "<script> $(window).on('load', function () { $('#MyErrorModal').modal({ backdrop: 'static' });});</script>", false);
                    }
                }
            }
            catch (Exception ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "modal_pull_application_button_Click" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
            }
        }

        public int getobs_id()
        {
            int tempobs_id = obs_id;
            return tempobs_id;
        }



        protected string getmobileno(string appid)
        {
            string decrypt_mobile_no = "";
            if (appid != null)
            {
                NpgsqlConnection conn = new NpgsqlConnection();
                NpgsqlCommand cmd = new NpgsqlCommand();
                conn.ConnectionString = ConfigurationManager.ConnectionStrings["connect"].ConnectionString;
                cmd.Connection = conn;
                conn.Open();
                NpgsqlTransaction myTrans = conn.BeginTransaction();
                cmd.Transaction = myTrans;
                try
                {
                    string querys = "select applicant_mobile_no  from esociety.applicant_details where app_id = @appid";
                    cmd.CommandText = querys;
                    cmd.Parameters.AddWithValue("@appid", Convert.ToInt64(appid));
                    NpgsqlDataReader dr = cmd.ExecuteReader();
                    if (dr.Read())
                    {
                        decrypt_mobile_no = Server.HtmlEncode(Encryption.Encrypt.Decrypt(dr["applicant_mobile_no"].ToString()));
                    }
                    else
                    {
                        decrypt_mobile_no = "";
                    }
                    dr.Close();
                }
                catch (NpgsqlException ex)
                {
                    CreateLogFiles Err = new CreateLogFiles();
                    Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "getmobileno()" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
                    //RecordUserAction("getmobileno", ex.Message, "F");
                    decrypt_mobile_no = "";
                }
                finally
                {
                    conn.Close();
                }
            }
            else
            {
                decrypt_mobile_no = "";
                RecordUserAction("getmobilenofunction", "mobile no not Fetched beacuse of Session Id", "F", "NA", 2);

            }
            return decrypt_mobile_no;
        }

        protected void histobservation_Click(object sender, EventArgs e)
        {
            try
            {

                if (Session["app_id"].ToString() != null || Session["app_id"].ToString() != "")
                {
                    loadGridView_historyobservation(Session["app_id"].ToString());
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

        protected void loadGridView_historyobservation(string app_id)
        {
            if (Session["app_id"].ToString() != null || Session["app_id"].ToString() != "")
            {

                NpgsqlConnection conn = new NpgsqlConnection();
                NpgsqlCommand cmd = new NpgsqlCommand();
                conn.ConnectionString = ConfigurationManager.ConnectionStrings["connect"].ConnectionString;
                cmd.Connection = conn;
                try
                {
                    string query = "SELECT app_id, observation_by_dh, remarks_sendobservation,submit_time_remarkssendobservation FROM esociety.remarks_table";
                    query = query + " where app_id = @app_id order by submit_time_remarkssendobservation asc";
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
                    Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "loadGridView_historyobservation()" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath) + " from " + ipaddress);

                    // RecordUserAction("Read", "Function Load", "Failed", "NA", 2);
                    Response.Write("<script language='javascript'>alert('" + "Error while loading...." + "')</script>");
                }
                finally
                {
                    conn.Close();
                }
            }
            }

       

        protected void gvhistory_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                string text = e.Row.Cells[1].Text;
                string[] Result = text.Split('|');
                int m = e.Row.RowIndex;   
                if (e.Row.RowIndex >=0)
                {
                    //e.Row.Cells[1].Text = Server.HtmlEncode(Result[0]);   // displays in seperate column

                    //e.Row.Cells[2].Text = Server.HtmlEncode(Result[1]);

                    e.Row.Cells[1].Text = Server.HtmlEncode(Result[0]) + "<br />" +
                   "<b> Observation :</b> " + Server.HtmlEncode(Result[1]);
                }

               
            }
            catch (Exception ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "gvhistory_RowDataBound" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
            }
        }

        protected void LbView_Click(object sender, EventArgs e)
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
                        Session["collection"] = "Other Documents";
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
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "Renewal_ImgBtnViewPdf3_Click" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
            }



            
        }

       
        protected void nav_amendment_Click(object sender, EventArgs e)
        {
            try
            {                
                if (psc.status_id >= 11)
                {
                    int amendstatus = ins.getOtherServicesStatus(Session["app_id"].ToString());

                    if (amendstatus == -1)
                    {
                    }
                    else if (amendstatus == 1)
                    {
                        nav_amendment.Enabled = false;
                        Label66.Text = "Alert!!";
                        Label69.ForeColor = System.Drawing.Color.Red;
                        Label69.Text = "Amendment not available.";
                        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "none", "<script> $(window).on('load', function () { $('#MyErrorModal').modal({ backdrop: 'static' });});</script>", false);

                    }
                    else
                    {
                        RecordUserAction("Redirect", "nav_amendment_Click Redirect to Amendment", "Success", Session["app_id"].ToString(), 1);
                        Response.Redirect("VerifyAmendment.aspx");
                    }
                
                  
                }
                else
                {
                    nav_amendment.Enabled = false;
                    Label66.Text = "Alert!!";
                    Label69.ForeColor = System.Drawing.Color.Red;
                    Label69.Text = "Amendment is not available at this stage.";
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "none", "<script> $(window).on('load', function () { $('#MyErrorModal').modal({ backdrop: 'static' });});</script>", false);

                }
            }
            catch (Exception ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "nav_amendment_Click" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
            }
        }

        protected void getAdditionaldocs(Int64 appid, int myid)
        {
            NpgsqlConnection conn = new NpgsqlConnection();
            NpgsqlCommand cmd = new NpgsqlCommand();
            conn.ConnectionString = ConfigurationManager.ConnectionStrings["connect"].ConnectionString;
            cmd.Connection = conn;
            try
            {
                conn.Open();
                string query = "SELECT object_id,docname from esociety.otherdoc_crossentry where app_id=@appid and myid=0";
                cmd.CommandText = query;
                cmd.Parameters.AddWithValue("@appid", appid);

                NpgsqlDataAdapter da1 = new NpgsqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                da1.Fill(ds);
             
                GridView_AddDocs.DataSource = ds;
                GridView_AddDocs.DataBind();
                
                da1.Dispose();
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

        protected void lookuptable_Click(object sender, EventArgs e)
        {
            FetchDetails socData = new FetchDetails();
            socData = ins.LoadFullData(Session["app_id"].ToString());

            if (socData.socname == "" || socData.socname == null)
            {
                Label69.ForeColor = System.Drawing.Color.Red;
                Label69.Text = "Society Name is blank";
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "none", "<script> $(window).on('load', function () { $('#MyErrorModal').modal({ backdrop: 'static' });});</script>", false);

            }
            else
            {

                NpgsqlConnection conn = new NpgsqlConnection();
                NpgsqlCommand cmd = new NpgsqlCommand();
                conn.ConnectionString = ConfigurationManager.ConnectionStrings["connect"].ConnectionString;
                cmd.Connection = conn;
                try
                {
                    string socname1 = Regex.Replace(socData.socname.ToString(), @"\s+", " "); //concatsocname(socData.socname.ToString());

                    conn.Open();
                    string query = "select esociety.applicant_details.login_id,esociety.applicant_details.app_id, mst_district.\"DistrictName\", socname, trim(regexp_replace((regexp_replace(socname, '\\s+', ' ', 'g')),'\\s*,\\s*',',','g')) as newname, socregid, regdate,";
                    query = query + " soc_taluka, status_table.status_id from esociety.applicant_details,esociety.society,esociety.status_table,esociety.mst_district";
                    query = query + " where esociety.applicant_details.app_id = esociety.society.app_id and";
                    query = query + " esociety.applicant_details.app_id  = esociety.status_table.app_id";
                    query = query + " and esociety.society.socdistrict = esociety.mst_district.\"DistrictID\" and status_table.status_id >= 3 and trim(regexp_replace((regexp_replace(socname, '\\s+', ' ', 'g')),'\\s*,\\s*',',','g')) LIKE @socname";                                    

                    cmd.CommandText = query;
                    cmd.Parameters.AddWithValue("@socname", "%" + socname1.ToUpper().Trim() + "%");
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "none", "<script> $(window).on('load', function () { $('#lookupModal').modal({ backdrop: 'static' });});</script>", false);

                    using (NpgsqlDataAdapter dr = new NpgsqlDataAdapter(cmd))
                    {
                        DataTable dt = new DataTable();
                        dr.Fill(dt);
                        gvLookup.DataSource = dt;
                        gvLookup.DataBind();
                    }
                                       

                    string query1 = "SELECT mst_district.\"DistrictName\", socname, socregid, regdate, reg_date, datemodified from";
                    query1 = query1 + " (SELECT * from esociety.society_all UNION ALL SELECT * from esociety.society_all_north)temptable join esociety.mst_district on";
                    query1 = query1 + " temptable.socdistrict = esociety.mst_district.\"DistrictID\" where upper(trim(regexp_replace((regexp_replace(socname, '\\s+', ' ', 'g')),'\\s*,\\s*',',','g'))) LIKE @socname";


                    cmd.CommandText = query1;
                    cmd.Parameters.AddWithValue("@socname", "%" + socname1.ToUpper().Trim() + "%");
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "none", "<script> $(window).on('load', function () { $('#lookupModal').modal({ backdrop: 'static' });});</script>", false);

                    using (NpgsqlDataAdapter dr1 = new NpgsqlDataAdapter(cmd))
                    {
                        DataTable dt1 = new DataTable();
                        dr1.Fill(dt1);
                        gvLookup_offline.DataSource = dt1;
                        gvLookup_offline.DataBind();
                    }
                    RecordUserAction("Read", "lookuptable_Click load lookup gridview", "Success", Session["app_id"].ToString(), 2);


                }
                catch (NpgsqlException ex)
                {
                    CreateLogFiles Err = new CreateLogFiles();
                    Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "lookuptable_Click()" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath) + " from " + ipaddress);
                    RecordUserAction("Read", "lookuptable_Click Error while binding gridview", "Failed", Session["app_id"].ToString(), 2);
                }
                finally
                {
                    socData = null;

                    conn.Close();
                }
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "none", "<script> $(window).on('load', function () { $('#lookupModal').modal({ backdrop: 'static' });});</script>", false);
            }
        }

        protected void gvLookup_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    string app_id = e.Row.Cells[2].Text;
                    if (app_id == value_application_id.Text.Trim())
                    {
                        e.Row.BackColor = System.Drawing.Color.FromName("#5bff684f");
                    }

                    int status = Convert.ToInt16(e.Row.Cells[7].Text);
                    string tstatus = "";
                    switch (status)
                    {
                        case 1:
                            tstatus = "Not Submitted";
                            break;
                        case 3:
                            tstatus = "Submitted first time to Dealing Hand";
                            break;
                        case 4:
                            tstatus = "Checked by Dealing Hand";
                            break;
                        case 5:
                            tstatus = "Application - User Side for Observation";
                            break;
                        case 6:
                            tstatus = "Submitted first time to Dealing Hand after observation";
                            break;
                        case 7:
                            tstatus = "Checked by Dealing Hand after observation";
                            break;
                        case 8:
                            tstatus = "Accepted for payment";
                            break;

                        case 9:
                            tstatus = "Rejected / Closed";
                            break;
                        case 10:
                            tstatus = "Payment Done";
                            break;

                        case 11:
                            tstatus = "";
                            break;
                        case 12:
                            tstatus = "Certificate Generated";
                            break;

                        default:
                            tstatus = "Status Not found";
                            break;
                    }
                    e.Row.Cells[7].Text = tstatus;
                                       
                    e.Row.Cells[5].Text = e.Row.Cells[5].Text.Replace("&nbsp;", "");
                    if (e.Row.Cells[5].Text != "" && e.Row.Cells[5].Text != null)
                    {
                        string tempdate = e.Row.Cells[5].Text;
                        DateTime regdate = Convert.ToDateTime(tempdate, french).Date;
                        e.Row.Cells[5].Text = regdate.ToShortDateString();
                    }
                }
            }
            catch (Exception ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "gvLookup_RowDataBound" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath) + " from " + ipaddress);

            }
        }

        protected void LkviewLookup_Click(object sender, EventArgs e)
        {
            try
            {
                LinkButton btn = (LinkButton)sender;
                GridViewRow row = (GridViewRow)btn.NamingContainer;
                string app_id = ((Label)gvLookup.Rows[row.RowIndex].FindControl("LbSearchApp_id")).Text;
                Session["app_id"] = app_id;
                Response.Redirect("VerifySociety.aspx");
            }
            catch (Exception ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(HttpContext.Current.Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "LkviewLookup_Click()" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath) + " from " + ipaddress);
            }
        }

     

        protected void gvLookup_offline_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                HiddenField hfdatemodify = (e.Row.FindControl("hfdatemodify") as HiddenField);
               

                if (hfdatemodify.Value == "0")
                {
                    string temp1 = ((Label)e.Row.FindControl("LbDate")).Text;

                  
                    e.Row.Cells[4].Text = temp1.ToString();
                }


                e.Row.Cells[0].Text = "-";
                e.Row.Cells[1].Text = "-";
                e.Row.Cells[6].Text = "-";

            }
        }

        

        protected string getPreviousObsofDh(string appid)
        {
            int count = Utility.getObsCount(appid);
            string dh_remark = "";
            NpgsqlConnection conn = new NpgsqlConnection();
            NpgsqlCommand cmd = new NpgsqlCommand();
            conn.ConnectionString = ConfigurationManager.ConnectionStrings["connect"].ConnectionString;
            cmd.Connection = conn;
            try
            {
                conn.Open();

                string query = "select observation_by_dh from esociety.remarks_table where app_id=@app_id and obs_count=@obs_count";
                cmd.CommandText = query;
                cmd.Parameters.AddWithValue("@app_id", Convert.ToInt64(appid));
                cmd.Parameters.AddWithValue("@obs_count", count - 1);
                NpgsqlDataReader rd = cmd.ExecuteReader();
                if (rd.Read())
                {
                    dh_remark = rd["observation_by_dh"].ToString();
                }
                else
                {
                    dh_remark = "";
                }
                rd.Close();
            }
            catch (NpgsqlException ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(HttpContext.Current.Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "getPreviousObsofDh()");
                dh_remark = "";
            }
            finally
            {
                conn.Close();
            }
            return dh_remark;
        }

        protected void gv_mangcomm_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.Cells[6].Text.Equals("Service (Govt. Employee)"))
                {
                    e.Row.BackColor = Color.PapayaWhip;
                }

                if (e.Row.Cells[6].Text.Equals("Others"))
                {
                    string occupothers = ((Label)e.Row.FindControl("LbOccupOthers")).Text;
                    e.Row.Cells[6].Text = e.Row.Cells[6].Text + " : " + occupothers;
                }

                if (Convert.ToInt32(ViewState["renewal"]) == 1)
                {
                    e.Row.Cells[11].Visible = false;
                }

            }
            catch (Exception ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "gv_mangcomm_RowDataBound" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
            }

        }
              


        protected void ImgBtn_renewalAppln_ViewPdf_Click(object sender, ImageClickEventArgs e)
        {

            try
            {
                if (Session["app_id"] != null)
                {
                    RecordUserAction("Read File", "View Renewal Application File  ", "Success", Session["app_id"].ToString(), 2);
                    getmyID(Convert.ToInt64(Session["app_id"].ToString()), 8);
                    Session["loadflag"] = "yes";
                    Session["myid"] = 8;
                    Session["collection"] = "Society Documents";
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
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "ImgBtn_renewalAppln_ViewPdf_Click" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
            }
        }

        protected void ImgBtn_Schedule1_ViewPdf_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                if (Session["app_id"] != null)
                {
                    RecordUserAction("Read File", "View Schedule 1 File ", "Success", Session["app_id"].ToString(), 2);
                    getmyID(Convert.ToInt64(Session["app_id"].ToString()), 9);
                    Session["loadflag"] = "yes";
                    Session["myid"] = 9;
                    Session["collection"] = "Society Documents";
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
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "ImgBtn_Schedule1_ViewPdf_Click" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
            }
        }


        protected void ImgBtn_Schedule6_ViewPdf_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                if (Session["app_id"] != null)
                {
                    RecordUserAction("Read File", "View Schedule 4 File", "Success", Session["app_id"].ToString(), 2);
                    getmyID(Convert.ToInt64(Session["app_id"].ToString()), 10);
                    Session["loadflag"] = "yes";
                    Session["myid"] = 10;
                    Session["collection"] = "Society Documents";
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
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "ImgBtn_Schedule6_ViewPdf_Click" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
            }
        }

        protected void ImgBtn_Schedule2_ViewPdf_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                if (Session["app_id"] != null)
                {
                    RecordUserAction("Read File", "View Schedule 4 File", "Success", Session["app_id"].ToString(), 2);
                    getmyID(Convert.ToInt64(Session["app_id"].ToString()), 11);
                    Session["loadflag"] = "yes";
                    Session["myid"] = 11;
                    Session["collection"] = "Society Documents";
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
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "ImgBtn_Schedule2_ViewPdf_Click" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
            }
        }

        protected void ImgBtn_Schedule4_ViewPdf_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                if (Session["app_id"] != null)
                {
                    RecordUserAction("Read File", "View Schedule 4 File", "Success", Session["app_id"].ToString(), 2);
                    getmyID(Convert.ToInt64(Session["app_id"].ToString()), 12);
                    Session["loadflag"] = "yes";
                    Session["myid"] = 12;
                    Session["collection"] = "Society Documents";
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
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "ImgBtn_Schedule4_ViewPdf_Click" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
            }
        }     

        protected void btn_gotodocumentoremployee_Click(object sender, EventArgs e)
        {
            try
            {
                int is_renewal = Convert.ToInt32(ViewState["renewal"]);
                if ( is_renewal == 1)
                {
                    RecordUserAction("Redirect", "btn_gotodocumentoremployee_Click Redirect from Members to Documents Division", "Success", Session["app_id"].ToString(), 2);
                    Session["flagsociety"] = 0;
                    //outputtext.InnerText = getsetsociety;
                    outputtext1.InnerText = getsetmembers;
                    Applicant.Visible = false;
                    Members.Visible = false;
                    Employees.Visible = false;
                    Documents.Visible = true;

                    loaddocuments();
                    loadadditionaldocuments();

                  //  fetchregdate();
                   // fetchfees();

                }
                else if(is_renewal == 2)
                {
                    RecordUserAction("Redirect", "btn_gotodocumentoremployee_Click Redirect from Members to Employee Division", "Success", Session["app_id"].ToString(), 2);
                    Session["flagsociety"] = 0;
                   // outputtext.InnerText = getsetsociety;
                    outputtext1.InnerText = getsetmembers;
                    Applicant.Visible = false;
                    Members.Visible = false;
                    Employees.Visible = true;
                    Documents.Visible = false;

                    loademployees();

                }
            }
            catch (Exception ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "btn_gotodocumentoremployee_Click" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
            }
        }

        protected void goback_tomembersoremployee_Click(object sender, EventArgs e)
        {
            try
            {
                int is_renewal = Convert.ToInt32(ViewState["renewal"]);
                if (is_renewal == 1)
                {
                    RecordUserAction("Redirect", "btn_gotodocumentoremployee_Click Redirect from Members to Documents Division", "Success", Session["app_id"].ToString(), 2);
                    Session["flagsociety"] = 0;
                    //outputtext.InnerText = getsetsociety;
                    outputtext1.InnerText = getsetmembers;
                    Applicant.Visible = false;
                    Members.Visible = true;
                    Employees.Visible = false;
                    Documents.Visible = false;

                    loadmembers();

                }
                else if (is_renewal == 2)
                {
                    RecordUserAction("Redirect", "btn_gotodocumentoremployee_Click Redirect from Members to Employee Division", "Success", Session["app_id"].ToString(), 2);
                    Session["flagsociety"] = 0;
                   // outputtext.InnerText = getsetsociety;
                    outputtext1.InnerText = getsetmembers;
                    Applicant.Visible = false;
                    Members.Visible = false;
                    Employees.Visible = true;
                    Documents.Visible = false;

                    loademployees();

                }
            }
            catch (Exception ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "goback_tomembersoremployee_Click" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
            }
        }



        //public Temp_table getObsData(string appid)
        //{
        //    int count = Utility.getObsCount(appid);
        //    Temp_table temp = new Temp_table();
        //    NpgsqlConnection conn = new NpgsqlConnection();
        //    NpgsqlCommand cmd = new NpgsqlCommand();
        //    conn.ConnectionString = ConfigurationManager.ConnectionStrings["connect"].ConnectionString;
        //    cmd.Connection = conn;
        //    try
        //    {
        //        conn.Open();
        //        // string query = "select remarks_sendobservation from esociety.remarks_table where app_id=@app_id and obs_count=@obs_count";
        //        string query = "select remarks_sendobservation from esociety.temp_table where app_id=@app_id";
        //        cmd.CommandText = query;
        //        cmd.Parameters.AddWithValue("@app_id", Convert.ToInt64(appid));
        //       // cmd.Parameters.AddWithValue("@obs_count", count - 1);
        //        NpgsqlDataReader rd = cmd.ExecuteReader();
        //        if (rd.Read())
        //        {
        //            temp.remarks_sendobservation = rd["remarks_sendobservation"].ToString();
        //        }
        //        else
        //        {
        //            temp = null;
        //        }
        //        rd.Close();
        //    }
        //    catch (NpgsqlException ex)
        //    {
        //        CreateLogFiles Err = new CreateLogFiles();
        //        Err.ErrorLog(HttpContext.Current.Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "getObsData()");
        //        temp = null;
        //    }
        //    finally
        //    {
        //        conn.Close();
        //    }
        //    return temp;
        //}


        protected void loadobjective()
        {
            NpgsqlConnection conn = new NpgsqlConnection();
            NpgsqlCommand cmd = new NpgsqlCommand();
            conn.ConnectionString = ConfigurationManager.ConnectionStrings["connect"].ConnectionString;
            cmd.Connection = conn;
            try
            {
                conn.Open();
              
                string query = "select objective from esociety.society_objectives where app_id=@app_id and active='Y' order by row_id ASC";
                cmd.CommandText = query;
                cmd.Parameters.AddWithValue("@app_id", Convert.ToInt64(Session["app_id"].ToString()));
                NpgsqlDataAdapter da = new NpgsqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                da.Fill(ds);               
                gv_objective.DataSource = ds;
                gv_objective.DataBind();                    
                
               
            }
            catch (NpgsqlException ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(HttpContext.Current.Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "loadobjective()");
                
            }
            finally
            {
                conn.Close();
            }
        }




        protected void loademployees()
        {
            NpgsqlConnection conn = new NpgsqlConnection();
            NpgsqlCommand cmd = new NpgsqlCommand();
            conn.ConnectionString = ConfigurationManager.ConnectionStrings["connect"].ConnectionString;
            cmd.Connection = conn;
            try
            {
                conn.Open();
                string query1 = "SELECT concat(employee_name,' and ' , designation) as name_desig, present_pay_scale, temporary_permanent, present_pay, dearness_allowance, special_pay,";
                query1 = query1 + " other_allowance, provident_fund, other_benefits, entered_by, entered_at, ipaddress, app_id, employee_id";
                query1 = query1 + " FROM esociety.society_employement where app_id = @app_id and active ='Y'";
                cmd.CommandText = query1;
                cmd.Parameters.AddWithValue("@app_id",Convert.ToInt64(Session["app_id"]));
                NpgsqlDataAdapter da = new NpgsqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                da.Fill(ds);
                if (ds.Tables[0].Rows.Count > 0)
                {
                    gv_employeedetails.DataSource = ds;
                    gv_employeedetails.DataBind();

                   
                }
                tot_employee.Text = gv_employeedetails.Rows.Count.ToString();

            }
            catch (NpgsqlException ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "loademployees()" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
               
                Response.Write("<script language='javascript'>alert('" + "Exception in bindgridview_oldmembers" + "')</script>");
            }
            finally
            {
                conn.Close();
            }
        }

        protected void LkCalcFees_Click(object sender, EventArgs e)
        {
            try
            {
                if(TxtBxRegistrationDate.Text == null || TxtBxRegistrationDate.Text == "")
                {
                    Label69.ForeColor = System.Drawing.Color.Red;
                    Label69.Text = "Registration Date is Blank";
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "none", "<script> $(window).on('load', function () { $('#MyErrorModal').modal({ backdrop: 'static' });});</script>", false);

                }
                else if (TxtBxRenewalDate.Text == null || TxtBxRenewalDate.Text == "")
                {
                    Label69.ForeColor = System.Drawing.Color.Red;
                    Label69.Text = "Last date for Renewal is Blank";
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "none", "<script> $(window).on('load', function () { $('#MyErrorModal').modal({ backdrop: 'static' });});</script>", false);

                }
                else if (TxtbxDueDate.Text == null || TxtbxDueDate.Text == "")
                {
                    Label69.ForeColor = System.Drawing.Color.Red;
                    Label69.Text = "Due date is Blank";
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "none", "<script> $(window).on('load', function () { $('#MyErrorModal').modal({ backdrop: 'static' });});</script>", false);

                }
                else if(Convert.ToDateTime(TxtBxRenewalDate.Text) < Convert.ToDateTime(TxtBxRegistrationDate.Text))
                {
                    Label69.ForeColor = System.Drawing.Color.Red;
                    Label69.Text = "Invalid Last date for Renewal";
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "none", "<script> $(window).on('load', function () { $('#MyErrorModal').modal({ backdrop: 'static' });});</script>", false);

                }
                else
                {
                    DateTime startdate = Convert.ToDateTime(TxtBxRenewalDate.Text, french);
                    DateTime enddate = Convert.ToDateTime(TxtbxDueDate.Text, french);
                    int months_delayed = Utility.GetMonthDifference(startdate.Date, enddate.Date);
                    int penalty_amt = months_delayed * 5;
                    int total_amt = 75 + penalty_amt;
                    TxtBxPenalty.Text = penalty_amt.ToString();
                    txtbxtotalfees.Text = total_amt.ToString();
                    hffees.Value = total_amt.ToString();
                }

            }
            catch (Exception ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "LkCalcFees_Click" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
            }

        }

        protected void fetchregdate()
        {

            string text = Utility.getOldRegistrationNo(Session["app_id"].ToString());
            string[] Result = text.Split('|');
            string socregid = Server.HtmlEncode(Result[0]);
            string district = Server.HtmlEncode(Result[1]);

            NpgsqlConnection conn = new NpgsqlConnection();
            NpgsqlCommand cmd = new NpgsqlCommand();
            conn.ConnectionString = ConfigurationManager.ConnectionStrings["connect"].ConnectionString;
            cmd.Connection = conn;
            conn.Open();
            NpgsqlTransaction myTrans = conn.BeginTransaction();
            cmd.Transaction = myTrans;

            try
            {               
                
                string query2 = "select regdate from esociety.society where socregid=@socregid";
                cmd.CommandText = query2;
                cmd.Parameters.AddWithValue("@socregid", socregid);
                NpgsqlDataReader rd1 = cmd.ExecuteReader();
                if (rd1.Read())
                {
                    //lblregdate.Text = Server.HtmlEncode(rd1["regdate"].ToString());                   
                    TxtBxRegistrationDate.Text = (Convert.ToDateTime(rd1["regdate"], french).Date).ToString("yyyy-MM-dd");
                    rd1.Close();
                }
                else
                {
                    rd1.Close();
                    string query3 = "select regdate,datemodified,reg_date from esociety.society_all where socregid=@socregid and socdistrict = @socdistrict";
                    cmd.CommandText = query3;
                    cmd.Parameters.AddWithValue("@socregid", socregid);
                    cmd.Parameters.AddWithValue("@socdistrict", Convert.ToInt32(district));
                    NpgsqlDataReader rd2 = cmd.ExecuteReader();
                    if (rd2.Read())
                    {
                        if (Convert.ToInt32(rd2["datemodified"]) == 0)
                        {
                            TxtBxRegistrationDate.Text = (Convert.ToDateTime(rd2["regdate"], french).Date).ToString("yyyy-MM-dd");
                        }
                        else if (Convert.ToInt32(rd2["datemodified"]) == 1)
                        {
                            TxtBxRegistrationDate.Text = (Convert.ToDateTime(rd2["reg_date"], french).Date).ToString("yyyy-MM-dd");
                        }

                        rd2.Close();
                    }
                    else
                    {
                        rd2.Close();
                        string query4 = "select regdate,datemodified,reg_date from esociety.society_all_north where socregid=@socregid and socdistrict=@socdistrict";
                        cmd.CommandText = query4;
                        cmd.Parameters.AddWithValue("@socregid", socregid);
                        cmd.Parameters.AddWithValue("@socdistrict", Convert.ToInt32(district));
                        NpgsqlDataReader rd3 = cmd.ExecuteReader();
                        if (rd3.Read())
                        {
                            if (Convert.ToInt32(rd2["datemodified"]) == 0)
                            {                                
                                TxtBxRegistrationDate.Text = (Convert.ToDateTime(rd3["regdate"], french).Date).ToString("yyyy-MM-dd");
                            }
                            else if (Convert.ToInt32(rd2["datemodified"]) == 1)
                            {                                
                                TxtBxRegistrationDate.Text = (Convert.ToDateTime(rd3["reg_date"], french).Date).ToString("yyyy-MM-dd");
                            }

                           
                        }
                        rd3.Close();
                    }

                }

                myTrans.Commit();

            }
            catch (NpgsqlException ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "fetchregdate" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
                myTrans.Rollback();
            }
            finally
            {
                conn.Close();
            }



        }

        protected int fetchfees()
        {
            int retval = 0;
            NpgsqlConnection conn = new NpgsqlConnection();
            NpgsqlCommand cmd = new NpgsqlCommand();
            conn.ConnectionString = ConfigurationManager.ConnectionStrings["connect"].ConnectionString;
            cmd.Connection = conn;        
           

            try
            {
                conn.Open();

                string query1 = "SELECT regdate, lastdateforrenewal, duedate, processfee, penaltyfee, totalfees, remarks FROM esociety.society_renewal_fees where app_id=@app_id";
                cmd.CommandText = query1;
                cmd.Parameters.AddWithValue("@app_id", Convert.ToInt64(Session["app_id"]));
                NpgsqlDataReader rd1 = cmd.ExecuteReader();
                if (rd1.Read())
                {
                   
                    TxtBxRenewalDate.Text = (Convert.ToDateTime(rd1["lastdateforrenewal"], french).Date).ToString("yyyy-MM-dd");
                    TxtbxDueDate.Text = (Convert.ToDateTime(rd1["duedate"], french).Date).ToString("yyyy-MM-dd");
                    TxtBxPenalty.Text = Server.HtmlEncode(rd1["penaltyfee"].ToString());
                    txtbxtotalfees.Text = Server.HtmlEncode(rd1["totalfees"].ToString());
                    txtareafeesremarks.InnerText = Server.HtmlEncode(rd1["remarks"].ToString());

                    int months = Utility.GetMonthDifference(Convert.ToDateTime(rd1["lastdateforrenewal"], french).Date, Convert.ToDateTime(rd1["duedate"], french).Date);
                    TxtBxPenalty.Text = (months * 5).ToString();
                    lkSaveFees.Visible = false;
                    LkCalcFees.Visible = false;
                    TxtBxRenewalDate.Enabled = false;
                    TxtBxRegistrationDate.Enabled = false;
                    TxtbxDueDate.Enabled = false;
                    txtbxtotalfees.Enabled = false;
                    txtareafeesremarks.Disabled = true;
                    retval = 1;
                }
                else
                {
                    lkSaveFees.Visible = true;
                    LkCalcFees.Visible = true;
                    retval = 0;
                }
                rd1.Close();




            }
            catch (NpgsqlException ex)
            {
                retval = 0;
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "fetchfees" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
               
            }
            finally
            {
                conn.Close();
            }

            return retval;

        }

        protected void LkSaveFees_modalbtn_Click(object sender, EventArgs e)
        {

            if (hffees.Value == null || hffees.Value == "")
            {
                Label69.ForeColor = System.Drawing.Color.Red;
                Label69.Text = "Click on Calculate Fees to calculate the total amount";
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "none", "<script> $(window).on('load', function () { $('#MyErrorModal').modal({ backdrop: 'static' });});</script>", false);

            }
            else if (Convert.ToInt32(hffees.Value) != Convert.ToInt32(txtbxtotalfees.Text))
            {
                Label69.ForeColor = System.Drawing.Color.Red;
                Label69.Text = "Total Fees incorrect. Click on calculate button to calculate the total fees";
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "none", "<script> $(window).on('load', function () { $('#MyErrorModal').modal({ backdrop: 'static' });});</script>", false);

            }
            else
            {
                string user_fullname = Session["userfirstname"].ToString() + " " + Session["usermiddlename"].ToString() + " " + Session["userlastname"].ToString();
                string text = Utility.getOldRegistrationNo(Session["app_id"].ToString());
                string[] Result = text.Split('|');
                string socregid = Server.HtmlEncode(Result[0]);

                int a = doValidation(txtareafeesremarks.InnerText, "Fees Remarks");

                if (a == 1)
                {
                    NpgsqlConnection conn = new NpgsqlConnection();
                    NpgsqlCommand cmd = new NpgsqlCommand();
                    conn.ConnectionString = ConfigurationManager.ConnectionStrings["connect"].ConnectionString;
                    cmd.Connection = conn;                 
                  

                    try
                    {
                        conn.Open();

                        string insertQuery = "INSERT INTO esociety.society_renewal_fees(socregid, socname, socdistrict, regdate, lastdateforrenewal, duedate, processfee, penaltyfee, totalfees, app_id, remarks, created_by_name, created_by_email, created_at, created_ipaddress, status)";
                        insertQuery = insertQuery + " VALUES(@socregid, @socname, @socdistrict, @regdate, @lastdateforrenewal, @duedate, @processfee, @penaltyfee, @totalfees, @app_id, @remarks, @created_by_name, @created_by_email, current_timestamp, @ipaddress, 'O')";
                        cmd.CommandText = insertQuery;
                        cmd.Parameters.AddWithValue("@socname", value_society_name.Text);
                        cmd.Parameters.AddWithValue("@socregid", socregid);
                        cmd.Parameters.AddWithValue("@socdistrict", Convert.ToInt32(Session["DistrictID"].ToString()));
                        cmd.Parameters.AddWithValue("@regdate", Convert.ToDateTime(TxtBxRegistrationDate.Text,french).Date);
                        cmd.Parameters.AddWithValue("@lastdateforrenewal", Convert.ToDateTime(TxtBxRenewalDate.Text, french).Date);
                        cmd.Parameters.AddWithValue("@duedate", Convert.ToDateTime(TxtbxDueDate.Text, french).Date);
                        cmd.Parameters.AddWithValue("@processfee", Convert.ToInt32(TxtBxProcessFee.Text));
                        cmd.Parameters.AddWithValue("@penaltyfee", Convert.ToInt32(TxtBxPenalty.Text));
                        cmd.Parameters.AddWithValue("@totalfees", Convert.ToInt32(txtbxtotalfees.Text));
                        cmd.Parameters.AddWithValue("@remarks",txtareafeesremarks.InnerText.Trim());
                        cmd.Parameters.AddWithValue("@app_id", Convert.ToInt64(Session["app_id"].ToString()));
                        cmd.Parameters.AddWithValue("@created_by_name", user_fullname);
                        cmd.Parameters.AddWithValue("@created_by_email", Session["firstname"].ToString());
                        cmd.Parameters.AddWithValue("@ipaddress", ipaddress);                       
                   
                        cmd.ExecuteNonQuery();
                        cmd.Parameters.Clear();                      

                        RecordUserAction("Insert", "Renewal fees Saved", "S", Session["app_id"].ToString(), 1);

                        lkSaveFees.Visible = false;
                        LkCalcFees.Visible = false;
                        TxtBxRenewalDate.Enabled = false;
                        TxtBxRegistrationDate.Enabled = false;
                        TxtbxDueDate.Enabled = false;
                        txtbxtotalfees.Enabled = false;
                        txtareafeesremarks.Disabled = true;
                       
                        Label42.Text = "Fees Saved Successfully!!";
                        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "none", "<script> $(window).on('load', function () { $('#successmodal').modal({ backdrop: 'static' });});</script>", false);




                    }
                    catch (NpgsqlException ex)
                    {
                        CreateLogFiles Err = new CreateLogFiles();
                        Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "LkSaveFees_modalbtn_Click()" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
                      
                        RecordUserAction("Insert", "Error while Saving fees", "F", "NA", 1);

                    }
                    finally
                    {
                        conn.Close();


                    }

                }


            }
        }

        protected void lkSaveFees_Click(object sender, EventArgs e)
        {
            try
            {
                RecordUserAction("Update", "lkSaveFees_Click Clicked on Save Fees Button", "Success", Session["app_id"].ToString(), 2);
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "none", "<script> $(window).on('load', function () { $('#feesconfirmation').modal({ backdrop: 'static' });});</script>", false);

            }
            catch (Exception ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "lkSaveFees_Click()" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
            }
        }

        protected void LkReject_Click(object sender, EventArgs e)
        {
            try
            {
                RecordUserAction("Update", "LkReject_Click Clicked ", "Success", Session["app_id"].ToString(), 2);
                AppIDHD.Value = Session["app_id"].ToString();
                Label44.Text = Session["app_id"].ToString();
                TextBox1.Text = "";
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "none", "<script> $(window).on('load', function () { $('#RejectUploadModal').modal({ backdrop: 'static' });});</script>", false);
            }
            catch (Exception ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "LkReject_Click" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
            }
        }
        public int InsertintoMongoDB(OtherDocuments doc, string sel_collection, Label lb)
        {
            Insert insr = new Insert();
            try
            {
                //RecordUserAction("InsertintoMongoDB", "write document to Mongo", "S");
                return insr.InsertMongoOtherDocs(doc, sel_collection);
            }
            catch (MongoException ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "InsertintoMongoDB()" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
               // RecordUserAction("InsertintoMongoDB", ex.Message, "F");
                lb.Text = "Saved to db failed";
                return 0;
            }
        }

        public int insertentryPosgres(ObjectId obj, long appid, string remarks)
        {
            int value = 0;
            if (Session["app_id"] != null)
            {
               
                if (Session["app_id"].ToString() == appid.ToString())
                {
                    NpgsqlConnection conn = new NpgsqlConnection();
                    NpgsqlCommand cmd = new NpgsqlCommand();
                    conn.ConnectionString = ConfigurationManager.ConnectionStrings["connect"].ConnectionString;
                    cmd.Connection = conn;
                    string objectid = obj.ToString();
                    try
                    {
                        conn.Open();
                        string grm_query = "UPDATE esociety.status_table SET society_reject_mongo = @society_reject_mongo, reject_at = CURRENT_TIMESTAMP,";
                        grm_query = grm_query + " reject_by_name = @reject_by_name, reject_by_email =@reject_by_email, reject_ipaddress = @reject_ipaddress,";
                        grm_query = grm_query + " remarks_reject =@remarks_reject WHERE app_id = @app_id";
                        cmd.CommandText = grm_query;
                        cmd.Parameters.AddWithValue("@society_reject_mongo", objectid);
                        cmd.Parameters.AddWithValue("@app_id", appid);
                        cmd.Parameters.AddWithValue("@reject_by_name", Session["userfirstname"].ToString() + " " + Session["usermiddlename"].ToString() + " " + Session["userlastname"].ToString());
                        cmd.Parameters.AddWithValue("@reject_by_email", Session["firstname"].ToString());
                        cmd.Parameters.AddWithValue("@reject_ipaddress", ipaddress);
                        cmd.Parameters.AddWithValue("@remarks_reject", remarks);
                        cmd.ExecuteNonQuery();
                        value = 1;
                        //RecordUserAction("insertentryPosgres", "objectid saved to posgres", "S");
                    }
                    catch (NpgsqlException ex)
                    {
                        value = 0;
                        CreateLogFiles Err = new CreateLogFiles();
                        Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "insertentryPosgres()" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
                        //RecordUserAction("insertentryPosgres", ex.Message, "F");
                        //var errorcode = ex.Message;
                        //if (errorcode.Contains("23503"))
                        //{
                        //    Response.Write("<script language='javascript'>alert('" + "Please Complete Society Details first." + "')</script>");
                        //}
                        //else
                        //{
                        //    Response.Write("<script language='javascript'>alert('" + "Exection Error" + "')</script>");
                        //}

                    }
                    finally
                    {
                        conn.Close();
                    }
                }
                else
                {
                    value = 0;
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "none", "<script> $(window).on('load', function () { $('#errorModal').modal({ backdrop: 'static' });});</script>", false);
                    Label46.Text = "Execution error";
                }
            }
            return value;
        }
        public int fileuploadfucntion(FileUpload fu, Label lb, OtherDocuments othdoc, string a, string ip, string mac, long App_ID, string collection,string remarks)
        {
            int flag = 0;

            if (Session["app_id"] != null)
            {
                try
                {
                    if (fu.HasFile)
                    {
                        string stringfilesize = ConfigurationManager.AppSettings["FileSize"];
                        int FileSize = Convert.ToInt32(stringfilesize) * 1024 * 1024;
                        HttpPostedFile file = fu.PostedFile;
                        byte[] document = new byte[file.ContentLength];
                        file.InputStream.Read(document, 0, file.ContentLength);
                      //  UInt32 mimetype;
                        //FindMimeFromData(0, null, document, 256, null, 0, out mimetype, 0);
                        //IntPtr mimeTypePtr = new IntPtr(mimetype);
                        //string mimeType = Marshal.PtrToStringUni(mimeTypePtr).ToLower();
                        //Marshal.FreeCoTaskMem(mimeTypePtr);
                        var scanExitCode = 0;//UtilityDAO.VirusScanFile(flupHeader.PostedFile.FileName);
                        if (scanExitCode == 0)
                        {
                            string UploadFileName = file.FileName;
                            string Extension = UploadFileName.Substring(UploadFileName.LastIndexOf('.') + 1).ToLower();
                            var extCount = UploadFileName.Split('.').Length - 1;

                            if (Extension.ToLower() != "pdf" || file.ContentType.ToLower() != "application/pdf")//|| mimeType != "application/pdf"
                            {
                                Label46.Text = "Please upload only Pdf file of maximum size 2MB allowed with extension .pdf!";
                                Label46.ForeColor = System.Drawing.Color.Red;
                                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "none", "<script> $(window).on('load', function () { $('#errorModal').modal({ backdrop: 'static' });});</script>", false);
                                flag = 0;
                            }
                            else if (extCount > 1)
                            {
                                Label46.Text = "File name can not contain multiple dots/extensions.";
                                Label46.ForeColor = System.Drawing.Color.Red;
                                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "none", "<script> $(window).on('load', function () { $('#errorModal').modal({ backdrop: 'static' });});</script>", false);

                                flag = 0;
                            }
                            else if (document == null)
                            {
                                Label46.Text = "Please upload file in pdf format or pdf not in correct format";
                                Label46.ForeColor = System.Drawing.Color.Red;
                                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "none", "<script> $(window).on('load', function () { $('#errorModal').modal({ backdrop: 'static' });});</script>", false);

                                flag = 0;
                            }
                            else if (file.ContentLength <= 0 || file.ContentLength > (FileSize))
                            {
                                Label46.Text = "Only Pdf file of maximum size 2MB allowed !";
                                Label46.ForeColor = System.Drawing.Color.Red;
                                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "none", "<script> $(window).on('load', function () { $('#errorModal').modal({ backdrop: 'static' });});</script>", false);

                                flag = 0;
                            }
                            else
                            {

                                fileSizeFront = fu.FileContent.Length;
                                documentBinary = new byte[fileSizeFront];
                                //Stream fs = fu.PostedFile.InputStream;
                                //BinaryReader br = new BinaryReader(fs);
                                //byte[] bytes = br.ReadBytes((Int32)fs.Length);
                                byte[] bytes = fu.FileBytes;
                                byte[] encrypt_bytes = obj_Byte_Encryption.EncryptData(bytes, System.Web.HttpContext.Current.Server.MapPath("~/ENCRYPT.KEY"));
                                string document_id = Utility.get_OtherDocsID();//other doc ID
                                othdoc.App_ID = App_ID;
                                othdoc.Doc_CT = ct;
                                othdoc.time_stamp = DateTime.Now.ToString();
                                ObjectId obj_id = ObjectId.GenerateNewId();
                                othdoc._Id = obj_id;
                                othdoc.IpAddress = ip;
                                othdoc.MacAddress = mac;
                                othdoc.Active = true;
                                othdoc.UpdatedBy = Session["userfirstname"].ToString() + " " + Session["usermiddlename"].ToString() + " " + Session["userlastname"].ToString();
                                othdoc.Doc_ID = document_id;
                                othdoc.doc_name = a;
                                othdoc.DocContent = encrypt_bytes;
                                int mvalue = InsertintoMongoDB(othdoc, collection, lb);
                                if (mvalue == 1)
                                {
                                    int pvalue = insertentryPosgres(obj_id, App_ID, remarks);
                                    if (pvalue == 1)
                                    {
                                        flag = 1;
                                    }
                                    else { flag = 0; }
                                }
                                else
                                {
                                    flag = 0;
                                }
                            }
                        }
                    }
                    else
                    {
                        lb.ForeColor = System.Drawing.Color.Red;
                        lb.Text = "Upload " + a + " Certificate file in PDF format";
                        Label46.ForeColor = System.Drawing.Color.Red;
                        Label46.Text = "Upload " + a + " Certificate file in PDF format";
                        flag = 0;
                    }

                }
                catch (Exception ex)
                {
                    CreateLogFiles Err = new CreateLogFiles();
                    Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "fileuploadfucntion()" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
                    lb.Text = "File upload failed.";
                    //RecordUserAction("Fileupload Function ", ex.Message, "F");
                    Label46.ForeColor = System.Drawing.Color.Red;
                    Label46.Text = "File upload failed.";
                }
            }
            return flag;
        }


        protected void btnUpload_Click(object sender, EventArgs e)
        {            
            Label46.Text = "";
            if (AppIDHD.Value == Session["app_id"].ToString())
            {
                if (TextBox1.Text == "" || TextBox1.Text == null)
                {
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "none", "<script> $(window).on('load', function () { $('#RejectUploadModal').modal({ backdrop: 'static' });});</script>", false);
                    TextBox1.Focus();
                    Label46.Visible = true;
                    Label46.Text = "Remarks are blank";
                }
                else if(!_val.validateData(TextBox1.Text, _val.reamrks_validation))
                {
                    TextBox1.Focus();
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "none", "<script> $(window).on('load', function () { $('#RejectUploadModal').modal({ backdrop: 'static' });});</script>", false);
                    Label46.Visible = true;
                    Label46.Text = "Remarks are Invalid / having special characters";
                }
                else if(!(FileUpload2.HasFile))
                {
                    FileUpload2.Focus();
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "none", "<script> $(window).on('load', function () { $('#RejectUploadModal').modal({ backdrop: 'static' });});</script>", false);
                    Label46.Visible = true;
                    Label46.Text = "File is not uploaded";
                }
                else
                {
                    int file_upload = fileuploadfucntion(FileUpload2, Label46, doc, "Reject Soceity Document", ipaddress, macaddress, Convert.ToInt64(Session["app_id"].ToString()), "Reject Society", TextBox1.Text);
                    if (file_upload == 1)
                    {
                        NpgsqlConnection conn = new NpgsqlConnection();
                        NpgsqlCommand cmd = new NpgsqlCommand();
                        conn.ConnectionString = ConfigurationManager.ConnectionStrings["connect"].ConnectionString;
                        cmd.Connection = conn;
                        conn.Open();
                        try
                        {
                            string complete_query = "Update esociety.status_table set status_id=9, reject_society = 1 where app_id=@app_id";
                            cmd.CommandText = complete_query;
                            cmd.Parameters.AddWithValue("@app_id", Convert.ToInt64(Session["app_id"].ToString()));
                            cmd.ExecuteNonQuery();
                            Response.Write("<script language='javascript'>alert('" + "File Uploaded Successfully" + "')</script>");
                        }
                        catch (NpgsqlException ex)
                        {
                            CreateLogFiles Err = new CreateLogFiles();
                            Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "btnUpload_Click" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
                           
                            //RecordUserAction("btnSubmit_Click", ex.Message, "F");
                            Response.Write("<script language='javascript'>alert('" + "Error while uploading" + "')</script>");
                        }
                        finally
                        {
                            conn.Close();
                        }
                    }
                    else
                    {

                    }
                }
            }
            else
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), "Mismatch AppID", "btnUpload_Click" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));

            }
        }

        protected void updatefeesinsociety()  // penalty fee is saved in regfee column of society table
        {
            string userloginid = Utility.getUserLoginID(Session["app_id"].ToString());
            NpgsqlConnection conn = new NpgsqlConnection();
            NpgsqlCommand cmd = new NpgsqlCommand();
            conn.ConnectionString = ConfigurationManager.ConnectionStrings["connect"].ConnectionString;
            cmd.Connection = conn;


            try
            {
                conn.Open();

                string updQuery = "Update esociety.society SET regfee=@penaltyfee, processfee=@processfee, totalfee=@totalfee where login_id=@login_id";
                cmd.CommandText = updQuery;               
                cmd.Parameters.AddWithValue("@penaltyfee", Convert.ToInt32(TxtBxPenalty.Text));
                cmd.Parameters.AddWithValue("@processfee", Convert.ToInt32(TxtBxProcessFee.Text));
                cmd.Parameters.AddWithValue("@totalfee", Convert.ToInt32(txtbxtotalfees.Text));               
                cmd.Parameters.AddWithValue("@login_id", userloginid);

                cmd.ExecuteNonQuery();
                cmd.Parameters.Clear();

                RecordUserAction("Insert", "Renewal fees Saved", "S", Session["app_id"].ToString(), 1);


            }
            catch (NpgsqlException ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "updatefees()" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));

                RecordUserAction("Update", "Exception while Updating Society in gridview", "F", "NA", 1);

            }
            finally
            {
                conn.Close();


            }
        }



    }
}
