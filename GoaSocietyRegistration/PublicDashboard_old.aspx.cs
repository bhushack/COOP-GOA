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

namespace GoaSocietyRegistration
{
    public partial class PublicDashboard : System.Web.UI.Page
    {
        string ipaddress = Utility.getIP();
        public int s_goa, n_goa, nsgh, shgh, nmm, smm, nngo, sngo, nscc, sscc, nother, sother;
        protected void Page_Load(object sender, EventArgs e)
        {
            HttpContext.Current.Response.AddHeader("Cache-Control", "no-cache, no-store, must-revalidate");
            HttpContext.Current.Response.AddHeader("Pragma", "no-cache");
            HttpContext.Current.Response.AddHeader("Expires", "0");
            count_statics();
            chart_card.Visible = true;
            Label5.Text = DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss");
        }

        public void count_statics()
        {
            try
            {
                n_goa = Convert.ToInt32(countSociety(551));
                s_goa = Convert.ToInt32(countSociety(552));
                int total = n_goa + s_goa;
                total_society.Text = total.ToString();
                total_societys.Text = total.ToString();
                total_societyss.Text = total.ToString();

                lbnorthgoa.Text = n_goa.ToString();
                lbsouth.Text = s_goa.ToString();

                procesedlabel.Text = getAllSociety(1);
                rejectedLable.Text = getAllSociety(2);

                nsgh = Convert.ToInt32(getMixedSociety(551, 1));
                labelneshg.Text = nsgh.ToString();
                nmm = Convert.ToInt32(getMixedSociety(551, 2));
                labelnmm.Text = nmm.ToString();
                nscc = Convert.ToInt32(getMixedSociety(551, 3));
                labelnnscc.Text = nscc.ToString();
                nngo = Convert.ToInt32(getMixedSociety(551, 4));
                labelnngo.Text = nngo.ToString();
                nother = Convert.ToInt32(getMixedSociety(551, 5));
                labelnos.Text = nother.ToString();
                int totaln = nsgh + nmm + nscc + nngo + nother;
                label8.Text = totaln.ToString();
                shgh = Convert.ToInt32(getMixedSociety(552, 1));
                labelwshg.Text = shgh.ToString();
                smm = Convert.ToInt32(getMixedSociety(552, 2));
                labelsmm.Text = smm.ToString();
                sscc = Convert.ToInt32(getMixedSociety(552, 3));
                labelsscc.Text = sscc.ToString();
                sngo = Convert.ToInt32(getMixedSociety(552, 4));
                labelsngo.Text = sngo.ToString();
                sother = Convert.ToInt32(getMixedSociety(552, 5));
                labelsos.Text = sother.ToString();
                int totals = shgh + smm + sscc + sngo + sother;
                label9.Text = totals.ToString();
                lbNorthAmt.Text = Sanitize.InputText(totalAmount(551));
                lbSouthAmt.Text = Sanitize.InputText(totalAmount(552));
                int temp = Convert.ToInt32(lbNorthAmt.Text) + Convert.ToInt32(lbSouthAmt.Text);
                lbtotalAmt.Text = Sanitize.InputText(temp.ToString());
            }
            catch (Exception ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "count_statics()" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
            }
        }

        protected string totalAmount(int district)
        {
            string result = null;
            NpgsqlConnection conn = new NpgsqlConnection();
            NpgsqlCommand cmd = new NpgsqlCommand();
            conn.ConnectionString = ConfigurationManager.ConnectionStrings["connect"].ConnectionString;
            cmd.Connection = conn;
            try
            {
                conn.Open();
                string query = "select sum(online_payment_details.total_amt) as total_society from esociety.society,esociety.online_payment_details";
                query = query + " where esociety.society.app_id = online_payment_details.app_id and esociety.society.socdistrict = @district";
                cmd.CommandText = query;
                cmd.Parameters.AddWithValue("@district", district);
                NpgsqlDataReader rd = cmd.ExecuteReader();
                if (rd.Read())
                {
                    if (rd["total_society"].ToString() == null || rd["total_society"].ToString() == "")
                    {
                        result = "0";
                    }
                    else
                    {
                        result = rd["total_society"].ToString();
                    }

                }
                else
                {
                    result = null;
                }
                rd.Close();
            }
            catch (NpgsqlException ex)
            {
                result = null;
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "totalAmount()" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));

                //RecordUserAction("MemberDetails_Page_Load", "Exception at Member Data", "F");
                Response.Write("<script language='javascript'>alert('" + "Exception at Member Data" + "')</script>");
            }
            finally
            {
                conn.Close();
            }
            return result;
        }



        protected string getMixedSociety(int district, int societytype)
        {
            string result = null;
            NpgsqlConnection conn = new NpgsqlConnection();
            NpgsqlCommand cmd = new NpgsqlCommand();
            conn.ConnectionString = ConfigurationManager.ConnectionStrings["connect"].ConnectionString;
            cmd.Connection = conn;
            try
            {
                conn.Open();
                string query = "select count(*) as total_society from esociety.society where socdistrict = @district and soctype= @societytype";
                cmd.CommandText = query;
                cmd.Parameters.AddWithValue("@district", district);
                cmd.Parameters.AddWithValue("@societytype", societytype);
                NpgsqlDataReader rd = cmd.ExecuteReader();
                if (rd.Read())
                {
                    result = rd["total_society"].ToString();
                }
                else
                {
                    result = null;
                }
                rd.Close();
            }
            catch (NpgsqlException ex)
            {
                result = null;
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "getMixedSociety()" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));

                //RecordUserAction("MemberDetails_Page_Load", "Exception at Member Data", "F");
                Response.Write("<script language='javascript'>alert('" + "Exception at Member Data" + "')</script>");
            }
            finally
            {
                conn.Close();
            }

            return result;
        }


        protected string getAllSociety(int flag)
        // flag 1 -->accepted 2--> rejected
        {
            string result = null;
            NpgsqlConnection conn = new NpgsqlConnection();
            NpgsqlCommand cmd = new NpgsqlCommand();
            conn.ConnectionString = ConfigurationManager.ConnectionStrings["connect"].ConnectionString;
            cmd.Connection = conn;
            try
            {
                conn.Open();
                string query = "";
                if (flag == 1)
                {
                    query = "select count(*) as noofsociety from esociety.society,esociety.status_table where esociety.society.app_id = esociety.status_table.app_id";
                    query = query + " and esociety.status_table.status_id >= 10 and(esociety.society.socdistrict = 552 or esociety.society.socdistrict = 551)";
                }
                else if (flag == 2)
                {
                    query = "select count(*) as noofsociety from esociety.society,esociety.status_table where esociety.society.app_id = esociety.status_table.app_id";
                    query = query + " and esociety.status_table.status_id = 9 and(esociety.society.socdistrict = 552 or esociety.society.socdistrict = 551)";
                }

                cmd.CommandText = query;
                NpgsqlDataReader rd = cmd.ExecuteReader();
                if (rd.Read())
                {
                    result = Server.HtmlEncode(rd["noofsociety"].ToString());
                }
                else
                {
                    result = null;
                }
                rd.Close();
            }
            catch (NpgsqlException ex)
            {
                result = null;
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "getAllSociety()" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));

                //RecordUserAction("MemberDetails_Page_Load", "Exception at Member Data", "F");
                Response.Write("<script language='javascript'>alert('" + "Exception at Member Data" + "')</script>");
            }
            finally
            {
                conn.Close();
            }
            return result;
        }

        protected Int64 countSociety(int districtid)
        {
            Int64 count = 0;
            NpgsqlConnection conn = new NpgsqlConnection();
            NpgsqlCommand cmd = new NpgsqlCommand();
            conn.ConnectionString = ConfigurationManager.ConnectionStrings["connect"].ConnectionString;
            cmd.Connection = conn;
            try
            {
                conn.Open();
                string query = "select count(*) as total_society from esociety.society where socdistrict = @district";
                cmd.CommandText = query;
                cmd.Parameters.AddWithValue("@district", districtid);
                NpgsqlDataReader rd = cmd.ExecuteReader();
                if (rd.Read())
                {
                    count = Convert.ToInt64(Server.HtmlEncode(rd["total_society"].ToString()));
                }
                else
                {
                    count = 0;
                }
                rd.Close();
            }
            catch (NpgsqlException ex)
            {
                count = 0;
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "countSociety()" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));

                //RecordUserAction("MemberDetails_Page_Load", "Exception at Member Data", "F");
                Response.Write("<script language='javascript'>alert('" + "Exception at Member Data" + "')</script>");
            }
            finally
            {
                conn.Close();
            }
            return count;
        }

    }
}