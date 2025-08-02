using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace XmlDocExtractionLib
{
    /// <summary>
    /// Extension methods for extracting the xml documentation identifiers
    /// from members.
    /// </summary>
    public static class MemberIdentifierExtractionExtensions
    {
        /// <summary>
        /// Gets the xml documentation identifier of the given <paramref name="type"/>.
        /// </summary>
        /// <param name="type">The type whose xml documentation identifier is requested.</param>
        /// <returns>The xml documentation identifier of the given <paramref name="type"/>.</returns>
        public static string GetTypeNameIdentifier(this Type type)

        {
            return $"T:{GetTypeNameIdentifierWithoutPrefix(type)}";
        }

        /// <summary>
        /// Gets the xml documentation identifier of the given <paramref name="fieldInfo"/>.
        /// </summary>
        /// <param name="fieldInfo">The field whose xml documentation identifier is requested.</param>
        /// <returns>The xml documentation identifier of the given <paramref name="fieldInfo"/>.</returns>
        public static string GetFieldNameIdentifier(this FieldInfo fieldInfo)
        {
            return $"F:{GetTypeNameIdentifierWithoutPrefix(fieldInfo.ReflectedType)}.{fieldInfo.Name}";
        }

        /// <summary>
        /// Gets the xml documentation identifier of the given enum value.
        /// </summary>
        /// <param name="enumType">The type of the enum whose value xml documentation identifier is requested.</param>
        /// <param name="value">The enum value whose value xml documentation identifier is requested.</param>
        /// <returns>The xml documentation identifier of the given enum value.</returns>
        public static string GetEnumValueNameIdentifier(this Type enumType, int value)
        {
            if (!enumType.IsEnum)
                throw new ArgumentException($"The given {nameof(enumType)}: {enumType.Name} is not an enum.", nameof(enumType));

            var enumValueName = Enum.GetName(enumType, value);
            return $"F:{GetTypeNameIdentifierWithoutPrefix(enumType)}.{enumValueName}";
        }

        /// <summary>
        /// Gets the xml documentation identifier of the given <paramref name="propertyInfo"/>.
        /// </summary>
        /// <param name="propertyInfo">The property whose xml documentation identifier is requested.</param>
        /// <returns>The xml documentation identifier of the given <paramref name="propertyInfo"/>.</returns>
        public static string GetPropertyNameIdentifier(this PropertyInfo propertyInfo)
        {
            var propertyName = propertyInfo.Name;
            if (propertyInfo.IsExplicitInterfaceImplementation(out _))
                propertyName = ConvertToExplicitInterfaceImplementationIdentifier(propertyName);

            return $"P:{GetTypeNameIdentifierWithoutPrefix(propertyInfo.ReflectedType)}.{propertyName}";
        }

        /// <summary>
        /// Gets the xml documentation identifier of the given <paramref name="eventInfo"/>.
        /// </summary>
        /// <param name="eventInfo">The event whose xml documentation identifier is requested.</param>
        /// <returns>The xml documentation identifier of the given <paramref name="eventInfo"/>.</returns>
        public static string GetEventNameIdentifier(this EventInfo eventInfo)
        {
            var eventName = eventInfo.Name;
            if (eventInfo.IsExplicitInterfaceImplementation(out _))
                eventName = ConvertToExplicitInterfaceImplementationIdentifier(eventName);

            return $"E:{GetTypeNameIdentifierWithoutPrefix(eventInfo.ReflectedType)}.{eventName}";
        }

        /// <summary>
        /// Gets the xml documentation identifier of the given <paramref name="methodBase"/>.
        /// </summary>
        /// <param name="methodBase">The method (including ctors) whose xml documentation identifier is requested.</param>
        /// <returns>The xml documentation identifier of the given <paramref name="methodBase"/>.</returns>
        public static string GetMethodNameIdentifier(this MethodBase methodBase)
        {
            var reflectedType = methodBase.ReflectedType;
            var reflectedTypeIdentifier = GetTypeNameIdentifierWithoutPrefix(reflectedType);

            var methodName = methodBase.Name;
            if (methodBase.IsConstructor)
            {
                methodName = "#ctor";
            }

            else if (methodBase.IsExplicitInterfaceImplementation(out _))
            {
                methodName = ConvertToExplicitInterfaceImplementationIdentifier(methodName);
            }

            var typeGenericMap = new Dictionary<string, int>();
            var typeGenericArguments = reflectedType.GetGenericArguments();
            for (int i = 0; i < typeGenericArguments.Length; i++)
            {
                typeGenericMap.Add(typeGenericArguments[i].Name, i);
            }

            var methodGenericMap = new Dictionary<string, int>();
            var methodGenericArguments = methodBase.IsConstructor ? Array.Empty<Type>() : methodBase.GetGenericArguments();
            for (int i = 0; i < methodGenericArguments.Length; i++)
            {
                methodGenericMap.Add(methodGenericArguments[i].Name, i);
            }

            var methodNameGenericSuffix = methodGenericMap.Count > 0 ? $"``{methodGenericMap.Count}" : string.Empty;

            var parametersIdentifiers = new List<string>();
            foreach (var parameterInfo in methodBase.GetParameters())
            {
                parametersIdentifiers.Add(GetParameterNameIdentifier(parameterInfo, typeGenericMap, methodGenericMap));
            }

            var parametersList = string.Join(",", parametersIdentifiers);

            var parametersSuffix = parametersIdentifiers.Count > 0 ? $"({parametersList})" : $"";

            var convertionsOperatorSuffix = string.Empty;
            if (methodBase.Name.Equals("op_Explicit") || methodBase.Name.Equals("op_Implicit"))
            {
                convertionsOperatorSuffix = $"~{GetParameterTypeNameIdentifier((methodBase as MethodInfo).ReturnType, typeGenericMap, methodGenericMap)}";
            }

            return $"M:{reflectedTypeIdentifier}.{methodName}{methodNameGenericSuffix}{parametersSuffix}{convertionsOperatorSuffix}";
        }

        /// <summary>
        /// Gets the xml documentation identifier of the given <paramref name="type"/>
        /// without the `T:` prefix.
        /// </summary>
        /// <param name="type">The type whose xml documentation identifier is requested.</param>
        /// <returns>
        /// The xml documentation identifier of the given <paramref name="type"/>
        /// without the `T:` prefix.
        /// </returns>
        private static string GetTypeNameIdentifierWithoutPrefix(Type type)
        {
            // Full name of a generic type is null
            // i.e. typeof(IEnumerable<T>).FullName is null while
            // typeof(IEnumerable<>).FullName is not null.
            var fullName = type.IsGenericType ? type.GetGenericTypeDefinition().FullName : type.FullName;

            // The first replace handles assembly information on the FullName of the Type
            // The second replace deals with nested types.
            string identifier = Regex.Replace(fullName, @"\[.*\]", string.Empty)
                                     .Replace('+', '.');
            return identifier;
        }

        /// <summary>
        /// Gets the xml documentation identifier of the given <paramref name="parameterInfo"/>.
        /// </summary>
        /// <param name="parameterInfo">The mnethod parameter whose xml documentation identifier is requested.</param>
        /// <param name="typeGenericMap">
        /// A mapping between a generic parameter name (e.g. T) of the type that defines the
        /// method which defines the given <paramref name="parameterInfo"/> to its
        /// generic parameter index.
        /// </param>
        /// <param name="methodGenericMap">
        /// A mapping between a generic parameter name (e.g. T) of the method that defines the given
        /// <paramref name="parameterInfo"/> to its generic parameter index.
        /// </param>
        /// <returns>The xml documentation identifier the given <paramref name="parameterInfo"/>.</returns>
        private static string GetParameterNameIdentifier(ParameterInfo parameterInfo, Dictionary<string, int> typeGenericMap, Dictionary<string, int> methodGenericMap)
        {
            var suffix = "";
            if (HasRefInOutParameter(parameterInfo))
                suffix = "@";

            return $"{GetParameterTypeNameIdentifier(parameterInfo.ParameterType, typeGenericMap, methodGenericMap)}{suffix}";
        }

        /// <summary>
        /// Gets the xml documentation identifier of a method parameter whose type is the given <paramref name="type"/>.
        /// </summary>
        /// <param name="type">The type of the method parameter whose xml documentation identifier is requested.</param>
        /// <param name="typeGenericMap">
        /// A mapping between a generic parameter name (e.g. T) of the type that defines the
        /// method which uses the given <paramref name="type"/> in its parameters to its
        /// generic parameter index.
        /// </param>
        /// <param name="methodGenericMap">
        /// A mapping between a generic parameter name (e.g. T) of the method which uses the
        /// given <paramref name="type"/> in its parameters to its generic parameter index.
        /// </param>
        /// <returns>The xml documentation identifier of a method parameter whose type is the given <paramref name="type"/>.</returns>
        private static string GetParameterTypeNameIdentifier(Type type, Dictionary<string, int> typeGenericMap, Dictionary<string, int> methodGenericMap)
        {
            if (type.HasElementType)
            {
                var elementType = type.GetElementType();
                var elementTypeIdentifier = GetParameterTypeNameIdentifier(elementType, typeGenericMap, methodGenericMap);
                if (type.IsArray)
                {
                    var rank = type.GetArrayRank();
                    if (rank > 1)
                    {
                        return $"{elementTypeIdentifier}[{string.Join(",", Enumerable.Repeat("0:", rank))}]";
                    }
                    else
                    {
                        return $"{elementTypeIdentifier}[]";
                    }
                }
                else if (type.IsPointer)
                {
                    return $"{elementTypeIdentifier}*";
                }
                else
                {
                    // This is a parameter that was passed by ref
                    // so do noting because we just want the type identifier.
                    return $"{elementTypeIdentifier}";
                }
            }

            if(type.IsGenericParameter)
            {
                int index;
                if (methodGenericMap.TryGetValue(type.Name, out index))
                    return $"``{index}";
                else if (typeGenericMap.TryGetValue(type.Name, out index))
                    return $"`{index}";
                else
                    throw new ArgumentException($"The given {nameof(type)}: {type.Name} is a generic parameter " +
                        $"that is not related to the method nor to the type that reflects the method.");
            }

            if (type.IsGenericType)
            {
                var genericParameters = string.Join(",", type.GetGenericArguments().Select(generic => GetParameterTypeNameIdentifier(generic, typeGenericMap, methodGenericMap)));
                return $"{GetGenericTypeNameIdentifierWithoutGenerics(type)}{{{genericParameters}}}";
            }

            // This is a regular type.
            return GetTypeNameIdentifierWithoutPrefix(type);
        }

        /// <summary>
        /// Retrieves the name of the given <paramref name="genericType"/> without
        /// all the generic suffix e.g. Tuple`3 → Tuple.
        /// </summary>
        /// <param name="genericType">The generic type whose name without the generic suffix is requested.</param>
        /// <returns>
        /// the name of the given <paramref name="genericType"/> without
        /// all the generic suffix
        /// </returns>
        /// <exception cref="ArgumentException">
        /// Thrown when the given <paramref name="genericType"/> is not a generic type.
        /// </exception>
        private static string GetGenericTypeNameIdentifierWithoutGenerics(Type genericType)
        {
            if (!genericType.IsGenericType)
                throw new ArgumentException($"The given {nameof(genericType)}: {genericType.Name} " +
                    $"is not a generic type.");

            var typeNameIdentifier = GetTypeNameIdentifierWithoutPrefix(genericType);
            return typeNameIdentifier.Substring(0, typeNameIdentifier.LastIndexOf('`'));
        }

        /// <summary>
        /// Check whether or not the given <paramref name="parameterInfo"/> has
        /// a <see langword="ref"/> or an <see langword="in"/> or an <see langword="out"/> prefix.
        /// </summary>
        /// <param name="parameterInfo">The parameter to check.</param>
        /// <returns>
        /// <see langword="true"/> if the given <paramref name="parameterInfo"/> has a
        /// <see langword="ref"/> or an <see langword="in"/> or an <see langword="out"/> prefix
        /// otherwise <see langword="false"/>.
        /// </returns>
        private static bool HasRefInOutParameter(ParameterInfo parameterInfo)
        {
            return parameterInfo.IsIn
                   || parameterInfo.IsOut
                   || parameterInfo.ParameterType.IsByRef;
        }

        /// <summary>
        /// Convert the given <paramref name="memberName"/> to an identifier
        /// that represents an explicit interface implementation member.
        /// </summary>
        /// <param name="memberName">The name of the member.</param>
        /// <returns>
        /// The xml documentation identifier of the member.
        /// </returns>
        private static string ConvertToExplicitInterfaceImplementationIdentifier(string memberName)
        {
            // When the member is an explicit interface implementation,
            // its name will be look like <interface namespace>.<declaring types if the interface is nested>.<interface name>.<member name>.
            // In order to distinguish between the explicit interface from the declaring
            // type of the member, we replace the '.' with '#'.
            // We replace < and > with { and } since for generics.
            return memberName.Replace('.', '#')
                             .Replace('<', '{')
                             .Replace('>', '}');
        }
    }
}
