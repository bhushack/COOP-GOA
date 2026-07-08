using GoaSocietyRegistration.Development;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Net;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using WS_Encryption;
using System.Data;
using iTextSharp.text.pdf;
using System.Collections.Specialized;

namespace GoaSocietyRegistration.User
{
    public partial class Amendment : System.Web.UI.Page
    {
        //static int n = 2;//n = file size in MB <------------------------------------
        //static long filesize = n * 1024 * 1024;

        
        string ct = string.Empty;
        Int64 fileSizeFront = 0;
        byte[] documentBinary = new Byte[0];
        string ipaddress = Utility.getIP();
        string macaddress = Utility.GetMACAddress(Utility.getIP());
        Validate _val = new Validate();
        OtherDocuments doc = new OtherDocuments();
        NICEncryption _encryption = new NICEncryption();
        ByteEncryption.ByteEncryption obj_Byte_Encryption = new ByteEncryption.ByteEncryption();
        System.Globalization.CultureInfo french = new System.Globalization.CultureInfo("fr-FR");
        [DllImport(@"urlmon.dll", CharSet = CharSet.Auto)]
        private extern static UInt32 FindMimeFromData(UInt32 pBC, [MarshalAs(UnmanagedType.LPStr)] String pwzUrl, [MarshalAs(UnmanagedType.LPArray)] byte[] pBuffer,
        UInt32 cbSize, [MarshalAs(UnmanagedType.LPStr)] String pwzMimeProposed, UInt32 dwMimeFlags, out UInt32 ppwzMimeOut, UInt32 dwReserverd);


        protected void Page_Load(object sender, EventArgs e)
        {
            HttpContext.Current.Response.AddHeader("Cache-Control", "no-cache, no-store, must-revalidate");
            HttpContext.Current.Response.AddHeader("Pragma", "no-cache");
            HttpContext.Current.Response.AddHeader("Expires", "0");
            if (!IsPostBack)
            {
                Session["au_flag1"] = 0;
                Session["au_flag2"] = 0;
                Session["au_flag3"] = 0;
                Session["au_flag4"] = 0;
                Session["au_flag5"] = 0;
                Session["au_flag6"] = 0;
                ViewState["edit_flag"] = 0;

                LoadSocietyDetails();

                bindgridview_existingmangcomm(Session["AppID"].ToString());

                FillSalutation();
                FillOccupation();
                FillProof();
                FillDesignation();

            }

            var fu1 = getPageStatus(Convert.ToInt64(Session["AppID"].ToString()), 1);
            if (fu1.Item1 == "Y")
            {
                RecordUserAction("Page_load", "Amendment File already Uploaded in mongo 1", "S");
                lbfu1status.Text = "File Uploaded";
                LB_OrgByelaws_Upload.Enabled = false;
                FileUpload1.Enabled = false;
                LB_OrgByelaws_View.Enabled = true;
                Session["au_flag1"] = 1;
            }
            else
            {
                lbfu1status.Text = "";
                LB_OrgByelaws_Upload.Enabled = true;
                FileUpload1.Enabled = true;
                Session["au_flag1"] = 0;

            }

            var fu2 = getPageStatus(Convert.ToInt64(Session["AppID"].ToString()), 2);
            if (fu2.Item1 == "Y")
            {
                RecordUserAction("Page_load", "Amendment File already Uploaded in mongo 2", "S");
                lbfu2status.Text = "File Uploaded";
                LB_StatementofChanges_Upload.Enabled = false;
                FileUpload2.Enabled = false;
                LB_StatementofChanges_View.Enabled = true;
                Session["au_flag2"] = 1;
            }
            else
            {
                lbfu2status.Text = "";
                LB_StatementofChanges_Upload.Enabled = true;
                FileUpload2.Enabled = true;
                Session["au_flag2"] = 0;

            }

            var fu3 = getPageStatus(Convert.ToInt64(Session["AppID"].ToString()), 3);
            if (fu3.Item1 == "Y")
            {
                RecordUserAction("Page_load", "Amendment File already Uploaded in mongo 3", "S");
                lbfu3status.Text = "File Uploaded";
                LB_AgmNotice_Upload.Enabled = false;
                FileUpload3.Enabled = false;
                LB_AgmNotice_View.Enabled = true;
                Session["au_flag3"] = 1;
            }
            else
            {
                lbfu3status.Text = "";
                LB_AgmNotice_Upload.Enabled = true;
                FileUpload3.Enabled = true;
                Session["au_flag3"] = 0;

            }

            var fu4 = getPageStatus(Convert.ToInt64(Session["AppID"].ToString()), 4);
            if (fu4.Item1 == "Y")
            {
                RecordUserAction("Page_load", "Amendment File already Uploaded in mongo 4", "S");
                lbfu4status.Text = "File Uploaded";
                LB_AgmResolution_Upload.Enabled = false;
                FileUpload4.Enabled = false;
                LB_AgmResolution_View.Enabled = true;
                Session["au_flag4"] = 1;
            }
            else
            {
                lbfu4status.Text = "";
                LB_AgmResolution_Upload.Enabled = true;
                FileUpload4.Enabled = true;
                Session["au_flag4"] = 0;

            }
            var fu5 = getPageStatus(Convert.ToInt64(Session["AppID"].ToString()), 5);
            if (fu5.Item1 == "Y")
            {
                RecordUserAction("Page_load", "Amendment File already Uploaded in mongo 5", "S");
                lbfu5status.Text = "File Uploaded";
                Lb_AmendByelaws_Upload.Enabled = false;
                FileUpload5.Enabled = false;
                Lb_AmendByelaws_View.Enabled = true;
                Session["au_flag5"] = 1;
            }
            else
            {
                lbfu5status.Text = "";
                Lb_AmendByelaws_Upload.Enabled = true;
                FileUpload5.Enabled = true;
                Session["au_flag5"] = 0;

            }
            var fu6 = getPageStatus(Convert.ToInt64(Session["AppID"].ToString()), 6);
            if (fu6.Item1 == "Y")
            {
                RecordUserAction("Page_load", "Amendment File already Uploaded in mongo 6", "S");
                lbfu6status.Text = "File Uploaded";
                LB_RevisedVersion_Upload.Enabled = false;
                FileUpload6.Enabled = false;
                LB_RevisedVersion_View.Enabled = true;
                Session["au_flag6"] = 1;
            }
            else
            {
                lbfu6status.Text = "";
                LB_RevisedVersion_Upload.Enabled = true;
                FileUpload6.Enabled = true;
                Session["au_flag6"] = 0;

            }
            get_status();
            bindgridview_additonaldocs(Session["AppID"].ToString());

            
            bindgridview_newmangcomm(Session["AppID"].ToString());

            if (Convert.ToInt32(ViewState["edit_flag"].ToString()) == 1)
            {
                btnAdd.Visible = false;
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

        public void FillProof()
        {
            NpgsqlConnection connect = new NpgsqlConnection();
            NpgsqlCommand cmd = new NpgsqlCommand();
            connect.ConnectionString = ConfigurationManager.ConnectionStrings["connect"].ConnectionString;
            cmd.Connection = connect;
            NpgsqlDataAdapter adapter;
            DataSet ds;
            try
            {

                connect.Open();
                string query = "SELECT \"ProofName\",\"ProofID\" FROM esociety.mst_idproof";
                cmd.CommandText = query;
                adapter = new NpgsqlDataAdapter(cmd);
                ds = new DataSet();
                adapter.Fill(ds, "esociety.mst_idproof");
                ddl_MemDocType.DataSource = ds.Tables[0];
                ddl_MemDocType.DataTextField = "ProofName";
                ddl_MemDocType.DataValueField = "ProofID";
                ddl_MemDocType.DataBind();
                ddl_MemDocType.Items.Insert(0, new ListItem("-- Select --", "-1"));
                cmd.Dispose();
                adapter.Dispose();
                ds = null;
            }
            catch (NpgsqlException ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "FillProof()" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
                RecordUserAction("FillProof", ex.Message, "F");
                Response.Write("<script language='javascript'>alert('ID Proof DropDown:" + "error" + "')</script>");
            }
            finally
            {
                connect.Close();
            }
        }
        public void FillOccupation()

        {
            NpgsqlConnection connect = new NpgsqlConnection();
            NpgsqlCommand cmd = new NpgsqlCommand();
            connect.ConnectionString = ConfigurationManager.ConnectionStrings["connect"].ConnectionString;
            cmd.Connection = connect;
            NpgsqlDataAdapter adapter;
            DataSet ds;
            try
            {
                connect.Open();
                string query = "SELECT \"OccupationName\",\"OccupationID\" FROM esociety.mst_occupation";
                cmd.CommandText = query;
                adapter = new NpgsqlDataAdapter(cmd);
                ds = new DataSet();
                adapter.Fill(ds, "esociety.mst_occupation");
                ddl_MemOccup.DataSource = ds.Tables[0];
                ddl_MemOccup.DataTextField = "OccupationName";
                ddl_MemOccup.DataValueField = "OccupationID";
                ddl_MemOccup.DataBind();
                ddl_MemOccup.Items.Insert(0, new ListItem("-- Select --", "-1"));
                cmd.Dispose();
                adapter.Dispose();
                ds = null;
            }
            catch (NpgsqlException ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "FillOccupation()" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
                RecordUserAction("FillOccupation", ex.Message, "F");
                Response.Write("<script language='javascript'>alert('Occupation DropDown:" + "Error" + "')</script>");
            }
            finally
            {
                connect.Close();
            }
        }

        public void FillSalutation()
        {
            NpgsqlConnection connect = new NpgsqlConnection();
            NpgsqlCommand cmd = new NpgsqlCommand();
            connect.ConnectionString = ConfigurationManager.ConnectionStrings["connect"].ConnectionString;
            cmd.Connection = connect;
            NpgsqlDataAdapter adapter;
            DataSet ds;
            try
            {
                connect.Open();
                string query = "SELECT salutation_name,salutation_id FROM esociety.mst_salutation";
                cmd.CommandText = query;
                adapter = new NpgsqlDataAdapter(cmd);
                ds = new DataSet();
                adapter.Fill(ds, "esociety.mst_salutation");
                ddl_salutation.DataSource = ds.Tables[0];
                ddl_salutation.DataTextField = "salutation_name";
                ddl_salutation.DataValueField = "salutation_id";
                ddl_salutation.DataBind();
                ddl_salutation.Items.Insert(0, new ListItem("-- Select --", "-1"));
                cmd.Dispose();
                adapter.Dispose();
                ds = null;
            }
            catch (NpgsqlException ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "FillSalutation()" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
                RecordUserAction("FillSalutation", ex.Message, "F");
                Response.Write("<script language='javascript'>alert('Salutation DropDown:" + "Error" + "')</script>");
            }
            finally
            {
                connect.Close();
            }
        }

        public void FillDesignation()
        {
            NpgsqlConnection connect = new NpgsqlConnection();
            NpgsqlCommand cmd = new NpgsqlCommand();
            connect.ConnectionString = ConfigurationManager.ConnectionStrings["connect"].ConnectionString;
            cmd.Connection = connect;
            NpgsqlDataAdapter adapter;
            DataSet ds;
            try
            {
                connect.Open();
                string query = "SELECT \"DesignationName\",\"DesignationID\" FROM esociety.mst_memberdesignation where \"DesignationID\"!=9";
                cmd.CommandText = query;
                adapter = new NpgsqlDataAdapter(cmd);
                ds = new DataSet();
                adapter.Fill(ds, "esociety.mst_memberdesignation");
                ddl_DesignMem.DataSource = ds.Tables[0];
                ddl_DesignMem.DataTextField = "DesignationName";
                ddl_DesignMem.DataValueField = "DesignationID";
                ddl_DesignMem.DataBind();
                ddl_DesignMem.Items.Insert(0, new ListItem("-- Select --", "-1"));
                cmd.Dispose();
                adapter.Dispose();
                ds = null;
            }
            catch (NpgsqlException ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "FillDesignation()" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
                RecordUserAction("FillDesignation", ex.Message, "F");
                Response.Write("<script language='javascript'>alert('Designation DropDown:" + "Error" + "')</script>");
            }
            finally
            {
                connect.Close();
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
            catch(Exception ex)
            {
                a = -1;
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "getnoofPages()" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));

            }
            return a;
        }
        public int fileuploadfunction(FileUpload fu, Label lb, OtherDocuments othdoc, string a, string ip, string mac, long App_ID, string collection, int myid)
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
                    Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "fileuploadfunction()" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
                    lb.Text = "File upload failed.";
                    RecordUserAction("Fileupload Function ", ex.Message, "F");
                    Label48.ForeColor = System.Drawing.Color.Red;
                    Label48.Text = "File upload failed.";
                }
            }
            return flag;
        }
        public int insertentryPosgres(ObjectId obj, string docid, long appid, int myid, string docname,int pagescount)
        {
            int value = 0;
            string ins_query = "";
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
                        if (myid == 0)
                        {
                            ins_query = "INSERT INTO esociety.otherdoc_amendment(object_id,app_id,other_docid,active,docname,amend_year,created_at,created_by,ipaddress,macaddress,noofpages)VALUES(@object_id,@app_id,@docid,'Y',@docname,@amend_year,current_timestamp,@created_by,@ipaddress,@macaddress,@noofpages)";

                        }
                        else
                        {
                           ins_query = "INSERT INTO esociety.docs_amendment(object_id, docid, app_id,active,myid,docname,amend_year,created_at,created_by,ipaddress,macaddress,noofpages)VALUES(@object_id,@docid, @app_id,'Y',@myid,@docname,@amend_year,current_timestamp,@created_by,@ipaddress,@macaddress,@noofpages)";

                        }
                        cmd.CommandText = ins_query;
                        cmd.Parameters.AddWithValue("@object_id", objectid);
                        cmd.Parameters.AddWithValue("@docid", docid);
                        cmd.Parameters.AddWithValue("@app_id", appid);
                        cmd.Parameters.AddWithValue("@myid", myid);
                        cmd.Parameters.AddWithValue("@docname", docname);
                        cmd.Parameters.AddWithValue("@amend_year",Convert.ToInt32(DateTime.Now.Year));
                        cmd.Parameters.AddWithValue("@created_by", Session["created_by"].ToString());
                        cmd.Parameters.AddWithValue("@ipaddress", ipaddress);
                        cmd.Parameters.AddWithValue("@macaddress", macaddress);
                        cmd.Parameters.AddWithValue("@noofpages", pagescount);
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

        public static Tuple<string, string> getPageStatus(Int64 app_id, int myid)
        {
            string active = "False";
            string objectid = "";
            var tuple = new Tuple<string, string>(active, objectid);
            NpgsqlConnection conn = new NpgsqlConnection();
            NpgsqlCommand cmd = new NpgsqlCommand();
            conn.ConnectionString = ConfigurationManager.ConnectionStrings["connect"].ConnectionString;
            cmd.Connection = conn;
            try
            {
                conn.Open();
                string query = "SELECT active,object_id FROM esociety.docs_amendment where app_id=@appid and myid=@myid";
                cmd.CommandText = query;
                cmd.Parameters.AddWithValue("@appid", app_id);
                cmd.Parameters.AddWithValue("@myid", myid);
                NpgsqlDataReader rd = cmd.ExecuteReader();
                //var tuple = new Tuple<string, string>(active, objectid);
                if (rd.Read())
                {
                    active = rd["active"].ToString();
                    objectid = rd["object_id"].ToString();
                    tuple = new Tuple<string, string>(active, objectid);
                }
                else
                {
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
           
        }
        protected byte[] openConnectionMongo_amendmentDocs(string objectid)//society docuemnt
        {
            try
            {
                var str = ConfigurationManager.AppSettings["mongoconnect"];
                IMongoDatabase database;
                IMongoClient client;
                client = new MongoClient(str);
                database = client.GetDatabase("eGoaSociety");
                var collection = database.GetCollection<OtherDocuments>("Amendment Documents");
                var status = collection.Find(x => x._Id == ObjectId.Parse(objectid)).FirstOrDefault();
                byte[] decrypt_pdf_bytes = status.DocContent;
                byte[] pdf = obj_Byte_Encryption.DecryptData(decrypt_pdf_bytes, System.Web.HttpContext.Current.Server.MapPath("~/ENCRYPT.KEY"));
                convertToPdf(pdf);
                RecordUserAction("openConnectionMongo_amendmentDocs", "byte[] to function", "S");
                return pdf;
            }
            catch (MongoException ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "openConnectionMongo_amendmentDocs()" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
                RecordUserAction("openConnectionMongo_amendmentDocs", ex.Message, "F");
                Response.Write("<script language='javascript'>alert('" + "PDF View Failed" + ".No file uploaded yet')</script>");
                return null;
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
                string select = "SELECT app_id, docid, myid,docname,amend_year,created_at,created_by,ipaddress,macaddress FROM esociety.docs_amendment where object_id=@object_id";
                cmd.CommandText = select;
                cmd.Parameters.AddWithValue("@object_id", obj);
                NpgsqlDataReader rd = cmd.ExecuteReader();
                if (rd.Read())
                {
                    ViewState["object_id"] = Server.HtmlEncode(obj);
                    ViewState["app_id"] = Server.HtmlEncode(rd["app_id"].ToString());
                    ViewState["docid"] = Server.HtmlEncode(rd["docid"].ToString());
                    ViewState["myid"] = Server.HtmlEncode(rd["myid"].ToString());
                    ViewState["docname"] = Server.HtmlEncode(rd["docname"].ToString());
                    ViewState["amend_year"] = Server.HtmlEncode(rd["amend_year"].ToString());
                    ViewState["created_at"] = Server.HtmlEncode(rd["created_at"].ToString());
                    ViewState["created_by"] = Server.HtmlEncode(rd["created_by"].ToString());
                    ViewState["ipaddress"] = Server.HtmlEncode(rd["ipaddress"].ToString());
                    ViewState["macaddress"] = Server.HtmlEncode(rd["macaddress"].ToString());
                }
                rd.Close();
                cmd.Parameters.Clear();
                string hist_inr = "INSERT INTO esociety.history_docs_amendment(object_id, app_id, docid, active, myid, docname, amend_year,created_at,created_by,ipaddress,macaddress)";
                hist_inr = hist_inr + " VALUES (@object_id, @app_id, @docid, 'N', @myid, @docname, @amend_year,@created_at,@created_by,@ipaddress,@macaddress)";
                cmd.CommandText = hist_inr;
                cmd.Parameters.AddWithValue("@object_id", ViewState["object_id"].ToString());
                cmd.Parameters.AddWithValue("@app_id", Convert.ToInt64(ViewState["app_id"]));
                cmd.Parameters.AddWithValue("@docid", ViewState["docid"].ToString());
                cmd.Parameters.AddWithValue("@myid", Convert.ToInt16(ViewState["myid"]));
                cmd.Parameters.AddWithValue("@docname", ViewState["docname"].ToString());
                cmd.Parameters.AddWithValue("@amend_year",Convert.ToInt32( ViewState["amend_year"].ToString()));
                cmd.Parameters.AddWithValue("@created_at", Convert.ToDateTime(ViewState["created_at"].ToString()));
                cmd.Parameters.AddWithValue("@created_by", ViewState["created_by"].ToString());
                cmd.Parameters.AddWithValue("@ipaddress", ViewState["ipaddress"].ToString());
                cmd.Parameters.AddWithValue("@macaddress", ViewState["macaddress"].ToString());

                cmd.ExecuteNonQuery();
                cmd.Parameters.Clear();
                string delete = "delete from esociety.docs_amendment where object_id=@obj";
                cmd.CommandText = delete;
                cmd.Parameters.AddWithValue("@obj", ViewState["object_id"].ToString());
                cmd.ExecuteNonQuery();
                cmd.Parameters.Clear();
               
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

        protected void LoadSocietyDetails()
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

                string query = "select socname, socaddr,mst_district.\"DistrictName\" as socdist, socregid, regdate from esociety.society join esociety.mst_district on esociety.society.socdistrict = esociety.mst_district.\"DistrictID\" where app_id = @app_id";
                query = query + " ";

                cmd.CommandText = query;
                cmd.Parameters.AddWithValue("@app_id", Convert.ToInt64(Session["AppID"].ToString()));
                NpgsqlDataReader rd = cmd.ExecuteReader();
                if (rd.Read())
                {
                    lblsocname.Text = Server.HtmlEncode(rd["socname"].ToString());
                    //lblsocaddr.Text = Server.HtmlEncode(rd["socaddr"].ToString());
                    lblregdate.Text = Convert.ToDateTime(rd["regdate"], french).Date.ToString("dd/MM/yyyy");
                    lblsocdistrict.Text = " " + Server.HtmlEncode(rd["socdist"].ToString());
                    lblregid.Text = Server.HtmlEncode(rd["socregid"].ToString());

                }
                rd.Close();
                cmd.Parameters.Clear();

                myTrans.Commit();

            }
            catch (NpgsqlException ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "Page_Load" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
                myTrans.Rollback();
            }
            finally
            {
                conn.Close();
            }
        }
        protected void LB_OrgByelaws_Upload_Click(object sender, EventArgs e)
        {
            try
            {
                if (Session["AppID"] != null)
                {
                    int b = fileuploadfunction(FileUpload1, lbfu1status, doc, "Original Bye-Laws", ipaddress, macaddress, Convert.ToInt64(Session["AppID"].ToString()), "Amendment Documents", 1);
                    if (b == 1)
                    {
                        lbfu1status.ForeColor = System.Drawing.Color.Green;
                        lbfu1status.Text = "Uploaded";
                        RecordUserAction("LB_OrgByelaws_Upload_Click", "File Uploaded Successfully", "S");
                        LB_OrgByelaws_View.Enabled = true;
                        FileUpload1.Enabled = false;
                        LB_OrgByelaws_Upload.Enabled = false;
                        Session["au_flag1"] = 1;
                    }
                    else
                    {
                        RecordUserAction("LB_OrgByelaws_Upload_Click", "File Uploaded Failed", "F");
                        lbfu1status.ForeColor = System.Drawing.Color.Red;
                        lbfu1status.Text = "Failed";
                        Session["au_flag1"] = 0;
                    }
                }
                else
                {
                    RecordUserAction("LB_OrgByelaws_Upload_Click", "Session null", "F");
                }
            }
            catch (Exception ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "LB_OrgByelaws_Upload_Click" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
            }
        }

        protected void LB_OrgByelaws_Delete_Click(object sender, EventArgs e)
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
                        LB_OrgByelaws_View.Enabled = false;
                        LB_OrgByelaws_Upload.Enabled = true;
                        FileUpload1.Enabled = true;
                        Session["au_flag1"] = 0;
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
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "LB_OrgByelaws_Delete_Click" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
            }
        }

        protected void LB_OrgByelaws_View_Click(object sender, EventArgs e)
        {
            try
            {
                var status = getPageStatus(Convert.ToInt64(Session["AppID"].ToString()), 1);
                string objectid = status.Item2;
                openConnectionMongo_amendmentDocs(objectid);
                RecordUserAction("LB_OrgByelaws_View_Click", "User Clicked on LB_OrgByelaws_View_Click", "S");
            }
            catch (Exception ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "LB_OrgByelaws_View_Click" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
            }
        }

        protected void LB_StatementofChanges_Upload_Click(object sender, EventArgs e)
        {
            try
            {
                if (Session["AppID"] != null)
                {
                    int b = fileuploadfunction(FileUpload2, lbfu2status, doc, "Statement of Changes", ipaddress, macaddress, Convert.ToInt64(Session["AppID"].ToString()), "Amendment Documents", 2);
                    if (b == 1)
                    {
                        lbfu2status.ForeColor = System.Drawing.Color.Green;
                        lbfu2status.Text = "Uploaded";
                        RecordUserAction("LB_StatementofChanges_Upload_Click", "File Uploaded Successfully", "S");
                        LB_StatementofChanges_View.Enabled = true;
                        FileUpload2.Enabled = false;
                        LB_StatementofChanges_Upload.Enabled = false;
                        Session["au_flag2"] = 1;
                    }
                    else
                    {
                        RecordUserAction("LB_StatementofChanges_Upload_Click", "File Uploaded Failed", "F");
                        lbfu2status.ForeColor = System.Drawing.Color.Red;
                        lbfu2status.Text = "Failed";
                        Session["au_flag2"] = 0;
                    }
                }
                else
                {
                    RecordUserAction("LB_StatementofChanges_Upload_Click", "Session null", "F");
                }
            }
            catch (Exception ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "LB_StatementofChanges_Upload_Click" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
            }
        }

        protected void LB_StatementofChanges_Delete_Click(object sender, EventArgs e)
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
                        LB_StatementofChanges_View.Enabled = false;
                        LB_StatementofChanges_Upload.Enabled = true;
                        FileUpload2.Enabled = true;
                        Session["au_flag2"] = 0;
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
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "LB_StatementofChanges_Delete_Click" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
            }
        }

        protected void LB_StatementofChanges_View_Click(object sender, EventArgs e)
        {
            try
            {
                var status = getPageStatus(Convert.ToInt64(Session["AppID"].ToString()), 2);
                string objectid = status.Item2;
                openConnectionMongo_amendmentDocs(objectid);
                RecordUserAction("LB_StatementofChanges_View_Click", "User Clicked on LB_StatementofChanges_View_Click", "S");
            }
            catch (Exception ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "LB_StatementofChanges_View_Click" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
            }
        }

        protected void LB_AgmNotice_Upload_Click(object sender, EventArgs e)
        {
            try
            {
                if (Session["AppID"] != null)
                {
                    int b = fileuploadfunction(FileUpload3, lbfu3status, doc, "AGM Notice", ipaddress, macaddress, Convert.ToInt64(Session["AppID"].ToString()), "Amendment Documents", 3);
                    if (b == 1)
                    {
                        lbfu3status.ForeColor = System.Drawing.Color.Green;
                        lbfu3status.Text = "Uploaded";
                        RecordUserAction("LB_AgmNotice_Upload_Click", "File Uploaded Successfully", "S");
                        LB_AgmNotice_View.Enabled = true;
                        FileUpload3.Enabled = false;
                        LB_AgmNotice_Upload.Enabled = false;
                        Session["au_flag3"] = 1;
                    }
                    else
                    {
                        RecordUserAction("LB_AgmNotice_Upload_Click", "File Uploaded Failed", "F");
                        lbfu3status.ForeColor = System.Drawing.Color.Red;
                        lbfu3status.Text = "Failed";
                        Session["au_flag3"] = 0;
                    }
                }
                else
                {
                    RecordUserAction("LB_AgmNotice_Upload_Click", "Session null", "F");
                }
            }
            catch (Exception ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "LB_AgmNotice_Upload_Click" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
            }
        }

        protected void LB_AgmNotice_Delete_Click(object sender, EventArgs e)
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
                        LB_AgmNotice_View.Enabled = false;
                        LB_AgmNotice_Upload.Enabled = true;
                        FileUpload3.Enabled = true;
                        Session["au_flag3"] = 0;
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
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "LB_AgmNotice_Delete_Click" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
            }
        }

        protected void LB_AgmNotice_View_Click(object sender, EventArgs e)
        {
            try
            {
                var status = getPageStatus(Convert.ToInt64(Session["AppID"].ToString()), 3);
                string objectid = status.Item2;
                openConnectionMongo_amendmentDocs(objectid);
                RecordUserAction("LB_AgmNotice_View_Click", "User Clicked on LB_AgmNotice_View_Click", "S");
            }
            catch (Exception ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "LB_AgmNotice_View_Click" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
            }
        }

        protected void LB_AgmResolution_Upload_Click(object sender, EventArgs e)
        {
            try
            {
                if (Session["AppID"] != null)
                {
                    int b = fileuploadfunction(FileUpload4, lbfu4status, doc, "AGM Resolution", ipaddress, macaddress, Convert.ToInt64(Session["AppID"].ToString()), "Amendment Documents", 4);
                    if (b == 1)
                    {
                        lbfu4status.ForeColor = System.Drawing.Color.Green;
                        lbfu4status.Text = "Uploaded";
                        RecordUserAction("LB_AgmResolution_Upload_Click", "File Uploaded Successfully", "S");
                        LB_AgmResolution_View.Enabled = true;
                        FileUpload4.Enabled = false;
                        LB_AgmResolution_Upload.Enabled = false;
                        Session["au_flag4"] = 1;
                    }
                    else
                    {
                        RecordUserAction("LB_AgmResolution_Upload_Click", "File Uploaded Failed", "F");
                        lbfu4status.ForeColor = System.Drawing.Color.Red;
                        lbfu4status.Text = "Failed";
                        Session["au_flag4"] = 0;
                    }
                }
                else
                {
                    RecordUserAction("LB_AgmResolution_Upload_Click", "Session null", "F");
                }
            }
            catch (Exception ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "LB_AgmResolution_Upload_Click" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
            }
        }

        protected void LB_AgmResolution_Delete_Click(object sender, EventArgs e)
        {
            try
            {
                var status = getPageStatus(Convert.ToInt64(Session["AppID"].ToString()), 4);
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
                        lbfu4status.Text = "File Deleted";
                        Response.Write("<script language='javascript'>alert('Deleted Successfully')</script>");
                        LB_AgmResolution_View.Enabled = false;
                        LB_AgmResolution_Upload.Enabled = true;
                        FileUpload4.Enabled = true;
                        Session["au_flag4"] = 0;
                    }
                    else
                    {
                        lbfu4status.Text = "Failed...Kindly try again";
                        Response.Write("<script language='javascript'>alert('Failed...Kindly try again')</script>");
                    }
                }
            }
            catch (Exception ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "LB_AgmResolution_Delete_Click" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
            }
        }

        protected void LB_AgmResolution_View_Click(object sender, EventArgs e)
        {
            try
            {
                var status = getPageStatus(Convert.ToInt64(Session["AppID"].ToString()), 4);
                string objectid = status.Item2;
                openConnectionMongo_amendmentDocs(objectid);
                RecordUserAction("LB_AgmResolution_View_Click", "User Clicked on LB_AgmResolution_View_Click", "S");
            }
            catch (Exception ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "LB_AgmResolution_View_Click" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
            }
        }

        protected void Lb_AmendByelaws_Upload_Click(object sender, EventArgs e)
        {
            try
            {
                if (Session["AppID"] != null)
                {
                    int b = fileuploadfunction(FileUpload5, lbfu5status, doc, "Amendment Bye-Laws", ipaddress, macaddress, Convert.ToInt64(Session["AppID"].ToString()), "Amendment Documents", 5);
                    if (b == 1)
                    {
                        lbfu5status.ForeColor = System.Drawing.Color.Green;
                        lbfu5status.Text = "Uploaded";
                        RecordUserAction("Lb_AmendByelaws_Upload_Click", "File Uploaded Successfully", "S");
                        Lb_AmendByelaws_View.Enabled = true;
                        FileUpload5.Enabled = false;
                        Lb_AmendByelaws_Upload.Enabled = false;
                        Session["au_flag5"] = 1;
                    }
                    else
                    {
                        RecordUserAction("Lb_AmendByelaws_Upload_Click", "File Uploaded Failed", "F");
                        lbfu5status.ForeColor = System.Drawing.Color.Red;
                        lbfu5status.Text = "Failed";
                        Session["au_flag5"] = 0;
                    }
                }
                else
                {
                    RecordUserAction("Lb_AmendByelaws_Upload_Click", "Session null", "F");
                }
            }
            catch (Exception ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "Lb_AmendByelaws_Upload_Click" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
            }
        }

        protected void Lb_AmendByelaws_Delete_Click(object sender, EventArgs e)
        {
            try
            {
                var status = getPageStatus(Convert.ToInt64(Session["AppID"].ToString()), 5);
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
                        lbfu5status.Text = "File Deleted";
                        Response.Write("<script language='javascript'>alert('Deleted Successfully')</script>");
                        Lb_AmendByelaws_View.Enabled = false;
                        Lb_AmendByelaws_Upload.Enabled = true;
                        FileUpload5.Enabled = true;
                        Session["au_flag5"] = 0;
                    }
                    else
                    {
                        lbfu5status.Text = "Failed...Kindly try again";
                        Response.Write("<script language='javascript'>alert('Failed...Kindly try again')</script>");
                    }
                }
            }
            catch (Exception ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "Lb_AmendByelaws_Delete_Click" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
            }
        }

        protected void Lb_AmendByelaws_View_Click(object sender, EventArgs e)
        {
            try
            {
                var status = getPageStatus(Convert.ToInt64(Session["AppID"].ToString()), 5);
                string objectid = status.Item2;
                openConnectionMongo_amendmentDocs(objectid);
                RecordUserAction("Lb_AmendByelaws_View_Click", "User Clicked on Lb_AmendByelaws_View_Click", "S");
            }
            catch (Exception ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "Lb_AmendByelaws_View_Click" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
            }
        }      

        protected void LB_RevisedVersion_Upload_Click(object sender, EventArgs e)
        {
            try
            {
                if (Session["AppID"] != null)
                {
                    int b = fileuploadfunction(FileUpload6, lbfu6status, doc, "Revised Version", ipaddress, macaddress, Convert.ToInt64(Session["AppID"].ToString()), "Amendment Documents", 6);
                    if (b == 1)
                    {
                        lbfu6status.ForeColor = System.Drawing.Color.Green;
                        lbfu6status.Text = "Uploaded";
                        RecordUserAction("LB_RevisedVersion_Upload_Click", "File Uploaded Successfully", "S");
                        LB_RevisedVersion_View.Enabled = true;
                        FileUpload6.Enabled = false;
                        LB_RevisedVersion_Upload.Enabled = false;
                        Session["au_flag6"] = 1;
                    }
                    else
                    {
                        RecordUserAction("LB_RevisedVersion_Upload_Click", "File Uploaded Failed", "F");
                        lbfu6status.ForeColor = System.Drawing.Color.Red;
                        lbfu6status.Text = "Failed";
                        Session["au_flag6"] = 0;
                    }
                }
                else
                {
                    RecordUserAction("LB_RevisedVersion_Upload_Click", "Session null", "F");
                }
            }
            catch (Exception ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "LB_RevisedVersion_Upload_Click" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
            }
        }

        protected void LB_RevisedVersion_Delete_Click(object sender, EventArgs e)
        {
            try
            {
                var status = getPageStatus(Convert.ToInt64(Session["AppID"].ToString()), 6);
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
                        lbfu6status.Text = "File Deleted";
                        Response.Write("<script language='javascript'>alert('Deleted Successfully')</script>");
                        LB_RevisedVersion_View.Enabled = false;
                        LB_RevisedVersion_Upload.Enabled = true;
                        FileUpload6.Enabled = true;
                        Session["au_flag6"] = 0;
                    }
                    else
                    {
                        lbfu6status.Text = "Failed...Kindly try again";
                        Response.Write("<script language='javascript'>alert('Failed...Kindly try again')</script>");
                    }
                }
            }
            catch (Exception ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "LB_RevisedVersion_Delete_Click" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
            }
        }

        protected void LB_RevisedVersion_View_Click(object sender, EventArgs e)
        {
            try
            {
                var status = getPageStatus(Convert.ToInt64(Session["AppID"].ToString()), 6);
                string objectid = status.Item2;
                openConnectionMongo_amendmentDocs(objectid);
                RecordUserAction("LB_RevisedVersion_View_Click", "User Clicked on LB_RevisedVersion_View_Click", "S");
            }
            catch (Exception ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "LB_RevisedVersion_View_Click" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
            }
        }      

        protected void btnconfirmSubmit_Click(object sender, EventArgs e)
        {
            string message = "Once you have submitted your application, it is not possible to make changes to the application form.";
            Label14.Text = message;
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "none", "<script> $(window).on('load', function () { $('#finalmodal').modal({ backdrop: 'static' });});</script>", false);

        }

        protected void View_adddocs_Click(object sender, ImageClickEventArgs e)
        {

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

        protected void btnAddDocs_Click(object sender, EventArgs e)
        {
            try
            {
               // Validate _val = new Validate();
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
                    int a = fileuploadfunction(FileUpload9, lblError, doc, txtbx_DocName.Text, ipaddress, macaddress, Convert.ToInt64(Session["AppID"].ToString()), "Amendment Documents", 0);
                    if (a == 1)
                    {
                        lblError.ForeColor = System.Drawing.Color.Green;
                        lblError.Text = "Uploaded Successfully";
                        RecordUserAction("btnAddDocs_Click", "File Uploaded Successfully", "S");
                        tr_addamenddocs.Visible = true;
                        bindgridview_additonaldocs(Session["AppID"].ToString());
                        Response.Redirect("Amendment.aspx");
                    }
                    else
                    {
                        RecordUserAction("btnAddDocs_Click", "File Uploaded Failed", "F");
                        lblError.ForeColor = System.Drawing.Color.Red;

                        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "none", "<script> $(window).on('load', function () { $('#btnAddDocs_Click').modal({ backdrop: 'static' });});</script>", false);
                    }
                }
            }
            catch (Exception ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "btnAddDocs_Click" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath) + " from " + ipaddress);
            }
        }

        public void bindgridview_additonaldocs(string AppID)
        {            

            NpgsqlConnection conn = new NpgsqlConnection();
            NpgsqlCommand cmd = new NpgsqlCommand();
            conn.ConnectionString = ConfigurationManager.ConnectionStrings["connect"].ConnectionString;
            cmd.Connection = conn;
            try
            {
                conn.Open();
                string query = "SELECT object_id,docname from esociety.otherdoc_amendment where app_id=@appid";
                cmd.CommandText = query;
                cmd.Parameters.AddWithValue("@appid", Convert.ToInt64(AppID));

                NpgsqlDataAdapter da = new NpgsqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                da.Fill(ds);
                if (ds.Tables[0].Rows.Count > 0)
                {
                        tr_addamenddocs.Visible = true;
                        GridView_AddAmendDocs.DataSource = ds;
                        GridView_AddAmendDocs.DataBind();
                }
                else
                {
                    tr_addamenddocs.Visible = false;
                    
                }
                da.Dispose();
            }
            catch (NpgsqlException ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "bindgridview_additonaldocs()" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath) + " from " + ipaddress);
                RecordUserAction("bindgridview_additonaldocs", ex.Message, "F");
                Response.Write("<script language='javascript'>alert('" + "Exception in Gridview" + "')</script>");
            }
            finally
            {
                conn.Close();
            }
        }

        protected void GridView_AddAmendDocs_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (Convert.ToInt32(Session["status_amend"]) == 1 || Convert.ToInt32(Session["status_amend"]) == 3)
            { }
            else
            {
                ((DataControlField)GridView_AddAmendDocs.Columns
            .Cast<DataControlField>()
            .Where(fld => fld.HeaderText == "Delete")
            .SingleOrDefault()).Visible = false;

            }
        }

        protected int DeleteFunction_AdditionalDocs(string obj)
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
                string select = "SELECT app_id, other_docid, docname,amend_year,created_at,created_by,ipaddress,macaddress FROM esociety.otherdoc_amendment where object_id=@object_id";
                cmd.CommandText = select;
                cmd.Parameters.AddWithValue("@object_id", obj);
                NpgsqlDataReader rd = cmd.ExecuteReader();
                if (rd.Read())
                {
                    ViewState["object_id"] = Server.HtmlEncode(obj);
                    ViewState["app_id"] = Server.HtmlEncode(rd["app_id"].ToString());
                    ViewState["other_docid"] = Server.HtmlEncode(rd["other_docid"].ToString());                   
                    ViewState["docname"] = Server.HtmlEncode(rd["docname"].ToString());
                    ViewState["amend_year"] = Server.HtmlEncode(rd["amend_year"].ToString());
                    ViewState["created_at"] = Server.HtmlEncode(rd["created_at"].ToString());
                    ViewState["created_by"] = Server.HtmlEncode(rd["created_by"].ToString());
                    ViewState["ipaddress"] = Server.HtmlEncode(rd["ipaddress"].ToString());
                    ViewState["macaddress"] = Server.HtmlEncode(rd["macaddress"].ToString());
                }
                rd.Close();
                cmd.Parameters.Clear();
                string hist_inr = "INSERT INTO esociety.history_otherdoc_amendment(object_id, app_id, other_docid, docname, amend_year,created_at,created_by,ipaddress,macaddress, active)";
                hist_inr = hist_inr + " VALUES (@object_id, @app_id, @other_docid, @docname, @amend_year,@created_at,@created_by,@ipaddress,@macaddress, 'N')";
                cmd.CommandText = hist_inr;
                cmd.Parameters.AddWithValue("@object_id", ViewState["object_id"].ToString());
                cmd.Parameters.AddWithValue("@app_id", Convert.ToInt64(ViewState["app_id"]));
                cmd.Parameters.AddWithValue("@other_docid", ViewState["other_docid"].ToString());               
                cmd.Parameters.AddWithValue("@docname", ViewState["docname"].ToString());
                cmd.Parameters.AddWithValue("@amend_year", Convert.ToInt32(ViewState["amend_year"].ToString()));
                cmd.Parameters.AddWithValue("@created_at", Convert.ToDateTime(ViewState["created_at"].ToString()));
                cmd.Parameters.AddWithValue("@created_by", ViewState["created_by"].ToString());
                cmd.Parameters.AddWithValue("@ipaddress", ViewState["ipaddress"].ToString());
                cmd.Parameters.AddWithValue("@macaddress", ViewState["macaddress"].ToString());

                cmd.ExecuteNonQuery();
                cmd.Parameters.Clear();
                string delete = "delete from esociety.otherdoc_amendment where object_id=@obj";
                cmd.CommandText = delete;
                cmd.Parameters.AddWithValue("@obj", ViewState["object_id"].ToString());
                cmd.ExecuteNonQuery();
                cmd.Parameters.Clear();

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

        protected void LBDelete_AddAmenddocs_Click(object sender, EventArgs e)
        {
            LinkButton btn = (LinkButton)sender;
            GridViewRow row = (GridViewRow)btn.NamingContainer;
            HiddenField hdobid = row.FindControl("hfobjectID") as HiddenField;

            if (hdobid != null)
            {
                try
                {
                    string objectid = hdobid.Value.ToString();
                    int a = DeleteFunction_AdditionalDocs(objectid);
                    if (a == 1)
                    {
                        Response.Write("<script language='javascript'>alert('Deleted Successfully')</script>");
                        bindgridview_additonaldocs(Session["AppID"].ToString());
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

        protected void LbAmendDocsView_Click(object sender, EventArgs e)
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
                    var collection = database.GetCollection<OtherDocuments>("Amendment Documents");
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

        protected void btnSubmitfinal_Click(object sender, EventArgs e)
        {
            if (Session["AppID"] != null)
            {
                int amendstatus = 0;
                Insert ins = new Insert();
                amendstatus = ins.getOtherServicesStatus(Session["AppID"].ToString());

                if (amendstatus == 0)
                {
                    Label15.Text = "This Society is not yet Registered.";
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "none", "<script> $(window).on('load', function () { $('#errormodal').modal({ backdrop: 'static' });});</script>", false);

                }
                else
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
                        string query = "select regdate from esociety.society where app_id=@app_id";
                        cmd.CommandText = query;
                        cmd.Parameters.AddWithValue("@app_id", Convert.ToInt64(Session["AppID"].ToString()));
                        NpgsqlDataReader rd = cmd.ExecuteReader();
                        if (rd.Read())
                        {
                            Session["regdate"] = Server.HtmlEncode(rd["regdate"].ToString());
                        }
                        rd.Close();
                        cmd.Parameters.Clear();
                        myTrans.Commit();
                    }
                    catch (NpgsqlException ex)
                    {
                        CreateLogFiles Err = new CreateLogFiles();
                        Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "btnSubmitfinal_Click" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
                        myTrans.Rollback();
                        RecordUserAction("btnSubmitfinal_Click", ex.Message, "F");
                        Response.Write("<script language='javascript'>alert('" + "Error while deletion" + "')</script>");
                    }
                    finally
                    {
                        conn.Close();
                    }

                    if (Session["regdate"].ToString() == "" || Session["regdate"].ToString() == null)
                    {
                        Response.Write("<script language='javascript'>alert('Society Registration is Incomplete')</script>");
                    }
                    else
                    {
                        if (Convert.ToInt32(Session["au_flag1"].ToString()) != 1)
                        {
                            Label15.Text = "You have not Uploaded Original Bye-Laws";
                            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "none", "<script> $(window).on('load', function () { $('#errormodal').modal({ backdrop: 'static' });});</script>", false);
                        }
                        else if (Convert.ToInt32(Session["au_flag2"].ToString()) != 1)
                        {
                            Label15.Text = "You have not Uploaded Statement of Changes";
                            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "none", "<script> $(window).on('load', function () { $('#errormodal').modal({ backdrop: 'static' });});</script>", false);
                        }
                        else if (Convert.ToInt32(Session["au_flag3"].ToString()) != 1)
                        //else if (!lbfu3status.Text.Equals("File Uploaded") && !lbfu3status.Text.Equals("Uploaded"))
                        {
                            Label15.Text = "You have not Uploaded AGM Notice";
                            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "none", "<script> $(window).on('load', function () { $('#errormodal').modal({ backdrop: 'static' });});</script>", false);
                        }
                        else if (Convert.ToInt32(Session["au_flag4"].ToString()) != 1)
                        {
                            Label15.Text = "You have not Uploaded Support of AGM Resolution";
                            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "none", "<script> $(window).on('load', function () { $('#errormodal').modal({ backdrop: 'static' });});</script>", false);
                        }
                        else if (Convert.ToInt32(Session["au_flag5"].ToString()) != 1)
                        {
                            Label15.Text = "You have not Uploaded Amendment Bye-Laws";
                            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "none", "<script> $(window).on('load', function () { $('#errormodal').modal({ backdrop: 'static' });});</script>", false);
                        }
                        else if (Convert.ToInt32(Session["au_flag6"].ToString()) != 1)
                        {
                            Label15.Text = "You have not Uploaded Revised Version";
                            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "none", "<script> $(window).on('load', function () { $('#errormodal').modal({ backdrop: 'static' });});</script>", false);
                        }
                        else
                        {
                            try
                            {
                                conn.Open();
                                string query = "";

                                if (amendstatus == 1)
                                {
                                    query = "Update esociety.status_amendment set created_by=@created_by,ipaddress=@ipaddress,amend_status=2,amend_submittime=current_timestamp where app_id=@app_id";
                                }
                                else if (amendstatus == 3)
                                {
                                    query = "Update esociety.status_amendment set created_by=@created_by,ipaddress=@ipaddress,amend_status=4,amend_obssubmittime=current_timestamp where app_id=@app_id";
                                }


                                cmd.CommandText = query;
                                cmd.Parameters.AddWithValue("@app_id", Convert.ToInt64(Session["AppID"].ToString()));
                                cmd.Parameters.AddWithValue("@ipaddress", ipaddress);
                                cmd.Parameters.AddWithValue("@created_by", Session["created_by"].ToString());

                                cmd.ExecuteNonQuery();
                                RecordUserAction("btnSubmitfinal_Click", "Data saved in Amendment status table", "S");
                            }
                            catch (NpgsqlException ex)
                            {
                                CreateLogFiles Err = new CreateLogFiles();
                                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "btnSubmitfinal_Click" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
                                RecordUserAction("btnSubmitfinal_Click", ex.Message, "F");
                                Response.Write("<script language='javascript'>alert('" + "DB Failed Error" + "')</script>");
                            }
                            finally
                            {
                                conn.Close();
                            }
                            string message = " Your Application has been submitted for Amendment. Please wait for our approval." + Convert.ToInt64(Session["AppID"].ToString());
                            Status.Text = message;
                            //  SendSMS sms = new SendSMS();
                            //  string mobile = getmobileno(Session["AppID"].ToString());
                            // string b = Session["AppID"].ToString();
                            //  string c = Session["login_id"].ToString();
                            // sms.send_otp_sms_submit(mobile, Session["AppID"].ToString(), Session["login_id"].ToString());
                            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "none", "<script> $(window).on('load', function () { $('#myModal1').modal({ backdrop: 'static' });});</script>", false);

                        }


                    }
                } 
            }
            else
            {
                RecordUserAction("btnSubmitfinal_Click", "Session null", "F");
            }
        }

        protected int checkIfSocietyIsActive(string appid)
        {
            DateTime registereddate = DateTime.Now.Date;
            DateTime todaydate = DateTime.Today.Date;
            TimeSpan diff;
            int yearsdiff = 0;

            int active = 0;


            NpgsqlConnection conn = new NpgsqlConnection();
            NpgsqlCommand cmd = new NpgsqlCommand();
            conn.ConnectionString = ConfigurationManager.ConnectionStrings["connect"].ConnectionString;
            cmd.Connection = conn;
            try
            {
                conn.Open();
                string query = "SELECT socregid,regdate FROM esociety.society where app_id=@app_id";
                cmd.CommandText = query;
                cmd.Parameters.AddWithValue("@app_id", Convert.ToInt64(appid));
                NpgsqlDataReader rd = cmd.ExecuteReader();
                if (rd.Read())
                {
                    if (!(string.IsNullOrEmpty(rd["regdate"].ToString())))
                    {

                        registereddate = Convert.ToDateTime(rd["regdate"], french).Date;

                        diff = todaydate - registereddate;
                        yearsdiff = (int)(diff.Days / (365.25));
                        if (yearsdiff >= 5)
                        {
                            active = 0;

                        }
                        else
                        {

                            active = 1;
                        }
                        
                    }
                    else
                    {
                        active = 0;
                        // lblRegdetailsError.Text = "Registration Dat not found";
                    }


                }

                rd.Close();

            }
            catch(NpgsqlException ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(HttpContext.Current.Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "checkIfSocietyIsActive()" + " Amendment.aspx");
            }
            finally
            {
                conn.Close();
            }
            return active;
        }

        protected void get_status()
        {                 

            try
            {             
                NpgsqlConnection conn = new NpgsqlConnection();
                NpgsqlCommand cmd = new NpgsqlCommand();
                conn.ConnectionString = ConfigurationManager.ConnectionStrings["connect"].ConnectionString;
                cmd.Connection = conn;
                try
                {
                    conn.Open();
                    string query = "SELECT amend_status,obsremarks_bydro from esociety.status_amendment where app_id=@appid";
                    cmd.CommandText = query;
                    cmd.Parameters.AddWithValue("@appid", Convert.ToInt64(Session["AppID"].ToString()));                   
                    NpgsqlDataReader rd = cmd.ExecuteReader();                    
                    if (rd.Read())
                    {
                        Session["status_amend"] = Convert.ToInt32(rd["amend_status"]);
                        ViewState["obsremarks_bydro"] = Server.HtmlEncode(rd["obsremarks_bydro"].ToString());

                    }
                    rd.Close();
                }
                catch (NpgsqlException ex)
                {
                    CreateLogFiles Err = new CreateLogFiles();
                    Err.ErrorLog(HttpContext.Current.Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "()" + " Amendment.aspx");
                    
                }
                finally
                {
                    conn.Close();
                }

                if (Convert.ToInt32(Session["status_amend"]) == 1)
                {
                    int isactive = checkIfSocietyIsActive(Session["AppID"].ToString());
                    if(isactive == 0)
                    {
                        btnSubmit.Enabled = false;
                        tradddocsbtn.Visible = false;
                        lblimpnotice.Text = Sanitize.InputText("This Society is Inactive as it has completed 5 years");
                    }
                }
                else if(Convert.ToInt32(Session["status_amend"]) == 3)
                {
                    //pleasedonec_changes.Visible = true;
                    observation_remarks.Visible = true;
                    observation_remarks.Text = Sanitize.InputText(ViewState["obsremarks_bydro"].ToString());
                    lblimpnotice.Text = Sanitize.InputText("Please do the necessary changes suggested by Officer and Submit your application again.");
                }
                else
                {
                    FileUpload1.Enabled = false;
                    FileUpload2.Enabled = false;
                    FileUpload3.Enabled = false;
                    FileUpload4.Enabled = false;
                    FileUpload5.Enabled = false;
                    FileUpload6.Enabled = false;
                    LB_OrgByelaws_Upload.Visible = false;
                    LB_OrgByelaws_Delete.Visible = false;
                    LB_StatementofChanges_Upload.Visible = false;
                    LB_StatementofChanges_Delete.Visible = false;
                    LB_AgmNotice_Upload.Visible = false;
                    LB_AgmNotice_Delete.Visible = false;                    
                    LB_AgmResolution_Upload.Visible = false;
                    LB_AgmResolution_Delete.Visible = false;
                    Lb_AmendByelaws_Upload.Visible = false;
                    Lb_AmendByelaws_Delete.Visible = false;
                    LB_RevisedVersion_Upload.Visible = false;
                    LB_RevisedVersion_Delete.Visible = false;
                    btnSubmit.Enabled = false;
                    tradddocsbtn.Visible = false;


                    if (Convert.ToInt32(Session["status_amend"]) == 0)
                    {
                        lblimpnotice.Text = Sanitize.InputText("This Society is not Registered Yet");
                        //lblnotice2.Visible = true;
                        // lblnotice2.Text = "";
                    }
                    else if (Convert.ToInt32(Session["status_amend"]) == 2 || Convert.ToInt32(Session["status_amend"]) == 4)
                    {
                        lblimpnotice.Text = Sanitize.InputText("Your Application for Amendment has been submitted to District Registrar.");
                        //lblnotice2.Visible = true;
                       // lblnotice2.Text = "";
                    }
                    else if (Convert.ToInt32(Session["status_amend"]) == 5)
                    {
                        lblimpnotice.Text = Sanitize.InputText("Your Application for Amendment is ACCEPTED");
                        lblimpnotice.ForeColor = System.Drawing.Color.Green;
                    }

                }
            }
            catch (Exception ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "get_status()" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
            }
        }

        protected void gv_exisitingmangcomm_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType.Equals(DataControlRowType.DataRow))
                {
                    string x = e.Row.Cells[10].Text;
                    e.Row.Cells[10].Text = _encryption.Decrypt(x, System.Web.HttpContext.Current.Server.MapPath("~/ENCRYPT.KEY"));
                    e.Row.Cells[10].Text = Server.HtmlEncode(Utility.MaskMobile(e.Row.Cells[9].Text, 0, "XXXXXX"));
                                       

                }

                if (Convert.ToInt32(Session["status_amend"]) == 1 || Convert.ToInt32(Session["status_amend"]) == 4)
                {

                }
                else
                {
                    ((DataControlField)gv_newmangcomm.Columns
                .Cast<DataControlField>()
                .Where(fld => fld.HeaderText == "Select")
                .SingleOrDefault()).Visible = false;                   
                }

            }

            catch (Exception ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "gv_exisitingmangcomm_RowDataBound" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
            }
        }

        protected void LKAddMembertoNew_Click(object sender, EventArgs e)
        {
            Member_Details member_new = new Member_Details();

            foreach (GridViewRow row in gv_exisitingmangcomm.Rows)
            {
                if (row.RowType == DataControlRowType.DataRow)
                {
                    CheckBox chkRow = (row.FindControl("chkbx") as CheckBox);
                    
                    if (chkRow.Checked)
                    {
                        member_new.app_id = ((Label)row.FindControl("lbAppid")).Text.ToString();
                        member_new.member_id =Convert.ToInt64(((Label)row.FindControl("lbmemberid")).Text.ToString());
                        member_new.fname = row.Cells[3].Text.ToString();
                        member_new.designtaion = row.Cells[6].Text.ToString();
                        member_new.design =Convert.ToInt32( ((Label)row.FindControl("lbdesign")).Text.ToString());
                        member_new.designtaion_others = ((Label)row.FindControl("lbdesigntaion_others")).Text.ToString();
                        member_new.occupatid = Convert.ToInt32(((Label)row.FindControl("lboccupatid")).Text.ToString());
                        member_new.occupation = row.Cells[7].Text.ToString();
                        member_new.occupation_others = ((Label)row.FindControl("lboccupation_others")).Text.ToString();
                        member_new.address = row.Cells[8].Text.ToString();
                        member_new.proofname = row.Cells[9].Text.ToString();
                        member_new.proofid = Convert.ToInt32(((Label)row.FindControl("lbproofid")).Text.ToString());
                        member_new.mangcomm = ((Label)row.FindControl("lbmangcomm")).Text.ToString();
                        member_new.active = ((Label)row.FindControl("lbactive")).Text.ToString();
                        member_new.document_mongoentry = ((Label)row.FindControl("lbdocument_mongoentry")).Text.ToString();
                        member_new.doc_id = ((Label)row.FindControl("lbdocid")).Text.ToString();
                        member_new.proof_document_no = row.Cells[10].Text.ToString();
                        member_new.salutation_id = Convert.ToInt32(((Label)row.FindControl("lbsalutationid")).Text.ToString());
                        member_new.salutation = row.Cells[2].Text.ToString();
                        member_new.gender = row.Cells[4].Text.ToString();
                        member_new.age = Convert.ToInt32(row.Cells[5].Text.Trim());
                        member_new.remarks = row.Cells[12].Text.ToString();
                        member_new.dateofadmission = row.Cells[11].Text.ToString();
                       // member_new.created_at = DateTime.Now.ToString();
                        member_new.created_by = Session["created_by"].ToString();
                        member_new.ipaddress = ipaddress;
                        member_new.macaddress = macaddress;


                        insertmemberintotemp(member_new);
                    }
                }
            }

            bindgridview_newmangcomm(Session["AppID"].ToString());
        }

        protected void gv_newmangcomm_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType.Equals(DataControlRowType.DataRow))
                {
                    string x = e.Row.Cells[9].Text;
                    e.Row.Cells[9].Text = _encryption.Decrypt(x, System.Web.HttpContext.Current.Server.MapPath("~/ENCRYPT.KEY"));
                    e.Row.Cells[9].Text = Server.HtmlEncode(Utility.MaskMobile(e.Row.Cells[9].Text, 0, "XXXXXX"));

                    HiddenField mid = e.Row.FindControl("hfmemID") as HiddenField;

                    if (mid != null)
                    {
                        foreach (GridViewRow row in gv_exisitingmangcomm.Rows)
                        {
                            if (row.RowType == DataControlRowType.DataRow)
                            {
                                CheckBox chkRow = (row.FindControl("chkbx") as CheckBox);
                                string existmid = ((Label)row.FindControl("lbmemberid")).Text.ToString();

                                if(mid.Value == existmid)
                                {
                                    chkRow.Enabled = false;
                                }
                            }
                        }
                    }


                }

                if (Convert.ToInt32(Session["status_amend"]) == 1 || Convert.ToInt32(Session["status_amend"]) == 4)
                {

                }
                else
                {
                    ((DataControlField)gv_newmangcomm.Columns
                .Cast<DataControlField>()
                .Where(fld => fld.HeaderText == "Delete")
                .SingleOrDefault()).Visible = false;
                    ((DataControlField)gv_newmangcomm.Columns
                   .Cast<DataControlField>()
                   .Where(fld => fld.HeaderText == "Edit")
                   .SingleOrDefault()).Visible = false;
                }

            }

            catch (Exception ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "gv_newmangcomm_RowDataBound" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
            }

        }


        protected void LBEdit_Click(object sender, EventArgs e)
        {
         
            
            ViewState["edit_flag"] = 1;
            btnAdd.Visible = false;           
            btnUpdate.Enabled = true;

            string status = "";
            if (Session["AppID"] != null)
            {
                
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "none", "<script> $(window).on('load', function () { $('#addmembersmodal').modal({ backdrop: 'static' });});</script>", false);

                LinkButton btn = (LinkButton)sender;
                GridViewRow row = (GridViewRow)btn.NamingContainer;
                //which value you are using
                HiddenField hdmid = row.FindControl("hfmemID") as HiddenField;
                HiddenField hdobid = row.FindControl("hfobjectID") as HiddenField;
                HiddenField hddesign = row.FindControl("hfdesignid") as HiddenField;
               // int headdesignid = getheadentrydesignation();

                //if (hd.Value != null && hdobid.Value != null)
                if (hdmid.Value != null)
                {
                    string object_ID = hdobid.Value;
                    Int64 memberID = Convert.ToInt64(hdmid.Value);
                    //document_mongoentry
                    setData(memberID);
                    if (hdobid.Value != null && hdobid.Value != "")
                    {
                        status = openConnectionMongo(object_ID);
                    }


                    try
                    {
                        Member_Details member_edit = new Member_Details();
                        member_edit = FetchTempMember(Session["AppID"].ToString(), memberID);

                        txt_MemName.Text = member_edit.fname;
                        ddl_DesignMem.SelectedValue = member_edit.design.ToString();
                        TxtBx_DesignOthers.Text = member_edit.designtaion_others;
                        if (ddl_DesignMem.SelectedValue == "8")
                        {
                            row_DesignOthers.Visible = true;
                        }
                        else
                        {
                            row_DesignOthers.Visible = false;
                        }
                        ddl_MemOccup.SelectedValue = member_edit.occupatid.ToString();
                        TxtBx_OccupOthers.Text = member_edit.occupation_others;
                        if (ddl_MemOccup.SelectedValue == "4")
                        {
                            row_OccupOthers.Visible = true;
                        }
                        else
                        {
                            row_OccupOthers.Visible = false;
                        }
                        txt_MemAddress.Text = member_edit.address;
                        ddl_MemDocType.SelectedValue = member_edit.proofid.ToString();
                        if (member_edit.salutation_id.ToString() == "" || member_edit.salutation_id.ToString() == null)
                        {
                            ddl_salutation.SelectedValue = "1";
                        }
                        else
                        {
                            ddl_salutation.SelectedValue = member_edit.salutation_id.ToString();
                        }

                        if (member_edit.gender.ToString() == "" || member_edit.gender.ToString() == null)
                        {
                            Rdbtngender.SelectedValue = Server.HtmlEncode("M");
                        }
                        else
                        {
                            Rdbtngender.SelectedValue = member_edit.gender;
                        }
                        if (member_edit.age.ToString() == "" || (member_edit.age.ToString() == null))
                        {
                            txtbx_age.Text = "0";
                        }
                        else
                        {
                            txtbx_age.Text = member_edit.age.ToString();
                        }
                        string mangcomm = member_edit.mangcomm.Trim();
                        if (mangcomm == "Yes")
                        {
                            ddlMcom.SelectedValue = "1";
                        }
                        else if (mangcomm == "No")
                        {
                            ddlMcom.SelectedValue = "2";
                        }
                        // ddlMcom.SelectedItem.Text = member_edit.mangcomm;
                        if (Convert.ToInt32(ddl_MemDocType.SelectedValue) == 6)
                        {
                            Label2.Text = "Other Document" + " No";
                        }
                        else
                        {
                            string value = ddl_MemDocType.SelectedItem.Text;
                            Label2.Text = value + " No";
                        }


                        string decrypt_docno = member_edit.proof_document_no;
                        TxtBxDocumentNo.Text = _encryption.Decrypt(decrypt_docno, System.Web.HttpContext.Current.Server.MapPath("~/ENCRYPT.KEY"));
                        TxtBxRemarks.Text = member_edit.remarks;

                        if (status == "True")
                        {
                            lbuploadmember.Text = "File is Present.Old File will be replaced with new File";
                        }
                        else
                        {
                            lbuploadmember.Text = "";
                        }

                        if (member_edit.dateofadmission != null && member_edit.dateofadmission != "")
                        {
                            txtbxdateadmission.Text = Server.HtmlEncode(Convert.ToDateTime(member_edit.dateofadmission).ToString("yyyy-MM-dd"));
                        }
                        btnUpdate.Visible = true;
                        trvisible.Visible = true;


                        RecordUserAction("LBUpdate_Click", "Gridview Data to textbox", "S");
                        bindgridview_newmangcomm(Session["AppID"].ToString());

                        
                        ddl_salutation.Enabled = true;
                        txt_MemName.Enabled = true;
                        Rdbtngender.Enabled = true;
                        ddlMcom.Enabled = true;
                       
                    }
                    catch (Exception ex)
                    {
                        CreateLogFiles Err = new CreateLogFiles();
                        Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "LBUpdate_Click" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
                        RecordUserAction("LBUpdate_Click", ex.Message, "F");
                        Response.Write("<script language='javascript'>alert('" + "Gridview Data to textbox Exception" + "')</script>");
                    }

                }
                else
                {
                    Response.Write("<script language='javascript'>alert('" + "Not Updated" + "')</script>");
                }
                ddl_DesignMem.Enabled = false;

                //ddlMcom.Enabled = true;
            }
            else
            {
                RecordUserAction("LBUpdate_Click", "Session null", "F");
            }
        }

        protected void LBDelete_Click(object sender, EventArgs e)
        {

        }

        protected void LbView_existing_Click(object sender, EventArgs e)
        {

        }

        protected void LbView_new_Click(object sender, EventArgs e)
        {

        }

        public void bindgridview_existingmangcomm(string AppID)
        {

            NpgsqlConnection conn = new NpgsqlConnection();
            NpgsqlCommand cmd = new NpgsqlCommand();
            conn.ConnectionString = ConfigurationManager.ConnectionStrings["connect"].ConnectionString;
            cmd.Connection = conn;
            try
            {
                conn.Open();

                string query1 = "SELECT member_id, app_id, fname, designtaion, design, occupation, occupatid, address, proofname, proofid, mangcomm, active, document_mongoentry, doc_id, proof_document_no,";
                query1 = query1 + " salutation_id, salutation, gender, age, designtaion_others, occupation_others, remarks, dateofadmission FROM esociety.members where app_id=@appid and active='Y' and mangcomm='Yes' order by member_id ASC";
                cmd.CommandText = query1;
                cmd.Parameters.AddWithValue("@appid", Convert.ToInt64(AppID));
                NpgsqlDataAdapter da1 = new NpgsqlDataAdapter(cmd);
                DataSet ds1 = new DataSet();
                da1.Fill(ds1);
                if (ds1.Tables[0].Rows.Count > 0)
                {
                    gv_exisitingmangcomm.DataSource = ds1;
                    gv_exisitingmangcomm.DataBind();
                    // txttotmangcomm.Text = Server.HtmlEncode(ds1.Tables[0].Rows.Count.ToString());

                    RecordUserAction("ManagingCommittee_Gridview", "GridView Data Loaded", "S");
                }

            }
            catch (NpgsqlException ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "bindgridview_existingmangcomm()" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
                RecordUserAction("bindgridview", ex.Message, "F");
                Response.Write("<script language='javascript'>alert('" + "Exception in Gridview" + "')</script>");
            }
            finally
            {
                conn.Close();
            }
        }

        public void bindgridview_newmangcomm(string AppID)
        {
            NpgsqlConnection conn = new NpgsqlConnection();
            NpgsqlCommand cmd = new NpgsqlCommand();
            conn.ConnectionString = ConfigurationManager.ConnectionStrings["connect"].ConnectionString;
            cmd.Connection = conn;
            try
            {
                conn.Open();               

                string query1 = "SELECT app_id,salutation,fname,gender,age,designtaion,design,occupation,occupatid,address,proofname,proofid,mangcomm,member_id,proof_document_no,document_mongoentry,remarks, dateofadmission FROM esociety.temp_amendment_committee where app_id=@appid and active='Y' and mangcomm='Yes' order by member_id ASC";
                cmd.CommandText = query1;
                cmd.Parameters.AddWithValue("@appid", Convert.ToInt64(AppID));
                NpgsqlDataAdapter da1 = new NpgsqlDataAdapter(cmd);
                DataSet ds1 = new DataSet();
                da1.Fill(ds1);               
                gv_newmangcomm.DataSource = ds1;
                gv_newmangcomm.DataBind();
                // txttotmangcomm.Text = Server.HtmlEncode(ds1.Tables[0].Rows.Count.ToString());

                RecordUserAction("ManagingCommittee_Gridview", "GridView Data Loaded", "S");
                

            }
            catch (NpgsqlException ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "bindgridview_newmangcomm()" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
                RecordUserAction("bindgridview", ex.Message, "F");
                Response.Write("<script language='javascript'>alert('" + "Exception in Gridview" + "')</script>");
            }
            finally
            {
                conn.Close();
            }
           // txt_MemName.Enabled = true;
           // txt_MemAddress.Enabled = true;
           // ddl_DesignMem.Enabled = true;
        }


        protected int insertmemberintotemp(Member_Details Member)
        {
            int value = 0;
            NpgsqlConnection connect = new NpgsqlConnection();
            NpgsqlCommand cmd = new NpgsqlCommand();
            connect.ConnectionString = ConfigurationManager.ConnectionStrings["connect"].ConnectionString;
            cmd.Connection = connect;
            try
            {
                connect.Open();
                string query = "INSERT INTO esociety.temp_amendment_committee(member_id,app_id,fname,designtaion,design,occupation,occupatid,address,proofname,proofid,mangcomm,created_at,created_by,ipaddress,macaddress,";
                query = query + " active,document_mongoentry,doc_id,proof_document_no,salutation_id,salutation,gender,age,designtaion_others,occupation_others,remarks,dateofadmission)VALUES(@mid,@app_id,@fname,@designtaion,@design,@occupation,@occupatid,@address,";
                query = query + " @proofname,@proofid,@mangcomm,current_timestamp,@created_by,@ipaddress,@macaddress,'Y',@document_mongoentry,@doc_id,@proof_document_no,@salutation_id,@salutation,@gender,@age,@designtaion_others,@occupation_others,@remarks,@dateofadmission)";

                cmd.CommandText = query;
                cmd.Parameters.AddWithValue("@mid", Member.member_id);
                cmd.Parameters.AddWithValue("@app_id", Convert.ToInt64(Member.app_id));
                cmd.Parameters.AddWithValue("@fname",Member.fname);
                cmd.Parameters.AddWithValue("@designtaion", Member.designtaion);
                cmd.Parameters.AddWithValue("@design", Member.design);
                cmd.Parameters.AddWithValue("@occupation", Member.occupation);
                cmd.Parameters.AddWithValue("@occupatid", Member.occupatid);
                cmd.Parameters.AddWithValue("@address", Member.address);
                cmd.Parameters.AddWithValue("@proofname", Member.proofname);
                cmd.Parameters.AddWithValue("@proofid", Member.proofid);
                cmd.Parameters.AddWithValue("@mangcomm", Member.mangcomm);
                cmd.Parameters.AddWithValue("@created_by",Member.created_by);
                cmd.Parameters.AddWithValue("@ipaddress", Member.ipaddress);
                cmd.Parameters.AddWithValue("@macaddress", Member.macaddress);
                cmd.Parameters.AddWithValue("@salutation_id", Member.salutation_id);
                cmd.Parameters.AddWithValue("@salutation", Member.salutation);
                cmd.Parameters.AddWithValue("@gender", Member.gender);
                cmd.Parameters.AddWithValue("@age", Member.age);
                string encrypt_docno = _encryption.Encrypt(Member.proof_document_no, System.Web.HttpContext.Current.Server.MapPath("~/ENCRYPT.KEY"));
                cmd.Parameters.AddWithValue("@proof_document_no", encrypt_docno);
                cmd.Parameters.AddWithValue("@designtaion_others", Member.designtaion_others);
                cmd.Parameters.AddWithValue("@occupation_others", Member.occupation_others);
                cmd.Parameters.AddWithValue("@document_mongoentry", Member.document_mongoentry);
                cmd.Parameters.AddWithValue("@doc_id",Member.doc_id);
                cmd.Parameters.AddWithValue("@remarks", Member.remarks);
                cmd.Parameters.AddWithValue("@dateofadmission", Convert.ToDateTime(Member.dateofadmission, french).Date);
                cmd.ExecuteNonQuery();
                value = 1;
            }
            catch (NpgsqlException ex)
            {
                value = 0;
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "insertmemberintotemp()" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
                RecordUserAction("btnAddOld_Click", ex.Message, "F"); var errorcode = ex.ErrorCode;
                if (errorcode == 23503)
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
                connect.Close();
            }
            return value;
           
        }

        protected void btnAdd_Click(object sender, EventArgs e)  //  // not completely working, need to add temp member id for new members and file upload query/function
        {
            if (Session["AppID"] != null && Session["created_by"] != null)
            {
                NpgsqlConnection connect = new NpgsqlConnection();
                NpgsqlCommand cmd = new NpgsqlCommand();
                connect.ConnectionString = ConfigurationManager.ConnectionStrings["connect"].ConnectionString;
                cmd.Connection = connect;


                if (ddl_salutation.SelectedValue == "-1")
                {
                    lblError.Text = "Please select Salutation!";
                    relaunchmodal();
                }
                else if (txt_MemName.Text == "" || txt_MemName.Text == null)
                {
                    lblError.Text = "Please Enter Name!";
                    relaunchmodal();
                }
                else if (txtbx_age.Text == "" || txtbx_age.Text == null)
                {
                    lblError.Text = "Please Enter Age!";
                    relaunchmodal();
                }
                else if (!_val.validateData(txt_MemName.Text, _val.alpharegex1))
                {
                    lblError.Text = "Please Enter Valid Name!";
                    relaunchmodal();
                }
                else if (!_val.validateData(txtbx_age.Text, _val.captcha_regex))
                {
                    lblError.Text = "Please Enter Valid Age!";
                    relaunchmodal();
                }
                else if (Int32.Parse(txtbx_age.Text) < 18)
                {
                    lblError.Text = "Age should be greater than 18!";
                    relaunchmodal();
                }
                else if (Int32.Parse(txtbx_age.Text) > 115)
                {
                    lblError.Text = "Age cannot be greater than 115!";
                    relaunchmodal();
                }
                else if (txt_MemAddress.Text == "" || txt_MemAddress.Text == null)
                {
                    lblError.Text = "Please Enter Address!";
                    relaunchmodal();
                }
                else if (!_val.validateData(txt_MemAddress.Text, _val.reamrks_validation))
                {
                    lblError.Text = "Please Enter Valid Address!";
                    relaunchmodal();
                }

                else if (ddl_DesignMem.SelectedValue == "-1")
                {
                    lblError.Text = "Please select Designation of the member!";
                    relaunchmodal();
                }
                else if (ddl_DesignMem.SelectedValue == "8" && (TxtBx_DesignOthers.Text == "" || TxtBx_DesignOthers.Text == null))
                {
                    lblError.Text = "Please Enter Other Designation!";
                    relaunchmodal();
                }
                else if (ddl_DesignMem.SelectedValue == "8" && (!_val.validateData(TxtBx_DesignOthers.Text, _val.alpharegex)))
                {
                    lblError.Text = "Please Enter Valid Designation!";
                    relaunchmodal();
                }
                else if (ddl_MemOccup.SelectedValue == "-1")
                {
                    lblError.Text = "Please Enter Occupation!";
                    relaunchmodal();
                }
                else if (ddl_MemOccup.SelectedValue == "4" && (TxtBx_OccupOthers.Text == "" || TxtBx_OccupOthers.Text == null))
                {
                    lblError.Text = "Please Enter Other Occupation!";
                    relaunchmodal();
                }
                else if (ddl_MemOccup.SelectedValue == "4" && (!_val.validateData(TxtBx_OccupOthers.Text, _val.alpharegex)))
                {
                    lblError.Text = "Please Enter Valid Occupation!";
                    relaunchmodal();
                }
                else if (ddl_MemDocType.SelectedValue == "-1")
                {
                    lblError.Text = "Please select Type of document to be added!";
                    relaunchmodal();
                }
                else if (TxtBxDocumentNo.Text == "" || TxtBxDocumentNo.Text == null)
                {
                    lblError.Text = "Please Enter document no !";
                    relaunchmodal();
                }
                else if (ddlMcom.SelectedValue == "0")
                {
                    lblError.Text = "Please select part of Managing Committee or not";
                    relaunchmodal();
                }
                else if (ddl_MemDocType.SelectedValue == "1" && (!_val.validateData(TxtBxDocumentNo.Text, _val.epicno)))
                {
                    lblError.Text = "Please Enter valid election card no";
                    relaunchmodal();
                }
                else if (ddl_MemDocType.SelectedValue == "2" && (!_val.validateData(TxtBxDocumentNo.Text, _val.cust_driving)))
                {
                    lblError.Text = "Please Enter valid Driving License no";
                    relaunchmodal();
                }
                else if (ddl_MemDocType.SelectedValue == "3" && (!_val.validateData(TxtBxDocumentNo.Text, _val.pan_regex)))
                {
                    lblError.Text = "Please Enter valid Pan card no";
                    relaunchmodal();
                }
                else if (ddl_MemDocType.SelectedValue == "4" && (!_val.validateData(TxtBxDocumentNo.Text, _val.passport)))
                {
                    lblError.Text = "Please Enter valid Passport no";
                    relaunchmodal();
                }
                else if (ddl_MemDocType.SelectedValue == "6" && (!_val.validateData(TxtBxDocumentNo.Text, _val.alpha_numericregex))) //for others
                {
                    lblError.Text = "Please Enter valid Identity Proof no";
                    relaunchmodal();
                }
                else if (ddl_MemDocType.SelectedValue == "5" && (!_val.validateData(TxtBxDocumentNo.Text, _val.adhar_regex)))
                {
                    lblError.Text = "Please Enter valid Aadhar Card no";
                    relaunchmodal();
                }
                else if (ddl_DesignMem.SelectedValue != "7" && check_entry(Convert.ToInt32(ddl_DesignMem.SelectedValue)) == 1)
                {
                    lblError.Text = "Designation is held by some other Member of Society!";
                    relaunchmodal();
                }
                else if (Convert.ToInt32(Session["Renewal"]) == 2 && (txtbxdateadmission.Text == null || txtbxdateadmission.Text == ""))
                {
                    lblError.Text = "Date of admission is blank";
                    relaunchmodal();
                }
                else
                {
                    lblError.Text = "";
                    int a = 0;
                    Int64 mid = Utility.get_memberID();
                    MemberDocs document = new MemberDocs();
                    string doc_name = ddl_MemDocType.SelectedItem.Text;
                    try
                    {

                        a = fileuploadfucntion(FileUpload6, Status, document, doc_name, ipaddress, macaddress, Convert.ToInt64(Session["AppID"].ToString()), "Members Document", mid);


                        if (a == 1)
                        {

                            string query = "";
                            connect.Open();


                            query = "Update esociety.temp_amendment_committee set fname=@fname, designtaion=@designtaion, design=@design, occupation=@occupation, occupatid=@occupatid, address=@address, proofname=@proofname,";
                            query = query + " proofid =@proofid, mangcomm=@mangcomm,created_at=current_timestamp, created_by=@created_by, ipaddress=@ipaddress, macaddress=@macaddress, active='Y',proof_document_no=@proof_document_no,";
                            query = query + " salutation_id =@salutation_id,salutation=@salutation,gender=@gender,age=@age,designtaion_others=@designtaion_others,occupation_others=@occupation_others,remarks=@remarks,dateofadmission=@dateofadmission where app_id=@app_id and member_id=@mid";


                            cmd.CommandText = query;
                            cmd.Parameters.AddWithValue("@mid", mid);
                            cmd.Parameters.AddWithValue("@app_id", Convert.ToInt64(Session["AppID"].ToString()));
                            cmd.Parameters.AddWithValue("@fname", txt_MemName.Text);
                            cmd.Parameters.AddWithValue("@designtaion", ddl_DesignMem.SelectedItem.Text);
                            cmd.Parameters.AddWithValue("@design", Convert.ToInt32(ddl_DesignMem.SelectedValue));
                            cmd.Parameters.AddWithValue("@occupation", ddl_MemOccup.SelectedItem.Text);
                            cmd.Parameters.AddWithValue("@occupatid", Convert.ToInt32(ddl_MemOccup.SelectedValue));
                            cmd.Parameters.AddWithValue("@address", txt_MemAddress.Text);
                            cmd.Parameters.AddWithValue("@proofname", ddl_MemDocType.SelectedItem.Text);
                            cmd.Parameters.AddWithValue("@proofid", Convert.ToInt32(ddl_MemDocType.SelectedValue));
                            cmd.Parameters.AddWithValue("@mangcomm", ddlMcom.SelectedItem.Text.Trim());
                            cmd.Parameters.AddWithValue("@created_by", Session["created_by"].ToString());
                            cmd.Parameters.AddWithValue("@ipaddress", ipaddress);
                            cmd.Parameters.AddWithValue("@macaddress", macaddress);
                            cmd.Parameters.AddWithValue("@salutation_id", Convert.ToInt32(ddl_salutation.SelectedValue));
                            cmd.Parameters.AddWithValue("@salutation", ddl_salutation.SelectedItem.Text);
                            cmd.Parameters.AddWithValue("@gender", Rdbtngender.SelectedValue);
                            cmd.Parameters.AddWithValue("@age", Convert.ToInt32(txtbx_age.Text));
                            string encrypt_docno = _encryption.Encrypt(TxtBxDocumentNo.Text, System.Web.HttpContext.Current.Server.MapPath("~/ENCRYPT.KEY"));
                            cmd.Parameters.AddWithValue("@proof_document_no", encrypt_docno);
                            cmd.Parameters.AddWithValue("@designtaion_others", TxtBx_DesignOthers.Text);
                            cmd.Parameters.AddWithValue("@occupation_others", TxtBx_OccupOthers.Text);
                            cmd.Parameters.AddWithValue("@remarks", TxtBxRemarks.Text);
                            if (Convert.ToInt32(Session["Renewal"]) == 2)
                            {
                                cmd.Parameters.AddWithValue("@dateofadmission", Convert.ToDateTime(txtbxdateadmission.Text, french).Date);
                            }
                            else
                            {
                                cmd.Parameters.AddWithValue("@dateofadmission", DBNull.Value);
                            }

                            cmd.ExecuteNonQuery();                           
                            bindgridview_newmangcomm(Session["AppID"].ToString());
                            RecordUserAction("btnAdd_Click", "Member Data Added Successfully", "S");
                            txt_MemAddress.Text = "";
                            txt_MemName.Text = "";
                            ddl_MemOccup.SelectedValue = "-1";
                            ddl_MemDocType.SelectedValue = "-1";
                            ddl_DesignMem.SelectedValue = "-1";
                            TxtBx_DesignOthers.Text = "";
                            TxtBx_OccupOthers.Text = "";
                            TxtBxDocumentNo.Text = "";
                            ddl_salutation.SelectedValue = "-1";
                            Rdbtngender.ClearSelection();
                            //txtbx_age.Text = "";
                            cmd.Parameters.Clear();
                                                    

                        }

                    }
                    catch (NpgsqlException ex)
                    {

                        CreateLogFiles Err = new CreateLogFiles();
                        Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "btnAdd_Click()" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
                        RecordUserAction("btnAdd_Click", ex.Message, "F"); var errorcode = ex.ErrorCode;
                        if (errorcode == 23503)
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
                        connect.Close();
                    }
                }
               
            }
            else
            {

                RecordUserAction("btnAdd_Click", "Sesiion null", "F");
            }

        }

        protected void btnUpdate_Click(object sender, EventArgs e)
        {
            if (Session["AppID"] != null && Session["created_by"] != null)
            {
                btnAdd.Visible = false;              
                tr_dateofadmission.Visible = true;  //false;
                btnUpdate.Visible = true;

                Int64 mid = getData();
                Member_Details member_history = new Member_Details();
                member_history = FetchTempMember(Session["AppID"].ToString(), mid);                

                if (ddl_salutation.SelectedValue == "-1")
                {
                    lberrormember.Text = "Please select Salutation!";
                    relaunchmodal();
                }
                else if (txt_MemName.Text == "" || txt_MemName.Text == null)
                {
                    lberrormember.Text = "Please Enter Name!";
                    relaunchmodal();
                }
                else if (txtbx_age.Text == "" || txtbx_age.Text == null)
                {
                    lberrormember.Text = "Please Enter Age!";
                    relaunchmodal();
                }
                else if (!_val.validateData(txtbx_age.Text, _val.captcha_regex))
                {
                    lberrormember.Text = "Please Enter Valid Age!";
                    relaunchmodal();
                }
                else if (Int32.Parse(txtbx_age.Text) > 115)
                {
                    lberrormember.Text = "Age cannot be greater than 115!";
                    relaunchmodal();
                }
                else if (Int32.Parse(txtbx_age.Text) < 18)
                {
                    lberrormember.Text = "Minimum age allowed is 18";
                    relaunchmodal();
                }
                else if (txt_MemAddress.Text == "" || txt_MemAddress.Text == null)
                {
                    lberrormember.Text = "Please Enter Address!";
                    relaunchmodal();
                }
                else if (!_val.validateData(txt_MemAddress.Text, _val.reamrks_validation))
                {
                    lberrormember.Text = "Please Enter Valid Address!";
                    relaunchmodal();
                }
                else if (ddl_DesignMem.SelectedValue == "-1")
                {
                    lberrormember.Text = "Please select Designation of the member!";
                    relaunchmodal();
                }
                else if (ddl_DesignMem.SelectedValue == "8" && (TxtBx_DesignOthers.Text == "" || TxtBx_DesignOthers.Text == null))
                {
                    lberrormember.Text = "Please Enter Other Designation!";
                    relaunchmodal();
                }
                else if (ddl_DesignMem.SelectedValue == "8" && (!_val.validateData(TxtBx_DesignOthers.Text, _val.alpharegex)))
                {
                    lberrormember.Text = "Please Enter Valid Designation!";
                    relaunchmodal();
                }
                else if (ddl_MemOccup.SelectedValue == "-1")
                {
                    lberrormember.Text = "Please Enter Occupation!";
                    relaunchmodal();
                }
                else if (ddl_MemOccup.SelectedValue == "4" && (TxtBx_OccupOthers.Text == "" || TxtBx_OccupOthers.Text == null))
                {
                    lberrormember.Text = "Please Enter Other Occupation!";
                    relaunchmodal();
                }
                else if (ddl_MemOccup.SelectedValue == "4" && (!_val.validateData(TxtBx_OccupOthers.Text, _val.alpharegex)))
                {
                    lberrormember.Text = "Please Enter Valid Occupation!";
                    relaunchmodal();
                }

                else if (ddl_MemDocType.SelectedValue == "-1")
                {
                    lberrormember.Text = "Please select Type of document to be added!";
                    relaunchmodal();
                }
                else if (TxtBxDocumentNo.Text == "" || TxtBxDocumentNo.Text == null)
                {
                    lberrormember.Text = "Please Enter document no !";
                    relaunchmodal();
                }
                else if (ddl_MemDocType.SelectedValue == "1" && (!_val.validateData(TxtBxDocumentNo.Text, _val.epicno)))
                {
                    lberrormember.Text = "Please Enter valid election card no";
                    relaunchmodal();
                }
                else if (ddl_MemDocType.SelectedValue == "2" && (!_val.validateData(TxtBxDocumentNo.Text, _val.cust_driving)))
                {
                    lberrormember.Text = "Please Enter valid Driving License no";
                    relaunchmodal();
                }
                else if (ddl_MemDocType.SelectedValue == "3" && (!_val.validateData(TxtBxDocumentNo.Text, _val.pan_regex)))
                {
                    lberrormember.Text = "Please Enter valid Pan card no";
                    relaunchmodal();
                }
                else if (ddl_MemDocType.SelectedValue == "4" && (!_val.validateData(TxtBxDocumentNo.Text, _val.passport)))
                {
                    lberrormember.Text = "Please Enter valid Passport no";
                    relaunchmodal();
                }
                else if (ddl_MemDocType.SelectedValue == "6" && (!_val.validateData(TxtBxDocumentNo.Text, _val.alpha_numericregex)))
                {
                    lberrormember.Text = "Please Enter valid Identity Proof no";
                    relaunchmodal();
                }
                else if (ddl_MemDocType.SelectedValue == "5" && (!_val.validateData(TxtBxDocumentNo.Text, _val.adhar_regex)))
                {
                    lberrormember.Text = "Please Enter valid Aadhar Card no";
                    relaunchmodal();
                }
                else if (ddlMcom.SelectedValue == "0")
                {
                    lberrormember.Text = "Please select part of Managing Committee or not";
                    relaunchmodal();
                }
                else if (Convert.ToInt32(Session["Renewal"]) == 2 && (txtbxdateadmission.Text == null || txtbxdateadmission.Text == ""))
                {
                    lberrormember.Text = " Date of admission is blank";
                    relaunchmodal();
                }
                else
                {
                    NpgsqlConnection connect = new NpgsqlConnection();
                    NpgsqlCommand cmd = new NpgsqlCommand();
                    connect.ConnectionString = ConfigurationManager.ConnectionStrings["connect"].ConnectionString;
                    cmd.Connection = connect;

                    connect.Open();
                    NpgsqlTransaction myTrans = connect.BeginTransaction();
                    cmd.Transaction = myTrans;
                    try
                    {
                        lberrormember.Text = " ";
                        cmd.Parameters.Clear();

                        string inst_history = "INSERT INTO esociety.temp_amendment_committee_history(member_id, app_id, fname, designtaion, design, occupation, occupatid, address, proofname, proofid,";
                        inst_history = inst_history + " mangcomm,created_at, created_by, ipaddress, macaddress, active, document_mongoentry, doc_id, proof_document_no,salutation_id,salutation,gender,age,designtaion_others,occupation_others,remarks,dateofadmission) VALUES";
                        inst_history = inst_history + " (@member_id, @app_id, @fname, @designtaion, @design, @occupation, @occupatid, @address, @proofname, @proofid,@mangcomm,@created_at,@created_by,";
                        inst_history = inst_history + " @ipaddress, @macaddress, 'N', @document_mongoentry, @doc_id, @proof_document_no,@salutation_id,@salutation,@gender,@age,@designtaion_others,@occupation_others,@remarks,@dateofadmission)";
                        cmd.CommandText = inst_history;
                        cmd.Parameters.AddWithValue("@app_id", Convert.ToInt64(member_history.app_id));
                        cmd.Parameters.AddWithValue("@member_id", member_history.member_id);
                        cmd.Parameters.AddWithValue("@fname", member_history.fname);
                        cmd.Parameters.AddWithValue("@designtaion", member_history.designtaion);
                        cmd.Parameters.AddWithValue("@design", member_history.design);
                        cmd.Parameters.AddWithValue("@occupation", member_history.occupation);
                        cmd.Parameters.AddWithValue("@occupatid", member_history.occupatid);
                        cmd.Parameters.AddWithValue("@address", member_history.address);
                        cmd.Parameters.AddWithValue("@proofname", member_history.proofname);
                        cmd.Parameters.AddWithValue("@proofid", member_history.proofid);
                        cmd.Parameters.AddWithValue("@mangcomm", member_history.mangcomm);
                        cmd.Parameters.AddWithValue("@created_at", Convert.ToDateTime(member_history.created_at));
                        cmd.Parameters.AddWithValue("@created_by", member_history.created_by);
                        cmd.Parameters.AddWithValue("@ipaddress", member_history.ipaddress);
                        cmd.Parameters.AddWithValue("@macaddress", member_history.macaddress);
                        cmd.Parameters.AddWithValue("@document_mongoentry", member_history.document_mongoentry);
                        cmd.Parameters.AddWithValue("@doc_id", member_history.doc_id);
                        cmd.Parameters.AddWithValue("@proof_document_no", member_history.proof_document_no);
                        cmd.Parameters.AddWithValue("@salutation_id", member_history.salutation_id);
                        cmd.Parameters.AddWithValue("@salutation", member_history.salutation);
                        cmd.Parameters.AddWithValue("@gender", member_history.gender);
                        cmd.Parameters.AddWithValue("@age", member_history.age);
                        cmd.Parameters.AddWithValue("@designtaion_others", member_history.designtaion_others);
                        cmd.Parameters.AddWithValue("@occupation_others", member_history.occupation_others);
                        cmd.Parameters.AddWithValue("@remarks", member_history.remarks);
                        cmd.Parameters.AddWithValue("@dateofadmission", member_history.dateofadmission);


                        cmd.ExecuteNonQuery();
                        cmd.Parameters.Clear();


                        string query = "";
                       
                            query = "update esociety.temp_amendment_committee set fname=@fname , occupation=@occupation, occupatid=@occupatid, address=@address, proofname=@proofname, proofid=@proofid,";
                            query = query + " mangcomm =@mangcomm, created_at=current_timestamp, created_by=@created_by, ipaddress=@ipaddress, macaddress=@macaddress,";
                            query = query + " proof_document_no =@proofno,active='Y',salutation_id=@salutation_id,salutation=@salutation,gender=@gender,age=@age,designtaion_others=@designtaion_others,occupation_others=@occupation_others,remarks = @remarks,dateofadmission=@dateofadmission where member_id=@mid and app_id=@AppID";
                       
                        cmd.CommandText = query;
                        cmd.Parameters.AddWithValue("@fname", txt_MemName.Text);
                        cmd.Parameters.AddWithValue("@designtaion", ddl_DesignMem.SelectedItem.Text);
                        //cmd.Parameters.AddWithValue("@design", Convert.ToInt32(ddl_DesignMem.SelectedValue));
                        cmd.Parameters.AddWithValue("@occupation", ddl_MemOccup.SelectedItem.Text);
                        cmd.Parameters.AddWithValue("@occupatid", Convert.ToInt32(ddl_MemOccup.SelectedValue));
                        cmd.Parameters.AddWithValue("@address", txt_MemAddress.Text);
                        cmd.Parameters.AddWithValue("@proofname", ddl_MemDocType.SelectedItem.Text);
                        cmd.Parameters.AddWithValue("@proofid", Convert.ToInt32(ddl_MemDocType.SelectedValue));
                        cmd.Parameters.AddWithValue("@mangcomm", ddlMcom.SelectedItem.Text.Trim());
                        cmd.Parameters.AddWithValue("@created_by", Session["created_by"].ToString());
                        cmd.Parameters.AddWithValue("@ipaddress", ipaddress);
                        cmd.Parameters.AddWithValue("@macaddress", macaddress);
                        cmd.Parameters.AddWithValue("@mid", mid);
                        string encrypt_docno = _encryption.Encrypt(TxtBxDocumentNo.Text, System.Web.HttpContext.Current.Server.MapPath("~/ENCRYPT.KEY"));
                        cmd.Parameters.AddWithValue("@proofno", encrypt_docno);
                        cmd.Parameters.AddWithValue("@salutation_id", Convert.ToInt32(ddl_salutation.SelectedValue));
                        cmd.Parameters.AddWithValue("@salutation", ddl_salutation.SelectedItem.Text);
                        cmd.Parameters.AddWithValue("@gender", Rdbtngender.SelectedValue);
                        cmd.Parameters.AddWithValue("@age", Convert.ToInt32(txtbx_age.Text));
                        cmd.Parameters.AddWithValue("@AppID", Convert.ToInt64(Session["AppID"].ToString()));
                        cmd.Parameters.AddWithValue("@designtaion_others", TxtBx_DesignOthers.Text);
                        cmd.Parameters.AddWithValue("@occupation_others", TxtBx_OccupOthers.Text);
                        cmd.Parameters.AddWithValue("@remarks", TxtBxRemarks.Text);                       
                        cmd.Parameters.AddWithValue("@dateofadmission", Convert.ToDateTime(txtbxdateadmission.Text, french).Date);
                        

                        cmd.ExecuteNonQuery();
                        cmd.Parameters.Clear();



                        if (FileUpload6.HasFile)
                        {
                            MemberDocs mongodoc = new MemberDocs();
                            HttpPostedFile file = FileUpload6.PostedFile;
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

                                if (Extension.ToLower() != "pdf" || file.ContentType.ToLower() != "application/pdf" || mimeType != "application/pdf"
                                    || extCount > 1
                                    || document == null
                                    || file.ContentLength <= 0 || file.ContentLength > (2 * 1024 * 1024))
                                {

                                    Status.Text = "File name can not contain multiple dots/extensions. Only Pdf file of maximum size 2 MB allowed !";
                                    Status.ForeColor = System.Drawing.Color.Red;
                                }
                                else
                                {
                                    fileSizeFront = FileUpload6.FileContent.Length;
                                    documentBinary = new byte[fileSizeFront];
                                    byte[] bytes = FileUpload6.FileBytes;
                                    byte[] encrypt_bytes = obj_Byte_Encryption.EncryptData(bytes, System.Web.HttpContext.Current.Server.MapPath("~/ENCRYPT.KEY"));
                                    ObjectId obj_id = ObjectId.GenerateNewId();
                                    string document_id = member_history.doc_id;//doc ID
                                    mongodoc.App_ID = Convert.ToInt64(Session["AppID"].ToString());
                                    mongodoc.Doc_CT = ct;
                                    mongodoc.time_stamp = DateTime.Now.ToString();
                                    mongodoc._Id = obj_id;
                                    mongodoc.IpAddress = ipaddress;
                                    mongodoc.MacAddress = macaddress;
                                    mongodoc.Active = true;
                                    mongodoc.Member_ID = mid;
                                    mongodoc.Doc_ID = document_id;
                                    mongodoc.UpdatedBy = Session["created_by"].ToString();
                                    mongodoc.doc_name = ddl_MemDocType.SelectedItem.Text;
                                    mongodoc.DocContent = encrypt_bytes;
                                    mongodoc._Id = obj_id;
                                    int amongo = InsertintoMongoDB(mongodoc, "Members Document");
                                    if (amongo == 1)
                                    {
                                        string query1 = "update esociety.temp_amendment_committee set document_mongoentry=@mongo where doc_id=@docid";
                                        cmd.CommandText = query1;
                                        cmd.Parameters.AddWithValue("@mongo", obj_id.ToString());
                                        cmd.Parameters.AddWithValue("@docid", member_history.doc_id);
                                        cmd.ExecuteNonQuery();
                                        Response.Write("<script>alert('Updated successful');</script>");
                                        bindgridview_newmangcomm(Session["AppID"].ToString());
                                        myTrans.Commit();
                                        ViewState["edit_flag"] = 0;
                                        ddl_DesignMem.Enabled = true;
                                        RecordUserAction("btnUpdate_Click", "Update Member Data", "S");
                                        Response.Redirect("MemberDetails.aspx");
                                    }
                                    else
                                    {
                                        myTrans.Rollback();
                                        Response.Write("<script>alert('Updation Failed');</script>");
                                    }

                                }
                            }
                        }

                        myTrans.Commit();

                        bindgridview_newmangcomm(Session["AppID"].ToString());

                    }
                    catch (NpgsqlException ex)
                    {
                        CreateLogFiles Err = new CreateLogFiles();
                        Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "btnUpdate_Click" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
                        myTrans.Rollback();
                        RecordUserAction("btnUpdate_Click", ex.Message, "F");
                        Response.Write("<script language='javascript'>alert('" + "Update Member Data Exception" + "')</script>");
                    }
                    finally
                    {
                        connect.Close();
                        ViewState["edit_flag"] = 0;
                        btnAdd.Visible = true;

                    }
                }
            }
            else
            {
                RecordUserAction("btnUpdate_Click", "Session value null on button click", "S");
            }
        }

        public void relaunchmodal()
        {           
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "none", "<script> $(window).on('load', function () { $('#addmembersmodal').modal({ backdrop: 'static' });});</script>", false);

        }

        public int fileuploadfucntion(FileUpload fu, Label lb, MemberDocs mongodoc, string a, string ip, string mac, long App_ID, string collection, Int64 mid)
        {
            RecordUserAction("fileuploadfucntion", "File Upload Fucntion Called", "S");
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

                            tr_dateofadmission.Visible = true;
                            //if (Convert.ToInt32(Session["Renewal"]) == 2)
                            //{
                            //    tr_dateofadmission.Visible = true;
                            //}
                            //else
                            //{
                            //    tr_dateofadmission.Visible = false;  
                            //}
                            if (Extension.ToLower() != "pdf" || file.ContentType.ToLower() != "application/pdf" || mimeType != "application/pdf")
                            {
                                lberrormember.Text = "Please upload only Pdf file of maximum size 2MB allowed with extension .pdf!";
                                lblError.ForeColor = System.Drawing.Color.Red;
                                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "none", "<script> $(window).on('load', function () { $('#addmembersmodal').modal({ backdrop: 'static' });});</script>", false);
                                flag = 0;
                            }
                            else if (extCount > 1)
                            {
                                lberrormember.Text = "File name can not contain multiple dots/extensions.";
                                lblError.ForeColor = System.Drawing.Color.Red;
                                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "none", "<script> $(window).on('load', function () { $('#addmembersmodal').modal({ backdrop: 'static' });});</script>", false);

                                flag = 0;
                            }
                            else if (document == null)
                            {
                                lberrormember.Text = "Please upload file in pdf format or pdf not in correct format";
                                lblError.ForeColor = System.Drawing.Color.Red;
                                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "none", "<script> $(window).on('load', function () { $('#addmembersmodal').modal({ backdrop: 'static' });});</script>", false);

                                flag = 0;
                            }
                            else if (file.ContentLength <= 0 || file.ContentLength > (FileSize))
                            {
                                lberrormember.Text = "Only Pdf file of maximum size 2MB allowed !";
                                lblError.ForeColor = System.Drawing.Color.Red;
                                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "none", "<script> $(window).on('load', function () { $('#addmembersmodal').modal({ backdrop: 'static' });});</script>", false);

                                flag = 0;
                            }

                            else
                            {
                                fileSizeFront = fu.FileContent.Length;
                                documentBinary = new byte[fileSizeFront];
                                //Stream fs = fu.PostedFile.InputStream;
                                //BinaryReader br = new BinaryReader(fs);
                                //byte[] bytes = br.ReadBytes((Int32)fs.Length);
                                byte[] bytes = fu.FileBytes;
                                byte[] encrypt_bytes = obj_Byte_Encryption.EncryptData(bytes, System.Web.HttpContext.Current.Server.MapPath("~/ENCRYPT.KEY"));
                                string document_id = Utility.get_docID();//doc ID
                                mongodoc.App_ID = App_ID;
                                mongodoc.Doc_CT = ct;
                                mongodoc.time_stamp = DateTime.Now.ToString();
                                ObjectId obj_id = ObjectId.GenerateNewId();
                                mongodoc._Id = obj_id;
                                mongodoc.IpAddress = ip;
                                mongodoc.MacAddress = mac;
                                mongodoc.Active = true;
                                mongodoc.Member_ID = mid;
                                mongodoc.Doc_ID = document_id;
                                mongodoc.UpdatedBy = Session["created_by"].ToString();
                                mongodoc.doc_name = a;
                                mongodoc.DocContent = encrypt_bytes;
                                int mvalue = InsertintoMongoDB(mongodoc, collection);
                                if (mvalue == 1)
                                {
                                    int pvalue = insertentryPosgres(obj_id, document_id, App_ID, mid);
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
                    else
                    {
                        lblError.ForeColor = System.Drawing.Color.Red;
                        lberrormember.Text = "Upload " + a + " Certificate file in PDF format";
                        flag = 0;
                        
                        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "none", "<script> $(window).on('load', function () { $('#addmembersmodal').modal({ backdrop: 'static' });});</script>", false);
                    }


                }
                catch (Exception ex)
                {
                    RecordUserAction("fileuploadfucntion", ex.Message, "F");
                    lberrormember.Text = "File Upload Function Excecution Failed";
                    CreateLogFiles Err = new CreateLogFiles();
                    Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "fileuploadfucntion()" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));

                }
            }
            return flag;

        }

        public int InsertintoMongoDB(MemberDocs doc, string sel_collection)
        {
            Insert insr = new Insert();
            try
            {
                RecordUserAction("InsertintoMongoDB", "Insert in MongoDB", "S");
                return insr.InsertintoMongoDB(doc, sel_collection);
            }
            catch (Exception ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "InsertintoMongoDB()" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
                RecordUserAction("InsertintoMongoDB", "Insert in MongoDB Failed", "F");
                Status.Text = "Insert in db Failed";
                return 0;
            }
        }

        public int insertentryPosgres(ObjectId obj, string docid, long appid, Int64 mid)
        {
            int value = 0;
            if (Session["AppID"].ToString() == appid.ToString())
            {
                NpgsqlConnection conn = new NpgsqlConnection();
                NpgsqlCommand cmd = new NpgsqlCommand();
                conn.ConnectionString = ConfigurationManager.ConnectionStrings["connect"].ConnectionString;
                cmd.Connection = conn;
                string objectid = obj.ToString();
                conn.Open();
                try
                {
                    string query = "INSERT INTO esociety.temp_amendment_committee(document_mongoentry, doc_id, app_id,member_id)VALUES(@document_mongoentry,@doc_id, @app_id,@member_id)";
                    cmd.CommandText = query;
                    cmd.Parameters.AddWithValue("@document_mongoentry", objectid);
                    cmd.Parameters.AddWithValue("@doc_id", docid);
                    cmd.Parameters.AddWithValue("@app_id", appid);
                    cmd.Parameters.AddWithValue("@member_id", mid);
                    cmd.ExecuteNonQuery();
                    value = 1;
                    RecordUserAction("insertentryPosgres", "Insert in members", "S");
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
                RecordUserAction("insertentryPosgres", "Execution Error/Session tampared", "F");
                Status.Text = "Execution error";
            }
            return value;
        }


        public void setData(Int64 value)
        {
            Session["amend_mid"] = value;
        }
        public Int64 getData()
        {
            return Convert.ToInt64(Session["amend_mid"].ToString());
        }


        public Member_Details FetchTempMember(string appid, Int64 memberid)
        {
            Member_Details member = new Member_Details();
            NpgsqlConnection conn = new NpgsqlConnection();
            NpgsqlCommand cmd = new NpgsqlCommand();
            conn.ConnectionString = ConfigurationManager.ConnectionStrings["connect"].ConnectionString;
            cmd.Connection = conn;
            conn.Open();
            NpgsqlTransaction myTrans = conn.BeginTransaction();
            cmd.Transaction = myTrans;
            try
            {

                string query = "Select member_id, app_id, fname, designtaion, design, occupation, occupatid, address, proofname, proofid, mangcomm, created_at, created_by,ipaddress, macaddress,active,";
                query = query + " document_mongoentry, doc_id, proof_document_no,salutation_id,salutation,gender,age,designtaion_others,occupation_others,remarks,dateofadmission from esociety.temp_amendment_committee where active='Y' and member_id=@mid and app_id=@AppID";
                cmd.CommandText = query;
                cmd.Parameters.AddWithValue("@mid", memberid);
                cmd.Parameters.AddWithValue("@AppID", Convert.ToInt64(appid));
                NpgsqlDataReader rd = cmd.ExecuteReader();
                if (rd.Read())
                {

                    member.app_id = appid;
                    member.member_id = memberid;
                    member.salutation = rd["salutation"].ToString();
                    member.fname = rd["fname"].ToString();
                    member.gender = rd["gender"].ToString();
                    member.age = Convert.ToInt32(rd["age"].ToString());
                    member.designtaion = rd["designtaion"].ToString();
                    member.design = Convert.ToInt32(rd["design"].ToString());
                    member.occupation = rd["occupation"].ToString();
                    member.occupatid = Convert.ToInt32(rd["occupatid"].ToString());
                    member.address = rd["address"].ToString();
                    member.proofname = rd["proofname"].ToString();
                    member.proofid = Convert.ToInt32(rd["proofid"].ToString());
                    member.mangcomm = rd["mangcomm"].ToString();
                    member.created_at = rd["created_at"].ToString();
                    member.created_by = rd["created_by"].ToString();
                    member.ipaddress = rd["ipaddress"].ToString();
                    member.macaddress = rd["macaddress"].ToString();
                    member.active = rd["active"].ToString();
                    member.document_mongoentry = rd["document_mongoentry"].ToString();
                    member.doc_id = rd["doc_id"].ToString();
                    member.proof_document_no = rd["proof_document_no"].ToString();
                    member.salutation_id = Convert.ToInt32(rd["salutation_id"].ToString());
                    member.designtaion_others = rd["designtaion_others"].ToString();
                    member.occupation_others = rd["occupation_others"].ToString();
                    member.remarks = rd["remarks"].ToString();
                    member.dateofadmission = rd["dateofadmission"].ToString();

                    cmd.Parameters.Clear();
                }
                rd.Close();
            }
            catch (NpgsqlException ex)
            {
                myTrans.Rollback();
                member = null;
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(HttpContext.Current.Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "FetchTempMember()---Insert.cs");
            }
            finally
            {
                conn.Close();
            }
            return member;
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
                var collection = database.GetCollection<MemberDocs>("Members Document");
                var status = collection.Find(x => x._Id == ObjectId.Parse(objectid)).FirstOrDefault();
                string abc = status.Active.ToString();
                RecordUserAction("openConnectionMongo", "Mongo Connected", "S");
                return abc;
            }
            catch (MongoException ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "openConnectionMongo()" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
                RecordUserAction("openConnectionMongo", ex.Message, "F");
                return null;
            }
        }

        public int check_entry(int designation)
        {
            int a = -1;
            NpgsqlConnection connect = new NpgsqlConnection();
            NpgsqlCommand cmd = new NpgsqlCommand();
            connect.ConnectionString = ConfigurationManager.ConnectionStrings["connect"].ConnectionString;
            cmd.Connection = connect;
            try
            {
                if (designation == 7 || designation == 8)
                {
                    btnAdd.Enabled = true;
                    lberrormember.Text = " ";
                    ddl_MemDocType.Enabled = true;
                    a= 0;
                }
                else
                {

                    connect.Open();
                    string query = "";
                    if (designation == 1 || designation == 9)
                    {
                        query = "Select fname from esociety.temp_amendment_committee where app_id=@AppID and (design=1 or design=9)";
                    }
                    else
                    {
                        query = "Select fname from esociety.temp_amendment_committee where app_id=@AppID and design=@design";
                    }
                    cmd.CommandText = query;
                    cmd.Parameters.AddWithValue("@AppID", Convert.ToInt64(Session["AppID"].ToString()));
                    cmd.Parameters.AddWithValue("@design", designation);
                    NpgsqlDataReader rd = cmd.ExecuteReader();
                    if (rd.Read())
                    {
                        string message = Server.HtmlEncode(rd["fname"].ToString());
                        message = message + " has held this Designation.";
                        btnAdd.Enabled = false;
                        ddl_MemDocType.Enabled = false;
                        lberrormember.Text = message;
                        //ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "none", "<script> $(window).on('load', function () { $('#myModal').modal({ backdrop: 'static' });});</script>", false);

                        a= 1;
                    }
                    else
                    {
                        lberrormember.Text = " ";
                        btnAdd.Enabled = true;
                        ddl_MemDocType.Enabled = true;
                        a= 0;

                    }
                    rd.Close();
                }
            }
            catch (NpgsqlException ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "check_entry()" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
                a= 1;
            }
            finally
            {

                connect.Close();
            }
            return a;
        }

        protected void ddl_MemDocType_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (Convert.ToInt32(ViewState["edit_flag"]) == 1)
                {
                    btnAdd.Visible = false;
                    btnUpdate.Visible = true;

                }                
                else
                {
                    btnAdd.Visible = true;                  
                    btnUpdate.Visible = false;
                }


                TxtBxDocumentNo.Text = "";
                trvisible.Visible = true;
                if (Convert.ToInt32(ddl_MemDocType.SelectedValue) == 1)
                {
                    btnAdd.Enabled = false;
                    string value = ddl_MemDocType.SelectedItem.Text;
                    Label19.Text = value + " No";
                    TxtBxDocumentNo.Attributes.Add("placeholder", "");
                   
                }
                else if (Convert.ToInt32(ddl_MemDocType.SelectedValue) == 2)
                {
                    btnAdd.Enabled = false;
                    string value = ddl_MemDocType.SelectedItem.Text;
                    Label19.Text = value + " No";
                    TxtBxDocumentNo.Attributes.Add("placeholder", "");
                    //TxtBxDocumentNo.Attributes.Add("placeholder", "Ex-  DL1420110012345");
                }
                else if (Convert.ToInt32(ddl_MemDocType.SelectedValue) == 3)
                {
                    btnAdd.Enabled = false;
                    string value = ddl_MemDocType.SelectedItem.Text;
                    Label19.Text = value + " No";
                    TxtBxDocumentNo.Attributes.Add("placeholder", "");
                    //TxtBxDocumentNo.Attributes.Add("placeholder", "Ex-  AAAPL1234C");
                }
                else if (Convert.ToInt32(ddl_MemDocType.SelectedValue) == 4)
                {
                    btnAdd.Enabled = false;
                    string value = ddl_MemDocType.SelectedItem.Text;
                    Label19.Text = value + " No";
                    TxtBxDocumentNo.Attributes.Add("placeholder", "");
                    //TxtBxDocumentNo.Attributes.Add("placeholder", "Ex-  A1234567");
                }
                else if (Convert.ToInt32(ddl_MemDocType.SelectedValue) == 5)
                {
                    btnAdd.Enabled = false;
                    string value = ddl_MemDocType.SelectedItem.Text;
                    Label19.Text = value + " No";
                    TxtBxDocumentNo.Attributes.Add("placeholder", "");
                    //  TxtBxDocumentNo.Text = "";
                }
                else if (Convert.ToInt32(ddl_MemDocType.SelectedValue) == 6)
                {
                    btnAdd.Enabled = false;
                    Label19.Text = "Other Document" + " No";
                    TxtBxDocumentNo.Attributes.Add("placeholder", "");
                    TxtBxDocumentNo.Text = "";
                }
                //if (Convert.ToInt32(Session["Renewal"]) == 2)
               // {
                    tr_dateofadmission.Visible = true;
                //}
               // else
               // {
                   // tr_dateofadmission.Visible = false;
                //}
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "none", "<script> $(window).on('load', function () { $('#addmembersmodal').modal({ backdrop: 'static' });});</script>", false);

            }
            catch (Exception ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "ddl_MemDocType_SelectedIndexChanged" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
            }
        }

        protected void Rdbtngender_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (Convert.ToInt32(ViewState["edit_flag"]) == 1)
                {
                    btnAdd.Visible = false;                  
                    btnUpdate.Visible = true;
                }               
                else
                {
                    btnAdd.Visible = true;                   
                    btnUpdate.Visible = false;
                }

                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "none", "<script> $(window).on('load', function () { $('#addmembersmodal').modal({ backdrop: 'show' });});</script>", false);

            }
            catch (Exception ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "Rdbtngender_SelectedIndexChanged" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
            }
        }

    

        protected void ddl_MemOccup_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (ddl_MemOccup.SelectedValue == "4")
                {
                    row_OccupOthers.Visible = true;
                }
                else
                {
                    row_OccupOthers.Visible = false;
                    TxtBx_OccupOthers.Text = "";
                }

                if (Convert.ToInt32(ViewState["edit_flag"]) == 1)
                {
                    btnAdd.Visible = false;
                    btnUpdate.Visible = true;
                }
                else
                {
                    btnAdd.Visible = true;
                    btnUpdate.Visible = false;
                }
                               
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "none", "<script> $(window).on('load', function () { $('#addmembersmodal').modal({ backdrop: 'static' });});</script>", false);

            }
            catch (Exception ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "ddl_MemOccup_SelectedIndexChanged" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));

            }

        }


        protected void ddl_DesignMem_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
               

                if (ddl_DesignMem.SelectedValue == "8")  // for others
                {
                    row_DesignOthers.Visible = true;
                }
                else
                {
                    row_DesignOthers.Visible = false;
                    TxtBx_DesignOthers.Text = "";
                }

                if (Convert.ToInt32(ViewState["edit_flag"]) == 1)
                {
                    btnAdd.Visible = false;
                    btnUpdate.Visible = true;
                }
                else
                {
                    btnAdd.Visible = true;
                    btnUpdate.Visible = false;
                }              


                check_entry(Convert.ToInt32(ddl_DesignMem.SelectedValue));

                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "none", "<script> $(window).on('load', function () { $('#addmembersmodal').modal({ backdrop: 'show' });});</script>", false);

            }
            catch (Exception ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "ddl_DesignMem_SelectedIndexChanged" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
            }
        }

        protected void TxtBxDocumentNo_TextChanged(object sender, EventArgs e)
        {
            try
            {

                lberrormember.Text = "";
                int tempflag = 0;
                int n = Convert.ToInt16(ddl_MemDocType.SelectedValue);
                switch (n)
                {
                    case 1:
                        if (ddl_MemDocType.SelectedValue == "1")
                        {
                            if (!_val.validateData(TxtBxDocumentNo.Text, _val.epicno))
                            {
                                btnAdd.Enabled = false;
                              
                                // btnAdd.Visible = false;
                                btnUpdate.Enabled = false;
                                lberrormember.Text = "Enter valid election card no";
                                relaunchmodal();
                            }
                            else
                            {
                                tempflag = 1;

                            }
                        }
                        break;
                    case 2:
                        if (ddl_MemDocType.SelectedValue == "2")
                        {
                            if (!_val.validateData(TxtBxDocumentNo.Text, _val.cust_driving))
                            {
                                btnAdd.Enabled = false;
                              
                                // btnAdd.Visible = false;
                                btnUpdate.Enabled = false;
                                lberrormember.Text = "Enter valid Driving License no";
                                relaunchmodal();
                            }
                            else
                            {
                                tempflag = 1;
                            }
                        }
                        break;
                    case 3:
                        if (ddl_MemDocType.SelectedValue == "3")
                        {
                            if (!_val.validateData(TxtBxDocumentNo.Text, _val.pan_regex))
                            {
                                btnAdd.Enabled = false;                                
                                // btnAdd.Visible = false;
                                btnUpdate.Enabled = false;
                                lberrormember.Text = "Enter valid PAN no";
                                relaunchmodal();
                            }
                            else
                            {
                                tempflag = 1;
                            }
                        }
                        break;
                    case 4:

                        if (ddl_MemDocType.SelectedValue == "4")
                        {
                            if (!_val.validateData(TxtBxDocumentNo.Text, _val.passport))
                            {
                                btnAdd.Enabled = false;
                                
                                // btnAdd.Visible = false;
                                btnUpdate.Enabled = false;
                                lberrormember.Text = "Enter valid Passport no";
                                relaunchmodal();
                            }
                            else
                            {
                                tempflag = 1; lberrormember.Text = "";
                            }
                        }
                        break;

                    case 5:
                        if (ddl_MemDocType.SelectedValue == "5")
                        {
                            if (!_val.validateData(TxtBxDocumentNo.Text, _val.adhar_regex))
                            {
                                btnAdd.Enabled = false;
                                
                                // btnAdd.Visible = false;
                                btnUpdate.Enabled = false;
                                lberrormember.Text = "Please Enter valid Aadhar Card no";
                                relaunchmodal();
                            }
                            else
                            {
                                tempflag = 1;
                            }
                        }
                        break;
                    //resolvede
                    case 6:
                        if (ddl_MemDocType.SelectedValue == "6")
                        {
                            if (!_val.validateData(TxtBxDocumentNo.Text, _val.alpha_numericregex))
                            {
                                btnAdd.Enabled = false;
                               
                                //  btnAdd.Visible = false;
                                btnUpdate.Enabled = false;
                                lberrormember.Text = "Enter valid no.No special Characters allowed.";
                                relaunchmodal();
                            }
                            else
                            {
                                tempflag = 1;

                            }
                        }
                        break;
                    default:
                        break;

                }

                if (tempflag == 1)
                {
                    if (Convert.ToInt32(ViewState["edit_flag"]) == 1)
                    {
                        btnAdd.Visible = false;
                        btnUpdate.Visible = true;
                    }
                    else
                    {
                        btnAdd.Visible = true;
                        btnAdd.Enabled = true;
                        btnUpdate.Visible = false;
                    }

                    lberrormember.Text = "";
                }

                
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "none", "<script> $(window).on('load', function () { $('#addmembersmodal').modal({ backdrop: 'static' });});</script>", false);

            }
            catch (Exception ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "TxtBxDocumentNo_TextChanged" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
            }
        }

        protected void close_modal_Click(object sender, EventArgs e)
        {
            try
            {

                ViewState["edit_flag"] = 0;
                btnAdd.Visible = true;
                lberrormember.Text = "";
                Status.Text = "";
                TxtBxDocumentNo.Text = "";
                trvisible.Visible = false;
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "none", "<script> $(window).on('load', function () { $('#addmembersmodal').modal('hide');});</script>", false);
            }
            catch (Exception ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "close_modal_Click" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
            }
        }

        protected void LkAddNewMember_Click(object sender, EventArgs e)
        {
            try
            {               
                TxtBxRemarks.Text = "";
                ddl_salutation.SelectedValue = "-1";
                ddl_salutation.Enabled = true;
                txt_MemName.Text = "";
                txt_MemName.Enabled = true;
                ddl_DesignMem.SelectedValue = "-1";
                ddl_DesignMem.Enabled = true;
                row_DesignOthers.Visible = false;
                ddl_MemOccup.SelectedValue = "-1";
                row_OccupOthers.Visible = false;
                ddl_salutation.SelectedValue = "-1";
                Rdbtngender.ClearSelection();
                Rdbtngender.Enabled = true;
                txtbx_age.Text = "";
                txt_MemAddress.Text = "";
                ddl_MemDocType.SelectedValue = "-1";
                ddlMcom.SelectedValue = "1";
                ddlMcom.Enabled = true;
                TxtBxDocumentNo.Text = "";
                lbuploadmember.Text = "";
                btnAdd.Visible = true;
                btnUpdate.Visible = false;                   
                TxtBxRemarks.Text = "";
                btnAdd.Enabled = true;
                lblError.Text = "";

                trvisible.Visible = false;
                tr_dateofadmission.Visible = true;
                   
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "none", "<script> $(window).on('load', function () { $('#addmembersmodal').modal({ backdrop: 'static' });});</script>", false);
               
            }
            catch (Exception ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "LkAddNewMember_Click" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
            }
        }
    }
}