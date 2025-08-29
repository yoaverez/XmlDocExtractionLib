namespace XmlDocExtractionLib.Tests.DummyTypes
{
    /// <summary>
    /// IDummyGenericInterface docs.
    /// </summary>
    public interface IDummyGenericInterface<T>
    {
        /// <summary>
        /// Prop0001 docs.
        /// </summary>
        public T Prop0001 { get; }

        /// <summary>
        /// Event0001 docs.
        /// </summary>
        public event Action<T> Event0001;

        /// <summary>
        /// Method0001 docs.
        /// </summary>
        unsafe void Method0001(T[,][] a, List<string> b, int***[][] c);
    }
}
