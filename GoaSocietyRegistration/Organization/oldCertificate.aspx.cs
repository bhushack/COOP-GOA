using GoaSocietyRegistration.Development;
using MongoDB.Driver;
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

namespace GoaSocietyRegistration.Organization
{
    public partial class oldCertificate : System.Web.UI.Page
    {
        ByteEncryption.ByteEncryption obj_Byte_Encryption = new ByteEncryption.ByteEncryption();
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Check4Tampering() || Page.RouteData.RouteHandler == null)
            {
                // RecordUserAction("Page_Load", "Access request failed. Tampered session", "F");
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
                        oldcertificateLoad();
                    }
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
        protected void oldcertificateLoad()
        {
            NpgsqlConnection conn = new NpgsqlConnection();
            NpgsqlCommand cmd = new NpgsqlCommand();
            conn.ConnectionString = ConfigurationManager.ConnectionStrings["connect"].ConnectionString;
            cmd.Connection = conn;
            try
            {
                string query = "";
                query = "select applicant_details.app_id,applicant_details.applicant_name,mst_memberdesignation.\"DesignationName\",";
                query = query + " society.socname,mst_societytype.societytype,esociety.status_table.status_id,esociety.society.final_certificate_mongo_entry from esociety.applicant_details";
                query = query + " INNER JOIN esociety.society on applicant_details.app_id = society.app_id";
                query = query + " INNER JOIN esociety.mst_memberdesignation on applicant_details.applicant_designation = mst_memberdesignation.\"DesignationID\"";
                query = query + " INNER JOIN esociety.mst_societytype on mst_societytype.societyid = society.soctype";
                query = query + " INNER JOIN esociety.status_table on esociety.status_table.app_id = esociety.applicant_details.app_id";
                query = query + " where esociety.status_table.status_id = 12 and applicant_district= @district";
                conn.Open();
                cmd.CommandText = query;
                cmd.Parameters.AddWithValue("@district", Convert.ToInt32(Session["DistrictID"].ToString()));
                NpgsqlDataReader dr = cmd.ExecuteReader();
                OldCertificate.DataSource = dr;
                OldCertificate.DataBind();
                dr.Close();
                Label3.Text = Server.HtmlEncode(OldCertificate.Rows.Count.ToString());
            }
            catch (NpgsqlException ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "oldcertificateLoad()" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
                //RecordUserAction("MemberDetails_Page_Load", "Exception at Member Data", "F");
                Response.Write("<script language='javascript'>alert('" + "Exception at Member Data" + "')</script>");
            }
            finally
            {
                conn.Close();
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
                    //RecordUserAction("Create", "convertToPdf -- Pdf Viewed", "Success", "NA", 1);
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
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "convertToPdf()" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath) + " from " + Utility.getIP());
            }
        }
        protected void oldcertificate_Click(object sender, EventArgs e)
        {
            try
            {
                LinkButton btn = (LinkButton)sender;
                GridViewRow row = (GridViewRow)btn.NamingContainer;
                string mongoentry = ((Label)OldCertificate.Rows[row.RowIndex].FindControl("mongoentryFINAL")).Text;
                if (mongoentry != null)
                {
                    byte[] bytes1 = openConnectionMongo_(mongoentry, "Society Certificate");
                    byte[] data = obj_Byte_Encryption.DecryptData(bytes1, System.Web.HttpContext.Current.Server.MapPath("~/ENCRYPT.KEY"));
                    convertToPdf(data);
                }
                else {

                    Response.Write("<script language='javascript'>alert('" + "PDF View Missing or failed" + ".No file uploaded yet')</script>");
                }
            }
            catch (Exception ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "oldcertificate_Click" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
            }
        }
    }
}