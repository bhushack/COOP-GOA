using Encryption;
using GoaSocietyRegistration.Development;
using MongoDB.Bson;
using MongoDB.Driver;
using Npgsql;
using NpgsqlTypes;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WS_Encryption;

namespace GoaSocietyRegistration
{
    public partial class MemberDetails : System.Web.UI.Page
    {

        Insert ins = new Insert();
        string ct = string.Empty;
        Int64 fileSizeFront = 0;
        byte[] documentBinary = new Byte[0];

        string ipaddress = Utility.getIP();
        string macaddress = Utility.GetMACAddress(Utility.getIP());
        //string macaddress = Utility.GetMACAddress();
        //int entry_exist;
        //static int edit_gridview_flag = 0;
    
        Validate _val = new Validate();
        OtherDocuments doc = new OtherDocuments();
        NICEncryption _encryption = new NICEncryption();
        ByteEncryption.ByteEncryption obj_Byte_Encryption = new ByteEncryption.ByteEncryption();

        [DllImport(@"urlmon.dll", CharSet = CharSet.Auto)]
        private extern static UInt32 FindMimeFromData(UInt32 pBC, [MarshalAs(UnmanagedType.LPStr)] String pwzUrl, [MarshalAs(UnmanagedType.LPArray)] byte[] pBuffer,
       UInt32 cbSize, [MarshalAs(UnmanagedType.LPStr)] String pwzMimeProposed, UInt32 dwMimeFlags, out UInt32 ppwzMimeOut, UInt32 dwReserverd);

        System.Globalization.CultureInfo french = new System.Globalization.CultureInfo("fr-FR");

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Check4Tampering() || Page.RouteData.RouteHandler == null)
            {
                RecordUserAction("Page_Load", "Access request failed. Tampered session", "F");
                SessionManage session = new SessionManage();
                session.__Abandon(Request, Response);
            }
            else
            {
                get_status();

                get_RenewalStatus();

                Label6.Text = "";//" Note:  Minimum 3  Managing Committee Members are required for a Society.";

                if (!IsPostBack)
                {
                     
                    string key = Encrypt.GenerateRandomKey(10, "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789#$%&*()");
                    var random = Encrypt.GenerateRandomSaltAES();
                    var randomiv = Encrypt.GenerateRandomIvAES();
                    Session["Enc_Random"] = random;
                    Session["Enc_Vector"] = randomiv;

                    HFEdit_flag.Value = "0";
                    Session["ml_flag"] = "0";
                    Session["md_hfvalue"] = "0";
                    FillSalutation();                   
                    FillOccupation();
                    FillProof();

                    if (Convert.ToInt32(Session["Renewal"]) == 2)
                    {
                        FillDesignation_renewal();
                    }
                    else
                    {
                        FillDesignation();
                    }

                    if (Session["login_id"] != null)
                    {
                        HttpContext.Current.Response.AddHeader("Cache-Control", "no-cache, no-store, must-revalidate");
                        HttpContext.Current.Response.AddHeader("Pragma", "no-cache");
                        HttpContext.Current.Response.AddHeader("Expires", "0");
                        txtTotMember.Text = "0";
                        txttotmangcomm.Text = "0";
                        NpgsqlConnection conn = new NpgsqlConnection();
                        NpgsqlCommand cmd = new NpgsqlCommand();
                        string loginid = Session["login_id"].ToString();
                        conn.ConnectionString = ConfigurationManager.ConnectionStrings["connect"].ConnectionString;
                        cmd.Connection = conn;
                        conn.Open(); 
                        try
                        {
                            string query = "SELECT fname,designtaion,occupation,address,proofname,mangcomm,proof_document_no,design,occupatid,proofid";
                            query = query + " member_id,document_mongoentry,salutation,gender,age FROM esociety.members where app_id=@appid";
                            cmd.CommandText = query;
                            cmd.Parameters.AddWithValue("@appid", Convert.ToInt64(Session["AppID"].ToString()));
                            NpgsqlDataReader rd = cmd.ExecuteReader();
                            if (rd.HasRows)
                            {
                                bindgridview(Session["AppID"].ToString());
                                RecordUserAction("MemberDetails_Page_Load", "Member Data Loaded in Gridview", "S");                               
                            }
                            rd.Close();
                        }
                        catch (NpgsqlException ex)
                        {
                            CreateLogFiles Err = new CreateLogFiles();
                            Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "Page_Load()" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
                            RecordUserAction("MemberDetails_Page_Load", "Exception at Member Data", "F");
                            Response.Write("<script language='javascript'>alert('" + "Exception at Member Data" + "')</script>");
                        }
                        finally
                        {
                            conn.Close();
                        }

                        if (Convert.ToInt32(Session["Renewal"]) == 2)
                        {
                            renewhead1.Visible = true;
                            renewhead3.Visible = true;
                            renewhead2.Visible = true;
                           
                            bindgridview_oldmembers(Session["OldAppId"].ToString());
                        }

                        presidententry();

                        ViewState["addoldmember_flag"] = "0";
                    }
                    else
                    {
                        RecordUserAction("MemberDetails_Page_Load", "Session Tampered", "F");
                        Response.Redirect("LoginModule.aspx");
                    }
                }
                var fu2 = getPageStatus(Convert.ToInt64(Session["AppID"].ToString()), 7);
                if (fu2.Item1 == "True")
                {
                    RecordUserAction("Page_load", "File already Uploaded in mongo 2", "S");
                    lbfu1status.ForeColor = System.Drawing.Color.Green;
                    lbfu1status.Text = "File Uploaded!!!";
                    LB_memberslist_upload.Enabled = false;
                    FileUpload1.Enabled = false;
                    LB_memberslist_view.Enabled = true;
                    LB_memberslist_delete.Enabled = true;
                    Session["ml_flag"] = 1;
                }
                else
                {
                    lbfu1status.Text = "No file available";
                    LB_memberslist_upload.Enabled = true;
                    FileUpload1.Enabled = true;
                    Session["ml_flag"] = 0;
                    LB_memberslist_delete.Enabled = false;
                    LB_memberslist_view.Enabled = false;
                }
                            

                if(Convert.ToInt32(HFEdit_flag.Value) == 1 || ViewState["addoldmember_flag"].ToString() == "1")
                {
                    btnAdd.Visible = false;
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
                    if (Context.Session != null && Session["login_id"] != null && Session["DoTAuthTok"] != null && Request.Cookies["DoTAuthTok"] != null)
                    {
                        if (!(Session["DoTAuthTok"].ToString()).Equals(Request.Cookies["DoTAuthTok"].Value))
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

        protected int getheadEntry(string app_id)
        {
            int value = 0;
            NpgsqlConnection conn = new NpgsqlConnection();
            NpgsqlCommand cmd = new NpgsqlCommand();
            conn.ConnectionString = ConfigurationManager.ConnectionStrings["connect"].ConnectionString;
            cmd.Connection = conn;
            conn.Open();
            try
            {
                string query = "Select head_entry from esociety.applicant_details where app_id=@AppID";
                cmd.CommandText = query;
                cmd.Parameters.AddWithValue("@AppID", Convert.ToInt64(app_id));
                NpgsqlDataReader rd = cmd.ExecuteReader();
                if (rd.Read())
                {
                    value = Convert.ToInt16(Server.HtmlEncode(rd["head_entry"].ToString()));
                }
                rd.Close();
            }
            catch (NpgsqlException ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "getheadEntry()" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
                value = -1;           

            }
            finally
            {
                conn.Close();
            }
            return value;
        }

        public int presidententry()
        {
            int a = 0;
            NpgsqlConnection conn = new NpgsqlConnection();
            NpgsqlCommand cmd = new NpgsqlCommand();
            conn.ConnectionString = ConfigurationManager.ConnectionStrings["connect"].ConnectionString;
            cmd.Connection = conn;
            conn.Open();
            NpgsqlTransaction myTrans = conn.BeginTransaction();
            cmd.Transaction = myTrans;
            try
            {
                if (Session["AppID"] != null)
                {
                    string query = "Select head_entry,applicant_name, applicant_address, applicant_designation from esociety.applicant_details where app_id=@AppID";
                    cmd.CommandText = query;
                    cmd.Parameters.AddWithValue("@AppID", Convert.ToInt64(Session["AppID"].ToString()));
                    NpgsqlDataReader rd = cmd.ExecuteReader();
                    if (rd.Read())
                    {
                       int  entry_exist = Convert.ToInt16(Server.HtmlEncode(rd["head_entry"].ToString()));
                        if (entry_exist == 0)
                        {
                            txt_MemName.Text = Server.HtmlEncode(rd["applicant_name"].ToString());
                            txt_MemName.Enabled = false;
                            txt_MemAddress.Text = Server.HtmlEncode(rd["applicant_address"].ToString());
                            txt_MemAddress.Enabled = false;
                            ddl_DesignMem.SelectedValue = Server.HtmlEncode(rd["applicant_designation"].ToString());
                            ddl_DesignMem.Enabled = false;
                            ddlMcom.Enabled = false;
                            btnAddOld.Visible = false;
                            if (Convert.ToInt32(Session["Renewal"]) == 2)
                            {
                                tr_dateofadmission.Visible = true;
                            }
                            else
                            {
                                tr_dateofadmission.Visible = false;
                            }
                            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "none", "<script> $(window).on('load', function () { $('#addmembersmodal').modal({ backdrop: 'static' });});</script>", false);
                            a = 0;
                        }
                        else
                        {
                            a = 1;
                        }
                    }
                    rd.Close();
                    cmd.Parameters.Clear();
                    
                    myTrans.Commit();
                }
                else
                {
                    RecordUserAction("presidententry()", "Session null", "F");
                }

            }
            catch (NpgsqlException ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "presidententry()" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
                myTrans.Rollback();
                RecordUserAction("presidententry()", ex.Message, "F");

            }
            finally
            {
                conn.Close();
            }
            return a;
        }
        public void get_status()
        {
            try
            {
                if (Convert.ToInt32(Session["status_Id"]) == 1 || Convert.ToInt32(Session["status_Id"]) == 2 || Convert.ToInt32(Session["status_Id"]) == 5)
                {

                    btnAdd.Visible = true;
                    Button1.Visible = true;
                    listupload.Visible = true;
                    listdelete.Visible = true;
                    FileUpload1.Enabled = true;
                    //SocietyDetailsBtn.Enabled = false;
                    //btnupdate.Enabled = false;

                }
                else
                {
                    btnAdd.Visible = false;
                    Button1.Visible = false;
                    listupload.Visible = false;
                    FileUpload1.Enabled = false;
                    listdelete.Visible = false;
                    FileUpload1.Enabled = false;

                }
            }
            catch (Exception ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "get_status()" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
            }
        }

        public void get_RenewalStatus()
        {
            try
            {
                if ((Convert.ToInt32(Session["status_Id"]) == 1 | Convert.ToInt32(Session["status_Id"]) == 2 || Convert.ToInt32(Session["status_Id"]) == 5) && Utility.checkifrenewal(Session["AppID"].ToString()) == 2 && Session["OldAppId"].ToString() != "" && getHeadEntry(Session["AppID"].ToString()) == 1)
                {
                    oldmembersgridview.Visible = true;
                    Oldmembers_tr.Visible = true;
                    btnAddOld.Visible = true;
                   
                }
                else
                {
                    oldmembersgridview.Visible = false;
                    Oldmembers_tr.Visible = false;
                    btnAddOld.Visible = false;
                   
                }
            }
            catch (Exception ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "get_RenewalStatus()" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
            }
        }


        private void RecordUserAction(string action, string description, string status)
        {
            /*Audit trail*/
            int count = 0;
            do
            {
                System.Web.HttpBrowserCapabilities browser = Request.Browser;
                UsersAuditTrails trail = new UsersAuditTrails();
                string uri = HttpContext.Current.Request.Url.AbsoluteUri;
                if (Session["login_id"] != null)
                {
                    trail.user_login_id = Session["login_id"].ToString();
                    trail.loggedin_status = "Y";
                }
                else
                {

                    trail.user_login_id = DBNull.Value.ToString();
                    trail.loggedin_status = "N";
                }
                trail.browser_session_id = HttpContext.Current.Request.Cookies[GlobalVars.AntiXsrfTokenKey] == null ? HttpContext.Current.Session.SessionID : HttpContext.Current.Request.Cookies[GlobalVars.AntiXsrfTokenKey].Value;
                trail.user_session_id = Session["loginsession"] != null ? Session["loginsession"].ToString() : "null";
                trail.referrer = uri != null ? uri.ToString().Length > 100 ? uri.ToString().Substring(0, 100) : uri.ToString() : string.Empty;
                trail.accessed_module = Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath);
                trail.action_performed = action;
                trail.action_description = description.Length > 200 ? description.Substring(0, 200) : description;
                trail.action_status = status;
                trail.tracked_datetime = DateTime.Now;
                trail.loggedin_ip = ipaddress;
                trail.loggedin_mac = macaddress;
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
                count = SaveAuditTrail(trail);
            } while (count == 0);
            /*Audit trail*/
        }
        public int SaveAuditTrail(UsersAuditTrails trial)
        {
            NpgsqlConnection conn = new NpgsqlConnection();
            NpgsqlCommand cmd = new NpgsqlCommand();
            conn.ConnectionString = ConfigurationManager.ConnectionStrings["connect"].ConnectionString;
            cmd.Connection = conn;
            try
            {
                conn.Open();
                string query = "INSERT INTO esociety.user_audit_trail( user_login_id,browser_session_id, user_session_id, loggedin_status, referrer, accessed_module,";
                query = query + " action_performed, action_description, action_status, ipaddress, macaddress,  tracked_datetime, browser_name,browser_version,device_type ) VALUES (@user_login_id,@browser_session_id, ";
                query = query + " @user_session_id, @loggedin_status, @referrer, @accessed_module, @action_performed, @action_description, @action_status, @ipaddress, @macaddress,";
                query = query + "  @tracked_datetime,@browser_name,@browser_version,@device_type)";
                cmd.CommandText = query;
                cmd.Parameters.AddWithValue("@user_login_id", trial.user_login_id);
                cmd.Parameters.AddWithValue("@browser_session_id", trial.browser_session_id);
                cmd.Parameters.AddWithValue("@user_session_id", trial.user_session_id);
                cmd.Parameters.AddWithValue("@loggedin_status", trial.loggedin_status);
                cmd.Parameters.AddWithValue("@referrer", trial.referrer);
                cmd.Parameters.AddWithValue("@accessed_module", trial.accessed_module);
                cmd.Parameters.AddWithValue("@action_performed", trial.action_performed);
                cmd.Parameters.AddWithValue("@action_description", trial.action_description);
                cmd.Parameters.AddWithValue("@action_status", trial.action_status);
                cmd.Parameters.AddWithValue("@ipaddress", trial.loggedin_ip);
                cmd.Parameters.AddWithValue("@macaddress", trial.loggedin_mac);
                cmd.Parameters.AddWithValue("@tracked_datetime", trial.tracked_datetime);
                cmd.Parameters.AddWithValue("@browser_name", trial.browser_name);
                cmd.Parameters.AddWithValue("@browser_version", trial.browser_version);
                cmd.Parameters.AddWithValue("@device_type", trial.device_type);
                cmd.ExecuteNonQuery();
                return 1;
            }
            catch (NpgsqlException ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "SaveAuditTrail()" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
                return 0;
            }
            finally
            {
                conn.Close();
            }

        }


        public void bindgridview_oldmembers(string OldAppId)
        {
            if (OldAppId != "" && OldAppId != null)
            {
                NpgsqlConnection conn = new NpgsqlConnection();
                NpgsqlCommand cmd = new NpgsqlCommand();
                conn.ConnectionString = ConfigurationManager.ConnectionStrings["connect"].ConnectionString;
                cmd.Connection = conn;
                try
                {
                    conn.Open();
                    string query1 = "SELECT app_id,salutation,fname,gender,age,case when (design='8') then concat('Other -' , designtaion_others) else designtaion end as designtaion,design,case when (occupatid='4') then concat('Others - ' , occupation_others) else occupation end as occupation,occupatid,address,proofname,proofid,mangcomm,created_at,created_by,ipaddress,";
                    query1 = query1 + " macaddress,member_id,active,document_mongoentry,doc_id,proof_document_no,salutation_id,designtaion_others,occupation_others,dateofadmission FROM esociety.members where app_id=@appid and active='Y'";
                    cmd.CommandText = query1;
                    cmd.Parameters.AddWithValue("@appid", Convert.ToInt64(OldAppId));
                    NpgsqlDataAdapter da = new NpgsqlDataAdapter(cmd);
                    DataSet ds = new DataSet();
                    da.Fill(ds);
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        grvOldMemberDetails.DataSource = ds;
                        grvOldMemberDetails.DataBind();

                        RecordUserAction("bindgridview_oldmembers", "Old Members Data Loaded", "S");
                    }

                }
                catch (NpgsqlException ex)
                {
                    CreateLogFiles Err = new CreateLogFiles();
                    Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "bindgridview_oldmembers()" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
                    RecordUserAction("bindgridview_oldmembers", ex.Message, "F");
                    Response.Write("<script language='javascript'>alert('" + "Exception in bindgridview_oldmembers" + "')</script>");
                }
                finally
                {
                    conn.Close();
                }
            }
        }
        public void bindgridview(string AppID)
        {
            NpgsqlConnection conn = new NpgsqlConnection();
            NpgsqlCommand cmd = new NpgsqlCommand();
            conn.ConnectionString = ConfigurationManager.ConnectionStrings["connect"].ConnectionString;
            cmd.Connection = conn;
            try
            {
                conn.Open();
                string query = "SELECT app_id,salutation,fname,gender,age,case when (design='8') then concat('Other -' , designtaion_others) else designtaion end as designtaion,design,case when (occupatid='4') then concat('Others - ' , occupation_others) else occupation end as occupation,occupatid,address,proofname,proofid,mangcomm,member_id,proof_document_no,document_mongoentry,remarks,dateofadmission FROM esociety.members where app_id=@appid and active='Y' and mangcomm='Yes' order by design ASC";
                cmd.CommandText = query;
                cmd.Parameters.AddWithValue("@appid", Convert.ToInt64(AppID));
                NpgsqlDataAdapter da = new NpgsqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                da.Fill(ds);
                if (ds.Tables[0].Rows.Count > 0)
                {
                    grvMemberDetails.DataSource = ds;
                    grvMemberDetails.DataBind();
                    txtTotMember.Text = Server.HtmlEncode(ds.Tables[0].Rows.Count.ToString());
                                        
                    RecordUserAction("MemberDetails_Gridview", "GridView Data Loaded", "S");
                }

                string query1 = "SELECT app_id,salutation,fname,gender,age,case when (design='8') then concat('Other -' , designtaion_others) else designtaion end as designtaion,design,case when (occupatid='4') then concat('Others - ' , occupation_others) else occupation end as occupation,occupatid,address,proofname,proofid,mangcomm,member_id,proof_document_no,document_mongoentry,remarks, dateofadmission FROM esociety.members where app_id=@appid and active='Y' and mangcomm='Yes' order by design ASC";
                cmd.CommandText = query1;
                cmd.Parameters.AddWithValue("@appid", Convert.ToInt64(AppID));
                NpgsqlDataAdapter da1 = new NpgsqlDataAdapter(cmd);
                DataSet ds1 = new DataSet();
                da1.Fill(ds1);
                if (ds1.Tables[0].Rows.Count > 0)
                {
                    gv_managingcomm.DataSource = ds1;
                    gv_managingcomm.DataBind();
                    txttotmangcomm.Text = Server.HtmlEncode(ds1.Tables[0].Rows.Count.ToString());

                    RecordUserAction("ManagingCommittee_Gridview", "GridView Data Loaded", "S");
                }

            }
            catch (NpgsqlException ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "bindgridview()" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
                RecordUserAction("bindgridview", ex.Message, "F");
                Response.Write("<script language='javascript'>alert('" + "Exception in Gridview" + "')</script>");
            }
            finally
            {
                conn.Close();
            }
            txt_MemName.Enabled = true;
            txt_MemAddress.Enabled = true;
            ddl_DesignMem.Enabled = true;
            //ddlMcom.Enabled = fal;

        }
        protected void ddl_DesignMem_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                //ddlMcom.SelectedValue = "1";
                //ddlMcom.Enabled = false;

                if (ddl_DesignMem.SelectedValue == "8")  // for others
                {
                    row_DesignOthers.Visible = true;                    
                }
                else
                {
                    row_DesignOthers.Visible = false;
                    TxtBx_DesignOthers.Text = "";
                }

                if (Convert.ToInt32(HFEdit_flag.Value) == 1)
                {
                    btnAdd.Visible = false;
                    btnAddOld.Visible = false;
                    btnUpdate.Visible = true;
                }
                else if (ViewState["addoldmember_flag"].ToString() == "1")
                {
                    btnAddOld.Visible = true;
                    btnAdd.Visible = false;
                    btnUpdate.Visible = false;
                }
                else
                {
                    btnAdd.Visible = true;
                    btnAddOld.Visible = false;
                    btnUpdate.Visible = false;
                }

                if(ddl_DesignMem.SelectedValue == "8" || ddl_DesignMem.SelectedValue == "7")
                {
                    ddlMcom.SelectedValue = "1";////////////////////////////////////////////////////////////////////////////////////////////////// ddlMcom.SelectedValue = "2"; chnaged from 2 to 1
                }
               

                check_entry(Convert.ToInt32(ddl_DesignMem.SelectedValue));

                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "none", "<script> $(window).on('load', function () { $('#addmembersmodal').modal({ backdrop: 'show' });});</script>", false);

            }
            catch (Exception ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "ddl_DesignMem_SelectedIndexChanged" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
            }
        }
        public void FillDesignation()
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
                string query = "SELECT \"DesignationName\",\"DesignationID\" FROM esociety.mst_memberdesignation where \"DesignationID\"!=9";
                cmd.CommandText = query;
                adapter = new NpgsqlDataAdapter(cmd);
                ds = new DataSet();
                adapter.Fill(ds, "esociety.mst_memberdesignation");
                ddl_DesignMem.DataSource = ds.Tables[0];
                ddl_DesignMem.DataTextField = "DesignationName";
                ddl_DesignMem.DataValueField = "DesignationID";
                ddl_DesignMem.DataBind();
                ddl_DesignMem.Items.Insert(0, new ListItem("-- Select --", "-1"));
                cmd.Dispose();
                adapter.Dispose();
                ds = null;
            }
            catch (NpgsqlException ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "FillDesignation()" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
                RecordUserAction("FillDesignation", ex.Message, "F");
                Response.Write("<script language='javascript'>alert('Designation DropDown:" + "Error" + "')</script>");
            }
            finally
            {
                connect.Close();
            }

        }


        public void FillDesignation_renewal()
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
                string query = "SELECT \"DesignationName\",\"DesignationID\" FROM esociety.mst_memberdesignation";
                cmd.CommandText = query;
                adapter = new NpgsqlDataAdapter(cmd);
                ds = new DataSet();
                adapter.Fill(ds, "esociety.mst_memberdesignation");
                ddl_DesignMem.DataSource = ds.Tables[0];
                ddl_DesignMem.DataTextField = "DesignationName";
                ddl_DesignMem.DataValueField = "DesignationID";
                ddl_DesignMem.DataBind();
                ddl_DesignMem.Items.Insert(0, new ListItem("-- Select --", "-1"));
                cmd.Dispose();
                adapter.Dispose();
                ds = null;
            }
            catch (NpgsqlException ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "FillDesignation_renewal()" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
                RecordUserAction("FillDesignation", ex.Message, "F");
                Response.Write("<script language='javascript'>alert('Designation DropDown:" + "Error" + "')</script>");
            }
            finally
            {
                connect.Close();
            }

        }

        public void FillProof()
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
                string query = "SELECT \"ProofName\",\"ProofID\" FROM esociety.mst_idproof";
                cmd.CommandText = query;
                adapter = new NpgsqlDataAdapter(cmd);
                ds = new DataSet();
                adapter.Fill(ds, "esociety.mst_idproof");
                ddl_MemDocType.DataSource = ds.Tables[0];
                ddl_MemDocType.DataTextField = "ProofName";
                ddl_MemDocType.DataValueField = "ProofID";
                ddl_MemDocType.DataBind();
                ddl_MemDocType.Items.Insert(0, new ListItem("-- Select --", "-1"));
                cmd.Dispose();
                adapter.Dispose();
                ds = null;
            }
            catch (NpgsqlException ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "FillProof()" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
                RecordUserAction("FillProof", ex.Message, "F");
                Response.Write("<script language='javascript'>alert('ID Proof DropDown:" + "error" + "')</script>");
            }
            finally
            {
                connect.Close();
            }
        }
        public void FillOccupation()

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
                string query = "SELECT \"OccupationName\",\"OccupationID\" FROM esociety.mst_occupation";
                cmd.CommandText = query;
                adapter = new NpgsqlDataAdapter(cmd);
                ds = new DataSet();
                adapter.Fill(ds, "esociety.mst_occupation");
                ddl_MemOccup.DataSource = ds.Tables[0];
                ddl_MemOccup.DataTextField = "OccupationName";
                ddl_MemOccup.DataValueField = "OccupationID";
                ddl_MemOccup.DataBind();
                ddl_MemOccup.Items.Insert(0, new ListItem("-- Select --", "-1"));
                cmd.Dispose();
                adapter.Dispose();
                ds = null;
            }
            catch (NpgsqlException ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "FillOccupation()" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
                RecordUserAction("FillOccupation", ex.Message, "F");
                Response.Write("<script language='javascript'>alert('Occupation DropDown:" + "Error" + "')</script>");
            }
            finally
            {
                connect.Close();
            }
        }

        public void FillSalutation()
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
                string query = "SELECT salutation_name,salutation_id FROM esociety.mst_salutation";
                cmd.CommandText = query;
                adapter = new NpgsqlDataAdapter(cmd);
                ds = new DataSet();
                adapter.Fill(ds, "esociety.mst_salutation");
                ddl_salutation.DataSource = ds.Tables[0];
                ddl_salutation.DataTextField = "salutation_name";
                ddl_salutation.DataValueField = "salutation_id";
                ddl_salutation.DataBind();
                ddl_salutation.Items.Insert(0, new ListItem("-- Select --", "-1"));
                cmd.Dispose();
                adapter.Dispose();
                ds = null;
            }
            catch (NpgsqlException ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "FillSalutation()" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
                RecordUserAction("FillSalutation", ex.Message, "F");
                Response.Write("<script language='javascript'>alert('Salutation DropDown:" + "Error" + "')</script>");
            }
            finally
            {
                connect.Close();
            }
        }

        protected void btnAdd_Click(object sender, EventArgs e)
        {
            string doc_no = Encrypt.DecryptStringAES(hfencryptNo.Value.ToString(), Session["Enc_Random"].ToString(), Session["Enc_Vector"].ToString());

            if (Session["AppID"] != null && Session["created_by"] != null)
            {               
                if (ddl_salutation.SelectedValue == "-1")
                {
                    lblError.Text = "Please select Salutation!";
                    relaunchmodal();
                }
                else if (txt_MemName.Text == "" || txt_MemName.Text == null)
                {
                    lblError.Text = "Please Enter Name!";
                    relaunchmodal();
                }
                else if (txtbx_age.Text == "" || txtbx_age.Text == null)
                {
                    lblError.Text = "Please Enter Age!";
                    relaunchmodal();
                }
                else if (!_val.validateData(txt_MemName.Text, _val.alpharegex1))
                {
                    lblError.Text = "Please Enter Valid Name!";
                    relaunchmodal();
                }
                else if (!_val.validateData(txtbx_age.Text, _val.captcha_regex))
                {
                    lblError.Text = "Please Enter Valid Age!";
                    relaunchmodal();
                }
                else if (Int32.Parse(txtbx_age.Text) < 18)
                {
                    lblError.Text = "Age should be greater than 18!";
                    relaunchmodal();
                }
                else if (Int32.Parse(txtbx_age.Text) > 115)
                {
                    lblError.Text = "Age cannot be greater than 115!";
                    relaunchmodal();
                }
                else if (txt_MemAddress.Text == "" || txt_MemAddress.Text == null)
                {
                    lblError.Text = "Please Enter Address!";
                    relaunchmodal();
                }
                else if (!_val.validateData(txt_MemAddress.Text, _val.reamrks_validation))
                {
                    lblError.Text = "Please Enter Valid Address!";
                    relaunchmodal();
                }

                else if (ddl_DesignMem.SelectedValue == "-1")
                {
                    lblError.Text = "Please select Designation of the member!";
                    relaunchmodal();
                }
                else if (ddl_DesignMem.SelectedValue == "8" && (TxtBx_DesignOthers.Text == "" || TxtBx_DesignOthers.Text == null))
                {
                    lblError.Text = "Please Enter Other Designation!";
                    relaunchmodal();
                }
                else if (ddl_DesignMem.SelectedValue == "8" && (!_val.validateData(TxtBx_DesignOthers.Text, _val.alpharegex)))
                {
                    lblError.Text = "Please Enter Valid Designation!";
                    relaunchmodal();
                }
                else if (ddl_MemOccup.SelectedValue == "-1")
                {
                    lblError.Text = "Please Enter Occupation!";
                    relaunchmodal();
                }
                else if (ddl_MemOccup.SelectedValue == "4" && (TxtBx_OccupOthers.Text == "" || TxtBx_OccupOthers.Text == null))
                {
                    lblError.Text = "Please Enter Other Occupation!";
                    relaunchmodal();
                }
                else if (ddl_MemOccup.SelectedValue == "4" && (!_val.validateData(TxtBx_OccupOthers.Text, _val.alpharegex)))
                {
                    lblError.Text = "Please Enter Valid Occupation!";
                    relaunchmodal();
                }
                else if (ddl_MemDocType.SelectedValue == "-1")
                {
                    lblError.Text = "Please select Type of document to be added!";
                    relaunchmodal();
                }
                else if (doc_no == "" || doc_no == null)
                {
                    lblError.Text = "Please Enter document no !";
                    relaunchmodal();
                }
                else if (ddlMcom.SelectedValue == "0")
                {
                    lblError.Text = "Please select part of Managing Committee or not";
                    relaunchmodal();
                }
                else if (ddl_MemDocType.SelectedValue == "1" && (!_val.validateData(doc_no, _val.epicno)))
                {
                    lblError.Text = "Please Enter valid election card no";
                    relaunchmodal();
                }
                else if (ddl_MemDocType.SelectedValue == "2" && (!_val.validateData(doc_no, _val.cust_driving)))
                {
                    lblError.Text = "Please Enter valid Driving License no";
                    relaunchmodal();
                }
                else if (ddl_MemDocType.SelectedValue == "3" && (!_val.validateData(doc_no, _val.pan_regex)))
                {
                    lblError.Text = "Please Enter valid Pan card no";
                    relaunchmodal();
                }
                else if (ddl_MemDocType.SelectedValue == "4" && (!_val.validateData(doc_no, _val.passport)))
                {
                    lblError.Text = "Please Enter valid Passport no";
                    relaunchmodal();
                }
                else if (ddl_MemDocType.SelectedValue == "6" && (!_val.validateData(doc_no, _val.alpha_numericregex))) //for others
                {
                    lblError.Text = "Please Enter valid Identity Proof no";
                    relaunchmodal();
                }
                else if (ddl_MemDocType.SelectedValue == "5" && (!_val.validateData(doc_no, _val.adhar_regex)))
                {
                    lblError.Text = "Please Enter valid Aadhar Card no";
                    relaunchmodal();
                }
                else if (ddl_DesignMem.SelectedValue != "7" && check_entry(Convert.ToInt32(ddl_DesignMem.SelectedValue)) == 1)
                {
                    lblError.Text = "Designation is held by some other Member of Society!";
                    relaunchmodal();
                }
                else if(Convert.ToInt32(Session["Renewal"]) == 2 && (txtbxdateadmission.Text == null ||txtbxdateadmission.Text == ""))
                {
                    lblError.Text = "Date of admission is blank";
                    relaunchmodal();
                }                
                else
                {
                    lblError.Text = "";
                    int a = 0;
                    Int64 mid = Utility.get_memberID();
                    MemberDocs document = new MemberDocs();
                    string doc_name = ddl_MemDocType.SelectedItem.Text;
                    a = fileuploadfucntion(FileUpload6, Status, document, doc_name, ipaddress, macaddress, Convert.ToInt64(Session["AppID"].ToString()), "Members Document", mid);

                    NpgsqlConnection connect = new NpgsqlConnection();
                    NpgsqlCommand cmd = new NpgsqlCommand();
                    connect.ConnectionString = ConfigurationManager.ConnectionStrings["connect"].ConnectionString;
                    cmd.Connection = connect;
                    string query = "";
                    connect.Open();
                    NpgsqlTransaction tran = connect.BeginTransaction();
                    try
                    {                        
                        if (a == 1)
                        {                            
                            cmd.Parameters.Clear();
                            int proofid = Convert.ToInt32(ddl_MemDocType.SelectedValue);
                            if (proofid == 5)
                            {
                                string encrypt_docno = _encryption.Encrypt(doc_no, System.Web.HttpContext.Current.Server.MapPath("~/ENCRYPT.KEY"));
                                var str = ConfigurationManager.AppSettings["aadharvault"];
                                IMongoDatabase database;
                                IMongoClient client;
                                client = new MongoClient(str);
                                Aadhar adr = new Aadhar();
                                adr.App_ID = Session["AppID"].ToString();
                                adr.Member_ID = mid.ToString();
                                ObjectId objectID = ObjectId.GenerateNewId();
                                adr._Id = objectID;
                                adr.Doc_ID = encrypt_docno;
                                database = client.GetDatabase("eSocietyAadharVault");
                                var collection = database.GetCollection<Aadhar>("aadhjarVault");
                                collection.InsertOne(adr);
                                query = "Update esociety.members set fname=@fname, designtaion=@designtaion, design=@design, occupation=@occupation, occupatid=@occupatid, address=@address, proofname=@proofname,";
                                query = query + " proofid =@proofid, mangcomm=@mangcomm,created_at=current_timestamp, created_by=@created_by, ipaddress=@ipaddress, macaddress=@macaddress, active='Y',proof_document_no=@proof_document_no,";
                                query = query + " salutation_id =@salutation_id,salutation=@salutation,gender=@gender,age=@age,designtaion_others=@designtaion_others,occupation_others=@occupation_others,remarks=@remarks,dateofadmission=@dateofadmission,";
                                query = query + " flag_aadhar = 'C' where app_id=@app_id and member_id=@mid";
                                cmd.CommandText = query;
                                cmd.Parameters.AddWithValue("@mid", mid);
                                cmd.Parameters.AddWithValue("@app_id", Convert.ToInt64(Session["AppID"].ToString()));
                                cmd.Parameters.AddWithValue("@fname", txt_MemName.Text);
                                cmd.Parameters.AddWithValue("@designtaion", ddl_DesignMem.SelectedItem.Text);
                                cmd.Parameters.AddWithValue("@design", Convert.ToInt32(ddl_DesignMem.SelectedValue));
                                cmd.Parameters.AddWithValue("@occupation", ddl_MemOccup.SelectedItem.Text);
                                cmd.Parameters.AddWithValue("@occupatid", Convert.ToInt32(ddl_MemOccup.SelectedValue));
                                cmd.Parameters.AddWithValue("@address", txt_MemAddress.Text);
                                cmd.Parameters.AddWithValue("@proofname", ddl_MemDocType.SelectedItem.Text);
                                cmd.Parameters.AddWithValue("@proofid", Convert.ToInt32(ddl_MemDocType.SelectedValue));
                                cmd.Parameters.AddWithValue("@mangcomm", ddlMcom.SelectedItem.Text.Trim());
                                cmd.Parameters.AddWithValue("@created_by", Session["created_by"].ToString());
                                cmd.Parameters.AddWithValue("@ipaddress", ipaddress);
                                cmd.Parameters.AddWithValue("@macaddress", macaddress);
                                cmd.Parameters.AddWithValue("@salutation_id", Convert.ToInt32(ddl_salutation.SelectedValue));
                                cmd.Parameters.AddWithValue("@salutation", ddl_salutation.SelectedItem.Text);
                                cmd.Parameters.AddWithValue("@gender", Rdbtngender.SelectedValue);
                                cmd.Parameters.AddWithValue("@age", Convert.ToInt32(txtbx_age.Text));                                
                                cmd.Parameters.AddWithValue("@proof_document_no", objectID.ToString());
                                cmd.Parameters.AddWithValue("@designtaion_others", TxtBx_DesignOthers.Text);
                                cmd.Parameters.AddWithValue("@occupation_others", TxtBx_OccupOthers.Text);
                                cmd.Parameters.AddWithValue("@remarks", TxtBxRemarks.Text);
                                if (Convert.ToInt32(Session["Renewal"]) == 2)
                                {
                                    cmd.Parameters.AddWithValue("@dateofadmission", Convert.ToDateTime(txtbxdateadmission.Text, french).Date);
                                }
                                else
                                {
                                    cmd.Parameters.AddWithValue("@dateofadmission", DBNull.Value);
                                }
                                cmd.ExecuteNonQuery();
                                cmd.Parameters.Clear();
                            }
                            else
                            {
                                query = "Update esociety.members set fname=@fname, designtaion=@designtaion, design=@design, occupation=@occupation, occupatid=@occupatid, address=@address, proofname=@proofname,";
                                query = query + " proofid =@proofid, mangcomm=@mangcomm,created_at=current_timestamp, created_by=@created_by, ipaddress=@ipaddress, macaddress=@macaddress, active='Y',proof_document_no=@proof_document_no,";
                                query = query + " salutation_id =@salutation_id,salutation=@salutation,gender=@gender,age=@age,designtaion_others=@designtaion_others,occupation_others=@occupation_others,remarks=@remarks,dateofadmission=@dateofadmission where app_id=@app_id and member_id=@mid";
                                cmd.CommandText = query;
                                cmd.Parameters.AddWithValue("@mid", mid);
                                cmd.Parameters.AddWithValue("@app_id", Convert.ToInt64(Session["AppID"].ToString()));
                                cmd.Parameters.AddWithValue("@fname", txt_MemName.Text);
                                cmd.Parameters.AddWithValue("@designtaion", ddl_DesignMem.SelectedItem.Text);
                                cmd.Parameters.AddWithValue("@design", Convert.ToInt32(ddl_DesignMem.SelectedValue));
                                cmd.Parameters.AddWithValue("@occupation", ddl_MemOccup.SelectedItem.Text);
                                cmd.Parameters.AddWithValue("@occupatid", Convert.ToInt32(ddl_MemOccup.SelectedValue));
                                cmd.Parameters.AddWithValue("@address", txt_MemAddress.Text);
                                cmd.Parameters.AddWithValue("@proofname", ddl_MemDocType.SelectedItem.Text);
                                cmd.Parameters.AddWithValue("@proofid", Convert.ToInt32(ddl_MemDocType.SelectedValue));
                                cmd.Parameters.AddWithValue("@mangcomm", ddlMcom.SelectedItem.Text.Trim());
                                cmd.Parameters.AddWithValue("@created_by", Session["created_by"].ToString());
                                cmd.Parameters.AddWithValue("@ipaddress", ipaddress);
                                cmd.Parameters.AddWithValue("@macaddress", macaddress);
                                cmd.Parameters.AddWithValue("@salutation_id", Convert.ToInt32(ddl_salutation.SelectedValue));
                                cmd.Parameters.AddWithValue("@salutation", ddl_salutation.SelectedItem.Text);
                                cmd.Parameters.AddWithValue("@gender", Rdbtngender.SelectedValue);
                                cmd.Parameters.AddWithValue("@age", Convert.ToInt32(txtbx_age.Text));
                                string encrypt_docno = _encryption.Encrypt(doc_no, System.Web.HttpContext.Current.Server.MapPath("~/ENCRYPT.KEY"));
                                cmd.Parameters.AddWithValue("@proof_document_no", encrypt_docno);
                                cmd.Parameters.AddWithValue("@designtaion_others", TxtBx_DesignOthers.Text);
                                cmd.Parameters.AddWithValue("@occupation_others", TxtBx_OccupOthers.Text);
                                cmd.Parameters.AddWithValue("@remarks", TxtBxRemarks.Text);
                                if (Convert.ToInt32(Session["Renewal"]) == 2)
                                {
                                    cmd.Parameters.AddWithValue("@dateofadmission", Convert.ToDateTime(txtbxdateadmission.Text, french).Date);
                                }
                                else
                                {
                                    cmd.Parameters.AddWithValue("@dateofadmission", DBNull.Value);
                                }
                                cmd.ExecuteNonQuery();
                                cmd.Parameters.Clear();
                            }
                            
                            grvMemberDetails.Visible = true;
                            bindgridview(Session["AppID"].ToString());
                            RecordUserAction("btnAdd_Click", "Member Data Added Successfully", "S");
                            txt_MemAddress.Text = "";
                            txt_MemName.Text = "";
                            ddl_MemOccup.SelectedValue = "-1";
                            ddl_MemDocType.SelectedValue = "-1";
                            ddl_DesignMem.SelectedValue = "-1";
                            TxtBx_DesignOthers.Text = "";
                            TxtBx_OccupOthers.Text = "";
                            TxtBxDocumentNo.Text = "";
                            ddl_salutation.SelectedValue = "-1";
                            Rdbtngender.ClearSelection();
                            //txtbx_age.Text = "";
                            cmd.Parameters.Clear();

                            string ins_master = "INSERT into esociety.master_members_table(member_id, app_id) VALUES (@member_id,@app_id)";
                            cmd.CommandText = ins_master;
                            cmd.Parameters.AddWithValue("@app_id", Convert.ToInt64(Session["AppID"].ToString()));
                            cmd.Parameters.AddWithValue("@member_id", mid);
                            cmd.ExecuteNonQuery();
                            cmd.Parameters.Clear();
                            int entry_exist = getheadEntry(Session["AppID"].ToString());
                            if (entry_exist == 0)
                            {
                                string query1 = "update esociety.applicant_details set head_entry= @head_entry where app_id = @app_id";
                                cmd.CommandText = query1;
                                cmd.Parameters.AddWithValue("@app_id", Convert.ToInt64(Session["AppID"].ToString()));
                                cmd.Parameters.AddWithValue("@head_entry", 1);
                                cmd.ExecuteNonQuery();
                                get_RenewalStatus();
                            }
                            else
                            {
                                get_RenewalStatus();
                            }
                            cmd.Parameters.Clear();
                            tran.Commit();                         
                            bindgridview(Session["AppID"].ToString());
                        }
                    }
                    catch (NpgsqlException ex)
                    {
                        ClientScript.RegisterStartupScript(GetType(), "text", "enablefields()", true);
                        tran.Rollback();
                        CreateLogFiles Err = new CreateLogFiles();
                        Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "btnAdd_Click()" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
                        RecordUserAction("btnAdd_Click", ex.Message, "F"); var errorcode = ex.ErrorCode;
                        if (errorcode == 23503)
                        {
                            Response.Write("<script language='javascript'>alert('" + "Please Complete Society Details first." + "')</script>");
                        }
                        else
                        {
                            Response.Write("<script language='javascript'>alert('" + "Exection Error" + "')</script>");
                        }
                        bindgridview(Session["AppID"].ToString());
                    }
                    finally
                    {
                        connect.Close();
                    }
                }
                btnAddOld.Visible = false;
            }
            else
            {
                ClientScript.RegisterStartupScript(GetType(), "text", "enablefields()", true);
                RecordUserAction("btnAdd_Click", "Sesiion null", "F");
            }

        }

        public void relaunchmodal()
        {
            if (Convert.ToInt32(Session["Renewal"]) == 2)
            {
                tr_dateofadmission.Visible = true;
            }
            else
            {
                tr_dateofadmission.Visible = false;
            }
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "none", "<script> $(window).on('load', function () { $('#addmembersmodal').modal({ backdrop: 'static' });});</script>", false);

        }
        public int check_entry(int designation)
        {
            int a = -1;
            NpgsqlConnection connect = new NpgsqlConnection();
            NpgsqlCommand cmd = new NpgsqlCommand();
            connect.ConnectionString = ConfigurationManager.ConnectionStrings["connect"].ConnectionString;
            cmd.Connection = connect;
            try
            {
                if (designation == 7 || designation == 8)
                {
                    btnAdd.Enabled = true;
                    lblError.Text = " ";
                    ddl_MemDocType.Enabled = true;
                    a= 0;
                }
                else
                {

                    connect.Open();
                    string query = "";
                    if (designation == 1 || designation == 9)
                    {
                        query = "Select fname from esociety.members where app_id=@AppID and (design=1 or design=9)";
                    }
                    else
                    {
                       query = "Select fname from esociety.members where app_id=@AppID and design=@design";
                    }
                    cmd.CommandText = query;
                    cmd.Parameters.AddWithValue("@AppID", Convert.ToInt64(Session["AppID"].ToString()));
                    cmd.Parameters.AddWithValue("@design", designation);
                    NpgsqlDataReader rd = cmd.ExecuteReader();
                    if (rd.Read())
                    {
                        string message = Server.HtmlEncode(rd["fname"].ToString());
                        message = message + " has held this Designation.";
                        btnAdd.Enabled = false;
                        ddl_MemDocType.Enabled = false;
                        lblError.Text = message;
                        //ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "none", "<script> $(window).on('load', function () { $('#myModal').modal({ backdrop: 'static' });});</script>", false);

                        a= 1;
                    }
                    else
                    {
                        lblError.Text = " ";
                        btnAdd.Enabled = true;
                        ddl_MemDocType.Enabled = true;
                        a= 0;

                    }
                    rd.Close();

                }
            }
            catch (NpgsqlException ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "check_entry()" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
                a= 1;
            }
            finally
            {

                connect.Close();
            }
            return a;
        }

        public int fileuploadfucntion(FileUpload fu, Label lb, MemberDocs mongodoc, string a, string ip, string mac, long App_ID, string collection, Int64 mid)
        {
            RecordUserAction("fileuploadfucntion", "File Upload Fucntion Called", "S");
            int flag = 0;
            if (Session["created_by"] != null)
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
                        UInt32 mimetype;
                        FindMimeFromData(0, null, document, 256, null, 0, out mimetype, 0);
                        IntPtr mimeTypePtr = new IntPtr(mimetype);
                        string mimeType = Marshal.PtrToStringUni(mimeTypePtr).ToLower();
                        Marshal.FreeCoTaskMem(mimeTypePtr);
                        var scanExitCode = 0;//UtilityDAO.VirusScanFile(flupHeader.PostedFile.FileName);
                        if (scanExitCode == 0)
                        {
                            string UploadFileName = file.FileName;
                            string Extension = UploadFileName.Substring(UploadFileName.LastIndexOf('.') + 1).ToLower();
                            var extCount = UploadFileName.Split('.').Length - 1;

                            if (Convert.ToInt32(Session["Renewal"]) == 2)
                            {
                                tr_dateofadmission.Visible = true;
                            }
                            else
                            {
                                tr_dateofadmission.Visible = false;
                            }
                            if (Extension.ToLower() != "pdf" || file.ContentType.ToLower() != "application/pdf" || mimeType != "application/pdf")
                            {
                                lblError.Text = "Please upload only Pdf file of maximum size 2MB allowed with extension .pdf!";
                                lblError.ForeColor = System.Drawing.Color.Red;
                                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "none", "<script> $(window).on('load', function () { $('#addmembersmodal').modal({ backdrop: 'static' });});</script>", false);
                                flag = 0;
                            }
                            else if (extCount > 1)
                            {
                                lblError.Text = "File name can not contain multiple dots/extensions.";
                                lblError.ForeColor = System.Drawing.Color.Red;
                                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "none", "<script> $(window).on('load', function () { $('#addmembersmodal').modal({ backdrop: 'static' });});</script>", false);

                                flag = 0;
                            }
                            else if (document == null)
                            {
                                lblError.Text = "Please upload file in pdf format or pdf not in correct format";
                                lblError.ForeColor = System.Drawing.Color.Red;
                                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "none", "<script> $(window).on('load', function () { $('#addmembersmodal').modal({ backdrop: 'static' });});</script>", false);

                                flag = 0;
                            }
                            else if (file.ContentLength <= 0 || file.ContentLength > (FileSize))
                            {
                                lblError.Text = "Only Pdf file of maximum size 2MB allowed !";
                                lblError.ForeColor = System.Drawing.Color.Red;
                                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "none", "<script> $(window).on('load', function () { $('#addmembersmodal').modal({ backdrop: 'static' });});</script>", false);

                                flag = 0;
                            }

                            else
                            {
                                string path = Utility.filesave(fu, Session["AppID"].ToString());
                                string resultedStr = "";
                                using (StreamReader sr = new StreamReader(path))
                                {
                                    resultedStr = SurroundingClass.ReadStreamWithTimeout(sr);
                                }
                                int scanned = Utility.checkFile(resultedStr);
                                if (scanned == 0)
                                {
                                    Response.Write("<script>alert('Invalid PDF uploaded');</script>");
                                    File.Delete(path);
                                }
                                else
                                {
                                    File.Delete(path);
                                    fileSizeFront = fu.FileContent.Length;
                                    documentBinary = new byte[fileSizeFront]; 
                                    byte[] bytes = fu.FileBytes;
                                    byte[] encrypt_bytes = obj_Byte_Encryption.EncryptData(bytes, System.Web.HttpContext.Current.Server.MapPath("~/ENCRYPT.KEY"));
                                    string document_id = Utility.get_docID();//doc ID
                                    mongodoc.App_ID = App_ID;
                                    mongodoc.Doc_CT = ct;
                                    mongodoc.time_stamp = DateTime.Now.ToString();
                                    ObjectId obj_id = ObjectId.GenerateNewId();
                                    mongodoc._Id = obj_id;
                                    mongodoc.IpAddress = ip;
                                    mongodoc.MacAddress = mac;
                                    mongodoc.Active = true;
                                    mongodoc.Member_ID = mid;
                                    mongodoc.Doc_ID = document_id;
                                    mongodoc.UpdatedBy = Session["created_by"].ToString();
                                    mongodoc.doc_name = a;
                                    mongodoc.DocContent = encrypt_bytes;
                                    int mvalue = InsertintoMongoDB(mongodoc, collection);
                                    if (mvalue == 1)
                                    {
                                        int pvalue = insertentryPosgres(obj_id, document_id, App_ID, mid);
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
                    }
                    else
                    {
                        lblError.ForeColor = System.Drawing.Color.Red;
                        lblError.Text = "Upload " + a + " Certificate file in PDF format";
                        flag = 0;
                        if (Convert.ToInt32(Session["Renewal"]) == 2)
                        {
                            tr_dateofadmission.Visible = true;
                        }
                        else
                        {
                            tr_dateofadmission.Visible = false;
                        }
                        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "none", "<script> $(window).on('load', function () { $('#addmembersmodal').modal({ backdrop: 'static' });});</script>", false);
                    }


                }
                catch (Exception ex)
                {
                    RecordUserAction("fileuploadfucntion", ex.Message, "F");
                    lblError.Text = "File Upload Function Excecution Failed";
                    CreateLogFiles Err = new CreateLogFiles();
                    Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "fileuploadfucntion()" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));

                }
            }
            return flag;

        }
        public int insertentryPosgres(ObjectId obj, string docid, long appid, Int64 mid)
        {
            int value = 0;
           
            if (Session["AppID"].ToString() == appid.ToString())
            {
                NpgsqlConnection conn = new NpgsqlConnection();
                NpgsqlCommand cmd = new NpgsqlCommand();
                conn.ConnectionString = ConfigurationManager.ConnectionStrings["connect"].ConnectionString;
                cmd.Connection = conn;
                string objectid = obj.ToString();
                conn.Open();
                try
                {
                    string query = "INSERT INTO esociety.members(document_mongoentry, doc_id, app_id,member_id)VALUES(@document_mongoentry,@doc_id, @app_id,@member_id)";
                    cmd.CommandText = query;
                    cmd.Parameters.AddWithValue("@document_mongoentry", objectid);
                    cmd.Parameters.AddWithValue("@doc_id", docid);
                    cmd.Parameters.AddWithValue("@app_id", appid);
                    cmd.Parameters.AddWithValue("@member_id", mid);
                    cmd.ExecuteNonQuery();
                    value = 1;
                    RecordUserAction("insertentryPosgres", "Insert in members", "S");
                }
                catch (NpgsqlException ex)
                {
                    value = 0;
                    CreateLogFiles Err = new CreateLogFiles();
                    Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "insertentryPosgres()" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
                    RecordUserAction("insertentryPosgres", ex.Message, "F");
                    var errorcode = ex.Message;
                    if (errorcode.Contains("23503"))
                    {
                        Response.Write("<script language='javascript'>alert('" + "Please Complete Society Details first." + "')</script>");
                    }
                    else
                    {
                        Response.Write("<script language='javascript'>alert('" + "Exection Error" + "')</script>");
                    }
                }
                finally
                {
                    conn.Close();
                }
            }
            else
            {
                value = 0;
                RecordUserAction("insertentryPosgres", "Execution Error/Session tampared", "F");
                Status.Text = "Execution error";
            }
            return value;
        }
        public int InsertintoMongoDB(MemberDocs doc, string sel_collection)
        {
            Insert insr = new Insert();
            try
            {
                RecordUserAction("InsertintoMongoDB", "Insert in MongoDB", "S");
                return insr.InsertintoMongoDB(doc, sel_collection);
            }
            catch (Exception ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "InsertintoMongoDB()" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
                RecordUserAction("InsertintoMongoDB", "Insert in MongoDB Failed", "F");
                Status.Text = "Insert in db Failed";
                return 0;
            }
        }
        protected void MemberDetailsBtn_Click(object sender, EventArgs e)
        {
            int isgov = Utility.getGovernmentSociety(Session["AppID"].ToString());
           
            // if (Convert.ToInt32(txttotmangcomm.Text) < minmembersreq)
            if (Convert.ToInt32(txtTotMember.Text) < 3 && isgov == 0)
            {
                string message = "Minimum 3 Managing Committee required";
                // string message = "Minimum 7 members required";
                Label1.Text = message; Label1.ForeColor = System.Drawing.Color.Red;
                message = "3 Members not added ";
                RecordUserAction("MemberDetailsBtn_Click", message, "F"); // no to be added in msg  // "Required no of Members not added"
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "none", "<script> $(window).on('load', function () { $('#myModal').modal({ backdrop: 'static' });});</script>", false);
            }
            else if (Utility.checkMembersList(Session["AppID"].ToString()) != 3 && isgov == 0)
            {
               


                string message = "President , Secretary, Treasurer are required to be part of Managing Committee.";
                Label1.ForeColor = System.Drawing.Color.Red;
                Label1.Text = message;
                RecordUserAction("MemberDetailsBtn_Click", message, "F"); // no to be added in msg  // "Required no of Members not added"
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "none", "<script> $(window).on('load', function () { $('#myModal').modal({ backdrop: 'static' });});</script>", false);

            }
           else if (Convert.ToInt32(txtTotMember.Text) < 2 && isgov == 1)
            {
                string message = "Minimum 2 Managing Committee required";
                // string message = "Minimum 7 members required";
                Label1.Text = message; Label1.ForeColor = System.Drawing.Color.Red;
                message = "3 Members not added ";
                RecordUserAction("MemberDetailsBtn_Click", message, "F"); // no to be added in msg  // "Required no of Members not added"
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "none", "<script> $(window).on('load', function () { $('#myModal').modal({ backdrop: 'static' });});</script>", false);
            }
            else if (Utility.checkMembersListgov(Session["AppID"].ToString()) != 2 && isgov == 1)
            {

                string message = "President , Secretary are required to be part of Managing Committee.";
                Label1.ForeColor = System.Drawing.Color.Red;
                Label1.Text = message;
                RecordUserAction("MemberDetailsBtn_Click", message, "F"); // no to be added in msg  // "Required no of Members not added"
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "none", "<script> $(window).on('load', function () { $('#myModal').modal({ backdrop: 'static' });});</script>", false);

            }
            else
            {
                NpgsqlConnection conn = new NpgsqlConnection();
                NpgsqlCommand cmd = new NpgsqlCommand();
                conn.ConnectionString = ConfigurationManager.ConnectionStrings["connect"].ConnectionString;
                cmd.Connection = conn;
                try
                {
                    conn.Open();
                    string query = "update esociety.society set totalmem=@total where app_id=@app_id";
                    cmd.CommandText = query;
                    cmd.Parameters.AddWithValue("@total", txtTotMember.Text);
                    cmd.Parameters.AddWithValue("@app_id", Convert.ToInt64(Session["AppID"].ToString()));
                    cmd.ExecuteNonQuery();
                    RecordUserAction("MemberDetailsBtn_Click", "Succss while writing number of members", "S");
                }
                catch (NpgsqlException ex)
                {
                    CreateLogFiles Err = new CreateLogFiles();
                    Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "MemberDetailsBtn_Click" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
                    RecordUserAction("MemberDetailsBtn_Click", ex.Message, "F");
                    Response.Write("<script language='javascript'>alert('" + "Exception while writing number of members" + "')</script>");
                }
                finally
                {
                    conn.Close();
                }
                RecordUserAction("MemberDetailsBtn_Click", "Document Download Page", "S");

                Response.Redirect("NormalMembers.aspx");

            }
        }
        protected void LBDelete_Click(object sender, EventArgs e)
        {
            if (Session["AppID"] != null)
            {
                LinkButton btn = (LinkButton)sender;
                GridViewRow row = (GridViewRow)btn.NamingContainer;
                HiddenField hd = row.FindControl("hfmemID") as HiddenField;
                if (hd.Value != null)
                {
                    Int64 memberID = Convert.ToInt64(hd.Value);
                    NpgsqlConnection connect = new NpgsqlConnection();
                    NpgsqlCommand cmd = new NpgsqlCommand();
                    connect.ConnectionString = ConfigurationManager.ConnectionStrings["connect"].ConnectionString;
                    cmd.Connection = connect;
                    connect.Open();
                    NpgsqlTransaction myTrans = connect.BeginTransaction();
                    cmd.Transaction = myTrans;
                    try
                    {
                        string query = "Select member_id, app_id, fname, designtaion, design, occupation, occupatid, address, proofname, proofid, mangcomm, created_at, created_by,";
                        query = query + " ipaddress, macaddress, document_mongoentry, doc_id, proof_document_no,salutation_id, salutation,gender,age,designtaion_others,occupation_others,dateofadmission";
                        query = query + " from esociety.members where active='Y' and member_id=@mid and app_id=@AppID";
                        cmd.CommandText = query;
                        cmd.Parameters.AddWithValue("@mid", memberID);
                        cmd.Parameters.AddWithValue("@AppID", Convert.ToInt64(Session["AppID"].ToString()));
                        NpgsqlDataReader rd = cmd.ExecuteReader();
                        if (rd.Read())
                        {
                            ViewState["appid"] = Convert.ToInt64(Session["AppID"].ToString());
                            ViewState["memberid"] = memberID;
                            ViewState["fname"] = Server.HtmlEncode(rd["fname"].ToString());
                            ViewState["designtaion"] = Server.HtmlEncode(rd["designtaion"].ToString());
                            ViewState["design"] = Server.HtmlEncode(rd["design"].ToString());
                            ViewState["occupatid"] = Server.HtmlEncode(rd["occupatid"].ToString());
                            ViewState["occupation"] = Server.HtmlEncode(rd["occupation"].ToString());
                            ViewState["address"] = Server.HtmlEncode(rd["address"].ToString());
                            ViewState["prrofid"] = Server.HtmlEncode(rd["proofid"].ToString());
                            ViewState["proofname"] = Server.HtmlEncode(rd["proofname"].ToString());
                            ViewState["mangcomm"] = Server.HtmlEncode(rd["mangcomm"].ToString());
                            ViewState["created_at"] = Server.HtmlEncode(rd["created_at"].ToString());
                            ViewState["created_by"] = Server.HtmlEncode(rd["created_by"].ToString());
                            ViewState["ipaddress"] = Server.HtmlEncode(rd["ipaddress"].ToString());
                            ViewState["macaddress"] = Server.HtmlEncode(rd["macaddress"].ToString());
                            ViewState["document_mongoentry"] = Server.HtmlEncode(rd["document_mongoentry"].ToString());
                            ViewState["doc_id"] = Server.HtmlEncode(rd["doc_id"].ToString());
                            ViewState["proof_document_no"] = Server.HtmlEncode(rd["proof_document_no"].ToString());
                            ViewState["salutation_id"] = Server.HtmlEncode(rd["salutation_id"].ToString());
                            ViewState["salutation"] = Server.HtmlEncode(rd["salutation"].ToString());
                            ViewState["gender"] = Server.HtmlEncode(rd["gender"].ToString());
                            ViewState["age"] = Server.HtmlEncode(rd["age"].ToString());
                            ViewState["designtaion_others"] = Server.HtmlEncode(rd["designtaion_others"].ToString());
                            ViewState["occupation_others"] = Server.HtmlEncode(rd["occupation_others"].ToString());
                            ViewState["dateofadmission"] = Server.HtmlEncode(rd["dateofadmission"].ToString());
                        }
                        rd.Close();
                        cmd.Parameters.Clear();
                        string inst_history = "INSERT INTO esociety.history_members(member_id, app_id, fname, designtaion, design, occupation, occupatid, address, proofname, proofid,";
                        inst_history = inst_history + " mangcomm, created_at, created_by, ipaddress, macaddress, active, document_mongoentry, doc_id, proof_document_no,salutation_id,salutation,gender,age,designtaion_others,occupation_others,dateofadmission,inserted_at,action,deleted_ip) VALUES";
                        inst_history = inst_history + " (@member_id, @app_id, @fname, @designtaion, @design, @occupation, @occupatid, @address, @proofname, @proofid,@mangcomm,@created_at,@created_by,";
                        inst_history = inst_history + " @ipaddress, @macaddress, 'N', @document_mongoentry, @doc_id, @proof_document_no,@salutation_id,@salutation,@gender,@age,@designtaion_others,@occupation_others,@dateofadmission,CURRENT_TIMESTAMP,'DELETE',@deleted_ip)";
                        cmd.CommandText = inst_history;
                        cmd.Parameters.AddWithValue("@app_id", Convert.ToInt64(ViewState["appid"].ToString()));
                        cmd.Parameters.AddWithValue("@member_id", Convert.ToInt64(ViewState["memberid"].ToString()));
                        cmd.Parameters.AddWithValue("@fname", ViewState["fname"].ToString());
                        cmd.Parameters.AddWithValue("@designtaion", ViewState["designtaion"].ToString());
                        cmd.Parameters.AddWithValue("@design", Convert.ToInt32(ViewState["design"].ToString()));
                        cmd.Parameters.AddWithValue("@occupation", ViewState["occupation"].ToString());
                        cmd.Parameters.AddWithValue("@occupatid", Convert.ToInt32(ViewState["occupatid"].ToString()));
                        cmd.Parameters.AddWithValue("@address", ViewState["address"].ToString());
                        cmd.Parameters.AddWithValue("@proofname", ViewState["proofname"].ToString());
                        cmd.Parameters.AddWithValue("@proofid", Convert.ToInt32(ViewState["prrofid"].ToString()));
                        cmd.Parameters.AddWithValue("@mangcomm", ViewState["mangcomm"].ToString());
                        cmd.Parameters.AddWithValue("@created_at", Convert.ToDateTime(ViewState["created_at"].ToString()));
                        cmd.Parameters.AddWithValue("@created_by", ViewState["created_by"].ToString());
                        cmd.Parameters.AddWithValue("@ipaddress", ViewState["ipaddress"].ToString());
                        cmd.Parameters.AddWithValue("@macaddress", ViewState["macaddress"].ToString());
                        cmd.Parameters.AddWithValue("@document_mongoentry", ViewState["document_mongoentry"].ToString());
                        cmd.Parameters.AddWithValue("@doc_id", ViewState["doc_id"].ToString());
                        cmd.Parameters.AddWithValue("@proof_document_no", ViewState["proof_document_no"].ToString());
                        cmd.Parameters.AddWithValue("@salutation_id", Convert.ToInt32(ViewState["salutation_id"].ToString()));
                        cmd.Parameters.AddWithValue("@salutation", ViewState["salutation"].ToString());
                        cmd.Parameters.AddWithValue("@gender", ViewState["gender"].ToString());
                        cmd.Parameters.AddWithValue("@age", Convert.ToInt32(ViewState["age"].ToString()));
                        cmd.Parameters.AddWithValue("@designtaion_others", ViewState["designtaion_others"].ToString());
                        cmd.Parameters.AddWithValue("@occupation_others", ViewState["occupation_others"].ToString());
                        cmd.Parameters.AddWithValue("@dateofadmission", ViewState["dateofadmission"].ToString());
                        cmd.Parameters.AddWithValue("@deleted_ip", ipaddress);
                        cmd.ExecuteNonQuery();
                        cmd.Parameters.Clear();
                        string query1 = "delete from esociety.members where member_id=@mid and app_id=@AppID and document_mongoentry=@mongo";
                        cmd.CommandText = query1;
                        cmd.Parameters.AddWithValue("@mid", memberID);
                        cmd.Parameters.AddWithValue("@AppID", Convert.ToInt64(Session["AppID"].ToString()));
                        cmd.Parameters.AddWithValue("@mongo", ViewState["document_mongoentry"].ToString());
                        cmd.ExecuteNonQuery();
                        cmd.Parameters.Clear();

                        string query2 = "delete from esociety.master_members_table where member_id=@mid and app_id=@AppID";
                        cmd.CommandText = query2;
                        cmd.Parameters.AddWithValue("@mid", memberID);
                        cmd.Parameters.AddWithValue("@AppID", Convert.ToInt64(Session["AppID"].ToString()));                        
                        cmd.ExecuteNonQuery();
                        cmd.Parameters.Clear();


                        myTrans.Commit();
                        Response.Write("<script>alert('Deleted successful');</script>");
                        RecordUserAction("LBDelete_Click", "Gridview Data Deleted Successfully", "S");
                        Response.Redirect("MemberDetails.aspx");
                        bindgridview(Session["AppID"].ToString());
                    }
                    catch (NpgsqlException ex)
                    {
                        CreateLogFiles Err = new CreateLogFiles();
                        Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "LBDelete_Click" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
                        myTrans.Rollback();
                        RecordUserAction("LBDelete_Click", ex.Message, "F");
                        Response.Write("<script language='javascript'>alert('" + "Gridview Exception. Delete Unsuccessful " + "')</script>");
                    }
                    finally
                    {
                        connect.Close();
                    }
                }
                else
                {
                    Response.Write("<script language='javascript'>alert('" + "File not Deleted. " + "')</script>");
                }
            }
            else
            {
                RecordUserAction("LBDelete_Click", "Session null", "F");
            }
        }
        public void setData(Int64 value)
        {
            Session["md_hfvalue"] = value;
        }
        public Int64 getData()
        {
            return Convert.ToInt64(Session["md_hfvalue"].ToString());
        }
        protected void LBUpdate_Click(object sender, EventArgs e)
        {
            if(Convert.ToInt32(Session["status_Id"]) == 9)
            {
                Label48.Text = "You cannot edit this application";
                Label48.ForeColor = System.Drawing.Color.Red;
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "none", "<script> $(window).on('load', function () { $('#errorModal').modal({ backdrop: 'static' });});</script>", false);

            }
            else
            {
                HFEdit_flag.Value = "1";
                btnAdd.Visible = false;
                btnAddOld.Visible = false;
                btnUpdate.Enabled = true;

                string status = "";
                if (Session["AppID"] != null)
                {
                    if (Convert.ToInt32(Session["Renewal"]) == 2)
                    {
                        tr_dateofadmission.Visible = true;
                    }
                    else
                    {
                        tr_dateofadmission.Visible = false;
                    }
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "none", "<script> $(window).on('load', function () { $('#addmembersmodal').modal({ backdrop: 'static' });});</script>", false);

                    LinkButton btn = (LinkButton)sender;
                    GridViewRow row = (GridViewRow)btn.NamingContainer;
                    //which value you are using
                    HiddenField hd = row.FindControl("hfmemID") as HiddenField;
                    HiddenField hdobid = row.FindControl("hfobjectID") as HiddenField;
                    HiddenField hddesign = row.FindControl("hfdesignid") as HiddenField;
                    int headdesignid = getheadentrydesignation();

                    //if (hd.Value != null && hdobid.Value != null)
                    if (hd.Value != null)
                    {
                        string object_ID = hdobid.Value;
                        Int64 memberID = Convert.ToInt64(hd.Value);
                        //document_mongoentry
                        setData(memberID);
                        if (hdobid.Value != null && hdobid.Value != "")
                        {
                            status = openConnectionMongo(object_ID);
                        }


                        try
                        {
                            Member_Details member_edit = new Member_Details();
                            member_edit = ins.FetchMember(Session["AppID"].ToString(), memberID);

                            txt_MemName.Text = member_edit.fname;
                            ddl_DesignMem.SelectedValue = member_edit.design.ToString();
                            TxtBx_DesignOthers.Text = member_edit.designtaion_others;
                            if (ddl_DesignMem.SelectedValue == "8")
                            {
                                row_DesignOthers.Visible = true;
                            }
                            else
                            {
                                row_DesignOthers.Visible = false;
                            }
                            ddl_MemOccup.SelectedValue = member_edit.occupatid.ToString();
                            TxtBx_OccupOthers.Text = member_edit.occupation_others;
                            if (ddl_MemOccup.SelectedValue == "4")
                            {
                                row_OccupOthers.Visible = true;
                            }
                            else
                            {
                                row_OccupOthers.Visible = false;
                            }
                            txt_MemAddress.Text = member_edit.address;
                            ddl_MemDocType.SelectedValue = member_edit.proofid.ToString();
                            if (member_edit.salutation_id.ToString() == "" || member_edit.salutation_id.ToString() == null)
                            {
                                ddl_salutation.SelectedValue = "1";
                            }
                            else
                            {
                                ddl_salutation.SelectedValue = member_edit.salutation_id.ToString();
                            }

                            if (member_edit.gender.ToString() == "" || member_edit.gender.ToString() == null)
                            {
                                Rdbtngender.SelectedValue = Server.HtmlEncode("M");
                            }
                            else
                            {
                                Rdbtngender.SelectedValue = member_edit.gender;
                            }
                            if (member_edit.age.ToString() == "" || (member_edit.age.ToString() == null))
                            {
                                txtbx_age.Text = "0";
                            }
                            else
                            {
                                txtbx_age.Text = member_edit.age.ToString();
                            }
                            string mangcomm = member_edit.mangcomm.Trim();
                            if (mangcomm == "Yes")
                            {
                                ddlMcom.SelectedValue = "1";
                            }
                            else if (mangcomm == "No")
                            {
                                ddlMcom.SelectedValue = "2";
                            }
                            // ddlMcom.SelectedItem.Text = member_edit.mangcomm;
                            if (Convert.ToInt32(ddl_MemDocType.SelectedValue) == 6)
                            {
                                Label2.Text = "Other Document" + " No";
                            }
                            else
                            {
                                string value = ddl_MemDocType.SelectedItem.Text;
                                Label2.Text = value + " No";
                            }

                            if (Convert.ToInt32(ddl_MemDocType.SelectedValue) == 5)
                            {
                                string aadhar = Utility.getAadharNo(memberID.ToString());
                                string finaladdhar = _encryption.Decrypt(aadhar, System.Web.HttpContext.Current.Server.MapPath("~/ENCRYPT.KEY"));
                                TxtBxDocumentNo.Text = Server.HtmlEncode(Utility.MaskMobile(finaladdhar, 0, "XXXXXXXX"));

                            }
                            else
                            {
                                string decrypt_docno = member_edit.proof_document_no;
                                TxtBxDocumentNo.Text = _encryption.Decrypt(decrypt_docno, System.Web.HttpContext.Current.Server.MapPath("~/ENCRYPT.KEY"));

                            }
                            TxtBxRemarks.Text = member_edit.remarks;
                            if (status == "True")
                            {
                                lbupload.Text = "File is Present.Old File will be replaced with new File";
                            }
                            else
                            {
                                lbupload.Text = "";
                            }

                            if (member_edit.dateofadmission != null && member_edit.dateofadmission != "")
                            {
                                txtbxdateadmission.Text = Server.HtmlEncode(Convert.ToDateTime(member_edit.dateofadmission).ToString("yyyy-MM-dd"));
                            }
                            btnUpdate.Visible = true;
                            trvisible.Visible = true;


                            RecordUserAction("LBUpdate_Click", "Gridview Data to textbox", "S");
                            bindgridview(Session["AppID"].ToString());

                            if (Convert.ToInt32(hddesign.Value) == headdesignid)  // if head entry then dont allow changes in salutation, name and gender
                            {
                                ddl_salutation.Enabled = false;
                                txt_MemName.Enabled = false;
                                Rdbtngender.Enabled = false;
                                ddlMcom.Enabled = false;

                            }
                            else
                            {
                                ddl_salutation.Enabled = true;
                                txt_MemName.Enabled = true;
                                Rdbtngender.Enabled = true;
                                ddlMcom.Enabled = true;
                            }
                        }
                        catch (Exception ex)
                        {
                            CreateLogFiles Err = new CreateLogFiles();
                            Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "LBUpdate_Click" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
                            RecordUserAction("LBUpdate_Click", ex.Message, "F");
                            Response.Write("<script language='javascript'>alert('" + "Gridview Data to textbox Exception" + "')</script>");
                        }

                    }
                    else
                    {
                        Response.Write("<script language='javascript'>alert('" + "Not Updated" + "')</script>");
                    }
                    ddl_DesignMem.Enabled = false;

                    //ddlMcom.Enabled = true;
                }
                else
                {
                    RecordUserAction("LBUpdate_Click", "Session null", "F");
                }
            } 
        }//linkbutton
        protected string openConnectionMongo(string objectid)
        {
            try
            {
                var str = ConfigurationManager.AppSettings["mongoconnect"];
                IMongoDatabase database;
                IMongoClient client;
                client = new MongoClient(str);
                database = client.GetDatabase("eGoaSociety");
                var collection = database.GetCollection<MemberDocs>("Members Document");
                var status = collection.Find(x => x._Id == ObjectId.Parse(objectid)).FirstOrDefault();
                string abc = status.Active.ToString();
                RecordUserAction("openConnectionMongo", "Mongo Connected", "S");
                return abc;
            }
            catch (MongoException ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "openConnectionMongo()" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
                RecordUserAction("openConnectionMongo", ex.Message, "F");
                return null;
            }
        }
        protected void btnUpdate_Click(object sender, EventArgs e)
        {
            string doc_no = Encrypt.DecryptStringAES(hfencryptNo.Value.ToString(), Session["Enc_Random"].ToString(), Session["Enc_Vector"].ToString());

            if (Session["AppID"] != null && Session["created_by"] != null)
            {
                btnAdd.Visible = false;
                btnAddOld.Visible = false;
                tr_dateofadmission.Visible = false;
                btnUpdate.Visible = true;                

                if (ddl_salutation.SelectedValue == "-1")
                {
                    lblError.Text = "Please select Salutation!";
                    relaunchmodal();
                }
                else if (txt_MemName.Text == "" || txt_MemName.Text == null)
                {
                    lblError.Text = "Please Enter Name!";
                    relaunchmodal();
                }
                else if (txtbx_age.Text == "" || txtbx_age.Text == null)
                {
                    lblError.Text = "Please Enter Age!";
                    relaunchmodal();
                }
                else if (!_val.validateData(txtbx_age.Text, _val.captcha_regex))
                {
                    lblError.Text = "Please Enter Valid Age!";
                    relaunchmodal();
                }
                else if (Int32.Parse(txtbx_age.Text) > 115)
                {
                    lblError.Text = "Age cannot be greater than 115!";
                    relaunchmodal();
                }
                else if (Int32.Parse(txtbx_age.Text) < 18)
                {
                    lblError.Text = "Minimum age allowed is 18";
                    relaunchmodal();
                }
                else if (txt_MemAddress.Text == "" || txt_MemAddress.Text == null)
                {
                    lblError.Text = "Please Enter Address!";
                    relaunchmodal();
                }
                else if (!_val.validateData(txt_MemAddress.Text, _val.reamrks_validation))
                {
                    lblError.Text = "Please Enter Valid Address!";
                    relaunchmodal();
                }
                else if (ddl_DesignMem.SelectedValue == "-1")
                {
                    lblError.Text = "Please select Designation of the member!";
                    relaunchmodal();
                }
                else if (ddl_DesignMem.SelectedValue == "8" && (TxtBx_DesignOthers.Text == "" || TxtBx_DesignOthers.Text == null))
                {
                    lblError.Text = "Please Enter Other Designation!";
                    relaunchmodal();
                }
                else if (ddl_DesignMem.SelectedValue == "8" && (!_val.validateData(TxtBx_DesignOthers.Text, _val.alpharegex)))
                {
                    lblError.Text = "Please Enter Valid Designation!";
                    relaunchmodal();
                }
                else if (ddl_MemOccup.SelectedValue == "-1")
                {
                    lblError.Text = "Please Enter Occupation!";
                    relaunchmodal();
                }
                else if (ddl_MemOccup.SelectedValue == "4" && (TxtBx_OccupOthers.Text == "" || TxtBx_OccupOthers.Text == null))
                {
                    lblError.Text = "Please Enter Other Occupation!";
                    relaunchmodal();
                }
                else if (ddl_MemOccup.SelectedValue == "4" && (!_val.validateData(TxtBx_OccupOthers.Text, _val.alpharegex)))
                {
                    lblError.Text = "Please Enter Valid Occupation!";
                    relaunchmodal();
                }

                else if (ddl_MemDocType.SelectedValue == "-1")
                {
                    lblError.Text = "Please select Type of document to be added!";
                    relaunchmodal();
                }
                else if (doc_no == "" || doc_no == null)
                {
                    lblError.Text = "Please Enter document no !";
                    relaunchmodal();
                }
                else if (ddl_MemDocType.SelectedValue == "1" && (!_val.validateData(doc_no, _val.epicno)))
                {
                    lblError.Text = "Please Enter valid election card no";
                    relaunchmodal();
                }
                else if (ddl_MemDocType.SelectedValue == "2" && (!_val.validateData(doc_no, _val.cust_driving)))
                {
                    lblError.Text = "Please Enter valid Driving License no";
                    relaunchmodal();
                }
                else if (ddl_MemDocType.SelectedValue == "3" && (!_val.validateData(doc_no, _val.pan_regex)))
                {
                    lblError.Text = "Please Enter valid Pan card no";
                    relaunchmodal();
                }
                else if (ddl_MemDocType.SelectedValue == "4" && (!_val.validateData(doc_no, _val.passport)))
                {
                    lblError.Text = "Please Enter valid Passport no";
                    relaunchmodal();
                }
                else if (ddl_MemDocType.SelectedValue == "6" && (!_val.validateData(doc_no, _val.alpha_numericregex)))
                {
                    lblError.Text = "Please Enter valid Identity Proof no";
                    relaunchmodal();
                }
                else if (ddl_MemDocType.SelectedValue == "5" && (!_val.validateData(doc_no, _val.adhar_regex)))
                {
                    lblError.Text = "Please Enter valid Aadhar Card no";
                    relaunchmodal();
                }
                else if (ddlMcom.SelectedValue == "0")
                {
                    lblError.Text = "Please select part of Managing Committee or not";
                    relaunchmodal();
                }
                else if(Convert.ToInt32(Session["Renewal"]) == 2 && (txtbxdateadmission.Text == null || txtbxdateadmission.Text == ""))
                {
                    lblError.Text = " Date of admission is blank";
                    relaunchmodal();
                }
                else
                {
                    Int64 mid = getData();
                    Member_Details member_history = new Member_Details();
                    member_history = ins.FetchMember(Session["AppID"].ToString(), mid);

                    NpgsqlConnection connect = new NpgsqlConnection();
                    NpgsqlCommand cmd = new NpgsqlCommand();
                    connect.ConnectionString = ConfigurationManager.ConnectionStrings["connect"].ConnectionString;
                    cmd.Connection = connect;

                    connect.Open();
                    NpgsqlTransaction myTrans = connect.BeginTransaction();
                    cmd.Transaction = myTrans;
                    try
                    {
                        lblError.Text = " ";
                        cmd.Parameters.Clear();

                        string inst_history = "INSERT INTO esociety.history_members(member_id, app_id, fname, designtaion, design, occupation, occupatid, address, proofname, proofid,";
                        inst_history = inst_history + " mangcomm,created_at, created_by, ipaddress, macaddress, active, document_mongoentry, doc_id, proof_document_no,salutation_id,salutation,gender,age,designtaion_others,occupation_others,remarks,dateofadmission,inserted_at,action,deleted_ip) VALUES";
                        inst_history = inst_history + " (@member_id, @app_id, @fname, @designtaion, @design, @occupation, @occupatid, @address, @proofname, @proofid,@mangcomm,@created_at,@created_by,";
                        inst_history = inst_history + " @ipaddress, @macaddress, 'N', @document_mongoentry, @doc_id, @proof_document_no,@salutation_id,@salutation,@gender,@age,@designtaion_others,@occupation_others,@remarks,@dateofadmission,CURRENT_TIMESTAMP,'UPDATE',@deleted_ip)";
                        cmd.CommandText = inst_history;
                        cmd.Parameters.AddWithValue("@app_id",Convert.ToInt64(member_history.app_id));
                        cmd.Parameters.AddWithValue("@member_id", member_history.member_id);
                        cmd.Parameters.AddWithValue("@fname", member_history.fname);
                        cmd.Parameters.AddWithValue("@designtaion", member_history.designtaion);
                        cmd.Parameters.AddWithValue("@design", member_history.design);
                        cmd.Parameters.AddWithValue("@occupation",member_history.occupation);
                        cmd.Parameters.AddWithValue("@occupatid", member_history.occupatid);
                        cmd.Parameters.AddWithValue("@address", member_history.address);
                        cmd.Parameters.AddWithValue("@proofname", member_history.proofname);
                        cmd.Parameters.AddWithValue("@proofid", member_history.proofid);
                        cmd.Parameters.AddWithValue("@mangcomm", member_history.mangcomm);
                        cmd.Parameters.AddWithValue("@created_at", Convert.ToDateTime(member_history.created_at));
                        cmd.Parameters.AddWithValue("@created_by", member_history.created_by);
                        cmd.Parameters.AddWithValue("@ipaddress", member_history.ipaddress);
                        cmd.Parameters.AddWithValue("@macaddress",member_history.macaddress);
                        cmd.Parameters.AddWithValue("@document_mongoentry", member_history.document_mongoentry);
                        cmd.Parameters.AddWithValue("@doc_id", member_history.doc_id);
                        cmd.Parameters.AddWithValue("@proof_document_no", member_history.proof_document_no);
                        cmd.Parameters.AddWithValue("@salutation_id", member_history.salutation_id);
                        cmd.Parameters.AddWithValue("@salutation", member_history.salutation);
                        cmd.Parameters.AddWithValue("@gender", member_history.gender);
                        cmd.Parameters.AddWithValue("@age", member_history.age);
                        cmd.Parameters.AddWithValue("@designtaion_others", member_history.designtaion_others);
                        cmd.Parameters.AddWithValue("@occupation_others", member_history.occupation_others);
                        cmd.Parameters.AddWithValue("@remarks", member_history.remarks);
                        cmd.Parameters.AddWithValue("@dateofadmission", member_history.dateofadmission);
                        cmd.Parameters.AddWithValue("@deleted_ip", ipaddress);
                        cmd.ExecuteNonQuery();
                        cmd.Parameters.Clear();


                        string query = "";

                        int proofid = Convert.ToInt32(ddl_MemDocType.SelectedValue);
                        if (proofid == 5)
                        {
                            string encrypt_docno = _encryption.Encrypt(doc_no, System.Web.HttpContext.Current.Server.MapPath("~/ENCRYPT.KEY"));
                            var str = ConfigurationManager.AppSettings["aadharvault"];
                            IMongoDatabase database;
                            IMongoClient client;
                            client = new MongoClient(str);
                            Aadhar adr = new Aadhar();
                            adr.App_ID = Session["AppID"].ToString();
                            adr.Member_ID = mid.ToString();
                            ObjectId objectID = ObjectId.GenerateNewId();
                            adr._Id = objectID;
                            adr.Doc_ID = encrypt_docno;
                            database = client.GetDatabase("eSocietyAadharVault");
                            var collection = database.GetCollection<Aadhar>("aadhjarVault");
                            collection.InsertOne(adr);
                            if (Convert.ToInt32(Session["Renewal"]) == 2) // is rennewal
                            {
                                query = "update esociety.members set fname=@fname , occupation=@occupation, occupatid=@occupatid, address=@address, proofname=@proofname, proofid=@proofid,";
                                query = query + " mangcomm =@mangcomm, created_at=current_timestamp, created_by=@created_by, ipaddress=@ipaddress, macaddress=@macaddress,";
                                query = query + " proof_document_no =@proofno,active='Y',salutation_id=@salutation_id,salutation=@salutation,gender=@gender,age=@age,designtaion_others=@designtaion_others,occupation_others=@occupation_others,remarks = @remarks,dateofadmission=@dateofadmission,flag_aadhar = 'C' where member_id=@mid and app_id=@AppID";
                            }
                            else
                            {
                                query = "update esociety.members set fname=@fname , occupation=@occupation, occupatid=@occupatid, address=@address, proofname=@proofname, proofid=@proofid,";
                                query = query + " mangcomm =@mangcomm, created_at=current_timestamp, created_by=@created_by, ipaddress=@ipaddress, macaddress=@macaddress,";
                                query = query + " proof_document_no =@proofno,active='Y',salutation_id=@salutation_id,salutation=@salutation,gender=@gender,age=@age,designtaion_others=@designtaion_others,occupation_others=@occupation_others,remarks = @remarks,flag_aadhar = 'C' where member_id=@mid and app_id=@AppID";
                            }
                            cmd.CommandText = query;
                            cmd.Parameters.AddWithValue("@fname", txt_MemName.Text);
                            cmd.Parameters.AddWithValue("@designtaion", ddl_DesignMem.SelectedItem.Text);
                            //cmd.Parameters.AddWithValue("@design", Convert.ToInt32(ddl_DesignMem.SelectedValue));
                            cmd.Parameters.AddWithValue("@occupation", ddl_MemOccup.SelectedItem.Text);
                            cmd.Parameters.AddWithValue("@occupatid", Convert.ToInt32(ddl_MemOccup.SelectedValue));
                            cmd.Parameters.AddWithValue("@address", txt_MemAddress.Text);
                            cmd.Parameters.AddWithValue("@proofname", ddl_MemDocType.SelectedItem.Text);
                            cmd.Parameters.AddWithValue("@proofid", Convert.ToInt32(ddl_MemDocType.SelectedValue));
                            cmd.Parameters.AddWithValue("@mangcomm", ddlMcom.SelectedItem.Text.Trim());
                            cmd.Parameters.AddWithValue("@created_by", Session["created_by"].ToString());
                            cmd.Parameters.AddWithValue("@ipaddress", ipaddress);
                            cmd.Parameters.AddWithValue("@macaddress", macaddress);
                            cmd.Parameters.AddWithValue("@mid", mid);                            
                            cmd.Parameters.AddWithValue("@proofno", objectID.ToString());
                            cmd.Parameters.AddWithValue("@salutation_id", Convert.ToInt32(ddl_salutation.SelectedValue));
                            cmd.Parameters.AddWithValue("@salutation", ddl_salutation.SelectedItem.Text);
                            cmd.Parameters.AddWithValue("@gender", Rdbtngender.SelectedValue);
                            cmd.Parameters.AddWithValue("@age", Convert.ToInt32(txtbx_age.Text));
                            cmd.Parameters.AddWithValue("@AppID", Convert.ToInt64(Session["AppID"].ToString()));
                            cmd.Parameters.AddWithValue("@designtaion_others", TxtBx_DesignOthers.Text);
                            cmd.Parameters.AddWithValue("@occupation_others", TxtBx_OccupOthers.Text);
                            cmd.Parameters.AddWithValue("@remarks", TxtBxRemarks.Text);
                            if (Convert.ToInt32(Session["Renewal"]) == 2)
                            {
                                cmd.Parameters.AddWithValue("@dateofadmission", Convert.ToDateTime(txtbxdateadmission.Text, french).Date);
                            }
                            else
                            {
                                cmd.Parameters.AddWithValue("@dateofadmission", DBNull.Value);
                            }

                            cmd.ExecuteNonQuery();
                            cmd.Parameters.Clear();
                        }
                        else
                        {
                            if (Convert.ToInt32(Session["Renewal"]) == 2) // is rennewal
                            {
                                query = "update esociety.members set fname=@fname , occupation=@occupation, occupatid=@occupatid, address=@address, proofname=@proofname, proofid=@proofid,";
                                query = query + " mangcomm =@mangcomm, created_at=current_timestamp, created_by=@created_by, ipaddress=@ipaddress, macaddress=@macaddress,";
                                query = query + " proof_document_no =@proofno,active='Y',salutation_id=@salutation_id,salutation=@salutation,gender=@gender,age=@age,designtaion_others=@designtaion_others,occupation_others=@occupation_others,remarks = @remarks,dateofadmission=@dateofadmission where member_id=@mid and app_id=@AppID";
                            }
                            else
                            {
                                query = "update esociety.members set fname=@fname , occupation=@occupation, occupatid=@occupatid, address=@address, proofname=@proofname, proofid=@proofid,";
                                query = query + " mangcomm =@mangcomm, created_at=current_timestamp, created_by=@created_by, ipaddress=@ipaddress, macaddress=@macaddress,";
                                query = query + " proof_document_no =@proofno,active='Y',salutation_id=@salutation_id,salutation=@salutation,gender=@gender,age=@age,designtaion_others=@designtaion_others,occupation_others=@occupation_others,remarks = @remarks where member_id=@mid and app_id=@AppID";
                            }
                            cmd.CommandText = query;
                            cmd.Parameters.AddWithValue("@fname", txt_MemName.Text);
                            cmd.Parameters.AddWithValue("@designtaion", ddl_DesignMem.SelectedItem.Text);
                            //cmd.Parameters.AddWithValue("@design", Convert.ToInt32(ddl_DesignMem.SelectedValue));
                            cmd.Parameters.AddWithValue("@occupation", ddl_MemOccup.SelectedItem.Text);
                            cmd.Parameters.AddWithValue("@occupatid", Convert.ToInt32(ddl_MemOccup.SelectedValue));
                            cmd.Parameters.AddWithValue("@address", txt_MemAddress.Text);
                            cmd.Parameters.AddWithValue("@proofname", ddl_MemDocType.SelectedItem.Text);
                            cmd.Parameters.AddWithValue("@proofid", Convert.ToInt32(ddl_MemDocType.SelectedValue));
                            cmd.Parameters.AddWithValue("@mangcomm", ddlMcom.SelectedItem.Text.Trim());
                            cmd.Parameters.AddWithValue("@created_by", Session["created_by"].ToString());
                            cmd.Parameters.AddWithValue("@ipaddress", ipaddress);
                            cmd.Parameters.AddWithValue("@macaddress", macaddress);
                            cmd.Parameters.AddWithValue("@mid", mid);
                            string encrypt_docno = _encryption.Encrypt(doc_no, System.Web.HttpContext.Current.Server.MapPath("~/ENCRYPT.KEY"));
                            cmd.Parameters.AddWithValue("@proofno", encrypt_docno);
                            cmd.Parameters.AddWithValue("@salutation_id", Convert.ToInt32(ddl_salutation.SelectedValue));
                            cmd.Parameters.AddWithValue("@salutation", ddl_salutation.SelectedItem.Text);
                            cmd.Parameters.AddWithValue("@gender", Rdbtngender.SelectedValue);
                            cmd.Parameters.AddWithValue("@age", Convert.ToInt32(txtbx_age.Text));
                            cmd.Parameters.AddWithValue("@AppID", Convert.ToInt64(Session["AppID"].ToString()));
                            cmd.Parameters.AddWithValue("@designtaion_others", TxtBx_DesignOthers.Text);
                            cmd.Parameters.AddWithValue("@occupation_others", TxtBx_OccupOthers.Text);
                            cmd.Parameters.AddWithValue("@remarks", TxtBxRemarks.Text);
                            if (Convert.ToInt32(Session["Renewal"]) == 2)
                            {
                                cmd.Parameters.AddWithValue("@dateofadmission", Convert.ToDateTime(txtbxdateadmission.Text, french).Date);
                            }
                            else
                            {
                                cmd.Parameters.AddWithValue("@dateofadmission", DBNull.Value);
                            }

                            cmd.ExecuteNonQuery();
                            cmd.Parameters.Clear();
                        }



                        if (FileUpload6.HasFile)
                        {
                            string path = Utility.filesave(FileUpload6, Session["AppID"].ToString());
                            string resultedStr = "";
                            using (StreamReader sr = new StreamReader(path))
                            {
                                resultedStr = SurroundingClass.ReadStreamWithTimeout(sr);
                            }
                            int scanned = Utility.checkFile(resultedStr);
                            if (scanned == 0)
                            {
                                Response.Write("<script>alert('Invalid PDF File');</script>");
                                File.Delete(path);
                            }
                            else
                            {
                                File.Delete(path);
                                string stringfilesize = ConfigurationManager.AppSettings["FileSize"];
                                int FileSize = Convert.ToInt32(stringfilesize) * 1024 * 1024;
                                MemberDocs mongodoc = new MemberDocs();
                                HttpPostedFile file = FileUpload6.PostedFile;
                                byte[] document = new byte[file.ContentLength];
                                file.InputStream.Read(document, 0, file.ContentLength);
                                UInt32 mimetype;
                                FindMimeFromData(0, null, document, 256, null, 0, out mimetype, 0);
                                IntPtr mimeTypePtr = new IntPtr(mimetype);
                                string mimeType = Marshal.PtrToStringUni(mimeTypePtr).ToLower();
                                Marshal.FreeCoTaskMem(mimeTypePtr);
                                var scanExitCode = 0;//UtilityDAO.VirusScanFile(flupHeader.PostedFile.FileName);
                                if (scanExitCode == 0)
                                {
                                    string UploadFileName = file.FileName;
                                    string Extension = UploadFileName.Substring(UploadFileName.LastIndexOf('.') + 1).ToLower();
                                    var extCount = UploadFileName.Split('.').Length - 1;

                                    if (Extension.ToLower() != "pdf" || file.ContentType.ToLower() != "application/pdf" || mimeType != "application/pdf")
                                    {
                                        lblError.Text = "Please upload only Pdf file of maximum size 2MB allowed with extension .pdf!";
                                        lblError.ForeColor = System.Drawing.Color.Red;
                                        relaunchmodal();
                                    }
                                    else if (extCount > 1)
                                    {
                                        lblError.Text = "File name can not contain multiple dots/extensions.";
                                        lblError.ForeColor = System.Drawing.Color.Red;
                                        relaunchmodal();
                                    }
                                    else if (document == null)
                                    {
                                        lblError.Text = "Please upload file in pdf format or pdf not in correct format";
                                        lblError.ForeColor = System.Drawing.Color.Red;
                                        relaunchmodal();
                                    }
                                    else if (file.ContentLength <= 0 || file.ContentLength > (FileSize))
                                    {
                                        lblError.Text = "Only Pdf file of maximum size 2MB allowed !";
                                        lblError.ForeColor = System.Drawing.Color.Red;
                                        relaunchmodal();
                                    }
                                    else
                                    {
                                        fileSizeFront = FileUpload6.FileContent.Length;
                                        documentBinary = new byte[fileSizeFront];
                                        byte[] bytes = FileUpload6.FileBytes;
                                        byte[] encrypt_bytes = obj_Byte_Encryption.EncryptData(bytes, System.Web.HttpContext.Current.Server.MapPath("~/ENCRYPT.KEY"));
                                        ObjectId obj_id = ObjectId.GenerateNewId();
                                        string document_id = member_history.doc_id;//doc ID
                                        mongodoc.App_ID = Convert.ToInt64(Session["AppID"].ToString());
                                        mongodoc.Doc_CT = ct;
                                        mongodoc.time_stamp = DateTime.Now.ToString();
                                        mongodoc._Id = obj_id;
                                        mongodoc.IpAddress = ipaddress;
                                        mongodoc.MacAddress = macaddress;
                                        mongodoc.Active = true;
                                        mongodoc.Member_ID = mid;
                                        mongodoc.Doc_ID = document_id;
                                        mongodoc.UpdatedBy = Session["created_by"].ToString();
                                        mongodoc.doc_name = ddl_MemDocType.SelectedItem.Text;
                                        mongodoc.DocContent = encrypt_bytes;
                                        mongodoc._Id = obj_id;
                                        int amongo = InsertintoMongoDB(mongodoc, "Members Document");
                                        if (amongo == 1)
                                        {
                                            string query1 = "update esociety.members set document_mongoentry=@mongo where doc_id=@docid";
                                            cmd.CommandText = query1;
                                            cmd.Parameters.AddWithValue("@mongo", obj_id.ToString());
                                            cmd.Parameters.AddWithValue("@docid", member_history.doc_id);
                                            cmd.ExecuteNonQuery();
                                            Response.Write("<script>alert('Updated successful');</script>");
                                            bindgridview(Session["AppID"].ToString());
                                            myTrans.Commit();
                                            HFEdit_flag.Value = "0";
                                            ddl_DesignMem.Enabled = true;
                                            RecordUserAction("btnUpdate_Click", "Update Member Data", "S");
                                            Response.Redirect("MemberDetails.aspx");
                                        }
                                        else
                                        {
                                            myTrans.Rollback();
                                            Response.Write("<script>alert('Updation Failed');</script>");
                                        }

                                    }
                                }
                            }
                        }
                        else
                        {
                            myTrans.Commit();
                        }
                        bindgridview(Session["AppID"].ToString());

                    }
                    catch (NpgsqlException ex)
                    {
                        ClientScript.RegisterStartupScript(GetType(), "text", "enablefields()", true);
                        CreateLogFiles Err = new CreateLogFiles();
                        Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "btnUpdate_Click" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
                        myTrans.Rollback();
                        RecordUserAction("btnUpdate_Click", ex.Message, "F");
                        Response.Write("<script language='javascript'>alert('" + "Update Member Data Exception" + "')</script>");
                    }
                    finally
                    {                       
                        connect.Close();
                        HFEdit_flag.Value = "0";
                        btnAdd.Visible = true;

                    }
                }
            }
            else
            {
                ClientScript.RegisterStartupScript(GetType(), "text", "enablefields()", true);
                RecordUserAction("btnUpdate_Click", "Session value null on button click", "S");
            }
        }
        protected void grvMemberDetails_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            try
            {
                if (Session["AppID"] != null)
                {
                    grvMemberDetails.PageIndex = e.NewPageIndex;
                    bindgridview(Session["AppID"].ToString());
                }
                else
                {
                    RecordUserAction("grvMemberDetails_PageIndexChanging", "Sesiion null", "F");
                }
            }
            catch (Exception ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "grvMemberDetails_PageIndexChanging" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
            }
        }
        protected void ddl_MemDocType_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (Convert.ToInt32(HFEdit_flag.Value) == 1)
                {
                    btnAdd.Visible = false;
                    btnAddOld.Visible = false;
                }
                else if ( ViewState["addoldmember_flag"].ToString() == "1")
                {
                    btnAddOld.Visible = true;
                    btnAdd.Visible = false;
                    btnUpdate.Visible = false;
                }
                else
                {
                    btnAdd.Visible = true;
                    btnAddOld.Visible = false;
                    btnUpdate.Visible = false;
                }


                TxtBxDocumentNo.Text = "";
                trvisible.Visible = true;
                if (Convert.ToInt32(ddl_MemDocType.SelectedValue) == 1)
                {
                   // btnAdd.Enabled = false;
                    string value = ddl_MemDocType.SelectedItem.Text;
                    Label2.Text = value + " No";
                    TxtBxDocumentNo.Attributes.Add("placeholder", "");
                    //TxtBxDocumentNo.Attributes.Add("placeholder", "Ex-  AAA1234567");
                }
                else if (Convert.ToInt32(ddl_MemDocType.SelectedValue) == 2)
                {
                    //btnAdd.Enabled = false;
                    string value = ddl_MemDocType.SelectedItem.Text;
                    Label2.Text = value + " No";
                    TxtBxDocumentNo.Attributes.Add("placeholder", "");
                    //TxtBxDocumentNo.Attributes.Add("placeholder", "Ex-  DL1420110012345");
                }
                else if (Convert.ToInt32(ddl_MemDocType.SelectedValue) == 3)
                {
                    //btnAdd.Enabled = false;
                    string value = ddl_MemDocType.SelectedItem.Text;
                    Label2.Text = value + " No";
                    TxtBxDocumentNo.Attributes.Add("placeholder", "");
                    //TxtBxDocumentNo.Attributes.Add("placeholder", "Ex-  AAAPL1234C");
                }
                else if (Convert.ToInt32(ddl_MemDocType.SelectedValue) == 4)
                {
                    //btnAdd.Enabled = false;
                    string value = ddl_MemDocType.SelectedItem.Text;
                    Label2.Text = value + " No";
                    TxtBxDocumentNo.Attributes.Add("placeholder", "");
                    //TxtBxDocumentNo.Attributes.Add("placeholder", "Ex-  A1234567");
                }
                else if (Convert.ToInt32(ddl_MemDocType.SelectedValue) == 5)
                {
                    //btnAdd.Enabled = false;
                    string value = ddl_MemDocType.SelectedItem.Text;
                    Label2.Text = value + " No";
                    TxtBxDocumentNo.Attributes.Add("placeholder", "");
                    //  TxtBxDocumentNo.Text = "";
                }
                else if (Convert.ToInt32(ddl_MemDocType.SelectedValue) == 6)
                {
                   // btnAdd.Enabled = false;
                    Label2.Text = "Other Document" + " No";
                    TxtBxDocumentNo.Attributes.Add("placeholder", "");
                    TxtBxDocumentNo.Text = "";
                }
                if (Convert.ToInt32(Session["Renewal"]) == 2)
                {
                    tr_dateofadmission.Visible = true;
                }
                else
                {
                    tr_dateofadmission.Visible = false;
                }
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "none", "<script> $(window).on('load', function () { $('#addmembersmodal').modal({ backdrop: 'static' });});</script>", false);

            }
            catch (Exception ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "ddl_MemDocType_SelectedIndexChanged" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
            }
        }
        protected void convertToPdf(byte[] mssg)
        {
            try
            {
                byte[] tmpfiledata = mssg;
                Session["pdfpath"] = "~/OutData/" + Session["AppID"].ToString() + ".pdf";
                string sPathToSaveFileTo = Server.MapPath("~/OutData/" + Session["AppID"].ToString() + ".pdf");
                using (System.IO.FileStream fs = new System.IO.FileStream(sPathToSaveFileTo, System.IO.FileMode.Create))
                using (System.IO.BinaryWriter bw = new System.IO.BinaryWriter(fs))
                {
                    bw.Write(tmpfiledata);
                    RecordUserAction("convertToImage", "File written to Image", "S");
                    bw.Close();
                }
                WebClient User = new WebClient();
                Byte[] FileBuffer = User.DownloadData(sPathToSaveFileTo);
                embed1.Attributes.Add("src", Session["pdfpath"].ToString());
                if (FileBuffer != null)
                {
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "none", "<script> $(window).on('load', function () { $('#pdfModal').modal({ backdrop: 'static' });});</script>", false);

                }
            }
            catch (Exception ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "convertToPdf()" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath) + " from " + ipaddress);
            }
            //try
            //{
            //    byte[] tmpfiledata = mssg;
            //    string sPathToSaveFileTo = Server.MapPath("~/SelectedFile.pdf");
            //    using (System.IO.FileStream fs = new System.IO.FileStream(sPathToSaveFileTo, System.IO.FileMode.Create, System.IO.FileAccess.ReadWrite))
            //    {

            //        using (System.IO.BinaryWriter bw = new System.IO.BinaryWriter(fs))
            //        {
            //            bw.Write(tmpfiledata);
            //            RecordUserAction("convertToPdf", "Pdf write Failed", "F");
            //            bw.Close();
            //        }
            //    }
            //    WebClient User = new WebClient();
            //    Byte[] FileBuffer = User.DownloadData(Server.HtmlEncode(sPathToSaveFileTo));
            //    if (FileBuffer != null)
            //    {
            //        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "none", "<script> $(window).on('load', function () { $('#pdfModal').modal({ backdrop: 'static' });});</script>", false);
            //        //Response.ContentType = "application/pdf";
            //        //// Response.AddHeader("content-length", FileBuffer.Length.ToString());
            //        //Response.Write("window.open('sPathToSaveFileTo','_blank');");
            //        //Response.BinaryWrite(FileBuffer);
            //    }
            //}
            //catch (Exception ex)
            //{
            //    CreateLogFiles Err = new CreateLogFiles();
            //    Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "convertToPdf()" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
            //}
        }
        protected void LbView_Click(object sender, EventArgs e)
        {
            LinkButton btn = (LinkButton)sender;
            GridViewRow row = (GridViewRow)btn.NamingContainer;
            HiddenField hdobid = row.FindControl("hfobjectID") as HiddenField;
            if (hdobid.Value != null && hdobid.Value != "")
            {
                string objectid = hdobid.Value;
                try
                {
                    var str = ConfigurationManager.AppSettings["mongoconnect"];
                    IMongoDatabase database;
                    IMongoClient client;
                    client = new MongoClient(str);
                    database = client.GetDatabase("eGoaSociety");
                    var collection = database.GetCollection<MemberDocs>("Members Document");
                    var status = collection.Find(x => x._Id == ObjectId.Parse(objectid)).FirstOrDefault();
                    byte[] decrypt_bytes = status.DocContent;
                    byte[] pdf = obj_Byte_Encryption.DecryptData(decrypt_bytes, System.Web.HttpContext.Current.Server.MapPath("~/ENCRYPT.KEY"));
                    convertToPdf(pdf);
                    RecordUserAction("LbView_Click", "Pdf Viewed", "S");
                }
                catch (MongoException ex)
                {
                    CreateLogFiles Err = new CreateLogFiles();
                    Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "LbView_Click" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
                    RecordUserAction("LbView_Click", ex.Message, "F");
                    Response.Write("<script language='javascript'>alert('" + "Pdf Viewed Failed" + "')</script>");
                }

                finally
                {
                    //conn.close();
                }
            }
            else
            {
                Response.Write("<script language='javascript'>alert('" + "Document not available as Member is not part of Managing Committee" + "')</script>");
            }
        }
        protected void grvMemberDetails_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                int head = getheadentrydesignation();
                HiddenField hdesignid = (e.Row.FindControl("hfdesignid") as HiddenField);
                if (hdesignid.Value != null)
                {
                    if (head == Convert.ToInt32(hdesignid.Value))
                    {
                        LinkButton deletebtn = (LinkButton)e.Row.FindControl("LBDelete");
                        if (deletebtn != null)
                        {
                            deletebtn.Enabled = false;
                        }
                        else
                        {
                            deletebtn.Enabled = true;
                        }
                    }

                    if (e.Row.RowType.Equals(DataControlRowType.DataRow))
                    {
                        Label pid = (e.Row.FindControl("lbupdate_proof_id") as Label);
                        string p_id = pid.Text;
                        if (p_id == "5")
                        {
                            Label mid = (e.Row.FindControl("lbupdate_memberid") as Label);
                            string m_id = mid.Text;
                            e.Row.Cells[9].Text = Utility.getAadharNo(m_id);
                            string x = e.Row.Cells[9].Text;
                            e.Row.Cells[9].Text = _encryption.Decrypt(x, System.Web.HttpContext.Current.Server.MapPath("~/ENCRYPT.KEY"));
                            e.Row.Cells[9].Text = Server.HtmlEncode(Utility.MaskMobile(e.Row.Cells[9].Text, 0, "XXXXXXXX"));
                        }
                        else
                        {
                            string x = e.Row.Cells[9].Text;
                            e.Row.Cells[9].Text = _encryption.Decrypt(x, System.Web.HttpContext.Current.Server.MapPath("~/ENCRYPT.KEY"));
                            e.Row.Cells[9].Text = Server.HtmlEncode(Utility.MaskMobile(e.Row.Cells[9].Text, 0, "XXXXXXXX"));
                        }








                    }
                    //e.Row.Cells[8].Visible = false;
                    if ((Convert.ToInt32(Session["status_Id"].ToString()) == 1) || (Convert.ToInt32(Session["status_Id"].ToString()) == 2) || (Convert.ToInt32(Session["status_Id"].ToString()) == 5))
                    {

                    }
                    else
                    {
                        ((DataControlField)grvMemberDetails.Columns
                    .Cast<DataControlField>()
                    .Where(fld => fld.HeaderText == "Delete")
                    .SingleOrDefault()).Visible = false;
                        ((DataControlField)grvMemberDetails.Columns
                       .Cast<DataControlField>()
                       .Where(fld => fld.HeaderText == "Edit")
                       .SingleOrDefault()).Visible = false;
                    }

                    if (Convert.ToInt32(Session["Renewal"]) == 1)   // if new appln then hide date of admission
                    {
                        ((DataControlField)grvMemberDetails.Columns
                      .Cast<DataControlField>()
                      .Where(fld => fld.HeaderText == "Date of admission")
                      .SingleOrDefault()).Visible = false;
                    }
                }
            }
            catch (Exception ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "grvMemberDetails_RowDataBound" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
            }
        }

        protected int getheadentrydesignation()
        {
            int retval = -1;

            NpgsqlConnection conn = new NpgsqlConnection();
            NpgsqlCommand cmd = new NpgsqlCommand();
            conn.ConnectionString = ConfigurationManager.ConnectionStrings["connect"].ConnectionString;
            cmd.Connection = conn;
            try
            {
                conn.Open();
                string query = "select applicant_designation from esociety.applicant_details where app_id=@app_id";
                cmd.CommandText = query;
                cmd.Parameters.AddWithValue("@app_id", Convert.ToInt64(Session["AppID"].ToString()));
                NpgsqlDataReader dr = cmd.ExecuteReader();

                if (dr.Read())
                {
                    retval = Convert.ToInt32(dr["applicant_designation"]);
                }
                dr.Close();

            }
            catch (NpgsqlException ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "getheadentrydesignation()" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
                retval = -1;
            }
            finally
            {
                conn.Close();
            }
            return retval;
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            try
            {               
                int a = presidententry();
                if (a == 1)
                {
                    TxtBxRemarks.Text = "";
                    ddl_salutation.SelectedValue = "-1";
                    ddl_salutation.Enabled = true;
                    txt_MemName.Text = "";
                    txt_MemName.Enabled = true;
                    ddl_DesignMem.SelectedValue = "-1";
                    ddl_DesignMem.Enabled = true;
                    row_DesignOthers.Visible = false;
                    ddl_MemOccup.SelectedValue = "-1";
                    row_OccupOthers.Visible = false;
                    ddl_salutation.SelectedValue = "-1";
                    Rdbtngender.ClearSelection();
                    Rdbtngender.Enabled = true;
                    txtbx_age.Text = "";
                    txt_MemAddress.Text = "";
                    ddl_MemDocType.SelectedValue = "-1";
                    ddlMcom.SelectedValue = "1";
                    ddlMcom.Enabled = true;
                    TxtBxDocumentNo.Text = "";
                    lbupload.Text = "";
                    btnAdd.Visible = true;
                    btnUpdate.Visible = false;
                    btnAddOld.Visible = false;
                    TxtBxRemarks.Text = "";
                    btnAdd.Enabled = true;
                    lblError.Text = "";
                    originalAadhar.Value = "";
                    trvisible.Visible = false;
                    if (Convert.ToInt32(Session["Renewal"]) == 2)
                    {
                        tr_dateofadmission.Visible = true;
                    }
                    else
                    {
                        tr_dateofadmission.Visible = false;
                    }
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "none", "<script> $(window).on('load', function () { $('#addmembersmodal').modal({ backdrop: 'static' });});</script>", false);
                }
            }
            catch (Exception ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "Button1_Click" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
            }
        }

        protected void TxtBxDocumentNo_TextChanged(object sender, EventArgs e)
        {
            try
            {           

                lblError.Text = "";
                int tempflag = 0;
                int n = Convert.ToInt16(ddl_MemDocType.SelectedValue);
                switch (n)
                {
                    case 1:
                        if (ddl_MemDocType.SelectedValue == "1")
                        {
                            if (!_val.validateData(TxtBxDocumentNo.Text, _val.epicno))
                            {
                                btnAdd.Enabled = false;
                                btnAddOld.Enabled = false;
                                // btnAdd.Visible = false;
                                btnUpdate.Enabled = false;
                                lblError.Text = "Enter valid election card no";
                                relaunchmodal();
                            }
                            else
                            {
                                tempflag = 1;
                                
                            }
                        }
                        break;
                    case 2:
                        if (ddl_MemDocType.SelectedValue == "2")
                        {
                            if (!_val.validateData(TxtBxDocumentNo.Text, _val.cust_driving))
                            {
                                btnAdd.Enabled = false;
                                btnAddOld.Enabled = false;
                               // btnAdd.Visible = false;
                                btnUpdate.Enabled = false;
                                lblError.Text = "Enter valid Driving License no";
                                relaunchmodal();
                            }
                            else
                            {
                                tempflag = 1;
                            }
                        }
                        break;
                    case 3:
                        if (ddl_MemDocType.SelectedValue == "3")
                        {
                            if (!_val.validateData(TxtBxDocumentNo.Text, _val.pan_regex))
                            {
                                btnAdd.Enabled = false;
                                btnAddOld.Enabled = false;
                                // btnAdd.Visible = false;
                                btnUpdate.Enabled = false;
                                lblError.Text = "Enter valid PAN no";
                                relaunchmodal();
                            }
                            else
                            {
                                tempflag = 1;
                            }
                        }
                        break;
                    case 4:

                        if (ddl_MemDocType.SelectedValue == "4")
                        {
                            if (!_val.validateData(TxtBxDocumentNo.Text, _val.passport))
                            {
                                btnAdd.Enabled = false;
                                btnAddOld.Enabled = false;
                                // btnAdd.Visible = false;
                                btnUpdate.Enabled = false;
                                lblError.Text = "Enter valid Passport no";
                                relaunchmodal();
                            }
                            else
                            {
                                tempflag = 1; lblError.Text = "";
                            }
                        }
                        break;

                    case 5:
                        if (ddl_MemDocType.SelectedValue == "5")
                        {
                            if (!_val.validateData(TxtBxDocumentNo.Text, _val.adhar_regex))
                            {
                                btnAdd.Enabled = false;
                                btnAddOld.Enabled = false;
                                // btnAdd.Visible = false;
                                btnUpdate.Enabled = false;
                                lblError.Text = "Please Enter valid Aadhar Card no";
                                relaunchmodal();
                            }
                            else
                            {
                                tempflag = 1;
                            }
                        }
                        break;
                    //resolvede
                    case 6:
                        if (ddl_MemDocType.SelectedValue == "6")
                        {
                            if (!_val.validateData(TxtBxDocumentNo.Text, _val.alpha_numericregex))
                            {
                                btnAdd.Enabled = false;
                                btnAddOld.Enabled = false;
                                //  btnAdd.Visible = false;
                                btnUpdate.Enabled = false;
                                lblError.Text = "Enter valid no.No special Characters allowed.";
                                relaunchmodal();
                            }
                            else
                            {
                                tempflag = 1;
                                
                            }
                        }
                        break;
                    default:
                        break;

                }

                if(tempflag == 1)
                {
                    if (Convert.ToInt32(HFEdit_flag.Value) == 1)
                    {

                        btnUpdate.Enabled = true;
                        btnAdd.Visible = false;
                        btnAddOld.Visible = false;

                    }
                    else if (ViewState["addoldmember_flag"].ToString() == "1")
                    {
                        btnAdd.Visible = false;
                        btnUpdate.Visible = false;
                        btnAddOld.Visible = true;
                    }
                    else
                    {
                        btnAdd.Enabled = true;
                        btnAddOld.Visible = false;
                    }

                    lblError.Text = "";
                }

                if (Convert.ToInt32(Session["Renewal"]) == 2)
                {
                    tr_dateofadmission.Visible = true;
                }
                else
                {
                    tr_dateofadmission.Visible = false;
                }
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "none", "<script> $(window).on('load', function () { $('#addmembersmodal').modal({ backdrop: 'static' });});</script>", false);

            }
            catch (Exception ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "TxtBxDocumentNo_TextChanged" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
            }
        }

        protected void close_modal_Click(object sender, EventArgs e)
        {
            try
            {
                ViewState["addoldmember_flag"] = "0";
                HFEdit_flag.Value = "0";
                btnAdd.Visible = true;
                lblError.Text = "";
                Status.Text = "";
                TxtBxDocumentNo.Text = "";
                trvisible.Visible = false;
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "none", "<script> $(window).on('load', function () { $('#addmembersmodal').modal('hide');});</script>", false);
            }
            catch (Exception ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "close_modal_Click" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
            }
        }


        protected void Rdbtngender_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (Convert.ToInt32(HFEdit_flag.Value) == 1)
                {
                    btnAdd.Visible = false;
                    btnAddOld.Visible = false;
                    btnUpdate.Visible = true;
                }
                else if (ViewState["addoldmember_flag"].ToString() == "1")
                {
                    btnAddOld.Visible = true;
                    btnAdd.Visible = false;
                    btnUpdate.Visible = false;
                }
                else
                {
                    btnAdd.Visible = true;
                    btnAddOld.Visible = false;
                    btnUpdate.Visible = false;
                }

                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "none", "<script> $(window).on('load', function () { $('#addmembersmodal').modal({ backdrop: 'show' });});</script>", false);

            }
            catch (Exception ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "Rdbtngender_SelectedIndexChanged" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
            }
        }

        protected void ddlMcom_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (ddlMcom.SelectedValue == "1")
                {
                    //upload.Visible = true;
                }
                else if (ddlMcom.SelectedValue == "2")
                {
                    //upload.Visible = false;
                }
                if (Convert.ToInt32(Session["Renewal"]) == 2)
                {
                    tr_dateofadmission.Visible = true;
                }
                else
                {
                    tr_dateofadmission.Visible = false;
                }
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "none", "<script> $(window).on('load', function () { $('#addmembersmodal').modal({ backdrop: 'static' });});</script>", false);

            }
            catch (Exception ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "ddlMcom_SelectedIndexChanged" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
            }
        }

        protected void ddl_MemOccup_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (ddl_MemOccup.SelectedValue == "4")
                {
                    row_OccupOthers.Visible = true;
                }
                else
                {
                    row_OccupOthers.Visible = false;
                    TxtBx_OccupOthers.Text = "";
                }

                if (Convert.ToInt32(HFEdit_flag.Value) == 1)
                {
                    btnAdd.Visible = false;
                    btnAddOld.Visible = false;                    
                    btnUpdate.Visible = true;
                }
                else if (ViewState["addoldmember_flag"].ToString() == "1")
                {
                    btnAddOld.Visible = true;
                    btnAdd.Visible = false;
                    btnUpdate.Visible = false;
                }
                else
                {
                    btnAdd.Visible = true;
                    btnAddOld.Visible = false;
                    btnUpdate.Visible = false;
                }

                // lblError.Text = "";
                if (Convert.ToInt32(Session["Renewal"]) == 2)
                {
                    tr_dateofadmission.Visible = true;
                }
                else
                {
                    tr_dateofadmission.Visible = false;
                }
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "none", "<script> $(window).on('load', function () { $('#addmembersmodal').modal({ backdrop: 'static' });});</script>", false);

            }
            catch (Exception ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "ddl_MemOccup_SelectedIndexChanged" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));

            }

        }


        protected void Btn_memberlistdocupload_Click(object sender, EventArgs e)
        {
        }



        public int fileuploadmemberslist(FileUpload fu, Label lb, OtherDocuments othdoc, string a, string ip, string mac, long App_ID, string collection, int myid)
        {
            int flag = 0;

            if (Session["created_by"] != null)
            {
                try
                {
                    if (fu.HasFile)
                    {
                        HttpPostedFile file = fu.PostedFile;
                        byte[] document = new byte[file.ContentLength];
                        file.InputStream.Read(document, 0, file.ContentLength);
                        UInt32 mimetype;
                        FindMimeFromData(0, null, document, 256, null, 0, out mimetype, 0);
                        IntPtr mimeTypePtr = new IntPtr(mimetype);
                        string mimeType = Marshal.PtrToStringUni(mimeTypePtr).ToLower();
                        Marshal.FreeCoTaskMem(mimeTypePtr);
                        var scanExitCode = 0;//UtilityDAO.VirusScanFile(flupHeader.PostedFile.FileName);
                        if (scanExitCode == 0)
                        {
                            string UploadFileName = file.FileName;
                            string Extension = UploadFileName.Substring(UploadFileName.LastIndexOf('.') + 1).ToLower();
                            var extCount = UploadFileName.Split('.').Length - 1;

                            if (Extension.ToLower() != "pdf" || file.ContentType.ToLower() != "application/pdf" || mimeType != "application/pdf"
                                || extCount > 1
                                || document == null
                                || file.ContentLength <= 0 || file.ContentLength > (2 * 1024 * 1024))
                            {

                                Label48.Text = "File name can not contain multiple dots/extensions. Only Pdf file of maximum size 5MB allowed !";
                                Label48.ForeColor = System.Drawing.Color.Red;
                                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "none", "<script> $(window).on('load', function () { $('#errorModal').modal({ backdrop: 'static' });});</script>", false);
                                flag = 0;
                            }
                            ////////////////////////
                            else
                            {
                                string path = Utility.filesave(FileUpload6, Session["AppID"].ToString());
                                string resultedStr = "";
                                using (StreamReader sr = new StreamReader(path))
                                {
                                    resultedStr = SurroundingClass.ReadStreamWithTimeout(sr);
                                }
                                int scanned = Utility.checkFile(resultedStr);
                                if (scanned == 0)
                                {
                                    Response.Write("<script>alert('Invalid PDF uploaded');</script>");
                                    File.Delete(path);
                                }
                                else
                                {
                                    File.Delete(path);
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
                                    othdoc.UpdatedBy = Session["created_by"].ToString();
                                    othdoc.Doc_ID = document_id;
                                    othdoc.doc_name = a;
                                    othdoc.DocContent = encrypt_bytes;
                                    int mvalue = InsertintoMongoDBlist(othdoc, collection);
                                    if (mvalue == 1)
                                    {
                                        int pvalue = insertmemberlistentryintoPosgres(obj_id, document_id, App_ID, myid);
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
                    }
                    else
                    {
                        lb.ForeColor = System.Drawing.Color.Red;
                        lb.Text = "Upload " + a + " Certificate file in PDF format";
                        Label48.ForeColor = System.Drawing.Color.Red;
                        Label48.Text = "Upload " + a + " Certificate file in PDF format";
                        flag = 0;
                    }

                }
                catch (Exception ex)
                {
                    CreateLogFiles Err = new CreateLogFiles();
                    Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "fileuploadmemberslist()" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
                    lb.Text = "File upload failed.";
                    RecordUserAction("Fileupload Function ", ex.Message, "F");
                    Label48.ForeColor = System.Drawing.Color.Red;
                    Label48.Text = "File upload failed.";
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "none", "<script> $(window).on('load', function () { $('#errorModal').modal({ backdrop: 'static' });});</script>", false);

                }
            }
            return flag;
        }

        public int InsertintoMongoDBlist(OtherDocuments doc, string sel_collection)
        {
            Insert insr = new Insert();
            try
            {
                RecordUserAction("InsertintoMongoDB", "write document to Mongo", "S");
                return insr.InsertMongoOtherDocs(doc, sel_collection);
            }
            catch (MongoException ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "InsertintoMongoDB()" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
                RecordUserAction("InsertintoMongoDB", ex.Message, "F");
                return 0;
            }
        }

        public int insertmemberlistentryintoPosgres(ObjectId obj, string docid, long appid, int myid)
        {
            int value = 0;
            if (Session["AppID"] != null)
            {
                NpgsqlConnection conn = new NpgsqlConnection();
                NpgsqlCommand cmd = new NpgsqlCommand();
                conn.ConnectionString = ConfigurationManager.ConnectionStrings["connect"].ConnectionString;
                cmd.Connection = conn;
                string objectid = obj.ToString();                
                if (Session["AppID"].ToString() == appid.ToString())
                {
                    try
                    {
                        conn.Open();
                        string grm_query = "INSERT INTO esociety.otherdoc_crossentry(object_id, other_docid, app_id,active,myid)VALUES(@object_id,@other_docid, @app_id,'True',@myid)";
                        cmd.CommandText = grm_query;
                        cmd.Parameters.AddWithValue("@object_id", objectid);
                        cmd.Parameters.AddWithValue("@other_docid", docid);
                        cmd.Parameters.AddWithValue("@app_id", appid);
                        cmd.Parameters.AddWithValue("@myid", myid);
                        cmd.ExecuteNonQuery();
                        value = 1;
                        RecordUserAction("insertentryPosgres", "objectid saved to posgres", "S");
                    }
                    catch (NpgsqlException ex)
                    {
                        value = 0;
                        CreateLogFiles Err = new CreateLogFiles();
                        Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "insertmemberlistentryintoPosgres()" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
                        RecordUserAction("insertentryPosgres", ex.Message, "F");
                        var errorcode = ex.Message;
                        if (errorcode.Contains("23503"))
                        {
                            Response.Write("<script language='javascript'>alert('" + "Please Complete Society Details first." + "')</script>");
                        }
                        else
                        {
                            Response.Write("<script language='javascript'>alert('" + "Exection Error" + "')</script>");
                        }

                    }
                    finally
                    {
                        conn.Close();
                    }
                }
                else
                {
                    value = 0;
                    lblError.Text = "Execution error";
                }
            }
            return value;
        }

        protected void Button2_close_Click(object sender, EventArgs e)
        {
            try
            {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "none", "<script> $(window).on('load', function () { $('#memberlistdocumentupload').modal('hide');});</script>", false);

            }
            catch (Exception ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "Button2_close_Click" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath) + " from " + ipaddress);

            }
        }

        protected void Addmemberdocs_Click1(object sender, EventArgs e)
        {

        }

        protected void LB_memberslist_view_Click(object sender, EventArgs e)
        {
            try
            {
                var status = getPageStatus(Convert.ToInt64(Session["AppID"].ToString()), 7);
                string objectid = status.Item2;
                openConnectionMongo_societyDocs(objectid);
                RecordUserAction("lkbutton_Click", "User Clicked on lkbutton_Click", "S");
            }
            catch (Exception ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "LB_memberslist_view_Click" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
            }
        }


        protected byte[] openConnectionMongo_societyDocs(string objectid)//society docuemnt
        {
            try
            {
                var str = ConfigurationManager.AppSettings["mongoconnect"];
                IMongoDatabase database;
                IMongoClient client;
                client = new MongoClient(str);
                database = client.GetDatabase("eGoaSociety");
                var collection = database.GetCollection<OtherDocuments>("Society Documents");
                var status = collection.Find(x => x._Id == ObjectId.Parse(objectid)).FirstOrDefault();
                byte[] decrypt_pdf_bytes = status.DocContent;
                byte[] pdf = obj_Byte_Encryption.DecryptData(decrypt_pdf_bytes, System.Web.HttpContext.Current.Server.MapPath("~/ENCRYPT.KEY"));
                convertToPdf(pdf);
                RecordUserAction("openConnectionMongo_societyDocs", "byte[] to function", "S");
                return pdf;
            }
            catch (MongoException ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "openConnectionMongo_societyDocs()" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
                RecordUserAction("openConnectionMongo_societyDocs", ex.Message, "F");
                Response.Write("<script language='javascript'>alert('" + "PDF View Failed" + ".No file uploaded yet')</script>");
                return null;
            }
        }

        protected void LB_memberslist_delete_Click(object sender, EventArgs e)
        {
            try
            {
                var status = getPageStatus(Convert.ToInt64(Session["AppID"].ToString()), 7);
                string objectid = status.Item2;
                if (objectid == "" || objectid == null)
                {
                    Response.Write("<script language='javascript'>alert('File Not present to delete')</script>");
                }
                else
                {

                    int a = DeleteFunction(objectid);
                    if (a == 1)
                    {
                        lbfu1status.Text = "File Deleted";
                        Response.Write("<script language='javascript'>alert('Deleted Successfully')</script>");
                        LB_memberslist_view.Enabled = false;
                        LB_memberslist_delete.Enabled = false;
                        LB_memberslist_upload.Enabled = true;
                        FileUpload1.Enabled = true;
                        Session["ml_flag"] = 0;
                    }
                    else
                    {
                        lbfu1status.Text = "Failed...Kindly try again";
                        Response.Write("<script language='javascript'>alert('Failed...Kindly try again')</script>");
                    }
                }
            }
            catch (Exception ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "LB_memberslist_delete_Click" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
            }
        }

        protected int DeleteFunction(string obj)
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
                string select = "SELECT app_id, other_docid, myid FROM esociety.otherdoc_crossentry where object_id=@object_id";
                cmd.CommandText = select;
                cmd.Parameters.AddWithValue("@object_id", obj);
                NpgsqlDataReader rd = cmd.ExecuteReader();
                if (rd.Read())
                {
                    ViewState["object_id"] = Server.HtmlEncode(obj);
                    ViewState["app_id"] = Server.HtmlEncode(rd["app_id"].ToString());
                    ViewState["other_docid"] = Server.HtmlEncode(rd["other_docid"].ToString());
                    ViewState["myid"] = Server.HtmlEncode(rd["myid"].ToString());
                }
                rd.Close();
                cmd.Parameters.Clear();
                string hist_inr = "INSERT INTO esociety.history_otherdoc_crossentry(object_id, app_id, other_docid, active, myid)VALUES (@object_id, @app_id, @other_docid, 'N', @myid)";
                cmd.CommandText = hist_inr;
                cmd.Parameters.AddWithValue("@object_id", ViewState["object_id"].ToString());
                cmd.Parameters.AddWithValue("@app_id", Convert.ToInt64(ViewState["app_id"]));
                cmd.Parameters.AddWithValue("@other_docid", ViewState["other_docid"].ToString());
                cmd.Parameters.AddWithValue("@myid", Convert.ToInt16(ViewState["myid"]));
                cmd.ExecuteNonQuery();
                cmd.Parameters.Clear();
                string delete = "delete from esociety.otherdoc_crossentry where object_id=@obj";
                cmd.CommandText = delete;
                cmd.Parameters.AddWithValue("@obj", ViewState["object_id"].ToString());
                cmd.ExecuteNonQuery();
                cmd.Parameters.Clear();

                myTrans.Commit();
                return 1;
            }
            catch (NpgsqlException ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "DeleteFunction()" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));

                myTrans.Rollback();
                RecordUserAction("DeleteFunction", ex.Message, "F");
                Response.Write("<script language='javascript'>alert('" + "Bind Error while Deletion of members list" + "')</script>");
                return 0;
            }
            finally
            {
                conn.Close();
            }
        }

        public static Tuple<string, string> getPageStatus(Int64 app_id, int myid)
        {
            string active = "False";
            string objectid = "";
            var tuple = new Tuple<string, string>(active, objectid);
            NpgsqlConnection conn = new NpgsqlConnection();
            NpgsqlCommand cmd = new NpgsqlCommand();
            conn.ConnectionString = ConfigurationManager.ConnectionStrings["connect"].ConnectionString;
            cmd.Connection = conn;
            try
            {
                conn.Open();
                string query = "SELECT active,object_id FROM esociety.otherdoc_crossentry where app_id=@appid and myid=@myid";
                cmd.CommandText = query;
                cmd.Parameters.AddWithValue("@appid", app_id);
                cmd.Parameters.AddWithValue("@myid", myid);
                NpgsqlDataReader rd = cmd.ExecuteReader();
                //var tuple = new Tuple<string, string>(active, objectid);
                if (rd.Read())
                {
                    active = rd["active"].ToString();
                    objectid = rd["object_id"].ToString();
                    tuple = new Tuple<string, string>(active, objectid); 
                }
                else
                {
                    tuple = new Tuple<string, string>(active, objectid); 
                }
                rd.Close();

            }
            catch (NpgsqlException ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(HttpContext.Current.Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "getPageStatus()" + " MemberDetails.aspx");
                //RecordUserAction("getPageStatus()", ex.Message, "S");
                //Response.Write("<script language='javascript'>alert('" + ex.Message + "')</script>");
                 tuple = new Tuple<string, string>(active, objectid);
            }
            finally
            {
                conn.Close();
            }
            return tuple;
        }

        protected void LB_memberslist_upload_Click(object sender, EventArgs e)  // myid=7 for members list 
        {
            try
            {
                if (Session["AppID"] != null)
                {
                    int a = fileuploadmemberslist(FileUpload1, lbfu1status, doc, "Members List", ipaddress, macaddress, Convert.ToInt64(Session["AppID"].ToString()), "Society Documents", 7);
                    if (a == 1)
                    {
                        lbfu1status.ForeColor = System.Drawing.Color.Green;
                        lbfu1status.Text = "Uploaded Successfully";
                        RecordUserAction("LB_Application_Upload_Click", "File Uploaded Successfully", "S");
                        LB_memberslist_view.Enabled = true;
                        LB_memberslist_delete.Enabled = true;
                        FileUpload1.Enabled = false;
                        LB_memberslist_upload.Enabled = false;
                        Session["ml_flag"] = 1;
                    }
                    else
                    {
                        RecordUserAction("LB_Application_Upload_Click", "File Uploaded Failed", "F");
                        lbfu1status.ForeColor = System.Drawing.Color.Red;
                        lbfu1status.Text = "Failed";
                        Session["ml_flag"] = 0;
                    }
                }
                else
                {
                    RecordUserAction("LB_Application_Upload_Click", "Session null", "F");
                }
            }
            catch (Exception ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "LB_memberslist_upload_Click" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
            }
        }

        protected int getHeadEntry(string app_id)
        {
            int retval = -1;

            NpgsqlConnection conn = new NpgsqlConnection();
            NpgsqlCommand cmd = new NpgsqlCommand();
            conn.ConnectionString = ConfigurationManager.ConnectionStrings["connect"].ConnectionString;
            cmd.Connection = conn;
            try
            {
                conn.Open();
                string query = "select head_entry from esociety.applicant_details where app_id=@app_id";
                cmd.CommandText = query;
                cmd.Parameters.AddWithValue("@app_id", Convert.ToInt64(app_id));
                NpgsqlDataReader dr = cmd.ExecuteReader();

                if (dr.Read())
                {
                    retval = Convert.ToInt32(dr["head_entry"]);
                }
                dr.Close();

            }
            catch (NpgsqlException ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "getHeadEntry()" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
                retval = -1;
            }
            finally
            {
                conn.Close();
            }
            return retval;
        }

        protected void CheckMembersListDoc()
        {
            int retval = -1;

            NpgsqlConnection conn = new NpgsqlConnection();
            NpgsqlCommand cmd = new NpgsqlCommand();
            conn.ConnectionString = ConfigurationManager.ConnectionStrings["connect"].ConnectionString;
            cmd.Connection = conn;
            try
            {
                conn.Open();
                string query = "select head_entry from esociety.applicant_details where app_id=@app_id";
                cmd.CommandText = query;
                cmd.Parameters.AddWithValue("@appid", Convert.ToInt64(Session["OldAppId"].ToString()));
                NpgsqlDataReader dr = cmd.ExecuteReader();

                if (dr.Read())
                {
                    retval = Convert.ToInt32(dr["head_entry"]);
                }
                dr.Close();

            }
            catch (NpgsqlException ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "CheckMembersListDoc()" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
                retval = -1;
            }
            finally
            {
                conn.Close();
            }

        }
        protected void grvOldMemberDetails_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType.Equals(DataControlRowType.DataRow))
                {
                    Label pid = (e.Row.FindControl("lbold_proof_id") as Label);
                    string p_id = pid.Text;
                    if (p_id == "5")
                    {
                        Label mid = (e.Row.FindControl("lbmold_memberid") as Label);
                        string m_id = mid.Text;
                        e.Row.Cells[9].Text = Utility.getAadharNo(m_id);
                        string x = e.Row.Cells[9].Text;
                        e.Row.Cells[9].Text = _encryption.Decrypt(x, System.Web.HttpContext.Current.Server.MapPath("~/ENCRYPT.KEY"));
                        e.Row.Cells[9].Text = Server.HtmlEncode(Utility.MaskMobile(e.Row.Cells[9].Text, 0, "XXXXXXXX"));
                    }
                    else
                    {
                        string x = e.Row.Cells[9].Text;
                        e.Row.Cells[9].Text = _encryption.Decrypt(x, System.Web.HttpContext.Current.Server.MapPath("~/ENCRYPT.KEY"));
                        e.Row.Cells[9].Text = Server.HtmlEncode(Utility.MaskMobile(e.Row.Cells[9].Text, 0, "XXXXXXXX"));
                    }
                }
                //e.Row.Cells[8].Visible = false;
                if (Convert.ToInt32(Session["status_Id"]) == 1 || Convert.ToInt32(Session["status_Id"]) == 2 || Convert.ToInt32(Session["status_Id"]) == 5)
                {

                }
                else
                {
                    ((DataControlField)grvMemberDetails.Columns
                .Cast<DataControlField>()
                .Where(fld => fld.HeaderText == "Add")
                .SingleOrDefault()).Visible = false;
                    
                }

                
            }
            catch (Exception ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "grvOldMemberDetails_RowDataBound" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
            }
        }

        protected void LBAdd_Click(object sender, EventArgs e)
        {
            ViewState["addoldmember_flag"] = '1';
            btnAdd.Visible = false;
            btnUpdate.Visible = false;
            btnAddOld.Visible = true;

            string status = "";
            if (Session["AppID"] != null)
            {
                if (Convert.ToInt32(Session["Renewal"]) == 2)
                {
                    tr_dateofadmission.Visible = true;
                }
                else
                {
                    tr_dateofadmission.Visible = false;
                }
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "none", "<script> $(window).on('load', function () { $('#addmembersmodal').modal({ backdrop: 'static' });});</script>", false);

                LinkButton btn = (LinkButton)sender;
                GridViewRow row = (GridViewRow)btn.NamingContainer;
                //which value you are using
                HiddenField hd = row.FindControl("hfmemIDOld") as HiddenField;
                HiddenField hdobid = row.FindControl("hfobjectIDOld") as HiddenField;

                //if (hd.Value != null && hdobid.Value != null)
                if (hd.Value != null)
                {
                    string object_ID = hdobid.Value;
                    Int64 memberID = Convert.ToInt64(hd.Value);
                    //document_mongoentry
                    setData(memberID);
                    if (hdobid.Value != null && hdobid.Value != "")
                    {
                        status = openConnectionMongo(object_ID);
                    }

                    Member_Details oldmember = ins.FetchMember(Session["OldAppID"].ToString(), memberID);

                    txt_MemName.Text = oldmember.fname;
                    ddl_DesignMem.SelectedValue = oldmember.design.ToString();
                    TxtBx_DesignOthers.Text = oldmember.designtaion_others;
                    if (ddl_DesignMem.SelectedValue == "8")
                    {
                        row_DesignOthers.Visible = true;
                    }
                    else
                    {
                        row_DesignOthers.Visible = false;
                    }
                    ddl_MemOccup.SelectedValue = oldmember.occupatid.ToString();
                    TxtBx_OccupOthers.Text = oldmember.occupation_others;
                    if (ddl_MemOccup.SelectedValue == "4")
                    {
                        row_OccupOthers.Visible = true;
                    }
                    else
                    {
                        row_OccupOthers.Visible = false;
                    }
                    txt_MemAddress.Text = oldmember.address;
                    ddl_MemDocType.SelectedValue = oldmember.proofid.ToString();
                    if (oldmember.salutation_id.ToString() == "" || oldmember.salutation_id.ToString() == null)
                    {
                        ddl_salutation.SelectedValue = "1";
                    }
                    else
                    {
                        ddl_salutation.SelectedValue = oldmember.salutation_id.ToString();
                    }

                    if (oldmember.gender.ToString() == "" || oldmember.gender.ToString() == null)
                    {
                        Rdbtngender.SelectedValue = Server.HtmlEncode("M");
                    }
                    else
                    {
                        Rdbtngender.SelectedValue = oldmember.gender;
                    }
                    if (oldmember.age.ToString() == "" || (oldmember.age.ToString() == null))
                    {
                        txtbx_age.Text = "0";
                    }
                    else
                    {
                        txtbx_age.Text = oldmember.age.ToString();
                    }
                    string mangcomm = oldmember.mangcomm.Trim();
                   // ddlMcom.SelectedItem.Text = oldmember.mangcomm;
                   if(mangcomm == "Yes")
                    {
                        ddlMcom.SelectedValue = "1";
                    }
                   else if(mangcomm == "No")
                    {
                        ddlMcom.SelectedValue = "2";
                    }

                    if (Convert.ToInt32(ddl_MemDocType.SelectedValue) == 6)
                    {
                        Label2.Text = "Other Document" + " No";
                    }
                    else
                    {
                        string value = ddl_MemDocType.SelectedItem.Text;
                        Label2.Text = value + " No";
                    }
                    string decrypt_docno = oldmember.proof_document_no;
                    TxtBxDocumentNo.Text = _encryption.Decrypt(decrypt_docno, System.Web.HttpContext.Current.Server.MapPath("~/ENCRYPT.KEY"));
                    TxtBxRemarks.Text = oldmember.remarks;

                    if (status == "True")
                    {
                        lbupload.Text = "File is Present.Old File will be replaced with new File";
                    }
                    else
                    {
                        lbupload.Text = "";
                    }
                    ViewState["document_mongoentry"] = oldmember.document_mongoentry;
                    ViewState["doc_id"] = oldmember.doc_id;

                    txtbxdateadmission.Text = Server.HtmlEncode(Convert.ToDateTime(oldmember.dateofadmission).ToString("yyyy-MM-dd"));

                    RecordUserAction("LBAdd_Click", "Gridview Data to textbox", "S");
                    bindgridview(Session["AppID"].ToString());

                    tr_dateofadmission.Visible = true;
                    btnUpdate.Visible = false;
                    trvisible.Visible = true;
                   


                }
                else
                {
                    Response.Write("<script language='javascript'>alert('" + "Not Updated" + "')</script>");
                }
                Rdbtngender.Enabled = false;
                txt_MemName.Enabled = false;
                ddlMcom.Enabled = true;
                ddl_MemDocType.Enabled = true;
            }
            else
            {
                RecordUserAction("LBAdd_Click", "Session null", "F");
            }
        }

        protected void LbViewOld_Click(object sender, EventArgs e)
        {
            LinkButton btn = (LinkButton)sender;
            GridViewRow row = (GridViewRow)btn.NamingContainer;
            HiddenField hdobid = row.FindControl("hfobjectIDOld") as HiddenField;
            if (hdobid.Value != null && hdobid.Value != "")
            {
                string objectid = hdobid.Value;
                try
                {
                    var str = ConfigurationManager.AppSettings["mongoconnect"];
                    IMongoDatabase database;
                    IMongoClient client;
                    client = new MongoClient(str);
                    database = client.GetDatabase("eGoaSociety");
                    var collection = database.GetCollection<MemberDocs>("Members Document");
                    var status = collection.Find(x => x._Id == ObjectId.Parse(objectid)).FirstOrDefault();
                    byte[] decrypt_bytes = status.DocContent;
                    byte[] pdf = obj_Byte_Encryption.DecryptData(decrypt_bytes, System.Web.HttpContext.Current.Server.MapPath("~/ENCRYPT.KEY"));
                    convertToPdf(pdf);
                    RecordUserAction("LbViewOld_Click", "Pdf Viewed", "S");
                }
                catch (MongoException ex)
                {
                    CreateLogFiles Err = new CreateLogFiles();
                    Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "LbViewOld_Click" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
                    RecordUserAction("LbViewOld_Click", ex.Message, "F");
                    Response.Write("<script language='javascript'>alert('" + "Pdf Viewed Failed" + "')</script>");
                }

                finally
                {
                    //conn.close();
                }
            }
            else
            {
                Response.Write("<script language='javascript'>alert('" + "Document not available as Member is not part of Managing Committee" + "')</script>");
            }
        }

        protected void btnAddOld_Click(object sender, EventArgs e)
        {
            if (Session["AppID"] != null && Session["created_by"] != null)
            {

                string doc_no = Encrypt.DecryptStringAES(hfencryptNo.Value.ToString(), Session["Enc_Random"].ToString(), Session["Enc_Vector"].ToString());



                NpgsqlConnection connect = new NpgsqlConnection();
                NpgsqlCommand cmd = new NpgsqlCommand();
                connect.ConnectionString = ConfigurationManager.ConnectionStrings["connect"].ConnectionString;
                cmd.Connection = connect;


                if (ddl_salutation.SelectedValue == "-1")
                {
                    lblError.Text = "Please select Salutation!";
                    relaunchmodal();
                }
                else if (txt_MemName.Text == "" || txt_MemName.Text == null)
                {
                    lblError.Text = "Please Enter Name!";
                    relaunchmodal();
                }
                else if (txtbx_age.Text == "" || txtbx_age.Text == null)
                {
                    lblError.Text = "Please Enter Age!";
                    relaunchmodal();
                }
                else if (!_val.validateData(txt_MemName.Text, _val.alpharegex1))
                {
                    lblError.Text = "Please Enter Valid Name!";
                    relaunchmodal();
                }
                else if (!_val.validateData(txtbx_age.Text, _val.captcha_regex))
                {
                    lblError.Text = "Please Enter Valid Age!";
                    relaunchmodal();
                }
                else if (Int32.Parse(txtbx_age.Text) < 18)
                {
                    lblError.Text = "Minimum age allowed is 18!";
                    relaunchmodal();
                }
                else if (Int32.Parse(txtbx_age.Text) > 115)
                {
                    lblError.Text = "Age cannot be greater than 115!";
                    relaunchmodal();
                }
                else if (txt_MemAddress.Text == "" || txt_MemAddress.Text == null)
                {
                    lblError.Text = "Please Enter Address!";
                    relaunchmodal();
                }
                else if (!_val.validateData(txt_MemAddress.Text, _val.reamrks_validation))
                {
                    lblError.Text = "Please Enter Valid Address!";
                    relaunchmodal();
                }

                else if (ddl_DesignMem.SelectedValue == "-1")
                {
                    lblError.Text = "Please select Designation of the member!";
                    relaunchmodal();
                }
                else if (ddl_DesignMem.SelectedValue == "8" && (TxtBx_DesignOthers.Text == "" || TxtBx_DesignOthers.Text == null))
                {
                    lblError.Text = "Please Enter Other Designation!";
                    relaunchmodal();
                }
                else if (ddl_DesignMem.SelectedValue == "8" && (!_val.validateData(TxtBx_DesignOthers.Text, _val.alpharegex)))
                {
                    lblError.Text = "Please Enter Valid Designation!";
                    relaunchmodal();
                }
                else if (ddl_MemOccup.SelectedValue == "-1")
                {
                    lblError.Text = "Please Enter Occupation!";
                    relaunchmodal();
                }
                else if (ddl_MemOccup.SelectedValue == "4" && (TxtBx_OccupOthers.Text == "" || TxtBx_OccupOthers.Text == null))
                {
                    lblError.Text = "Please Enter Other Occupation!";
                    relaunchmodal();
                }
                else if (ddl_MemOccup.SelectedValue == "4" && (!_val.validateData(TxtBx_OccupOthers.Text, _val.alpharegex)))
                {
                    lblError.Text = "Please Enter Valid Occupation!";
                    relaunchmodal();
                }
                else if (ddl_MemDocType.SelectedValue == "-1")
                {
                    lblError.Text = "Please select Type of document to be added!";
                    relaunchmodal();
                }
                else if (doc_no == "" || doc_no == null)
                {
                    lblError.Text = "Please Enter document no !";
                    relaunchmodal();
                }
                else if (ddlMcom.SelectedValue == "0")
                {
                    lblError.Text = "Please select part of Managing Committee or not";
                    relaunchmodal();
                }
                else if (ddl_MemDocType.SelectedValue == "1" && (!_val.validateData(doc_no, _val.epicno)))
                {
                    lblError.Text = "Please Enter valid election card no";
                    relaunchmodal();
                }
                else if (ddl_MemDocType.SelectedValue == "2" && (!_val.validateData(doc_no, _val.cust_driving)))
                {
                    lblError.Text = "Please Enter valid Driving License no";
                    relaunchmodal();
                }
                else if (ddl_MemDocType.SelectedValue == "3" && (!_val.validateData(doc_no, _val.pan_regex)))
                {
                    lblError.Text = "Please Enter valid Pan card no";
                    relaunchmodal();
                }
                else if (ddl_MemDocType.SelectedValue == "4" && (!_val.validateData(doc_no, _val.passport)))
                {
                    lblError.Text = "Please Enter valid Passport no";
                    relaunchmodal();
                }
                else if (ddl_MemDocType.SelectedValue == "6" && (!_val.validateData(doc_no, _val.alpha_numericregex))) //for others
                {
                    lblError.Text = "Please Enter valid Identity Proof no";
                    relaunchmodal();
                }
                else if (ddl_MemDocType.SelectedValue == "5" && (!_val.validateData(doc_no, _val.adhar_regex)))
                {
                    lblError.Text = "Please Enter valid Aadhar Card no";
                    relaunchmodal();
                }
                else if (ddl_DesignMem.SelectedValue != "7" && check_entry(Convert.ToInt32(ddl_DesignMem.SelectedValue)) == 1)
                {
                    lblError.Text = "Designation is held by some other Member of Society!";
                    relaunchmodal();
                }
                else if (Convert.ToInt32(Session["Renewal"]) == 2 && (txtbxdateadmission.Text == null || txtbxdateadmission.Text == ""))
                {
                    lblError.Text = "Date of Admission is Blank";
                    relaunchmodal();
                }
                else
                {

                    int entry_exist = getheadEntry(Session["AppID"].ToString());
                    lblError.Text = "";
                    int a = 0;
                    Int64 mid = Utility.get_memberID();
                    MemberDocs document = new MemberDocs();
                    string doc_name = ddl_MemDocType.SelectedItem.Text;
                    try
                    {
                        if (FileUpload6.HasFile)
                        {
                            a = fileuploadfucntion(FileUpload6, Status, document, doc_name, ipaddress, macaddress, Convert.ToInt64(Session["AppID"].ToString()), "Members Document", mid);

                        }
                        else
                        {
                            a = 2;
                        }

                        string query = "";
                        connect.Open();
                        int proofid = Convert.ToInt32(ddl_MemDocType.SelectedValue);
                        if (proofid == 5)
                        {
                            string encrypt_docno = _encryption.Encrypt(doc_no, System.Web.HttpContext.Current.Server.MapPath("~/ENCRYPT.KEY"));
                            var str = ConfigurationManager.AppSettings["aadharvault"];
                            IMongoDatabase database;
                            IMongoClient client;
                            client = new MongoClient(str);
                            Aadhar adr = new Aadhar();
                            adr.App_ID = Session["AppID"].ToString();
                            adr.Member_ID = mid.ToString();
                            ObjectId objectID = ObjectId.GenerateNewId();
                            adr._Id = objectID;
                            adr.Doc_ID = encrypt_docno;
                            database = client.GetDatabase("eSocietyAadharVault");
                            var collection = database.GetCollection<Aadhar>("aadhjarVault");
                            collection.InsertOne(adr);
                            if (a == 1)
                            {
                                query = "Update esociety.members set fname=@fname, designtaion=@designtaion, design=@design, occupation=@occupation, occupatid=@occupatid, address=@address, proofname=@proofname,";
                                query = query + " proofid =@proofid, mangcomm=@mangcomm,created_at=current_timestamp, created_by=@created_by, ipaddress=@ipaddress, macaddress=@macaddress, active='Y',proof_document_no=@proof_document_no,";
                                query = query + " salutation_id =@salutation_id,salutation=@salutation,gender=@gender,age=@age,designtaion_others=@designtaion_others,occupation_others=@occupation_others,remarks=@remarks,dateofadmission=@dateofadmission,flag_aadhar = 'C' where app_id=@app_id and member_id=@mid";

                            }
                            else if (a == 2)
                            {
                                query = "INSERT INTO esociety.members(member_id,app_id,fname,designtaion,design,occupation,occupatid,address,proofname,proofid,mangcomm,created_at,created_by,ipaddress,macaddress,";
                                query = query + " active,document_mongoentry,doc_id,proof_document_no,salutation_id,salutation,gender,age,designtaion_others,occupation_others,remarks,dateofadmission,flag_aadhar)VALUES(@mid,@app_id,@fname,@designtaion,@design,@occupation,@occupatid,@address,";
                                query = query + " @proofname,@proofid,@mangcomm,current_timestamp,@created_by,@ipaddress,@macaddress,'Y',@document_mongoentry,@doc_id,@proof_document_no,@salutation_id,@salutation,@gender,@age,@designtaion_others,@occupation_others,@remarks,@dateofadmission,'C')";

                            }
                            cmd.CommandText = query;
                            cmd.Parameters.AddWithValue("@mid", mid);
                            cmd.Parameters.AddWithValue("@app_id", Convert.ToInt64(Session["AppID"].ToString()));
                            cmd.Parameters.AddWithValue("@fname", txt_MemName.Text);
                            cmd.Parameters.AddWithValue("@designtaion", ddl_DesignMem.SelectedItem.Text);
                            cmd.Parameters.AddWithValue("@design", Convert.ToInt32(ddl_DesignMem.SelectedValue));
                            cmd.Parameters.AddWithValue("@occupation", ddl_MemOccup.SelectedItem.Text);
                            cmd.Parameters.AddWithValue("@occupatid", Convert.ToInt32(ddl_MemOccup.SelectedValue));
                            cmd.Parameters.AddWithValue("@address", txt_MemAddress.Text);
                            cmd.Parameters.AddWithValue("@proofname", ddl_MemDocType.SelectedItem.Text);
                            cmd.Parameters.AddWithValue("@proofid", Convert.ToInt32(ddl_MemDocType.SelectedValue));
                            cmd.Parameters.AddWithValue("@mangcomm", ddlMcom.SelectedItem.Text.Trim());
                            cmd.Parameters.AddWithValue("@created_by", Session["created_by"].ToString());
                            cmd.Parameters.AddWithValue("@ipaddress", ipaddress);
                            cmd.Parameters.AddWithValue("@macaddress", macaddress);
                            cmd.Parameters.AddWithValue("@salutation_id", Convert.ToInt32(ddl_salutation.SelectedValue));
                            cmd.Parameters.AddWithValue("@salutation", ddl_salutation.SelectedItem.Text);
                            cmd.Parameters.AddWithValue("@gender", Rdbtngender.SelectedValue);
                            cmd.Parameters.AddWithValue("@age", Convert.ToInt32(txtbx_age.Text));
                            cmd.Parameters.AddWithValue("@proof_document_no", objectID.ToString());
                            cmd.Parameters.AddWithValue("@designtaion_others", TxtBx_DesignOthers.Text);
                            cmd.Parameters.AddWithValue("@occupation_others", TxtBx_OccupOthers.Text);
                            cmd.Parameters.AddWithValue("@document_mongoentry", ViewState["document_mongoentry"].ToString());
                            cmd.Parameters.AddWithValue("@doc_id", ViewState["doc_id"].ToString());
                            cmd.Parameters.AddWithValue("@remarks", TxtBxRemarks.Text);
                            cmd.Parameters.AddWithValue("@remarks", Convert.ToDateTime(txtbxdateadmission.Text, french).Date);

                            if (Convert.ToInt32(Session["Renewal"]) == 2) // if renewal
                            {
                                cmd.Parameters.AddWithValue("@dateofadmission", Convert.ToDateTime(txtbxdateadmission.Text, french).Date);
                            }
                            else
                            {
                                cmd.Parameters.AddWithValue("@dateofadmission", DBNull.Value);
                            }
                            cmd.ExecuteNonQuery();
                        }
                        else
                        {
                            if (a == 1)
                            {
                                query = "Update esociety.members set fname=@fname, designtaion=@designtaion, design=@design, occupation=@occupation, occupatid=@occupatid, address=@address, proofname=@proofname,";
                                query = query + " proofid =@proofid, mangcomm=@mangcomm,created_at=current_timestamp, created_by=@created_by, ipaddress=@ipaddress, macaddress=@macaddress, active='Y',proof_document_no=@proof_document_no,";
                                query = query + " salutation_id =@salutation_id,salutation=@salutation,gender=@gender,age=@age,designtaion_others=@designtaion_others,occupation_others=@occupation_others,remarks=@remarks,dateofadmission=@dateofadmission where app_id=@app_id and member_id=@mid";

                            }
                            else if (a == 2)
                            {
                                query = "INSERT INTO esociety.members(member_id,app_id,fname,designtaion,design,occupation,occupatid,address,proofname,proofid,mangcomm,created_at,created_by,ipaddress,macaddress,";
                                query = query + " active,document_mongoentry,doc_id,proof_document_no,salutation_id,salutation,gender,age,designtaion_others,occupation_others,remarks,dateofadmission)VALUES(@mid,@app_id,@fname,@designtaion,@design,@occupation,@occupatid,@address,";
                                query = query + " @proofname,@proofid,@mangcomm,current_timestamp,@created_by,@ipaddress,@macaddress,'Y',@document_mongoentry,@doc_id,@proof_document_no,@salutation_id,@salutation,@gender,@age,@designtaion_others,@occupation_others,@remarks,@dateofadmission)";

                            }
                            cmd.CommandText = query;
                            cmd.Parameters.AddWithValue("@mid", mid);
                            cmd.Parameters.AddWithValue("@app_id", Convert.ToInt64(Session["AppID"].ToString()));
                            cmd.Parameters.AddWithValue("@fname", txt_MemName.Text);
                            cmd.Parameters.AddWithValue("@designtaion", ddl_DesignMem.SelectedItem.Text);
                            cmd.Parameters.AddWithValue("@design", Convert.ToInt32(ddl_DesignMem.SelectedValue));
                            cmd.Parameters.AddWithValue("@occupation", ddl_MemOccup.SelectedItem.Text);
                            cmd.Parameters.AddWithValue("@occupatid", Convert.ToInt32(ddl_MemOccup.SelectedValue));
                            cmd.Parameters.AddWithValue("@address", txt_MemAddress.Text);
                            cmd.Parameters.AddWithValue("@proofname", ddl_MemDocType.SelectedItem.Text);
                            cmd.Parameters.AddWithValue("@proofid", Convert.ToInt32(ddl_MemDocType.SelectedValue));
                            cmd.Parameters.AddWithValue("@mangcomm", ddlMcom.SelectedItem.Text.Trim());
                            cmd.Parameters.AddWithValue("@created_by", Session["created_by"].ToString());
                            cmd.Parameters.AddWithValue("@ipaddress", ipaddress);
                            cmd.Parameters.AddWithValue("@macaddress", macaddress);
                            cmd.Parameters.AddWithValue("@salutation_id", Convert.ToInt32(ddl_salutation.SelectedValue));
                            cmd.Parameters.AddWithValue("@salutation", ddl_salutation.SelectedItem.Text);
                            cmd.Parameters.AddWithValue("@gender", Rdbtngender.SelectedValue);
                            cmd.Parameters.AddWithValue("@age", Convert.ToInt32(txtbx_age.Text));
                            string encrypt_docno = _encryption.Encrypt(doc_no, System.Web.HttpContext.Current.Server.MapPath("~/ENCRYPT.KEY"));
                            cmd.Parameters.AddWithValue("@proof_document_no", encrypt_docno);
                            cmd.Parameters.AddWithValue("@designtaion_others", TxtBx_DesignOthers.Text);
                            cmd.Parameters.AddWithValue("@occupation_others", TxtBx_OccupOthers.Text);
                            cmd.Parameters.AddWithValue("@document_mongoentry", ViewState["document_mongoentry"].ToString());
                            cmd.Parameters.AddWithValue("@doc_id", ViewState["doc_id"].ToString());
                            cmd.Parameters.AddWithValue("@remarks", TxtBxRemarks.Text);
                            cmd.Parameters.AddWithValue("@remarks", Convert.ToDateTime(txtbxdateadmission.Text, french).Date);

                            if (Convert.ToInt32(Session["Renewal"]) == 2) // if renewal
                            {
                                cmd.Parameters.AddWithValue("@dateofadmission", Convert.ToDateTime(txtbxdateadmission.Text, french).Date);
                            }
                            else
                            {
                                cmd.Parameters.AddWithValue("@dateofadmission", DBNull.Value);
                            }


                            cmd.ExecuteNonQuery();
                        }

                        cmd.Parameters.Clear();
                        grvMemberDetails.Visible = true;
                        bindgridview(Session["AppID"].ToString());
                        RecordUserAction("btnAddOld_Click", "Old Member Data Added New Committee Successfully", "S");
                        txt_MemAddress.Text = "";
                        txt_MemName.Text = "";
                        ddl_MemOccup.SelectedValue = "-1";
                        ddl_MemDocType.SelectedValue = "-1";
                        ddl_DesignMem.SelectedValue = "-1";
                        TxtBx_DesignOthers.Text = "";
                        TxtBx_OccupOthers.Text = "";
                        TxtBxDocumentNo.Text = "";
                        ddl_salutation.SelectedValue = "-1";
                        Rdbtngender.ClearSelection();
                        //txtbx_age.Text = "";
                        if (entry_exist == 0)
                        {
                            string query1 = "update esociety.applicant_details set head_entry= @head_entry where app_id = @AppID";
                            cmd.CommandText = query1;
                            cmd.Parameters.AddWithValue("@AppID", Convert.ToInt64(Session["AppID"].ToString()));
                            cmd.Parameters.AddWithValue("@head_entry", 1);
                            cmd.ExecuteNonQuery();
                        }
                    }


                    catch (NpgsqlException ex)
                    {
                        ClientScript.RegisterStartupScript(GetType(), "text", "enablefields()", true);
                        CreateLogFiles Err = new CreateLogFiles();
                        Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "btnAddOld_Click()" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
                        RecordUserAction("btnAddOld_Click", ex.Message, "F"); var errorcode = ex.ErrorCode;
                        if (errorcode == 23503)
                        {
                            Response.Write("<script language='javascript'>alert('" + "Please Complete Society Details first." + "')</script>");
                        }
                        else
                        {
                            Response.Write("<script language='javascript'>alert('" + "Exection Error" + "')</script>");
                        }
                    }
                    finally
                    {
                        connect.Close();
                    }
                    btnAddOld.Visible = false;
                    btnAdd.Visible = true;
                    tr_dateofadmission.Visible = false;
                    ViewState["addoldmember_flag"] = '0';
                }
            }
            else
            {
                ClientScript.RegisterStartupScript(GetType(), "text", "enablefields()", true);
            }
        }

        protected void gv_managingcomm_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            
            try
            {
              
                if (e.Row.RowType.Equals(DataControlRowType.DataRow))
                {
                    Label pid = (e.Row.FindControl("lbmang_proof_id") as Label);
                    string p_id = pid.Text;                   
                    if (p_id == "5")
                    {
                        Label mid = (e.Row.FindControl("lbmang_memberid") as Label);
                        string m_id = mid.Text;
                        e.Row.Cells[10].Text = Utility.getAadharNo(m_id);
                        string x = e.Row.Cells[10].Text;
                        e.Row.Cells[10].Text = _encryption.Decrypt(x, System.Web.HttpContext.Current.Server.MapPath("~/ENCRYPT.KEY"));
                        e.Row.Cells[10].Text = Server.HtmlEncode(Utility.MaskMobile(e.Row.Cells[10].Text, 0, "XXXXXXXX"));
                    }
                    else
                    {
                        string x = e.Row.Cells[10].Text;
                        e.Row.Cells[10].Text = _encryption.Decrypt(x, System.Web.HttpContext.Current.Server.MapPath("~/ENCRYPT.KEY"));
                        e.Row.Cells[10].Text = Server.HtmlEncode(Utility.MaskMobile(e.Row.Cells[10].Text, 0, "XXXXXXXX"));
                    }
                }

                

                if(Convert.ToInt32(Session["Renewal"]) == 1)
                {
                    e.Row.Cells[11].Visible = false;
                }

                


            }
            catch (Exception ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "gv_managingcomm_RowDataBound" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
            }
        }

        protected void btnBack_Click(object sender, EventArgs e)
        {
            Response.Redirect("SocietyDetails.aspx");
        }
    }
}
