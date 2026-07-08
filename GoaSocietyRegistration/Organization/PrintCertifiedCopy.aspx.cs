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
    public partial class PrintCertifiedCopy : System.Web.UI.Page
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
                loaddocsappliedfor();
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


        protected void loaddocsappliedfor()
        {
            NpgsqlConnection conn = new NpgsqlConnection();
            NpgsqlCommand cmd = new NpgsqlCommand();
            conn.ConnectionString = ConfigurationManager.ConnectionStrings["connect"].ConnectionString;
            cmd.Connection = conn;
            try
            {                
                string query = "";
                query = "select certifiedcopies.app_id, certifiedcopies.cert_guid,certifiedcopies.active, socname, socregid, docname, myid,";
                query = query + " certifiedcopies_crosstable.noofcopies, certifiedcopies.echallan_no, online_payment_details.echallan_rcpt_cross_entry";
                query = query + " from esociety.certifiedcopies inner join esociety.certifiedcopies_crosstable on esociety.certifiedcopies.cert_guid = esociety.certifiedcopies_crosstable.cert_guid";
                query = query + " INNER JOIN esociety.online_payment_details on esociety.online_payment_details.app_id = certifiedcopies.app_id where certifiedcopies.app_id=@app_id and certifiedcopies.active = 'Y'";
                query = query + "  and certifiedcopies.status = 2 and certifiedcopies.districtid = @district and online_payment_details.active = 'Y' and online_payment_details.onlinepayment_id = 3 "; 



                conn.Open();
                cmd.CommandText = query;
                cmd.Parameters.AddWithValue("@app_id", Convert.ToInt64(Session["app_id"].ToString()));
                cmd.Parameters.AddWithValue("@district", Convert.ToInt32(Session["DistrictID"].ToString()));
                NpgsqlDataReader dr = cmd.ExecuteReader();
                //gv_docs.DataSource = dr;
                //gv_docs.DataBind();

                if (dr.Read())
                {
                    lblSocname.Text = Server.HtmlEncode(dr["socname"].ToString());
                    lblsocregno.Text = Server.HtmlEncode(dr["socregid"].ToString());
                }
                dr.Close();

                NpgsqlDataReader dr1 = cmd.ExecuteReader();

                gv_docs.DataSource = dr1;
                gv_docs.DataBind();
                dr1.Close();

                RecordUserAction("Read", "Documents Loaded in gridview", "S", Session["app_id"].ToString(), 1);



            }
            catch (NpgsqlException ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "loaddocsappliedfor()" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));

                Response.Write("<script language='javascript'>alert('" + "Exception at Certified Copy Documents List" + "')</script>");
                RecordUserAction("Read", "Error while loading docs in gridview", "F", Session["app_id"].ToString(), 1);
            }
            finally
            {
                conn.Close();
            }
        }

        protected void LbDownload_Click(object sender, EventArgs e)
        {
            LinkButton btn = (LinkButton)sender;
            GridViewRow row = (GridViewRow)btn.NamingContainer;

            Label myid = row.FindControl("lblid") as Label;
            Label app_id = row.FindControl("lblAppid") as Label;

            int id = Convert.ToInt32(myid.Text);
            string objectid = "";
            if (id != 100)
            {

                objectid = getobjID(Convert.ToInt64(app_id.Text.ToString()), id);
            }
            else
            {
                objectid = getfinalcertmongo();
            }

            if (objectid != "" && objectid != null)
            {               
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
                        convertToPdf(pdf, id);
                    }
                    else if (Convert.ToInt32(myid.Text) == 100)
                    {
                            var collection = database.GetCollection<Society_Certificate>("Society Certificate");
                            var status = collection.Find(x => x._Id == MongoDB.Bson.ObjectId.Parse(objectid)).FirstOrDefault();
                            byte[] decrypt_pdf_bytes = status.DocContent;
                            byte[] pdf = obj_Byte_Encryption.DecryptData(decrypt_pdf_bytes, System.Web.HttpContext.Current.Server.MapPath("~/ENCRYPT.KEY"));
                            convertToPdf(pdf, id);
                      
                    }
                    else
                    {
                        var collection = database.GetCollection<OtherDocuments>("Society Documents");
                        var status = collection.Find(x => x._Id == ObjectId.Parse(objectid)).FirstOrDefault();
                        byte[] decrypt_pdf_bytes = status.DocContent;
                        byte[] pdf = obj_Byte_Encryption.DecryptData(decrypt_pdf_bytes, System.Web.HttpContext.Current.Server.MapPath("~/ENCRYPT.KEY"));
                        convertToPdf(pdf, id);
                    }

                    RecordUserAction("Read", "LbDownload_Click", "S", Session["app_id"].ToString(), 1);


                }
                catch (MongoException ex)
                {
                    CreateLogFiles Err = new CreateLogFiles();
                    Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "LbDownload_Click" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath) + " from " + ipaddress);
                   
                    Response.Write("<script language='javascript'>alert('" + "Pdf Viewed Failed" + "')</script>");
                }
            }
            else
            {

                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), "DB id is null. Please Check", "LbView_Click");
              
                Response.Write("<script language='javascript'>alert('" + "Pdf laoding Failed" + "')</script>");

            }

        }

        protected void convertToPdf(byte[] mssg, int id)
        {

            try
            {
                byte[] tmpfiledata = mssg;
                string path = "~/OutData/ " + Session["app_id"].ToString() + ".pdf";
                string sPathToSaveFileTo = Server.MapPath(path);
                using (System.IO.FileStream fs = new System.IO.FileStream(sPathToSaveFileTo, System.IO.FileMode.Create, System.IO.FileAccess.ReadWrite))
                {
                    using (System.IO.BinaryWriter bw = new System.IO.BinaryWriter(fs))
                    {
                        bw.Write(tmpfiledata);
                        //RecordUserAction("convertToPdf", "File written to pdf", "S");
                        bw.Close();
                    }
                }
                WebClient User = new WebClient();
                Byte[] FileBuffer = User.DownloadData(sPathToSaveFileTo);

                if (FileBuffer != null)
                {
                    Response.Clear();
                    MemoryStream ms = new MemoryStream(tmpfiledata);
                    Response.ContentType = "application/pdf";
                    string app_id = Session["app_id"].ToString();
                    if (id == 100)
                    {
                        string name = app_id + "_Certificate.pdf";
                        Response.AddHeader("content-disposition", "attachment;filename=" + name);
                    }
                    else if (id == 2)
                    {
                        string name = app_id + "_MemorandumofAssociation.pdf";
                        Response.AddHeader("content-disposition", "attachment;filename=" + name);
                    }
                    else if (id == 3)
                    {
                        string name = app_id + "_RulesandRegulaions.pdf";
                        Response.AddHeader("content-disposition", "attachment;filename=" + name);
                    }
                    else if (id == 0)
                    {
                        string name = app_id + "_Additionaldoc.pdf";
                        Response.AddHeader("content-disposition", "attachment;filename=" + name);
                    }
                    
                    else { }

                    Response.Buffer = true;
                    ms.WriteTo(Response.OutputStream);
                    Response.End();
                }
            }
            catch (Exception ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "convertToPdf()" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath) + " from " + ipaddress);
            }
        }


        protected string getobjID(Int64 appid, int myid)
        {
            string retval = "";
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

                    retval = objectid;
                }
                else
                {
                    retval = "";
                }
                dr.Close();
            }
            catch (NpgsqlException ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "getobjID()" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
                Response.Write("<script language='javascript'>alert('" + "Not getting PDF ID" + "')</script>");
                retval = "";
            }
            finally
            {

                conn.Close();
            }
            return retval;
        }

        protected string getfinalcertmongo()
        {
            string retval = "";
            NpgsqlConnection conn = new NpgsqlConnection();
            NpgsqlCommand cmd = new NpgsqlCommand();
            conn.ConnectionString = ConfigurationManager.ConnectionStrings["connect"].ConnectionString;
            cmd.Connection = conn;
            try
            {
                conn.Open();

                string socquery = "Select socname, socregid, final_certificate_mongo_entry from esociety.society where active = 'Y' and app_id=@app_id";

                cmd.CommandText = socquery;
                cmd.Parameters.AddWithValue("@app_id", Convert.ToInt64(Session["app_id"].ToString()));
                NpgsqlDataReader dr = cmd.ExecuteReader();

                if (dr.Read())
                {                  
                    string objid = Server.HtmlEncode(dr["final_certificate_mongo_entry"].ToString());
                    retval = objid;
                }
                
                dr.Close();               


            }
            catch (NpgsqlException ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "getfinalcertmongo" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
                Response.Write("<script language='javascript'>alert('" + "Error while loading details" + "')</script>");
            }
            finally
            {
                conn.Close();
            }
            return retval;
        }

        protected void issue_confirm_modal_btn_Click(object sender, EventArgs e)
        {
            string user_fullname = Session["userfirstname"].ToString() + " " + Session["usermiddlename"].ToString() + " " + Session["userlastname"].ToString();
            NpgsqlConnection conn = new NpgsqlConnection();
            NpgsqlCommand cmd = new NpgsqlCommand();
            conn.ConnectionString = ConfigurationManager.ConnectionStrings["connect"].ConnectionString;
            cmd.Connection = conn;            
            conn.Open();
            NpgsqlTransaction trans = conn.BeginTransaction();
            try
            {
                string upd_query = "UPDATE esociety.certifiedcopies SET issued_by_name=@issued_by_name, issued_by_email = @issued_by_email, issued_at = current_timestamp, issued_ipaddress=@ipaddress, active='C',status=3 where app_id = @app_id and active='Y'";
                cmd.CommandText = upd_query;
                cmd.Parameters.AddWithValue("@issued_by_name", user_fullname);
                cmd.Parameters.AddWithValue("@issued_by_email", Session["firstname"].ToString());
                cmd.Parameters.AddWithValue("@ipaddress", ipaddress);           
                cmd.Parameters.AddWithValue("@app_id", Convert.ToInt64(Session["app_id"].ToString()));
                cmd.ExecuteNonQuery();

                string query1 = "UPDATE esociety.certifiedcopies_crosstable SET active='C' where app_id = @app_id and active='Y'";
                cmd.CommandText = query1;
                cmd.Parameters.AddWithValue("@issued_by_name", user_fullname);
                cmd.Parameters.AddWithValue("@issued_by_email", Session["firstname"].ToString());
                cmd.Parameters.AddWithValue("@ipaddress", ipaddress);
                cmd.Parameters.AddWithValue("@app_id", Convert.ToInt64(Session["app_id"].ToString()));
                cmd.ExecuteNonQuery();

                trans.Commit();
                RecordUserAction("Update", "Issue Certified Copies Button Click", "S", Session["app_id"].ToString(), 1);
                Response.Redirect("CertifiedCopy.aspx");
            }
            catch (NpgsqlException ex)
            {
                trans.Rollback();
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "issue_confirm_modal_btn_Click()" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
                RecordUserAction("Update", "Issue Certified Copies Button Click", "F", Session["app_id"].ToString(), 1);
                Response.Write("<script language='javascript'>alert('" + "Exection Error" + "')</script>");
            }
            finally
            {
                conn.Close();
            }
        }

        protected void Lkissue_Click(object sender, EventArgs e)
        {
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "none", "<script> $(window).on('load', function () { $('#issue_confirmation_modal').modal({ backdrop: 'static' });});</script>", false);
        }

        protected void LkBack_Click(object sender, EventArgs e)
        {          
            Response.Redirect("CertifiedCopy.aspx");
         
        }
    }
}