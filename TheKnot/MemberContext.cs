namespace TheKnot.Membership.Security
{
    using System;
    using System.Web;
    using TheKnot.Membership;

    public sealed class MemberContext
    {
        private const string BASE_KEY = "MemberContext:";
        private IAdminMember member;

        private MemberContext()
        {
            this.member = null;
            if (TheKnot.Membership.Security.Authentication.IsAuthenticated(false))
            {
                long userID = TheKnot.Membership.Security.Authentication.GetUserID();
                this.member = this.GetAdminMemberFromContext("MemberContext:" + userID);
                if (this.member == null)
                {
                    this.member = AdminMemberFactory.GetInstance(TheKnot.Membership.Security.SiteContext.Current).Find(userID);
                    this.SaveToContext(this.member, "MemberContext:" + userID);
                }
            }
        }

        private MemberContext(long userId)
        {
            this.member = null;
            this.member = this.GetAdminMemberFromContext("MemberContext:" + userId);
            if (this.member == null)
            {
                this.member = AdminMemberFactory.GetInstance(TheKnot.Membership.Security.SiteContext.Current).Find(userId);
                this.SaveToContext(this.member, "MemberContext:" + userId);
            }
        }

        public static void ForceMemCache()
        {
            ForceMemCache(true);
        }

        public static void ForceMemCache(bool isTurnedOn)
        {
            HttpContext.Current.Items["TheKnot.Membership.IgnoreHttpCache"] = isTurnedOn;
        }

        public void Gate()
        {
            TheKnot.Membership.Security.GatingManager.Gate(TheKnot.Membership.Security.GatingRules.UserNameRequired | TheKnot.Membership.Security.GatingRules.PostWedding | TheKnot.Membership.Security.GatingRules.SiteVerification, this.member, TheKnot.Membership.Security.SiteContext.Current);
        }

        public void Gate(TheKnot.Membership.Security.GatingRules settings)
        {
            TheKnot.Membership.Security.GatingManager.Gate(settings, this.member, TheKnot.Membership.Security.SiteContext.Current);
        }

        private IAdminMember GetAdminMemberFromContext(string key)
        {
            if (HttpContext.Current == null)
            {
                return null;
            }
            return (IAdminMember) HttpContext.Current.Items[key];
        }

        public static TheKnot.Membership.Security.MemberContext GetContextForUser(long userId)
        {
            return new TheKnot.Membership.Security.MemberContext(userId);
        }

        internal static string GetSiteMemberProfileUrl(SiteEnum site)
        {
            switch (site)
            {
                case 2:
                    return "http://global.thenest.com/join/MemberProfile.aspx";

                case 3:
                    return "http://global.lilaguide.com/join/MemberProfile.aspx";

                case 4:
                    return "http://global.thenestbaby.com/join/MemberProfile.aspx";

                case 7:
                    return "http://global.weddingchannel.com/join/MemberProfile.aspx";

                case 11:
                    return "http://global.thebump.com/join/MemberProfile.aspx";

                case 12:
                    return ("http://" + HttpContext.Current.Request.Url.Host + "/join/MemberProfile.aspx");

                case 15:
                    return "http://global.giftregistry360.com/join/MemberProfile.aspx";

                case 0x10:
                    return "http://global.breastfeeding.com/join/MemberProfile.aspx";
            }
            return "http://global.theknot.com/join/MemberProfile.aspx";
        }

        public bool IsAuthenticated(bool force)
        {
            return this.IsAuthenticated(force, TheKnot.Membership.Security.GatingRules.None);
        }

        public bool IsAuthenticated(bool force, TheKnot.Membership.Security.GatingRules gatingRules)
        {
            bool flag = false;
            if (force)
            {
                flag = TheKnot.Membership.Security.Authentication.IsAuthenticated(true);
                this.Gate(gatingRules);
            }
            else
            {
                flag = TheKnot.Membership.Security.Authentication.IsAuthenticated(force) && (this.member != null);
            }
            if (flag && (gatingRules != TheKnot.Membership.Security.GatingRules.None))
            {
                this.Gate(gatingRules);
            }
            return flag;
        }

        private void SaveToContext(object objectToCache, string key)
        {
            if ((objectToCache != null) && (HttpContext.Current != null))
            {
                HttpContext.Current.Items[key] = objectToCache;
            }
        }

        public IAdminMember AdminMember
        {
            get
            {
                return this.member;
            }
        }

        public static TheKnot.Membership.Security.MemberContext Current
        {
            get
            {
                return new TheKnot.Membership.Security.MemberContext();
            }
        }

        public INestingMember NestingMember
        {
            get
            {
                return this.member;
            }
        }

        public IParentingMember ParentingMember
        {
            get
            {
                return this.member;
            }
        }

        public IWeddingMember WeddingMember
        {
            get
            {
                return this.member;
            }
        }
    }
}

