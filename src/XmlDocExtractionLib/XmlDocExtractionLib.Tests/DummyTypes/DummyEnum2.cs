namespace XmlDocExtractionLib.Tests.DummyTypes
{
    public enum DummyEnum2
    {
        /// <summary>
        /// Summary 1.
        /// </summary>
        /// <remarks>Remarks 1</remarks>
        /// <summary>
        /// Summary 2.
        /// </summary>
        /// <remarks>Remarks 2</remarks>
        ValueWithDuplicateDocs,

        /// <summary>
        /// Summary 1.
        /// </summary>
        /// <remarks>Remarks 1</remarks>
        ValueWithoutDuplicateDocs,

        ValueWithoutDocs,
    }
}
