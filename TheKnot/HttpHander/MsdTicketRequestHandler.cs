namespace TheKnot.Membership.Security.HttpHander
{
    using System;
    using System.Web;
    using TheKnot.Membership.Security;

    public class MsdTicketRequestHandler : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            TheKnot.Membership.Security.Authentication.Provider.AuthenticateMsdRequest();
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}

