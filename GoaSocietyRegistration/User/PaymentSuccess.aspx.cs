using GoaSocietyRegistration.Development;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;
using WS_Encryption;

namespace GoaSocietyRegistration.User
{
    public partial class PaymentSuccess : System.Web.UI.Page
    {
        string ipaddress = Utility.getIP();
        string macaddress = Utility.GetMACAddress(Utility.getIP());
        //string macaddress = Utility.GetMACAddress();
        protected void Page_Load(object sender, EventArgs e)
        {
            //if (Check4Tampering())
            //{
            //    RecordUserAction("Page_Load", "Access request failed. Tampered session", "F");
            //    SessionManage session = new SessionManage();
            //    session.__Abandon(Request, Response);
            //}
            //else
            //{
                try
                {
                HttpContext.Current.Response.AddHeader("Cache-Control", "no-cache, no-store, must-revalidate");
                HttpContext.Current.Response.AddHeader("Pragma", "no-cache");
                HttpContext.Current.Response.AddHeader("Expires", "0");
                if (!IsPostBack)
                    {
               
              
                        if (Session["login_id"] != null)
                        {
                        RecordUserAction("Page_Load", "Page Load Succesfull", "S");
                        // app_id = (Session["loginsession"].ToString() != "" && Session["loginsession"] != null) ? Convert.ToInt64(Session["loginsession"]) : 0;
                        //RecordUserTrail("Page_Load", "Page accessed successfully", "S");
                        PaymentConfirmation(Request.Form["nicencdata"]);
                            //below two line for live and after that dummy payment options
                            //string encrypted_PaymentResponse = "Yb4SwkcvkpFGDdrpPA76A1bRZCOA2AuAxwx4QkqoF8ZvbCFLeMosuqVArXUJRBOT";
                            //PaymentConfirmation(encrypted_PaymentResponse);
                          //  string plain_data = "<eChallanPaymentStatus><echallanNo>201901138310</echallanNo><status>S</status><totalAmount>550</totalAmount><sbiReferenceNo>CPV3243880</sbiReferenceNo><bankReceiveDate>08/10/2019 08:19:38 PM</bankReceiveDate><treasuryReceiveDate>09/10/2019 12:00:00 AM</treasuryReceiveDate></eChallanPaymentStatus>";
                           // PaymentConfirmation(plain_data);
                        }
                        else
                        {
                            // PaymentConfirmation(Request.Form["nicencdata"]);
                             RecordUserAction("Page_Load", "Page Load Tampered", "F");
                    
                     
                             Response.Redirect("LoginModule.aspx");
                        }
                    }
                }
                catch (Exception ex)
                {
                    CreateLogFiles Err = new CreateLogFiles();
                    Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "Page_Load" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
                    RecordUserAction("Page_Load", ("Exception " + ex.Message + "--" + ex.StackTrace), "E");
                    //ShowUserAlert("Something went wrong. Please try later !!");
                }
            //}
        }
        private bool Check4Tampering()
        {
            bool sessHackedCheck = false;
            //if (Request.UrlReferrer != null)
            //{
            //    Uri uri = new Uri(Request.UrlReferrer.ToString());
            //    RecordUserAction("Page_Load", "Page Load 1", "F");
            //    string referer = uri.Scheme + Uri.SchemeDelimiter + uri.Host;
            //    RecordUserAction("Page_Load", "Page Load 2", "F");
            //    if (!GlobalVars.AntiXsrfRefererHeaderList.Contains(referer))
            //        RecordUserAction("Page_Load", "Page Load 3", "F");
            //    sessHackedCheck = true;
            //    RecordUserAction("Page_Load", "Page Load 4", "F");
            //}
            if (!sessHackedCheck)
            {
                if (Context.Session != null && Session["login_id"] != null && Session["DoTAuthTok"] != null && Request.Cookies["DoTAuthTok"] != null)
                {
                    RecordUserAction("Page_Load", "Page Load 5", "F");
                    if (!(Session["DoTAuthTok"].ToString()).Equals(Request.Cookies["DoTAuthTok"].Value))
                        RecordUserAction("Page_Load", "Page Load 6", "F");
                    sessHackedCheck = true;
                    RecordUserAction("Page_Load", "Page Load 7", "F");
                }
                else
                    RecordUserAction("Page_Load", "Page Load 8", "F");
                sessHackedCheck = true;
                RecordUserAction("Page_Load", "Page Load 9", "F");
            }
            RecordUserAction("Page_Load", sessHackedCheck.ToString() , "F");
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
                trail.user_session_id = Session["loginsession"] != null ? Session["loginsession"].ToString() : "null";
                trail.referrer = uri != null ? uri.ToString().Length > 100 ? uri.ToString().Substring(0, 100) : uri.ToString() : string.Empty;
                trail.accessed_module = "Payment Success";
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
        //public void PaymentConfirmation(string encrypted_PaymentResponse)
        //{
        //    try
        //    {
        //        Utility utilityDao = new Utility();
        //        InitiatePayment paymentdao = new InitiatePayment();
        //        NICEncryption _encryption = new NICEncryption();
        //        OnlinePaymentDetails payOnlineObj = new OnlinePaymentDetails();
        //        var xmlArgs = new Dictionary<string, string>();
        //        string plain_PayStatusResponse = "";
        //        decimal decimalCheck;
        //        string encrypted_StatusReq = "", encrypted_PayStatusResponse = "";


        //        plain_PayStatusResponse = encrypted_PaymentResponse;
        //        XmlDocument PayStatusXML = new XmlDocument();
        //        PayStatusXML.XmlResolver = null;
        //        PayStatusXML.LoadXml(plain_PayStatusResponse);
        //        string status = PayStatusXML.GetElementsByTagName("status").Item(0).InnerText.Trim();
        //        string totalAmount = PayStatusXML.GetElementsByTagName("totalAmount").Item(0).InnerText.Trim();
        //        string sbiReferenceNo = PayStatusXML.GetElementsByTagName("sbiReferenceNo").Item(0).InnerText.Trim();
        //        string bankReceiveDate = PayStatusXML.GetElementsByTagName("bankReceiveDate").Item(0).InnerText.Trim();
        //        string treasuryReceiveDate = PayStatusXML.GetElementsByTagName("treasuryReceiveDate").Item(0).InnerText.Trim();


        //        lblTransaction.Text = "Society Registration";
        //        // lblReceiptNo.Text = Server.HtmlEncode(ViewState["VS_ReceiptNo"].ToString());
        //        lblAmount.Text = Server.HtmlEncode(totalAmount);
        //        lbleChallanNo.Text = Server.HtmlEncode(Session["plainechallanno"].ToString());
        //        lblBankRefNo.Text = Server.HtmlEncode(sbiReferenceNo);
        //        lblBankRcvdDate.Text = Server.HtmlEncode(bankReceiveDate);

        //        //save echallan status in local DB & write back to corresponding application DB
        //        payOnlineObj.echallan_no = Session["plainechallanno"].ToString();
        //        payOnlineObj.nicencdata = encrypted_PaymentResponse;
        //        payOnlineObj.paystat_response_xml = plain_PayStatusResponse;
        //        payOnlineObj.status = status;
        //        payOnlineObj.total_amt = decimal.TryParse(totalAmount, out decimalCheck) ? Convert.ToDecimal(totalAmount) : 0;
        //        payOnlineObj.bank_ref_no = sbiReferenceNo;
        //        payOnlineObj.bank_rcvd_date = bankReceiveDate;
        //        payOnlineObj.treasury_rcvd_date = treasuryReceiveDate;
        //        int retStatus = Insert.UpdateEchallanStatus(payOnlineObj);
        //        if (retStatus == 1)
        //        {
        //            RecordUserAction("PaymentConfirmation", "Echallan status updated", "S");
        //            BtnPrint.Visible = true;
        //            btnBack.Visible = true;

        //        }
        //        else
        //        {
        //            RecordUserAction("PaymentConfirmation", "Echallan status updation request failed", "F");
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        CreateLogFiles Err = new CreateLogFiles();
        //        Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "PaymentConfirmation()" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
        //    }
        //}

        public void PaymentConfirmation(string encrypted_PaymentResponse)
        {
            CreateLogFiles Err = new CreateLogFiles();
            Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), encrypted_PaymentResponse, "succees payment" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
            //Utility utilityDao = new Utility();
            //InitiatePayment paymentdao = new InitiatePayment();
            //NICEncryption _encryption = new NICEncryption();
            //OnlinePaymentDetails payOnlineObj = new OnlinePaymentDetails();
            //var xmlArgs = new Dictionary<string, string>();
            //decimal decimalCheck;
            //long PmtReturnStatus = 0;
            //string strUtf8_StatusReq_xml = "", plain_PayStatusResponse = "", plain_PaymentResponse = "";
            //string encrypted_StatusReq = "", encrypted_PayStatusResponse = "";
            //string txn_type = "", ReturnStatus = "", txnName = string.Empty;
            try
            {
                //RecordUserAction("PaymentConfirmation", "Resource accessed successfully", "S");

                //using (var onPayRef = new RTOwebsite2.PayOnlineRef.service())
                //{
                //    plain_PaymentResponse = _encryption.Decrypt(encrypted_PaymentResponse, HttpContext.Current.Server.MapPath("~/RTO.KEY"));
                //    string[] paymentResponseArr = plain_PaymentResponse.Split('|');
                //    string[] paymentStatus = paymentResponseArr[1].Split('=');
                //    if (paymentStatus[1].Trim().Equals("S"))
                //    {
                //        string[] successChallan = paymentResponseArr[0].Split('=');
                //        string eChallanNo = successChallan[1].Trim();
                //        if (!eChallanNo.Equals(""))
                //        {
                //            cphMain_pnlRegistration.Visible = true;
                //            var xmlStatusArgs = new Dictionary<string, string>();
                //            xmlArgs["eChallanNo"] = eChallanNo;
                //            strUtf8_StatusReq_xml = paymentdao.GetPaymentStatusXML(xmlArgs);               
                //            encrypted_StatusReq = _encryption.Encrypt(strUtf8_StatusReq_xml, System.Web.HttpContext.Current.Server.MapPath("~/RTO.KEY"));
                //            ServicePointManager.Expect100Continue = true;
                //            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls
                //                   | SecurityProtocolType.Tls11
                //                   | SecurityProtocolType.Tls12
                //                   | SecurityProtocolType.Ssl3;
                //            ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
                //            var statusResult = onPayRef.eChallanPaymentStatus(GlobalVars.SocietyDemandCode, encrypted_StatusReq);
                //            if (statusResult != null)
                //            {
                //                string[] payStatusResponseArr = statusResult.Split('|');
                //                if (payStatusResponseArr[0].ToString().Trim().Equals("N"))
                //                {
                //                    encrypted_PayStatusResponse = payStatusResponseArr[2].ToString().Trim();
                //                    plain_PayStatusResponse = _encryption.Decrypt(encrypted_PayStatusResponse, System.Web.HttpContext.Current.Server.MapPath("~/RTO.KEY"));
                //                    XmlDocument PayStatusXML = new XmlDocument();
                //                    PayStatusXML.XmlResolver = null;
                //                    PayStatusXML.LoadXml(plain_PayStatusResponse);
                //                    string status = PayStatusXML.GetElementsByTagName("status").Item(0).InnerText.Trim();
                //                    string totalAmount = PayStatusXML.GetElementsByTagName("totalAmount").Item(0).InnerText.Trim();
                //                    string sbiReferenceNo = PayStatusXML.GetElementsByTagName("sbiReferenceNo").Item(0).InnerText.Trim();
                //                    string bankReceiveDate = PayStatusXML.GetElementsByTagName("bankReceiveDate").Item(0).InnerText.Trim();
                //                    string treasuryReceiveDate = PayStatusXML.GetElementsByTagName("treasuryReceiveDate").Item(0).InnerText.Trim();
                //                    lblTransaction.Text = "Society Registration";
                //                    // lblReceiptNo.Text = Server.HtmlEncode(ViewState["VS_ReceiptNo"].ToString());
                //                    lblAmount.Text = Server.HtmlEncode(totalAmount);
                //                    lbleChallanNo.Text = Server.HtmlEncode(eChallanNo);
                //                    lblBankRefNo.Text = Server.HtmlEncode(sbiReferenceNo);
                //                    lblBankRcvdDate.Text = Server.HtmlEncode(bankReceiveDate);

                //                    //save echallan status in local DB & write back to corresponding application DB
                //                    payOnlineObj.echallan_no = eChallanNo;
                //                    payOnlineObj.nicencdata = encrypted_PaymentResponse;
                //                    payOnlineObj.paystat_response_xml = plain_PayStatusResponse;
                //                    payOnlineObj.status = status;
                //                    payOnlineObj.total_amt = decimal.TryParse(totalAmount, out decimalCheck) ? Convert.ToDecimal(totalAmount) : 0;
                //                    payOnlineObj.bank_ref_no = sbiReferenceNo;
                //                    payOnlineObj.bank_rcvd_date = bankReceiveDate;
                //                    payOnlineObj.treasury_rcvd_date = treasuryReceiveDate;
                //                    int retStatus = Insert.UpdateEchallanStatus(payOnlineObj);
                //                    if (retStatus == 1)
                //                    {
                //                        RecordUserAction("PaymentConfirmation", "Echallan status updated", "S");
                //                        BtnPrint.Visible = true;
                //                        btnBack.Visible = true;

                //                    }
                //                    else
                //                    {
                //                        RecordUserAction("PaymentConfirmation", "Echallan status updation request failed", "F");
                //                    }
                //                }
                //            }
                //        }
                //        else
                //        {
                //            cphMain_pnlRegistration.Visible = false;
                //        }
                //    }
                //}
            }
            catch (Exception ex)
            {
                //CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message, "PaymentConfirmation");
                RecordUserAction("PaymentConfirmation", ("Exception " + ex.Message + "--" + ex.StackTrace), "E");
                //ShowUserAlert("Something went wrong. Please try later !!" + "<br/>" + ex.StackTrace + "<br/>" + ex.InnerException);

                //testing
                //string dec = "echallanno=201900091587|status=S";
                //encrypted_PaymentResponse = _encryption.Encrypt(dec, HttpContext.Current.Server.MapPath("RTO.KEY"));
                //Checkpost tax
                //encrypted_PaymentResponse = "Yb4SwkcvkpFGDdrpPA76A0JS333uccDL6CEx6Fmrx9SFOAPCifoyClH1/whUxlZI";
                //Cess fee
                //encrypted_PaymentResponse = "Yb4SwkcvkpFGDdrpPA76A19LMYQVM+nQwEXMifnfz7dhBMacsFVYBRjjDocn4jYK";
                //CounterSignature
                //encrypted_PaymentResponse = "Yb4SwkcvkpFGDdrpPA76AzZPIUKBwjAZsdBFHJzDmmbH08FbMl2u235/0ZRjzq6y";
                //Passenger tax
                //encrypted_PaymentResponse = "Yb4SwkcvkpFGDdrpPA76A9xIvYbsdIswYys+wWSVdXF1CP46FWXbI31oVfkirEc5";
                //Road tax
                //encrypted_PaymentResponse = "Yb4SwkcvkpFGDdrpPA76A1bRZCOA2AuAxwx4QkqoF8ZvbCFLeMosuqVArXUJRBOT";
            }
            Response.Redirect("Dashboard.aspx");
        }

        protected void btnBack_Click(object sender, EventArgs e)
        {
            Response.Redirect("Dashboard.aspx");
        }
    }
}