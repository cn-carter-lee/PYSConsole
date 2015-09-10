namespace TheKnot.Membership.Security
{
    using System;
    using System.Web;
    using TheKnot.Membership;
    using TheKnot.Membership.Security.HelperClasses;

    internal class GatingManager
    {
        internal static void Gate(TheKnot.Membership.Security.GatingRules settings, IAdminMember member, SiteEnum site)
        {
            if ((((member != null) && (settings != TheKnot.Membership.Security.GatingRules.None)) && ((HttpContext.Current != null) && (site != null))) && TheKnot.Membership.Security.Authentication.IsAuthenticated(false))
            {
                TheKnot.Membership.Security.GatingRules none = TheKnot.Membership.Security.GatingRules.None;
                bool flag = false;
                IUserSiteVerification userSiteVerification = member.GetUserSiteVerification(site);
                if ((((((site != 12) && (site != 7)) && (site != 1)) || ((site == 1) && (member.get_WeddingDate() == DateTime.MinValue))) || (((site == 7) && (member.get_WeddingDate() == DateTime.MinValue)) || ((site == 12) && (member.get_WeddingDate() == DateTime.MinValue)))) || (((((site == 12) && (member.get_OriginationSite() != 12)) && (member.get_WeddingDate() > DateTime.Now)) || (((site == 7) && (member.get_OriginationSite() != 7)) && (member.get_WeddingDate() > DateTime.Now))) || (((site == 1) && (member.get_OriginationSite() != 1)) && (member.get_WeddingDate() > DateTime.Now))))
                {
                    if ((userSiteVerification == null) && ((settings & TheKnot.Membership.Security.GatingRules.SiteVerification) == TheKnot.Membership.Security.GatingRules.SiteVerification))
                    {
                        flag = true;
                        none |= TheKnot.Membership.Security.GatingRules.SiteVerification;
                    }
                    else if (!userSiteVerification.get_Verified() && ((settings & TheKnot.Membership.Security.GatingRules.SiteVerification) == TheKnot.Membership.Security.GatingRules.SiteVerification))
                    {
                        flag = !userSiteVerification.get_Verified();
                        none |= TheKnot.Membership.Security.GatingRules.SiteVerification;
                    }
                }
                if (((userSiteVerification != null) && ((settings & TheKnot.Membership.Security.GatingRules.PostWedding) == TheKnot.Membership.Security.GatingRules.PostWedding)) && (!userSiteVerification.get_PostWeddingVerified() && (member.get_WeddingDate().Date < DateTime.Now)))
                {
                    flag = flag || (!userSiteVerification.get_PostWeddingVerified() && (member.get_WeddingDate().Date < DateTime.Now));
                    none |= TheKnot.Membership.Security.GatingRules.PostWedding;
                }
                if (((settings & TheKnot.Membership.Security.GatingRules.UserNameRequired) == TheKnot.Membership.Security.GatingRules.UserNameRequired) && ((member.get_UserName() == null) || (member.get_UserName().Trim().Length == 0)))
                {
                    flag = (flag || (member.get_UserName() == null)) || (member.get_UserName().Trim().Length == 0);
                    none |= TheKnot.Membership.Security.GatingRules.UserNameRequired;
                }
                if (!flag && ((settings & TheKnot.Membership.Security.GatingRules.ShippingAddressMissing) == TheKnot.Membership.Security.GatingRules.ShippingAddressMissing))
                {
                    bool flag2 = false;
                    foreach (IAddress address in member.get_Addresses())
                    {
                        if ((address.get_AddressType() == 3) && !address.IsEmpty())
                        {
                            flag2 = true;
                            break;
                        }
                    }
                    if (!flag2)
                    {
                        flag = true;
                        none = TheKnot.Membership.Security.GatingRules.ShippingAddressMissing;
                    }
                }
                if (flag)
                {
                    HttpContext.Current.Response.Redirect(string.Concat(new object[] { TheKnot.Membership.Security.MemberContext.GetSiteMemberProfileUrl(site), "?gate=", (int) none, "&target=", HttpContext.Current.Server.UrlEncode(TheKnot.Membership.Security.HelperClasses.UrlHelper.GetTargetUrl(HttpContext.Current)) }), true);
                }
            }
        }
    }
}

