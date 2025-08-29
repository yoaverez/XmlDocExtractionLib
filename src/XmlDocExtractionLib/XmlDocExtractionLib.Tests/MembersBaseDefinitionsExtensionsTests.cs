using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Reflection;
using XmlDocExtractionLib.Tests.DummyTypes.TypesWithInheritdocs;

namespace XmlDocExtractionLib.Tests
{
    [TestClass]
    public class MembersBaseDefinitionsExtensionsTests
    {
        private static BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;

        #region GetTypeBaseDefinitions Tests

        [TestMethod]
        public void GetTypeBaseDefinitions_TypeIsNotInterfaceAndDoesNotHaveBaseType_ReturnEmptyEnumerable()
        {
            // Arrange
            // Only object does not have a base type.
            var testedType = typeof(object);
            var expectedBaseDefinition = new Type[] { };

            // Act
            var actualBaseDefinition = testedType.GetTypeBaseDefinitions().ToArray();

            // Assert
            CollectionAssert.AreEqual(expectedBaseDefinition, actualBaseDefinition);
        }

        [TestMethod]
        public void GetTypeBaseDefinitions_TypeIsNotInterfaceAndDoesHaveBaseTypes_ReturnCorrectBaseDefinitions()
        {
            // Arrange
            var testedType = typeof(Inheritdoc1);
            var expectedBaseDefinition = new Type[]
            {
                typeof(InheritDocBase2),
                typeof(InheritDocBase1),
                typeof(object),
            };

            // Act
            var actualBaseDefinition = testedType.GetTypeBaseDefinitions().ToArray();

            // Assert
            CollectionAssert.AreEqual(expectedBaseDefinition, actualBaseDefinition);
        }

        [TestMethod]
        public void GetTypeBaseDefinitions_TypeIsInterfaceAndDoesNotImplementsInterfaces_ReturnEmptyEnumerable()
        {
            // Arrange
            var testedType = typeof(IInheritdoc1);
            var expectedBaseDefinition = new Type[] { };

            // Act
            var actualBaseDefinition = testedType.GetTypeBaseDefinitions().ToArray();

            // Assert
            CollectionAssert.AreEqual(expectedBaseDefinition, actualBaseDefinition);
        }

        [TestMethod]
        public void GetTypeBaseDefinitions_TypeIsInterfaceAndDoesImplementsInterfaces_ReturnEmptyEnumerable()
        {
            // Arrange
            var testedType = typeof(IInheritdoc3);
            var expectedBaseDefinition = new Type[]
            {
                typeof(IInheritdoc1),
                typeof(IInheritdoc2),
            };

            // Act
            var actualBaseDefinition = testedType.GetTypeBaseDefinitions().ToArray();

            // Assert
            CollectionAssert.AreEqual(expectedBaseDefinition, actualBaseDefinition);
        }

        #endregion GetTypeBaseDefinitions Tests

        #region GetConstructorBaseDefinitions Tests

        [TestMethod]
        public void GetConstructorBaseDefinitions_CtorReflectedTypeDoesNotHaveBaseType_ReturnEmptyEnumerable()
        {
            // Arrange
            // Only object does not have a base type.
            var testedCtor = GetConstructor(typeof(object));
            var expectedBaseDefinition = new ConstructorInfo[] { };

            // Act
            var actualBaseDefinition = testedCtor.GetConstructorBaseDefinitions().ToArray();

            // Assert
            CollectionAssert.AreEqual(expectedBaseDefinition, actualBaseDefinition);
        }

        [TestMethod]
        public void GetConstructorBaseDefinitions_CtorIsSelfDeclared_ReturnEmptyEnumerable()
        {
            // Arrange
            var testedCtor = GetConstructor(typeof(InheritDocBase1), typeof(int));
            var expectedBaseDefinition = new ConstructorInfo[] { };

            // Act
            var actualBaseDefinition = testedCtor.GetConstructorBaseDefinitions().ToArray();

            // Assert
            CollectionAssert.AreEqual(expectedBaseDefinition, actualBaseDefinition);
        }

        [TestMethod]
        public void GetConstructorBaseDefinitions_CtorHasMultipleBaseDefinitions_ReturnCurrectBaseDefinitions()
        {
            // Arrange
            var testedCtor = GetConstructor(typeof(Inheritdoc1), typeof(int));
            var expectedBaseDefinition = new ConstructorInfo[]
            {
                GetConstructor(typeof(InheritDocBase2), typeof(int)),
                GetConstructor(typeof(InheritDocBase1), typeof(int)),
            };

            // Act
            var actualBaseDefinition = testedCtor.GetConstructorBaseDefinitions().ToArray();

            // Assert
            CollectionAssert.AreEqual(expectedBaseDefinition, actualBaseDefinition);
        }

        [TestMethod]
        public void GetConstructorBaseDefinitions_CtorLooksTheSameAsBaseCtorWithDifferentVisability_ReturnCurrectBaseDefinitions()
        {
            // Arrange
            var testedCtor = GetConstructor(typeof(Inheritdoc1), typeof(string));
            var expectedBaseDefinition = new ConstructorInfo[]
            {
                GetConstructor(typeof(InheritDocBase2), typeof(string)),
            };

            // Act
            var actualBaseDefinition = testedCtor.GetConstructorBaseDefinitions().ToArray();

            // Assert
            CollectionAssert.AreEqual(expectedBaseDefinition, actualBaseDefinition);
        }

        [TestMethod]
        public void GetConstructorBaseDefinitions_CtorLooksTheSameAsBasePrivateCtor_ReturnCurrectBaseDefinitions()
        {
            // Arrange
            var testedCtor = GetConstructor(typeof(Inheritdoc1), typeof(bool));
            var expectedBaseDefinition = new ConstructorInfo[]
            {
                GetConstructor(typeof(InheritDocBase2), typeof(bool)),
            };

            // Act
            var actualBaseDefinition = testedCtor.GetConstructorBaseDefinitions().ToArray();

            // Assert
            CollectionAssert.AreEqual(expectedBaseDefinition, actualBaseDefinition);
        }

        [TestMethod]
        public void GetConstructorBaseDefinitions_CtorLooksTheSameAsSecondBasePrivateCtor_ReturnEmptyEnumerable()
        {
            // Arrange
            var testedCtor = GetConstructor(typeof(Inheritdoc1), typeof(long));
            var expectedBaseDefinition = new ConstructorInfo[] { };

            // Act
            var actualBaseDefinition = testedCtor.GetConstructorBaseDefinitions().ToArray();

            // Assert
            CollectionAssert.AreEqual(expectedBaseDefinition, actualBaseDefinition);
        }

        #endregion GetConstructorBaseDefinitions Tests

        #region GetMethodBaseDefinitions Tests

        [TestMethod]
        public void GetMethodBaseDefinitions_MethodIsSelfDeclared_ReturnEmptyEnumerable()
        {
            // Arrange
            var testedMethod = GetMethod(typeof(Inheritdoc1), nameof(Inheritdoc1.SelfDeclaredMethod));
            var expectedBaseDefinitions = Array.Empty<MethodInfo>();

            // Act
            var actualBaseDefinitions = testedMethod.GetMethodBaseDefinitions().ToArray();

            // Assert
            CollectionAssert.AreEqual(expectedBaseDefinitions, actualBaseDefinitions);
        }

        [TestMethod]
        public void GetMethodBaseDefinitions_MethodHasTheNewModifier_ReturnEmptyEnumerable()
        {
            // Arrange
            var testedMethod = GetMethod(typeof(Inheritdoc1), nameof(Inheritdoc1.Method1ForNewModifier));
            var expectedBaseDefinitions = Array.Empty<MethodInfo>();

            // Act
            var actualBaseDefinitions = testedMethod.GetMethodBaseDefinitions().ToArray();

            // Assert
            CollectionAssert.AreEqual(expectedBaseDefinitions, actualBaseDefinitions);
        }

        [TestMethod]
        public void GetMethodBaseDefinitions_MethodImplementsMethodWithNewModifier_ReturnCorrectBaseDefinitions()
        {
            // Arrange
            var testedMethod = GetMethod(typeof(Inheritdoc1), nameof(Inheritdoc1.Method2ForNewModifier));
            var expectedBaseDefinitions = new MethodInfo[]
            {
                GetMethod(typeof(InheritDocBase1), nameof(InheritDocBase1.Method2ForNewModifier)),
                GetMethod(typeof(IInheritdoc3), nameof(IInheritdoc3.Method2ForNewModifier)),
            };

            // Act
            var actualBaseDefinitions = testedMethod.GetMethodBaseDefinitions().ToArray();

            // Assert
            CollectionAssert.AreEqual(expectedBaseDefinitions, actualBaseDefinitions);
        }

        [TestMethod]
        public void GetMethodBaseDefinitions_ToStringOverrides_ReturnCorrectBaseDefinitions()
        {
            // Arrange
            var testedMethod = GetMethod(typeof(Inheritdoc1), nameof(Inheritdoc1.ToString));
            var expectedBaseDefinitions = new MethodInfo[]
            {
                GetMethod(typeof(InheritDocBase2), nameof(InheritDocBase2.ToString)),
                GetMethod(typeof(InheritDocBase1), nameof(InheritDocBase1.ToString)),
                GetMethod(typeof(object), nameof(object.ToString)),
            };

            // Act
            var actualBaseDefinitions = testedMethod.GetMethodBaseDefinitions().ToArray();

            // Assert
            CollectionAssert.AreEqual(expectedBaseDefinitions, actualBaseDefinitions);
        }

        [TestMethod]
        public void GetMethodBaseDefinitions_MethodIsExplicitInterfaceImplementation_ReturnCorrectBaseDefinitions()
        {
            // Arrange
            var interfaceType = typeof(IInheritdoc2);
            var testedMethod = GetMethod(typeof(InheritDocBase1), $"{interfaceType.Namespace}.{nameof(IInheritdoc2)}.{nameof(IInheritdoc2.MethodForExplicitInterfaceImplementation)}");
            var expectedBaseDefinitions = new MethodInfo[]
            {
                GetMethod(typeof(IInheritdoc2), nameof(IInheritdoc2.MethodForExplicitInterfaceImplementation)),
            };

            // Act
            var actualBaseDefinitions = testedMethod.GetMethodBaseDefinitions().ToArray();

            // Assert
            CollectionAssert.AreEqual(expectedBaseDefinitions, actualBaseDefinitions);
        }

        [TestMethod]
        public void GetMethodBaseDefinitions_MethodLooksTheSameAsBasePrivateMethod_ReturnEmptyEnumerable()
        {
            // Arrange
            var testedMethod = GetMethod(typeof(Inheritdoc1), "BasePrivateMethod");
            var expectedBaseDefinitions = new MethodInfo[]
            {
            };

            // Act
            var actualBaseDefinitions = testedMethod.GetMethodBaseDefinitions().ToArray();

            // Assert
            CollectionAssert.AreEqual(expectedBaseDefinitions, actualBaseDefinitions);
        }

        #endregion GetMethodBaseDefinitions Tests

        #region GetPropertyBaseDefinitions Tests

        [TestMethod]
        public void GetPropertyBaseDefinitions_PropertyIsSelfDeclared_ReturnEmptyEnumerable()
        {
            // Arrange
            var testedProperty = GetProperty(typeof(Inheritdoc1), nameof(Inheritdoc1.SelfDeclaredProp));
            var expectedBaseDefinitions = Array.Empty<PropertyInfo>();

            // Act
            var actualBaseDefinitions = testedProperty.GetPropertyBaseDefinitions().ToArray();

            // Assert
            CollectionAssert.AreEqual(expectedBaseDefinitions, actualBaseDefinitions);
        }

        [TestMethod]
        public void GetPropertyBaseDefinitions_PropertyIsInheritedAndWithAdditionalSetter_ReturnCorrectBaseDefinitions()
        {
            // Arrange
            var testedProperty = GetProperty(typeof(InheritDocBase1), nameof(InheritDocBase1.PropWithGetterOnly));
            var expectedBaseDefinitions = new PropertyInfo[]
            {
                GetProperty(typeof(IInheritdoc2), nameof(IInheritdoc2.PropWithGetterOnly)),
            };

            // Act
            var actualBaseDefinitions = testedProperty.GetPropertyBaseDefinitions().ToArray();

            // Assert
            CollectionAssert.AreEqual(expectedBaseDefinitions, actualBaseDefinitions);
        }

        [TestMethod]
        public void GetPropertyBaseDefinitions_PropertyIsInheritedAndWithAdditionalGetter_ReturnCorrectBaseDefinitions()
        {
            // Arrange
            var testedProperty = GetProperty(typeof(InheritDocBase1), nameof(InheritDocBase1.PropWithSetterOnly));
            var expectedBaseDefinitions = new PropertyInfo[]
            {
                GetProperty(typeof(IInheritdoc2), nameof(IInheritdoc2.PropWithSetterOnly)),
            };

            // Act
            var actualBaseDefinitions = testedProperty.GetPropertyBaseDefinitions().ToArray();

            // Assert
            CollectionAssert.AreEqual(expectedBaseDefinitions, actualBaseDefinitions);
        }

        [TestMethod]
        public void GetPropertyBaseDefinitions_PropertyHasTheNewModifier_ReturnEmptyEnumerable()
        {
            // Arrange
            var testedProperty = GetProperty(typeof(Inheritdoc1), nameof(Inheritdoc1.Prop1ForNewModifier));
            var expectedBaseDefinitions = new PropertyInfo[]
            {
            };

            // Act
            var actualBaseDefinitions = testedProperty.GetPropertyBaseDefinitions().ToArray();

            // Assert
            CollectionAssert.AreEqual(expectedBaseDefinitions, actualBaseDefinitions);
        }

        [TestMethod]
        public void GetPropertyBaseDefinitions_PropertyIsOverrideOfPropertyWithTheNewModifier_ReturnCorrectBaseDefinitions()
        {
            // Arrange
            var testedProperty = GetProperty(typeof(Inheritdoc1), nameof(Inheritdoc1.Prop2ForNewModifier));
            var expectedBaseDefinitions = new PropertyInfo[]
            {
                GetProperty(typeof(InheritDocBase2), nameof(InheritDocBase2.Prop2ForNewModifier)),
                GetProperty(typeof(InheritDocBase1), nameof(InheritDocBase1.Prop2ForNewModifier)),
                GetProperty(typeof(IInheritdoc3), nameof(IInheritdoc3.Prop2ForNewModifier)),
            };

            // Act
            var actualBaseDefinitions = testedProperty.GetPropertyBaseDefinitions().ToArray();

            // Assert
            CollectionAssert.AreEqual(expectedBaseDefinitions, actualBaseDefinitions);
        }

        [TestMethod]
        public void GetPropertyBaseDefinitions_PropertyLooksTheSameAsBasePrivateProperty_ReturnEmptyEnumerable()
        {
            // Arrange
            var testedProperty = GetProperty(typeof(Inheritdoc1), "PrivateProp");
            var expectedBaseDefinitions = new PropertyInfo[]
            {
            };

            // Act
            var actualBaseDefinitions = testedProperty.GetPropertyBaseDefinitions().ToArray();

            // Assert
            CollectionAssert.AreEqual(expectedBaseDefinitions, actualBaseDefinitions);
        }

        [TestMethod]
        public void GetPropertyBaseDefinitions_PropertyIsExplicitInterfaceImplementation_ReturnCorrectBaseDefinitions()
        {
            // Arrange
            var interfaceType = typeof(IInheritdoc2);
            var testedProperty = GetProperty(typeof(InheritDocBase1), $"{interfaceType.FullName}.{nameof(IInheritdoc2.PropForExplicitInterfaceImplementation)}");
            var expectedBaseDefinitions = new PropertyInfo[]
            {
                GetProperty(interfaceType, nameof(IInheritdoc2.PropForExplicitInterfaceImplementation)),
            };

            // Act
            var actualBaseDefinitions = testedProperty.GetPropertyBaseDefinitions().ToArray();

            // Assert
            CollectionAssert.AreEqual(expectedBaseDefinitions, actualBaseDefinitions);
        }

        #endregion GetPropertyBaseDefinitions Tests

        #region GetEventBaseDefinitions Tests

        [TestMethod]
        public void GetEventBaseDefinitions_EventIsSelfDeclared_ReturnEmptyEnumerable()
        {
            // Arrange
            var testedEvent = GetEvent(typeof(Inheritdoc1), nameof(Inheritdoc1.SelfDeclaredEvent));
            var expectedBaseDefinitions = Array.Empty<EventInfo>();

            // Act
            var actualBaseDefinitions = testedEvent.GetEventBaseDefinitions().ToArray();

            // Assert
            CollectionAssert.AreEqual(expectedBaseDefinitions, actualBaseDefinitions);
        }

        [TestMethod]
        public void GetEventBaseDefinitions_EventHasTheNewModifier_ReturnEmptyEnumerable()
        {
            // Arrange
            var testedEvent = GetEvent(typeof(Inheritdoc1), nameof(Inheritdoc1.Event1ForNewModifier));
            var expectedBaseDefinitions = new PropertyInfo[]
            {
            };

            // Act
            var actualBaseDefinitions = testedEvent.GetEventBaseDefinitions().ToArray();

            // Assert
            CollectionAssert.AreEqual(expectedBaseDefinitions, actualBaseDefinitions);
        }

        [TestMethod]
        public void GetEventBaseDefinitions_EventIsOverrideOfEventWithTheNewModifier_ReturnCorrectBaseDefinitions()
        {
            // Arrange
            var testedEvent = GetEvent(typeof(Inheritdoc1), nameof(Inheritdoc1.Event2ForNewModifier));
            var expectedBaseDefinitions = new EventInfo[]
            {
                GetEvent(typeof(InheritDocBase2), nameof(InheritDocBase2.Event2ForNewModifier)),
                GetEvent(typeof(InheritDocBase1), nameof(InheritDocBase1.Event2ForNewModifier)),
                GetEvent(typeof(IInheritdoc3), nameof(IInheritdoc3.Event2ForNewModifier)),
            };

            // Act
            var actualBaseDefinitions = testedEvent.GetEventBaseDefinitions().ToArray();

            // Assert
            CollectionAssert.AreEqual(expectedBaseDefinitions, actualBaseDefinitions);
        }

        [TestMethod]
        public void GetEventBaseDefinitions_EventLooksTheSameAsBasePrivateEvent_ReturnEmptyEnumerable()
        {
            // Arrange
            var testedEvent = GetEvent(typeof(Inheritdoc1), "PrivateEvent");
            var expectedBaseDefinitions = new EventInfo[]
            {
            };

            // Act
            var actualBaseDefinitions = testedEvent.GetEventBaseDefinitions().ToArray();

            // Assert
            CollectionAssert.AreEqual(expectedBaseDefinitions, actualBaseDefinitions);
        }

        [TestMethod]
        public void GetEventBaseDefinitions_EventIsExplicitInterfaceImplementation_ReturnCorrectBaseDefinitions()
        {
            // Arrange
            var interfaceType = typeof(IInheritdoc2);
            var testedEvent = GetEvent(typeof(InheritDocBase1), $"{interfaceType.FullName}.{nameof(IInheritdoc2.EventForExplicitInterfaceImplementation)}");
            var expectedBaseDefinitions = new EventInfo[]
            {
                GetEvent(interfaceType, nameof(IInheritdoc2.EventForExplicitInterfaceImplementation)),
            };

            // Act
            var actualBaseDefinitions = testedEvent.GetEventBaseDefinitions().ToArray();

            // Assert
            CollectionAssert.AreEqual(expectedBaseDefinitions, actualBaseDefinitions);
        }

        #endregion GetEventBaseDefinitions Tests

        private static ConstructorInfo GetConstructor(Type type, params Type[] parametersTypes)
        {
            return type.GetConstructor(bindingFlags, parametersTypes);
        }

        private static MethodInfo GetMethod(Type type, string methodName)
        {
            return type.GetMethod(methodName, bindingFlags);
        }

        private static PropertyInfo GetProperty(Type type, string propertyName)
        {
            return type.GetProperty(propertyName, bindingFlags);
        }

        private static EventInfo GetEvent(Type type, string eventName)
        {
            return type.GetEvent(eventName, bindingFlags);
        }
    }
}
