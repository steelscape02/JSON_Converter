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
            Console.Write(Local.enter_json_name);
            filename = Console.ReadLine();
            var currDir = Directory.GetCurrentDirectory();
            var projectDir = Directory.GetParent(currDir)?.Parent?.Parent?.FullName; //pull back 3 levels to project folder (from net bin)
            if (filename == null) continue;
            
            if (projectDir != null) filename = Path.Combine(projectDir, filename);
            fileFound = File.Exists(filename);
        }

        if (filename != null)
        {
            var reader = new JsonReader(filename);
            var jsonTree = reader.ReadJson();
        
            Console.WriteLine(CSharpDm.BuildRoot(jsonTree));
            var contents = File.ReadAllText(filename);
        }

        
    }
}