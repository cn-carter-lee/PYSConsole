namespace TheKnot.Membership.Security.HttpHander
{
    using System;
    using System.Web;
    using TheKnot.Membership.Security;

    public class MsdTicketCreationHandler : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            TheKnot.Membership.Security.Authentication.Provider.CreateMsdTicket();
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

