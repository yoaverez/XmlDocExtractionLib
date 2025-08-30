using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Xml.Linq;

namespace XmlDocExtractionLib.Tests
{
    [TestClass]
    public class XmlUtilsTests
    {
        #region RemoveElementWithNextWhitespace Tests

        [TestMethod]
        public void RemoveElementWithNextWhitespace_NextNodeIsATextNodeWithOnlyWhitespaces_BothElementAndTextNodeAreRemoved()
        {
            // Arrange
            var originalXmlStr = $"<member> hey there <remove/> \n \r\n<anotherElement /></member>";
            var expectedXmlStr = $"<member> hey there <anotherElement /></member>";
            var originalXml = XElement.Parse(originalXmlStr, LoadOptions.PreserveWhitespace);
            var testedXml = new XElement(originalXml);
            var testedElement = testedXml.Element("remove");

            // Act
            testedElement.RemoveElementWithNextWhitespace();

            // Assert
            Assert.AreEqual(expectedXmlStr, testedXml.ToString());
        }

        [TestMethod]
        public void RemoveElementWithNextWhitespace_NextNodeIsATextNodeWithSomeWhitespaces_ElementIsRemovedAndTextNodeIsTrimmedFromStart()
        {
            // Arrange
            var originalXmlStr = $"<member> hey there <remove/> \n text\r\n<anotherElement /></member>";
            var expectedXmlStr = $"<member> hey there text\r\n<anotherElement /></member>";
            var originalXml = XElement.Parse(originalXmlStr, LoadOptions.PreserveWhitespace);
            var testedXml = new XElement(originalXml);
            var testedElement = testedXml.Element("remove");

            // Act
            testedElement.RemoveElementWithNextWhitespace();

            // Assert
            Assert.AreEqual(expectedXmlStr, testedXml.ToString());
        }

        [TestMethod]
        public void RemoveElementWithNextWhitespace_NextNodeIsANotATextNode_ElementIsRemovedAndNextElementIsUnChanged()
        {
            // Arrange
            var originalXmlStr = $"<member> hey there <remove/><anotherElement /></member>";
            var expectedXmlStr = $"<member> hey there <anotherElement /></member>";
            var originalXml = XElement.Parse(originalXmlStr, LoadOptions.PreserveWhitespace);
            var testedXml = new XElement(originalXml);
            var testedElement = testedXml.Element("remove");

            // Act
            testedElement.RemoveElementWithNextWhitespace();

            // Assert
            Assert.AreEqual(expectedXmlStr, testedXml.ToString());
        }

        [TestMethod]
        public void RemoveElementWithNextWhitespace_NextNodeIsNull_InvalidOperationExceptionIsThrown()
        {
            // Arrange
            var originalXmlStr = $"<member> hey there <remove/><anotherElement /></member>";
            var expectedXmlStr = $"";
            var originalXml = XElement.Parse(originalXmlStr, LoadOptions.PreserveWhitespace);
            var testedXml = new XElement(originalXml);
            var testedElement = testedXml;

            // Act + Assert
            Assert.ThrowsException<InvalidOperationException>(testedElement.RemoveElementWithNextWhitespace);
        }

        #endregion RemoveElementWithNextWhitespace Tests

        #region ReplaceElementAndRemovePrevAndNextWhitespace Tests

        [TestMethod]
        public void ReplaceElementAndRemovePrevAndNextWhitespace_XPathEvaluationIsASingleTextNode_TrimWhitespacesFromTextNode()
        {
            // Arrange
            var originalXmlStr = $"<member> hey there <replace/> </member>";
            var expectedXmlStr = $"<member> hey there replacement string </member>";
            var originalXml = XElement.Parse(originalXmlStr, LoadOptions.PreserveWhitespace);
            var testedXml = new XElement(originalXml);
            var testedElement = testedXml.Element("replace");

            // Act
            testedElement.ReplaceElementAndRemovePrevAndNextWhitespace(new List<XNode> { new XText("    replacement string ") });

            // Assert
            Assert.AreEqual(expectedXmlStr, testedXml.ToString());
        }

        [TestMethod]
        public void ReplaceElementAndRemovePrevAndNextWhitespace_XPathEvaluationHasMultipleTextNodes_TrimWhitespacesFromStartOfFirstTextNodeAndFromTheEndOfTheLastNode()
        {
            // Arrange
            var originalXmlStr = $"<member> hey there <replace/> </member>";
            var expectedXmlStr = $"<member> hey there replacement string  hello </member>";
            var originalXml = XElement.Parse(originalXmlStr, LoadOptions.PreserveWhitespace);
            var testedXml = new XElement(originalXml);
            var testedElement = testedXml.Element("replace");

            // Act
            testedElement.ReplaceElementAndRemovePrevAndNextWhitespace(new List<XNode> { new XText("    replacement string "), new XText(" hello ") });

            // Assert
            Assert.AreEqual(expectedXmlStr, testedXml.ToString());
        }

        [TestMethod]
        public void ReplaceElementAndRemovePrevAndNextWhitespace_XPathEvaluationIsASingleElementNodes_DoesNotTrimWhitespacesAndElementIsReplacedCorrectly()
        {
            // Arrange
            var originalXmlStr = $"<member> hey there <replace/> </member>";
            var expectedXmlStr = $"<member> hey there <newElement /> hello </member>";
            var originalXml = XElement.Parse(originalXmlStr, LoadOptions.PreserveWhitespace);
            var testedXml = new XElement(originalXml);
            var testedElement = testedXml.Element("replace");

            // Act
            testedElement.ReplaceElementAndRemovePrevAndNextWhitespace(new List<XNode> { new XElement("newElement"), new XText(" hello ") });

            // Assert
            Assert.AreEqual(expectedXmlStr, testedXml.ToString());
        }

        [TestMethod]
        public void ReplaceElementAndRemovePrevAndNextWhitespace_XPathEvaluationIsASingleTextNodeWithOnlyWhitespaces_ElementIsRemovedWithTrailingWhitespaces()
        {
            // Arrange
            var originalXmlStr = $"<member> hey there <replace/> </member>";
            var expectedXmlStr = $"<member> hey there </member>";
            var originalXml = XElement.Parse(originalXmlStr, LoadOptions.PreserveWhitespace);
            var testedXml = new XElement(originalXml);
            var testedElement = testedXml.Element("replace");

            // Act
            testedElement.ReplaceElementAndRemovePrevAndNextWhitespace(new List<XNode> { new XText("  \n \r\n") });

            // Assert
            Assert.AreEqual(expectedXmlStr, testedXml.ToString());
        }

        [TestMethod]
        public void ReplaceElementAndRemovePrevAndNextWhitespace_XPathEvaluationIsEmptyListOfXnodes_ElementIsRemovedWithTrailingWhitespaces()
        {
            // Arrange
            var originalXmlStr = $"<member> hey there <replace/> </member>";
            var expectedXmlStr = $"<member> hey there </member>";
            var originalXml = XElement.Parse(originalXmlStr, LoadOptions.PreserveWhitespace);
            var testedXml = new XElement(originalXml);
            var testedElement = testedXml.Element("replace");

            // Act
            testedElement.ReplaceElementAndRemovePrevAndNextWhitespace(new List<XNode> { });

            // Assert
            Assert.AreEqual(expectedXmlStr, testedXml.ToString());
        }

        [TestMethod]
        public void ReplaceElementAndRemovePrevAndNextWhitespace_XPathEvaluationIsNotAListOfXnodes_ElementIsReplacedWithoutRemovingTrailingWhitespaces()
        {
            // Arrange
            var originalXmlStr = $"<member> hey there <replace/> </member>";
            var expectedXmlStr = $"<member> hey there  hello  </member>";
            var originalXml = XElement.Parse(originalXmlStr, LoadOptions.PreserveWhitespace);
            var testedXml = new XElement(originalXml);
            var testedElement = testedXml.Element("replace");

            // Act
            testedElement.ReplaceElementAndRemovePrevAndNextWhitespace(" hello ");

            // Assert
            Assert.AreEqual(expectedXmlStr, testedXml.ToString());
        }

        #endregion ReplaceElementAndRemovePrevAndNextWhitespace Tests
    }
}
