namespace TheKnot.Membership.Security.HelperClasses
{
    using System;
    using System.Web;

    public class UrlHelper
    {
        public static string GetTargetUrl(HttpContext context)
        {
            try
            {
                if ((context.Items["membership_target_url"] != null) && (context.Items["membership_target_url"].ToString().Length > 0))
                {
                    return context.Items["membership_target_url"].ToString();
                }
                if ((context.Items["sitecore_target_url"] != null) && (context.Items["sitecore_target_url"].ToString().Length > 0))
                {
                    return context.Items["sitecore_target_url"].ToString();
                }
                if ((context.Request.QueryString["target"] != null) && (context.Request.QueryString["target"].Trim().Length > 0))
                {
                    return context.Request.QueryString["target"].Trim();
                }
            }
            catch
            {
            }
            return context.Request.Url.AbsoluteUri;
        }
    }
}

