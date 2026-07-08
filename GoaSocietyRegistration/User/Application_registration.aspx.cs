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
using WS_Encryption;

namespace GoaSocietyRegistration
{
    public partial class Application_registration : System.Web.UI.Page
    {
        //static string appid;
        NICEncryption _encryption = new NICEncryption();
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Check4Tampering() || Page.RouteData.RouteHandler == null)
            {
                SessionManage session = new SessionManage();
                session.__Abandon(Request, Response);
            }
            else
            {
                lbDate.Text = DateTime.Today.ToString("dd/MM/yyyy");
                if (Session["login_id"] != null)
                {
                    HttpContext.Current.Response.AddHeader("Cache-Control", "no-cache, no-store, must-revalidate");
                    HttpContext.Current.Response.AddHeader("Pragma", "no-cache");
                    HttpContext.Current.Response.AddHeader("Expires", "0");
                    string loginid = Session["login_id"].ToString();
                    //appid = Session["AppID"].ToString();
                    NpgsqlConnection conn = new NpgsqlConnection();
                    NpgsqlCommand cmd = new NpgsqlCommand();
                    conn.ConnectionString = ConfigurationManager.ConnectionStrings["connect"].ConnectionString;
                    cmd.Connection = conn;
                    conn.Open();
                    NpgsqlTransaction myTrans = conn.BeginTransaction();
                    cmd.Transaction = myTrans;

                    try
                    {
                        // string query = "select applicant_name,applicant_mobile_no,applicant_designation from esociety.applicant_details where app_id=@app_id";

                        string query = "select applicant_name, applicant_mobile_no, mst_memberdesignation.\"DesignationName\" from esociety.applicant_details,";
                        query = query + " esociety.mst_memberdesignation where mst_memberdesignation.\"DesignationID\" = applicant_details.applicant_designation and app_id = @app_id";


                        cmd.CommandText = query;
                        cmd.Parameters.AddWithValue("@app_id", Convert.ToInt64(Session["AppID"].ToString()));
                        NpgsqlDataReader rd = cmd.ExecuteReader();
                        if (rd.Read())
                        {                           
                            lbname.Text = Server.HtmlEncode(rd["applicant_name"].ToString());
                            lbappName.Text = "(" + Server.HtmlEncode(rd["applicant_name"].ToString()) + ")";
                            lbphone.Text = Encryption.Encrypt.Decrypt(Server.HtmlEncode(rd["applicant_mobile_no"].ToString()));

                            lbDesignation.Text = Server.HtmlEncode(rd["DesignationName"].ToString());
                            lbDesignations.Text = Server.HtmlEncode(rd["DesignationName"].ToString());


                        }
                        
                        rd.Close();
                        cmd.Parameters.Clear();
                        string query1 = "select socname,socaddr,socdistrict from esociety.society where app_id=@app_id";
                        cmd.CommandText = query1;
                        cmd.Parameters.AddWithValue("@app_id", Convert.ToInt64(Session["AppID"].ToString()));
                        NpgsqlDataReader dr = cmd.ExecuteReader();
                        if (dr.Read())
                        {
                            lbsocname.Text = Server.HtmlEncode(dr["socname"].ToString());
                            lbsonamee.Text = Server.HtmlEncode(dr["socname"].ToString());
                            lbsocAddress.Text = Server.HtmlEncode(dr["socaddr"].ToString());
                            string district = Server.HtmlEncode(dr["socdistrict"].ToString());
                            if (district == "551")
                            {
                                lbPlace.Text = "Panaji - ";
                                lbDistrict.Text = "North Goa";
                            }
                            else if (district == "552")
                            {
                                lbPlace.Text = "Margao - ";
                                lbDistrict.Text = "South Goa";
                            }
                            else
                            {
                                lbDistrict.Text = "";
                            }
                        }
                        dr.Close();
                        myTrans.Commit();

                    }
                    catch (NpgsqlException ex)
                    {
                        CreateLogFiles Err = new CreateLogFiles();
                        Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "Page_Load" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
                        myTrans.Rollback();
                    }
                    finally
                    {
                        conn.Close();
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
        protected void LinkButton2_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/User/DocumentUpload.aspx");
        }
    }
}