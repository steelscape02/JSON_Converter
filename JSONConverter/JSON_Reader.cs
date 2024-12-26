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
    
    /// <summary>
    /// Recursively iterate through a JSON tree from a specified header (set initially as <c>current</c>)
    /// </summary>
    /// <param name="current">Starting header</param>
    /// <param name="headers">A list of headers with associated child objects</param>
    /// <param name="currHeader">The current header to work on.</param>
    /// <exception cref="Exception"></exception>
    private static void SubRecursive(JsonNode current, List<Header> headers, Header currHeader)
    {
        switch (current)
        {
            //base case
            case JsonValue jsonValue: //TODO: Add type thing for instances where currHeader only appears once (no children)
            {
                var type = jsonValue.GetValueKind().ToString();
                if (!Header.SubPresent()) break;
                
                currHeader.ChangeType(type); //set primitive for the last element in currHeader's subHeads list (which would be the last guy added)

                
                
                if(!headers.Contains(currHeader))
                    headers.Add(currHeader);
                break;
            }
            case JsonArray jsonArray:
            {
                var isObj = true;
                if (currHeader == null)
                {
                    throw new Exception("currHeader is null on an array");
                }
                foreach (var i in jsonArray)
                {
                    switch (i) //cannot be an array, so check Prim and 
                    {
                        case JsonValue jValue:
                            currHeader.Type = $"List<{jValue.GetValueKind().ToString()}>";
                            isObj = false;
                            break;
                        case JsonObject jObj:
                            SubRecursive(jObj, headers, currHeader);
                            break;
                    }

                    if (isObj)
                    {
                        currHeader.Type = $"List<{currHeader.Name}>";
                    }
                }

                break;
            }
            case JsonObject jsonObject:
            {
                currHeader.Type = currHeader.Name;
                foreach (var j in jsonObject)
                {
                    if (j.Value == null)
                    {
                        if(currHeader.HasSubHead(j.Key))
                            currHeader.AddNull(j.Key);
                        return;
                    }
                    switch (j.Value)
                    {
                        case JsonObject: 
                            var jObjHead = new Header(j.Key);
                            currHeader.AddChild(jObjHead); //TODO: Ends up adding all following headers to their own children, messing up things
                            //Console.WriteLine("A man has fallen in LEGO city");
                            SubRecursive(j.Value, headers, jObjHead);
                            break;
                        case JsonArray:
                            currHeader.AddSubHeads(j.Key, "Array"); //TODO: Add array type (maybe add a method in Header)
                            SubRecursive(j.Value, headers, currHeader);
                            break;
                        case JsonValue jValue: //catch primitive object members
                            currHeader.AddSubHeads(j.Key, jValue.GetValueKind().ToString());
                            SubRecursive(j.Value, headers, currHeader);
                            break;
                    }

                    
                    
                }
                break;
            }
        }
    }

    private static void SubRecursive4(JsonNode current, List<Element> elements, Element? headElem)
    {
        switch (current)
        {
            case JsonValue jsonValue: //TODO: add type
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
                        
                        SubRecursive4(element.Value, elements, elem);
                        elements.Add(elem);
                    }
                }
                else
                {
                    List<string> known = [];
                    foreach (var element in jsonObject) //if the name exists multiple times, reject
                    {
                        
                        if (element.Value == null) continue; //TODO: Need fix to address null object
                        if (known.Contains(element.Key)){
                            Console.WriteLine("OH!"); //12.26.2024 - Never reached
                            continue; //keep for rejecting repeat item
                        }
                        
                        known.Add(element.Key);
                        var elem = new Element(element.Value.GetValueKind().ToString(), element.Key);
                        headElem.AddChild(elem);
                        SubRecursive4(element.Value, elements, headElem);

                    }
                }
                
                break;
        }
    }
    
    private static void SubRecursive3(JsonNode current, List<Element> elements, Element? headElem)
    {
        //TODO: Rework to capture all occurrences. This will be used to nullify prims and show arrays (and their types)
        Element? curr;
        switch (current)
        {
            case JsonValue jsonValue: //lowest level, but not base case (best to exit at parent object)
                var type = jsonValue.GetValueKind().ToString();
                var prim = new Element(type,jsonValue.ToString());
                headElem.AddChild(prim);
                //TODO: Check headElem children. If all prim (3 prim types), run ChangeType() to alter to respective primitive array type
                
                break;
            case JsonArray jsonArray:
                if (headElem == null)
                {
                    throw new Exception("Parent is null on an array");
                }
                //set array type later
                curr = new Element("Array");
                headElem.AddChild(curr);
                foreach (var i in jsonArray) //keep current headElem and run all children under the array
                {
                    if (i != null) SubRecursive3(i, elements, curr);
                }
                
                
                break;
            case JsonObject jsonObject:
                
                //TODO: Check headElem children. If all obj, run ChangeType() to alter to list of respective object
                var e = 0;
                var found = false;
                foreach (var i in jsonObject) //create new headElem for obj, run all children under
                {
                    
                    if (e == 0) //create headElem at first index
                    {
                        if (headElem == null)
                        {
                            headElem = new Element("Object",i.Key); //create new headElem for null
                        }
                        else
                        {
                            curr = new Element("Object",i.Key); //update existing headElem
                            headElem.AddChild(curr);
                            
                            headElem = curr;
                        }
                    }
                    SubRecursive3(i.Value,elements,headElem); //TODO: Handle possible i.Value null
                    e += 1;
                }
                
                //Base Case
                //TODO: Make this read seq to check that HeadElem's name is not on the list (will create errors, but that's somethin else
                found = elements.Any(i => i.Name == headElem.Name);

                if (!found)
                {
                    elements.Add(headElem);
                }
                break;
        }
    }
}