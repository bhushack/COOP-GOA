using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.Net.NetworkInformation;
using System.Configuration;
using System.Management;
using System.Net;
using System.Runtime.InteropServices;
using Npgsql;
using System.Web.UI.WebControls;
using System.Data;
using System.IO;
using System.Web.Script.Serialization;
using System.Security.Cryptography;
using GoaSocietyRegistration.Development;
using System.Diagnostics;
using MongoDB.Driver;
using MongoDB.Bson;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using System.Text.RegularExpressions;
using GoaSocietyRegistration.Organization.Digital.Design;

namespace GoaSocietyRegistration
{
    public class Utility
    {
        System.Globalization.CultureInfo french = new System.Globalization.CultureInfo("fr-FR");

        [DllImport("iphlpapi.dll", ExactSpelling = true)]
        public static extern int SendARP(int DestIP, int SrcIP, byte[] pMacAddr, ref uint PhyAddrLen);
        //public static string getIP()
        //{
        //    string ipaddress = "NA";
        //    ipaddress = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
        //    if (ipaddress == "" || ipaddress == null)
        //        ipaddress = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
        //    if(ipaddress=="::1" )
        //    {
        //        ipaddress = "10.155.4.42";
        //    }
        //    return ipaddress;
        //}
        public static string getIP()
        {
            try
            {
                var LocalIp = (HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"] != null && HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"] != "")
               ? HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"]
               : HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                if (LocalIp.Contains(","))
                    LocalIp = LocalIp.Split(',').First().Trim();
                return LocalIp;
            }
            catch (Exception ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(HttpContext.Current.Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "getIP()----Utility.cs");
                return null;
            }
        }

        public static ObjectId getObjID(string memberid)
        {
            ObjectId obj = new ObjectId();
            NpgsqlConnection conn = new NpgsqlConnection();
            NpgsqlCommand cmd = new NpgsqlCommand();
            conn.ConnectionString = ConfigurationManager.ConnectionStrings["connect"].ConnectionString;
            cmd.Connection = conn;
            string query;
            try
            {
                conn.Open();
                query = "SELECT proof_document_no FROM esociety.members WHERE member_id=@member_id and proofid=5";
                cmd.CommandText = query;
                cmd.Parameters.AddWithValue("@member_id", Convert.ToInt64(memberid));
                NpgsqlDataReader rd = cmd.ExecuteReader();
                if (rd.Read())
                {
                    obj = ObjectId.Parse(rd["proof_document_no"].ToString());
                }
                rd.Close();
            }
            catch (Exception ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(HttpContext.Current.Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "getObjID()----Utility.cs");
            }
            finally
            {
                conn.Close();
            }
            return obj;
        }

        public static int checkFile(string file)
        {
            CreateLogFiles Err = new CreateLogFiles();
            int a = 0;
            try
            { 
                if (Regex.IsMatch(file, @"(\/JS\s*\(.*\))|(\/JavaScript)|(\/OpenAction\s*\()"))
                {
                    Err.ErrorLog(HttpContext.Current.Server.MapPath("~/Logs/ErrorLog"), "JavaScript Execution", "checkFile()----Utility.cs");
                    a = 0;
                }
                //else if (Regex.IsMatch(file, @"((https?|ftp):\/\/[^\s/$.?#].[^\s]*)"))
                //{
                //    Err.ErrorLog(HttpContext.Current.Server.MapPath("~/Logs/ErrorLog"), "URL Injection", "checkFile()----Utility.cs");
                //    a = 0;
                //}
                else if (Regex.IsMatch(file, @"\/EmbeddedFile\s*\/Name"))
                {
                    Err.ErrorLog(HttpContext.Current.Server.MapPath("~/Logs/ErrorLog"), "Embedded Files", "checkFile()----Utility.cs");
                    a = 0;
                }
                else if (Regex.IsMatch(file, @"\/SubmitForm\s*\/F\s*\((.*)\)"))
                {
                    Err.ErrorLog(HttpContext.Current.Server.MapPath("~/Logs/ErrorLog"), "Form Submission", "checkFile()----Utility.cs");
                    a = 0;
                }
                else if (Regex.IsMatch(file, @"\/Launch\s*\/F\s*\((.*)\)"))
                {
                    Err.ErrorLog(HttpContext.Current.Server.MapPath("~/Logs/ErrorLog"), "Launch Actions", "checkFile()----Utility.cs");
                    a = 0;
                }
                else if (Regex.IsMatch(file, @"<script.*>.*<\/script>"))
                {
                    Err.ErrorLog(HttpContext.Current.Server.MapPath("~/Logs/ErrorLog"), "Cross-Site Scripting (XSS)", "checkFile()----Utility.cs");
                    a = 0;
                }
                //else if (Regex.IsMatch(file, @"\/Producer\s*\(.*\)|\/Creator\s*\(.*\)|\/Author\s*\(.*\)|\/Title\s*\(.*\)"))
                //{
                //    Err.ErrorLog(HttpContext.Current.Server.MapPath("~/Logs/ErrorLog"), "Metadata Manipulation", "checkFile()----Utility.cs");
                //    a = 0;
                //}
                else if (Regex.IsMatch(file, @"\/AA\s*<</O\s*\((.*)\)"))
                {
                    Err.ErrorLog(HttpContext.Current.Server.MapPath("~/Logs/ErrorLog"), "Page Actions", "checkFile()----Utility.cs");
                    a = 0;
                }
                else  
                {
                    a = 1;
                }
            }
            catch(Exception ex)
            {
               
                Err.ErrorLog(HttpContext.Current.Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "checkFile()----Utility.cs");
                a = -1;
            }
            return a;
        }
        public static string getAadharNo(string memberid)
        {
            string aadhar = "";
            try
            {
                var str = ConfigurationManager.AppSettings["mongoconnect"];
                IMongoDatabase database;
                IMongoClient client;
                client = new MongoClient(str);
                database = client.GetDatabase("eSocietyAadharVault");
                var collection = database.GetCollection<Aadhar>("aadhjarVault");
                var status = collection.Find(x => x._Id == getObjID(memberid)).FirstOrDefault();
                aadhar = status.Doc_ID;
            }
            catch (Exception ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(HttpContext.Current.Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "getAadharNo()----Utility.cs");
                aadhar = "";
            }
            return aadhar;
        }
        public static int VirusScanFile(string fileName)
        {
            Process virusScanProcess = new Process();
            try
            {
                string filePath = fileName;
                virusScanProcess.StartInfo.UseShellExecute = false;
                virusScanProcess.StartInfo.RedirectStandardOutput = true;

                virusScanProcess.StartInfo.FileName = "C:\\Program Files (x86)\\Trend Micro\\Security Agent\\Ntrtscan.exe";
                virusScanProcess.StartInfo.Arguments = " -Scan -ScanType 3 -file " + filePath + " -DisableRemediation";
                virusScanProcess.Start();
                string output = virusScanProcess.StandardOutput.ReadToEnd();
                virusScanProcess.WaitForExit();
                return virusScanProcess.ExitCode;
            }
            catch (Exception ex)
            {
                return -1;
            }
            finally
            {
                virusScanProcess.Close();
            }
        }
        public static string Encryptdata(string password)
        {
            string strmsg = string.Empty;
            try
            {
                byte[] encode = new byte[password.Length];
                encode = Encoding.UTF8.GetBytes(password);
                strmsg = Convert.ToBase64String(encode);
            }
            catch (Exception ex)
            {
                strmsg = "";
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(HttpContext.Current.Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "Encryptdata()---Utility.cs");
            }

            return strmsg;
        }
        public static Int64 get_EmployeeID()
        {
            NpgsqlConnection conn = new NpgsqlConnection();
            NpgsqlCommand cmd = new NpgsqlCommand();
            conn.ConnectionString = ConfigurationManager.ConnectionStrings["connect"].ConnectionString;
            cmd.Connection = conn;
            string query;
            try
            {
                conn.Open();
                string value = "";
                cmd.Parameters.Clear();
                query = "select nextval('esociety.employee_id')";
                cmd.CommandText = query;
                value = Convert.ToString(cmd.ExecuteScalar());
                int i = Convert.ToInt32(value);
                value = i.ToString("000000");

                string mid = DateTime.Now.Year.ToString() + value;

                Int64 Mem_ID = Convert.ToInt64(mid);
                return Mem_ID;
            }
            catch (Exception ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(HttpContext.Current.Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "get_EmployeeID()----Utility.cs");
                return 0;
            }
            finally
            {
                conn.Close();
            }

        }
        public static string GetMACAddress(string IPAddr)
        {

            try
            {
                IPAddress IP = IPAddress.Parse(IPAddr);

                NetworkInterface[] nics = NetworkInterface.GetAllNetworkInterfaces();
                String sMacAddress = string.Empty;
                foreach (NetworkInterface adapter in nics)
                {
                    if (sMacAddress == String.Empty)// only return MAC Address from first card
                    {
                        IPInterfaceProperties properties = adapter.GetIPProperties();
                        sMacAddress = adapter.GetPhysicalAddress().ToString();
                    }
                }
                return sMacAddress;
            }
            catch (Exception ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(HttpContext.Current.Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "GetMACAddress()---Utility.cs");
                return null;
            }
        }
        public static string getloginNew()
        {
            NpgsqlConnection conn = new NpgsqlConnection();
            NpgsqlCommand cmd = new NpgsqlCommand();
            conn.ConnectionString = ConfigurationManager.ConnectionStrings["connect"].ConnectionString;
            cmd.Connection = conn;
            try
            {
                string value = "";
                conn.Open();
                cmd.Parameters.Clear();
                string query = "select nextval('esociety.login_id')";
                cmd.CommandText = query;
                value = Convert.ToString(cmd.ExecuteScalar());
                int i = Convert.ToInt32(value);
                //value = i.ToString("0000");
                string loginid_temp = "N" + value + DateTime.Now.ToString("HHmmMM");
                return loginid_temp;
            }
            catch (NpgsqlException ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(HttpContext.Current.Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "getloginid()----Utility.cs");
                return "0";
            }
            finally
            {
                conn.Close();
            }
        }
        public static string getloginrenewal()
        {
            NpgsqlConnection conn = new NpgsqlConnection();
            NpgsqlCommand cmd = new NpgsqlCommand();
            conn.ConnectionString = ConfigurationManager.ConnectionStrings["connect"].ConnectionString;
            cmd.Connection = conn;
            try
            {
                string value = "";
                conn.Open();
                cmd.Parameters.Clear();
                string query = "select nextval('esociety.login_id')";
                cmd.CommandText = query;
                value = Convert.ToString(cmd.ExecuteScalar());
                int i = Convert.ToInt32(value);
                //value = i.ToString("0000");
                string loginid_temp = "R" + value + DateTime.Now.ToString("HHmmMM");
                return loginid_temp;
            }
            catch (NpgsqlException ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(HttpContext.Current.Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "getloginid()----Utility.cs");
                return "0";
            }
            finally
            {
                conn.Close();
            }
        }
        public static int checkMembersList(string app_id)
        {
            int a = 0;
            NpgsqlConnection conn = new NpgsqlConnection();
            NpgsqlCommand cmd = new NpgsqlCommand();
            conn.ConnectionString = ConfigurationManager.ConnectionStrings["connect"].ConnectionString;
            cmd.Connection = conn;
            try
            {
                conn.Open();
                string query = "SELECT count(*) as intcount	FROM esociety.members where (design = 1 or design = 3 or design = 5) and app_id=@app_id";
                cmd.CommandText = query;
                cmd.Parameters.AddWithValue("@app_id", Convert.ToInt64(app_id));
                NpgsqlDataReader rd = cmd.ExecuteReader();
                if (rd.Read())
                {
                    string temp = rd["intcount"].ToString();
                    a = Convert.ToInt16(temp);
                }
                rd.Close();
            }
            catch (NpgsqlException ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(HttpContext.Current.Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "checkMembersList()----Utility.cs");
                a = -1;
            }
            finally
            {
                conn.Close();
            }
            return a;
        }
        public static int checkMembersListgov(string app_id)
        {
            int a = 0;
            NpgsqlConnection conn = new NpgsqlConnection();
            NpgsqlCommand cmd = new NpgsqlCommand();
            conn.ConnectionString = ConfigurationManager.ConnectionStrings["connect"].ConnectionString;
            cmd.Connection = conn;
            try
            {
                conn.Open();
                string query = "SELECT count(*) as intcount	FROM esociety.members where (design = 1 or design = 3) and app_id=@app_id";
                cmd.CommandText = query;
                cmd.Parameters.AddWithValue("@app_id", Convert.ToInt64(app_id));
                NpgsqlDataReader rd = cmd.ExecuteReader();
                if (rd.Read())
                {
                    string temp = rd["intcount"].ToString();
                    a = Convert.ToInt16(temp);
                }
                rd.Close();
            }
            catch (NpgsqlException ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(HttpContext.Current.Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "checkMembersList()----Utility.cs");
                a = -1;
            }
            finally
            {
                conn.Close();
            }
            return a;
        }
        public static int get_districtid(string app_id)
        {
            int disid = 0;
            NpgsqlConnection conn = new NpgsqlConnection();
            NpgsqlCommand cmd = new NpgsqlCommand();
            conn.ConnectionString = ConfigurationManager.ConnectionStrings["connect"].ConnectionString;
            cmd.Connection = conn;
            string query;
            try
            {
                conn.Open();
                query = "SELECT socdistrict	FROM esociety.society where app_id = @app_id";
                cmd.CommandText = query;
                cmd.Parameters.AddWithValue("@app_id", Convert.ToInt64(app_id));
                NpgsqlDataReader rd = cmd.ExecuteReader();
                if (rd.Read())
                {

                    string temp = rd["socdistrict"].ToString();
                    disid = Convert.ToInt32(temp);
                }
                rd.Close();
            }
            catch (Exception ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(HttpContext.Current.Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "get_districtid()----Utility.cs");
                disid = 0;
            }
            finally
            {
                conn.Close();
            }

            return disid;

        }

        //conn.Open();
        //string value = "";
        //cmd.Parameters.Clear();
        //query = "select nextval('esociety.reg_id')";
        //cmd.CommandText = query;
        //value = Convert.ToString(cmd.ExecuteScalar());
        //int i = Convert.ToInt32(value);
        //string docid = value+"/GOA/"+DateTime.Now.Year.ToString();


        //reg_id
        public static string getRegistrationID(int districtid)
        {
            string regid = "";
            NpgsqlConnection conn = new NpgsqlConnection();
            NpgsqlCommand cmd = new NpgsqlCommand();
            conn.ConnectionString = ConfigurationManager.ConnectionStrings["connect"].ConnectionString;
            cmd.Connection = conn;
            string query;
            conn.Open();
            NpgsqlTransaction trans = conn.BeginTransaction();
            try
            {
                cmd.Parameters.Clear();
                query = "SELECT soc_reg_no FROM esociety.mst_sequence_master where districtid = @districtid";
                cmd.CommandText = query;
                cmd.Parameters.AddWithValue("@districtid", districtid);
                NpgsqlDataReader rd = cmd.ExecuteReader();
                if (rd.Read())
                {
                    regid = rd["soc_reg_no"].ToString();
                    string temp = regid;
                    rd.Close();
                    regid = regid + "/GOA/" + DateTime.Now.Year.ToString();
                    //cmd.Parameters.Clear();
                    //string upquery = "UPDATE esociety.mst_sequence_master SET soc_reg_no = @soc_reg_no WHERE districtid = @districtid";
                    //cmd.CommandText = upquery;
                    //cmd.Parameters.AddWithValue("@soc_reg_no", temp);
                    //cmd.Parameters.AddWithValue("@districtid", districtid);
                    //cmd.ExecuteNonQuery();
                    //cmd.Parameters.Clear();
                    trans.Commit();
                }
                else
                {
                    rd.Close();
                    trans.Rollback();
                    regid = "0";
                }
            }
            catch (Exception ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(HttpContext.Current.Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "getRegistrationID()----Utility.cs");
                regid = "0";
            }
            finally
            {
                conn.Close();
            }
            return regid;
        }
        public static Int64 get_ApplicationID()
        {
            NpgsqlConnection conn = new NpgsqlConnection();
            NpgsqlCommand cmd = new NpgsqlCommand();
            conn.ConnectionString = ConfigurationManager.ConnectionStrings["connect"].ConnectionString;
            cmd.Connection = conn;
            string query;
            try
            {
                conn.Open();
                string value = "";
                cmd.Parameters.Clear();
                query = "select nextval('esociety.app_id')";
                cmd.CommandText = query;
                value = Convert.ToString(cmd.ExecuteScalar());
                //int i = Convert.ToInt32(value);
                //value = i.ToString("000000");
                string time = DateTime.Now.ToString("MMddHHmm");
                string app_id = DateTime.Now.Year.ToString() + value + time;

                Int64 App_ID = Convert.ToInt64(app_id);
                return App_ID;
            }
            catch (Exception ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(HttpContext.Current.Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "get_ApplicationID()----Utility.cs");
                return 0;
            }
            finally
            {
                conn.Close();
            }
        }
        public static Int64 get_memberID()
        {
            NpgsqlConnection conn = new NpgsqlConnection();
            NpgsqlCommand cmd = new NpgsqlCommand();
            conn.ConnectionString = ConfigurationManager.ConnectionStrings["connect"].ConnectionString;
            cmd.Connection = conn;
            string query;
            try
            {
                conn.Open();
                string value = "";
                cmd.Parameters.Clear();
                query = "select nextval('esociety.member_id')";
                cmd.CommandText = query;
                value = Convert.ToString(cmd.ExecuteScalar());
                int i = Convert.ToInt32(value);
                value = i.ToString("0000");

                string mid = DateTime.Now.Year.ToString() + value;

                Int64 Mem_ID = Convert.ToInt64(mid);
                return Mem_ID;
            }
            catch (Exception ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(HttpContext.Current.Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "get_memberID()----Utility.cs");
                return 0;
            }
            finally
            {
                conn.Close();
            }
        }

        public static string get_docID()
        {
            NpgsqlConnection conn = new NpgsqlConnection();
            NpgsqlCommand cmd = new NpgsqlCommand();
            conn.ConnectionString = ConfigurationManager.ConnectionStrings["connect"].ConnectionString;
            cmd.Connection = conn;
            string query;
            try
            {
                conn.Open();
                string value = "";
                cmd.Parameters.Clear();
                query = "select nextval('esociety.doc_id')";
                cmd.CommandText = query;
                value = Convert.ToString(cmd.ExecuteScalar());
                int i = Convert.ToInt32(value);
                value = i.ToString("0000");
                string docid = "D" + DateTime.Now.Year.ToString() + value;
                return docid;
            }
            catch (Exception ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(HttpContext.Current.Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "get_docID()----Utility.cs");
                return "0";
            }
            finally
            {
                conn.Close();
            }
        }

        public static string getFinalCertificateUploadID()
        {
            NpgsqlConnection conn = new NpgsqlConnection();
            NpgsqlCommand cmd = new NpgsqlCommand();
            conn.ConnectionString = ConfigurationManager.ConnectionStrings["connect"].ConnectionString;
            cmd.Connection = conn;
            string query;
            try
            {
                conn.Open();
                string value = "";
                cmd.Parameters.Clear();
                query = "select nextval('esociety.finalcertificateuploadid')";
                cmd.CommandText = query;
                value = Convert.ToString(cmd.ExecuteScalar());
                int i = Convert.ToInt32(value);
                value = i.ToString("00");
                string docid = "Soc" + DateTime.Now.Year.ToString() + value;
                return docid;
            }
            catch (Exception ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(HttpContext.Current.Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "getFinalCertificateUploadID()----Utility.cs");
                return "0";
            }
            finally
            {
                conn.Close();
            }
        }
        public static string getEchallanNo(string app_id, int payid)
        {
            string echallan = null;
            NpgsqlConnection conn = new NpgsqlConnection();
            NpgsqlCommand cmd = new NpgsqlCommand();
            conn.ConnectionString = ConfigurationManager.ConnectionStrings["connect"].ConnectionString;
            cmd.Connection = conn;
            try
            {
                conn.Open();
                string query = "SELECT echallan_no FROM esociety.online_payment_details where app_id=@app_id and status = 'S' and active='Y' and onlinepayment_id = @payid";
                cmd.CommandText = query;
                cmd.Parameters.AddWithValue("@app_id", Convert.ToInt64(app_id));
                cmd.Parameters.AddWithValue("@payid", payid);
                NpgsqlDataReader rd = cmd.ExecuteReader();
                if (rd.Read())
                {
                    echallan = rd["echallan_no"].ToString();
                }
                else
                {
                    echallan = null;
                }
                rd.Close();
            }
            catch (NpgsqlException ex)
            {
                echallan = null;
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(HttpContext.Current.Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "Update()---BaseUtility.cs");
            }
            finally
            {
                conn.Close();
            }
            return echallan;
        }

        public static string get_OtherDocsID()
        {
            NpgsqlConnection conn = new NpgsqlConnection();
            NpgsqlCommand cmd = new NpgsqlCommand();
            conn.ConnectionString = ConfigurationManager.ConnectionStrings["connect"].ConnectionString;
            cmd.Connection = conn;
            string query;
            try
            {
                conn.Open();
                string value = "";
                cmd.Parameters.Clear();
                query = "select nextval('esociety.other_doc')";
                cmd.CommandText = query;
                value = Convert.ToString(cmd.ExecuteScalar());
                int i = Convert.ToInt32(value);
                value = i.ToString("0000");
                string docid = "OD" + DateTime.Now.Year.ToString() + value;
                return docid;
            }
            catch (Exception ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(HttpContext.Current.Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "get_OtherDocsID()----Utility.cs");
                return "0";
            }
            finally
            {
                conn.Close();
            }
        }
        public static string get_echallan_receiptDocsID()
        {
            NpgsqlConnection conn = new NpgsqlConnection();
            NpgsqlCommand cmd = new NpgsqlCommand();
            conn.ConnectionString = ConfigurationManager.ConnectionStrings["connect"].ConnectionString;
            cmd.Connection = conn;
            string query;
            try
            {
                conn.Open();
                string value = "";
                cmd.Parameters.Clear();
                query = "select nextval('esociety.challanreceiptdocno')";
                cmd.CommandText = query;
                value = Convert.ToString(cmd.ExecuteScalar());
                int i = Convert.ToInt32(value);
                value = i.ToString("0000");
                string docid = "RCPT" + DateTime.Now.Year.ToString() + value;
                return docid;
            }
            catch (Exception ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(HttpContext.Current.Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "get_echallan_receiptDocsID()----Utility.cs");
                return "0";
            }
            finally
            {
                conn.Close();
            }
        }
        public static string get_echallan_pdfDocsID()
        {
            NpgsqlConnection conn = new NpgsqlConnection();
            NpgsqlCommand cmd = new NpgsqlCommand();
            conn.ConnectionString = ConfigurationManager.ConnectionStrings["connect"].ConnectionString;
            cmd.Connection = conn;
            string query;
            try
            {
                conn.Open();
                string value = "";
                cmd.Parameters.Clear();
                query = "select nextval('esociety.challanpdfdocno')";
                cmd.CommandText = query;
                value = Convert.ToString(cmd.ExecuteScalar());
                int i = Convert.ToInt32(value);
                value = i.ToString("0000");
                string docid = "PDF" + DateTime.Now.Year.ToString() + value;
                return docid;
            }
            catch (Exception ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(HttpContext.Current.Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "get_echallan_pdfDocsID()----Utility.cs");
                return "0";
            }
            finally
            {
                conn.Close();
            }
        }

        public static int GenerateRandomInt(int minVal = 10000000, int maxVal = 99999999)
        {
            var rnd = new byte[4];
            using (var rng = new RNGCryptoServiceProvider())
                rng.GetBytes(rnd);
            var i = Math.Abs(BitConverter.ToInt32(rnd, 0));
            return Convert.ToInt32(i % (maxVal - minVal + 1) + minVal);
        }

        public static string GenerateRandomString(int length, string allowableChars = null)
        {
            try
            {

                if (string.IsNullOrEmpty(allowableChars))
                    allowableChars = @"ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

                // Generate random data
                var rnd = new byte[length];
                using (var rng = new RNGCryptoServiceProvider())
                    rng.GetBytes(rnd);

                // Generate the output string
                var allowable = allowableChars.ToCharArray();
                var l = allowable.Length;
                var chars = new char[length];
                for (var i = 0; i < length; i++)
                    chars[i] = allowable[rnd[i] % l];
                return new string(chars);
            }
            catch (Exception ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(HttpContext.Current.Server.MapPath("~/Logs/ErrorLog"), ex.Message, "GenerateRandomString()---Utility.cs");
                return null;
            }
        }

        public static string MaskMobile(string docno, int startIndex, string mask)
        {
            string result = docno;
            try
            {
                if (string.IsNullOrEmpty(docno))
                    return string.Empty;


                int starLength = mask.Length;


                if (docno.Length >= startIndex)
                {
                    result = docno.Insert(startIndex, mask);
                    if (result.Length >= (startIndex + starLength * 2))
                        result = result.Remove((startIndex + starLength), starLength);
                    else
                        result = result.Remove((startIndex + starLength), result.Length - (startIndex + starLength));

                }
            }
            catch (Exception ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(HttpContext.Current.Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "MaskDocNo()---Utility.cs");
            }

            return result;
        }
        public static int logout_user(Int64 loginsessionid)
        {
            NpgsqlConnection conn = new NpgsqlConnection();
            NpgsqlCommand cmd = new NpgsqlCommand();
            conn.ConnectionString = ConfigurationManager.ConnectionStrings["connect"].ConnectionString;
            cmd.Connection = conn;
            try
            {
                conn.Open();
                string query = "UPDATE esociety.loginentries_user	SET  user_logouttime=current_timestamp	WHERE login_sess_count=@sessionloginid";
                cmd.CommandText = query;
                cmd.Parameters.AddWithValue("@sessionloginid", loginsessionid);
                cmd.ExecuteNonQuery();
                return 1;
            }
            catch (NpgsqlException ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(HttpContext.Current.Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "logout()----Utility.cs");
                return 0;
            }
            finally
            {
                conn.Close();
            }
        }
        public static int logout(Int64 loginsessionid)
        {
            NpgsqlConnection conn = new NpgsqlConnection();
            NpgsqlCommand cmd = new NpgsqlCommand();
            conn.ConnectionString = ConfigurationManager.ConnectionStrings["connect"].ConnectionString;
            cmd.Connection = conn;
            try
            {
                conn.Open();
                string query = "UPDATE esociety.loginentries	SET  user_logouttime=current_timestamp	WHERE login_sess_count=@sessionloginid";
                cmd.CommandText = query;
                cmd.Parameters.AddWithValue("@sessionloginid", loginsessionid);
                cmd.ExecuteNonQuery();
                return 1;
            }
            catch (NpgsqlException ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(HttpContext.Current.Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "logout()----Utility.cs");
                return 0;
            }
            finally
            {
                conn.Close();
            }
        }

        /**Sunidhi**/
        public static int getObsCount(string app_id)
        {
            int value = -1;
            NpgsqlConnection conn = new NpgsqlConnection();
            NpgsqlCommand cmd = new NpgsqlCommand();
            conn.ConnectionString = ConfigurationManager.ConnectionStrings["connect"].ConnectionString;
            cmd.Connection = conn;
            try
            {
                conn.Open();
                string query = "SELECT obs_count FROM esociety.temp_table where app_id = @app_id";
                cmd.CommandText = query;
                cmd.Parameters.AddWithValue("@app_id", Convert.ToInt64(app_id));
                NpgsqlDataReader rd = cmd.ExecuteReader();
                if (rd.Read())
                {
                    value = Convert.ToInt16(rd["obs_count"].ToString());
                }
                else
                {
                    value = -1;
                }
                rd.Close();
            }
            catch (NpgsqlException ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(HttpContext.Current.Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "getObsCount()----Utility.cs");
                value = -1;
            }
            finally
            {
                conn.Close();
            }
            return value;
        }

        public static int checkifrenewal(string app_id)
        {
            int value = -1;
            NpgsqlConnection conn = new NpgsqlConnection();
            NpgsqlCommand cmd = new NpgsqlCommand();
            conn.ConnectionString = ConfigurationManager.ConnectionStrings["connect"].ConnectionString;
            cmd.Connection = conn;
            try
            {
                conn.Open();
                string query = "SELECT new_or_renewal,old_socregid FROM esociety.applicant_details where app_id = @app_id";
                cmd.CommandText = query;
                cmd.Parameters.AddWithValue("@app_id", Convert.ToInt64(app_id));
                NpgsqlDataReader rd = cmd.ExecuteReader();
                if (rd.Read())
                {
                    value = Convert.ToInt32(rd["new_or_renewal"]);

                }
                else
                {
                    value = -1;
                }
                rd.Close();
            }
            catch (NpgsqlException ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(HttpContext.Current.Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "checkifrenewal()----Utility.cs");
                value = -1;
            }
            finally
            {
                conn.Close();
            }
            return value;
        }

        public static string getOldRegistrationNo(string app_id)
        {
            string temp = "";
            NpgsqlConnection conn = new NpgsqlConnection();
            NpgsqlCommand cmd = new NpgsqlCommand();
            conn.ConnectionString = ConfigurationManager.ConnectionStrings["connect"].ConnectionString;
            cmd.Connection = conn;
            try
            {
                conn.Open();
                string query = "SELECT old_socregid, old_socdistrict FROM esociety.applicant_details where app_id = @app_id";
                cmd.CommandText = query;
                cmd.Parameters.AddWithValue("@app_id", Convert.ToInt64(app_id));
                NpgsqlDataReader rd = cmd.ExecuteReader();
                if (rd.Read())
                {
                    temp = rd["old_socregid"].ToString() + '|' + rd["old_socdistrict"].ToString();


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
                Err.ErrorLog(HttpContext.Current.Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "getOldRegistrationNo()----Utility.cs");
                temp = "";
            }
            finally
            {
                conn.Close();
            }
            return temp;
        }


        public static string getUserLoginID(string appid)
        {
            string temp = "";
            if (appid != null)
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
                    string querys = "select login_id from esociety.applicant_details where app_id = @appid";
                    cmd.CommandText = querys;
                    cmd.Parameters.AddWithValue("@appid", Convert.ToInt64(appid));
                    NpgsqlDataReader rd = cmd.ExecuteReader();
                    if (rd.Read())
                    {
                        temp = rd["login_id"].ToString();
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
                    Err.ErrorLog(HttpContext.Current.Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "getUserLoginID()----Utility.cs");
                    //RecordUserAction("getmobileno", ex.Message, "F");
                    temp = "";
                }
                finally
                {
                    conn.Close();
                }
            }

            return temp;
        }


        public static void FillDistrictSoc(DropDownList ddldistrict)
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
                string query = "SELECT \"DistrictName\", \"DistrictID\" FROM esociety.mst_district where \"DistrictID\" != 3";
                cmd.CommandText = query;
                adapter = new NpgsqlDataAdapter(cmd);
                ds = new DataSet();
                adapter.Fill(ds, "esociety.mst_district");
                ddldistrict.DataSource = ds.Tables[0];
                ddldistrict.DataTextField = "DistrictName";
                ddldistrict.DataValueField = "DistrictID";
                ddldistrict.DataBind();
                ddldistrict.Items.Insert(0, new ListItem("-- Select --", "-1"));
                cmd.Dispose();
                adapter.Dispose();
                ds = null;
            }
            catch (NpgsqlException ex)
            {

                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(HttpContext.Current.Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "FillDistrictSoc()----Utility.cs");

            }
            finally
            {
                connect.Close();
            }
        }

        public static void FillTaluka(DropDownList ddltaluka, int DistrictID)
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
                string query = "SELECT \"TalukaName\",\"TalukaID\" FROM esociety.mst_taluka where \"DistrictID\"=@DistrictID";
                cmd.CommandText = query;
                cmd.Parameters.AddWithValue("@DistrictID", DistrictID);
                adapter = new NpgsqlDataAdapter(cmd);
                ds = new DataSet();
                adapter.Fill(ds, "esociety.mst_taluka");
                ddltaluka.DataSource = ds.Tables[0];
                ddltaluka.DataTextField = "TalukaName";
                ddltaluka.DataValueField = "TalukaID";
                ddltaluka.DataBind();
                ddltaluka.Items.Insert(0, new ListItem("-- Select --", "-1"));
                cmd.Dispose();
                adapter.Dispose();
                ds = null;
            }
            catch (NpgsqlException ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(HttpContext.Current.Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "FillTaluka()----Utility.cs");

            }
            finally
            {
                connect.Close();
            }
        }


        public static string getAdminName(string useremail)
        {
            string name = "";
            if (useremail != null)
            {
                NpgsqlConnection conn = new NpgsqlConnection();
                NpgsqlCommand cmd = new NpgsqlCommand();
                conn.ConnectionString = ConfigurationManager.ConnectionStrings["connect"].ConnectionString;
                cmd.Connection = conn;
                conn.Open();
                try
                {
                    string querys = "select concat(userfirstname,' ',usermiddlename,' ',userlastname) as adminname from esociety.admin_table where username = @username";
                    cmd.CommandText = querys;
                    cmd.Parameters.AddWithValue("@username", useremail);
                    NpgsqlDataReader rd = cmd.ExecuteReader();
                    if (rd.Read())
                    {
                        name = rd["adminname"].ToString();
                    }
                    else
                    {
                        name = "";
                    }
                    rd.Close();
                }
                catch (NpgsqlException ex)
                {
                    CreateLogFiles Err = new CreateLogFiles();
                    Err.ErrorLog(HttpContext.Current.Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "getAdminName()----Utility.cs");
                    name = "";
                }
                finally
                {
                    conn.Close();
                }
            }
            return name;
        }


        public static int GetMonthDifference(DateTime startDate, DateTime endDate)
        {
            int month = 0;
            try
            {
                System.Globalization.CultureInfo french = new System.Globalization.CultureInfo("fr-FR");

                //DateTime endDate = Convert.ToDateTime(DateTime.Today, french).Date;
                int monthsApart = 12 * (startDate.Year - endDate.Year) + startDate.Month - endDate.Month;
                month = Math.Abs(monthsApart);
            }
            catch (Exception ex)
            {
                month = -1;
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(HttpContext.Current.Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "GetMonthDifference()----Utility.cs");
            }
            return month;
        }


        public static string getTokenID(string app_id)
        {
            string token = "";
            NpgsqlConnection conn = new NpgsqlConnection();
            NpgsqlCommand cmd = new NpgsqlCommand();
            conn.ConnectionString = ConfigurationManager.ConnectionStrings["connect"].ConnectionString;
            cmd.Connection = conn;
            try
            {
                conn.Open();
                string query = "SELECT login_id FROM esociety.applicant_details where app_id = @app_id";
                cmd.CommandText = query;
                cmd.Parameters.AddWithValue("@app_id", Convert.ToInt64(app_id));
                NpgsqlDataReader rd = cmd.ExecuteReader();
                if (rd.Read())
                {
                    token = rd["login_id"].ToString();
                }
                else
                {
                    token = "";
                }
                rd.Close();
            }
            catch (NpgsqlException ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(HttpContext.Current.Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "getTokenID()----Utility.cs");
                token = "";
            }
            finally
            {
                conn.Close();
            }
            return token;
        }
        public static string getAppID(string tokenID)
        {
            string app_id = "";
            NpgsqlConnection conn = new NpgsqlConnection();
            NpgsqlCommand cmd = new NpgsqlCommand();
            conn.ConnectionString = ConfigurationManager.ConnectionStrings["connect"].ConnectionString;
            cmd.Connection = conn;
            try
            {
                conn.Open();
                string query = "SELECT app_id FROM esociety.applicant_details where login_id = @login_id";
                cmd.CommandText = query;
                cmd.Parameters.AddWithValue("@login_id", tokenID);
                NpgsqlDataReader rd = cmd.ExecuteReader();
                if (rd.Read())
                {
                    app_id = rd["app_id"].ToString();
                }
                else
                {
                    app_id = "";
                }
                rd.Close();
            }
            catch (NpgsqlException ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(HttpContext.Current.Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "getAppID()----Utility.cs");
                app_id = "";
            }
            finally
            {
                conn.Close();
            }
            return app_id;
        }
        public static int getGovernmentSociety(string app_id)
        {
            int value = 0;
            NpgsqlConnection conn = new NpgsqlConnection();
            NpgsqlCommand cmd = new NpgsqlCommand();
            conn.ConnectionString = ConfigurationManager.ConnectionStrings["connect"].ConnectionString;
            cmd.Connection = conn;
            try
            {
                conn.Open();
                string query = "SELECT is_gov_society FROM esociety.applicant_details where app_id = @app_id";
                cmd.CommandText = query;
                cmd.Parameters.AddWithValue("@app_id", Convert.ToInt64(app_id));
                NpgsqlDataReader rd = cmd.ExecuteReader();
                if (rd.Read())
                {
                    string temp = rd["is_gov_society"].ToString();
                    value = Convert.ToInt16(temp);
                }
                else
                {
                    value = -1;
                }
                rd.Close();
            }
            catch (NpgsqlException ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(HttpContext.Current.Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "getGovernmentSociety()----Utility.cs");
                value = -2;
            }
            finally
            {
                conn.Close();
            }
            return value;
        }

        public static string getTaluka(int talukaid)
        {
            string taluka = "";
            NpgsqlConnection conn = new NpgsqlConnection();
            NpgsqlCommand cmd = new NpgsqlCommand();
            conn.ConnectionString = ConfigurationManager.ConnectionStrings["connect"].ConnectionString;
            cmd.Connection = conn;
            try
            {
                conn.Open();
                string query = "SELECT \"TalukaName\" FROM esociety.mst_taluka where \"TalukaID\" = @talukaid";
                cmd.CommandText = query;
                cmd.Parameters.AddWithValue("@talukaid", talukaid);
                NpgsqlDataReader rd = cmd.ExecuteReader();
                if (rd.Read())
                {
                    taluka = rd["TalukaName"].ToString();
                }
                else
                {
                    taluka = "";
                }
                rd.Close();
            }
            catch (NpgsqlException ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(HttpContext.Current.Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "getTaluka()----Utility.cs");
                taluka = "";
            }
            finally
            {
                conn.Close();
            }
            return taluka;
        }


        public static string filesave(FileUpload fu,string app_id)
        {
            string path = "";
            string fileExtention = System.IO.Path.GetExtension(fu.PostedFile.FileName);
            string fileName = app_id + ".pdf";
            string folderPath = "~/OutData/ScanPdf/";
            path = HttpContext.Current.Server.MapPath(folderPath + fileName);
            fu.SaveAs(HttpContext.Current.Server.MapPath(folderPath + fileName));            
            return path;
        }
        
        public static bool getPasswordReset(string username)
        {
            bool value = false;
            NpgsqlConnection conn = new NpgsqlConnection();
            NpgsqlCommand cmd = new NpgsqlCommand();
            conn.ConnectionString = ConfigurationManager.ConnectionStrings["connect"].ConnectionString;
            cmd.Connection = conn;
            try
            {
                conn.Open();
                string qurey = "SELECT reset_password FROM esociety.admin_table where username=@username";
                cmd.CommandText = qurey;
                cmd.Parameters.AddWithValue("@username", username);
                NpgsqlDataReader rd = cmd.ExecuteReader();
                if (rd.Read())
                {
                    value =  Convert.ToBoolean(rd["reset_password"].ToString());
                }
            }
            catch (NpgsqlException ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(HttpContext.Current.Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "getTaluka()----Utility.cs");
                value = false;
            }
            finally
            {
                conn.Close();
            }
            return value;
        }


        public static int save_card_data(CardData card, int districtid)
        {
            CreateLogFiles Err = new CreateLogFiles();
            NpgsqlConnection conn = new NpgsqlConnection();
            NpgsqlCommand cmd = new NpgsqlCommand();
            conn.ConnectionString = ConfigurationManager.ConnectionStrings["connect"].ConnectionString;
            cmd.Connection = conn;
            int value = 0;
            try
            {
                conn.Open();
                string query = "INSERT INTO esociety.digital_sign_officer_verify(dsc_name, dsc_publicketstring, dsc_rawdatastring, dsc_serialnumberstring, dsc_issuer, dsc_notafter,";
                query = query + " dsc_notbefore, dsc_version, dsc_reg_ip, dsc_reg_at,active,districtid, status) VALUES (@dsc_name, @dsc_publicketstring, @dsc_rawdatastring, @dsc_serialnumberstring, @dsc_issuer,";
                query = query + " @dsc_notafter, @dsc_notbefore, @dsc_version, @dsc_reg_ip, CURRENT_TIMESTAMP,'N',@districtid, 1)";
                cmd.CommandText = query;
                cmd.Parameters.AddWithValue("@dsc_name", card.Name);
                cmd.Parameters.AddWithValue("@dsc_publicketstring", card.publicketString);
                cmd.Parameters.AddWithValue("@dsc_rawdatastring", card.rawdataString);
                cmd.Parameters.AddWithValue("@dsc_serialnumberstring", card.serialnumberString);
                cmd.Parameters.AddWithValue("@dsc_issuer", card.Issuer);
                cmd.Parameters.AddWithValue("@dsc_notafter", card.NotAfter);
                cmd.Parameters.AddWithValue("@dsc_notbefore", card.NotBefore);
                cmd.Parameters.AddWithValue("@dsc_version", card.version);
                cmd.Parameters.AddWithValue("@dsc_reg_ip", getIP());
                cmd.Parameters.AddWithValue("@districtid", districtid);
                cmd.ExecuteNonQuery();
                value = 1;// success
            }
            catch (NpgsqlException ex)
            {
                var errorcode = ex.ErrorCode;
                if (errorcode == 23505)
                {
                    value = 2;
                }
                else
                {
                    value = -1;
                    Err.ErrorLog(HttpContext.Current.Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "save_card_data()----Utility.cs");
                }
            }
            finally
            {
                conn.Close();
            }
            return value;
        }

        public static string AESDecrypt(string cipherText, string key)
        {
            try
            {
                RijndaelManaged rijndaelCipher = new RijndaelManaged();
                rijndaelCipher.Mode = CipherMode.CBC;
                rijndaelCipher.Padding = PaddingMode.PKCS7;
                rijndaelCipher.KeySize = 0x80;
                rijndaelCipher.BlockSize = 0x80;
                byte[] encryptedData = Convert.FromBase64String(cipherText);
                byte[] pwdBytes = Encoding.UTF8.GetBytes(key);
                byte[] keyBytes = new byte[0x10];
                int len = pwdBytes.Length;
                if (len > keyBytes.Length)
                {
                    len = keyBytes.Length;
                }
                Array.Copy(pwdBytes, keyBytes, len);
                rijndaelCipher.Key = keyBytes;
                rijndaelCipher.IV = keyBytes;
                ICryptoTransform transform = rijndaelCipher.CreateDecryptor();
                byte[] plainText = transform.TransformFinalBlock(encryptedData, 0, encryptedData.Length);
                return (Encoding.UTF8.GetString(plainText));
            }
            catch(Exception ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(HttpContext.Current.Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "AESDecrypt()----Utility.cs");
                return null;
            }
        }
    }
}