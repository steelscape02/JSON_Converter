namespace JSONConverter;

public class Element(string? type,string name = "")
{
    public HashSet<Element> Children = [];
    public string Name { get; set; } = name;
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

    public bool ChangeType(string name, string newType)
    {
        foreach (var obj in Children)
        {
            // Check if the current object's name matches
            if (obj.Name == name)
            {
                obj.Type ??= newType;
                return true;
            }

            // If the current object has children, recursively search them
            if (obj.Children.Count <= 0) continue;
            if (obj.ChangeType(name, newType))
            {
                return true; // Stop searching once a match is found
            }
        }

        return false;
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