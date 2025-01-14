using SettingsBuilder.Models;

namespace SettingsBuilder.SettingsBuilder;

public class CheeseSettings : IOverridable, ISettingsRoot
{
    public Cheese? Cheese { get; set; }
}