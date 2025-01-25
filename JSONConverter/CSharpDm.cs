using System.Text.RegularExpressions;

namespace JSONConverter;

// --C# DOM Creator--
/// <summary>
/// Builds a C# data model (DM). Uses a <c>HashSet</c> of <c>Element</c> objects to recursively print a set of classes that
/// can be used to parse the JSON tree represented by the <c>HashSet</c>.
/// </summary>

//TODO: REPAIR LIST NAMING ON CLASSES AND BBOX ARTIFACT (tester.json (still prints prim array when header))

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

            if (element.Prim) continue;
            var headerType = PrintType(element,true);
            rootClass += $"   {rename}{Vis} {headerType} {element.LegalName()} {{get; set;}}\n";
        }

        rootClass += "}\n";
        classDefinitions.Add(rootClass);

        // Recursively build subclasses
        var visited = new HashSet<string>();
        foreach (var element in elements.Where(element => !element.Prim))
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
        var type = PrintType(element,true);

        // Avoid re-creating classes
        if (IsPrimitive(type) || !visited.Add(type))
            return;
        
        var classDef = $"{Vis} class {type}\n{{\n";

        foreach (var child in element.Children)
        {
            child.Type ??= Element.Types.Null;
            var childType = PrintType(child,false);
            if (child.Children.Count > 0)
            {
                
                classDef += $"    {Vis} {childType} {child.LegalName()} {{get; set;}}\n";
                BuildSubDm(child, visited, classDefinitions); // Recursive call for nested children
            }
            else
            {
                var nulled = child.Nullable ? childType + "?" : childType;
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

    //TODO: Comment
    private static bool IsPrimitive(string type)
    {
        var primitives = new HashSet<string>
        {
            "string", "int", "long", "float", "double", "decimal", "bool", "object"
        };
        return primitives.Contains(type);
    }

    /// <summary>
    /// Make the given text "friendly". If the given text was from a list object, return a capitalized and singular version of the word. Otherwise, capitalize and return.
    /// </summary>
    /// <param name="text">The text to convert</param>
    /// <param name="list"><c>true</c> if the caller is editing a List <c>Element</c> name</param>
    /// <returns>The "friendly" name</returns>
    private static string MakeFriendly(string text, bool list = false)
    {
        //check against basic plural rules
        var cap = text[0].ToString().ToUpper();

        if (!list)
        {
            //just capitalize the word and return
            text = cap + text.Substring(1, text.Length - 1);
        }
        else
        {
            if (text.EndsWith("ies"))
            {
                text = cap + text.Substring(1, text.Length - 4) + "y";
            }
            //s -> remove s (except special cases)
            else if (text.EndsWith("s"))
                text = cap + text.Substring(1, text.Length - 2);
            text = "List<" + text + ">";
        }
        return text;
    }

    //TODO: Comment
    private static string PrintType(Element elem,bool header)
    {
        var type = elem.Type.ToString();
        switch (elem.Type)
        {
            case Element.Types.Array:
            case Element.Types.Object:
                var isList = elem.List;
                if (header) isList = false;
                type = MakeFriendly(elem.Name,isList);
                break;
            case Element.Types.Null:
                type = "object";
                break;
            case Element.Types.Integer:
            case Element.Types.Double:
            case Element.Types.Float:
            case Element.Types.Long:
            case Element.Types.String:
            case Element.Types.Boolean:
                type = type?.ToLower();
                break;
            case null:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        return type;
    }
}