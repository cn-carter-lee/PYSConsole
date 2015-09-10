using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PYS.Entity
{
    public class TargetEncryptedSessionInfo
    {
        public string requestId
        {
            get;
            set;
        }

        public string registryEmail
        {
            get;
            set;
        }

        public string grSessionId
        {
            get;
            set;
        }

        public string cookie
        {
            get;
            set;
        }

        public string listId
        {
            get;
            set;
        }
    }
}
