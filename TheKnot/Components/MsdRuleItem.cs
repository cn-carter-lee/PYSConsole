namespace TheKnot.Membership.Security.Components
{
    using System;

    [Serializable]
    public class MsdRuleItem
    {
        private bool enabled;
        private string name;
        private string type;

        public MsdRuleItem()
        {
            this.name = string.Empty;
            this.type = string.Empty;
            this.enabled = true;
        }

        internal MsdRuleItem(string name, string type)
        {
            this.name = string.Empty;
            this.type = string.Empty;
            this.enabled = true;
            this.name = name;
            this.type = type;
        }

        public bool Enabled
        {
            get
            {
                return this.enabled;
            }
            set
            {
                this.enabled = value;
            }
        }

        public string Name
        {
            get
            {
                return this.name;
            }
            set
            {
                this.name = value;
            }
        }

        public string Type
        {
            get
            {
                return this.type;
            }
            set
            {
                this.type = value;
            }
        }
    }
}

