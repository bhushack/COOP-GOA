using GoaSocietyRegistration.Development;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using WS_Encryption;
using Encryption;

namespace GoaSocietyRegistration
{
    [Serializable]
    [System.Web.Script.Services.ScriptService]
    public partial class Applicant : System.Web.UI.Page
    {
        //int flag = 0;
        int count = 0;
        int otp_count = 0;
        string ipaddress = Utility.getIP();
        string macaddress = Utility.GetMACAddress(Utility.getIP());
        System.Globalization.CultureInfo french = new System.Globalization.CultureInfo("fr-FR");
        //string macaddress = Utility.GetMACAddress();
        Insert insrt = new Insert();
        Validate _val = new Validate();
        SessionManage session = new SessionManage();
        NpgsqlConnection connect = new NpgsqlConnection();
        NpgsqlCommand cmd = new NpgsqlCommand();
        NICEncryption _encryption = new NICEncryption();
        NpgsqlDataAdapter adapter;
        DataSet ds;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Check4Tampering())
            {


                RecordUserAction("Page_Load", "Access request failed. Tampered session", "F");
                session.__Abandon(Request, Response);
            }
            else
            {
                if (!IsPostBack)
                {
                    Utility.FillDistrictSoc(ddlAppDistrict);
                    Utility.FillDistrictSoc(ddlregdistrict);
                    FillDesignation_new();
                    //Session["OTP-Intiliaze"] = "false";
                    string strcapcha = Utility.GenerateRandomString(6, "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789");
                    ImgCaptcha.ImageUrl = "Development/GenerateCaptcha.aspx?" + DateTime.Now.Ticks.ToString() + "&capstr=" + Server.HtmlEncode(strcapcha.ToString());
                    Session["captcha"] =  strcapcha;

                }
                HttpContext.Current.Response.AddHeader("Cache-Control", "no-cache, no-store, must-revalidate");
                HttpContext.Current.Response.AddHeader("Pragma", "no-cache");
                HttpContext.Current.Response.AddHeader("Expires", "0");
                RecordUserAction("Applicant Page Onload", "Applicant Page Load Success", "S");
                txtAppName.Focus();
            }
        }
        private bool Check4Tampering()
        {
            bool sessHackedCheck = false;
            if (Request.UrlReferrer != null && GlobalVars.AntiPageRequest.Contains(Path.GetFileName(Page.AppRelativeVirtualPath)))
            {
                Uri uri = new Uri(Request.UrlReferrer.ToString());
                string referrer = uri.Scheme + Uri.SchemeDelimiter + uri.Host;
                if (!GlobalVars.AntiXsrfRefererHeaderList.Contains(referrer))
                    sessHackedCheck = true;
            }
            else
            {
                sessHackedCheck = true;
            }
            return sessHackedCheck;
        }
        protected int validattion()
        {
            int value = 0;
            try
            {
                if (Rdbtn_neworrenew.SelectedIndex == -1)
                {
                    lblError.Text = "Please Select New Society or applying for Renewal!";
                    value = 1;
                }
                else if (txtAppName.Text == "")
                {
                    lblError.Text = "Please Enter Applicant Name!";
                    value = 1;
                }
                else if (!_val.validateData(txtAppName.Text, _val.name1))
                {
                    lblError.Text = "Invalid Applicant Name";
                    value = 1;
                }
                else if (txtAppAddress.Text == "")
                {
                    lblError.Text = "Please Enter Applicant Address!";
                    value = 1;
                }
                else if (!_val.validateData(txtAppAddress.Text, _val.reamrks_validation))
                {
                    lblError.Text = "Wrong/Invalid Address!";
                    value = 1;
                }
                else if (txtAppMobileNo.Text == "")
                {
                    lblError.Text = "Please Enter Applicant Mobile number!";
                    value = 1;
                }
                else if (!_val.validateData(txtAppMobileNo.Text, _val.mobile_regex))
                {
                    lblError.Text = "Invalid Applicant Mobile number!";
                    value = 1;
                }
                else if (ddlAppDistrict.SelectedValue == "-1")
                {
                    lblError.Text = "Please Select District!";
                    value = 1;
                }
                else if (txtAppMobileNo.Text.Length < 10 || txtAppMobileNo.Text.Length > 10)
                {
                    lblError.Text = "Please Enter 10 digit Mobile Number!";
                    value = 1;
                }
                else if (ddlapplicantdesign.SelectedValue == "-1")
                {
                    lblError.Text = "Please Select Designation!";
                    value = 1;
                }
                else if (TxtBxEmailaddress.Text == "" || TxtBxEmailaddress.Text == null)
                {
                    lblError.Text = "Please enter Email ID!";
                    value = 1;
                }
                else if (Rdbtn_govtornot.SelectedIndex == -1)
                {
                    lblError.Text = "Please Select Government Society or not";
                    value = 1;
                }
                else if ((!_val.validateData(TxtBxEmailaddress.Text, _val.emailformatregex) || (TxtBxEmailaddress.Text.Length == 0)))
                {
                    lblError.Text = "Invalid Email/ Error in email";
                    value = 1;
                }
                else
                {
                    value = 0;
                }
            }
            catch (Exception ex)
            {
                value = 0;
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "validattion()" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
            }

            return value;
        }
        public void btnGenOTP_Click(object sender, EventArgs e)
        {
            try
            {
                lbstatus.Text = "";
                if (count == 0)
                {
                    if (Session["captcha"].ToString() == txtbxcaptcha.Text)
                    {

                        txtbxcaptcha.Text = string.Empty;
                        Session.Remove("captcha");
                        int val = validattion();
                        if (val == 0)
                        {
                            txtAppName.Enabled = false;
                            txtAppAddress.Enabled = false;
                            txtAppMobileNo.Enabled = false;
                            ddlAppDistrict.Enabled = false;
                            txtAppMobileNo.Enabled = false;
                            ddlapplicantdesign.Enabled = false;
                            TxtBxEmailaddress.Enabled = false;

                            string mobileno = txtAppMobileNo.Text;
                            SendSMS sms = new SendSMS();
                           string result = sms.send_otp_sms(mobileno);
                            string[] otp = result.Split('|');
                            RecordUserAction("OTP Btn Click", "Otp sent to User Mobile", "S");
                          //  string otp = "11111111";
                          if (otp[0] != "0" && otp[1] == "OK")
                           // if (otp != "0")
                            {
                               Session["OTP"] = otp[0];
                             //   Session["OTP"] = otp;
                                divOTP.Visible = true;
                                divOTPSubmit.Visible = true;
                                btnOTPSubmit.Visible = true;
                                btnGenOTP.Text = "Resend OTP";
                                RecordUserAction("OTP Entered", "Otp entered", "S");                              
                            }
                            else
                            {
                                // lblError.Text = errortext;
                                divOTP.Visible = false;
                                divOTPSubmit.Visible = false;
                                btnOTPSubmit.Visible = false;
                                btnGenOTP.Text = "Generate OTP";
                                RecordUserAction("Generate Otp", "Otp generated again", "S");

                            }
                        }
                        else
                        {
                            //error encountered in validation
                        }
                        refreshCaptcha();
                    }
                    else
                    {
                        txtbxcaptcha.Text = string.Empty;
                        divOTP.Visible = false;
                        divOTPSubmit.Visible = false;
                        btnOTPSubmit.Visible = false;
                        Response.Write("<script language='javascript'>alert('Wrong Captcha')</script>");
                        refreshCaptcha();
                    }
                    count++;
                }
                else
                {
                    btnGenOTP.Enabled = true;
                    refreshCaptcha();
                    Response.Write("<script language='javascript'>alert('Dont click again and again...OTP will generated and sent to your mobile')</script>");
                }
            }
            catch (Exception ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "btnGenOTP_Click" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
            }


        }

        //public void FillDistrict()
        //{
        //    connect.ConnectionString = ConfigurationManager.ConnectionStrings["connect"].ConnectionString;
        //    cmd.Connection = connect;
        //    try
        //    {
        //        connect.Open();
        //        string query = "SELECT \"DistrictName\",\"DistrictID\" FROM esociety.mst_district where \"DistrictID\" != 3";
        //        cmd.CommandText = query;
        //        adapter = new NpgsqlDataAdapter(cmd);
        //        ds = new DataSet();
        //        adapter.Fill(ds, "mst_district");
        //        ddlAppDistrict.DataSource = ds.Tables[0];
        //        ddlAppDistrict.DataTextField = "DistrictName";
        //        ddlAppDistrict.DataValueField = "DistrictID";
        //        ddlAppDistrict.DataBind();
        //        ddlAppDistrict.Items.Insert(0, new ListItem("-- Select --", "-1"));

        //        ddlregdistrict.DataSource = ds.Tables[0];
        //        ddlregdistrict.DataTextField = "DistrictName";
        //        ddlregdistrict.DataValueField = "DistrictID";
        //        ddlregdistrict.DataBind();
        //        ddlregdistrict.Items.Insert(0, new ListItem("-- Select --", "0"));
        //        cmd.Dispose();
        //        adapter.Dispose();
        //        ds = null;
        //    }
        //    catch (NpgsqlException ex)
        //    {
        //        CreateLogFiles Err = new CreateLogFiles();
        //        Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "FillDistrict()" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));

        //        Response.Write("<script language='javascript'>alert('District DropDown: Error')</script>");
        //    }
        //    finally
        //    {
        //        connect.Close();
        //    }
        //}
        public void FillDesignation_renewal()
        {
            NpgsqlConnection connect = new NpgsqlConnection();
            NpgsqlCommand command = new NpgsqlCommand();
            connect.ConnectionString = ConfigurationManager.ConnectionStrings["connect"].ConnectionString;
            command.Connection = connect;
            try
            {
                connect.Open();
                string query = "SELECT \"DesignationName\",\"DesignationID\" FROM esociety.mst_memberdesignation where \"DesignationID\"=@one or \"DesignationID\"=@three or \"DesignationID\"=@nine";
                command.CommandText = query;
                command.Parameters.AddWithValue("@one", 1);
                command.Parameters.AddWithValue("@three", 3);
                command.Parameters.AddWithValue("@nine", 9);
                adapter = new NpgsqlDataAdapter(command);
                ds = new DataSet();
                adapter.Fill(ds, "esociety.mst_memberdesignation");
                ddlapplicantdesign.DataSource = ds.Tables[0];
                ddlapplicantdesign.DataTextField = "DesignationName";
                ddlapplicantdesign.DataValueField = "DesignationID";
                ddlapplicantdesign.DataBind();
                ddlapplicantdesign.Items.Insert(0, new ListItem("-- Select --", "-1"));
                command.Dispose();
                adapter.Dispose();
                ds = null;
            }
            catch (NpgsqlException ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "FillDesignation()" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));

                Response.Write("<script language='javascript'>alert('Data not loading')</script>");
            }
            finally
            {
                connect.Close();
            }
        }

        public void FillDesignation_new()
        {
            NpgsqlConnection connect = new NpgsqlConnection();
            NpgsqlCommand command = new NpgsqlCommand();
            connect.ConnectionString = ConfigurationManager.ConnectionStrings["connect"].ConnectionString;
            command.Connection = connect;
            try
            {
                connect.Open();
                string query = "SELECT \"DesignationName\",\"DesignationID\" FROM esociety.mst_memberdesignation where \"DesignationID\"=@one or \"DesignationID\"=@three";
                command.CommandText = query;
                command.Parameters.AddWithValue("@one", 1);
                command.Parameters.AddWithValue("@three", 3);
                adapter = new NpgsqlDataAdapter(command);
                ds = new DataSet();
                adapter.Fill(ds, "esociety.mst_memberdesignation");
                ddlapplicantdesign.DataSource = ds.Tables[0];
                ddlapplicantdesign.DataTextField = "DesignationName";
                ddlapplicantdesign.DataValueField = "DesignationID";
                ddlapplicantdesign.DataBind();
                ddlapplicantdesign.Items.Insert(0, new ListItem("-- Select --", "-1"));
                command.Dispose();
                adapter.Dispose();
                ds = null;
            }
            catch (NpgsqlException ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "FillDesignation()" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));

                Response.Write("<script language='javascript'>alert('Data not loading')</script>");
            }
            finally
            {
                connect.Close();
            }
        }
        protected void btnOTPSubmit_Click(object sender, EventArgs e)
        {
            
            if (Session["OTP"] != null)
            {
                if (Session["captcha"] != null)
                {
                    if (Session["captcha"].ToString() == txtbxcaptcha.Text)
                    {
                        if (Session["OTP"].ToString() == txtOTP.Text)
                        {
                            txtbxcaptcha.Text = string.Empty;
                            Session.Remove("captcha");
                            connect.ConnectionString = ConfigurationManager.ConnectionStrings["connect"].ConnectionString;
                            string Login_ID = "";
                            cmd.Connection = connect;
                            if (Convert.ToInt32(Rdbtn_neworrenew.SelectedValue) == 1)
                            {
                                Login_ID = Utility.getloginNew();
                            }
                            else if (Convert.ToInt32(Rdbtn_neworrenew.SelectedValue) == 2)
                            {
                                Login_ID = Utility.getloginrenewal();
                            }
                            Session["session"] = Login_ID;
                            connect.Open();
                            NpgsqlTransaction trans = connect.BeginTransaction();
                            try
                            {
                                int val = validattion();
                                if (val == 0)
                                {
                                    string encrypted_email = "";
                                    cmd.Transaction = trans;
                                    cmd.Parameters.Clear();
                                    string query = "INSERT INTO esociety.applicant_details(login_id,applicant_name, applicant_address, applicant_designation, applicant_district, applicant_mobile_no,";
                                    query = query + " created_by, created_at, ipaddress, mac_address,complete,active,applicant_email,new_or_renewal,old_socregid,old_socdistrict,is_gov_society)VALUES(@login_id,";
                                    query = query + " @applicant_name,@applicant_address,@applicant_designation,@applicant_district,@applicant_mobile_no,@created_by,current_timestamp,@ipaddress,";
                                    query = query + " @mac_address,'Y','Y',@applicant_email,@new_or_renewal,@old_socregid,@old_socdistrict,@is_gov_society)";
                                    cmd.CommandText = query;
                                    cmd.Parameters.AddWithValue("@login_id", Login_ID);
                                    cmd.Parameters.AddWithValue("@applicant_name", txtAppName.Text);
                                    cmd.Parameters.AddWithValue("@applicant_address", txtAppAddress.Text);
                                    cmd.Parameters.AddWithValue("@applicant_designation", Convert.ToInt32(ddlapplicantdesign.SelectedValue));
                                    cmd.Parameters.AddWithValue("@applicant_district", Convert.ToInt32(ddlAppDistrict.SelectedValue));
                                    //string encrypted_mobile = AES_algorithm.Encrypt(txtAppMobileNo.Text);
                                    string encrypted_mobile = Encrypt.Encryptt(txtAppMobileNo.Text);
                                    cmd.Parameters.AddWithValue("@applicant_mobile_no", encrypted_mobile);
                                    cmd.Parameters.AddWithValue("@created_by", txtAppName.Text);
                                    cmd.Parameters.AddWithValue("@ipaddress", ipaddress);
                                    cmd.Parameters.AddWithValue("@mac_address", macaddress);
                                    if (TxtBxEmailaddress.Text == "" || TxtBxEmailaddress.Text == null)
                                    {
                                        encrypted_email = "NA";
                                    }
                                    else
                                    {
                                        encrypted_email = Encrypt.Encryptt(TxtBxEmailaddress.Text);//AES_algorithm.Encrypt(TxtBxEmailaddress.Text);
                                    }
                                    cmd.Parameters.AddWithValue("@applicant_email", encrypted_email);
                                    cmd.Parameters.AddWithValue("@new_or_renewal", Convert.ToInt32(Rdbtn_neworrenew.SelectedValue));
                                    //cmd.Parameters.AddWithValue("@old_socregid", TxtBxOldRegNo.Text);
                                    if (Convert.ToInt32(Rdbtn_neworrenew.SelectedValue) == 1)
                                    {
                                        cmd.Parameters.AddWithValue("@old_socregid", "");
                                        cmd.Parameters.AddWithValue("@old_socdistrict", 0);
                                    }
                                    else if (Convert.ToInt32(Rdbtn_neworrenew.SelectedValue) == 2)
                                    {
                                        cmd.Parameters.AddWithValue("@old_socregid", ViewState["SocietyRegId"].ToString());
                                        cmd.Parameters.AddWithValue("@old_socdistrict", Convert.ToInt32(ddlregdistrict.SelectedValue));

                                    }
                                    if (Convert.ToInt32(Rdbtn_govtornot.SelectedValue) == 0)//no
                                    {
                                        cmd.Parameters.AddWithValue("@is_gov_society", 0);
                                    }
                                    else if (Convert.ToInt32(Rdbtn_govtornot.SelectedValue) == 1) //yes
                                    {
                                        cmd.Parameters.AddWithValue("@is_gov_society", 1);
                                    }

                                    cmd.ExecuteNonQuery();
                                    cmd.Parameters.Clear();
                                    string temp_table_query = "INSERT INTO esociety.temp_table(login_id, created_by, obs_count) VALUES(@login_id, @created_by, @obs_count)";
                                    cmd.CommandText = temp_table_query;
                                    cmd.Parameters.AddWithValue("@login_id", Login_ID);
                                    cmd.Parameters.AddWithValue("@created_by", txtAppName.Text);
                                    cmd.Parameters.AddWithValue("@obs_count", 1);
                                    cmd.ExecuteNonQuery();
                                    cmd.Parameters.Clear();
                                    string otherserv_query = "INSERT into esociety.status_amendment(login_id,amend_status) VALUES(@login_id,0)";
                                    cmd.CommandText = otherserv_query;
                                    cmd.Parameters.AddWithValue("@login_id", Login_ID);
                                    cmd.ExecuteNonQuery();
                                    cmd.Parameters.Clear();
                                    string loginquery = "INSERT INTO esociety.usertable(user_firstname, user_loginname, user_mobile, created_at, created_by, user_created_ip, user_created_mac,active) VALUES (@user_firstname, @user_loginname, @user_mobile, current_timestamp, @created_by, @user_created_ip, @user_created_mac,'Y')";
                                    cmd.CommandText = loginquery;
                                    cmd.Parameters.AddWithValue("@user_firstname", txtAppName.Text);
                                    cmd.Parameters.AddWithValue("@user_loginname", Login_ID);
                                    //string encrypted_mobiles = AES_algorithm.Encrypt(txtAppMobileNo.Text);

                                    string encrypted_mobiles = Encrypt.Encryptt(txtAppMobileNo.Text);

                                    cmd.Parameters.AddWithValue("@user_mobile", encrypted_mobiles);
                                    cmd.Parameters.AddWithValue("@created_by", txtAppName.Text);
                                    cmd.Parameters.AddWithValue("@user_created_ip", ipaddress);
                                    cmd.Parameters.AddWithValue("@user_created_mac", macaddress);

                                    cmd.ExecuteNonQuery();
                                    trans.Commit();
                                    Session.Remove("OTP");
                                    txtOTP.Text = string.Empty;
                                    //if (Session["OTP-Intiliaze"].ToString() != "true")
                                    //{
                                    SendSMS sendsms = new SendSMS();
                                    //Session["OTP-Intiliaze"] = "true";
                                    Session["login_id"] = Login_ID;   /********SN***********/
                                    sendsms.token_generate(txtAppMobileNo.Text, Login_ID);
                                    //}  

                                    int value = setlogs();

                                    if (value == 1)
                                    {
                                        SessionManage sessionmanage = new SessionManage();
                                        string OldID = Context.Session.SessionID;
                                        string newID = generateNewSessionId();
                                        sessionmanage.__ReInitialiseAuthCookie(Response);
                                        if (Session["login_id"] != null)
                                        {

                                            RecordUserAction("Submit Button Click", "Applicant Registered Successfully", "S");
                                            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "none", "<script> $(window).on('load', function () { $('#myModal1').modal({ backdrop: 'static' });});</script>", false);
                                            Response.Redirect("~/User/Dashboard.aspx");

                                        }
                                        else
                                        {
                                            Status.ForeColor = System.Drawing.Color.Green;
                                            string message = "Your Token No is " + session + " Click on Proceed to Login.";
                                            Status.Text = Server.HtmlEncode(message);
                                            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "none", "<script> $(window).on('load', function () { $('#AccountCreatedSuccessful').modal({ backdrop: 'static' });});</script>", false);
                                            //redirect to login and send sms 
                                        }
                                    }
                                    else
                                    {
                                        Status.ForeColor = System.Drawing.Color.Green;
                                        string message = "Your Token No/Login Name is " + Login_ID;
                                        Status.Text = Server.HtmlEncode(message);

                                        //string message = "Your Token No is " + session + " Click on Proceed to Login.";
                                        //Status.Text = Server.HtmlEncode(message);
                                        // ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "none", "<script> $(window).on('load', function () { $('#AccountCreatedSuccessful').modal({ backdrop: 'static' });});</script>", false);
                                        //redirect to login and send sms 
                                    }





                                    //redirect to login and send sms         
                                    //If sucessful Redirected open Next Page
                                }

                            }
                            catch (NpgsqlException ex)
                            {
                                CreateLogFiles Err = new CreateLogFiles();
                                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "btnOTPSubmit_Click" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));

                                trans.Rollback();
                                //Session["OTP-Intiliaze"] = "false";
                                Response.Write("<script language='javascript'>alert('Execution Error')</script>");
                                RecordUserAction("Submit Button Click", ex.Message + "Applicant details entry to DB fail, Not registered", "F");
                            }
                            finally
                            {
                                connect.Close();
                            }
                        }
                        else
                        {
                            refreshCaptcha();
                            lblError.Text = "Invalid OTP. Please enter correct OTP. ";
                            RecordUserAction("Incorrect OTP", "Entered Wrong OTP", "F");
                        }
                    }
                    else
                    {
                        refreshCaptcha();
                        lblError.Text = "Invalid Captcha. Please enter correct captcha. ";
                        RecordUserAction("Incorrect Captcha", "Entered Wrong Captcha", "F");
                    }
                }
                else {
                    refreshCaptcha();
                    RecordUserAction("Incorrect Captcha", "Entered Wrong Captcha", "F");
                    Response.Write("<script language='javascript'>alert('Please Enter Correct Captcha')</script>");
                }
            }
            else
            {
                refreshCaptcha();
                RecordUserAction("Incorrect OTP", "Entered Wrong OTP", "F");
                Response.Write("<script language='javascript'>alert('Please Enter Correct OTP')</script>");
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
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "none", "<script> $(window).on('load', function () { $('#myModal2').modal({ backdrop: 'static' });});</script>", false);
                }
                else if (ConfirmCancel.Equals("Cancel"))
                {
                    lblMSG1.Text = message;
                    RedirecttoLoginBtn.Visible = false;
                    btnCancel.Text = "Okay";
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "none", "<script> $(window).on('load', function () { $('#myModal2').modal({ backdrop: 'static' });});</script>", false);
                }
            }
            catch (Exception ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "ShowUserAlert()" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
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
                trail.user_session_id = Session["loginsession"] != null ? Session["loginsession"].ToString() : "null";
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
                count = insrt.SaveAuditTrail(trail);
            } while (count == 0);
            /*Audit trail*/
        }
        protected void RedirecttoLoginBtn_Click(object sender, EventArgs e)
        {
            RecordUserAction("Login Button Click", "Redirect to Login Button after Registration", "S");
            Response.Redirect("~/User/LoginModule.aspx");
        }

        protected void lklogin_Click(object sender, EventArgs e)
        {
            RecordUserAction("Login Button Click", " Directly Clicked on Login Button", "S");
            Response.Redirect("Default.aspx");

        }


        public void refreshCaptcha()
        {
            try
            {
                string strcapcha = Utility.GenerateRandomString(6, "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789");
                ImgCaptcha.ImageUrl = "Development/GenerateCaptcha.aspx?" + DateTime.Now.Ticks.ToString() + "&capstr=" + Server.HtmlEncode(strcapcha.ToString());
                Session["captcha"] = strcapcha;
            }
            catch (Exception ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "refreshCaptcha()" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
            }

        }

   

        protected void Rdbtn_neworrenew_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (Rdbtn_neworrenew.SelectedValue == "1")
                {
                    applicant.Visible = true;
                    info.Visible = true;
                    regnodetails.Visible = false;
                    regdistdetails.Visible = false;
                    regdetailsbtn.Visible = false;
                    regsocnamedetails.Visible = false;
                    regdatedetails.Visible = false;
                    TxtBxOldRegNo.Text = "";
                    //ddlregdistrict.SelectedValue = "0";
                    TxtBxOldSocName.Text = "";
                    TxtBxOldRegDate.Text = "";
                    BtnRegDetails.Visible = false;
                    ViewState["SocietyRegId"] = "";


                }
                else if (Rdbtn_neworrenew.SelectedValue == "2")
                {
                    BtnRegDetails.Visible = true;
                    applicant.Visible = false;
                    info.Visible = false;
                    regnodetails.Visible = true;
                    regdistdetails.Visible = true;
                    regdetailsbtn.Visible = true;
                    BtnProceed.Visible = false;
                    btnClear.Visible = false;
                    TxtBxOldRegNo.Enabled = true;
                    ddlregdistrict.Enabled = true;
                    TxtBxOldRegNo.Text = "";
                    //ddlregdistrict.SelectedValue = "0";
                    lblRegdetailsError.Text = "";
                    ViewState["SocietyRegId"] = "";
                    FillDesignation_renewal();
                }
            }
            catch (Exception ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "Rdbtn_neworrenew_SelectedIndexChanged()" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
            }

        }

        protected void BtnRegDetails_Click(object sender, EventArgs e)
        {
            try
            {
                if (TxtBxOldRegNo.Text == "" || TxtBxOldRegNo.Text == null)
                {
                    lblRegdetailsError.Text = "Please Enter Certificate/Registration No. !";
                }
                else if (ddlregdistrict.SelectedValue == "-1")
                {
                    lblRegdetailsError.Text = " Please Select Registered District!";
                }
                else
                {                   
                    FetchOldSocietyDetails();
                    
                }
            }
            catch (Exception ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "BtnRegDetails_Click()" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
            }

        }

        protected void BtnProceed_Click(object sender, EventArgs e)
        {
            Rdbtn_neworrenew.Enabled = false;
            applicant.Visible = true;
            info.Visible = true;
            regdetailsbtn.Visible = false;
            TxtBxOldRegNo.Enabled = false;
            ddlregdistrict.Enabled = false;
        }


        public void FetchOldSocietyDetails()
        {
            DateTime registereddate = DateTime.Now.Date;
            DateTime todaydate = DateTime.Today.Date;
            TimeSpan diff;
            int yearsdiff = 0;
            int societyfound = 0;
            int displaydata = 0;
            string registration_id = "";
            

            NpgsqlConnection conn = new NpgsqlConnection();
            NpgsqlCommand cmd = new NpgsqlCommand();
            conn.ConnectionString = ConfigurationManager.ConnectionStrings["connect"].ConnectionString;
            cmd.Connection = conn;
            conn.Open();
            NpgsqlTransaction myTrans = conn.BeginTransaction();
            cmd.Transaction = myTrans;
            try
            {
               
                string query = "SELECT socname,socregid,socdistrict,regdate FROM esociety.society where upper(socregid) = @socregid and socdistrict = @socdistrict and active='Y'";
                cmd.CommandText = query;
               // cmd.Parameters.AddWithValue("@socregid", TxtBxOldRegNo.Text);
                cmd.Parameters.AddWithValue("@socregid",TxtBxOldRegNo.Text.ToUpper().Trim());
                cmd.Parameters.AddWithValue("@socdistrict", Convert.ToInt32(ddlregdistrict.SelectedValue));
                NpgsqlDataReader rd = cmd.ExecuteReader();
                if (rd.Read())
                {
                    if(!(string.IsNullOrEmpty(rd["regdate"].ToString())))
                    {
                        
                        registereddate = Convert.ToDateTime(rd["regdate"], french).Date;                      
                       
                        diff = todaydate - registereddate;
                        yearsdiff = (int)(diff.Days / (365.25));
                        if (yearsdiff >= 5)
                        {
                            lblRegdetailsError.Text = "SOCIETY DATA FOUND!! Verify and Click on Proceed. "; 
                            BtnProceed.Visible = true;
                            
                        }
                        else
                        {

                            lblRegdetailsError.Text = "You are not Eligible for Renewal!!!";
                        }
                        TxtBxOldSocName.Text = rd["socname"].ToString();
                        TxtBxOldRegDate.Text = registereddate.ToString("dd/MM/yyyy");
                        registration_id = rd["socregid"].ToString();
                        regsocnamedetails.Visible = true;
                        regdatedetails.Visible = true;
                        BtnRegDetails.Visible = false;
                        TxtBxOldRegNo.Enabled = false;
                        ddlregdistrict.Enabled = false;
                        btnClear.Visible = true;
                        societyfound = 1;
                    }
                    else
                    {
                        societyfound = 0;
                        lblRegdetailsError.Text = "Registered Date Not available";
                    }
                    
                    
                }
               
                rd.Close();
                if(societyfound==0)
                {
                    displaydata = 0;
                    string query1 = "SELECT socname,socregid,socdistrict,regdate,reg_date,datemodified FROM esociety.society_all where upper(socregid) = @socregid and socdistrict = @socdistrict and active='Y'";
                    cmd.CommandText = query1;
                    // cmd.Parameters.AddWithValue("@socregid", TxtBxOldRegNo.Text);
                    cmd.Parameters.AddWithValue("@socregid", TxtBxOldRegNo.Text.ToUpper().Trim());
                    cmd.Parameters.AddWithValue("@socdistrict", Convert.ToInt32(ddlregdistrict.SelectedValue));
                    NpgsqlDataReader rd1 = cmd.ExecuteReader();
                    if (rd1.Read())
                    {
                       // if (!(string.IsNullOrEmpty(rd1["regdate"].ToString())))
                      //  {
                            //registereddate = Convert.ToDateTime(rd1["regdate"],french).Date;

                            if (Convert.ToInt32(rd1["datemodified"].ToString()) == 1 && !(string.IsNullOrEmpty(rd1["reg_date"].ToString())))
                            {
                                registereddate = Convert.ToDateTime(rd1["reg_date"], french).Date;
                                displaydata = 1;

                            }
                            else if (Convert.ToInt32(rd1["datemodified"].ToString()) == 0 && !(string.IsNullOrEmpty(rd1["regdate"].ToString())))
                            {
                                registereddate = Convert.ToDateTime(rd1["regdate"], french).Date;
                                displaydata = 1;
                            }
                            else
                            {
                                lblRegdetailsError.Text = "Registered Date Not available. Please Contact your respective District Office.";
                                displaydata = 0;

                            }

                            if (displaydata == 1)
                            {
                                diff = todaydate - registereddate;
                                yearsdiff = (int)(diff.Days / (365.25));
                                if (yearsdiff >= 5)
                                {
                                    lblRegdetailsError.ForeColor = System.Drawing.Color.Green;
                                    lblRegdetailsError.Text = "SOCIETY DATA FOUND!! Verify and Click on Proceed. ";
                                    BtnProceed.Visible = true;
                                }
                                else
                                {

                                    lblRegdetailsError.Text = "You are not Eligible for Renewal!!!";
                                }
                                TxtBxOldSocName.Text = rd1["socname"].ToString();
                                TxtBxOldRegDate.Text = registereddate.ToString("dd/MM/yyyy");
                                registration_id = rd["socregid"].ToString();
                                regsocnamedetails.Visible = true;
                                regdatedetails.Visible = true;
                                BtnRegDetails.Visible = false;
                                TxtBxOldRegNo.Enabled = false;
                                ddlregdistrict.Enabled = false;
                                btnClear.Visible = true;
                            }
                       // }
                       // else
                     //   {
                      //      lblRegdetailsError.Text = "Registered Date Not available";
                       // }
                        societyfound = 1;



                    }
                    else
                    {
                        lblRegdetailsError.Text = "No Data found!! Please contact the Department. ";
                        societyfound = 0;
                    }
                    rd1.Close();
                }

                if (societyfound == 0)
                {
                    displaydata = 0;
                    string query1 = "SELECT socname,socregid,socdistrict,regdate,reg_date,datemodified FROM esociety.society_all_north where upper(socregid) = @socregid and socdistrict = @socdistrict and active='Y'";
                    cmd.CommandText = query1;
                    // cmd.Parameters.AddWithValue("@socregid", TxtBxOldRegNo.Text);
                    cmd.Parameters.AddWithValue("@socregid", TxtBxOldRegNo.Text.ToUpper().Trim());
                    cmd.Parameters.AddWithValue("@socdistrict", Convert.ToInt32(ddlregdistrict.SelectedValue));
                    NpgsqlDataReader rd2 = cmd.ExecuteReader();
                    if (rd2.Read())
                    {
                        //if (!(string.IsNullOrEmpty(rd2["regdate"].ToString())))
                       // {
                            //registereddate = Convert.ToDateTime(rd2["regdate"],french).Date;
                            if (Convert.ToInt32(rd2["datemodified"].ToString()) == 1 && !(string.IsNullOrEmpty(rd2["reg_date"].ToString())))
                            {
                                registereddate = Convert.ToDateTime(rd2["reg_date"], french).Date;
                                displaydata = 1;

                            }
                            else if (Convert.ToInt32(rd2["datemodified"].ToString()) == 0 && !(string.IsNullOrEmpty(rd2["regdate"].ToString())))
                            {
                                registereddate = Convert.ToDateTime(rd2["regdate"], french).Date;
                                displaydata = 1;
                            }
                            else
                            {
                                lblRegdetailsError.Text = "Registered Date Not available. Please contact the Department.";
                                displaydata = 0;

                             }

                            if (displaydata == 1)
                            {
                                diff = todaydate - registereddate;
                                yearsdiff = (int)(diff.Days / (365.25));
                                if (yearsdiff >= 5)
                                {
                                    lblRegdetailsError.ForeColor = System.Drawing.Color.Green;
                                    lblRegdetailsError.Text = "SOCIETY DATA FOUND!! Verify and Click on Proceed. ";
                                    BtnProceed.Visible = true;
                                }
                                else
                                {

                                    lblRegdetailsError.Text = "You are not Eligible for Renewal!!!";
                                }
                                TxtBxOldSocName.Text = rd2["socname"].ToString();
                                TxtBxOldRegDate.Text = registereddate.ToString("dd/MM/yyyy");
                                registration_id = rd["socregid"].ToString();
                                regsocnamedetails.Visible = true;
                                regdatedetails.Visible = true;
                                BtnRegDetails.Visible = false;
                                TxtBxOldRegNo.Enabled = false;
                                ddlregdistrict.Enabled = false;
                                btnClear.Visible = true;
                            }
                      //  }
                     //   else
                       // {
                        //    lblRegdetailsError.Text = "Registered Date Not available";
                      //  }
                        societyfound = 1;
                    }
                    else
                    {
                        lblRegdetailsError.Text = "No Data found!! Please contact the Department. ";
                        societyfound = 0;
                    }
                    rd2.Close();
                }
                ViewState["SocietyRegId"] = registration_id;
                myTrans.Commit();

            }
            catch (NpgsqlException ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "FetchOldSocietyDetails()" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
                myTrans.Rollback();
                lblRegdetailsError.Text = "No Data found!!! Please contact the Department.";
            }
            finally
            {
                conn.Close();
               
            }
        }

        protected void btnClear_Click(object sender, EventArgs e)
        {
            Response.Redirect("Applicant.aspx");
        }

        //protected List<string> getregdatesuggestions(string regid)
        //{
        //    List<string> RegistrationIds = new List<string>();

        //    NpgsqlConnection conn = new NpgsqlConnection();
        //    NpgsqlCommand cmd = new NpgsqlCommand();
        //    conn.ConnectionString = ConfigurationManager.ConnectionStrings["connect"].ConnectionString;
        //    cmd.Connection = conn;
        //    conn.Open();
        //    NpgsqlTransaction myTrans = conn.BeginTransaction();
        //    cmd.Transaction = myTrans;
        //    try
        //    {
        //        string query = "select socname,regdate,socregid from esociety.society where socregid LIKE @socregid";
        //        cmd.CommandText = query;
        //        cmd.Parameters.AddWithValue("@socregid", "%" + regid + "%");
        //        NpgsqlDataReader dr = cmd.ExecuteReader();

        //        if (dr.Read())
        //        {
        //            RegistrationIds.Add(dr["ProductName"].ToString());


        //        }
        //        dr.Close();


        //        string query1 = "select socname,regdate,socregid from esociety.society_all where socregid LIKE @socregid";
        //        cmd.CommandText = query1;
        //        cmd.Parameters.AddWithValue("@socregid", "%" + regid + "%");
        //        NpgsqlDataReader rd = cmd.ExecuteReader();


        //        if (rd.Read())
        //        {
        //            RegistrationIds.Add(dr["regdate"].ToString());
        //        }
        //        rd.Close();
        //        myTrans.Commit();
        //    }
        //    catch (NpgsqlException ex)
        //    {
        //        CreateLogFiles Err = new CreateLogFiles();
        //        Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "checksocietyname()" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));

        //        myTrans.Rollback();
        //    }
        //    finally
        //    {
        //        conn.Close();
        //    }

        //    return RegistrationIds;
        //}

        public Int32 getlogin_sess_count_user()
        {
            //keep track of login and logout and to update logout time based on this...
            NpgsqlConnection connect = new NpgsqlConnection();
            NpgsqlCommand cmd = new NpgsqlCommand();
            connect.ConnectionString = ConfigurationManager.ConnectionStrings["connect"].ConnectionString;
            cmd.Connection = connect;
            try
            {
                string value = "";
                connect.Open();
                cmd.Parameters.Clear();
                string query = "select nextval('esociety.login_count_user')";
                cmd.CommandText = query;
                value = Convert.ToString(cmd.ExecuteScalar());
                int i = Convert.ToInt32(value);
                return i;
            }
            catch (Exception ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "getlogin_sess_count_user()" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));

                RecordUserAction("getlogin_sess_count_user", ex.Message, "F");
                return 0;
            }
            finally
            {
                connect.Close();
            }
        }

        public int setlogs()
        {
            if (Session["login_id"] != null)
            {
                Int64 loginsession = getlogin_sess_count_user();
                Session["loginsession_count"] = loginsession;
                NpgsqlConnection conn = new NpgsqlConnection();
                NpgsqlCommand cmd = new NpgsqlCommand();
                conn.ConnectionString = ConfigurationManager.ConnectionStrings["connect"].ConnectionString;
                cmd.Connection = conn;
                try
                {
                    conn.Open();
                    string query = "INSERT INTO esociety.loginentries_user(login_sess_count, user_loginname, user_logintime, user_logouttime, ipaddress, macaddress) VALUES(@login_sess_count, @user_loginname, current_timestamp, current_timestamp, @ipaddress, @macaddress)";
                    cmd.CommandText = query;
                    cmd.Parameters.AddWithValue("@login_sess_count", loginsession);
                    cmd.Parameters.AddWithValue("@user_loginname", Session["login_id"].ToString());
                    cmd.Parameters.AddWithValue("@ipaddress", ipaddress);
                    cmd.Parameters.AddWithValue("@macaddress", macaddress);
                    cmd.ExecuteNonQuery();
                    RecordUserAction("setlogs", "Login Session generated", "S");
                    return 1;

                }
                catch (NpgsqlException ex)
                {
                    CreateLogFiles Err = new CreateLogFiles();
                    Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "setlogs()" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));

                    RecordUserAction("setlogs", ex.Message, "F");
                    lbstatus.ForeColor = System.Drawing.Color.Red;
                    lbstatus.Text = "Login Failed";
                    return 0;
                }
                finally
                {
                    conn.Close();
                }
            }
            else
            {
                return 0;

            }

        }

        private string generateNewSessionId()
        {
            System.Web.SessionState.SessionIDManager manager = new System.Web.SessionState.SessionIDManager();
            string oldId = manager.GetSessionID(Context);
            string newId = manager.CreateSessionID(Context);
            bool isAdd = false, isRedir = false;
            try
            {
                manager.SaveSessionID(Context, newId, out isRedir, out isAdd);
                HttpApplication ctx = (HttpApplication)HttpContext.Current.ApplicationInstance;
                HttpModuleCollection mods = ctx.Modules;
                System.Web.SessionState.SessionStateModule ssm = (SessionStateModule)mods.Get("Session");
                System.Reflection.FieldInfo[] fields = ssm.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Instance);
                SessionStateStoreProviderBase store = null;
                System.Reflection.FieldInfo rqIdField = null, rqLockIdField = null, rqStateNotFoundField = null;
                foreach (System.Reflection.FieldInfo field in fields)
                {
                    if (field.Name.Equals("_store")) store = (SessionStateStoreProviderBase)field.GetValue(ssm);
                    if (field.Name.Equals("_rqId")) rqIdField = field;
                    if (field.Name.Equals("_rqLockId")) rqLockIdField = field;
                    if (field.Name.Equals("_rqSessionStateNotFound")) rqStateNotFoundField = field;
                }
                object lockId = rqLockIdField.GetValue(ssm);
                if ((lockId != null) && (oldId != null)) store.ReleaseItemExclusive(Context, oldId, lockId);
                rqStateNotFoundField.SetValue(ssm, true);
                rqIdField.SetValue(ssm, newId);
            }
            catch (Exception ex)
            {
                newId = null;
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "generateNewSessionId()" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
            }

            return newId;
        }

        protected void lkrefreshCaptcha_Click(object sender, EventArgs e)
        {
            refreshCaptcha();
        }
    }
}