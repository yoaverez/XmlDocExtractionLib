using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Reflection;
using XmlDocExtractionLib.Tests.DummyTypes;

namespace XmlDocExtractionLib.Tests
{
    [TestClass]
    public class ExplicitInterfaceImplementationExtensionsTests
    {
        private static BindingFlags bindingFlags;

        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
        }

        #region Is Method ExplicitInterfaceImplementation Tests

        [TestMethod]
        public void IsExplicitInterfaceImplementation_MethodIsExplicitInterfaceImplementation_ReturnTrueAndDeclaringInterfaceMethod()
        {
            // Arrange
            var testedType = typeof(DummyGenericType<,>);
            var methodFullName = "XmlDocExtractionLib.Tests.DummyTypes.IDummyGenericInterface<S>.Method0001";
            var methodShortName = methodFullName.Split('.').Last();
            var method = testedType.GetMethod(methodFullName, bindingFlags);

            var expectedDeclaringInterface = typeof(IDummyGenericInterface<>).MakeGenericType(testedType.GetGenericArguments()[1]);
            var expectedInterfaceMethod = expectedDeclaringInterface.GetMethod(methodShortName);

            // Act
            var actualResult = method.IsExplicitInterfaceImplementation(out var actualInterfaceMethod);

            // Assert
            Assert.IsTrue(actualResult);
            Assert.AreEqual(expectedInterfaceMethod, actualInterfaceMethod);
            Assert.AreEqual(expectedDeclaringInterface, actualInterfaceMethod.DeclaringType);
        }

        [TestMethod]
        public void IsExplicitInterfaceImplementation_MethodIsNotExplicitInterfaceImplementation_ReturnFalse()
        {
            // Arrange
            var testedType = typeof(DummyGenericType<,>);
            var method = testedType.GetMethod("Method002", bindingFlags);

            // Act
            var actualResult = method.IsExplicitInterfaceImplementation(out _);

            // Assert
            Assert.IsFalse(actualResult);
        }

        #endregion Is Method ExplicitInterfaceImplementation Tests

        #region Is Property ExplicitInterfaceImplementation Tests

        [TestMethod]
        public void IsExplicitInterfaceImplementation_PropertyIsExplicitInterfaceImplementation_ReturnTrueAndDeclaringInterfaceProperty()
        {
            // Arrange
            var testedType = typeof(DummyGenericType<,>);
            var propFullName = "XmlDocExtractionLib.Tests.DummyTypes.IDummyGenericInterface<S>.Prop0001";
            var propShortName = propFullName.Split('.').Last();
            var property = testedType.GetProperty(propFullName, bindingFlags);

            var expectedDeclaringInterface = typeof(IDummyGenericInterface<>).MakeGenericType(testedType.GetGenericArguments()[1]);
            var expectedInterfaceProperty = expectedDeclaringInterface.GetProperty(propShortName);

            // Act
            var actualResult = property.IsExplicitInterfaceImplementation(out var actualInterfacePropery);

            // Assert
            Assert.IsTrue(actualResult);
            Assert.AreEqual(expectedInterfaceProperty, actualInterfacePropery);
            Assert.AreEqual(expectedDeclaringInterface, actualInterfacePropery.DeclaringType);
        }

        [TestMethod]
        public void IsExplicitInterfaceImplementation_PropertyIsNotExplicitInterfaceImplementation_ReturnFalse()
        {
            // Arrange
            var testedType = typeof(DummyGenericType<,>);
            var property = testedType.GetProperty("Prop002", bindingFlags);

            // Act
            var actualResult = property.IsExplicitInterfaceImplementation(out _);

            // Assert
            Assert.IsFalse(actualResult);
        }

        #endregion Is Property ExplicitInterfaceImplementation Tests

        #region Is Event ExplicitInterfaceImplementation Tests

        [TestMethod]
        public void IsExplicitInterfaceImplementation_EventIsExplicitInterfaceImplementation_ReturnTrueAndDeclaringInterfaceEvent()
        {
            // Arrange
            var testedType = typeof(DummyGenericType<,>);
            var eventFullName = "XmlDocExtractionLib.Tests.DummyTypes.IDummyGenericInterface<S>.Event0001";
            var eventShortName = eventFullName.Split('.').Last();
            var eventInfo = testedType.GetEvent(eventFullName, bindingFlags);

            var expectedDeclaringInterface = typeof(IDummyGenericInterface<>).MakeGenericType(testedType.GetGenericArguments()[1]);
            var expectedInterfaceEvent = expectedDeclaringInterface.GetEvent(eventShortName);

            // Act
            var actualResult = eventInfo.IsExplicitInterfaceImplementation(out var actualInterfaceEvent);

            // Assert
            Assert.IsTrue(actualResult);
            Assert.AreEqual(expectedInterfaceEvent, actualInterfaceEvent);
            Assert.AreEqual(expectedDeclaringInterface, actualInterfaceEvent.DeclaringType);
        }

        [TestMethod]
        public void IsExplicitInterfaceImplementation_EventIsNotExplicitInterfaceImplementation_ReturnFalse()
        {
            // Arrange
            var testedType = typeof(DummyGenericType<,>);
            var eventInfo = testedType.GetEvent("Event002", bindingFlags);

            // Act
            var actualResult = eventInfo.IsExplicitInterfaceImplementation(out _);

            // Assert
            Assert.IsFalse(actualResult);
        }

        #endregion Is Event ExplicitInterfaceImplementation Tests
    }
}
