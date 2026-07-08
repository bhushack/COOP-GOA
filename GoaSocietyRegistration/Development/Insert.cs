using GoaSocietyRegistration;
using GoaSocietyRegistration.Development;
using MongoDB.Bson;
using MongoDB.Driver;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WS_Encryption;

namespace GoaSocietyRegistration
{
    public class Insert
    {
        ByteEncryption.ByteEncryption obj_Byte_Encryption = new ByteEncryption.ByteEncryption();
        NICEncryption _encryption = new NICEncryption();
        public int InsertintoMongoDB(MemberDocs doc, string collect)
        {
            var str = ConfigurationManager.AppSettings["mongoconnect"];
            IMongoDatabase database;
            IMongoClient client;
            client = new MongoClient(str);
            try
            {   
                database = client.GetDatabase("eGoaSociety");
                var collection = database.GetCollection<MemberDocs>(collect);
                collection.InsertOne(doc);
                return 1;
            }
            catch (MongoException ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(HttpContext.Current.Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "InsertintoMongoDB()----Insert.cs");
                return 0;
            }

        }

        public int InserFinalSociety(Society_Certificate doc, string collect)
        {
            var str = ConfigurationManager.AppSettings["mongoconnect"];
            IMongoDatabase database;
            IMongoClient client;
            client = new MongoClient(str);
            try
            {
                database = client.GetDatabase("eGoaSociety");

                var collection = database.GetCollection<Society_Certificate>(collect);
                collection.InsertOne(doc);
                return 1;
            }
            catch (Exception ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(HttpContext.Current.Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "InserFinalSociety()----Insert.cs");
                return 0;
            }

        }

        public int InsertMongoOtherDocs(OtherDocuments doc, string collect)
        {
            var str = ConfigurationManager.AppSettings["mongoconnect"];
            IMongoDatabase database;
            IMongoClient client;
            client = new MongoClient(str);
            try
            {
                
                database = client.GetDatabase("eGoaSociety");

                var collection = database.GetCollection<OtherDocuments>(collect);
                collection.InsertOne(doc);
                return 1;
            }
            catch (Exception ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(HttpContext.Current.Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "InsertMongoOtherDocs()----Insert.cs");
                return 0;
            }

        }
        public int InsertMongoEchallanReceipt(EchallanReceipt rcpt, string collect)
        {
            var str = ConfigurationManager.AppSettings["mongoconnect"];
            IMongoDatabase database;
            IMongoClient client;
            client = new MongoClient(str);
            try
            {
                database = client.GetDatabase("eGoaSociety");

                var collection = database.GetCollection<EchallanReceipt>(collect);
                collection.InsertOne(rcpt);
                return 1;
            }
            catch (Exception ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(HttpContext.Current.Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "InsertMongoEchallanReceipt()----Insert.cs");
                return 0;
            }

        }
        public FetchDetails LoadFullData(string appid)
        {
            FetchDetails fulldata = new FetchDetails();
            NpgsqlConnection conn = new NpgsqlConnection();
            NpgsqlCommand cmd = new NpgsqlCommand();
            conn.ConnectionString = ConfigurationManager.ConnectionStrings["connect"].ConnectionString;
            cmd.Connection = conn;
            conn.Open();
            NpgsqlTransaction myTrans = conn.BeginTransaction();
            cmd.Transaction = myTrans;
            try
            {
                string app_query = "select applicant_name,applicant_address,applicant_email,applicant_mobile_no,mst_district.\"DistrictName\",";
                app_query = app_query + " mst_memberdesignation.\"DesignationName\" from esociety.mst_district, esociety.applicant_details, esociety.mst_memberdesignation";
                app_query = app_query + " where mst_district.\"DistrictID\" = applicant_details.applicant_district and ";
                app_query = app_query + " mst_memberdesignation.\"DesignationID\" = applicant_details.applicant_designation and app_id = @appid";
                cmd.CommandText = app_query;
                cmd.Parameters.AddWithValue("@appid", Convert.ToInt64(appid));
                NpgsqlDataReader dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    fulldata.applicant_name = dr["applicant_name"].ToString();
                    fulldata.applicant_address = dr["applicant_address"].ToString();
                    string encrypt_mobile = dr["applicant_mobile_no"].ToString();
                    fulldata.applicant_email = dr["applicant_email"].ToString();
                    fulldata.applicant_mobile_no = Encryption.Encrypt.Decrypt(encrypt_mobile);                   
                    fulldata.districtname = dr["DistrictName"].ToString();
                    fulldata.designationname = dr["DesignationName"].ToString();
                }
                dr.Close();
                cmd.Parameters.Clear();
                string soc_query = "select socname,mst_societytype.societytype,socaddr,mst_district.\"DistrictName\",mst_taluka.\"TalukaName\",regfee,processfee,totalfee,doc_one,doc_two";
                soc_query = soc_query + " from esociety.mst_societytype, esociety.society, esociety.mst_district, esociety.mst_taluka where mst_societytype.societyid = society.soctype and";
                soc_query = soc_query + " mst_district.\"DistrictID\" = society.socdistrict and society.soc_taluka = mst_taluka.\"TalukaID\" and app_id =@appid";
                cmd.CommandText = soc_query;
                cmd.Parameters.AddWithValue("@appid", Convert.ToInt64(appid));
                NpgsqlDataReader rd = cmd.ExecuteReader();
                if (rd.Read())
                {
                    fulldata.societytype = rd["societytype"].ToString();
                    fulldata.socname = rd["socname"].ToString();
                    fulldata.socaddr = rd["socaddr"].ToString();
                    fulldata.soc_dname = rd["DistrictName"].ToString();
                    fulldata.soc_talukaname = rd["TalukaName"].ToString();
                    fulldata.regfee =   rd["regfee"].ToString();
                    fulldata.processfee = rd["processfee"].ToString();
                    fulldata.totalfee =  rd["totalfee"].ToString();
                    fulldata.doc_one = rd["doc_one"].ToString();
                    fulldata.doc_two = rd["doc_two"].ToString();


                

                }
                rd.Close();
                cmd.Parameters.Clear();
                myTrans.Commit();
            }
            catch (NpgsqlException ex)
            {
                myTrans.Rollback();
                fulldata = null;
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(HttpContext.Current.Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "LoadFullData()----Insert.cs");
            }
            finally
            {
                conn.Close();
            }
            return fulldata;
        }
        public int save_status_dh(string observation_text, int status_applicant, string app_id, int appl_status,int districtid, string uname)
        {//save observation by dealing hand
            int value = 0;
            int count = Utility.getObsCount(app_id);
            NpgsqlConnection conn = new NpgsqlConnection();
            NpgsqlCommand cmd = new NpgsqlCommand();
            conn.ConnectionString = ConfigurationManager.ConnectionStrings["connect"].ConnectionString;
            cmd.Connection = conn;
            conn.Open();
            NpgsqlTransaction trans = conn.BeginTransaction();
            try
            {
                string obs_query = "";
                //if (appl_status == 3)
                //{
                //    observation sent to sro first time appl_status == 3 done
                //      obs_query = "UPDATE esociety.temp_table set observationbydhfirsttime=@multipleobservations, submit_time_observation_first_time=current_timestamp where app_id=@app_id";
                //}
                //else if (appl_status == 6)
                //{
                //    appl_status = 6 second time observation saved
                //    obs_query = "UPDATE esociety.remarks_table set observation_dhsecondtime=@multipleobservations, submit_time_obs_dhsecondtime=current_timestamp where app_id=@app_id";
                //}

                obs_query = "INSERT INTO esociety.remarks_table(app_id,login_id,observation_by_dh,submit_time_observationbydh,district_registered_at,updated_by,updated_at,ipaddress,obs_count,updated_by_name) VALUES (@app_id,@login_id,@multipleobservations,current_timestamp,@district_registered_at,@updated_by,current_timestamp,@ipaddress,@obs_count,@updated_by_name)";

                cmd.CommandText = obs_query;
                cmd.Parameters.AddWithValue("@app_id", Convert.ToInt64(app_id));
                cmd.Parameters.AddWithValue("@login_id", Utility.getUserLoginID(app_id));
                cmd.Parameters.AddWithValue("@multipleobservations", observation_text);
                cmd.Parameters.AddWithValue("@district_registered_at", districtid);
                cmd.Parameters.AddWithValue("@obs_count",count);
                cmd.Parameters.AddWithValue("@updated_by", uname);
                cmd.Parameters.AddWithValue("@ipaddress", Utility.getIP());
                cmd.Parameters.AddWithValue("@updated_by_name", Utility.getAdminName(uname));
                cmd.ExecuteNonQuery();
                cmd.Parameters.Clear();
                string status_query = "UPDATE esociety.status_table set status_id=@status_id where app_id=@app_id";              
                cmd.CommandText = status_query;
                cmd.Parameters.AddWithValue("@app_id", Convert.ToInt64(app_id));
                cmd.Parameters.AddWithValue("@status_id", status_applicant);
                cmd.ExecuteNonQuery();
                cmd.Parameters.Clear();
                string update_tmp = "";                
                update_tmp = "UPDATE esociety.temp_table SET observation_by_dh = @multipleobservations WHERE app_id = @app_id";                               
                cmd.CommandText = update_tmp;
                cmd.Parameters.AddWithValue("@multipleobservations", observation_text);
                cmd.Parameters.AddWithValue("@app_id", Convert.ToInt64(app_id));
                cmd.ExecuteNonQuery();
                cmd.Parameters.Clear();

                trans.Commit();
                value = 1;
            }
            catch (NpgsqlException ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(HttpContext.Current.Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "save_status_dh()----Insert.cs");

                trans.Rollback();
                value = 0;
            }
            finally
            {
                conn.Close();
            }
            return value;
        }
        public int checked_by_sro_after_dh(int status_applicant, string app_id, string remarks_text,string uname)
        {//now he chnages status to 5 or 8 or 9 from 4 or 7 (4 and 7 checked by dh and pending to sro)
            int value = 0;
            
            int count = Utility.getObsCount(app_id);
            string submittime = getSubmissionTime(app_id);
            NpgsqlConnection conn = new NpgsqlConnection();
            NpgsqlCommand cmd = new NpgsqlCommand();
            conn.ConnectionString = ConfigurationManager.ConnectionStrings["connect"].ConnectionString;
            cmd.Connection = conn;
            conn.Open();
            NpgsqlTransaction trans = conn.BeginTransaction();
            try
            {
                
                string remarks_query = "";
                if (status_applicant == 5)
                {
                    remarks_query = "UPDATE esociety.remarks_table set remarks_sendobservation=@multipleremarks, submit_time_remarkssendobservation=current_timestamp,";
                   
                    
                }
                else if (status_applicant == 8)
                {
                    remarks_query = "UPDATE esociety.remarks_table set remarks_accepted=@multipleremarks, remarks_accepted_submit_time=current_timestamp,";
                    
                }
                else if (status_applicant == 9)
                {
                    remarks_query = "UPDATE esociety.remarks_table set remarks_rejected=@multipleremarks, remarks_rejected_submit_time=current_timestamp,"; //,updated_by= @updated_by, updated_at=CURRENT_TIMESTAMP, ipaddress= @ipaddress
                   
                }

                remarks_query = remarks_query + " updated_by= @updated_by, updated_at=CURRENT_TIMESTAMP, ipaddress= @ipaddress,application_obs_submission_time_by_user=@submittime,updated_by_name=@updated_by_name where app_id=@app_id and obs_count=@obs_count";
                cmd.CommandText = remarks_query;
                cmd.Parameters.AddWithValue("@app_id", Convert.ToInt64(app_id));
                cmd.Parameters.AddWithValue("@multipleremarks", remarks_text);
                cmd.Parameters.AddWithValue("@obs_count", count);
                cmd.Parameters.AddWithValue("@updated_by", uname);
                cmd.Parameters.AddWithValue("@ipaddress", Utility.getIP());
                cmd.Parameters.AddWithValue("@submittime", Convert.ToDateTime(submittime));
                cmd.Parameters.AddWithValue("@updated_by_name",Utility.getAdminName(uname));

                cmd.ExecuteNonQuery();
                cmd.Parameters.Clear();
                string status_query = "UPDATE esociety.status_table set status_id=@status_id where app_id=@app_id";
                cmd.CommandText = status_query;
                cmd.Parameters.AddWithValue("@app_id", Convert.ToInt64(app_id));
                cmd.Parameters.AddWithValue("@status_id", status_applicant);
                cmd.ExecuteNonQuery();
                cmd.Parameters.Clear();

                string update_tmp = "";
                if (status_applicant == 5) 
                {
                    update_tmp = "UPDATE esociety.temp_table SET remarks_sendobservation = @remarks,obs_count = @obs_count WHERE app_id = @app_id";

                }
                else if (status_applicant == 8) // need to confirm obs count
                {
                    update_tmp = "UPDATE esociety.temp_table SET remarks_accepted = @remarks WHERE app_id = @app_id";
                }
                else if (status_applicant == 9) // need to confirm obs count
                {
                    update_tmp = "UPDATE esociety.temp_table SET remarks_rejected = @remarks WHERE app_id = @app_id";
                }
                cmd.CommandText = update_tmp;
                cmd.Parameters.AddWithValue("@remarks", remarks_text);
                cmd.Parameters.AddWithValue("@obs_count", count + 1);
                cmd.Parameters.AddWithValue("@app_id", Convert.ToInt64(app_id));
                cmd.ExecuteNonQuery();
                cmd.Parameters.Clear();

                if(status_applicant == 9)
                {
                    //disabled on 07072025///
                    //string upd_soc = "UPDATE esociety.society SET active='N' WHERE app_id = @app_id";
                    //cmd.CommandText = upd_soc;                   
                    //cmd.Parameters.AddWithValue("@app_id", Convert.ToInt64(app_id));
                    //cmd.ExecuteNonQuery();
                    //cmd.Parameters.Clear();

                    //
                    //latest changes on 07072025 for adding reject in society name//
                    string socname = "";
                    string urej_soc = "SELECT socname FROM esociety.society where active = 'Y' and app_id = @app_id";
                    cmd.CommandText = urej_soc;
                    cmd.Parameters.AddWithValue("@app_id", Convert.ToInt64(app_id));
                    NpgsqlDataReader rd = cmd.ExecuteReader();
                    if (rd.Read())
                    {
                        socname = rd["socname"].ToString();
                        rd.Close();
                        cmd.Parameters.Clear();
                        string upd_query = "UPDATE esociety.society SET socname = @socname, active = 'N' WHERE active = 'Y' and app_id = @app_id";
                        cmd.CommandText = upd_query;
                        socname = socname + " Rejected on : " + DateTime.Now.ToString();
                        cmd.Parameters.AddWithValue("@socname", socname);
                        cmd.Parameters.AddWithValue("@app_id", Convert.ToInt64(app_id));
                        cmd.ExecuteNonQuery();
                        cmd.Parameters.Clear();
                    }
                    else
                    {
                        trans.Rollback();
                    }
                    ///

                }

                trans.Commit();

                value = 1;
            }
            catch (NpgsqlException ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(HttpContext.Current.Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace , "checked_by_sro_after_dh()----Insert.cs");
                value = 0;
                trans.Rollback();
            }
            finally
            {
                conn.Close();
            }
            return value;
        }
       
        public Page_status_Check getPageStatus(string appid)
        {
            if (appid != null)
            {
                NpgsqlConnection conn = new NpgsqlConnection();
                NpgsqlCommand cmd = new NpgsqlCommand();
                conn.ConnectionString = ConfigurationManager.ConnectionStrings["connect"].ConnectionString;
                cmd.Connection = conn;
                Page_status_Check ps = new Page_status_Check();
                try
                {

                    conn.Open();
                    string query = "SELECT status_id FROM esociety.status_table where app_id=@appid";
                    cmd.CommandText = query;
                    cmd.Parameters.AddWithValue("@appid", Convert.ToInt64(appid));
                    NpgsqlDataReader rd = cmd.ExecuteReader();
                    if (rd.Read())
                    {
                        ps.status_id = (int)rd["status_id"];

                    }
                    else
                    {
                        ps = null;
                    }
                    rd.Close();
                }
                catch (NpgsqlException ex)
                {
                    CreateLogFiles Err = new CreateLogFiles();
                    Err.ErrorLog(HttpContext.Current.Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "getPageStatus()--Insert.cs");
                    ps = null;
                }
                finally
                {
                    conn.Close();
                }
                return ps;
            }
            else
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(HttpContext.Current.Server.MapPath("~/Logs/ErrorLog"), "app id null", "getPageStatus()--Insert.cs");
                return null;
            }
        }

        public int getOtherServicesStatus(string appid)
        {
            int status_id=0;

            if (appid != null)
            {
                NpgsqlConnection conn = new NpgsqlConnection();
                NpgsqlCommand cmd = new NpgsqlCommand();
                conn.ConnectionString = ConfigurationManager.ConnectionStrings["connect"].ConnectionString;
                cmd.Connection = conn;              
                try
                {

                    conn.Open();
                    string query = "SELECT amend_status FROM esociety.status_amendment where app_id=@appid";
                    cmd.CommandText = query;
                    cmd.Parameters.AddWithValue("@appid", Convert.ToInt64(appid));
                    NpgsqlDataReader rd = cmd.ExecuteReader();
                    if (rd.Read())
                    {
                        status_id = (int)rd["amend_status"];

                    }
                    else
                    {
                        status_id = -1;
                    }
                    rd.Close();
                }
                catch (NpgsqlException ex)
                {
                    CreateLogFiles Err = new CreateLogFiles();
                    Err.ErrorLog(HttpContext.Current.Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "getOtherServicesStatus()--Insert.cs");
                    status_id = -1;
                }
                finally
                {
                    conn.Close();
                }
               
            }
            else
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(HttpContext.Current.Server.MapPath("~/Logs/ErrorLog"), "app id null", "getOtherServicesStatus()--Insert.cs");
                
            }
            return status_id;
        }


        public Temp_table getRemarksData(string appid) 
        {
            int count = Utility.getObsCount(appid);
            Temp_table temp = new Temp_table();
            NpgsqlConnection conn = new NpgsqlConnection();
            NpgsqlCommand cmd = new NpgsqlCommand();
            conn.ConnectionString = ConfigurationManager.ConnectionStrings["connect"].ConnectionString;
            cmd.Connection = conn;
            try
            {
                conn.Open();
                string query = "SELECT login_id, observation_by_dh,remarks_sendobservation, remarks_accepted, remarks_rejected";
                query = query + " FROM esociety.temp_table where app_id=@app_id";
                cmd.CommandText = query;
                cmd.Parameters.AddWithValue("@app_id", Convert.ToInt64(appid));
             
                NpgsqlDataReader rd = cmd.ExecuteReader();
                if (rd.Read())
                {
                    temp.observation_by_dh = rd["observation_by_dh"].ToString();                   
                    temp.remarks_sendobservation = rd["remarks_sendobservation"].ToString();
                    temp.remarks_accepted = rd["remarks_accepted"].ToString();
                    temp.remarks_rejected = rd["remarks_rejected"].ToString();
                }
                else
                {
                    
                }
                rd.Close();
            }
            catch (NpgsqlException ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(HttpContext.Current.Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "getRemarksData()---Insert.cs");
                
            }
            finally
            {
                conn.Close();
            }
            return temp;
        }

        protected string getSubmissionTime(string appid)
        {
            string temp = "";
            int count = Utility.getObsCount(appid);
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
                    string querys = "";
                    if (count == 1)
                    {
                        querys = "select application_submission_time from esociety.status_table where app_id = @appid";
                    }
                    else
                    {
                        querys = "select application_obs_submission_time from esociety.status_table where app_id = @appid";
                    }
                    cmd.CommandText = querys;
                    cmd.Parameters.AddWithValue("@appid", Convert.ToInt64(appid));
                    NpgsqlDataReader dr = cmd.ExecuteReader();
                    if (dr.Read())
                    {
                        if (count == 1)
                        {

                            temp = dr["application_submission_time"].ToString();
                        }
                        else
                        {

                            temp = dr["application_obs_submission_time"].ToString();
                        }
                    }
                    else
                    {
                        temp = "";
                    }
                    dr.Close();
                }
                catch (NpgsqlException ex)
                {
                    CreateLogFiles Err = new CreateLogFiles();
                    Err.ErrorLog(HttpContext.Current.Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "getSubmissionTime()---Insert.cs");
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
       
        //public static int UpdateEchallanStatus(OnlinePaymentDetails online_pay)
        //{
        //    int returnvalue;
        //    NpgsqlConnection conn = new NpgsqlConnection();
        //    NpgsqlCommand cmd = new NpgsqlCommand();
        //    conn.ConnectionString = ConfigurationManager.ConnectionStrings["connect"].ConnectionString;
        //    cmd.Connection = conn;
        //    conn.Open();
        //    NpgsqlTransaction trans;
        //    trans = conn.BeginTransaction();
        //    cmd.Transaction = trans;
        //    try
        //    {
        //        cmd.Parameters.Clear();
        //        string echallan_rcpt = Utility.get_echallan_receiptDocsID();//eReceipt doc ID
        //        ObjectId obj_id = ObjectId.GenerateNewId();
        //        string update_challan_status_query = "update esociety.online_payment_details set paystat_response_xml=@paystat_response_xml, status=@status, total_amt=@total_amt,";
        //        update_challan_status_query = update_challan_status_query + " bank_ref_no = @bank_ref_no, bank_rcvd_date=@bank_rcvd_date, treasury_rcvd_date=@treasury_rcvd_date ";
        //        update_challan_status_query = update_challan_status_query + "  where echallan_no = @echallan_no";
        //        cmd.CommandText = update_challan_status_query;
        //        cmd.Parameters.AddWithValue("@paystat_response_xml", online_pay.paystat_response_xml);
        //        cmd.Parameters.AddWithValue("@status", online_pay.status);
        //        cmd.Parameters.AddWithValue("@total_amt", Convert.ToDouble(online_pay.total_amt));
        //        cmd.Parameters.AddWithValue("@bank_ref_no", online_pay.bank_ref_no);
        //        cmd.Parameters.AddWithValue("@bank_rcvd_date", online_pay.bank_rcvd_date);
        //        cmd.Parameters.AddWithValue("@treasury_rcvd_date", online_pay.treasury_rcvd_date);
        //        cmd.Parameters.AddWithValue("@echallan_no", online_pay.echallan_no);
        //        cmd.ExecuteNonQuery();
        //        cmd.Parameters.Clear();
        //        //registration id generation
        //        string socregid = Utility.getRegistrationID();
        //        string query = "update esociety.society set socregid = @socregid, regdate =@regdate where echallan_no = @echallan_no";
        //        cmd.CommandText = query;
        //        cmd.Parameters.AddWithValue("@socregid", socregid);
        //        cmd.Parameters.AddWithValue("@regdate", online_pay.bank_rcvd_date);
        //        cmd.Parameters.AddWithValue("@echallan_no", online_pay.echallan_no);
        //        cmd.ExecuteNonQuery();
        //        cmd.Parameters.Clear();
        //        string query1 = "Update esociety.status_table set status_id=@status_id where echallan_no=@echallan_no";
        //        cmd.CommandText = query1;
        //        cmd.Parameters.AddWithValue("@status_id", 10);
        //        cmd.Parameters.AddWithValue("@echallan_no", online_pay.echallan_no);
        //        cmd.ExecuteNonQuery();
        //        trans.Commit();
        //        returnvalue = 1;
        //    }
        //    catch (NpgsqlException ex)
        //    {
        //        returnvalue = 0;
        //        trans.Rollback();
        //        CreateLogFiles Err = new CreateLogFiles();
        //        Err.ErrorLog(HttpContext.Current.Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "UpdateEchallanStatus()---Insert.cs");

        //    }
        //    finally
        //    {
        //        conn.Close();
        //    }
        //    return returnvalue;
        //}
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
                Err.ErrorLog(HttpContext.Current.Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "SaveAuditTrail()----Insert.cs");
                return 0;
            }
            finally
            {
                conn.Close();
            }

        }
        public int SaveOrganizationAuditTrail(UsersAuditTrails trial)
        {
            NpgsqlConnection conn = new NpgsqlConnection();
            NpgsqlCommand cmd = new NpgsqlCommand();
            conn.ConnectionString = ConfigurationManager.ConnectionStrings["connect"].ConnectionString;
            cmd.Connection = conn;
            try
            {
                conn.Open();
                string query = "INSERT INTO esociety.admin_audit_trail( admin_login_id,browser_session_id, admin_session_id, loggedin_status, referrer, accessed_module,";
                query = query + " action_performed, action_description, action_status, ipaddress, tracked_datetime, browser_name,browser_version,device_type,app_id,is_crud,admin_login_name) VALUES (@admin_login_id,@browser_session_id, ";
                query = query + " @admin_session_id, @loggedin_status, @referrer, @accessed_module, @action_performed, @action_description, @action_status, @ipaddress, ";
                query = query + "  @tracked_datetime,@browser_name,@browser_version,@device_type, @app_id,@is_crud,@admin_login_name)";
                cmd.CommandText = query;
                cmd.Parameters.AddWithValue("@admin_login_id", trial.admin_login_id);
                cmd.Parameters.AddWithValue("@browser_session_id", trial.browser_session_id);
                cmd.Parameters.AddWithValue("@admin_session_id", trial.admin_session_id);
                cmd.Parameters.AddWithValue("@loggedin_status", trial.loggedin_status);
                cmd.Parameters.AddWithValue("@referrer", trial.referrer);
                cmd.Parameters.AddWithValue("@accessed_module", trial.accessed_module);
                cmd.Parameters.AddWithValue("@action_performed", trial.action_performed);
                cmd.Parameters.AddWithValue("@action_description", trial.action_description);
                cmd.Parameters.AddWithValue("@action_status", trial.action_status);
                cmd.Parameters.AddWithValue("@ipaddress", trial.loggedin_ip);
                cmd.Parameters.AddWithValue("@app_id", trial.app_id);
                cmd.Parameters.AddWithValue("@tracked_datetime", trial.tracked_datetime);
                cmd.Parameters.AddWithValue("@browser_name", trial.browser_name);
                cmd.Parameters.AddWithValue("@browser_version", trial.browser_version);
                cmd.Parameters.AddWithValue("@device_type", trial.device_type);
                cmd.Parameters.AddWithValue("@is_crud", trial.is_crud);
                cmd.Parameters.AddWithValue("@admin_login_name", trial.admin_login_name);
                cmd.ExecuteNonQuery();
                return 1;
            }
            catch (NpgsqlException ex)
            { CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(HttpContext.Current.Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "SaveOrganizationAuditTrail()---Insert.cs");
                return 0;
            }
            finally
            {
                conn.Close();
            }

        }
        public Society_Details FetchSociety(string id,int index, int regdistrict) // index 1: id is login_id, index 2: id is regid, index 3: id is app_id  // regdistrict is passed only for old societies
        {
            Society_Details society = new Society_Details();
            NpgsqlConnection conn = new NpgsqlConnection();
            NpgsqlCommand cmd = new NpgsqlCommand();
            conn.ConnectionString = ConfigurationManager.ConnectionStrings["connect"].ConnectionString;
            cmd.Connection = conn;
            conn.Open();
            NpgsqlTransaction myTrans = conn.BeginTransaction();
            cmd.Transaction = myTrans;
            try
            {
                int datafound = 0;
                string fetch_query = "SELECT socname, socaddr, socregid, app_id, regfee, processfee, doc_id, totalmem, created_at, created_by, ipaddress, macaddress,active,";
                fetch_query = fetch_query + "  soc_taluka, socdistrict, soctype, doc_one, doc_two, complete, totalfee, login_id, complete_data,pincode,final_certificate_mongo_entry,regdate from esociety.society";

                if (index == 1)
                {
                    fetch_query = fetch_query + " WHERE login_id=@login_id";
                }
                else if (index == 2)
                {
                    fetch_query = fetch_query + " WHERE socregid=@login_id";
                }
                else if (index == 3)
                {
                    fetch_query = fetch_query + " WHERE app_id=@app_id";
                }
                
                
                cmd.CommandText = fetch_query;

                cmd.Parameters.AddWithValue("@login_id", id);
                if (index == 3)
                {
                    cmd.Parameters.AddWithValue("@app_id", Convert.ToInt64(id));
                }
                NpgsqlDataReader dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    datafound = 1;
                    society.socname = dr["socname"].ToString();
                    society.socaddr = dr["socaddr"].ToString();
                    society.socregid = dr["socregid"].ToString();
                    society.app_id = dr["app_id"].ToString();
                    
                    if (dr["regfee"].ToString() == "" || (dr["regfee"].ToString() == null))
                    {
                        society.regfee = 0;
                    }
                    else
                    {
                        society.regfee = Convert.ToInt32(dr["regfee"].ToString());
                    }
                    if (dr["processfee"].ToString() == "" || (dr["processfee"].ToString() == null))
                    {
                        society.processfee = 0;
                    }
                    else
                    {
                        society.processfee = Convert.ToInt32(dr["processfee"].ToString());
                    }
                    
                    society.doc_id = dr["doc_id"].ToString();
                    society.totalmem = dr["totalmem"].ToString();
                    society.created_at = dr["created_at"].ToString();
                    society.created_by = dr["created_by"].ToString();
                    society.ipaddress = dr["ipaddress"].ToString();
                    society.macaddress = dr["macaddress"].ToString();
                    society.active = dr["active"].ToString();
       
                    
                    
                    if (dr["soc_taluka"].ToString() == "" || (dr["soc_taluka"].ToString() == null))
                    {
                        society.soc_taluka = -1;
                    }
                    else
                    {
                        society.soc_taluka = Convert.ToInt32(dr["soc_taluka"]);
                    }
                    society.socdistrict = Convert.ToInt32(dr["socdistrict"]);
                    if (dr["soctype"].ToString() == "" || (dr["soctype"].ToString() == null))
                    {
                        society.soctype = -1;
                    }
                    else
                    {
                        society.soctype = Convert.ToInt32(dr["soctype"]);
                    }
                    
                    society.doc_one = dr["doc_one"].ToString();
                    society.doc_two = dr["doc_two"].ToString();
                    society.complete = dr["complete"].ToString();
                   
                    if (dr["totalfee"].ToString() == "" || (dr["totalfee"].ToString() == null))
                    {
                        society.totalfee = 0;
                    }
                    else
                    {
                        society.totalfee = Convert.ToInt32(dr["totalfee"].ToString());
                    }
                    society.login_id = dr["login_id"].ToString();
                    society.complete_data = dr["complete_data"].ToString();
                    society.pincode = dr["pincode"].ToString();
                    society.final_certificate_mongo_entry = dr["final_certificate_mongo_entry"].ToString();
                    society.regdate = dr["regdate"].ToString();
                    if (index == 2)
                    {
                        society.tempflag = 1;
                    }


                }
                dr.Close();

                if(datafound == 0 && index == 2)
                {
                    string fetch_query1 = "SELECT socname, socregid, active, socdistrict,socaddr from esociety.society_all";
                    fetch_query1 = fetch_query1 + " WHERE socregid=@socregid and socdistrict=@socdistrict";
                    cmd.CommandText = fetch_query1;

                    cmd.Parameters.AddWithValue("@socregid", id);
                    cmd.Parameters.AddWithValue("@socdistrict", regdistrict);
                    NpgsqlDataReader rd = cmd.ExecuteReader();
                    if (rd.Read())
                    {
                        datafound = 1;
                        society.socname = rd["socname"].ToString();                        
                        society.socregid = rd["socregid"].ToString(); 
                        society.active =  rd["active"].ToString();                        
                        society.socdistrict = Convert.ToInt32(rd["socdistrict"]);
                        society.socaddr = rd["socaddr"].ToString();
                        if (index == 2)
                        {
                            society.tempflag = 2;
                        }

                    }
                    rd.Close();
                }

                if (datafound == 0 && index == 2)
                {
                    string fetch_query2 = "SELECT socname, socregid, active, socdistrict,socaddr from esociety.society_all_north";
                    fetch_query2 = fetch_query2 + " WHERE socregid=@socregid and socdistrict=@socdistrict";
                    cmd.CommandText = fetch_query2;

                    cmd.Parameters.AddWithValue("@socregid", id);
                    cmd.Parameters.AddWithValue("@socdistrict", regdistrict);
                    NpgsqlDataReader rd = cmd.ExecuteReader();
                    if (rd.Read())
                    {
                        datafound = 1;
                        society.socname = rd["socname"].ToString();
                        society.socregid = rd["socregid"].ToString();
                        society.active = rd["active"].ToString();
                        society.socdistrict = Convert.ToInt32(rd["socdistrict"]);
                        society.socaddr = rd["socaddr"].ToString();
                        if (index == 2)
                        {
                            society.tempflag = 3;
                        }

                    }
                    rd.Close();
                }
                myTrans.Commit();

            }
            catch (NpgsqlException ex)
            {
                myTrans.Rollback();
                society = null;
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(HttpContext.Current.Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "FetchSocietyDetails()---Insert.cs");
            }
            finally
            {
                conn.Close();
            }
            return society;
        }



        public Member_Details FetchMember(string appid, Int64 memberid)
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
                query = query + " document_mongoentry, doc_id, proof_document_no,salutation_id,salutation,gender,age,designtaion_others,occupation_others,remarks,dateofadmission from esociety.members where active='Y' and member_id=@mid and app_id=@AppID";
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
                Err.ErrorLog(HttpContext.Current.Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "FetchSocietyDetails()---Insert.cs");
            }
            finally
            {
                conn.Close();
            }
            return member;
        }
                

    }
}