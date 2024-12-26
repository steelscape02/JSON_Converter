using JSONConverter.Resources;

namespace JSONConverter;

internal abstract class Program
{
    private static void Main()
    {
        Console.WriteLine(local.title);
    
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
        reader.GetHeaders();
        
        

    }
}