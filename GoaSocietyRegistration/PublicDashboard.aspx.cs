using GoaSocietyRegistration.Development;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using WS_Encryption;
using Npgsql;
using System;
using System.Data;
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
        System.Globalization.CultureInfo french = new System.Globalization.CultureInfo("fr-FR");

        protected void lkshow_Click(object sender, EventArgs e)
        {
            gridviewlist.Visible = true;
            brap_views();
        }

        protected void ddlFinancialYear_SelectedIndexChanged(object sender, EventArgs e)
        {
            labelTotalApplicationsReceived.Text = brapStatic(2, ddlFinancialYear.SelectedItem.Text);
            labelTotalApplicationsApproved.Text = brapStatic(3, ddlFinancialYear.SelectedItem.Text);
            labelAvgTime.Text = brapStatic(4, ddlFinancialYear.SelectedItem.Text);
            labelMedianTime.Text = brapStatic(5, ddlFinancialYear.SelectedItem.Text);
            label3.Text = brapStatic(6, ddlFinancialYear.SelectedItem.Text);
            labelMaxTime.Text = brapStatic(7, ddlFinancialYear.SelectedItem.Text);
            labelAvgFee.Text = brapStatic(8, ddlFinancialYear.SelectedItem.Text);
        }

        protected void brap_view_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            brap_view.PageIndex = e.NewPageIndex;
            this.brap_views();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                PopulateFinancialYears();
            }
            HttpContext.Current.Response.AddHeader("Cache-Control", "no-cache, no-store, must-revalidate");
            HttpContext.Current.Response.AddHeader("Pragma", "no-cache");
            HttpContext.Current.Response.AddHeader("Expires", "0");
            count_statics();
            //brap_views();
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
                //total_society.Text = total.ToString();
                //total_societys.Text = total.ToString();
                //total_societyss.Text = total.ToString();

                //lbnorthgoa.Text = n_goa.ToString();
                //lbsouth.Text = s_goa.ToString();

                //procesedlabel.Text = getAllSociety(1);
               /// rejectedLable.Text = getAllSociety(2);

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
                //lbNorthAmt.Text = Sanitize.InputText(totalAmount(551));
                //lbSouthAmt.Text = Sanitize.InputText(totalAmount(552));
               // int temp = Convert.ToInt32(lbNorthAmt.Text) + Convert.ToInt32(lbSouthAmt.Text);
               // lbtotalAmt.Text = Sanitize.InputText(temp.ToString());

                labelTimeLimit.Text ="7";
                labelTotalApplicationsReceived.Text = brapStatic(2, ddlFinancialYear.SelectedItem.Text);
                labelTotalApplicationsApproved.Text = brapStatic(3, ddlFinancialYear.SelectedItem.Text);
                labelAvgTime.Text = brapStatic(4, ddlFinancialYear.SelectedItem.Text);
                labelMedianTime.Text = brapStatic(5, ddlFinancialYear.SelectedItem.Text);
                label3.Text = brapStatic(6, ddlFinancialYear.SelectedItem.Text);
                labelMaxTime.Text = brapStatic(7, ddlFinancialYear.SelectedItem.Text);
                labelAvgFee.Text = brapStatic(8, ddlFinancialYear.SelectedItem.Text);
            }
            catch (Exception ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "count_statics()" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
                Response.Write("<script language='javascript'>alert('" + "Soemthing went wrong" + "')</script>");
            }
        }

        private void PopulateFinancialYears()
        {
            int currentYear = DateTime.Now.Year;
            int startYear = currentYear; // Starting from 5 years ago, adjust as necessary
            int endYear = currentYear - 5; // Up to 5 years in the future, adjust as necessary

            for (int year = startYear; year >= endYear; year--)
            {
                string financialYear = $"{year}-{(year + 1).ToString()}";
                ddlFinancialYear.Items.Add(new ListItem(financialYear, financialYear));
            }
        }
        protected void brap_views()
        {
            string[] arr = ddlFinancialYear.SelectedItem.Text.Split('-');
            string fyear = arr[0].ToString();
            fyear = fyear + "-04-01";
            string syear = arr[1].ToString();
            syear = syear + "-03-31";
            NpgsqlConnection conn = new NpgsqlConnection();
            NpgsqlCommand cmd = new NpgsqlCommand();
            conn.ConnectionString = ConfigurationManager.ConnectionStrings["connect"].ConnectionString;
            cmd.Connection = conn;
            try
            {
                conn.Open();
                string query1 = "SELECT application_no, application_date, approval_date, fee_details, total_fee  FROM esociety.society_view where (approval_date is not null) and(approval_date between  @date1 and @date2)";
                cmd.CommandText = query1;
                cmd.Parameters.AddWithValue("@date1", Convert.ToDateTime(fyear, french));
                cmd.Parameters.AddWithValue("@date2", Convert.ToDateTime(syear, french));
                NpgsqlDataAdapter da = new NpgsqlDataAdapter(cmd);
                System.Data.DataSet ds = new DataSet();
                da.Fill(ds);
                if (ds.Tables[0].Rows.Count > 0)
                {
                    brap_view.DataSource = ds;
                    brap_view.DataBind();
                }

            }
            catch (NpgsqlException ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "brap_views()" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
                Response.Write("<script language='javascript'>alert('" + "Soemthing went wrong" + "')</script>");
            }
            finally
            {
                conn.Close();
            }
            //NpgsqlConnection conn = new NpgsqlConnection();
            //NpgsqlCommand cmd = new NpgsqlCommand();
            //conn.ConnectionString = ConfigurationManager.ConnectionStrings["connect"].ConnectionString;
            //cmd.Connection = conn;
            //try
            //{
            //    conn.Open();
            //    string query1 = "SELECT application_no, application_date, approval_date, fee_details, total_fee  FROM esociety.society_view ";
            //    cmd.CommandText = query1;
            //  //  cmd.Parameters.AddWithValue("@app_id", Convert.ToInt64(Session["app_id"]));
            //    NpgsqlDataAdapter da = new NpgsqlDataAdapter(cmd);
            //    System.Data.DataSet ds = new DataSet();
            //    da.Fill(ds);
            //    if (ds.Tables[0].Rows.Count > 0)
            //    {
            //        brap_view.DataSource = ds;
            //        brap_view.DataBind();
            //    }
            //}
            //catch (NpgsqlException ex)
            //{
            //    CreateLogFiles Err = new CreateLogFiles();
            //    Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "loademployees()" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
            //    Response.Write("<script language='javascript'>alert('" + "Soemthing went wrong" + "')</script>");
            //}
            //finally
            //{
            //    conn.Close();
            //}
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
                Response.Write("<script language='javascript'>alert('" + "Soemthing went wrong" + "')</script>");
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
                string query = "select count(*) as total_society from esociety.society inner join esociety.status_table on status_table.app_id = society.app_id  where socdistrict = @district and soctype= @societytype and status_id >= 3";
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
                Response.Write("<script language='javascript'>alert('" + "Soemthing went wrong" + "')</script>");
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
                Response.Write("<script language='javascript'>alert('" + "Soemthing went wrong" + "')</script>");
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
                string query = "select count(*) as total_society from esociety.society inner join esociety.status_table on status_table.app_id = society.app_id  where socdistrict = @district and status_id >= 3";
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
                Response.Write("<script language='javascript'>alert('" + "Soemthing went wrong" + "')</script>");
            }
            finally
            {
                conn.Close();
            }
            return count;
        }
        protected string brapStatic(int flag,string financial_year)
        {
            string result = null;
            NpgsqlConnection conn = new NpgsqlConnection();
            NpgsqlCommand cmd = new NpgsqlCommand();
            conn.ConnectionString = ConfigurationManager.ConnectionStrings["connect"].ConnectionString;
            cmd.Connection = conn;
            string[] arr = financial_year.Split('-');
            string fyear = arr[0].ToString();
            fyear = fyear + "-04-01";
            string syear = arr[1].ToString();
            syear = syear + "-03-31";
            try
            {
                conn.Open();
                string query = " ";
               if (flag == 1)
                {
                    query = "";
                }
                else if (flag == 2)
                {
                    //query = "select count(*) from esociety.status_table inner join esociety.remarks_table on remarks_table.app_id = status_table.app_id";
                    //query = query + " where (cast(status_table.status_id as int) >= 3) and (remarks_accepted_submit_time is not null) and (remarks_accepted_submit_time between @date1 and @date2)";
                    query = "select count(*) from esociety.status_table INNER JOIN esociety.society_view on status_table.app_id = society_view.application_no";
                    query = query + " where(cast(status_table.status_id as int) >= 3) and(application_date is not null) and (application_date between @date1 and @date2)";
                }
                    else if (flag == 3)
                {
                    query = "select count(*) from esociety.status_table INNER JOIN esociety.society_view on status_table.app_id = society_view.application_no where status_id in ('8','10','11','12')";
                    query = query + " and(approval_date is not null) and (approval_date between @date1 and @date2)";
                }
                else if (flag == 4)
                {
                    query = "select Round(avg (days_diff))||' days' from esociety.society_view where (approval_date is not null) and(approval_date between  @date1 and @date2)";
                }
                else if (flag == 5)
                {
                    query = "SELECT  percentile_cont(0.5) WITHIN GROUP(ORDER BY days_diff)||' days' as median_Date FROM esociety.society_view where (approval_date is not null) and(approval_date between  @date1 and @date2)";
                }
                else if (flag == 6)
                {
                    query = "select Min(days_diff)||' days'from esociety.society_view where days_diff != 0 and (approval_date is not null) and(approval_date between  @date1 and @date2)";
                }
                else if (flag == 7)
                {
                    query = "select Max(days_diff)||' days' from esociety.society_view where (approval_date is not null) and(approval_date between  @date1 and @date2)";
                }
                else if (flag == 8)
                {
                    query = "SELECT round(avg(fee_details)) FROM esociety.society_view where (approval_date is not null) and(approval_date between  @date1 and @date2)";
                }
                cmd.CommandText = query;
                cmd.Parameters.AddWithValue("@date1", Convert.ToDateTime(fyear, french));
                cmd.Parameters.AddWithValue("@date2", Convert.ToDateTime(syear, french));
                NpgsqlDataReader rd = cmd.ExecuteReader();
                if (rd.Read())
                {
                    if(rd[0].ToString()==""|| rd[0].ToString() == null)
                    {
                        result = "0";
                    }
                    else
                    {
                        result = Server.HtmlEncode(rd[0].ToString());
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
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), $"{ex.Message} {ex.StackTrace} Query: {cmd.CommandText}", "brapStatic()");
                Response.Write("<script language='javascript'>alert('" + "Soemthing went wrong" + "')</script>");
            }
            finally
            {
                conn.Close();
            }
            return result;
        }
    }
}