using System.Buffers;
using System.Data;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using JSONConverter.Resources;

namespace JSONConverter;



public class JSON_Reader(string filename)
{
    public string _filename { get; } = filename;

    //imaginary text list
    public void GetHeaders(string parent = "") //TODO: recursive run?
    {
        var opts = new JsonReaderOptions
        {
            AllowTrailingCommas = true,
            CommentHandling = JsonCommentHandling.Skip
        };
        if (parent != "") //special behavior for op param
        {
            //TODO: read json FROM given parent (if present) and add to headers as a child
            return;
        }
        //TODO: read json and capture unique headers as parents. If children are present run again to capture child headers
        var reader = File.ReadAllText(_filename);
        string? currHeader = null;
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
        foreach (var i in headers)
        {
            if (i != null)
            {
                var secondRoot = new TreeNode<string>(i);
                var temp = root[i]; //lookup guy
                
                SubRecursive(temp, secondRoot);
                TreeRoot.AddChild(secondRoot);
            }
        }
        Console.WriteLine(local.page_div); //Just for debugging
        Console.WriteLine("Root: " + TreeRoot);
        Console.WriteLine("Children of Root:");
        foreach (var child in TreeRoot.Children) 
        {
            if (child.Children.Count >= 0)
            {
                Console.WriteLine($"  Children of {child}");
                foreach (var i in child.Children)
                {
                    Console.WriteLine("    Key: " + i);
                }
            }
            else
                Console.WriteLine("  " + child);
        }
        
        
        //TODO: 
    }
    
    private void SubRecursive(JsonNode current, TreeNode<string> parent, TreeNode<string>? currChild = null) //needs a hierarchal data struct (maybe a HeaderList data struct again?)
    {
        if (current is not JsonArray && current is not JsonObject) //base case
        {
            var finalNode = new TreeNode<string>(current.ToString());//current _should_ be a primitive here
            if (currChild is null)
                parent.AddChild(finalNode);
            else
                currChild.AddChild(finalNode);
        }

        if (current is JsonArray)
        {

            foreach (var i in current.AsArray())
            {
                var subNode = new TreeNode<string>(i?.ToString() ?? string.Empty);
                if(currChild is null)
                    parent.AddChild(subNode);
                else
                    currChild.AddChild(subNode);
                if (i != null) SubRecursive(i, parent, subNode);
            }
        }
        else if (current is JsonObject)
        {
            foreach (var i in current.AsObject())
            {
                var subNode = new TreeNode<string>(i.Key);
                if(currChild is null)
                    parent.AddChild(subNode);
                else
                    currChild.AddChild(subNode);
                if (i.Value != null) SubRecursive(i.Value, parent, subNode);
            }
        }
    }
}