using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;

namespace XmlDocExtractionLib
{
    /// <summary>
    /// Extension methods for extracting xml documentation
    /// from members.
    /// </summary>
    public static class XmlDocsExtractionExtensions
    {
        /// <summary>
        /// A set of first level xml document elements that have no
        /// attribute that identifies them
        /// e.g. summary and remarks elements.
        /// </summary>
        private static HashSet<string> elementsWithNoIdentifier;

        /// <summary>
        /// A mapping between first level xml document element to
        /// its corresponding identifier attribute name.
        /// e.g. for param element the name of the identifier attribute
        /// is "name".
        /// </summary>
        private static Dictionary<string, string> elementToIdentifierName;

        /// <summary>
        /// Initialize all the static members of the <see cref="XmlDocsExtractionExtensions"/>
        /// class.
        /// </summary>
        static XmlDocsExtractionExtensions()
        {
            elementsWithNoIdentifier = new HashSet<string>
            {
                "summary", "remarks", "returns", "value",
                "example",
            };
            elementToIdentifierName = new Dictionary<string, string>
            {
                ["param"] = "name",
                ["exception"] = "cref",
                ["typeparam"] = "name",
            };
        }

        /// <summary>
        /// Retrieves the xml documentation of the given <paramref name="memberInfo"/>
        /// base on the given <paramref name="xmlExtractionContext"/>.
        /// </summary>
        /// <param name="memberInfo">The member whose xml documentation is requested.</param>
        /// <param name="xmlExtractionContext">The context in which to search for the xml documentation.</param>
        /// <param name="resolveInheritdoc">Whether or not to resolve inheritdoc elements.</param>
        /// <returns>
        /// The xml documentation of the given <paramref name="memberInfo"/> if found
        /// within the given <paramref name="xmlExtractionContext"/>,
        /// otherwise <see langword="null"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown when any of the given parameters are <see langword="null"/>
        /// </exception>
        public static XElement? GetXmlDocumentation(this MemberInfo memberInfo, IXmlExtractionContext xmlExtractionContext, bool resolveInheritdoc = false)
        {
            if (memberInfo == null)
                throw new ArgumentNullException($"Given {nameof(memberInfo)} is null.");

            if (xmlExtractionContext == null)
                throw new ArgumentNullException($"Given {nameof(xmlExtractionContext)} is null.");

            if (xmlExtractionContext.TryGetMemberXmlDocsFromMember(memberInfo, out var xmlDoc))
            {
                if (resolveInheritdoc)
                    xmlDoc = InheritdocResolver.ResolveInheritDocumentation(xmlDoc, xmlExtractionContext, memberInfo);

                // Clean xml doc from multiple elements
                // e.g. summary and then another summary.
                RemoveDuplicateElements(xmlDoc);
                return xmlDoc;
            }

            return null;
        }

        /// <summary>
        /// Retrieves the xml documentation of the enum value with the given
        /// <paramref name="enumType"/> and <paramref name="enumValue"/>
        /// base on the given <paramref name="xmlExtractionContext"/>.
        /// </summary>
        /// <param name="enumType">The type of the enum whose value's xml documentation is requested.</param>
        /// <param name="enumValue">The value of the enum whose xml documentation is requested.</param>
        /// <param name="xmlExtractionContext">The context in which to search for the xml documentation.</param>
        /// <param name="resolveInheritdoc">Whether or not to resolve inheritdoc elements.</param>
        /// <returns>
        /// The xml documentation of the enum value with the given
        /// <paramref name="enumType"/> and <paramref name="enumValue"/> if found
        /// within the given <paramref name="xmlExtractionContext"/>,
        /// otherwise <see langword="null"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown when any of the given parameters are <see langword="null"/>
        /// </exception>
        public static XElement? GetEnumValueXmlDocumentation(this Type enumType, int enumValue, IXmlExtractionContext xmlExtractionContext, bool resolveInheritdoc = false)
        {
            if (enumType == null)
                throw new ArgumentNullException($"Given {nameof(enumType)} is null.");

            if (xmlExtractionContext == null)
                throw new ArgumentNullException($"Given {nameof(xmlExtractionContext)} is null.");

            var enumValueIdentifier = enumType.GetEnumValueNameIdentifier(enumValue);
            if (xmlExtractionContext.TryGetMemberXmlDocsFromMemberIdentifier(enumValueIdentifier, out var xmlDoc))
            {
                if (resolveInheritdoc)
                    xmlDoc = InheritdocResolver.ResolveInheritDocumentation(xmlDoc, xmlExtractionContext);

                // Clean xml doc from multiple elements
                // e.g. summary and then another summary.
                RemoveDuplicateElements(xmlDoc);
                return xmlDoc;
            }

            return null;
        }

        /// <summary>
        /// Remove all duplicated of same kind from the first level elements.
        /// e.g. take only the first summary element.
        /// </summary>
        /// <param name="memberXml">The xml doc to remove duplicated from.</param>
        private static void RemoveDuplicateElements(XElement memberXml)
        {
            var seenSingleElements = new HashSet<string>();
            var seenMultipleElements = new Dictionary<string, HashSet<string>>();

            foreach (var element in memberXml.Elements().ToArray())
            {
                var elementName = element.Name.LocalName;
                if (elementsWithNoIdentifier.Contains(elementName))
                {
                    if (seenSingleElements.Contains(elementName))
                        element.Remove();
                    else
                        seenSingleElements.Add(elementName);
                }

                else if (elementToIdentifierName.ContainsKey(elementName))
                {
                    var attribute = element.Attribute(elementToIdentifierName[elementName]).Value;

                    if(seenMultipleElements.TryGetValue(elementName, out var attributesValues))
                    {
                        if (attributesValues.Contains(attribute))
                            element.Remove();
                        else
                            attributesValues.Add(attribute);
                    }
                    else
                    {
                        seenMultipleElements.Add(elementName, new HashSet<string> { attribute });
                    }
                }
            }
        }
    }
}
