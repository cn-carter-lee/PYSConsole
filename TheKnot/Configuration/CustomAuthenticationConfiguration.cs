namespace TheKnot.Membership.Security.Configuration
{
    using System;
    using System.Collections;
    using TheKnot.Membership.Security.Providers;

    internal sealed class CustomAuthenticationConfiguration
    {
        private TheKnot.Membership.Security.Providers.CustomAuthenticationProvider defaultProvider;
        private Hashtable providerList;

        public CustomAuthenticationConfiguration(Hashtable providers, TheKnot.Membership.Security.Providers.CustomAuthenticationProvider cap)
        {
            this.providerList = providers;
            this.defaultProvider = cap;
        }

        public TheKnot.Membership.Security.Providers.CustomAuthenticationProvider GetProvider(string name)
        {
            return (this.providerList[name] as TheKnot.Membership.Security.Providers.CustomAuthenticationProvider);
        }

        public TheKnot.Membership.Security.Providers.CustomAuthenticationProvider DefaultProvider
        {
            get
            {
                return this.defaultProvider;
            }
        }
    }
}

