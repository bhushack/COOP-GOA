using GoaSocietyRegistration.Development;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;


namespace GoaSocietyRegistration.Organization
{
    public partial class EchallanStatus : System.Web.UI.Page
    {
        string ipaddress = Utility.getIP();
        string macaddress = Utility.GetMACAddress(Utility.getIP());
        Validate val = new Validate();
        Insert ins = new Insert();
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Check4Tampering() || Page.RouteData.RouteHandler == null)
            {
                RecordUserAction("Load", "Tampered Session on Page_Load", "Failed", "NA", 2);
                SessionManage session = new SessionManage();
                session.__AbandonAdmin(Request, Response);
            }
            else if (checkroles())
            {

                RecordUserAction("Load", "Unauthorized Access - Role", "Failed", "NA", 2);
                Label5.Text = "Permission Denied. Please Contact to your Admin.";
                Label5.ForeColor = System.Drawing.Color.Red;
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "none", "<script> $(window).on('load', function () { $('#permission_error_modal').modal({ backdrop: 'static' });});</script>", false);

            }
            else
            {
                HttpContext.Current.Response.AddHeader("Cache-Control", "no-cache, no-store, must-revalidate");
                HttpContext.Current.Response.AddHeader("Pragma", "no-cache");
                HttpContext.Current.Response.AddHeader("Expires", "0");
                statustable.Visible = false;
            }
        }

        private bool checkroles()
        {
            bool role_hacked = false;
            List<int> AllowedRoles = new List<int> { 1, 2, 3, 4 };
            if (Context.Session != null && !AllowedRoles.Contains(Convert.ToInt32(Session["ROLE"])))
                role_hacked = true;
            return role_hacked;
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

        protected void CheckStatus_Click(object sender, EventArgs e)
        {
            try
            {
                if (TxtBxechallanno.Text == null || TxtBxechallanno.Text == "")
                {
                    callErrorModal("Please Enter eChallan no");
                }
                else if (!val.validateData(TxtBxechallanno.Text, val.echallanno_regex))
                {
                    callErrorModal("Please Enter Valid eChallan no");
                }
                else
                {
                    printstatus(TxtBxechallanno.Text);
                }
            }
            catch (Exception ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "CheckStatus_Click()" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath) + " from " + ipaddress);
            }
        }



        protected void printstatus(string echallan_no)
        {

            InitiatePayment ins = new InitiatePayment();

            string temp_echallanstatus = ins.GetEchallanPaymentStatus(echallan_no);
            string[] Result = temp_echallanstatus.Split('|');
            string echallanstatus = Result[0];

            XmlDocument PayStatusXML = new XmlDocument();
            PayStatusXML.XmlResolver = null;
            PayStatusXML.LoadXml(Result[1]);
            string eChallanStatus = PayStatusXML.GetElementsByTagName("status").Item(0).InnerText.Trim();
            string total_amt = PayStatusXML.GetElementsByTagName("totalAmount").Item(0).InnerText.Trim();
            string bank_ref_no = PayStatusXML.GetElementsByTagName("sbiReferenceNo").Item(0).InnerText.Trim();
            string bank_rcvd_date = PayStatusXML.GetElementsByTagName("bankReceiveDate").Item(0).InnerText.Trim();
            string treasury_rcvd_date = PayStatusXML.GetElementsByTagName("treasuryReceiveDate").Item(0).InnerText.Trim();
            textdisplay.Text = "Echallan Status    " + Sanitize.InputText(PayStatusXML.GetElementsByTagName("status").Item(0).InnerText.Trim()) + "<br>" +
            "Total Amount   " + Sanitize.InputText(PayStatusXML.GetElementsByTagName("totalAmount").Item(0).InnerText.Trim()) + "<br>" +
              "Bank Reference No   " + Sanitize.InputText(PayStatusXML.GetElementsByTagName("sbiReferenceNo").Item(0).InnerText.Trim()) + "<br>" +
              "Bank Recieved Date   " + Sanitize.InputText(PayStatusXML.GetElementsByTagName("bankReceiveDate").Item(0).InnerText.Trim()) + "<br>" +
               "Treasury Recieved Date   " + Sanitize.InputText(PayStatusXML.GetElementsByTagName("treasuryReceiveDate").Item(0).InnerText.Trim());
        }




        protected void callErrorModal(string msg)
        {
            Label50.Text = Server.HtmlEncode(msg);
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "none", "<script> $(window).on('load', function () { $('#msgmodal').modal({ backdrop: 'static' });});</script>", false);
        }

        protected void permission_Click(object sender, EventArgs e)
        {
            // RecordUserAction("Redirect", "Redirection to Dashboard", "Success", "NA", 1);
            Response.Redirect("Dashboard.aspx");
        }

        protected void Lkreset_Click(object sender, EventArgs e)
        {
            TxtBxechallanno.Text = "";
            textdisplay.Text = "";
        }
    }

}
