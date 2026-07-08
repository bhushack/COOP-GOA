using GoaSocietyRegistration.Development;
using Npgsql;
using System;
using System.Configuration;
using System.Data;
using System.IO;
using System.Web;
using System.Web.UI;

namespace GoaSocietyRegistration.User
{
    public partial class Schedule2 : System.Web.UI.Page
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
                HttpContext.Current.Response.AddHeader("Cache-Control", "no-cache, no-store, must-revalidate");
                HttpContext.Current.Response.AddHeader("Pragma", "no-cache");
                HttpContext.Current.Response.AddHeader("Expires", "0");
                if (!IsPostBack)
                {
                
                    loadsocdetails();
                    loadGridview(Convert.ToInt64(Session["AppID"].ToString()));
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
            Society_Details soc = ins.FetchSociety(Session["AppID"].ToString(), 3, 3);

            lblsocname.Text = Server.HtmlEncode(soc.socname) + " , " + Server.HtmlEncode(soc.socaddr);

            lblregno.Text = Session["OldRegNo"].ToString();


            NpgsqlConnection conn = new NpgsqlConnection();
            NpgsqlCommand cmd = new NpgsqlCommand();
            conn.ConnectionString = ConfigurationManager.ConnectionStrings["connect"].ConnectionString;
            cmd.Connection = conn;

            try
            {
                conn.Open();
                string query = "select applicant_name, applicant_mobile_no, applicant_address, mst_memberdesignation.\"DesignationName\",old_socregid from esociety.applicant_details,";
                query = query + " esociety.mst_memberdesignation where mst_memberdesignation.\"DesignationID\" = applicant_details.applicant_designation and app_id = @app_id";

                cmd.CommandText = query;
                cmd.Parameters.AddWithValue("@app_id", Convert.ToInt64(Session["AppID"].ToString()));
                NpgsqlDataReader rd = cmd.ExecuteReader();
                if (rd.Read())
                {

                    lblappname.Text = "(" + Server.HtmlEncode(rd["applicant_name"].ToString()) + ")";

                    lbladdress.Text = Server.HtmlEncode(rd["applicant_address"].ToString());

                }
                rd.Close();
                cmd.Parameters.Clear();
            }
            catch (NpgsqlException ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "loadsocdetails" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));

            }
            finally
            {
                conn.Close();
            }

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

                    
                }
              

            }
            catch (NpgsqlException ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "loadGridview()" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));                
                Response.Write("<script language='javascript'>alert('" + "Exception in bindgridview_oldmembers" + "')</script>");
            }
            finally
            {
                conn.Close();
            }
        }

    }
}