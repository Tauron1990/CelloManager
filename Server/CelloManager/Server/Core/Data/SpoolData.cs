namespace CelloManager.Avalonia.Core.Data;

public sealed record SpoolData(string Id, string Name, string Category, int Amount, int NeedAmount)
{
    public static string CreateId(string name, string category)
        => $"{name}-{category}";
    
    public static SpoolData New(string name, string category, int amount) 
        => new(CreateId(name, category), name, category, amount, -1);
}