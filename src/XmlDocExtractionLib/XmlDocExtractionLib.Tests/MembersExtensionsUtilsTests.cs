using Microsoft.VisualStudio.TestTools.UnitTesting;
using XmlDocExtractionLib.Tests.DummyTypes;
using XmlDocExtractionLib.Tests.DummyTypes.TypesWithInheritdocs;

namespace XmlDocExtractionLib.Tests
{
    [TestClass]
    public class MembersExtensionsUtilsTests
    {
        #region GetPropertyInfoFromAccessor Tests

        [TestMethod]
        public void GetPropertyInfoFromAccessor_PropertyHasOnlyAGetter_ReturnCorrectProperty()
        {
            // Arrange
            var expectedProp = typeof(IInheritdoc2).GetProperty(nameof(IInheritdoc2.PropWithGetterOnly));
            var accessor = expectedProp.GetGetMethod(true);

            // Act
            var actualProp = accessor.GetPropertyInfoFromAccessor();

            // Assert
            Assert.AreEqual(expectedProp, actualProp);
        }

        [TestMethod]
        public void GetPropertyInfoFromAccessor_PropertyHasOnlyASetter_ReturnCorrectProperty()
        {
            // Arrange
            var expectedProp = typeof(IInheritdoc2).GetProperty(nameof(IInheritdoc2.PropWithSetterOnly));
            var accessor = expectedProp.GetSetMethod(true);

            // Act
            var actualProp = accessor.GetPropertyInfoFromAccessor();

            // Assert
            Assert.AreEqual(expectedProp, actualProp);
        }

        [TestMethod]
        public void GetPropertyInfoFromAccessor_PropertyHasBothAccessors_ReturnCorrectProperty()
        {
            // Arrange
            var expectedProp = typeof(Inheritdoc1).GetProperty(nameof(Inheritdoc1.SelfDeclaredProp));
            var accessor1 = expectedProp.GetGetMethod(true);
            var accessor2 = expectedProp.GetSetMethod(true);

            // Act
            var actualProp1 = accessor1.GetPropertyInfoFromAccessor();
            var actualProp2 = accessor2.GetPropertyInfoFromAccessor();

            // Assert
            Assert.AreEqual(expectedProp, actualProp1);
            Assert.AreEqual(expectedProp, actualProp2);
        }

        [TestMethod]
        public void GetPropertyInfoFromAccessor_AccessorIsNotOfAProperty_ThrowsArgumentException()
        {
            // Arrange
            var accessor = typeof(Inheritdoc1).GetMethod(nameof(Inheritdoc1.Method1));

            // Act + Assert
            Assert.ThrowsException<ArgumentException>(() => accessor.GetPropertyInfoFromAccessor());
        }

        #endregion GetPropertyInfoFromAccessor Tests

        #region GetEventInfoFromAccessor Tests

        [TestMethod]
        public void GetEventInfoFromAccessor_EventHasBothAccessors_ReturnCorrectEvent()
        {
            // Arrange
            var expectedEvent = typeof(AbstractDummyClass).GetEvent(nameof(AbstractDummyClass.Event1));
            var accessor1 = expectedEvent.GetAddMethod(true);
            var accessor2 = expectedEvent.GetRemoveMethod(true);

            // Act
            var actualProp1 = accessor1.GetEventInfoFromAccessor();
            var actualProp2 = accessor2.GetEventInfoFromAccessor();

            // Assert
            Assert.AreEqual(expectedEvent, actualProp1);
            Assert.AreEqual(expectedEvent, actualProp2);
        }

        [TestMethod]
        public void GetEventInfoFromAccessor_AccessorIsNotOfAnEvent_ThrowsArgumentException()
        {
            // Arrange
            var accessor = typeof(Inheritdoc1).GetMethod(nameof(Inheritdoc1.Method1));

            // Act + Assert
            Assert.ThrowsException<ArgumentException>(() => accessor.GetEventInfoFromAccessor());
        }

        #endregion GetEventInfoFromAccessor Tests
    }
}
