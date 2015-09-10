namespace TheKnot.Membership.Security.Providers
{
    using System;
    using TheKnot.Membership.Security.Components;

    public abstract class CustomAuthenticationProvider : TheKnot.Membership.Security.Providers.ProviderBase
    {
        private TheKnot.Membership.Security.Components.MsdRuleItemCollection msdRuleItemCollection = new TheKnot.Membership.Security.Components.MsdRuleItemCollection();

        protected CustomAuthenticationProvider()
        {
        }

        internal abstract void AuthenticateMsdRequest();
        internal abstract void AuthenticateVsdRequest();
        internal abstract void CreateAuthenticationTickets(decimal userId, Uri target);
        internal abstract void CreateMsdTicket();
        internal abstract void CreateVsdTicket();
        internal abstract void DeleteMsdTicket();
        internal abstract void DeleteVsdTicket();
        internal abstract void RemoveAuthenticationTickets();
        internal abstract bool VerifyVsdTicket(bool isVerified);

        public abstract bool Debug { get; }

        public abstract string Host { get; }

        public abstract string LogOffUrl { get; }

        public abstract string LogonUrl { get; }

        public abstract Uri MsdAuthenticateTicketUrl { get; }

        public abstract Uri MsdCreateTicketUrl { get; }

        public abstract Uri MsdDeleteTicketUrl { get; }

        internal TheKnot.Membership.Security.Components.MsdRuleItemCollection MsdRules
        {
            get
            {
                return this.msdRuleItemCollection;
            }
        }

        public abstract string VsdCookieName { get; }

        public abstract string VsdLandingUrl { get; }
    }
}

