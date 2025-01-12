using Source.Models.Enums;

namespace Source.Models;

public class Cheese : ISettings
{
    public string Name { get; init; } = null!;
    
    public decimal Price { get; init; }
    
    public List<string> Flavours { get; init; } = [];
    
    public Milk? Milk { get; init; }
    
    public Farm? Origin { get; init; }
}