using System.Reflection;
using System;

namespace XmlDocExtractionLib
{
    /// <summary>
    /// Generic extensions methods for <see cref="MemberInfo"/>.
    /// </summary>
    public static class MembersExtensionsUtils
    {
        /// <summary>
        /// Binding flags for finding members.
        /// </summary>
        private static BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;

        /// <summary>
        /// Retrieve the <see cref="PropertyInfo"/> of the property that the given
        /// <paramref name="declaringInterfaceAccessorMethod"/> belongs to.
        /// </summary>
        /// <param name="declaringInterfaceAccessorMethod">The accessor (get or set) method of the property.</param>
        /// <returns>
        /// The <see cref="PropertyInfo"/> of the property that the given
        /// <paramref name="declaringInterfaceAccessorMethod"/> belongs to.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// Thrown when the given <paramref name="declaringInterfaceAccessorMethod"/>
        /// does not belong to any property.</exception>
        public static PropertyInfo GetPropertyInfoFromAccessor(this MethodBase declaringInterfaceAccessorMethod)
        {
            var declaringInterface = declaringInterfaceAccessorMethod.ReflectedType;

            foreach (var prop in declaringInterface.GetProperties(bindingFlags))
            {
                if (declaringInterfaceAccessorMethod.Equals(prop.GetGetMethod(true))
                   || declaringInterfaceAccessorMethod.Equals(prop.GetSetMethod(true)))
                {
                    return prop;
                }
            }

            throw new ArgumentException($"The given {nameof(declaringInterfaceAccessorMethod)}: {declaringInterfaceAccessorMethod.Name} " +
                                        $"is not a property accessor", nameof(declaringInterfaceAccessorMethod));
        }

        /// <summary>
        /// Retrieve the <see cref="EventInfo"/> of the event that the given
        /// <paramref name="declaringInterfaceAccessorMethod"/> belongs to.
        /// </summary>
        /// <param name="declaringInterfaceAccessorMethod">The accessor (add or remove) method of the event.</param>
        /// <returns>
        /// The <see cref="EventInfo"/> of the event that the given
        /// <paramref name="declaringInterfaceAccessorMethod"/> belongs to.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// Thrown when the given <paramref name="declaringInterfaceAccessorMethod"/>
        /// does not belong to any event.</exception>
        public static EventInfo GetEventInfoFromAccessor(this MethodBase declaringInterfaceAccessorMethod)
        {
            var declaringInterface = declaringInterfaceAccessorMethod.ReflectedType;

            foreach (var eventInfo in declaringInterface.GetEvents(bindingFlags))
            {
                if (declaringInterfaceAccessorMethod.Equals(eventInfo.GetAddMethod(true))
                   || declaringInterfaceAccessorMethod.Equals(eventInfo.GetRemoveMethod(true)))
                {
                    return eventInfo;
                }
            }

            throw new ArgumentException($"The given {nameof(declaringInterfaceAccessorMethod)}: {declaringInterfaceAccessorMethod.Name} " +
                                        $"is not an event accessor", nameof(declaringInterfaceAccessorMethod));
        }
    }
}
