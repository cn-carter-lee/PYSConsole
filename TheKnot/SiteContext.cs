namespace TheKnot.Membership.Security
{
    using System;
    using System.Web;
    using TheKnot.Membership;

    public sealed class SiteContext
    {
        public static string GetSiteJoinPage()
        {
            if (Current == 15)
            {
                return (HttpContext.Current.Request.Url.Host + "/join/MemberProfile.aspx");
            }
            return string.Empty;
        }

        private static SiteEnum GetSiteType(string host)
        {
            SiteEnum enum2 = 0;
            if (host != null)
            {
                host = host.ToLower();
                if (host.EndsWith("thenestbaby.com"))
                {
                    return 4;
                }
                if (host.EndsWith("thebump.com"))
                {
                    return 11;
                }
                if (host.EndsWith("theknot.com"))
                {
                    return 1;
                }
                if (host.EndsWith("thenest.com"))
                {
                    return 2;
                }
                if (host.EndsWith("lilaguide.com"))
                {
                    return 3;
                }
                if (host.EndsWith("weddingchannel.com"))
                {
                    return 7;
                }
                if (host.EndsWith("weddings.com"))
                {
                    return 12;
                }
                if (host.EndsWith("giftregistry360.com"))
                {
                    return 15;
                }
                if (host.EndsWith("breastfeeding.com"))
                {
                    enum2 = 0x10;
                }
            }
            return enum2;
        }

        public static SiteEnum Parse(Uri uri)
        {
            if (uri == null)
            {
                return 0;
            }
            return GetSiteType(uri.Host);
        }

        public static SiteEnum Current
        {
            get
            {
                string str = "Site.Current";
                SiteEnum siteType = 0;
                if (HttpContext.Current != null)
                {
                    if (HttpContext.Current.Items[str] != null)
                    {
                        return *(((SiteEnum*) HttpContext.Current.Items[str]));
                    }
                    siteType = GetSiteType(HttpContext.Current.Request.Url.Host);
                    HttpContext.Current.Items[str] = siteType;
                }
                return siteType;
            }
        }

        public static string RootDomain
        {
            get
            {
                switch (Current)
                {
                    case 2:
                        return "thenest.com";

                    case 3:
                        return "lilaguide.com";

                    case 4:
                        return "thenestbaby.com";

                    case 7:
                        return "weddingchannel.com";

                    case 11:
                        return "thebump.com";

                    case 12:
                        return "weddings.com";

                    case 15:
                        return "giftregistry360.com";

                    case 0x10:
                        return "breastfeeding.com";
                }
                return "theknot.com";
            }
        }

        public static string RootDomainForHttpCookie
        {
            get
            {
                return ("." + RootDomain);
            }
        }
    }
}

