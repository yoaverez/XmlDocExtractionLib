
namespace XmlDocExtractionLib.Tests.DummyTypes
{
    public class SubDummyClass : AbstractDummyClass
    {
        /// <summary>
        /// VirtualProp1 docs.
        /// </summary>
        public override int VirtualProp1 { get => base.VirtualProp1; set => base.VirtualProp1 = value; }

        /// <summary>
        /// VirtualEvent1 docs.
        /// </summary>
        public override event Action VirtualEvent1;

        /// <summary>
        /// VirtualMethod1 docs.
        /// </summary>
        public override void VirtualMethod1()
        {
            base.VirtualMethod1();
        }

        /// <summary>
        /// Summary 1.
        /// </summary>
        /// <param name="a">a 1</param>
        /// <summary>
        /// Summary 2.
        /// </summary>
        /// <remarks>Remarks 1.</remarks>
        /// <param name="a">a 2</param>
        /// <remarks>Remarks 2.</remarks>
        /// <typeparam name="T">typeParam 1</typeparam>
        /// <returns>returns 1</returns>
        /// <typeparam name="T">typeParam 2</typeparam>
        /// <returns>returns 2</returns>
        /// <exception cref="Exception">Exception1.</exception>
        /// <example>Example 1</example>
        /// <exception cref="Exception">Exception2.</exception>
        /// <example>Example 2</example>
        public int MethodWithDuplicatesDocs<T>(int a)
        {
            return 0;
        }

        /// <summary>
        /// Summary 1.
        /// </summary>
        /// <param name="a">a 1</param>
        /// <remarks>Remarks 1.</remarks>
        /// <typeparam name="T">typeParam 1</typeparam>
        /// <returns>returns 1</returns>
        /// <exception cref="Exception">Exception1.</exception>
        /// <example>Example 1</example>
        public int MethodWithoutDuplicatesDocs<T>(int a)
        {
            return 0;
        }

        /// <value>Value 1</value>
        /// <value>Value 2</value>
        public int PropWithDuplicatesDocs { get; set; }

        /// <value>Value 1</value>
        public int PropWithoutDuplicatesDocs { get; set; }

        public int PropWithoutDocs {  get; set; }
    }
}
