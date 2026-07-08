using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;

namespace GoaSocietyRegistration.Development
{
    public class SurroundingClass
    {
        public static string ReadStreamWithTimeout(StreamReader reader)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            int intC;
            StringBuilder sb = new StringBuilder();
            while ((intC = reader.Read()) != -1 && sw.ElapsedMilliseconds < 15000)
            {
                char c = (char)intC;
                sb.Append(c);
            }
            sw.Stop();
            return sb.ToString();
        }

        private partial class CSharpImpl
        {
            [Obsolete("Please refactor calling code to use normal Visual Basic assignment")]
            public static T __Assign<T>(ref T target, T value)
            {
                target = value;
                return value;
            }
        }
    }
}