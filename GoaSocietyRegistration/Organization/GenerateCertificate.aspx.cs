using GoaSocietyRegistration.Development;
using iTextSharp.text;
using iTextSharp.text.pdf;
using MongoDB.Bson;
using MongoDB.Driver;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Configuration;

using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace GoaSocietyRegistration.Organization
{
    public partial class GenerateCertificate : System.Web.UI.Page
    {
        System.Globalization.CultureInfo french = new System.Globalization.CultureInfo("fr-FR");
        Insert ins = new Insert();
        string ct = string.Empty;
        Int64 fileSizeFront = 0;
        byte[] documentBinary = new Byte[0];
        string ipaddress = Utility.getIP();
        string macaddress = Utility.GetMACAddress(Utility.getIP());
        Society_Certificate doc = new Society_Certificate();
        [DllImport(@"urlmon.dll", CharSet = CharSet.Auto)]
        private extern static UInt32 FindMimeFromData(UInt32 pBC, [MarshalAs(UnmanagedType.LPStr)] String pwzUrl, [MarshalAs(UnmanagedType.LPArray)] byte[] pBuffer,
       UInt32 cbSize, [MarshalAs(UnmanagedType.LPStr)] String pwzMimeProposed, UInt32 dwMimeFlags, out UInt32 ppwzMimeOut, UInt32 dwReserverd);
        ByteEncryption.ByteEncryption obj_Byte_Encryption = new ByteEncryption.ByteEncryption();

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
                        ApproveSocietyList();
                        GenerateCertificateList();
                    }
                    TabName.Value = Request.Form[TabName.UniqueID];
                }
                else
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('As per new Security Guidelines, you are requested to reset the password');window.location ='ChangePassword.aspx';", true);
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


        protected void ApproveSocietyList()
        {
            NpgsqlConnection conn = new NpgsqlConnection();
            NpgsqlCommand cmd = new NpgsqlCommand();
            conn.ConnectionString = ConfigurationManager.ConnectionStrings["connect"].ConnectionString;
            cmd.Connection = conn;
            try
            {
                string query = "";
                query = "select applicant_details.app_id,applicant_details.applicant_name,mst_memberdesignation.\"DesignationName\", applicant_details.new_or_renewal,";
                query = query + " society.socname,mst_societytype.societytype,esociety.status_table.status_id,echallan_pdf_cross_entry,echallan_rcpt_cross_entry,";
                query = query + " final_certificate_mongo_entry from esociety.applicant_details";
                query = query + " INNER JOIN esociety.society on applicant_details.app_id = society.app_id";
                query = query + " INNER JOIN esociety.mst_memberdesignation on applicant_details.applicant_designation = mst_memberdesignation.\"DesignationID\"";
                query = query + " INNER JOIN esociety.mst_societytype on mst_societytype.societyid = society.soctype";
                query = query + " INNER JOIN esociety.status_table on esociety.status_table.app_id = esociety.applicant_details.app_id";
                query = query + " INNER JOIN esociety.online_payment_details on esociety.online_payment_details.app_id=applicant_details.app_id";
                query = query + " INNER JOIN esociety.mst_district on esociety.mst_district.\"DistrictID\" =esociety.society.socdistrict";
                query = query + " where esociety.status_table.status_id = 10 and society.socdistrict= @district and esociety.online_payment_details.active = 'Y' and esociety.online_payment_details.onlinepayment_id = 1";
                //correct query and after that uplaod pdf and show that pdf in old certificates

                conn.Open();
                cmd.CommandText = query;
                cmd.Parameters.AddWithValue("@district", Convert.ToInt32(Session["DistrictID"].ToString()));
                NpgsqlDataReader dr = cmd.ExecuteReader();
                gvApprove.DataSource = dr;
                gvApprove.DataBind();
                dr.Close();
                Label2.Text = Server.HtmlEncode(gvApprove.Rows.Count.ToString());
            }
            catch (NpgsqlException ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "ApproveSocietyList()" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));

                //RecordUserAction("MemberDetails_Page_Load", "Exception at Member Data", "F");
                Response.Write("<script language='javascript'>alert('" + "Exception at Approve Society List" + "')</script>");
            }
            finally
            {
                conn.Close();
            }
        }


        protected void GenerateCertificateList()
        {
            NpgsqlConnection conn = new NpgsqlConnection();
            NpgsqlCommand cmd = new NpgsqlCommand();
            conn.ConnectionString = ConfigurationManager.ConnectionStrings["connect"].ConnectionString;
            cmd.Connection = conn;
            try
            {
                string query = "";
                query = "select applicant_details.app_id,applicant_details.applicant_name,mst_memberdesignation.\"DesignationName\", applicant_details.new_or_renewal,";
                query = query + " society.socname,mst_societytype.societytype,esociety.status_table.status_id,echallan_pdf_cross_entry,echallan_rcpt_cross_entry,";
                query = query + " final_certificate_mongo_entry from esociety.applicant_details";
                query = query + " INNER JOIN esociety.society on applicant_details.app_id = society.app_id";
                query = query + " INNER JOIN esociety.mst_memberdesignation on applicant_details.applicant_designation = mst_memberdesignation.\"DesignationID\"";
                query = query + " INNER JOIN esociety.mst_societytype on mst_societytype.societyid = society.soctype";
                query = query + " INNER JOIN esociety.status_table on esociety.status_table.app_id = esociety.applicant_details.app_id";
                query = query + " INNER JOIN esociety.online_payment_details on esociety.online_payment_details.app_id=applicant_details.app_id";
                query = query + " INNER JOIN esociety.mst_district on esociety.mst_district.\"DistrictID\" =esociety.society.socdistrict";
                query = query + " where esociety.status_table.status_id = 11 and society.socdistrict= @district and esociety.online_payment_details.active = 'Y' and esociety.online_payment_details.onlinepayment_id = 1";
                //correct query and after that uplaod pdf and show that pdf in old certificates

                conn.Open();
                cmd.CommandText = query;
                cmd.Parameters.AddWithValue("@district", Convert.ToInt32(Session["DistrictID"].ToString()));
                NpgsqlDataReader dr = cmd.ExecuteReader();
                gridViewGenerateCertificate.DataSource = dr;
                gridViewGenerateCertificate.DataBind();
                dr.Close();
                Label3.Text = Server.HtmlEncode(gridViewGenerateCertificate.Rows.Count.ToString());
            }
            catch (NpgsqlException ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "GenerateCertificateList()" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));

                //RecordUserAction("MemberDetails_Page_Load", "Exception at Member Data", "F");
                Response.Write("<script language='javascript'>alert('" + "Exception at Generate Society List" + "')</script>");
            }
            finally
            {
                conn.Close();
            }
        }


        public string getRegID(string app_id)
        {
            string regID = "";
            NpgsqlConnection conn = new NpgsqlConnection();
            NpgsqlCommand cmd = new NpgsqlCommand();
            conn.ConnectionString = ConfigurationManager.ConnectionStrings["connect"].ConnectionString;
            cmd.Connection = conn;
            try
            {
                conn.Open();
                string query = "SELECT socregid FROM esociety.society where app_id = @app_id";
                cmd.CommandText = query;
                cmd.Parameters.AddWithValue("@app_id", Convert.ToInt64(app_id));
                NpgsqlDataReader rd = cmd.ExecuteReader();
                if (rd.Read())
                {
                    regID = rd["socregid"].ToString();
                }
                rd.Close();
            }
            catch (NpgsqlException ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "getRegID()" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
                regID = "F";
                //RecordUserAction("MemberDetails_Page_Load", "Exception at Member Data", "F");
                Response.Write("<script language='javascript'>alert('" + "Exception at Generate Society List" + "')</script>");
            }
            finally
            {
                conn.Close();
            }
            return regID;
        }

        protected void LkGenerateCertificate_Click(object sender, EventArgs e)
        {
            try
            {
                LinkButton btn = (LinkButton)sender;
                GridViewRow row = (GridViewRow)btn.NamingContainer;
                string app_id = ((Label)gridViewGenerateCertificate.Rows[row.RowIndex].FindControl("LbApp_id")).Text;
                Session["app_id"] = app_id;
                if (app_id == "" || app_id == null)
                {
                    //RecordUserAction("LbViewverification_Click", "Application ID is Null", "S");
                }
                else
                {
                    //string redirectPage = "Registration.aspx";
                    string renewal_flag = ((Label)gridViewGenerateCertificate.Rows[row.RowIndex].FindControl("lbrenewal")).Text;
                    string societyName = ((Label)gridViewGenerateCertificate.Rows[row.RowIndex].FindControl("LbSocietyName")).Text;
                    
                    int distirct_id = Convert.ToInt32(Session["DistrictID"].ToString());
                    if (Convert.ToInt16(renewal_flag) == 1)//fresh
                    {
                        string reg_ID_check = getRegID(app_id);
                        if (reg_ID_check == "F")
                        {
                            CreateLogFiles Err = new CreateLogFiles();
                            Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), "Failed Fetching", "LkGenerateCertificate_Click()" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
                        }
                        else if (reg_ID_check == "" || reg_ID_check == null)
                        {
                            NpgsqlConnection conn = new NpgsqlConnection();
                            NpgsqlCommand cmd = new NpgsqlCommand();
                            conn.ConnectionString = ConfigurationManager.ConnectionStrings["connect"].ConnectionString;
                            cmd.Connection = conn;
                            conn.Open();
                            NpgsqlTransaction trans = conn.BeginTransaction();
                            try
                            {
                                string regid = "";
                                cmd.Parameters.Clear();
                                string reg_query = "SELECT soc_reg_no FROM esociety.master_sequence where districtid = @districtid";
                                cmd.CommandText = reg_query;
                                cmd.Parameters.AddWithValue("@districtid", distirct_id);
                                NpgsqlDataReader rd = cmd.ExecuteReader();
                                if (rd.Read())
                                {
                                    regid = rd["soc_reg_no"].ToString();
                                    string temp = regid;
                                    rd.Close();
                                    regid = regid + "/Goa/" + DateTime.Now.Year.ToString();
                                    cmd.Parameters.Clear();
                                    int value_temp = Convert.ToInt32(temp);
                                    value_temp = value_temp + 1;
                                    string upquery = "UPDATE esociety.master_sequence SET soc_reg_no = @soc_reg_no WHERE districtid = @districtid";
                                    cmd.CommandText = upquery;
                                    cmd.Parameters.AddWithValue("@soc_reg_no", value_temp);
                                    cmd.Parameters.AddWithValue("@districtid", distirct_id);
                                    cmd.ExecuteNonQuery();
                                    cmd.Parameters.Clear();
                                    string query = "update esociety.society set socregid = @socregid, regdate =@regdate where app_id = @app_id";
                                    cmd.CommandText = query;
                                    cmd.Parameters.AddWithValue("@socregid", regid);
                                    cmd.Parameters.AddWithValue("@regdate", Convert.ToDateTime(DateTime.Today, french));
                                    cmd.Parameters.AddWithValue("@app_id", Convert.ToInt64(app_id));
                                    cmd.ExecuteNonQuery();
                                    cmd.Parameters.Clear();
                                    string doa_query = "UPDATE esociety.members SET dateofadmission = CURRENT_DATE WHERE app_id = @app_id and active = 'Y'";
                                    cmd.CommandText = doa_query;
                                    cmd.Parameters.AddWithValue("@app_id", Convert.ToInt64(app_id));
                                    cmd.ExecuteNonQuery();
                                    cmd.Parameters.Clear();
                                    string final_query = "UPDATE esociety.master_members_table SET dateofadmission = CURRENT_DATE, socregid = @socregid WHERE app_id = @app_id";
                                    cmd.CommandText = final_query;
                                    cmd.Parameters.AddWithValue("@socregid", regid);
                                    cmd.Parameters.AddWithValue("@app_id", Convert.ToInt64(app_id));
                                    cmd.ExecuteNonQuery();
                                    cmd.Parameters.Clear();
                                    string update_master_query = "INSERT INTO esociety.mst_society_master(socname, socregid, regdate, socdistrit, active, society_reg_year)";
                                    update_master_query = update_master_query + " VALUES (@socname, @socregid, CURRENT_DATE, @socdistrit, 'Y', @society_reg_year)";
                                    cmd.CommandText = update_master_query;
                                    cmd.Parameters.AddWithValue("@socname", societyName);
                                    cmd.Parameters.AddWithValue("@socregid", regid);
                                    cmd.Parameters.AddWithValue("@socdistrit", Convert.ToInt32(Session["DistrictID"].ToString()));
                                    cmd.Parameters.AddWithValue("@society_reg_year", DateTime.Today.Year);
                                    cmd.ExecuteNonQuery();
                                    cmd.Parameters.Clear();
                                    trans.Commit();
                                }
                                rd.Close();
                                genrate();
                            }
                            catch (NpgsqlException ex)
                            {
                                CreateLogFiles Err = new CreateLogFiles();
                                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "LkGenerateCertificate_Click()" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
                                trans.Rollback();
                                //RecordUserAtion("MemberDetails_Page_Load", "Exception at Member Data", "F");
                                Response.Write("<script language='javascript'>alert('" + "Exception at Generate Society List" + "')</script>");
                            }
                            finally
                            {
                                conn.Close();
                            }
                        }
                        else if (reg_ID_check != "" && reg_ID_check != null)
                        {
                            genrate();
                        }                       
                       
                    }
                    else //renewal
                    {
                        
                    }

                    




                    //ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "none", "<script language='javascript'>Popup('" + redirectPage + "');</script>", false);

                    //Response.Redirect("Registration.aspx");
                }
            }
            catch (Exception ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "LkGenerateCertificate_Click" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
            }
        }

        protected void ImageButton1_Command(object sender, CommandEventArgs e)
        {
            ImageButton btn = (ImageButton)sender;
            GridViewRow row = (GridViewRow)btn.NamingContainer;
            HiddenField hd = row.FindControl("hfechallanpdf") as HiddenField;
            if (hd.Value != null)
            {
                string objectid = hd.Value.ToString();
                try
                {
                    var str = ConfigurationManager.AppSettings["mongoconnect"];
                    IMongoDatabase database;
                    IMongoClient client;
                    client = new MongoClient(str);
                    database = client.GetDatabase("eGoaSociety");
                    var collection = database.GetCollection<EchallanReceipt>("eChallanPDF");
                    var status = collection.Find(x => x._Id == ObjectId.Parse(objectid)).FirstOrDefault();
                    byte[] pdf = status.DocContent;
                    convertToPdf(pdf);
                }
                catch (Exception ex)
                {
                    Response.Write("<script language='javascript'>alert('" + "PDF View Failed" + "')</script>");
                    CreateLogFiles Err = new CreateLogFiles();
                    Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "ImageButton1_Command" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));

                }
            }
            else
            {
                Response.Write("<script language='javascript'>alert('" + "PDF View Failed" + "')</script>");
            }
        }
        protected void convertToPdf(byte[] mssg)
        {
            System.IO.FileStream fs;
            try
            {
                byte[] tmpfiledata = mssg;
                Session["pdfpath"] = "~/OutData/" + Session["org_pdf"].ToString() + "_org.pdf";
                string sPathToSaveFileTo = Server.MapPath("~/OutData/" + Session["org_pdf"].ToString() + "_org.pdf");
                using (fs = new System.IO.FileStream(sPathToSaveFileTo, System.IO.FileMode.Create))
                using (System.IO.BinaryWriter bw = new System.IO.BinaryWriter(fs))
                {
                    bw.Write(tmpfiledata);
                    RecordUserAction("Create", "convertToPdf -- Pdf Viewed", "Success", "NA", 1);
                    bw.Close();
                }
                WebClient User = new WebClient();
                Byte[] FileBuffer = User.DownloadData(sPathToSaveFileTo);
                embed1.Attributes.Add("src", Session["pdfpath"].ToString());
                if (FileBuffer != null)
                {
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "none", "<script> $(window).on('load', function () { $('#pdfModal').modal({ backdrop: 'static' });});</script>", false);

                }
            }
            catch (Exception ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "convertToPdf()" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath) + " from " + ipaddress);
            }
            //try
            //{
            //    byte[] tmpfiledata = mssg;
            //    string sPathToSaveFileTo = Server.MapPath("~/SelectedFile.pdf");
            //    using (System.IO.FileStream fs = new System.IO.FileStream(sPathToSaveFileTo, System.IO.FileMode.Create, System.IO.FileAccess.ReadWrite))
            //    {

            //        using (System.IO.BinaryWriter bw = new System.IO.BinaryWriter(fs))
            //        {
            //            bw.Write(tmpfiledata);
            //            bw.Close();
            //        }
            //    }
            //    WebClient User = new WebClient();
            //    Byte[] FileBuffer = User.DownloadData(Server.HtmlEncode(sPathToSaveFileTo));
            //    if (FileBuffer != null)
            //    {
            //        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "none", "<script> $(window).on('load', function () { $('#pdfModal').modal({ backdrop: 'static' });});</script>", false);
            //        //Response.ContentType = "application/pdf";
            //        ////Response.AddHeader("content-length", FileBuffer.Length.ToString());
            //        //Response.Write("window.open('sPathToSaveFileTo','_blank');");
            //        //Response.BinaryWrite(FileBuffer);
            //    }
            //}
            //catch (Exception ex)
            //{
            //    CreateLogFiles Err = new CreateLogFiles();
            //    Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "convertToPdf()" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
            //}
        }
        protected void ImageButton2_Command(object sender, CommandEventArgs e)
        {
            ImageButton btn = (ImageButton)sender;
            GridViewRow row = (GridViewRow)btn.NamingContainer;
            HiddenField hd = row.FindControl("hfechallanreceipt") as HiddenField;
            if (hd.Value != null)
            {
                IMongoDatabase database;
                IMongoClient client;
                string objectid = hd.Value.ToString();
                try
                {
                    var str = ConfigurationManager.AppSettings["mongoconnect"];

                    client = new MongoClient(str);
                    database = client.GetDatabase("eGoaSociety");
                    var collection = database.GetCollection<EchallanReceipt>("eChallanReceipt");
                    var status = collection.Find(x => x._Id == ObjectId.Parse(objectid)).FirstOrDefault();
                    byte[] pdf = status.DocContent;
                    convertToPdf(pdf);
                }
                catch (Exception ex)
                {
                    CreateLogFiles Err = new CreateLogFiles();
                    Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "ImageButton2_Command" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));

                    Response.Write("<script language='javascript'>alert('" + "PDF View Failed Error" + "')</script>");
                }
                finally
                {
                    client = null;
                }
            }
            else { Response.Write("<script language='javascript'>alert('" + "PDF View Failed " + "')</script>"); }
        }

        protected void LkUpload_Click(object sender, EventArgs e)
        {
            try
            {
                LinkButton btn = (LinkButton)sender;
                GridViewRow row = (GridViewRow)btn.NamingContainer;
                string app_id = ((Label)gridViewGenerateCertificate.Rows[row.RowIndex].FindControl("LbApp_id")).Text;
                TxtBxAppID.Text = Server.HtmlEncode(app_id);
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "none", "<script> $(window).on('load', function () { $('#UploadModal').modal({ backdrop: 'static' });});</script>", false);

            }
            catch (Exception ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "LkUpload_Click" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
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
        public int fileuploadfucntion(FileUpload fu, Label lb, Society_Certificate othdoc, string ip, string mac, long App_ID, string collection)
        {
            int flag = 0;
            string a = "Society Registration Certificate";

            try
            {
                if (fu.HasFile)
                {
                    HttpPostedFile file = fu.PostedFile;
                    byte[] document = new byte[file.ContentLength];
                    file.InputStream.Read(document, 0, file.ContentLength);
                    UInt32 mimetype;
                    FindMimeFromData(0, null, document, 256, null, 0, out mimetype, 0);
                    IntPtr mimeTypePtr = new IntPtr(mimetype);
                    string mimeType = Marshal.PtrToStringUni(mimeTypePtr).ToLower();
                    Marshal.FreeCoTaskMem(mimeTypePtr);
                    var scanExitCode = 0;//UtilityDAO.VirusScanFile(flupHeader.PostedFile.FileName);
                    if (scanExitCode == 0)
                    {
                        string UploadFileName = file.FileName;
                        string Extension = UploadFileName.Substring(UploadFileName.LastIndexOf('.') + 1).ToLower();
                        var extCount = UploadFileName.Split('.').Length - 1;

                        if (Extension.ToLower() != "pdf" || file.ContentType.ToLower() != "application/pdf" || mimeType != "application/pdf"
                            || extCount > 1
                            || document == null
                            || file.ContentLength <= 0 || file.ContentLength > (5 * 1024 * 1024))
                        {
                            lb.Visible = true;
                            lb.Text = "File name can not contain multiple dots/extensions. Only Pdf file of maximum size 5MB allowed !";
                            lb.ForeColor = System.Drawing.Color.Red;
                            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "none", "<script> $(window).on('load', function () { $('#errorModal').modal({ backdrop: 'static' });});</script>", false);
                            flag = 0;
                        }
                        ////////////////////////
                        else
                        {
                            string path = Utility.filesave(fu, App_ID.ToString());
                            string resultedStr = "";
                            using (StreamReader sr = new StreamReader(path))
                            {
                                resultedStr = SurroundingClass.ReadStreamWithTimeout(sr);
                            }
                            int scanned = Utility.checkFile(resultedStr);
                            if (scanned == 0)
                            {
                                Response.Write("<script>alert('Invalid PDF uploaded');</script>");
                                File.Delete(path);
                            }
                            else
                            {
                                File.Delete(path);
                                fileSizeFront = fu.FileContent.Length;
                                documentBinary = new byte[fileSizeFront];
                                //Stream fs = fu.PostedFile.InputStream;
                                //BinaryReader br = new BinaryReader(fs);
                                //byte[] bytes = br.ReadBytes((Int32)fs.Length);
                                byte[] bytes = fu.FileBytes;
                                byte[] encrypt_bytes = obj_Byte_Encryption.EncryptData(bytes, System.Web.HttpContext.Current.Server.MapPath("~/ENCRYPT.KEY"));
                                string document_id = Utility.getFinalCertificateUploadID();//other doc IDvrtyrtyrtvyrty
                                othdoc.App_ID = App_ID;
                                othdoc.Doc_CT = ct;
                                othdoc.time_stamp = DateTime.Now.ToString();
                                ObjectId obj_id = ObjectId.GenerateNewId();
                                othdoc._Id = obj_id;
                                othdoc.IpAddress = ip;
                                othdoc.MacAddress = mac;
                                othdoc.Active = true;
                                othdoc.UpdatedBy = Session["firstname"].ToString();
                                othdoc.Doc_ID = document_id;
                                othdoc.doc_name = a;
                                othdoc.DocContent = encrypt_bytes;
                                int amongo = InsertintoMongoDB(othdoc, collection, lb);
                                if (amongo == 1)
                                {
                                    int pvalue = insertentryPosgres(obj_id, App_ID);
                                    if (pvalue == 1)
                                    {
                                        flag = 1;
                                    }
                                    else
                                    {
                                        flag = 0;
                                    }

                                }
                                else
                                {
                                    flag = 0;
                                }
                            }
                        }
                    }
                }
                else
                {
                    lb.Visible = true;
                    lb.ForeColor = System.Drawing.Color.Red;
                    lb.Text = "Upload " + a + " Certificate file in PDF format";
                    LbError.ForeColor = System.Drawing.Color.Red;
                    LbError.Text = "Upload " + a + " Certificate file in PDF format";
                    flag = 0;
                }

            }
            catch (Exception ex)
            {
                lb.Visible = true;
                lb.Text = "File upload failed.";
                //RecordUserAction("Fileupload Function ", ex.Message, "F");
                lb.ForeColor = System.Drawing.Color.Red;
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "fileuploadfucntion()" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));

            }

            return flag;
        }
        public int insertentryPosgres(ObjectId obj, long appid)
        {
            int result = 0;

            NpgsqlConnection conn = new NpgsqlConnection();
            NpgsqlCommand cmd = new NpgsqlCommand();
            conn.ConnectionString = ConfigurationManager.ConnectionStrings["connect"].ConnectionString;
            cmd.Connection = conn;
            string objectid = obj.ToString();
            conn.Open();
            try
            {
                string grm_query = "UPDATE esociety.society SET final_certificate_mongo_entry=@object_id where app_id = @app_id";
                cmd.CommandText = grm_query;
                cmd.Parameters.AddWithValue("@object_id", objectid);
                cmd.Parameters.AddWithValue("@app_id", appid);
                cmd.ExecuteNonQuery(); result = 1;
                //RecordUserAction("insertentryPosgres", "objectid saved to posgres", "S");
            }
            catch (NpgsqlException ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "insertentryPosgres()" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
                result = 0;
                //RecordUserAction("insertentryPosgres", ex.Message, "F");
                Response.Write("<script language='javascript'>alert('" + "Exection Error" + "')</script>");
            }
            finally
            {
                conn.Close();
            }
            return result;

        }
        public int InsertintoMongoDB(Society_Certificate doc, string sel_collection, Label lb)
        {
            Insert insr = new Insert();
            try
            {
                //RecordUserAction("InsertintoMongoDB", "write document to Mongo", "S");
                return insr.InserFinalSociety(doc, sel_collection);
            }
            catch (MongoException ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "InsertintoMongoDB()" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));

                // RecordUserAction("InsertintoMongoDB", ex.Message, "F");
                lb.Text = "Saved to db failed";
                return 0;
            }
        }
        protected void UploadDocument_Click(object sender, EventArgs e)
        {
            try
            {
                if (!FileUpload1.HasFile)
                {
                    LbError.Text = "File has not been uploaded";
                    LbError.Visible = true;
                    LbError.ForeColor = System.Drawing.Color.Red;
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "none", "<script> $(window).on('load', function () { $('#UploadModal').modal({ backdrop: 'static' });});</script>", false);
                }
                else
                {
                    int x = fileuploadfucntion(FileUpload1, LbError, doc, ipaddress, macaddress, Convert.ToInt64(TxtBxAppID.Text), "Society Certificate");


                    //foreach (GridViewRow row in gridViewGenerateCertificate.Rows)
                    //{
                    //    LinkButton view = (LinkButton)row.FindControl("LkView");
                    //    LinkButton delete = (LinkButton)row.FindControl("LkDelete");
                    //    LinkButton uploadNoc = (LinkButton)row.FindControl("LkUpload");
                    //    CheckBox proceed = (CheckBox)row.FindControl("chkconfirm");

                    //    if (x == 1)
                    //    {
                    //        // RecordUserAction("adddNOC_Click", "File Uploaded Successfully", "S");
                    //        view.Enabled = true;
                    //        delete.Enabled = true;
                    //        uploadNoc.Enabled = false;
                    //        proceed.Enabled = true;
                    //        //ENABLE PROCEED
                    //    }
                    //    else
                    //    {
                    //        //RecordUserAction("adddNOC_Click", "Upload Failed", "F");
                    //        uploadNoc.Enabled = true;
                    //        view.Enabled = false;
                    //        delete.Enabled = false;
                    //        proceed.Enabled = false;
                    //        //FALSE PROCEED

                    //    }
                        
                    //}

                    GenerateCertificateList();
                    ApproveSocietyList();

                }
            }
            catch (Exception ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "UploadDocument_Click" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
            }
        }

        protected void LkView_Click(object sender, EventArgs e)
        {
            try
            {
                LinkButton btn = (LinkButton)sender;
                GridViewRow row = (GridViewRow)btn.NamingContainer;
                string mongoentry = ((Label)gridViewGenerateCertificate.Rows[row.RowIndex].FindControl("mongoentryFINAL")).Text;
                if (mongoentry != null)
                {
                    byte[] bytes1 = openConnectionMongo_(mongoentry, "Society Certificate");
                    byte[] data = obj_Byte_Encryption.DecryptData(bytes1, System.Web.HttpContext.Current.Server.MapPath("~/ENCRYPT.KEY"));
                    convertToPdf(data);
                }
            }
            catch (Exception ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "LkView_Click" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
            }
        }

        protected byte[] openConnectionMongo_(string objectid, string collection_name)
        {
            try
            {
                var str = ConfigurationManager.AppSettings["mongoconnect"];
                IMongoDatabase database;
                IMongoClient client;
                client = new MongoClient(str);
                database = client.GetDatabase("eGoaSociety");
                var collection = database.GetCollection<Society_Certificate>(collection_name);
                var status = collection.Find(x => x._Id == MongoDB.Bson.ObjectId.Parse(objectid)).FirstOrDefault();
                byte[] decrypt_pdf_bytes = status.DocContent;
                return decrypt_pdf_bytes;
            }
            catch (MongoException ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "openConnectionMongo_()" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));

                //RecordUserAction("openConnectionMongo_societyDocs", ex.Message, "F");
                Response.Write("<script language='javascript'>alert('" + "PDF View Failed" + ".No file uploaded yet')</script>");
                return null;
            }
        }

        protected void gridViewGenerateCertificate_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType.Equals(DataControlRowType.DataRow))
                {
                    if (e.Row.Cells[6].Text.Contains("1"))
                    {
                        string str = e.Row.Cells[4].Text;
                        str = "New";
                        e.Row.Cells[6].Text = str;

                    }
                    else if (e.Row.Cells[6].Text.Contains("2"))
                    {
                        string str = e.Row.Cells[4].Text;
                        str = "Renewal";
                        e.Row.Cells[6].Text = str;

                    }
                    else
                    {
                        string str = e.Row.Cells[6].Text;
                        str = "NA";
                        e.Row.Cells[6].Text = str;
                    }


                }

                foreach (GridViewRow row in gridViewGenerateCertificate.Rows)
                {
                    string status = ((Label)gridViewGenerateCertificate.Rows[row.RowIndex].FindControl("mongoentryFINAL")).Text;
                    LinkButton view = (LinkButton)row.FindControl("LkView");
                    LinkButton delete = (LinkButton)row.FindControl("LkDelete");
                    LinkButton upload = (LinkButton)row.FindControl("LkUpload");
                    CheckBox proceed = (CheckBox)row.FindControl("chkconfirm");
                    if (status == null || status == "")
                    {
                        upload.Enabled = true;
                        view.Enabled = false;
                        delete.Enabled = false;
                        proceed.Enabled = false;
                    }
                    else
                    {
                        upload.Enabled = false;
                        view.Enabled = true;
                        delete.Enabled = true;
                        proceed.Enabled = true;
                    }


                }
            }
            catch (Exception ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "gridViewGenerateCertificate_RowDataBound" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
            }
        }

        protected void LkDelete_Click(object sender, EventArgs e)
        {
            try
            {
                LinkButton btn = (LinkButton)sender;
                GridViewRow row = (GridViewRow)btn.NamingContainer;
                string app_id = ((Label)gridViewGenerateCertificate.Rows[row.RowIndex].FindControl("LbApp_id")).Text;
                hdAppID.Value = Server.HtmlEncode(app_id);
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "none", "<script> $(window).on('load', function () { $('#DeleteConfirm').modal({ backdrop: 'static' });});</script>", false);
            }
            catch (Exception ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "LkDelete_Click" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
            }
        }


        protected void BtnYes_Click(object sender, EventArgs e)
        {
            if (hdAppID.Value != null)
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
                    string query = "SELECT * FROM esociety.society where app_id=@app_id";
                    cmd.CommandText = query;
                    cmd.Parameters.AddWithValue("@app_id", Convert.ToInt64(hdAppID.Value));
                    NpgsqlDataReader rd = cmd.ExecuteReader();
                    if (rd.Read())
                    {
                        ViewState["socname"] = rd["socname"].ToString();
                        ViewState["socaddr"] = rd["socaddr"].ToString();
                        ViewState["regdate"] = rd["regdate"].ToString();
                        if (rd["socregid"] == null)
                        {
                            ViewState["socregid"] = "NA";
                        }
                        else
                        {
                            ViewState["socregid"] = rd["socregid"].ToString();
                        }
                        ViewState["regfee"] = (int)rd["regfee"];
                        ViewState["processfee"] = (int)rd["processfee"];
                        //ViewState["doc_id"] = (long)rd["doc_id"];
                        ViewState["totalmem"] = rd["totalmem"].ToString();
                        ViewState["created_at"] = (DateTime)rd["created_at"];
                        ViewState["created_by"] = rd["created_by"].ToString();
                        ViewState["ipaddress"] = rd["ipaddress"].ToString();
                        ViewState["macaddress"] = rd["macaddress"].ToString();
                        ViewState["active"] = rd["active"].ToString();
                        ViewState["soc_taluka"] = (int)rd["soc_taluka"];
                        ViewState["socdistrict"] = (int)rd["socdistrict"];
                        ViewState["soctype"] = (int)rd["soctype"];
                        ViewState["doc_one"] = rd["doc_one"].ToString();
                        ViewState["doc_two"] = rd["doc_two"].ToString();
                        ViewState["complete"] = rd["complete"].ToString();
                        ViewState["totalfee"] = (int)rd["totalfee"];
                        ViewState["login_id"] = rd["login_id"].ToString();
                        ViewState["complete_data"] = rd["complete_data"].ToString();
                        ViewState["pincode"] = (Decimal)rd["pincode"];
                        ViewState["echallan_no"] = rd["echallan_no"].ToString();
                        ViewState["final_certificate_mongo_entry"] = rd["final_certificate_mongo_entry"].ToString();
                    }
                    rd.Close();
                    cmd.Parameters.Clear();

                    string insert_query = "INSERT INTO esociety.history_society( socname, socaddr, app_id, regfee, processfee, totalmem, created_at, created_by,";
                    insert_query = insert_query + " ipaddress, macaddress, active, soc_taluka, socdistrict, soctype, doc_one, doc_two, complete, totalfee, login_id,";
                    insert_query = insert_query + " complete_data, pincode,final_certificate_mongo_entry) VALUES (@socname, @socaddr, @app_id, @regfee, @processfee, @totalmem, @created_at,";
                    insert_query = insert_query + " @created_by, @ipaddress, @macaddress, @active, @soc_taluka, @socdistrict, @soctype, @doc_one, @doc_two,";
                    insert_query = insert_query + " @complete, @totalfee, @login_id, @complete_data, @pincode,@final_certificate_mongo_entry)";
                    cmd.CommandText = insert_query;
                    cmd.Parameters.AddWithValue("@socname", ViewState["socname"].ToString());
                    cmd.Parameters.AddWithValue("@socaddr", ViewState["socaddr"].ToString());
                    cmd.Parameters.AddWithValue("@app_id", Convert.ToInt64(hdAppID.Value));
                    cmd.Parameters.AddWithValue("@regfee", Convert.ToInt64(ViewState["regfee"].ToString()));
                    cmd.Parameters.AddWithValue("@processfee", Convert.ToInt64(ViewState["processfee"].ToString()));
                    cmd.Parameters.AddWithValue("@totalmem", ViewState["totalmem"].ToString());
                    cmd.Parameters.AddWithValue("@created_at", Convert.ToDateTime(ViewState["created_at"].ToString()));
                    cmd.Parameters.AddWithValue("@created_by", ViewState["created_by"].ToString());
                    cmd.Parameters.AddWithValue("@ipaddress", ViewState["ipaddress"].ToString());
                    cmd.Parameters.AddWithValue("@macaddress", ViewState["macaddress"].ToString());
                    cmd.Parameters.AddWithValue("@active", ViewState["active"].ToString());
                    cmd.Parameters.AddWithValue("@soc_taluka", Convert.ToInt64(ViewState["soc_taluka"].ToString()));
                    cmd.Parameters.AddWithValue("@socdistrict", Convert.ToInt64(ViewState["socdistrict"].ToString()));
                    cmd.Parameters.AddWithValue("@soctype", Convert.ToInt64(ViewState["soctype"].ToString()));
                    cmd.Parameters.AddWithValue("@doc_one", ViewState["doc_one"].ToString());
                    cmd.Parameters.AddWithValue("@doc_two", ViewState["doc_two"].ToString());
                    cmd.Parameters.AddWithValue("@complete", ViewState["complete"].ToString());
                    cmd.Parameters.AddWithValue("@totalfee", Convert.ToInt64(ViewState["totalfee"].ToString()));
                    cmd.Parameters.AddWithValue("@login_id", ViewState["login_id"].ToString());
                    cmd.Parameters.AddWithValue("@complete_data", ViewState["complete_data"].ToString());
                    cmd.Parameters.AddWithValue("@pincode", Convert.ToInt64(ViewState["pincode"].ToString()));
                    cmd.Parameters.AddWithValue("@final_certificate_mongo_entry", ViewState["final_certificate_mongo_entry"].ToString());
                    cmd.ExecuteNonQuery();
                    cmd.Parameters.Clear();
                    string update_query = "UPDATE esociety.society SET	final_certificate_mongo_entry=@mongo WHERE app_id = @app_id";
                    cmd.CommandText = update_query;
                    cmd.Parameters.AddWithValue("@mongo", DBNull.Value);
                    cmd.Parameters.AddWithValue("@app_id", Convert.ToInt64(hdAppID.Value));
                    cmd.ExecuteNonQuery();
                    myTrans.Commit();
                    foreach (GridViewRow row in gridViewGenerateCertificate.Rows)
                    {
                        LinkButton view = (LinkButton)row.FindControl("LkView");
                        LinkButton delete = (LinkButton)row.FindControl("LkDelete");
                        LinkButton uploadNoc = (LinkButton)row.FindControl("LkUpload");
                        CheckBox proceed = (CheckBox)row.FindControl("chkconfirm");


                        // RecordUserAction("adddNOC_Click", "File Uploaded Successfully", "S");
                        view.Enabled = false;
                        delete.Enabled = false;
                        uploadNoc.Enabled = true;
                        proceed.Enabled = false;
                        //ENABLE PROCEED

                        //RecordUserAction("adddNOC_Click", "Upload Failed", "F");

                        //FALSE PROCEED




                    }
                    Response.Write("<script language='javascript'>alert('" + "Document deleted Successfully" + "')</script>");
                }
                catch (NpgsqlException ex)
                {
                    myTrans.Rollback();
                    foreach (GridViewRow row in gridViewGenerateCertificate.Rows)
                    {
                        LinkButton view = (LinkButton)row.FindControl("LkView");
                        LinkButton delete = (LinkButton)row.FindControl("LkDelete");
                        LinkButton uploadNoc = (LinkButton)row.FindControl("LkUpload");
                        CheckBox proceed = (CheckBox)row.FindControl("chkconfirm");


                        uploadNoc.Enabled = false;
                        view.Enabled = true;
                        delete.Enabled = true;
                        proceed.Enabled = true;
                    }
                    CreateLogFiles Err = new CreateLogFiles();
                    Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "BtnYes_Click" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));

                    Response.Write("<script language='javascript'>alert('" + "Exception Encountered.." + "')</script>");
                }
                finally
                {
                    GenerateCertificateList();
                    ApproveSocietyList();
                    conn.Close();
                }


            }
        }

        protected void chkconfirm_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                CheckBox btn = (CheckBox)sender;
                GridViewRow row = (GridViewRow)btn.NamingContainer;
                string app_id = ((Label)gridViewGenerateCertificate.Rows[row.RowIndex].FindControl("LbApp_id")).Text;
                hdAppID.Value = Server.HtmlEncode(app_id);
                BtnProceed.Visible = true;
                BtnYes.Visible = false;
                Label48.Text = "click proceed to continue.";
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "none", "<script> $(window).on('load', function () { $('#DeleteConfirm').modal({ backdrop: 'static' });});</script>", false);

            }
            catch (Exception ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "chkconfirm_CheckedChanged" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
            }
        }

        protected void BtnProceed_Click(object sender, EventArgs e)
        {
            if (hdAppID.Value != null)
            {
                NpgsqlConnection conn = new NpgsqlConnection();
                NpgsqlCommand cmd = new NpgsqlCommand();
                conn.ConnectionString = ConfigurationManager.ConnectionStrings["connect"].ConnectionString;
                cmd.Connection = conn;
                try
                {
                    conn.Open();
                    string query = "UPDATE esociety.status_table SET status_id = 12 WHERE app_id =@app_id ";
                    cmd.CommandText = query;
                    cmd.Parameters.AddWithValue("@app_id", Convert.ToInt64(hdAppID.Value));
                    cmd.ExecuteNonQuery();

                }
                catch (NpgsqlException ex)
                {
                    CreateLogFiles Err = new CreateLogFiles();
                    Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "BtnProceed_Click" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));

                    Response.Write("<script language='javascript'>alert('" + "Exception Encountered.." + "')</script>");
                }
                finally
                {
                    GenerateCertificateList();
                    ApproveSocietyList();
                    conn.Close();

                }

            }

        }




        ///////////////////////////////////////////////////////////////////Auto Generated PDF


        public string getofficedetails()
        {
            string temp_office = "|||||";
            NpgsqlConnection conn = new NpgsqlConnection();
            NpgsqlCommand cmd = new NpgsqlCommand();
            conn.ConnectionString = ConfigurationManager.ConnectionStrings["connect"].ConnectionString;
            cmd.Connection = conn;
            string app_id = Session["app_id"].ToString();
            try
            {
                conn.Open();
                string query = "select userfirstname,userlastname,user_designation,address,office_tel_no,officename,\"DistrictName\" from esociety.admin_table,esociety.mst_district where admin_table.district_id=mst_district.\"DistrictID\" and username=@username";
                cmd.CommandText = query;
                cmd.Parameters.AddWithValue("@username", Server.HtmlEncode(Session["LoginName"].ToString()));
                NpgsqlDataReader rd = cmd.ExecuteReader();
                if (rd.Read())
                {
                    string officename = rd["officename"].ToString();
                    string officeaddress = Server.HtmlEncode(rd["address"].ToString());
                    string officeteleno = Server.HtmlEncode(rd["office_tel_no"].ToString());
                    string officeemail = Server.HtmlEncode(Session["LoginName"].ToString());
                    string firstname = Server.HtmlEncode(rd["userfirstname"].ToString());
                    string lastname = Server.HtmlEncode(rd["userlastname"].ToString());
                    string registrarname = "(" + firstname + " " + lastname + ")";
                    registrarname = registrarname.ToUpper();
                    string designation = Server.HtmlEncode(rd["user_designation"].ToString()) + " (" + Server.HtmlEncode(rd["DistrictName"].ToString()) + ")";
                    temp_office = officename + "|" + officeaddress + "|" + officeteleno + "|" + officeemail + "|" + registrarname + "|" + designation;
                }
                rd.Close();
                cmd.Parameters.Clear();
            }
            catch (NpgsqlException ex)
            {
                temp_office = "|||||";
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "getofficedetails()" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath) + " from " + ipaddress);

            }
            finally
            {
                conn.Close();
            }
            return temp_office;
        }
        public string getsocietydetails()
        {
            string soc_temp = "||";
            NpgsqlConnection conn = new NpgsqlConnection();
            NpgsqlCommand cmd = new NpgsqlCommand();
            conn.ConnectionString = ConfigurationManager.ConnectionStrings["connect"].ConnectionString;
            cmd.Connection = conn;
            string app_id = Session["app_id"].ToString();
            try
            {
                conn.Open();
                cmd.Parameters.Clear();
                string query1 = "select socname,socaddr,\"TalukaName\",totalfee,socregid,regdate from esociety.society,esociety.mst_taluka where app_id=@app_id and society.soc_taluka=mst_taluka.\"TalukaID\"";
                //cmd.Parameters.AddWithValue(); Application id
                cmd.CommandText = query1;
                cmd.Parameters.AddWithValue("@app_id", Convert.ToInt64(app_id));
                NpgsqlDataReader dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                   string societyname = Server.HtmlEncode(dr["socname"].ToString());// + ", " + Server.HtmlEncode(dr["socaddr"].ToString()) + ", " + Server.HtmlEncode(dr["TalukaName"].ToString()) + " Goa";
                  
                    string societyregistrationid = Server.HtmlEncode(dr["socregid"].ToString());
                    string amountpaid = "Paid ₹ " + Server.HtmlEncode(dr["totalfee"].ToString());
                    //DateTime dt = Convert.ToDateTime(Server.HtmlEncode(dr["regdate"].ToString()));
                    DateTime dt = DateTime.Now;
                    string date = dt.ToString("dd") + " " + dt.ToString("MMMM") + " , " + dt.Year.ToString();
                    soc_temp = societyname + "|" + societyregistrationid + "|" + amountpaid;
                }
                dr.Close();
                cmd.Parameters.Clear();
            }
            catch (NpgsqlException ex)
            {
                soc_temp = "||";
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "getsocietydetails()" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath) + " from " + ipaddress);

            }
            finally
            {
                conn.Close();
            }
            return soc_temp;
        }
        public string getpaymentdetails()
        {
            string payment_temp = "||";
            NpgsqlConnection conn = new NpgsqlConnection();
            NpgsqlCommand cmd = new NpgsqlCommand();
            conn.ConnectionString = ConfigurationManager.ConnectionStrings["connect"].ConnectionString;
            cmd.Connection = conn;
            string app_id = Session["app_id"].ToString();
            try
            {
                conn.Open();
                cmd.Parameters.Clear();
                string query2 = "select total_amt,echallan_no,bank_rcvd_date from esociety.online_payment_details where app_id=@app_id and active='Y' and status='S' and onlinepayment_id=1";
                cmd.CommandText = query2;
                cmd.Parameters.AddWithValue("@app_id", Convert.ToInt64(app_id));
                NpgsqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    string amountpaid = "Paid ₹" + Server.HtmlEncode(reader["total_amt"].ToString()) + "/-";
                    string echallanno = Server.HtmlEncode(reader["echallan_no"].ToString());

                    var dob = reader["bank_rcvd_date"].ToString();
                    DateTime date = Convert.ToDateTime(dob, french);

                    //var dateFormat = Server.HtmlEncode(Convert.ToString(reader["bank_rcvd_date"]));
                    //DateTime date = DateTime.Parse(dateFormat);
                    string echallandate = date.ToString("dd/MM/yyyy");//.Substring(0, 10);
                    payment_temp = amountpaid + "|" + echallanno + "|" + echallandate;
                }
                reader.Close();

            }
            catch (NpgsqlException ex)
            {
                payment_temp = "||";
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "getpaymentdetails()" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath) + " from " + ipaddress);

            }
            finally
            {
                conn.Close();
            }
            return payment_temp;
        }


        protected void genrate()
        {
            try
            {
                string officedetails = getofficedetails();
                string[] Result_officedetails = officedetails.Split('|');
                string societydetails = getsocietydetails();
                string[] Result_societydetails = societydetails.Split('|');
                string paymentdetails = getpaymentdetails();
                string[] Result_paymentdetails = paymentdetails.Split('|');
                Document document = new Document(PageSize.A4);
                FileStream fs = new FileStream(Server.MapPath("~/OutData/" + "SocietyCertificate_" + Session["app_id"].ToString() + ".pdf"), FileMode.Create, FileAccess.Write);
                PdfWriter writer = PdfWriter.GetInstance(document, fs);
                document.Open();

                PdfContentByte cb = writer.DirectContent;
                BaseFont bf = BaseFont.CreateFont(BaseFont.TIMES_ROMAN, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
                cb.SetColorFill(BaseColor.BLACK);
                PdfGState gs1 = new PdfGState();
                gs1.BlendMode = new PdfName("Darken");
                cb.SetGState(gs1);
                var spacer = new Paragraph("")
                {
                    SpacingAfter = 2f,
                    SpacingBefore = 2f,
                };
                var spacertop = new Paragraph("")
                {
                    SpacingAfter = 1f,
                    SpacingBefore = 1f,
                };
                var spacepara = new Paragraph("")
                {
                    SpacingAfter = 4f,
                    SpacingBefore = 4f,
                };

                Font regular = new Font(Font.FontFamily.TIMES_ROMAN, 12);
                Font regular10 = new Font(Font.FontFamily.TIMES_ROMAN, 10);
                Font bold8 = new Font(Font.FontFamily.TIMES_ROMAN, 8, Font.BOLD);
                Font bold = new Font(Font.FontFamily.TIMES_ROMAN, 14, Font.BOLD);
                Font bold10 = new Font(Font.FontFamily.TIMES_ROMAN, 10, Font.BOLD);
                Font bold12 = new Font(Font.FontFamily.TIMES_ROMAN, 12, Font.BOLD);
                Font bold14 = new Font(Font.FontFamily.TIMES_ROMAN, 14, Font.BOLD);
                //Paragraph top = new Paragraph();


                Chunk newline = new Chunk(Environment.NewLine);
                Paragraph abovetable = new Paragraph();
                abovetable.Add(newline); abovetable.Add(newline);
                document.Add(abovetable);


                Chunk deptName = new Chunk("Government of Goa", regular);
                Chunk officename = new Chunk(Result_officedetails[0], bold14);
                Chunk officeaddress = new Chunk(Result_officedetails[1], regular);
                Chunk telphone = new Chunk("Off Tel No.", bold10);
                Chunk notel = new Chunk(Result_officedetails[2], regular10);
                Chunk email = new Chunk("  Email:- ", bold10);
                Chunk emailaddress = new Chunk(Result_officedetails[3], regular10);
                emailaddress.SetUnderline(0.1f, -2f);
                Chunk website = new Chunk(" Website:- ", bold10);
                Chunk webname = new Chunk("www.registration.goa.gov.in", regular10);

                Paragraph phr = new Paragraph();
                phr.Add(Environment.NewLine); //phr.Add(spacepara);
                //phr.Add(newline); phr.Add(newline);
                phr.Add(deptName);
                phr.Add(Environment.NewLine);
                //phr.Add(spacepara);
                // phr.Add(newline); phr.Add(newline);
                phr.Add(officename);
                phr.Add(Environment.NewLine);
                //phr.Add(spacepara);
                // phr.Add(newline); phr.Add(newline);
                phr.Add(officeaddress);
                phr.Add(Environment.NewLine);
                phr.Add(telphone);
                phr.Add(notel);
                phr.Add(email);
                phr.Add(emailaddress);
                phr.Add(website);
                phr.Add(webname);
                phr.Add(newline);
                phr.Add(newline);
                phr.Leading = 35;
                PdfPTable myTable = new PdfPTable(1);
                PdfPCell cellWithRowspan = new PdfPCell(phr);


                cellWithRowspan.HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER;

                myTable.AddCell(cellWithRowspan);
                myTable.SetWidthPercentage(new float[] { 550 }, PageSize.LEGAL);
                document.Add(spacer); document.Add(spacer);
                document.Add(myTable);


                document.Add(spacer);
                string imagePath = Server.MapPath("../") + "\\Goa.png";
                iTextSharp.text.Image img = iTextSharp.text.Image.GetInstance(imagePath);
                img.Alignment = Element.ALIGN_CENTER;
                img.ScaleToFit(120f, 120f);
                document.Add(img);
                document.Add(spacer); document.Add(newline);
                Font bold18 = new Font(Font.FontFamily.TIMES_ROMAN, 18, Font.BOLD);
                Chunk head = new Chunk("CERTIFICATE OF REGISTRATION", bold18);
                head.SetUnderline(0.2f, -2f);

                Paragraph headings = new Paragraph();
                headings.Add(head);
                headings.Alignment = Element.ALIGN_CENTER;

                Font italics = new Font(Font.FontFamily.TIMES_ROMAN, 14, Font.ITALIC);
                // headings.set
                document.Add(headings);
                Paragraph seerule = new Paragraph("(See Rule 5)", regular10);
                seerule.Alignment = Element.ALIGN_CENTER;
                document.Add(seerule);
                document.Add(spacer);
                Font bold16 = new Font(Font.FontFamily.TIMES_ROMAN, 16, Font.BOLD);
                Paragraph act = new Paragraph("(The Societies Registration Act, 1860)", bold16);
                act.Alignment = Element.ALIGN_CENTER;
                document.Add(act);
                Paragraph centralact = new Paragraph("(Central Act 21 of 1860)", regular10);
                centralact.Alignment = Element.ALIGN_CENTER;
                document.Add(centralact);
                document.Add(spacer);
                document.Add(spacer);

                document.Add(newline);


                ////////////////////////////////////////////////////////////////////////////////////
                Phrase regphrase = new Phrase();
                Font bold20 = new Font(Font.FontFamily.TIMES_ROMAN, 20, Font.BOLD);
                //BaseFont bfTimes = BaseFont.CreateFont(BaseFont.TIMES_ROMAN, BaseFont.CP1252, false);
                // Font times = new Font(bfTimes, 20, Font.BOLD,BaseColor.ORANGE);
                // public Font(BaseFont bf, float size, int style, BaseColor color);
                Chunk rgno = new Chunk("Registered No. ", bold20);
                Chunk regvalue = new Chunk(Result_societydetails[1], bold20);

                rgno.SetUnderline(0.1f, -2f);

                regvalue.SetUnderline(0.1f, -2f);

                regphrase.Add(rgno);
                regphrase.Add(regvalue);
                regphrase.Add(newline); regphrase.Add(newline);
                ////////////////////////////////////////////////////////////////////////////////

                PdfPTable myTablereg = new PdfPTable(1);
                PdfPCell regcell = new PdfPCell(regphrase);
                regcell.BackgroundColor = BaseColor.LIGHT_GRAY;
                myTablereg.SetWidthPercentage(new float[] { 350 }, PageSize.LEGAL);

                regcell.HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER;
                myTablereg.AddCell(regcell);

                document.Add(spacer); document.Add(spacer);

                // regcell.BackgroundColor = BaseColor.BLACK;
                document.Add(myTablereg);
                document.Add(spacer); document.Add(newline);

                document.Add(newline);
                string text = @"           It is certified that the Society ";
                Chunk beginning = new Chunk(text, italics);
                Phrase p1 = new Phrase(beginning);
                Font regular14 = new Font(Font.FontFamily.TIMES_ROMAN, 14);
                Font mybold16 = new Font(Font.FontFamily.TIMES_ROMAN, 16, Font.BOLD);
                string socname = "\"" + Result_societydetails[0].Trim() + "\"";
                Chunk c1 = new Chunk(socname.Trim(), mybold16);
                Chunk c2 = new Chunk(" has this day been duly registered under the Societies Registration Act, 1860 (Central Act 21 of 1860).", italics);
                p1.Add(c1);
                p1.Add(c2);
                Paragraph textpg = new Paragraph();
                textpg.Add(p1);
                textpg.Alignment = Element.ALIGN_JUSTIFIED;
                textpg.Leading = 22;
                textpg.IndentationLeft = 20f;
                textpg.IndentationRight = 20f;
                document.Add(textpg);

                document.Add(spacer);


                Chunk date = new Chunk("           Given under my hand this day of ", regular);
                Chunk no1 = new Chunk(DateTime.Now.Date.ToString("dd/MMMM/yyyy"), bold);

                Paragraph no = new Paragraph();
                no.Add(date);
                no.Add(no1);
                no.IndentationLeft = 20f;
                no.Add(Environment.NewLine);
                no.Add(Environment.NewLine);
                no.Add(Environment.NewLine); 
                document.Add(spacer);
                document.Add(no);


               
                Chunk c31 = new Chunk(Result_officedetails[4], bold);

                Chunk c32 = new Chunk("Inspector General of Societies");
                //Chunk c32 = new Chunk("INSPECTOR GENERAL OF SOCIETIES");
                string disname = "", place = ""; ;
                int did = Convert.ToInt32(Session["DistrictID"].ToString());
                if (did == 551)
                {
                    disname = " (North)";
                    place = "Panaji Goa";
                }
                else if (did == 552)
                {
                    place = "Margao Goa";
                    disname = " (South)";
                }
                else
                {
                    place = "";
                    disname = "";
                }
                Chunk c321 = new Chunk(disname);
                Chunk c4321 = new Chunk(place);
                Chunk c33 = new Chunk(Result_officedetails[5], regular);
                //
                


               // signature.Add(table);
                //
               // signature.Add(c31);
               
               // signature.Add(Environment.NewLine);
               // signature.Add(c32);signature.Add(c321);
               // signature.Add(Environment.NewLine);
               // signature.Add(c4321);
               // signature.Add(Environment.NewLine);
               //// signature.Add(c33);
               // signature.IndentationLeft = 50;
               // signature.Alignment = Element.ALIGN_RIGHT;

               // signature.Add(Environment.NewLine);
               // signature.Add(Environment.NewLine);
               // signature.IndentationRight = 20f;
               // document.Add(signature); signature.Add(Environment.NewLine);
               // signature.Add(Environment.NewLine);



                ///////////////////////////////////////////////////////////////////////////////////////////////////////

                PdfPTable table = new PdfPTable(2);
                // table.DefaultCell.Border = Rectangle.NO_BORDER;
              
                PdfPCell cell = new PdfPCell(new Phrase("")) { HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER, VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE, Border = Rectangle.NO_BORDER, BorderColor = BaseColor.WHITE };
                table.AddCell(cell);
                cell = new PdfPCell(new Phrase(c31)) { Border = Rectangle.NO_BORDER, BorderColor = BaseColor.WHITE };
                cell.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                table.AddCell(cell);
                cell = new PdfPCell(new Phrase("")) { Border = Rectangle.NO_BORDER, BorderColor = BaseColor.WHITE };
                table.AddCell(cell);

                cell = new PdfPCell(new Phrase(c32 + "" + c321)) { Border = Rectangle.NO_BORDER, BorderColor = BaseColor.WHITE };
                table.AddCell(cell);
                cell = new PdfPCell(new Phrase(" ")) { Border = Rectangle.NO_BORDER, BorderColor = BaseColor.WHITE };
                table.AddCell(cell);
                cell = new PdfPCell(new Phrase(c4321)) { Border = Rectangle.NO_BORDER, BorderColor = BaseColor.WHITE };
                cell.HorizontalAlignment = PdfPCell.ALIGN_CENTER;

                table.AddCell(cell);


                table.DefaultCell.Border = Rectangle.NO_BORDER;

                //table.SetWidthPercentage(new float[] { 550 }, PageSize.LEGAL);
                document.Add(table);

                Paragraph signature = new Paragraph();
                signature.Add(Environment.NewLine);
               // signature.Add(Environment.NewLine);
                document.Add(signature);
                //cell.a



                

             
                ////////////////////////////////////////////////////////////////////////////////////////////////////////


                Phrase challandetails = new Phrase(Result_paymentdetails[0], regular);
                // Chunk ch1 = new Chunk(Result_paymentdetails[1], regular10);
                Chunk ch2 = new Chunk(" vide eChallan No ", regular);
                Chunk ch3 = new Chunk(Result_paymentdetails[1] + " dated " + Result_paymentdetails[2], regular10);
                Chunk ch4 = new Chunk(" towards Processing and Registration fees.", regular);
                // challandetails.Add(ch1);
                challandetails.Add(ch2);
                challandetails.Add(ch3);
                challandetails.Add(ch4);


                PdfPTable challandata = new PdfPTable(1);
                PdfPCell challancell = new PdfPCell(challandetails);
                challancell.BackgroundColor = BaseColor.LIGHT_GRAY;
                challancell.HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER;
                challandata.AddCell(challancell);

                
                document.Add(challandata);




                /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

                var content = writer.DirectContent;
                var pageBorder = new Rectangle(document.PageSize);

                pageBorder.Left += document.LeftMargin;
                pageBorder.Right -= document.RightMargin;
                pageBorder.Top -= document.TopMargin;
                pageBorder.Bottom += document.BottomMargin;

                content.SetColorStroke(BaseColor.BLACK);
                content.Rectangle(pageBorder.Left - 2f, pageBorder.Bottom + 2f, pageBorder.Width + 4f, pageBorder.Height + 4f);
                content.SetLineWidth(5);
                content.Stroke();


                document.Close();
                fs.Close();
                writer.Close();

                byte[] bytes = System.IO.File.ReadAllBytes(Server.MapPath(@"~/OutData/" + "SocietyCertificate_" + Session["app_id"].ToString() + ".pdf"));
                Response.Clear();
                //msgModal("Please continue with digital signing of the certificate");
                Response.ContentType = "application/pdf";
                Response.AddHeader("Content-Disposition", "attachment; filename=SocietyCertificate_" + Session["app_id"].ToString() + ".pdf");
                Response.ContentType = "application/pdf";
                Response.Buffer = true;
                Response.Cache.SetCacheability(HttpCacheability.NoCache);
                Response.BinaryWrite(bytes);
                Response.End();
                Response.Close();
                //Response.Redirect("GenerateCertificate.aspx");

            }
            catch (Exception ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "genrate()" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath) + " from " + ipaddress);

            }
        }
        public void msgModal(string msg)
        {
            Label50.Text = msg;
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "none", "<script> $(window).on('load', function () { $('#msgmodal').modal({ backdrop: 'static' });});</script>", false);

        }
        protected void LkApprove_Click(object sender, EventArgs e)
        {
            try
            {
                LinkButton btn = (LinkButton)sender;
                GridViewRow row = (GridViewRow)btn.NamingContainer;
                string app_id = ((Label)gvApprove.Rows[row.RowIndex].FindControl("LbApp_id_approve")).Text;
                HiddenField2.Value = app_id;
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "none", "<script> $(window).on('load', function () { $('#ApproveSociety').modal({ backdrop: 'static' });});</script>", false);
                CheckBox1.Checked = false;

            }
            catch (Exception ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "LkApprove_Click" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
            }
        }

        protected void ImageButton2_approve_Command(object sender, CommandEventArgs e)
        {
            ImageButton btn = (ImageButton)sender;
            GridViewRow row = (GridViewRow)btn.NamingContainer;
            HiddenField hd = row.FindControl("hfechallanreceipt_approve") as HiddenField;
            if (hd.Value != null)
            {
                IMongoDatabase database;
                IMongoClient client;
                string objectid = hd.Value.ToString();
                try
                {
                    var str = ConfigurationManager.AppSettings["mongoconnect"];

                    client = new MongoClient(str);
                    database = client.GetDatabase("eGoaSociety");
                    var collection = database.GetCollection<EchallanReceipt>("eChallanReceipt");
                    var status = collection.Find(x => x._Id == ObjectId.Parse(objectid)).FirstOrDefault();
                    byte[] pdf = status.DocContent;
                    convertToPdf(pdf);
                }
                catch (Exception ex)
                {
                    CreateLogFiles Err = new CreateLogFiles();
                    Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "ImageButton2_approve_Command" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));

                    Response.Write("<script language='javascript'>alert('" + "PDF View Failed Error" + "')</script>");
                }
                finally
                {
                    client = null;
                }
            }
            else { Response.Write("<script language='javascript'>alert('" + "PDF View Failed " + "')</script>"); }
        }

        protected void BtnApproveModal_Click(object sender, EventArgs e)
        {
            Label5.Text = "";
            if (!(CheckBox1.Checked))
            {
                Label5.Visible = true;
                Label5.Text = "Please select the checkbox";
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "none", "<script> $(window).on('load', function () { $('#ApproveSociety').modal({ backdrop: 'static' });});</script>", false);
            }
            else if (HiddenField2.Value == "" || HiddenField2.Value == null)
            {
                Label5.Visible = true;
                Label5.Text = "Application ID not found";
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "none", "<script> $(window).on('load', function () { $('#ApproveSociety').modal({ backdrop: 'static' });});</script>", false);
            }
            else
            {
                Label5.Visible = false;
                string app_id = HiddenField2.Value;
                NpgsqlConnection conn = new NpgsqlConnection();
                NpgsqlCommand cmd = new NpgsqlCommand();
                conn.ConnectionString = ConfigurationManager.ConnectionStrings["connect"].ConnectionString;
                cmd.Connection = conn;
                string token = Utility.getTokenID(app_id);
                conn.Open();
                NpgsqlTransaction trans = conn.BeginTransaction();
                try
                {
                    cmd.Parameters.Clear();
                    string updquery = "UPDATE esociety.status_amendment SET app_id=@app_id,amend_status=1 WHERE login_id=@login_id";
                    cmd.CommandText = updquery;
                    cmd.Parameters.AddWithValue("@app_id", Convert.ToInt64(app_id));
                    cmd.Parameters.AddWithValue("@login_id", token);
                    cmd.ExecuteNonQuery();
                    cmd.Parameters.Clear();
                    string query1 = "Update esociety.status_table set status_id=@status_id where app_id=@app_id";
                    cmd.CommandText = query1;
                    cmd.Parameters.AddWithValue("@status_id", 11);
                    cmd.Parameters.AddWithValue("@app_id", Convert.ToInt64(app_id));
                    cmd.ExecuteNonQuery();
                    cmd.Parameters.Clear();

                    trans.Commit();
                    ApproveSocietyList();
                    GenerateCertificateList();
                    Response.Write("<script language='javascript'>alert('" + "Society Approved. Kindly generate the certificate" + "')</script>");
                }
                catch (NpgsqlException ex)
                {
                    CreateLogFiles Err = new CreateLogFiles();
                    Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "BtnApproveModal_Click()" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
                    trans.Rollback(); Label5.Visible = true; Label5.Text = "Failed";
                    //RecordUserAction("MemberDetails_Page_Load", "Exception at Member Data", "F");
                    Response.Write("<script language='javascript'>alert('" + "Exception at Generate Society List" + "')</script>");
                }
                finally
                {
                    conn.Close();
                }
            }
        }

        protected void gvApprove_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType.Equals(DataControlRowType.DataRow))
                {
                    if (e.Row.Cells[6].Text.Contains("1"))
                    {
                        string str = e.Row.Cells[4].Text;
                        str = "New";
                        e.Row.Cells[6].Text = str;

                    }
                    else if (e.Row.Cells[6].Text.Contains("2"))
                    {
                        string str = e.Row.Cells[4].Text;
                        str = "Renewal";
                        e.Row.Cells[6].Text = str;

                    }
                    else
                    {
                        string str = e.Row.Cells[6].Text;
                        str = "NA";
                        e.Row.Cells[6].Text = str;
                    }
                }
            }
            catch (Exception ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "gvApprove_RowDataBound()" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
            }
        }

         
    }
}