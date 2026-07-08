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
    public partial class CancelRegistration : System.Web.UI.Page
    {
        Insert ins = new Insert();
        Validate _val = new Validate();
        string ipaddress = Utility.getIP();
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Check4Tampering() || Page.RouteData.RouteHandler == null)
            {
                // RecordUserAction("Page_Load", "Access request failed. Tampered session", "F");
                SessionManage session = new SessionManage();
                session.__AbandonAdmin(Request, Response);
            }
            else if (checkroles())
            {
                Label5.Text = "Permission Denied. Please Contact to your SRO.";
                Label5.ForeColor = System.Drawing.Color.Red;
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
        public void callMsgModal(string msg)
        {
            Label2.Text = msg;
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "none", "<script> $(window).on('load', function () { $('#msgModal').modal({ backdrop: 'static' });});</script>", false);
        }
        protected void LinkButton1_Click(object sender, EventArgs e)
        {
            if (TxtBxRegNo.Text == "" || TxtBxRegNo.Text == null)
            {
                TxtBxRegNo.Focus();
                callMsgModal("Registration No is blank");
            }
            else if (!_val.validateData(TxtBxRegNo.Text.Trim(), _val.Regno))
            {
                TxtBxRegNo.Focus();
                callMsgModal("Registration No is Invalid / wrong");
            }
            else
            {
                TxtBxRegNo.Enabled = false;
                NpgsqlConnection conn = new NpgsqlConnection();
                NpgsqlCommand cmd = new NpgsqlCommand();
                conn.ConnectionString = ConfigurationManager.ConnectionStrings["connect"].ConnectionString;
                cmd.Connection = conn;
                try
                {
                    int districtID = Convert.ToInt32(Session["DistrictID"].ToString());
                    conn.Open();
                    string query = "SELECT socname,applicant_name,socdistrict,society.app_id FROM esociety.society INNER JOIN esociety.applicant_details on applicant_details.app_id = society.app_id";
                    query = query + " where socregid=@socregid and socdistrict=@socdistrict";
                    cmd.CommandText = query;
                    cmd.Parameters.AddWithValue("@socregid", TxtBxRegNo.Text.Trim());
                    cmd.Parameters.AddWithValue("@socdistrict", Convert.ToInt32(Session["DistrictID"].ToString()));
                    NpgsqlDataReader rd = cmd.ExecuteReader();
                    if (rd.Read())
                    {
                        string app_disID = rd["socdistrict"].ToString();
                        if (Convert.ToInt16(app_disID) == districtID)
                        {
                            TxtBxSocName.Text = rd["socname"].ToString().Trim();
                            TxtAppName.Text = rd["applicant_name"].ToString().Trim();
                            hfAppID.Value = rd["app_id"].ToString().Trim();
                            rd.Close();
                            data.Visible = true;
                        }
                        else
                        {
                            callMsgModal("Applciation for society is of another District");
                            data.Visible = false;
                            rd.Close();
                        }
                    }
                    else
                    {
                        callMsgModal("No record found");
                        data.Visible = false;
                    }
                }
                catch (NpgsqlException ex)
                {
                    CreateLogFiles Err = new CreateLogFiles();
                    Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "LinkButton1_Click()" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
                    RecordUserAction("Read", " Read Society comes for cancellation", "F", "NA", 1);
                    Response.Write("<script language='javascript'>alert('" + "Couldn't fetch the data" + "')</script>");
                    data.Visible = false;
                }
                finally
                {
                    conn.Close();
                }
            }
        }
        protected void LinkButton3_Click(object sender, EventArgs e)
        {
            Response.Redirect("CancelRegistration.aspx");
        }

        public int CountWords(string input)
        {
            // Split the string by spaces, tabs, newlines, or any white space and remove empty entries
            var words = input.Split(new[] { ' ', '\t', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);

            // Return the number of words
            return words.Length;
        }

        protected void LinkButton2_Click(object sender, EventArgs e)
        {
            if (txtRemarks.Text == "" || txtRemarks.Text == null)
            {
                txtRemarks.Focus();
                callMsgModal("Remarks are blank");
            }
            else if (!_val.validateData(txtRemarks.Text.Trim(), _val.reamrks_validation))
            {
                txtRemarks.Focus();
                callMsgModal("Remarks are Invalid / having special characters");
            }
            else if (!(CountWords(txtRemarks.Text.Trim()) > 30))
            {
                txtRemarks.Focus();
                callMsgModal("Please provide a detail remarks as why this application is being rejected");
            }
            else
            {
                Label7.Text = "You are about to delete the Registration No of the society. Click Delete Registartion to continue";
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "none", "<script> $(window).on('load', function () { $('#deleteModalAlert').modal({ backdrop: 'static' });});</script>", false);                
            }
        }

        protected void lkDelete_Click(object sender, EventArgs e)
        {
            string reg_no = TxtBxRegNo.Text;
            string[] datas = reg_no.Split('/');
            string no = datas[0].ToString().Trim();
            string year = datas[2].ToString().Trim();
            string Curr_year = DateTime.Now.Year.ToString();
            int dis_id = Convert.ToInt32(Session["DistrictID"].ToString());
            if (Curr_year == year)
            {
                NpgsqlConnection conn = new NpgsqlConnection();
                NpgsqlCommand cmd = new NpgsqlCommand();
                conn.ConnectionString = ConfigurationManager.ConnectionStrings["connect"].ConnectionString;
                cmd.Connection = conn; conn.Open();
                NpgsqlTransaction trans = conn.BeginTransaction();
                try
                {
                    cmd.Parameters.Clear();
                    string query = "SELECT (soc_reg_no-1) as seq_no,soc_reg_no FROM esociety.master_sequence where districtid=@districtid and soc_year=@soc_year";
                    cmd.CommandText = query;
                    cmd.Parameters.AddWithValue("@districtid", Convert.ToInt32(dis_id));
                    cmd.Parameters.AddWithValue("@soc_year", Convert.ToInt32(year));
                    NpgsqlDataReader dr = cmd.ExecuteReader();
                    if (dr.Read())
                    {
                        string temp = dr["seq_no"].ToString().Trim();
                        string ori = dr["soc_reg_no"].ToString().Trim();
                        if (temp == no)
                        {
                            dr.Close();
                            //cancellation is possible
                            cmd.Parameters.Clear();
                            string upd_query = "UPDATE esociety.master_sequence set soc_reg_no=@soc_reg_no where districtid=@districtid and soc_year=@soc_year";
                            cmd.CommandText = upd_query;
                            cmd.Parameters.AddWithValue("@soc_reg_no", (Convert.ToInt16(ori) - 1));
                            cmd.Parameters.AddWithValue("@districtid", Convert.ToInt32(dis_id));
                            cmd.Parameters.AddWithValue("@soc_year", Convert.ToInt32(year));
                            cmd.ExecuteNonQuery();
                            cmd.Parameters.Clear();
                            string ins_remarks = "INSERT INTO esociety.registration_cancellation(app_id, soc_name, applicant_name, remarks, done_at, done_by, done_from,reg_no)";
                            ins_remarks = ins_remarks + " VALUES(@app_id, @soc_name, @applicant_name, @remarks, CURRENT_TIMESTAMP, @done_by, @done_from,@reg_no)";
                            cmd.CommandText = ins_remarks;
                            cmd.Parameters.AddWithValue("@app_id", Convert.ToInt64(hfAppID.Value));
                            cmd.Parameters.AddWithValue("@soc_name", TxtBxSocName.Text.Trim());
                            cmd.Parameters.AddWithValue("@applicant_name", TxtAppName.Text.Trim());
                            cmd.Parameters.AddWithValue("@remarks", txtRemarks.Text.Trim());
                            cmd.Parameters.AddWithValue("@done_by", Session["logginname"].ToString().Trim());
                            cmd.Parameters.AddWithValue("@done_from", ipaddress);
                            cmd.Parameters.AddWithValue("@reg_no", TxtBxRegNo.Text.Trim());
                            cmd.ExecuteNonQuery();
                            cmd.Parameters.Clear();
                            string upd_tabel = "update esociety.society set socregid=NULL where app_id = @app_id";
                            cmd.CommandText = upd_tabel;
                            cmd.Parameters.AddWithValue("@app_id", Convert.ToInt64(hfAppID.Value));
                            cmd.ExecuteNonQuery();
                            cmd.Parameters.Clear();
                            string upd_status = "update esociety.status_table set status_id = 9,remarks_reject='Your Application is cancelled.' where app_id = @app_id";
                            cmd.CommandText = upd_status;
                            cmd.Parameters.AddWithValue("@app_id", Convert.ToInt64(hfAppID.Value));
                            cmd.ExecuteNonQuery();
                            cmd.Parameters.Clear();
                            trans.Commit();
                            Label4.Text = "Society Registration cancelled successfully";
                            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "none", "<script> $(window).on('load', function () { $('#RedModal').modal({ backdrop: 'static' });});</script>", false);
                        }
                        else
                        {
                            dr.Close();
                            trans.Rollback();
                            callMsgModal("Cancellation is not possible at this stage. Please contact Adminstrator");
                            data.Visible = false;                          
                        }
                    }
                    else
                    {
                        dr.Close();
                    }
                }
                catch (NpgsqlException ex)
                {
                    trans.Rollback();
                    CreateLogFiles Err = new CreateLogFiles();
                    Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "LinkButton2_Click()" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
                    RecordUserAction("Read", " Read Society comes for cancellation", "F", "NA", 1);
                    Response.Write("<script language='javascript'>alert('" + "Couldn't Cancel the registration. Please try after sometime" + "')</script>");
                }
                finally
                {
                    conn.Close();
                }
            }
            else
            {
                callMsgModal("You can delete registration of society of current year only"); data.Visible = false;
            }
        }

        protected void permission_Click(object sender, EventArgs e)
        {
            Response.Redirect("Dashboard.aspx");
        }
    }
}