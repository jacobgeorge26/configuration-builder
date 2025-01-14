using Source.Models.Interfaces;

namespace Source.Models.CheeseModels;

public class Farm : ISettings
{
    public string? Name { get; set; }

    public string? Location { get; set; }
}