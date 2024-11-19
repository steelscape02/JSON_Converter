using System.Globalization;
using JSONConverter.Resources;
using System.Resources;
using System.Drawing;

namespace JSONConverter;

//C:\Users\nicho\RiderProjects\JSONConverter\JSONConverter\tester.json

internal abstract class Program
{
    private static void Main(string[] args)
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
        
        var reader = new JSON_Reader(filename);
        reader.GetHeaders();

    }
}