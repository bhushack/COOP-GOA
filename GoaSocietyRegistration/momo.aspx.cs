using GoaSocietyRegistration.Development;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace GoaSocietyRegistration
{
    public partial class momo : System.Web.UI.Page
    {
        string ipaddress = Utility.getIP();
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                loadMOMO();
            }
        }
        public void loadMOMO()
        {
            NpgsqlConnection conn = new NpgsqlConnection();
            NpgsqlCommand cmd = new NpgsqlCommand();
            conn.ConnectionString = ConfigurationManager.ConnectionStrings["connect"].ConnectionString;
            cmd.Connection = conn;
            try
            {
                string query = "SELECT applicant_details.login_id , socname, applicant_name, applicant_mobile_no, applicant_email,applicant_address,socaddr,\"DistrictName\",socregid, Date(application_submission_time) as application_submission_time,";
                query = query + " status_name FROM  esociety.applicant_details";
                query = query + " inner join esociety.society on society.app_id = applicant_details.app_id";
                query = query + " inner join esociety.status_table on status_table.app_id = applicant_details.app_id";
                query = query + " inner join esociety.mst_district on mst_district.\"DistrictID\" = society.socdistrict";
                query = query + " inner join esociety.mst_status on mst_status.status_id = status_table.status_id";
                query = query + " where application_submission_time between '2024-02-01' and '2025-02-14' and";
                query = query + " (status_table.status_id = 3  or status_table.status_id = 6 or status_table.status_id = 8 or status_table.status_id = 10  or status_table.status_id = 11 or status_table.status_id = 12)";
                    query = query + " order by application_submission_time";
               
                conn.Open();
                cmd.CommandText = query;
              //cmd.Parameters.AddWithValue("@district", Convert.ToInt32(Session["DistrictID"].ToString()));
                NpgsqlDataReader dr = cmd.ExecuteReader();
                vegmomo.DataSource = dr;
                vegmomo.DataBind();
                dr.Close();
               
            }
            catch (NpgsqlException ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "loadnewappln()" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
                Response.Write("<script language='javascript'>alert('" + "Exception at Certified Copy List" + "')</script>");
            }
            finally
            {
                conn.Close();
            }
        }

        protected void vegmomo_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType.Equals(DataControlRowType.DataRow))
                {
                    string y = e.Row.Cells[3].Text;
                    e.Row.Cells[3].Text = Encryption.Encrypt.Decrypt(y);
                    string x = e.Row.Cells[4].Text;
                    e.Row.Cells[4].Text = Encryption.Encrypt.Decrypt(x);
                }
            }
            catch(Exception ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "vegmomo_RowDataBound()" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));

            }
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            try
            {
                Response.Clear();
                Response.Buffer = true;
                Response.AddHeader("content-disposition", "attachment;filename=GridViewExportExcel.xls");
                Response.Charset = "";
                Response.ContentType = "application/vnd.ms-excel";
                using (StringWriter sw = new StringWriter())
                {
                    HtmlTextWriter hw = new HtmlTextWriter(sw);

                    //To Export all pages
                    vegmomo.AllowPaging = false;
                    this.loadMOMO();

                    vegmomo.HeaderRow.BackColor = Color.White;
                    foreach (TableCell cell in vegmomo.HeaderRow.Cells)
                    {
                        cell.BackColor = vegmomo.HeaderStyle.BackColor;
                    }
                    foreach (GridViewRow row in vegmomo.Rows)
                    {
                        row.BackColor = Color.White;
                        foreach (TableCell cell in row.Cells)
                        {
                            if (row.RowIndex % 2 == 0)
                            {
                                cell.BackColor = vegmomo.AlternatingRowStyle.BackColor;
                            }
                            else
                            {
                                cell.BackColor = vegmomo.RowStyle.BackColor;
                            }
                            cell.CssClass = "textmode";
                        }
                    }

                    vegmomo.RenderControl(hw);

                    //style to format numbers to string
                    string style = @"<style> .textmode { } </style>";
                    Response.Write(style);
                    Response.Output.Write(sw.ToString());
                    Response.Flush();
                    Response.End();

                }
            }
            catch (Exception ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "LkExporttoExcel_Click" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath) + " from " + ipaddress);
            }
            finally
            {
               // Response.Redirect("AuditLog.aspx");
            }
        }
    }
}