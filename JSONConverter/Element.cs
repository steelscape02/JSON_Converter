namespace JSONConverter;

public class Element(string? type,string name = "")
{
    private readonly List<Element> _children = [];
    public string Name { get; private set; } = name;
    public string? Type { get; set; }= type;
    public bool Nullable { get; set; }

    public bool AddChild(Element newChild)
    {
        var match = _children.FirstOrDefault(x => x.Name == newChild.Name);
        if (match != null)
        {
            if (newChild.Type.Length > 1)
            {
                if(newChild.Name.Length > 1) Console.WriteLine($"{newChild.Name} : {newChild.Type}");
                match.Type = newChild.Type;
            }
            return false;
        }
        _children.Add(newChild);
        return true;
    }

    public Element? GetMatching(Element element)
    {
        var match = _children.FirstOrDefault(x => x.Name == element.Name);
        return match ?? null;
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