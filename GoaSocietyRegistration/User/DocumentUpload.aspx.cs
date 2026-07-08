using GoaSocietyRegistration.Development;
using iTextSharp.text.pdf;
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
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WS_Encryption;

namespace GoaSocietyRegistration
{    public partial class DocumentUpload : System.Web.UI.Page
    {       

        //static string appid;
        string ct = string.Empty;
        Int64 fileSizeFront = 0;
        byte[] documentBinary = new Byte[0];
        string ipaddress = Utility.getIP();
        string macaddress = Utility.GetMACAddress(Utility.getIP());
        //string macaddress = Utility.GetMACAddress();
        OtherDocuments doc = new OtherDocuments();
        NICEncryption _encryption = new NICEncryption();
        ByteEncryption.ByteEncryption obj_Byte_Encryption = new ByteEncryption.ByteEncryption();

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
                get_status();
                if (Session["login_id"] != null)
                {
                    if (!IsPostBack)
                    {
                        Session["du_flag1"] = 0;
                        Session["du_flag2"] = 0;
                        Session["du_flag3"] = 0;
                        Session["du_flag5"] = 0;
                        Session["du_flag6"] = 0;
                        Session["du_flag8"] = 0;
                        Session["du_flag9"] = 0;
                        Session["du_flag10"] = 0;
                        Session["du_flag11"] = 0;
                        Session["du_flag12"] = 0;
                        Session["du_complete_data"] = "";
                        Session["du_noofmembers"] = 0;
                    }
                    HttpContext.Current.Response.AddHeader("Cache-Control", "no-cache, no-store, must-revalidate");
                    HttpContext.Current.Response.AddHeader("Pragma", "no-cache");
                    HttpContext.Current.Response.AddHeader("Expires", "0");
                    string loginid = Session["login_id"].ToString();
                    //appid = Session["AppID"].ToString();
                    //app_id = Convert.ToInt32(Session["AppID"].ToString());

                    int renewalstatus = Utility.checkifrenewal(Session["AppID"].ToString());
                    if (renewalstatus == 1)
                    {
                        var fu1 = getPageStatus(Convert.ToInt64(Session["AppID"].ToString()), 1);
                        if (fu1.Item1 == "True")
                        {
                            RecordUserAction("Page_load", "File already Uploaded in mongo 1", "S");
                            lbfu1status.Text = "File Uploaded";
                            LB_Application_Upload.Enabled = false;
                            FileUpload1.Enabled = false;
                            LB_Application_View.Enabled = true;
                            Session["du_flag1"] = 1;
                        }
                        else
                        {
                            lbfu1status.Text = "";
                            LB_Application_Upload.Enabled = true;
                            FileUpload1.Enabled = true;
                            Session["du_flag1"] = 0;

                        }
                        var fu2 = getPageStatus(Convert.ToInt64(Session["AppID"].ToString()), 2);
                        if (fu2.Item1 == "True")
                        {
                            RecordUserAction("Page_load", "File already Uploaded in mongo 2", "S");
                            lbfu2status.Text = "File Uploaded";
                            LB_Memorandom_Upload.Enabled = false;
                            FileUpload2.Enabled = false;
                            LB_Memorandum_View.Enabled = true;
                            Session["du_flag2"] = 1;
                        }
                        else
                        {
                            lbfu2status.Text = "";
                            LB_Memorandom_Upload.Enabled = true;
                            FileUpload2.Enabled = true;
                            Session["du_flag2"] = 0;
                        }
                        var fu3 = getPageStatus(Convert.ToInt64(Session["AppID"].ToString()), 3);
                        if (fu3.Item1 == "True")
                        {
                            RecordUserAction("Page_load", "File already Uploaded in mongo 3", "S");
                            lbfu3status.Text = "File Uploaded";
                            LB_Bylaws_Upload.Enabled = false;
                            FileUpload3.Enabled = false;
                            LB_Bylaws_View.Enabled = true;
                            Session["du_flag3"] = 1;
                        }
                        else
                        {
                            lbfu3status.Text = "";
                            LB_Bylaws_Upload.Enabled = true;
                            FileUpload3.Enabled = true;
                            Session["du_flag3"] = 0;
                        }


                        //commenting Certificate
                        //var fu4 = getPageStatus(Convert.ToInt64(appid), 4);
                        //if (fu4.Item1 == "True")
                        //{
                        //    RecordUserAction("Page_load", "File already Uploaded in mongo 4", "S");
                        //    lbfu4status.Text = "File Uploaded";
                        //    LinkButton3.Enabled = false;
                        //    FileUpload7.Enabled = false;
                        //    LinkButton10.Enabled = true;
                        //    flag4 = 1;
                        //}
                        //else
                        //{
                        //    lbfu4status.Text = "";
                        //    LinkButton3.Enabled = true;
                        //    FileUpload7.Enabled = true;
                        //    flag4 = 0;
                        //}
                        ///////////////////////////////////////////
                        //var fu5 = getPageStatus(Convert.ToInt64(Session["AppID"].ToString()), 5);
                        //if (fu5.Item1 == "True")
                        //{
                        //    RecordUserAction("Page_load", "File already Uploaded in mongo 5", "S");
                        //    Label5.Text = "File Uploaded";
                        //    LinkButton5.Enabled = false;
                        //    FileUpload4.Enabled = false;
                        //    LinkButton11.Enabled = true;
                        //    LinkButton6.Enabled = true;
                        //    Session["du_flag5"] = 1;
                        //    getdocname1();
                        //}
                        //else if (fu5.Item1 == "" || fu5.Item1 == null)
                        //{
                        //    Label5.Text = "";
                        //    LinkButton5.Enabled = true;
                        //    FileUpload4.Enabled = true;
                        //    Session["du_flag5"] = 0;
                        //}
                        //else
                        //{
                        //    Label5.Text = "";
                        //    LinkButton5.Enabled = true;
                        //    FileUpload4.Enabled = true;
                        //    Session["du_flag5"] = 0;
                        //}
                        /////////////////////////////////////////////
                        //var fu6 = getPageStatus(Convert.ToInt64(Session["AppID"].ToString()), 6);
                        //if (fu6.Item1 == "True")
                        //{
                        //    RecordUserAction("Page_load", "File already Uploaded in mongo 6", "S");
                        //    Label10.Text = "File Uploaded";
                        //    LinkButton7.Enabled = false;
                        //    FileUpload5.Enabled = false;
                        //    LinkButton12.Enabled = true;
                        //    LinkButton8.Enabled = true;
                        //    Session["du_flag6"] = 1;
                        //    getdocname2();
                        //}
                        //else if (fu6.Item1 == "" || fu6.Item1 == null)
                        //{
                        //    Label10.Text = "";
                        //    LinkButton7.Enabled = true;
                        //    FileUpload5.Enabled = true;
                        //    Session["du_flag6"] = 0;
                        //}
                        //else
                        //{
                        //    Label10.Text = "";
                        //    LinkButton7.Enabled = true;
                        //    FileUpload5.Enabled = true;
                        //    Session["du_flag6"] = 0;
                        //}
                    }
                    else if (renewalstatus == 2)
                    {
                        var rfu1 = getPageStatus(Convert.ToInt64(Session["AppID"].ToString()), 8);//Application oF Renewal
                        if (rfu1.Item1 == "True")
                        {
                            RecordUserAction("Page_load", "File already Uploaded in mongo 8", "S");
                            lbrfu1status.Text = "File Uploaded";
                            Lb_RenewalApplication_Upload.Enabled = false;
                            RenewalFileUpload1.Enabled = false;
                            Lb_RenewalApplication_View.Enabled = true;
                            Session["du_flag8"] = 1;
                        }
                        else
                        {
                            lbrfu1status.Text = "";
                            Lb_RenewalApplication_Upload.Enabled = true;
                            RenewalFileUpload1.Enabled = true;
                            Session["du_flag8"] = 0;

                        }

                        var rfu2 = getPageStatus(Convert.ToInt64(Session["AppID"].ToString()), 9); // Schedule 1/ Managing Committee
                        if (rfu2.Item1 == "True")
                        {
                            RecordUserAction("Page_load", "File already Uploaded in mongo 9", "S");
                            lbrfu2status.Text = "File Uploaded";
                            LB_Schedule1_Upload.Enabled = false;
                            RenewalFileUpload2.Enabled = false;
                            LB_Schedule1_View.Enabled = true;
                            Session["du_flag9"] = 1;
                        }
                        else
                        {
                            lbrfu2status.Text = "";
                            LB_Schedule1_Upload.Enabled = true;
                            RenewalFileUpload2.Enabled = true;
                            Session["du_flag9"] = 0;

                        }

                        var rfu3 = getPageStatus(Convert.ToInt64(Session["AppID"].ToString()), 10);  // Schedule 6/ All Member Details witjh date of admission
                        if (rfu3.Item1 == "True")
                        {
                            RecordUserAction("Page_load", "File already Uploaded in mongo 10", "S");
                            lbrfu3status.Text = "File Uploaded";
                            LB_Schedule6_Upload.Enabled = false;
                            RenewalFileUpload3.Enabled = false;
                            LB_Schedule6_View.Enabled = true;
                            Session["du_flag10"] = 1;
                        }
                        else
                        {
                            lbrfu3status.Text = "";
                            LB_Schedule6_Upload.Enabled = true;
                            RenewalFileUpload3.Enabled = true;
                            Session["du_flag10"] = 0;

                        }

                        var rfu4 = getPageStatus(Convert.ToInt64(Session["AppID"].ToString()), 11);  // Schedule II/ Employee Details
                        if (rfu4.Item1 == "True")
                        {
                            RecordUserAction("Page_load", "File already Uploaded in mongo 10", "S");
                            lbrfu4status.Text = "File Uploaded";
                            LB_Schedule2_Upload.Enabled = false;
                            RenewalFileUpload4.Enabled = false;
                            LB_Schedule2_View.Enabled = true;
                            Session["du_flag11"] = 1;
                        }
                        else
                        {
                            lbrfu4status.Text = "";
                            LB_Schedule2_Upload.Enabled = true;
                            RenewalFileUpload4.Enabled = true;
                            Session["du_flag11"] = 0;

                        }

                        var rfu5 = getPageStatus(Convert.ToInt64(Session["AppID"].ToString()), 12);  // Schedule II/ Employee Details
                        if (rfu5.Item1 == "True")
                        {
                            RecordUserAction("Page_load", "File already Uploaded in mongo 10", "S");
                            lbrfu5status.Text = "File Uploaded";
                            LB_Schedule4_Upload.Enabled = false;
                            RenewalFileUpload5.Enabled = false;
                            LB_Schedule4_View.Enabled = true;
                            Session["du_flag12"] = 1;
                        }
                        else
                        {
                            lbrfu5status.Text = "";
                            LB_Schedule4_Upload.Enabled = true;
                            RenewalFileUpload5.Enabled = true;
                            Session["du_flag12"] = 0;

                        }


                    }
                    bindgridview(Session["AppID"].ToString());
                    ////////////////////////////////////////////
                }
                else
                {
                    RecordUserAction("DocumentUpload_Page_Load", "Session Tampared", "F");
                    Response.Redirect("LoginModule.aspx");
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
        //public void getdocname1()
        //{
        //    if (Session["AppID"] != null)
        //    {
        //        NpgsqlConnection conn = new NpgsqlConnection();
        //        NpgsqlCommand cmd = new NpgsqlCommand();
        //        conn.ConnectionString = ConfigurationManager.ConnectionStrings["connect"].ConnectionString;
        //        cmd.Connection = conn;
        //        try
        //        {
        //            conn.Open();
        //            string query = "select doc_one,doc_two from esociety.society where app_id=@appid";
        //            cmd.CommandText = query;
        //            cmd.Parameters.AddWithValue("@appid", Convert.ToInt64(Session["AppID"].ToString()));
        //            NpgsqlDataReader rd = cmd.ExecuteReader();
        //            if (rd.Read())
        //            {
        //                txtDoc3.Text = Server.HtmlEncode(rd["doc_one"].ToString());
        //                if (txtDoc3.Text == "" || txtDoc3.Text == null)
        //                {
        //                    txtDoc3.Enabled = true;
        //                }
        //                else
        //                {
        //                    chkDoc3.Checked = true;
        //                    txtDoc3.Enabled = false;
        //                }


        //            }
        //            rd.Close();
        //            RecordUserAction("getdocname1()", "Success", "S");
        //        }
        //        catch (NpgsqlException ex)
        //        {
        //            CreateLogFiles Err = new CreateLogFiles();
        //            Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "getdocname1()" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
        //            RecordUserAction("getdocname1()", ex.Message, "F");
        //        }
        //        finally
        //        {
        //            conn.Close();
        //        }
        //    }
        //    else
        //    {
        //        RecordUserAction("getdocname1()", "Session null", "F");
        //    }
        //}
        //public void getdocname2()
        //{
        //    if (Session["AppID"] != null)
        //    {
        //        NpgsqlConnection conn = new NpgsqlConnection();
        //        NpgsqlCommand cmd = new NpgsqlCommand();
        //        conn.ConnectionString = ConfigurationManager.ConnectionStrings["connect"].ConnectionString;
        //        cmd.Connection = conn;
        //        try
        //        {
        //            conn.Open();
        //            string query = "select doc_one,doc_two from esociety.society where app_id=@appid";
        //            cmd.CommandText = query;
        //            cmd.Parameters.AddWithValue("@appid", Convert.ToInt64(Session["AppID"].ToString()));
        //            NpgsqlDataReader rd = cmd.ExecuteReader();
        //            if (rd.Read())
        //            {
        //                txtDoc4.Text = Server.HtmlEncode(rd["doc_two"].ToString());
        //                if (txtDoc4.Text == "" || txtDoc4.Text == null)
        //                {
        //                    txtDoc4.Enabled = true;
        //                }
        //                else
        //                {
        //                    chkDoc4.Checked = true;
        //                    txtDoc4.Enabled = false;
        //                }

        //            }
        //            rd.Close();
        //            RecordUserAction("getdocname2()", "Success", "S");
        //        }
        //        catch (NpgsqlException ex)
        //        {
        //            CreateLogFiles Err = new CreateLogFiles();
        //            Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "getdocname2()" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
        //            RecordUserAction("getdocname2()", ex.Message, "F");
        //        }
        //        finally
        //        {
        //            conn.Close();
        //        }
        //    }
        //    else
        //    {
        //        RecordUserAction("getdocname2()", "Session null", "F");
        //    }
        //}
        public void get_status()
        {
            try
            {
                int isrenewal = Utility.checkifrenewal(Session["AppID"].ToString());

                if (isrenewal == 1)
                {
                    documents.Visible = true;
                    renewaldocuments.Visible = false;
                }
                else if (isrenewal == 2)
                {
                    documents.Visible = false;
                    renewaldocuments.Visible = true;
                }


                if (Convert.ToInt32(Session["status_Id"]) == 1 || Convert.ToInt32(Session["status_Id"]) == 2 || Convert.ToInt32(Session["status_Id"]) == 5)
                {

                }
                else
                {

                    LB_Application_Upload.Visible = false;
                    LB_Application_Delete.Visible = false;
                    LB_Memorandom_Upload.Visible = false;
                    LB_Memorandum_Delete.Visible = false;
                    LB_Bylaws_Upload.Visible = false;
                    LB_Bylaws_Delete.Visible = false;
                    //LinkButton3.Visible = false;
                    // LinkButton4.Visible = false;
                   
                    btnSubmit.Enabled = false;
                    tr_adddocbtn1.Visible = false;
                    tr_adddocbtn2.Visible = false;

                    //renewal related controls
                    Lb_RenewalApplication_Upload.Visible = false;
                    Lb_RenewalApplication_Delete.Visible = false;
                    LB_Schedule1_Upload.Visible = false;
                    LB_Schedule1_Delete.Visible = false;
                    LB_Schedule6_Upload.Visible = false;
                    LB_Schedule6_Delete.Visible = false;
                    LB_Schedule2_Upload.Visible = false;
                    LB_Schedule2_Delete.Visible = false;
                    LB_Schedule4_Upload.Visible = false;
                    LB_Schedule4_Delete.Visible = false;                   
                    btnrenewaldocssubmit.Enabled = false;

                    Lb_GenerateRenewalApplication.Visible = false;
                    LB_GenerateSchedule1.Visible = false;
                    LB_GenerateSchedule2.Visible = false;
                    LB_GenerateSchedule6.Visible = false;
                }
            }
            catch (Exception ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "get_status()" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
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
                string query = "SELECT active,object_id FROM esociety.otherdoc_crossentry where app_id=@appid and myid=@myid";
                cmd.CommandText = query;
                cmd.Parameters.AddWithValue("@appid", app_id);
                cmd.Parameters.AddWithValue("@myid", myid);
                NpgsqlDataReader rd = cmd.ExecuteReader();
                //var tuple = new Tuple<string, string>(active, objectid);
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
                Err.ErrorLog(HttpContext.Current.Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "()" + " DocumentUpload.aspx");
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
            //try
            //{
            //    byte[] tmpfiledata = mssg;
            //    string sPathToSaveFileTo = Server.MapPath("~/SelectedFile.pdf");
            //    using (System.IO.FileStream fs = new System.IO.FileStream(sPathToSaveFileTo, System.IO.FileMode.Create, System.IO.FileAccess.ReadWrite))
            //    {

            //        using (System.IO.BinaryWriter bw = new System.IO.BinaryWriter(fs))
            //        {
            //            bw.Write(tmpfiledata);
            //            RecordUserAction("convertToPdf", "File written to pdf", "S");
            //            bw.Close();
            //        }
            //    }
            //    WebClient User = new WebClient();
            //    Byte[] FileBuffer = User.DownloadData(Server.HtmlEncode(sPathToSaveFileTo));
            //    if (FileBuffer != null)
            //    {
            //        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "none", "<script> $(window).on('load', function () { $('#pdfModal').modal({ backdrop: 'static' });});</script>", false);
            //        //Response.ContentType = "application/pdf";
            //        ////Response.AddHeader("content-length", FileBuffer.Length.ToString());
            //        //Response.Write("window.open('sPathToSaveFileTo','_blank');");
            //        //Response.BinaryWrite(FileBuffer);
            //        RecordUserAction("convertToPdf", "byte[] to pdf", "S");
            //    }
            //}
            //catch (Exception ex)
            //{
            //    CreateLogFiles Err = new CreateLogFiles();
            //    Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "convertToPdf()" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
            //}
        }
        protected byte[] openConnectionMongo_societyDocs(string objectid)//society docuemnt
        {
            try
            {
                var str = ConfigurationManager.AppSettings["mongoconnect"];
                IMongoDatabase database;
                IMongoClient client;
                client = new MongoClient(str);
                database = client.GetDatabase("eGoaSociety");
                var collection = database.GetCollection<OtherDocuments>("Society Documents");
                var status = collection.Find(x => x._Id == ObjectId.Parse(objectid)).FirstOrDefault();
                byte[] decrypt_pdf_bytes = status.DocContent;
                byte[] pdf = obj_Byte_Encryption.DecryptData(decrypt_pdf_bytes, System.Web.HttpContext.Current.Server.MapPath("~/ENCRYPT.KEY"));
                convertToPdf(pdf);
                RecordUserAction("openConnectionMongo_societyDocs", "byte[] to function", "S");
                return pdf;
            }
            catch (MongoException ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "openConnectionMongo_societyDocs()" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
                RecordUserAction("openConnectionMongo_societyDocs", ex.Message, "F");
                Response.Write("<script language='javascript'>alert('" + "PDF View Failed" + ".No file uploaded yet')</script>");
                return null;
            }
        }
        //public void getmyID(Int64 appid, int myid)
        //{
        //    RecordUserAction("getmyID ", "Function Called", "S");
        //    NpgsqlConnection conn = new NpgsqlConnection();
        //    NpgsqlCommand cmd = new NpgsqlCommand();
        //    conn.ConnectionString = ConfigurationManager.ConnectionStrings["connect"].ConnectionString;
        //    cmd.Connection = conn;
        //    try
        //    {
        //        conn.Open();
        //        string query = "select object_id  from esociety.otherdoc_crossentry where app_id=@appid and myid=@myid";
        //        cmd.CommandText = query;
        //        cmd.Parameters.AddWithValue("@appid", appid);
        //        cmd.Parameters.AddWithValue("@myid", myid);
        //        NpgsqlDataReader dr = cmd.ExecuteReader();
        //        if (dr.Read())
        //        {
        //            string objectid = Server.HtmlEncode(dr["object_id"].ToString());
        //            openConnectionMongo_societyDocs(objectid);
        //        }
        //    }
        //    catch (NpgsqlException ex)
        //    {
        //        CreateLogFiles Err = new CreateLogFiles();
        //        Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "getmyID()" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
        //        RecordUserAction("getmyID", ex.Message, "F");
        //        Response.Write("<script language='javascript'>alert('" + "Execution Error" + "')</script>");
        //    }
        //    finally
        //    {

        //        conn.Close();
        //    }
        //}

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
        protected string openConnectionMongo(string objectid)
        {
            try
            {
                var str = ConfigurationManager.AppSettings["mongoconnect"];
                IMongoDatabase database;
                IMongoClient client;
                client = new MongoClient(str);
                database = client.GetDatabase("eGoaSociety");
                var collection = database.GetCollection<OtherDocuments>("Society Documents");
                var status = collection.Find(x => x._Id == ObjectId.Parse(objectid)).FirstOrDefault();
                string abc = status.Active.ToString();
                return abc;
            }
            catch (MongoException ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "openConnectionMongo()" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
                RecordUserAction("openConnectionMongo", ex.Message, "F");
                return null;
            }
            finally
            {
                //conn.close();
            }
        }


        public int getnoofPages(byte[] arr)
        {
            int a = 0;
            try
            {

                PdfReader pdfReader = new PdfReader(arr);
                a = pdfReader.NumberOfPages;

            }
            catch (Exception ex)
            {
                a = -1;
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "getnoofPages()" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));

            }
            return a;
        }

        public int fileuploadfucntion(FileUpload fu, Label lb, OtherDocuments othdoc, string a, string ip, string mac, long App_ID, string collection, int myid)
        {
            int flag = 0;

            if (Session["created_by"] != null)
            {
                try
                {
                    if (fu.HasFile)
                    {
                        string stringfilesize = ConfigurationManager.AppSettings["FileSize"];
                        int FileSize = Convert.ToInt32(stringfilesize) * 1024 * 1024;
                        HttpPostedFile file = fu.PostedFile;
                        byte[] document = new byte[file.ContentLength];
                        file.InputStream.Read(document, 0, file.ContentLength);
                        UInt32 mimetype;
                        FindMimeFromData(0, null, document, 256, null, 0, out mimetype, 0);
                        IntPtr mimeTypePtr = new IntPtr(mimetype);
                        string mimeType = Marshal.PtrToStringUni(mimeTypePtr).ToLower();
                        Marshal.FreeCoTaskMem(mimeTypePtr);
                        var scanExitCode = 0;//UtilityDAO.VirusScanFile(flupHeader.PostedFile.FileName);
                        if (scanExitCode == 0)
                        {
                            string UploadFileName = file.FileName;
                            string Extension = UploadFileName.Substring(UploadFileName.LastIndexOf('.') + 1).ToLower();
                            var extCount = UploadFileName.Split('.').Length - 1;

                            if (Extension.ToLower() != "pdf" || file.ContentType.ToLower() != "application/pdf" || mimeType != "application/pdf")
                            {
                                Label48.Text = "Please upload only Pdf file of maximum size 2MB allowed with extension .pdf!";
                                Label48.ForeColor = System.Drawing.Color.Red;
                                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "none", "<script> $(window).on('load', function () { $('#errorModal').modal({ backdrop: 'static' });});</script>", false);
                                flag = 0;
                            }
                            else if (extCount > 1)
                            {
                                Label48.Text = "File name can not contain multiple dots/extensions.";
                                Label48.ForeColor = System.Drawing.Color.Red;
                                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "none", "<script> $(window).on('load', function () { $('#errorModal').modal({ backdrop: 'static' });});</script>", false);

                                flag = 0;
                            }
                            else if (document == null)
                            {
                                Label48.Text = "Please upload file in pdf format or pdf not in correct format";
                                Label48.ForeColor = System.Drawing.Color.Red;
                                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "none", "<script> $(window).on('load', function () { $('#errorModal').modal({ backdrop: 'static' });});</script>", false);

                                flag = 0;
                            }
                            else if (file.ContentLength <= 0 || file.ContentLength > (FileSize))
                            {
                                Label48.Text = "Only Pdf file of maximum size 2MB allowed !";
                                Label48.ForeColor = System.Drawing.Color.Red;
                                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "none", "<script> $(window).on('load', function () { $('#errorModal').modal({ backdrop: 'static' });});</script>", false);

                                flag = 0;
                            }
                            else
                            {
                                string path = Utility.filesave(fu, Session["AppID"].ToString());
                                string resultedStr = "";
                                using (StreamReader sr = new StreamReader(path))
                                {
                                    resultedStr = SurroundingClass.ReadStreamWithTimeout(sr);
                                }
                                int scanned = Utility.checkFile(resultedStr);
                                if (scanned == 0)
                                {
                                    Response.Write("<script>alert('Invalid PDF uploaded');</script>");
                                    File.Delete(path);
                                }
                                else
                                {
                                    File.Delete(path);

                                    fileSizeFront = fu.FileContent.Length;
                                    documentBinary = new byte[fileSizeFront];
                                    //Stream fs = fu.PostedFile.InputStream;
                                    //BinaryReader br = new BinaryReader(fs);
                                    //byte[] bytes = br.ReadBytes((Int32)fs.Length);
                                    byte[] bytes = fu.FileBytes;
                                    int noofpages = getnoofPages(bytes);
                                    byte[] encrypt_bytes = obj_Byte_Encryption.EncryptData(bytes, System.Web.HttpContext.Current.Server.MapPath("~/ENCRYPT.KEY"));
                                    string document_id = Utility.get_OtherDocsID();//other doc ID
                                    othdoc.App_ID = App_ID;
                                    othdoc.Doc_CT = ct;
                                    othdoc.time_stamp = DateTime.Now.ToString();
                                    ObjectId obj_id = ObjectId.GenerateNewId();
                                    othdoc._Id = obj_id;
                                    othdoc.IpAddress = ip;
                                    othdoc.MacAddress = mac;
                                    othdoc.Active = true;
                                    othdoc.UpdatedBy = Session["created_by"].ToString();
                                    othdoc.Doc_ID = document_id;
                                    othdoc.doc_name = a;
                                    othdoc.DocContent = encrypt_bytes;
                                    int mvalue = InsertintoMongoDB(othdoc, collection, lb);
                                    if (mvalue == 1)
                                    {
                                        int pvalue = insertentryPosgres(obj_id, document_id, App_ID, myid, a, noofpages);
                                        if (pvalue == 1)
                                        {
                                            flag = 1;
                                        }
                                        else { flag = 0; }
                                    }
                                    else
                                    {
                                        flag = 0;
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        lb.ForeColor = System.Drawing.Color.Red;
                        lb.Text = "Upload " + Sanitize.InputText(a) + " Certificate file in PDF format";
                        Label48.ForeColor = System.Drawing.Color.Red;
                        Label48.Text = "Upload " + Sanitize.InputText(a) + " Certificate file in PDF format";
                        flag = 0;
                    }

                }
                catch (Exception ex)
                {
                    CreateLogFiles Err = new CreateLogFiles();
                    Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "fileuploadfucntion()" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
                    lb.Text = "File upload failed.";
                    RecordUserAction("Fileupload Function ", ex.Message, "F");
                    Label48.ForeColor = System.Drawing.Color.Red;
                    Label48.Text = "File upload failed.";
                }
            }
            return flag;
        }
        public int insertentryPosgres(ObjectId obj, string docid, long appid, int myid, string docname,int pagecount)
        {
            int value = 0;
            if (Session["AppID"] != null)
            {
                NpgsqlConnection conn = new NpgsqlConnection();
                NpgsqlCommand cmd = new NpgsqlCommand();
                conn.ConnectionString = ConfigurationManager.ConnectionStrings["connect"].ConnectionString;
                cmd.Connection = conn;
                string objectid = obj.ToString();
                if (Session["AppID"].ToString() == appid.ToString())
                {
                    try
                    {
                        conn.Open();
                        string grm_query = "INSERT INTO esociety.otherdoc_crossentry(object_id, other_docid, app_id,active,myid,docname,pagecount) VALUES";
                        grm_query = grm_query + " (@object_id,@other_docid, @app_id,'True',@myid,@docname,@pagecount)";
                        cmd.CommandText = grm_query;
                        cmd.Parameters.AddWithValue("@object_id", objectid);
                        cmd.Parameters.AddWithValue("@other_docid", docid);
                        cmd.Parameters.AddWithValue("@app_id", appid);
                        cmd.Parameters.AddWithValue("@myid", myid);
                        cmd.Parameters.AddWithValue("@docname", docname);
                        cmd.Parameters.AddWithValue("@pagecount", pagecount);
                        cmd.ExecuteNonQuery();
                        value = 1;
                        RecordUserAction("insertentryPosgres", "objectid saved to posgres", "S");
                    }
                    catch (NpgsqlException ex)
                    {
                        value = 0;
                        CreateLogFiles Err = new CreateLogFiles();
                        Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "insertentryPosgres()" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
                        RecordUserAction("insertentryPosgres", ex.Message, "F");
                        var errorcode = ex.Message;
                        if (errorcode.Contains("23503"))
                        {
                            Response.Write("<script language='javascript'>alert('" + "Please Complete Society Details first." + "')</script>");
                        }
                        else
                        {
                            Response.Write("<script language='javascript'>alert('" + "Exection Error" + "')</script>");
                        }

                    }
                    finally
                    {
                        conn.Close();
                    }
                }
                else
                {
                    value = 0;
                    lblError.Text = "Execution error";
                }
            }
            return value;
        }
        public int InsertintoMongoDB(OtherDocuments doc, string sel_collection, Label lb)
        {
            Insert insr = new Insert();
            try
            {
                RecordUserAction("InsertintoMongoDB", "write document to Mongo", "S");
                return insr.InsertMongoOtherDocs(doc, sel_collection);
            }
            catch (MongoException ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "InsertintoMongoDB()" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
                RecordUserAction("InsertintoMongoDB", ex.Message, "F");
                lb.Text = "Saved to db failed";
                return 0;
            }
        }
        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            if (Session["AppID"] != null)
            {
                Page_status_Check psc = new Page_status_Check();
                Insert ins = new Insert();
                psc = ins.getPageStatus(Session["AppID"].ToString());
                NpgsqlConnection conn = new NpgsqlConnection();
                NpgsqlCommand cmd = new NpgsqlCommand();
                conn.ConnectionString = ConfigurationManager.ConnectionStrings["connect"].ConnectionString;
                cmd.Connection = conn;
                conn.Open();
                NpgsqlTransaction myTrans = conn.BeginTransaction();
                cmd.Transaction = myTrans;
                try
                {
                    string query = "select complete_data from esociety.society where app_id=@app_id";
                    cmd.CommandText = query;
                    cmd.Parameters.AddWithValue("@app_id", Convert.ToInt64(Session["AppID"].ToString()));
                    NpgsqlDataReader rd = cmd.ExecuteReader();
                    if (rd.Read())
                    {
                        Session["du_complete_data"] = Server.HtmlEncode(rd["complete_data"].ToString());
                    }
                    rd.Close();
                    cmd.Parameters.Clear();
                    string count_query = "select count(*) as totalmembers from esociety.members where app_id=@app_id ";
                    cmd.CommandText = count_query;
                    cmd.Parameters.AddWithValue("@app_id", Convert.ToInt64(Session["AppID"].ToString()));
                    NpgsqlDataReader dr = cmd.ExecuteReader();
                    if (dr.Read())
                    {
                        Session["du_noofmembers"] = Convert.ToInt32(Server.HtmlEncode(dr["totalmembers"].ToString()));
                    }
                    dr.Close();
                    cmd.Parameters.Clear();

                    string count_objective = "select count(*) as objectivetotal from esociety.society_objectives where app_id=@app_id and active='Y'";
                    cmd.CommandText = count_objective;
                    cmd.Parameters.AddWithValue("@app_id", Convert.ToInt64(Session["AppID"].ToString()));
                    NpgsqlDataReader dr1 = cmd.ExecuteReader();
                    if (dr1.Read())
                    {
                        Session["du_count_objective"] = Convert.ToInt32(Server.HtmlEncode(dr1["objectivetotal"].ToString()));
                    }
                    dr1.Close();
                    cmd.Parameters.Clear();
                    RecordUserAction("btnSubmit_Click", "Success", "S");
                    myTrans.Commit();
                }
                catch (NpgsqlException ex)
                {
                    CreateLogFiles Err = new CreateLogFiles();
                    Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "btnSubmit_Click" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
                    myTrans.Rollback();
                    RecordUserAction("btnSubmit_Click", ex.Message, "F");
                    Response.Write("<script language='javascript'>alert('" + "Error while submitting" + "')</script>");
                }
                finally
                {
                    conn.Close();
                }
                int isrenewal = Utility.checkifrenewal(Session["AppID"].ToString());
                if (Session["du_complete_data"].ToString() == "Yes")
                {
                    if (CheckBox1.Checked != true && isrenewal == 1)
                    {
                        Label15.Text = "Please Tick Mark the checkbox of Application for Registration";
                        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "none", "<script> $(window).on('load', function () { $('#errormodal').modal({ backdrop: 'static' });});</script>", false);
                    }
                    else if (Convert.ToInt32(Session["du_flag1"].ToString()) != 1 && isrenewal == 1)
                    //else if (!lbfu1status.Text.Equals("File Uploaded") && !lbfu1status.Text.Equals("Uploaded"))
                    {
                        Label15.Text = "You have not Uploaded Application for Registration File";
                        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "none", "<script> $(window).on('load', function () { $('#errormodal').modal({ backdrop: 'static' });});</script>", false);
                    }
                    else if (chkDoc1.Checked != true && isrenewal == 1)
                    {
                        Label15.Text = "Please Tick Mark the checkbox of Memorandum of Association";
                        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "none", "<script> $(window).on('load', function () { $('#errormodal').modal({ backdrop: 'static' });});</script>", false);
                    }
                    else if (Convert.ToInt32(Session["du_flag2"].ToString()) != 1 && isrenewal == 1)
                    //else if (!lbfu2status.Text.Equals("File Uploaded") && !lbfu2status.Text.Equals("Uploaded"))
                    {
                        Label15.Text = "You have not Uploaded Memorandum of Association File";
                        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "none", "<script> $(window).on('load', function () { $('#errormodal').modal({ backdrop: 'static' });});</script>", false);
                    }

                    else if (chkDoc2.Checked != true && isrenewal == 1)
                    {
                        Label15.Text = "Please Tick Mark the checkbox of Rules And Regulation";
                        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "none", "<script> $(window).on('load', function () { $('#errormodal').modal({ backdrop: 'static' });});</script>", false);
                    }
                    else if (Convert.ToInt32(Session["du_flag3"].ToString()) != 1 && isrenewal == 1)
                    //else if (!lbfu3status.Text.Equals("File Uploaded") && !lbfu3status.Text.Equals("Uploaded"))
                    {
                        Label15.Text = "You have not Uploaded Rules And Regulation File";
                        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "none", "<script> $(window).on('load', function () { $('#errormodal').modal({ backdrop: 'static' });});</script>", false);
                    }
                    else if (Convert.ToInt32(Session["du_flag8"].ToString()) != 1 && isrenewal == 2)
                    {
                        Label15.Text = "You have not Uploaded Application For Renewal";
                        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "none", "<script> $(window).on('load', function () { $('#errormodal').modal({ backdrop: 'static' });});</script>", false);
                    }
                    else if (Convert.ToInt32(Session["du_flag9"].ToString()) != 1 && isrenewal == 2)
                    {
                        Label15.Text = "You have not Uploaded  Schedule I";
                        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "none", "<script> $(window).on('load', function () { $('#errormodal').modal({ backdrop: 'static' });});</script>", false);
                    }
                    else if (Convert.ToInt32(Session["du_flag10"].ToString()) != 1 && isrenewal == 2)
                    {
                        Label15.Text = "You have not Uploaded Schedule VI";
                        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "none", "<script> $(window).on('load', function () { $('#errormodal').modal({ backdrop: 'static' });});</script>", false);
                    }
                    else if (Convert.ToInt32(Session["du_flag11"].ToString()) != 1 && isrenewal == 2)
                    {
                        Label15.Text = "You have not Uploaded Schedule II";
                        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "none", "<script> $(window).on('load', function () { $('#errormodal').modal({ backdrop: 'static' });});</script>", false);
                    }
                    else if (Convert.ToInt32(Session["du_flag12"].ToString()) != 1 && isrenewal == 2)
                    {
                        Label15.Text = "You have not Uploaded Schedule VI";
                        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "none", "<script> $(window).on('load', function () { $('#errormodal').modal({ backdrop: 'static' });});</script>", false);
                    }
                    else if (Utility.checkMembersList(Session["AppID"].ToString()) != 3 && (Utility.getGovernmentSociety(Session["AppID"].ToString()) == 0))
                    {
                        Label15.Text = "President , Secretary, Treasurer are required to be part of Managing Committee.";
                        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "none", "<script> $(window).on('load', function () { $('#errormodal').modal({ backdrop: 'static' });});</script>", false);
                    }
                    else if (Utility.checkMembersListgov(Session["AppID"].ToString()) != 2 && (Utility.getGovernmentSociety(Session["AppID"].ToString()) == 1))
                    {
                        Label15.Text = "President , Secretary are required to be part of Managing Committee.";
                        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "none", "<script> $(window).on('load', function () { $('#errormodal').modal({ backdrop: 'static' });});</script>", false);
                    }
                    else if(Convert.ToInt32(Session["du_count_objective"].ToString()) < 1)
                    {
                        Label15.Text = "Society Objectives are not added";
                        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "none", "<script> $(window).on('load', function () { $('#errormodal').modal({ backdrop: 'static' });});</script>", false);

                    }
                    else if (Convert.ToInt32(Session["du_noofmembers"].ToString()) >= 7)
                    {
                        try
                        {
                            conn.Open();
                            string complete_query = "";
                            if (psc.status_id == 1)
                            {
                                complete_query = "Update esociety.status_table set admin_checked='No',status_id=3,application_submission_time=current_timestamp where app_id=@app_id";
                            }
                            else if (psc.status_id == 5)
                            {
                                complete_query = "Update esociety.status_table set admin_checked='No',status_id=6,application_obs_submission_time=current_timestamp  where app_id=@app_id";
                            }
                            cmd.CommandText = complete_query;
                            cmd.Parameters.AddWithValue("@app_id", Convert.ToInt64(Session["AppID"].ToString()));
                            cmd.ExecuteNonQuery();

                            string history_query = "INSERT INTO esociety.application_submission_history( app_id, login_id, submitted_at, ipaddress)";
                            history_query = history_query + " VALUES(@app_id, @login_id, CURRENT_TIMESTAMP, @ipaddress)";
                            cmd.CommandText = history_query;
                            cmd.Parameters.AddWithValue("@app_id", Convert.ToInt64(Session["AppID"].ToString()));
                            cmd.Parameters.AddWithValue("@login_id", Session["login_id"].ToString());
                            cmd.Parameters.AddWithValue("@ipaddress", ipaddress);

                            cmd.ExecuteNonQuery();
                            cmd.Parameters.Clear();

                            RecordUserAction("btnSubmit_Click", "Data saved in status table", "S");
                        }
                        catch (NpgsqlException ex)
                        {
                            CreateLogFiles Err = new CreateLogFiles();
                            Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "btnSubmit_Click" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
                            RecordUserAction("btnSubmit_Click", ex.Message, "F");
                            Response.Write("<script language='javascript'>alert('" + "DB Failed Error" + "')</script>");
                        }
                        finally
                        {
                            conn.Close();
                        }
                        string message = " Your Application No. has been registered.  Please wait for our approval and note down your Application Id for future. " + Convert.ToInt64(Session["AppID"].ToString());
                        Status.Text = message;
                        SendSMS sms = new SendSMS();
                        string mobile = getmobileno(Session["AppID"].ToString());
                        string b = Session["AppID"].ToString();
                        string c = Session["login_id"].ToString();
                        sms.send_otp_sms_submit(mobile, Session["AppID"].ToString(), Session["login_id"].ToString());
                        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "none", "<script> $(window).on('load', function () { $('#myModal1').modal({ backdrop: 'static' });});</script>", false);

                    }
                    else
                    {
                        Response.Write("<script language='javascript'>alert('Details Incomplete or at least 7 Managing Committee / Members are required.')</script>");
                    }
                }
                else if (Session["du_complete_data"].ToString() == "" || Session["du_complete_data"].ToString() == null)
                {
                    Response.Write("<script language='javascript'>alert('Society Data Incomplete.')</script>");
                }
            }
            else
            {
                RecordUserAction("btnSubmit_Click", "Session null", "F");
            }
        }

        protected void LB_Application_Upload_Click(object sender, EventArgs e)
        {
            try
            {
                if (Session["AppID"] != null)
                {
                    int a = fileuploadfucntion(FileUpload1, lbfu1status, doc, "Application for Registration", ipaddress, macaddress, Convert.ToInt64(Session["AppID"].ToString()), "Society Documents", 1);
                    if (a == 1)
                    {
                        lbfu1status.ForeColor = System.Drawing.Color.Green;
                        lbfu1status.Text = "Uploaded Successfully";
                        RecordUserAction("LB_Application_Upload_Click", "File Uploaded Successfully", "S");
                        LB_Application_View.Enabled = true;
                        FileUpload1.Enabled = false;
                        LB_Application_Upload.Enabled = false;
                        Session["du_flag1"] = 1;
                    }
                    else
                    {
                        RecordUserAction("LB_Application_Upload_Click", "File Uploaded Failed", "F");
                        lbfu1status.ForeColor = System.Drawing.Color.Red;
                        lbfu1status.Text = "Failed";
                        Session["du_flag1"] = 0;
                    }
                }
                else
                {
                    RecordUserAction("LB_Application_Upload_Click", "Session null", "F");
                }
            }
            catch (Exception ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "LB_Application_Upload_Click" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
            }
        }

        protected void LB_Memorandom_Upload_Click(object sender, EventArgs e)
        {
            try
            {
                if (Session["AppID"] != null)
                {
                    int b = fileuploadfucntion(FileUpload2, lbfu2status, doc, "Memorandum of Association", ipaddress, macaddress, Convert.ToInt64(Session["AppID"].ToString()), "Society Documents", 2);
                    if (b == 1)
                    {
                        lbfu2status.ForeColor = System.Drawing.Color.Green;
                        lbfu2status.Text = "Uploaded";
                        RecordUserAction("LB_Memorandom_Upload_Click", "File Uploaded Successfully", "S");
                        LB_Memorandum_View.Enabled = true;
                        FileUpload2.Enabled = false;
                        LB_Memorandom_Upload.Enabled = false;
                        Session["du_flag2"] = 1;
                    }
                    else
                    {
                        RecordUserAction("LB_Memorandom_Upload_Click", "File Uploaded Failed", "F");
                        lbfu2status.ForeColor = System.Drawing.Color.Red;
                        lbfu2status.Text = "Failed";
                        Session["du_flag2"] = 0;
                    }
                }
                else
                {
                    RecordUserAction("LB_Memorandom_Upload_Click", "Session null", "F");
                }
            }
            catch (Exception ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "LB_Memorandom_Upload_Click" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
            }
        }

        protected void LB_Bylaws_Upload_Click(object sender, EventArgs e)
        {
            try
            {
                if (Session["AppID"] != null)
                {
                    int c = fileuploadfucntion(FileUpload3, lbfu3status, doc, "Rules And Regulation", ipaddress, macaddress, Convert.ToInt64(Session["AppID"].ToString()), "Society Documents", 3);
                    if (c == 1)
                    {
                        RecordUserAction("LB_Bylaws_Upload_Click", "File Uploaded Successfully", "S");
                        lbfu3status.ForeColor = System.Drawing.Color.Green;
                        lbfu3status.Text = "Uploaded";
                        LB_Bylaws_View.Enabled = true;
                        FileUpload3.Enabled = false;
                        LB_Bylaws_Upload.Enabled = false;
                        Session["du_flag3"] = 1;
                    }
                    else
                    {
                        RecordUserAction("LB_Bylaws_Upload_Click", "File Uploaded Failed", "F");
                        lbfu3status.ForeColor = System.Drawing.Color.Red;
                        lbfu3status.Text = "Failed";
                        Session["du_flag3"] = 0;
                    }
                }
                else
                {
                    RecordUserAction("LinkButton1_Click", "Session null", "F");
                }
            }
            catch (Exception ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "LB_Bylaws_Upload_Click" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
            }
        }

        //protected void LinkButton3_Click(object sender, EventArgs e)
        //{
        //    if (Session["AppID"] != null)
        //    {
        //        int d = fileuploadfucntion(FileUpload7, lbfu4status, doc, "Certificate", ipaddress, macaddress, Convert.ToInt64(Session["AppID"].ToString()), "Society Documents", 4);
        //        if (d == 1)
        //        {
        //            lbfu4status.ForeColor = System.Drawing.Color.Green;
        //            lbfu4status.Text = "Uploaded";
        //            RecordUserAction("LinkButton3_Click", "File Uploaded Successfully", "S");
        //            LinkButton10.Enabled = true;
        //            flag4 = 1;
        //        }
        //        else
        //        {
        //            lbfu4status.ForeColor = System.Drawing.Color.Red;
        //            lbfu4status.Text = "Failed";
        //            RecordUserAction("LinkButton3_Click", "File Uploaded Failed", "F");
        //            flag4 = 0;
        //        }
        //    }
        //    else
        //    {
        //        RecordUserAction("LinkButton3_Click", "Session null", "F");
        //    }
        //}

        //protected void LinkButton7_Click(object sender, EventArgs e)
        //{
        //    if (Session["AppID"] != null)
        //    {
        //        if (chkDoc4.Checked == true)
        //        {
        //            Validate _val = new Validate();
        //            NpgsqlConnection conn = new NpgsqlConnection();
        //            NpgsqlCommand cmd = new NpgsqlCommand();
        //            conn.ConnectionString = ConfigurationManager.ConnectionStrings["connect"].ConnectionString;
        //            cmd.Connection = conn;
        //            if (txtDoc4.Text == null || txtDoc4.Text == "")
        //            {
        //                lblError.Text = "Please Enter  Name!";
        //            }
        //            else if (!_val.validateData(txtDoc4.Text, _val.name))
        //            {
        //                lblError.Text = "Please Enter Valid Name!";

        //            }
        //            else
        //            {
        //                string docname = txtDoc4.Text;
        //                int f = fileuploadfucntion(FileUpload5, Label10, doc, docname, ipaddress, macaddress, Convert.ToInt64(Session["AppID"].ToString()), "Society Documents", 6);
        //                if (f == 1)
        //                {
        //                    LinkButton12.Enabled = true;
        //                    Label10.ForeColor = System.Drawing.Color.Green;
        //                    Label10.Text = "Uploaded";
        //                    RecordUserAction("LinkButton7_Click", "File Uploaded Successfully", "S");
        //                    Session["du_flag6"] = 1;
        //                    try
        //                    {
        //                        conn.Open();
        //                        string update2 = "update esociety.society set doc_two=@docname1 where app_id=@app_id";
        //                        cmd.CommandText = update2;
        //                        cmd.Parameters.AddWithValue("@docname1", txtDoc4.Text);
        //                        cmd.Parameters.AddWithValue("@app_id", Convert.ToInt64(Session["AppID"].ToString()));
        //                        cmd.ExecuteNonQuery();
        //                        FileUpload5.Enabled = false;
        //                        LinkButton7.Enabled = false;
        //                    }
        //                    catch (NpgsqlException ex)
        //                    {
        //                        CreateLogFiles Err = new CreateLogFiles();
        //                        Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "LinkButton7_Click" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
        //                    }
        //                    finally
        //                    {
        //                        conn.Close();
        //                    }
        //                }
        //                else
        //                {
        //                    Label10.ForeColor = System.Drawing.Color.Red;
        //                    Label10.Text = "Failed";
        //                    RecordUserAction("LinkButton7_Click", "File Uploaded Failed", "F");
        //                    Session["du_flag6"] = 0;
        //                }
        //            }
        //        }
        //        else
        //        {

        //            Response.Write("<script language='javascript'>alert('Please select the checkbox and Enter the document name')</script>");
        //        }
        //    }
        //    else
        //    {
        //        RecordUserAction("LinkButton7_Click", "Session null", "F");
        //    }
        //}

        //protected void chkDoc4_CheckedChanged(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        if (chkDoc4.Checked == true)
        //        {
        //            RecordUserAction("chkDoc4_CheckedChanged", "Checked", "S");
        //            LinkButton7.Enabled = true;
        //            LinkButton8.Enabled = true;
        //            LinkButton12.Enabled = false;
        //        }
        //        else
        //        {
        //            RecordUserAction("chkDoc4_CheckedChanged", "Unchecked", "F");
        //            LinkButton7.Enabled = false;
        //            LinkButton8.Enabled = false;
        //            LinkButton12.Enabled = false;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        CreateLogFiles Err = new CreateLogFiles();
        //        Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "chkDoc4_CheckedChanged" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
        //    }
        //}

        //protected void chkDoc3_CheckedChanged(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        if (chkDoc3.Checked == true)
        //        {
        //            RecordUserAction("chkDoc3_CheckedChanged", "Checked", "S");
        //            LinkButton5.Enabled = true;
        //            LinkButton6.Enabled = true;
        //            LinkButton11.Enabled = false;
        //        }
        //        else
        //        {
        //            RecordUserAction("chkDoc3_CheckedChanged", "Unchecked", "F");
        //            LinkButton5.Enabled = false;
        //            LinkButton6.Enabled = false;
        //            LinkButton11.Enabled = false;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        CreateLogFiles Err = new CreateLogFiles();
        //        Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "chkDoc3_CheckedChanged" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
        //    }
        //}

        //protected void LinkButton5_Click(object sender, EventArgs e)
        //{
        //    if (Session["AppID"] != null)
        //    {
        //        if (chkDoc3.Checked == true)
        //        {
        //            Validate _val = new Validate();
        //            if (txtDoc3.Text == null || txtDoc3.Text == "")
        //            {
        //                lblError.Text = "Please Enter  Name!";
        //            }
        //            else if (!_val.validateData(txtDoc3.Text, _val.name))
        //            {
        //                lblError.Text = "Please Enter Valid Name. No special characters allowed!";

        //            }
        //            else
        //            {
        //                string docname3 = Server.HtmlEncode(txtDoc3.Text);
        //                int x = fileuploadfucntion(FileUpload4, Label5, doc, docname3, ipaddress, macaddress, Convert.ToInt64(Session["AppID"].ToString()), "Society Documents", 5);


        //                if (x == 1)
        //                {
        //                    LinkButton11.Enabled = true;
        //                    NpgsqlConnection conn = new NpgsqlConnection();
        //                    NpgsqlCommand cmd = new NpgsqlCommand();
        //                    conn.ConnectionString = ConfigurationManager.ConnectionStrings["connect"].ConnectionString;
        //                    cmd.Connection = conn;
        //                    Label5.ForeColor = System.Drawing.Color.Green;
        //                    Label5.Text = "Uploaded";
        //                    RecordUserAction("LinkButton5_Click", "File Uploaded Successfully", "S");
        //                    Session["du_flag5"] = 1;
        //                    try
        //                    {
        //                        conn.Open();
        //                        string update1 = "update esociety.society set doc_one=@docname where app_id=@app_id";
        //                        cmd.CommandText = update1;
        //                        cmd.Parameters.AddWithValue("@docname", txtDoc3.Text);
        //                        cmd.Parameters.AddWithValue("@app_id", Convert.ToInt64(Session["AppID"].ToString()));
        //                        cmd.ExecuteNonQuery();
        //                        FileUpload4.Enabled = false;
        //                        LinkButton5.Enabled = false;
        //                    }
        //                    catch (NpgsqlException ex)
        //                    {
        //                        CreateLogFiles Err = new CreateLogFiles();
        //                        Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "LinkButton5_Click" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
        //                    }
        //                    finally
        //                    {
        //                        conn.Close();
        //                    }
        //                }
        //                else
        //                {
        //                    Label5.ForeColor = System.Drawing.Color.Red;
        //                    Label5.Text = "Failed";
        //                    RecordUserAction("LinkButton5_Click", "File Uploaded Failed", "F");
        //                    Session["du_flag5"] = 0;
        //                }
        //            }
        //        }
        //        else
        //        {

        //            Response.Write("<script language='javascript'>alert('Please select the checkbox and Enter the document name')</script>");
        //        }
        //    }
        //    else
        //    {

        //    }
        //}

        protected void gotohomepage_Click(object sender, EventArgs e)
        {
            Response.Redirect("Dashboard.aspx");
        }



        protected void LB_Application_View_Click(object sender, EventArgs e)
        {
            try
            {
                var status = getPageStatus(Convert.ToInt64(Session["AppID"].ToString()), 1);
                string objectid = status.Item2;
                openConnectionMongo_societyDocs(objectid);
                RecordUserAction("LB_Application_View_Click", "User Clicked on LB_Application_View_Click", "S");
            }
            catch (Exception ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "LB_Application_View_Click" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
            }
        }

        protected void LB_Memorandum_View_Click(object sender, EventArgs e)
        {
            try
            {
                var status = getPageStatus(Convert.ToInt64(Session["AppID"].ToString()), 2);
                string objectid = status.Item2;
                openConnectionMongo_societyDocs(objectid);
                RecordUserAction("LB_Memorandum_View_Click", "User Clicked on LB_Memorandum_View_Click", "S");
            }
            catch (Exception ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "LB_Memorandum_View_Click" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
            }
        }

        protected void LB_Bylaws_View_Click(object sender, EventArgs e)
        {
            try
            {
                var status = getPageStatus(Convert.ToInt64(Session["AppID"].ToString()), 3);
                string objectid = status.Item2;
                openConnectionMongo_societyDocs(objectid);
                RecordUserAction("LB_Bylaws_View_Click", "User Clicked on LB_Bylaws_View_Click", "S");
            }
            catch (Exception ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "LB_Bylaws_View_Click" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
            }
        }

        protected void LinkButton10_Click(object sender, EventArgs e)
        {
            try
            {
                var status = getPageStatus(Convert.ToInt64(Session["AppID"].ToString()), 4);
                string objectid = status.Item2;
                openConnectionMongo_societyDocs(objectid);
                RecordUserAction("LinkButton10_Click", "User Clicked on LinkButton10_Click", "S");
            }
            catch (Exception ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "LinkButton10_Click" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
            }
        }

        //protected void LinkButton11_Click(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        var status = getPageStatus(Convert.ToInt64(Session["AppID"].ToString()), 5);
        //        string objectid = status.Item2;
        //        openConnectionMongo_societyDocs(objectid);
        //        RecordUserAction("LinkButton11_Click", "User Clicked on LinkButton11_Click", "S");
        //    }
        //    catch (Exception ex)
        //    {
        //        CreateLogFiles Err = new CreateLogFiles();
        //        Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "LinkButton11_Click" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
        //    }
        //}

        //protected void LinkButton12_Click(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        var status = getPageStatus(Convert.ToInt64(Session["AppID"].ToString()), 6);
        //        string objectid = status.Item2;
        //        openConnectionMongo_societyDocs(objectid);
        //        RecordUserAction("LinkButton12_Click", "User Clicked on LinkButton12_Click", "S");
        //    }
        //    catch (Exception ex)
        //    {
        //        CreateLogFiles Err = new CreateLogFiles();
        //        Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "LinkButton12_Click" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
        //    }
        //}

        protected void LB_Application_Delete_Click(object sender, EventArgs e)
        {
            try
            {
                var status = getPageStatus(Convert.ToInt64(Session["AppID"].ToString()), 1);
                string objectid = status.Item2;
                if (objectid == "" || objectid == null)
                {
                    Response.Write("<script language='javascript'>alert('File Not present to delete')</script>");
                }
                else
                {

                    int a = DeleteFunction(objectid);
                    if (a == 1)
                    {
                        lbfu1status.Text = "File Deleted";
                        Response.Write("<script language='javascript'>alert('Deleted Successfully')</script>");
                        LB_Application_View.Enabled = false;
                        LB_Application_Upload.Enabled = true;
                        FileUpload1.Enabled = true;
                        Session["du_flag1"] = 0;
                    }
                    else
                    {
                        lbfu1status.Text = "Failed...Kindly try again";
                        Response.Write("<script language='javascript'>alert('Failed...Kindly try again')</script>");
                    }
                }
            }
            catch (Exception ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "LB_Application_Delete_Click" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
            }
        }

        protected int DeleteFunction(string obj)
        {
            NpgsqlConnection conn = new NpgsqlConnection();
            NpgsqlCommand cmd = new NpgsqlCommand();
            conn.ConnectionString = ConfigurationManager.ConnectionStrings["connect"].ConnectionString;
            cmd.Connection = conn;
            conn.Open();
            NpgsqlTransaction myTrans = conn.BeginTransaction();
            cmd.Transaction = myTrans;
            try
            {
                string select = "SELECT app_id, other_docid, myid,docname FROM esociety.otherdoc_crossentry where object_id=@object_id";
                cmd.CommandText = select;
                cmd.Parameters.AddWithValue("@object_id", obj);
                NpgsqlDataReader rd = cmd.ExecuteReader();
                if (rd.Read())
                {
                    ViewState["object_id"] = Server.HtmlEncode(obj);
                    ViewState["app_id"] = Server.HtmlEncode(rd["app_id"].ToString());
                    ViewState["other_docid"] = Server.HtmlEncode(rd["other_docid"].ToString());
                    ViewState["myid"] = Server.HtmlEncode(rd["myid"].ToString());
                    ViewState["docname"] = Server.HtmlEncode(rd["docname"].ToString());
                }
                rd.Close();
                cmd.Parameters.Clear();
                string hist_inr = "INSERT INTO esociety.history_otherdoc_crossentry(object_id, app_id, other_docid, active, myid, docname)VALUES (@object_id, @app_id, @other_docid, 'N', @myid, @docname)";
                cmd.CommandText = hist_inr;
                cmd.Parameters.AddWithValue("@object_id", ViewState["object_id"].ToString());
                cmd.Parameters.AddWithValue("@app_id", Convert.ToInt64(ViewState["app_id"]));
                cmd.Parameters.AddWithValue("@other_docid", ViewState["other_docid"].ToString());
                cmd.Parameters.AddWithValue("@myid", Convert.ToInt16(ViewState["myid"]));
                cmd.Parameters.AddWithValue("@docname", ViewState["docname"].ToString());
                cmd.ExecuteNonQuery();
                cmd.Parameters.Clear();
                string delete = "delete from esociety.otherdoc_crossentry where object_id=@obj";
                cmd.CommandText = delete;
                cmd.Parameters.AddWithValue("@obj", ViewState["object_id"].ToString());
                cmd.ExecuteNonQuery();
                cmd.Parameters.Clear();
                if (Convert.ToInt16(ViewState["myid"]) == 5)
                {
                    string delete1 = "Update esociety.society set doc_one=@val  where app_id=@app_id";
                    cmd.CommandText = delete1;
                    cmd.Parameters.AddWithValue("@val", DBNull.Value);
                    cmd.Parameters.AddWithValue("@app_id", Convert.ToInt64(ViewState["app_id"]));
                    cmd.ExecuteNonQuery();
                    cmd.Parameters.Clear();
                }
                else if (Convert.ToInt16(ViewState["myid"]) == 6)
                {
                    string delete2 = "Update esociety.society set doc_two=@val where app_id=@app_id";
                    cmd.CommandText = delete2;
                    cmd.Parameters.AddWithValue("@val", DBNull.Value);
                    cmd.Parameters.AddWithValue("@app_id", Convert.ToInt64(ViewState["app_id"]));
                    cmd.ExecuteNonQuery();
                    cmd.Parameters.Clear();

                }
                myTrans.Commit();
                return 1;
            }
            catch (NpgsqlException ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "DeleteFunction()" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));

                myTrans.Rollback();
                RecordUserAction("DeleteFunction", ex.Message, "F");
                Response.Write("<script language='javascript'>alert('" + "Bind Error while Deletion" + "')</script>");
                return 0;
            }
            finally
            {
                conn.Close();
            }
        }

        protected void LB_Memorandum_Delete_Click(object sender, EventArgs e)
        {
            try
            {
                var status = getPageStatus(Convert.ToInt64(Session["AppID"].ToString()), 2);
                string objectid = status.Item2;
                if (objectid == "" || objectid == null)
                {
                    Response.Write("<script language='javascript'>alert('File Not present to delete')</script>");
                }
                else
                {

                    int a = DeleteFunction(objectid);
                    if (a == 1)
                    {
                        lbfu2status.Text = "File Deleted";
                        Response.Write("<script language='javascript'>alert('Deleted Successfully')</script>");
                        LB_Memorandum_View.Enabled = false;
                        LB_Memorandom_Upload.Enabled = true;
                        FileUpload2.Enabled = true;
                        Session["du_flag2"] = 0;
                    }
                    else
                    {
                        lbfu2status.Text = "Failed...Kindly try again";
                        Response.Write("<script language='javascript'>alert('Failed...Kindly try again')</script>");
                    }
                }
            }
            catch (Exception ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "LB_Memorandum_Delete_Click" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
            }
        }

        protected void LB_Bylaws_Delete_Click(object sender, EventArgs e)
        {
            try
            {
                var status = getPageStatus(Convert.ToInt64(Session["AppID"].ToString()), 3);
                string objectid = status.Item2;
                if (objectid == "" || objectid == null)
                {
                    Response.Write("<script language='javascript'>alert('File Not present to delete')</script>");
                }
                else
                {

                    int a = DeleteFunction(objectid);
                    if (a == 1)
                    {
                        lbfu3status.Text = "File Deleted";
                        Response.Write("<script language='javascript'>alert('Deleted Successfully')</script>");
                        LB_Bylaws_View.Enabled = false;
                        LB_Bylaws_Upload.Enabled = true;
                        FileUpload3.Enabled = true;
                        Session["du_flag3"] = 0;
                    }
                    else
                    {
                        lbfu3status.Text = "Failed...Kindly try again";
                        Response.Write("<script language='javascript'>alert('Failed...Kindly try again')</script>");
                    }
                }
            }
            catch (Exception ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "LB_Bylaws_Delete_Click" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
            }
        }
        //certificate link button disabled
        //protected void LinkButton4_Click(object sender, EventArgs e)
        //{
        //    var status = getPageStatus(Convert.ToInt64(appid), 4);
        //    string objectid = status.Item2;
        //    if (objectid == "" || objectid == null)
        //    {
        //        Response.Write("<script language='javascript'>alert('File Not present to delete')</script>");
        //    }
        //    else
        //    {

        //        int a = DeleteFunction(objectid);
        //        if (a == 1)
        //        {
        //            lbfu4status.Text = "File Deleted";
        //            Response.Write("<script language='javascript'>alert('Deleted Successfully')</script>");
        //            LinkButton10.Enabled = false;
        //            LinkButton3.Enabled = true;
        //            FileUpload7.Enabled = true;
        //            flag4 = 0;
        //        }
        //        else
        //        {
        //            lbfu4status.Text = "Failed...Kindly try again";
        //            Response.Write("<script language='javascript'>alert('Failed...Kindly try again')</script>");
        //        }
        //    }

        //}

        //protected void LinkButton6_Click(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        var status = getPageStatus(Convert.ToInt64(Session["AppID"].ToString()), 5);
        //        string objectid = status.Item2;
        //        if (objectid == "" || objectid == null)
        //        {
        //            Response.Write("<script language='javascript'>alert('File Not present to delete')</script>");
        //        }
        //        else
        //        {

        //            int a = DeleteFunction(objectid);
        //            if (a == 1)
        //            {
        //                Label5.Text = "File Deleted";
        //                Response.Write("<script language='javascript'>alert('Deleted Successfully')</script>");
        //                LinkButton11.Enabled = false;
        //                LinkButton5.Enabled = true;
        //                FileUpload4.Enabled = true;
        //                txtDoc3.Enabled = true;
        //                Session["du_flag5"] = 0;
        //            }
        //            else
        //            {
        //                Label5.Text = "Failed...Kindly try again";
        //                Response.Write("<script language='javascript'>alert('Failed...Kindly try again')</script>");
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        CreateLogFiles Err = new CreateLogFiles();
        //        Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "LinkButton6_Click" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
        //    }
        //}

        //protected void LinkButton8_Click(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        var status = getPageStatus(Convert.ToInt64(Session["AppID"].ToString()), 6);
        //        string objectid = status.Item2;
        //        if (objectid == "" || objectid == null)
        //        {
        //            Response.Write("<script language='javascript'>alert('File Not present to delete')</script>");
        //        }
        //        else
        //        {

        //            int a = DeleteFunction(objectid);
        //            if (a == 1)
        //            {

        //                Label10.Text = "File Deleted";
        //                Response.Write("<script language='javascript'>alert('Deleted Successfully')</script>");
        //                LinkButton12.Enabled = false;
        //                LinkButton7.Enabled = true;
        //                FileUpload5.Enabled = true;
        //                txtDoc4.Enabled = true;
        //                Session["du_flag6"] = 0;
        //            }
        //            else
        //            {
        //                Label10.Text = "Failed...Kindly try again";
        //                Response.Write("<script language='javascript'>alert('Failed...Kindly try again')</script>");
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        CreateLogFiles Err = new CreateLogFiles();
        //        Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "LinkButton8_Click" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
        //    }
        //}

        protected void Button2_Click(object sender, EventArgs e)
        {
            if (Session["AppID"].ToString() != null && Session["AppID"].ToString() != "")
            {
                string redirectPage = "Application_registration.aspx";

                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "none", "<script language='javascript'>Popup('" + redirectPage + "');</script>", false);
            }
            // Response.Write("<script> window.open('" + redirectPage + "', '_blank');</script>");
            // Response.Redirect("~/User/Application_registration.aspx");
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
                    var str = ConfigurationManager.AppSettings["mongoconnect"];
                    IMongoDatabase database;
                    IMongoClient client;
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

                    RecordUserAction("ImageButton3_Command", ex.Message, "F");
                    Response.Write("<script language='javascript'>alert('" + "Cannot load pdf" + "')</script>");
                }
                finally
                {

                }
            }
            else
            {
                Response.Write("<script language='javascript'>alert('" + "Cannot load pdf" + "')</script>");
            }
        }

        protected string getmobileno(string appid)
        {
            string decrypt_mobile_no = "";
            if (Session["login_id"] != null)
            {
                NpgsqlConnection conn = new NpgsqlConnection();
                NpgsqlCommand cmd = new NpgsqlCommand();
                conn.ConnectionString = ConfigurationManager.ConnectionStrings["connect"].ConnectionString;
                cmd.Connection = conn;
                conn.Open();
                NpgsqlTransaction myTrans = conn.BeginTransaction();
                cmd.Transaction = myTrans;
                try
                {
                    string querys = "select applicant_mobile_no  from esociety.applicant_details where app_id = @appid";
                    cmd.CommandText = querys;
                    cmd.Parameters.AddWithValue("@appid", Convert.ToInt64(appid));
                    NpgsqlDataReader dr = cmd.ExecuteReader();
                    if (dr.Read())
                    {
                        decrypt_mobile_no = Server.HtmlEncode(Encryption.Encrypt.Decrypt(dr["applicant_mobile_no"].ToString()));
                    }
                    else
                    {
                        decrypt_mobile_no = "";
                    }
                    dr.Close();
                }
                catch (NpgsqlException ex)
                {
                    CreateLogFiles Err = new CreateLogFiles();
                    Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "getmobileno()" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));

                    RecordUserAction("getmobileno", ex.Message, "F");
                    decrypt_mobile_no = "";
                }
                finally
                {
                    conn.Close();
                }
            }
            else
            {
                decrypt_mobile_no = "";
                RecordUserAction("getmobilenofunction", "mobile no not Fetched beacuse of Session Id", "F");

            }
            return decrypt_mobile_no;
        }

        protected void viewform()
        {
            if (Session["login_id"] != null)
            {
                int renewal = Utility.checkifrenewal(Session["AppID"].ToString());
                if (renewal == 1)
                {
                    Div1.Visible = true;
                    Div2.Visible = false;
                    div_employee.Visible = false;
                }
                else if (renewal == 2)
                {
                    Div1.Visible = false;
                    Div2.Visible = true;
                    div_employee.Visible = true;
                }

                int exists = getmyID(Convert.ToInt64(Session["AppID"].ToString()), 7);
                if (exists == 1)
                {
                    listexist.Text = "Members List Available!!!";
                }
                else
                {
                    listexist.Text = "Members List Not Available!!!";
                }
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
                        if (Server.HtmlEncode(dr["applicant_email"].ToString()) == "NA")
                        {
                            appemail.Text = "";
                        }
                        else
                        {
                            string decrypt_email = Server.HtmlEncode(dr["applicant_email"].ToString());
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

                        value_society_type.Text = HttpUtility.HtmlEncode(dr["societytype"].ToString());
                        value_society_name.Text = Server.HtmlEncode(dr["socname"].ToString());
                        //valsocietyotherdoc1.Text = Server.HtmlEncode(dr["doc_one"].ToString());
                       // valsocietyotherdoc2.Text = Server.HtmlEncode(dr["doc_two"].ToString());
                        value_society_address.Text = Server.HtmlEncode(dr["socaddr"].ToString());
                        value_society_district.Text = Server.HtmlEncode(dr["DistrictName"].ToString());
                        value_taluka.Text = Server.HtmlEncode(dr["TalukaName"].ToString());
                        value_registration_fee.Text = "₹ " + Server.HtmlEncode(dr["regfee"].ToString());
                        value_processing_fee.Text = "₹ " + Server.HtmlEncode(dr["processfee"].ToString());
                        value_total_fee.Text = "₹ " + Server.HtmlEncode(dr["totalfee"].ToString());
                        //if (Server.HtmlEncode(rd["doc_one"].ToString()) == "" || Server.HtmlEncode(rd["doc_one"].ToString()) == null)
                        //{

                        //    docone.Visible = false;
                        //}
                        //else
                        //{
                        //    docone.Visible = true;
                        //    valsocietyotherdoc1.Text = Server.HtmlEncode(rd["doc_one"].ToString());

                        //}
                        //if (Server.HtmlEncode(rd["doc_two"].ToString()) == "" || Server.HtmlEncode(rd["doc_two"].ToString()) == null)
                        //{

                        //    doctwo.Visible = false;
                        //}
                        //else
                        //{
                        //    doctwo.Visible = true;
                        //    valsocietyotherdoc2.Text = Server.HtmlEncode(rd["doc_two"].ToString());

                        //}

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
                    NpgsqlDataReader rdr1 = cmd.ExecuteReader();
                    grvAddDocs.DataSource = rdr1;
                    grvAddDocs.DataBind();
                    rdr1.Close();


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



                    myTrans.Commit();
                }
                catch (NpgsqlException ex)
                {
                    CreateLogFiles Err = new CreateLogFiles();
                    Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "viewform()" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));

                    RecordUserAction("On Load Page", ex.Message, "F");
                    Response.Write("<script language='javascript'>alert('" + "View Applicant Details on Load Page Failed" + "')</script>");
                }
                finally
                {
                    conn.Close();
                }
            }
            else
            {
                RecordUserAction("View Form", "View Applicant Details not Fetched beacuse of Session Id", "F");
            }

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
        protected void Button1_Click(object sender, EventArgs e)
        {
            viewform();
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "none", "<script> $(window).on('load', function () { $('#exampleModalLong').modal({ backdrop: 'static' });});</script>", false);
        }
        protected void ImageButton7_Click(object sender, ImageClickEventArgs e)
        {
            Response.Redirect("DocumentUpload.aspx");
        }
        protected void btnconfirmsubmit_Click(object sender, EventArgs e)
        {
            string message = "Once you have submitted your application, it is not possible to make changes to the application form.";
            Label14.Text = message;
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "none", "<script> $(window).on('load', function () { $('#finalmodal').modal({ backdrop: 'static' });});</script>", false);
        }

        protected void ImageButton4_Click(object sender, ImageClickEventArgs e)
        {
            Response.Redirect("MemberDetails.aspx");
        }

        protected void Btn_RenewAppl_Click(object sender, EventArgs e)
        {
           
                string redirectPage = "Application_Renewal.aspx";

                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "none", "<script language='javascript'>Popup('" + redirectPage + "');</script>", false);
           
        }

        protected void Lb_RenewalApplication_Upload_Click(object sender, EventArgs e)
        {
            try
            {
                if (Session["AppID"] != null)
                {
                    //RENEWAL APPL ID=8 for mongo
                    int a = fileuploadfucntion(RenewalFileUpload1, lbrfu1status, doc, "Application for Renewal", ipaddress, macaddress, Convert.ToInt64(Session["AppID"].ToString()), "Society Documents", 8);
                    if (a == 1)
                    {
                        lbrfu1status.ForeColor = System.Drawing.Color.Green;
                        lbrfu1status.Text = "Uploaded Successfully";
                        RecordUserAction("Lb_RenewalApplication_Upload_Click", "File Uploaded Successfully", "S");
                        Lb_RenewalApplication_View.Enabled = true;
                        RenewalFileUpload1.Enabled = false;
                        Lb_RenewalApplication_Upload.Enabled = false;
                        Session["du_flag8"] = 1;
                    }
                    else
                    {
                        RecordUserAction("Lb_RenewalApplication_Upload_Click", "File Uploaded Failed", "F");
                        lbrfu1status.ForeColor = System.Drawing.Color.Red;
                        lbrfu1status.Text = "Failed";
                        Session["du_flag8"] = 0;
                    }
                }
                else
                {
                    RecordUserAction("Lb_RenewalApplication_Upload_Click", "Session null", "F");
                }
            }
            catch (Exception ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "Lb_RenewalApplication_Upload_Click" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
            }
        }

        protected void Lb_RenewalApplication_Delete_Click(object sender, EventArgs e)
        {
            try
            {
                var status = getPageStatus(Convert.ToInt64(Session["AppID"].ToString()), 8);
                string objectid = status.Item2;
                if (objectid == "" || objectid == null)
                {
                    Response.Write("<script language='javascript'>alert('File Not present to delete')</script>");
                }
                else
                {

                    int a = DeleteFunction(objectid);
                    if (a == 1)
                    {
                        lbrfu1status.Text = "File Deleted";
                        Response.Write("<script language='javascript'>alert('Deleted Successfully')</script>");
                        Lb_RenewalApplication_View.Enabled = false;
                        Lb_RenewalApplication_Upload.Enabled = true;
                        RenewalFileUpload1.Enabled = true;
                        Session["du_flag1"] = 0;
                    }
                    else
                    {
                        lbrfu1status.Text = "Failed...Kindly try again";
                        Response.Write("<script language='javascript'>alert('Failed...Kindly try again')</script>");
                    }
                }
            }
            catch (Exception ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "Lb_RenewalApplication_Delete_Click" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
            }
        }

        protected void Lb_RenewalApplication_View_Click(object sender, EventArgs e)
        {
            try
            {
                var status = getPageStatus(Convert.ToInt64(Session["AppID"].ToString()), 8);
                string objectid = status.Item2;
                openConnectionMongo_societyDocs(objectid);
                RecordUserAction("Lb_RenewalApplication_View_Click", "User Clicked on Lb_RenewalApplication_View_Click", "S");
            }
            catch (Exception ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "Lb_RenewalApplication_View_Click" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
            }
        }
        

        protected void LB_Schedule4_Upload_Click(object sender, EventArgs e)    // function to upload Schedule Iv
        {
            try
            {
                if (Session["AppID"] != null)
                {
                    int b = fileuploadfucntion(RenewalFileUpload5, lbrfu5status, doc, "Schedule IV", ipaddress, macaddress, Convert.ToInt64(Session["AppID"].ToString()), "Society Documents", 12);
                    if (b == 1)
                    {
                        lbrfu5status.ForeColor = System.Drawing.Color.Green;
                        lbrfu5status.Text = "Uploaded";
                        RecordUserAction("LB_Schedule4_Upload_Click", "File Uploaded Successfully", "S");
                        LB_Schedule4_View.Enabled = true;
                        LB_Schedule4_Upload.Enabled = false;
                        RenewalFileUpload3.Enabled = false;
                        Session["du_flag12"] = 1;
                    }
                    else
                    {
                        RecordUserAction("LB_Schedule4_Upload_Click", "File Uploaded Failed", "F");
                        lbrfu5status.ForeColor = System.Drawing.Color.Red;
                        lbrfu5status.Text = "Failed";
                        Session["du_flag12"] = 0;
                    }
                }
                else
                {
                    RecordUserAction("LB_Schedule4_Upload_Click", "Session null", "F");
                }
            }
            catch (Exception ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "LB_Schedule4_Upload_Click" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
            }
        }

        protected void LB_Schedule4_Delete_Click(object sender, EventArgs e) // delete schedule IV
        {
            try
            {
                var status = getPageStatus(Convert.ToInt64(Session["AppID"].ToString()), 12);
                string objectid = status.Item2;
                if (objectid == "" || objectid == null)
                {
                    Response.Write("<script language='javascript'>alert('File Not present to delete')</script>");
                }
                else
                {

                    int a = DeleteFunction(objectid);
                    if (a == 1)
                    {
                        lbrfu5status.Text = "File Deleted";
                        Response.Write("<script language='javascript'>alert('Deleted Successfully')</script>");
                        LB_Schedule4_View.Enabled = false;
                        LB_Schedule4_Upload.Enabled = true;
                        RenewalFileUpload5.Enabled = true;
                        Session["du_flag12"] = 0;
                    }
                    else
                    {
                        lbrfu3status.Text = "Failed...Kindly try again";
                        Response.Write("<script language='javascript'>alert('Failed...Kindly try again')</script>");
                    }
                }
            }
            catch (Exception ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "LB_Schedule4_Delete_Click" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
            }
        }

        protected void LB_Schedule4_View_Click(object sender, EventArgs e)  // view schedule IV
        {
            try
            {
                var status = getPageStatus(Convert.ToInt64(Session["AppID"].ToString()), 12);
                string objectid = status.Item2;
                openConnectionMongo_societyDocs(objectid);
                RecordUserAction("LB_Schedule4_View_Click", "User Clicked on LB_Schedule4_View_Click", "S");
            }
            catch (Exception ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "LB_Schedule4_View_Click" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
            }
        }

        protected void btnrenewaldocssubmit_Click(object sender, EventArgs e)
        {
            string message = "Once you have submitted your application, it is not possible to make changes to the application form.";
            Label14.Text = message;
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "none", "<script> $(window).on('load', function () { $('#finalmodal').modal({ backdrop: 'static' });});</script>", false);

        }

        protected void btnrenewalform_view_Click(object sender, EventArgs e)
        {
            viewform();
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "none", "<script> $(window).on('load', function () { $('#exampleModalLong').modal({ backdrop: 'static' });});</script>", false);

        }

       

        protected void LkAddDocs_Click(object sender, EventArgs e)
        {
            try
            {
                txtbx_DocName.Text = "";
                FileUpload9.Dispose();
                lblError.Text = "";
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "none", "<script> $(window).on('load', function () { $('#otherdocumentuploads').modal({ backdrop: 'static' });});</script>", false);
            }
            catch (Exception ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "LkAddDocs_Click" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath) + " from " + ipaddress);
            }
        }

        protected void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                Validate _val = new Validate();
                if (txtbx_DocName.Text == "" || txtbx_DocName.Text == null)
                {
                    lblError.Text = "Document Name is Blank";
                    txtbx_DocName.Focus();
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "none", "<script> $(window).on('load', function () { $('#otherdocumentuploads').modal({ backdrop: 'static' });});</script>", false);
                }
                else if (!_val.validateData(txtbx_DocName.Text, _val.name))
                {
                    lblError.Text = "Document Name is Invalid";
                    txtbx_DocName.Focus();
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "none", "<script> $(window).on('load', function () { $('#otherdocumentuploads').modal({ backdrop: 'static' });});</script>", false);
                }
                else if (!FileUpload9.HasFile)
                {
                    lblError.Text = "File not Uploaded";
                    FileUpload9.Focus();
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "none", "<script> $(window).on('load', function () { $('#otherdocumentuploads').modal({ backdrop: 'static' });});</script>", false);
                }
                else
                {
                    int a = fileuploadfucntion(FileUpload9, lblError, doc, txtbx_DocName.Text, ipaddress, macaddress, Convert.ToInt64(Session["AppID"].ToString()), "Other Documents", 0);
                    if (a == 1)
                    {
                        lblError.ForeColor = System.Drawing.Color.Green;
                        lblError.Text = "Uploaded Successfully";
                        RecordUserAction("btnAdd_Click", "File Uploaded Successfully", "S");
                        tr_adddocsgridview.Visible = true;
                        bindgridview(Session["AppID"].ToString());
                        Response.Redirect("DocumentUpload.aspx");
                    }
                    else
                    {
                        RecordUserAction("btnAdd_Click", "File Uploaded Failed", "F");
                        lblError.ForeColor = System.Drawing.Color.Red;

                        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "none", "<script> $(window).on('load', function () { $('#otherdocumentuploads').modal({ backdrop: 'static' });});</script>", false);
                    }
                }
            }
            catch (Exception ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "btnAdd_Click" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath) + " from " + ipaddress);
            }
        }


        public void bindgridview(string AppID)
        {
            int renewal_flag = Utility.checkifrenewal(AppID);

            NpgsqlConnection conn = new NpgsqlConnection();
            NpgsqlCommand cmd = new NpgsqlCommand();
            conn.ConnectionString = ConfigurationManager.ConnectionStrings["connect"].ConnectionString;
            cmd.Connection = conn;
            try
            {
                conn.Open();
                string query = "SELECT object_id,docname from esociety.otherdoc_crossentry where app_id=@appid and myid=0";
                cmd.CommandText = query;
                cmd.Parameters.AddWithValue("@appid", Convert.ToInt64(AppID));

                NpgsqlDataAdapter da = new NpgsqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                da.Fill(ds);
                if (ds.Tables[0].Rows.Count > 0)
                {
                    if (renewal_flag == 1)
                    {
                        tr_regadddocs.Visible = true;
                        GridViewAdditionalDocs_reg.DataSource = ds;
                        GridViewAdditionalDocs_reg.DataBind();
                    }
                    else if (renewal_flag == 2)
                    {
                        tr_adddocsgridview.Visible = true;
                        GrideViewAdditionalDocs.DataSource = ds;
                        GrideViewAdditionalDocs.DataBind();
                    }


                }
                else
                {
                    tr_adddocsgridview.Visible = false;
                    tr_regadddocs.Visible = false;
                }
                da.Dispose();
            }
            catch (NpgsqlException ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "bindgridview()" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath) + " from " + ipaddress);
                RecordUserAction("bindgridview", ex.Message, "F");
                Response.Write("<script language='javascript'>alert('" + "Exception in Gridview" + "')</script>");
            }
            finally
            {
                conn.Close();
            }
        }

        protected void LBDelete_Click(object sender, EventArgs e)
        {
            LinkButton btn = (LinkButton)sender;
            GridViewRow row = (GridViewRow)btn.NamingContainer;
            HiddenField hdobid = row.FindControl("hfobjectID") as HiddenField;

            if (hdobid != null)
            {
                try
                {
                    string objectid = hdobid.Value.ToString();
                    int a = DeleteFunction(objectid);
                    if (a == 1)
                    {
                        Response.Write("<script language='javascript'>alert('Deleted Successfully')</script>");
                        bindgridview(Session["AppID"].ToString());
                    }
                    else
                    {
                        Response.Write("<script language='javascript'>alert('Failed...Kindly try again')</script>");
                    }


                }
                catch (Exception ex)
                {
                    CreateLogFiles Err = new CreateLogFiles();
                    Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "LBDelete_Click" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
                }
            }

        }

        


        protected void LbView_Click(object sender, EventArgs e)
        {
            LinkButton btn = (LinkButton)sender;
            GridViewRow row = (GridViewRow)btn.NamingContainer;
            HiddenField hdobid = row.FindControl("hfobjectID") as HiddenField;
            if (hdobid != null)
            {
                string objectid = hdobid.Value;
                try
                {
                    var str = ConfigurationManager.AppSettings["mongoconnect"];
                    IMongoDatabase database;
                    IMongoClient client;
                    client = new MongoClient(str);
                    database = client.GetDatabase("eGoaSociety");
                    var collection = database.GetCollection<OtherDocuments>("Other Documents");
                    var status = collection.Find(x => x._Id == ObjectId.Parse(objectid)).FirstOrDefault();
                    byte[] decrypt_pdf_bytes = status.DocContent;
                    byte[] pdf = obj_Byte_Encryption.DecryptData(decrypt_pdf_bytes, System.Web.HttpContext.Current.Server.MapPath("~/ENCRYPT.KEY"));
                    convertToPdf(pdf);
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

        protected void GridViewAdditionalDocs_reg_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (Convert.ToInt32(Session["status_Id"]) == 1 || Convert.ToInt32(Session["status_Id"]) == 2 || Convert.ToInt32(Session["status_Id"]) == 5)
                { }
                else
                {
                    ((DataControlField)GridViewAdditionalDocs_reg.Columns
                .Cast<DataControlField>()
                .Where(fld => fld.HeaderText == "Delete")
                .SingleOrDefault()).Visible = false;

                }
            }
            catch (Exception ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "GridViewAdditionalDocs_reg_RowDataBound" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
            }
        }

        protected void GrideViewAdditionalDocs_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (Convert.ToInt32(Session["status_Id"]) == 1 || Convert.ToInt32(Session["status_Id"]) == 2 || Convert.ToInt32(Session["status_Id"]) == 5)
                { }
                else
                {
                    ((DataControlField)GrideViewAdditionalDocs.Columns
                .Cast<DataControlField>()
                .Where(fld => fld.HeaderText == "Delete")
                .SingleOrDefault()).Visible = false;

                }
            }
            catch (Exception ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "GrideViewAdditionalDocs_RowDataBound" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
            }

        }

        protected void BtnMemo_Click(object sender, EventArgs e)
        {
            if (Session["AppID"].ToString() != null && Session["AppID"].ToString() != "")
            {
                string redirectPage = "MemorandumOfAssociation.aspx";

                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "none", "<script language='javascript'>Popup('" + redirectPage + "');</script>", false);

            }
        }

        protected void Lb_GenerateRenewalApplication_Click(object sender, EventArgs e)
        {
            if(Session["AppID"].ToString() != null && Session["AppID"].ToString() != "")
            {
                string redirectPage = "Application_Renewal.aspx";

                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "none", "<script language='javascript'>Popup('" + redirectPage + "');</script>", false);
            }
        }

        protected void LB_GenerateSchedule1_Click(object sender, EventArgs e)
        {
            if (Session["AppID"].ToString() != null && Session["AppID"].ToString() != "")
            {
                string redirectPage = "Schedule1.aspx";

                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "none", "<script language='javascript'>Popup('" + redirectPage + "');</script>", false);
            }
        }

        protected void LB_Schedule1_Upload_Click(object sender, EventArgs e)
        {
            try
            {
                if (Session["AppID"] != null)
                {
                    int b = fileuploadfucntion(RenewalFileUpload2, lbrfu2status, doc, "Schedule I", ipaddress, macaddress, Convert.ToInt64(Session["AppID"].ToString()), "Society Documents", 9);
                    if (b == 1)
                    {
                        lbrfu2status.ForeColor = System.Drawing.Color.Green;
                        lbrfu2status.Text = "Uploaded";
                        RecordUserAction("LB_Schedule2_Upload_Click", "File Uploaded Successfully", "S");
                        LB_Schedule1_View.Enabled = true;
                        RenewalFileUpload2.Enabled = false;
                        LB_Schedule1_Upload.Enabled = false;
                        Session["du_flag9"] = 1;
                    }
                    else
                    {
                        RecordUserAction("LB_Schedule2_Upload_Click", "File Uploaded Failed", "F");
                        lbrfu2status.ForeColor = System.Drawing.Color.Red;
                        lbrfu2status.Text = "Failed";
                        Session["du_flag9"] = 0;
                    }
                }
                else
                {
                    RecordUserAction("LB_Schedule2_Upload_Click", "Session null", "F");
                }
            }
            catch (Exception ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "LB_Schedule1_Upload_Click" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
            }
        }

        protected void LB_Schedule1_Delete_Click(object sender, EventArgs e)
        {
            try
            {
                var status = getPageStatus(Convert.ToInt64(Session["AppID"].ToString()), 9);
                string objectid = status.Item2;
                if (objectid == "" || objectid == null)
                {
                    Response.Write("<script language='javascript'>alert('File Not present to delete')</script>");
                }
                else
                {

                    int a = DeleteFunction(objectid);
                    if (a == 1)
                    {
                        lbrfu2status.Text = "File Deleted";
                        Response.Write("<script language='javascript'>alert('Deleted Successfully')</script>");
                        LB_Schedule1_View.Enabled = false;
                        LB_Schedule1_Upload.Enabled = true;
                        RenewalFileUpload2.Enabled = true;
                        Session["du_flag9"] = 0;
                    }
                    else
                    {
                        lbrfu2status.Text = "Failed...Kindly try again";
                        Response.Write("<script language='javascript'>alert('Failed...Kindly try again')</script>");
                    }
                }
            }
            catch (Exception ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "LB_Schedule1_Delete_Click" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
            }
        }

        protected void LB_Schedule1_View_Click(object sender, EventArgs e)
        {
            try
            {
                var status = getPageStatus(Convert.ToInt64(Session["AppID"].ToString()), 9);
                string objectid = status.Item2;
                openConnectionMongo_societyDocs(objectid);
                RecordUserAction("LB_Schedule1_View_Click", "User Clicked on LB_EmpDetails_View_Click", "S");
            }
            catch (Exception ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "LB_Schedule1_View_Click" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
            }
        }

        

        protected void LB_GenerateSchedule2_Click(object sender, EventArgs e)
        {
            if (Session["AppID"].ToString() != null && Session["AppID"].ToString() != "")
            {
                string redirectPage = "Schedule2.aspx";

                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "none", "<script language='javascript'>Popup('" + redirectPage + "');</script>", false);
            }
        }

        protected void LB_Schedule2_Upload_Click(object sender, EventArgs e) // upload schedule 2/ employee details
        {
            try
            {
                if (Session["AppID"] != null)
                {
                    int b = fileuploadfucntion(RenewalFileUpload4, lbrfu4status, doc, "Schedule II", ipaddress, macaddress, Convert.ToInt64(Session["AppID"].ToString()), "Society Documents", 11);
                    if (b == 1)
                    {
                        lbrfu4status.ForeColor = System.Drawing.Color.Green;
                        lbrfu4status.Text = "Uploaded";
                        RecordUserAction("LB_Schedule2_Upload_Click", "File Uploaded Successfully", "S");
                        LB_Schedule2_View.Enabled = true;
                        RenewalFileUpload4.Enabled = false;
                        LB_Schedule2_Upload.Enabled = false;
                        Session["du_flag11"] = 1;
                    }
                    else
                    {
                        RecordUserAction("LB_Schedule2_Upload_Click", "File Uploaded Failed", "F");
                        lbrfu4status.ForeColor = System.Drawing.Color.Red;
                        lbrfu4status.Text = "Failed";
                        Session["du_flag11"] = 0;
                    }
                }
                else
                {
                    RecordUserAction("LB_Schedule2_Upload_Click", "Session null", "F");
                }
            }
            catch (Exception ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "LB_Schedule2_Upload_Click" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
            }
        }

        protected void LB_Schedule2_Delete_Click(object sender, EventArgs e)
        {
            try
            {
                var status = getPageStatus(Convert.ToInt64(Session["AppID"].ToString()), 11);
                string objectid = status.Item2;
                if (objectid == "" || objectid == null)
                {
                    Response.Write("<script language='javascript'>alert('File Not present to delete')</script>");
                }
                else
                {

                    int a = DeleteFunction(objectid);
                    if (a == 1)
                    {
                        lbrfu4status.Text = "File Deleted";
                        Response.Write("<script language='javascript'>alert('Deleted Successfully')</script>");
                        LB_Schedule2_View.Enabled = false;
                        LB_Schedule2_Upload.Enabled = true;
                        RenewalFileUpload4.Enabled = true;
                        Session["du_flag11"] = 0;
                    }
                    else
                    {
                        lbrfu4status.Text = "Failed...Kindly try again";
                        Response.Write("<script language='javascript'>alert('Failed...Kindly try again')</script>");
                    }
                }
            }
            catch (Exception ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "LB_Schedule2_Delete_Click" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
            }
        }

        protected void LB_Schedule2_View_Click(object sender, EventArgs e)
        {
            try
            {
                var status = getPageStatus(Convert.ToInt64(Session["AppID"].ToString()), 11);
                string objectid = status.Item2;
                openConnectionMongo_societyDocs(objectid);
                RecordUserAction("LB_Schedule2_View_Click", "User Clicked on LB_EmpDetails_View_Click", "S");
            }
            catch (Exception ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "LB_Schedule2_View_Click" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
            }
        }


        protected void LB_GenerateSchedule6_Click(object sender, EventArgs e)
        {
            if (Session["AppID"].ToString() != null && Session["AppID"].ToString() != "")
            {
                string redirectPage = "ScheduleVI.aspx";

                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "none", "<script language='javascript'>Popup('" + redirectPage + "');</script>", false);
            }
        }       
       

        protected void LB_Schedule6_Upload_Click(object sender, EventArgs e)
        {
            try
            {
                if (Session["AppID"] != null)
                {                    
                    int a = fileuploadfucntion(RenewalFileUpload3, lbrfu3status, doc, "Schedule VI", ipaddress, macaddress, Convert.ToInt64(Session["AppID"].ToString()), "Society Documents", 10);
                    if (a == 1)
                    {
                        lbrfu3status.ForeColor = System.Drawing.Color.Green;
                        lbrfu3status.Text = "Uploaded Successfully";
                        RecordUserAction("LB_Schedule6_Upload_Click", "File Uploaded Successfully", "S");
                        LB_Schedule6_View.Enabled = true;
                        RenewalFileUpload3.Enabled = false;
                        LB_Schedule6_Upload.Enabled = false;
                        Session["du_flag10"] = 1;
                    }
                    else
                    {
                        RecordUserAction("LB_Schedule6_Upload_Click", "File Uploaded Failed", "F");
                        lbrfu3status.ForeColor = System.Drawing.Color.Red;
                        lbrfu3status.Text = "Failed";
                        Session["du_flag10"] = 0;
                    }
                }
                else
                {
                    RecordUserAction("LB_Schedule6_Upload_Click", "Session null", "F");
                }
            }
            catch (Exception ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "LB_Schedule6_Upload_Click" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
            }
        }

        protected void LB_Schedule6_Delete_Click(object sender, EventArgs e)
        {
            try
            {
                var status = getPageStatus(Convert.ToInt64(Session["AppID"].ToString()), 10);
                string objectid = status.Item2;
                if (objectid == "" || objectid == null)
                {
                    Response.Write("<script language='javascript'>alert('File Not present to delete')</script>");
                }
                else
                {

                    int a = DeleteFunction(objectid);
                    if (a == 1)
                    {
                        lbrfu3status.Text = "File Deleted";
                        Response.Write("<script language='javascript'>alert('Deleted Successfully')</script>");
                        LB_Schedule6_View.Enabled = false;
                        LB_Schedule6_Upload.Enabled = true;
                        RenewalFileUpload3.Enabled = true;
                        Session["du_flag10"] = 0;
                    }
                    else
                    {
                        lbrfu4status.Text = "Failed...Kindly try again";
                        Response.Write("<script language='javascript'>alert('Failed...Kindly try again')</script>");
                    }
                }
            }
            catch (Exception ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "LB_Schedule6_Delete_Click" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
            }
        }


        protected void LB_Schedule6_View_Click(object sender, EventArgs e)
        {
            try
            {
                var status = getPageStatus(Convert.ToInt64(Session["AppID"].ToString()), 10);
                string objectid = status.Item2;
                openConnectionMongo_societyDocs(objectid);
                RecordUserAction("LB_Schedule6_View_Click", "User Clicked on LB_EmpDetails_View_Click", "S");
            }
            catch (Exception ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "LB_Schedule6_View_Click" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
            }
        }

        protected void BtnBack_Click(object sender, EventArgs e)
        {
            if (Convert.ToInt32(Session["Renewal"]) == 2)
            {
                Response.Redirect("PaidEmployee.aspx");
            }
            else
            {
                Response.Redirect("NormalMembers.aspx");
            }
        }

        protected void Button3_Click(object sender, EventArgs e)
        {
            if (Convert.ToInt32(Session["Renewal"]) == 2)
            {
                Response.Redirect("PaidEmployee.aspx");
            }
            else
            {
                Response.Redirect("NormalMembers.aspx");
            }
        }

        protected void ImageButton_employee_Command(object sender, CommandEventArgs e)
        {
            Response.Redirect("PaidEmployee.aspx");
        }
    }
}