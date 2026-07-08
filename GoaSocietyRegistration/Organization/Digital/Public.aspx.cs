using GoaSocietyRegistration.Development;
using GoaSocietyRegistration.Organization.Digital.Design;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;

namespace GoaSocietyRegistration.Organization.Digital
{
    public partial class Public : System.Web.UI.Page
    {
        string ipaddress = Utility.getIP();
        protected void Page_Load(object sender, EventArgs e)
        {
            CreateLogFiles Err = new CreateLogFiles();
            try
            {
                if (Request.QueryString["mode"].ToString() == "ServerDate")
                {
                    Response.ContentType = "text/plain";
                    Response.Write(DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt"));
                }
                else if (Request.QueryString["mode"].ToString() == "CreatePayload")
                {
                    SignatureUserData UserData = new SignatureUserData()
                    {
                        UserId = "SESSION-UID",
                        UserName = "SESSION-NAME",
                        Designation = "SESSION-DESIGNATION",
                        Contact = "SESSION-CONTACT",
                        Location = "",
                        //Reason = "",
                    };
                    APIResponse resp = new APIResponse();
                    Session["AES"] = Convert.ToBase64String(DigitalEncrypt.DigitalEncrypt.CreateAesKey());
                    try
                    {
                        if ((Session["username"] != null || Session["username"].ToString() != "") && (Session["app_id"] != null || Session["app_id"].ToString() != ""))
                        {
                            resp.Result = DSCSign.CreatePayLoad(Request.Params["ApplnRefNo"].ToString(), Session["AES"].ToString(), Request.Params["Continue"].ToString(), UserData, Session["app_id"].ToString(), Session["username"].ToString());

                            //int value = Utility.ins_data_sign(Session["username"].ToString(), Session["app_id"].ToString());
                            //if (value == 1)
                            //{
                            //      }
                        }
                    }
                    catch (Exception ex)
                    {
                        resp.ErrorCode = 500;
                        resp.ErrorMessage = "Something went wrong. Please try again.";
                        Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "Page_Load--CreatePayload" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath) + " from " + ipaddress);

                    }
                    Response.ContentType = "application/json";
                    Response.Write(JsonConvert.SerializeObject(resp));
                }
                else if (Request.QueryString["mode"].ToString() == "CreatePayloadThirdParty")
                {
                    SignatureUserData UserData = new SignatureUserData()
                    {
                        UserId = "SESSION-UID",
                        UserName = "SESSION-NAME",
                        Designation = "SESSION-DESIGNATION",
                        Contact = "SESSION-CONTACT",
                        Location = "",
                        //Reason = "",
                    };
                    APIResponse resp = new APIResponse();
                    Session["AES"] = Convert.ToBase64String(DigitalEncrypt.DigitalEncrypt.CreateAesKey());
                    try
                    {
                        if ((Session["username"] != null || Session["username"].ToString() != "") && (Session["ref_id"] != null || Session["ref_id"].ToString() != "") && (Session["app_id"] != null || Session["app_id"].ToString() != ""))
                        {
                            //resp.Result = DSCSign.CreatePayLoad_thirdParty(Request.Params["ApplnRefNo"].ToString(), Session["AES"].ToString(), Request.Params["Continue"].ToString(), UserData, Session["app_id"].ToString(), Session["username"].ToString());
                            //int value = Utility.ins_data_sign(Session["username"].ToString(), Session["ref_id"].ToString());
                            //if (value == 1)
                            //{

                            //}
                        }
                    }
                    catch (Exception ex)
                    {
                        resp.ErrorCode = 500;
                        resp.ErrorMessage = "Something went wrong. Please try again.";
                        Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "Page_Load--CreatePayload" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath) + " from " + ipaddress);

                    }

                    Response.ContentType = "application/json";
                    Response.Write(JsonConvert.SerializeObject(resp));


                }

                else if (Request.QueryString["mode"].ToString() == "CreatePayloadRegister")
                {

                    SignatureUserData UserData = new SignatureUserData()
                    {
                        UserId = "SESSION-UID",
                        UserName = "SESSION-NAME",
                        Designation = "SESSION-DESIGNATION",
                        Contact = "SESSION-CONTACT",
                        Location = "Goa",
                        Reason = "To Certify document.",
                    };
                    APIResponse resp = new APIResponse();
                    Session["AES"] = Convert.ToBase64String(DigitalEncrypt.DigitalEncrypt.CreateAesKey());
                    try
                    {
                        CreatePDF create = new CreatePDF();
                        int a = create.createpdf();
                        if (a == 1)
                        {

                            resp.Result = DSCSign.CreatePayLoad_reg(Request.Params["ApplnRefNo"].ToString(), Session["AES"].ToString(), Request.Params["Continue"].ToString(), UserData, Session["docname"].ToString());
                        }
                        else
                        {
                            resp.ErrorCode = 500;
                            Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), "" + " " + "", "Page_Load--CreatePayloadRegister pdf failed" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath) + " from " + ipaddress);
                            resp.ErrorMessage = "Something went wrong. Please try again.(pdf)";
                        }
                    }
                    catch (Exception ex)
                    {
                        resp.ErrorCode = 500;
                        resp.ErrorMessage = "Something went wrong. Please try again.";
                        Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "Page_Load--CreatePayloadRegister" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath) + " from " + ipaddress);
                    }
                    Response.ContentType = "application/json";
                    Response.Write(JsonConvert.SerializeObject(resp));
                }
                 
                else if (Request.QueryString["mode"].ToString() == "SavePayload")
                {
                    APIResponse resp = new APIResponse();
                    try
                    {
                        if (Session["AES"] != null)
                        {
                            string encValue = DigitalEncrypt.DigitalEncrypt.AESDecrypt(Request.Params["Payload"].ToString(), Session["AES"].ToString());
                            string xml = Encoding.UTF8.GetString(Convert.FromBase64String(encValue));
                            XmlDocument doc = new XmlDocument();
                            doc.XmlResolver = null;
                            doc.LoadXml(xml);
                            string IPv4 = "";
                            string IPv6 = "";
                            string MAC = "";
                            string Cert = "";
                            XmlNodeList nodeList = doc.GetElementsByTagName("System");
                            foreach (XmlNode node in nodeList)
                            {
                                XmlNode n = node.SelectSingleNode("IPV4");
                                if (n != null)
                                {
                                    IPv4 = n.InnerText;
                                }
                                n = node.SelectSingleNode("IPV6");
                                if (n != null)
                                {
                                    IPv6 = n.InnerText;
                                }
                                n = node.SelectSingleNode("MAC");
                                if (n != null)
                                {
                                    MAC = n.InnerText;
                                }
                                n = node.SelectSingleNode("Cert");
                                if (n != null)
                                {
                                    Cert = n.InnerText;
                                }
                            }
                            X509Certificate2 card = new X509Certificate2(Convert.FromBase64String(Cert));

                            string temp = card.Subject;
                            string stringAfterChar = temp.Substring(temp.IndexOf("CN="));

                            string[] splitsubject = stringAfterChar.Split(',');
                            string name = splitsubject[0].Substring(3).Trim();
                            // data.Name = name;

                            //string temp = card.Subject;
                            //string[] splitsubject = temp.Split(',');
                            //string name = splitsubject[0].Substring(3).Trim();
                            string loggedin = Session["userfirstname"].ToString().Trim() + " " + Session["usermiddlename"].ToString().Trim() + " " + Session["userlastname"].ToString().Trim();
                            if (Session["usermiddlename"].ToString() == "")
                            {
                                loggedin = Session["userfirstname"].ToString().Trim() + " " + Session["userlastname"].ToString().Trim();
                            }
                            if (loggedin.ToUpper().Equals(name.ToUpper()))
                            {
                                nodeList = doc.GetElementsByTagName("Data");
                                foreach (XmlNode node in nodeList)
                                {
                                    string ApplnRefNo = node.Attributes["ID"].Value;
                                    string type = node.Attributes["Type"].Value;
                                    string data = node.InnerText; long result = 0;
                                    switch (type.ToLower())
                                    {
                                        case "pdf":
                                            File.WriteAllBytes(Server.MapPath("~/OutData/Certificate_" + Session["app_id"].ToString() + "[temp][file].pdf"), Convert.FromBase64String(data));
                                            byte[] file = File.ReadAllBytes(Server.MapPath("~/OutData/Certificate_" + Session["app_id"].ToString() + "[temp][file].pdf"));

                                            if (file != null)
                                            {
                                                result = 1;
                                            }
                                            else
                                            {
                                                result = 0;
                                            }
                                            //  File.WriteAllBytes(Server.MapPath("~/tmp/" + ApplnRefNo + ".pdf"), Convert.FromBase64String(data));


                                            if (result > 0)
                                            {

                                                int a = 1;// Utility.upload_Signed_Certi(file, Session["app_id"].ToString(), Session["username"].ToString(), false, Session["echallan_certificate"].ToString(), Session["certi_outward"].ToString(), loggedin, "");
                                                if (a == 1)
                                                {
                                                    resp.Result = "Success";
                                                    Session["Msg"] = "Successfully signed and uploaded certificate";
                                                }
                                                else
                                                {
                                                    resp.Result = "";
                                                    resp.ErrorCode = 500;
                                                    resp.ErrorMessage = "Not able to Upload signed certificate.";
                                                    Session["Msg"] = "NotSuccess";
                                                }
                                            }
                                            else
                                            {
                                                resp.Result = "";
                                                resp.ErrorCode = 500;
                                                resp.ErrorMessage = "Not able to save signed certificate.";
                                                Session["Msg"] = "NotSuccess";
                                            }
                                            break;
                                        case "text":
                                            File.WriteAllBytes(Server.MapPath("~/OutData/" + Session["app_id"].ToString() + "text.txt"), Convert.FromBase64String(data));

                                            //File.WriteAllText(Server.MapPath("~/tmp/text.txt"), data);

                                            result = 1;
                                            if (result > 0)
                                            {
                                                resp.Result = "Success";
                                                Session["Msg"] = "Success";
                                            }
                                            else
                                            {
                                                resp.Result = "";
                                                resp.ErrorCode = 500;
                                                resp.ErrorMessage = "Not able to save signed text.";
                                                Session["Msg"] = "NotSuccess";
                                            }
                                            break;
                                    }
                                }
                            }
                            else
                            {
                                resp.ErrorCode = 500;
                                resp.ErrorMessage = "Username and DSC mismatch";
                            }
                        }
                        else
                        {
                            resp.ErrorCode = 500;
                            resp.ErrorMessage = "Session timed out. Please try again.";
                        }
                    }
                    catch (Exception ex)
                    {
                        resp.ErrorCode = 500;
                        resp.ErrorMessage = "Something went wrong. Please try again.";
                        Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "Page_Load--SavePayload" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath) + " from " + ipaddress);
                    }
                    Response.ContentType = "application/json";
                    Response.Write(JsonConvert.SerializeObject(resp));
                }
                ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                else if (Request.QueryString["mode"].ToString() == "SavePayloadThirdParty")
                {
                    APIResponse resp = new APIResponse();
                    try
                    {
                        if (Session["AES"] != null)
                        {
                            string encValue = DigitalEncrypt.DigitalEncrypt.AESDecrypt(Request.Params["Payload"].ToString(), Session["AES"].ToString());
                            string xml = Encoding.UTF8.GetString(Convert.FromBase64String(encValue));
                            XmlDocument doc = new XmlDocument();
                            doc.XmlResolver = null;
                            doc.LoadXml(xml);
                            string IPv4 = "";
                            string IPv6 = "";
                            string MAC = "";
                            string Cert = "";
                            XmlNodeList nodeList = doc.GetElementsByTagName("System");
                            foreach (XmlNode node in nodeList)
                            {
                                XmlNode n = node.SelectSingleNode("IPV4");
                                if (n != null)
                                {
                                    IPv4 = n.InnerText;
                                }
                                n = node.SelectSingleNode("IPV6");
                                if (n != null)
                                {
                                    IPv6 = n.InnerText;
                                }
                                n = node.SelectSingleNode("MAC");
                                if (n != null)
                                {
                                    MAC = n.InnerText;
                                }
                                n = node.SelectSingleNode("Cert");
                                if (n != null)
                                {
                                    Cert = n.InnerText;
                                }
                            }
                            X509Certificate2 card = new X509Certificate2(Convert.FromBase64String(Cert));
                            string temp = card.Subject;
                            string stringAfterChar = temp.Substring(temp.IndexOf("CN="));

                            string[] splitsubject = stringAfterChar.Split(',');
                            string name = splitsubject[0].Substring(3).Trim();



                            //string temp = card.Subject;
                            // string[] splitsubject = temp.Split(',');
                            // string name = splitsubject[0].Substring(3);
                            string loggedin = Session["userfirstname"].ToString().Trim() + " " + Session["usermiddlename"].ToString().Trim() + " " + Session["userlastname"].ToString().Trim();
                            if (Session["usermiddlename"].ToString() == "")
                            {
                                loggedin = Session["userfirstname"].ToString().Trim() + " " + Session["userlastname"].ToString().Trim();
                            }
                            if (loggedin.ToUpper().Equals(name.ToUpper()))
                            {
                                nodeList = doc.GetElementsByTagName("Data");
                                foreach (XmlNode node in nodeList)
                                {
                                    string ApplnRefNo = node.Attributes["ID"].Value;
                                    string type = node.Attributes["Type"].Value;
                                    string data = node.InnerText; long result = 0;
                                    switch (type.ToLower())
                                    {
                                        case "pdf":
                                            File.WriteAllBytes(Server.MapPath("~/OutData/Certificate_" + Session["ref_id"].ToString() + "[temp][file].pdf"), Convert.FromBase64String(data));
                                            byte[] file = File.ReadAllBytes(Server.MapPath("~/OutData/Certificate_" + Session["ref_id"].ToString() + "[temp][file].pdf"));

                                            if (file != null)
                                            {
                                                result = 1;
                                            }
                                            else
                                            {
                                                result = 0;
                                            }
                                            //  File.WriteAllBytes(Server.MapPath("~/tmp/" + ApplnRefNo + ".pdf"), Convert.FromBase64String(data));


                                            if (result > 0)
                                            {

                                                int a = 1;// Utility.upload_Signed_Certi(file, Session["ref_id"].ToString(), Session["username"].ToString(), true, Session["echallan_certificate"].ToString(), Session["certi_outward"].ToString(), loggedin, "");
                                                if (a == 1)
                                                {
                                                    resp.Result = "Success";
                                                    Session["Msg"] = "Successfully signed and uploaded certificate";
                                                }
                                                else
                                                {
                                                    resp.Result = "";
                                                    resp.ErrorCode = 500;
                                                    resp.ErrorMessage = "Not able to Upload signed certificate.";
                                                    Session["Msg"] = "NotSuccess";
                                                }
                                            }
                                            else
                                            {
                                                resp.Result = "";
                                                resp.ErrorCode = 500;
                                                resp.ErrorMessage = "Not able to save signed certificate.";
                                                Session["Msg"] = "NotSuccess";
                                            }
                                            break;
                                        case "text":
                                            File.WriteAllBytes(Server.MapPath("~/OutData/" + Session["ref_id"].ToString() + "text.txt"), Convert.FromBase64String(data));

                                            //File.WriteAllText(Server.MapPath("~/tmp/text.txt"), data);

                                            result = 1;
                                            if (result > 0)
                                            {
                                                resp.Result = "Success";
                                                Session["Msg"] = "Success";
                                            }
                                            else
                                            {
                                                resp.Result = "";
                                                resp.ErrorCode = 500;
                                                resp.ErrorMessage = "Not able to save signed text.";
                                                Session["Msg"] = "NotSuccess";
                                            }
                                            break;
                                    }
                                }
                            }
                            else
                            {
                                resp.ErrorCode = 500;
                                resp.ErrorMessage = "Username and DSC mismatch";
                            }
                        }
                        else
                        {
                            resp.ErrorCode = 500;
                            resp.ErrorMessage = "Session timed out. Please try again.";
                        }
                    }
                    catch (Exception ex)
                    {
                        resp.ErrorCode = 500;
                        resp.ErrorMessage = "Something went wrong. Please try again.";
                        Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "Page_Load--SavePayload3rd party" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath) + " from " + ipaddress);
                    }
                    Response.ContentType = "application/json";
                    Response.Write(JsonConvert.SerializeObject(resp));
                }
                ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                else if (Request.QueryString["mode"].ToString() == "CreateVerificationPayload")
                {
                    string Type = "TEXT";
                    string Data = "SOME TEXT";
                    string SigData = File.ReadAllText(Server.MapPath("~/OutData/" + Session["app_id"].ToString() + "text.txt"));
                    // string SigData = File.ReadAllText(Server.MapPath("~/tmp/text.txt"));
                    APIResponse resp = new APIResponse();
                    Session["AES"] = Convert.ToBase64String(DigitalEncrypt.DigitalEncrypt.CreateAesKey());
                    try
                    {
                        resp.Result = SigVerify.CreatePayLoad(Type, Data, SigData, Session["AES"].ToString());
                    }
                    catch (Exception ex)
                    {
                        resp.ErrorCode = 500;
                        resp.ErrorMessage = "Something went wrong. Please try again.";
                        Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "Page_Load--CreateVerificationPayload" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath) + " from " + ipaddress);

                    }

                    Response.ContentType = "application/json";
                    Response.Write(JsonConvert.SerializeObject(resp));
                }
                else if (Request.QueryString["mode"].ToString() == "SaveVerifyPayload")
                {
                    APIResponse resp = new APIResponse();

                    try
                    {
                        if (Session["AES"] != null)
                        {
                            string encValue = DigitalEncrypt.DigitalEncrypt.AESDecrypt(Request.Params["Payload"].ToString(), Session["AES"].ToString());

                            if (encValue == "true")
                            {
                                resp.Result = "Verified";
                            }
                            else
                            {
                                resp.Result = "Not Verified";
                            }
                        }
                        else
                        {
                            resp.ErrorCode = 500;
                            resp.ErrorMessage = "Session timed out. Please try again.";
                        }
                    }
                    catch (Exception ex)
                    {
                        resp.ErrorCode = 500;
                        resp.ErrorMessage = "Something went wrong. Please try again.";
                        Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "Page_Load--SaveVerifyPayload" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath) + " from " + ipaddress);

                    }

                    Response.ContentType = "application/json";
                    Response.Write(JsonConvert.SerializeObject(resp));

                }
                else if (Request.QueryString["mode"].ToString() == "SavePayload")
                {
                    APIResponse resp = new APIResponse();
                    try
                    {
                        if (Session["AES"] != null)
                        {
                            string encValue = DigitalEncrypt.DigitalEncrypt.AESDecrypt(Request.Params["Payload"].ToString(), Session["AES"].ToString());
                            string xml = Encoding.UTF8.GetString(Convert.FromBase64String(encValue));
                            XmlDocument doc = new XmlDocument();
                            doc.XmlResolver = null;
                            doc.LoadXml(xml);
                            string IPv4 = "";
                            string IPv6 = "";
                            string MAC = "";
                            string Cert = "";
                            XmlNodeList nodeList = doc.GetElementsByTagName("System");
                            foreach (XmlNode node in nodeList)
                            {
                                XmlNode n = node.SelectSingleNode("IPV4");
                                if (n != null)
                                {
                                    IPv4 = n.InnerText;
                                }
                                n = node.SelectSingleNode("IPV6");
                                if (n != null)
                                {
                                    IPv6 = n.InnerText;
                                }
                                n = node.SelectSingleNode("MAC");
                                if (n != null)
                                {
                                    MAC = n.InnerText;
                                }
                                n = node.SelectSingleNode("Cert");
                                if (n != null)
                                {
                                    Cert = n.InnerText;
                                }
                            }
                            X509Certificate2 card = new X509Certificate2(Convert.FromBase64String(Cert));

                            string temp = card.Subject;
                            string stringAfterChar = temp.Substring(temp.IndexOf("CN="));

                            string[] splitsubject = stringAfterChar.Split(',');
                            string name = splitsubject[0].Substring(3).Trim();
                            // data.Name = name;

                            //string temp = card.Subject;
                            //string[] splitsubject = temp.Split(',');
                            //string name = splitsubject[0].Substring(3).Trim();
                            string loggedin = Session["userfirstname"].ToString().Trim() + " " + Session["usermiddlename"].ToString().Trim() + " " + Session["userlastname"].ToString().Trim();
                            if (Session["usermiddlename"].ToString() == "")
                            {
                                loggedin = Session["userfirstname"].ToString().Trim() + " " + Session["userlastname"].ToString().Trim();
                            }
                            if (loggedin.ToUpper().Equals(name.ToUpper()))
                            {
                                nodeList = doc.GetElementsByTagName("Data");
                                foreach (XmlNode node in nodeList)
                                {
                                    string ApplnRefNo = node.Attributes["ID"].Value;
                                    string type = node.Attributes["Type"].Value;
                                    string data = node.InnerText; long result = 0;
                                    switch (type.ToLower())
                                    {
                                        case "pdf":
                                            File.WriteAllBytes(Server.MapPath("~/OutData/Certificate_" + Session["app_id"].ToString() + "[temp][file].pdf"), Convert.FromBase64String(data));
                                            byte[] file = File.ReadAllBytes(Server.MapPath("~/OutData/Certificate_" + Session["app_id"].ToString() + "[temp][file].pdf"));

                                            if (file != null)
                                            {
                                                result = 1;
                                            }
                                            else
                                            {
                                                result = 0;
                                            }
                                            //  File.WriteAllBytes(Server.MapPath("~/tmp/" + ApplnRefNo + ".pdf"), Convert.FromBase64String(data));


                                            if (result > 0)
                                            {

                                                //int a = Utility.upload_Signed_Certi(file, Session["app_id"].ToString(), Session["username"].ToString(), false, Session["echallan_certificate"].ToString(), Session["certi_outward"].ToString(), loggedin, "");
                                                //if (a == 1)
                                                //{
                                                //    resp.Result = "Success";
                                                //    Session["Msg"] = "Successfully signed and uploaded certificate";
                                                //}
                                                //else
                                                //{
                                                //    resp.Result = "";
                                                //    resp.ErrorCode = 500;
                                                //    resp.ErrorMessage = "Not able to Upload signed certificate.";
                                                //    Session["Msg"] = "NotSuccess";
                                                //}
                                            }
                                            else
                                            {
                                                resp.Result = "";
                                                resp.ErrorCode = 500;
                                                resp.ErrorMessage = "Not able to save signed certificate.";
                                                Session["Msg"] = "NotSuccess";
                                            }
                                            break;
                                        case "text":
                                            File.WriteAllBytes(Server.MapPath("~/OutData/" + Session["app_id"].ToString() + "text.txt"), Convert.FromBase64String(data));

                                            //File.WriteAllText(Server.MapPath("~/tmp/text.txt"), data);

                                            result = 1;
                                            if (result > 0)
                                            {
                                                resp.Result = "Success";
                                                Session["Msg"] = "Success";
                                            }
                                            else
                                            {
                                                resp.Result = "";
                                                resp.ErrorCode = 500;
                                                resp.ErrorMessage = "Not able to save signed text.";
                                                Session["Msg"] = "NotSuccess";
                                            }
                                            break;
                                    }
                                }
                            }
                            else
                            {
                                resp.ErrorCode = 500;
                                resp.ErrorMessage = "Username and DSC mismatch";
                            }
                        }
                        else
                        {
                            resp.ErrorCode = 500;
                            resp.ErrorMessage = "Session timed out. Please try again.";
                        }
                    }
                    catch (Exception ex)
                    {
                        resp.ErrorCode = 500;
                        resp.ErrorMessage = "Something went wrong. Please try again.";
                        Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "Page_Load--SavePayload" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath) + " from " + ipaddress);
                    }
                    Response.ContentType = "application/json";
                    Response.Write(JsonConvert.SerializeObject(resp));
                }
            }
            catch (Exception ex)
            {
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "Page_Load" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath) + " from " + ipaddress);
            }
        }
    }
}