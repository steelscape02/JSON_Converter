namespace JSONConverter;

public class Element(string? type,string name = "")
{
    public HashSet<Element> Children = [];
    public string Name { get; } = name;
    public string? Type { get; set; }= type;
    public bool Nullable { get; set; }
    public bool List { get; set; }

    public bool AddChild(Element newChild)
    {
        var added = Children.Add(newChild);
        return added;
    }

    public Element? GetMatching(Element element)
    {
        var match = Children.FirstOrDefault(x => x.Name == element.Name);
        return match ?? null;
    }
    
    public void ClearChildren()
    {
        Children.Clear();
    }

    public string Summary(string summary = "",string spacing = "") //testing ONLY
    {
        var type = Nullable ? $"{Type}?" : Type; //TODO: Add object catch to "?" solos
        summary += $"\n{spacing}Name: {Name}, Type: {type} - {Children.Count} children";
        foreach (var child in Children)
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