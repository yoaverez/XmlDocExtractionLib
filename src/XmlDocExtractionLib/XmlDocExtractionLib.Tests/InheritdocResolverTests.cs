using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Reflection;
using System.Xml.Linq;
using System.Xml.XPath;
using XmlDocExtractionLib.Tests.DummyTypes.TypesWithInheritdocs;

namespace XmlDocExtractionLib.Tests
{
    [TestClass]
    public class InheritdocResolverTests
    {
        private static XmlExtractionContext xmlExtractionContext;

        private static BindingFlags bindingFlags;

        [ClassInitialize]
        public static void ClassInitialize(TestContext testContext)
        {
            bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static;
            var testAssembly = typeof(InheritdocResolverTests).Assembly;
            var libAssembly = typeof(InheritdocResolver).Assembly;

            xmlExtractionContext = new XmlExtractionContext();
            xmlExtractionContext.AddAssembly(testAssembly, $"{testAssembly.GetName().Name}.xml");
            xmlExtractionContext.AddAssembly(libAssembly, $"{libAssembly.GetName().Name}.xml");
        }

        [TestMethod]
        public void ResolveInheritDocumentation_TypeInheritsDocumentationFromBase_XmlResolvedCorrectly()
        {
            // Arrange
            var testedMember = typeof(Inheritdoc1);
            xmlExtractionContext.TryGetMemberXmlDocsFromMember(testedMember, out var memberXml);

            var expectedbaseMember = typeof(InheritDocBase2);
            xmlExtractionContext.TryGetMemberXmlDocsFromMember(expectedbaseMember, out var baseMemberXml);

            var copyOfBaseMember = new XElement(baseMemberXml);
            var expectedResolvedMember = new XElement(memberXml);
            expectedResolvedMember.Descendants("inheritdoc").Single().ReplaceWith(copyOfBaseMember.XPathEvaluate("/node()"));

            // Act
            var actualResolvedMember = InheritdocResolver.ResolveInheritDocumentation(memberXml, xmlExtractionContext, testedMember);

            // Assert
            Assert.AreEqual(expectedResolvedMember.ToString(), actualResolvedMember.ToString());
        }

        [TestMethod]
        public void ResolveInheritDocumentation_MemberInheritsDocumentationFromBases_XmlResolvedCorrectly()
        {
            // Arrange
            var testedMember = typeof(Inheritdoc1).GetMember(nameof(Inheritdoc1.Method1), bindingFlags).Single();
            xmlExtractionContext.TryGetMemberXmlDocsFromMember(testedMember, out var memberXml);

            var expectedbaseMember = typeof(IInheritdoc1).GetMember(nameof(IInheritdoc1.Method1), bindingFlags).Single();
            xmlExtractionContext.TryGetMemberXmlDocsFromMember(expectedbaseMember, out var baseMemberXml);

            var copyOfBaseMember = new XElement(baseMemberXml);
            var expectedResolvedMember = new XElement(memberXml);
            expectedResolvedMember.Descendants("inheritdoc").Single().ReplaceWith(copyOfBaseMember.XPathEvaluate("/node()"));

            // Act
            var actualResolvedMember = InheritdocResolver.ResolveInheritDocumentation(memberXml, xmlExtractionContext, testedMember);

            // Assert
            Assert.AreEqual(expectedResolvedMember.ToString(), actualResolvedMember.ToString());
        }

        [TestMethod]
        public void ResolveInheritDocumentation_MemberInheritsDocumentationFromCrefAndTheCrefMemberInheritFromBase_XmlResolvedCorrectly()
        {
            // Arrange
            var testedMember = typeof(InheritDocBase1).GetMember(nameof(InheritDocBase1.Method2), bindingFlags).Single();
            xmlExtractionContext.TryGetMemberXmlDocsFromMember(testedMember, out var memberXml);

            var expectedMemberFromWhichToInherit = typeof(IInheritdoc1).GetMember(nameof(IInheritdoc1.Method1), bindingFlags).Single();
            xmlExtractionContext.TryGetMemberXmlDocsFromMember(expectedMemberFromWhichToInherit, out var memberFromWhichToInheritXml);

            var copyOfMemberFromWhichToInherit = new XElement(memberFromWhichToInheritXml);
            var expectedResolvedMember = new XElement(memberXml);
            expectedResolvedMember.Descendants("inheritdoc").Single().ReplaceWith(copyOfMemberFromWhichToInherit.XPathEvaluate("/summary/node()"));

            // Act
            var actualResolvedMember = InheritdocResolver.ResolveInheritDocumentation(memberXml, xmlExtractionContext, testedMember);

            // Assert
            Assert.AreEqual(expectedResolvedMember.ToString(), actualResolvedMember.ToString());
        }

        [TestMethod]
        public void ResolveInheritDocumentation_MethodHasInheritdocInsideASingleParam_InheritdocIsReplacedByTheValuesOfAllTheParamsInTheInheritedObject()
        {
            // Arrange
            var testedMember = typeof(InheritDocBase2).GetMember(nameof(InheritDocBase2.MethodWithParameters), bindingFlags).Single();
            xmlExtractionContext.TryGetMemberXmlDocsFromMember(testedMember, out var memberXml);

            var expectedMemberFromWhichToInherit = typeof(InheritDocBase1).GetMember(nameof(InheritDocBase1.MethodWithParameters), bindingFlags).Single();
            xmlExtractionContext.TryGetMemberXmlDocsFromMember(expectedMemberFromWhichToInherit, out var memberFromWhichToInheritXml);

            var copyOfMemberFromWhichToInherit = new XElement(memberFromWhichToInheritXml);
            var expectedResolvedMember = new XElement(memberXml);
            expectedResolvedMember.Descendants("inheritdoc").Single().ReplaceWith(copyOfMemberFromWhichToInherit.XPathEvaluate("/param/node()"));

            // Act
            var actualResolvedMember = InheritdocResolver.ResolveInheritDocumentation(memberXml, xmlExtractionContext, testedMember);

            // Assert
            Assert.AreEqual(expectedResolvedMember.ToString(), actualResolvedMember.ToString());
        }

        [TestMethod]
        public void ResolveInheritDocumentation_MemberHasInheritdocWithCrefInsideInThirdLevelWithoutPathAttribute_InheritdocIsRemoved()
        {
            // Arrange
            var testedMember = typeof(InheritDocBase2).GetMember(nameof(InheritDocBase2.MemberWithCrefLevel3WithoutPath), bindingFlags).Single();
            xmlExtractionContext.TryGetMemberXmlDocsFromMember(testedMember, out var memberXml);

            var expectedResolvedMember = new XElement(memberXml);
            expectedResolvedMember.Descendants("inheritdoc").Single().Remove();

            // Act
            var actualResolvedMember = InheritdocResolver.ResolveInheritDocumentation(memberXml, xmlExtractionContext, testedMember);

            // Assert
            Assert.AreEqual(expectedResolvedMember.ToString(), actualResolvedMember.ToString());
        }

        [TestMethod]
        public void ResolveInheritDocumentation_MemberHasInheritdocWithCrefInsideInThirdLevelWithPathAttribute_InheritdocIsReplaced()
        {
            // Arrange
            var testedMember = typeof(InheritDocBase2).GetEvent(nameof(InheritDocBase2.MemberWithCrefLevel3WithPath), bindingFlags);
            xmlExtractionContext.TryGetMemberXmlDocsFromMember(testedMember, out var memberXml);

            var expectedMemberFromWhichToInherit = typeof(InheritDocBase2).GetMember("PrivateProp", bindingFlags).Single();
            xmlExtractionContext.TryGetMemberXmlDocsFromMember(expectedMemberFromWhichToInherit, out var memberFromWhichToInheritXml);

            var copyOfMemberFromWhichToInherit = new XElement(memberFromWhichToInheritXml);
            var expectedResolvedMember = new XElement(memberXml);
            expectedResolvedMember.Descendants("inheritdoc").Single().ReplaceWith(copyOfMemberFromWhichToInherit.XPathEvaluate("/summary/node()"));

            // Act
            var actualResolvedMember = InheritdocResolver.ResolveInheritDocumentation(memberXml, xmlExtractionContext, testedMember);

            // Assert
            Assert.AreEqual(expectedResolvedMember.ToString(), actualResolvedMember.ToString());
        }

        [TestMethod]
        public void ResolveInheritDocumentation_MemberHasSummaryAndThenInheritdoc_TwoSummariesFirstRegularAndThenInheritdoc()
        {
            // Arrange
            var testedMember = typeof(Inheritdoc1).GetMethod(nameof(Inheritdoc1.MethodWithSummaryAndThenInheritdoc), bindingFlags);
            xmlExtractionContext.TryGetMemberXmlDocsFromMember(testedMember, out var memberXml);

            var expectedMemberFromWhichToInherit = typeof(IInheritdoc1).GetMember(nameof(IInheritdoc1.Method1), bindingFlags).Single();
            xmlExtractionContext.TryGetMemberXmlDocsFromMember(expectedMemberFromWhichToInherit, out var memberFromWhichToInheritXml);

            var copyOfMemberFromWhichToInherit = new XElement(memberFromWhichToInheritXml);
            var expectedResolvedMember = new XElement(memberXml);
            expectedResolvedMember.Descendants("inheritdoc").Single().ReplaceWith(copyOfMemberFromWhichToInherit.XPathEvaluate("/node()"));

            // Act
            var actualResolvedMember = InheritdocResolver.ResolveInheritDocumentation(memberXml, xmlExtractionContext, testedMember);

            // Assert
            Assert.AreEqual(expectedResolvedMember.ToString(), actualResolvedMember.ToString());
        }

        [TestMethod]
        public void ResolveInheritDocumentation_MemberHasInheritdocAndThenSummary_TwoSummariesFirstInheritdocAndThenRegularSummary()
        {
            // Arrange
            var testedMember = typeof(Inheritdoc1).GetMethod(nameof(Inheritdoc1.MethodWithInheritdocAndThenSummary), bindingFlags);
            xmlExtractionContext.TryGetMemberXmlDocsFromMember(testedMember, out var memberXml);

            var expectedMemberFromWhichToInherit = typeof(IInheritdoc1).GetMember(nameof(IInheritdoc1.Method1), bindingFlags).Single();
            xmlExtractionContext.TryGetMemberXmlDocsFromMember(expectedMemberFromWhichToInherit, out var memberFromWhichToInheritXml);

            var copyOfMemberFromWhichToInherit = new XElement(memberFromWhichToInheritXml);
            var expectedResolvedMember = new XElement(memberXml);
            expectedResolvedMember.Descendants("inheritdoc").Single().ReplaceWith(copyOfMemberFromWhichToInherit.XPathEvaluate("/node()"));

            // Act
            var actualResolvedMember = InheritdocResolver.ResolveInheritDocumentation(memberXml, xmlExtractionContext, testedMember);

            // Assert
            Assert.AreEqual(expectedResolvedMember.ToString(), actualResolvedMember.ToString());
        }

        [TestMethod]
        public void ResolveInheritDocumentation_MemberHasInheritdocButBaseDefinitionHasNoDocumentation_ReturnEmptyDocumentation()
        {
            // Arrange
            var testedMember = typeof(Inheritdoc1).GetProperty(nameof(Inheritdoc1.PropWithoutBaseDefinition), bindingFlags);
            xmlExtractionContext.TryGetMemberXmlDocsFromMember(testedMember, out var memberXml);

            var expectedResolvedMember = new XElement(memberXml);
            expectedResolvedMember.Descendants("inheritdoc").Single().Remove();

            // Act
            var actualResolvedMember = InheritdocResolver.ResolveInheritDocumentation(memberXml, xmlExtractionContext, testedMember);

            // Assert
            Assert.AreEqual(expectedResolvedMember.ToString(), actualResolvedMember.ToString());
        }

        [TestMethod]
        public void ResolveInheritDocumentation_TypeHasInheritdocButBaseDefinitionHasNoDocumentation_ReturnEmptyDocumentation()
        {
            // Arrange
            var testedMember = typeof(Inheritdoc2);
            xmlExtractionContext.TryGetMemberXmlDocsFromMember(testedMember, out var memberXml);

            var expectedResolvedMember = new XElement(memberXml);
            expectedResolvedMember.Descendants("inheritdoc").Single().Remove();

            // Act
            var actualResolvedMember = InheritdocResolver.ResolveInheritDocumentation(memberXml, xmlExtractionContext, testedMember);

            // Assert
            Assert.AreEqual(expectedResolvedMember.ToString(), actualResolvedMember.ToString());
        }

        [TestMethod]
        public void ResolveInheritDocumentation_FieldHasInheritdocWithoutCRef_ReturnEmptyDocumentation()
        {
            // Arrange
            var testedMember = typeof(Inheritdoc1).GetField("fieldWithoutBaseDefinitions", bindingFlags);
            xmlExtractionContext.TryGetMemberXmlDocsFromMember(testedMember, out var memberXml);

            var expectedResolvedMember = new XElement(memberXml);
            expectedResolvedMember.Descendants("inheritdoc").Single().Remove();

            // Act
            var actualResolvedMember = InheritdocResolver.ResolveInheritDocumentation(memberXml, xmlExtractionContext, testedMember);

            // Assert
            Assert.AreEqual(expectedResolvedMember.ToString(), actualResolvedMember.ToString());
        }

        [TestMethod]
        public void ResolveInheritDocumentation_MethodHasInheritdocWithCRefFromAnotherAssembly_InheritdocIsReplaced()
        {
            // Arrange
            var testedMember = typeof(Inheritdoc1).GetMethod(nameof(InheritDocBase2.MethodWithFarInheritdoc), bindingFlags);
            xmlExtractionContext.TryGetMemberXmlDocsFromMember(testedMember, out var memberXml);

            var expectedbaseMember = typeof(MembersExtensionsUtils).GetMember(nameof(MembersExtensionsUtils.GetEventInfoFromAccessor), bindingFlags).Single();
            xmlExtractionContext.TryGetMemberXmlDocsFromMember(expectedbaseMember, out var baseMemberXml);

            var copyOfBaseMember = new XElement(baseMemberXml);
            var expectedResolvedMember = new XElement(memberXml);
            expectedResolvedMember.Descendants("inheritdoc").Single().ReplaceWith(copyOfBaseMember.XPathEvaluate("/node()"));

            // Act
            var actualResolvedMember = InheritdocResolver.ResolveInheritDocumentation(memberXml, xmlExtractionContext, testedMember);

            // Assert
            Assert.AreEqual(expectedResolvedMember.ToString(), actualResolvedMember.ToString());
        }
    }
}
