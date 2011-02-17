#region usings

using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Xml;
using Microsoft.Security.Application;

#endregion

namespace ASC.Common.Utils
{
    public class HtmlUtil
    {
        protected static Regex htmlTags = new Regex(@"</?(H|h)(T|t)(M|m)(L|l)(.|\n)*?>");

        public static string GetText(string html)
        {
            return GetText(html, 0);
        }

		public static string GetText(string html, bool preserveSpaces)
		{
			return GetText(html, 0, "...", false, preserveSpaces);
		}

        public static string GetText(string html, int maxLength)
        {
            return GetText(html, maxLength, "...");
        }

		public static string GetText(string html, int maxLength, bool preserveSpaces)
		{
			return GetText(html, maxLength, "...", false, preserveSpaces);
		}

        public static string GetText(string html, int maxLength, string endBlockTemplate)
        {
            return GetText(html, maxLength, endBlockTemplate, false);
        }

        public static string GetText(string html, int maxLength, string endBlockTemplate, bool calcEndBlockTemplate)
        {
			return GetText(html, maxLength, endBlockTemplate, calcEndBlockTemplate, false);
        }

		public static string GetText(string html, int maxLength, string endBlockTemplate, bool calcEndBlockTemplate, bool preserveSpaces)
		{
			html = html ?? string.Empty;
			endBlockTemplate = endBlockTemplate ?? string.Empty;

			string unformatedText = html;

			if (preserveSpaces)
			{
				var regex = new Regex(	@"<br\s*\/*>",	RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.IgnorePatternWhitespace | RegexOptions.Compiled);
				unformatedText = HttpUtility.HtmlDecode(regex.Replace(unformatedText, " "));
			}

			var tags = new Regex(@"</?(.|\n)*?>");
			unformatedText = HttpUtility.HtmlDecode(tags.Replace(unformatedText, string.Empty));

			var spaces = new Regex(@"\s+");
			unformatedText = HttpUtility.HtmlDecode(spaces.Replace(unformatedText, " ")).Trim();

			if (maxLength == 0 || unformatedText.Length < maxLength)
				return unformatedText;
			maxLength = calcEndBlockTemplate ? maxLength - endBlockTemplate.Length : maxLength;
			int lastSpaceIndex = 0, spaceIndex = 0;
			while (spaceIndex < maxLength)
			{
				lastSpaceIndex = spaceIndex;
				spaceIndex = unformatedText.IndexOf(' ', lastSpaceIndex + 1);
				if (spaceIndex < 0)
					break;
			}
			if (lastSpaceIndex > 0)
			{
				unformatedText = unformatedText.Remove(lastSpaceIndex);
			}
			else
			{
				unformatedText = unformatedText.Substring(0, maxLength);
			}
			if (!endBlockTemplate.Equals(string.Empty))
			{
				unformatedText += endBlockTemplate;
			}
			return unformatedText;
		}

        public static string ToPlainText(string html)
        {
            if (string.IsNullOrEmpty(html)) return html;
            html = HttpUtility.HtmlDecode(html).Replace("&", "&amp;");
            html = Regex.Replace(html, @"<br(\s)*>", "<br/>");
            html = string.Format("<root>{0}</root>", html);
            var xmlDocument = new XmlDocument();
            var settings = new XmlReaderSettings
                               {
                                   IgnoreWhitespace = true,
                                   IgnoreComments = true,
                                   IgnoreProcessingInstructions = true,
                                   CloseInput = true,
                               };
            xmlDocument.Load(XmlReader.Create(new StringReader(html), settings));
            var text = new StringBuilder();
            HtmlNodeToString(xmlDocument, text);
            return text.ToString();
        }

        private static void HtmlNodeToString(XmlNode node, StringBuilder text)
        {
            string value = node.Value;
            if (value != null && 0 < value.Length)
            {
                var spaces = new[] {' ', '\t', '\v', '\r', '\n', (char) 160,};
                bool spaceStart = value.IndexOfAny(spaces, 0, 1) == 0;
                bool spaceEnd = value.IndexOfAny(spaces, value.Length - 1, 1) == value.Length - 1;
                value = (spaceStart ? " " : "") + value.Trim(spaces) + (spaceEnd ? " " : "");
                text.Append(value);
            }
            if (node.Name == "a" || node.Name == "img")
            {
                foreach (XmlNode child in node.ChildNodes)
                {
                    HtmlNodeToString(child, text);
                }
                const string linkFormat = " {0}";
                if (node.Name == "a" && GetAttribute(node, "href") != node.InnerXml)
                    text.AppendFormat(linkFormat, GetAttribute(node, "href"));
                if (node.Name == "img" && GetAttribute(node, "src") != node.InnerXml)
                    text.AppendFormat(linkFormat, GetAttribute(node, "src"));
                return;
            }
            if (node.Name == "div" || node.Name == "br" || node.Name == "p") text.AppendLine();
            foreach (XmlNode child in node.ChildNodes)
            {
                HtmlNodeToString(child, text);
            }
        }

        private static string GetAttribute(XmlNode node, string attributeName)
        {
            XmlNode attribute = node.Attributes.GetNamedItem(attributeName);
            return attribute != null ? attribute.Value : null;
        }

        public static string SanitizeFragment(string htmlText)
        {
            return Sanitizer.GetSafeHtmlFragment(htmlText);
        }

        public static string SanitizeHtml(string htmlText)
        {
            return Sanitizer.GetSafeHtml(htmlText);
        }
    }
}