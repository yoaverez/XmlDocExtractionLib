using System.Reflection;
using System;

namespace XmlDocExtractionLib
{
    /// <summary>
    /// Generic extensions methods for <see cref="MemberInfo"/>.
    /// </summary>
    internal static class MembersExtensionsUtils
    {
        /// <summary>
        /// Binding flags for finding members.
        /// </summary>
        private static BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;

        /// <summary>
        /// Retrieve the <see cref="PropertyInfo"/> of the property that the given
        /// <paramref name="accessorMethod"/> belongs to.
        /// </summary>
        /// <param name="accessorMethod">The accessor (get or set) method of the property.</param>
        /// <returns>
        /// The <see cref="PropertyInfo"/> of the property that the given
        /// <paramref name="accessorMethod"/> belongs to.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// Thrown when the given <paramref name="accessorMethod"/>
        /// does not belong to any property.</exception>
        public static PropertyInfo GetPropertyInfoFromAccessor(this MethodBase accessorMethod)
        {
            var declaringAccessor = accessorMethod.DeclaringType;

            foreach (var prop in declaringAccessor.GetProperties(bindingFlags))
            {
                if (accessorMethod.Equals(prop.GetGetMethod(true))
                   || accessorMethod.Equals(prop.GetSetMethod(true)))
                {
                    return prop;
                }
            }

            throw new ArgumentException($"The given {nameof(accessorMethod)}: {accessorMethod.Name} " +
                                        $"is not a property accessor", nameof(accessorMethod));
        }

        /// <summary>
        /// Retrieve the <see cref="EventInfo"/> of the event that the given
        /// <paramref name="accessorMethod"/> belongs to.
        /// </summary>
        /// <param name="accessorMethod">The accessor (add or remove) method of the event.</param>
        /// <returns>
        /// The <see cref="EventInfo"/> of the event that the given
        /// <paramref name="accessorMethod"/> belongs to.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// Thrown when the given <paramref name="accessorMethod"/>
        /// does not belong to any event.</exception>
        public static EventInfo GetEventInfoFromAccessor(this MethodBase accessorMethod)
        {
            var declaringAccessor = accessorMethod.DeclaringType;

            foreach (var eventInfo in declaringAccessor.GetEvents(bindingFlags))
            {
                if (accessorMethod.Equals(eventInfo.GetAddMethod(true))
                   || accessorMethod.Equals(eventInfo.GetRemoveMethod(true)))
                {
                    return eventInfo;
                }
            }

            throw new ArgumentException($"The given {nameof(accessorMethod)}: {accessorMethod.Name} " +
                                        $"is not an event accessor", nameof(accessorMethod));
        }
    }
}
