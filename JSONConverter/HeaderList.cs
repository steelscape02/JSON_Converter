using JSONConverter.Resources;

namespace JSONConverter;

public class HeaderList()
{
    private Dictionary<string,List<string>> KeyValueData { get; set; } = new();

    public void AddKeyValue(string key, List<string> value)
    {
        KeyValueData.Add(key, value);
    }

    public bool ContainsKey(string? key)
    {
        return key != null && KeyValueData.ContainsKey(key);
    }

    public void Display() //test func
    {
        Console.WriteLine(local.page_div);
        foreach (var item in KeyValueData)
        {
            Console.WriteLine($"{item.Key}: {string.Join(", ", item.Value)}");
        }
    }
}