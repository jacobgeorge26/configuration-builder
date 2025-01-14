using SettingsBuilder.Models;

namespace SettingsBuilder.SettingsBuilder;

public class CheeseSettings : IFileModel, ISettings
{
    public Cheese? Cheese { get; set; }
}