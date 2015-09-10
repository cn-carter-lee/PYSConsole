namespace TheKnot.Membership.Security.Components
{
    using System;
    using TheKnot.Membership;

    public abstract class MsdRuleBase
    {
        protected MsdRuleBase()
        {
        }

        public abstract void ProcessRequest(long userId, SiteEnum siteType, ref string originalTarget);

        public abstract ExecutionPoint ExecutionPoints { get; }

        [Flags]
        public enum ExecutionPoint
        {
            TicketAuthentication = 1,
            TicketCreation = 2
        }
    }
}

