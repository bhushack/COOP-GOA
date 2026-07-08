using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace GoaSocietyRegistration.Development
{
    public partial class admincaptcha : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var capstr = Request.QueryString["capstr"];
            if (!string.IsNullOrWhiteSpace(capstr) && capstr.Length == 6)
            {
                Response.Clear();
                int height = 50;
                int width = 160;
                Bitmap bmp = new Bitmap(width, height);
                Font font = new Font("Thaoma", 14, FontStyle.Bold);
                Graphics g = Graphics.FromImage(bmp);
                font = FindBestFitFont(g, capstr.ToString(), font);
                SizeF size = g.MeasureString(capstr.ToString(), font);
                g.Clear(Color.Azure);
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                g.DrawString(capstr, font, new SolidBrush(Color.Crimson), (width - size.Width) / 2, (height - size.Height) / 2);
                g.DrawRectangle(new Pen(Color.Aquamarine), 1, 1, width - 2, height - 2);
                g.FillRectangle(new HatchBrush(HatchStyle.Percent20, Color.DimGray, Color.Transparent), g.ClipBounds);
                g.Flush();
                Response.ContentType = "image/jpeg";
                bmp.Save(Response.OutputStream, ImageFormat.Jpeg);
                g.Dispose();
                bmp.Dispose();
            }
            else
                return;
        }

        private Font FindBestFitFont(Graphics g, string text, Font font)
        {
            while (true)
            {
                SizeF size = g.MeasureString(text, font);
                // It fits, back out
                if (size.Height <= 35 && size.Width <= 120) { return font; }
                // Try a smaller font (90% of old size)
                Font oldFont = font;
                font = new Font(font.Name, (float)(font.Size * .9), font.Style);
                oldFont.Dispose();
            }
        }
    }
}