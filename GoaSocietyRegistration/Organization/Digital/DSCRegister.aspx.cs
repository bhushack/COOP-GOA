using GoaSocietyRegistration.Development;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace GoaSocietyRegistration.Organization.Digital
{
    public partial class DSCRegister : System.Web.UI.Page
    {
        string ipaddress = Utility.getIP();
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                string dist = "";
                if (Session["DistrictID"].ToString() == "551")
                {
                    dist = "North Goa";
                }
                else
                {
                    dist = "South Goa";
                }
                TextBox2.Text = "District - Registrar " + dist;
                Session["card_data"] = 1;
                TextBox1.Text = Session["userfirstname"].ToString() + " " + Session["usermiddlename"].ToString() + " " + Session["userlastname"].ToString();
                Session["docname"] = Session["userfirstname"].ToString();
                DataTable dt = new DataTable();
                dt.Columns.Add("Id");
                dt.Columns.Add("Name");
                for (int i = 1; i <= 1; i++)
                {
                    DataRow dr = dt.NewRow();
                    dr["Id"] = "AX-RF-" + i;
                    dr["Name"] = "I ACCEPT THAT DSC TOKEN CONNECTED BELONGS TO ME.";
                    dt.Rows.Add(dr);
                }

                Repeater1.DataSource = dt;
                Repeater1.DataBind();
            }
            catch (Exception ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "Page_Load" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath) + " from " + ipaddress);
            }
        }

        protected void LinkButton1_Click(object sender, EventArgs e)
        { 
            try
            {
                int value = Utility.logout(Convert.ToInt64(Session["SessionID"]));
                if (value == 1)
                {
                    Session.Clear();
                    Session.Abandon();
                    Session.RemoveAll();
                    Response.Cookies.Clear();
                    Response.Redirect("~/OrganizationLogin.aspx");
                }
                else
                {
                    Response.Write("<script language='javascript'>alert(' Error While logout....Try after sometime ')</script>");
                    Response.Redirect("Admin/AdminDashboard.aspx");
                }
            }
            catch (Exception ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(HttpContext.Current.Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "");
            }
        }
    }
}