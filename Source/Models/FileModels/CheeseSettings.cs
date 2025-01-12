using Source.Models.CheeseModels;

namespace Source.Models.FileModels;

public class CheeseSettings : IFileModel
{
    public Cheese? Cheese { get; init; }
}