using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GoaSocietyRegistration
{
    public class GlobalVars
    {
        //public const string MakePaymentUrl = "https://egov.goa.nic.in/echallanpg/haveechallan.aspx"; //Test
         public const string MakePaymentUrl = "https://echallanpg.goa.gov.in/haveechallan.aspx"; //Live Mini Cloud

        public const string echallanpgPrintUrl = "http://10.155.31.81/echallanpg/mainpage.aspx"; //Test


        public const string echallanpgWsUrl = "http://10.155.31.81/echallanws/service.asmx"; //Test        


      
        /// <summary> Test Service codes for E  Society
      //  public const string SocietyDemandCode = "10";
      //  public const string SocietyHOA = "1475001050100";
      //  public const string SocietySvcCode = "217";
      
        /// <summary> Live Service codes for E  Society
        public const string SocietyDemandCode = "10";
        public const string SocietyHOA = "1475001050100";
        public const string SocietySvcCode = "233";

        /*Vahan4 WebAPI Constants - END*/

        /*CSRF/XSRF Prevention Constants - START*/
        public const string AntiXsrfTokenKey = "__DoTAntiXsrfToken";
        public const string AntiXsrfUserNameKey = "__DoTAntiXsrfUserName";
        /*CSRF/XSRF Prevention Constants - END*/
        /* 192.168.101.143 marriage private ip*/
        /*  223.31.97.149 marriage public ip */
        /*CSRF/XSRF Prevention Referer Constants - START*/

        public const string AntiXsrfRefererHeader1 = "http://localhost";
        public const string AntiXsrfRefererHeader2 = "http://10.155.4.50";
        public const string AntiXsrfRefererHeader3 = "http://10.155.4.29";
        public const string AntiXsrfRefererHeader4 = "http://10.155.4.42";
        public const string AntiXsrfRefererHeader5 = "https://reg.goa.gov.in";//website name 
        public const string AntiXsrfRefererHeader11 = "http://10.155.173.118";//website name 

        public const string AntiXsrfRefererHeader6 = "https://10.155.173.118";//server ip
        public const string AntiXsrfRefererHeader7 = "http://10.155.173.118";//server ip
        public const string AntiXsrfRefererHeader8 = "http://164.100.183.43";//server ip (publid IP)
        public const string AntiXsrfRefererHeader9 = "https://164.100.183.43";//server ip (publid IP)
        public const string AntiXsrfRefererHeader10 = "https://societyreg.goa.gov.in";//website name  
        public const string AntiXsrfPaymentHeader1 = "http://10.155.31.81";
        public const string AntiXsrfPaymentHeader2 = "https://egov.goa.nic.in";
        public const string AntiXsrfPaymentHeader3 = "https://164.100.148.73";
        public const string AntiXsrfPaymentHeader4 = "https://egov.goa.nic.in";
        public const string AntiXsrfPaymentHeader7 = "https://echallanpg.goa.gov.in";       
        public const string AntiXsrfPaymentHeader5 = "https://164.100.183.25";
        public const string AntiXsrfPaymentHeader6 = "https://164.100.183.26";
        public const string AntiXsrfRefererHeader12 = "http://societyreg.goa.gov.in";//website name  

        public static readonly List<string> AntiXsrfRefererHeaderList = new List<string>() {
        AntiXsrfRefererHeader1,
        AntiXsrfRefererHeader2,
        AntiXsrfRefererHeader3,
        AntiXsrfRefererHeader4,
        AntiXsrfRefererHeader5,
        AntiXsrfRefererHeader6,
        AntiXsrfRefererHeader7,
        AntiXsrfRefererHeader8,
        AntiXsrfRefererHeader9,
        AntiXsrfRefererHeader10,
        AntiXsrfRefererHeader11,
         AntiXsrfRefererHeader12,
        AntiXsrfPaymentHeader1,
        AntiXsrfPaymentHeader2,
        AntiXsrfPaymentHeader3,
        AntiXsrfPaymentHeader4,
        AntiXsrfPaymentHeader5,
        AntiXsrfPaymentHeader6,
        AntiXsrfPaymentHeader7};

        public const string AntiPageRequestHeader01 = "Default.aspx";
        public const string AntiPageRequestHeader1 = "Dashboard.aspx";
        public const string AntiPageRequestHeader2 = "error.aspx";
        public const string AntiPageRequestHeader3 = "OrganizationLogin.aspx";
        public const string AntiPageRequestHeader4 = "Applicant.aspx";
        public const string AntiPageRequestHeader5 = "ViewApplicantDetails.aspx";
        public const string AntiPageRequestHeader6 = "SocietyDetails.aspx";
        public const string AntiPageRequestHeader7 = "ChangePassword.aspx";
        public const string AntiPageRequestHeader8 = "ApplicationAudit.aspx";
        public const string AntiPageRequestHeader9 = "CreateUser.aspx";
        public const string AntiPageRequestHeader10 = "ApproveSociety.aspx";
        public const string AntiPageRequestHeader11 = "GenerateCertificate.aspx";
        public const string AntiPageRequestHeader12 = "oldCertificate.aspx";
        public const string AntiPageRequestHeader13 = "GeneratePassword.aspx";
        public const string AntiPageRequestHeader14 = "Registration.aspx";
        public const string AntiPageRequestHeader15 = "SocietyList.aspx";
        public const string AntiPageRequestHeader16 = "VerifySociety.aspx";
        public const string AntiPageRequestHeader17 = "ViewPdf.aspx";
        public const string AntiPageRequestHeader18 = "Application_registration.aspx";
        public const string AntiPageRequestHeader19 = "DocumentUpload.aspx";
        public const string AntiPageRequestHeader20 = "InitiatePayment.aspx";
        public const string AntiPageRequestHeader21 = "LoginModule.aspx";
        public const string AntiPageRequestHeader22 = "MemberDetails.aspx";
        public const string AntiPageRequestHeader23 = "Society.Master";
        public const string AntiPageRequestHeader24 = "admin.Master";
        public const string AntiPageRequestHeader25 = "PaymentSuccess.aspx";
        public const string AntiPageRequestHeader26 = "admincaptcha.aspx";
        public const string AntiPageRequestHeader27 = "GenerateCaptcha.aspx";
        public const string AntiPageRequestHeader28 = "ViewProfile.aspx";
        public const string AntiPageRequestHeader29 = "AdminUser.aspx";
        public const string AntiPageRequestHeader30 = "success.aspx";
        public const string AntiPageRequestHeader31 = "echallanpg/success.aspx";
        public const string AntiPageRequestHeader32 = "Application_renewal.aspx";
        public const string AntiPageRequestHeader33 = "AllServices.aspx";
        public const string AntiPageRequestHeader34 = "EchallanStatus.aspx";
        public const string AntiPageRequestHeader35 = "EditSocietyDetails.aspx";
        public const string AntiPageRequestHeader36 = "Search.aspx";
        public const string AntiPageRequestHeader37 = "Amendment.aspx";
        public const string AntiPageRequestHeader38 = "SocietyAmendment.aspx";
        public const string AntiPageRequestHeader39 = "VerifyAmendment.aspx";
        public const string AntiPageRequestHeader40 = "Feedback.aspx";
        public const string AntiPageRequestHeader41 = "PendingApplications.aspx";
        public const string AntiPageRequestHeader42 = "AddOldSociety.aspx";
        public const string AntiPageRequestHeader43 = "ViewFeedback.aspx";
        public const string AntiPageRequestHeader44 = "MemorandumOfAssociation.aspx";
        public const string AntiPageRequestHeader45 = "PaidEmployee.aspx";
        public const string AntiPageRequestHeader46 = "Schedule1.aspx";
        public const string AntiPageRequestHeader47 = "Schedule2.aspx";
        public const string AntiPageRequestHeader48 = "ScheduleVI.aspx";
        public const string AntiPageRequestHeader49 = "NormalMembers.aspx";
        public const string AntiPageRequestHeader50 = "ChangeofFees.aspx";
        public const string AntiPageRequestHeader51 = "CertifiedCopy.aspx";
        public const string AntiPageRequestHeader52 = "ApplyCertifiedCopy.aspx";
        public const string AntiPageRequestHeader53 = "PrintCertifiedCopy.aspx";
        public const string AntiPageRequestHeader54 = "PublicDashboard.aspx";
        public const string AntiPageRequestHeader55 = "DisableLogin.aspx";
        public const string AntiPageRequestHeader56 = "ChangeMobile.aspx";
        public const string AntiPageRequestHeader57 = "CancelRegistration.aspx";
        public const string AntiPageRequestHeader58 = "VerifyCertificate.aspx";



        public static readonly List<string> AntiPageRequest = new List<string>
        {
             AntiPageRequestHeader01,
            AntiPageRequestHeader1,
            AntiPageRequestHeader2,
            AntiPageRequestHeader3,
            AntiPageRequestHeader4,
            AntiPageRequestHeader5,
            AntiPageRequestHeader6,
            AntiPageRequestHeader7,
            AntiPageRequestHeader8,
            AntiPageRequestHeader9,
            AntiPageRequestHeader10,
            AntiPageRequestHeader11,
            AntiPageRequestHeader12,
            AntiPageRequestHeader13,
            AntiPageRequestHeader14,
            AntiPageRequestHeader15,
            AntiPageRequestHeader16,
            AntiPageRequestHeader17,
            AntiPageRequestHeader18,
            AntiPageRequestHeader19,
            AntiPageRequestHeader20,
            AntiPageRequestHeader21,
            AntiPageRequestHeader22,
            AntiPageRequestHeader23,
            AntiPageRequestHeader24,
            AntiPageRequestHeader25,
            AntiPageRequestHeader26,
            AntiPageRequestHeader27,
            AntiPageRequestHeader28,
            AntiPageRequestHeader29,
            AntiPageRequestHeader30,
            AntiPageRequestHeader31,
            AntiPageRequestHeader32,
            AntiPageRequestHeader33,
            AntiPageRequestHeader34,
            AntiPageRequestHeader35,
            AntiPageRequestHeader36,
            AntiPageRequestHeader37,
            AntiPageRequestHeader38,
            AntiPageRequestHeader39,
            AntiPageRequestHeader40,
            AntiPageRequestHeader41,
            AntiPageRequestHeader42,
            AntiPageRequestHeader43,
            AntiPageRequestHeader44,
            AntiPageRequestHeader45,
            AntiPageRequestHeader46,
            AntiPageRequestHeader47,
            AntiPageRequestHeader48,
            AntiPageRequestHeader49,
            AntiPageRequestHeader50,
            AntiPageRequestHeader51,
            AntiPageRequestHeader52,
            AntiPageRequestHeader53,
            AntiPageRequestHeader54,
            AntiPageRequestHeader55,
            AntiPageRequestHeader56,
            AntiPageRequestHeader57,
            AntiPageRequestHeader58
        };

        /*CSRF/XSRF Prevention Referer Constants - END*/

        /* START mongoDB encryption keys*/
        // public const string mongoRijEncKey = "%HG&8F3@VB0GJ581$";
        // public const string mongoRijEncSalt = "GH%56E3#43I%OKJ&47H$";
        /* END mongoDB encryption keys*/

        //Configurable :: Captcha generation character domain for Login & Registration
        public const string __CaptchaCombinations = "0123456789ABCDEFGHJKLMNPQRSTUVWXYZ0123456789abcdefghijkmnpqrstuvwxyz@*#$";

        //Configurable :: No. of password history count to be stored. Table - HistUserslogin
        public const int __PasswordHistoryCount = 3;
    }
}