namespace TheKnot.Membership.Security.Providers
{
    using System;
    using System.Runtime.Serialization;

    [Serializable]
    public class CustomAuthenticationException : Exception
    {
        public CustomAuthenticationException()
        {
        }

        public CustomAuthenticationException(string message) : base(message)
        {
        }

        protected CustomAuthenticationException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public CustomAuthenticationException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}

