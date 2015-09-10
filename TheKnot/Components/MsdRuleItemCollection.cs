namespace TheKnot.Membership.Security.Components
{
    using System;
    using System.Collections;
    using System.Reflection;

    [Serializable]
    public class MsdRuleItemCollection : CollectionBase
    {
        public int Add(TheKnot.Membership.Security.Components.MsdRuleItem value)
        {
            return ((IList) this).Add(value);
        }

        public bool Contains(TheKnot.Membership.Security.Components.MsdRuleItem value)
        {
            return ((IList) this).Contains(value);
        }

        public void CopyTo(Exception[] array, int index)
        {
            ((ICollection) this).CopyTo(array, index);
        }

        public int IndexOf(TheKnot.Membership.Security.Components.MsdRuleItem value)
        {
            return ((IList) this).IndexOf(value);
        }

        public void Insert(int index, TheKnot.Membership.Security.Components.MsdRuleItem value)
        {
            ((IList) this).Insert(index, value);
        }

        public void Remove(TheKnot.Membership.Security.Components.MsdRuleItem value)
        {
            ((IList) this).Remove(value);
        }

        public TheKnot.Membership.Security.Components.MsdRuleItem this[int index]
        {
            get
            {
                return (TheKnot.Membership.Security.Components.MsdRuleItem) this[index];
            }
            set
            {
                this[index] = value;
            }
        }
    }
}

