using System.Text.Json;
using System.Text.Json.Nodes;

namespace JSONConverter;

public class JsonReader(string filename)
{
    private string Filename { get; } = filename;

    public HashSet<Element> ReadJson()
    {
        var reader = File.ReadAllText(Filename);
        
        var document = JsonNode.Parse(reader) ?? "Blah"; //funny fallback added
        var root = document.Root; //root is JSON Object
        var baseStuff = new HashSet<Element>();
        SubRecursive(root, baseStuff, null);
        return baseStuff;
    }
    
    /// <summary>
    /// Search through a JSON object recursively to find all child items.
    /// </summary>
    /// <param name="current">The initial JSON object (<i>Typically <c>root</c></i>)</param>
    /// <param name="elements">An <b>empty</b> Element HashSet to store the top level (root) elements with their respective children</param>
    /// <param name="headElem">The current head element. When execution begins, this should be <c>null</c> unless another start point has been determined</param>
    // <exception cref="ArgumentOutOfRangeException"></exception> //TODO: Add back. Reduce existing exceptions?
    private static void SubRecursive(JsonNode current, HashSet<Element> elements, Element? headElem)
    {
        switch (current)
        {
            case JsonValue jsonValue:
                
                ArgumentNullException.ThrowIfNull(headElem);
                switch (jsonValue.GetValueKind())
                {
                    case JsonValueKind.String:
                        headElem.Type = "string";
                        break;
                    case JsonValueKind.Number:
                        //Accuracy: int & long > double > float 
                        var type = GetNumType(jsonValue.ToString());
                        var newPrec = GetNumPrecision(type);
                        var oldPrec = GetNumPrecision(headElem.Type);
                        if (newPrec > oldPrec && newPrec != -1) //-1 is the error case of GetNumPrecision
                            headElem.Type = type;
                        break;
                    case JsonValueKind.True:
                    case JsonValueKind.False:
                        headElem.Type = "bool";
                        break;
                    case JsonValueKind.Null:
                    case JsonValueKind.Undefined:
                    case JsonValueKind.Object:
                    case JsonValueKind.Array:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(current.ToString()); //current isn't doing good things
                }
                break;
            case JsonArray jsonArray:
                
                var isObj = true;
                var isPrim = true;
                var primType = "";

                ArgumentNullException.ThrowIfNull(headElem); //TODO: Is this optimal?
                foreach (var i in jsonArray)
                {
                    switch (i)
                    {
                        case JsonObject:
                        {
                            isPrim = false;
                            var obj = new Element("Object");
                            var match = headElem.GetMatching(obj);
                            SubRecursive(i, elements, match ?? headElem);

                            break;
                        }
                        case JsonValue:
                        {
                            isObj = false;
                            var val = i.AsValue();
                            
                            primType = GetNumType(i.ToString());
                            var prim = new Element(primType,val.ToString());
                            var added = headElem.AddChild(prim);
                            if(added) SubRecursive(i,elements,prim);
                            else
                            {
                                var match = headElem.GetMatching(prim);
                                if (match != null)
                                    SubRecursive(i, elements, match);
                            }
                            break;
                        }
                        case JsonArray: //unlikely, but possible JsonArray nesting
                        {
                            SubRecursive(i, elements, headElem);
                            break;
                        }
                        default:
                            throw new ArgumentOutOfRangeException(nameof(current));
                    }
                }

                if (isObj)
                {
                    var objType = MakeFriendly(headElem.Name,true);
                    headElem.Type = $"List<{objType}>";
                    headElem.List = true;
                }
                else if (isPrim)
                {
                    headElem.Type = $"List<{primType}>";
                    headElem.ClearChildren(); //kill all prim type child elems
                }
                else
                {
                    headElem.Type = "List<Mixed>"; //TODO: Determine an error case
                }
                
                break;
            case JsonObject jsonObject: 
                if (headElem == null)
                {
                    foreach (var element in jsonObject)
                    {
                        
                        var valueKind = element.Value?.GetValueKind();
                        var type = valueKind.ToString();
                        if(valueKind == JsonValueKind.Object) type = MakeFriendly(element.Key);
                        var elem = new Element(type, element.Key);
                        var added = elements.Add(elem);
                        if(added)
                            if (element.Value != null)
                                SubRecursive(element.Value, elements, elem);
                    }
                }
                else
                {
                    
                    foreach (var element in jsonObject)
                    {
                        var valueKind = element.Value?.GetValueKind();
                        var type = valueKind.ToString();
                        if(valueKind == JsonValueKind.Object) type = MakeFriendly(element.Key);
                        var elem = new Element(type, element.Key);

                        if (valueKind is null or JsonValueKind.Null) elem.Nullable = true; //null is possible due to ? above
                        
                        var added = headElem.AddChild(elem);
                        if(added)
                        {
                            if (element.Value != null) SubRecursive(element.Value, elements, elem);
                        }
                        else
                        {
                            var match = headElem.GetMatching(elem);
                            if (match == null) continue;
                            if (elem.Nullable) match.Nullable = true;

                            if (element.Value != null) SubRecursive(element.Value, elements, match);
                        }
                    }
                }
                break;
        }
    }
    
    /// <summary>
    /// Make the given text "friendly". If the given text was from a list object, return a capitalized and singular version of the word. Otherwise, capitalize and return.
    /// </summary>
    /// <param name="text">The text to convert</param>
    /// <param name="list"><c>true</c> if the caller is editing a List <c>Element</c> name</param>
    /// <returns>The "friendly" name</returns>
    private static string MakeFriendly(string text,bool list = false)
    {
        //check against basic plural rules
        var cap = text[0].ToString().ToUpper();
        
        if(!list)
        { //just capitalize the word and return
            text = cap + text.Substring(1, text.Length - 1);
        }
        else if (text.EndsWith("ies"))
        {
            text = cap + text.Substring(1, text.Length - 4) + "y";
        }
        //s -> remove s (except special cases)
        else if (text.EndsWith("s"))
            text = cap + text.Substring(1, text.Length - 2);
        
        
        return text;
    }

    /// <summary>
    /// Gets the type of the given number string using type parsing
    /// </summary>
    /// <param name="num">The string representation of the number</param>
    /// <returns>A string of the type</returns>
    private static string GetNumType(string? num)
    {
        if (int.TryParse(num, out _))
        {
            return "int";
        }

        if (long.TryParse(num, out _))
        {
            return "long";
        }

        if (double.TryParse(num, out _))
        {
            return "double";
        }

        if (decimal.TryParse(num, out _))
        {
            return "decimal";
        }

        return "";
    }

    /// <summary>
    /// Finds the "precision" of a number using its classification (<c>string</c>, <c>double</c>, etc..) for comparison
    /// of precision when deciding what type to keep
    /// </summary>
    /// <param name="type">The numerical type as a <c>string</c></param>
    /// <returns>
    /// The numerical precision of the number, or <c>-1</c> if the type is not a basic C# number variable type
    /// </returns>
    private static int GetNumPrecision(string? type)
    {
        return type switch
        {
            "int" => 0,
            "long" => 1,
            "double" => 2,
            "float" => 3,
            _ => -1 //default
        };
    }
}