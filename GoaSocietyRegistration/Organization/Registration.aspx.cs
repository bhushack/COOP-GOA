using GoaSocietyRegistration.Development;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
namespace GoaSocietyRegistration
{
    public partial class Registration : System.Web.UI.Page
    {
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
                if (Session["SessionID"] != null || Session["firstname"] != null)
                {

                    HttpContext.Current.Response.AddHeader("Cache-Control", "no-cache, no-store, must-revalidate");
                    HttpContext.Current.Response.AddHeader("Pragma", "no-cache");
                    HttpContext.Current.Response.AddHeader("Expires", "0");
                    NpgsqlConnection conn = new NpgsqlConnection();
                    NpgsqlCommand cmd = new NpgsqlCommand();
                    conn.ConnectionString = ConfigurationManager.ConnectionStrings["connect"].ConnectionString;
                    cmd.Connection = conn;
                    conn.Open();
                    string app_id = Session["App_ID"].ToString();
                    NpgsqlTransaction myTrans = conn.BeginTransaction();
                    cmd.Transaction = myTrans;
                    try
                    {
                        string query = "select userfirstname,userlastname,user_designation,address,office_tel_no,officename,\"DistrictName\" from esociety.admin_table,esociety.mst_district where admin_table.district_id=mst_district.\"DistrictID\" and username=@username";
                        cmd.CommandText = query;
                        cmd.Parameters.AddWithValue("@username", Server.HtmlEncode(Session["LoginName"].ToString()));
                        NpgsqlDataReader rd = cmd.ExecuteReader();
                        if (rd.Read())
                        {
                            lbofficeName.Text = Server.HtmlEncode(rd["officename"].ToString());
                            lbofficeaddress.Text = Server.HtmlEncode(rd["address"].ToString());
                            lbOfficeTelNo.Text = Server.HtmlEncode(rd["office_tel_no"].ToString());
                            lbEmail.Text = Server.HtmlEncode(Session["LoginName"].ToString());
                            string firstname = Server.HtmlEncode(rd["userfirstname"].ToString());
                            string lastname = Server.HtmlEncode(rd["userlastname"].ToString());
                            lbRegistrarName.Text = "(" + firstname + " " + lastname + ")";
                            lbRegistrarName.Text = lbRegistrarName.Text.ToUpper();
                            lbDesignation.Text = Server.HtmlEncode(rd["user_designation"].ToString()) + " (" + Server.HtmlEncode(rd["DistrictName"].ToString()) + ")";
                        }
                        rd.Close();
                        cmd.Parameters.Clear();
                        string query1 = "select socname,socaddr,\"TalukaName\",totalfee,socregid,regdate from esociety.society,esociety.mst_taluka where app_id=@app_id and society.soc_taluka=mst_taluka.\"TalukaID\"";
                        //cmd.Parameters.AddWithValue(); Application id
                        cmd.CommandText = query1;
                        cmd.Parameters.AddWithValue("@app_id", Convert.ToInt64(app_id));
                        NpgsqlDataReader dr = cmd.ExecuteReader();
                        if (dr.Read())
                        {
                            string value = "'" + Server.HtmlEncode(dr["socname"].ToString()) + ", " + Server.HtmlEncode(dr["socaddr"].ToString()) + ", " + Server.HtmlEncode(dr["TalukaName"].ToString()) + " Goa";
                            lbSocietyName.Text = value;
                            lbRegistration.Text = Server.HtmlEncode(dr["socregid"].ToString());
                            lbAmountPaid.Text = "Paid ₹ " + Server.HtmlEncode(dr["totalfee"].ToString());
                            //DateTime dt = Convert.ToDateTime(Server.HtmlEncode(dr["regdate"].ToString()));
                            DateTime dt = DateTime.Now;
                            string date = dt.ToString("dd") + " " + dt.ToString("MMMM") + " , " + dt.Year.ToString();
                            Label11.Text = date;
                        }
                        dr.Close();
                        cmd.Parameters.Clear();
                        string query2 = "select total_amt,echallan_no,bank_rcvd_date from esociety.online_payment_details where app_id=@app_id and active='Y' and status='S'";
                        cmd.CommandText = query2;
                        cmd.Parameters.AddWithValue("@app_id", Convert.ToInt64(app_id));
                        NpgsqlDataReader reader = cmd.ExecuteReader();
                        if (reader.Read())
                        {
                            lbAmountPaid.Text = "Paid ₹" + Server.HtmlEncode(reader["total_amt"].ToString()) + "/-";
                            lbeChallanNo.Text = Server.HtmlEncode(reader["echallan_no"].ToString());
                            var dateFormat = Server.HtmlEncode(Convert.ToString(reader["bank_rcvd_date"]));
                           DateTime date = DateTime.Parse(dateFormat);
                            lbeChallanDate.Text = date.ToString().Substring(0, 10);
                        }
                        reader.Close();
                        myTrans.Commit();
                    }
                    catch (NpgsqlException ex)
                    {
                        myTrans.Rollback();
                    }
                    finally
                    {
                        conn.Close();
                    }
                }
                else
                {
                    Response.Redirect("LoginModule.aspx");
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