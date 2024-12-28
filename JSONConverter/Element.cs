using System.Reflection.PortableExecutable;

namespace JSONConverter;

public class Element(string type,string name = "")
{
    public string Type { get; set; }= type;
    
    private List<Element> _children = [];
    public string Name { get; } = name;

    public void AddChild(Element newChild)
    {
        if (_children.Contains(newChild))
        {
            return;
        }
        _children.Add(newChild);
    }
    
    public void ClearChildren()
    {
        _children.Clear();
    }

    public string Summary(string summary = "",string spacing = "") //testing ONLY
    {
        summary += $"\n{spacing}Name: {Name}, Type: {Type} - {_children.Count} children";
        foreach (var child in _children)
        {
            summary += $"{spacing}{child.Summary("",spacing + "   ")}";
        }  
        
        return summary;
    }
    
    //.Contains() override
    public override bool Equals(object? obj)
    {
        if (obj is Element other)
        {
            return Name == other.Name;
        }
        return false;
    }
    public override int GetHashCode()
    {
        return Name.GetHashCode();
    }
}