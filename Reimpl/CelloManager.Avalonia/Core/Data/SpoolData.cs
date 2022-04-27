using Akavache;

namespace CelloManager.Avalonia.Core.Data;

public sealed record SpoolData(string Id, string Name, string Category, int Amount, int NeedAmount)
{
    public static SpoolData New(string name, string category, int needAmount) 
        => new($"{name}-{category}", name, category, needAmount, -1);
}