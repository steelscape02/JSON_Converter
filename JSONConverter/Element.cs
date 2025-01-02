namespace JSONConverter;

public class Element(string? type,string name = "")
{
    private readonly HashSet<Element> _children = [];
    public string Name { get; set; } = name;
    public string? Type { get; set; }= type;
    public bool Nullable { get; set; }

    public bool AddChild(Element newChild)
    {
        var added = _children.Add(newChild);
        return added;
    }

    public Element? GetMatching(Element element)
    {
        var match = _children.FirstOrDefault(x => x.Name == element.Name);
        return match ?? null;
    }

    public bool ChangeType(string name, string newType)
    {
        foreach (var obj in _children)
        {
            // Check if the current object's name matches
            if (obj.Name == name)
            {
                obj.Type ??= newType;
                return true;
            }

            // If the current object has children, recursively search them
            if (obj._children.Count <= 0) continue;
            if (obj.ChangeType(name, newType))
            {
                return true; // Stop searching once a match is found
            }
        }

        return false;
    }
    
    public void ChangeChildName(string newName)
    {
        foreach (var child in _children)
        {
            child.Name = newName;
        }
    }
    
    public void ClearChildren()
    {
        _children.Clear();
    }

    public string Summary(string summary = "",string spacing = "") //testing ONLY
    {
        var type = (Nullable) ? $"{Type}?" : Type;
        summary += $"\n{spacing}Name: {Name}, Type: {type} - {_children.Count} children";
        foreach (var child in _children)
        {
            summary += $"{spacing}{child.Summary("",spacing + "   ")}";
        }  
        
        return summary;
    }
    
    //.Contains() overrides
    public override bool Equals(object? obj)
    {
        if (obj is Element other) //return Name == other.Name && Age == other.Age;
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