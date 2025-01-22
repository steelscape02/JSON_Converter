using System.Text.RegularExpressions;

namespace JSONConverter;

// --C# DOM Creator--
/// <summary>
/// Builds a C# data model (DM). Uses a <c>HashSet</c> of <c>Element</c> objects to recursively print a set of classes that
/// can be used to parse the JSON tree represented by the <c>HashSet</c>.
/// </summary>
public class CSharpDm //TODO: Optimize this structure (inc naming)
{
    /// <summary>
    /// The default visibility of the C# elements in the data model
    /// </summary>
    private const string Vis = "public";

    /// <summary>
    /// The default name for the base class. Almost always "Root"
    /// </summary>
    private const string BaseName = "Root";

    /// <summary>
    /// Tracks if a variable name has been changed due to containing illegal characters.
    /// Used to improve the corresponding JSON package
    /// </summary>
    private static bool _overWrite;
    /// <summary>
    /// Builds the Root class, with calls to <c>BuildSubDm</c> to create the child classes.
    /// </summary>
    /// <param name="elements">A <c>HashSet</c> of <c>Element</c> objects representing the <c>Root</c> class of
    /// the JSON response</param>
    /// <returns>A string representation of a C# data model</returns>
    public static string BuildRoot(HashSet<Element> elements)
    {
        var classDefinitions = new List<string>();

        // Collect all classes, starting with the root
        var rootClass = $"{Vis} class {BaseName}\n{{\n";

        foreach (var element in elements)
        {
            string? rename;
            if (element.Rename)
            {
                rename = $"\n   [JsonPropertyName(\"{element.Name}\")]\n   ";
                _overWrite = true;
            }
            else
                rename = null;
            rootClass += $"   {rename}{Vis} {element.Type} {element.LegalName()} {{get; set;}}\n";
        }

        rootClass += "}\n";
        classDefinitions.Add(rootClass);

        // Recursively build sub-classes
        var visited = new HashSet<string>();
        foreach (var element in elements)
        {
            BuildSubDm(element, visited, classDefinitions);
        }

        if (_overWrite) //add Json.Serialization package to use JsonPropertyName
        {
            classDefinitions.Insert(0,"using System.Text.Json.Serialization;\n");
        }
        return string.Join("\n", classDefinitions);
    }

    /// <summary>
    /// Recursively builds child classes for nested elements.
    /// </summary>
    private static void BuildSubDm(Element element, HashSet<string> visited, List<string> classDefinitions)
    {
        if (element.Type == null) return;
        var type = element.List ? RemoveList(element.Type) : element.Type;

        // Avoid re-creating classes
        if (IsPrimitive(RemoveList(type)) || !visited.Add(type))
            return;

        var classDef = $"{Vis} class {type}\n{{\n";

        foreach (var child in element.Children)
        {
            if (string.IsNullOrEmpty(child.Type))
            {
                child.Type = "object";
            }

            if (child.Children.Count > 0)
            {
                classDef += $"    {Vis} {child.Type} {child.LegalName()} {{get; set;}}\n";
                BuildSubDm(child, visited, classDefinitions); // Recursive call for nested children
            }
            else
            {
                var nulled = child.Nullable ? child.Type + "?" : child.Type;
                string? rename;
                if (child.Rename)
                {
                    rename = $"\n    [JsonPropertyName(\"{child.Name}\")]\n    ";
                    _overWrite = true;
                }
                else
                    rename = null;
                classDef += $"    {rename}{Vis} {nulled} {child.LegalName()} {{get; set;}}\n";
            }
        }

        classDef += "}\n";
        classDefinitions.Add(classDef);
    }

    /// <summary>
    /// Removes the <c>List</c> generic type from a string (<c>List<![CDATA[<>]]></c>).
    /// </summary>
    private static string RemoveList(string text)
    {
        return Regex.Replace(text, @"^.*?<|>.*?$", "");
    }
    
    private static bool IsPrimitive(string type)
    {
        var primitives = new HashSet<string>
        {
            "string", "int", "long", "float", "double", "decimal", "bool", "object"
        };
        return primitives.Contains(type);
    }
}