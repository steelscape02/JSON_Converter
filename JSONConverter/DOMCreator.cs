using System.Text.RegularExpressions;

namespace JSONConverter;

public class DomCreator(string name = "")
{
    private string vis = "public";
    private string _summary = "";

    public string BuildRoot(HashSet<Element> elements)
    {
        var root = $"{vis} class Root\n{{\n";
        foreach (var element in elements)
        {
            root += "   " + $"{vis} {element.Type} {element.Name} {{get; set;}}\n";
        }

        _summary += root;
        _summary += "}\n";
        _summary += BuildSubDom(elements);
        return _summary;
    }
    
    private string BuildSubDom(HashSet<Element> elements,string summary = "")
    {
        foreach (var element in elements)
        {
            if (element.Children.Count > 0)
            {
                if (element.Type != null) summary = SubClassDom(element.Children, element, summary);
                summary = BuildSubDom(element.Children,summary);
            }
        }

        return summary;
    }
    
    private string SubClassDom(HashSet<Element> elements, Element currHeader, string summary = "")
    {
        string type = "";
        if (currHeader.Type != null)
        {
            type = currHeader.List ? RemoveList(currHeader.Type) : currHeader.Type;
        }

        summary += $"{vis} class {type}\n{{\n";
        foreach (var element in elements)
        {
            if (string.IsNullOrEmpty(element.Type)) element.Type = "object";
            var nulled = element.Nullable ? element.Type + "?" : element.Type;
            summary += "    " + $"{vis} {nulled} {element.Name} {{get; set;}}\n";
        }
        summary += "}\n";
        return summary;
    }

    private string RemoveList(string text)
    {
        var pattern = @"<(.+?)>";
        var result = Regex.Match(text, pattern).Groups[1].Value;
        return result;
    }
}