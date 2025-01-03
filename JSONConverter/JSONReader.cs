using System.Text.Json;
using System.Text.Json.Nodes;
using JSONConverter.Resources;

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
        SubRecursive(root,baseStuff,null);
        
        // Console.WriteLine(Local.page_div); //Just for debugging
        // var summary = "";
        // foreach (var element in baseStuff)
        // {
        //     summary += element.Summary();
        // }
        // Console.WriteLine(summary);
        return baseStuff;
    }
    
    private static void SubRecursive(JsonNode current, HashSet<Element> elements, Element? headElem)
    {
        switch (current)
        {
            case JsonValue jsonValue:
                
                ArgumentNullException.ThrowIfNull(headElem); //TODO: Is this optimal?
                switch (jsonValue.GetValueKind())
                {
                    case JsonValueKind.String:
                        
                        headElem.Type = "string";
                        break;
                    case JsonValueKind.Number:
                        //TODO: Keep highest accuracy for nums
                        //Accuracy: int : long > double > float 
                        var type = GetNumType(jsonValue.ToString());
                        var newPrec = GetNumPrecision(type);
                        var oldPrec = GetNumPrecision(headElem.Type);
                        if (newPrec > oldPrec)
                            headElem.Type = type;
                        //if(headElem.Name == "gap") Console.WriteLine($"Gap located w/ val: {jsonValue}");
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
                        throw new ArgumentOutOfRangeException(nameof(current)); //current isn't doing good things
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
                            if (match != null)
                            {
                                SubRecursive(i, elements, match);
                            }
                            else
                            {
                                SubRecursive(i,elements,headElem);
                            }
                            
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
                                {
                                    if(match.Type != null && prim.Type != null && prim.Type.Length > match.Type.Length) match.Type = prim.Type;
                                    SubRecursive(i, elements, match);
                                }
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
                    var objType = MakeFriendly(headElem.Name);
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
                    headElem.Type = "List<Mixed>"; //TODO: Make this a little more useful
                }
                
                break;
            case JsonObject jsonObject: 
                if (headElem == null) //start point (basically base case)
                {
                    foreach (var element in jsonObject)
                    {
                        //first iter (from root, so just grab all keys)
                        var type = element.Value?.GetValueKind().ToString();
                        if(type == "Object") type = MakeFriendly(element.Key);
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
                        var type = element.Value?.GetValueKind().ToString();
                        if(type == "Object") type = MakeFriendly(element.Key);
                        var elem = new Element(type, element.Key);
                        
                        if (type == null) elem.Nullable = true;
                        
                        var added = headElem.AddChild(elem);
                        if(added) SubRecursive(element.Value, elements, elem);
                        else
                        {
                            // TODO: Currently too scared to delete this code chunk. It just really confused me and the replacement code is a lot a bit cooler
                            
                            // if (element.Value == null || element.Value.GetValueKind() != JsonValueKind.Object) continue;
                            // foreach (var i in element.Value.AsObject())
                            // {
                            //     if (i.Value == null) continue;
                            //     var felt = new Element(i.Value?.GetValueKind().ToString(), i.Key);
                            //     if (string.IsNullOrEmpty(felt.Type)) continue;
                            //     headElem.ChangeType(i.Key, GetNumType(i.Value?.ToString()));
                            // }
                            var match = headElem.GetMatching(elem);
                            SubRecursive(element.Value, elements, match);
                        }
                    }
                }
                break;
        }
    }

    private static string MakeFriendly(string text)
    {
        //TODO: Make more fancy to accomodate grammatically stupid friendly names (like properties to Propertie)
        if (text[^1].ToString().Equals("s", StringComparison.CurrentCultureIgnoreCase))
        {
            var cap = text[0].ToString().ToUpper();
            text = cap + text.Substring(1, text.Length - 2);
            return text;
        }
        else
        {
            var cap = text[0].ToString().ToUpper();
            text = cap + text.Substring(1, text.Length - 1);
            return text;
        }
    }

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

    private static int GetNumPrecision(string type)
    {
        switch (type)
        {
            case "int":
            case "long":
                return 0;
            case "double":
                return 1;
            case "float":
                return 2;
            default:
                return -1;
        }
    }
}