namespace TheKnot.Membership.Security.Configuration
{
    using System;
    using System.Collections;
    using System.Collections.Specialized;
    using System.Configuration;
    using System.Xml;
    using TheKnot.Membership.Security.Components;
    using TheKnot.Membership.Security.HelperClasses;
    using TheKnot.Membership.Security.Providers;

    internal sealed class CustomAuthenticationConfigurationHandler : IConfigurationSectionHandler
    {
        public object Create(object parent, object configContext, XmlNode section)
        {
            if (section == null)
            {
                throw new ArgumentNullException("section");
            }
            Hashtable providers = new Hashtable();
            TheKnot.Membership.Security.Providers.CustomAuthenticationProvider cap = null;
            foreach (XmlNode node in section.ChildNodes)
            {
                if ((node.NodeType == XmlNodeType.Element) && (node.Name == "providers"))
                {
                    foreach (XmlNode node2 in node.ChildNodes)
                    {
                        TheKnot.Membership.Security.Providers.CustomAuthenticationProvider provider2 = this.CreateProviderFromXml(node2);
                        providers.Add(provider2.Name, provider2);
                        if (cap == null)
                        {
                            cap = provider2;
                        }
                    }
                }
            }
            return new TheKnot.Membership.Security.Configuration.CustomAuthenticationConfiguration(providers, cap);
        }

        private TheKnot.Membership.Security.Providers.CustomAuthenticationProvider CreateProviderFromXml(XmlNode node)
        {
            if ((node.NodeType != XmlNodeType.Element) || !(node.Name == "add"))
            {
                throw new ConfigurationException("Unrecognized tag: " + node.Name, node);
            }
            string input = null;
            string str2 = null;
            TheKnot.Membership.Security.HelperClasses.XmlNodeHelper.GetAndRemoveAttribute(node, "name", true, ref input);
            TheKnot.Membership.Security.HelperClasses.XmlNodeHelper.GetAndRemoveAttribute(node, "type", true, ref str2);
            NameValueCollection configuration = new NameValueCollection();
            foreach (XmlAttribute attribute in node.Attributes)
            {
                if ((attribute.Name != null) && (attribute.Name.Length > 0))
                {
                    configuration.Add(attribute.Name, attribute.Value);
                }
            }
            TheKnot.Membership.Security.Providers.CustomAuthenticationProvider provider = (TheKnot.Membership.Security.Providers.CustomAuthenticationProvider) Activator.CreateInstance(Type.GetType(str2, true));
            provider.Initialize(input, configuration);
            foreach (XmlNode node2 in node.ChildNodes)
            {
                if (node2.Name == "msdRules")
                {
                    foreach (XmlNode node3 in node2.ChildNodes)
                    {
                        if ((node3.NodeType == XmlNodeType.Element) && (node3.Name == "add"))
                        {
                            string str3 = null;
                            string str4 = null;
                            TheKnot.Membership.Security.HelperClasses.XmlNodeHelper.GetAndRemoveAttribute(node3, "name", true, ref str3);
                            TheKnot.Membership.Security.HelperClasses.XmlNodeHelper.GetAndRemoveAttribute(node3, "type", true, ref str4);
                            provider.MsdRules.Add(new TheKnot.Membership.Security.Components.MsdRuleItem(str3, str4));
                        }
                    }
                }
            }
            return provider;
        }
    }
}

