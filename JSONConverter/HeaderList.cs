namespace JSONConverter;

public class HeaderList(string name)
{
    private Dictionary<string,List<string>> KeyValueData { get; set; } = new();

    public string Name { get; set; } = name;

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
        foreach (var item in KeyValueData)
        {
            Console.WriteLine($"{item.Key}: {string.Join(", ", item.Value)}");
        }
    }
}