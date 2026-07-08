using GoaSocietyRegistration.Development;
using GoaSocietyRegistration.Organization.Digital.Design;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace GoaSocietyRegistration.Organization.Digital
{
    public partial class VerifyRegister : System.Web.UI.Page
    {
        string ipaddress = Utility.getIP();
        System.Globalization.CultureInfo french = new System.Globalization.CultureInfo("fr-FR");
        protected void Page_Load(object sender, EventArgs e)
        {
            //HttpContext.Current.Response.AppendHeader("Access-Control-Allow-Origin", "https://reg.goa.gov.in/");//*
            CreateLogFiles Err = new CreateLogFiles();

            try
            {
                string content = string.Empty;
                using (var reader = new StreamReader(Request.InputStream))
                    //content = reader.ReadToEnd();
                    content = SurroundingClass.ReadStreamWithTimeout(reader);
                X509Certificate2 card = new X509Certificate2(Convert.FromBase64String(content));
                CardData data = new CardData();
                data.publicketString = card.GetPublicKeyString();
                data.rawdataString = card.GetRawCertDataString();
                data.serialnumberString = card.GetSerialNumberString();
                data.Issuer = card.Issuer;
                data.NotAfter = Convert.ToDateTime(card.NotAfter, french);
                data.NotBefore = Convert.ToDateTime(card.NotBefore, french);
                data.version = card.Version.ToString();
                string temp = card.Subject;
                string issuername = card.IssuerName.ToString();
                string stringAfterChar = temp.Substring(temp.IndexOf("CN="));

                string[] splitsubject = stringAfterChar.Split(',');
                string name = splitsubject[0].Substring(3).Trim();
                data.Name = name;

                if (card.NotAfter < DateTime.Now)
                {
                    Response.ContentType = "text/plain";
                    Response.Write("0");
                }
                else
                {
                    int a = Utility.save_card_data(data, 0);
                    if (a == 1)
                    {
                        Response.ContentType = "text/plain";
                        Response.Write("1");
                    }
                    else if (a == 2)
                    {
                        Response.ContentType = "text/plain";
                        Response.Write("2");
                    }
                    else if (a == -1)
                    {
                        Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), "save card:" + a, "Page_Load" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath) + " from " + ipaddress);

                        //Response.ContentType = "text/plain";
                        //Response.Write("-1");
                    }

                    //Response.Write("<script>alert('Card Data saved!');</script>");


                }
            }
            catch (Exception ex)
            {
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "Page_Load" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath) + " from " + ipaddress);
            }

        }
    }
}