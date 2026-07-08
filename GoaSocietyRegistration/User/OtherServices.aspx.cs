using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Npgsql;
using System.Configuration;
using GoaSocietyRegistration.Development;
using System.IO;

namespace GoaSocietyRegistration.User
{
    public partial class OtherServices : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            HttpContext.Current.Response.AddHeader("Cache-Control", "no-cache, no-store, must-revalidate");
            HttpContext.Current.Response.AddHeader("Pragma", "no-cache");
            HttpContext.Current.Response.AddHeader("Expires", "0");
            LoadSocietyDetails();
        }

        protected void LkAmendment_Click(object sender, EventArgs e)
        {
            Response.Redirect("Amendment.aspx");
        }

        protected void LkReturnFiling_Click(object sender, EventArgs e)
        {

        }

        protected void LoadSocietyDetails()
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
                
                string query = "select socname, socaddr,mst_district.\"DistrictName\" as socdist from esociety.society join esociety.mst_district on esociety.society.socdistrict = esociety.mst_district.\"DistrictID\" where app_id = @app_id";
                query = query + " ";

                cmd.CommandText = query;
                cmd.Parameters.AddWithValue("@app_id", Convert.ToInt64(Session["AppID"].ToString()));
                NpgsqlDataReader rd = cmd.ExecuteReader();
                if (rd.Read())
                {
                    lblsocname.Text = Server.HtmlEncode(rd["socname"].ToString());
                    lblsocaddr.Text = Server.HtmlEncode(rd["socaddr"].ToString());
                    lblsocdistrict.Text = " " + Server.HtmlEncode(rd["socdist"].ToString());
                  
                }
                rd.Close();
                cmd.Parameters.Clear();

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
    }
}