namespace XmlDocExtractionLib.Tests.DummyTypes.TypesWithInheritdocs
{
    /// <summary>
    /// InheritDocBase2 docs.
    /// </summary>
    public abstract class InheritDocBase2 : InheritDocBase1
    {
        /// <summary>
        /// InheritDocBase2 ctor1 docs.
        /// </summary>
        public InheritDocBase2(int x) : base(x)
        {
        }

        /// <summary>
        /// InheritDocBase2 ctor2 docs.
        /// </summary>
        public InheritDocBase2(string x) : base(0)
        {

        }

        /// <summary>
        /// InheritDocBase2 ctor3 docs.
        /// </summary>
        protected InheritDocBase2(char x) : base(0)
        {

        }

        /// <summary>
        /// InheritDocBase2 ctor4 docs.
        /// </summary>
        private InheritDocBase2(bool x) : base(0)
        {

        }

        /// <summary>
        /// OnNumber docs.
        /// </summary>
        protected virtual event Action<int> OnNumber;

        /// <summary>
        /// Prop docs.
        /// </summary>
        protected virtual int Prop { get; set; }

        /// <summary>
        /// PropWithGetterOnly InheritDocBase2 docs.
        /// </summary>
        public override int PropWithGetterOnly { get; set; }

        /// <summary>
        /// PropWithSetterOnly InheritDocBase2 docs.
        /// </summary>
        public override int PropWithSetterOnly { get; set; }

        /// <summary>
        /// Prop1ForNewModifier docs.
        /// </summary>
        public int Prop1ForNewModifier { get; }

        /// <summary>
        /// Prop2ForNewModifier InheritDocBase2 docs.
        /// </summary>
        public override int Prop2ForNewModifier { get; }

        /// <summary>
        /// PrivateProp docs.
        /// </summary>
        private int PrivateProp { get; }

        /// <summary>
        /// Event2ForNewModifier InheritDocBase2 docs.
        /// </summary>
        public override event Action<int> Event2ForNewModifier;

        /// <summary>
        /// PrivateEvent docs.
        /// </summary>
        private event Action<int> PrivateEvent;

        /// <inheritdoc/>
        public override void Method1()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Method2 InheritDocBase2 docs.
        /// </summary>
        public override void Method2()
        {
            base.Method2();
        }

        /// <summary>
        /// Method3 InheritDocBase2 docs.
        /// </summary>
        /// <param name="a"><inheritdoc/></param>
        public override void Method3(string a)
        {
            base.Method3(a);
        }

        public override string ToString()
        {
            return base.ToString();
        }

        private void BasePrivateMethod()
        {

        }

        /// <inheritdoc cref="MembersExtensionsUtils.GetEventInfoFromAccessor(System.Reflection.MethodBase)"/>
        public void MethodWithFarInheritdoc(int a)
        {

        }

        /// <summary>
        /// MethodWithParameters InheritDocBase2 docs.
        /// </summary>
        /// <param name="a"><inheritdoc/></param>
        /// <param name="b"></param>
        public override void MethodWithParameters(int a, bool b)
        {

        }

        /// <summary>
        /// <b>
        /// <inheritdoc cref="PrivateProp"/>
        /// level 3 docs.
        /// </b>
        /// </summary>
        public int MemberWithCrefLevel3WithoutPath { get; set; }

        /// <summary>
        /// <b>
        /// <inheritdoc cref="PrivateProp" path="/summary/node()"/>
        /// level 3 docs.
        /// </b>
        /// </summary>
        public event Action MemberWithCrefLevel3WithPath;
    }
}
