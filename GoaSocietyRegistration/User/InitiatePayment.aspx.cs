using Npgsql;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;
using WS_Encryption;
using ByteEncryption;
using MongoDB.Bson;
using GoaSocietyRegistration.Development;

namespace GoaSocietyRegistration
{
    [Serializable]
    public partial class InitiatePayment : System.Web.UI.Page
    {
        string ipaddress = Utility.getIP();
        string macaddress = Utility.GetMACAddress(Utility.getIP());
        //string macaddress = Utility.GetMACAddress();
    
         byte[] temp = null;
        protected void Page_Load(object sender, EventArgs e)
        {
            HttpContext.Current.Response.AddHeader("Cache-Control", "no-cache, no-store, must-revalidate");
            HttpContext.Current.Response.AddHeader("Pragma", "no-cache");
            HttpContext.Current.Response.AddHeader("Expires", "0");
            if (Session["login_id"] != null && Session["AppID"] != null)
            {
                string appid = Session["AppID"].ToString();
            }
            else
            {
                Response.Redirect("LoginModule.aspx");
            }
        }
        protected void Button1_Click(object sender, EventArgs e)
        {

        }
        //public void InitiateOnlinePayment(string ReceiptNo, string Txn, string Amt, string App_id, Int64 UserID, string type, PaymentUserDetails societyownerdetials, Hashtable htTransactionObjs)
        public void InitiateOnlinePayment( PayUserDetails pay)
        {
            if (Session["AppID"] != null && Session["created_by"] != null)
            {
                pay.app_id = Session["AppID"].ToString();
                int renewalflag = Convert.ToInt32(Session["Renewal"].ToString()); // if 1:new, 2:renewal
                NICEncryption _encryption = new NICEncryption();
                ByteEncryption.ByteEncryption obj_Byte_Encryption = new ByteEncryption.ByteEncryption();

                long txnId = 0;
                string txnTypeDesc = string.Empty;
                string strUtf8_eChallanReq_xml = "", encrypted_eChallanReq = "";
                string plain_echallanno = "", PaymentServiceCode = "0", ServiceHOA = "0";
                string PartyName = "", PartyAddress = "";
                try
                {
                    if (HttpContext.Current.Session["plainechallanno"] != (null))
                        HttpContext.Current.Session.Remove("plainechallanno");
                 
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
                    xmlArgs["PartyRef"] = "SOC/" + Session["login_id"].ToString();
                    xmlArgs["OtherDetails"] = "Registration Department";
                    if (renewalflag == 1 )
                    {
                        xmlArgs["Reason"] = "Society Processing and Registration Fees";
                    }
                    else
                    {
                         xmlArgs["Reason"] = "Society Processing and Renewal Fees";
                    }
                   
                    CreateLogFiles PErr = new CreateLogFiles();              


                    strUtf8_eChallanReq_xml = GetPaymentRequestXML(xmlArgs);
                    encrypted_eChallanReq = _encryption.Encrypt(strUtf8_eChallanReq_xml, System.Web.HttpContext.Current.Server.MapPath("~/RTO.KEY"));
               
                    if (encrypted_eChallanReq != "")
                    {
                        using (var onPayRef = new PayOnlineRef.service())
                        {
                            ServicePointManager.Expect100Continue = true;
                            ServicePointManager.SecurityProtocol =
                                    SecurityProtocolType.Tls12;
                                  
                            ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;
                       
                                             
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
                                        setbytes(pdf3);
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
                                        cmd.Parameters.AddWithValue("@onlinepayment_id", renewalflag);
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
                                        string query2 = "Update esociety.status_table set echallan_no=@echallan_no where app_id=@app_id";
                                        cmd.CommandText = query2;
                                        cmd.Parameters.AddWithValue("@app_id", Convert.ToInt64(pay.app_id));
                                        cmd.Parameters.AddWithValue("@echallan_no", plain_echallanno);
                                        cmd.ExecuteNonQuery();
                                        cmd.Parameters.Clear();
                                        string query1 = "Update esociety.society set echallan_no=@echallan_no where app_id=@app_id";
                                        cmd.CommandText = query1;
                                        cmd.Parameters.AddWithValue("@app_id", Convert.ToInt64(pay.app_id));
                                        cmd.Parameters.AddWithValue("@echallan_no", plain_echallanno);
                                        cmd.ExecuteNonQuery();
                                        cmd.Parameters.Clear();
                                        EchallanReceipt echallanreceipt = new EchallanReceipt();
                                        echallanreceipt.Active = true;
                                        echallanreceipt.App_ID = Convert.ToInt64(Session["AppID"].ToString());
                                        echallanreceipt.DocContent = temp;
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
                                        if (value == 1) { myTrans.Commit(); }
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
            else {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), "Individiual_Page Session null error", "InitiateOnlinePayment");
            }
        }
        public void setbytes(byte[] pdf)
        {
            temp = pdf;

        }
        public int InsertintoMongoDB(EchallanReceipt rcpt, string sel_collection)
        {
            Insert insr = new Insert();
            try
            {
                return insr.InsertMongoEchallanReceipt(rcpt, sel_collection);
            }
            catch (Exception ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "InsertintoMongoDB()" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));

                return 0;
            }
        }
        public string GetPaymentRequestXML(Dictionary<string, string> xmlArgs)
        {
            string strUtf8_Req_xml = "";
            try
            {
                XmlWriterSettings xmlWriterSettings = new XmlWriterSettings();
                xmlWriterSettings.Encoding = new UTF8Encoding(false);
                xmlWriterSettings.ConformanceLevel = ConformanceLevel.Document;
                xmlWriterSettings.Indent = true;

                using (MemoryStream output1 = new MemoryStream())
                {
                    using (XmlWriter writer = XmlWriter.Create(output1, xmlWriterSettings))
                    {
                        writer.WriteStartElement("USERDATA");
                        writer.WriteElementString("MobileNo", xmlArgs["MobileNo"]);
                        writer.WriteElementString("ChallanCount", xmlArgs["ChallanCount"]);
                        writer.WriteElementString("Demand", xmlArgs["Demand"]);
                        writer.WriteElementString("Head1", xmlArgs["Head1"]);
                        writer.WriteElementString("Amount1", xmlArgs["Amount1"]);
                        writer.WriteElementString("TotalAmt", xmlArgs["TotalAmt"]);
                        writer.WriteElementString("IPAddress", xmlArgs["IPAddress"]);
                        writer.WriteElementString("PartyName", xmlArgs["PartyName"]);
                        writer.WriteElementString("PartyAddress", xmlArgs["PartyAddress"]);
                        writer.WriteElementString("PartyPin", xmlArgs["PartyPin"]);
                        writer.WriteElementString("PartyTaluka", xmlArgs["PartyTaluka"]);
                        writer.WriteElementString("PartyRef", xmlArgs["PartyRef"]);
                        writer.WriteElementString("OtherDetails", xmlArgs["OtherDetails"]);
                        writer.WriteElementString("Reason", xmlArgs["Reason"]);
                        writer.WriteEndElement();
                        writer.Flush();
                    }
                    strUtf8_Req_xml = Encoding.UTF8.GetString(output1.ToArray());
                }
            }
            catch (Exception ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "GetPaymentRequestXML()" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
            }
            return strUtf8_Req_xml;
        }
        public string GetEchallanPaymentStatus(string eChallanNo)
        {
            var xmlArgs = new Dictionary<string, string>();
            string strUtf8_StatusReq_xml = "", encrypted_StatusReq = "", encrypted_PayStatusResponse = "", plain_PayStatusResponse = "", status = string.Empty;
            //PaymentDAO paymentDao = new PaymentDAO();
            NICEncryption _encryption = new NICEncryption();
            try
            {
                using (var onPayRef = new PayOnlineRef.service())
                {
                    xmlArgs["eChallanNo"] = eChallanNo;
                    //dao get payment status xml below code comment
                    strUtf8_StatusReq_xml = GetPaymentStatusXML(xmlArgs);
                    encrypted_StatusReq = _encryption.Encrypt(strUtf8_StatusReq_xml, System.Web.HttpContext.Current.Server.MapPath("~/RTO.KEY"));

                  
                    ServicePointManager.Expect100Continue = true;
                    ServicePointManager.SecurityProtocol =     SecurityProtocolType.Tls12| SecurityProtocolType.Tls11| SecurityProtocolType.Tls ;
                    ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;
                    //ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;
                    ServicePointManager.ServerCertificateValidationCallback  += (sender, cert, chain, ssl) => { return true; }; //delegate { return true; };
                    var statusResult = onPayRef.eChallanPaymentStatus(GlobalVars.SocietyDemandCode, encrypted_StatusReq);
                    if (statusResult != null)
                    {
                        string[] payStatusResponseArr = statusResult.Split('|');
                        if (payStatusResponseArr[0].ToString().Trim().Equals("N"))
                        {
                            encrypted_PayStatusResponse = payStatusResponseArr[2].ToString().Trim();
                            plain_PayStatusResponse = _encryption.Decrypt(encrypted_PayStatusResponse, System.Web.HttpContext.Current.Server.MapPath("~/RTO.KEY"));
                            XmlDocument PayStatusXML = new XmlDocument();
                            PayStatusXML.XmlResolver = null;
                            PayStatusXML.LoadXml(plain_PayStatusResponse);
                            status = PayStatusXML.GetElementsByTagName("status").Item(0).InnerText.Trim() + "|" + plain_PayStatusResponse;
                            //status = PayStatusXML.GetElementsByTagName("status").Item(0).InnerText.Trim();
                        }
                    }
                }
                return status;
            }           
            catch (HttpException hex) {

                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), hex.Message + " " + hex.StackTrace, "httpexception()" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));

                return status;
            }
            catch (Exception ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "GetEchallanPaymentStatus()" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));

                return status;
            }
        }
        public string GetPaymentStatusXML(Dictionary<string, string> xmlArgs)
        {
            string strUtf8_Req_xml = "";
            try
            {
                XmlWriterSettings xmlWriterSettings = new XmlWriterSettings();
                xmlWriterSettings.Encoding = new UTF8Encoding(false);
                xmlWriterSettings.ConformanceLevel = ConformanceLevel.Document;
                xmlWriterSettings.Indent = true;

                using (MemoryStream output1 = new MemoryStream())
                {
                    using (XmlWriter writer = XmlWriter.Create(output1, xmlWriterSettings))
                    {
                        writer.WriteStartElement("Echallan");
                        writer.WriteElementString("eChallanNo", xmlArgs["eChallanNo"]);
                        writer.WriteEndElement();
                        writer.Flush();
                    }
                    strUtf8_Req_xml = Encoding.UTF8.GetString(output1.ToArray());
                }
            }
            catch (Exception ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "GetPaymentStatusXML()" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
            }
            return strUtf8_Req_xml;
        }
        public void ConfirmPayment(string Amt)
        {
            string plain_PaymentReq = "", encrypted_PaymentReq = "";
            string PlainEchallanNo = "";
            NICEncryption _encryption = new NICEncryption();
            var xmlArgs = new Dictionary<string, string>();
            try
            {
                PlainEchallanNo = Convert.ToString(HttpContext.Current.Session["plainechallanno"]);
                if (!PlainEchallanNo.Equals(""))
                {
                    plain_PaymentReq = "echallanno=" + PlainEchallanNo + "| totalamt=" + Amt;
                    encrypted_PaymentReq = _encryption.Encrypt(plain_PaymentReq, HttpContext.Current.Server.MapPath("~/RTO.KEY"));
                    if (encrypted_PaymentReq != "")
                    {
                        var values = new NameValueCollection();
                        values["demand"] = GlobalVars.SocietyDemandCode;
                        values["encdata"] = encrypted_PaymentReq;
                        _RedirectWithData(values, GlobalVars.MakePaymentUrl);
                    }
                }
            }
            catch (Exception ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "ConfirmPayment()" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
            }
        }
        public static void _RedirectWithData(NameValueCollection data, string url)
        {
            try
            {
                StringBuilder s = new StringBuilder();
                s.Append("<html>");
                s.AppendFormat("<body onload='document.forms[\"form\"].submit()'>");
                s.AppendFormat("<form name='form' action='{0}' method='post'>", url);
                foreach (string key in data)
                {
                    s.AppendFormat("<input type='hidden' name='{0}' value='{1}' />", key, data[key]);
                }
                s.Append("</form></body></html>");
                HttpResponse response = HttpContext.Current.Response;
                response.Clear();
                response.Write(s.ToString());
                response.End();
            }
            catch (Exception ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(HttpContext.Current.Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "_RedirectWithData()" + " InitiatePayment.aspx");
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
                    RedirecttoLoginBtn.Text = "Confirm";
                    btnCancel.Text = "Cancel";
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "none", "<script> $(window).on('load', function () { $('#myModal').modal({ backdrop: 'static' });});</script>", false);
                }
                else if (ConfirmCancel.Equals("Cancel"))
                {
                    lblMSG1.Text = message;
                    RedirecttoLoginBtn.Visible = false;
                    btnCancel.Text = "Okay";
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "none", "<script> $(window).on('load', function () { $('#myModal').modal({ backdrop: 'static' });});</script>", false);
                }
            }
            catch (Exception ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "ShowUserAlert()" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
            }
        }
        public Tuple<string, int?, string> getPendingeChallanDetails(string appid)
        {
            int renewalflag = Convert.ToInt32(Session["Renewal"].ToString()); // if 1:new, 2:renewal
            var tuple = new Tuple<string, int?, string>("", null, "");
            NpgsqlConnection conn = new NpgsqlConnection();
            NpgsqlCommand cmd = new NpgsqlCommand();
            conn.ConnectionString = ConfigurationManager.ConnectionStrings["connect"].ConnectionString;
            cmd.Connection = conn;
            try
            {
                conn.Open();
                string srh_query = "select echallan_no,echallangeneratedon,status from esociety.online_payment_details where app_id=@app_id and active='Y' and onlinepayment_id=@onlinepayment_id";
                cmd.CommandText = srh_query;
                cmd.Parameters.AddWithValue("@app_id", Convert.ToInt64(appid));
                cmd.Parameters.AddWithValue("@onlinepayment_id", renewalflag);
                NpgsqlDataReader rd = cmd.ExecuteReader();
                if (rd.Read())
                {
                    string echallanno = Server.HtmlEncode(rd["echallan_no"].ToString());
                    DateTime temp = (DateTime)rd["echallangeneratedon"];
                    string status =Server.HtmlEncode(rd["status"].ToString());
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
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "getPendingeChallanDetails()" + " InitiatePayment.aspx");
                tuple = new Tuple<string, int?, string>("", null, "");
                
            }
            finally
            {
                conn.Close();

            }
            return tuple;
        }
        public int SoftDeleteEchallanEntry(string eChallanNo, string eChallanStatus)
        {
            int returnVal = 0;
            int renewalflag = Convert.ToInt32(Session["Renewal"].ToString()); // if 1:new, 2:renewal

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
                cmd.Parameters.AddWithValue("@onlinepayment_id", renewalflag);
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
        public byte[] print_ereceipt(string echallan_no)
        {
            string strUtf8_StatusReq_xml = "", encrypted_StatusReq = "", pdf = string.Empty;
            var xmlArgs = new Dictionary<string, string>();
            NICEncryption _encryption = new NICEncryption();
            byte[] pdf3 = null;
            ByteEncryption.ByteEncryption obj_Byte_Encryption = new ByteEncryption.ByteEncryption();
            try
            {
                using (var onPayRef = new PayOnlineRef.service())
                {
                    xmlArgs["eChallanNo"] = echallan_no;
                    strUtf8_StatusReq_xml = GetPaymentStatusXML(xmlArgs);
                    encrypted_StatusReq = _encryption.Encrypt(strUtf8_StatusReq_xml, System.Web.HttpContext.Current.Server.MapPath("~/RTO.KEY"));
                    ServicePointManager.Expect100Continue = true;
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                          
                    //ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;
                //    ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => false;
                    ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, ssl) => { return true; }; //= delegate { return true; };
                    // var statusResult = onPayRef.ereceipt(GlobalVars.SocietyDemandCode, encrypted_StatusReq);
                    DataSet statusResult = onPayRef.ereceipt(GlobalVars.SocietyDemandCode, encrypted_StatusReq);
                    if (statusResult != null && statusResult.Tables.Count > 0)
                    {
                        DataRow dtRow = statusResult.Tables[0].Rows[0];

                        string error = statusResult.Tables[0].Rows[0][0].ToString().Trim();

                        if (error == "Y")
                        {
                            for (int i = 0; i < statusResult.Tables[0].Rows.Count; i++)
                            {
                                byte[] buffer = (byte[])(statusResult.Tables[0].Rows[i][3]);
                                byte[] bytes = obj_Byte_Encryption.DecryptData(buffer, System.Web.HttpContext.Current.Server.MapPath("~/RTO.KEY"));

                                statusResult.Tables[0].Rows[i][3] = bytes;//encrytion of pdf bytes
                                statusResult.Tables[0].Rows[i][2] = _encryption.Decrypt(statusResult.Tables[0].Rows[i][2].ToString(), System.Web.HttpContext.Current.Server.MapPath("~/RTO.KEY"));//encryption echallan no
                                pdf3 = (byte[])(statusResult.Tables[0].Rows[0][3]);
                            }
                        }
                        else
                        {
                            pdf3 = null;
                        }
                    }
                    else
                    {
                        pdf3 = null;
                    }
                }

            }
            catch (Exception ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "print_ereceipt()" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
                return null;
            }

            return pdf3;

        }
        protected void RedirecttoLoginBtn_Click(object sender, EventArgs e)
        {

        }
        protected void convertToPdf(byte[] mssg)
        {
            try
            {
                byte[] tmpfiledata = mssg;
                string sPathToSaveFileTo = Server.MapPath("~/SelectedFile.pdf");
                using (System.IO.FileStream fs = new System.IO.FileStream(sPathToSaveFileTo, System.IO.FileMode.Create, System.IO.FileAccess.ReadWrite))
                {

                    using (System.IO.BinaryWriter bw = new System.IO.BinaryWriter(fs))
                    {
                        bw.Write(tmpfiledata);
                        bw.Close();
                    }
                }
                WebClient User = new WebClient();
                Byte[] FileBuffer = User.DownloadData(sPathToSaveFileTo);
            }
            catch (Exception ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "convertToPdf()" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
            }            
        }

    }

}