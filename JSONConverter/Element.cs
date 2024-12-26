namespace JSONConverter;

public class Element(string type,string name = "")
{
    public string Type { get; set; }= type;
    private readonly List<Element> _children = [];
    public string Name { get; } = name;
    private readonly string _name = name;
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
        return summary;
    }
}