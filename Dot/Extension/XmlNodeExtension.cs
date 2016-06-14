using System.Xml;

namespace Dot.Extension
{
    public static class XmlNodeExtension
    {
        public static XmlNode GetNode(this XmlNode parentNode, string nodeName)
        {
            return parentNode.SelectSingleNode(nodeName);
        }

        public static string GetAttributeValue(this XmlNode node, string attrName, string defaultValue = "")
        {
            if (node == null || node.Attributes == null || node.Attributes.Count == 0)
                return defaultValue;

            var attr = node.Attributes[attrName];
            return attr != null ? attr.Value : defaultValue;
        }
    }
}