namespace JSONConverter;

public class Element(string type,string name = "")
{
    public string Type { get; set; }= type;
    public Element? Parent { get; set; }
    private readonly List<Element> _children = [];
    public string Name { get; set; } = name;
    private string _name = name;
    private string _type = type;

    public bool AddChild(Element newChild)
    {
        newChild.Parent = this;
        if (_children.Contains(newChild))
        {
            return false;
        }
        _children.Add(newChild);
        return true;
    }

    public void ChangeType(string newType)
    {
        _type = newType;
    }

    public string Summary() //testing ONLY
    {
        //TODO: Add 1 count check. If 1, it may be an array, so run the child's count
        var count = _children.Count;
        if (count == 1)
        {
            count = _children[0]._children.Count;
        }
        var summary = $"Name: {_name}, Type: {_type} - {_children.Count} children";
        return summary;
    }
}