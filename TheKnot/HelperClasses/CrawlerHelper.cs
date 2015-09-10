namespace TheKnot.Membership.Security.HelperClasses
{
    using System;
    using System.Runtime.InteropServices;
    using System.Text.RegularExpressions;
    using System.Web;
    using TheKnot.Membership;
    using TheKnot.Membership.Security;

    public class CrawlerHelper
    {
        public static bool IsCrawlerAndReturnId(out long userId)
        {
            bool flag = false;
            userId = -1L;
            try
            {
                string requesterUserAgent = RequesterUserAgent;
                if ((requesterUserAgent == null) || (requesterUserAgent.Trim().Length == 0))
                {
                    return false;
                }
                foreach (CrawlerUserAgentRegex regex in CrawlerUserAgentRegexList)
                {
                    if (Regex.IsMatch(requesterUserAgent, regex.get_UserAgentRegex(), RegexOptions.IgnoreCase))
                    {
                        flag = true;
                        userId = regex.get_ReferredUserId();
                        return flag;
                    }
                }
            }
            catch
            {
            }
            return flag;
        }

        private static CrawlerUserAgentRegex[] CrawlerUserAgentRegexList
        {
            get
            {
                return AdminMemberFactory.GetInstance(TheKnot.Membership.Security.SiteContext.Current).get_CrawlerUserAgentRegexList();
            }
        }

        private static string RequesterUserAgent
        {
            get
            {
                return HttpContext.Current.Request.ServerVariables["HTTP_USER_AGENT"];
            }
        }
    }
}

