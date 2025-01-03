using System.Text.Json;
using JSONConverter.Resources;

namespace JSONConverter;

internal abstract class Program
{
    private static void Main()
    {
        Console.WriteLine(Local.title);
    
        //get JSON file info
        var fileFound = false;
        var filename = "";
        while (!fileFound)
        {
            //Console.Write(local.enter_json_name);
            //filename = Console.ReadLine();
            filename = "C:\\Users\\nicho\\RiderProjects\\JSONConverter\\JSONConverter\\tester.json";
            fileFound = File.Exists(filename);
        }
        
        var reader = new JsonReader(filename);
        var jsonTree = reader.ReadJson();
        var display = new DomCreator();
        Console.WriteLine(display.BuildRoot(jsonTree));
        var contents = File.ReadAllText(filename);
        //TODO: Not catching all nulls (i think ones that first had a value, then went null), and issues with int precision (first is an int, later a double)
        //Root? root = JsonSerializer.Deserialize<Root>(contents);
        //Console.WriteLine(root.type);
    }
}