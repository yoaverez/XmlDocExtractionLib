using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using System.Xml.XPath;

namespace XmlDocExtractionLib
{
    /// <summary>
    /// Resolver for inherit doc elements of xml documentation.
    /// </summary>
    public static class InheritdocResolver
    {
        /// <summary>
        /// The name of the xml element of inheritdoc element.
        /// </summary>
        private const string INHERITDOC = "inheritdoc";

        /// <summary>
        /// The name of the attribute that references another member
        /// inside an inheritdoc element.
        /// </summary>
        private const string CREF = "cref";

        /// <summary>
        /// The name of the attribute that tells which xml elements from
        /// the inherit member to use.
        /// </summary>
        private const string PATH = "path";

        /// <summary>
        /// The name of the attribute that contains the identifier
        /// of members.
        /// </summary>
        private const string NAME = "name";

        /// <summary>
        /// The name of the xml element that contains all the documentation
        /// of a single member.
        /// </summary>
        private const string MEMBER = "member";

        /// <summary>
        /// Resolve the given <paramref name="originalDoc"/> inheritdoc
        /// elements.
        /// </summary>
        /// <param name="originalDoc">The xml documentation that whose inheritdoc resolve is requested.</param>
        /// <param name="xmlExtractionContext">The context in which to search inherit documentation.</param>
        /// <param name="memberInfo">The member which corresponds to the given <paramref name="originalDoc"/>. Default to <see langword="null"/>.</param>
        /// <remarks>
        /// All the inheritdocs that couldn't be resolve are removed.
        /// Inheritdocs may not be resolved if the given <paramref name="xmlExtractionContext"/>
        /// is not sufficient enough.
        /// </remarks>
        /// <returns>
        /// A copy of the given <paramref name="originalDoc"/> in which all the inheritdoc elements
        /// are replaced by the real inherited documentation.
        /// </returns>
        public static XElement ResolveInheritDocumentation(XElement originalDoc, IXmlExtractionContext xmlExtractionContext, MemberInfo? memberInfo = null)
        {
            var originalCopy = new XElement(originalDoc);
            foreach (var inheritdocNode in originalCopy.Descendants(INHERITDOC).ToArray())
            {
                var cref = inheritdocNode.Attribute(CREF);
                XElement resolveInheritDocumentation = null;
                if (cref != null)
                {
                    resolveInheritDocumentation = ResolveSinleInheritdocWithCref(cref.Value, xmlExtractionContext);
                }
                else
                {
                    var memberIdentifier = originalCopy.Attribute(NAME).Value!;
                    if (memberInfo != null || xmlExtractionContext.TryGetMemberFromMemberIdentifier(memberIdentifier, out memberInfo))
                    {
                        resolveInheritDocumentation = ResolveSinleInheritdocWithoutCref(memberInfo, xmlExtractionContext);
                    }
                }

                if (resolveInheritDocumentation != null)
                {
                    var pathAttribute = inheritdocNode.Attribute(PATH);
                    var pathValue = pathAttribute?.Value;

                    if (pathValue == null && inheritdocNode.Ancestors().Count() > 2)
                    {
                        // Visual studio resolve inheritdoc without a path attribute
                        // Only if the inheritdoc is in the first level of documentation
                        // (i.e. under member element) or in the second level of documentation
                        // (i.e. directly inside summary, remarks, returns, param etc.).
                        // Therefore, if we got here, we are not in the first two levels
                        // of the xml member documentation and we do not have the path attribute
                        // so we just remove this inheritdoc.
                        inheritdocNode.Remove();
                    }
                    else
                    {
                        pathValue ??= inheritdocNode.GetXPathFromMember();

                        var xpathEvaluation = resolveInheritDocumentation.XPathEvaluate(pathValue);
                        inheritdocNode.ReplaceWith(xpathEvaluation);
                    }
                }
                else
                {
                    // Could not resolve inheritdoc so just
                    // remove it.
                    inheritdocNode.Remove();
                }
            }

            return originalCopy;
        }

        /// <summary>
        /// Handles a single inheritdoc element resolve
        /// in an element that has the cref attribute.
        /// </summary>
        /// <param name="crefMemberIdentifier">The identifier of the member from which to inherit the documentation.</param>
        /// <param name="xmlExtractionContext">The context in which to search inherit documentation.</param>
        /// <returns>
        /// The resolved xml documentation of the member to inherit from if the documentation
        /// was found otherwise <see langword="null"/>.
        /// </returns>
        private static XElement? ResolveSinleInheritdocWithCref(string crefMemberIdentifier, IXmlExtractionContext xmlExtractionContext)
        {
            XElement crefMemberXml = null;
            if (xmlExtractionContext.TryGetMemberXmlDocsFromMemberIdentifier(crefMemberIdentifier, out var unresolvedCRefMemberXml))
            {
                crefMemberXml = ResolveInheritDocumentation(unresolvedCRefMemberXml, xmlExtractionContext);
            }

            return crefMemberXml;
        }

        /// <summary>
        /// Handles a single inheritdoc element resolve
        /// in an element that does not have the cref attribute.
        /// </summary>
        /// <param name="memberInfo">The member whose inherit documentation is requested.</param>
        /// <param name="xmlExtractionContext">The context in which to search inherit documentation.</param>
        /// <returns>
        /// The resolved xml documentation of the member to inherit from if the documentation
        /// was found otherwise <see langword="null"/>.
        /// </returns>
        private static XElement? ResolveSinleInheritdocWithoutCref(MemberInfo memberInfo, IXmlExtractionContext xmlExtractionContext)
        {
            return memberInfo.MemberType switch
            {
                MemberTypes.TypeInfo or MemberTypes.NestedType => GetTypeInheritDocumentation(memberInfo as Type, xmlExtractionContext),
                MemberTypes.Property => GetPropertyEventMethodOrCtorInheritDocumentation(memberInfo as PropertyInfo, xmlExtractionContext, (member) => member.GetPropertyBaseDefinitions(), (member) => member.GetMemberIdentifier()),
                MemberTypes.Event => GetPropertyEventMethodOrCtorInheritDocumentation(memberInfo as EventInfo, xmlExtractionContext, (member) => member.GetEventBaseDefinitions(), (member) => member.GetMemberIdentifier()),
                MemberTypes.Constructor => GetPropertyEventMethodOrCtorInheritDocumentation(memberInfo as ConstructorInfo, xmlExtractionContext, (member) => member.GetConstructorBaseDefinitions(), (member) => member.GetMemberIdentifier()),
                MemberTypes.Method => GetPropertyEventMethodOrCtorInheritDocumentation(memberInfo as MethodInfo, xmlExtractionContext, (member) => member.GetMethodBaseDefinitions(), (member) => member.GetMemberIdentifier()),

                // Other member types can not inherit documentation
                // without specifying from which member they inherit the
                // documentation.
                _ => null,
            };
        }

        /// <summary>
        /// Get type inherit documentation from base definitions.
        /// </summary>
        /// <param name="type">The type whose inherit documentation is requested.</param>
        /// <param name="xmlExtractionContext">The context in which to search inherit documentation.</param>
        /// <returns>
        /// The documentation of the type base definition if
        /// base definition documentation was found otherwise <see langword="null"/>.
        /// </returns>
        private static XElement? GetTypeInheritDocumentation(Type type, IXmlExtractionContext xmlExtractionContext)
        {
            var baseDefinition = type.GetTypeBaseDefinitions().FirstOrDefault();
            if (baseDefinition is not null)
            {
                if (xmlExtractionContext.TryGetMemberXmlDocsFromMember(baseDefinition, out var unresolvedInheritedXml))
                    return ResolveInheritDocumentation(unresolvedInheritedXml, xmlExtractionContext, baseDefinition);
            }
            return null;
        }

        /// <summary>
        /// Get member that has base definitions (property, event, method, ctor)
        /// inherit documentation.
        /// </summary>
        /// <typeparam name="T">The type of the member whose inherit documentation is requested.</typeparam>
        /// <param name="member">The member whose inherit documentation is requested.</param>
        /// <param name="xmlExtractionContext">The context in which to search inherit documentation.</param>
        /// <param name="baseDefinitionsGetter">A method that gets all the base definitions of the <paramref name="member"/>.</param>
        /// <param name="identifierGetter">A method that gets a member identifier from the member itself.</param>
        /// <returns>
        /// The documentation of the <paramref name="member"/> base definition if
        /// base definition documentation was found otherwise <see langword="null"/>.
        /// </returns>
        private static XElement GetPropertyEventMethodOrCtorInheritDocumentation<T>(T member, IXmlExtractionContext xmlExtractionContext, Func<T, IEnumerable<T>> baseDefinitionsGetter, Func<T, string> identifierGetter) where T : MemberInfo
        {
            XElement unresolvedInheritedXml = null;
            var baseDefinition = baseDefinitionsGetter(member).FirstOrDefault((currentMember) =>
            {
                return xmlExtractionContext.TryGetMemberXmlDocsFromMemberIdentifier(identifierGetter(currentMember), out unresolvedInheritedXml);
            });
            if (baseDefinition != null)
            {
                return ResolveInheritDocumentation(unresolvedInheritedXml, xmlExtractionContext, baseDefinition);
            }

            return null;
        }

        /// <summary>
        /// Get the xpath query that gets the given <paramref name="element"/>
        /// from the member xml document that contains the <paramref name="element"/>.
        /// </summary>
        /// <param name="element">The element whose xpath query is requested.</param>
        /// <returns>
        /// The xpath query that gets the given <paramref name="element"/>
        /// from the member xml document that contains the <paramref name="element"/>.
        /// </returns>
        /// <remarks>
        /// Since this resolver is meant to resolve inherit doc like visual studio
        /// resolves inherit doc, there are the following remarks:
        /// <list type="number">
        /// <item>
        /// The given <paramref name="element"/> should in the first or second level of documentation
        /// e.g. right under the member element or right under the summary, remarks, param, returns
        /// etc. elements.
        /// </item>
        /// <item>
        /// This method will not return an xpath that is specific to the given <paramref name="element"/>,
        /// it will return an xpath that will lead to all the sibling elements that have the same element name
        /// as the given <paramref name="element"/>.
        /// e.g. if the element is a first param, then the resulted xpath of this method
        /// will be "/param/node()" and not "param[0]/node()".
        /// </item>
        /// </list>
        /// </remarks>
        private static string GetXPathFromMember(this XElement element)
        {
            var xpathComponents = new List<string>();
            foreach (var ancestor in element.Ancestors().Reverse())
            {
                string currentComponent;
                string name = ancestor.Name.LocalName;
                if (name.Equals(MEMBER))
                    currentComponent = string.Empty;
                else
                {
                    currentComponent = "/" + name;
                }
                xpathComponents.Add(currentComponent);
            }
            return string.Concat(xpathComponents) + "/node()";
        }
    }
}
