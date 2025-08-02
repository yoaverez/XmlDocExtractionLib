namespace XmlDocExtractionLib.Tests.DummyTypes
{
    /// <summary>
    /// DummyGenericType docs.
    /// </summary>
    public class DummyGenericType<T, S> : IDummyInterface, IDummyGenericInterface<S>
    {
        /// <summary>
        /// fieldA docs.
        /// </summary>
        public T fieldA;

        /// <summary>
        /// fieldB docs.
        /// </summary>
        public S fieldB;

        /// <summary>
        /// PropA docs.
        /// </summary>
        public T PropA { get; }

        /// <summary>
        /// PropB docs.
        /// </summary>
        public S PropB { get;  set; }

        /// <summary>
        /// EventA docs.
        /// </summary>
        public event Action<S> EventA;

        /// <summary>
        /// Explicit interface implementation of Event001 docs.
        /// </summary>
        event Action<int> IDummyInterface.Event001 { add { } remove { } }

        /// <summary>
        /// Implicit interface implementation of Event002 docs.
        /// </summary>
        public event Action<int> Event002;

        /// <summary>
        /// Explicit interface implementation of Event0001 docs.
        /// </summary>
        event Action<S> IDummyGenericInterface<S>.Event0001 { add { } remove { } }

        /// <summary>
        /// Explicit interface implementation of Prop001 docs.
        /// </summary>
        int IDummyInterface.Prop001 => throw new NotImplementedException();

        /// <summary>
        /// Implicit interface implementation of Prop002 docs.
        /// </summary>
        public int Prop002 => throw new NotImplementedException();

        /// <summary>
        /// Explicit interface implementation of Prop0001 docs.
        /// </summary>
        S IDummyGenericInterface<S>.Prop0001 => throw new NotImplementedException();

        /// <summary>
        /// MethodA docs.
        /// </summary>
        public void MethodA<G>(T[,][] a, List<S> b, Dictionary<T, G> c) { }

        /// <summary>
        /// Explicit interface implementation of Method001 docs.
        /// </summary>
        void IDummyInterface.Method001<T1>(T1[,][] a, List<string> b)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Explicit interface implementation of Method0001 docs.
        /// </summary>
        unsafe void IDummyGenericInterface<S>.Method0001(S[,][] a, List<string> b, int***[][] c)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Implicit interface implementation of Method002 docs.
        /// </summary>
        public void Method002<T1>(int a, bool b)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// DummyGenericNestedType docs.
        /// </summary>
        public class DummyGenericNestedType<R>
        {
            /// <summary>
            /// field01 docs.
            /// </summary>
            public R field01;

            /// <summary>
            /// Prop01 docs.
            /// </summary>
            public R Prop01 { get; set; }

            /// <summary>
            /// Event01 docs.
            /// </summary>
            public event Action<R> Event01;

            /// <summary>
            /// Method01 docs.
            /// </summary>
            public void Method01<G>(ref T[,][] a, List<S> b, in Dictionary<T, G> c, out int d)
            {
                d = 4;
            }
        }
    }
}
