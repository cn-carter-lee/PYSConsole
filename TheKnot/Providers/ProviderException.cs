namespace TheKnot.Membership.Security.Providers
{
    using System;
    using System.Runtime.Serialization;

    [Serializable, Obsolete("Use TheKnot.Membership assembly instead.")]
    public class ProviderException : Exception
    {
        public ProviderException()
        {
        }

        public ProviderException(string message) : base(message)
        {
        }

        protected ProviderException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public ProviderException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}

