namespace XmlDocExtractionLib.Tests.DummyTypes
{
    public abstract class AbstractDummyClass
    {
        /// <summary>
        /// Prop1 docs.
        /// </summary>
        public int Prop1 {  get; set; }

        public virtual int VirtualProp1 { get; set; }

        /// <summary>
        /// Event1 docs.
        /// </summary>
        public event Action Event1;

        public virtual event Action VirtualEvent1;

        /// <summary>
        /// Method1 docs.
        /// </summary>
        public void Method1() { }

        public virtual void VirtualMethod1() { }
    }
}
