using GoaSocietyRegistration.Development;
using MongoDB.Bson;
using MongoDB.Driver;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WS_Encryption;

namespace GoaSocietyRegistration
{
    public partial class ViewApplicantDetails : System.Web.UI.Page
    {
        string ipaddress = Utility.getIP();
        string macaddress = Utility.GetMACAddress(Utility.getIP());
        //string macaddress = Utility.GetMACAddress();
        Insert insrt = new Insert();
        NICEncryption _encryption = new NICEncryption();
       // static string appid;
        ByteEncryption.ByteEncryption obj_Byte_Encryption = new ByteEncryption.ByteEncryption();
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Check4Tampering() || Page.RouteData.RouteHandler == null)
            {
                RecordUserAction("Page_Load", "Access request failed. Tampered session", "F");
                SessionManage session = new SessionManage();
                session.__Abandon(Request, Response);
            }
            else
            {
                if (Session["login_id"] != null)
                {
                    int isrenewal = Utility.checkifrenewal(Session["AppID"].ToString());

                    if (isrenewal == 1)
                    {
                        div_employee.Visible = false;
                        documents.Visible = true;
                        Documents_Renewal.Visible = false;
                    }
                    else if (isrenewal == 2)
                    {
                        div_employee.Visible = true;
                        documents.Visible = false;
                        Documents_Renewal.Visible = true;
                    }

                    HttpContext.Current.Response.AddHeader("Cache-Control", "no-cache, no-store, must-revalidate");
                    HttpContext.Current.Response.AddHeader("Pragma", "no-cache");
                    HttpContext.Current.Response.AddHeader("Expires", "0");
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
                        string app_query = "select applicant_name,applicant_address,applicant_mobile_no,mst_district.\"DistrictName\",";
                        app_query = app_query + " mst_memberdesignation.\"DesignationName\",applicant_email from esociety.mst_district, esociety.applicant_details, esociety.mst_memberdesignation";
                        app_query = app_query + " where mst_district.\"DistrictID\" = applicant_details.applicant_district and ";
                        app_query = app_query + " mst_memberdesignation.\"DesignationID\" = applicant_details.applicant_designation and app_id = @appid";
                        cmd.CommandText = app_query;
                        cmd.Parameters.AddWithValue("@appid", Convert.ToInt64(Session["AppID"].ToString()));
                        NpgsqlDataReader dr = cmd.ExecuteReader();
                        if (dr.Read())
                        {
                            ViewAppName.Text = Server.HtmlEncode(dr["applicant_name"].ToString());
                            appaddress.Text = Server.HtmlEncode(dr["applicant_address"].ToString());
                            appdesignation.Text = Server.HtmlEncode(dr["DesignationName"].ToString());
                            appdistrict.Text = Server.HtmlEncode(dr["DistrictName"].ToString());
                            string decrypt_mobile = Server.HtmlEncode(dr["applicant_mobile_no"].ToString());
                            appmobileno.Text = Server.HtmlEncode(Encryption.Encrypt.Decrypt(decrypt_mobile)); 
                            string decrypt_email = dr["applicant_email"].ToString();
                            if (decrypt_email == "NA")
                            {
                                appemail.Text = "";
                            }
                            else {
                                appemail.Text = Server.HtmlEncode(Encryption.Encrypt.Decrypt(decrypt_email));
                            }
                            RecordUserAction("Society_onLoad", "Society Data Loaded from DB", "S");

                        }
                        dr.Close();
                        cmd.Parameters.Clear();
                        string soc_query = "select socname,doc_one,doc_two,mst_societytype.societytype,socaddr,mst_district.\"DistrictName\",mst_taluka.\"TalukaName\",regfee,processfee,totalfee";
                        soc_query = soc_query + " from esociety.mst_societytype, esociety.society, esociety.mst_district, esociety.mst_taluka where mst_societytype.societyid = society.soctype and";
                        soc_query = soc_query + " mst_district.\"DistrictID\" = society.socdistrict and society.soc_taluka = mst_taluka.\"TalukaID\" and app_id =@appid";
                        cmd.CommandText = soc_query;
                        cmd.Parameters.AddWithValue("@appid", Convert.ToInt64(Session["AppID"].ToString()));
                        NpgsqlDataReader rd = cmd.ExecuteReader();
                        if (rd.Read())
                        {
                            value_society_type.Text = Server.HtmlEncode(dr["societytype"].ToString());
                            value_society_name.Text = Server.HtmlEncode(dr["socname"].ToString());
                            valsocietyotherdoc1.Text = Server.HtmlEncode(dr["doc_one"].ToString());
                            valsocietyotherdoc2.Text = Server.HtmlEncode(dr["doc_two"].ToString());
                            value_society_address.Text = Server.HtmlEncode(dr["socaddr"].ToString());
                            value_society_district.Text = Server.HtmlEncode(dr["DistrictName"].ToString());
                            value_taluka.Text = Server.HtmlEncode(dr["TalukaName"].ToString());
                            value_registration_fee.Text = "₹ " + Server.HtmlEncode(dr["regfee"].ToString());
                            value_processing_fee.Text = "₹ " + Server.HtmlEncode(dr["processfee"].ToString());
                            value_total_fee.Text = "₹ " + Server.HtmlEncode(dr["totalfee"].ToString());
                            if (Server.HtmlEncode(rd["doc_one"].ToString()) == "" || Server.HtmlEncode(rd["doc_one"].ToString()) == null)
                            {

                                docone.Visible = false;
                            }
                            else
                            {
                                docone.Visible = true;
                                valsocietyotherdoc1.Text =  Server.HtmlEncode(rd["doc_one"].ToString());

                            }
                            if (Server.HtmlEncode(rd["doc_two"].ToString()) == "" || Server.HtmlEncode(rd["doc_two"].ToString()) == null)
                            {

                                doctwo.Visible = false;
                            }
                            else
                            {
                                doctwo.Visible = true;
                                valsocietyotherdoc2.Text =  Server.HtmlEncode(rd["doc_two"].ToString());

                            }

                        }
                        rd.Close();
                        cmd.Parameters.Clear();
                        string mem_query = "select fname,designtaion,occupation,address,proofname,mangcomm,doc_id,document_mongoentry,member_id from esociety.members where app_id=@appid and active='Y'";
                        cmd.CommandText = mem_query;
                        cmd.Parameters.AddWithValue("@appid", Convert.ToInt64(Session["AppID"].ToString()));
                        NpgsqlDataReader rdr = cmd.ExecuteReader();
                        grvMemberDetails.DataSource = rdr;
                        grvMemberDetails.DataBind();
                        value_total_members.Text = grvMemberDetails.Rows.Count.ToString();
                        rdr.Close();

                        string AdditionalDocs_query = "SELECT object_id,docname from esociety.otherdoc_crossentry where app_id=@appid and myid=0";
                        cmd.CommandText = AdditionalDocs_query;
                        cmd.Parameters.AddWithValue("@appid", Convert.ToInt64(Session["AppID"].ToString()));
                        NpgsqlDataAdapter da1 = new NpgsqlDataAdapter(cmd);
                        DataSet ds = new DataSet();
                        da1.Fill(ds);
                        grvAdditionalDocs.DataSource = ds;
                        grvAdditionalDocs.DataBind();
                        da1.Dispose();

                        if (Convert.ToInt32(Session["Renewal"]) == 2)
                        {
                            string employee_query = "SELECT concat(employee_name,' and ' , designation) as name_desig, present_pay_scale, temporary_permanent, present_pay, dearness_allowance, special_pay,";
                            employee_query = employee_query + " other_allowance, provident_fund, other_benefits, entered_by, entered_at, ipaddress, app_id, employee_id";
                            employee_query = employee_query + " FROM esociety.society_employement where app_id = @app_id and active ='Y'";
                            cmd.CommandText = employee_query;
                            cmd.Parameters.AddWithValue("@app_id", Convert.ToInt64(Session["AppID"].ToString()));
                            NpgsqlDataReader rdr2 = cmd.ExecuteReader();
                            gv_employee.DataSource = rdr2;
                            gv_employee.DataBind();
                            value_total_employee.Text = gv_employee.Rows.Count.ToString();
                            rdr2.Close();
                                                 
                          


                        }

                        myTrans.Commit();
                    }
                    catch (NpgsqlException ex)
                    {
                        CreateLogFiles Err = new CreateLogFiles();
                        Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "Page_Load" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
                        RecordUserAction("On Load Page", ex.Message, "F");
                        Response.Write("<script language='javascript'>alert('" + " View Form Error." + "')</script>");
                    }
                    finally
                    {
                        conn.Close();
                    }

                    int exists = getmyID(Convert.ToInt64(Session["AppID"].ToString()), 7);
                    if (exists == 1)
                    {
                        listexist.Text = "Members List Available!!!";
                    }
                    else
                    {
                        listexist.Text = "Members List Not Available!!!";
                        listexist.ForeColor = System.Drawing.Color.Green;

                    }
                }
                else
                {
                    RecordUserAction("On Load Page", "View Applicant Details not Fetched beacuse of Session Id", "F");
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
                    if (Context.Session != null && Session["login_id"] != null && Session["DoTAuthTok"] != null && Request.Cookies["DoTAuthTok"] != null)
                    {
                        if (!(Session["DoTAuthTok"].ToString()).Equals(Request.Cookies["DoTAuthTok"].Value))
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
        protected void NextBtnToSociety_Click(object sender, EventArgs e)
        {
            RecordUserAction("Next Button Click", "Applicant To Society Page Next Button", "S");
            Response.Redirect("SocietyDetails.aspx");
        }
        private void RecordUserAction(string action, string description, string status)
        {
            /*Audit trail*/
            int count = 0;
            do
            {
                System.Web.HttpBrowserCapabilities browser = Request.Browser;
                UsersAuditTrails trail = new UsersAuditTrails();
                string uri = HttpContext.Current.Request.Url.AbsoluteUri;
                if (Session["login_id"] != null)
                {
                    trail.user_login_id = Server.HtmlEncode(Session["login_id"].ToString());
                    trail.loggedin_status = "Y";
                }
                else
                {

                    trail.user_login_id = DBNull.Value.ToString();
                    trail.loggedin_status = "N";
                }
                trail.browser_session_id = HttpContext.Current.Request.Cookies[GlobalVars.AntiXsrfTokenKey] == null ? HttpContext.Current.Session.SessionID : HttpContext.Current.Request.Cookies[GlobalVars.AntiXsrfTokenKey].Value;
                trail.user_session_id = Session["loginsession"] != null ? Session["loginsession"].ToString() : "null";
                trail.referrer = uri != null ? uri.ToString().Length > 100 ? uri.ToString().Substring(0, 100) : uri.ToString() : string.Empty;
                trail.accessed_module = Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath);
                trail.action_performed = action;
                trail.action_description = description.Length > 200 ? description.Substring(0, 200) : description;
                trail.action_status = status;
                trail.tracked_datetime = DateTime.Now;
                trail.loggedin_ip = ipaddress;
                trail.loggedin_mac = macaddress;
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
                count = insrt.SaveAuditTrail(trail);
            } while (count == 0);
            /*Audit trail*/
        }

        protected void ImageButton3_Command(object sender, CommandEventArgs e)
        {
            ImageButton btn = (ImageButton)sender;
            GridViewRow row = (GridViewRow)btn.NamingContainer;
            var abcd = Convert.ToInt64(Session["AppID"].ToString());       
            HiddenField h1 = row.FindControl("hfdmongodoc") as HiddenField;//cross document entry i.e. mongo object iddocument_mongoentry
            if (h1.Value != null)
            {
                string objectid = h1.Value.ToString();



                //byte[] temp = openConnectionMongo(memberID,objectid,docid);

                try
                {
                    //var str = ConfigurationManager.AppSettings["mongoconnect"];
                    //IMongoDatabase database;
                    //IMongoClient client;
                    //client = new MongoClient(str);
                    //database = client.GetDatabase("eGoaSociety");
                    //var collection = database.GetCollection<OtherDocuments>("Society Documents");

                    var str = ConfigurationManager.AppSettings["mongoconnect"];
                    IMongoDatabase database;
                    IMongoClient client;
                   // MongoClientSettings mongosettings = new MongoClientSettings();
               
                    client = new MongoClient(str);
                
                    database = client.GetDatabase("eGoaSociety");
                    var collection = database.GetCollection<MemberDocs>("Members Document");
                    var status = collection.Find(x => x._Id == ObjectId.Parse(objectid)).FirstOrDefault();
                    byte[] decrypt_bytes = status.DocContent;
                    byte[] pdf = obj_Byte_Encryption.DecryptData(decrypt_bytes, System.Web.HttpContext.Current.Server.MapPath("~/ENCRYPT.KEY"));
                    convertToPdf(pdf);
                }
                catch (MongoException ex)
                {
                    CreateLogFiles Err = new CreateLogFiles();
                    Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "ImageButton3_Command" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
                    RecordUserAction("ImageButton3_command", ex.Message, "F");
                    Response.Write("<script language='javascript'>alert('" + "View File Error. " + "')</script>");
                }
            }
            else {
                Response.Write("<script language='javascript'>alert('" + "View File Error. " + "')</script>");
            }
        }
        protected void convertToPdf(byte[] mssg)
        {
            try
            {
                byte[] tmpfiledata = mssg;
                Session["pdfpath"] = "~/OutData/" + Session["AppID"].ToString() + ".pdf";
                string sPathToSaveFileTo = Server.MapPath("~/OutData/" + Session["AppID"].ToString() + ".pdf");
                using (System.IO.FileStream fs = new System.IO.FileStream(sPathToSaveFileTo, System.IO.FileMode.Create))
                using (System.IO.BinaryWriter bw = new System.IO.BinaryWriter(fs))
                {
                    bw.Write(tmpfiledata);
                    RecordUserAction("convertToImage", "File written to Image", "S");
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
            catch(Exception ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "convertToPdf()" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
            }
            
        }

        protected void ImageButton7_Click(object sender, ImageClickEventArgs e)
        {
            Response.Redirect("DocumentUpload.aspx");
        }

        public int getmyID(Int64 appid, int myid)
        {
            int exists = 0;
            NpgsqlConnection conn = new NpgsqlConnection();
            NpgsqlCommand cmd = new NpgsqlCommand();
            conn.ConnectionString = ConfigurationManager.ConnectionStrings["connect"].ConnectionString;
            cmd.Connection = conn;
            try
            {
                conn.Open();
                string query = "select object_id  from esociety.otherdoc_crossentry where app_id=@appid and myid=@myid";
                cmd.CommandText = query;
                cmd.Parameters.AddWithValue("@appid", appid);
                cmd.Parameters.AddWithValue("@myid", myid);
                NpgsqlDataReader dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    string objectid = Server.HtmlEncode(dr["object_id"].ToString());
                    Session["ObjectID"] = objectid;
                    exists = 1;
                }
                else
                {
                    Session["loadflag"] = "stop";
                    exists = 0;
                }
                dr.Close();
            }
            catch (NpgsqlException ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "getmyID()" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
                Response.Write("<script language='javascript'>alert('" + "Not getting PDF ID" + "')</script>");
                exists = 0;
            }
            finally
            {

                conn.Close();
            }
            return exists;
        }

        protected void ImageButton10_Click(object sender, ImageClickEventArgs e)
        {
            Response.Redirect("MemberDetails.aspx");
        }

        protected void LbView_AddDocs_Click(object sender, EventArgs e)
        {
            Response.Redirect("DocumentUpload.aspx");
        }

        protected void ImageButton_employee_Command(object sender, CommandEventArgs e)
        {
            Response.Redirect("PaidEmployee.aspx");
        }
    }
}