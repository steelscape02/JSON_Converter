namespace JSONConverter;

public class Header(string name)
{
    private Dictionary<string,string> KeyValueData { get; set; } = new();

    public string Name { get; set; } = name;

    public void AddKeyValue(string key, string value)
    {
        KeyValueData.Add(key, value);
    }
}