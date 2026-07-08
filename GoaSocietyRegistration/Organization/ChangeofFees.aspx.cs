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
    public partial class ChangeofFees : System.Web.UI.Page
    {
        System.Globalization.CultureInfo french = new System.Globalization.CultureInfo("fr-FR");
        string ipaddress = Utility.getIP();
        Validate vs = new Validate();
        Insert ins = new Insert();        
        Page_status_Check psc = new Page_status_Check();

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
                Label8.Text = "Permission Denied. Please Contact to your Admin.";
                Label8.ForeColor = System.Drawing.Color.Red;
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "none", "<script> $(window).on('load', function () { $('#permission_error_modal').modal({ backdrop: 'static' });});</script>", false);

            }
            else
            {
                HttpContext.Current.Response.AddHeader("Cache-Control", "no-cache, no-store, must-revalidate");
                HttpContext.Current.Response.AddHeader("Pragma", "no-cache");
                HttpContext.Current.Response.AddHeader("Expires", "0");
            }
        }

        private bool checkroles()
        {
            bool role_hacked = false;
            List<int> AllowedRoles = new List<int> { 3 };
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

        protected void fetchfees()
        {
            if (Session["app_id"] != null)
            {
                psc = ins.getPageStatus(Session["app_id"].ToString());
                int res = 0;

                if (psc.status_id == 8)
                {
                    NpgsqlConnection conn = new NpgsqlConnection();
                    NpgsqlCommand cmd = new NpgsqlCommand();
                    conn.ConnectionString = ConfigurationManager.ConnectionStrings["connect"].ConnectionString;
                    cmd.Connection = conn;


                    try
                    {
                        conn.Open();

                        string query1 = "SELECT app_id, socname,socregid, socdistrict, regdate, lastdateforrenewal, duedate, processfee, penaltyfee, totalfees, remarks,status, created_by_name, created_by_email, created_at, created_ipaddress FROM esociety.society_renewal_fees where app_id=@app_id";
                        cmd.CommandText = query1;
                        cmd.Parameters.AddWithValue("@app_id", Convert.ToInt64(Session["app_id"]));
                        NpgsqlDataReader rd1 = cmd.ExecuteReader();
                        if (rd1.Read())
                        {
                            ViewState["socname"] = rd1["socname"].ToString();
                            ViewState["socregid"] = rd1["socregid"].ToString();
                            ViewState["app_id"] = rd1["app_id"].ToString();
                            ViewState["socdistrict"] = rd1["socdistrict"].ToString();
                            ViewState["regdate"] = rd1["regdate"].ToString();
                            ViewState["lastdateforrenewal"] = rd1["lastdateforrenewal"].ToString();
                            ViewState["duedate"] = rd1["duedate"].ToString();
                            ViewState["processfee"] = rd1["processfee"].ToString();
                            ViewState["penaltyfee"] = rd1["penaltyfee"].ToString();
                            ViewState["totalfees"] = rd1["totalfees"].ToString();
                            ViewState["remarks"] = rd1["remarks"].ToString();
                            ViewState["status"] = rd1["status"].ToString();
                            ViewState["created_by_name"] = rd1["created_by_name"].ToString();
                            ViewState["created_by_email"] = rd1["created_by_email"].ToString();
                            ViewState["created_at"] = rd1["created_at"].ToString();
                            ViewState["created_ipaddress"] = rd1["created_ipaddress"].ToString();

                            TxtBxRenewalDate.Text = (Convert.ToDateTime(rd1["lastdateforrenewal"], french).Date).ToString("yyyy-MM-dd");
                            TxtbxDueDate.Text = (Convert.ToDateTime(rd1["duedate"], french).Date).ToString("yyyy-MM-dd");
                            TxtBxPenalty.Text = Server.HtmlEncode(rd1["penaltyfee"].ToString());
                            txtbxtotalfees.Text = Server.HtmlEncode(rd1["totalfees"].ToString());
                            txtareafeesremarks.InnerText = Server.HtmlEncode(rd1["remarks"].ToString());

                            showdata.Visible = true;
                            TxtBxAppID.Enabled = false;
                            LkSearch.Enabled = false;
                            res = 1;

                        }
                        else
                        {
                            res = 0;
                            Label69.ForeColor = System.Drawing.Color.Red;
                            Label69.Text = "Renewal Fee is not added for this Application.";
                            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "none", "<script> $(window).on('load', function () { $('#MyErrorModal').modal({ backdrop: 'static' });});</script>", false);


                        }
                        rd1.Close();

                    }
                    catch (NpgsqlException ex)
                    {
                        res = 0;
                        CreateLogFiles Err = new CreateLogFiles();
                        Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "fetchfees" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));

                    }
                    finally
                    {
                        conn.Close();
                    }

                    if (res == 1)
                    {
                        fetchregdate();
                    }
                }
                else
                {
                    Label69.ForeColor = System.Drawing.Color.Red;
                    Label69.Text = "You cannot edit application at this stage";
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "none", "<script> $(window).on('load', function () { $('#MyErrorModal').modal({ backdrop: 'static' });});</script>", false);

                }
            }
           
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
                string userloginid = Utility.getUserLoginID(Session["app_id"].ToString());
                string user_fullname = Session["userfirstname"].ToString() + " " + Session["usermiddlename"].ToString() + " " + Session["userlastname"].ToString();
                string text = Utility.getOldRegistrationNo(Session["app_id"].ToString());
                string[] Result = text.Split('|');
                string socregid = Server.HtmlEncode(Result[0]);

                int a = doValidation(txtareafeesremarks.InnerText);

                if (a == 1)
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

                        string histQuery = "INSERT INTO esociety.society_renewal_fees_history(socregid, socname, socdistrict, regdate, lastdateforrenewal, duedate, processfee, penaltyfee, totalfees, app_id, remarks, created_by_name, created_by_email, created_at, created_ipaddress, status)";
                        histQuery = histQuery + " VALUES(@socregid, @socname, @socdistrict, @regdate, @lastdateforrenewal, @duedate, @processfee, @penaltyfee, @totalfees, @app_id, @remarks, @created_by_name, @created_by_email, @created_at, @created_ipaddress, @status)";
                        cmd.CommandText = histQuery;
                        cmd.Parameters.AddWithValue("@socregid", ViewState["socregid"].ToString());
                        cmd.Parameters.AddWithValue("@socname", ViewState["socname"].ToString());                     
                        cmd.Parameters.AddWithValue("@socdistrict",Convert.ToInt32(ViewState["socdistrict"].ToString()));
                        cmd.Parameters.AddWithValue("@regdate",Convert.ToDateTime(ViewState["regdate"].ToString(),french).Date);
                        cmd.Parameters.AddWithValue("@lastdateforrenewal", Convert.ToDateTime(ViewState["lastdateforrenewal"].ToString(), french).Date);
                        cmd.Parameters.AddWithValue("@duedate", Convert.ToDateTime(ViewState["duedate"].ToString(), french).Date);
                        cmd.Parameters.AddWithValue("@processfee", Convert.ToInt32(ViewState["processfee"].ToString()));
                        cmd.Parameters.AddWithValue("@penaltyfee", Convert.ToInt32(ViewState["penaltyfee"].ToString()));
                        cmd.Parameters.AddWithValue("@totalfees", Convert.ToInt32(ViewState["totalfees"].ToString()));
                        cmd.Parameters.AddWithValue("@app_id", Convert.ToInt64(ViewState["app_id"].ToString()));
                        cmd.Parameters.AddWithValue("@remarks", ViewState["remarks"].ToString());
                        cmd.Parameters.AddWithValue("@status", ViewState["status"].ToString());
                        cmd.Parameters.AddWithValue("@created_by_name", ViewState["created_by_name"].ToString());
                        cmd.Parameters.AddWithValue("@created_by_email", ViewState["created_by_email"].ToString());
                        cmd.Parameters.AddWithValue("@created_at", ViewState["created_at"].ToString());
                        cmd.Parameters.AddWithValue("@created_ipaddress", ViewState["created_ipaddress"].ToString());

                        cmd.ExecuteNonQuery();
                        cmd.Parameters.Clear();



                        string updQuery = "Update esociety.society_renewal_fees SET lastdateforrenewal=@lastdateforrenewal, duedate=@duedate, processfee=@processfee, penaltyfee=@penaltyfee,";
                        updQuery = updQuery + " totalfees =@totalfees, remarks=@remarks, created_by_name=@created_by_name, created_by_email=@created_by_email, created_at = current_timestamp, created_ipaddress=@created_ipaddress, status=@status where app_id=@app_id";
                      
                        cmd.CommandText = updQuery;                    
                                               
                        cmd.Parameters.AddWithValue("@lastdateforrenewal", Convert.ToDateTime(TxtBxRenewalDate.Text, french).Date);
                        cmd.Parameters.AddWithValue("@duedate", Convert.ToDateTime(TxtbxDueDate.Text, french).Date);
                        cmd.Parameters.AddWithValue("@processfee", Convert.ToInt32(TxtBxProcessFee.Text));
                        cmd.Parameters.AddWithValue("@penaltyfee", Convert.ToInt32(TxtBxPenalty.Text));
                        cmd.Parameters.AddWithValue("@totalfees", Convert.ToInt32(txtbxtotalfees.Text));
                        cmd.Parameters.AddWithValue("@remarks", txtareafeesremarks.InnerText.Trim());
                        cmd.Parameters.AddWithValue("@created_by_name", user_fullname);
                        cmd.Parameters.AddWithValue("@created_by_email", Session["firstname"].ToString());
                        cmd.Parameters.AddWithValue("@created_ipaddress", ipaddress);
                        cmd.Parameters.AddWithValue("@status", 'E');   // status O is orginal and E is for edited
                        cmd.Parameters.AddWithValue("@app_id", Convert.ToInt64(Session["app_id"].ToString()));                        

                        cmd.ExecuteNonQuery();
                        cmd.Parameters.Clear();

                        string updQuery1 = "Update esociety.society SET processfee=@processfee, regfee=@penaltyfee,  totalfee=@totalfee where login_id=@login_id";

                        cmd.CommandText = updQuery1;

                    
                        cmd.Parameters.AddWithValue("@processfee", Convert.ToInt32(TxtBxProcessFee.Text));
                        cmd.Parameters.AddWithValue("@penaltyfee", Convert.ToInt32(TxtBxPenalty.Text));
                        cmd.Parameters.AddWithValue("@totalfee", Convert.ToInt32(txtbxtotalfees.Text));                      
                        cmd.Parameters.AddWithValue("@login_id", userloginid);

                        cmd.ExecuteNonQuery();
                        cmd.Parameters.Clear();

                        myTrans.Commit();


                        Label65.Text = "Fees Saved Successfully!!";
                        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "none", "<script> $(window).on('load', function () { $('#redirectModal').modal({ backdrop: 'static' });});</script>", false);




                    }
                    catch (NpgsqlException ex)
                    {
                        myTrans.Rollback();
                        CreateLogFiles Err = new CreateLogFiles();
                        Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "LkSaveFees_modalbtn_Click()" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));

                        RecordUserAction("Update", "Error while Saving Changes in fees", "F", "NA", 1);

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

        protected void btnRedirect_Click(object sender, EventArgs e)
        {
            Response.Redirect("ChangeofFees.aspx");
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


        protected void LkSearch_Click(object sender, EventArgs e)
        {
            //psc = ins.getPageStatus(Session["app_id"].ToString());

            if (TxtBxAppID.Text == null || TxtBxAppID.Text == "")
            {
                Label69.ForeColor = System.Drawing.Color.Red;
                Label69.Text = "Application ID is blank";
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "none", "<script> $(window).on('load', function () { $('#MyErrorModal').modal({ backdrop: 'static' });});</script>", false);

            }
            else if (!vs.validateData(TxtBxAppID.Text.Trim(), vs.numericregex))
            {
                Label69.ForeColor = System.Drawing.Color.Red;
                Label69.Text = "Invalid Application ID";
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "none", "<script> $(window).on('load', function () { $('#MyErrorModal').modal({ backdrop: 'static' });});</script>", false);

            }
            else if(Utility.get_districtid(TxtBxAppID.Text.Trim()) != Convert.ToInt32(Session["DistrictID"]))
            {
                Label69.ForeColor = System.Drawing.Color.Red;
                Label69.Text = "This application doesnt belong to your District";
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "none", "<script> $(window).on('load', function () { $('#MyErrorModal').modal({ backdrop: 'static' });});</script>", false);

            }            
            else
            {
                Session["app_id"] = Sanitize.InputText(TxtBxAppID.Text.Trim());
                fetchfees();
            }
        }

        protected void LkClear_Click(object sender, EventArgs e)
        {
            //Response.Redirect("ChangeofFees.aspx");
            showdata.Visible = false;
            TxtBxAppID.Enabled = true;
            LkSearch.Enabled = true;
        }

        protected void lkeditfees_Click(object sender, EventArgs e)
        {
             
            string data = checkechallan(TxtBxAppID.Text);
            string[] result = data.Split('|');
            if (result[3].ToString().Trim().Equals("Y"))
            {
                loadModal("Please Tell the party to delete all old echallan. After that you will be able to change the fees");
               

            }
            else
            {
                //enable edit

                TxtbxDueDate.Enabled = true;
                TxtBxRenewalDate.Enabled = true;
                lkSaveFees.Enabled = true;
                LkCalcFees.Enabled = true;
                lkeditfees.Visible = false;
                lkSaveFees.Visible = true;
                LkCalcFees.Visible = true;
                txtareafeesremarks.Disabled = false;
                lkcancelchanges.Visible = true;

            }

        }


        protected string checkechallan(string app_id)
        {
            NpgsqlConnection conn = new NpgsqlConnection();
            NpgsqlCommand cmd = new NpgsqlCommand();
            conn.ConnectionString = ConfigurationManager.ConnectionStrings["connect"].ConnectionString;
            cmd.Connection = conn;
            string i = "|||";
            try
            {
                conn.Open();
                string query = "SELECT echallan_no,status,total_amt,active FROM esociety.online_payment_details where app_id = @app_id and active ='Y'";
                cmd.CommandText = query;
                cmd.Parameters.AddWithValue("@app_id", Convert.ToInt64(app_id));
                NpgsqlDataReader rd = cmd.ExecuteReader();
                if (rd.Read())
                {
                    i = rd["echallan_no"].ToString() + "|" + rd["status"].ToString() + "|" + rd["total_amt"].ToString() + "|" + rd["active"].ToString();
                }
                else
                {
                    i = "|||";
                }
                rd.Close();
            }
            catch (NpgsqlException ex)
            {
                i = "|||";
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "checkechallan " + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath) + " from " + ipaddress);
                
            }
            finally
            {
                conn.Close();
            }
            return i;
        }

        protected void loadModal(string msg)
        {
            Label69.ForeColor = System.Drawing.Color.Red;
            Label69.Text = msg;
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "none", "<script> $(window).on('load', function () { $('#MyErrorModal').modal({ backdrop: 'static' });});</script>", false);

        }

        protected void LkCalcFees_Click(object sender, EventArgs e)
        {
            try
            {
                if (TxtBxRegistrationDate.Text == null || TxtBxRegistrationDate.Text == "")
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
                else if (Convert.ToDateTime(TxtBxRenewalDate.Text) < Convert.ToDateTime(TxtBxRegistrationDate.Text))
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

        protected void lkcancelchanges_Click(object sender, EventArgs e)
        {
            Response.Redirect("ChangeofFees.aspx");
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

        protected void permission_Click(object sender, EventArgs e)
        {
            Response.Redirect("Dashboard.aspx");
        }
    }
}