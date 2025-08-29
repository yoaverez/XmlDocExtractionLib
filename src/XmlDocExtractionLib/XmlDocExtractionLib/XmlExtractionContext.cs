using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace XmlDocExtractionLib
{
    /// <inheritdoc cref="IXmlExtractionContext"/>
    public class XmlExtractionContext : IXmlExtractionContext
    {
        /// <summary>
        /// A mapping between member identifier to its corresponding
        /// <see cref="MemberInfo"/>.
        /// </summary>
        private ConcurrentDictionary<string, MemberInfo> memberIdentifierToMemberInfo;

        /// <summary>
        /// action to perform lazily for updating the
        /// <see cref="memberIdentifierToMemberInfo"/> since
        /// this is a heavy operation.
        /// </summary>
        private List<Action> lazyAssemblyLoader;

        /// <summary>
        /// A set containing all the loaded assemblies.
        /// </summary>
        private HashSet<string> loadedAssemblies;

        /// <summary>
        /// A mapping between member identifier to its xml documentation.
        /// </summary>
        private ConcurrentDictionary<string, XElement> memberIdentifierToMemberXml;

        /// <summary>
        /// Create new instance of the <see cref="XmlExtractionContext"/> class.
        /// </summary>
        public XmlExtractionContext()
        {
            memberIdentifierToMemberInfo = new ConcurrentDictionary<string, MemberInfo>();
            lazyAssemblyLoader = new List<Action>();
            loadedAssemblies = new HashSet<string>();
            memberIdentifierToMemberXml = new ConcurrentDictionary<string, XElement>();
        }

        /// <summary>
        /// Add the given <paramref name="assembly"/> and xml documentation
        /// to this xml extraction context.
        /// </summary>
        /// <param name="assembly">The assembly from which the xml documentation was created.</param>
        /// <param name="pathToXmlDocumentationFile">The path to the given <paramref name="assembly"/> xml documentation file.</param>
        public void AddAssembly(Assembly assembly, string pathToXmlDocumentationFile)
        {
            var newAssemblyName = assembly.GetName().Name;

            // Add the assembly only if it is not already loaded.
            if (!loadedAssemblies.Contains(newAssemblyName))
            {
                loadedAssemblies.Add(newAssemblyName);
                LoadXml(pathToXmlDocumentationFile);
                lazyAssemblyLoader.Add(() => LoadAllMembers(assembly));
            }
        }

        #region IXmlExtractionContext Implementation

        /// <inheritdoc/>
        public bool TryGetMemberXmlDocsFromMemberIdentifier(string memberIdentifier, out XElement memberXml)
        {
            memberXml = null;
            var result = memberIdentifierToMemberXml.TryGetValue(memberIdentifier, out var originMemberXml);
            if(result)
                // Return a copy of the original xml.
                memberXml = new XElement(originMemberXml);

            return result;
        }

        /// <inheritdoc/>
        public bool TryGetMemberXmlDocsFromMember(MemberInfo memberInfo, out XElement memberXml)
        {
            var memberIdentifier = memberInfo.GetMemberIdentifier();
            return TryGetMemberXmlDocsFromMemberIdentifier(memberIdentifier, out memberXml);
        }

        /// <inheritdoc/>
        public bool TryGetMemberFromMemberIdentifier(string memberIdentifier, out MemberInfo memberInfo)
        {
            while (!memberIdentifierToMemberInfo.TryGetValue(memberIdentifier, out memberInfo) && lazyAssemblyLoader.Count > 0)
            {
                var action = lazyAssemblyLoader[0];
                lazyAssemblyLoader.RemoveAt(0);
                action();
            }

            if (memberInfo is null)
            {
                return false;
            }

            return true;
        }

        #endregion IXmlExtractionContext Implementation

        /// <summary>
        /// Load the xml documentation file from the given
        /// <paramref name="pathToXmlDocumentation"/>.
        /// </summary>
        /// <param name="pathToXmlDocumentation">The path to the xml documentation file to load.</param>
        private void LoadXml(string pathToXmlDocumentation)
        {
            var xmlDocumentation = XDocument.Load(pathToXmlDocumentation);
            var xmlMembers = xmlDocumentation.Descendants("members")
                                             .Single()
                                             .Elements("member");
            Parallel.ForEach(xmlMembers, (xmlMember) =>
            {
                memberIdentifierToMemberXml.TryAdd(xmlMember.Attribute("name")!.Value, xmlMember);
            });
        }

        /// <summary>
        /// Load a mapping between member identifier to its
        /// corresponding <see cref="MemberInfo"/> from all the members
        /// defined in the given <paramref name="assembly"/> into
        /// <see cref="memberIdentifierToMemberInfo"/>.
        /// </summary>
        /// <param name="assembly">
        /// The assembly whose defined members
        /// this method load.
        /// </param>
        private void LoadAllMembers(Assembly assembly)
        {
            Parallel.ForEach(assembly.DefinedTypes, (typeInfo) =>
            {
                // There is no reason that the add will fail because there
                // are not supposed to be members with same identifier.
                memberIdentifierToMemberInfo.TryAdd(typeInfo.GetMemberIdentifier(), typeInfo);

                Parallel.ForEach(typeInfo.DeclaredMembers, (member) =>
                {
                    var memberIdentifier = member.GetMemberIdentifier();

                    // There is no reason that the add will fail because there
                    // are not supposed to be members with same identifier.
                    memberIdentifierToMemberInfo.TryAdd(memberIdentifier, member);
                });
            });
        }
    }
}
