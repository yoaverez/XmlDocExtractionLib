using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Reflection;
using System.Xml.Linq;
using XmlDocExtractionLib.Tests.DummyTypes.TypesWithInheritdocs;

namespace XmlDocExtractionLib.Tests
{
    [TestClass]
    public class XmlExtractionContextTests
    {
        private static Assembly testAssembly;

        private static string testAssemblyXmlPath;

        private static Assembly libAssembly;

        private static string libAssemblyXmlPath;

        private static Dictionary<string, XElement> xmlMembers;

        private static BindingFlags bindingFlags;

        private XmlExtractionContext xmlExtractionContext;

        [ClassInitialize]
        public static void ClassInitialize(TestContext testContext)
        {
            bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static;

            testAssembly = typeof(XmlExtractionContextTests).Assembly;
            testAssemblyXmlPath = $"{testAssembly.GetName().Name!}.xml";
            libAssembly = typeof(XmlExtractionContext).Assembly;
            libAssemblyXmlPath = $"{libAssembly.GetName().Name!}.xml";

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

        [TestInitialize]
        public void TestInitialize()
        {
            xmlExtractionContext = new XmlExtractionContext();
        }

        #region TryGetMemberXmlDocsFromMemberIdentifier Tests

        [DataRow(typeof(Inheritdoc1))]
        [DataRow(typeof(XmlExtractionContext))]
        [DataTestMethod]
        public void TryGetMemberXmlDocsFromMemberIdentifier_NoAssembliesLoaded_ReturnFalse(Type type)
        {
            // Arrange
            var memberIdentifier = type.GetMemberIdentifier();

            // Act
            var actualResult = xmlExtractionContext.TryGetMemberXmlDocsFromMemberIdentifier(memberIdentifier, out _);

            // Assert
            Assert.IsFalse(actualResult);
        }

        [DataRow(typeof(Inheritdoc1))]
        [DataRow(typeof(XmlExtractionContext))]
        [DataTestMethod]
        public void TryGetMemberXmlDocsFromMemberIdentifier_MultipleAssembliesLoaded_ReturnTrue(Type type)
        {
            // Arrange
            xmlExtractionContext.AddAssembly(testAssembly, testAssemblyXmlPath);
            xmlExtractionContext.AddAssembly(libAssembly, libAssemblyXmlPath);
            var memberIdentifier = type.GetMemberIdentifier();

            var expectedXmlDoc = xmlMembers[memberIdentifier];

            // Act
            var actualResult = xmlExtractionContext.TryGetMemberXmlDocsFromMemberIdentifier(memberIdentifier, out var actualXmlDoc);

            // Assert
            Assert.IsTrue(actualResult);
            Assert.AreEqual(expectedXmlDoc.ToString(), actualXmlDoc.ToString());
        }

        #endregion TryGetMemberXmlDocsFromMemberIdentifier Tests

        #region TryGetMemberXmlDocsFromMember Tests

        [DataRow(typeof(Inheritdoc1))]
        [DataRow(typeof(XmlExtractionContext))]
        [DataTestMethod]
        public void TryGetMemberXmlDocsFromMember_NoAssembliesLoaded_ReturnFalse(Type type)
        {
            // Act
            var actualResult = xmlExtractionContext.TryGetMemberXmlDocsFromMember(type, out _);

            // Assert
            Assert.IsFalse(actualResult);
        }

        [DataRow(typeof(Inheritdoc1))]
        [DataRow(typeof(XmlExtractionContext))]
        [DataTestMethod]
        public void TryGetMemberXmlDocsFromMember_MultipleAssembliesLoaded_ReturnTrue(Type type)
        {
            // Arrange
            xmlExtractionContext.AddAssembly(testAssembly, testAssemblyXmlPath);
            xmlExtractionContext.AddAssembly(libAssembly, libAssemblyXmlPath);
            var memberIdentifier = type.GetMemberIdentifier();

            var expectedXmlDoc = xmlMembers[memberIdentifier];

            // Act
            var actualResult = xmlExtractionContext.TryGetMemberXmlDocsFromMember(type, out var actualXmlDoc);

            // Assert
            Assert.IsTrue(actualResult);
            Assert.AreEqual(expectedXmlDoc.ToString(), actualXmlDoc.ToString());
        }

        #endregion TryGetMemberXmlDocsFromMember Tests

        #region TryGetMemberFromMemberIdentifier Tests

        [DataRow(typeof(Inheritdoc1))]
        [DataTestMethod]
        public void TryGetMemberFromMemberIdentifier_MemberDoesNotExists_ReturnFalse(Type type)
        {
            // Arrange
            xmlExtractionContext.AddAssembly(testAssembly, testAssemblyXmlPath);
            xmlExtractionContext.AddAssembly(libAssembly, libAssemblyXmlPath);

            var unexistingMemberIdentifier = "a";

            // Act
            var actualResult = xmlExtractionContext.TryGetMemberFromMemberIdentifier(unexistingMemberIdentifier, out _);

            // Assert
            Assert.IsFalse(actualResult);
        }

        [DataRow(typeof(Inheritdoc1))]
        [DataRow(typeof(XmlExtractionContext))]
        [DataTestMethod]
        public void TryGetMemberFromMemberIdentifier_TypeDoesExists_ReturnTrue(Type type)
        {
            // Arrange
            xmlExtractionContext.AddAssembly(testAssembly, testAssemblyXmlPath);
            xmlExtractionContext.AddAssembly(libAssembly, libAssemblyXmlPath);

            var memberIdentifier = type.GetMemberIdentifier();

            // Act
            var actualResult = xmlExtractionContext.TryGetMemberFromMemberIdentifier(memberIdentifier, out var actualMember);

            // Assert
            Assert.IsTrue(actualResult);
            Assert.AreEqual(type, actualMember);
        }

        [DataRow(typeof(Inheritdoc1), nameof(Inheritdoc1.PropWithGetterOnly))]
        [DataRow(typeof(Inheritdoc1), nameof(Inheritdoc1.SelfDeclaredEvent))]
        [DataRow(typeof(Inheritdoc1), nameof(Inheritdoc1.Method1))]
        [DataRow(typeof(XmlExtractionContext), "lazyAssemblyLoader")]
        [DataTestMethod]
        public void TryGetMemberFromMemberIdentifier_MemberDoesExists_ReturnTrue(Type type, string memberName)
        {
            // Arrange
            // The "Last" if for the event.
            var testedMember = type.GetMember(memberName, bindingFlags).Last();
            xmlExtractionContext.AddAssembly(testAssembly, testAssemblyXmlPath);
            xmlExtractionContext.AddAssembly(libAssembly, libAssemblyXmlPath);

            var memberIdentifier = testedMember.GetMemberIdentifier();

            // Act
            var actualResult = xmlExtractionContext.TryGetMemberFromMemberIdentifier(memberIdentifier, out var actualMember);

            // Assert
            Assert.IsTrue(actualResult);
            Assert.AreEqual(testedMember, actualMember);
        }

        #endregion TryGetMemberFromMemberIdentifier Tests
    }
}
