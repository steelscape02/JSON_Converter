using System.Diagnostics;
using System.Net.Mime;

namespace JSONConverter;
/// <summary>
/// Stores an individual JSON element for efficient management of the JSON tree
/// </summary>
public class Element
{
    /// <summary>
    /// Stores an individual JSON element for efficient management of the JSON tree
    /// </summary>
    /// <param name="type">The type of the JSON item</param>
    /// <param name="name">The name of the JSON item. Can be parsed to improve readability and comply with language restrictions</param>
    public Element(string? type,string name = "")
    {
        Name = name;
        if (_illegal.Any(name.Contains))
        {
            Rename = true;
        }
        Type = type;
    }
    
    /// <summary>
    /// The Child elements of the parent <c>Element</c>
    /// </summary>
    public readonly HashSet<Element> Children = [];
    
    /// <summary>
    /// The name of the <c>Element</c>
    /// </summary>
    public string Name { get; private set; }

    /// <summary>
    /// The JSON type of the <c>Element</c>
    /// </summary>
    public string? Type { get; set; }
    
    /// <summary>
    /// The nullability of the <c>Element</c>. When <c>true</c>, this <c>Element</c> could be null, when <c>false</c>
    /// the <c>Element</c> never appeared as null
    /// </summary>
    public bool Nullable { get; set; }
    
    /// <summary>
    /// If an illegal character is found, the <c>Name</c> member variable will need to be edited when printed. If <c>true</c>,
    /// this <c>Element</c> must be renamed in the DOM, if <c>false</c> the name is valid without renaming
    /// </summary>
    public bool Rename { get; }
    
    /// <summary>
    /// If the <c>Element</c> is a list, this will be <c>true</c>
    /// </summary>
    public bool List { get; set; }
    
    private readonly char[] _illegal = ['#', '$', '%', '^', '&', '*', '(', ')', '-', '+', '=', //list of illegal characters for variable naming
        '{', '}', '[', ']', '|', '\\', ';', ':', '"', '\'', '<', '>', ',', '.', '/', '?', '!'];

    /// <summary>
    /// Add child to the <c>Children</c> list
    /// </summary>
    /// <param name="newChild">The desired <c>Element</c> to add</param>
    /// <returns><c>true</c> if added, <c>false</c> if not</returns>
    public bool AddChild(Element newChild)
    {
        var added = Children.Add(newChild);
        return added;
    }

    /// <summary>
    /// Get the duplicate <c>Element</c> in the Children list <i>if present</i>
    /// </summary>
    /// <param name="element">The <c>Element</c> to search for a duplicate of</param>
    /// <returns>The matching <c>Element</c> if found, otherwise <c>null</c></returns>
    public Element? GetMatching(Element element)
    {
        var match = Children.FirstOrDefault(x => x.Name == element.Name);
        return match ?? null;
    }
    
    /// <summary>
    /// Remove all Children for this element
    /// </summary>
    public void ClearChildren()
    {
        Children.Clear();
    }

    /// <summary>
    /// Removes all illegal characters from the <c>Name</c> member variable
    /// </summary>
    /// <returns>An unaltered <c>Name</c> if no illegal characters where found, otherwise a <c>Name</c> with all illegal characters removed</returns>
    public string LegalName()
    {
        if (!Rename) return Name;
        while (_illegal.Any(Name.Contains))
        {
            var index = Name.IndexOfAny(_illegal);
            Name = Name.Remove(index,1);
        }
        return Name;
    }

    /// <summary>
    /// <b>For testing <i>only</i></b>. Returns an indented list of all child elements of the called <c>Element</c>
    /// </summary>
    /// <param name="summary"><b>Optional</b>Sets the initial summary text for the summary</param>
    /// <param name="spacing"><b>Optional</b>Sets initial padding for Summary</param>
    /// <returns>A completed summary of all child <c>Element</c> objects</returns>
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