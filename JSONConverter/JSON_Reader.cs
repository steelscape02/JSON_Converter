using System.Buffers;
using System.Data;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace JSONConverter;



public class JSON_Reader(string filename)
{
    public string _filename { get; } = filename;
    private List<HeaderList> _headers = []; //hierarchical tree of headers

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
        JsonNode document = JsonNode.Parse(reader) ?? "Blah"; //funny fallback added
        JsonNode root = document.Root;
        var options = new JsonSerializerOptions { WriteIndented = true }; 
        //Console.WriteLine(root.ToJsonString(options));
        JsonNode? temp; //lookup guy
        
        foreach (var i in headers)
        {
            if (i != null)
            {
                temp = root[i];
                //Console.WriteLine(temp.GetType());
                if (temp.GetType() == typeof(JsonArray)) //TODO: When Recursive tree method is built, only these type checks should exist with a call and return from the func.
                {
                    List<JsonNode> uniqueHeaders = [];
                    List<string> subheads = [];//😊
                    foreach (var header in temp.AsArray()) //TODO: Replace all of this with a recursive method to search through ALL subtrees until no more are present
                    {
                        //Console.WriteLine($"Header: {header.GetType()}"); //var type test
                        if (header.GetType() == typeof(JsonObject))
                        {
                            
                            foreach (var e in header.AsObject())
                            {
                                if (!subheads.Contains(e.Key))
                                {
                                    Console.WriteLine(e.Key);
                                    if (e.Value != null && e.Value.GetType() == typeof(JsonObject))
                                    {
                                        foreach (var r in e.Value.AsObject())
                                        {
                                            Console.WriteLine($"Subhead: {r.Key}");
                                        }
                                    }
                                    else
                                    {
                                        Console.WriteLine($"Subhead Primitive: {e.Value}");
                                    }
                                    subheads.Add(e.Key);
                                }
                                
                            }
                        }
                        else
                        {
                            if (!uniqueHeaders.Contains(header))
                            {
                                uniqueHeaders.Add(header);
                            }
                        }
                    }
                    //Console.WriteLine(string.Join(", ",temp.AsArray()));
                    //Console.WriteLine($"Array Found: {string.Join(", ", uniqueHeaders)}");
                }
                else if (temp.GetType() == typeof(JsonObject)) //TODO: split into smaller objs
                {
                    
                    //Console.WriteLine($"Object found: {string.Join(", ",uniqueHeaders)}");
                    Console.WriteLine("Object found");
                }
                else
                {
                    Console.WriteLine($"Primitive: {temp}");
                }
            }
        }

        
        

        //TODO: 
    }
    /// <summary>
    /// Recursively runs through a given JSON document and finds all subtrees, attaching them to a growing list
    /// </summary>
    /// <param name="headers"></param>
    /// <returns></returns>
    private List<string> SubRecursive(List<string> headers) //needs a hierarchal data struct (maybe a HeaderList data struct again?)
    {
        
        return new List<string>(); //temp
    }
}