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
using GoaSocietyRegistration;
using GoaSocietyRegistration.Development;
using System.Text.RegularExpressions;

namespace GoaSocietyRegistration
{
    [Serializable]
    public partial class SocietyDetails : System.Web.UI.Page
    {
        // static int edit = 0;//0->fresh entry 1->edit
        static string socNameUpper;
        string ipaddress = Utility.getIP();
        string macaddress = Utility.GetMACAddress(Utility.getIP());
        //string macaddress = Utility.GetMACAddress();
        Validate _val = new Validate();
        Insert ins = new Insert();
        
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
                if (!IsPostBack)
                {
                    Session["sd_edit"] = 0;
                    ViewState["edit_flag"] = 0;
                    Utility.FillDistrictSoc(ddlSocDistrict);
                    FillSocietyType();
                }


                get_status();
                //code to be shifted on dashborad re
                if (Session["login_id"] != null)
                {
                    HttpContext.Current.Response.AddHeader("Cache-Control", "no-cache, no-store, must-revalidate");
                    HttpContext.Current.Response.AddHeader("Pragma", "no-cache");
                    HttpContext.Current.Response.AddHeader("Expires", "0");
                    NpgsqlConnection conn = new NpgsqlConnection();
                    NpgsqlCommand cmd = new NpgsqlCommand();
                    string loginid = Session["login_id"].ToString();
                    //Label7.Text = loginid;
                    conn.ConnectionString = ConfigurationManager.ConnectionStrings["connect"].ConnectionString;
                    cmd.Connection = conn;
                    try
                    {
                        conn.Open();
                        string query = "select login_id,app_id,created_by from esociety.temp_table where login_id=@sid";
                        cmd.CommandText = query;
                        cmd.Parameters.AddWithValue("@sid", loginid);
                        NpgsqlDataReader rd = cmd.ExecuteReader();
                        if (rd.Read())
                        {
                            string appid = Server.HtmlEncode(rd["app_id"].ToString());
                            Session["created_by"] = Server.HtmlEncode(rd["created_by"].ToString());
                            RecordUserAction("Society_onLoad", "Society Page Load Success", "S");
                            // setloginName(loginid);                           
                            if (appid == "" || appid == null)
                            {
                                Int64 AppID = Utility.get_ApplicationID();
                                Session["AppID"] = AppID;
                              
                                // setAppID(loginid, AppID);
                            }
                            else if(Convert.ToInt32(ViewState["edit_flag"].ToString())!=1)                            //if (Utility.checkifrenewal(appid)==1)
                            {
                                Session["AppID"] = Convert.ToInt64(appid);

                                try
                                {
                                    Society_Details society1 = ins.FetchSociety(loginid, 1, 3);


                                    string value = Server.HtmlEncode(society1.complete_data);

                                    if (value == "Yes")
                                    {
                                        ddlSocType.SelectedValue = Server.HtmlEncode(society1.soctype.ToString());
                                        txtSocName.Text = Server.HtmlEncode(society1.socname);
                                        txtSocAddress.Text = Server.HtmlEncode(society1.socaddr);
                                        ddlSocDistrict.SelectedValue = Server.HtmlEncode(society1.socdistrict.ToString());
                                        int tid = (int)(society1.soc_taluka);
                                        setTaluka(tid);
                                        TxtbxPincode.Text = Server.HtmlEncode(society1.pincode);

                                        //ViewState["totalmem"] = dr["totalmem"].ToString();
                                        //FillTaluka(Convert.ToInt32(ddlSocDistrict.SelectedValue));

                                        lblAmt.Text = Server.HtmlEncode(society1.regfee.ToString());
                                        hfregfee.Value = Server.HtmlEncode(society1.regfee.ToString());
                                        lblProcAmt.Text =  Server.HtmlEncode(society1.processfee.ToString());
                                        hfprocfee.Value = Server.HtmlEncode(society1.processfee.ToString());
                                        lblTotAmt.Text =  Server.HtmlEncode(society1.totalfee.ToString());

                                        loadobjective(Session["AppID"].ToString());

                                        btnnext.Visible = true;
                                        btnBack.Visible = true;
                                        btnedit.Visible = true;
                                        //SocietyDetailsBtn.Visible = false;
                                        ddlSocType.Enabled = false;
                                        txtSocName.Enabled = false;
                                        txtSocAddress.Enabled = false;
                                        ddlSocDistrict.Enabled = false;
                                        ddlSocTaluka.Enabled = false;
                                        SocietyDetailsBtn.Visible = false;
                                        TxtbxPincode.Enabled = false;
                                        gv_objective.Enabled = false;
                                        addobjec_div.Visible = false;



                                        RecordUserAction("Society_onLoad", "Society Data Loaded from DB", "S");
                                    }
                                    else
                                    {
                                        loadobjective(Session["AppID"].ToString());

                                        if (Convert.ToInt32(Session["Renewal"]) == 2 && !IsPostBack)
                                        {
                                            Society_Details society_renew = new Society_Details();
                                            society_renew = ins.FetchSociety(Session["OldRegNo"].ToString(), 2 , Convert.ToInt32((Session["OldRegDistrict"].ToString())));                                            
                                            setData(society_renew);                                           
                                        }
                                    }
                                    if(Convert.ToInt32(Session["Renewal"]) == 2)
                                    {
                                        tr_regfee.Visible = false;
                                        tr_processfee.Visible = false;
                                        tr_totfee.Visible = false;
                                    }


                                }
                                catch (NpgsqlException ex)
                                {
                                    CreateLogFiles Err = new CreateLogFiles();
                                    Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "Page_Load1" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
                                    RecordUserAction("Society_onLoad", ex.Message, "F");
                                    Response.Write("<script language='javascript'>alert('" + "Data not loading from DB" + "')</script>");
                                }
                                finally
                                {
                                    conn.Close();
                                }
                            }
                            else if(Convert.ToInt32(ViewState["edit_flag"].ToString()) == 1)
                            {
                                string tid = ddlSocTaluka.SelectedValue;
                                FillTaluka(Convert.ToInt32(ddlSocDistrict.SelectedValue));
                                ddlSocTaluka.SelectedValue = tid;
                                btnedit.Visible = false;
                                txtSocAddress.Enabled = true;
                                ddlSocTaluka.Enabled = true;
                                TxtbxPincode.Enabled = true;
                                
                            }
                        }
                        rd.Close();
                    }
                    catch (NpgsqlException ex)
                    {
                        CreateLogFiles Err = new CreateLogFiles();
                        Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "Page_Load2" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
                        RecordUserAction("SocietyDetailsBtn_Click", ex.Message, "F");
                        Response.Write("<script language='javascript'>alert('" + "data not written in db table" + "')</script>");
                    }
                    finally
                    {
                        conn.Close();
                    }
                }
                else
                {

                    RecordUserAction("Society_onLoad", "Session Tampered", "F");
                    Response.Redirect("LoginModule.aspx");
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
        public void get_status()
        {
            try
            {
                if (Convert.ToInt32(Session["status_Id"]) == 3)
                {
                    SocietyDetailsBtn.Enabled = false;

                    //btnupdate.Enabled = false;

                }

                if (Convert.ToInt32(Session["status_Id"]) == 1 || Convert.ToInt32(Session["status_Id"]) == 2 || Convert.ToInt32(Session["status_Id"]) == 5)
                {
                    btnedit.Enabled = true;
                }
                else
                {
                    btnedit.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "get_status()" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
                
            }
        }
        public void setTaluka(int talukaid)
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
                string query = "SELECT \"TalukaName\",\"TalukaID\" FROM esociety.mst_taluka where \"TalukaID\"= @talukaid";
                cmd.CommandText = query;
                cmd.Parameters.AddWithValue("@talukaid", talukaid);
                adapter = new NpgsqlDataAdapter(cmd);
                ds = new DataSet();
                adapter.Fill(ds, "esociety.mst_taluka"); 
                ddlSocTaluka.DataSource = ds.Tables[0];
                ddlSocTaluka.DataTextField = "TalukaName";
                ddlSocTaluka.DataValueField = "TalukaID";
                ddlSocTaluka.DataBind();
                cmd.Dispose();
                adapter.Dispose();
                ds = null;
            }
            catch (NpgsqlException ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "setTaluka()" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
                RecordUserAction("setTaluka", ex.Message, "F");
                Response.Write("<script language='javascript'>alert('Taluka Soceity Type:" + "Taluka Dropdown Loading Error" + "')</script>");
            }
            finally
            {
                connect.Close();
            }
        }

        public void setFees(int soctypeid)
        {
            NpgsqlConnection conn = new NpgsqlConnection();
            NpgsqlCommand cmd = new NpgsqlCommand();
            conn.ConnectionString = ConfigurationManager.ConnectionStrings["connect"].ConnectionString;
            cmd.Connection = conn;
            try
            {
                conn.Open();
                string query = "SELECT reg_fees, process_fess, total_fees FROM esociety.mst_fees where soc_type_id=@soc_type_id";
                cmd.CommandText = query;
                cmd.Parameters.AddWithValue("@soc_type_id", soctypeid);
                NpgsqlDataReader rd = cmd.ExecuteReader();
                if (rd.Read())
                {
                    lblProcAmt.Text = Server.HtmlEncode(rd["process_fess"].ToString());
                    lblProcAmt.Enabled = false;
                    hfprocfee.Value = Server.HtmlEncode(rd["process_fess"].ToString());
                    hfregfee.Value = Server.HtmlEncode(rd["reg_fees"].ToString());
                    lblAmt.Text =  Server.HtmlEncode(rd["reg_fees"].ToString());
                    lblAmt.Enabled = false;
                    lblTotAmt.Text = Server.HtmlEncode(rd["total_fees"].ToString());
                    lblTotAmt.Enabled = false;
                }
                rd.Close();
            }
            catch (NpgsqlException ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "setFees" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
                //RecordUserAction("MemberDetails_Page_Load", "Exception at Member Data", "F");
                Response.Write("<script language='javascript'>alert('" + "Error encountered" + "')</script>");
            }
            finally
            { conn.Close(); }
        }



        protected void ddlSocType_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                errorforsociety.Visible = false;
                //txtSocName.Text = "";
                if (ddlSocType.SelectedValue == "1" || ddlSocType.SelectedValue == "2" || ddlSocType.SelectedValue == "3" || ddlSocType.SelectedValue == "4" || ddlSocType.SelectedValue == "5")
                {

                    setFees(Convert.ToInt32(ddlSocType.SelectedValue));                  
                    SocietyDetailsBtn.Enabled = true;
                }
                else
                {
                    lblProcAmt.Text = "";
                    lblTotAmt.Text = "";
                    SocietyDetailsBtn.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "ddlSocType_SelectedIndexChanged()" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
               
            }


        }
        protected int checksocietyname(string society_name)
        {
            int i = 0, namefound = 0;
            if (society_name == null || society_name == "" || society_name.Trim().Length == 0)
            {
                errorforsociety.ForeColor = System.Drawing.Color.Red;
                errorforsociety.Text = "Enter Society Name";
                i = 2;
            }
            else
            {
                //string socname = society_name.ToUpper().Trim();
                string socname = Regex.Replace(society_name, @"\s+", " ");
                socname = Regex.Replace(socname, @"\s*,\s*", ",");
                errorforsociety.Text = "";
                int isnew =Convert.ToInt32(Session["Renewal"]);

                if (isnew == 1)
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
                        //string query = "select socname from esociety.society where socname=@socname";

                       // string query = "Select socname from esociety.society where trim(regexp_replace(socname, '\\s+', ' ', 'g')) LIKE @socname";
                        string query = "Select socname from esociety.society where active = 'Y' and trim(regexp_replace((regexp_replace(socname, '\\s+', ' ', 'g')),'\\s*,\\s*',',','g')) LIKE @socname";

                        cmd.CommandText = query;
                        cmd.Parameters.AddWithValue("@socname", socname.ToUpper().Trim());
                        NpgsqlDataReader dr = cmd.ExecuteReader();

                        if (dr.Read())
                        {
                            errorforsociety.Visible = true;
                            errorforsociety.Text = "Society name is already registered. Please try different Name";
                            errorforsociety.ForeColor = System.Drawing.Color.Red;
                            SocietyDetailsBtn.Enabled = false;
                            dr.Close();
                            namefound = 1;
                            i = 1;
                        }
                        else
                        {
                            namefound = 0;
                            dr.Close();
                        }

                        if (namefound == 0)
                        {
                            //  string query1 = "select socname from esociety.society_all where socname=@socname";
                           // string query1 = "Select socname from esociety.society_all where trim(regexp_replace(socname, '\\s+', ' ', 'g')) LIKE @socname";
                            string query1 = "Select socname from esociety.society_all where trim(regexp_replace((regexp_replace(socname, '\\s+', ' ', 'g')),'\\s*,\\s*',',','g')) LIKE @socname";

                            cmd.CommandText = query1;
                            cmd.Parameters.AddWithValue("@socname", socname.ToUpper().Trim());
                            NpgsqlDataReader rd = cmd.ExecuteReader();


                            if (rd.Read())
                            {
                                errorforsociety.Visible = true;
                                errorforsociety.Text = "Society name is already registered. Please try different Name";
                                errorforsociety.ForeColor = System.Drawing.Color.Red;
                                SocietyDetailsBtn.Enabled = false;
                                namefound = 1;
                                i = 1;
                            }
                            else
                            {
                                namefound = 0;
                                errorforsociety.Visible = false;
                                //errorforsociety.Text = "Society name available";
                                //errorforsociety.ForeColor = System.Drawing.Color.Green;
                                SocietyDetailsBtn.Enabled = true;
                                i = 0;

                            }
                            rd.Close();
                        }

                        if (namefound == 0)
                        {
                            // string query2 = "select socname from esociety.society_all_north where socname=@socname";
                            // string query2 = "Select socname from esociety.society_all_north where trim(regexp_replace(socname, '\\s+', ' ', 'g')) LIKE @socname";
                            string query2 = "Select socname from esociety.society_all_north where trim(regexp_replace((regexp_replace(socname, '\\s+', ' ', 'g')),'\\s*,\\s*',',','g')) LIKE @socname";

                            cmd.CommandText = query2;
                            cmd.Parameters.AddWithValue("@socname", socname.ToUpper().Trim());
                            NpgsqlDataReader rd1 = cmd.ExecuteReader();


                            if (rd1.Read())
                            {
                                errorforsociety.Visible = true;
                                errorforsociety.Text = "Society name is already registered. Please try different Name";
                                errorforsociety.ForeColor = System.Drawing.Color.Red;
                                SocietyDetailsBtn.Enabled = false;
                                namefound = 1;
                                i = 1;
                            }
                            else
                            {
                                namefound = 0;
                                errorforsociety.Visible = false;
                                //errorforsociety.Text = "Society name available";
                                //errorforsociety.ForeColor = System.Drawing.Color.Green;
                                SocietyDetailsBtn.Enabled = true;
                                i = 0;

                            }
                            rd1.Close();
                        }
                        myTrans.Commit();


                    }
                    catch (NpgsqlException ex)
                    {
                        CreateLogFiles Err = new CreateLogFiles();
                        Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "checksocietyname()" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
                        RecordUserAction("txtSocname_textChanged", ex.Message, "F");
                        Response.Write("<script language='javascript'>alert('" + " Error Society name check from db  " + "')</script>");
                        i = 2;
                        myTrans.Rollback();
                    }
                    finally
                    {
                        conn.Close();
                    }
                }


            }
            return i;
        }

        protected void txtSocName_TextChanged(object sender, EventArgs e)
        {
            if (Convert.ToInt32(Session["sd_edit"].ToString()) == 0)
            {
                string socname = txtSocName.Text.ToUpper().Trim();
                int socnameexist = checksocietyname(socname);
                socNameUpper = txtSocName.Text.ToUpper();

              
            }
            else
            {
               
                if (txtSocName.Text == null || txtSocName.Text == "" || txtSocName.Text.Trim().Length == 0)
                {
                    errorforsociety.ForeColor = System.Drawing.Color.Red;
                    errorforsociety.Text = "Enter Society Name";
                }
                else
                {
                    string socname = txtSocName.Text.ToUpper().Trim();
                    errorforsociety.Text = "";
                    NpgsqlConnection conn = new NpgsqlConnection();
                    NpgsqlCommand cmd = new NpgsqlCommand();
                    conn.ConnectionString = ConfigurationManager.ConnectionStrings["connect"].ConnectionString;
                    cmd.Connection = conn;
                    try
                    {
                        conn.Open();
                        string query = "select socname from esociety.society where socname=@socname";  //and soctype=@soctype
                        cmd.CommandText = query;
                        cmd.Parameters.AddWithValue("@socname", socname);
                        cmd.Parameters.AddWithValue("@soctype", Convert.ToInt32(ddlSocType.SelectedValue));
                        NpgsqlDataReader dr = cmd.ExecuteReader();

                        if (dr.Read())
                        {
                            errorforsociety.Visible = true;
                            errorforsociety.Text = "Society name is already registered. Please try different Name";
                            errorforsociety.ForeColor = System.Drawing.Color.Red;
                            SocietyDetailsBtn.Enabled = false;
                        }
                        else
                        {
                            //errorforsociety.Visible = true;
                            //errorforsociety.Text = "Society name available";
                            //errorforsociety.ForeColor = System.Drawing.Color.Green;
                            SocietyDetailsBtn.Enabled = true;

                        }
                        dr.Close();
                    }
                    catch (NpgsqlException ex)
                    {
                        CreateLogFiles Err = new CreateLogFiles();
                        Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "txtSocName_TextChanged" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
                        RecordUserAction("txtSocnameTextchanged", ex.Message, "F");
                        Response.Write("<script language='javascript'>alert('" + "Error Society name check from db" + "')</script>");
                    }
                    finally
                    {
                        conn.Close();
                    }
                    socNameUpper = txtSocName.Text.ToUpper();
                }
            }
        }
        protected void ddlSocDistrict_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                FillTaluka(Convert.ToInt32(ddlSocDistrict.SelectedValue));
            }
            catch (Exception ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "ddlSocDistrict_SelectedIndexChanged()" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));               
            }

        }
        public void FillSocietyType()
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
                string query = "SELECT societytype,societyid FROM esociety.mst_societytype where active = 'Y'";
                cmd.CommandText = query;
                adapter = new NpgsqlDataAdapter(cmd);
                ds = new DataSet();
                adapter.Fill(ds, "esociety.mst_societytype");
                ddlSocType.DataSource = ds.Tables[0];
                ddlSocType.DataTextField = "societytype";
                ddlSocType.DataValueField = "societyid";
                ddlSocType.DataBind();
                ddlSocType.Items.Insert(0, new ListItem("-- Select --", "-1"));
                cmd.Dispose();
                adapter.Dispose();
                ds = null;
            }
            catch (NpgsqlException ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "FillSocietyType()" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
                RecordUserAction("FillSocietyType", ex.Message, "F");
                Response.Write("<script language='javascript'>alert('Soceity Type:" + "Society Dropdown Loading Error" + "')</script>");
            }
            finally
            {
                connect.Close();
            }
        }
      
        public void FillTaluka(int DistrictID)
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
                string query = "SELECT \"TalukaName\",\"TalukaID\" FROM esociety.mst_taluka where \"DistrictID\"=@DistrictID";
                cmd.CommandText = query;
                cmd.Parameters.AddWithValue("@DistrictID", DistrictID);
                adapter = new NpgsqlDataAdapter(cmd);
                ds = new DataSet();
                adapter.Fill(ds, "esociety.mst_taluka");
                ddlSocTaluka.DataSource = ds.Tables[0];
                ddlSocTaluka.DataTextField = "TalukaName";
                ddlSocTaluka.DataValueField = "TalukaID";
                ddlSocTaluka.DataBind();
                ddlSocTaluka.Items.Insert(0, new ListItem("-- Select --", "-1"));
                cmd.Dispose();
                adapter.Dispose();
                ds = null;
            }
            catch (NpgsqlException ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "FillTaluka()" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
                RecordUserAction("FillTaluka", ex.Message, "F");
                Response.Write("<script language='javascript'>alert('Taluka Soceity Type:" + "Society Taluka Loading Error" + "')</script>");
            }
            finally
            {
                connect.Close();
            }
        }
        protected void SocietyDetailsBtn_Click(object sender, EventArgs e)
        {
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "none", "<script> $(window).on('load', function () { $('#confirmModal').modal({ backdrop: 'static' });});</script>", false);
        }

        protected void btnnext_Click(object sender, EventArgs e)
        {
            RecordUserAction("btnnext_Click", "Redirection to MemberDetais.aspx", "S");
            Response.Redirect("MemberDetails.aspx");
        }

        protected void confirmmodalbutton_Click(object sender, EventArgs e)
        {


            if (Session["AppID"] != null && Session["created_by"] != null)
            {
                if (ddlSocType.SelectedValue == "-1")
                {
                    lblError.Text = "";
                    lblError.Text = "Society Type not Selected";
                }
                else if (txtSocName.Text == "" || txtSocName.Text == null)
                {
                    lblError.Text = "";
                    lblError.Text = "Enter Society Name";
                }
                else if (!_val.validateData(txtSocName.Text.Replace('\'', '`'), _val.society_name))
                {
                    lblError.Text = "";
                    lblError.Text = "Invalid Society Name";
                }
                else if (!(Convert.ToInt32(checksocietyname(txtSocName.Text)) == 0))
                {
                    lblError.Text = "";
                    lblError.Text = "Invalid Society Name or Society Name not available";
                }
                else if(!(Convert.ToInt32(sanitizesocname(txtSocName.Text)) == 0))
                {
                    lblError.Text = "";
                    lblError.Text = "Invalid Society Name";
                }
                else if (txtSocAddress.Text == "" || txtSocAddress.Text == null)
                {
                    lblError.Text = "";
                    lblError.Text = "Enter Society Address";
                }
                else if (!_val.validateData(txtSocAddress.Text, _val.reamrks_validation))
                {
                    lblError.Text = "";
                    lblError.Text = "Special Characters Not Allowed in Society Address";
                }
                else if (ddlSocDistrict.SelectedValue == "-1")
                {
                    lblError.Text = "";
                    lblError.Text = "Society District Not Selected";
                }
                else if (ddlSocTaluka.SelectedValue == "-1")
                {
                    lblError.Text = "";
                    lblError.Text = "Society Taluka Not Selected";
                }
                else if (TxtbxPincode.Text == "" || TxtbxPincode.Text == null)
                {
                    lblError.Text = "";
                    lblError.Text = "Society Pincode Not Entered";
                }
                else if (!_val.validateData(TxtbxPincode.Text, _val.pincode_regex))
                {
                    lblError.Text = "";
                    lblError.Text = "Society Pincode Not in Correct Format";
                }
                else if (TxtbxPincode.Text.Length > 6 || TxtbxPincode.Text.Length < 6)
                {
                    lblError.Text = "";
                    lblError.Text = "Enter Correct Pincode";
                }
                else if (Convert.ToInt32(hfprocfee.Value) <= 0 || Convert.ToInt32(hfregfee.Value) <= 0)
                {
                    lblError.Text = "";
                    lblError.Text = "Please Select Society type again.";
                }
                else if ((hfprocfee.Value != lblProcAmt.Text) || (hfregfee.Value != lblAmt.Text))
                {
                    lblError.Text = "";
                    lblError.Text = "Please Select Society type again.";
                }
                else if(gv_objective.Rows.Count < 1)
                {
                    lblError.Text = "";
                    lblError.Text = "Please add Society Objective";
                }                
                else
                {
                    NpgsqlConnection conn = new NpgsqlConnection();
                    NpgsqlCommand cmd = new NpgsqlCommand();
                    string loginid = Session["login_id"].ToString();
                    conn.ConnectionString = ConfigurationManager.ConnectionStrings["connect"].ConnectionString;
                    cmd.Connection = conn;
                    try
                    {
                        conn.Open();
                        string society_query = "INSERT INTO esociety.society(soctype,socname,socaddr,soc_taluka,socdistrict,app_id,regfee,processfee,totalfee,created_at,created_by,ipaddress,macaddress,active,complete,login_id,complete_data,pincode) VALUES (@soctype,@socname,@socaddr,@soc_taluka,@socdistrict,@app_id,@regfee,@processfee,@totalfee,current_timestamp,@created_by,@ipaddress,@macaddress,'Y','Y',@login_id,'Yes',@pincode)";
                        cmd.CommandText = society_query;
                        cmd.Parameters.AddWithValue("@soctype", Convert.ToInt32(ddlSocType.SelectedValue));
                        cmd.Parameters.AddWithValue("@socname", txtSocName.Text.ToUpper().Replace('\'','`'));
                        cmd.Parameters.AddWithValue("@socaddr", txtSocAddress.Text);
                        cmd.Parameters.AddWithValue("@soc_taluka", Convert.ToInt32(ddlSocTaluka.SelectedValue));
                        cmd.Parameters.AddWithValue("@socdistrict", Convert.ToInt32(ddlSocDistrict.SelectedValue));
                        cmd.Parameters.AddWithValue("@app_id", Convert.ToInt64(Session["AppID"].ToString()));
                        cmd.Parameters.AddWithValue("@regfee", Convert.ToInt32(hfregfee.Value));
                        cmd.Parameters.AddWithValue("@processfee", Convert.ToInt32(hfprocfee.Value));
                        int tfees = Convert.ToInt32(hfregfee.Value) + Convert.ToInt32(hfprocfee.Value);
                        cmd.Parameters.AddWithValue("@totalfee", tfees);
                        cmd.Parameters.AddWithValue("@created_by", Session["created_by"]);
                        cmd.Parameters.AddWithValue("@ipaddress", ipaddress);
                        cmd.Parameters.AddWithValue("@macaddress", macaddress);
                        cmd.Parameters.AddWithValue("@login_id", loginid);
                        cmd.Parameters.AddWithValue("@pincode", Convert.ToInt32(TxtbxPincode.Text));
                        cmd.ExecuteNonQuery();
                        RecordUserAction("SocietyDetailsBtn_Click", "Society new data entry", "S");
                        Response.Redirect("MemberDetails.aspx");
                    }
                    catch (NpgsqlException ex)
                    {
                        CreateLogFiles Err = new CreateLogFiles();
                        Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "confirmmodalbutton_Click" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
                        RecordUserAction("SocietyDetailsBtn_Click", ex.Message, "F");
                        Response.Write("<script language='javascript'>alert('" + "Society Btn Click Error" + "')</script>");
                    }
                    finally
                    {
                        conn.Close();
                    }
                }

            }
            else
            {

                RecordUserAction("SocietyDetailsBtn_Click", "Sesiion null", "F");
            }
        }



        protected void ddlregdistrict_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        public void setData(Society_Details oldSociety)
        {
            try
            {
                // FillDistrictSoc();
                //  FillSocietyType();
                if (oldSociety.tempflag == 1)
                {
                    ddlSocType.SelectedValue = Server.HtmlEncode(oldSociety.soctype.ToString());
                    txtSocName.Text = Server.HtmlEncode(oldSociety.socname);
                    txtSocAddress.Text = Server.HtmlEncode(oldSociety.socaddr);
                    ddlSocDistrict.SelectedValue = Server.HtmlEncode(oldSociety.socdistrict.ToString());
                    // ddlSocTaluka.SelectedValue = Server.HtmlEncode(oldSociety.soc_taluka.ToString());

                    FillTaluka(Convert.ToInt32(ddlSocDistrict.SelectedValue));
                    ddlSocTaluka.SelectedValue = Server.HtmlEncode(oldSociety.soc_taluka.ToString());
                    //int tid = oldSociety.soc_taluka;
                    //setTaluka(tid);
                    TxtbxPincode.Text = Server.HtmlEncode(oldSociety.pincode);
                    //lblAmt.Text = Server.HtmlEncode(oldSociety.regfee.ToString());           
                    //hfregfee.Value = Server.HtmlEncode(oldSociety.regfee.ToString());
                    // lblProcAmt.Text = Server.HtmlEncode(oldSociety.processfee.ToString());
                    // hfprocfee.Value = Server.HtmlEncode(oldSociety.processfee.ToString());
                    // lblTotAmt.Text = Server.HtmlEncode(oldSociety.totalfee.ToString());       
                    setFees(Convert.ToInt32(ddlSocType.SelectedValue));

                    btnnext.Visible = false;
                    btnBack.Visible = false;
                    btnedit.Visible = false;
                    ddlSocType.Enabled = true;
                    txtSocName.Enabled = false;
                    txtSocAddress.Enabled = true;
                    ddlSocDistrict.Enabled = false;

                    ddlSocTaluka.Enabled = true;
                    SocietyDetailsBtn.Visible = true;
                    TxtbxPincode.Enabled = true;
                }
                else if(oldSociety.tempflag == 2 || oldSociety.tempflag == 3)
                {
                    txtSocName.Text = Server.HtmlEncode(oldSociety.socname);
                    ddlSocDistrict.SelectedValue = Server.HtmlEncode(oldSociety.socdistrict.ToString());
                    FillTaluka(Convert.ToInt32(ddlSocDistrict.SelectedValue));
                    ddlSocDistrict.Enabled = false;
                    txtSocName.Enabled = false;
                }
                RecordUserAction("setData", "Old Society Data Loaded from DB", "S");
            }
            catch(Exception ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "setData()" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
                RecordUserAction("setData", "Old Society Data Loading Failed", "F");
            }
        }

        protected void btnedit_Click(object sender, EventArgs e)
        {
            try
            {
                ViewState["edit_flag"] = 1;

                btnupdate.Visible = true;
                btnnext.Enabled = false;
                btnBack.Enabled = false;
                btnedit.Visible = false;

                string tid = ddlSocTaluka.SelectedValue;
                FillTaluka(Convert.ToInt32(ddlSocDistrict.SelectedValue));
                ddlSocTaluka.SelectedValue = tid;

                txtSocAddress.Enabled = true;
                ddlSocTaluka.Enabled = true;
                TxtbxPincode.Enabled = true;
                gv_objective.Enabled = true;
                addobjec_div.Visible = true;
                RecordUserAction("btnedit_Click", "Society Edit Click", "S");
            }
            catch (Exception ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "btnedit_Click()" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
               
            }



        }

        protected void btnupdate_Click(object sender, EventArgs e)
        {
            Society_Details society_edit = new Society_Details();
            string tempname = society_edit.socname;
            society_edit = ins.FetchSociety(Session["login_id"].ToString(), 1, 3);
            if (Session["AppID"] != null && Session["created_by"] != null)
            {
                btnupdate.Visible = false;
                btnedit.Visible = true;
                btnnext.Enabled = true;
                btnBack.Enabled = true;

                

                if (txtSocAddress.Text == "" || txtSocAddress.Text == null)
                {
                    lblError.Text = "";
                    lblError.Text = "Enter Society Address";
                }
                else if (!_val.validateData(txtSocAddress.Text, _val.reamrks_validation))
                {
                    lblError.Text = "";
                    lblError.Text = "Special Characters Not Allowed in Society Address";
                }
                else if (ddlSocTaluka.SelectedValue == "-1")
                {
                    lblError.Text = "";
                    lblError.Text = "Society Taluka Not Selected";
                }
                else if (TxtbxPincode.Text == "" || TxtbxPincode.Text == null)
                {
                    lblError.Text = "";
                    lblError.Text = "Society Pincode Not Entered";
                }
                else if (!_val.validateData(TxtbxPincode.Text, _val.pincode_regex))
                {
                    lblError.Text = "";
                    lblError.Text = "Society Pincode Not in Correct Format";
                }
                else if (TxtbxPincode.Text.Length > 6 || TxtbxPincode.Text.Length < 6)
                {
                    lblError.Text = "";
                    lblError.Text = "Enter Correct Pincode";
                }
                else if(gv_objective.Rows.Count < 1)
                {
                    lblError.Text = "";
                    lblError.Text = "Society Objective not added";
                }
                else
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
                        
                        lblError.Text = " ";
                        cmd.Parameters.Clear();

                        string insert_query = "INSERT INTO esociety.history_society( socname, socaddr, app_id, regfee, processfee, totalmem, created_at, created_by,";
                        insert_query = insert_query + " ipaddress, macaddress, active, soc_taluka, socdistrict, soctype, doc_one, doc_two, complete, totalfee, login_id,";
                        insert_query = insert_query + " complete_data, pincode,final_certificate_mongo_entry) VALUES (@socname, @socaddr, @app_id, @regfee, @processfee, @totalmem, @created_at,";
                        insert_query = insert_query + " @created_by, @ipaddress, @macaddress, @active, @soc_taluka, @socdistrict, @soctype, @doc_one, @doc_two,";
                        insert_query = insert_query + " @complete, @totalfee, @login_id, @complete_data, @pincode,@final_certificate_mongo_entry)";
                        cmd.CommandText = insert_query;
                        cmd.Parameters.AddWithValue("@socname", society_edit.socname);
                        cmd.Parameters.AddWithValue("@socaddr", society_edit.socaddr);
                        cmd.Parameters.AddWithValue("@app_id", Convert.ToInt64(society_edit.app_id));
                        cmd.Parameters.AddWithValue("@regfee", society_edit.regfee);
                        cmd.Parameters.AddWithValue("@processfee", society_edit.processfee);
                        cmd.Parameters.AddWithValue("@totalmem", society_edit.totalmem);
                        cmd.Parameters.AddWithValue("@created_at", Convert.ToDateTime(society_edit.created_at));
                        cmd.Parameters.AddWithValue("@created_by", society_edit.created_by);
                        cmd.Parameters.AddWithValue("@ipaddress", society_edit.ipaddress);
                        cmd.Parameters.AddWithValue("@macaddress", society_edit.macaddress);
                        cmd.Parameters.AddWithValue("@active", society_edit.active);
                        cmd.Parameters.AddWithValue("@soc_taluka", society_edit.soc_taluka);
                        cmd.Parameters.AddWithValue("@socdistrict", society_edit.socdistrict);
                        cmd.Parameters.AddWithValue("@soctype", society_edit.soctype);
                        cmd.Parameters.AddWithValue("@doc_one", society_edit.doc_one);
                        cmd.Parameters.AddWithValue("@doc_two", society_edit.doc_two);
                        cmd.Parameters.AddWithValue("@complete", society_edit.complete);
                        cmd.Parameters.AddWithValue("@totalfee", society_edit.totalfee);
                        cmd.Parameters.AddWithValue("@login_id", society_edit.login_id);
                        cmd.Parameters.AddWithValue("@complete_data", society_edit.complete_data);
                        cmd.Parameters.AddWithValue("@pincode", Convert.ToInt64(society_edit.pincode));
                        cmd.Parameters.AddWithValue("@final_certificate_mongo_entry", society_edit.final_certificate_mongo_entry);

                        cmd.ExecuteNonQuery();
                        cmd.Parameters.Clear();


                        string query = "UPDATE esociety.society SET socaddr=@socaddr,soc_taluka=@soc_taluka,created_at = current_timestamp, created_by = @created_by, ipaddress = @ipaddress,"; //
                        query = query + "pincode=@pincode,macaddress=@macaddress where login_id=@login_id";    //app_id=@app_id";
                        cmd.CommandText = query;                        
                        cmd.Parameters.AddWithValue("@socaddr", txtSocAddress.Text);                        
                        cmd.Parameters.AddWithValue("@soc_taluka", Convert.ToInt32(ddlSocTaluka.SelectedValue));
                        cmd.Parameters.AddWithValue("@created_by", Session["created_by"]);
                        cmd.Parameters.AddWithValue("@ipaddress", ipaddress);
                        cmd.Parameters.AddWithValue("@macaddress", macaddress);
                        cmd.Parameters.AddWithValue("@pincode", Convert.ToInt64(TxtbxPincode.Text)); 
                        cmd.Parameters.AddWithValue("@login_id", Session["login_id"].ToString());
                        //cmd.Parameters.AddWithValue("@app_id", Convert.ToInt64(Session["AppID"].ToString()));
                        cmd.ExecuteNonQuery();
                        cmd.Parameters.Clear();
                        myTrans.Commit();
                        RecordUserAction("btnupdate_Click", "Update Society Data", "S");

                    }
                    catch (NpgsqlException ex)
                    {
                        CreateLogFiles Err = new CreateLogFiles();
                        Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "btnUpdate_Click" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
                        myTrans.Rollback();
                        RecordUserAction("btnUpdate_Click", ex.Message, "F");
                        Response.Write("<script language='javascript'>alert('" + "Update Society Data Exception" + "')</script>");
                    }
                    finally
                    {
                        conn.Close();
                        ViewState["edit_flag"] = 0;
                        txtSocAddress.Enabled = false;
                        TxtbxPincode.Enabled = false;
                        ddlSocTaluka.Enabled = false;
                        gv_objective.Enabled = false;
                        addobjec_div.Visible = false;

                    }
                }
                
            }
            else
            {
                RecordUserAction("btnUpdate_Click", "Session value null on button click", "S");
            }
        }

        protected void btn_addobjec_Click(object sender, EventArgs e)
        {

            if (Session["AppID"] != null && Session["created_by"] != null)
            {
                if (txtbx_object.Text == null || txtbx_object.Text == "")
                {
                    lblError.Text = "";
                    lblError.Text = "Enter the Objective in textbox(one point at a time) and click on add";
                }
                else if (!_val.validateData(txtbx_object.Text.Trim(), _val.reamrks_validation))
                {
                    lblError.Text = "";
                    lblError.Text = "Special Characters Not Allowed in Society Objective";
                }
                else
                {
                    NpgsqlConnection conn = new NpgsqlConnection();
                    NpgsqlCommand cmd = new NpgsqlCommand();
                    string loginid = Session["login_id"].ToString();
                    conn.ConnectionString = ConfigurationManager.ConnectionStrings["connect"].ConnectionString;
                    cmd.Connection = conn;
                    try
                    {
                        conn.Open();
                        string ins_query = "INSERT INTO esociety.society_objectives(app_id,login_id,objective,active,added_at,added_ipaddress) VALUES (@app_id,@login_id,@objective,@active,current_timestamp,@added_ipaddress)";
                        cmd.CommandText = ins_query;
                        cmd.Parameters.AddWithValue("@app_id", Convert.ToInt64(Session["AppID"].ToString()));
                        cmd.Parameters.AddWithValue("@login_id", Session["login_id"].ToString());
                        cmd.Parameters.AddWithValue("@objective", txtbx_object.Text);
                        cmd.Parameters.AddWithValue("@active", 'Y');
                        cmd.Parameters.AddWithValue("@added_by", Session["created_by"]);
                        cmd.Parameters.AddWithValue("@added_ipaddress", ipaddress);
                        cmd.ExecuteNonQuery();
                        RecordUserAction("btn_addobjec_Click", "Society Objective added", "S");

                        loadobjective(Session["AppID"].ToString());
                        txtbx_object.Text = "";
                    }
                    catch (NpgsqlException ex)
                    {
                        CreateLogFiles Err = new CreateLogFiles();
                        Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "btn_addobjec_Click" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
                        RecordUserAction("btn_addobjec_Click", ex.Message, "F");
                        Response.Write("<script language='javascript'>alert('" + "Something went Wrong" + "')</script>");
                    }
                    finally
                    {
                        conn.Close();
                    }
                }

            }
            else
            {

                RecordUserAction("btn_addobjec_Click", "Session null", "F");
            }
        }


        protected void loadobjective(string AppID)
        {
            NpgsqlConnection conn = new NpgsqlConnection();
            NpgsqlCommand cmd = new NpgsqlCommand();
            conn.ConnectionString = ConfigurationManager.ConnectionStrings["connect"].ConnectionString;
            cmd.Connection = conn;
            try
            {
                conn.Open();
                string query = "SELECT objective,row_id,app_id FROM esociety.society_objectives where app_id=@appid and active='Y' order by row_id ASC";
                cmd.CommandText = query;
                cmd.Parameters.AddWithValue("@appid", Convert.ToInt64(AppID));
                //using (NpgsqlDataAdapter dr = new NpgsqlDataAdapter(cmd))
                //{
                //    DataTable dt = new DataTable();
                //    dr.Fill(dt);
                //    gv_objective.DataSource = dt;
                //    gv_objective.DataBind();

                //    RecordUserAction("loadobjective", "Objective gridview Loaded", "S");

                //}
                NpgsqlDataAdapter da = new NpgsqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                da.Fill(ds);
                if (ds.Tables[0].Rows.Count > 0)
                {
                    gv_objective.Visible = true;
                    gv_objective.DataSource = ds;
                    gv_objective.DataBind();


                    RecordUserAction("loadobjective", "Objective gridview Loaded", "S");
                }
                else
                {
                    gv_objective.DataSource = ds;
                    gv_objective.DataBind();
                    gv_objective.Visible = false;
                }


            }
            catch (NpgsqlException ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "loadobjective()" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
                RecordUserAction("loadobjective", ex.Message, "F");
                Response.Write("<script language='javascript'>alert('" + "Exception in Gridview" + "')</script>");
            }
            finally
            {
                conn.Close();
            }
           

        }

        protected void LBDelete_Click(object sender, EventArgs e)
        {
            if (Session["AppID"] != null)
            {
                LinkButton btn = (LinkButton)sender;
                GridViewRow row = (GridViewRow)btn.NamingContainer;
                HiddenField hd = row.FindControl("hfrowID") as HiddenField;
                if (hd.Value != null)
                {
                    Int64 row_id_objective = Convert.ToInt64(hd.Value);

                    NpgsqlConnection connect = new NpgsqlConnection();
                    NpgsqlCommand cmd = new NpgsqlCommand();
                    connect.ConnectionString = ConfigurationManager.ConnectionStrings["connect"].ConnectionString;
                    cmd.Connection = connect;               
                 
                    try
                    {
                        connect.Open();
                        string query1 = "update esociety.society_objectives set active='N', deleted_at=current_timestamp where row_id=@row_id and app_id=@AppID";
                        cmd.CommandText = query1;
                        cmd.Parameters.AddWithValue("@row_id", row_id_objective);
                        cmd.Parameters.AddWithValue("@AppID", Convert.ToInt64(Session["AppID"].ToString()));                        
                        cmd.ExecuteNonQuery();
                        RecordUserAction("LBDelete_Click", "Gridview Data Deleted Successfully", "S");
                                                          
                        loadobjective(Session["AppID"].ToString());

                       //  Response.Write("<script>alert('Deleted successfully');</script>");
                        Label65.Text = "Objective Deleted succcessfully";
                        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "none", "<script> $(window).on('load', function () { $('#redirectModal').modal({ backdrop: 'static' });});</script>", false);
                        
                    }
                    catch (NpgsqlException ex)
                    {
                        CreateLogFiles Err = new CreateLogFiles();
                        Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "LBDelete_Click" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));                       
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

        protected void btnRedirect_Click(object sender, EventArgs e)
        {
            try
            {
                Response.Redirect("SocietyDetails.aspx");
            }
            catch (Exception ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "btnRedirect_Click()" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));

            }

        }

        protected void btnBack_Click(object sender, EventArgs e)
        {
            RecordUserAction("btnBack_Click", "Redirection to Dashboard.aspx", "S");
            Response.Redirect("Dashboard.aspx");
        }

        protected int sanitizesocname(string socname)
        {
            int i = 0;

            if (socname != null && socname != "")
            {

                string[] soc = socname.Split(new Char[] { ' ', ',', '.', '-', '\n', '\t' });
              
                errorforsociety.Text = "";
                int isnew = Convert.ToInt32(Session["Renewal"]);
                if (isnew == 1)
                {
                    NpgsqlConnection conn = new NpgsqlConnection();
                    NpgsqlCommand cmd = new NpgsqlCommand();
                    conn.ConnectionString = ConfigurationManager.ConnectionStrings["connect"].ConnectionString;
                    cmd.Connection = conn;
                            
                    try
                    {
                        conn.Open();
                        foreach (string name in soc)
                        {
                            if (name.Trim() != "" && name.Trim() != null)
                            {
                                cmd.Parameters.Clear();
                                string query = "Select * from esociety.mst_restrictedwords where active = 'Y' and upper(restrictedname) = @name";

                                cmd.CommandText = query;
                                cmd.Parameters.AddWithValue("@name", name.Trim().ToUpper());
                                NpgsqlDataReader dr = cmd.ExecuteReader();

                                if (dr.Read())
                                {
                                    errorforsociety.Visible = true;
                                    errorforsociety.Text = Sanitize.InputText(name) + " is NOT Allowed in Society Name";
                                    errorforsociety.ForeColor = System.Drawing.Color.Red;
                                    SocietyDetailsBtn.Enabled = false;
                                    dr.Close();
                                    i = 1;
                                    return i;
                                }
                                else
                                {
                                   
                                }
                                dr.Close();                                
                            }
                        }
                    }
                    catch (NpgsqlException ex)
                    {
                        CreateLogFiles Err = new CreateLogFiles();
                        Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "sanitizesocname()" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
                        RecordUserAction("sanitizesocname", ex.Message, "F");
                        Response.Write("<script language='javascript'>alert('" + " Error Society Name Sanitize from db  " + "')</script>");
                        i = 2;
                     
                    }
                    finally
                    {
                        conn.Close();
                    }
                }


            }
            return i;
        }
    }
}