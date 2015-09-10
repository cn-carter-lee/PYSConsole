namespace TheKnot.Membership.Security.HttpHander
{
    using System;
    using System.Web;
    using TheKnot.Membership.Security;
    using TheKnot.Membership.Security.Providers;

    public class MsdTicketDeletionHandler : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            if (context == null)
            {
                throw new HttpException("HttpContext was unavailable.");
            }
            TheKnot.Membership.Security.Authentication.Provider.DeleteMsdTicket();
            UriBuilder builder = new UriBuilder(context.Request.QueryString["originalTarget"]);
            if ((builder == null) || (builder.Uri.PathAndQuery.Trim().Length == 0))
            {
                throw new TheKnot.Membership.Security.Providers.CustomAuthenticationException("The application requires 'originalTarget' as query string variable.");
            }
            if (builder.Query.IndexOf("MsdVisit") == -1)
            {
                if (builder.Query.Length > 0)
                {
                    builder.Query = builder.Query + "&MsdVisit=1";
                }
                else
                {
                    builder.Query = "MsdVisit=1";
                }
            }
            context.Response.Redirect(builder.Uri.AbsoluteUri, true);
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

