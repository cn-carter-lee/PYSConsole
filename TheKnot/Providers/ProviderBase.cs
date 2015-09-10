namespace TheKnot.Membership.Security.Providers
{
    using System;
    using System.Collections.Specialized;
    using System.Data.SqlClient;

    public abstract class ProviderBase
    {
        private string description;
        private bool initialized;
        private string name;
        private SqlTransaction transaction;

        protected ProviderBase()
        {
        }

        public virtual void Initialize(string name, NameValueCollection configuration)
        {
            lock (this)
            {
                if (this.initialized)
                {
                    throw new InvalidOperationException("Provider already initialized");
                }
                this.initialized = true;
            }
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }
            if (name.Length == 0)
            {
                throw new ArgumentException("configuration provider name null or empty.", "name");
            }
            this.name = name;
            if (configuration != null)
            {
                this.description = configuration["description"];
                configuration.Remove("description");
            }
        }

        public virtual string Description
        {
            get
            {
                return this.description;
            }
        }

        public virtual string Name
        {
            get
            {
                return this.name;
            }
        }

        public virtual SqlTransaction Transaction
        {
            get
            {
                return this.transaction;
            }
            set
            {
                this.transaction = value;
            }
        }
    }
}

