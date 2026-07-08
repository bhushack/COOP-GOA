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

namespace GoaSocietyRegistration.Organization
{
    
    public partial class ApproveDSC : System.Web.UI.Page
    {
        Validate val = new Validate();
        string ipaddress = Utility.getIP();
        protected void Page_Load(object sender, EventArgs e)
        {
            HttpContext.Current.Response.AddHeader("Cache-Control", "no-cache, no-store, must-revalidate");
            HttpContext.Current.Response.AddHeader("Pragma", "no-cache");
            HttpContext.Current.Response.AddHeader("Expires", "0");
            if (!IsPostBack)
            {
                dscRegisterbind();
                RegisteredDSCBind();
                rejectedDSC();
                ExpiredDSC();
            }
            TabName.Value = Request.Form[TabName.UniqueID];
        }
        private bool checkroles()
        {
            bool role_hacked = false;
            List<int> AllowedRoles = new List<int> { 1, 2, 3, 4 };
            if (Context.Session != null && !AllowedRoles.Contains(Convert.ToInt32(Session["ROLE"])))
                role_hacked = true;
            return role_hacked;

        }
        protected void permission_Click(object sender, EventArgs e)
        {
            // RecordUserAction("Redirect", "Redirection to Dashboard", "Success", "NA", 1);
            Response.Redirect("Dashboard.aspx");
        }

        protected void rejectedDSC()
        {
            NpgsqlConnection conn = new NpgsqlConnection();
            NpgsqlCommand cmd = new NpgsqlCommand();
            conn.ConnectionString = ConfigurationManager.ConnectionStrings["connect"].ConnectionString;
            cmd.Connection = conn;
            try
            {
                string query = "SELECT dsc_name, dsc_publicketstring, dsc_notafter, dsc_reg_ip, dsc_reg_at, \"DistrictName\",reason_for_reject FROM esociety.digital_sign_officer_verify";
                query = query + " INNER JOIN esociety.mst_district on mst_district.\"DistrictID\" = digital_sign_officer_verify.districtid";
                query = query + " where active = 'N' and status = 3";
                conn.Open();
                cmd.CommandText = query;
                using (NpgsqlDataAdapter dr = new NpgsqlDataAdapter(cmd))
                {
                    DataTable dt = new DataTable();
                    dr.Fill(dt);
                    dscrejected.DataSource = dt;
                    dscrejected.DataBind();
                    Label3.Text = Server.HtmlEncode(dscrejected.Rows.Count.ToString());
                }
            }
            catch (NpgsqlException ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "rejectedDSC()" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath) + " from " + ipaddress);
                //                RecordUserAction("RequestNOCGridView", "Exception at Member Data", "F");
                Response.Write("<script language='javascript'>alert('" + "Loading Error......" + "')</script>");
            }
            finally
            {

                conn.Close();
            }
        }
        protected void ExpiredDSC()
        {
            NpgsqlConnection conn = new NpgsqlConnection();
            NpgsqlCommand cmd = new NpgsqlCommand();
            conn.ConnectionString = ConfigurationManager.ConnectionStrings["connect"].ConnectionString;
            cmd.Connection = conn;
            try
            {
                string query = "SELECT dsc_name, dsc_publicketstring, dsc_notafter, dsc_reg_ip, dsc_reg_at, \"DistrictName\",approved_by_name,approved_at,active,status FROM esociety.digital_sign_officer_verify";
                query = query + " INNER JOIN esociety.mst_district on mst_district.\"DistrictID\" = digital_sign_officer_verify.districtid";
                query = query + " where active = 'D' and status = 2";
                conn.Open();
                cmd.CommandText = query;
                using (NpgsqlDataAdapter dr = new NpgsqlDataAdapter(cmd))
                {
                    DataTable dt = new DataTable();
                    dr.Fill(dt);
                    gvExpiredDSC.DataSource = dt;
                    gvExpiredDSC.DataBind();
                    Label6.Text = Server.HtmlEncode(gvExpiredDSC.Rows.Count.ToString());
                }
            }
            catch (NpgsqlException ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "RegisteredDSCBind()" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath) + " from " + ipaddress);
                //                RecordUserAction("RequestNOCGridView", "Exception at Member Data", "F");
                Response.Write("<script language='javascript'>alert('" + "Loading Error......" + "')</script>");
            }
            finally
            {

                conn.Close();
            }
        }
        protected void RegisteredDSCBind()
        {
            NpgsqlConnection conn = new NpgsqlConnection();
            NpgsqlCommand cmd = new NpgsqlCommand();
            conn.ConnectionString = ConfigurationManager.ConnectionStrings["connect"].ConnectionString;
            cmd.Connection = conn;
            try
            {
                string query = "SELECT dsc_name, dsc_publicketstring, dsc_notafter, dsc_reg_ip, dsc_reg_at, \"DistrictName\",approved_by_name,approved_at,active,status FROM esociety.digital_sign_officer_verify";
                query = query + " INNER JOIN esociety.mst_district on mst_district.\"DistrictID\" = digital_sign_officer_verify.districtid";
                query = query + " where (active = 'Y' or active = 'N') and status = 2";
                conn.Open();
                cmd.CommandText = query;
                using (NpgsqlDataAdapter dr = new NpgsqlDataAdapter(cmd))
                {
                    DataTable dt = new DataTable();
                    dr.Fill(dt);
                    DscApproved.DataSource = dt;
                    DscApproved.DataBind();
                    Label2.Text = Server.HtmlEncode(DscApproved.Rows.Count.ToString());
                }
            }
            catch (NpgsqlException ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "RegisteredDSCBind()" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath) + " from " + ipaddress);
                //                RecordUserAction("RequestNOCGridView", "Exception at Member Data", "F");
                Response.Write("<script language='javascript'>alert('" + "Loading Error......" + "')</script>");
            }
            finally
            {

                conn.Close();
            }
        }
        protected void dscRegisterbind()
        {
            NpgsqlConnection conn = new NpgsqlConnection();
            NpgsqlCommand cmd = new NpgsqlCommand();
            conn.ConnectionString = ConfigurationManager.ConnectionStrings["connect"].ConnectionString;
            cmd.Connection = conn;
            try
            {
                string query = "SELECT dsc_name, dsc_publicketstring, dsc_notafter, dsc_reg_ip, dsc_reg_at, \"DistrictName\" FROM esociety.digital_sign_officer_verify";
                query = query + " INNER JOIN esociety.mst_district on mst_district.\"DistrictID\" = digital_sign_officer_verify.districtid";
                query = query + " where active = 'N' and status = 1";
                conn.Open();
                cmd.CommandText = query;
                using (NpgsqlDataAdapter dr = new NpgsqlDataAdapter(cmd))
                {
                    DataTable dt = new DataTable();
                    dr.Fill(dt);
                    dscRegister.DataSource = dt;
                    dscRegister.DataBind();
                    Label1.Text = Server.HtmlEncode(dscRegister.Rows.Count.ToString());
                }
            }
            catch (NpgsqlException ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "dscRegisterbind()" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath) + " from " + ipaddress);
                //                RecordUserAction("RequestNOCGridView", "Exception at Member Data", "F");
                Response.Write("<script language='javascript'>alert('" + "Loading Error......" + "')</script>");
            }
            finally
            {

                conn.Close();
            }
        }
        public void msgModal(string msg)
        {
            Label50.Text = msg;
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "none", "<script> $(window).on('load', function () { $('#msgmodal').modal({ backdrop: 'static' });});</script>", false);

        }
        protected void lkaccept_Click(object sender, EventArgs e)
        {
            try
            {
                LinkButton btn = (LinkButton)sender;
                GridViewRow row = (GridViewRow)btn.NamingContainer;
                string key = ((Label)dscRegister.Rows[row.RowIndex].FindControl("dsckey")).Text;
                NpgsqlConnection conn = new NpgsqlConnection();
                NpgsqlCommand cmd = new NpgsqlCommand();
                conn.ConnectionString = ConfigurationManager.ConnectionStrings["connect"].ConnectionString;
                cmd.Connection = conn;
                try
                {
                    conn.Open();
                    string query = "UPDATE esociety.digital_sign_officer_verify SET active = 'Y', status = 2, approved_by_name = @approved_by_name, approved_at = CURRENT_TIMESTAMP,";
                    query = query + " approved_from = @approved_from, approved_by_email = @approved_by_email, reason_for_reject = 'Approved' WHERE dsc_publicketstring = @dsc_publicketstring";
                    query = query + " and active = 'N' and status = 1";
                    cmd.CommandText = query;
                    cmd.Parameters.AddWithValue("@approved_by_name", Session["logginname"].ToString());
                    cmd.Parameters.AddWithValue("@approved_from", ipaddress);
                    cmd.Parameters.AddWithValue("@approved_by_email", Session["firstname"].ToString());
                    cmd.Parameters.AddWithValue("@dsc_publicketstring", key);
                    cmd.ExecuteNonQuery();
                    msgModal("DSC Approved successfully");
                    dscRegisterbind();
                    RegisteredDSCBind();
                    rejectedDSC();
                    ExpiredDSC();
                }
                catch (NpgsqlException ex)
                {
                    CreateLogFiles Err = new CreateLogFiles();
                    Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "lkaccept_Click-npgsql" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath) + " from " + ipaddress);
                }
                finally
                {
                    conn.Close();
                }
            }
            catch (Exception ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "lkaccept_Click" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath) + " from " + ipaddress);

            }
        }

        protected void lkreject_Click(object sender, EventArgs e)
        {
            try
            {
                hfkey.Value = "";
                LinkButton btn = (LinkButton)sender;
                GridViewRow row = (GridViewRow)btn.NamingContainer;
                string key = ((Label)dscRegister.Rows[row.RowIndex].FindControl("dsckey")).Text;
                hfkey.Value = key;
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "none", "<script> $(window).on('load', function () { $('#RejectReason').modal({ backdrop: 'static' });});</script>", false);

            }
            catch (Exception ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "lkreject_Click" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath) + " from " + ipaddress);

            }

        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            if (Txtbxremarks.Text == "" || Txtbxremarks.Text == null)
            {
                Label5.Text = "Remarks are blank";
                Label5.Visible = true;
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "none", "<script> $(window).on('load', function () { $('#dscRegister').modal({ backdrop: 'static' });});</script>", false);
                Txtbxremarks.Focus();
            }
            else if (!val.validateData(Txtbxremarks.Text, val.reamrks_validation))
            {
                Label5.Text = "Remarks are invalid / special characters are present";
                Label5.Visible = true;
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "none", "<script> $(window).on('load', function () { $('#dscRegister').modal({ backdrop: 'static' });});</script>", false);
                Txtbxremarks.Focus();
            }
            else
            {
                NpgsqlConnection conn = new NpgsqlConnection();
                NpgsqlCommand cmd = new NpgsqlCommand();
                conn.ConnectionString = ConfigurationManager.ConnectionStrings["connect"].ConnectionString;
                cmd.Connection = conn;
                try
                {
                    conn.Open();
                    string query = "UPDATE esociety.digital_sign_officer_verify SET active = 'N', status = 3, approved_by_name = @approved_by_name, approved_at = CURRENT_TIMESTAMP,";
                    query = query + " approved_from = @approved_from, approved_by_email = @approved_by_email, reason_for_reject = @reason_for_reject WHERE dsc_publicketstring = @dsc_publicketstring";
                    query = query + " and  status = 1";
                    cmd.CommandText = query;
                    cmd.Parameters.AddWithValue("@approved_by_name", Session["logginname"].ToString());
                    cmd.Parameters.AddWithValue("@approved_from", ipaddress);
                    cmd.Parameters.AddWithValue("@approved_by_email", Session["firstname"].ToString());
                    cmd.Parameters.AddWithValue("@dsc_publicketstring", hfkey.Value);
                    cmd.Parameters.AddWithValue("@reason_for_reject", Txtbxremarks.Text);
                    cmd.ExecuteNonQuery();
                    msgModal("DSC Rejected and reason saved successfully");
                    dscRegisterbind();
                    RegisteredDSCBind();
                    rejectedDSC();
                    ExpiredDSC();
                }
                catch (NpgsqlException ex)
                {

                    CreateLogFiles Err = new CreateLogFiles();
                    Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "lkreject_Click" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath) + " from " + ipaddress);

                }
                finally
                {
                    conn.Close();
                }
            }
        }

        protected void lkactivate_Click(object sender, EventArgs e)
        {
            try
            {
                LinkButton btn = (LinkButton)sender;
                GridViewRow row = (GridViewRow)btn.NamingContainer;
                string key = ((Label)DscApproved.Rows[row.RowIndex].FindControl("dsckeyapproved")).Text;
                NpgsqlConnection conn = new NpgsqlConnection();
                NpgsqlCommand cmd = new NpgsqlCommand();
                conn.ConnectionString = ConfigurationManager.ConnectionStrings["connect"].ConnectionString;
                cmd.Connection = conn;
                conn.Open();
                NpgsqlTransaction tran = conn.BeginTransaction();
                try
                {
                    cmd.Parameters.Clear();
                    string hist_query = "INSERT INTO esociety.history_dsc_store(dsc_public_key, activate_at, ipaddress, done_by_name, done_by_email)";
                    hist_query = hist_query + " VALUES(@dsc_public_key, CURRENT_TIMESTAMP, @ipaddress, @done_by_name, @done_by_email)";
                    cmd.CommandText = hist_query;
                    cmd.Parameters.AddWithValue("@dsc_public_key", key);
                    cmd.Parameters.AddWithValue("@ipaddress", ipaddress);
                    cmd.Parameters.AddWithValue("@done_by_name", Session["logginname"].ToString());
                    cmd.Parameters.AddWithValue("@done_by_email", Session["firstname"].ToString());
                    cmd.ExecuteNonQuery();
                    cmd.Parameters.Clear();
                    string query = "UPDATE esociety.digital_sign_officer_verify SET active = 'Y' WHERE dsc_publicketstring = @dsc_publicketstring and active = 'N'";
                    cmd.CommandText = query;
                    cmd.Parameters.AddWithValue("@dsc_publicketstring", key);
                    cmd.ExecuteNonQuery(); tran.Commit();
                    msgModal("DSC Activated successfully");
                    dscRegisterbind();
                    RegisteredDSCBind();
                    rejectedDSC();
                    ExpiredDSC();
                }
                catch (NpgsqlException ex)
                {
                    tran.Rollback();
                    CreateLogFiles Err = new CreateLogFiles();
                    Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "lkactivate_Click----pgsql" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath) + " from " + ipaddress);
                }
                finally
                {
                    conn.Close();
                }
            }
            catch (Exception ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "lkactivate_Click" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath) + " from " + ipaddress);

            }
        }

        protected void lkdeactivate_Click(object sender, EventArgs e)
        {
            try
            {
                LinkButton btn = (LinkButton)sender;
                GridViewRow row = (GridViewRow)btn.NamingContainer;
                string key = ((Label)DscApproved.Rows[row.RowIndex].FindControl("dsckeyapproved")).Text;
                NpgsqlConnection conn = new NpgsqlConnection();
                NpgsqlCommand cmd = new NpgsqlCommand();
                conn.ConnectionString = ConfigurationManager.ConnectionStrings["connect"].ConnectionString;
                cmd.Connection = conn;
                conn.Open();
                NpgsqlTransaction tran = conn.BeginTransaction();
                try
                {
                    cmd.Parameters.Clear();
                    string hist_query = "INSERT INTO esociety.history_dsc_store(dsc_public_key, deatcivated_at, ipaddress, done_by_name, done_by_email)";
                    hist_query = hist_query + " VALUES(@dsc_public_key, CURRENT_TIMESTAMP, @ipaddress, @done_by_name, @done_by_email)";
                    cmd.CommandText = hist_query;
                    cmd.Parameters.AddWithValue("@dsc_public_key", key);
                    cmd.Parameters.AddWithValue("@ipaddress", ipaddress);
                    cmd.Parameters.AddWithValue("@done_by_name", Session["logginname"].ToString());
                    cmd.Parameters.AddWithValue("@done_by_email", Session["firstname"].ToString());
                    cmd.ExecuteNonQuery();
                    cmd.Parameters.Clear();
                    string query = "UPDATE esociety.digital_sign_officer_verify SET active = 'N' WHERE dsc_publicketstring = @dsc_publicketstring and active = 'Y'";
                    cmd.CommandText = query;
                    cmd.Parameters.AddWithValue("@dsc_publicketstring", key);
                    cmd.ExecuteNonQuery();
                    tran.Commit();
                    msgModal("DSC Deactivated successfully");
                    dscRegisterbind();
                    RegisteredDSCBind();
                    rejectedDSC();
                    ExpiredDSC();
                }
                catch (NpgsqlException ex)
                {
                    tran.Rollback();
                    CreateLogFiles Err = new CreateLogFiles();
                    Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "lkdeactivate_Click----pgsql" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath) + " from " + ipaddress);
                }
                finally
                {
                    conn.Close();
                }
            }
            catch (Exception ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "lkdeactivate_Click" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath) + " from " + ipaddress);

            }
        }

        protected void DscApproved_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                foreach (GridViewRow row in DscApproved.Rows)
                {
                    string active = ((Label)DscApproved.Rows[row.RowIndex].FindControl("active")).Text;
                    string status = ((Label)DscApproved.Rows[row.RowIndex].FindControl("status")).Text;
                    LinkButton activate = (LinkButton)row.FindControl("lkactivate");
                    LinkButton deactivate = (LinkButton)row.FindControl("lkdeactivate");
                    if (status == "2" && active == "Y")
                    {
                        deactivate.Visible = true;
                    }
                    else if (status == "2" && active == "N")
                    {
                        activate.Visible = true;
                    }
                    else
                    {
                        activate.Visible = false;
                        deactivate.Visible = false;
                    }
                }
            }
            catch (Exception ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "DscApproved_RowDataBound" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath) + " from " + ipaddress);
            }
        }
    }
}