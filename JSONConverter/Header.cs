using System.Runtime.InteropServices.JavaScript;

namespace JSONConverter;
/// <summary>
/// Creates a header that can be used as a class in a generated C# document
/// </summary>
public class Header
{
    public Header(string name)
    {
        Name = name;
    }

    public string Name { get; set; }
    //list of sub headers with appropriate primitive variable type (Or a capitalized version of the word if it's a class, and array with type if that's the thing)
    private Dictionary<string,string> subHeads { get; set; }

    /// <summary>
    /// Add a subhead to the <c>subHeads</c> Dictionary
    /// </summary>
    /// <param name="name">The name of the item</param>
    /// <param name="type">The type of the item. Use the following: <list type = "bullet">
    /// <item>Primitives (with null)</item>
    /// <item>Object Name (use <c>CapWord</c>)</item>
    /// <item>Primitive array with type</item>
    /// </list>
    /// </param>
    public void AddSubHeads(string name, string type = "")
    {
        //capitalize name if type is Object, append List<VARTYPE> to name if type is Array
        if (type.ToLower() == "object")
            name = CapWord(name);
        subHeads.Add(name, type);
    }

    public bool HasSubHead(string name)
    {
        return subHeads.ContainsKey(name);
    }

    
    public void ChangeType(string newType)
    {
        var last = subHeads.LastOrDefault();
        subHeads[last.Key] = newType;
    }

    public void AddNull(string name)
    {
        subHeads[name] = subHeads[name] + "?";
    }
    
    /// <summary>
    /// Capitalize the first letter of the word provided. Used for <c>subHeads</c> when an object is found
    /// </summary>
    /// <param name="word">
    ///The word to be capitalized
    /// </param>
    /// <returns>The capitalized word</returns>
    private string CapWord(string word)
    {
        var newWord = string.Concat(word[0].ToString().ToUpper(), word.AsSpan(1));
        return newWord;
    }

    public void Display()
    {
        Console.WriteLine("Name: " + Name);
        foreach (var subHead in subHeads)
        {
            Console.WriteLine($"  {subHead.Key}: {subHead.Value}");
        }
    }
}