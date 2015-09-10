namespace TheKnot.Membership.Security.HelperClasses
{
    using System;
    using System.Configuration;
    using System.Globalization;
    using System.Xml;

    internal class XmlNodeHelper
    {
        private XmlNodeHelper()
        {
        }

        internal static XmlNode GetAndRemoveAttribute(XmlNode node, string attribute, bool isRequired, ref int input)
        {
            XmlNode node2 = node.Attributes.RemoveNamedItem(attribute);
            if (isRequired && ((node2 == null) || (node2.Value.Trim().Length == 0)))
            {
                throw new ConfigurationException("Missing required attribute: " + attribute, node);
            }
            try
            {
                input = Convert.ToInt32(node2.Value, CultureInfo.InvariantCulture);
            }
            catch (FormatException)
            {
                throw new ConfigurationException(attribute + " attribute must be of type Int.");
            }
            catch (OverflowException)
            {
                int num = 0x7fffffff;
                throw new ConfigurationException(attribute + " attribute must be greater than zero and less than " + num.ToString(CultureInfo.InvariantCulture));
            }
            return node2;
        }

        internal static XmlNode GetAndRemoveAttribute(XmlNode node, string attribute, bool isRequired, ref string input)
        {
            XmlNode node2 = node.Attributes.RemoveNamedItem(attribute);
            if (isRequired && ((node2 == null) || (node2.Value.Trim().Length == 0)))
            {
                throw new ConfigurationException("Missing required attribute: " + attribute, node);
            }
            input = node2.Value;
            return node2;
        }
    }
}

