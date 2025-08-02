namespace XmlDocExtractionLib.Tests.DummyTypes
{
    /// <summary>
    /// IDummyInterface docs.
    /// </summary>
    public interface IDummyInterface
    {
        /// <summary>
        /// Prop001 docs.
        /// </summary>
        int Prop001 { get; }

        /// <summary>
        /// Prop002 docs.
        /// </summary>
        int Prop002 { get; }

        /// <summary>
        /// Event001 docs.
        /// </summary>
        event Action<int> Event001;

        /// <summary>
        /// Event002 docs.
        /// </summary>
        event Action<int> Event002;

        /// <summary>
        /// Method001 docs.
        /// </summary>
        void Method001<T>(T[,][] a, List<string> b);

        /// <summary>
        /// Method002 docs.
        /// </summary>
        void Method002<T>(int a, bool b);
    }
}
