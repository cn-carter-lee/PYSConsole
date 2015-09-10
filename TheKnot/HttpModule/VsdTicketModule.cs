namespace TheKnot.Membership.Security.HttpModule
{
    using System;
    using System.Web;
    using TheKnot.Membership.Security;

    public class VsdTicketModule : IHttpModule
    {
        private void context_BeginRequest(object sender, EventArgs e)
        {
            if (HttpContext.Current.Response.ContentType == "text/html")
            {
                TheKnot.Membership.Security.Authentication.Provider.AuthenticateVsdRequest();
            }
        }

        public void Dispose()
        {
        }

        public void Init(HttpApplication context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }
            context.BeginRequest += new EventHandler(this.context_BeginRequest);
        }
    }
}

