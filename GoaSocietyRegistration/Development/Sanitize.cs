using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace GoaSocietyRegistration.Development
{
    public class Sanitize
    {
        public static string InputText(string Value)
        {
            Value = HttpUtility.HtmlEncode(Value.Replace("'", "''").Replace(";", ""));
            return Value;
        }

        public static string FileName(string Value)
        {
            if (Path.IsPathRooted(Value))
            {
                throw new ArgumentNullException("error");
            }

            return string.Join("_", Value.Split(Path.GetInvalidFileNameChars()));
        }
        internal static object System()
        {
            throw new NotImplementedException();
        }
    }
}