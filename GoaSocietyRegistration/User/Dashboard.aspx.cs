using MongoDB.Bson;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;
using GoaSocietyRegistration;
using GoaSocietyRegistration.Development;
using WS_Encryption;
using System.Web.Security.AntiXss;
using Microsoft.Security.Application;
using System.Text.RegularExpressions;
using MongoDB.Driver;
using System.IO;
using System.Data;
using System.Web.UI.HtmlControls;

namespace GoaSocietyRegistration
{
    [Serializable]
    public partial class Dashboard : System.Web.UI.Page
    {
        InitiatePayment payment = new InitiatePayment();
        NpgsqlConnection conn = new NpgsqlConnection();
        NpgsqlCommand cmd = new NpgsqlCommand();
        NICEncryption _encryption = new NICEncryption();
        string ipaddress = Utility.getIP();
        string macaddress = Utility.GetMACAddress(Utility.getIP());
        //string macaddress = Utility.GetMACAddress();
        Validate vs = new Validate();

        PayUserDetails pay = new PayUserDetails();
        //static string Totalamt = "";
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Check4Tampering() ||  Page.RouteData.RouteHandler == null)
            {
                RecordUserAction("Page_Load", "Access request failed. Tampered session", "F");
                SessionManage session = new SessionManage();
                session.__Abandon(Request, Response);


            }
            else
            {
                if (Session["login_id"] != null)
                {
                    if (!IsPostBack)
                    {
                        Session["common_logout"] = Session["login_id"];
                        Session["Totalamt"] = "";
                    }
                    HttpContext.Current.Response.AddHeader("Cache-Control", "no-cache, no-store, must-revalidate");
                    HttpContext.Current.Response.AddHeader("Pragma", "no-cache");
                    HttpContext.Current.Response.AddHeader("Expires", "0");
                    string loginid = Server.HtmlEncode(Session["login_id"].ToString());
                    //lblloginid.Text = "(" + loginid + ")";
                    lblloginid.Text = " " + loginid + " ";
                    conn.ConnectionString = ConfigurationManager.ConnectionStrings["connect"].ConnectionString;
                    cmd.Connection = conn;
                    try
                    {
                        conn.Open();
                        string query = "select app_id,created_by from esociety.temp_table where login_id=@sid";
                        cmd.CommandText = query;
                        cmd.Parameters.AddWithValue("@sid", loginid);
                        NpgsqlDataReader rd = cmd.ExecuteReader();
                        if (rd.Read())
                        {
                            string appid = Server.HtmlEncode(rd["app_id"].ToString());

                            Session["created_by"] = Server.HtmlEncode(rd["created_by"].ToString());
                            setloginName(loginid);
                            rd.Close();
                            if (appid == "" || appid == null)
                            {
                                Int64 AppID = Utility.get_ApplicationID();
                                Session["AppID"] = AppID;
                                setAppID(loginid, AppID);
                                set_status();
                                Response.Redirect("Dashboard.aspx");

                            }
                            else
                            {
                                Session["AppID"] = Convert.ToInt64(appid);
                                if (Utility.checkifrenewal(appid) == 2)
                                {
                                    Session["Renewal"] = 2;
                                    string text = Utility.getOldRegistrationNo(appid);
                                    string[] Result = text.Split('|');
                                    Session["OldRegNo"] = Server.HtmlEncode(Result[0]);
                                    Session["OldRegDistrict"] = Server.HtmlEncode(Result[1]);
                                    Session["OldAppId"] = getOldAppId(Sanitize.InputText(Session["OldRegNo"].ToString()));
                                    //paidemployee.Visible = true;
                                }
                                else
                                {
                                    Session["Renewal"] = 1;
                                }

                                try
                                {
                                    cmd.Parameters.Clear();

                                    Temp_table tmp_table = new Temp_table();
                                    Insert ins = new Insert();
                                    // tmp_table = ins.getData(Session["AppID"].ToString());
                                    tmp_table = ins.getRemarksData(Session["AppID"].ToString());

                                    string query1 = "select mst_status.status_name, mst_status.status_id from esociety.mst_status, esociety.status_table where mst_status.status_id = status_table.status_id and status_table.app_id=@sid";
                                    cmd.CommandText = query1;
                                    cmd.Parameters.AddWithValue("@sid", (Session["AppID"]));
                                    NpgsqlDataReader kr = cmd.ExecuteReader();
                                    if (kr.Read())
                                    {

                                        ViewState["profile_status"] = Server.HtmlEncode(kr["status_name"].ToString());//status name
                                        int status_id = Convert.ToInt32(Server.HtmlEncode(kr["status_id"].ToString()));
                                        kr.Close();
                                        Session["status_Id"] = status_id;//status id
                                        if (Convert.ToInt32(Session["status_Id"]) == 1 || Convert.ToInt32(Session["status_Id"]) == 2 || Convert.ToInt32(Session["status_Id"]) == 5)
                                        {
                                            editapplicationbtn.Text = "Edit Application";
                                            editapplicationbtn.Enabled = true;
                                            if (Convert.ToInt32(Session["status_Id"]) == 5)
                                            {
                                                try
                                                {
                                                    string text = Server.HtmlEncode(tmp_table.remarks_sendobservation);
                                                    observation_remarks.Visible = true;
                                                    observation_remarks.Text = text;
                                                    lblimpnotice.Visible = false;
                                                    pleasedonec_changes.Visible = true;
                                                }
                                                catch (IOException ex)
                                                {
                                                    //RecordUserAction("observation failed to load first time", ex.Message, "F");
                                                    CreateLogFiles Err = new CreateLogFiles();
                                                    Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "Page_Load1" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));

                                                }
                                            }
                                        }
                                        else if (status_id == 8)
                                        {
                                            if (Session["AppID"] != null)
                                            {
                                                edit_application.Visible = false;
                                                Div1.Visible = true;
                                                kr.Close();
                                                cmd.Parameters.Clear();
                                                lblappid.Text = Session["AppID"].ToString();
                                                string socquery = "select socname, socaddr, pincode, totalfee,regfee,processfee, mst_taluka.\"TalukaName\" from esociety.society,esociety.mst_taluka where mst_taluka.\"TalukaID\" = society.soc_taluka and app_id = @appid";
                                                cmd.CommandText = socquery;
                                                cmd.Parameters.AddWithValue("@appid", Convert.ToInt64(appid));
                                                NpgsqlDataReader sr = cmd.ExecuteReader();
                                                if (sr.Read())
                                                {
                                                    lblsocietyname.Text = Server.HtmlEncode(sr["socname"].ToString());
                                                    ViewState["socname"] = Server.HtmlEncode(sr["socname"].ToString());
                                                    lbltotalfee.Text = Server.HtmlEncode(sr["totalfee"].ToString());
                                                    regis_fees.Text = Server.HtmlEncode(sr["regfee"].ToString());
                                                    ViewState["socaddr"] = Server.HtmlEncode(sr["socaddr"].ToString());
                                                    ViewState["totalfee"] = Server.HtmlEncode(sr["totalfee"].ToString());
                                                    ViewState["soc_taluka"] = Server.HtmlEncode(sr["TalukaName"].ToString());
                                                    ViewState["pincode"] = Server.HtmlEncode(sr["pincode"].ToString());
                                                    if (Convert.ToInt32(Session["Renewal"].ToString()) == 2)
                                                    {
                                                        onrenewal.Visible = false;
                                                        Label8.Text = "Renewal Fee ₹ " + Sanitize.InputText(sr["processfee"].ToString()) + " & Penalty Fee ₹ ";
                                                    }
                                                    else
                                                    {
                                                        onrenewal.Visible = false;
                                                    }
                                                }
                                                sr.Close();
                                                cmd.Parameters.Clear();
                                                string query2 = "select applicant_mobile_no,applicant_email from esociety.applicant_details where app_id=@appid";
                                                cmd.CommandText = query2;
                                                cmd.Parameters.AddWithValue("@appid", Convert.ToInt64(appid));
                                                NpgsqlDataReader dr = cmd.ExecuteReader();
                                                if (dr.Read())
                                                {
                                                    // lbremarks_applicant.Text = Server.HtmlEncode(dr["remarks"].ToString());
                                                    lbremarks_applicant.Text = Server.HtmlEncode(tmp_table.remarks_accepted);
                                                    ViewState["applicant_mobile_no"] = Server.HtmlEncode(Encryption.Encrypt.Decrypt(dr["applicant_mobile_no"].ToString()));
                                                    Session["mobile_no"] = Server.HtmlEncode(Encryption.Encrypt.Decrypt(dr["applicant_mobile_no"].ToString()));
                                                    if (dr["applicant_email"].ToString() == "NA")
                                                    {
                                                        ViewState["applicant_email"] = Server.HtmlEncode(dr["applicant_email"].ToString());
                                                    }
                                                    else
                                                    {
                                                        ViewState["applicant_email"] = Server.HtmlEncode(Encryption.Encrypt.Decrypt(dr["applicant_email"].ToString()));
                                                    }                                                    
                                                }
                                                dr.Close();
                                                string echallanstatus = null;
                                                //it will check status of that challan no online 
                                                var Status = payment.getPendingeChallanDetails(Session["AppID"].ToString());//working
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
                                                            //if (echallanstatus.Equals("D"))
                                                            //{
                                                            conn.Close();
                                                            updatePaymentDB(echallanno, Result[1]);/////<------------------error

                                                            RecordUserAction("payment_Click", "Payment Success", "S");
                                                            Response.Redirect("Dashboard.aspx");
                                                        }
                                                        else
                                                        {
                                                            //Response.Write("<script language=javascript>alert('ERROR');</script>");

                                                            paymentgridview.Visible = true;
                                                            getPaymentGridviewNew();
                                                            RecordUserAction("Page_Load", "Application ready to proceed for Payment", "S");
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    paymentgridview.Visible = true;
                                                    getPaymentGridviewNew();
                                                    RecordUserAction("Page_Load", "Application ready to proceed for Payment", "S");

                                                }



                                            }
                                        }
                                        else if (status_id == 9)
                                        {
                                            edit_application.Visible = false;
                                            Div1.Visible = true;
                                            lblremarks_app.Text = "Your Application has been Rejected due to following reasons.";
                                            kr.Close();
                                            cmd.Parameters.Clear();
                                            string query2 = "select remarks from esociety.applicant_details where app_id=@appid";
                                            cmd.CommandText = query2;
                                            cmd.Parameters.AddWithValue("@appid", Convert.ToInt64(appid));
                                            NpgsqlDataReader dr = cmd.ExecuteReader();
                                            btnReject.Visible = true;
                                            btnPayment.Visible = false;
                                            if (dr.Read())
                                            {
                                                lbremarks_applicant.Text = Server.HtmlEncode(dr["remarks"].ToString());                                                
                                            }
                                            dr.Close();
                                            lbremarks_applicant.ForeColor = System.Drawing.Color.Red;
                                            btnReject.Visible = true;
                                            btnPayment.Visible = false;
                                            paymentDIV.Visible = false;
                                            //lbremarks_applicant.Text = Server.HtmlEncode(tmp_table.remarks_rejected);
                                            lbNote.Visible = true;
                                            lbNote.ForeColor = System.Drawing.Color.Red;
                                            lbNote.Text = "Kindly Fill a Fresh Application for Society Registration.";
                                            

                                          int a = checkreject();
                                            if (a == 1)
                                            {
                                                LinkButton1.Visible = false;
                                                paymentDIV.Visible = false;
                                                lbNote.Visible = false;
                                                loadremarks();
                                                rejectwithstatus.Visible = true;
                                              
                                            }


                                        }
                                        else if (status_id == 10 || status_id == 12)
                                        {
                                            if (Session["AppID"] != null)
                                            {
                                                edit_application.Visible = false;
                                                Div1.Visible = true;
                                                lbremarks_applicant.Visible = false;
                                                lblRemarks.Visible = false;
                                                checkPreviousPaymentStatus();
                                                btnPayment.Visible = false;
                                                lblappid.Text = Session["AppID"].ToString();
                                                cmd.Parameters.Clear();
                                                showchallan.Visible = true;
                                                string socquery = "select socname, socaddr, pincode,echallan_no,totalfee,regfee, mst_taluka.\"TalukaName\" from esociety.society,esociety.mst_taluka where mst_taluka.\"TalukaID\" = society.soc_taluka and app_id = @appid";
                                                cmd.CommandText = socquery;
                                                cmd.Parameters.AddWithValue("@appid", Convert.ToInt64(appid));
                                                kr.Close();
                                                NpgsqlDataReader gr = cmd.ExecuteReader();
                                                if (gr.Read())
                                                {
                                                    lblsocietyname.Text = Server.HtmlEncode(gr["socname"].ToString());
                                                    lbltotalfee.Text = Server.HtmlEncode(gr["totalfee"].ToString());
                                                    regis_fees.Text = Server.HtmlEncode(gr["regfee"].ToString());
                                                    lblshowchallan.Text = Server.HtmlEncode(gr["echallan_no"].ToString());

                                                }
                                                gr.Close();
                                                cmd.Parameters.Clear();
                                            }
                                        }
                                        else
                                        {
                                            kr.Close();
                                            edit_application.Visible = true;
                                            Div1.Visible = false;
                                        }

                                        //if(status_id >= 10)
                                        //{
                                        //    amendment.Visible = true;
                                        //}
                                        //else
                                        //{
                                        //    amendment.Visible = false;
                                        //}

                                    }
                                    else
                                    {
                                        kr.Close();
                                    }

                                }
                                catch (NpgsqlException ex)
                                {
                                    CreateLogFiles Err = new CreateLogFiles();
                                    Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "Page_Load2" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
                                    RecordUserAction("Page_Load", ex.Message, "F");
                                    //Response.Write("<script language='javascript'>alert('" + ex.Message + "')</script>");
                                    ShowUserAlert("Error", "Re-direct");
                                }
                                finally
                                {
                                    conn.Close();
                                }

                                //check logic form this point 
                                if (Convert.ToInt32(Session["status_Id"]) == 3 || Convert.ToInt32(Session["status_Id"]) == 6 || Convert.ToInt32(Session["status_Id"]) == 4 || Convert.ToInt32(Session["status_Id"]) == 7)
                                {

                                    lblimpnotice.Text = "Your Application has been submitted to District Registrar.";
                                    lblnotice2.Visible = true;
                                    lblnotice2.Text = "Your Application ID is : " + Session["AppID"].ToString();
                                    editapplicationbtn.ForeColor = System.Drawing.Color.White;
                                    editapplicationbtn.Text = "Submitted";
                                    editapplicationbtn.Enabled = false;
                                    //payment due
                                }

                            }

                        }
                        else { rd.Close(); }

                    }
                    catch (NpgsqlException ex)
                    {
                        CreateLogFiles Err = new CreateLogFiles();
                        Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "Page_Load3" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
                        Response.Write("<script language='javascript'>alert('" + "Page Load Failed" + "')</script>");
                        RecordUserAction("Page_Load", ex.Message, "F");
                    }
                    finally
                    {
                        conn.Close();
                    }
                }
                else
                {
                    RecordUserAction("Page_Load", "Session request failed. Tampered session", "F");
                    Response.Redirect("LoginModule.aspx");
                }

            }
        }

        public int checkreject()
        {
            int _flag = 0;
            NpgsqlConnection conn = new NpgsqlConnection();
            NpgsqlCommand cmd = new NpgsqlCommand();
            conn.ConnectionString = ConfigurationManager.ConnectionStrings["connect"].ConnectionString;
            cmd.Connection = conn;
            try
            {
                conn.Open();
                string query = "SELECT reject_society FROM esociety.status_table where app_id = @app_id and status_id = 9";
                cmd.CommandText = query;
                cmd.Parameters.AddWithValue("@app_id", Convert.ToInt64(Session["AppID"].ToString()));
                NpgsqlDataReader rd = cmd.ExecuteReader();
                if (rd.Read())
                {
                    string temp = rd["reject_society"].ToString();
                    _flag = Convert.ToInt16(temp);
                }
                rd.Close();
            }
            catch (NpgsqlException ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "checkreject()" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
                RecordUserAction("set_status()", ex.Message, "F");
                Response.Write("<script language='javascript'>alert('" + "Execution Error" + "')</script>");

            }
            finally
            {
                conn.Close();
            }
            return _flag;
        }

        public void loadremarks()
        {
            NpgsqlConnection conn = new NpgsqlConnection();
            NpgsqlCommand cmd = new NpgsqlCommand();
            conn.ConnectionString = ConfigurationManager.ConnectionStrings["connect"].ConnectionString;
            cmd.Connection = conn;
            conn.Open();
            try
            {
                string query = "SELECT society_reject_mongo,remarks_reject FROM esociety.status_table where app_id = @app_id and status_id = 9 and reject_society=1";
                cmd.CommandText = query;
                cmd.Parameters.AddWithValue("@app_id", Convert.ToInt64(Session["AppID"].ToString()));
                NpgsqlDataReader rd = cmd.ExecuteReader();
                if (rd.Read())
                {
                    Label11.Text = Sanitize.InputText(rd["remarks_reject"].ToString());
                    HFMongo.Value = Sanitize.InputText(rd["society_reject_mongo"].ToString());
                }
                rd.Close();
            }
            catch (NpgsqlException ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "loadremarks()" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
               
                RecordUserAction("Page_Load", ex.Message, "F");
            }
            finally
            {
                conn.Close();
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

        public void checkPreviousPaymentStatus()
        {
            try
            {
                if (Session["AppID"] != null)
                {
                    var Status = payment.getPendingeChallanDetails(Session["AppID"].ToString());
                    if (!string.IsNullOrWhiteSpace(Status.Item3))
                    {
                        if (Status.Item3.ToString().Trim() == "S")
                        {
                            btnpaymentstatus.Visible = false;
                            btnPayment.Visible = false;
                            paymentpaid.ForeColor = System.Drawing.Color.Green;
                            paymentpaid.Text = "Success";
                            LinkButton1.Enabled = true;
                            LinkButton1.Visible = true;
                            gotooffice.ForeColor = System.Drawing.Color.Green;
                            gotooffice.Text = "Please collect your Certificate from our Office, after two working days. Thankyou!";
                            RecordUserAction("PaymentStatusBtnClick", "Payment Success. ", "S");
                            // btnprintcertificate.Visible = true;

                        }
                        else if (Status.Item3.ToString().Trim() == "F")
                        {
                            btnpaymentstatus.Visible = false;
                            btnPayment.Visible = true;
                            paymentpaid.ForeColor = System.Drawing.Color.Red;
                            paymentpaid.Text = "Failed";
                            RecordUserAction("PaymentStatusBtnClick", "Payment Failed. ", "F");

                        }
                        else if ((Status.Item3.ToString().Trim() == "P"))
                        {
                            btnpaymentstatus.Visible = false;
                            btnPayment.Visible = false;
                            paymentpaid.ForeColor = System.Drawing.Color.Purple;
                            paymentpaid.Text = "Pending";
                            RecordUserAction("PaymentStatusBtnClick", "Payment Pending. ", "P");
                        }

                    }
                    else
                    {
                        btnPayment.Visible = true;
                        btnpaymentstatus.Visible = false;
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
        public void set_status()
        {
            if (Session["AppID"] != null)
            {
                NpgsqlConnection conn = new NpgsqlConnection();
                NpgsqlCommand cmd = new NpgsqlCommand();
                conn.ConnectionString = ConfigurationManager.ConnectionStrings["connect"].ConnectionString;
                cmd.Connection = conn;
                conn.Open();
                try
                {
                    string complete_query = "INSERT INTO esociety.status_table(app_id, status_id,login_id,reject_society) VALUES(@app_id,@status_id,@login_id,0)";
                    cmd.CommandText = complete_query;
                    cmd.Parameters.AddWithValue("@app_id", Convert.ToInt64(Session["AppID"].ToString()));
                    cmd.Parameters.AddWithValue("@status_id", 1);
                    cmd.Parameters.AddWithValue("@login_id", Session["login_id"].ToString());
                    cmd.ExecuteNonQuery();
                    RecordUserAction("set_status()", "insert success", "S");
                }
                catch (NpgsqlException ex)
                {
                    CreateLogFiles Err = new CreateLogFiles();
                    Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "set_status()" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
                    RecordUserAction("set_status()", ex.Message, "F");
                    Response.Write("<script language='javascript'>alert('" + "Execution Error" + "')</script>");

                }
                finally
                {
                    conn.Close();
                }
            }
            else
            {
                RecordUserAction("set_status()", "Session null", "F");
            }
        }
        public void setAppID(string loginid, Int64 appid)
        {

            NpgsqlConnection conn = new NpgsqlConnection();
            NpgsqlCommand cmd = new NpgsqlCommand();
            conn.ConnectionString = ConfigurationManager.ConnectionStrings["connect"].ConnectionString;
            cmd.Connection = conn;
            conn.Open();
            NpgsqlTransaction trans = conn.BeginTransaction();
            try
            {

                cmd.Transaction = trans;
                cmd.Parameters.Clear();
                string query = "UPDATE esociety.temp_table SET app_id=@appid WHERE login_id=@login_id";
                cmd.CommandText = query;
                cmd.Parameters.AddWithValue("@appid", appid);
                cmd.Parameters.AddWithValue("@login_id", loginid);
                cmd.ExecuteNonQuery();
                cmd.Parameters.Clear();
                string query1 = "UPDATE esociety.applicant_details SET app_id=@appid, head_entry=@head_entry WHERE login_id=@login_id";
                cmd.CommandText = query1;
                cmd.Parameters.AddWithValue("@appid", appid);
                cmd.Parameters.AddWithValue("@login_id", loginid);
                cmd.Parameters.AddWithValue("@head_entry", 0);
                cmd.ExecuteNonQuery();
                trans.Commit();
                RecordUserAction("setAppID()", "transaction success", "S");

            }
            catch (NpgsqlException ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "setAppID()" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
                Response.Write("<script language='javascript'>alert('Error')</script>");
                RecordUserAction("setAppID()", ex.Message, "F");
                trans.Rollback();
            }
            finally
            {
                conn.Close();
            }
        }
        public void setloginName(string token)
        {

            NpgsqlConnection conn = new NpgsqlConnection();
            NpgsqlCommand cmd = new NpgsqlCommand();
            conn.ConnectionString = ConfigurationManager.ConnectionStrings["connect"].ConnectionString;
            cmd.Connection = conn;
            try
            {
                conn.Open();
                string query1 = "SELECT user_firstname FROM esociety.usertable where user_loginname=@sid";
                cmd.CommandText = query1;
                cmd.Parameters.AddWithValue("@sid", token);
                NpgsqlDataReader rd = cmd.ExecuteReader();
                if (rd.Read())
                {
                    lblusername.Text = Server.HtmlEncode(rd["user_firstname"].ToString());
                }
                rd.Close();
                RecordUserAction("SetLoginName", "Success", "S");
            }
            catch (NpgsqlException ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "setloginName()" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
                Response.Write("<script language='javascript'>alert('" + "Exection Error" + "')</script>");
                RecordUserAction("SetLoginName", ex.Message, "F");
            }
            finally
            {
                conn.Close();
            }
        }
        protected void editapplicationbtn_Click(object sender, EventArgs e)
        {
            RecordUserAction("EditApplBtn", "Edit Application Button Clicked ", "S");
            Response.Redirect("SocietyDetails.aspx");

        }
        protected void btnPayment_Click(object sender, EventArgs e)
        {
            try
            {
                if (Session["AppID"] != null)
                {
                    RecordUserAction("PaymentBtnClick", "Proceed to Payment ", "S");
                    bool savePtEntry = false, saveEchallanEntry = false;
                    string recpt_no = "", echallanNo = "";
                    string echallanstatus;

                    //user details object            
                    pay.app_id = Session["AppID"].ToString();
                    pay.Address = ViewState["socaddr"].ToString();
                    pay.amount = Server.HtmlEncode(ViewState["totalfee"].ToString());
                    pay.Applicant = ViewState["socname"].ToString();
                    // pay.Emailid = AES_algorithm.Decrypt(ViewState["applicant_email"].ToString());
                    string mobile = ViewState["applicant_mobile_no"].ToString();
                    pay.MobileNo = mobile;
                    pay.Pincode = ViewState["pincode"].ToString();
                    pay.societyName = ViewState["socname"].ToString();
                    pay.taluka = ViewState["soc_taluka"].ToString();
                    pay.TotalAmt = Server.HtmlEncode(ViewState["totalfee"].ToString());
                    Session["Totalamt"] = pay.TotalAmt;
                    if (!pay.TotalAmt.Equals("0"))
                    {
                        var Status = payment.getPendingeChallanDetails(Session["AppID"].ToString());
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
                                        ShowUserAlert("Payment of <b style='color:red'>₹ " + Convert.ToInt32(Session["Totalamt"].ToString()) + "/-</b> against eChallan No <b style='color:red'> " + echallanno + " </b> and Receipt No <b style='color:red'> " + recpt_no + " </b> is already been done", "Cancel");
                                        check_btnpaymentstatus_Click();
                                        btnPayment.Visible = false;

                                    }
                                    else if (echallanstatus.Equals("F"))
                                    {
                                        //if (Validity <= 7)
                                        //    ShowUserAlert("Valid eChallan with No <b style='color:red'> " + echallanno + " </b> exists for <b style='color:red'> " + Session["AppID"].ToString() + " </b>, with an amount of <b style='color:red'>₹" + Convert.ToInt32(Session["Totalamt"].ToString()) + "/-</b>. Click Confirm to proceed with online payment", "Confirm");
                                        //else
                                        //{
                                            int result = payment.SoftDeleteEchallanEntry(echallanno, echallanstatus);
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
                                            int result = payment.SoftDeleteEchallanEntry(echallanno, echallanstatus);
                                            if (result > 0)
                                            {
                                                savePtEntry = false;
                                                saveEchallanEntry = true;
                                            }
                                        }
                                        else
                                        {
                                            var message = "eChallan with No. " + echallanno + " for ₹" + Convert.ToInt32(Session["Totalamt"].ToString()) + " (" + Session["AppID"].ToString() + ") returned status "
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
                                        int result = payment.SoftDeleteEchallanEntry(echallanno, "E");
                                        if (result > 0)
                                        {
                                            savePtEntry = false;
                                            saveEchallanEntry = true;
                                        }
                                    }
                                    else
                                        ShowUserAlert("eChallan No.<b style='color:red'>" + echallanno + "</b> exist for Application ID:<b style='color:red'> " + Session["AppID"].ToString() + " </b>, Amount:<b style='color:red'>" + Convert.ToInt32(Session["Totalamt"].ToString()) + "/-</b>.do you want to continue with this eChallan No. Confirm to proceed", "Confirm");
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

                         payment.InitiateOnlinePayment(pay);     // ----> enable this for live and belwo line
                        echallanNo = (Session != null && Session["plainechallanno"] != null) ? Convert.ToString(Session["plainechallanno"]) : "";
                        if (!echallanNo.Equals(""))
                            ShowUserAlert("For Payment of <b style='color:red'>₹" + Convert.ToInt32(Session["Totalamt"].ToString()) + "/-</b> with eChallan No <b style='color:red'> " + echallanNo + " </b> and Receipt No <b style='color:red'> " + recpt_no + " </b> you will be redirected to Payment Gateway. Click Confirm to proceed", "Confirm");
                        ////remove 2 lines below
                        //else
                        //    Response.Write("<script language='javascript'>alert('" + "Please note : Payment is disabled" + "')</script>");
                   
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
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "none", "<script> $(window).on('load', function () { $('#myModal').modal({ backdrop: 'static' });});</script>", false);
                }
                else if (ConfirmCancel.Equals("Cancel"))
                {
                    lblMSG1.Text = message;
                    RedirecttoLoginBtn.Visible = false;
                    errorpage.Visible = false;
                    btnCancel.Text = "Okay";
                    btnrefresh.Visible = false;
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "none", "<script> $(window).on('load', function () { $('#myModal').modal({ backdrop: 'static' });});</script>", false);
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
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "none", "<script> $(window).on('load', function () { $('#myModal').modal({ backdrop: 'static' });});</script>", false);
                }
                else if (ConfirmCancel.Equals("Re-direct"))
                {
                    lblMSG1.Text = message;
                    RedirecttoLoginBtn.Visible = false;
                    btnCancel.Visible = false;
                    errorpage.Visible = true;
                    errorpage.Text = "Cancel";
                    btnrefresh.Visible = false;
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "none", "<script> $(window).on('load', function () { $('#myModal').modal({ backdrop: 'static' });});</script>", false);
                }
            }
            catch (Exception ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "ShowUserAlert()" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
            }
        }
        protected void RedirecttoLoginBtn_Click(object sender, EventArgs e)
        {

             payment.ConfirmPayment(Session["Totalamt"].ToString());  
           // Response.Redirect("PaymentSuccess.aspx");
        }
        protected void errorpage_Click(object sender, EventArgs e)
        {
            RecordUserAction("ErrorPage", "Error Page Load ", "S");
            Response.Redirect("usererrorpage.aspx");
        }
        protected void btnReject_Click(object sender, EventArgs e)
        {

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
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "()" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
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

                    var Status = payment.getPendingeChallanDetails(Session["AppID"].ToString());
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
                                LinkButton1.Enabled = true;
                                LinkButton1.Visible = true;
                                ShowUserAlert("Payment of <b style='color:red'>₹ " + "amount" + "/-</b> against eChallan No <b style='color:red'>" + echallanno + "</b> and Receipt No <b style='color:red'>" + "recipt no" + "</b> is already been done", "Refresh");
                                btnpaymentstatus.Visible = false;

                            }
                            else if (echallanstatus.Equals("F"))
                            {   //not required to updatePaymentDB
                                //updatePaymentDB(echallanno, xmlstatus);
                                RecordUserAction("UpdatePaymentBtn", "Payment Failed. Db not updating only for Trial..", "F");
                                ShowUserAlert("Payment of <b style='color:red'>₹ " + "amount" + "/-</b> against eChallan No <b style='color:red'>" + echallanno + "</b> and Receipt No <b style='color:red'>" + "recipt no" + "</b> is failed. Please Click on Make Payment to Initiate", "Cancel");
                                btnPayment.Enabled = true;
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
        protected void btnpaymentstatus_Click(object sender, EventArgs e)
        {
            check_btnpaymentstatus_Click();
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
                    cmd.Parameters.AddWithValue("@bank_rcvd_date",bank_rcvd_date);
                    cmd.Parameters.AddWithValue("@treasury_rcvd_date", treasury_rcvd_date);
                    cmd.Parameters.AddWithValue("@echallan_no", echallan_no);
                    cmd.Parameters.AddWithValue("@echallan_RCPT_cross_entry", obj_id.ToString());
                    cmd.Parameters.AddWithValue("@echallanrcpt_doc_id", echallan_rcpt);
                    cmd.ExecuteNonQuery();
                    cmd.Parameters.Clear();
                    //registration id generation
                    int distirct_id = Utility.get_districtid(Session["AppID"].ToString());
                    int renewalflag = Convert.ToInt32(Session["Renewal"].ToString()); // if 1:new, 2:renewal
                    if (distirct_id != 0)
                    {
                        ///string socregid = Utility.getRegistrationID(distirct_id);
                        if (renewalflag == 1)//For New
                        {
                                cmd.Parameters.Clear();
                            //string updquery = "UPDATE esociety.status_amendment SET app_id=@app_id,amend_status=1 WHERE login_id=@login_id";
                            //cmd.CommandText = updquery;
                            //cmd.Parameters.AddWithValue("@app_id", Convert.ToInt64(Session["AppID"].ToString()));
                            //cmd.Parameters.AddWithValue("@login_id", Session["login_id"].ToString());
                            //cmd.ExecuteNonQuery();
                            //cmd.Parameters.Clear();
                            //string otherserv_query = "INSERT into esociety.status_amendment(login_id,app_id,amend_status) VALUES(@login_id,@app_id,1)";
                            //cmd.CommandText = otherserv_query;
                            //cmd.Parameters.AddWithValue("@app_id", Convert.ToInt64(Session["AppID"].ToString()));
                            //cmd.Parameters.AddWithValue("@login_id", Session["login_id"].ToString());
                            //cmd.ExecuteNonQuery();
                            //cmd.Parameters.Clear();
                            string query1 = "Update esociety.status_table set status_id=@status_id where echallan_no=@echallan_no";
                                cmd.CommandText = query1;
                                cmd.Parameters.AddWithValue("@status_id", 10);
                                cmd.Parameters.AddWithValue("@echallan_no", echallan_no);
                                cmd.ExecuteNonQuery();
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
                                RecordUserAction("updatePaymentDB ", "Transaction Rollback mongo saving failed", "F");
                                myTrans.Commit();
                                }
                           

                        }
                        else if (renewalflag == 2)//For Renewal
                        {
                            string regid = "";
                            cmd.Parameters.Clear();
                            string reg_query = "SELECT old_socregid FROM esociety.applicant_details where app_id = @app_id and login_id = @login_id";
                            cmd.CommandText = reg_query;
                            cmd.Parameters.AddWithValue("@app_id", Session["AppID"].ToString());
                            cmd.Parameters.AddWithValue("@login_id", Session["login_id"].ToString());      
                            NpgsqlDataReader rd = cmd.ExecuteReader();
                            if (rd.Read())
                            {
                                regid = rd["old_socregid"].ToString();
                                string temp = regid;
                                rd.Close();
                                cmd.Parameters.Clear();
                                ////////////////////////////             
                                string query = "update esociety.society set socregid = @socregid, regdate = @regdate where app_id = @app_id";
                                cmd.CommandText = query;
                                cmd.Parameters.AddWithValue("@socregid", regid);
                                cmd.Parameters.AddWithValue("@regdate", bank_rcvd_date);
                                cmd.Parameters.AddWithValue("@app_id", Convert.ToInt64(Session["AppID"].ToString()));
                                cmd.ExecuteNonQuery();
                                cmd.Parameters.Clear();
                                //string updquery = "UPDATE esociety.status_amendment SET app_id=@app_id,amend_status=1 WHERE login_id=@login_id";
                                //cmd.CommandText = updquery;
                                //cmd.Parameters.AddWithValue("@app_id", Convert.ToInt64(Session["AppID"].ToString()));
                                //cmd.Parameters.AddWithValue("@login_id", Session["login_id"].ToString());
                                //cmd.ExecuteNonQuery();
                                //cmd.Parameters.Clear();
                                string otherserv_query = "INSERT into esociety.status_amendment(login_id,app_id,amend_status) VALUES(@login_id,@app_id,1)";
                                cmd.CommandText = otherserv_query;
                                cmd.Parameters.AddWithValue("@app_id", Convert.ToInt64(Session["AppID"].ToString()));
                                cmd.Parameters.AddWithValue("@login_id", Session["login_id"].ToString());
                                cmd.ExecuteNonQuery();
                                cmd.Parameters.Clear();
                                string query1 = "Update esociety.status_table set status_id=@status_id where echallan_no=@echallan_no";
                                cmd.CommandText = query1;
                                cmd.Parameters.AddWithValue("@status_id", 10);
                                cmd.Parameters.AddWithValue("@echallan_no", echallan_no);
                                cmd.ExecuteNonQuery();
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
                                //////////////////////////
                            }
                            else
                            {
                                rd.Close();
                            }
                        }
                       
                        else
                        {

                            myTrans.Rollback();
                            RecordUserAction("updatePaymentDB ", "Transaction Rollback no read ", "F");

                        }
                    }
                    else
                    {
                        myTrans.Rollback();
                        RecordUserAction("updatePaymentDB ", "Transaction Rollback 0 district id", "F");
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
        protected string getMongoid(Int64 app_id, int payment_id)
        {
            string mongo = null;
            NpgsqlConnection conn = new NpgsqlConnection();
            NpgsqlCommand cmd = new NpgsqlCommand();
            conn.ConnectionString = ConfigurationManager.ConnectionStrings["connect"].ConnectionString;
            cmd.Connection = conn;
            try
            {
                string query = "";
                query = "select echallan_rcpt_cross_entry from esociety.online_payment_details where app_id = @app_id and onlinepayment_id=@onlinepayment_id";
                conn.Open();
                cmd.CommandText = query;
                cmd.Parameters.AddWithValue("@app_id", app_id);
                cmd.Parameters.AddWithValue("@onlinepayment_id", payment_id);
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
        protected void LinkButton1_Click(object sender, EventArgs e)
        {
            try
            {
                int renewalflag = Convert.ToInt32(Session["Renewal"].ToString());
                string objectid = getMongoid(Convert.ToInt64(Session["AppID"].ToString()), renewalflag);
                if (objectid == null || objectid == "")
                {
                    NpgsqlConnection conn = new NpgsqlConnection();
                    NpgsqlCommand cmd = new NpgsqlCommand();
                    conn.ConnectionString = ConfigurationManager.ConnectionStrings["connect"].ConnectionString;
                    cmd.Connection = conn;                  
                    ObjectId obj_id = ObjectId.GenerateNewId();
                    string echallan_rcpt = Utility.get_echallan_receiptDocsID();//eReceipt doc ID
                    EchallanReceipt echallanreceipt = new EchallanReceipt();
                    string echallan_no = Utility.getEchallanNo(Session["AppID"].ToString(),1);
                    conn.Open();
                    NpgsqlTransaction myTrans = conn.BeginTransaction();
                    cmd.Transaction = myTrans;
                    try
                    {
                        cmd.Parameters.Clear();
                        string update_challan_status_query = "update esociety.online_payment_details set echallan_RCPT_cross_entry=@echallan_RCPT_cross_entry, echallanrcpt_doc_id=@echallanrcpt_doc_id where echallan_no =@echallan_no and onlinepayment_id=1";
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
                            Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), "Object id Null ", "LinkButton1_Click" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath) + " from " + ipaddress);
                            myTrans.Rollback();
                            Response.Write("<script language='javascript'>alert('" + "Receipt not available..Try after sometime" + "')</script>");
                        }
                    }
                    catch (NpgsqlException ex)
                    {
                        CreateLogFiles Err = new CreateLogFiles();
                        Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.StackTrace + " " + ex.Message, "LinkButton1_Click" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath) + " from " + ipaddress);
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
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "LinkButton1_Click" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
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
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "convertToPdf()" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
            }
        }

        private void RecordUserAction(string action, string description, string status)
        {
            Insert _insrt = new Insert();
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
                count = _insrt.SaveAuditTrail(trail);
            } while (count == 0);
            /*Audit trail*/
        }

        protected void btnrefresh_Click(object sender, EventArgs e)
        {
            RecordUserAction("RefreshBtnModal", "Page Refreshed", "S");
            Response.Redirect("Dashboard.aspx");
        }

        protected string getobject(string challan_for_pdf)
        {
            string temp = "";
            int renewal_flag = Convert.ToInt32(Session["Renewal"].ToString());
            NpgsqlConnection conn = new NpgsqlConnection();
            NpgsqlCommand cmd = new NpgsqlCommand();
            conn.ConnectionString = ConfigurationManager.ConnectionStrings["connect"].ConnectionString;
            cmd.Connection = conn;
            try
            {
                string query = "";
                query = " Select echallan_pdf_cross_entry from esociety.online_payment_details where echallan_no = @echallan_no and onlinepayment_id=@onlinepayment_id";
                conn.Open();
                cmd.CommandText = query;
                cmd.Parameters.AddWithValue("@echallan_no", challan_for_pdf);
                cmd.Parameters.AddWithValue("@onlinepayment_id", renewal_flag);
                NpgsqlDataReader dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    temp = Server.HtmlEncode(dr["echallan_pdf_cross_entry"].ToString());

                }
                dr.Close();
            }
            catch (NpgsqlException ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "getobject()" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
                //RecordUserAction("MemberDetails_Page_Load", "Exception at Member Data", "F");
                Response.Write("<script language='javascript'>alert('" + "Error. Pleae try after some time" + "')</script>");
            }
            finally
            {
                conn.Close();
            }

            return temp;

        }
        protected void ViewChallan_Click(object sender, EventArgs e)
        {
            if (Session["plainechallanno"] != null)
            {
                string objectid = getobject(Session["plainechallanno"].ToString());
                if (objectid != null || objectid != "")
                {
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
                        CreateLogFiles Err = new CreateLogFiles();
                        Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "ViewChallan_Click" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
                        Response.Write("<script language='javascript'>alert('" + "PDF View Failed" + "')</script>");
                    }
                }
            }
        }

        protected string getOldAppId(string regid)
        {
            string temp = "";
            NpgsqlConnection conn = new NpgsqlConnection();
            NpgsqlCommand cmd = new NpgsqlCommand();
            conn.ConnectionString = ConfigurationManager.ConnectionStrings["connect"].ConnectionString;
            cmd.Connection = conn;
            try
            {
                conn.Open();
                string query = "SELECT app_id FROM esociety.society where socregid = @socregid";
                cmd.CommandText = query;
                cmd.Parameters.AddWithValue("@socregid", regid);
                NpgsqlDataReader rd = cmd.ExecuteReader();
                if (rd.Read())
                {
                    temp = rd["app_id"].ToString();
                }
                else
                {
                    temp = "";
                }
                rd.Close();
            }
            catch (NpgsqlException ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(HttpContext.Current.Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "getOldRegistraionNo()" + " " + "  BaseUtility" + " from " + Utility.getIP());
                temp = "";
            }
            finally
            {
                conn.Close();
            }
            return temp;
        }
     
        protected void getPaymentGridviewNew()
        {
            int renewalflag = Convert.ToInt32(Session["Renewal"].ToString()); // if 1:new, 2:renewal
            NpgsqlConnection conn = new NpgsqlConnection();
            NpgsqlCommand cmd = new NpgsqlCommand();
            conn.ConnectionString = ConfigurationManager.ConnectionStrings["connect"].ConnectionString;
            cmd.Connection = conn;
            try
            {
                string query = "select echallan_no,echallangeneratedon,status,app_id from esociety.online_payment_details where app_id=@app_id and onlinepayment_id = @onlinepayment_id";
                cmd.CommandText = query;
                cmd.Parameters.AddWithValue("@app_id", Convert.ToInt64(Session["AppID"].ToString()));
                cmd.Parameters.AddWithValue("@onlinepayment_id", renewalflag);
                using (NpgsqlDataAdapter dr = new NpgsqlDataAdapter(cmd))
                {
                    DataTable dt = new DataTable();
                    dr.Fill(dt);
                    gridviewPayment.DataSource = dt;
                    gridviewPayment.DataBind();
                }
                int count = gridviewPayment.Rows.Count;
                if (count >= 1)
                {
                    LkBtnMasterRefresh.Visible = true;
                }


            }
            catch (NpgsqlException ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "getPaymentGridview()" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath) + " from " + ipaddress);
                //RecordUserAction("MemberDetails_Page_Load", "Exception at Member Data", "F");
                Response.Write("<script language='javascript'>alert('" + "Failed, Please try again." + "')</script>");
            }
            finally
            {
                conn.Close();
            }
        }
       


        protected void LkBtnMasterRefresh_Click(object sender, EventArgs e)
        {
            int result = MakeeChallanInactive(Session["AppID"].ToString());
            if (result == 1)
            {
                //fetch all echallan in for while loop
                foreach (GridViewRow row in gridviewPayment.Rows)
                {
                    string echallan = ((Label)gridviewPayment.Rows[row.RowIndex].FindControl("lbechallan")).Text;
                    string temp_echallanstatus = payment.GetEchallanPaymentStatus(echallan);
                    string[] Result = temp_echallanstatus.Split('|');
                    if (!string.IsNullOrWhiteSpace(Result[0]))
                    {
                        if (Result[0].Equals("S"))
                        {
                            //active this echallan and update in status_applicant
                            RecordUserAction("payment_Click", "Payment Success", "S");
                            int a = update_payment_status(Session["AppID"].ToString(), echallan);
                            if (a == 1)
                            {
                                Response.Redirect("Dashboard.aspx");
                            }
                        }
                    }

                }
            }
            else
            {
                ///Oops something went wrong
            }
        }

        protected int update_payment_status(string app_id, string echallan)
        {
            int success = 0;
            int renewalflag = Convert.ToInt32(Session["Renewal"].ToString()); // if 1:new, 2:renewal
            NpgsqlConnection conn = new NpgsqlConnection();
            NpgsqlCommand cmd = new NpgsqlCommand();
            conn.ConnectionString = ConfigurationManager.ConnectionStrings["connect"].ConnectionString;
            cmd.Connection = conn;
            conn.Open();
            NpgsqlTransaction trans = conn.BeginTransaction();
            try
            {
                cmd.Parameters.Clear();
                string update_query = "UPDATE esociety.online_payment_details SET active = 'Y' WHERE app_id = @app_id and onlinepayment_id = @onlinepayment_id and echallan_no = @echallan_no";
                cmd.CommandText = update_query;
                cmd.Parameters.AddWithValue("@app_id", Convert.ToInt64(app_id));
                cmd.Parameters.AddWithValue("@echallan_no", echallan);
                cmd.Parameters.AddWithValue("@onlinepayment_id", renewalflag);
                cmd.ExecuteNonQuery();
                cmd.Parameters.Clear();
                string update_status_query = "UPDATE esociety.status_table SET  echallan_no = @echallan_no WHERE app_id = @app_id ";
                cmd.CommandText = update_status_query;
                cmd.Parameters.AddWithValue("@app_id", Convert.ToInt64(app_id));
                cmd.Parameters.AddWithValue("@echallan_no", echallan);
                cmd.ExecuteNonQuery();
                trans.Commit();
                success = 1;
            }
            catch (NpgsqlException ex)
            {
                success = 0;
                trans.Rollback();
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.StackTrace + " " + ex.Message, "update_payment_status()" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath) + " from " + ipaddress);
                Response.Write("<script language='javascript'>alert('" + "Try after sometime" + "')</script>");
            }
            finally
            {
                conn.Close();
            }
            return success;
        }
        protected int MakeeChallanInactive(string app_id)
        {
            int renewalflag = Convert.ToInt32(Session["Renewal"].ToString()); // if 1:new, 2:renewal
            NpgsqlConnection conn = new NpgsqlConnection();
            NpgsqlCommand cmd = new NpgsqlCommand();
            conn.ConnectionString = ConfigurationManager.ConnectionStrings["connect"].ConnectionString;
            cmd.Connection = conn;
            int success = 0;
            try
            {
                conn.Open();
                string update_query = "UPDATE esociety.online_payment_details SET active = 'N' WHERE app_id = @app_id and onlinepayment_id = @onlinepayment_id";
                cmd.CommandText = update_query;
                cmd.Parameters.AddWithValue("@app_id", Convert.ToInt64(app_id));
                cmd.Parameters.AddWithValue("@onlinepayment_id", renewalflag);  
                int a = cmd.ExecuteNonQuery();
                if (a > 0)
                {
                    success = 1;
                }
                else
                {
                    success = 0;
                }
            }
            catch (NpgsqlException ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.StackTrace + " " + ex.Message, "MakeeChallanInactive()" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath) + " from " + ipaddress);
                Response.Write("<script language='javascript'>alert('" + "Try after sometime" + "')</script>");
            }
            finally
            {
                conn.Close();
            }
            return success;
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
        //Full changes here
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


            getPaymentGridviewNew();
            Response.Redirect("Dashboard.aspx");
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
                            Response.Redirect("Dashboard.aspx");
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
                    int result = payment.SoftDeleteEchallanEntry(echallan, "DE");
                    if (result > 0)
                    {
                        var message = "Your previous challan is deleted by you. Now you can generate new challan and proceed";
                        ShowUserAlert(message, "Refresh");
                    }

                }
            }
            getPaymentGridviewNew();
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
        protected byte[] openConnectionMongo_CancelDoc(string objectid, string collection_name)
        {
            ByteEncryption.ByteEncryption obj_Byte_Encryption = new ByteEncryption.ByteEncryption();
            try
            {
                var str = ConfigurationManager.AppSettings["mongoconnect"];
                IMongoDatabase database;
                IMongoClient client;
                client = new MongoClient(str);
                database = client.GetDatabase("eGoaSociety");
                var collection = database.GetCollection<OtherDocuments>(collection_name);
                var status = collection.Find(x => x._Id == ObjectId.Parse(objectid)).FirstOrDefault();
                byte[] decrypt_pdf_bytes = status.DocContent;
                byte[] pdf = obj_Byte_Encryption.DecryptData(decrypt_pdf_bytes, System.Web.HttpContext.Current.Server.MapPath("~/ENCRYPT.KEY"));
                convertToPdf(pdf);
                RecordUserAction("openConnectionMongo_CancelDoc", "byte[] to function", "S");
                return pdf;
            }
            catch (MongoException ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "openConnectionMongo_CancelDoc" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath) + " from " + ipaddress);
                RecordUserAction("openConnectionMongo_CancelDoc", ex.Message, "F");
                Response.Write("<script language='javascript'>alert('" + "PDF View Failed" + ".No file uploaded yet')</script>");
                return null;
            }
        }
        protected void LkBtnViewDoc_Click(object sender, EventArgs e)
        {
            string mongo = HFMongo.Value;
            if (mongo != null || mongo != "")
            {
                openConnectionMongo_CancelDoc(mongo, "Reject Society");
            }
        }

        /////////////////////////////////

//        if (renewalflag == 1)//For New
//                        {
//                            string regid = "";
//        cmd.Parameters.Clear();
//                            string reg_query = "SELECT soc_reg_no FROM esociety.master_sequence where districtid = @districtid";
//        cmd.CommandText = reg_query;
//                            cmd.Parameters.AddWithValue("@districtid", distirct_id);
//                            NpgsqlDataReader rd = cmd.ExecuteReader();
//                            if (rd.Read())
//                            {
//                                regid = rd["soc_reg_no"].ToString();
//        string temp = regid;
//        rd.Close();
//                                regid = regid + "/GOA/" + DateTime.Now.Year.ToString();
//                                cmd.Parameters.Clear();
//                                int value_temp = Convert.ToInt32(temp);
//        value_temp = value_temp + 1;
//                                string upquery = "UPDATE esociety.master_sequence SET soc_reg_no = @soc_reg_no WHERE districtid = @districtid";
//        cmd.CommandText = upquery;
//                                cmd.Parameters.AddWithValue("@soc_reg_no", value_temp);
//                                cmd.Parameters.AddWithValue("@districtid", distirct_id);
//                                cmd.ExecuteNonQuery();
//                                cmd.Parameters.Clear();

//                                string query = "update esociety.society set socregid = @socregid, regdate =@regdate where app_id = @app_id";
//        cmd.CommandText = query;
//                                cmd.Parameters.AddWithValue("@socregid", regid);
//                                cmd.Parameters.AddWithValue("@regdate", bank_rcvd_date);
//                                cmd.Parameters.AddWithValue("@app_id", Convert.ToInt64(Session["AppID"].ToString()));
//                                cmd.ExecuteNonQuery();
//                                cmd.Parameters.Clear();
//                                string updquery = "UPDATE esociety.status_amendment SET app_id=@app_id,amend_status=1 WHERE login_id=@login_id";
//        cmd.CommandText = updquery;
//                                cmd.Parameters.AddWithValue("@app_id", Convert.ToInt64(Session["AppID"].ToString()));
//                                cmd.Parameters.AddWithValue("@login_id", Session["login_id"].ToString());
//                                cmd.ExecuteNonQuery();
//                                cmd.Parameters.Clear();
//                                string query1 = "Update esociety.status_table set status_id=@status_id where echallan_no=@echallan_no";
//        cmd.CommandText = query1;
//                                cmd.Parameters.AddWithValue("@status_id", 10);
//                                cmd.Parameters.AddWithValue("@echallan_no", echallan_no);
//                                cmd.ExecuteNonQuery();
//                                byte[] pdf = payment.print_ereceipt(echallan_no);
//                                if (pdf != null)
//                                {
//                                    echallanreceipt.Active = true;
//                                    echallanreceipt.App_ID = Convert.ToInt64(Session["AppID"].ToString());
//                                    echallanreceipt.DocContent = pdf;
//                                    echallanreceipt.Doc_CT = "application/pdf";
//                                    echallanreceipt.Doc_ID = echallan_rcpt;
//                                    echallanreceipt.doc_name = echallan_no;
//                                    echallanreceipt.IpAddress = ipaddress;
//                                    echallanreceipt.MacAddress = macaddress;
//                                    echallanreceipt.time_stamp = DateTime.Now.ToString();
//                                    echallanreceipt._Id = obj_id;
//                                    echallanreceipt.UpdatedBy = Session["created_by"].ToString();
//        int value = InsertintoMongoDB(echallanreceipt, "eChallanReceipt");
//                                    if (value == 1)
//                                    {
//                                        myTrans.Commit();
//                                    }
//                                    else
//                                    {
//                                        myTrans.Rollback();
//                                        RecordUserAction("updatePaymentDB ", "Transaction Rollback mongo saving failed", "F");
//}
//                                }
//                                else
//                                {
//                                    myTrans.Rollback();
//                                }
//                            }


//                        }
    ////////////////////////////////////////////////////////




    }
}