namespace TheKnot.Membership.Security.Providers.Cookie
{
    using System;
    using System.Collections.Specialized;
    using System.Globalization;
    using System.Text.RegularExpressions;
    using System.Web;
    using TheKnot.Membership;
    using TheKnot.Membership.Security;
    using TheKnot.Membership.Security.Components;
    using TheKnot.Membership.Security.HelperClasses;
    using TheKnot.Membership.Security.Providers;

    public class CookieAuthenticationProvider : TheKnot.Membership.Security.Providers.CustomAuthenticationProvider
    {
        private bool debug;
        private string host;
        private string logOffUrl;
        private string logonUrl;
        private Uri msdAuthenticateTicketUrl;
        private int msdCookieExpirationInDays;
        private string msdCookieName;
        private Uri msdCreateTicketUrl;
        private Uri msdDeleteTicketUrl;
        private string msdDomain;
        private int vsdCookieExpirationInHours;
        private string vsdCookieName;
        private string vsdLandingUrl;

        public static bool AuthenticateCookie(HttpCookie cookie)
        {
            bool flag = false;
            if ((cookie != null) && cookie.HasKeys)
            {
                try
                {
                    if (TheKnot.Membership.Security.HelperClasses.CryptoHelper.EncryptHmacSha1(cookie["exp"] + cookie["guid"]) == cookie["digest"])
                    {
                        flag = true;
                    }
                    else if (TheKnot.Membership.Security.HelperClasses.CryptoHelper.EncryptHmacSha1(cookie["exp"] + cookie["guid"] + cookie["oldMemberId"] + cookie["userId"]) == cookie["digest"])
                    {
                        flag = true;
                    }
                    DateTime time = new DateTime(0x7b2, 1, 1);
                    if ((cookie["exp"] != null) && (time.AddDays((double) Convert.ToInt32(cookie["exp"], CultureInfo.InvariantCulture)) < DateTime.Today))
                    {
                        flag = false;
                    }
                }
                catch (FormatException)
                {
                    flag = false;
                }
            }
            return flag;
        }

        internal override void AuthenticateMsdRequest()
        {
            HttpContext current = HttpContext.Current;
            if (current == null)
            {
                throw new HttpException("HttpContext was unavailable.");
            }
            HttpCookie cookie = current.Request.Cookies[this.msdCookieName];
            bool flag = AuthenticateCookie(cookie);
            if (!flag)
            {
                this.DeleteMsdTicket();
            }
            if ((current.Request.QueryString["landingUrl"] == null) || (current.Request.QueryString["landingUrl"].Trim().Length == 0))
            {
                throw new TheKnot.Membership.Security.Providers.CustomAuthenticationException("The application requires 'landingUrl' as query string variable.");
            }
            UriBuilder builder = null;
            try
            {
                builder = new UriBuilder(current.Request.QueryString["landingUrl"]);
            }
            catch (UriFormatException)
            {
                builder = new UriBuilder("http://www.theknot.com");
            }
            SiteEnum siteType = TheKnot.Membership.Security.SiteContext.Parse(builder.Uri);
            string originalTarget = current.Request.QueryString["originalTarget"];
            long userId = 0L;
            if ((flag && (cookie != null)) && cookie.HasKeys)
            {
                try
                {
                    userId = Convert.ToInt64(TheKnot.Membership.Security.HelperClasses.CryptoHelper.DecryptDesFromWeb(cookie["userId"]));
                }
                catch (FormatException)
                {
                }
            }
            foreach (TheKnot.Membership.Security.Components.MsdRuleItem item in base.MsdRules)
            {
                if (item.Enabled)
                {
                    try
                    {
                        TheKnot.Membership.Security.Components.MsdRuleBase base2 = Activator.CreateInstance(Type.GetType(item.Type, true)) as TheKnot.Membership.Security.Components.MsdRuleBase;
                        if ((base2 != null) && ((TheKnot.Membership.Security.Components.MsdRuleBase.ExecutionPoint.TicketAuthentication & base2.ExecutionPoints) == TheKnot.Membership.Security.Components.MsdRuleBase.ExecutionPoint.TicketAuthentication))
                        {
                            base2.ProcessRequest(userId, siteType, ref originalTarget);
                        }
                    }
                    catch (Exception)
                    {
                    }
                }
            }
            string str2 = "originalTarget=" + current.Server.UrlEncode(originalTarget) + "&MsdVisit=1";
            if (flag)
            {
                string str3 = str2;
                str2 = str3 + "&exp=" + cookie["exp"] + "&guid=" + cookie["guid"] + "&digest=" + cookie["digest"] + "&userId=" + cookie["userId"];
            }
            builder.Query = str2;
            current.Response.Redirect(builder.Uri.AbsoluteUri, true);
        }

        internal override void AuthenticateVsdRequest()
        {
            HttpContext current = HttpContext.Current;
            if (current == null)
            {
                throw new HttpException("HttpContext was unavailable.");
            }
            HttpCookie cookie = current.Request.Cookies[this.vsdCookieName];
            bool flag = AuthenticateCookie(cookie);
            if (flag)
            {
                HttpContext.Current.Items.Add("UserIdentity", this.ReadUserIdFromVsdCookie());
            }
            else
            {
                long num;
                if (TheKnot.Membership.Security.HelperClasses.CrawlerHelper.IsCrawlerAndReturnId(out num))
                {
                    HttpContext.Current.Items.Add("UserIdentity", num);
                }
                else if (((current.Request.FilePath != null) && (current.Request.FilePath.EndsWith(".aspx") || current.Request.FilePath.EndsWith("/"))) && ((current.Request.QueryString["device"] == null) || (((current.Request.QueryString["device"].ToLower() != "externalapplication") && (current.Request.QueryString["device"].ToLower() != "rss")) && ((current.Request.QueryString["device"].ToLower() != "index") && (current.Request.QueryString["device"].ToLower() != "xmlsitemap")))))
                {
                    cookie = current.Request.Cookies["temp"];
                    HttpCookie cookie2 = current.Request.Cookies["wcsession"];
                    HttpCookie cookie3 = current.Request.Cookies["djid"];
                    if ((((((cookie != null) || (cookie2 != null)) || (cookie3 != null)) && (current.Request.QueryString["MsdVisit"] != "1")) && ((current.Request.Cookies[this.VsdCookieName] == null) || (current.Request.Cookies[this.VsdCookieName]["MsdVisit"] != "1"))) && !flag)
                    {
                        string externalSite = GetExternalSite();
                        string str2 = current.Server.UrlEncode(this.VsdLandingUrl);
                        current.Response.Redirect(string.Concat(new object[] { this.msdAuthenticateTicketUrl, "?landingUrl=", str2, "&originalTarget=", current.Server.UrlEncode(this.GetVsdTargetUrl(current)), "&externalSite=", current.Server.UrlEncode(externalSite) }), true);
                    }
                }
            }
        }

        internal override void CreateAuthenticationTickets(decimal userId, Uri target)
        {
            if (userId <= 0M)
            {
                throw new ArgumentException("userId must be greater than zero.", "userId");
            }
            if (target == null)
            {
                throw new ArgumentNullException("target");
            }
            HttpContext current = HttpContext.Current;
            if (current == null)
            {
                throw new HttpException("HttpContext was unavailable.");
            }
            this.DeleteVsdTicket();
            string str = current.Server.UrlEncode(GetExternalSite());
            string str2 = current.Server.UrlEncode(this.VsdLandingUrl);
            string url = string.Concat(new object[] { this.MsdCreateTicketUrl, "?userId=", TheKnot.Membership.Security.HelperClasses.CryptoHelper.EncryptDesForWeb(userId.ToString(CultureInfo.InvariantCulture)), "&landingUrl=", str2, "&originalTarget=", current.Server.UrlEncode(target.AbsoluteUri), "&externalSite=", str });
            if (current.Request.Cookies["partner"] != null)
            {
                string str4 = current.Request.Cookies["partner"].Value;
                if ((str4 != null) && (str4.Trim().Length > 0))
                {
                    url = url + "&pid=" + str4;
                }
            }
            current.Response.Redirect(url, true);
        }

        internal override void CreateMsdTicket()
        {
            HttpContext current = HttpContext.Current;
            if (current == null)
            {
                throw new HttpException("HttpContext was unavailable.");
            }
            if ((current.Request.QueryString["landingUrl"] == null) || (current.Request.QueryString["landingUrl"].Trim().Length == 0))
            {
                throw new TheKnot.Membership.Security.Providers.CustomAuthenticationException("The application requires 'landingUrl' as query string variable.");
            }
            UriBuilder builder = null;
            try
            {
                builder = new UriBuilder(current.Request.QueryString["landingUrl"]);
            }
            catch (UriFormatException)
            {
                builder = new UriBuilder("http://www.theknot.com");
            }
            SiteEnum siteType = TheKnot.Membership.Security.SiteContext.Parse(builder.Uri);
            string originalTarget = current.Request.QueryString["originalTarget"];
            long userId = 0L;
            HttpCookie cookie = null;
            if (current.Request.QueryString["userId"] != null)
            {
                userId = Convert.ToInt64(TheKnot.Membership.Security.HelperClasses.CryptoHelper.DecryptDes(current.Request.QueryString["userId"]));
                IWeddingMember member = WeddingMemberFactory.GetInstance(TheKnot.Membership.Security.SiteContext.Current).Find(userId);
                cookie = new HttpCookie(this.msdCookieName);
                DateTime time = new DateTime(0x7b2, 1, 1);
                DateTime time2 = DateTime.Today.AddDays((double) this.msdCookieExpirationInDays);
                TimeSpan span = time2.Subtract(time);
                cookie["exp"] = span.Days.ToString(CultureInfo.InvariantCulture);
                cookie["guid"] = Guid.NewGuid().ToString();
                cookie["userId"] = current.Server.UrlEncode(current.Request.QueryString["userId"]);
                if (member != null)
                {
                    cookie["oldmemberid"] = TheKnot.Membership.Security.HelperClasses.CryptoHelper.EncryptDesForWeb(member.get_OldMemberId());
                }
                else
                {
                    cookie["oldmemberid"] = current.Request.QueryString["userId"];
                }
                cookie["MsdVisit"] = "1";
                cookie["digest"] = TheKnot.Membership.Security.HelperClasses.CryptoHelper.EncryptHmacSha1(cookie["exp"] + cookie["guid"] + cookie["oldmemberid"] + cookie["userId"]);
                cookie.Expires = time2;
                cookie.Domain = this.msdDomain;
                current.Response.Cookies.Add(cookie);
                builder.Query = "MsdVisit=1&exp=" + cookie["exp"] + "&guid=" + cookie["guid"] + "&digest=" + cookie["digest"] + "&userId=" + cookie["userId"];
                try
                {
                    userId = Convert.ToInt64(TheKnot.Membership.Security.HelperClasses.CryptoHelper.DecryptDesFromWeb(cookie["userId"]));
                }
                catch (FormatException)
                {
                }
                foreach (TheKnot.Membership.Security.Components.MsdRuleItem item in base.MsdRules)
                {
                    if (item.Enabled)
                    {
                        try
                        {
                            TheKnot.Membership.Security.Components.MsdRuleBase base2 = Activator.CreateInstance(Type.GetType(item.Type, true)) as TheKnot.Membership.Security.Components.MsdRuleBase;
                            if ((base2 != null) && ((TheKnot.Membership.Security.Components.MsdRuleBase.ExecutionPoint.TicketCreation & base2.ExecutionPoints) == TheKnot.Membership.Security.Components.MsdRuleBase.ExecutionPoint.TicketCreation))
                            {
                                base2.ProcessRequest(userId, siteType, ref originalTarget);
                            }
                        }
                        catch (Exception exception)
                        {
                            if (this.Debug)
                            {
                                throw exception;
                            }
                        }
                    }
                }
            }
            else
            {
                builder.Query = "MsdVisit=1";
            }
            current.Response.Redirect(builder.Uri.AbsoluteUri + "&originalTarget=" + current.Server.UrlEncode(originalTarget), true);
        }

        internal override void CreateVsdTicket()
        {
            HttpContext current = HttpContext.Current;
            if (current == null)
            {
                throw new HttpException("HttpContext was unavailable.");
            }
            HttpCookie cookie = new HttpCookie(this.vsdCookieName);
            if (((current.Request.QueryString["exp"] != null) && (current.Request.QueryString["guid"] != null)) && ((current.Request.QueryString["digest"] != null) && (current.Request.QueryString["userId"] != null)))
            {
                long num = Convert.ToInt64(TheKnot.Membership.Security.HelperClasses.CryptoHelper.DecryptDes(current.Request.QueryString["userId"]));
                IWeddingMember member = WeddingMemberFactory.GetInstance(TheKnot.Membership.Security.SiteContext.Current).Find(num);
                if (((member != null) && ((member.get_Status() == 1) || (member.get_Status() == 4))) && ((member.get_MembershipStatus() == 2) || (member.get_MembershipStatus() == 1)))
                {
                    cookie["exp"] = current.Request.QueryString["exp"];
                    cookie["guid"] = current.Request.QueryString["guid"];
                    cookie["digest"] = current.Request.QueryString["digest"];
                    cookie["userId"] = current.Server.UrlEncode(current.Request.QueryString["userId"]);
                    cookie["oldmemberid"] = TheKnot.Membership.Security.HelperClasses.CryptoHelper.EncryptDesForWeb(member.get_OldMemberId());
                    IUserSiteVerification userSiteVerification = member.GetUserSiteVerification(TheKnot.Membership.Security.SiteContext.Current);
                    if ((userSiteVerification == null) || !userSiteVerification.get_Verified())
                    {
                        cookie["IsVerified"] = "0";
                    }
                    else if (userSiteVerification.get_Verified())
                    {
                        cookie["IsVerified"] = "1";
                    }
                }
            }
            if (this.vsdCookieExpirationInHours > 0)
            {
                cookie.Expires = DateTime.Now.AddHours((double) this.vsdCookieExpirationInHours);
            }
            cookie.Domain = TheKnot.Membership.Security.SiteContext.RootDomainForHttpCookie;
            cookie["MsdVisit"] = current.Request.QueryString["MSDVisit"];
            current.Response.Cookies.Add(cookie);
        }

        internal string DecodeString(string Value)
        {
            if (Value == null)
            {
                return "";
            }
            Regex regex = new Regex("%([a-fA-F0-9][a-fA-F0-9])");
            return regex.Replace(Value, new MatchEvaluator(this.MatchDecode));
        }

        internal override void DeleteMsdTicket()
        {
            HttpContext current = HttpContext.Current;
            if (current == null)
            {
                throw new HttpException("HttpContext was unavailable.");
            }
            HttpCookie cookie = new HttpCookie(this.msdCookieName) {
                Expires = DateTime.Today.AddDays(-1.0),
                Domain = this.msdDomain
            };
            current.Response.Cookies.Add(cookie);
        }

        internal override void DeleteVsdTicket()
        {
            HttpContext current = HttpContext.Current;
            if (current == null)
            {
                throw new HttpException("HttpContext was unavailable.");
            }
            HttpCookie cookie = new HttpCookie(this.vsdCookieName) {
                Expires = DateTime.Today.AddDays(-1.0),
                Domain = TheKnot.Membership.Security.SiteContext.RootDomainForHttpCookie
            };
            current.Response.Cookies.Add(cookie);
        }

        private static string GetExternalSite()
        {
            HttpCookie cookie = HttpContext.Current.Request.Cookies["Referrer"];
            if (cookie != null)
            {
                return cookie.Value;
            }
            if ((HttpContext.Current.Request.UrlReferrer != null) && (HttpContext.Current.Request.UrlReferrer.AbsoluteUri.Trim().Length > 0))
            {
                return HttpContext.Current.Request.UrlReferrer.AbsoluteUri;
            }
            return HttpContext.Current.Request.Url.AbsoluteUri;
        }

        [Obsolete("Use KnotInc.Web.Security.Site.RootDomainForHttpCookie")]
        private static string GetHighLevelDomain()
        {
            return TheKnot.Membership.Security.SiteContext.RootDomainForHttpCookie;
        }

        public HttpCookie GetMemberTicket(long userId)
        {
            HttpCookie cookie = null;
            if (userId > 0L)
            {
                IWeddingMember member = WeddingMemberFactory.GetInstance(TheKnot.Membership.Security.SiteContext.Current).Find(userId);
                if (member != null)
                {
                    cookie = new HttpCookie(this.msdCookieName);
                    DateTime time = new DateTime(0x7b2, 1, 1);
                    DateTime time2 = DateTime.Today.AddDays((double) this.msdCookieExpirationInDays);
                    TimeSpan span = time2.Subtract(time);
                    cookie["exp"] = span.Days.ToString(CultureInfo.InvariantCulture);
                    cookie["guid"] = Guid.NewGuid().ToString();
                    cookie["oldmemberid"] = TheKnot.Membership.Security.HelperClasses.CryptoHelper.EncryptDesForWeb(member.get_OldMemberId());
                    cookie["userId"] = TheKnot.Membership.Security.HelperClasses.CryptoHelper.EncryptDesForWeb(userId.ToString());
                    cookie["digest"] = TheKnot.Membership.Security.HelperClasses.CryptoHelper.EncryptHmacSha1(cookie["exp"] + cookie["guid"] + cookie["oldmemberid"] + cookie["userId"]);
                    cookie["MsdVisit"] = "1";
                    cookie.Expires = time2;
                    cookie.Domain = this.msdDomain;
                }
            }
            return cookie;
        }

        private string GetVsdTargetUrl(HttpContext context)
        {
            try
            {
                if ((context.Request.Headers["sitecore_target_url"] != null) && (context.Request.Headers["sitecore_target_url"].Length > 0))
                {
                    return context.Request.Headers["sitecore_target_url"];
                }
                if ((context.Request.Headers["membership_target_url"] != null) && (context.Request.Headers["membership_target_url"].Length > 0))
                {
                    return context.Request.Headers["membership_target_url"];
                }
            }
            catch
            {
            }
            return context.Request.Url.AbsoluteUri;
        }

        public override void Initialize(string name, NameValueCollection configuration)
        {
            int num;
            if (configuration == null)
            {
                throw new ArgumentNullException("configuration");
            }
            if ((name == null) || (name.Length == 0))
            {
                name = "CookieAuthenticationProvider";
            }
            if ((configuration["description"] == null) || (configuration["description"].Length == 0))
            {
                configuration.Remove("description");
                configuration.Add("description", "Cookie Authentication Provider.");
            }
            base.Initialize(name, configuration);
            string str = configuration["msdCookieExpirationInDays"];
            try
            {
                this.msdCookieExpirationInDays = Convert.ToInt32(str, CultureInfo.InvariantCulture);
            }
            catch (FormatException)
            {
                throw new TheKnot.Membership.Security.Providers.ProviderException("MsdCookieExpirationInDays must be of type Int.");
            }
            catch (OverflowException)
            {
                num = 0x7fffffff;
                throw new TheKnot.Membership.Security.Providers.ProviderException("MsdCookieExpirationInDays must be greater than zero and less than " + num.ToString(CultureInfo.InvariantCulture));
            }
            if (this.msdCookieExpirationInDays == 0)
            {
                throw new TheKnot.Membership.Security.Providers.ProviderException("MsdCookieExpirationInDays must be greater than zero");
            }
            str = configuration["vsdCookieExpirationInHours"];
            try
            {
                this.vsdCookieExpirationInHours = Convert.ToInt32(str, CultureInfo.InvariantCulture);
            }
            catch (FormatException)
            {
                throw new TheKnot.Membership.Security.Providers.ProviderException("VsdCookieExpirationInHours must be of type Int.");
            }
            catch (OverflowException)
            {
                num = 0x7fffffff;
                throw new TheKnot.Membership.Security.Providers.ProviderException("VsdCookieExpirationInHours must be greater than -1 and less than " + num.ToString(CultureInfo.InvariantCulture));
            }
            if (this.vsdCookieExpirationInHours < 0)
            {
                throw new TheKnot.Membership.Security.Providers.ProviderException("VsdCookieExpirationInHours must be greater than -1");
            }
            this.logOffUrl = configuration["logOffUrl"];
            this.logonUrl = configuration["logonUrl"];
            this.msdCookieName = configuration["msdCookieName"];
            this.msdDomain = configuration["msdDomain"];
            this.vsdCookieName = configuration["vsdCookieName"];
            this.msdAuthenticateTicketUrl = new Uri(configuration["msdAuthenticateTicketUrl"]);
            this.msdCreateTicketUrl = new Uri(configuration["msdCreateTicketUrl"]);
            this.msdDeleteTicketUrl = new Uri(configuration["msdDeleteTicketUrl"]);
            this.vsdLandingUrl = configuration["vsdLandingUrl"];
            this.debug = Convert.ToBoolean(configuration["debug"], CultureInfo.InvariantCulture);
            this.host = configuration["host"];
            configuration.Remove("msdCookieExpirationInDays");
            configuration.Remove("logOffUrl");
            configuration.Remove("logonUrl");
            configuration.Remove("msdCookieName");
            configuration.Remove("msdDomain");
            configuration.Remove("vsdCookieName");
            configuration.Remove("vsdCookieExpirationInHours");
            configuration.Remove("msdAuthenticateTicketUrl");
            configuration.Remove("msdCreateTicketUrl");
            configuration.Remove("msdDeleteTicketUrl");
            configuration.Remove("vsdLandingUrl");
            configuration.Remove("debug");
            configuration.Remove("host");
            if (configuration.Count > 0)
            {
                string key = configuration.GetKey(0);
                if ((key != null) && (key.Length > 0))
                {
                    throw new TheKnot.Membership.Security.Providers.ProviderException("Attribute not recognized '" + key + "'");
                }
            }
        }

        private string MatchDecode(Match m)
        {
            int index = 0;
            return Uri.HexUnescape(m.Value, ref index).ToString();
        }

        internal long ReadUserIdFromVsdCookie()
        {
            long num = 0L;
            HttpCookie cookie = HttpContext.Current.Request.Cookies[this.vsdCookieName];
            if (((cookie != null) && (cookie["userid"] != null)) && (cookie["oldmemberid"] != null))
            {
                num = Convert.ToInt64(TheKnot.Membership.Security.HelperClasses.CryptoHelper.DecryptDesFromWeb(cookie["userid"]));
                TheKnot.Membership.Security.HelperClasses.CryptoHelper.DecryptDesFromWeb(cookie["oldmemberid"]);
            }
            return num;
        }

        internal override void RemoveAuthenticationTickets()
        {
            if (HttpContext.Current.Request.QueryString["MsdVisit"] != "1")
            {
                this.DeleteVsdTicket();
                string str = HttpContext.Current.Server.UrlEncode(HttpContext.Current.Request.Url.AbsoluteUri);
                HttpContext.Current.Response.Redirect(this.MsdDeleteTicketUrl + "?originalTarget=" + str, true);
            }
        }

        internal override bool VerifyVsdTicket(bool isVerified)
        {
            try
            {
                if (HttpContext.Current.Request.Cookies[this.vsdCookieName] == null)
                {
                    return false;
                }
                HttpContext.Current.Request.Cookies[this.vsdCookieName].Domain = TheKnot.Membership.Security.SiteContext.RootDomainForHttpCookie;
                HttpContext.Current.Request.Cookies[this.vsdCookieName]["IsVerified"] = isVerified ? "1" : "0";
                HttpContext.Current.Response.Cookies[this.vsdCookieName].Domain = HttpContext.Current.Request.Cookies[this.vsdCookieName].Domain;
                HttpContext.Current.Response.Cookies[this.vsdCookieName].Value = HttpContext.Current.Request.Cookies[this.vsdCookieName].Value;
            }
            catch
            {
                return false;
            }
            return true;
        }

        public override bool Debug
        {
            get
            {
                return this.debug;
            }
        }

        public override string Host
        {
            get
            {
                return this.host;
            }
        }

        public override string LogOffUrl
        {
            get
            {
                return string.Format(CultureInfo.InvariantCulture, this.logOffUrl, new object[] { TheKnot.Membership.Security.SiteContext.RootDomainForHttpCookie });
            }
        }

        public override string LogonUrl
        {
            get
            {
                if (TheKnot.Membership.Security.SiteContext.Current == 12)
                {
                    return ("http://" + HttpContext.Current.Request.Url.Host + "/join/memberlogin.aspx");
                }
                return string.Format(CultureInfo.InvariantCulture, this.logonUrl, new object[] { TheKnot.Membership.Security.SiteContext.RootDomainForHttpCookie });
            }
        }

        public override Uri MsdAuthenticateTicketUrl
        {
            get
            {
                return this.msdAuthenticateTicketUrl;
            }
        }

        public int MsdCookieExpirationInDays
        {
            get
            {
                return this.msdCookieExpirationInDays;
            }
        }

        public string MsdCookieName
        {
            get
            {
                return this.msdCookieName;
            }
        }

        public override Uri MsdCreateTicketUrl
        {
            get
            {
                return this.msdCreateTicketUrl;
            }
        }

        public override Uri MsdDeleteTicketUrl
        {
            get
            {
                return this.msdDeleteTicketUrl;
            }
        }

        public string MsdDomain
        {
            get
            {
                return this.msdDomain;
            }
        }

        public int VsdCookieExpirationInHours
        {
            get
            {
                return this.vsdCookieExpirationInHours;
            }
        }

        public override string VsdCookieName
        {
            get
            {
                return this.vsdCookieName;
            }
        }

        public override string VsdLandingUrl
        {
            get
            {
                return string.Format(CultureInfo.InvariantCulture, this.vsdLandingUrl, new object[] { TheKnot.Membership.Security.SiteContext.RootDomainForHttpCookie });
            }
        }
    }
}

