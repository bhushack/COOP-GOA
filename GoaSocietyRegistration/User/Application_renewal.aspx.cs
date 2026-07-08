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

namespace GoaSocietyRegistration.User
{
    public partial class Application_renewal : System.Web.UI.Page
    {

        System.Globalization.CultureInfo french = new System.Globalization.CultureInfo("fr-FR");
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
                        //string query = "select applicant_name,applicant_mobile_no from esociety.applicant_details where app_id=@app_id";
                        string query = "select applicant_name, applicant_mobile_no, mst_memberdesignation.\"DesignationName\",old_socregid from esociety.applicant_details,";
                        query = query + " esociety.mst_memberdesignation where mst_memberdesignation.\"DesignationID\" = applicant_details.applicant_designation and app_id = @app_id";
                       
                        cmd.CommandText = query;
                        cmd.Parameters.AddWithValue("@app_id", Convert.ToInt64(Session["AppID"].ToString()));
                        NpgsqlDataReader rd = cmd.ExecuteReader();
                        if (rd.Read())
                        {
                            lblrepname.Text = Server.HtmlEncode(rd["applicant_name"].ToString());
                            lbappName.Text = "(" + Server.HtmlEncode(rd["applicant_name"].ToString()) + ")";
                            lbappName1.Text = "(" + Server.HtmlEncode(rd["applicant_name"].ToString()) + ")";
                            lblDesignations.Text = Server.HtmlEncode(rd["DesignationName"].ToString());
                            lblheadname.Text = " "+ Server.HtmlEncode(rd["applicant_name"].ToString()) + " " + lblDesignations.Text + " ";
                            lbldesign.Text = Server.HtmlEncode(rd["DesignationName"].ToString());
                            lblregno.Text = Server.HtmlEncode(rd["old_socregid"].ToString());
                        }
                        rd.Close();
                        cmd.Parameters.Clear();
                        string query1 = "select socname,socdistrict from esociety.society where app_id=@app_id";
                        cmd.CommandText = query1;
                        cmd.Parameters.AddWithValue("@app_id", Convert.ToInt64(Session["AppID"].ToString()));
                        NpgsqlDataReader dr = cmd.ExecuteReader();
                        if (dr.Read())
                        {
                            lblsocname.Text = Server.HtmlEncode(dr["socname"].ToString());
                            lblstyname.Text = Server.HtmlEncode(dr["socname"].ToString());

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

                        string query2 = "select regdate from esociety.society where socregid=@socregid";
                        cmd.CommandText = query2;
                        cmd.Parameters.AddWithValue("@socregid", lblregno.Text);
                        NpgsqlDataReader rd1 = cmd.ExecuteReader();
                        if (rd1.Read())
                        {
                            //lblregdate.Text = Server.HtmlEncode(rd1["regdate"].ToString());
                            lblregdate.Text = (Convert.ToDateTime(rd1["regdate"],french).Date).ToString("dd/MM/yyyy");
                            rd1.Close();
                        }
                        else
                        {
                            rd1.Close();
                            string query3 = "select regdate,datemodified,reg_date from esociety.society_all where socregid=@socregid";
                            cmd.CommandText = query3;
                            cmd.Parameters.AddWithValue("@socregid", lblregno.Text);
                            NpgsqlDataReader rd2 = cmd.ExecuteReader();
                            if (rd2.Read())
                            {
                                if(Convert.ToInt32(rd2["datemodified"])==0)
                                {
                                    lblregdate.Text = (Convert.ToDateTime(rd2["regdate"],french).Date).ToString("dd/MM/yyyy");
                                }
                                else if (Convert.ToInt32(rd2["datemodified"]) == 1)
                                {
                                    lblregdate.Text = (Convert.ToDateTime(rd2["reg_date"],french).Date).ToString("dd/MM/yyyy");
                                }

                                rd2.Close();
                            }
                            else
                            {
                                rd2.Close();
                                string query4 = "select regdate,datemodified,reg_date from esociety.society_all_north where socregid=@socregid";
                                cmd.CommandText = query4;
                                cmd.Parameters.AddWithValue("@socregid", lblregno.Text);
                                NpgsqlDataReader rd3 = cmd.ExecuteReader();
                                if (rd3.Read())
                                {
                                    if (Convert.ToInt32(rd2["datemodified"]) == 0)
                                    {
                                        lblregdate.Text = (Convert.ToDateTime(rd3["regdate"],french).Date).ToString("dd/MM/yyyy");
                                    }
                                    else if (Convert.ToInt32(rd2["datemodified"]) == 1)
                                    {
                                        lblregdate.Text = (Convert.ToDateTime(rd3["reg_date"], french).Date).ToString("dd/MM/yyyy");
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
                    Response.Redirect("~/User/LoginModule.aspx");
                }

                lbldate.Text = DateTime.Today.ToString("dd/MM/yyyy");
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

      

       
    }
}