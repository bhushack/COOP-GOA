using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;

namespace GoaSocietyRegistration
{
    public class Validate
    {
        public readonly String alpharegex = @"^[a-zA-Z\s]+$";
        public readonly String alpha_numericregex = @"^[a-zA-Z0-9\s]+$";
        public readonly String numericregex = @"^[0-9]+$";
        public readonly String emailformatregex = @"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$";
        public readonly String weburlregex = @"(?<http>(http:[/][/]|www.)([a-z]|[A-Z]|[0-9]|[/.]|[~])*)";
        public readonly String percentageregex = @"(^(100(?:\.0{1,2})?))|(?!^0*$)(?!^0*\.0*$)^\d{1,2}(\.\d{1,2})?$";
        public readonly String decimalregex = @"^[1-9]\d{1,6}(\.\d{1,2})?$";
        public readonly String ipaddressregex = @"^(?:(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){3}(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)";
        public readonly String imageregex = @"([^\s]+(?=\.(jpeg|jpg|gif|png|JPEG|JPG|GIF|PNG))\.\2)";
        public readonly String dateregex = @"^(3[01]|[12][0-9]|0[1-9])/(1[0-2]|0[1-9])/[0-9]{4}$";
        public readonly String alpha_spcl_chars_regex = @"[a-zA-Z0-9@#$%&*+\-_(),+:;?.,![\]\s\\/]+$";
        public readonly String mobile_regex = @"^[6-9][0-9]{9}$";
        //public readonly String pincode_regex = @"^[0-9]{6}$";
        public readonly String pincode_regex = @"^[1-9]{1}[0-9]{5}$";
        public readonly String echallanno_regex = @"^[0-9]{12}$";
        public readonly String xml_regex = @"[a-zA-Z0-9@#$%&*+\-_(),+:;?.,![\]\s\\/<>]+$";
        public readonly String query_regex = @"[a-zA-Z0-9%&*+-_(),+?.,[\]\s\\/<>']+$";
        public readonly String ddl_regex = @"^[a-zA-Z0-9?\-_(),.\s\/|]+$";
        public readonly String string_builder_regex = @"^[a-zA-Z0-9@#$%&*+\-_(),^+:;?.,![\]\s\\/|~=<>'""]+$";
        public readonly String decimal_number_regex = @"^\d{1,18}(?:\.\d{1,2})?$";
        public readonly String statusdesc_regex = @"^[a-zA-Z0-9\-_(),+:;?.,![\]\s\\/]+$";
        public readonly String head_regex = @"^[0-9]{13}$";
        //nagesh
        public readonly String address = @"^[\sa-zA-Z0-9()-]+$";
        public readonly String pancard = @"^([a-zA-Z]){5}([0-9]){4}([a-zA-Z]){1}?$";
        public readonly String macaddress = @"^[a-fA-F0-9:]{17}|[a-fA-F0-9]{12}$";//mac address
        public readonly String epicno = @"^([a-zA-Z]){3}([0-9]){7}?$";//election card no
        public readonly String selfpassword = @"[a-zA-Z0-9@#$&*+\-_(),+:;?.,=![\]\s\\/]+$";
        public readonly String drivingleave = @"^([A-Z]{2})(\d{2})(\d{4})(\d{7})$";//driving license
        public readonly String cust_driving = @"^[a-zA-Z0-9-\s]+$";
        public readonly String Regno = @"^\d{1,4}/Goa/\d{4}$";

        public readonly String passport = @"^(?!^0+$)[a-zA-Z0-9]{3,20}$";

        //  public readonly String username_regex = @"^[a-zA-Z0-9\\s]+$";

        public readonly String username_regex = @"[0-9]{3}";
        public readonly String password_regex = @"^([a-zA-Z0-9\\-\\._&]*)$";
        public readonly String captcha_regex = @"^([0-9]*)$";

        /*samson*/
        //public readonly String alpha_spcl_chars_regex_so = @"[a-zA-Z0-9-_./]+$"; 
        public readonly String alpha_spcl_chars_regex_so = @"^[a-zA-Z0-9\-\/\\_\\.]*$";
        /*end samson*/

        /*Abhishek*/
        // public readonly String pan_regex = @"[A-Z]{5}\d{4}[A-Z]{1}";
        /* Ankur */
        public readonly String pan_regex = "^([A-Z]){5}([0-9]){4}([A-Z]){1}?$";
        /*End Abhishek*/
        /* Ankur */
        public readonly String observation_validation = @"^[\sa-zA-Z0-9-,._()/:@|]+$";
        public readonly String reamrks_validation = @"^[\sa-zA-Z0-9-,._()/:@]+$";
        /*  ankur end  */
        //aru boc
        public readonly String account_number = @"^[a-zA-Z0-9/\-_]*$";   //account_number

        public readonly String name = @"^[a-zA-Z\s]*$";  //name
        /*Sunidhi*/
        public readonly String society_name = @"^[\sa-zA-Z0-9-,.`_()-]+$";  // for society name       
     /*   public readonly String society_name = @"^[\sa-zA-Z()-,.`_-]+$";  origanl regex for society    */
        public readonly String alpharegex1 = @"^[a-zA-Z`\s]+$";

        public readonly String name1 = @"^[a-zA-Z`\s]*$";  //name
        /*  Sunidhi end  */

        public readonly string adhar_regex = @"^[0-9]{12}$";     

        //ri password policy string
        public readonly String password_policy = @"^(?=.*[0-9])(?=.*[a-z])(?=.*[A-Z])(?=.*[@!#$%&^+=*()-.:?[\]_|{}~])[a-zA-Z0-9@!#$%&^+=*()-.:?[\]_|{}~]{8,15}$";

        //aru eoc


        public string HtmlEncode(string x, String regexpression)
        {
            string value = "";
            if (Regex.IsMatch(x, regexpression)){ value=x; }else { value = null; ; }
            return value;
            //if (x == null)
            //{
            //    return x;
            //}
            //return Regex.Replace(x, regexpression, delegate (Match match)
            //{
            //    string v = match.ToString();
            //    return char.ToUpper(v[0]) + v.Substring(1);

            //}
            //);
        }


        public Boolean validateData(String input, String regexpression)

        { if (Regex.IsMatch(input, regexpression)) { return true; } { return false; } }
    }
}