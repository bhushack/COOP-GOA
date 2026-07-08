using System;
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
    public partial class CertifiedCopy : System.Web.UI.Page
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
                if (!IsPostBack)
                {

                    loadnewappln();
                    loadarchive();

                }
            }
        }


        private bool checkroles()
        {
            bool role_hacked = false;
            List<int> AllowedRoles = new List<int> { 3, 4 };
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


        protected void loadnewappln()
        {
            NpgsqlConnection conn = new NpgsqlConnection();
            NpgsqlCommand cmd = new NpgsqlCommand();
            conn.ConnectionString = ConfigurationManager.ConnectionStrings["connect"].ConnectionString;
            cmd.Connection = conn;
            try
            {
                string query = "";
                query = "select certifiedcopies.app_id, certifiedcopies.cert_guid,certifiedcopies.active, socname, socregid, date(certifiedcopies.created_at) as appliedon,";
                query = query + " certifiedcopies.echallan_no, online_payment_details.echallan_rcpt_cross_entry";
                query = query + " from esociety.certifiedcopies INNER JOIN esociety.online_payment_details on esociety.online_payment_details.app_id = certifiedcopies.app_id";
                query = query + " where certifiedcopies.active = 'Y' and  certifiedcopies.status = 2 and certifiedcopies.districtid = @district and online_payment_details.active = 'Y' and online_payment_details.onlinepayment_id = 3 "; 
                conn.Open();
                cmd.CommandText = query;
                cmd.Parameters.AddWithValue("@district", Convert.ToInt32(Session["DistrictID"].ToString()));
                NpgsqlDataReader dr = cmd.ExecuteReader();
                newappln.DataSource = dr;
                newappln.DataBind();
                dr.Close();
                Label2.Text = Server.HtmlEncode(newappln.Rows.Count.ToString());
            }
            catch (NpgsqlException ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "loadnewappln()" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));               
                Response.Write("<script language='javascript'>alert('" + "Exception at Certified Copy List" + "')</script>");
            }
            finally
            {
                conn.Close();
            }
        }

        protected void loadarchive()
        {
            NpgsqlConnection conn = new NpgsqlConnection();
            NpgsqlCommand cmd = new NpgsqlCommand();
            conn.ConnectionString = ConfigurationManager.ConnectionStrings["connect"].ConnectionString;
            cmd.Connection = conn;
            try
            {

                string query = "select certifiedcopies.app_id, certifiedcopies.cert_guid, certifiedcopies_crosstable.docname, certifiedcopies.active, socname, socregid, date(certifiedcopies.created_at) as appliedon";
                query = query + " from esociety.certifiedcopies join esociety.certifiedcopies_crosstable on certifiedcopies.cert_guid =  certifiedcopies_crosstable.cert_guid where certifiedcopies.active = 'C' and certifiedcopies.districtid = @district";
               
                conn.Open();
                cmd.CommandText = query;
                cmd.Parameters.AddWithValue("@district", Convert.ToInt32(Session["DistrictID"].ToString()));
                NpgsqlDataReader dr = cmd.ExecuteReader();
                gv_certcopyarchive.DataSource = dr;
                gv_certcopyarchive.DataBind();
                dr.Close();
                //Label3.Text = Server.HtmlEncode(gv_certcopyarchive.Rows.Count.ToString());
            }
            catch (NpgsqlException ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "loadarchive()" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
                Response.Write("<script language='javascript'>alert('" + "Exception at Certified Copy List" + "')</script>");
            }
            finally
            {
                conn.Close();
            }
        }

        protected void LkReceipt_Click(object sender, EventArgs e)
        {

            LinkButton btn = (LinkButton)sender;
            GridViewRow row = (GridViewRow)btn.NamingContainer;
            HiddenField hd = row.FindControl("hdreceipt") as HiddenField;
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
                    Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "LkReceipt_Click" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));

                    Response.Write("<script language='javascript'>alert('" + "PDF View Failed Error" + "')</script>");
                }
                finally
                {
                    client = null;
                }
            }
            else { Response.Write("<script language='javascript'>alert('" + "PDF View Failed " + "')</script>"); }
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
            
        }


        protected void permission_Click(object sender, EventArgs e)
        {
            Response.Redirect("Dashboard.aspx");
        }

       

        protected void Lkredirect_Click(object sender, EventArgs e)
        {
            try
            {
                LinkButton btn = (LinkButton)sender;
                GridViewRow row = (GridViewRow)btn.NamingContainer;
                string app_id = ((Label)newappln.Rows[row.RowIndex].FindControl("LbApplicationID")).Text;
                Session["app_id"] = app_id;
                Response.Redirect("PrintCertifiedCopy.aspx");
            }
            catch (Exception ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "Lkredirect_Click()" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath) + " from " + ipaddress);
            }
        }

        protected void gv_certcopyarchive_DataBound(object sender, EventArgs e)
        {
           
            for (int i = gv_certcopyarchive.Rows.Count - 1; i > 0; i--)
            {
                GridViewRow row = gv_certcopyarchive.Rows[i];
                Label rguid = row.FindControl("LBGuid") as Label;
                GridViewRow previousRow = gv_certcopyarchive.Rows[i - 1];
                Label pguid = previousRow.FindControl("LBGuid") as Label;

                if (rguid.Text == pguid.Text)
                {
                   // previousRow.Cells[0].Text = inc.ToString(); inc++;
                    for (int j = 0; j < 7; j++)
                    {

                        if (previousRow.Cells[j].RowSpan == 0)
                        {
                            if (j != 6)
                            {
                                if (row.Cells[j].RowSpan == 0)
                                {
                                    previousRow.Cells[j].RowSpan += 2;
                                }
                                else
                                {
                                    previousRow.Cells[j].RowSpan = row.Cells[j].RowSpan + 1;
                                }
                                row.Cells[j].Visible = false;
                            }
                            else if (j == 6)
                            {
                                string temp = previousRow.Cells[j].Text + " , " + row.Cells[j].Text;
                                previousRow.Cells[j].Text = temp;
                                row.Cells[j].Visible = false;

                            }
                        }

                    }
                }

              
            }

            int inc = 1;
            foreach (GridViewRow row1 in gv_certcopyarchive.Rows)
            {
                if (row1.Cells[0].Visible == true)
                {
                    row1.Cells[0].Text = inc.ToString(); inc++;
                }
            }

            Label3.Text = (inc-1).ToString();



        }
        
    
    }
}