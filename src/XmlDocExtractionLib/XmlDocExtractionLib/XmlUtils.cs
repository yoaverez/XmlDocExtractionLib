using System.Collections;
using System.Linq;
using System.Xml.Linq;

namespace XmlDocExtractionLib
{
    /// <summary>
    /// Extensions for xml nodes and elements.
    /// </summary>
    internal static class XmlUtils
    {
        /// <summary>
        /// Remove the given <paramref name="element"/> from it parent and also
        /// remove the next whitespace afterwards.
        /// </summary>
        /// <param name="element">The element to remove.</param>
        public static void RemoveElementWithNextWhitespace(this XElement element)
        {
            var nextNode = element.NodesAfterSelf().FirstOrDefault();
            element.Remove();
            if(nextNode != null)
            {
                if(nextNode is XText textNode)
                {
                    textNode.Value = textNode.Value.TrimStart();
                    if(string.IsNullOrEmpty(textNode.Value))
                        textNode.Remove();
                }
            }
        }

        /// <summary>
        /// Replace the given <paramref name="element"/>
        /// with the given <paramref name="xpathEvaluation"/>
        /// after trimming the whitespace from the <paramref name="xpathEvaluation"/>.
        /// </summary>
        /// <param name="element">The element to replace.</param>
        /// <param name="xpathEvaluation">The replacement of the <paramref name="element"/>.</param>
        public static void ReplaceElementAndRemovePrevAndNextWhitespace(this XElement element, object xpathEvaluation)
        {
            var nodes = (xpathEvaluation as IEnumerable)?.Cast<XNode>();
            if (nodes is null)
            {
                element.ReplaceWith(xpathEvaluation);
            }
            else
            {
                if (nodes.Any())
                {
                    if(nodes.First() is XText firstNodeText)
                        firstNodeText.Value = firstNodeText.Value.TrimStart();

                    if (nodes.Last() is XText lastNodeText)
                        lastNodeText.Value = lastNodeText.Value.TrimEnd();

                    var noneEmptyNodes = nodes.Where(node =>
                    {
                        return !(node is XText textNode) || string.IsNullOrEmpty(textNode.Value);
                    });

                    if (noneEmptyNodes.Any())
                        element.ReplaceWith(noneEmptyNodes);
                }
                else
                {
                    element.RemoveElementWithNextWhitespace();
                }
            }
        }
    }
}
