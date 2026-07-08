using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Web;

namespace GoaSocietyRegistration.Development
{
    public class PDFiumWrapper
    {
        [DllImport("pdfium.dll", CharSet = CharSet.Unicode)]
        private static extern IntPtr FPDF_LoadDocument(string filePath, string password);

        [DllImport("pdfium.dll")]
        private static extern int FPDF_GetMetaText(IntPtr document, string tag, byte[] buffer, int buflen);

        [DllImport("pdfium.dll")]
        private static extern void FPDF_CloseDocument(IntPtr document);

        public static string GetMetadata(string filePath)
        {
            IntPtr doc = FPDF_LoadDocument(filePath, null);
            if (doc == IntPtr.Zero)
            {
                throw new Exception("Failed to load PDF document.");
            }

            byte[] buffer = new byte[4096]; // Adjust the buffer size as needed
            int length = FPDF_GetMetaText(doc, "Title", buffer, buffer.Length);
            if (length <= 0)
            {
                FPDF_CloseDocument(doc);
                throw new Exception("Failed to extract metadata.");
            }

            string metadata = System.Text.Encoding.UTF8.GetString(buffer, 0, length);

            FPDF_CloseDocument(doc);

            return metadata;
        }
    }
}