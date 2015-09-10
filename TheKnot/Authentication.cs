namespace TheKnot.Membership.Security
{
    using System;
    using System.Configuration;
    using System.Web;
    using TheKnot.Membership;
    using TheKnot.Membership.Security.Configuration;
    using TheKnot.Membership.Security.HelperClasses;
    using TheKnot.Membership.Security.Providers;

    public sealed class Authentication
    {
        private static TheKnot.Membership.Security.Configuration.CustomAuthenticationConfiguration configuration = (ConfigurationSettings.GetConfig("TheKnot.Membership.Security") as TheKnot.Membership.Security.Configuration.CustomAuthenticationConfiguration);

        static Authentication()
        {
            if ((configuration == null) || (configuration.DefaultProvider == null))
            {
                throw new ConfigurationException("Unable to get 'TheKnot.Membership.Security' configuration.");
            }
        }

        private Authentication()
        {
        }

        public static ValidationStatusEnum Authenticate(long memberId, Uri target, bool createTickets)
        {
            IAdminMember member = AdminMemberFactory.GetInstance(TheKnot.Membership.Security.SiteContext.Current).Find(memberId);
            if (member == null)
            {
                return 1;
            }
            if ((member.get_Status() != 1) && (member.get_Status() != 4))
            {
                return 2;
            }
            if ((member.get_MembershipStatus() != 2) && (member.get_MembershipStatus() != 1))
            {
                return 3;
            }
            if (createTickets)
            {
                if (target == null)
                {
                    throw new ArgumentNullException("Target URI cannot be null when creating authentication tickets");
                }
                Provider.CreateAuthenticationTickets(member.get_MemberId(), target);
            }
            return 0;
        }

        public static ValidationStatusEnum Authenticate(string email, string password, Uri target, bool createTickets)
        {
            if ((email == null) || (email.Trim().Length == 0))
            {
                throw new ArgumentNullException("email");
            }
            IAdminMember member = AdminMemberFactory.GetInstance(TheKnot.Membership.Security.SiteContext.Current).Find(email, password);
            if (member == null)
            {
                return 1;
            }
            if ((member.get_Status() != 1) && (member.get_Status() != 4))
            {
                return 2;
            }
            if ((member.get_MembershipStatus() != 2) && (member.get_MembershipStatus() != 1))
            {
                return 3;
            }
            if (createTickets)
            {
                if (target == null)
                {
                    throw new ArgumentNullException("Target URI cannot be null when creating authentication tickets");
                }
                Provider.CreateAuthenticationTickets(member.get_MemberId(), target);
            }
            return 0;
        }

        public static ValidationStatusEnum Authenticate(string email, string password, Uri target, bool createTickets, long facebookID)
        {
            if ((email == null) || (email.Trim().Length == 0))
            {
                throw new ArgumentNullException("email");
            }
            IAdminMember member = AdminMemberFactory.GetInstance(TheKnot.Membership.Security.SiteContext.Current).Find(email, password);
            if (member == null)
            {
                return 1;
            }
            if ((member.get_Status() != 1) && (member.get_Status() != 4))
            {
                return 2;
            }
            if ((member.get_MembershipStatus() != 2) && (member.get_MembershipStatus() != 1))
            {
                return 3;
            }
            member.CreateFacebookConnect(TheKnot.Membership.Security.SiteContext.Current).set_FacebookID(facebookID);
            AdminMemberFactory.GetInstance(TheKnot.Membership.Security.SiteContext.Current).SaveFacebook(member, TheKnot.Membership.Security.SiteContext.Current, facebookID);
            if (createTickets)
            {
                if (target == null)
                {
                    throw new ArgumentNullException("Target URI cannot be null when creating authentication tickets");
                }
                Provider.CreateAuthenticationTickets(member.get_MemberId(), target);
            }
            return 0;
        }

        public static void CreateVsdTicket()
        {
            Provider.CreateVsdTicket();
        }

        internal static long GetUserID()
        {
            try
            {
                if (HttpContext.Current.Items["UserIdentity"] != null)
                {
                    return (long) HttpContext.Current.Items["UserIdentity"];
                }
                return 0L;
            }
            catch
            {
                return 0L;
            }
        }

        internal static bool IsAuthenticated(bool forceLogOn)
        {
            if (GetUserID() > 0L)
            {
                return true;
            }
            if (forceLogOn)
            {
                HttpContext.Current.Response.Redirect(Provider.LogonUrl + "?target=" + HttpContext.Current.Server.UrlEncode(TheKnot.Membership.Security.HelperClasses.UrlHelper.GetTargetUrl(HttpContext.Current)), true);
            }
            return false;
        }

        public static void RemoveAuthenticationTickets()
        {
            Provider.RemoveAuthenticationTickets();
        }

        public static void VerifyVsdTicket(bool isVerified)
        {
            Provider.VerifyVsdTicket(isVerified);
        }

        internal static TheKnot.Membership.Security.Providers.CustomAuthenticationProvider Provider
        {
            get
            {
                return configuration.GetProvider("CookieAuthenticationProvider");
            }
        }

        public static string VsdCookieName
        {
            get
            {
                return Provider.VsdCookieName;
            }
        }
    }
}

