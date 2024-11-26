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
        //Console.WriteLine(string.Join('\n', headers));
        var document = JsonNode.Parse(reader) ?? "Blah"; //funny fallback added
        var root = document.Root;
        // var options = new JsonSerializerOptions { WriteIndented = true }; 
        // Console.WriteLine(root.ToJsonString(options));
        
        var TreeRoot = new TreeNode<string>("Root");
        foreach (var i in headers) //replace with a recursive print method
        {
            if (i != null)
            {
                var secondRoot = new TreeNode<string>(i);
                var temp = root[i]; //lookup guy
                
                SubRecursive(temp, secondRoot);
                TreeRoot.AddChild(secondRoot);
            }
        }
        // Console.WriteLine(local.page_div); //Just for debugging
        // Console.WriteLine("Root: " + TreeRoot);
        // Console.WriteLine("Children of Root:");
        // foreach (var child in TreeRoot.Children) 
        // {
        //     if (child.Children.Count >= 0)
        //     {
        //         Console.WriteLine($"  Children of {child}");
        //         foreach (var i in child.Children)
        //         {
        //             Console.WriteLine( "    Key: " + i);
        //             Console.WriteLine($"    Count: {i.Children.Count}");
        //         }
        //     }
        //     else
        //         Console.WriteLine("  " + child);
        // }
    }
    
    private static void SubRecursive(JsonNode current, TreeNode<string> parent, TreeNode<string>? currChild = null) //needs a hierarchal data struct (maybe a HeaderList data struct again?)
    {
        if (current is not JsonArray && current is not JsonObject) //base case
        {
            //demo for SubRecursive2, or a rebuild of this one
            //Print the path, which shows all of the elements up to that point. From here you can trace the object type (primitive, list, etc..) that is needed
            
            return;
        }

        if (current is JsonArray)
        {

            foreach (var i in current.AsArray())
            {
                
                if (i == null) continue;
                //check if prim
                if (i is JsonArray)
                {
                    foreach (var j in i.AsArray())
                    {
                        if(j == null) continue;
                        var subNode = new TreeNode<string>(j?.ToString() ?? string.Empty);
                        if(currChild is null)
                            parent.AddChild(subNode);
                        else
                        {
                            if(!currChild.Children.Contains(subNode)) //TODO: Fix this atrocity
                                currChild.AddChild(subNode);
                        }
                        SubRecursive(j, parent, subNode);
                    }
                }

                if (i is JsonObject)
                {
                    foreach (var j in i.AsObject())
                    {
                        if (j.Value == null) continue;
                        var subNode = new TreeNode<string>(j.Key);
                        //if (currChild.Children.Contains(subNode)) continue;
                        if(currChild is null)
                            parent.AddChild(subNode);
                        else
                        {
                            if(!currChild.Children.Contains(subNode))
                                currChild.AddChild(subNode);
                        }
                            
                        SubRecursive(j.Value, parent, subNode);
                        
                    }
                }
                // else
                // {
                //     var subNode = new TreeNode<string>(i?.ToString() ?? string.Empty);
                //     if(currChild is null)
                //         parent.AddChild(subNode);
                //     else
                //         currChild.AddChild(subNode);
                //     SubRecursive(i, parent, subNode);
                // }
                

            }
        }
        else if (current is JsonObject)
        {
            foreach (var i in current.AsObject())
            {
                if (i.Value == null) continue;
                var subNode = new TreeNode<string>(i.Key);
                if(currChild is null)
                    parent.AddChild(subNode);
                else
                    currChild.AddChild(subNode);
                SubRecursive(i.Value, parent, subNode);
            }
        }
    }
    
    //**REPLACEMENT TO SubRecursive IF IT WORKS**
    
    //look through list and use JSONNode's GetPath attribute. Drill through until you find a primitive, check if that primitive is already in a list,
    //if it isn't in the list, attach the path and variable type (perhaps split and add, to construct a cute TreeNode), then construct a tree
    
    //will need a second recursive function to parse the returned list and make a tree
    private static void SubRecursive2(JsonNode current, List<Header> headers, Header? currHeader = null)
    { 
        if (current is JsonValue jsonValue) //base case
        {
            var type = jsonValue.GetValueKind();
            currHeader.ChangeType(type.ToString()); //set primitive for the last element in currHeader's subHeads list (which would be the last guy addeed)
            if (currHeader != null) headers.Add(currHeader);
        }
        else if (current is JsonArray jsonArray)
        {
            var isPrim = true; //check if array is just primitives, if so the item should NOT be added
            if (currHeader == null)
            {
                throw new Exception("currHeader is null on an array");
            }
            foreach (var i in jsonArray)
            {
                if (i is null)
                {
                    isPrim = false;
                    continue;
                }
                if (i is JsonObject jObj)
                {
                    isPrim = false;
                    foreach (var j in jObj)
                    {
                        if (j.Value == null)
                        {
                            if (currHeader.HasSubHead(j.Key))
                            {
                                currHeader.AddNull(j.Key);
                            }
                        }
                        else
                        {
                            SubRecursive2(j.Value, headers, currHeader);
                        }
                    }
                }
            }
        }
        else if (current is JsonObject jsonObject) //TODO: Build jsonObject recursion
        {
            
        }
    }
}