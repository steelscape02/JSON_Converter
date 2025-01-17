using System.Text.RegularExpressions;

namespace JSONConverter;

// --C# DOM Creator--

public class DomCreator //TODO: Optimize this structure (inc naming)
{
    private const string Vis = "public";
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
        summary += BuildSubDom(elements);
        return summary;
    }
    
    private static string BuildSubDom(HashSet<Element> elements,string summary = "")
    {
        foreach (var element in elements.Where(element => element.Children.Count > 0)
                     .Where(element => !summary.Contains(element.LegalName())))
        {
            if (element.Type != null) summary = SubClassDom(element.Children, element, summary);
            
            summary = BuildSubDom(element.Children,summary);
        }

        return summary;
    }
    
    private static string SubClassDom(HashSet<Element> elements, Element currHeader, string summary = "")
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