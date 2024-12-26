using System.Net;
using System.Text.Json;
using System.Text.Json.Nodes;
using JSONConverter.Resources;

namespace JSONConverter;

public class JsonReader(string filename)
{
    private string Filename { get; } = filename;

    //imaginary text list
    public void GetHeaders()
    {
        var reader = File.ReadAllText(Filename);
        
        var document = JsonNode.Parse(reader) ?? "Blah"; //funny fallback added
        var root = document.Root; //root is JSON Object
        var baseStuff = new List<Element>();
        SubRecursive4(root,baseStuff,null);
        
        Console.WriteLine(local.page_div); //Just for debugging
        foreach (var element in baseStuff)
        {
            
            Console.WriteLine(element.Summary());
        }
    }
    
    private static void SubRecursive4(JsonNode current, List<Element> elements, Element? headElem)
    {
        switch (current)
        {
            case JsonValue jsonValue: //TODO: fix to prevent repeat running (currently will repeat over whole array or other item)
                
                if(headElem == null)
                    throw new ArgumentNullException(headElem.ToString());
                if (elements.Contains(headElem)) break;
                if (headElem.Type == "Object") break; //prevent overwriting complex structure naming
                switch (jsonValue.GetValueKind())
                {
                    case JsonValueKind.String:
                        headElem.ChangeType("string");
                        break;
                    case JsonValueKind.Number:
                        if (int.TryParse(jsonValue.ToString(), out _))
                        {
                            headElem.ChangeType("int");
                        }
                        else if (long.TryParse(jsonValue.ToString(), out _))
                        {
                            headElem.ChangeType("long");
                        }
                        else if (double.TryParse(jsonValue.ToString(), out _))
                        {
                            headElem.ChangeType("double");
                        }
                        else if (decimal.TryParse(jsonValue.ToString(), out _))
                        {
                            headElem.ChangeType("decimal");
                        }
                        
                        break;
                    case JsonValueKind.True:
                    case JsonValueKind.False:
                        headElem.ChangeType("bool");
                        break;
                    case JsonValueKind.Null:
                        headElem.ChangeType("null");
                        break;
                    case JsonValueKind.Undefined:
                    case JsonValueKind.Object:
                    case JsonValueKind.Array:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(headElem.ToString());
                }
                break;
            case JsonArray jsonArray:
                
                var isObj = true;
                var isPrim = true;
                var primType = "";
                
                foreach (var i in jsonArray)
                {
                    switch (i) //TODO: prevent repeat scanning, especially on lists of objects
                    {
                        case JsonObject:
                        {
                            isPrim = false;
                            var obj = new Element("Object");
                            SubRecursive4(i,elements,obj);
                            headElem.AddChild(obj);
                            break;
                        }
                        case JsonValue:
                        {
                            isObj = false;
                            var val = i.AsValue();
                            primType = val.GetValueKind().ToString();
                            var prim = new Element(primType,val.ToString());
                            SubRecursive4(i,elements,prim);
                            headElem.AddChild(prim);
                            break;
                        }
                        case JsonArray: //unlikely, but possible JsonArray nesting
                            SubRecursive4(i,elements,headElem);
                            break;
                    }
                }

                if (isObj)
                {
                    headElem.ChangeType($"List<{headElem.Name}>");
                }
                else if (isPrim)
                {
                    headElem.ChangeType($"List<{primType}>");
                }
                else
                {
                    headElem.ChangeType("List<Mixed>"); //TODO: Make this a little more useful
                }
                
                break;
            case JsonObject jsonObject:
                //TODO: Regard objects as List<Object>'s and indv objects (if you see an object next, it's not a list, if an array, it's a list of the obj)
                //TODO: Track number of occurrences of each item
                    //TODO: Or just track obj lists (as enclosed repeating items HAVE to be enclosed first)
                
                if (headElem == null) //start point (basically base case)
                {
                    foreach (var element in jsonObject)
                    {
                        //first iter (from root, so just grab all keys)
                        var elem = new Element(element.Value.GetValueKind().ToString(), element.Key);
                        if (elements.Contains(elem)) continue;
                        SubRecursive4(element.Value, elements, elem);
                        elements.Add(elem);
                    }
                }
                else
                {
                    foreach (var element in jsonObject)
                    {
                        
                        if (element.Value == null) continue; //TODO: Need fix to address null object
                        
                        var elem = new Element(element.Value.GetValueKind().ToString(), element.Key);
                        if (elements.Contains(elem)) continue;
                        headElem.AddChild(elem);
                        SubRecursive4(element.Value, elements, headElem);

                    }
                }
                break;
        }
    }
}