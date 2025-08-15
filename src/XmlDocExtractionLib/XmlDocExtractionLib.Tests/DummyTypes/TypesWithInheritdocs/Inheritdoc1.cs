namespace XmlDocExtractionLib.Tests.DummyTypes.TypesWithInheritdocs
{
    /// <inheritdoc/>
    public class Inheritdoc1 : InheritDocBase2
    {
        /// <inheritdoc/>
        public Inheritdoc1(int x) : base(x)
        {
        }

        /// <inheritdoc/>
        protected Inheritdoc1(string x) : base(0)
        {

        }

        /// <inheritdoc/>
        public Inheritdoc1(char x) : base(0)
        {

        }

        /// <inheritdoc/>
        private Inheritdoc1(bool x) : base(0)
        {

        }

        /// <inheritdoc/>
        private Inheritdoc1(long x) : base(0)
        {

        }

        /// <inheritdoc/>
        protected override event Action<int> OnNumber;

        /// <inheritdoc/>
        protected override int Prop { get; set; }

        public int SelfDeclaredProp { get; set; }

        /// <inheritdoc/>
        public override int PropWithGetterOnly { get; set; }

        /// <inheritdoc/>
        public override int PropWithSetterOnly { get; set; }

        /// <inheritdoc/>
        public new int Prop1ForNewModifier { get; set; }

        /// <inheritdoc/>
        public override int Prop2ForNewModifier { get; }

        /// <inheritdoc/>
        private int PrivateProp { get; }

        public event Action<int> SelfDeclaredEvent;

        public event Action<int> Event1ForNewModifier;

        /// <inheritdoc/>
        public override event Action<int> Event2ForNewModifier;

        /// <inheritdoc/>
        private event Action<int> PrivateEvent;

        /// <inheritdoc/>
        public override void Method1()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public override void Method2()
        {
            base.Method2();
        }

        public void SelfDeclaredMethod()
        {

        }

        /// <inheritdoc/>
        public new void Method1ForNewModifier()
        {

        }

        public override void Method2ForNewModifier()
        {
            throw new NotImplementedException();
        }

        public override string ToString()
        {
            return base.ToString();
        }

        private void BasePrivateMethod()
        {

        }
    }
}
