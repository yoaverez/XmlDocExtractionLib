using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Reflection;
using System.Xml.Linq;
using XmlDocExtractionLib.Tests.DummyTypes;

namespace XmlDocExtractionLib.Tests
{
    [TestClass]
    public class MemberIdentifierExtractionExtensionsTests
    {
        private static Dictionary<string, string> identifiersToSummary;

        private static BindingFlags bindingFlags;

        [ClassInitialize]
        public static void ClassInitialize(TestContext testContext)
        {
            bindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static ;

            var xmlDocumentationFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlDocumentation = XDocument.Load(xmlDocumentationFile);
            identifiersToSummary = xmlDocumentation.Descendants("members")
                                                   .Single()
                                                   .Elements("member")
                                                   .ToDictionary(x => x.Attribute("name")!.Value, x => x.Element("summary")!.Value.Trim())!;
        }

        #region GetTypeNameIdentifier Tests

        [DataRow(typeof(DummyEnum), "DummyEnum docs.")]
        [DataRow(typeof(DummyClass), "DummyClass docs.")]
        [DataRow(typeof(DummyStruct), "DummyStruct docs.")]
        [DataRow(typeof(DummyRecord), "DummyRecord docs.")]
        [DataRow(typeof(IDummyInterface), "IDummyInterface docs.")]
        [DataRow(typeof(DummyGenericType<,>), "DummyGenericType docs.")]
        [DataRow(typeof(DummyGenericType<,>.DummyGenericNestedType<>), "DummyGenericNestedType docs.")]
        [DataTestMethod]
        public void GetTypeNameIdentifier_IdentifierIsCorrect(Type type, string expectedSummaryDocumentation)
        {
            // Act
            var identifier = type.GetTypeNameIdentifier();

            // Assert
            var actualSummaryDocumentation = identifiersToSummary[identifier];
            Assert.AreEqual(expectedSummaryDocumentation, actualSummaryDocumentation);
        }

        #endregion GetTypeNameIdentifier Tests

        #region GetFieldNameIdentifier Tests

        [DataRow(typeof(DummyClass), "field1")]
        [DataRow(typeof(DummyGenericType<,>), "fieldA")]
        [DataRow(typeof(DummyGenericType<,>), "fieldB")]
        [DataRow(typeof(DummyGenericType<,>.DummyGenericNestedType<>), "field01")]
        [DataTestMethod]
        public void GetFieldNameIdentifier_IdentifierIsCorrect(Type type, string fieldName)
        {
            // Arrange
            var expectedSummaryDocumentation = $"{fieldName} docs.";
            var fieldInfo = type.GetField(fieldName);

            // Act
            var identifier = fieldInfo.GetFieldNameIdentifier();

            // Assert
            var actualSummaryDocumentation = identifiersToSummary[identifier];
            Assert.AreEqual(expectedSummaryDocumentation, actualSummaryDocumentation);
        }

        #endregion GetFieldNameIdentifier Tests

        #region GetEnumValueNameIdentifier Tests

        [TestMethod]
        public void GetEnumValueNameIdentifier_TypeIsEnum_IdentifierIsCorrect()
        {
            // Arrange
            var enumType = typeof(DummyEnum);
            var enumValue = DummyEnum.Value1;
            var expectedSummaryDocumentation = "Value1 docs.";

            // Act
            var identifier = enumType.GetEnumValueNameIdentifier((int)enumValue);

            // Assert
            var actualSummaryDocumentation = identifiersToSummary[identifier];
            Assert.AreEqual(expectedSummaryDocumentation, actualSummaryDocumentation);
        }

        [TestMethod]
        public void GetEnumValueNameIdentifier_TypeIsNotEnum_ThrowsArgumentException()
        {
            // Arrange
            var enumType = typeof(int);
            var enumValue = DummyEnum.Value1;

            // Act + Assert
            Assert.ThrowsException<ArgumentException>(() => enumType.GetEnumValueNameIdentifier((int)enumValue));
        }

        #endregion GetEnumValueNameIdentifier Tests

        #region GetPropertyNameIdentifier Tests

        [DataRow(typeof(DummyClass), "Prop1")]
        [DataRow(typeof(DummyGenericType<,>), "PropA")]
        [DataRow(typeof(DummyGenericType<,>), "PropB")]
        [DataRow(typeof(DummyGenericType<,>.DummyGenericNestedType<>), "Prop01")]
        [DataRow(typeof(IDummyInterface), "Prop001")]
        [DataRow(typeof(IDummyGenericInterface<>), "Prop0001")]
        [DataTestMethod]
        public void GetPropertyNameIdentifier_IdentifierIsCorrect(Type type, string propertyName)
        {
            // Arrange
            var expectedSummaryDocumentation = $"{propertyName} docs.";
            var propertyInfo = type.GetProperty(propertyName, bindingFlags);

            // Act
            var identifier = propertyInfo.GetPropertyNameIdentifier();

            // Assert
            var actualSummaryDocumentation = identifiersToSummary[identifier];
            Assert.AreEqual(expectedSummaryDocumentation, actualSummaryDocumentation);
        }

        [DataRow(typeof(DummyGenericType<,>), "XmlDocExtractionLib.Tests.DummyTypes.IDummyInterface.Prop001")]
        [DataRow(typeof(DummyGenericType<,>), "XmlDocExtractionLib.Tests.DummyTypes.IDummyGenericInterface<S>.Prop0001")]
        [DataTestMethod]
        public void GetPropertyNameIdentifier_PropertyIsExplicitInterfaceImplementation_IdentifierIsCorrect(Type type, string propertyName)
        {
            // Arrange
            var propName = propertyName.Split('.').Last();
            var expectedSummaryDocumentation = $"Explicit interface implementation of {propName} docs.";
            var propertyInfo = type.GetProperty(propertyName, bindingFlags);

            // Act
            var identifier = propertyInfo.GetPropertyNameIdentifier();

            // Assert
            var actualSummaryDocumentation = identifiersToSummary[identifier];
            Assert.AreEqual(expectedSummaryDocumentation, actualSummaryDocumentation);
        }

        #endregion GetPropertyNameIdentifier Tests

        #region GetEventNameIdentifier Tests

        [DataRow(typeof(DummyClass), "Event1")]
        [DataRow(typeof(DummyGenericType<,>), "EventA")]
        [DataRow(typeof(DummyGenericType<,>.DummyGenericNestedType<>), "Event01")]
        [DataRow(typeof(IDummyInterface), "Event001")]
        [DataRow(typeof(IDummyGenericInterface<>), "Event0001")]
        [DataTestMethod]
        public void GetEventNameIdentifier_IdentifierIsCorrect(Type type, string eventName)
        {
            // Arrange
            var expectedSummaryDocumentation = $"{eventName} docs.";
            var eventInfo = type.GetEvent(eventName, bindingFlags);

            // Act
            var identifier = eventInfo.GetEventNameIdentifier();

            // Assert
            var actualSummaryDocumentation = identifiersToSummary[identifier];
            Assert.AreEqual(expectedSummaryDocumentation, actualSummaryDocumentation);
        }

        [DataRow(typeof(DummyGenericType<,>), "XmlDocExtractionLib.Tests.DummyTypes.IDummyInterface.Event001")]
        [DataRow(typeof(DummyGenericType<,>), "XmlDocExtractionLib.Tests.DummyTypes.IDummyGenericInterface<S>.Event0001")]
        [DataTestMethod]
        public void GetEventNameIdentifier_EventIsExplicitInterfaceImplementation_IdentifierIsCorrect(Type type, string eventName)
        {
            // Arrange
            var shortEventName = eventName.Split('.').Last();
            var expectedSummaryDocumentation = $"Explicit interface implementation of {shortEventName} docs.";
            var eventInfo = type.GetEvent(eventName, bindingFlags);

            // Act
            var identifier = eventInfo.GetEventNameIdentifier();

            // Assert
            var actualSummaryDocumentation = identifiersToSummary[identifier];
            Assert.AreEqual(expectedSummaryDocumentation, actualSummaryDocumentation);
        }

        #endregion GetEventNameIdentifier Tests

        #region GetMethodNameIdentifier Tests

        [DataRow(typeof(DummyClass), "Method1")]
        [DataRow(typeof(DummyClass), "op_Addition")]
        [DataRow(typeof(DummyClass), "Finalize")]
        [DataRow(typeof(DummyGenericType<,>), "MethodA")]
        [DataRow(typeof(DummyGenericType<,>.DummyGenericNestedType<>), "Method01")]
        [DataRow(typeof(IDummyInterface), "Method001")]
        [DataRow(typeof(IDummyGenericInterface<>), "Method0001")]
        [DataTestMethod]
        public void GetMethodNameIdentifier_IdentifierIsCorrect(Type type, string methodName)
        {
            // Arrange
            var expectedSummaryDocumentation = $"{methodName} docs.";
            var methodInfo = type.GetMethod(methodName, bindingFlags);

            // Act
            var identifier = methodInfo.GetMethodNameIdentifier();

            // Assert
            var actualSummaryDocumentation = identifiersToSummary[identifier];
            Assert.AreEqual(expectedSummaryDocumentation, actualSummaryDocumentation);
        }

        [DataRow(typeof(DummyClass), new Type[] { }, "Ctor1")]
        [DataRow(typeof(DummyClass), new Type[] { typeof(int[]), typeof(bool*[,][]) }, "Ctor2")]
        [DataTestMethod]
        public void GetMethodNameIdentifier_Ctors_IdentifierIsCorrect(Type type, Type[] parameterTypes, string ctorName)
        {
            // Arrange
            var expectedSummaryDocumentation = $"{ctorName} docs.";
            var ctorInfo = type.GetConstructor(bindingFlags, parameterTypes);

            // Act
            var identifier = ctorInfo.GetMethodNameIdentifier();

            // Assert
            var actualSummaryDocumentation = identifiersToSummary[identifier];
            Assert.AreEqual(expectedSummaryDocumentation, actualSummaryDocumentation);
        }

        [DataRow(typeof(DummyGenericType<,>), "XmlDocExtractionLib.Tests.DummyTypes.IDummyInterface.Method001")]
        [DataRow(typeof(DummyGenericType<,>), "XmlDocExtractionLib.Tests.DummyTypes.IDummyGenericInterface<S>.Method0001")]
        [DataTestMethod]
        public void GetMethodNameIdentifier_PropertyIsExplicitInterfaceImplementation_IdentifierIsCorrect(Type type, string methodName)
        {
            // Arrange
            var methodShortName = methodName.Split('.').Last();
            var expectedSummaryDocumentation = $"Explicit interface implementation of {methodShortName} docs.";
            var methodInfo = type.GetMethod(methodName, bindingFlags);

            // Act
            var identifier = methodInfo.GetMethodNameIdentifier();

            // Assert
            var actualSummaryDocumentation = identifiersToSummary[identifier];
            Assert.AreEqual(expectedSummaryDocumentation, actualSummaryDocumentation);
        }

        #endregion GetMethodNameIdentifier Tests
    }
}
