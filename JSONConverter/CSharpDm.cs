using System.Text.RegularExpressions;

namespace JSONConverter;

// --C# DOM Creator--
/// <summary>
/// Builds a C# data model (DM). Uses a <c>HashSet</c> of <c>Element</c> objects to recursively print a set of classes that
/// can be used to parse the JSON tree represented by the <c>HashSet</c>.
/// </summary>
public partial class CSharpDm //TODO: Optimize this structure (inc naming)
{
    /// <summary>
    /// The default visibility of the C# elements in the data model
    /// </summary>
    private const string Vis = "public";
    
    /// <summary>
    /// Builds the Root class, with calls to <c>BuildSubDm</c> to create the child classes.
    /// </summary>
    /// <param name="elements">A <c>HashSet</c> of <c>Element</c> objects representing the <c>Root</c> class of
    /// the JSON response</param>
    /// <returns>A string representation of a C# data model</returns>
    public static string BuildRoot(HashSet<Element> elements)
    {
        
        var summary = $"{Vis} class Root\n{{\n";

        foreach (var element in elements)
        {
            var rename = element.Rename ? $"\n   [JsonProperty(\"{element.Name}\")]\n   " : null;
            summary += $"   {rename}{Vis} {element.Type} {element.LegalName()} {{get; set;}}\n";
        }
        
        summary += "}\n";
        summary += BuildSubDm(elements);
        return summary;
    }
    
    //TODO: FIX!!!!!!!!!!!!!!!!!! Lesser classes not getting added
    private static string BuildSubDm(HashSet<Element> elements)
    {
        //look through each Root element for children
        var summary = "";
        foreach (var element in elements.Where(element => element.Children.Count > 0)
                     .Where(element => !summary.Contains(element.LegalName())))
        {
            summary += ChildDm(element);
        }
        return summary;
    }
    /// <summary>
    /// Builds a data model (DM) for any of the root <c>Element</c> objects which have children
    /// </summary>
    /// <param name="currHeader"></param>
    /// <param name="summary"></param>
    /// <returns></returns>
    private static string ChildDm(Element currHeader, string summary = "")
    {
        var type = "";
        if (currHeader.Type != null)
        {
            type = currHeader.List ? RemoveList(currHeader.Type) : currHeader.Type;
        }

        summary += $"\n{Vis} class {type}\n{{\n";
        foreach (var element in currHeader.Children)
        {
            if (element.Children.Count > 0)
            {
                Console.WriteLine(element.Name);
                summary += BuildSubDm(element.Children);
            }
            if (string.IsNullOrEmpty(element.Type)) element.Type = "object";
            var nulled = element.Nullable ? element.Type + "?" : element.Type;
            var rename = element.Rename ? $"\n[JsonProperty(\"{element.Name}\")]\n    " : null;
            summary += "    " + rename + $"{Vis} {nulled} {element.LegalName()} {{get; set;}}\n";
        }
        summary += "}\n";
        return summary;
    }

    /// <summary>
    /// Removes the <c>List</c> generic type from a string (<c>List<![CDATA[<>]]></c>)
    /// </summary>
    /// <param name="text">The text containing a <c>List</c> generic type</param>
    /// <returns>The type argument of the <c>List</c></returns>
    private static string RemoveList(string text)
    {
        var result = Regex.Replace(text, @"^.*?<|>.*?$", "");
        return result;
    }
}