using GoaSocietyRegistration.Development;
using GoaSocietyRegistration.Organization.Digital.Design;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Xml;

namespace GoaSocietyRegistration.Organization.Digital
{
    internal class DSCSign
    {
        static string ipaddress = Utility.getIP();
        private static XmlElement CreateDataElement(XmlDocument doc, string dataId, string dataType, string dataBase64)
        {
            XmlElement datalm = doc.CreateElement(string.Empty, "Data", string.Empty);
            XmlAttribute dataidAttr = doc.CreateAttribute("ID");
            dataidAttr.Value = dataId;
            datalm.Attributes.Append(dataidAttr);
            XmlAttribute datatypeAttr = doc.CreateAttribute("Type");
            datatypeAttr.Value = dataType;
            datalm.Attributes.Append(datatypeAttr);
            XmlText datalmText = doc.CreateTextNode(dataBase64);
            datalm.AppendChild(datalmText);
            return datalm;
        }

        private static XmlDocument CreateInputXML(string AppRefNo, string app_id)
        {
            XmlDocument doc = new XmlDocument();
            XmlDeclaration xmlDeclaration = doc.CreateXmlDeclaration("1.0", "UTF-8", null);
            XmlElement root = doc.DocumentElement;
            doc.InsertBefore(xmlDeclaration, root);

            XmlElement request = doc.CreateElement(string.Empty, "Request", string.Empty);
            doc.AppendChild(request);

            XmlElement verify = doc.CreateElement(string.Empty, "Verify", string.Empty);
            request.AppendChild(verify);

            XmlElement verifydatalm = doc.CreateElement(string.Empty, "DataURL", string.Empty);
            verify.AppendChild(verifydatalm);

            string srvVerifyUrl = ConfigurationManager.AppSettings["DOMAIN_NAME"].ToString() + "Organization/Digital/Verify";
            XmlText verifydataurllmText = doc.CreateTextNode(srvVerifyUrl);
            verifydatalm.AppendChild(verifydataurllmText);

            XmlElement input = doc.CreateElement(string.Empty, "Input", string.Empty);
            request.AppendChild(input);

            byte[] pdfToSign = File.ReadAllBytes(HttpContext.Current.Server.MapPath("~/OutData/Certificate_" + app_id + "[temp][file].pdf"));
            XmlElement datalm = CreateDataElement(doc, AppRefNo, "PDF", Convert.ToBase64String(pdfToSign));
            input.AppendChild(datalm);


            ////ForSigningText
            //datalm = CreateDataElement(doc, AppRefNo, "TEXT", "SOME TEXT");
            //input.AppendChild(datalm);

            return doc;
        }
        private static XmlDocument CreateInputXML_register(string AppRefNo, string name, int docname)
        {
            CreateLogFiles Err = new CreateLogFiles();
            XmlDocument doc = new XmlDocument();
            try
            {

                XmlDeclaration xmlDeclaration = doc.CreateXmlDeclaration("1.0", "UTF-8", null);
                XmlElement root = doc.DocumentElement;
                doc.InsertBefore(xmlDeclaration, root);

                XmlElement request = doc.CreateElement(string.Empty, "Request", string.Empty);
                doc.AppendChild(request);

                XmlElement verify = doc.CreateElement(string.Empty, "Verify", string.Empty);
                request.AppendChild(verify);

                XmlElement verifydatalm = doc.CreateElement(string.Empty, "DataURL", string.Empty);
                verify.AppendChild(verifydatalm);

                string srvVerifyUrl = ConfigurationManager.AppSettings["DOMAIN_NAME"].ToString() + "Organization/Digital/VerifyRegister";//?district=" + HttpContext.Current.Session["DistrictID"];


                XmlText verifydataurllmText = doc.CreateTextNode(srvVerifyUrl);
                verifydatalm.AppendChild(verifydataurllmText);

                XmlElement input = doc.CreateElement(string.Empty, "Input", string.Empty);
                request.AppendChild(input);

                byte[] pdfToSign = File.ReadAllBytes(HttpContext.Current.Server.MapPath("~/OutData/" + name + ".pdf"));

                XmlElement datalm = CreateDataElement(doc, AppRefNo, "PDF", Convert.ToBase64String(pdfToSign));
                input.AppendChild(datalm);
            }
            catch (Exception ex)

            {
                Err.ErrorLog(HttpContext.Current.Server.MapPath("~/Logs/ErrorLog"), "exception:" + ex.Message + ex.StackTrace, "Page_Load" + " " + "DSC Sign" + " from " + ipaddress);
            }
            return doc;

        }
         
        private static XmlDocument CreateInputXML_sign(string AppRefNo, string app_id, int docname, string name)
        {
            XmlDocument doc = new XmlDocument();
            XmlDeclaration xmlDeclaration = doc.CreateXmlDeclaration("1.0", "UTF-8", null);
            XmlElement root = doc.DocumentElement;
            doc.InsertBefore(xmlDeclaration, root);

            XmlElement request = doc.CreateElement(string.Empty, "Request", string.Empty);
            doc.AppendChild(request);

            XmlElement verify = doc.CreateElement(string.Empty, "Verify", string.Empty);
            request.AppendChild(verify);

            XmlElement verifydatalm = doc.CreateElement(string.Empty, "DataURL", string.Empty);
            verify.AppendChild(verifydatalm);

            string srvVerifyUrl = ConfigurationManager.AppSettings["DOMAIN_NAME"].ToString() + "Organization/Digital/Verify";
            XmlText verifydataurllmText = doc.CreateTextNode(srvVerifyUrl);
            verifydatalm.AppendChild(verifydataurllmText);

            XmlElement input = doc.CreateElement(string.Empty, "Input", string.Empty);
            request.AppendChild(input);

            byte[] pdfToSign = File.ReadAllBytes(HttpContext.Current.Server.MapPath("~/OutData/Certificate_" + app_id + "[temp][file].pdf"));
            XmlElement datalm = CreateDataElement(doc, AppRefNo, "PDF", Convert.ToBase64String(pdfToSign));
            input.AppendChild(datalm);

            return doc;
        }
        private static string EncryptPayload(string text, string AESKEY)
        {
            string req = DigitalEncrypt.DigitalEncrypt.AESEncrypt(text, AESKEY);
            string token = DigitalEncrypt.DigitalEncrypt.RSAEncryptPayload(Encoding.UTF8.GetBytes(AESKEY));
            string json = "{\"req\":\"" + req + "\", \"token\":\"" + token + "\"}";
            return json;
        }

        public static string CreatePayLoad(string AppRefNo, string AESKey, string Continue, SignatureUserData UserData, string app_ref_id, string name)
        {

            System.Web.HttpContext.Current.Session["card_data"] = 0;
            string inputData = Convert.ToBase64String(Encoding.UTF8.GetBytes(CreateInputXML_sign(AppRefNo, app_ref_id, 0, name).InnerXml));
            string srvDateUrl = ConfigurationManager.AppSettings["DOMAIN_NAME"].ToString() + "Organization/Digital/Public?mode=ServerDate";
            string jsonReq = "{" +
                 "\"AppProp\" :{\"Mode\": \"DigitalSign\", \"InputData\": \"" + inputData + "\", \"ServerDateURL\":\"" + srvDateUrl + "\", \"ApplySessionHack\":\"" + Continue + "\"}," +
                  "\"SignProp\" :{\"SignerRole\": \"" + UserData.Designation + "\", \"State\":\"Goa\", \"City\":\"" + UserData.Location + "\", \"Country\":\"India\", \"PostalCode\":\"\", \"Reason\":\"" + UserData.Reason + "\",\"SignPos\":{\"llx\":390, \"lly\":60, \"urx\":580, \"ury\":100}}" +
                  "}";
            return EncryptPayload(jsonReq, AESKey);
        }

       
        public static string CreatePayLoad_reg(string AppRefNo, string AESKey, string Continue, SignatureUserData UserData, string name)
        {

            System.Web.HttpContext.Current.Session["card_data"] = 1;

            string inputData = Convert.ToBase64String(Encoding.UTF8.GetBytes(CreateInputXML_register(AppRefNo, name, 1).InnerXml));
            string srvDateUrl = ConfigurationManager.AppSettings["DOMAIN_NAME"].ToString() + "Organization/Digital/Public?mode=ServerDate";
            string jsonReq = "{" +
                 "\"AppProp\" :{\"Mode\": \"DigitalSign\", \"InputData\": \"" + inputData + "\", \"ServerDateURL\":\"" + srvDateUrl + "\", \"ApplySessionHack\":\"" + Continue + "\"}," +
                  "\"SignProp\" :{\"SignerRole\": \"" + UserData.Designation + "\", \"State\":\"Goa\", \"City\":\"" + UserData.Location + "\", \"Country\":\"India\", \"PostalCode\":\"\", \"Reason\":\"" + UserData.Reason + "\",\"SignPos\":{\"llx\":360, \"lly\":5, \"urx\":630, \"ury\":43}}" +
                  "}";
            return EncryptPayload(jsonReq, AESKey);
        }
    }
}