using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Reflection;
using System.Xml.Linq;
using XmlDocExtractionLib.Tests.DummyTypes;

namespace XmlDocExtractionLib.Tests
{
    [TestClass]
    public class XmlDocsExtractionExtensionsTests
    {
        private static Dictionary<string, XElement> xmlMembers;

        private static XmlExtractionContext xmlExtractionContext;

        [ClassInitialize]
        public static void ClassInitialize(TestContext testContext)
        {
            var testAssembly = typeof(XmlExtractionContextTests).Assembly;
            var testAssemblyXmlPath = $"{testAssembly.GetName().Name!}.xml";
            var libAssembly = typeof(XmlExtractionContext).Assembly;
            var libAssemblyXmlPath = $"{libAssembly.GetName().Name!}.xml";

            xmlExtractionContext = new XmlExtractionContext();
            xmlExtractionContext.AddAssembly(testAssembly, testAssemblyXmlPath);
            xmlExtractionContext.AddAssembly(libAssembly, libAssemblyXmlPath);

            var testXmlDocumentation = XDocument.Load(testAssemblyXmlPath, LoadOptions.PreserveWhitespace);
            var testXmlMembers = testXmlDocumentation.Descendants("members")
                                                     .Single()
                                                     .Elements("member");

            var libXmlDocumentation = XDocument.Load(libAssemblyXmlPath, LoadOptions.PreserveWhitespace);
            var libXmlMembers = libXmlDocumentation.Descendants("members")
                                                   .Single()
                                                   .Elements("member");

            xmlMembers = testXmlMembers.Concat(libXmlMembers).ToDictionary(x => x.Attribute("name")!.Value, x => x);
        }

        #region GetXmlDocumentation on Members Tests

        [TestMethod]
        public void GetXmlDocumentationOnMembers_GivenMemberIsNull_ArgumentNullExceptionIsThrown()
        {
            // Arrange
            MemberInfo testedMember = null;

            // Act + Assert
            Assert.ThrowsException<ArgumentNullException>(() => testedMember.GetXmlDocumentation(xmlExtractionContext));
        }

        [TestMethod]
        public void GetXmlDocumentationOnMembers_GivenContextIsNull_ArgumentNullExceptionIsThrown()
        {
            // Arrange
            MemberInfo testedMember = typeof(SubDummyClass).GetMembers().First();

            // Act + Assert
            Assert.ThrowsException<ArgumentNullException>(() => testedMember.GetXmlDocumentation(null));
        }

        [DataRow(false)]
        [DataRow(true)]
        [DataTestMethod]
        public void GetXmlDocumentationOnMembers_GivenMethodHasDuplicateDocs_ReturnDocsWithoutDuplication(bool resolveInheritdoc)
        {
            // Arrange
            MemberInfo testedMember = typeof(SubDummyClass).GetMember(nameof(SubDummyClass.MethodWithDuplicatesDocs)).Single();

            MemberInfo expectedMember = typeof(SubDummyClass).GetMember(nameof(SubDummyClass.MethodWithoutDuplicatesDocs)).Single();
            var expectedDocs = new XElement(xmlMembers[expectedMember.GetMemberIdentifier()]);
            expectedDocs.Attribute("name")!.Value = testedMember.GetMemberIdentifier();

            // This is needed since the removal of trailing white spaces
            // comes after element and not before element.
            var expectedDocsString = expectedDocs.ToString().Replace("</member>", "    </member>");

            // Act
            var actualDocs = testedMember.GetXmlDocumentation(xmlExtractionContext, resolveInheritdoc);

            // Assert
            Assert.AreEqual(expectedDocsString, actualDocs.ToString());
        }

        [DataRow(false)]
        [DataRow(true)]
        [DataTestMethod]
        public void GetXmlDocumentationOnMembers_GivenPropertyHasDuplicateDocs_ReturnDocsWithoutDuplication(bool resolveInheritdoc)
        {
            // Arrange
            MemberInfo testedMember = typeof(SubDummyClass).GetMember(nameof(SubDummyClass.PropWithDuplicatesDocs)).Single();

            MemberInfo expectedMember = typeof(SubDummyClass).GetMember(nameof(SubDummyClass.PropWithoutDuplicatesDocs)).Single();
            var expectedDocs = new XElement(xmlMembers[expectedMember.GetMemberIdentifier()]);
            expectedDocs.Attribute("name")!.Value = testedMember.GetMemberIdentifier();

            // This is needed since the removal of trailing white spaces
            // comes after element and not before element.
            var expectedDocsString = expectedDocs.ToString().Replace("</member>", "    </member>");

            // Act
            var actualDocs = testedMember.GetXmlDocumentation(xmlExtractionContext, resolveInheritdoc);

            // Assert
            Assert.AreEqual(expectedDocsString, actualDocs.ToString());
        }

        [DataRow(false)]
        [DataRow(true)]
        [DataTestMethod]
        public void GetXmlDocumentationOnMembers_MemberDoesNotHaveDocs_ReturnsNull(bool resolveInheritdoc)
        {
            // Arrange
            MemberInfo testedMember = typeof(SubDummyClass).GetMember(nameof(SubDummyClass.PropWithoutDocs)).Single();

            // Act
            var actualDocs = testedMember.GetXmlDocumentation(xmlExtractionContext, resolveInheritdoc);

            // Assert
            Assert.IsNull(actualDocs);
        }

        [DataRow(false)]
        [DataRow(true)]
        [DataTestMethod]
        public void GetXmlDocumentationOnMembers_MemberHasElementSpaceElementInDocs_WhitespaceIsPreserved(bool resolveInheritdoc)
        {
            // Arrange
            MemberInfo testedMember = typeof(SubDummyClass);

            var expectedString = $"<summary>{Environment.NewLine}" +
                                 $"            docs with <see langword=\"false\"/> <see langword=\"true\"/>.{Environment.NewLine}" +
                                 $"            </summary>";
            var expectedDocs = XElement.Parse(expectedString, LoadOptions.PreserveWhitespace);

            // Act
            var actualDocs = testedMember.GetXmlDocumentation(xmlExtractionContext, resolveInheritdoc).Element("summary");

            // Assert
            Assert.AreEqual(expectedDocs.ToString(), actualDocs.ToString());
        }

        #endregion GetXmlDocumentation on Members Tests

        #region GetXmlDocumentation on EnumValues Tests

        [TestMethod]
        public void GetXmlDocumentationOnEnumValues_GivenMemberIsNull_ArgumentNullExceptionIsThrown()
        {
            // Arrange
            Type testedEnumType = null;

            // Act + Assert
            Assert.ThrowsException<ArgumentNullException>(() => testedEnumType.GetEnumValueXmlDocumentation((int)DummyEnum2.ValueWithDuplicateDocs, xmlExtractionContext));
        }

        [TestMethod]
        public void GetXmlDocumentationOnEnumValues_GivenContextIsNull_ArgumentNullExceptionIsThrown()
        {
            // Arrange
            var testedEnumType = typeof(DummyEnum2);

            // Act + Assert
            Assert.ThrowsException<ArgumentNullException>(() => testedEnumType.GetEnumValueXmlDocumentation((int)DummyEnum2.ValueWithDuplicateDocs, null));
        }

        [DataRow(false)]
        [DataRow(true)]
        [DataTestMethod]
        public void GetXmlDocumentationOnEnumValues_GivenEnumValueHasDuplicateDocs_ReturnDocsWithoutDuplication(bool resolveInheritdoc)
        {
            // Arrange
            var testedEnumType = typeof(DummyEnum2);

            var valueWithExpectedDocs = (int)DummyEnum2.ValueWithoutDuplicateDocs;
            var expectedDocs = new XElement(xmlMembers[testedEnumType.GetEnumValueNameIdentifier(valueWithExpectedDocs)]);
            expectedDocs.Attribute("name")!.Value = testedEnumType.GetEnumValueNameIdentifier((int)DummyEnum2.ValueWithDuplicateDocs);

            // This is needed since the removal of trailing white spaces
            // comes after element and not before element.
            var expectedDocsString = expectedDocs.ToString().Replace("</member>", "    </member>");

            // Act
            var actualDocs = testedEnumType.GetEnumValueXmlDocumentation((int)DummyEnum2.ValueWithDuplicateDocs, xmlExtractionContext, resolveInheritdoc);

            // Assert
            Assert.AreEqual(expectedDocsString, actualDocs.ToString());
        }

        [DataRow(false)]
        [DataRow(true)]
        [DataTestMethod]
        public void GetXmlDocumentationOnEnumValues_EnumValueDoesNotHaveDocs_ReturnsNull(bool resolveInheritdoc)
        {
            // Arrange
            var testedEnumType = typeof(DummyEnum2);

            // Act
            var actualDocs = testedEnumType.GetEnumValueXmlDocumentation((int)DummyEnum2.ValueWithoutDocs, xmlExtractionContext, resolveInheritdoc);

            // Assert
            Assert.IsNull(actualDocs);
        }

        #endregion GetXmlDocumentation on EnumValues Tests
    }
}
