namespace XmlDocExtractionLib.Tests.DummyTypes.TypesWithInheritdocs
{
    /// <summary>
    /// IInheritdoc2 docs.
    /// </summary>
    /// <remarks>IInheritdoc2 remarks</remarks>
    public interface IInheritdoc2
    {
        /// <summary>
        /// PropWithGetterOnly docs.
        /// </summary>
        int PropWithGetterOnly { get; }

        /// <summary>
        /// PropWithSetterOnly docs.
        /// </summary>
        int PropWithSetterOnly { set; }

        /// <summary>
        /// Prop2ForNewModifier docs.
        /// </summary>
        int Prop2ForNewModifier { get; }

        /// <summary>
        /// PropForExplicitInterfaceImplementation docs.
        /// </summary>
        int PropForExplicitInterfaceImplementation { get; }

        /// <summary>
        /// Event2ForNewModifier docs.
        /// </summary>
        event Action<int> Event2ForNewModifier;

        /// <summary>
        /// EventForExplicitInterfaceImplementation docs.
        /// </summary>
        event Action<int> EventForExplicitInterfaceImplementation;

        /// <summary>
        /// Method1 of IInheritdoc2 docs.
        /// </summary>
        void Method1();

        /// <summary>
        /// Method1ForNewModifier docs.
        /// </summary>
        void Method1ForNewModifier();

        /// <summary>
        /// Method2ForNewModifier docs.
        /// </summary>
        void Method2ForNewModifier();

        /// <summary>
        /// MethodForExplicitInterfaceImplementation docs.
        /// </summary>
        void MethodForExplicitInterfaceImplementation();
    }
}
