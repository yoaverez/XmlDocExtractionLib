using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace XmlDocExtractionLib
{
    /// <summary>
    /// Extension methods for getting members base definitions extensions.
    /// </summary>
    public static class MembersBaseDefinitionsExtensions
    {
        /// <summary>
        /// Binding flags for finding members.
        /// </summary>
        private static BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly;

        /// <summary>
        /// Get the base definitions of the given <paramref name="type"/>.
        /// </summary>
        /// <param name="type">The type whose base definitions are requested.</param>
        /// <returns>The base definitions of the given <paramref name="type"/>.</returns>
        public static IEnumerable<Type> GetTypeBaseDefinitions(this Type type)
        {
            // If the type is not an interface,
            // return all it base types.
            if (!type.IsInterface)
            {
                var baseType = type.BaseType;
                while (baseType != null)
                {
                    yield return baseType;
                    baseType = baseType.BaseType;
                }
            }

            // The given type is an interface,
            // return all it implemented interfaces.
            else
            {
                foreach (var implementedInterface in type.GetInterfaces())
                {
                    yield return implementedInterface;
                }
            }
        }

        /// <summary>
        /// Get the base definitions of the given <paramref name="constructor"/>.
        /// </summary>
        /// <param name="constructor">The ctor whose base definitions are requested.</param>
        /// <returns>The base definitions of the given <paramref name="constructor"/>.</returns>
        public static IEnumerable<ConstructorInfo> GetConstructorBaseDefinitions(this ConstructorInfo constructor)
        {
            var ctorParams = constructor.GetParameters().Select(x => x.ParameterType).ToArray();

            var baseType = constructor.ReflectedType.BaseType;
            while (baseType != null)
            {
                var ctor = baseType?.GetConstructor(bindingFlags, null, ctorParams, null);
                if (ctor != null)
                {
                    yield return ctor;
                }
                else
                {
                    // If any base type in the chain
                    // doesn't have the ctor than the other
                    // base type ctors won't be remarked as
                    // base definitions of the given ctor.
                    break;
                }
                baseType = baseType.BaseType;
            }
        }

        /// <summary>
        /// Get the base definitions of the given <paramref name="method"/>.
        /// </summary>
        /// <param name="method">The method whose base definitions are requested.</param>
        /// <returns>The base definitions of the given <paramref name="method"/>.</returns>
        public static IEnumerable<MethodInfo> GetMethodBaseDefinitions(this MethodInfo method)
        {
            var reflectedType = method.ReflectedType;

            if (!reflectedType.IsInterface)
            {
                // Method is override (without new modifier and no self proclaimed).
                if (!method.Equals(method.GetBaseDefinition()))
                {
                    var methodParams = method.GetParameters().Select(x => x.ParameterType).ToArray();
                    var currentBase = reflectedType.BaseType;
                    while (currentBase != null)
                    {
                        var candidateMethod = currentBase.GetMethod(method.Name, bindingFlags, null, methodParams, null);

                        if (candidateMethod != null)
                            yield return candidateMethod;

                        currentBase = currentBase.BaseType;
                    }
                }
            }

            // Check for first interface that defines the method.
            var isInterfaceMethodFound = false;
            foreach (var implementedInterface in reflectedType.GetInterfaces())
            {
                var map = reflectedType.GetInterfaceMap(implementedInterface);
                for (int i = 0; i < map.TargetMethods.Length; i++)
                {
                    if (map.TargetMethods[i].Equals(method))
                    {
                        yield return map.InterfaceMethods[i];

                        isInterfaceMethodFound = true;

                        // We found a method so there is no reason
                        // to keep searching in this interface
                        // and in other interfaces.
                        break;
                    }
                }

                // If an interface method was found,
                // there is no need to check other interfaces.
                if (isInterfaceMethodFound)
                    break;
            }
        }

        /// <summary>
        /// Get the base definitions of the given <paramref name="property"/>.
        /// </summary>
        /// <param name="property">The property whose base definitions are requested.</param>
        /// <returns>The base definitions of the given <paramref name="property"/>.</returns>
        public static IEnumerable<PropertyInfo> GetPropertyBaseDefinitions(this PropertyInfo property)
        {
            var getter = property.GetGetMethod(true);
            var setter = property.GetSetMethod(true);

            return GetMultiAccessorMemberBaseDefintions(getter, setter, (accessor) => accessor.GetPropertyInfoFromAccessor());
        }

        /// <summary>
        /// Get the base definitions of the given <paramref name="eventInfo"/>.
        /// </summary>
        /// <param name="eventInfo">The event whose base definitions are requested.</param>
        /// <returns>The base definitions of the given <paramref name="eventInfo"/>.</returns>
        public static IEnumerable<EventInfo> GetEventBaseDefinitions(this EventInfo eventInfo)
        {
            var adder = eventInfo.GetAddMethod(true);
            var remover = eventInfo.GetRemoveMethod(true);

            return GetMultiAccessorMemberBaseDefintions(adder, remover, (accessor) => accessor.GetEventInfoFromAccessor());
        }

        /// <summary>
        /// Get a base definition of member with multiple accessors
        /// such as a property or an event.
        /// </summary>
        /// <typeparam name="T">The type of the member e.g. <see cref="PropertyInfo"/>.</typeparam>
        /// <param name="accessor1">The first accessor of the member.</param>
        /// <param name="accessor2">The second accessor of the member.</param>
        /// <param name="memberGetter">A method that finds the correct member from its accessor.</param>
        /// <returns>
        /// An enumerable of all the base definitions of the of the member
        /// with the given accessors.
        /// </returns>
        private static IEnumerable<T> GetMultiAccessorMemberBaseDefintions<T>(MethodInfo accessor1, MethodInfo accessor2, Func<MethodInfo, T> memberGetter) where T : MemberInfo
        {
            var accessor1BaseDefinitions = accessor1?.GetMethodBaseDefinitions() ?? Array.Empty<MethodInfo>();

            var accessor2BaseDefinitions = accessor2?.GetMethodBaseDefinitions() ?? Array.Empty<MethodInfo>();

            var iterator1 = accessor1BaseDefinitions.GetEnumerator();
            var iterator2 = accessor2BaseDefinitions.GetEnumerator();

            var isIterator1StillRunning = iterator1.MoveNext();
            var isIterator2StillRunning = iterator2.MoveNext();

            // Go until any iterator finished.
            while (isIterator1StillRunning && isIterator2StillRunning)
            {
                yield return memberGetter(iterator1.Current);

                isIterator1StillRunning = iterator1.MoveNext();
                isIterator2StillRunning = iterator2.MoveNext();
            }

            // If iterator 2 was finished,
            // run iterator 1 until it finishes.
            if (isIterator1StillRunning)
            {
                do
                {
                    yield return memberGetter(iterator1.Current);
                }
                while (iterator1.MoveNext());
            }

            // If iterator 1 was finished,
            // run iterator 2 until it finishes.
            if (isIterator2StillRunning)
            {
                do
                {
                    yield return memberGetter(iterator2.Current);
                }
                while (iterator2.MoveNext());
            }
        }
    }
}
