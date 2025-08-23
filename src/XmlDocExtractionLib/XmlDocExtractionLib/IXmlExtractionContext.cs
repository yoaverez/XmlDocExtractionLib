using System.Reflection;
using System.Xml.Linq;

namespace XmlDocExtractionLib
{
    /// <summary>
    /// A context for xml documentation extraction.
    /// Manages all the loaded assemblies and their corresponding
    /// xml documentation files.
    /// </summary>
    public interface IXmlExtractionContext
    {
        /// <summary>
        /// Try get the xml of the member with the given <paramref name="memberIdentifier"/>.
        /// </summary>
        /// <param name="memberIdentifier">The identifier of the member whose xml node is requested.</param>
        /// <param name="memberXml">The xml if found otherwise the default of <see cref="XElement"/>.</param>
        /// <returns>
        /// <see langword="true"/> an xml node corresponding to the given
        /// <paramref name="memberIdentifier"/> was found
        /// otherwise <see langword="false"/>.
        /// </returns>
        bool TryGetMemberXmlDocsFromMemberIdentifier(string memberIdentifier, out XElement memberXml);

        /// <summary>
        /// Try get the xml of the given <paramref name="memberInfo"/>.
        /// </summary>
        /// <param name="memberInfo">The member whose xml node is requested.</param>
        /// <param name="memberXml">The xml if found otherwise the default of <see cref="XElement"/>.</param>
        /// <returns>
        /// <see langword="true"/> an xml node corresponding to the given
        /// <paramref name="memberInfo"/> was found
        /// otherwise <see langword="false"/>.
        /// </returns>
        bool TryGetMemberXmlDocsFromMember(MemberInfo memberInfo, out XElement memberXml);

        /// <summary>
        /// Try get the <see cref="MemberInfo"/> that is corresponding to the
        /// given <paramref name="memberIdentifier"/>.
        /// </summary>
        /// <param name="memberIdentifier">The identifier of the requested member.</param>
        /// <param name="memberInfo">The member if found otherwise the default of <see cref="MemberInfo"/>.</param>
        /// <returns>
        /// <see langword="true"/> a member corresponding to the given
        /// <paramref name="memberIdentifier"/> was found
        /// otherwise <see langword="false"/>.
        /// </returns>
        bool TryGetMemberFromMemberIdentifier(string memberIdentifier, out MemberInfo memberInfo);
    }
}
