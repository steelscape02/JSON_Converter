using System.Reflection.PortableExecutable;

namespace JSONConverter;

public class Element(string type,string name = "")
{
    public string Type { get; set; }= type;
    
    private List<Element> _children = [];
    public string Name { get; } = name;
    private string _name = name;
    private string _type = type;

    public void AddChild(Element newChild)
    {
        if (_children.Contains(newChild))
        {
            return;
        }
        _children.Add(newChild);
    }

    public void ChangeType(string newType)
    {
        _type = newType;
    }

    public void ChangeName(string name)
    {
        _name = name;
    }

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

    public string Summary(string summary = "",string spacing = "") //testing ONLY
    {
        if(spacing == "")
            summary += $"\n{spacing}Name: {_name}, Type: {_type} - {_children.Count} children";
        foreach (var child in _children)
        {
            
            summary += $"\n{spacing}   Name: {child._name} Type: {child._type} - {child._children.Count} children";
            if(child._children.Count > 0)
            {
                summary += $"\n{spacing}{child.Summary(summary,spacing + "   ")}";
            }
            
        }  //TODO: Fix "building" effect (keeps printing the progressing summary
        
        return summary;
    }
}