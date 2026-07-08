using GoaSocietyRegistration.Development;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace GoaSocietyRegistration.Organization.Digital
{
    public partial class CreatePDF : System.Web.UI.Page
    {
        string ipaddress = Utility.getIP();
        protected void Page_Load(object sender, EventArgs e)
        {
            createpdf();
        }
        public int createpdf()
        {
            int a = 0;
            Document document = new Document(PageSize.A4, 10, 10, 10, 10);
            FileStream fs = new FileStream(Server.MapPath("~/OutData/" + Session["docname"].ToString() + ".pdf"), FileMode.Create, FileAccess.Write);
            PdfWriter writer = PdfWriter.GetInstance(document, fs);
            try
            {
                // Session["docname"] = "abc";
                using (System.IO.MemoryStream memoryStream = new System.IO.MemoryStream())
                {


                    // PdfWriter writer = PdfWriter.GetInstance(document, memoryStream);
                    document.Open();

                    Chunk chunk = new Chunk("DSC Registered successfully.");
                    document.Add(chunk);

                    document.Close();
                    byte[] bytes = memoryStream.ToArray();
                    //byte[] bytes = System.IO.File.ReadAllBytes(Server.MapPath(@"~/PDFData/" + "SocietyCertificate_" +  ".pdf"));
                    memoryStream.Close();
                    //Response.Clear();
                    //Response.ContentType = "application/pdf";
                    // string[] fname = Session["username"].ToString().Split();
                    // string pdfName = "User";// + fname[0].ToString();
                    //Response.AddHeader("Content-Disposition", "attachment; filename=" + pdfName + ".pdf");
                    //Response.ContentType = "application/pdf";
                    //Response.Buffer = true;
                    //Response.Cache.SetCacheability(System.Web.HttpCacheability.NoCache);
                    //Response.BinaryWrite(bytes);
                    ////Response.End();
                    //Response.Close();
                    a = 1;
                }
                writer.Close();// done for audit
                fs.Close();// done for audit
            }
            catch (Exception ex)
            {
                CreateLogFiles Err = new CreateLogFiles();
                Err.ErrorLog(Server.MapPath("~/Logs/ErrorLog"), ex.Message + " " + ex.StackTrace, "createpdf()" + " " + Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath) + " from " + ipaddress);
                a = 0;
            }
            finally
            {

                if (fs != null)
                {
                    fs.Close();
                }

            }
            return a;
        }
    }
}