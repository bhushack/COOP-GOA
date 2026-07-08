using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Xml;

namespace GoaSocietyRegistration.Organization.Digital.Design
{
    public class SigVerify
    {
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

        private static XmlDocument CreateInputXML(string AppRefNo)
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

            string srvVerifyUrl = ConfigurationManager.AppSettings["DOMAIN_NAME"].ToString() + "/Verify.aspx";
            XmlText verifydataurllmText = doc.CreateTextNode(srvVerifyUrl);
            verifydatalm.AppendChild(verifydataurllmText);

            XmlElement input = doc.CreateElement(string.Empty, "Input", string.Empty);
            request.AppendChild(input);

            byte[] pdfToSign = File.ReadAllBytes(HttpContext.Current.Server.MapPath("~/doc.pdf"));
            XmlElement datalm = CreateDataElement(doc, AppRefNo, "PDF", Convert.ToBase64String(pdfToSign));
            input.AppendChild(datalm);

            datalm = CreateDataElement(doc, AppRefNo, "TEXT", "SOME TEXT TO BE SIGNED");
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

        public static string CreatePayLoad(string Type, string Data, string SignData, string AESKey)
        {
            //string inputData = Convert.ToBase64String(Encoding.UTF8.GetBytes(CreateInputXML(AppRefNo).InnerXml));
            string jsonReq = "{" +
                 "\"Verify\" :{\"Data\": \"" + Data + "\", \"Type\": \"" + Type + "\", \"SignData\":\"" + SignData + "\"}" +
                  "}";
            return EncryptPayload(jsonReq, AESKey);
        }
    }
}