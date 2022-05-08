using System.Text.Json;

namespace OctopusUsage.Extensions;

public static class SerializationExtensions
{
    private static readonly JsonSerializerOptions DefaultSerializerSettings = new()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true,
    };

    public static T? Deserialize<T>(this string json) => JsonSerializer.Deserialize<T>(json, DefaultSerializerSettings);

    public static T? DeserializeCustom<T>(this string json, JsonSerializerOptions settings) => JsonSerializer.Deserialize<T>(json, settings);

    public static string Serialize<T>(this T item)
    {
        Guard.Against.Null(item, nameof(item));
        return JsonSerializer.Serialize(item, DefaultSerializerSettings);
    }

    public static string SerializeCustom<T>(this T item, JsonSerializerOptions settings)
    {
        Guard.Against.Null(item, nameof(item));
        return JsonSerializer.Serialize(item, settings);
    }
}