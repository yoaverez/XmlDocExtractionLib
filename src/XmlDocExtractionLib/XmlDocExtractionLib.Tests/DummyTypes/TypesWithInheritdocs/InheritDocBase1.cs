namespace XmlDocExtractionLib.Tests.DummyTypes.TypesWithInheritdocs
{
    /// <summary>
    /// InheritDocBase1 docs.
    /// </summary>
    /// <remarks>InheritDocBase1 remarks.</remarks>
    public abstract class InheritDocBase1 : IInheritdoc3
    {
        /// <summary>
        /// PropWithGetterOnly InheritDocBase1 docs.
        /// </summary>
        public abstract int PropWithGetterOnly { get; set; }

        /// <summary>
        /// PropWithSetterOnly InheritDocBase1 docs.
        /// </summary>
        public abstract int PropWithSetterOnly { get; set; }

        /// <summary>
        /// Prop2ForNewModifier InheritDocBase1 docs.
        /// </summary>
        public abstract int Prop2ForNewModifier { get; }

        /// <inheritdoc/>
        int IInheritdoc2.PropForExplicitInterfaceImplementation { get; }

        /// <summary>
        /// Event2ForNewModifier InheritDocBase1 docs.
        /// </summary>
        public abstract event Action<int> Event2ForNewModifier;

        /// <inheritdoc/>
        event Action<int> IInheritdoc2.EventForExplicitInterfaceImplementation { add { } remove { } }

        /// <summary>
        /// InheritDocBase1 ctor1 docs.
        /// </summary>
        public InheritDocBase1(int x)
        {

        }

        /// <summary>
        /// InheritDocBase1 ctor2 docs.
        /// </summary>
        private InheritDocBase1(long x) { }

        /// <inheritdoc/>
        public abstract void Method1();

        /// <summary>
        /// Method2 docs.
        /// <inheritdoc cref="Method1"/>
        /// </summary>
        public virtual void Method2()
        {

        }

        /// <summary>
        /// Method3 docs.
        /// </summary>
        /// <param name="a">aaa</param>
        public virtual void Method3(string a)
        {

        }

        /// <summary>
        /// Method1ForNewModifier InheritDocBase1 docs.
        /// </summary>
        public void Method1ForNewModifier()
        {

        }

        public abstract void Method2ForNewModifier();

        public override string ToString()
        {
            return base.ToString();
        }

        /// <inheritdoc/>
        void IInheritdoc2.MethodForExplicitInterfaceImplementation()
        {

        }

        /// <summary>
        /// aaaa
        /// </summary>
        /// <param name="a">a</param>
        /// <param name="b">b</param>
        public virtual void MethodWithParameters(int a, bool b)
        {

        }
    }
}
