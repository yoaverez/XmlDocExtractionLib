using System;
using System.Reflection;

namespace XmlDocExtractionLib
{
    /// <summary>
    /// Extension methods for checking if a member represents
    /// an explicit interface implementation.
    /// </summary>
    public static class ExplicitInterfaceImplementationExtensions
    {
        /// <summary>
        /// Binding flags for finding members.
        /// </summary>
        private static BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;

        /// <summary>
        /// Checks whether or not the <paramref name="method"/> is an explicit
        /// interface implementation.
        /// </summary>
        /// <param name="method">
        /// The method to check if it is an explicit
        /// interface implementation.
        /// </param>
        /// <param name="declaringInterfaceMethod">
        /// If the <paramref name="method"/> is an explicit interface implementation
        /// then the <see cref="MethodBase"/> of the interface method.
        /// Otherwise the default of <see cref="Type"/>.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the <paramref name="method"/> is an explicit interface
        /// implementation otherwise <see langword="false"/>.
        /// </returns>
        public static bool IsExplicitInterfaceImplementation(this MethodBase method, out MethodBase declaringInterfaceMethod)
        {
            declaringInterfaceMethod = default;

            // Explicit implementation means that the method should be private.
            if (!method.IsPrivate)
                return false;

            var type = method.DeclaringType;
            if (!type.IsInterface)
            {
                foreach (var interfaceType in type.GetInterfaces())
                {
                    var map = type.GetInterfaceMap(interfaceType);
                    for (int i = 0; i < map.TargetMethods.Length; i++)
                    {
                        if (map.TargetMethods[i] == method)
                        {
                            declaringInterfaceMethod = map.InterfaceMethods[i];
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Checks whether or not the <paramref name="property"/> is an explicit
        /// interface implementation.
        /// </summary>
        /// <param name="property">
        /// The property to check if it is an explicit
        /// interface implementation.
        /// </param>
        /// <param name="declaringInterfaceProperty">
        /// If the <paramref name="property"/> is an explicit interface implementation
        /// then the <see cref="PropertyInfo"/> of the interface property.
        /// Otherwise the default of <see cref="Type"/>.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the <paramref name="property"/> is an explicit interface
        /// implementation otherwise <see langword="false"/>.
        /// </returns>
        public static bool IsExplicitInterfaceImplementation(this PropertyInfo property, out PropertyInfo declaringInterfaceProperty)
        {
            declaringInterfaceProperty = default;

            var getter = property.GetGetMethod(true);
            var setter = property.GetSetMethod(true);

            var type = property.DeclaringType;
            if (!type.IsInterface)
            {
                foreach (var interfaceType in type.GetInterfaces())
                {
                    var map = type.GetInterfaceMap(interfaceType);
                    for (int i = 0; i < map.TargetMethods.Length; i++)
                    {
                        if ((getter != null && getter.IsPrivate && map.TargetMethods[i] == getter)
                            || (setter != null && setter.IsPrivate && map.TargetMethods[i] == setter))
                        {
                            declaringInterfaceProperty = map.InterfaceMethods[i].GetPropertyInfoFromAccessor();
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Checks whether or not the <paramref name="eventInfo"/> is an explicit
        /// interface implementation.
        /// </summary>
        /// <param name="eventInfo">
        /// The event to check if it is an explicit
        /// interface implementation.
        /// </param>
        /// <param name="declaringInterfaceEvent">
        /// If the <paramref name="eventInfo"/> is an explicit interface implementation
        /// then the <see cref="EventInfo"/> of the interface that declares that event.
        /// Otherwise the default of <see cref="Type"/>.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the <paramref name="eventInfo"/> is an explicit interface
        /// implementation otherwise <see langword="false"/>.
        /// </returns>
        public static bool IsExplicitInterfaceImplementation(this EventInfo eventInfo, out EventInfo declaringInterfaceEvent)
        {
            declaringInterfaceEvent = default;

            var adder = eventInfo.GetAddMethod(true);
            var remover = eventInfo.GetRemoveMethod(true);

            var type = eventInfo.DeclaringType;
            if (!type.IsInterface)
            {
                foreach (var interfaceType in type.GetInterfaces())
                {
                    var map = type.GetInterfaceMap(interfaceType);
                    for (int i = 0; i < map.TargetMethods.Length; i++)
                    {
                        if ((adder != null && adder.IsPrivate && map.TargetMethods[i] == adder)
                            || (remover != null && remover.IsPrivate && map.TargetMethods[i] == remover))
                        {
                            declaringInterfaceEvent = map.InterfaceMethods[i].GetEventInfoFromAccessor();
                            return true;
                        }
                    }
                }
            }
            return false;
        }
    }
}
