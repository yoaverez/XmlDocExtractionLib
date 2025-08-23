## About

XmlDocExtractionLib is an easy to use library designed to extract xml
documentation from types and members from a documentation xml file that is generated
by the compiler.

## Key Features

- Support extracting documentation from members with the new modifier.
- Support extracting documentation from enum values.
- Support extracting documentation from explicit interface implementation members.
- Allow resolving inherit documentation from multiple assemblies.
- Expose some of the useful inner functionality like:
    * Extensions methods for getting the identifiers of members.
    * Extension methods for getting members base definitions.
    * The resolver that resolve inherit documentation.

## How to Use
```cs
// Add this using in order to be able to use the extension methods.
using XmlDocExtractionLib;

// Create an instance of the XmlExtractionContext class.
var xmlExtractionContext = new XmlExtractionContext();

// Add all the assemblies and their corresponding xml files to the context.
xmlExtractionContext.AddAssembly(assembly, assemblyXmlDocumentationFilePath);

// Get the member whose documentation you are after.
var member = typeof(ComplexClass);

// Get the documentation.
member.GetXmlDocumentation(xmlExtractionContext, resolveInheritdoc: true);

/*
* For getting the documentation of an enum value you need to have the type of the enum 
* and the value of the enum.
*/
var enumType = typeof(Color);
enumType.GetEnumValueXmlDocumentation((int)Color.Red, xmlExtractionContext, resolveInheritdoc: true);
```