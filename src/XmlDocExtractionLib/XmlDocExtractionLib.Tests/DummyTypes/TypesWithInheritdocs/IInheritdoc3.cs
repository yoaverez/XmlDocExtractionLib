namespace XmlDocExtractionLib.Tests.DummyTypes.TypesWithInheritdocs
{
    /// <inheritdoc/>
    public interface IInheritdoc3 : IInheritdoc1, IInheritdoc2
    {
        /// <summary>
        /// Prop2ForNewModifier IInheritdoc3 docs.
        /// </summary>
        new int Prop2ForNewModifier { get; }

        /// <summary>
        /// Event2ForNewModifier IInheritdoc3 docs.
        /// </summary>
        new event Action<int> Event2ForNewModifier;

        /// <summary>
        /// Method2ForNewModifier IInheritdoc3 docs.
        /// </summary>
        new void Method2ForNewModifier();
    }
}
