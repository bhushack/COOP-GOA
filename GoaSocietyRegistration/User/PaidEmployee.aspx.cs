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

namespace GoaSocietyRegistration.User
{
    public partial class PaidEmployee : System.Web.UI.Page
    {
        Validate _val = new Validate();
        string ipaddress = Utility.getIP();
        string macaddress = Utility.GetMACAddress(Utility.getIP());
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
                HttpContext.Current.Response.AddHeader("Cache-Control", "no-cache, no-store, must-revalidate");
                HttpContext.Current.Response.AddHeader("Pragma", "no-cache");
                HttpContext.Current.Response.AddHeader("Expires", "0");
                if (!IsPostBack)
                {
                    ViewState["edit_data"] = 0;
                    loadGridview(Convert.ToInt64(Session["AppID"].ToString()));
                }
            }            
        }

        public void callModal()
        {
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "none", "<script> $(window).on('load', function () { $('#AddEmployeeData').modal({ backdrop: 'static' });});</script>", false);
        }
        public void clear()
        {
            txt_MemName.Text = "";
            TxtDesignation.Text = "";
            TxtBxPayScale.Text = "";
            txtData.Text = "";
            TxtPayMonth.Text = "";
            TxtBxDa.Text = "";
            TxtBxspcialPay.Text = "";
            TxtBxotherallowance.Text = "";
            txtprovident.Text = "";
            txtothers.Text = "";
            btnAdd.Visible = true;
            BtnUpdate.Visible = false;

        }
        protected void btnAddEmployee_Click(object sender, EventArgs e)
        {
            try
            {
                callModal();
                clear();
            }
            catch (Exception ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "btnAddEmployee_Click" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));

            }

        }

        public int _doValidation()
        {
            int vali = 0;
            try
            {
                if (txt_MemName.Text == "" || txt_MemName.Text == null)
                {
                    callModal();
                    Label2.Text = "Employee Name is blank";
                    txt_MemName.Focus();
                    vali = 0;
                }
                else if (!_val.validateData(txt_MemName.Text, _val.name))
                {
                    callModal();
                    Label2.Text = "Employee Name is Invalid / having special characters";
                    txt_MemName.Focus();
                    vali = 0;
                }
                else if (TxtDesignation.Text == "" || TxtDesignation.Text == null)
                {
                    callModal();
                    Label2.Text = "Employee Designation is blank";
                    TxtDesignation.Focus();
                    vali = 0;
                }
                else if (!_val.validateData(TxtDesignation.Text, _val.address))
                {
                    callModal();
                    Label2.Text = "Employee Name is Invalid / having special characters";
                    TxtDesignation.Focus(); vali = 0;
                }
                else if (TxtBxPayScale.Text == "" || TxtBxPayScale.Text == null)
                {
                    callModal();
                    Label2.Text = "Employee Pay scale is blank";
                    TxtBxPayScale.Focus(); vali = 0;
                }
                else if (!_val.validateData(TxtBxPayScale.Text, _val.address))
                {
                    callModal();
                    Label2.Text = "Employee Pay scale is Invalid / having special characters";
                    TxtBxPayScale.Focus(); vali = 0;
                }
                else if (txtData.Text == "" || txtData.Text == null)
                {
                    callModal();
                    Label2.Text = "Temporary or Permanent / full time or part time data is blank";
                    txtData.Focus(); vali = 0;
                }
                else if (!_val.validateData(txtData.Text, _val.address))
                {
                    callModal();
                    Label2.Text = "Temporary or Permanent / full time or part time is Invalid / having special characters";
                    txtData.Focus(); vali = 0;
                }
                else if (TxtPayMonth.Text == "" || TxtPayMonth.Text == null)
                {
                    callModal();
                    Label2.Text = "Present pay per month data is blank";
                    TxtPayMonth.Focus(); vali = 0;
                }
                else if (!_val.validateData(TxtPayMonth.Text, _val.address))
                {
                    callModal(); vali = 0;
                    Label2.Text = "Present pay per month is Invalid / having special characters";
                    TxtPayMonth.Focus();
                }
                else if (TxtBxDa.Text == "" || TxtBxDa.Text == null)
                {
                    callModal(); vali = 0;
                    Label2.Text = "Dearness Allowance data is blank";
                    TxtBxDa.Focus();
                }
                else if (!_val.validateData(TxtBxDa.Text, _val.address))
                {
                    callModal(); vali = 0;
                    Label2.Text = "Dearness Allowance is Invalid / having special characters";
                    TxtBxDa.Focus();
                }
                else if (!_val.validateData(TxtBxspcialPay.Text, _val.address))
                {
                    callModal(); vali = 0;
                    Label2.Text = "Special Pay is Invalid / having special characters";
                    TxtBxspcialPay.Focus();
                }
                ////////////////////////////////////////////////////
                else if (!_val.validateData(TxtBxotherallowance.Text, _val.address))
                {
                    callModal(); vali = 0;
                    Label2.Text = "Other Allowances is Invalid / having special characters";
                    TxtBxotherallowance.Focus();
                }
                else if (!_val.validateData(txtprovident.Text, _val.address))
                {
                    callModal(); vali = 0;
                    Label2.Text = "Provident Fund is Invalid / having special characters";
                    txtprovident.Focus();
                }
                else if (!_val.validateData(txtothers.Text, _val.address))
                {
                    callModal(); vali = 0;
                    Label2.Text = "Other benefits and amenities is Invalid / having special characters";
                    txtothers.Focus();
                }
                //////////////////////////////////////////////
                else
                {
                    vali = 1;
                }
            }
            catch (Exception ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "_doValidation() " + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
                vali = 0;
            }
            return vali;
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
        protected void loadGridview(long app_id)
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
                cmd.Parameters.AddWithValue("@app_id", app_id);
                NpgsqlDataAdapter da = new NpgsqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                da.Fill(ds);
                if (ds.Tables[0].Rows.Count > 0)
                {
                    gridview_employeeData.DataSource = ds;
                    gridview_employeeData.DataBind();

                    RecordUserAction("loadGridview", "Old Members Data Loaded", "S");
                }
                txtTotMember.Text = gridview_employeeData.Rows.Count.ToString();

            }
            catch (NpgsqlException ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "loadGridview()" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
                RecordUserAction("loadGridview", ex.Message, "F");
                Response.Write("<script language='javascript'>alert('" + "Exception in bindgridview_oldmembers" + "')</script>");
            }
            finally
            {
                conn.Close();
            }
        }
        protected void btnAdd_Click(object sender, EventArgs e)
        {
            int validation = _doValidation();
            if (validation == 1)
            {
                NpgsqlConnection conn = new NpgsqlConnection();
                NpgsqlCommand cmd = new NpgsqlCommand();
                conn.ConnectionString = ConfigurationManager.ConnectionStrings["connect"].ConnectionString;
                cmd.Connection = conn;
                conn.Open();
                try
                {
                    long employee_id = Utility.get_EmployeeID();
                    string query = "INSERT INTO esociety.society_employement(employee_name, designation, present_pay_scale, temporary_permanent, present_pay,";
                    query = query + " dearness_allowance, special_pay, other_allowance, provident_fund, other_benefits, entered_by, entered_at, ipaddress, app_id,";
                    query = query + " employee_id, active) VALUES (@employee_name, @designation, @present_pay_scale, @temporary_permanent, @present_pay,";
                    query = query + " @dearness_allowance, @special_pay, @other_allowance, @provident_fund, @other_benefits, @entered_by, CURRENT_TIMESTAMP,";
                    query = query + " @ipaddress, @app_id, @employee_id, @active)";
                    cmd.CommandText = query;
                    cmd.Parameters.AddWithValue("@employee_name", Server.HtmlEncode(txt_MemName.Text));
                    cmd.Parameters.AddWithValue("@designation", TxtDesignation.Text);
                    cmd.Parameters.AddWithValue("@present_pay_scale", TxtBxPayScale.Text);
                    cmd.Parameters.AddWithValue("@temporary_permanent", txtData.Text);
                    cmd.Parameters.AddWithValue("@present_pay", TxtPayMonth.Text);
                    cmd.Parameters.AddWithValue("@dearness_allowance", TxtBxDa.Text);
                    cmd.Parameters.AddWithValue("@special_pay", TxtBxspcialPay.Text);
                    cmd.Parameters.AddWithValue("@other_allowance", TxtBxotherallowance.Text);
                    cmd.Parameters.AddWithValue("@provident_fund", txtprovident.Text);
                    cmd.Parameters.AddWithValue("@other_benefits", txtothers.Text);
                    cmd.Parameters.AddWithValue("@entered_by", Session["login_id"].ToString());
                    cmd.Parameters.AddWithValue("@ipaddress", ipaddress);
                    cmd.Parameters.AddWithValue("@app_id", Convert.ToInt64(Session["AppID"].ToString()));
                    cmd.Parameters.AddWithValue("@employee_id", employee_id);
                    cmd.Parameters.AddWithValue("@active", 'Y');
                    cmd.ExecuteNonQuery();
                    Response.Write("<script language='javascript'>alert('" + "Employee Added." + "')</script>");
                    loadGridview(Convert.ToInt64(Session["AppID"].ToString()));
                }
                catch (Exception ex)
                {
                    CreateLogFiles Err = new CreateLogFiles();
                    Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "btnAdd_Click  " + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));

                }
                finally
                {
                    conn.Close();
                }
            }
        }

        protected void loadData(string employeeID)
        {
            NpgsqlConnection conn = new NpgsqlConnection();
            NpgsqlCommand cmd = new NpgsqlCommand();
            conn.ConnectionString = ConfigurationManager.ConnectionStrings["connect"].ConnectionString;
            cmd.Connection = conn;
            conn.Open();
            try
            {
                string query = "SELECT employee_name, designation, present_pay_scale, temporary_permanent, present_pay, dearness_allowance, special_pay, other_allowance,";
                query = query + " provident_fund, other_benefits, entered_by, entered_at, ipaddress, app_id FROM esociety.society_employement";
                query = query + " where employee_id = @employee_id and active = 'Y'";
                cmd.CommandText = query;
                cmd.Parameters.AddWithValue("@employee_id",Convert.ToInt64(employeeID));
                NpgsqlDataReader rd = cmd.ExecuteReader();
                if (rd.Read())
                {
                    txt_MemName.Text = rd["employee_name"].ToString();
                    ViewState["employee_name"] = rd["employee_name"].ToString();
                    TxtDesignation.Text = rd["designation"].ToString();
                    ViewState["designation"] = rd["designation"].ToString();
                    TxtBxPayScale.Text = rd["present_pay_scale"].ToString();
                    ViewState["present_pay_scale"] = rd["present_pay_scale"].ToString();
                    txtData.Text = rd["temporary_permanent"].ToString();
                    ViewState["temporary_permanent"] = rd["temporary_permanent"].ToString();
                    TxtPayMonth.Text = rd["present_pay"].ToString();
                    ViewState["present_pay"] = rd["present_pay"].ToString();
                    TxtBxDa.Text = rd["dearness_allowance"].ToString();
                    ViewState["dearness_allowance"] = rd["dearness_allowance"].ToString();
                    TxtBxspcialPay.Text = rd["special_pay"].ToString();
                    ViewState["special_pay"] = rd["special_pay"].ToString();
                    TxtBxotherallowance.Text = rd["other_allowance"].ToString();
                    ViewState["other_allowance"] = rd["other_allowance"].ToString();
                    txtprovident.Text = rd["provident_fund"].ToString();
                    ViewState["provident_fund"] = rd["provident_fund"].ToString();
                    txtothers.Text = rd["other_benefits"].ToString();
                    ViewState["other_benefits"] = rd["other_benefits"].ToString();
                    ViewState["entered_by"] = rd["entered_by"].ToString();
                    ViewState["entered_at"] = rd["entered_at"].ToString();
                    ViewState["ipaddress"] = rd["ipaddress"].ToString();
                    ViewState["app_id"] = rd["app_id"].ToString();
                    ViewState["employee_id"] = employeeID;
                }
                rd.Close();
            }
            catch (Exception ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "loadData()   " + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));

            }
            finally
            {
                conn.Close();
            }
        }

        protected void LkEdit_Click(object sender, EventArgs e)
        {
            try
            {
                LinkButton btn = (LinkButton)sender;
                GridViewRow row = (GridViewRow)btn.NamingContainer;
                string employeeID = ((Label)gridview_employeeData.Rows[row.RowIndex].FindControl("lbEmployeeID")).Text;
                HfEmployyeIDEDIT.Value = employeeID;
                ViewState["edit_data"] = 1;
                btnAdd.Visible = false;
                BtnUpdate.Visible = true;
                loadData(employeeID);
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "none", "<script> $(window).on('load', function () { $('#AddEmployeeData').modal({ backdrop: 'static' });});</script>", false);
            }
            catch (Exception ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "LkEdit_Click  " + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));

            }
        }

        protected void BtnUpdate_Click(object sender, EventArgs e)
        {
            if (HfEmployyeIDEDIT.Value == ViewState["employee_id"].ToString())
            {
                NpgsqlConnection conn = new NpgsqlConnection();
                NpgsqlCommand cmd = new NpgsqlCommand();
                conn.ConnectionString = ConfigurationManager.ConnectionStrings["connect"].ConnectionString;
                cmd.Connection = conn;
                conn.Open();
                string employeeID = HfEmployyeIDEDIT.Value;
                NpgsqlTransaction trans = conn.BeginTransaction();
                try
                {
                    cmd.Parameters.Clear();
                    string query = "INSERT INTO esociety.society_employement_history(employee_name, designation, present_pay_scale, temporary_permanent, present_pay,";
                    query = query + " dearness_allowance, special_pay, other_allowance, provident_fund, other_benefits, entered_by, ipaddress, app_id, active,";
                    query = query + " employee_id, updated_at, updated_ipaddress, entered_at) VALUES (@employee_name, @designation, @present_pay_scale,";
                    query = query + " @temporary_permanent, @present_pay, @dearness_allowance, @special_pay, @other_allowance, @provident_fund, @other_benefits,";
                    query = query + " @entered_by, @ipaddress, @app_id, @active, @employee_id, CURRENT_TIMESTAMP, @updated_ipaddress, @entered_at)";
                    cmd.CommandText = query;
                    cmd.Parameters.AddWithValue("@employee_name", ViewState["employee_name"].ToString());
                    cmd.Parameters.AddWithValue("@designation", ViewState["designation"].ToString());
                    cmd.Parameters.AddWithValue("@present_pay_scale", ViewState["present_pay_scale"].ToString());
                    cmd.Parameters.AddWithValue("@temporary_permanent", ViewState["temporary_permanent"].ToString());
                    cmd.Parameters.AddWithValue("@present_pay", ViewState["present_pay"].ToString());
                    cmd.Parameters.AddWithValue("@dearness_allowance", ViewState["dearness_allowance"].ToString());
                    cmd.Parameters.AddWithValue("@special_pay", ViewState["special_pay"].ToString());
                    cmd.Parameters.AddWithValue("@other_allowance", ViewState["other_allowance"].ToString());
                    cmd.Parameters.AddWithValue("@provident_fund", ViewState["provident_fund"].ToString());
                    cmd.Parameters.AddWithValue("@other_benefits", ViewState["other_benefits"].ToString());
                    cmd.Parameters.AddWithValue("@entered_by", ViewState["entered_by"].ToString());
                    cmd.Parameters.AddWithValue("@entered_at", ViewState["entered_at"].ToString());
                    cmd.Parameters.AddWithValue("@ipaddress", ViewState["ipaddress"].ToString());
                    cmd.Parameters.AddWithValue("@app_id", Convert.ToInt64(ViewState["app_id"].ToString()));
                    cmd.Parameters.AddWithValue("@employee_id",Convert.ToInt64(ViewState["employee_id"].ToString()));
                    cmd.Parameters.AddWithValue("@active", 'Y');
                    cmd.Parameters.AddWithValue("@updated_ipaddress", ipaddress);
                    cmd.ExecuteNonQuery();
                    cmd.Parameters.Clear();
                    string update_query = "UPDATE esociety.society_employement SET employee_name = @employee_name, designation = @designation,";
                    update_query = update_query + " present_pay_scale = @present_pay_scale, temporary_permanent = @temporary_permanent, present_pay = @present_pay,";
                    update_query = update_query + " dearness_allowance = @dearness_allowance, special_pay = @special_pay, other_allowance = @other_allowance,";
                    update_query = update_query + " provident_fund = @provident_fund, other_benefits = @other_benefits WHERE app_id = @app_id and active = 'Y'";
                    update_query = update_query + " and employee_id = @employee_id";
                    cmd.CommandText = update_query;
                    cmd.Parameters.AddWithValue("@employee_name", Server.HtmlEncode(txt_MemName.Text));
                    cmd.Parameters.AddWithValue("@designation", TxtDesignation.Text);
                    cmd.Parameters.AddWithValue("@present_pay_scale", TxtBxPayScale.Text);
                    cmd.Parameters.AddWithValue("@temporary_permanent", txtData.Text);
                    cmd.Parameters.AddWithValue("@present_pay", TxtPayMonth.Text);
                    cmd.Parameters.AddWithValue("@dearness_allowance", TxtBxDa.Text);
                    cmd.Parameters.AddWithValue("@special_pay", TxtBxspcialPay.Text);
                    cmd.Parameters.AddWithValue("@other_allowance", TxtBxotherallowance.Text);
                    cmd.Parameters.AddWithValue("@provident_fund", txtprovident.Text);
                    cmd.Parameters.AddWithValue("@other_benefits", txtothers.Text);
                    cmd.Parameters.AddWithValue("@app_id", Convert.ToInt64(Session["AppID"].ToString()));
                    cmd.Parameters.AddWithValue("@employee_id",Convert.ToInt64(ViewState["employee_id"].ToString()));
                    cmd.ExecuteNonQuery();
                    trans.Commit();
                    Response.Write("<script language='javascript'>alert('" + "Employee Updated." + "')</script>");
                    loadGridview(Convert.ToInt64(Session["AppID"].ToString()));
                }
                catch (Exception ex)
                {
                    CreateLogFiles Err = new CreateLogFiles();
                    Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "BtnUpdate_Click  " + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
                    trans.Rollback();
                    loadGridview(Convert.ToInt64(Session["AppID"].ToString()));
                }
                finally
                {
                    conn.Close();
                    btnAdd.Visible = true ;
                    BtnUpdate.Visible = false;
                }
            }
            else
            {
                callModal();
                Label2.Text = "Employee ID Mismatch";
            }

        }

        protected void LkDelete_Click(object sender, EventArgs e)
        {
            try
            {
                LinkButton btn = (LinkButton)sender;
                GridViewRow row = (GridViewRow)btn.NamingContainer;
                string employeeID = ((Label)gridview_employeeData.Rows[row.RowIndex].FindControl("lbEmployeeID")).Text;
                hdEmployeeID.Value = employeeID;
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "none", "<script> $(window).on('load', function () { $('#deleteMsgModal').modal({ backdrop: 'static' });});</script>", false);

            }
            catch (Exception ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "LkDelete_Click  " + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));

            }
        }

        protected void btnConfirm_Click(object sender, EventArgs e)
        {
            NpgsqlConnection conn = new NpgsqlConnection();
            NpgsqlCommand cmd = new NpgsqlCommand();
            conn.ConnectionString = ConfigurationManager.ConnectionStrings["connect"].ConnectionString;
            cmd.Connection = conn;
            conn.Open();
            try
            {
                string employeeID = hdEmployeeID.Value;
                string query = "UPDATE esociety.society_employement SET active = 'N', deleted_at = CURRENT_TIMESTAMP WHERE employee_id = @employee_id and app_id = @app_id and active = 'Y'";
                cmd.CommandText = query;
                cmd.Parameters.AddWithValue("@employee_id", Convert.ToInt64(employeeID));
                cmd.Parameters.AddWithValue("@app_id", Convert.ToInt64(Session["AppID"].ToString()));
                cmd.ExecuteNonQuery();
                Response.Write("<script language='javascript'>alert('" + "Employee Deleted." + "')</script>");
                loadGridview(Convert.ToInt64(Session["AppID"].ToString()));
            }
            catch (Exception ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "btnConfirm_Click  " + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));

            }
            finally
            {
                conn.Close();
            }
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            Response.Redirect("DocumentUpload.aspx");
        }

        protected void btnBack_Click(object sender, EventArgs e)
        {
            Response.Redirect("NormalMembers.aspx");
        }

        protected void gridview_employeeData_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (Convert.ToInt32(Session["status_Id"]) == 1 || Convert.ToInt32(Session["status_Id"]) == 2 || Convert.ToInt32(Session["status_Id"]) == 5)
                {

                }
                else
                {
                    ((DataControlField)gridview_employeeData.Columns
                .Cast<DataControlField>()
                .Where(fld => fld.HeaderText == "Delete")
                .SingleOrDefault()).Visible = false;
                    ((DataControlField)gridview_employeeData.Columns
                   .Cast<DataControlField>()
                   .Where(fld => fld.HeaderText == "Edit")
                   .SingleOrDefault()).Visible = false;
                }

               

            }
            catch (Exception ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "gridview_employeeData_RowDataBound" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
            }
        }

        public void get_status()
        {
            try
            {
                if (Convert.ToInt32(Session["status_Id"]) == 1 || Convert.ToInt32(Session["status_Id"]) == 2 || Convert.ToInt32(Session["status_Id"]) == 5)
                {
                    btnAddEmployee.Visible = true;



                }
                else
                {
                    btnAddEmployee.Visible = false;
                  

                }
            }
            catch (Exception ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "get_status()" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
            }
        }
    }
}