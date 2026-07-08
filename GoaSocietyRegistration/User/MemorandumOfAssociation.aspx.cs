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
    public partial class MemorandumOfAssociation : System.Web.UI.Page
    {
               
        string ipaddress = Utility.getIP();
        string macaddress = Utility.GetMACAddress(Utility.getIP());        
      
        Insert ins = new Insert();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Check4Tampering() || Page.RouteData.RouteHandler == null)
            {
                SessionManage session = new SessionManage();
                session.__Abandon(Request, Response);
            }
            else
            {
                if (Session["login_id"] != null)
                {
                    HttpContext.Current.Response.AddHeader("Cache-Control", "no-cache, no-store, must-revalidate");
                    HttpContext.Current.Response.AddHeader("Pragma", "no-cache");
                    HttpContext.Current.Response.AddHeader("Expires", "0");
                    if (!IsPostBack)
                    {
                        loadsocdetails();
                        bindgridview(Session["AppID"].ToString());
                    }
                }
                else
                {
                    // RecordUserAction("DocumentUpload_Page_Load", "Session Tampared", "F");
                    Response.Redirect("~/User/LoginModule.aspx");
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
                            sessHackedCheck = false;
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
        protected void loadsocdetails()
        {
            Society_Details soc = ins.FetchSociety(Session["login_id"].ToString(), 1, 3);

            
            string sname = Server.HtmlEncode(soc.socname);
            lblsocname.Text = "\" " +sname + " \"" ;

            lblsocname1.Text = Server.HtmlEncode(soc.socname);
            lblsocaddr.Text = Server.HtmlEncode(soc.socaddr);
           

           
        }

        protected void bindgridview(string AppID)
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

                string fetch_obj = "SELECT objective,row_id,app_id FROM esociety.society_objectives where app_id=@appid and active='Y' order by row_id ASC";
                cmd.CommandText = fetch_obj;
                cmd.Parameters.AddWithValue("@appid", Convert.ToInt64(AppID));
                using (NpgsqlDataAdapter dr = new NpgsqlDataAdapter(cmd))
                {
                    DataTable dt = new DataTable();
                    dr.Fill(dt);
                    gv_objective.DataSource = dt;
                    gv_objective.DataBind();

                }

                string fetch_mangcomm = "SELECT fname,designtaion,occupation,address FROM esociety.members where app_id=@appid and mangcomm='Yes' order by design ASC";
                cmd.CommandText = fetch_mangcomm;
                cmd.Parameters.AddWithValue("@appid", Convert.ToInt64(AppID));
                using (NpgsqlDataAdapter dr = new NpgsqlDataAdapter(cmd))
                {
                    DataTable dt = new DataTable();
                    dr.Fill(dt);
                    gv_mangcomm.DataSource = dt;
                    gv_mangcomm.DataBind();

                }
                lbldate.Text = DateTime.Now.Date.ToString("dd/MM/yyyy");
                string fetch_members = "SELECT fname,designtaion,occupation,address FROM esociety.members where app_id=@appid order by design ASC";               
                cmd.CommandText = fetch_members;
                cmd.Parameters.AddWithValue("@appid", Convert.ToInt64(AppID));
                using (NpgsqlDataAdapter dr = new NpgsqlDataAdapter(cmd))
                {
                    DataTable dt = new DataTable();
                    dr.Fill(dt);
                    gv_members.DataSource = dt;
                    gv_members.DataBind();

                }

                myTrans.Commit();

            }
            catch (NpgsqlException ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "bindgridview" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
                myTrans.Rollback();
            }
            finally
            {
                conn.Close();
            }
        }



    }
}