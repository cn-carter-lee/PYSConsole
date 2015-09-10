namespace TheKnot.Membership.Security.Providers
{
    using System;
    using System.Collections;
    using TheKnot.Membership;

    [Obsolete("Use TheKnot.Membership assembly instead.")]
    public sealed class ProviderFactory
    {
        private static TheKnot.Membership.Security.Providers.ProviderFactory instance;
        private Hashtable membershipSettings = ((Hashtable) ConfigurationSettings.GetConfig("SecuritySettings/Membership"));
        private Hashtable profileSettings = ((Hashtable) ConfigurationSettings.GetConfig("SecuritySettings/Profile"));

        private ProviderFactory()
        {
            this.CheckInvariance();
        }

        private void CheckInvariance()
        {
            if ((this.profileSettings == null) || (this.profileSettings.Count == 0))
            {
                throw new ApplicationException("Unable to get SecuritySettings/Profile configuration.");
            }
            if ((this.membershipSettings == null) || (this.membershipSettings.Count == 0))
            {
                throw new ApplicationException("Unable to get SecuritySettings/Membership configuration.");
            }
        }

        private string GetMembershipProviderName(SiteEnum site)
        {
            return "SqlMembershipProvider";
        }

        private string GetProfileProviderName(SiteEnum site)
        {
            return "SqlProfileProvider";
        }

        public static TheKnot.Membership.Security.Providers.ProviderFactory Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new TheKnot.Membership.Security.Providers.ProviderFactory();
                }
                return instance;
            }
        }
    }
}

