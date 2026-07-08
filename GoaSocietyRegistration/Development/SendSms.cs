using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;

namespace GoaSocietyRegistration.Development
{
    public class SendSMS
    {
        string errortext = "";      
        string smsline1 = "https://smsgw.sms.gov.in/failsafe/HttpLink?username=reggoa.otp&pin=P2%40pJ7%26yS2&message=";
      
        //string userAuthenticationURI = "http://itgesms.goa.gov.in/index.php?app=ws&u=SRHNS136-CRSRSTTR14&h=02475f9b9bb86464fd8566734b3bf555&op=pv&to=";
        string userAuthenticationURI = "";
       
        public string send_otp_sms(string mobileno)// Send Otp 
        {
            string temp=null;
            try
            {
                string message;
                long otpcode = generate_OTP_Code();
                message = "Your OTP for Goa Online Society Registration is " + otpcode + " - Registration Department, Goa";
                string smsline2 = "&signature=REGGOA&dlt_entity_id=1401406560000034950&dlt_template_id=1407162244684632338";
                if (errortext == "")
                {
                    string htmlEncodedMsg = message.Replace("#", "%23").Replace("&", "%26").Replace("$", "%24");
                    userAuthenticationURI = smsline1 + message + "&mnumber=" + mobileno + smsline2;
                    string jsonData = String.Empty;
                    using (WebClient client = new WebClient())
                    {
                        ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;
                        ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;
                        string s = client.DownloadString(userAuthenticationURI);
                        if (s.Contains("API000"))
                        {
                            temp = "OK";
                            //RecordUserAction("Contains ok in status", "Sms sent successfully", "S");
                        }
                        else
                        {
                            temp = "NOTOK";
                            // RecordUserAction("ok is not thier in status", "Some issue delivering Message", "F");
                        }
                    }

                }
                return otpcode.ToString() + "|" + temp;
            }
          
            catch (Exception ex)
            {

                //RecordUserAction("send_otp_sms", ex.Message, "F");
                //errortext = "Error while sending OTP";
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(HttpContext.Current.Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "send_otp_sms()---SendSms.cs");
                return "0";

            }
        }
        public void send_otp_sms_submit(string mobileno, string app_id, string token_id)// Send Submit 
        {
            try
            {
                string message;              
               
                message = "Dear User, Your application is submitted for Society Registration. You can use " + token_id + " to login - Registration Department Goa";
                string smsline2 = "&signature=REGGOA&dlt_entity_id=1401406560000034950&dlt_template_id=1407162244750115568";
                if (errortext == "")
                {
                    string htmlEncodedMsg = message.Replace("#", "%23").Replace("&", "%26").Replace("$", "%24");
                    userAuthenticationURI = smsline1 + message + "&mnumber=" + mobileno + smsline2;
                    string jsonData = String.Empty;
                    using (WebClient client = new WebClient())
                    {

                        string s = client.DownloadString(userAuthenticationURI);
                        if (s.Contains("API000"))
                        {
                          
                            //RecordUserAction("Contains ok in status", "Sms sent successfully", "S");
                        }
                        else
                        {
                           
                            // RecordUserAction("ok is not thier in status", "Some issue delivering Message", "F");
                        }
                    }

                }
               
            }
            catch (Exception ex)
            {
             
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(HttpContext.Current.Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "send_otp_sms_submit()---SendSms.cs");
              

            }
        }
        public string send_sms_dep_user(string mobileno)
        {//dont touch dlt and other thing
            string temp = null;
            try
            {
                string message;
                //smsline1 = "https://smsgw.sms.gov.in/failsafe/HttpLink?username=reggoa.sms&pin=P2%40pJ7%26yS2&message=";
                string  smsline2 = "&signature=REGGOA&dlt_entity_id=1401406560000034950&dlt_template_id=1407160812013326516";

                //otpcode = generate_OTP_Code();
                message = "Your account has been created. Kindly generate your password on the first attempt - Registration Department, Goa";

                if (errortext == "")
                {
                    string htmlEncodedMsg = message.Replace("#", "%23").Replace("&", "%26").Replace("$", "%24");
                    //userAuthenticationURI = userAuthenticationURI + mobileno + "&msg=" + message;
                    userAuthenticationURI = smsline1 + message + "&mnumber=" + mobileno + smsline2;
                    string jsonData = String.Empty;
                    using (WebClient client = new WebClient())
                    {

                        string s = client.DownloadString(userAuthenticationURI);
                        if (s.Contains("API000"))
                        {
                            temp = "OK";
                            //RecordUserAction("Contains ok in status", "Sms sent successfully", "S");
                        }
                        else
                        {
                            temp = "NOTOK";
                            // RecordUserAction("ok is not thier in status", "Some issue delivering Message", "F");
                        }
                    }

                }
                return temp;
            }
            catch (Exception ex)
            {
                //RecordUserAction("send_otp_sms", ex.Message, "F");
                //errortext = "Error while sending OTP";
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(HttpContext.Current.Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "send_sms_dep_user()" + "");
                return "0";
            }
        }
        public void token_generate(string mobileno, string token_id)// Send Submit 
        {
            try
            {
                string message;              
                message = "Dear User, Your Token number for Society Registration is " + token_id + " to login - Registration Department Goa";
                string smsline2 = "&signature=REGGOA&dlt_entity_id=1401406560000034950&dlt_template_id=1407162244720268234";
                if (errortext == "")
                {
                    string htmlEncodedMsg = message.Replace("#", "%23").Replace("&", "%26").Replace("$", "%24");
                    userAuthenticationURI = smsline1 + message + "&mnumber=" + mobileno + smsline2;
                    string jsonData = String.Empty;
                    using (WebClient client = new WebClient())
                    {

                        string s = client.DownloadString(userAuthenticationURI);
                        if (s.Contains("API000"))
                        {

                            //RecordUserAction("Contains ok in status", "Sms sent successfully", "S");
                        }
                        else
                        {

                            // RecordUserAction("ok is not thier in status", "Some issue delivering Message", "F");
                        }
                    }

                }

            }
            catch (Exception ex)
            {

                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(HttpContext.Current.Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "send_otp_sms_submit()---SendSms.cs");


            }

        }
        public void observation_send_sms_br_sro(string mobileno)
        {
            CreateLogFiles Err = new CreateLogFiles();
            //smsline1 = "https://smsgw.sms.gov.in/failsafe/HttpLink?username=reggoa.sms&pin=P2%40pJ7%26yS2&message=";
             string smsline2 = "&signature=REGGOA&dlt_entity_id=1401406560000034950&dlt_template_id=1407162244762802456";   
            try
            {
                string message;
                message = "Due to some shortfall in your application for Society Registration, is reverted back to you. Please comply and submit again - Registration Department, Goa";
                if (errortext == "")
                {
                    string htmlEncodedMsg = message.Replace("#", "%23").Replace("&", "%26").Replace("$", "%24");
                    // userAuthenticationURI = userAuthenticationURI + mobileno + "&msg=" + message;
                    userAuthenticationURI = smsline1 + message + "&mnumber=" + mobileno + smsline2;
                    string jsonData = String.Empty;
                    using (WebClient client = new WebClient())
                    {

                        string s = client.DownloadString(userAuthenticationURI);
                        if (s.Contains("API000"))
                        {                          
                           
                        }
                        else
                        {                           
                           
                        }
                    }

                }
               
            }
            catch (Exception ex)
            {
                Err.ErrorLog(HttpContext.Current.Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "observation_send_sms_br_sro()");
            
            }
          
        }
        public void accepted_send_sms_br_sro(string mobileno)
        {
    
            try
            {
                 //smsline1 = "https://smsgw.sms.gov.in/failsafe/HttpLink?username=reggoa.sms&pin=P2%40pJ7%26yS2&message=";
                string message;

               
                    message = " Your application for Society Registration is accepted and found ok by our Officer.Now you can do payment after login into the portal - Registration Department Goa";
                    string smsline2 = "&signature=REGGOA&dlt_entity_id=1401406560000034950&dlt_template_id=1407162244774113358";
                    if (errortext == "")
                    {
                        string htmlEncodedMsg = message.Replace("#", "%23").Replace("&", "%26").Replace("$", "%24");
                        userAuthenticationURI = smsline1 + message + "&mnumber=" + mobileno + smsline2;
                        string jsonData = String.Empty;
                        using (WebClient client = new WebClient())
                        {

                            string s = client.DownloadString(userAuthenticationURI);
                            if (s.Contains("API000"))
                            {

                                //RecordUserAction("Contains ok in status", "Sms sent successfully", "S");
                            }
                            else
                            {

                                // RecordUserAction("ok is not thier in status", "Some issue delivering Message", "F");
                            }
                        }

                    }

                }
                catch (Exception ex)
                {
                    //RecordUserAction("send_otp_sms", ex.Message, "F");
                    //errortext = "Error while sending OTP";
                    CreateLogFiles Err = new CreateLogFiles();
                    Err.ErrorLog(HttpContext.Current.Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "accepted_send_sms_br_sro()---SendSms.cs");
                    // return "0";

                }
            
            }
        public void rejected_send_sms_br_sro(string mobileno)
        {
            try
            {
                string message;   
                message = "Your application for Society Registration is rejected by our Officer.";
                string smsline2 = "&signature=REGGOA&dlt_entity_id=1401406560000034950&dlt_template_id=1407162244789016560";
                if (errortext == "")
                {
                    string htmlEncodedMsg = message.Replace("#", "%23").Replace("&", "%26").Replace("$", "%24");
                    userAuthenticationURI = smsline1 + message + "&mnumber=" + mobileno + smsline2;
                    string jsonData = String.Empty;
                    using (WebClient client = new WebClient())
                    {

                        string s = client.DownloadString(userAuthenticationURI);
                        if (s.Contains("API000"))
                        {

                            //RecordUserAction("Contains ok in status", "Sms sent successfully", "S");
                        }
                        else
                        {

                            // RecordUserAction("ok is not thier in status", "Some issue delivering Message", "F");
                        }
                    }

                }

            }
            catch (Exception ex)
            {
              
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(HttpContext.Current.Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "accepted_send_sms_br_sro()---SendSms.cs");
              

            }
            
        }
        public Int64 generate_OTP_Code()
        {
            try
            {
                Int64 otpcode = 0;
                otpcode = Utility.GenerateRandomInt();
                return otpcode;
            }
            catch (Exception ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(HttpContext.Current.Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "generate_OTP_Code()----SendSms.cs");

                return 0;
            }
        }
    }
}