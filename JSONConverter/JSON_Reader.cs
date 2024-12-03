using System.Text.Json;
using System.Text.Json.Nodes;
using JSONConverter.Resources;

namespace JSONConverter;



public class JSON_Reader(string filename)
{
    public string _filename { get; } = filename;

    //imaginary text list
    public void GetHeaders()
    {
        var opts = new JsonReaderOptions
        {
            AllowTrailingCommas = true,
            CommentHandling = JsonCommentHandling.Skip
        };
        
        var reader = File.ReadAllText(_filename);
        List<string?> headers = [];
        var headerRead = new Utf8JsonReader(File.ReadAllBytes(_filename), opts);
        while (headerRead.Read()) //get all possible headers
        {
            if(headerRead.TokenType == JsonTokenType.PropertyName && headerRead.CurrentDepth == 1 &&
               !headers.Contains(headerRead.GetString())) //get all current depth 1 (header) properties. Basically ROOT
            {
                headers.Add(headerRead.GetString());
            }
        }
        var document = JsonNode.Parse(reader) ?? "Blah"; //funny fallback added
        var root = document.Root;
        
        var baseStuff = new List<Header>();
        foreach (var i in headers) //replace with a recursive print method
        {
            if (i == null) continue;
            var temp = root[i]; //lookup guy
            var tempHeader = new Header(i);
            if (temp != null) SubRecursive2(temp, baseStuff, tempHeader);
            baseStuff.Add(tempHeader);
        }
        Console.WriteLine(local.page_div); //Just for debugging
        foreach (var child in baseStuff) 
        {
            child.Display();
        }
    }
    
    private static void SubRecursive2(JsonNode current, List<Header> headers, Header currHeader)
    {
        switch (current)
        {
            //base case
            case JsonValue jsonValue: //TODO: Add type thing for instances where currHeader only appears once (no children)
            {
                var type = jsonValue.GetValueKind().ToString();
                if (!currHeader.SubPresent()) break;
                
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
                            SubRecursive2(jObj, headers, currHeader);
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
                        case JsonObject: //TODO: Add a new currHeader to this and run as a separate SubRecur
                            currHeader.AddSubHeads(j.Key,"Object");
                            break;
                        case JsonArray jArr:
                            currHeader.AddSubHeads(j.Key, "Array");
                            break;
                        case JsonValue jValue: //catch primitive object members
                            currHeader.AddSubHeads(j.Key, jValue.GetValueKind().ToString());
                            break;
                    }

                    currHeader.Type = currHeader.Name;
                    SubRecursive2(j.Value, headers, currHeader);
                }
                break;
            }
        }
    }
}