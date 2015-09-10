namespace TheKnot.Membership.Security
{
    using System;

    [Flags]
    public enum GatingRules
    {
        None = 0,
        PostWedding = 2,
        ShippingAddressMissing = 8,
        SiteVerification = 1,
        UserNameRequired = 4
    }
}

