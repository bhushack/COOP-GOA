using Npgsql;
using System;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using GoaSocietyRegistration.Development;
using MongoDB.Driver;
using MongoDB.Bson;
using WS_Encryption;
using System.Runtime.InteropServices;
using System.Net;
using System.Xml;
using System.Text.RegularExpressions;
using System.Collections.Generic;

namespace GoaSocietyRegistration.User
{
    public partial class ApplyCertifiedCopy : System.Web.UI.Page
    {
        InitiatePayment payment = new InitiatePayment();
        PayUserDetails pay = new PayUserDetails();
        string ct = string.Empty;
        Int64 fileSizeFront = 0;
        byte[] documentBinary = new Byte[0];
        string ipaddress = Utility.getIP();
        string macaddress = Utility.GetMACAddress(Utility.getIP());

        OtherDocuments doc = new OtherDocuments();
        NICEncryption _encryption = new NICEncryption();
        ByteEncryption.ByteEncryption obj_Byte_Encryption = new ByteEncryption.ByteEncryption();

        Society_Details soc = new Society_Details();
        Insert ins = new Insert();        

        //static int flag4; for certificate not required as of now

        [DllImport(@"urlmon.dll", CharSet = CharSet.Auto)]
        private extern static UInt32 FindMimeFromData(UInt32 pBC, [MarshalAs(UnmanagedType.LPStr)] String pwzUrl, [MarshalAs(UnmanagedType.LPArray)] byte[] pBuffer,
       UInt32 cbSize, [MarshalAs(UnmanagedType.LPStr)] String pwzMimeProposed, UInt32 dwMimeFlags, out UInt32 ppwzMimeOut, UInt32 dwReserverd);


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
                HttpContext.Current.Response.AddHeader("Cache-Control", "no-cache, no-store, must-revalidate");
                HttpContext.Current.Response.AddHeader("Pragma", "no-cache");
                HttpContext.Current.Response.AddHeader("Expires", "0");
                if (!IsPostBack)
                {
                    //Filldropdown();
                    loadHistory();
                    loaddetails(Session["AppID"].ToString());
                    hftotalnoofpages.Value = "0";
                    hftotalamt.Value = "0";
                    hfcertselected_flag.Value = "0";
                    Session["fir_flag"] = 0;


                }

                string activeentry = getstatus(Session["AppID"].ToString());
                string[] arr = activeentry.Split('|');
                int row = Convert.ToInt32(arr[0].ToString());  // if 0 then there is no active application
                string totalamount = arr[1].ToString();
                int status = Convert.ToInt32(arr[2].ToString());
                string guidtemp = arr[3].ToString();
                if (row == 1)
                {
                    Session["cert_guid"] = Sanitize.InputText(guidtemp);
                    gv_docs.Enabled = false;
                    fetchcertifiedcopy_docs(Session["AppID"].ToString(), Session["cert_guid"].ToString());

                    LkSave.Visible = false;
                    lblamt.Text = totalamount.ToString();
                    hftotalamt.Value = totalamount.ToString();


                    if (status == 1)
                    {
                        LkDiscard.Visible = true;
                        LkProceedtoPayment.Visible = true;
                        NpgsqlConnection conn = new NpgsqlConnection();
                        NpgsqlCommand cmd = new NpgsqlCommand();
                        conn.ConnectionString = ConfigurationManager.ConnectionStrings["connect"].ConnectionString;
                        cmd.Connection = conn;
                        conn.Open();
                        string query2 = "select applicant_mobile_no,applicant_email from esociety.applicant_details where app_id=@appid";
                        cmd.CommandText = query2;
                        cmd.Parameters.AddWithValue("@appid", Convert.ToInt64(Session["AppID"].ToString()));
                        NpgsqlDataReader dr = cmd.ExecuteReader();
                        if (dr.Read())
                        {
                            ViewState["applicant_mobile_no"] = Server.HtmlEncode(Encryption.Encrypt.Decrypt(dr["applicant_mobile_no"].ToString()));


                        }
                        dr.Close();
                        conn.Close();

                        //first fetch active challan no. if it is there 
                        string echallanstatus = null;
                        //it will check status of that challan no online 
                        var Status = getPendingeChallanDetails(Session["AppID"].ToString());

                        if (!Status.Item1.Equals(""))
                        {
                            echallanstatus = string.Empty;
                            if (!Status.Item1.Equals(""))
                            {
                                string echallanno = Status.Item1;
                                string temp_echallanstatus = payment.GetEchallanPaymentStatus(echallanno);
                                string[] Result = temp_echallanstatus.Split('|');
                                echallanstatus = Result[0];
                                int? Validity = Status.Item2;
                                if (!string.IsNullOrWhiteSpace(echallanstatus) && echallanstatus.Equals("S"))
                                {
                                    LkDiscard.Visible = false;
                                    updatePaymentDB(echallanno, Result[1]);

                                    RecordUserAction("payment_Click", "Payment Success", "S");
                                    Response.Redirect("ApplyCertifiedCopy.aspx");

                                }
                                else
                                {
                                    LkSave.Visible = false;
                                    LkDiscard.Visible = false;
                                    approvedandgotopaymentstatus.Visible = true;
                                    getPaymentGridview();
                                    paymentdone_details.Visible = false;
                                    RecordUserAction("Page_Load", "Application ready to proceed for Payment", "S");
                                }
                            }
                        }

                        else
                        {

                            approvedandgotopaymentstatus.Visible = false;
                            // getPaymentGridview();
                            LkSave.Visible = false;
                            paymentdone_details.Visible = false;
                            RecordUserAction("Page_Load", "Application ready to proceed for Payment", "S");
                        }
                    }
                    else if (status == 2)
                    {
                        checkPreviousPaymentStatus();
                        paymentdone_details.Visible = true;
                        LkSave.Visible = false;
                        LkProceedtoPayment.Visible = false;
                        LkDiscard.Visible = false;
                        approvedandgotopaymentstatus.Visible = false;
                        getpaymentdonedata(Session["AppID"].ToString());

                    }
                }
                else
                {
                    approvedandgotopaymentstatus.Visible = false;
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
                    trail.user_login_id = Session["login_id"].ToString();
                    trail.loggedin_status = "Y";
                }
                else
                {

                    trail.user_login_id = DBNull.Value.ToString();
                    trail.loggedin_status = "N";
                }
                trail.browser_session_id = HttpContext.Current.Request.Cookies[GlobalVars.AntiXsrfTokenKey] == null ? HttpContext.Current.Session.SessionID : HttpContext.Current.Request.Cookies[GlobalVars.AntiXsrfTokenKey].Value;
                trail.user_session_id = Session["loginsession"] != null ? Session["loginsession"].ToString() : "null";//check this line and correct it 
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
                count = SaveAuditTrail(trail);
            } while (count == 0);
            /*Audit trail*/
        }
        public int SaveAuditTrail(UsersAuditTrails trial)
        {
            NpgsqlConnection conn = new NpgsqlConnection();
            NpgsqlCommand cmd = new NpgsqlCommand();
            conn.ConnectionString = ConfigurationManager.ConnectionStrings["connect"].ConnectionString;
            cmd.Connection = conn;
            try
            {
                conn.Open();
                string query = "INSERT INTO esociety.user_audit_trail( user_login_id,browser_session_id, user_session_id, loggedin_status, referrer, accessed_module,";
                query = query + " action_performed, action_description, action_status, ipaddress, macaddress,  tracked_datetime, browser_name,browser_version,device_type ) VALUES (@user_login_id,@browser_session_id, ";
                query = query + " @user_session_id, @loggedin_status, @referrer, @accessed_module, @action_performed, @action_description, @action_status, @ipaddress, @macaddress,";
                query = query + "  @tracked_datetime,@browser_name,@browser_version,@device_type)";
                cmd.CommandText = query;
                cmd.Parameters.AddWithValue("@user_login_id", trial.user_login_id);
                cmd.Parameters.AddWithValue("@browser_session_id", trial.browser_session_id);
                cmd.Parameters.AddWithValue("@user_session_id", trial.user_session_id);
                cmd.Parameters.AddWithValue("@loggedin_status", trial.loggedin_status);
                cmd.Parameters.AddWithValue("@referrer", trial.referrer);
                cmd.Parameters.AddWithValue("@accessed_module", trial.accessed_module);
                cmd.Parameters.AddWithValue("@action_performed", trial.action_performed);
                cmd.Parameters.AddWithValue("@action_description", trial.action_description);
                cmd.Parameters.AddWithValue("@action_status", trial.action_status);
                cmd.Parameters.AddWithValue("@ipaddress", trial.loggedin_ip);
                cmd.Parameters.AddWithValue("@macaddress", trial.loggedin_mac);
                cmd.Parameters.AddWithValue("@tracked_datetime", trial.tracked_datetime);
                cmd.Parameters.AddWithValue("@browser_name", trial.browser_name);
                cmd.Parameters.AddWithValue("@browser_version", trial.browser_version);
                cmd.Parameters.AddWithValue("@device_type", trial.device_type);
                cmd.ExecuteNonQuery();
                return 1;
            }
            catch (NpgsqlException ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "SaveAuditTrail()" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));

                return 0;

            }
            finally
            {
                conn.Close();
            }

        }

        protected void loadHistory()
        {
            NpgsqlConnection conn = new NpgsqlConnection();
            NpgsqlCommand cmd = new NpgsqlCommand();
            conn.ConnectionString = ConfigurationManager.ConnectionStrings["connect"].ConnectionString;
            cmd.Connection = conn;
            try
            {

                conn.Open();
                string query = "select certifiedcopies.app_id, certifiedcopies.cert_guid,concat(certifiedcopies.status,'|',certifiedcopies.active) as status ,  docname, date(certifiedcopies.created_at) as applieddate,";
                query = query + " certifiedcopies_crosstable.noofcopies from esociety.certifiedcopies inner join esociety.certifiedcopies_crosstable on esociety.certifiedcopies.cert_guid = esociety.certifiedcopies_crosstable.cert_guid";
                query = query + " where certifiedcopies.app_id=@app_id order by applieddate ASC ";
                cmd.CommandText = query;
                cmd.Parameters.AddWithValue("@app_id", Convert.ToInt64(Session["AppID"].ToString()));
                using (NpgsqlDataAdapter dr = new NpgsqlDataAdapter(cmd))
                {
                    DataTable dt = new DataTable();
                    dr.Fill(dt);
                    GridviewApplicationHistory.DataSource = dt;
                    GridviewApplicationHistory.DataBind();
                }
            }
            catch (NpgsqlException ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "loadHistory()" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath) + " from " + ipaddress);
            }
            finally
            {
                conn.Close();
            }
        }

        protected void loaddetails(string appid)
        {
            if (appid != null && appid != "")
            {
                string certificate_mongo = "";
                NpgsqlConnection conn = new NpgsqlConnection();
                NpgsqlCommand cmd = new NpgsqlCommand();
                conn.ConnectionString = ConfigurationManager.ConnectionStrings["connect"].ConnectionString;
                cmd.Connection = conn;
                try
                {
                    conn.Open();

                    string socquery = "Select socname,socregid,final_certificate_mongo_entry from esociety.society where active = 'Y' and app_id=@app_id";

                    cmd.CommandText = socquery;
                    cmd.Parameters.AddWithValue("@app_id", Convert.ToInt64(appid));
                    NpgsqlDataReader dr = cmd.ExecuteReader();

                    if (dr.Read())
                    {
                        lblSocname.Text = Server.HtmlEncode(dr["socname"].ToString());
                        lblsocregno.Text = Server.HtmlEncode(dr["socregid"].ToString());
                        ViewState["socregid"] = dr["socregid"].ToString();
                        certificate_mongo = Server.HtmlEncode(dr["final_certificate_mongo_entry"].ToString());
                        hfcertmongo.Value = "";


                    }
                    dr.Close();



                    string docsquery = "";

                    if (Convert.ToInt32(Session["Renewal"]) == 1)
                    {
                        docsquery= "Select object_id, myid, docname, pagecount from esociety.otherdoc_crossentry where app_id=@app_id and (myid = 2 or myid = 3 or myid = 0) order by myid DESc";
                    }
                    else if((Convert.ToInt32(Session["Renewal"]) == 2))
                    {
                        docsquery = "Select object_id, myid, docname, pagecount from esociety.otherdoc_crossentry where app_id=@app_id and (myid = 0 or myid=9 or myid=10 or myid=11 or myid=12) order by myid DESc";
                    }
                    cmd.CommandText = docsquery;
                    cmd.Parameters.AddWithValue("@app_id", Convert.ToInt64(appid));

                    using (NpgsqlDataAdapter rdr = new NpgsqlDataAdapter(cmd))
                    {
                        DataTable dt = new DataTable();
                        rdr.Fill(dt);

                        DataRow drow = dt.NewRow();

                        drow[0] = certificate_mongo;
                        drow[1] = 100;
                        drow[2] = "Society Registration Certificate";
                        drow[3] = 1;

                        //dt.Rows.Add(drow);
                        dt.Rows.InsertAt(drow, 0);

                        dt.Columns.Add("amt", typeof(System.Int32));


                        foreach (DataRow row in dt.Rows)
                        {
                            string a = row[1].ToString();
                            if (row[1].ToString() == "100")
                            {
                                row["amt"] = 10;
                            }
                            else
                            {
                                row["amt"] = 5;
                            }

                        }


                        gv_docs.DataSource = dt;
                        gv_docs.DataBind();

                    }


                }
                catch (NpgsqlException ex)
                {
                    CreateLogFiles Err = new CreateLogFiles();
                    Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "loaddetails" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
                    Response.Write("<script language='javascript'>alert('" + "Error while loading details" + "')</script>");
                }
                finally
                {
                    conn.Close();
                }
            }
        }

        protected void chkbx_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                int totalamt = 0;
                int totalpages = 0;
                // int copies = Convert.ToInt32(ddlnoofcopies.SelectedValue.ToString());
                CheckBox chkbx = (CheckBox)sender;
                GridViewRow row = (GridViewRow)chkbx.NamingContainer;
                DropDownList ddlcopies = row.FindControl("ddlnoofcopies") as DropDownList;
                int copies = Convert.ToInt32(ddlcopies.SelectedValue.ToString());
                Label lbpagecount = row.FindControl("lblpagecount") as Label;
                Label amt = row.FindControl("lblamt") as Label;
                string myid = ((Label)row.FindControl("lblid")).Text;

                //////////////////////
                Label noofpages = row.FindControl("noofpages") as Label;
                Label amountperdoc = row.FindControl("amountperdoc") as Label;


                if (Convert.ToInt32(myid) == 100)
                {
                    if (chkbx.Checked)
                    {
                        div_fir.Visible = false;
                        hfcertselected_flag.Value = "1";
                    }
                    else
                    {
                        div_fir.Visible = false;
                        hfcertselected_flag.Value = "0";
                    }
                }



                if (chkbx.Checked)
                {

                    int nopages = Convert.ToInt32(ddlcopies.SelectedValue.ToString()) * Convert.ToInt32(lbpagecount.Text.ToString());
                    noofpages.Text = nopages.ToString();
                    int amount = nopages * Convert.ToInt32(amt.Text.ToString());
                    amountperdoc.Text = amount.ToString();

                }
                else
                {
                    noofpages.Text = "0";
                    amountperdoc.Text = "0";
                }



                foreach (GridViewRow row1 in gv_docs.Rows)
                {
                    if (row1.RowType == DataControlRowType.DataRow)
                    {
                        string amtofdoc = ((Label)gv_docs.Rows[row1.RowIndex].FindControl("amountperdoc")).Text;

                        string pages = ((Label)gv_docs.Rows[row1.RowIndex].FindControl("noofpages")).Text;

                        totalamt = totalamt + Convert.ToInt32(amtofdoc);

                        totalpages = totalpages + Convert.ToInt32(pages);

                    }
                }

                lblamt.Text = totalamt.ToString();
                hftotalamt.Value = totalamt.ToString();
                hftotalnoofpages.Value = totalpages.ToString();
            }
            catch (Exception ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "chkbx_CheckedChanged()" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath) + " from " + ipaddress);
            }



        }

        protected void ddlnoofcopies_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                int totalamt = 0;
                int totalpages = 0;

                DropDownList ddlcopies = (DropDownList)sender;
                GridViewRow row = (GridViewRow)ddlcopies.NamingContainer;
                CheckBox chkbx = row.FindControl("chkbx") as CheckBox;
                int copies = Convert.ToInt32(ddlcopies.SelectedValue.ToString());
                Label lbpagecount = row.FindControl("lblpagecount") as Label;
                Label amt = row.FindControl("lblamt") as Label;

                //////////////////////
                Label noofpages = row.FindControl("noofpages") as Label;
                Label amountperdoc = row.FindControl("amountperdoc") as Label;

                if (chkbx.Checked)
                {
                    int nopages = Convert.ToInt32(ddlcopies.SelectedValue.ToString()) * Convert.ToInt32(lbpagecount.Text.ToString());
                    noofpages.Text = nopages.ToString();
                    int amount = nopages * Convert.ToInt32(amt.Text.ToString());
                    amountperdoc.Text = amount.ToString();
                }
                else
                {
                    noofpages.Text = "0";
                    amountperdoc.Text = "0";
                }

                foreach (GridViewRow row1 in gv_docs.Rows)
                {
                    if (row1.RowType == DataControlRowType.DataRow)
                    {
                        string amtofdoc = ((Label)gv_docs.Rows[row1.RowIndex].FindControl("amountperdoc")).Text;
                        string pages = ((Label)gv_docs.Rows[row1.RowIndex].FindControl("noofpages")).Text;

                        totalamt = totalamt + Convert.ToInt32(amtofdoc);

                        totalpages = totalpages + Convert.ToInt32(pages);

                    }
                }



                lblamt.Text = totalamt.ToString();
                hftotalamt.Value = totalamt.ToString();

                hftotalnoofpages.Value = totalpages.ToString();
            }
            catch (Exception ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "ddlnoofcopies_SelectedIndexChanged()" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath) + " from " + ipaddress);
            }





        }

        protected void LbView_Click(object sender, EventArgs e)  // not used
        {
            LinkButton btn = (LinkButton)sender;
            GridViewRow row = (GridViewRow)btn.NamingContainer;
            HiddenField hdobid = row.FindControl("hfobjectID") as HiddenField;
            Label myid = row.FindControl("lblid") as Label;
            if (hdobid != null && myid != null)
            {
                string objectid = hdobid.Value;
                try
                {
                    var str = ConfigurationManager.AppSettings["mongoconnect"];
                    IMongoDatabase database;
                    IMongoClient client;
                    client = new MongoClient(str);
                    database = client.GetDatabase("eGoaSociety");
                    if (Convert.ToInt32(myid.Text) == 0)
                    {
                        var collection = database.GetCollection<OtherDocuments>("Other Documents");
                        var status = collection.Find(x => x._Id == ObjectId.Parse(objectid)).FirstOrDefault();
                        byte[] decrypt_pdf_bytes = status.DocContent;
                        byte[] pdf = obj_Byte_Encryption.DecryptData(decrypt_pdf_bytes, System.Web.HttpContext.Current.Server.MapPath("~/ENCRYPT.KEY"));
                        convertToPdf(pdf);
                    }
                    else if (Convert.ToInt32(myid.Text) == 100)
                    {
                        if (hfcertmongo.Value != "" && hfcertmongo.Value != null)
                        {
                            var collection = database.GetCollection<Society_Certificate>("Society Certificate");
                            var status = collection.Find(x => x._Id == MongoDB.Bson.ObjectId.Parse(objectid)).FirstOrDefault();
                            byte[] decrypt_pdf_bytes = status.DocContent;
                            byte[] pdf = obj_Byte_Encryption.DecryptData(decrypt_pdf_bytes, System.Web.HttpContext.Current.Server.MapPath("~/ENCRYPT.KEY"));
                            convertToPdf(pdf);
                        }
                    }
                    else
                    {
                        var collection = database.GetCollection<OtherDocuments>("Society Documents");
                        var status = collection.Find(x => x._Id == ObjectId.Parse(objectid)).FirstOrDefault();
                        byte[] decrypt_pdf_bytes = status.DocContent;
                        byte[] pdf = obj_Byte_Encryption.DecryptData(decrypt_pdf_bytes, System.Web.HttpContext.Current.Server.MapPath("~/ENCRYPT.KEY"));
                        convertToPdf(pdf);
                    }

                    RecordUserAction("LbView_Click", "Pdf Viewed", "S");
                }
                catch (MongoException ex)
                {
                    CreateLogFiles Err = new CreateLogFiles();
                    Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "LbView_Click" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath) + " from " + ipaddress);
                    RecordUserAction("LbView_Click", ex.Message, "F");
                    Response.Write("<script language='javascript'>alert('" + "Pdf Viewed Failed" + "')</script>");
                }
            }
            else
            {

                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), "DB id is null. Please Check", "LbView_Click");
                RecordUserAction("LbView_Click", "DB Id is null", "F");
                Response.Write("<script language='javascript'>alert('" + "Pdf laoding Failed" + "')</script>");

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
            catch (Exception ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "convertToPdf()" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath) + " from " + ipaddress);
            }

        }

        protected void LkSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (hftotalamt.Value == "" || hftotalamt.Value == null || hftotalamt.Value == "0")
                {
                    // call error modal
                    Label20.ForeColor = System.Drawing.Color.Red;
                    Label20.Text = "Select Document";
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "none", "<script> $(window).on('load', function () { $('#myModal').modal({ backdrop: 'static' });});</script>", false);

                }
                //else if (Convert.ToInt32(hfcertselected_flag.Value) == 1 && Convert.ToInt32(Session["fir_flag"].ToString()) != 1)
                //{
                //    Label20.ForeColor = System.Drawing.Color.Red;
                //    Label20.Text = "FIR is not uploaded";
                //    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "none", "<script> $(window).on('load', function () { $('#myModal').modal({ backdrop: 'static' });});</script>", false);

                //}
                else
                {
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "none", "<script> $(window).on('load', function () { $('#confirmModal').modal({ backdrop: 'static' });});</script>", false);



                }
            }
            catch (Exception ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "LkSave_Click" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath) + " from " + ipaddress);

            }
        }

        protected void gv_docs_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType.Equals(DataControlRowType.DataRow))
                {
                    Label myid = (e.Row.FindControl("lblid") as Label);

                    if (Convert.ToInt32(myid.Text) == 100)
                    {
                        DropDownList ddl = (DropDownList)e.Row.FindControl("ddlnoofcopies");
                        if (ddl != null)
                        {
                            ddl.Enabled = false;
                        }
                        else
                        {
                            ddl.Enabled = true;
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "gv_docs_RowDataBound" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
            }
        }


        protected string getstatus(string app_id)
        {
            string check = "";
            NpgsqlConnection conn = new NpgsqlConnection();
            NpgsqlCommand cmd = new NpgsqlCommand();
            conn.ConnectionString = ConfigurationManager.ConnectionStrings["connect"].ConnectionString;
            cmd.Connection = conn;
            conn.Open();
            try
            {
                string query = "Select totalamt,status,cert_guid FROM esociety.certifiedcopies where app_id = @app_id and active = 'Y' and status != 3";
                cmd.CommandText = query;
                cmd.Parameters.AddWithValue("@app_id", Convert.ToInt64(app_id));
                NpgsqlDataReader rd = cmd.ExecuteReader();
                if (rd.HasRows)
                {
                    if (rd.Read())
                    {
                        check = "1|" + rd["totalamt"].ToString() + "|" + rd["status"].ToString() + "|" + rd["cert_guid"].ToString();

                        ViewState["certifiedcopyfee"] = rd["totalamt"].ToString();


                    }
                    else
                    {
                        check = "0||0|0";
                    }
                }
                else
                {
                    check = "0||0|0";
                }
                rd.Close();
            }
            catch (NpgsqlException ex)
            {
                check = "0||0|0";
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, " getstatus()" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath) + " from " + ipaddress);
               
            }
            finally
            {

                conn.Close();
            }
            return check;

        }


        protected void fetchcertifiedcopy_docs(string app_id, string cert_guid)
        {

            NpgsqlConnection conn = new NpgsqlConnection();
            NpgsqlCommand cmd = new NpgsqlCommand();
            conn.ConnectionString = ConfigurationManager.ConnectionStrings["connect"].ConnectionString;
            cmd.Connection = conn;
            conn.Open();
            try
            {
                string docsquery = "Select docname, myid, noofcopies from esociety.certifiedcopies_crosstable where app_id=@app_id and cert_guid=@cert_guid and active ='Y' order by myid DESc";

                cmd.CommandText = docsquery;
                cmd.Parameters.AddWithValue("@app_id", Convert.ToInt64(app_id));
                cmd.Parameters.AddWithValue("@cert_guid", cert_guid);

                using (NpgsqlDataAdapter rdr = new NpgsqlDataAdapter(cmd))
                {
                    DataTable dt = new DataTable();
                    rdr.Fill(dt);

                    foreach (DataRow row in dt.Rows)
                    {
                        object docid = row["myid"];
                        object copies = row["noofcopies"];

                        foreach (GridViewRow row1 in gv_docs.Rows)
                        {
                            if (row1.RowType == DataControlRowType.DataRow)
                            {
                                string myid = ((Label)gv_docs.Rows[row1.RowIndex].FindControl("lblid")).Text;

                                if (Convert.ToInt32(docid) == Convert.ToInt32(myid))
                                {
                                    CheckBox chkbx = row1.FindControl("chkbx") as CheckBox;
                                    DropDownList ddl = row1.FindControl("ddlnoofcopies") as DropDownList;
                                    string pc = ((Label)gv_docs.Rows[row1.RowIndex].FindControl("lblpagecount")).Text;
                                    string amt = ((Label)gv_docs.Rows[row1.RowIndex].FindControl("lblamt")).Text;

                                    Label noofpages = row1.FindControl("noofpages") as Label;
                                    Label amountperdoc = row1.FindControl("amountperdoc") as Label;

                                    chkbx.Checked = true;
                                    ddl.SelectedValue = copies.ToString();
                                    int nopages = Convert.ToInt32(ddl.SelectedValue.ToString()) * Convert.ToInt32(pc);
                                    noofpages.Text = nopages.ToString();
                                    int amount = nopages * Convert.ToInt32(amt);
                                    amountperdoc.Text = amount.ToString();

                                }

                            }
                        }


                    }

                }
            }
            catch (NpgsqlException ex)
            {

                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, " fetchcertifiedcopy_docs()" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath) + " from " + ipaddress);
                RecordUserAction("Read", ex.Message, "F");
            }
            finally
            {

                conn.Close();
            }


        }

        protected void LkDiscard_Click(object sender, EventArgs e)
        {            
           ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "none", "<script> $(window).on('load', function () { $('#deleteconfirmation').modal({ backdrop: 'static' });});</script>", false);

        }

        protected void btnRedirect_Click(object sender, EventArgs e)
        {
            Response.Redirect("ApplyCertifiedCopy.aspx");
        }

        protected void GridviewApplicationHistory_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType.Equals(DataControlRowType.DataRow))
                {
                    
                    string combine = e.Row.Cells[5].Text;
                    string[] result = combine.Split('|');
                    string status = result[0].ToString();
                    string active = result[1].ToString();
                    if (status == "3" && active == "D")
                    {
                        e.Row.Cells[5].Text = "Cancelled by User";
                    }
                    else if (status == "3" && active == "C")
                    {
                        e.Row.Cells[5].Text = "Issued by Department";
                    }
                    else if (status == "1" && active == "Y")
                    {
                        e.Row.Cells[5].Text = "Active at User Side";
                    }
                    else if (status == "2" && active == "Y")
                    {
                        e.Row.Cells[5].Text = "Payment done and Application Ready at Organisation Side";
                    }
                    
                }
            }
            catch (Exception ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "GridviewApplicationHistory_RowDataBound" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath) + " from " + ipaddress);

            }
        }

        protected void deleteappln_modalbtn_Click(object sender, EventArgs e)
        {

            NpgsqlConnection conn = new NpgsqlConnection();
            NpgsqlCommand cmd = new NpgsqlCommand();
            conn.ConnectionString = ConfigurationManager.ConnectionStrings["connect"].ConnectionString;
            cmd.Connection = conn;
            conn.Open();
            NpgsqlTransaction trans = conn.BeginTransaction();
            try
            {
                string query = "UPDATE esociety.certifiedcopies SET active = 'D', status = 4, deleted_at = current_timestamp WHERE app_id = @app_id and active = 'Y'";
                cmd.CommandText = query;
                cmd.Parameters.AddWithValue("@app_id", Convert.ToInt64(Session["AppID"].ToString()));
                cmd.ExecuteNonQuery();

                string query1 = "UPDATE esociety.certifiedcopies_crosstable SET active = 'D' WHERE app_id = @app_id and active = 'Y'";
                cmd.CommandText = query1;
                cmd.Parameters.AddWithValue("@app_id", Convert.ToInt64(Session["AppID"].ToString()));
                cmd.ExecuteNonQuery();
                trans.Commit();
                RecordUserAction("Update", "deleteappln_modalbtn_Click Certifiedcopy list deleted", "S");
                Label65.Text = "Deleted succcessfully";
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "none", "<script> $(window).on('load', function () { $('#redirectModal').modal({ backdrop: 'static' });});</script>", false);



            }
            catch (NpgsqlException ex)
            {
                trans.Rollback();
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "deleteappln_modalbtn_Click()" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
                Response.Write("<script language='javascript'>alert('" + "Error while Deleting" + "')</script>");

            }
            finally
            {
                conn.Close();
            }
        }

        protected void confirmmodalbutton_Click(object sender, EventArgs e)
        {
            string objguid = Guid.NewGuid().ToString();
            Session["cert_guid"] = objguid;
            int district = Utility.get_districtid(Session["AppID"].ToString());

            NpgsqlConnection connect = new NpgsqlConnection();
            NpgsqlCommand cmd = new NpgsqlCommand();
            connect.ConnectionString = ConfigurationManager.ConnectionStrings["connect"].ConnectionString;
            cmd.Connection = connect;
            connect.Open();
            NpgsqlTransaction trans = connect.BeginTransaction();
            try
            {
                int selectitemcount = 0;
                foreach (GridViewRow row in gv_docs.Rows)
                {
                    if (row.RowType == DataControlRowType.DataRow)
                    {
                        CheckBox chkbx = row.FindControl("chkbx") as CheckBox;
                        string copies = ((DropDownList)gv_docs.Rows[row.RowIndex].FindControl("ddlnoofcopies")).SelectedValue.ToString();
                        string myid = ((Label)gv_docs.Rows[row.RowIndex].FindControl("lblid")).Text;
                        string docname = ((Label)gv_docs.Rows[row.RowIndex].FindControl("lbldocname")).Text;

                        if (chkbx.Checked == true)
                        {
                            string query = "";
                            query = "INSERT INTO esociety.certifiedcopies_crosstable( app_id, cert_guid, docname, myid, noofcopies, active, created_at, created_by, ipaddress, macaddress)";
                            query = query + " VALUES(@app_id,@cert_guid,@docname,@myid,@noofcopies,'Y',current_timestamp,@created_by,@ipaddress,@macaddress)";
                            cmd.CommandText = query;
                            cmd.Parameters.AddWithValue("@app_id", Convert.ToInt64(Session["AppID"].ToString()));
                            cmd.Parameters.AddWithValue("@cert_guid", Session["cert_guid"].ToString());
                            cmd.Parameters.AddWithValue("@docname", docname);
                            cmd.Parameters.AddWithValue("@myid", Convert.ToInt32(myid));
                            cmd.Parameters.AddWithValue("@noofcopies", Convert.ToInt32(copies));
                            cmd.Parameters.AddWithValue("@created_by", Session["created_by"].ToString());
                            cmd.Parameters.AddWithValue("@ipaddress", ipaddress);
                            cmd.Parameters.AddWithValue("@macaddress", macaddress);
                            cmd.ExecuteNonQuery();
                            cmd.Parameters.Clear();

                            selectitemcount++;
                        }

                    }
                }

                if (selectitemcount > 0)
                {
                    string query1 = "";
                    query1 = "INSERT INTO esociety.certifiedcopies(app_id, cert_guid, districtid, active, status, totalamt, created_at, created_by, ipaddress, socregid, socname)";
                    query1 = query1 + " VALUES(@app_id,@cert_guid,@districtid,@active,@status,@totalamt,current_timestamp,@created_by,@ipaddress, @socregid, @socname)";
                    cmd.CommandText = query1;
                    cmd.Parameters.AddWithValue("@app_id", Convert.ToInt64(Session["AppID"].ToString()));
                    cmd.Parameters.AddWithValue("@cert_guid", Session["cert_guid"].ToString());
                    cmd.Parameters.AddWithValue("@districtid", district);
                    cmd.Parameters.AddWithValue("@active", 'Y');
                    cmd.Parameters.AddWithValue("@status", 1);
                    cmd.Parameters.AddWithValue("@totalamt", Convert.ToInt32(hftotalamt.Value));
                    cmd.Parameters.AddWithValue("@created_by", Session["created_by"].ToString());
                    cmd.Parameters.AddWithValue("@ipaddress", ipaddress);
                    cmd.Parameters.AddWithValue("@created_by", Session["created_by"].ToString());
                    cmd.Parameters.AddWithValue("@ipaddress", ipaddress);
                    cmd.Parameters.AddWithValue("@socregid", lblsocregno.Text.ToString());
                    cmd.Parameters.AddWithValue("@socname", lblSocname.Text.ToString());
                    cmd.ExecuteNonQuery();
                    cmd.Parameters.Clear();
                }

                trans.Commit();
                gv_docs.Enabled = false;
                LkSave.Visible = false;
                LkDiscard.Visible = true;
                LkProceedtoPayment.Visible = true;                
                RecordUserAction("Insert", "Certified Copy List Saved", "S");
                Label20.ForeColor = System.Drawing.Color.Green;
                Label20.Text = "Saved Successfully";
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "none", "<script> $(window).on('load', function () { $('#myModal').modal({ backdrop: 'static' });});</script>", false);


            }
            catch (NpgsqlException ex)
            {
                trans.Rollback();
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "confirmmodalbutton_Click()" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
                Response.Write("<script language='javascript'>alert('" + "Exception while Saving" + "')</script>");
               
            }
            finally
            {
                connect.Close();
            }



        }

        public Tuple<string, int?, string> getPendingeChallanDetails(string appid)
        {
            // onlinepaymentid for Certfied copies is 3
            var tuple = new Tuple<string, int?, string>("", null, "");
            NpgsqlConnection conn = new NpgsqlConnection();
            NpgsqlCommand cmd = new NpgsqlCommand();
            conn.ConnectionString = ConfigurationManager.ConnectionStrings["connect"].ConnectionString;
            cmd.Connection = conn;
            try
            {
                conn.Open();
                string srh_query = "select online_payment_details.echallan_no,online_payment_details.echallangeneratedon,online_payment_details.status from esociety.online_payment_details inner join esociety.certifiedcopies on esociety.online_payment_details.echallan_no =  esociety.certifiedcopies.echallan_no";
                srh_query = srh_query + " where online_payment_details.app_id =@app_id and online_payment_details.active='Y' and online_payment_details.onlinepayment_id=@onlinepayment_id and certifiedcopies.cert_guid = @guid";
                cmd.CommandText = srh_query;
                cmd.Parameters.AddWithValue("@app_id", Convert.ToInt64(appid));
                cmd.Parameters.AddWithValue("@onlinepayment_id", 3);
                cmd.Parameters.AddWithValue("@guid", Session["cert_guid"].ToString());
                NpgsqlDataReader rd = cmd.ExecuteReader();
                if (rd.Read())
                {
                    string echallanno = Server.HtmlEncode(rd["echallan_no"].ToString());
                    DateTime temp = (DateTime)rd["echallangeneratedon"];
                    string status = Server.HtmlEncode(rd["status"].ToString());
                    DateTime TodaysDate = DateTime.Now.Date;
                    int? d3 = (int?)(TodaysDate - temp).TotalDays;
                    tuple = new Tuple<string, int?, string>(echallanno, d3, status);
                    
                }
                else
                {
                    tuple = new Tuple<string, int?, string>("", null, "");
                     
                }
                rd.Close();
            }
            catch (NpgsqlException ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "getPendingeChallanDetails()" + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
                tuple = new Tuple<string, int?, string>("", null, "");
                
            }
            finally
            {
                conn.Close();

            }
            return tuple;
        }

        public void updatePaymentDB(string echallan_no, string plain_PayStatusResponse)
        {
            if (Session["AppID"] != null)
            {
                XmlDocument PayStatusXML = new XmlDocument();
                PayStatusXML.XmlResolver = null;
                PayStatusXML.LoadXml(plain_PayStatusResponse);
                string eChallanStatus = PayStatusXML.GetElementsByTagName("status").Item(0).InnerText.Trim();
                string total_amt = PayStatusXML.GetElementsByTagName("totalAmount").Item(0).InnerText.Trim();
                string bank_ref_no = PayStatusXML.GetElementsByTagName("sbiReferenceNo").Item(0).InnerText.Trim();
                string bank_rcvd_date = PayStatusXML.GetElementsByTagName("bankReceiveDate").Item(0).InnerText.Trim();
                string treasury_rcvd_date = PayStatusXML.GetElementsByTagName("treasuryReceiveDate").Item(0).InnerText.Trim();
                EchallanReceipt echallanreceipt = new EchallanReceipt();
                NpgsqlConnection conn = new NpgsqlConnection();
                NpgsqlCommand cmd = new NpgsqlCommand();
                conn.ConnectionString = ConfigurationManager.ConnectionStrings["connect"].ConnectionString;
                cmd.Connection = conn;
                conn.Open();
                NpgsqlTransaction myTrans = conn.BeginTransaction();
                cmd.Transaction = myTrans;
                try
                {
                    cmd.Parameters.Clear();
                    string echallan_rcpt = Utility.get_echallan_receiptDocsID();//eReceipt doc ID
                    ObjectId obj_id = ObjectId.GenerateNewId();
                    string update_challan_status_query = "update esociety.online_payment_details set paystat_response_xml=@paystat_response_xml, status=@status, total_amt=@total_amt,";
                    update_challan_status_query = update_challan_status_query + " bank_ref_no = @bank_ref_no, bank_rcvd_date=@bank_rcvd_date, treasury_rcvd_date=@treasury_rcvd_date,";
                    update_challan_status_query = update_challan_status_query + " echallan_RCPT_cross_entry=@echallan_RCPT_cross_entry, echallanrcpt_doc_id=@echallanrcpt_doc_id where echallan_no =@echallan_no";
                    cmd.CommandText = update_challan_status_query;
                    cmd.Parameters.AddWithValue("@paystat_response_xml", plain_PayStatusResponse);
                    cmd.Parameters.AddWithValue("@status", eChallanStatus);
                    cmd.Parameters.AddWithValue("@total_amt", Convert.ToDouble(total_amt));
                    cmd.Parameters.AddWithValue("@bank_ref_no", bank_ref_no);
                    cmd.Parameters.AddWithValue("@bank_rcvd_date", bank_rcvd_date);
                    cmd.Parameters.AddWithValue("@treasury_rcvd_date", treasury_rcvd_date);
                    cmd.Parameters.AddWithValue("@echallan_no", echallan_no);
                    cmd.Parameters.AddWithValue("@echallan_RCPT_cross_entry", obj_id.ToString());
                    cmd.Parameters.AddWithValue("@echallanrcpt_doc_id", echallan_rcpt);
                    cmd.ExecuteNonQuery();
                    cmd.Parameters.Clear();
                   
                    string query1 = "Update esociety.certifiedcopies set status= @status, echallan_no=@echallan_no WHERE app_id = @app_id and active = 'Y'";
                    cmd.CommandText = query1;
                    cmd.Parameters.AddWithValue("@status", 2);
                    cmd.Parameters.AddWithValue("@echallan_no", echallan_no);
                    cmd.Parameters.AddWithValue("@app_id", Convert.ToInt64(Session["AppID"].ToString()));
                    cmd.ExecuteNonQuery();
                    cmd.Parameters.Clear();

                    LkProceedtoPayment.Visible = false;       
                           
                    byte[] pdf = payment.print_ereceipt(echallan_no);
                    if (pdf != null)
                    {
                        echallanreceipt.Active = true;
                        echallanreceipt.App_ID = Convert.ToInt64(Session["AppID"].ToString());
                        echallanreceipt.DocContent = pdf;
                        echallanreceipt.Doc_CT = "application/pdf";
                        echallanreceipt.Doc_ID = echallan_rcpt;
                        echallanreceipt.doc_name = echallan_no;
                        echallanreceipt.IpAddress = ipaddress;
                        echallanreceipt.MacAddress = macaddress;
                        echallanreceipt.time_stamp = DateTime.Now.ToString();
                        echallanreceipt._Id = obj_id;
                        echallanreceipt.UpdatedBy = Session["created_by"].ToString();
                        int value = InsertintoMongoDB(echallanreceipt, "eChallanReceipt");
                        if (value == 1)
                        {
                            myTrans.Commit();
                        }
                        else
                        {
                            myTrans.Rollback();
                            RecordUserAction("updatePaymentDB ", "Transaction Rollback mongo saving failed", "F");
                        }
                    }
                    else
                    {
                        myTrans.Rollback();
                    }

                    
                }
                catch (NpgsqlException ex)
                {
                    CreateLogFiles Err = new CreateLogFiles();
                    Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "updatePaymentDB()" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
                    myTrans.Rollback();
                    RecordUserAction("updatePaymentDB ", ex.Message, "F");
                    Response.Write("<script language='javascript'>alert('" + "Saving Failed" + "')</script>");
                }
                finally
                {
                    conn.Close();
                }
            }
            else
            {
                RecordUserAction("updatePaymentDB", "Session null ", "F");
            }

        }

        public int InsertintoMongoDB(EchallanReceipt rcpt, string sel_collection)
        {
            Insert insr = new Insert();
            try
            {
                RecordUserAction("InsertintoMongoDB", "write document to Mongo", "S");
                return insr.InsertMongoEchallanReceipt(rcpt, sel_collection);
            }
            catch (MongoException ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "InsertintoMongoDB()" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
                RecordUserAction("InsertintoMongoDB", ex.Message, "F");
                // lb.Text = ex.Message;
                return 0;
            }
        }
        protected void check_btnpaymentstatus_Click()
        {
            try
            {
                if (Session["AppID"] != null)
                {

                    var Status = getPendingeChallanDetails(Session["AppID"].ToString());
                    string echallanstatus = string.Empty;
                    if (!Status.Item1.Equals(""))
                    {
                        string echallanno = Status.Item1;
                        Session["plainechallanno"] = echallanno;
                        //upper two lines for live and below for testing
                        //string echallanno = "201901138292";
                        //Session["plainechallanno"] = "201901138292";
                        string temp_result = payment.GetEchallanPaymentStatus(echallanno);
                        string[] Result = temp_result.Split('|');
                        echallanstatus = Result[0];
                        if (!string.IsNullOrWhiteSpace(echallanstatus))
                        {
                            string xmlstatus = Result[1];
                            if (echallanstatus.Equals("S"))
                            {
                                RecordUserAction("UpdatePaymentBtn", "Payment Success. Updating DB..", "S");
                                updatePaymentDB(echallanno, xmlstatus);
                                //LinkButton Lb1 = (LinkButton)Master.FindControl("LinkButton1");
                                //Lb1.Visible = true;
                                LinkButton14.Enabled = true;
                                LinkButton14.Visible = true;
                                ShowUserAlert("Payment of <b style='color:red'>₹ " + "amount" + "/-</b> against eChallan No <b style='color:red'>" + echallanno + "</b> and Receipt No <b style='color:red'>" + "recipt no" + "</b> is already been done", "Refresh");
                                //btnpaymentstatus.Visible = false;

                            }
                            else if (echallanstatus.Equals("F"))
                            {   //not required to updatePaymentDB
                                //updatePaymentDB(echallanno, xmlstatus);
                                RecordUserAction("UpdatePaymentBtn", "Payment Failed. Db not updating only for Trial..", "F");
                                ShowUserAlert("Payment of <b style='color:red'>₹ " + "amount" + "/-</b> against eChallan No <b style='color:red'>" + echallanno + "</b> and Receipt No <b style='color:red'>" + "recipt no" + "</b> is failed. Please Click on Make Payment to Initiate", "Cancel");
                                LkProceedtoPayment.Enabled = true;
                            }
                            else if (echallanstatus.Equals("P"))
                            {
                                RecordUserAction("UpdatePaymentBtn", "Payment pending. Check Again..", "S");
                                var message = "eChallan with No. " + echallanno + " for ₹" + "amount" + " (" + Session["AppID"].ToString() + ") returned status "
                                + "Payment is under process.<br/>If the status is Pending and your bank account has been debited, please check the status "
                                + "again later. If your bank account has not been debited, you may re-initiate the payment "
                                + "<a href='https://echallanpg/haveechallan.aspx' rel='noopener noreferrer' target='_blank'>HERE</a>";
                                ShowUserAlert(message, "Cancel");

                            }
                            else { }

                        }
                        else
                        {
                            ShowUserAlert("Invalid transaction (not found in e-Challan)", "Cancel");
                        }
                    }
                    else
                    {
                        ShowUserAlert("Invalid transaction (not found in e-Challan)", "Cancel");

                    }
                }
                else
                {
                    RecordUserAction("check_btnpaymentstatus_Click", "Session null ", "F");
                }
            }
            catch (Exception ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "check_btnpaymentstatus_Click()" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
            }
        }

        protected void getPaymentGridview()
        {
            NpgsqlConnection conn = new NpgsqlConnection();
            NpgsqlCommand cmd = new NpgsqlCommand();
            conn.ConnectionString = ConfigurationManager.ConnectionStrings["connect"].ConnectionString;
            cmd.Connection = conn;
            try
            {
                string query = "select online_payment_details.app_id,online_payment_details.echallan_no,online_payment_details.echallangeneratedon,online_payment_details.status from esociety.online_payment_details";
                query = query + " INNER JOIN esociety.certifiedcopies on certifiedcopies.echallan_no = online_payment_details.echallan_no where online_payment_details.app_id = @app_id";
                query = query + " and online_payment_details.onlinepayment_id = 3 and certifiedcopies.cert_guid = @guid";
                cmd.CommandText = query;
                cmd.Parameters.AddWithValue("@app_id", Convert.ToInt64(Session["AppID"].ToString()));
                cmd.Parameters.AddWithValue("@guid", Session["cert_guid"].ToString());
                using (NpgsqlDataAdapter dr = new NpgsqlDataAdapter(cmd))
                {
                    DataTable dt = new DataTable();
                    dr.Fill(dt);
                    gridviewPayment.DataSource = dt;
                    gridviewPayment.DataBind();
                }
            }
            catch (NpgsqlException ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "getPaymentGridview()" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath) + " from " + ipaddress);
               
                Response.Write("<script language='javascript'>alert('" + "Failed, Please try again." + "')</script>");
            }
            finally
            {
                conn.Close();
            }
        }

        protected void LkProceedtoPayment_Click(object sender, EventArgs e)
        {
            try
            {
                if (Session["AppID"] != null)
                {
                    soc = ins.FetchSociety(Session["AppID"].ToString(), 3, 3);
                    string soctaluka = Utility.getTaluka(soc.soc_taluka);
                    RecordUserAction("PaymentBtnClick", "Proceed to Payment ", "S");
                    bool savePtEntry = false, saveEchallanEntry = false;
                    string recpt_no = "", echallanNo = "";
                    string echallanstatus;

                    //user details object            
                    pay.app_id = Session["AppID"].ToString();
                    pay.Address = Server.HtmlEncode(soc.socaddr);
                    pay.amount = Server.HtmlEncode(ViewState["certifiedcopyfee"].ToString());
                    pay.Applicant = Server.HtmlEncode(soc.socname);                   
                    string mobile = ViewState["applicant_mobile_no"].ToString(); 
                    pay.MobileNo = mobile;
                    pay.Pincode = Server.HtmlEncode(soc.pincode);
                    pay.societyName = Server.HtmlEncode(soc.socname);
                    pay.taluka = soctaluka;
                    pay.TotalAmt = Server.HtmlEncode(ViewState["certifiedcopyfee"].ToString());
                    Session["Totalamt_certcopy"] = pay.TotalAmt;
                    if (!pay.TotalAmt.Equals("0"))
                    {
                        var Status = getPendingeChallanDetails(Session["AppID"].ToString());
                        if (!Status.Item1.Equals(""))
                        {
                            echallanstatus = string.Empty;
                            if (!Status.Item1.Equals(""))
                            {
                                string echallanno = Status.Item1;
                                Session["plainechallanno"] = echallanno;
                                string temp_echallanstatus = payment.GetEchallanPaymentStatus(echallanno);
                                string[] Result = temp_echallanstatus.Split('|');
                                echallanstatus = Result[0];
                                int? Validity = Status.Item2;
                                //get detials from echllanno
                                //receipt no change here                               
                                recpt_no = "SOC/" + Session["AppID"].ToString();

                                if (!string.IsNullOrWhiteSpace(echallanstatus))
                                {
                                    if (echallanstatus.Equals("S"))
                                    {
                                        ShowUserAlert("Payment of <b style='color:red'>₹ " + Convert.ToInt32(Session["Totalamt_certcopy"].ToString()) + "/-</b> against eChallan No <b style='color:red'> " + echallanno + " </b> and Receipt No <b style='color:red'> " + recpt_no + " </b> is already been done", "Cancel");
                                        check_btnpaymentstatus_Click();
                                        LkProceedtoPayment.Visible = false;

                                    }
                                    else if (echallanstatus.Equals("F"))
                                    {
                                        //if (Validity <= 7)
                                        //    ShowUserAlert("Valid eChallan with No <b style='color:red'> " + echallanno + " </b> exists for <b style='color:red'> " + Session["AppID"].ToString() + " </b>, with an amount of <b style='color:red'>₹" + Convert.ToInt32(Session["Totalamt"].ToString()) + "/-</b>. Click Confirm to proceed with online payment", "Confirm");
                                        //else
                                        //{
                                        int result = SoftDeleteEchallanEntry(echallanno, echallanstatus);
                                        if (result > 0)
                                        {
                                            savePtEntry = false;
                                            saveEchallanEntry = true;
                                        }
                                        //}
                                    }
                                    else if (echallanstatus.Equals("P"))
                                    {
                                        if (Validity > 7)
                                        {
                                            int result = SoftDeleteEchallanEntry(echallanno, echallanstatus);
                                            if (result > 0)
                                            {
                                                savePtEntry = false;
                                                saveEchallanEntry = true;
                                            }
                                        }
                                        else
                                        {
                                            var message = "eChallan with No. " + echallanno + " for ₹" + Convert.ToInt32(Session["Totalamt_certcopy"].ToString()) + " (" + Session["AppID"].ToString() + ") returned status "
                                            + "Payment is under process.<br/>If the status is Pending and your bank account has been debited, please check the status "
                                            + "again later. If your bank account has not been debited, you may re-initiate the payment "
                                            + "<a href='https://egov.goa.nic.in/echallanpg/haveechallan.aspx' rel='noopener noreferrer' target='_blank'>HERE</a>";
                                            ShowUserAlert(message, "Cancel");
                                        }
                                    }
                                }
                                else
                                {
                                    if (Validity > 7)
                                    {
                                        int result = SoftDeleteEchallanEntry(echallanno, "E");
                                        if (result > 0)
                                        {
                                            savePtEntry = false;
                                            saveEchallanEntry = true;
                                        }
                                    }
                                    else
                                        ShowUserAlert("eChallan No.<b style='color:red'>" + echallanno + "</b> exist for Application ID:<b style='color:red'> " + Session["AppID"].ToString() + " </b>, Amount:<b style='color:red'>" + Convert.ToInt32(Session["Totalamt_certcopy"].ToString()) + "/-</b>.do you want to continue with this eChallan No. Confirm to proceed", "Confirm");
                                }
                            }
                        }
                        //if pending challan not exists then proceed to new challan and payment process
                        else { savePtEntry = true; saveEchallanEntry = true; }
                    }
                    else
                        ShowUserAlert("Cannot proceed.Amount Should be greater than 0!!", "Cancel");
                    //new challan and payment process starts from here
                    if (savePtEntry || saveEchallanEntry)
                    {
                        InitiateOnlinePayment(pay);
                        echallanNo =  (Session != null && Session["plainechallanno"] != null) ? Convert.ToString(Session["plainechallanno"]) : "";
                        if (!echallanNo.Equals(""))
                            ShowUserAlert("For Payment of <b style='color:red'>₹" + Convert.ToInt32(Session["Totalamt_certcopy"].ToString()) + "/-</b> with eChallan No <b style='color:red'> " + echallanNo + " </b> and Receipt No <b style='color:red'> " + recpt_no + " </b> you will be redirected to Payment Gateway. Click Confirm to proceed", "Confirm");
                        else Response.Write("<script language='javascript'>alert('" + "Please note : Payment server not responded" + "')</script>");
                    }
                }
                else
                {
                    RecordUserAction("btnPayment_Click", "Session null ", "F");
                }
            }
            catch (Exception ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "btnPayment_Click" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
            }
        }

        private void ShowUserAlert(string message, string ConfirmCancel)
        {
            try
            {
                if (ConfirmCancel.Equals("Confirm"))
                {
                    lblMSG1.Text = message;
                    RedirecttoLoginBtn.Visible = true;
                    errorpage.Visible = false;
                    RedirecttoLoginBtn.Text = "Confirm";
                    btnCancel.Text = "Cancel";
                    btnrefresh.Visible = false;
                    //ViewChallan.Visible = true;
                    // ViewChallan.Text = "View eChallan";
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "none", "<script> $(window).on('load', function () { $('#alertmodal').modal({ backdrop: 'static' });});</script>", false);
                }
                else if (ConfirmCancel.Equals("Cancel"))
                {
                    lblMSG1.Text = message;
                    RedirecttoLoginBtn.Visible = false;
                    errorpage.Visible = false;
                    btnCancel.Text = "Okay";
                    btnrefresh.Visible = false;
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "none", "<script> $(window).on('load', function () { $('#alertmodal').modal({ backdrop: 'static' });});</script>", false);
                }
                else if (ConfirmCancel.Equals("Refresh"))
                {
                    lblMSG1.Text = message;
                    RedirecttoLoginBtn.Visible = false;
                    errorpage.Visible = false;
                    btnCancel.Text = "Okay";
                    btnCancel.Visible = false;
                    btnrefresh.Visible = true;
                    btnrefresh.Text = "Ok";
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "none", "<script> $(window).on('load', function () { $('#alertmodal').modal({ backdrop: 'static' });});</script>", false);
                }
                else if (ConfirmCancel.Equals("Re-direct"))
                {
                    lblMSG1.Text = message;
                    RedirecttoLoginBtn.Visible = false;
                    btnCancel.Visible = false;
                    errorpage.Visible = true;
                    errorpage.Text = "Cancel";
                    btnrefresh.Visible = false;
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "none", "<script> $(window).on('load', function () { $('#alertmodal').modal({ backdrop: 'static' });});</script>", false);
                }
            }
            catch (Exception ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "ShowUserAlert()" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
            }
        }

        protected void lkUpdateStatus_Click(object sender, EventArgs e)
        {
                LinkButton btn = (LinkButton)sender;
                GridViewRow row = (GridViewRow)btn.NamingContainer;
                string app_id = ((Label)gridviewPayment.Rows[row.RowIndex].FindControl("lbpaymentapp_id")).Text;
                string echallanno = ((Label)gridviewPayment.Rows[row.RowIndex].FindControl("lbechallan")).Text;
                ///////////////////
                string temp_echallanstatus = payment.GetEchallanPaymentStatus(echallanno);
                string[] Result = temp_echallanstatus.Split('|');
                if (!string.IsNullOrWhiteSpace(Result[0]))
                {
                    if (Result[0].Equals("S"))
                    {
                        updatePaymentDB(echallanno, Result[1]);
                        RecordUserAction("payment_Click", "Payment Success", "S");
                    }
                    if (Result[0].Equals("F"))
                    {
                        updatePaymentDBFail(echallanno, Result[1]);
                        RecordUserAction("payment_Click", "Payment Failed", "S");
                    }
                }


                getPaymentGridview();
                Response.Redirect("ApplyCertifiedCopy.aspx");
           
        }

        protected void CheckBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (CheckBox1.Checked == true)
            {
                RecordUserAction("payment_Click", "Checkbox clicked" + Session["AppID"].ToString(), "S");
                string echallan = hdechallan.Value;
                if (echallan != null)
                {
                    string temp_echallanstatus = payment.GetEchallanPaymentStatus(echallan);
                    string[] Result = temp_echallanstatus.Split('|');
                    if (!string.IsNullOrWhiteSpace(Result[0]))
                    {
                        if (Result[0].Equals("S"))
                        {
                            updatePaymentDB(echallan, Result[1]);
                            RecordUserAction("payment_Click", "Payment Success" + Session["AppID"].ToString(), "S");
                            lkdeleteoldechallan.Enabled = false;
                            Response.Redirect("ApplyCertifiedCopy.aspx");
                        }
                        else if (Result[0].Equals("P"))
                        {
                            Label45.Text = "Please wait Your payment status is Pending. Do not delete echallan if you already paid or amount is deducted from your account.";
                            lkdeleteoldechallan.Enabled = true;
                            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "none", "<script> $(window).on('load', function () { $('#deletechallanModal').modal({ backdrop: 'static' });});</script>", false);
                        }
                        else
                        {
                            lkdeleteoldechallan.Enabled = true;
                            RecordUserAction("eChallandelete", "Challan delete checkbox agreed by user", "S");
                            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "none", "<script> $(window).on('load', function () { $('#deletechallanModal').modal({ backdrop: 'static' });});</script>", false);

                        }
                    }
                    else
                    {
                        lkdeleteoldechallan.Enabled = true;
                        RecordUserAction("eChallandelete", "Challan delete checkbox agreed by user", "S");
                        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "none", "<script> $(window).on('load', function () { $('#deletechallanModal').modal({ backdrop: 'static' });});</script>", false);

                    }
                }
            }
            else
            {
                lkdeleteoldechallan.Enabled = false;
                RecordUserAction("payment_Click", "Checkbox unchecked" + Session["AppID"].ToString(), "S");
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "none", "<script> $(window).on('load', function () { $('#deletechallanModal').modal({ backdrop: 'static' });});</script>", false);
            }
        }

        protected void lkdeleteoldechallan_Click(object sender, EventArgs e)
        {
            foreach (GridViewRow row in gridviewPayment.Rows)
            {
                string modalechallan = hdechallan.Value;
                string echallan = ((Label)gridviewPayment.Rows[row.RowIndex].FindControl("lbechallan")).Text; ///from gridview
                if (modalechallan == echallan)
                {

                    RecordUserAction("eChallandelete", "Challan delete button pressed", "S");
                    int result = SoftDeleteEchallanEntry(echallan, "DE");
                    if (result > 0)
                    {
                        LkDiscard.Visible = true;
                        var message = "Your previous challan is deleted by you. Now you can generate new challan and proceed";
                        ShowUserAlert(message, "Refresh");
                    }

                }
            }
            getPaymentGridview();
        }

        public void updatePaymentDBFail(string echallan_no, string plain_PayStatusResponse)
        {

            XmlDocument PayStatusXML = new XmlDocument();
            PayStatusXML.XmlResolver = null;
            PayStatusXML.LoadXml(plain_PayStatusResponse);
            string eChallanStatus = PayStatusXML.GetElementsByTagName("status").Item(0).InnerText.Trim();
            string total_amt = PayStatusXML.GetElementsByTagName("totalAmount").Item(0).InnerText.Trim();
            string bank_ref_no = PayStatusXML.GetElementsByTagName("sbiReferenceNo").Item(0).InnerText.Trim();
            string bank_rcvd_date = PayStatusXML.GetElementsByTagName("bankReceiveDate").Item(0).InnerText.Trim();
            string treasury_rcvd_date = PayStatusXML.GetElementsByTagName("treasuryReceiveDate").Item(0).InnerText.Trim();
            EchallanReceipt echallanreceipt = new EchallanReceipt();
            NpgsqlConnection conn = new NpgsqlConnection();
            NpgsqlCommand cmd = new NpgsqlCommand();
            conn.ConnectionString = ConfigurationManager.ConnectionStrings["connect"].ConnectionString;
            cmd.Connection = conn;
            conn.Open();
            NpgsqlTransaction myTrans = conn.BeginTransaction();
            cmd.Transaction = myTrans;

            try
            {

                cmd.Parameters.Clear();
                string echallan_rcpt = Utility.get_echallan_receiptDocsID();//eReceipt doc ID
                ObjectId obj_id = ObjectId.GenerateNewId();
                string update_challan_status_query = "update esociety.online_payment_details set paystat_response_xml=@paystat_response_xml, status=@status, total_amt=@total_amt,";
                update_challan_status_query = update_challan_status_query + " bank_ref_no = @bank_ref_no, bank_rcvd_date=@bank_rcvd_date,treasury_rcvd_date=@treasury_rcvd_date, ";
                update_challan_status_query = update_challan_status_query + " echallan_RCPT_cross_entry=@echallan_RCPT_cross_entry, echallanrcpt_doc_id=@echallanrcpt_doc_id where echallan_no =@echallan_no";
                cmd.CommandText = update_challan_status_query;

                cmd.Parameters.AddWithValue("@paystat_response_xml", plain_PayStatusResponse);
                cmd.Parameters.AddWithValue("@status", eChallanStatus);
                cmd.Parameters.AddWithValue("@total_amt", Convert.ToDouble(total_amt));
                cmd.Parameters.AddWithValue("@bank_ref_no", bank_ref_no);
                cmd.Parameters.AddWithValue("@bank_rcvd_date", bank_rcvd_date);
                cmd.Parameters.AddWithValue("@treasury_rcvd_date", treasury_rcvd_date);
                cmd.Parameters.AddWithValue("@echallan_no", echallan_no);
                cmd.Parameters.AddWithValue("@echallan_RCPT_cross_entry", obj_id.ToString());
                cmd.Parameters.AddWithValue("@echallanrcpt_doc_id", echallan_rcpt);
                cmd.ExecuteNonQuery();
                cmd.Parameters.Clear();
                byte[] pdf = payment.print_ereceipt(echallan_no);
                if (pdf != null)
                {
                    echallanreceipt.Active = true;
                    echallanreceipt.App_ID = Convert.ToInt64(Session["AppID"].ToString());
                    echallanreceipt.DocContent = pdf;
                    echallanreceipt.Doc_CT = "application/pdf";
                    echallanreceipt.Doc_ID = echallan_rcpt;
                    echallanreceipt.doc_name = echallan_no;
                    echallanreceipt.IpAddress = ipaddress;
                    echallanreceipt.MacAddress = macaddress;
                    echallanreceipt.time_stamp = DateTime.Now.ToString();
                    echallanreceipt._Id = obj_id;
                    echallanreceipt.UpdatedBy = Session["created_by"].ToString();
                    int value = InsertintoMongoDB(echallanreceipt, "eChallanReceipt");
                    if (value == 1) { myTrans.Commit(); }
                    else
                    {
                        myTrans.Rollback();
                        RecordUserAction("updatePaymentDB ", "Transaction Rollback with Failed Payment Status", "F");
                    }
                }
                else
                {
                    myTrans.Commit();
                    RecordUserAction("updatePaymentDB ", "Transaction Committed with Failed Payment Status", "S");
                }




            }
            catch (NpgsqlException ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "updatePaymentDB" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath) + " from " + ipaddress);
                myTrans.Rollback();
                RecordUserAction("updatePaymentDB", ("Exception " + ex.Message + "--" + ex.StackTrace), "E");
                Response.Write("<script language='javascript'>alert('" + "Saving Failed" + "')</script>");

            }
            finally
            {
                conn.Close();
            }
        }

        protected void gridviewPayment_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    Label challanstatus = (e.Row.FindControl("lbstatus") as Label);
                    LinkButton update = (e.Row.FindControl("lkUpdateStatus") as LinkButton);
                    LinkButton delete = (e.Row.FindControl("LkDeleteold") as LinkButton);
                    if (challanstatus.Text.Trim() == "DE")
                    {
                        update.Enabled = false;
                        delete.Enabled = false;

                    }
                    else if (challanstatus.Text.Trim() == "F")
                    {
                        update.Enabled = true;
                        delete.Enabled = true;

                    }
                    else if (challanstatus.Text.Trim() == "S")
                    {
                        update.Enabled = false;
                        delete.Enabled = false;
                    }
                    else
                    {
                        update.Enabled = true;
                        delete.Enabled = true;
                    }

                }

            }
            catch (Exception ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "gridviewPayment_RowDataBound" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath) + " from " + ipaddress);
            }
        }

        public int SoftDeleteEchallanEntry(string eChallanNo, string eChallanStatus)
        {
            int returnVal = 0;          

            NpgsqlConnection conn = new NpgsqlConnection();
            NpgsqlCommand cmd = new NpgsqlCommand();
            conn.ConnectionString = ConfigurationManager.ConnectionStrings["connect"].ConnectionString;
            cmd.Connection = conn;
            conn.Open();

            try
            {
                string delete_echallan_query = "update esociety.online_payment_details set active=@active, status=@status  where echallan_no=@echallan_no and onlinepayment_id=@onlinepayment_id";
                cmd.CommandText = delete_echallan_query;
                cmd.Parameters.AddWithValue("@echallan_no", eChallanNo);
                cmd.Parameters.AddWithValue("@active", 'N');
                cmd.Parameters.AddWithValue("@status", eChallanStatus);
                cmd.Parameters.AddWithValue("@onlinepayment_id", 3);
                cmd.ExecuteNonQuery();
                returnVal = 1;
            }
            catch (NpgsqlException ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "SoftDeleteEchallanEntry()" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
                Response.Write("<script language='javascript'>alert('" + "Challan Expired Error" + "')</script>");

            }
            finally
            {
                conn.Close();
            }
            return returnVal;
        }

        protected void RedirecttoLoginBtn_Click(object sender, EventArgs e)
        {

            payment.ConfirmPayment(Session["Totalamt_certcopy"].ToString());
            
        }

        public void checkPreviousPaymentStatus()
        {
            try
            {
                if (Session["AppID"] != null)
                {
                    var Status = getPendingeChallanDetails(Session["AppID"].ToString());
                    if (!string.IsNullOrWhiteSpace(Status.Item3))
                    {
                        if (Status.Item3.ToString().Trim() == "S")
                        {
                            //btnpaymentstatus.Visible = false;
                            LkProceedtoPayment.Visible = false;
                            paymentpaid.ForeColor = System.Drawing.Color.Green;
                            paymentpaid.Text = "Success";
                            LinkButton14.Enabled = true;
                            LinkButton14.Visible = true;                            
                            RecordUserAction("PaymentStatusBtnClick", "Payment Success. ", "S");                           

                        }
                        else if (Status.Item3.ToString().Trim() == "F")
                        {
                            // btnpaymentstatus.Visible = false;
                            LkProceedtoPayment.Visible = true;
                            paymentpaid.ForeColor = System.Drawing.Color.Red;
                            paymentpaid.Text = "Failed";
                            RecordUserAction("PaymentStatusBtnClick", "Payment Failed. ", "F");

                        }
                        else if ((Status.Item3.ToString().Trim() == "P"))
                        {
                           // btnpaymentstatus.Visible = false;
                            LkProceedtoPayment.Visible = false;
                            paymentpaid.ForeColor = System.Drawing.Color.Purple;
                            paymentpaid.Text = "Pending";
                            RecordUserAction("PaymentStatusBtnClick", "Payment Pending. ", "P");
                        }

                    }
                    else
                    {
                        LkProceedtoPayment.Visible = true;
                        //btnpaymentstatus.Visible = false;
                        RecordUserAction("PaymentStatusBtnClick", "Proceed to Payment. ", "P");
                    }
                }
                else
                {
                    RecordUserAction("checkPreviousPaymentStatus()", "Session null", "F");
                }
            }
            catch (Exception ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "checkPreviousPaymentStatus()" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
            }
        }

        protected void btnrefresh_Click(object sender, EventArgs e)
        {
            RecordUserAction("RefreshBtnModal", "Page Refreshed", "S");
            Response.Redirect("ApplyCertifiedCopy.aspx");
        }

        protected void LinkButton14_Click(object sender, EventArgs e)
        {
            try
            {
                string objectid = getMongoid(Convert.ToInt64(Session["AppID"].ToString()));
                if (objectid == null || objectid == "")
                {
                    NpgsqlConnection conn = new NpgsqlConnection();
                    NpgsqlCommand cmd = new NpgsqlCommand();
                    conn.ConnectionString = ConfigurationManager.ConnectionStrings["connect"].ConnectionString;
                    cmd.Connection = conn;
                    ObjectId obj_id = ObjectId.GenerateNewId();
                    string echallan_rcpt = Utility.get_echallan_receiptDocsID();//eReceipt doc ID
                    EchallanReceipt echallanreceipt = new EchallanReceipt();
                    string echallan_no = Utility.getEchallanNo(Session["AppID"].ToString(),3);
                    conn.Open();
                    NpgsqlTransaction myTrans = conn.BeginTransaction();
                    cmd.Transaction = myTrans;
                    try
                    {
                        cmd.Parameters.Clear();
                        string update_challan_status_query = "update esociety.online_payment_details set echallan_RCPT_cross_entry=@echallan_RCPT_cross_entry, echallanrcpt_doc_id=@echallanrcpt_doc_id where echallan_no =@echallan_no and onlinepayment_id=3";
                        cmd.CommandText = update_challan_status_query;
                        cmd.Parameters.AddWithValue("@echallan_no", echallan_no);
                        cmd.Parameters.AddWithValue("@echallan_RCPT_cross_entry", obj_id.ToString());
                        cmd.Parameters.AddWithValue("@echallanrcpt_doc_id", echallan_rcpt);
                        cmd.ExecuteNonQuery();
                        cmd.Parameters.Clear();
                        byte[] pdf = payment.print_ereceipt((echallan_no));
                        if (pdf != null)
                        {
                            echallanreceipt.Active = true;
                            echallanreceipt.App_ID = Convert.ToInt64(Session["AppID"].ToString());
                            echallanreceipt.DocContent = pdf;
                            echallanreceipt.Doc_CT = "application/pdf";
                            echallanreceipt.Doc_ID = echallan_rcpt;
                            echallanreceipt.doc_name = echallan_no;
                            echallanreceipt.IpAddress = ipaddress;
                            echallanreceipt.MacAddress = macaddress;
                            echallanreceipt.time_stamp = DateTime.Now.ToString();
                            echallanreceipt._Id = obj_id;
                            echallanreceipt.UpdatedBy = Session["created_by"].ToString();
                            int value = InsertintoMongoDB(echallanreceipt, "eChallanReceipt");
                            if (value == 1)
                            {
                                myTrans.Commit();
                                openConnectionMongo_societyDocs(obj_id.ToString(), "eChallanReceipt");
                            }
                            else
                            {
                                myTrans.Rollback();
                            }

                        }
                        else
                        {
                            CreateLogFiles Err = new CreateLogFiles();
                            Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), "Object id Null ", "LinkButton14_Click" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath) + " from " + ipaddress);
                            myTrans.Rollback();
                            Response.Write("<script language='javascript'>alert('" + "Receipt not available..Try after sometime" + "')</script>");
                        }
                    }
                    catch (NpgsqlException ex)
                    {
                        CreateLogFiles Err = new CreateLogFiles();
                        Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.StackTrace + " " + ex.Message, "LinkButton14_Click" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath) + " from " + ipaddress);
                        myTrans.Rollback();
                        Response.Write("<script language='javascript'>alert('" + "Receipt not available..Try after sometime" + "')</script>");
                    }
                    finally
                    {
                        conn.Close();
                    }

                }

                else
                {
                    openConnectionMongo_societyDocs(objectid, "eChallanReceipt");
                }


            }
            catch (Exception ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "LinkButton14_Click" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
            }

        }

        protected byte[] openConnectionMongo_societyDocs(string objectid, string collection_name)
        {
            try
            {
                var str = ConfigurationManager.AppSettings["mongoconnect"];
                IMongoDatabase database;
                IMongoClient client;
                client = new MongoClient(str);
                database = client.GetDatabase("eGoaSociety");
                byte[] filebyte = null;
                var collection = database.GetCollection<EchallanReceipt>(collection_name);
                var status = collection.Find(x => x._Id == ObjectId.Parse(objectid)).FirstOrDefault();
                byte[] decrypt_pdf_bytes = status.DocContent;
                filebyte = decrypt_pdf_bytes;
                convertToPdf(decrypt_pdf_bytes);
                RecordUserAction("openConnectionMongo_societyDocs", "byte[] to function", "S");
                return filebyte;
            }
            catch (MongoException ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "openConnectionMongo_societyDocs" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath) + " from " + ipaddress);
                RecordUserAction("openConnectionMongo_societyDocs", ex.Message, "F");
                Response.Write("<script language='javascript'>alert('" + "PDF View Failed" + ".No file uploaded yet')</script>");
                return null;
            }
        }

        protected string getMongoid(Int64 app_id)
        {
            string mongo = null;
            NpgsqlConnection conn = new NpgsqlConnection();
            NpgsqlCommand cmd = new NpgsqlCommand();
            conn.ConnectionString = ConfigurationManager.ConnectionStrings["connect"].ConnectionString;
            cmd.Connection = conn;
            try
            {
                string query = "";
                query = "select echallan_rcpt_cross_entry from esociety.online_payment_details where app_id = @app_id and onlinepayment_id = 3 and active = 'Y'";
                conn.Open();
                cmd.CommandText = query;
                cmd.Parameters.AddWithValue("@app_id", app_id);
                NpgsqlDataReader dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    mongo = Server.HtmlEncode(dr["echallan_rcpt_cross_entry"].ToString());
                }
                else
                {
                    mongo = null;
                }
                dr.Close();
            }
            catch (NpgsqlException ex)
            {
                mongo = null;
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "getMongoid()" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath) + " from " + ipaddress);

                Response.Write("<script language='javascript'>alert('" + "Error. Pleae try after some time" + "')</script>");
            }
            finally
            {
                conn.Close();
            }
            return mongo;
        }

        protected void LkDeleteold_Click(object sender, EventArgs e)
        {
            try
            {
                LinkButton btn = (LinkButton)sender;
                GridViewRow row = (GridViewRow)btn.NamingContainer;
                string app_id = ((Label)gridviewPayment.Rows[row.RowIndex].FindControl("lbpaymentapp_id")).Text;
                string echallanno = ((Label)gridviewPayment.Rows[row.RowIndex].FindControl("lbechallan")).Text;
                hdechallan.Value = echallanno;
                hdapplicationid.Value = app_id;
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "none", "<script> $(window).on('load', function () { $('#deletechallanModal').modal({ backdrop: 'static' });});</script>", false);
            }
            catch (Exception ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "LkDeleteold_Click" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath) + " from " + ipaddress);
            }
        }

        protected void InitiateOnlinePayment(PayUserDetails pay)
        {
            if (Session["AppID"] != null && Session["created_by"] != null)
            {
                pay.app_id = Session["AppID"].ToString();
               // int renewalflag = Convert.ToInt32(Session["Renewal"].ToString()); // if 1:new, 2:renewal
                NICEncryption _encryption = new NICEncryption();
                ByteEncryption.ByteEncryption obj_Byte_Encryption = new ByteEncryption.ByteEncryption();

                byte[] cpdf_temp = null;
                long txnId = 0;
                string txnTypeDesc = string.Empty;
                string strUtf8_eChallanReq_xml = "", encrypted_eChallanReq = "";
                string plain_echallanno = "", PaymentServiceCode = "0", ServiceHOA = "0";
                string PartyName = "", PartyAddress = "";
                try
                {
                    if (HttpContext.Current.Session["plainechallanno"] != (null))
                        HttpContext.Current.Session.Remove("plainechallanno");
                    //PaymentServiceCode = GlobalVars.RoadTaxSvcCode;
                    ServiceHOA = GlobalVars.SocietyHOA;

                    var xmlArgs = new Dictionary<string, string>();
                    xmlArgs["MobileNo"] = pay.MobileNo;
                    xmlArgs["ChallanCount"] = "1";
                    xmlArgs["Demand"] = GlobalVars.SocietyDemandCode;
                    xmlArgs["Head1"] = ServiceHOA;
                    xmlArgs["Amount1"] = pay.TotalAmt;
                    xmlArgs["TotalAmt"] = pay.TotalAmt;
                    xmlArgs["IPAddress"] = ipaddress;//"10.155.4.42";
                    PartyName = pay.societyName;
                    PartyName = Regex.Replace(PartyName, "[ ](?=[ ])|[^A-Za-z0-9 ]+", "");
                    xmlArgs["PartyName"] = PartyName.Length > 50 ? PartyName.Substring(0, 50) : PartyName;
                    PartyAddress = pay.Address;
                    PartyAddress = Regex.Replace(PartyAddress, "[ ](?=[ ])|[^A-Za-z0-9(. , _ ; \\ - /)]", " ");
                    xmlArgs["PartyAddress"] = PartyAddress.Length > 100 ? PartyAddress.Substring(0, 100) : PartyAddress;
                    xmlArgs["PartyPin"] = pay.Pincode;
                    xmlArgs["PartyTaluka"] = pay.taluka; // TO BE FETCHED FROM SOCIETY TABLE
                    xmlArgs["PartyRef"] = "SOC/" + ViewState["socregid"].ToString();
                    xmlArgs["OtherDetails"] = "Registration Department";
                    xmlArgs["Reason"] = "Society Documents Certified Copy Fees";
                    

                    CreateLogFiles PErr = new CreateLogFiles();

                    strUtf8_eChallanReq_xml = payment.GetPaymentRequestXML(xmlArgs);
                    encrypted_eChallanReq = _encryption.Encrypt(strUtf8_eChallanReq_xml, System.Web.HttpContext.Current.Server.MapPath("~/RTO.KEY"));

                    if (encrypted_eChallanReq != "")
                    {
                        using (var onPayRef = new PayOnlineRef.service())
                        {
                            ServicePointManager.Expect100Continue = true;
                            ServicePointManager.SecurityProtocol =
                                    SecurityProtocolType.Tls12;

                            ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;

                            // ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };                          
                            var result = onPayRef.generateEchallanPDFNew(GlobalVars.SocietyDemandCode, encrypted_eChallanReq, GlobalVars.SocietySvcCode);
                            // var result = "UX0L6pdbCYwo8XlNrKwidg=="
                            if (result != null && result.Tables.Count > 0)
                            {
                                DataRow dtRow = result.Tables[0].Rows[0];
                                if (dtRow["errorflag"].ToString().Trim().Equals("Y"))
                                {
                                    string encrypted_echallanno = dtRow["eno"].ToString().Trim();
                                    plain_echallanno = _encryption.Decrypt(encrypted_echallanno, System.Web.HttpContext.Current.Server.MapPath("~/RTO.KEY"));
                                    Session["plainechallanno"] = plain_echallanno;
                                    //echallan pdf filebytes
                                    for (int i = 0; i < result.Tables[0].Rows.Count; i++)
                                    {
                                        byte[] buffer = (byte[])(result.Tables[0].Rows[i][3]);
                                        byte[] bytes = obj_Byte_Encryption.DecryptData(buffer, System.Web.HttpContext.Current.Server.MapPath("~/RTO.KEY"));

                                        result.Tables[0].Rows[i][3] = bytes;//encrytion of pdf bytes
                                        result.Tables[0].Rows[i][2] = _encryption.Decrypt(result.Tables[0].Rows[i][2].ToString(), System.Web.HttpContext.Current.Server.MapPath("~/RTO.KEY"));//encryption echallan no
                                        byte[] pdf3 = (byte[])(result.Tables[0].Rows[0][3]);
                                        cpdf_temp = pdf3;
                                    }


                                    //Save eChallan generated in DB  
                                    NpgsqlConnection conn = new NpgsqlConnection();
                                    NpgsqlCommand cmd = new NpgsqlCommand();
                                    conn.ConnectionString = ConfigurationManager.ConnectionStrings["connect"].ConnectionString;
                                    cmd.Connection = conn;
                                    conn.Open();
                                    NpgsqlTransaction myTrans = conn.BeginTransaction();
                                    cmd.Transaction = myTrans;
                                    try
                                    {
                                        string echallan_pdf = Utility.get_echallan_pdfDocsID();//eReceipt doc ID
                                        ObjectId obj_id = ObjectId.GenerateNewId();
                                        string payment_details = "INSERT INTO esociety.online_payment_details(app_id, onlinepayment_id, echallanreq_xml, echallan_no, echallangeneratedon,";
                                        payment_details = payment_details + "  active, updated_by, updated_on, ipaddress, macaddress,echallan_pdf_cross_entry,echallanpdf_doc_id)";
                                        payment_details = payment_details + " VALUES(@app_id, @onlinepayment_id,  @echallanreq_xml, @echallan_no, @echallangeneratedon,";
                                        payment_details = payment_details + " 'Y', @updated_by, current_timestamp, @ipaddress, @macaddress,@echallan_pdf_cross_entry,@echallanpdf_doc_id)";
                                        cmd.CommandText = payment_details;
                                        cmd.Parameters.AddWithValue("@app_id", Convert.ToInt64(Session["AppID"].ToString()));
                                        cmd.Parameters.AddWithValue("@onlinepayment_id", 3);
                                        cmd.Parameters.AddWithValue("@echallanreq_xml", strUtf8_eChallanReq_xml);
                                        cmd.Parameters.AddWithValue("@echallan_no", plain_echallanno);
                                        cmd.Parameters.AddWithValue("@echallangeneratedon", DateTime.Now.Date);
                                        cmd.Parameters.AddWithValue("@updated_by", Session["created_by"].ToString());
                                        cmd.Parameters.AddWithValue("@ipaddress", ipaddress);
                                        cmd.Parameters.AddWithValue("@macaddress", macaddress);
                                        cmd.Parameters.AddWithValue("@echallan_pdf_cross_entry", obj_id.ToString());
                                        cmd.Parameters.AddWithValue("@echallanpdf_doc_id", echallan_pdf);
                                        cmd.ExecuteNonQuery();
                                        cmd.Parameters.Clear();
                                       
                                        string query1 = "Update esociety.certifiedcopies set echallan_no=@echallan_no where app_id=@app_id and active = 'Y' and status = 1";
                                        cmd.CommandText = query1;
                                        cmd.Parameters.AddWithValue("@app_id", Convert.ToInt64(pay.app_id));
                                        cmd.Parameters.AddWithValue("@echallan_no", plain_echallanno);
                                        cmd.ExecuteNonQuery();
                                        cmd.Parameters.Clear();
                                        EchallanReceipt echallanreceipt = new EchallanReceipt();
                                        echallanreceipt.Active = true;
                                        echallanreceipt.App_ID = Convert.ToInt64(Session["AppID"].ToString());
                                        echallanreceipt.DocContent = cpdf_temp;
                                        // convertToPdf(temp);
                                        echallanreceipt.Doc_CT = "application/pdf";
                                        echallanreceipt.Doc_ID = echallan_pdf;
                                        echallanreceipt.doc_name = Session["plainechallanno"].ToString();
                                        echallanreceipt.IpAddress = ipaddress;
                                        echallanreceipt.MacAddress = macaddress;
                                        echallanreceipt.time_stamp = DateTime.Now.ToString();
                                        echallanreceipt._Id = obj_id;
                                        echallanreceipt.UpdatedBy = Session["created_by"].ToString();
                                        int value = InsertintoMongoDB(echallanreceipt, "eChallanPDF");
                                        if (value == 1) { myTrans.Commit();                                                                                                                              
                                        }
                                        else
                                        {
                                            myTrans.Rollback();
                                        }


                                    }
                                    catch (NpgsqlException ex)
                                    {
                                        CreateLogFiles Err = new CreateLogFiles();
                                        Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "InitiateOnlinePayment()1" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));

                                        myTrans.Rollback();
                                        Response.Write("<script language='javascript'>alert('" + "Challan generation failed" + "')</script>");
                                    }
                                    finally
                                    {
                                        conn.Close();
                                    }
                                    
                                    //Set echallan request id in ViewState for further use                               
                                }
                            }
                        }
                    }    //deleted
                }
                catch (Exception ex)
                {
                    HttpContext.Current.Session["eChallanReqID"] = 0;
                    HttpContext.Current.Session["plainechallanno"] = "";
                    CreateLogFiles Err = new CreateLogFiles();
                    Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "InitiateOnlinePayment2()" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));

                    Response.Write("<script language='javascript'>alert('" + "Error. Please try after some time." + "')</script>");
                }
            }
            else
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), "Individiual_Page Session null error", "InitiateOnlinePayment");
            }
        }

        protected void getpaymentdonedata(string appid)  // receipt of echallan payment
        {
           Society_Details soc1 = ins.FetchSociety(Session["AppID"].ToString(), 3, 3);
            NpgsqlConnection conn = new NpgsqlConnection();
            NpgsqlCommand cmd = new NpgsqlCommand();
            conn.ConnectionString = ConfigurationManager.ConnectionStrings["connect"].ConnectionString;
            cmd.Connection = conn;
            try
            {
                conn.Open();
                string query = "SELECT  online_payment_details.echallan_no, online_payment_details.bank_ref_no, online_payment_details.status, online_payment_details.total_amt,";
                query = query + " online_payment_details.bank_rcvd_date from esociety.online_payment_details inner join esociety.certifiedcopies on online_payment_details.echallan_no = certifiedcopies.echallan_no";
                query = query + " where certifiedcopies.app_id = @app_id and certifiedcopies.active = @active and onlinepayment_id = 3 and cert_guid = @guid";
                cmd.CommandText = query;
                cmd.Parameters.AddWithValue("@app_id",Convert.ToInt64(appid));
                cmd.Parameters.AddWithValue("@active", "Y");
                cmd.Parameters.AddWithValue("@guid", Session["cert_guid"].ToString());
                NpgsqlDataReader rd = cmd.ExecuteReader();
                if (rd.Read())
                {                 

                    lbltotalfee.Text = Server.HtmlEncode(rd["total_amt"].ToString());
                    lblshowchallan.Text = Server.HtmlEncode(rd["echallan_no"].ToString());
                    string abc = Server.HtmlEncode(rd["status"].ToString());
                    if (abc.Trim() == "S")
                    {
                        paymentpaid.ForeColor = System.Drawing.Color.Green;
                        paymentpaid.Text = "Success";
                    }
                    else
                    {
                        paymentpaid.ForeColor = System.Drawing.Color.Red;
                        paymentpaid.Text = "Status Unknown";
                    }
                    lblsocietyname.Text = Sanitize.InputText(soc1.socname);
                    lblappid.Text = Sanitize.InputText(soc1.app_id);
                }
                rd.Close();
            }
            catch (NpgsqlException ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "getpaymentdonedata()" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath) + " from " + ipaddress);
            }
            finally
            {
                conn.Close();
            }
        }

    }
}