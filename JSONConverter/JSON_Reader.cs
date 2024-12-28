using System.Text.Json;
using System.Text.Json.Nodes;
using JSONConverter.Resources;

namespace JSONConverter;

public class JsonReader(string filename)
{
    private string Filename { get; } = filename;

    public void GetHeaders()
    {
        var reader = File.ReadAllText(Filename);
        
        var document = JsonNode.Parse(reader) ?? "Blah"; //funny fallback added
        var root = document.Root; //root is JSON Object
        var baseStuff = new List<Element>();
        SubRecursive(root,baseStuff,null);
        
        Console.WriteLine(Local.page_div); //Just for debugging
        var summary = "";
        foreach (var element in baseStuff)
        {
            summary += element.Summary();
        }
        Console.WriteLine(summary);
    }
    
    private static void SubRecursive(JsonNode current, List<Element> elements, Element? headElem)
    {
        switch (current)
        {
            case JsonValue jsonValue:
                
                ArgumentNullException.ThrowIfNull(headElem); //TODO: Is this optimal?
                if (headElem.Type == "Object") break; //prevent overwriting complex structure naming
                switch (jsonValue.GetValueKind())
                {
                    case JsonValueKind.String:
                        headElem.Type = "string";
                        break;
                    case JsonValueKind.Number:
                        headElem.Type = GetNumType(jsonValue.ToString());
                        break;
                    case JsonValueKind.True:
                    case JsonValueKind.False:
                        headElem.Type = "bool";
                        break;
                    case JsonValueKind.Null:
                        headElem.Type = "null"; //TODO: Add ? to type ... eventually -- Currently never reached
                        break;
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
                    switch (i) //TODO: prevent repeat scanning, especially on lists of objects
                    {
                        case JsonObject:
                        {
                            isPrim = false;
                            var obj = new Element("Object");
                            SubRecursive(i,elements,obj);
                            headElem.AddChild(obj);
                            break;
                        }
                        case JsonValue:
                        {
                            isObj = false;
                            var val = i.AsValue();
                            primType = GetNumType(i.ToString());
                            var prim = new Element(primType,val.ToString());
                            SubRecursive(i,elements,prim);
                            headElem.AddChild(prim);
                            break;
                        }
                        case JsonArray: //unlikely, but possible JsonArray nesting
                            SubRecursive(i,elements,headElem);
                            break;
                    }
                }

                if (isObj)
                {
                    headElem.Type = $"List<{MakeFriendly(headElem.Name)}>";
                }
                else if (isPrim)
                {
                    headElem.Type = $"List<{primType}>"; //TODO: Change to use a method version of the JsonValue finder thing to get a more accurate list type
                    headElem.ClearChildren();
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
                        if (element.Value == null) continue;
                        var elem = new Element(element.Value.GetValueKind().ToString(), element.Key);
                        if (elements.Contains(elem)) continue;
                        elements.Add(elem);
                        SubRecursive(element.Value, elements, elem);
                    }
                }
                else
                {
                    
                    foreach (var element in jsonObject)
                    {
                        
                        if (element.Value == null) continue; //TODO: Need fix to address null object
                        var name = element.Key;
                        if (headElem.Type.Contains("List")) //TODO: Never reached
                        {
                            name = MakeFriendly(headElem.Name);
                        }
                        var elem = new Element(element.Value.GetValueKind().ToString(), name);
                        if (elements.Contains(elem) && element.Value is not JsonValue) continue;
                        headElem.AddChild(elem);
                        SubRecursive(element.Value, elements, elem);

                    }
                }
                break;
        }
    }

    private static string MakeFriendly(string text)
    {
        if (text[^1].ToString().Equals("s", StringComparison.CurrentCultureIgnoreCase))
        {
            var cap = text[0].ToString().ToUpper();
            text = cap + text.Substring(1, text.Length - 2);
            return text;
        }

        return text;
    }

    private static string GetNumType(string num)
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
}