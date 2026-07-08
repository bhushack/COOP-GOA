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
using WS_Encryption;

namespace GoaSocietyRegistration.Admin
{
    public partial class ViewPdf : System.Web.UI.Page
    {
        byte[] bytes1;
        ByteEncryption.ByteEncryption obj_Byte_Encryption = new ByteEncryption.ByteEncryption();
        NICEncryption _encryption = new NICEncryption();
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
                if (Session["loadflag"].ToString() == "stop")
                {
                    fmissingerror.Visible = true;
                }
                else
                {
                    // msg.Visible = false;
                    int docid = Convert.ToInt32(Session["docid"].ToString());
                    if (Session["ObjectID"].ToString() != "aa")
                    {
                        bytes1 = openConnectionMongo_societyDocs(Session["ObjectID"].ToString(), Session["collection"].ToString(), docid);
                    }
                    else
                    {
                        var status = getPageStatus(Convert.ToInt64(Session["app_id"].ToString()), Convert.ToInt32(Session["myid"]));
                        if (status.Item1 == "False") 
                        {
                            // msg.Visible = true;
                            fmissingerror.Visible = true;
                        }
                        else
                        {
                        string objectid = status.Item2;
                        bytes1 = openConnectionMongo_societyDocs(objectid, Session["collection"].ToString(), docid);
                        }
                    }
                    if (bytes1 != null)
                    {
                        convertToPdf(bytes1);
                        Session["myid"] = 0;
                        Session["loadflag"] = "stop";
                    }
                    else
                    {
                        fmissingerror.Visible = true;
                        CreateLogFiles Err = new CreateLogFiles();
                        Err.ErrorLog(HttpContext.Current.Server.MapPath("~/Logs/ErrorLog"), "Missing File", "PageLoad---ViewPdf");
                    }                  
                }
            }

        }
        private bool Check4Tampering()
        {


            bool sessHackedCheck = false, pagesessionhack = false;
            //HOST NAME check
            if (Request.UrlReferrer != null)
            {
                Uri uri = new Uri(Request.UrlReferrer.ToString());
                string referer = uri.Scheme + Uri.SchemeDelimiter + uri.Host;
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
                    }
                }
                else
                    sessHackedCheck = true;
            }
            else
            {

                sessHackedCheck = true;
            }
            return sessHackedCheck;
        }
  
        protected void convertToPdf(byte[] mssg)
        {
            try
            {
                if (mssg != null)
                {


                    byte[] tmpfiledata = mssg;
                    // Session["pdfpath"] = "~/OutData/" + Session["app_id"].ToString() + "_org.pdf";
                    string sPathToSaveFileTo = Server.MapPath("~/OutData/" + Session["org_pdf"].ToString() + "_org.pdf");
                    using (System.IO.FileStream fs = new System.IO.FileStream(sPathToSaveFileTo, System.IO.FileMode.Create))
                    using (System.IO.BinaryWriter bw = new System.IO.BinaryWriter(fs))
                    {
                        bw.Write(tmpfiledata);
                        //RecordUserAction("convertToImage", "File written to Image", "S");
                        bw.Close();
                    }
                    WebClient User = new WebClient();
                    Byte[] FileBuffer = User.DownloadData(sPathToSaveFileTo);
                    if (FileBuffer != null)
                    {

                        Response.ContentType = "application/pdf";
                        //        //Response.AddHeader("content-length", FileBuffer.Length.ToString());
                        Response.Write("window.open('sPathToSaveFileTo','_blank');");
                        Response.BinaryWrite(FileBuffer);
                        //        // RecordUserAction("convertToPdf", "byte[] to pdf", "S");
                    }
                }
            }
            catch (Exception ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "convertToPdf()" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
            }
        }
        protected byte[] openConnectionMongo_societyDocs(string objectid, string collection_name, int docid)
        {//docid=1-->MemberDocs 2---->OtherDocuments
            try
            {
                var str = ConfigurationManager.AppSettings["mongoconnect"];
                IMongoDatabase database;
                IMongoClient client;
                client = new MongoClient(str);
                database = client.GetDatabase("eGoaSociety");
                byte[] decrypt_pdf_bytes = null;
                if (docid == 1)
                {
                    var collection = database.GetCollection<MemberDocs>(collection_name);
                    var status = collection.Find(x => x._Id == MongoDB.Bson.ObjectId.Parse(objectid)).FirstOrDefault();
                    byte[] decrypt_bytes = status.DocContent;
                    decrypt_pdf_bytes = obj_Byte_Encryption.DecryptData(decrypt_bytes, System.Web.HttpContext.Current.Server.MapPath("~/ENCRYPT.KEY"));
                }
                else
                {
                    var collection = database.GetCollection<OtherDocuments>(collection_name);
                    var status = collection.Find(x => x._Id == MongoDB.Bson.ObjectId.Parse(objectid)).FirstOrDefault();

                   byte[] decrypt_bytes = status.DocContent;
                    decrypt_pdf_bytes = obj_Byte_Encryption.DecryptData(decrypt_bytes, System.Web.HttpContext.Current.Server.MapPath("~/ENCRYPT.KEY"));
                }
               
                return decrypt_pdf_bytes;
            }
            catch (MongoException ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "openConnectionMongo_societyDocs()" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
                
                //RecordUserAction("openConnectionMongo_societyDocs", ex.Message, "F");
                Response.Write("<script language='javascript'>alert('" + "PDF View Failed" + ".No file uploaded yet')</script>");
                return null;
            }
        }
        public static Tuple<string, string> getPageStatus(Int64 app_id, int myid)
        {
            var tuple = new Tuple<string, string>("", "");
            NpgsqlConnection conn = new NpgsqlConnection();
            NpgsqlCommand cmd = new NpgsqlCommand();
            conn.ConnectionString = ConfigurationManager.ConnectionStrings["connect"].ConnectionString;
            cmd.Connection = conn;
            try
            {
                conn.Open();
                 string query = "select object_id, active from esociety.otherdoc_crossentry where app_id=@appid and myid=@myid";
          
                cmd.CommandText = query;
                cmd.Parameters.AddWithValue("@appid", app_id);
                cmd.Parameters.AddWithValue("@myid", myid);
                NpgsqlDataReader rd = cmd.ExecuteReader();             
                if (rd.Read())
                {
                    string active = rd["active"].ToString();
                    string objectid = rd["object_id"].ToString();
                    tuple = new Tuple<string, string>(active, objectid);

                }
                else
                {
                    string active = "False";
                    string objectid = "";
                    tuple = new Tuple<string, string>(active, objectid);

                }
                rd.Close();
            }
            catch (NpgsqlException ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(HttpContext.Current.Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "getPageStatus()-----ViewPdf.cs");
                //RecordUserAction("getPageStatus()", ex.Message, "S");
                //Response.Write("<script language='javascript'>alert('" + ex.Message + "')</script>");
                string active = "False";
                string objectid = "";
                tuple = new Tuple<string, string>(active, objectid);

            }
            finally
            {
                conn.Close();
            }
            return tuple;
        }

    }
}