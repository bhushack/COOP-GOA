using GoaSocietyRegistration.Development;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace GoaSocietyRegistration.Organization
{
    public partial class AddOldSociety : System.Web.UI.Page
    {
        Insert ins = new Insert();
        System.Globalization.CultureInfo french = new System.Globalization.CultureInfo("fr-FR");
        string ipaddress = Utility.getIP();
        Validate _val = new Validate();
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
                if (!IsPostBack)
                {
                    Utility.FillDistrictSoc(ddlSocDistrict);
                    ddlSocDistrict.SelectedValue = Session["DistrictID"].ToString();
                    ddlSocDistrict.Enabled = false;
                    Utility.FillTaluka(ddlSocTaluka, Convert.ToInt32(ddlSocDistrict.SelectedValue));                                      
                }
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

        protected void permission_Click(object sender, EventArgs e)
        {
            Response.Redirect("Dashboard.aspx");
        }

        protected void TxtBxRegNo_TextChanged(object sender, EventArgs e)
        {

        }

        protected void LkBtnClear_Click(object sender, EventArgs e)
        {
            try
            {
                TxtBxSocName.Text = "";
                TxtBxRegNo.Text = "";
                TxtBxSocRegYear.Text = "";
                TxtBxSocRegDate.Text = "";
                ddlSocTaluka.SelectedValue = "-1";
                TxtBxSocAddr.Text = "";
                lblError.Text = "";
                errorsocregno.Text = "";

            }
            catch(Exception ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "LkBtnClear_Click" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));

            }
        }

        protected void LkBtnAddSoc_Click(object sender, EventArgs e)
        {
            if (TxtBxSocName.Text == "" || TxtBxSocName.Text == null)
            {
                lblError.Text = "";
                lblError.Text = "Enter Society Name";
                
            }
            else if (!_val.validateData(TxtBxSocName.Text.Replace('\'', '`'), _val.society_name))
            {
                lblError.Text = "";
                lblError.Text = "Invalid Society Name";
               
            }
            else if (!(Convert.ToInt32(checksocietyname(TxtBxSocName.Text)) == 0))
            {
                lblError.Text = "";
                lblError.Text = "Society Name already exists";
            }
            else if (TxtBxRegNo.Text == "" || TxtBxRegNo.Text == null)
            {
                lblError.Text = "";
                lblError.Text = "Enter Society Registration Number";
                
            }
            else if (!(Convert.ToInt32(checkRegistrationNo(TxtBxRegNo.Text)) == 0))
            {
                lblError.Text = "";
                lblError.Text = "Registration Number already exists";
            }
            else if (TxtBxSocRegYear.Text == "" || TxtBxSocRegYear.Text == null)
            {
                lblError.Text = "";
                lblError.Text = "Enter Society Registration Year";

            }
            else if (Convert.ToInt64(TxtBxSocRegYear.Text.Trim()) < 1947 || Convert.ToInt64(TxtBxSocRegYear.Text.Trim()) > Convert.ToInt32(System.DateTime.Now.Year.ToString()))
            {
                lblError.Text = "";
                lblError.Text = "Invalid Society Registration Year";

            }
            else if (TxtBxSocRegDate.Text == "" || TxtBxSocRegDate.Text == null)
            {
                lblError.Text = "";
                lblError.Text = "Enter Society Registration Date";

            }
            else if (ddlSocTaluka.SelectedValue == "-1")
            {
                lblError.Text = "";
                lblError.Text = "Select Society Taluka";
                
            }
            else if (TxtBxSocAddr.Text != "" && TxtBxSocAddr.Text != null && !_val.validateData(TxtBxSocAddr.Text, _val.reamrks_validation))
            {
                lblError.Text = "";
                lblError.Text = "Special Characters Not Allowed in Society Address";
               
            }
            else
            {
                string user_fullname = Session["userfirstname"].ToString() + " " + Session["usermiddlename"].ToString() + " " + Session["userlastname"].ToString();

                NpgsqlConnection conn = new NpgsqlConnection();
                NpgsqlCommand cmd = new NpgsqlCommand();
                conn.ConnectionString = ConfigurationManager.ConnectionStrings["connect"].ConnectionString;
                cmd.Connection = conn;            
               
                try
                {
                    conn.Open();
                    string insertQuery = "";
                    if(ddlSocDistrict.SelectedValue == "551")
                    {
                        insertQuery = "INSERT into esociety.society_all_north(socname, socregid, regdate, socdistrict, active, society_year, old_village_name, socaddr, datemodified, added_by, added_by_name, added_at, added_ipaddress)";
                        insertQuery = insertQuery + "VALUES(@socname, @socregid, @regdate, @socdistrict,'Y', @society_year, @old_village_name, @socaddr, 0, @added_by, @added_by_name, current_timestamp, @added_ipaddress)";

                    }
                    else if(ddlSocDistrict.SelectedValue == "552")
                    {
                        insertQuery = "INSERT into esociety.society_all(socname, socregid, regdate, socdistrict, active, society_year, old_village_name, socaddr, datemodified, added_by, added_by_name, added_at, added_ipaddress)";
                        insertQuery = insertQuery + "VALUES(@socname, @socregid, @regdate, @socdistrict, 'Y', @society_year, @old_village_name, @socaddr,0, @added_by, @added_by_name, current_timestamp, @added_ipaddress)";
                         
                    }
                    cmd.CommandText = insertQuery;
                    cmd.Parameters.AddWithValue("@socname", TxtBxSocName.Text.ToUpper().Replace('\'', '`'));
                    cmd.Parameters.AddWithValue("@socregid",TxtBxRegNo.Text.ToUpper());
                    cmd.Parameters.AddWithValue("@regdate", TxtBxSocRegDate.Text);
                    cmd.Parameters.AddWithValue("@socdistrict", Convert.ToInt32(ddlSocDistrict.SelectedValue));
                    cmd.Parameters.AddWithValue("@society_year", Convert.ToInt32(TxtBxSocRegYear.Text));
                    cmd.Parameters.AddWithValue("@old_village_name", ddlSocTaluka.SelectedItem.ToString());
                    cmd.Parameters.AddWithValue("@socaddr", TxtBxSocAddr.Text);              
                    cmd.Parameters.AddWithValue("@added_by", Session["firstname"].ToString());
                    cmd.Parameters.AddWithValue("@added_by_name", user_fullname);
                    cmd.Parameters.AddWithValue("@added_ipaddress", ipaddress);                    
                    cmd.ExecuteNonQuery();
                    cmd.Parameters.Clear();

                    Label11.Text = "Society Details added successfully";
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "none", "<script> $(window).on('load', function () { $('#successmodal').modal({ backdrop: 'static' });});</script>", false);

                    RecordUserAction("Insert", "Sucessfully Added Society in table", "S", "NA", 1);

                    TxtBxSocName.Text = "";
                    TxtBxRegNo.Text = "";
                    TxtBxSocRegYear.Text = "";
                    TxtBxSocRegDate.Text = "";
                    ddlSocTaluka.SelectedValue = "-1";
                    TxtBxSocAddr.Text = "";
                    lblError.Text = "";

                }
                catch (NpgsqlException ex)
                {
                    CreateLogFiles Err = new CreateLogFiles();
                    Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "LkBtnAddSoc_Click()" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
                   
                    RecordUserAction("Insert", "Exception while Adding Society in table", "F", "NA", 1);
                    Label50.Text = "Error while adding Society Details. Please try Again!";
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "none", "<script> $(window).on('load', function () { $('#errormodal').modal({ backdrop: 'static' });});</script>", false);


                }
                finally
                {
                    conn.Close();
                  
                }
            }

        }

        protected int checkRegistrationNo(string socregno)
        {
            int i = 0;
            if (socregno == null || socregno == "" || socregno.Trim().Length == 0)
            {
                errorsocregno.ForeColor = System.Drawing.Color.Red;
                errorsocregno.Text = "Enter Registration No";
                i = 2;
            }
            else
            {
                string socname = socregno.ToUpper().Trim();
                errorsocregno.Text = "";

                string query = "";
                NpgsqlConnection conn = new NpgsqlConnection();
                NpgsqlCommand cmd = new NpgsqlCommand();
                conn.ConnectionString = ConfigurationManager.ConnectionStrings["connect"].ConnectionString;
                cmd.Connection = conn;                 
                    
                try
                {
                    conn.Open();
                    if(ddlSocDistrict.SelectedValue == "551" )
                    {
                        query = "select socregid from esociety.society_all_north where upper(socregid)=@socregid";
                    }
                    else if(ddlSocDistrict.SelectedValue == "552")
                    {
                        query = "select socregid from esociety.society_all where upper(socregid)=@socregid";

                    }
                        
                    cmd.CommandText = query;
                    cmd.Parameters.AddWithValue("@socregid", socname);
                    NpgsqlDataReader dr = cmd.ExecuteReader();

                    if (dr.Read())
                    {
                        errorsocregno.Visible = true;
                        errorsocregno.Text = "This Registration Nymber is already exists";
                        errorsocregno.ForeColor = System.Drawing.Color.Red;                        
                        dr.Close();
                       
                        i = 1;
                    }
                    else
                    {                       
                        dr.Close();
                    }

                        
                       

                }
                catch (NpgsqlException ex)
                {
                    CreateLogFiles Err = new CreateLogFiles();
                    Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "checkRegistrationNo()" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
                    Response.Write("<script language='javascript'>alert('" + " Error Society Registration No check from db  " + "')</script>");
                   
                    
                }
                finally
                {
                    conn.Close();
                }
               


            }
            return i;
        }

        protected void ddlSocDistrict_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                Utility.FillTaluka(ddlSocTaluka, Convert.ToInt32(ddlSocDistrict.SelectedValue));
            }
            catch (Exception ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "ddlSocDistrict_SelectedIndexChanged" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
            }

        }


        protected int checksocietyname(string society_name)
        {
            int i = 0, namefound = 0;           
           
            string socname = Regex.Replace(society_name, @"\s+", " ");
            lblError.Text = "";           

            
            NpgsqlConnection conn = new NpgsqlConnection();
            NpgsqlCommand cmd = new NpgsqlCommand();
            conn.ConnectionString = ConfigurationManager.ConnectionStrings["connect"].ConnectionString;
            cmd.Connection = conn;
            conn.Open();
            NpgsqlTransaction myTrans = conn.BeginTransaction();
            cmd.Transaction = myTrans;
            try
            {

                // string query1 = "Select socname from esociety.society_all where trim(regexp_replace(socname, '\\s+', ' ', 'g')) LIKE @socname";
                string query1 = "Select socname from esociety.society_all where trim(regexp_replace((regexp_replace(socname, '\\s+', ' ', 'g')),'\\s*,\\s*',',','g')) LIKE @socname";

                cmd.CommandText = query1;
                cmd.Parameters.AddWithValue("@socname", socname.ToUpper().Trim());
                NpgsqlDataReader rd = cmd.ExecuteReader();


                if (rd.Read())
                {
                    lblError.Visible = true;
                    lblError.Text = "Society name is already exists";
                    lblError.ForeColor = System.Drawing.Color.Red;
                    
                    namefound = 1;
                    i = 1;
                }
               
                rd.Close();
              

                if (namefound == 0)
                {
                    //string query2 = "Select socname from esociety.society_all_north where trim(regexp_replace(socname, '\\s+', ' ', 'g')) LIKE @socname";
                    string query2 = "Select socname from esociety.society_all_north where trim(regexp_replace((regexp_replace(socname, '\\s+', ' ', 'g')),'\\s*,\\s*',',','g')) LIKE @socname";
                    cmd.CommandText = query2;
                    cmd.Parameters.AddWithValue("@socname", socname.ToUpper().Trim());
                    NpgsqlDataReader rd1 = cmd.ExecuteReader();


                    if (rd1.Read())
                    {
                        lblError.Visible = true;
                        lblError.Text = "Society name is already registered. Please try different Name";
                        lblError.ForeColor = System.Drawing.Color.Red;                      
                        namefound = 1;
                        i = 1;
                    }
                    else
                    {
                        namefound = 0;
                        lblError.Visible = true;
                        lblError.Text = "Society name available";
                        lblError.ForeColor = System.Drawing.Color.Green;                       
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
                RecordUserAction("Read", "Exception while checking society name in table", "F", "NA", 1);
                Response.Write("<script language='javascript'>alert('" + " Error Society name check from db  " + "')</script>");
                i = 2;
                myTrans.Rollback();
            }
            finally
            {
                conn.Close();
            }
                       
            return i;
        }
    }
}