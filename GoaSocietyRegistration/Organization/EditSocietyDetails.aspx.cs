using GoaSocietyRegistration.Development;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Npgsql;
using System.Configuration;
using System.IO;
using System.Data;
using System.Text.RegularExpressions;

namespace GoaSocietyRegistration.Organization
{
    public partial class EditSocietyDetails : System.Web.UI.Page
    {
        Insert ins = new Insert();
        System.Globalization.CultureInfo french = new System.Globalization.CultureInfo("fr-FR");
        string ipaddress = Utility.getIP();
        string macaddress = Utility.GetMACAddress(Utility.getIP());

        Validate _val = new Validate();
        //DataTable dtsort = new DataTable();
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
                Label5.Text = "Permission Denied. Please Contact to your Admin.";
                Label5.ForeColor = System.Drawing.Color.Red;
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "none", "<script> $(window).on('load', function () { $('#permission_error_modal').modal({ backdrop: 'static' });});</script>", false);

            }
            else
            {
                HttpContext.Current.Response.AddHeader("Cache-Control", "no-cache, no-store, must-revalidate");
                HttpContext.Current.Response.AddHeader("Pragma", "no-cache");
                HttpContext.Current.Response.AddHeader("Expires", "0");
                if (!IsPostBack)
                {
                    FillYear();
                    Utility.FillDistrictSoc(ddl_district);
                    loadsocietylist(1);
                }
            }



        }

        private bool checkroles()
        {
            bool role_hacked = false;
            List<int> AllowedRoles = new List<int> {1, 3, 4 };
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


        protected void loadsocietylist(int index)
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

                string searchQuery = "SELECT socname, socregid, regdate, socdistrict, mst_district.\"DistrictName\" as socdist, society_year, old_village_name, socaddr, reg_date, datemodified from";
                searchQuery = searchQuery + " (SELECT * from esociety.society_all UNION ALL SELECT * from esociety.society_all_north)temptable join esociety.mst_district on";
                searchQuery = searchQuery + " temptable.socdistrict = esociety.mst_district.\"DistrictID\" where "; // socdistrict=@socdistrict and

                if (index == 1)
                {
                    searchQuery = searchQuery + " society_year=@society_year order by society_year,socregid ASC, socdistrict DESC";
                }
                else if (index == 2)
                {
                    searchQuery = searchQuery + " upper(socregid) LIKE @socregid order by society_year,socdistrict,socregid ASC";
                }
                else if (index == 3)
                {
                    searchQuery = searchQuery + " upper(socname) LIKE @socname order by society_year,socdistrict,socregid ASC";
                }
               
                               
                cmd.CommandText = searchQuery;
                //cmd.Parameters.AddWithValue("@socdistrict", Convert.ToInt32(Session["DistrictID"].ToString()));
                cmd.Parameters.AddWithValue("@society_year", Convert.ToInt32(ddlyear.SelectedValue));
                if (index == 2)
                {
                    cmd.Parameters.AddWithValue("@socregid", "%" + TxtBxSearchRegno.Text.ToUpper().Trim() + "%");
                }               
                else if (index == 3)
                {
                    cmd.Parameters.AddWithValue("@socname", "%" + TxtBxSearchRegName.Text.ToUpper().Trim() + "%");
                }
                NpgsqlDataAdapter da = new NpgsqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                da.Fill(ds);
              
                GridviewSocietyList.DataSource = ds;
                GridviewSocietyList.DataBind();

                RecordUserAction("Read", "Society List loaded in Gridview", "S", "NA", 1);
               // Label3.Text = Server.HtmlEncode(GridviewSocietyList.Rows.Count.ToString());
                Label3.Text = ds.Tables[0].Rows.Count.ToString();

                myTrans.Commit();

            }
            catch (NpgsqlException ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "loadsocietylist()" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
                RecordUserAction("Read", "Exception while Society List in gridview", "F", "NA", 1);
                myTrans.Rollback();

            }
            finally
            {
                conn.Close();
                SocietyList.Visible = true;
            }
        }

       

        public void FillYear()
        {
            int currentyear = Convert.ToInt32(System.DateTime.Now.Year.ToString());
            for (int i = 1947; i <= currentyear; i++)
            {
                ddlyear.Items.Add(i.ToString());
            }
          
            ddlyear.SelectedValue = currentyear.ToString();
        }


        protected void ddl_district_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        protected void LbEditSociety_Click(object sender, EventArgs e)
        {

            bool societyfound = false;
            LinkButton btn = (LinkButton)sender;
            GridViewRow row = (GridViewRow)btn.NamingContainer;
            string regid = ((Label)GridviewSocietyList.Rows[row.RowIndex].FindControl("LbRegId")).Text;

            HiddenField hddistrict = row.FindControl("hfdistrict") as HiddenField;

            lblError.Text = "";

            if (Convert.ToInt32(hddistrict.Value) == Convert.ToInt32(Session["DistrictID"].ToString()) && Convert.ToInt32(Session["ROLE"]) == 3) // edit allowed only for DR
            {

                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "none", "<script> $(window).on('load', function () { $('#editsocietymodal').modal({ backdrop: 'static' });});</script>", false);
 
                NpgsqlConnection conn = new NpgsqlConnection();
                NpgsqlCommand cmd = new NpgsqlCommand();
                conn.ConnectionString = ConfigurationManager.ConnectionStrings["connect"].ConnectionString;
                cmd.Connection = conn;
                try
                {
                    conn.Open();

                    string query = "Select socname,socregid,regdate,socdistrict,society_year,old_village_name,socaddr,reg_date,datemodified,added_by,added_by_name,added_at,added_ipaddress,";
                    query = query + "updated_by,updated_by_name,updated_at,updated_ipaddress from esociety.society_all where socregid=@socregid and socdistrict=@socdistrict";
                    cmd.CommandText = query;
                    cmd.Parameters.AddWithValue("@socregid", regid);
                    cmd.Parameters.AddWithValue("@socdistrict", Convert.ToInt32(hddistrict.Value));
                    NpgsqlDataReader rd = cmd.ExecuteReader();
                    if (rd.Read())
                    {
                        societyfound = true;
                        TxtBxSocName.Text = Server.HtmlEncode(rd["socname"].ToString());
                        ViewState["socname"] = Server.HtmlEncode(rd["socname"].ToString());
                        TxtBxRegNo.Text = Server.HtmlEncode(rd["socregid"].ToString());
                        ViewState["socregid"] = Server.HtmlEncode(rd["socregid"].ToString());
                        ddl_district.SelectedValue = Server.HtmlEncode(rd["socdistrict"].ToString());
                        ViewState["socdistrict"] = Server.HtmlEncode(rd["socdistrict"].ToString());
                        ViewState["regdate"] = Server.HtmlEncode(rd["regdate"].ToString());
                        if (Convert.ToInt32(rd["datemodified"].ToString()) == 0)
                        {

                            if (Server.HtmlEncode(rd["regdate"].ToString()) != "" && (Server.HtmlEncode(rd["regdate"].ToString()) != null))
                            {
                                TxtBxRegDate.Text = Convert.ToDateTime(rd["regdate"], french).Date.ToString("dd/MM/yyyy");
                            }
                            ViewState["reg_date"] = "";
                        }
                        else if (Convert.ToInt32(rd["datemodified"].ToString()) == 1)
                        {

                            if (rd["reg_date"].ToString() != "" && (rd["reg_date"].ToString() != null))
                            {
                                TxtBxRegDate.Text = Convert.ToDateTime(rd["reg_date"], french).Date.ToString("dd/MM/yyyy");
                            }
                            ViewState["reg_date"] = rd["reg_date"].ToString();
                        }
                        ViewState["datemodified"] = Server.HtmlEncode(rd["datemodified"].ToString());
                        ViewState["added_by"] = Server.HtmlEncode(rd["added_by"].ToString());
                        ViewState["added_by_name"] = Server.HtmlEncode(rd["added_by_name"].ToString());
                        ViewState["added_at"] = Server.HtmlEncode(rd["added_at"].ToString());
                        ViewState["added_ipaddress"] = Server.HtmlEncode(rd["added_ipaddress"].ToString());
                        ViewState["society_year"] = Server.HtmlEncode(rd["society_year"].ToString());
                        TxtBxVillage.Text = Server.HtmlEncode(rd["old_village_name"].ToString());
                        ViewState["old_village_name"] = Server.HtmlEncode(rd["old_village_name"].ToString());
                        TxtBxAddr.Text = Server.HtmlEncode(rd["socaddr"].ToString());
                        ViewState["socaddr"] = Server.HtmlEncode(rd["socaddr"].ToString());
                        ViewState["updated_by"] = Server.HtmlEncode(rd["updated_by"].ToString());
                        ViewState["updated_at"] = Server.HtmlEncode(rd["updated_at"].ToString());
                        ViewState["updated_ipaddress"] = Server.HtmlEncode(rd["updated_ipaddress"].ToString());
                        ViewState["updated_by_name"] = Server.HtmlEncode(rd["updated_by_name"].ToString());
                        ViewState["tabletoupdate"] = "society_all";



                    }
                    rd.Close();

                    if (!societyfound)
                    {
                        string query1 = "Select socname,socregid,regdate,socdistrict,society_year,old_village_name,socaddr,reg_date,datemodified,added_by,added_by_name,added_at,added_ipaddress,";
                        query1 = query1 + " updated_by,updated_at,updated_ipaddress,updated_by_name from esociety.society_all_north where socregid=@socregid and socdistrict=@socdistrict";
                        cmd.CommandText = query1;
                        cmd.Parameters.AddWithValue("@socregid", regid);
                        cmd.Parameters.AddWithValue("@socdistrict", Convert.ToInt32(hddistrict.Value));
                        NpgsqlDataReader dr = cmd.ExecuteReader();
                        if (dr.Read())
                        {
                            societyfound = true;
                            TxtBxSocName.Text = Server.HtmlEncode(dr["socname"].ToString());
                            ViewState["socname"] = Server.HtmlEncode(dr["socname"].ToString());
                            TxtBxRegNo.Text = Server.HtmlEncode(dr["socregid"].ToString());
                            ViewState["socregid"] = Server.HtmlEncode(dr["socregid"].ToString());
                            ddl_district.SelectedValue = Server.HtmlEncode(dr["socdistrict"].ToString());
                            ViewState["socdistrict"] = Server.HtmlEncode(dr["socdistrict"].ToString());
                            ViewState["regdate"] = Server.HtmlEncode(dr["regdate"].ToString());
                            if (Convert.ToInt32(dr["datemodified"].ToString()) == 0)
                            {

                                if (dr["regdate"].ToString() != "" && (dr["regdate"].ToString() != null))
                                {
                                    TxtBxRegDate.Text = Convert.ToDateTime(dr["regdate"], french).Date.ToString("dd/MM/yyyy");
                                }
                                ViewState["reg_date"] = "";
                            }
                            else if (Convert.ToInt32(dr["datemodified"].ToString()) == 1)
                            {

                                if (dr["reg_date"].ToString() != "" && (dr["reg_date"].ToString() != null))
                                {
                                    TxtBxRegDate.Text = Convert.ToDateTime(dr["reg_date"], french).Date.ToString("dd/MM/yyyy");
                                }
                                ViewState["reg_date"] = dr["reg_date"].ToString();
                            }

                            ViewState["datemodified"] = Server.HtmlEncode(dr["datemodified"].ToString());
                            ViewState["added_by"] = Server.HtmlEncode(rd["added_by"].ToString());
                            ViewState["added_by_name"] = Server.HtmlEncode(rd["added_by_name"].ToString());
                            ViewState["added_at"] = Server.HtmlEncode(rd["added_at"].ToString());
                            ViewState["added_ipaddress"] = Server.HtmlEncode(rd["added_ipaddress"].ToString());
                            ViewState["society_year"] = Server.HtmlEncode(dr["society_year"].ToString());
                            TxtBxVillage.Text = Server.HtmlEncode(dr["old_village_name"].ToString());
                            ViewState["old_village_name"] = Server.HtmlEncode(dr["old_village_name"].ToString());
                            TxtBxAddr.Text = Server.HtmlEncode(dr["socaddr"].ToString());
                            ViewState["socaddr"] = Server.HtmlEncode(dr["socaddr"].ToString());
                            ViewState["updated_by"] = Server.HtmlEncode(rd["updated_by"].ToString());
                            ViewState["updated_at"] = Server.HtmlEncode(rd["updated_at"].ToString());
                            ViewState["updated_ipaddress"] = Server.HtmlEncode(rd["updated_ipaddress"].ToString());
                            ViewState["updated_by_name"] = Server.HtmlEncode(rd["updated_by_name"].ToString());
                            ViewState["tabletoupdate"] = "society_all_north";

                        }
                        dr.Close();
                    }



                }
                catch (NpgsqlException ex)
                {
                    CreateLogFiles Err = new CreateLogFiles();
                    Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "LbEditSociety_Click()" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
                    RecordUserAction("Read", "Exception while loading Society Details in Edit Modal", "F", "NA", 1);

                }
                finally
                {
                    conn.Close();
                }
            }
            else
            {
                if(Convert.ToInt32(Session["ROLE"]) == 4)
                {
                    Label4.Text = "You can't edit details of this Society.";
                }
                else
                {
                    Label4.Text = "You can't edit details of this Society. It belongs to South district";
                }
               
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "none", "<script> $(window).on('load', function () { $('#myerrorModal').modal({ backdrop: 'static' });});</script>", false);
                 
            }


        }

        protected void btnUpdate_Click(object sender, EventArgs e)
        {
            if (ViewState["socregid"].ToString() != "" && ViewState["socregid"].ToString() != null)
            {

                if (TxtBxSocName.Text == "" || TxtBxSocName.Text == null)
                {
                    lblError.Text = "";
                    lblError.Text = "Enter Society Name";
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "none", "<script> $(window).on('load', function () { $('#editsocietymodal').modal({ backdrop: 'static' });});</script>", false);

                }
                else if (!_val.validateData(TxtBxSocName.Text.Replace('\'', '`'), _val.society_name))
                {
                    lblError.Text = "";
                    lblError.Text = "Invalid Society Name";
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "none", "<script> $(window).on('load', function () { $('#editsocietymodal').modal({ backdrop: 'static' });});</script>", false);

                }
                else if (!(Convert.ToInt32(checksocietyname(TxtBxSocName.Text)) == 0))
                {
                    lblError.Text = "";
                    lblError.Text = "Society Name already exists";
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "none", "<script> $(window).on('load', function () { $('#editsocietymodal').modal({ backdrop: 'static' });});</script>", false);

                }
                else if (TxtBxRegDate.Text == "" || TxtBxRegDate.Text == null)
                {
                    lblError.Text = "";
                    lblError.Text = "Enter Society Registration Date";
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "none", "<script> $(window).on('load', function () { $('#editsocietymodal').modal({ backdrop: 'static' });});</script>", false);

                }
                else if (TxtBxVillage.Text != "" && TxtBxVillage.Text != null && !_val.validateData(TxtBxVillage.Text, _val.reamrks_validation))
                {
                    lblError.Text = "";
                    lblError.Text = "Special Characters Not Allowed in Village Name";
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "none", "<script> $(window).on('load', function () { $('#editsocietymodal').modal({ backdrop: 'static' });});</script>", false);

                }
                else if (TxtBxAddr.Text != "" && TxtBxAddr.Text != null && !_val.validateData(TxtBxAddr.Text, _val.reamrks_validation))
                {
                    lblError.Text = "";
                    lblError.Text = "Special Characters Not Allowed in Society Address";
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "none", "<script> $(window).on('load', function () { $('#editsocietymodal').modal({ backdrop: 'static' });});</script>", false);

                }

                else
                {
                    string user_fullname = Session["userfirstname"].ToString() + " " + Session["usermiddlename"].ToString() + " " + Session["userlastname"].ToString();
                    string query = "";
                    NpgsqlConnection conn = new NpgsqlConnection();
                    NpgsqlCommand cmd = new NpgsqlCommand();
                    conn.ConnectionString = ConfigurationManager.ConnectionStrings["connect"].ConnectionString;
                    cmd.Connection = conn;
                    conn.Open();
                    NpgsqlTransaction myTrans = conn.BeginTransaction();
                    cmd.Transaction = myTrans;

                    try
                    {
                        string insertQuery = "INSERT into esociety.history_society_all(socname,socregid,regdate,socdistrict,society_year,old_village_name,socaddr,created_at,created_by,ipaddress,macaddress,reg_date,datemodified,added_by,added_by_name,added_at,added_ipaddress,updated_by,updated_by_name,updated_at,updated_ipaddress)";
                        insertQuery = insertQuery + "VALUES(@socname,@socregid,@regdate,@socdistrict,@society_year,@old_village_name,@socaddr,current_timestamp,@created_by,@ipaddress,@macaddress,@reg_date,@datemodified,@added_by,@added_by_name,@added_at,@added_ipaddress,@updated_by,@updated_by_name,@updated_at,@updated_ipaddress)";
                        cmd.CommandText = insertQuery;
                        cmd.Parameters.AddWithValue("@socname", ViewState["socname"].ToString());
                        cmd.Parameters.AddWithValue("@socregid", ViewState["socregid"].ToString());
                        cmd.Parameters.AddWithValue("@regdate", ViewState["regdate"].ToString());
                        cmd.Parameters.AddWithValue("@socdistrict", Convert.ToInt32(ViewState["socdistrict"].ToString()));
                        cmd.Parameters.AddWithValue("@society_year", Convert.ToInt32(ViewState["society_year"].ToString()));
                        cmd.Parameters.AddWithValue("@old_village_name", ViewState["old_village_name"].ToString());
                        cmd.Parameters.AddWithValue("@socaddr", ViewState["socaddr"].ToString());
                        cmd.Parameters.AddWithValue("@created_by", Session["firstname"].ToString());
                        cmd.Parameters.AddWithValue("@ipaddress", ipaddress);
                        cmd.Parameters.AddWithValue("@macaddress", macaddress);
                        cmd.Parameters.AddWithValue("@reg_date", ViewState["reg_date"].ToString());
                        cmd.Parameters.AddWithValue("@datemodified", Convert.ToInt32(ViewState["datemodified"].ToString()));
                        cmd.Parameters.AddWithValue("@added_by", ViewState["added_by"].ToString());
                        cmd.Parameters.AddWithValue("@added_by_name", ViewState["added_by_name"].ToString());
                        cmd.Parameters.AddWithValue("@added_at", ViewState["added_at"].ToString());                    
                        cmd.Parameters.AddWithValue("@added_ipaddress", ViewState["added_ipaddress"].ToString());
                        cmd.Parameters.AddWithValue("@updated_by", ViewState["updated_by"].ToString());
                        cmd.Parameters.AddWithValue("@updated_by_name", ViewState["updated_by_name"].ToString());
                        cmd.Parameters.AddWithValue("@updated_at", ViewState["updated_at"].ToString());
                        cmd.Parameters.AddWithValue("@updated_ipaddress", ViewState["updated_ipaddress"].ToString());
                        cmd.ExecuteNonQuery();
                        cmd.Parameters.Clear();

                        if (ViewState["tabletoupdate"].ToString() == "society_all")
                        {
                            query = "Update esociety.society_all SET socname=@socname,reg_date=@reg_date,old_village_name=@old_village_name,socaddr=@socaddr,datemodified=1,updated_by=@updated_by,updated_by_name = @updated_by_name, updated_at = current_timestamp,updated_ipaddress=@updated_ipaddress where socregid=@regid and socdistrict=@socdistrict";

                        }
                        else if (ViewState["tabletoupdate"].ToString() == "society_all_north")
                        {
                            query = "Update esociety.society_all_north SET socname=@socname,reg_date=@reg_date,old_village_name=@old_village_name,socaddr=@socaddr,datemodified=1,updated_by=@updated_by,updated_by_name = @updated_by_name,updated_at = current_timestamp,updated_ipaddress=@updated_ipaddress where socregid=@regid and socdistrict=@socdistrict";

                        }
                        cmd.CommandText = query;

                        cmd.Parameters.AddWithValue("@socname", TxtBxSocName.Text.Replace('\'','`'));
                        cmd.Parameters.AddWithValue("@reg_date", Convert.ToDateTime(TxtBxRegDate.Text, french).Date);
                        cmd.Parameters.AddWithValue("@old_village_name", TxtBxVillage.Text);
                        cmd.Parameters.AddWithValue("@socaddr", TxtBxAddr.Text);
                        cmd.Parameters.AddWithValue("@regid", TxtBxRegNo.Text);
                        cmd.Parameters.AddWithValue("@socdistrict", Convert.ToInt32(Session["DistrictID"].ToString()));
                        cmd.Parameters.AddWithValue("@updated_by", Session["firstname"].ToString());
                        cmd.Parameters.AddWithValue("@updated_by_name", user_fullname);
                        cmd.Parameters.AddWithValue("@updated_ipaddress", ipaddress);

                        cmd.ExecuteNonQuery();
                        cmd.Parameters.Clear();

                        RecordUserAction("Update", "Society Updated in gridview", "S", "NA", 1);

                        myTrans.Commit();

                    }
                    catch (NpgsqlException ex)
                    {
                        CreateLogFiles Err = new CreateLogFiles();
                        Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "btnUpdate_Click()" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
                        myTrans.Rollback();
                        RecordUserAction("Update", "Exception while Updating Society in gridview", "F", "NA", 1);

                    }
                    finally
                    {
                        conn.Close();
                        TxtBxSocName.Text = "";
                        TxtBxRegNo.Text = "";
                        TxtBxRegDate.Text = "";
                        ddl_district.SelectedValue = "-1";
                        TxtBxVillage.Text = "";
                        TxtBxAddr.Text = "";
                        int index = Convert.ToInt32(Rdbtn_search.SelectedValue);
                        loadsocietylist(index);

                    }
                }
            }
        }

        protected void close_modal_Click(object sender, EventArgs e)
        {
            try
            {
                TxtBxSocName.Text = "";
                TxtBxRegNo.Text = "";
                TxtBxRegDate.Text = "";
                ddl_district.SelectedValue = "-1";
                TxtBxVillage.Text = "";
                TxtBxAddr.Text = "";
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "none", "<script> $(window).on('load', function () { $('#editsocietymodal').modal('hide');});</script>", false);
            }
            catch (Exception ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "close_modal_Click" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
            }
        }

        protected void search_society_Click(object sender, EventArgs e)
        {
            int index = Convert.ToInt32(Rdbtn_search.SelectedValue);
            if (index == 2 && (TxtBxSearchRegno.Text == "" || TxtBxSearchRegno.Text == null))
            {
                Label4.Text = "Please Enter Registration No";
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "none", "<script> $(window).on('load', function () { $('#myerrorModal').modal({ backdrop: 'static' });});</script>", false);

            }            
            else if (index == 3 && (TxtBxSearchRegName.Text == "" || TxtBxSearchRegName.Text == null))
            {
                Label4.Text = "Please Enter Registered Name";
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "none", "<script> $(window).on('load', function () { $('#myerrorModal').modal({ backdrop: 'static' });});</script>", false);

            }
            else
            {
                loadsocietylist(index);
            }

        }

        protected void Rdbtn_search_SelectedIndexChanged(object sender, EventArgs e)
        {
            SocietyList.Visible = false;

            if (Rdbtn_search.SelectedValue == "-1")
            {
                searchregno.Visible = false;
                searchregdate.Visible = false;
                searchregname.Visible = false;
                search_society.Enabled = false;
                div_ddlyear.Visible = false;
            }
            else if (Rdbtn_search.SelectedValue == "1")
            {
                searchregno.Visible = false;
                searchregdate.Visible = false;
                searchregname.Visible = false;
                searchbtn.Visible = false;
                div_ddlyear.Visible = true;
                loadsocietylist(1);
            }
            else if (Rdbtn_search.SelectedValue == "2")
            {
                searchregno.Visible = true;
                searchregdate.Visible = false;
                searchregname.Visible = false;
                searchbtn.Visible = true;
                TxtBxSearchRegno.Text = "";
                div_ddlyear.Visible = false;
            }            
            else if (Rdbtn_search.SelectedValue == "3")
            {
                searchregno.Visible = false;
                searchregdate.Visible = false;
                searchregname.Visible = true;
                searchbtn.Visible = true;
                TxtBxSearchRegName.Text = "";
                div_ddlyear.Visible = false;
            }
            int temp = Rdbtn_search.SelectedIndex;
            int index = Convert.ToInt32(Rdbtn_search.SelectedValue);
          

        }


        protected void ddlyear_SelectedIndexChanged(object sender, EventArgs e)
        {
            int index = Convert.ToInt32(Rdbtn_search.SelectedValue);
            loadsocietylist(index);
        }

        protected void permission_Click(object sender, EventArgs e)
        {
            // RecordUserAction("Redirect", "Redirection to Dashboard", "Success", "NA", 1);
            Response.Redirect("Dashboard.aspx");
        }

        protected void GridviewSocietyList_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                HiddenField hfdatemodify = (e.Row.FindControl("hfdatemodify") as HiddenField);
                // HiddenField hfregdate = (e.Row.FindControl("hfregdate") as HiddenField);
                // Label lblt = (e.Row.FindControl("lblTot") as Label);

                if (hfdatemodify.Value == "0")
                {
                    string temp1 = ((Label)e.Row.FindControl("LbDate")).Text;
                    // string regid = (e.Row.FindControl("LbDate").ToString());
                    e.Row.Cells[3].Text = temp1.ToString();
                }

            }
        }

        protected void GridviewSocietyList_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            GridviewSocietyList.PageIndex = e.NewPageIndex;
            int index = Convert.ToInt32(Rdbtn_search.SelectedValue);
            loadsocietylist(index);
        }

        protected void gdSearch_DataBound(object sender, EventArgs e)
        {
            try
            {
                if (GridviewSocietyList.BottomPagerRow != null)
                {
                    DropDownList ddl = (DropDownList)GridviewSocietyList.BottomPagerRow.Cells[0].FindControl("ddlPaging");
                    Label gridpages = (Label)GridviewSocietyList.BottomPagerRow.Cells[0].FindControl("totalpages");

                    for (int cnt = 0; cnt < GridviewSocietyList.PageCount; cnt++)
                    {
                        int curr = cnt + 1;
                        ListItem item = new ListItem(curr.ToString());
                        if (cnt == GridviewSocietyList.PageIndex)
                        {
                            item.Selected = true;
                        }

                        ddl.Items.Add(item);

                    }
                    gridpages.Text = "out of " + GridviewSocietyList.PageCount.ToString();
                }
            }
            catch(Exception ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "gdSearch_DataBound" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
                Response.Write("<script language='javascript'>alert('" + " Something went wrong  " + "')</script>");
            }
        }

        protected void ddlPaging_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                DropDownList ddl = (DropDownList)GridviewSocietyList.BottomPagerRow.Cells[0].FindControl("ddlPaging");
                GridviewSocietyList.PageIndex = ddl.SelectedIndex;

                int index = Convert.ToInt32(Rdbtn_search.SelectedValue);
                loadsocietylist(index);
            }
            catch(Exception ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "ddlPaging_SelectedIndexChanged" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
                Response.Write("<script language='javascript'>alert('" + " Something went wrong  " + "')</script>");
            }

        }

        protected int checksocietyname(string society_name)
        {
            int i = 0, namefound = 0;

            string socname = Regex.Replace(society_name, @"\s+", " ");
            socname = Regex.Replace(socname, @"\s*,\s*", ",");
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

                string query1 = "Select socname from esociety.society_all where trim(regexp_replace((regexp_replace(socname, '\\s+', ' ', 'g')),'\\s*,\\s*',',','g')) LIKE @socname and upper(socregid) != @socregid";
                cmd.CommandText = query1;
                cmd.Parameters.AddWithValue("@socname", socname.ToUpper().Trim());
                cmd.Parameters.AddWithValue("@socregid", TxtBxRegNo.Text.ToUpper().Trim());
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
                    // string query2 = "Select socname from esociety.society_all_north where trim(regexp_replace(socname, '\\s+', ' ', 'g')) LIKE @socname and upper(socregid) != @socregid";
                    string query2 = "Select socname from esociety.society_all_north where trim(regexp_replace((regexp_replace(socname, '\\s+', ' ', 'g')),'\\s*,\\s*',',','g')) LIKE @socname and upper(socregid) != @socregid";

                    cmd.CommandText = query2;
                    cmd.Parameters.AddWithValue("@socname", socname.ToUpper().Trim());
                    cmd.Parameters.AddWithValue("@socregid", TxtBxRegNo.Text.ToUpper().Trim());
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