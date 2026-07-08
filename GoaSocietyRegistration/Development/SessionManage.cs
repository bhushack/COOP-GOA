using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GoaSocietyRegistration.Development
{
    public class SessionManage
    {
        public void __Abandon(HttpRequest Request, HttpResponse Response)
        {
            HttpContext context = HttpContext.Current;
            bool isUserLoggedIn = false;
            if (context.Session != null)
                isUserLoggedIn = context.Session["login_id"] != null ? true : false;
            __Sanitise();
            __SanitiseCookies(Request, Response);
            __SignOut();
            if (isUserLoggedIn)               
            Response.Redirect("~/error.aspx");
            else
                Response.Redirect("~/error.aspx");
        }

        public void __SignOut()
        {
            System.Web.Security.FormsAuthentication.SignOut();
        }

        public void __Sanitise()
        {
            HttpContext context = HttpContext.Current;
            if (context.Session != null)
            {
                context.Session.Clear();
                context.Session.RemoveAll();
                context.Session.Abandon();
            }
        }

        public void __SanitiseCookies(HttpRequest Request, HttpResponse Response)
        {
            __GetNewASP_Cookie(Request, Response);
            if (Request.Cookies["DoTAuthTok"] != null)
                Response.Cookies["DoTAuthTok"].Expires = DateTime.Now.AddDays(-1);
        }

        public void __GetNewASP_Cookie(HttpRequest Request, HttpResponse Response)
        {
            if (Request.Cookies["ASP.NET_SessionId"] != null)
                Response.Cookies["ASP.NET_SessionId"].Expires = DateTime.Now.AddDays(-1);
        }

        public void __ReInitialiseAuthCookie(HttpResponse Response)
        {
            HttpContext context = HttpContext.Current;
            if (context.Session != null)
            {
                context.Session.Remove("DoTAuthTok");
            }
            string guid = Guid.NewGuid().ToString();
            context.Session["DoTAuthTok"] = guid;
            var cookie = new HttpCookie("DoTAuthTok")
            {
                HttpOnly = true,
                Value = guid
            };
            Response.Cookies.Add(cookie);
        }
        public void __AbandonAdmin(HttpRequest Request, HttpResponse Response)
        {
            HttpContext context = HttpContext.Current;
            bool isUserLoggedIn = false;
            if (context.Session != null)
                isUserLoggedIn = context.Session["firstname"] != null && context.Session["role_id"] != null ? true : false;
            __SanitiseAdmin();
            __SanitiseCookiesAdmin(Request, Response);
            __SignOutAdmin();
            if (isUserLoggedIn)
                Response.Redirect("~/error.aspx");
            else
                Response.Redirect("~/error.aspx");
        }

        public void __SignOutAdmin()
        {
            System.Web.Security.FormsAuthentication.SignOut();
        }

        public void __SanitiseAdmin()
        {
            HttpContext context = HttpContext.Current;
            if (context.Session != null)
            {
                context.Session.Clear();
                context.Session.RemoveAll();
                context.Session.Abandon();
            }
        }

        public void __SanitiseCookiesAdmin(HttpRequest Request, HttpResponse Response)
        {
            __GetNewASP_CookieAdmin(Request, Response);
            if (Request.Cookies["DoTAuthTokAdmin"] != null)
                Response.Cookies["DoTAuthTokAdmin"].Expires = DateTime.Now.AddDays(-1);
        }

        public void __GetNewASP_CookieAdmin(HttpRequest Request, HttpResponse Response)
        {
            if (Request.Cookies["ASP.NET_SessionId"] != null)
                Response.Cookies["ASP.NET_SessionId"].Expires = DateTime.Now.AddDays(-1);
        }

        public void __ReInitialiseAuthCookieAdmin(HttpResponse Response)
        {
            HttpContext context = HttpContext.Current;
            if (context.Session != null)
            {
                context.Session.Remove("DoTAuthTokAdmin");
            }
            string guid = Guid.NewGuid().ToString();
            context.Session["DoTAuthTokAdmin"] = guid;
            var cookie = new HttpCookie("DoTAuthTokAdmin")
            {
                HttpOnly = true,
                Value = guid
            };
            Response.Cookies.Add(cookie);
        }
    }
}