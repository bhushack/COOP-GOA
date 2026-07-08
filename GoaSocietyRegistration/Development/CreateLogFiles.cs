using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace GoaSocietyRegistration.Development
{
    public class CreateLogFiles
    {
        private string sLogFormat;
        private string sErrorTime;

        public CreateLogFiles()
        {
            //sLogFormat used to create log files format :
            // dd/mm/yyyy hh:mm:ss AM/PM ==> Log Message
            sLogFormat = DateTime.Now.ToShortDateString().ToString() + " " + DateTime.Now.ToLongTimeString().ToString() + " ==> ";

            //this variable used to create log filename format "
            //for example filename : ErrorLogYYYYMMDD
            string sYear = DateTime.Now.Year.ToString();
            string sMonth = DateTime.Now.Month.ToString();
            string sDay = DateTime.Now.Day.ToString();
            sErrorTime = sYear + sMonth + sDay;
        }
        public void ErrorLog(string sPathName, string sErrMsg, string location)
        {
            StreamWriter sw = new StreamWriter(sPathName + sErrorTime, true);
            sw.WriteLine("---------------------------------------------------------------------------------------------------------------------------------");
            sw.WriteLine(sLogFormat + sErrMsg + location);
            sw.Flush();
            sw.Close();
        }
    }
}