namespace XmlDocExtractionLib.Tests.DummyTypes
{
    /// <summary>
    /// DummyClass docs.
    /// </summary>
    public class DummyClass
    {
        /// <summary>
        /// field1 docs.
        /// </summary>
        public int field1;

        /// <summary>
        /// Prop1 docs.
        /// </summary>
        public int Prop1 { get; set; }

        /// <summary>
        /// Event1 docs.
        /// </summary>
        public event Action<int> Event1;

        /// <summary>
        /// Method1 docs.
        /// </summary>
        public unsafe int Method1(int*[][,,][][,,] a) { return 1; }
    }
}
