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
    /// Builds the Root class, with calls <c>BuildSubDm</c> to create the child classes.
    /// </summary>
    /// <param name="elements">A <c>HashSet</c> of <c>Element</c> objects representing the <c>Root</c> class of
    /// a JSON response</param>
    /// <returns>A string representation of a C# data model</returns>
    public static string BuildRoot(HashSet<Element> elements)
    {
        
        var summary = $"{Vis} class Root\n{{\n";

        foreach (var element in elements)
        {
            var rename = element.Rename ? $"\n   [JsonProperty(\"{element.Name}\")]\n   " : null;
            summary += $"   {rename}{Vis} {element.Type} {element.LegalName()} {{get; set;}}\n";
        }

        summary += "}";
        summary += "}\n";
        summary += BuildSubDm(elements);
        return summary;
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="elements"></param>
    /// <param name="summary"></param>
    /// <returns></returns>
    private static string BuildSubDm(HashSet<Element> elements,string summary = "")
    {
        foreach (var element in elements.Where(element => element.Children.Count > 0)
                     .Where(element => !summary.Contains(element.LegalName())))
        {
            if (element.Type != null) summary = SubClassDm(element.Children, element, summary);
            
            summary = BuildSubDm(element.Children,summary);
        }

        return summary;
    }
    //TODO: Explain what SubClassDm does. Rename and address how it relates to BuildSubDm
    private static string SubClassDm(HashSet<Element> elements, Element currHeader, string summary = "")
    {
        var type = "";
        if (currHeader.Type != null)
        {
            type = currHeader.List ? RemoveList(currHeader.Type) : currHeader.Type;
        }

        summary += $"{Vis} class {type}\n{{\n";
        foreach (var element in elements)
        {
            if (string.IsNullOrEmpty(element.Type)) element.Type = "object";
            var nulled = element.Nullable ? element.Type + "?" : element.Type;
            var rename = element.Rename ? $"\n[JsonProperty(\"{element.Name}\")]\n    " : null;
            summary += "    " + rename + $"{Vis} {nulled} {element.LegalName()} {{get; set;}}\n";
        }
        summary += "}\n";
        return summary;
    }

    private static string RemoveList(string text)
    {
        var pattern = @"<>";
        var result = Regex.Match(text, pattern).Groups[1].Value;
        return result;
    }
}