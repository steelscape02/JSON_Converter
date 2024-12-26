namespace JSONConverter;

public class Element(string type,string name = "")
{
    public string Type { get; set; }= type;
    private readonly List<Element> _children = [];
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

    public string Summary() //testing ONLY
    {
        var summary = $"Name: {_name}, Type: {_type} - {_children.Count} children";
        foreach (var child in _children)
        {
            summary += $"\n   Name: {child._name} Type: {child._type} - {child._children.Count} children";
        }
        if(_children.Count > 0)
            if(_children[0]._children.Count > 0)
                Console.WriteLine(_children[0]._children[0].Summary());
        return summary;
    }
}