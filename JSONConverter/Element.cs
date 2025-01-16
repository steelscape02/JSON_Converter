namespace JSONConverter;

public class Element
{
    public HashSet<Element> Children = [];

    public Element(string? type,string name = "")
    {
        Name = name;
        if (_illegal.Any(name.Contains))
        {
            Rename = true;
        }
        Type = type;
    }
    private List<string> _illegal = ["#", "$", "%", "^", "&", "*", "(", ")", "-", "+", "=", //list of illegal characters for variable naming
        "{", "}", "[", "]", "|", "\\", ";", ":", "'", "\"", "<", ">", ",", ".", "/", "?", "!"];
    public string Name { get; set; }
    public string? Type { get; set; }
    public bool Nullable { get; set; }
    public bool Rename { get; set; }
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
    
    public void FriendlyName()
    {
        //TODO: Expand w Pluralize.NET?
        //check against basic plural rules
        var cap = Name[0].ToString().ToUpper();
        //ies -> y
        if(!List)
        { //just capitalize the word and return
            Name = cap + Name.Substring(1, Name.Length - 1);
        }
        else if (Name.EndsWith("ies"))
        {
            Name = cap + Name.Substring(1, Name.Length - 4) + "y";
        }
        //s -> remove s (except special cases)
        else if (Name.EndsWith("s"))
        {
            Name = cap + Name.Substring(1, Name.Length - 2);
        }
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