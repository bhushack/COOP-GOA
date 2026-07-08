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

namespace GoaSocietyRegistration.Organization
{
    
    public partial class Dashboard : System.Web.UI.Page
    {
    /********************************************************************
     * 
     * in applicant_details new_or_renewal = 1 --> New application
     *                      new_or_renewal = 2 --> Renewal application
    ********************************************************************/


        string ipaddress = Utility.getIP();
        Insert ins = new Insert();
        public int s_goa, n_goa,nsgh,shgh, nmm,smm, nngo,sngo, nscc,sscc, nother,sother;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Check4Tampering() || Page.RouteData.RouteHandler == null)
            {
                RecordUserAction("Load", "Tampered Session on Page_Load", "Failed", "NA", 2);
                SessionManage session = new SessionManage();
                session.__AbandonAdmin(Request, Response);
            }
            else
            {
                HttpContext.Current.Response.AddHeader("Cache-Control", "no-cache, no-store, must-revalidate");
                HttpContext.Current.Response.AddHeader("Pragma", "no-cache");
                HttpContext.Current.Response.AddHeader("Expires", "0");
                bool value = Utility.getPasswordReset(Session["firstname"].ToString().Trim());
                if (value)
                {
                    if (!IsPostBack)
                    {

                        Session["common_logout"] = Session["firstname"];


                        if (Convert.ToInt32(Session["ROLE"]) == 3 || Convert.ToInt32(Session["ROLE"]) == 4)
                        {
                            societyApproval();
                            societyApproval_Renewal();
                            societyApprovalObs();
                            societyApprovalObs_Renewal();
                            chart_card.Visible = false;

                            if (Convert.ToInt32(Session["ROLE"]) == 3)
                            {
                                LkAdvSeach1.Visible = true;
                            }
                        }
                        else
                        {
                            upper_row.Visible = false;
                            count_statics();
                            chart_card.Visible = true;
                            hide_dashboard_grid.Visible = false;
                            labelverify.Visible = false;
                        }

                    }

                    Calendar.TodayDayStyle.BackColor = System.Drawing.Color.Orange;
                    Calendar.TodayDayStyle.ForeColor = System.Drawing.Color.Black;
                }
                else
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('As per new Security Guidelines, you are requested to reset the password');window.location ='ChangePassword.aspx';", true);
                }
            }
                 
        }

        protected void LkAdvSeach_Click(object sender, EventArgs e)
        {
            Response.Redirect("PendingApplications.aspx");
        }
            

        public void count_statics()
        {
            try {
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
                lbtotalAmt.Text = temp.ToString();
            }
            catch (Exception ex) {
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
                if(rd.Read())
                {
                    if (rd["total_society"].ToString() == null || rd["total_society"].ToString() =="")
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



        protected string getMixedSociety(int district , int societytype)
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
                if(rd.Read())
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
                    result= Server.HtmlEncode(rd["noofsociety"].ToString());
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
                if(rd.Read())
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
                            //HttpCookie user_cookie = new HttpCookie("DoTAuthTokAdmin");
                            //user_cookie.HttpOnly = true;
                            //HttpContext.Current.Session["DoTAuthTokAdmin"] = Guid.NewGuid().ToString();
                            //user_cookie.Value = HttpContext.Current.Session["DoTAuthTokAdmin"].ToString();
                            //HttpContext.Current.Response.Cookies.Add(user_cookie);
                            //CSRF token
                            string currentPageName = Request.Url.Segments[Request.Url.Segments.Length - 1];
                            string previousPageName = Request.UrlReferrer.Segments[Request.UrlReferrer.Segments.Length - 1];
                            if (currentPageName == previousPageName)
                            {
                                if (HttpContext.Current.Request.HttpMethod == "POST" && (HttpContext.Current.Session["_csrfToken"].Equals(HiddenField1.Value)))
                                {
                                    HttpContext.Current.Session["_csrfToken"] = Guid.NewGuid().ToString();
                                    HiddenField1.Value = HttpContext.Current.Session["_csrfToken"].ToString();
                                    sessHackedCheck = false;
                                }
                                else if (HttpContext.Current.Request.HttpMethod == "GET")
                                {
                                    HttpContext.Current.Session["_csrfToken"] = Guid.NewGuid().ToString();
                                    HiddenField1.Value = HttpContext.Current.Session["_csrfToken"].ToString();
                                    sessHackedCheck = false;
                                }
                                else
                                {

                                    sessHackedCheck = true;
                                }
                            }
                            else
                            {
                                HttpContext.Current.Session["_csrfToken"] = Guid.NewGuid().ToString();
                                HiddenField1.Value = HttpContext.Current.Session["_csrfToken"].ToString();
                                sessHackedCheck = false;
                            }

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
        protected void societyApprovalObs()
        {
            NpgsqlConnection conn = new NpgsqlConnection();
            NpgsqlCommand cmd = new NpgsqlCommand();
            conn.ConnectionString = ConfigurationManager.ConnectionStrings["connect"].ConnectionString;
            cmd.Connection = conn;
            try
            {
                string query = "";
                if (Convert.ToInt32(Session["ROLE"]) == 3)
                {
                    query = "select applicant_details.app_id,applicant_details.applicant_name,mst_memberdesignation.\"DesignationName\",";
                    query = query + " society.socname,mst_societytype.societytype,esociety.status_table.status_id,status_table.application_obs_submission_time from esociety.applicant_details";
                    query = query + " INNER JOIN esociety.society on applicant_details.app_id = society.app_id";
                    query = query + " INNER JOIN esociety.mst_memberdesignation on applicant_details.applicant_designation = mst_memberdesignation.\"DesignationID\"";
                    query = query + " INNER JOIN esociety.mst_societytype on mst_societytype.societyid = society.soctype";
                    query = query + " INNER JOIN esociety.status_table on esociety.status_table.app_id = esociety.applicant_details.app_id";
                    query = query + " INNER JOIN esociety.mst_district on esociety.mst_district.\"DistrictID\" =esociety.society.socdistrict";
                    query = query + " where (esociety.status_table.status_id = 7 or  esociety.status_table.status_id = 6) and society.socdistrict=@district and applicant_details.new_or_renewal = 1 order by status_table.application_obs_submission_time ASC";
                }
                else if (Convert.ToInt32(Session["ROLE"]) == 4)
                {
                    query = "select applicant_details.app_id,applicant_details.applicant_name,mst_memberdesignation.\"DesignationName\",";
                    query = query + " society.socname,mst_societytype.societytype,esociety.status_table.status_id,status_table.application_obs_submission_time from esociety.applicant_details";
                    query = query + " INNER JOIN esociety.society on applicant_details.app_id = society.app_id";
                    query = query + " INNER JOIN esociety.mst_memberdesignation on applicant_details.applicant_designation = mst_memberdesignation.\"DesignationID\"";
                    query = query + " INNER JOIN esociety.mst_societytype on mst_societytype.societyid = society.soctype";
                    query = query + " INNER JOIN esociety.status_table on esociety.status_table.app_id = esociety.applicant_details.app_id";
                    query = query + " INNER JOIN esociety.mst_district on esociety.mst_district.\"DistrictID\" =esociety.society.socdistrict";
                    query = query + " where esociety.status_table.status_id = 6  and society.socdistrict=@district and applicant_details.new_or_renewal = 1 order by status_table.application_obs_submission_time ASC";

                }
                conn.Open();
                cmd.CommandText = query;
                cmd.Parameters.AddWithValue("@district", Convert.ToInt32(Session["DistrictID"].ToString()));
                NpgsqlDataReader dr = cmd.ExecuteReader();
                grvobservation_society.DataSource = dr;
                grvobservation_society.DataBind();
                Lbcount.Text = grvobservation_society.Rows.Count.ToString();
                observationcountlabel.Text = grvobservation_society.Rows.Count.ToString();
                dr.Close();
            }
            catch (NpgsqlException ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "societyApprovalObs()" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
                //RecordUserAction("MemberDetails_Page_Load", "Exception at Member Data", "F");
                Response.Write("<script language='javascript'>alert('" + "Exception while loading Obs Applications" + "')</script>");
            }
            finally
            {
                conn.Close();
            }



        }
        protected void societyApproval()
        {
            NpgsqlConnection conn = new NpgsqlConnection();
            NpgsqlCommand cmd = new NpgsqlCommand();
            conn.ConnectionString = ConfigurationManager.ConnectionStrings["connect"].ConnectionString;
            cmd.Connection = conn;
            try
            {
                string query = "";
                if (Convert.ToInt32(Session["ROLE"]) == 3)
                {
                    query = "select applicant_details.app_id,applicant_details.applicant_name,mst_memberdesignation.\"DesignationName\",";
                    query = query + " society.socname,mst_societytype.societytype,esociety.status_table.status_id,status_table.application_submission_time from esociety.applicant_details";
                    query = query + " INNER JOIN esociety.society on applicant_details.app_id = society.app_id";
                    query = query + " INNER JOIN esociety.mst_memberdesignation on applicant_details.applicant_designation = mst_memberdesignation.\"DesignationID\"";
                    query = query + " INNER JOIN esociety.mst_societytype on mst_societytype.societyid = society.soctype";
                    query = query + " INNER JOIN esociety.status_table on esociety.status_table.app_id = esociety.applicant_details.app_id";
                    query = query + " INNER JOIN esociety.mst_district on esociety.mst_district.\"DistrictID\" =esociety.society.socdistrict";
                    query = query + " where (esociety.status_table.status_id = 3 or esociety.status_table.status_id = 4) and society.socdistrict=@district and applicant_details.new_or_renewal = 1 order by status_table.application_submission_time ASC";
                }
                else if (Convert.ToInt32(Session["ROLE"]) == 4)
                {
                    query = "select applicant_details.app_id,applicant_details.applicant_name,mst_memberdesignation.\"DesignationName\",";
                    query = query + " society.socname,mst_societytype.societytype,esociety.status_table.status_id,status_table.application_submission_time from esociety.applicant_details";
                    query = query + " INNER JOIN esociety.society on applicant_details.app_id = society.app_id";
                    query = query + " INNER JOIN esociety.mst_memberdesignation on applicant_details.applicant_designation = mst_memberdesignation.\"DesignationID\"";
                    query = query + " INNER JOIN esociety.mst_societytype on mst_societytype.societyid = society.soctype";
                    query = query + " INNER JOIN esociety.status_table on esociety.status_table.app_id = esociety.applicant_details.app_id";
                    query = query + " INNER JOIN esociety.mst_district on esociety.mst_district.\"DistrictID\" = esociety.society.socdistrict";
                    query = query + " where esociety.status_table.status_id = 3  and society.socdistrict=@district and applicant_details.new_or_renewal = 1 order by status_table.application_submission_time ASC";

                }
                conn.Open();
                cmd.CommandText = query;
                cmd.Parameters.AddWithValue("@district", Convert.ToInt32(Session["DistrictID"].ToString()));
                NpgsqlDataReader dr = cmd.ExecuteReader();
                grvApplicantDetails.DataSource = dr;
                grvApplicantDetails.DataBind();
                Label3.Text = grvApplicantDetails.Rows.Count.ToString();
                verifycountlabel.Text = grvApplicantDetails.Rows.Count.ToString();
                dr.Close();
            }
            catch (NpgsqlException ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "societyApproval()" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));

                //RecordUserAction("MemberDetails_Page_Load", "Exception at Member Data", "F");
                Response.Write("<script language='javascript'>alert('" + "Exception at Applications" + "')</script>");
            }
            finally
            {
                conn.Close();
            }
        }

        protected void societyApproval_Renewal()
        {
            NpgsqlConnection conn = new NpgsqlConnection();
            NpgsqlCommand cmd = new NpgsqlCommand();
            conn.ConnectionString = ConfigurationManager.ConnectionStrings["connect"].ConnectionString;
            cmd.Connection = conn;
            try
            {
                string query = "";
                if (Convert.ToInt32(Session["ROLE"]) == 3)
                {
                    query = "select applicant_details.app_id,applicant_details.applicant_name,mst_memberdesignation.\"DesignationName\",";
                    query = query + " society.socname,mst_societytype.societytype,esociety.status_table.status_id,status_table.application_submission_time from esociety.applicant_details";
                    query = query + " INNER JOIN esociety.society on applicant_details.app_id = society.app_id";
                    query = query + " INNER JOIN esociety.mst_memberdesignation on applicant_details.applicant_designation = mst_memberdesignation.\"DesignationID\"";
                    query = query + " INNER JOIN esociety.mst_societytype on mst_societytype.societyid = society.soctype";
                    query = query + " INNER JOIN esociety.status_table on esociety.status_table.app_id = esociety.applicant_details.app_id";
                    query = query + " INNER JOIN esociety.mst_district on esociety.mst_district.\"DistrictID\" =esociety.society.socdistrict";
                    query = query + " where (esociety.status_table.status_id = 3 or esociety.status_table.status_id = 4) and society.socdistrict=@district and applicant_details.new_or_renewal = 2 order by status_table.application_submission_time ASC";
                }
                else if (Convert.ToInt32(Session["ROLE"]) == 4)
                {
                    query = "select applicant_details.app_id,applicant_details.applicant_name,mst_memberdesignation.\"DesignationName\",";
                    query = query + " society.socname,mst_societytype.societytype,esociety.status_table.status_id,status_table.application_submission_time from esociety.applicant_details";
                    query = query + " INNER JOIN esociety.society on applicant_details.app_id = society.app_id";
                    query = query + " INNER JOIN esociety.mst_memberdesignation on applicant_details.applicant_designation = mst_memberdesignation.\"DesignationID\"";
                    query = query + " INNER JOIN esociety.mst_societytype on mst_societytype.societyid = society.soctype";
                    query = query + " INNER JOIN esociety.status_table on esociety.status_table.app_id = esociety.applicant_details.app_id";
                    query = query + " INNER JOIN esociety.mst_district on esociety.mst_district.\"DistrictID\" = esociety.society.socdistrict";
                    query = query + " where esociety.status_table.status_id = 3  and society.socdistrict=@district and applicant_details.new_or_renewal = 2 order by status_table.application_submission_time ASC";

                }
                conn.Open();
                cmd.CommandText = query;
                cmd.Parameters.AddWithValue("@district", Convert.ToInt32(Session["DistrictID"].ToString()));
                NpgsqlDataReader dr = cmd.ExecuteReader();
                gv_ApplicationsRenewal.DataSource = dr;
                gv_ApplicationsRenewal.DataBind();
                lbrenewalcount.Text = gv_ApplicationsRenewal.Rows.Count.ToString();
                verifycountlabel.Text =(Convert.ToInt32(verifycountlabel.Text) + gv_ApplicationsRenewal.Rows.Count).ToString();
                dr.Close();
            }
            catch (NpgsqlException ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "societyApproval_Renewal()" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
                Response.Write("<script language='javascript'>alert('" + "Exception while Loading Renewal Applications" + "')</script>");
            }
            finally
            {
                conn.Close();
            }
        }

        protected void societyApprovalObs_Renewal()
        {
            NpgsqlConnection conn = new NpgsqlConnection();
            NpgsqlCommand cmd = new NpgsqlCommand();
            conn.ConnectionString = ConfigurationManager.ConnectionStrings["connect"].ConnectionString;
            cmd.Connection = conn;
            try
            {
                string query = "";
                if (Convert.ToInt32(Session["ROLE"]) == 3)
                {
                    query = "select applicant_details.app_id,applicant_details.applicant_name,mst_memberdesignation.\"DesignationName\",";
                    query = query + " society.socname,mst_societytype.societytype,esociety.status_table.status_id,status_table.application_obs_submission_time from esociety.applicant_details";
                    query = query + " INNER JOIN esociety.society on applicant_details.app_id = society.app_id";
                    query = query + " INNER JOIN esociety.mst_memberdesignation on applicant_details.applicant_designation = mst_memberdesignation.\"DesignationID\"";
                    query = query + " INNER JOIN esociety.mst_societytype on mst_societytype.societyid = society.soctype";
                    query = query + " INNER JOIN esociety.status_table on esociety.status_table.app_id = esociety.applicant_details.app_id";
                    query = query + " INNER JOIN esociety.mst_district on esociety.mst_district.\"DistrictID\" =esociety.society.socdistrict";
                    query = query + " where (esociety.status_table.status_id = 7 or  esociety.status_table.status_id = 6) and society.socdistrict=@district and applicant_details.new_or_renewal =2 order by status_table.application_obs_submission_time ASC";
                }
                else if (Convert.ToInt32(Session["ROLE"]) == 4)
                {
                    query = "select applicant_details.app_id,applicant_details.applicant_name,mst_memberdesignation.\"DesignationName\",";
                    query = query + " society.socname,mst_societytype.societytype,esociety.status_table.status_id,status_table.application_obs_submission_time from esociety.applicant_details";
                    query = query + " INNER JOIN esociety.society on applicant_details.app_id = society.app_id";
                    query = query + " INNER JOIN esociety.mst_memberdesignation on applicant_details.applicant_designation = mst_memberdesignation.\"DesignationID\"";
                    query = query + " INNER JOIN esociety.mst_societytype on mst_societytype.societyid = society.soctype";
                    query = query + " INNER JOIN esociety.status_table on esociety.status_table.app_id = esociety.applicant_details.app_id";
                    query = query + " INNER JOIN esociety.mst_district on esociety.mst_district.\"DistrictID\" =esociety.society.socdistrict";
                    query = query + " where esociety.status_table.status_id = 6  and society.socdistrict=@district and applicant_details.new_or_renewal =2 order by status_table.application_obs_submission_time ASC";

                }
                conn.Open();
                cmd.CommandText = query;
                cmd.Parameters.AddWithValue("@district", Convert.ToInt32(Session["DistrictID"].ToString()));
                NpgsqlDataReader dr = cmd.ExecuteReader();
                gv_obsrenewal.DataSource = dr;
                gv_obsrenewal.DataBind();
                lbrenewalobscount.Text = gv_obsrenewal.Rows.Count.ToString();
                observationcountlabel.Text =(Convert.ToInt32(observationcountlabel.Text) + gv_obsrenewal.Rows.Count).ToString();
                dr.Close();
            }
            catch (NpgsqlException ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "societyApprovalObs_Renewal()" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
                //RecordUserAction("MemberDetails_Page_Load", "Exception at Member Data", "F");
                Response.Write("<script language='javascript'>alert('" + "Exception while loading Obs Applications(Renewal)" + "')</script>");
            }
            finally
            {
                conn.Close();
            }



        }
        protected void ENameLinkBtn_Click(object sender, EventArgs e)
        {
            try
            {
                LinkButton btn = (LinkButton)sender;
                GridViewRow row = (GridViewRow)btn.NamingContainer;
                string index = ((Label)grvApplicantDetails.Rows[row.RowIndex].FindControl("lblRowNum")).Text;
                string app_id = ((Label)grvApplicantDetails.Rows[row.RowIndex].FindControl("LbApp_id")).Text;
                Session["app_id"] = app_id;
                if (app_id == "" || app_id == null)
                {
                    RecordUserAction("Verification_New", "Application ID is Null", "Failed", app_id, 2);
                }
                else
                {
                    // Response.Redirect("VerifySociety.aspx");
                    //comment last paragraph and uncomment upper line if first serve not required
                    if (Convert.ToInt32(index) == 1)
                    {
                        RecordUserAction("Verification_New", "Application open for Verfication", "Success", app_id, 1);
                        Response.Redirect("VerifySociety.aspx");
                    }
                    else
                    {
                        RecordUserAction("Verification_New", "Cicked Application not listed on Serail No 1", "Failed", app_id, 1);
                        Response.Write("<script language='javascript'>alert('" + "You can verify application only in Serial." + "')</script>");
                    }
                }
            }
            catch (Exception ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "ENameLinkBtn_Click" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
            }
        }
        protected void ENameLinkBtn_obs_Click(object sender, EventArgs e)
        {
            try
            {
                LinkButton btn = (LinkButton)sender;
                GridViewRow row = (GridViewRow)btn.NamingContainer;
                string index = ((Label)grvobservation_society.Rows[row.RowIndex].FindControl("lblRowNum_obs")).Text;
                string app_id = ((Label)grvobservation_society.Rows[row.RowIndex].FindControl("LbApp_id_obs")).Text;
                Session["app_id"] = app_id;
                if (app_id == "" || app_id == null)
                {
                    RecordUserAction("Verification_Obs", "Application ID is Null", "Failed", app_id, 2);
                }
                else
                {
                    // Response.Redirect("VerifySociety.aspx");
                    //comment last paragraph and uncomment upper line if first serve not required
                    if (Convert.ToInt32(index) == 1)
                    {
                        RecordUserAction("Verification_Obs", "Application open for Verfication", "Success", app_id, 1);
                        Response.Redirect("VerifySociety.aspx");
                    }
                    else
                    {
                        RecordUserAction("Verification_Obs", "Clicked Application not listed on Serail No 1", "Failed", app_id, 1);
                        Response.Write("<script language='javascript'>alert('" + "You can verify application only in Serial." + "')</script>");
                    }
                }
            }
            catch (Exception ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "ENameLinkBtn_obs_Click" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
            }
        }
        private void RecordUserAction(string action, string description, string status, string app_id, int crud)
        {
            /*Audit trail*/
            int count = 0;
            do
            {
                System.Web.HttpBrowserCapabilities browser = Request.Browser;
                UsersAuditTrails trail = new UsersAuditTrails();
                string uri = HttpContext.Current.Request.Url.AbsoluteUri;
                if (Session["firstname"] != null)
                {
                    trail.admin_login_id = Session["firstname"].ToString();
                    trail.loggedin_status = "Y";
                }
                else
                {

                    trail.admin_login_id = "Invalid Token";
                    trail.loggedin_status = "N";
                }
                trail.app_id = app_id == null ? "Null" : app_id;
                trail.browser_session_id = HttpContext.Current.Request.Cookies[GlobalVars.AntiXsrfTokenKey] == null ? HttpContext.Current.Session.SessionID : HttpContext.Current.Request.Cookies[GlobalVars.AntiXsrfTokenKey].Value;
                trail.admin_session_id = Session["DoTAuthTokAdmin"].ToString() == null ? "Invalid Session" : Session["DoTAuthTokAdmin"].ToString();//login entries value
                trail.referrer = uri != null ? uri.ToString().Length > 100 ? uri.ToString().Substring(0, 100) : uri.ToString() : string.Empty;
                trail.accessed_module = Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath);
                trail.action_performed = action;
                trail.action_description = description.Length > 200 ? description.Substring(0, 200) : description;
                trail.action_status = status;
                trail.tracked_datetime = DateTime.Now;
                trail.loggedin_ip = ipaddress;
                trail.browser_name = browser.Browser;
                trail.browser_version = browser.Version;
                string strUserAgent = Request.UserAgent.ToString().ToLower();
                if (strUserAgent != null)
                {
                    if (Request.Browser.IsMobileDevice == true || strUserAgent.Contains("iphone") ||
                        strUserAgent.Contains("blackberry") || strUserAgent.Contains("mobile") ||
                        strUserAgent.Contains("windows ce") || strUserAgent.Contains("opera mini") ||
                        strUserAgent.Contains("palm"))
                    {
                        trail.device_type = strUserAgent;
                    }
                    else
                    {
                        trail.device_type = "Laptop/ Desktop";
                    }

                }
                trail.is_crud = crud;
                if (Session["userfirstname"] != null)
                {
                    trail.admin_login_name = Session["userfirstname"].ToString() + " " + Session["usermiddlename"].ToString() + " " + Session["userlastname"].ToString();

                }
                else
                {
                    trail.admin_login_name = "Invalid UserName";
                }
                count = ins.SaveOrganizationAuditTrail(trail);
            } while (count == 0);
            /*Audit trail*/


        }


        protected void ENameLinkBtn_renewal_Click(object sender, EventArgs e)
        {
            try
            {
                LinkButton btn = (LinkButton)sender;
                GridViewRow row = (GridViewRow)btn.NamingContainer;
                string index = ((Label)gv_ApplicationsRenewal.Rows[row.RowIndex].FindControl("lblRowNum")).Text;
                string app_id = ((Label)gv_ApplicationsRenewal.Rows[row.RowIndex].FindControl("LbApp_id")).Text;
                Session["app_id"] = app_id;
                if (app_id == "" || app_id == null)
                {
                    RecordUserAction("Verification_Renewal", "Renewal Application ID is Null", "Failed", app_id, 2);
                }
                else
                {
                    // Response.Redirect("VerifySociety.aspx");
                    //comment last paragraph and uncomment upper line if first serve not required
                    if (Convert.ToInt32(index) == 1)
                    {
                        RecordUserAction("Verification_Renewal", "Renewal Application open for Verfication", "Success", app_id, 1);
                        Response.Redirect("VerifySociety.aspx");
                    }
                    else
                    {
                        RecordUserAction("Verification_Renewal", "Clicked Renewal Application not listed on Serail No 1", "Failed", app_id, 1);
                        Response.Write("<script language='javascript'>alert('" + "You can verify application only in Serial." + "')</script>");
                    }
                }
            }
            catch (Exception ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "ENameLinkBtn_renewal_Click" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
            }
        }

        protected void ENameLinkBtn_obs_renewal_Click(object sender, EventArgs e)
        {
            try
            {
                LinkButton btn = (LinkButton)sender;
                GridViewRow row = (GridViewRow)btn.NamingContainer;
                string index = ((Label)gv_obsrenewal.Rows[row.RowIndex].FindControl("lblRowNum_obs")).Text;
                string app_id = ((Label)gv_obsrenewal.Rows[row.RowIndex].FindControl("LbApp_id_obs")).Text;
                Session["app_id"] = app_id;
                if (app_id == "" || app_id == null)
                {
                    RecordUserAction("Verification_Obs", "Application ID is Null", "Failed", app_id, 2);
                }
                else
                {
                    // Response.Redirect("VerifySociety.aspx");
                    //comment last paragraph and uncomment upper line if first serve not required
                    if (Convert.ToInt32(index) == 1)
                    {
                        RecordUserAction("Verification_Obs Renewal", "Renewal Application open for Verfication", "Success", app_id, 1);
                        Response.Redirect("VerifySociety.aspx");
                    }
                    else
                    {
                        RecordUserAction("Verification_Obs Renewal", "Clicked Renewal Application not listed on Serail No 1", "Failed", app_id, 1);
                        Response.Write("<script language='javascript'>alert('" + "You can verify application only in Serial." + "')</script>");
                    }
                }
            }
            catch (Exception ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "ENameLinkBtn_obs_renewal_Click" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
            }
        }

    }
}