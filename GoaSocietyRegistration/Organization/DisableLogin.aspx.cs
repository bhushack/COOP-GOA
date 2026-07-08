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
    public partial class DisableLogin : System.Web.UI.Page
    {
        string ipaddress = Utility.getIP();
        string macaddress = Utility.GetMACAddress(Utility.getIP());
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Check4Tampering() || Page.RouteData.RouteHandler == null)
            {
                // RecordUserAction("Page_Load", "Access request failed. Tampered session", "F");
                SessionManage session = new SessionManage();
                session.__AbandonAdmin(Request, Response);
            }
            else
            {
                HttpContext.Current.Response.AddHeader("Cache-Control", "no-cache, no-store, must-revalidate");
                HttpContext.Current.Response.AddHeader("Pragma", "no-cache");
                HttpContext.Current.Response.AddHeader("Expires", "0");

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
        protected void Lkgetinfo_Click(object sender, EventArgs e)
        {
            NpgsqlConnection conn = new NpgsqlConnection();
            NpgsqlCommand cmd = new NpgsqlCommand();
            conn.ConnectionString = ConfigurationManager.ConnectionStrings["connect"].ConnectionString;
            cmd.Connection = conn;
            try
            {
                Page_status_Check psc = new Page_status_Check();
                Insert ins = new Insert();
                psc = ins.getPageStatus(Utility.getAppID(LkLogin.Text));

                LkLogin.Enabled = false;
                int token_district = Utility.get_districtid(Utility.getAppID(LkLogin.Text));
                if (token_district == Convert.ToInt32(Session["DistrictID"].ToString()))
                {
                    if (psc.status_id == 1 || psc.status_id == 2 || psc.status_id == 3 || psc.status_id == 4 || psc.status_id == 5 || psc.status_id == 6 || psc.status_id == 7)
                    {
                        conn.Open();
                        string query = "SELECT socname, socaddr, societytype, applicant_name FROM esociety.society inner join esociety.mst_societytype on mst_societytype.societyid = society.soctype";
                        query = query + " inner join esociety.applicant_details on applicant_details.login_id = society.login_id where society.login_id = @login_id";
                        cmd.CommandText = query;
                        cmd.Parameters.AddWithValue("@login_id", LkLogin.Text);
                        NpgsqlDataReader rd = cmd.ExecuteReader();
                        if (rd.Read())
                        {
                            data.Visible = true;
                            Label1.Text = Sanitize.InputText(rd["applicant_name"].ToString());
                            Label2.Text = Sanitize.InputText(rd["socname"].ToString());
                            Label3.Text = Sanitize.InputText(rd["socaddr"].ToString());
                            Label4.Text = Sanitize.InputText(rd["societytype"].ToString());
                        }
                        else
                        {
                            LkLogin.Enabled = false;
                            data.Visible = false;
                            Response.Write("<script language=javascript>alert('No Data found with Login ID');</script>");
                        }
                        rd.Close();
                    }
                    else
                    {
                        LkLogin.Enabled = false;
                        data.Visible = false;
                        Response.Write("<script language=javascript>alert('Cannot be disabled now.');</script>");
                    }
                }
                else
                {
                    LkLogin.Enabled = false;
                    data.Visible = false;
                    Response.Write("<script language=javascript>alert('Login ID of Other District');</script>");
                }
            }
            catch (NpgsqlException ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "Lkgetinfo_Click()" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
                Response.Write("<script language='javascript'>alert('" + "Exception at Certified Copy List" + "')</script>");
            }
            finally
            {
                conn.Close();
            }
        }      
        protected void lkdisable_Click(object sender, EventArgs e)
        {
            NpgsqlConnection conn = new NpgsqlConnection();
            NpgsqlCommand cmd = new NpgsqlCommand();
            conn.ConnectionString = ConfigurationManager.ConnectionStrings["connect"].ConnectionString;
            cmd.Connection = conn;
            conn.Open();
            NpgsqlTransaction tran = conn.BeginTransaction();
            try
            {
                string socname = "";
                cmd.Parameters.Clear();
                string query = "UPDATE esociety.applicant_details SET active = 'N' WHERE login_id = @login_id";
                cmd.CommandText = query;
                cmd.Parameters.AddWithValue("@login_id", LkLogin.Text);
                cmd.ExecuteNonQuery();
                cmd.Parameters.Clear();
                string dis_user = "UPDATE esociety.usertable SET active = 'N' WHERE user_loginname = @user_loginname";
                cmd.CommandText = dis_user;
                cmd.Parameters.AddWithValue("@user_loginname", LkLogin.Text);
                cmd.ExecuteNonQuery();
                cmd.Parameters.Clear();
                string up_status = "UPDATE esociety.status_table SET status_id = 13 WHERE login_id = @login_id";
                cmd.CommandText = up_status;
                cmd.Parameters.AddWithValue("@login_id", LkLogin.Text);
                cmd.ExecuteNonQuery();
                cmd.Parameters.Clear();
                string upd_soc = "SELECT socname FROM esociety.society where active = 'Y' and login_id = @login_id";
                cmd.CommandText = upd_soc;
                cmd.Parameters.AddWithValue("@login_id", LkLogin.Text);
                NpgsqlDataReader rd = cmd.ExecuteReader();
                if (rd.Read())
                {
                    socname = rd["socname"].ToString();
                    rd.Close();
                    cmd.Parameters.Clear();
                    string upd_query = "UPDATE esociety.society SET socname = @socname, active = 'N' WHERE active = 'Y' and login_id = @login_id";
                    cmd.CommandText = upd_query;
                    socname = socname + " Disabled on : " + DateTime.Now.ToString();
                    cmd.Parameters.AddWithValue("@socname", socname);
                    cmd.Parameters.AddWithValue("@login_id", LkLogin.Text);
                    cmd.ExecuteNonQuery();
                    tran.Commit();
                    data.Visible = false;
                    LkLogin.Text = "";
                    LkLogin.Enabled = false;
                    Label5.ForeColor = System.Drawing.Color.Green;
                    Label5.Text = "Login disabled successfully";
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "none", "<script> $(window).on('load', function () { $('#permission_error_modal').modal({ backdrop: 'static' });});</script>", false);

                }
                else
                {
                    tran.Rollback();
                }
                rd.Close();

            }
            catch (NpgsqlException ex)
            {
                tran.Rollback();
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "lkdisable_Click()" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
                Response.Write("<script language='javascript'>alert('" + "Exception at Certified Copy List" + "')</script>");
            }
            finally
            {
                conn.Close();
            }
        }
    }
}