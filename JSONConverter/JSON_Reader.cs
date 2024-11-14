using JSONConverter.Resources;
using System.IO;
using System.Runtime.InteropServices;
using System.Text.Json;

namespace JSONConverter;



public class JSON_Reader(string filename)
{
    public string _filename { get; } = filename;
    private List<Header> _headers = []; //hierarchical tree of headers

    //imaginary text list
    public void GetHeaders(string parent = "") //TODO: recursive run?
    {
        var options = new JsonReaderOptions
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
        var reader = new Utf8JsonReader(File.ReadAllBytes(_filename), options);
        List<string?> headers = [];
        while (reader.Read())
        {
            
            //TODO: Convert to a switch case. Once a 1 depth header is reached, attach all following headers to it (with corresponding tree) until the next 1 header is reached, then repeat.
            if(reader.TokenType == JsonTokenType.PropertyName && reader.CurrentDepth == 1 && 
               !headers.Contains(reader.GetString())) //get all current depth 1 (header) properties. Basically ROOT
            {
                
                //Console.WriteLine($"Pos: {reader.CurrentDepth}\nName: {reader.GetString()}"); //print header pos and name
                headers.Add(reader.GetString());
            }

            if (reader.TokenType == JsonTokenType.PropertyName && reader.CurrentDepth == 2 &&
                !headers.Contains(reader.GetString()))
            {
                Console.WriteLine($"Parent: {reader.Position.GetInteger()}");
            }
        }

        //TODO: 
    }
}