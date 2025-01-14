using System.Text.Json;
using System.Text.Json.Serialization;

namespace SettingsBuilder.Helpers;

public static class JsonHelpers
{
    public static JsonSerializerOptions JsonSerializerOptions => new()
    {
        PropertyNameCaseInsensitive = true,
        AllowTrailingCommas = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        WriteIndented = true
    };
}