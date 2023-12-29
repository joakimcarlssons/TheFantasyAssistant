using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using System.Text;

namespace TFA.Utils;

public static class JsonUtils
{
    public static StringContent AsJsonBody<T>(this T data)
        => new(JsonSerializer.Serialize(data, new JsonSerializerOptions
        {
            Encoder = JavaScriptEncoder.Create(UnicodeRanges.All)
        }), Encoding.UTF8, "application/json");

    public static T? GetJsonElementValue<T>(this JsonElement element)
    {
        switch (element.ValueKind)
        {
            case JsonValueKind.Number:
                {
                    if (TypeUtils.IsType<T, int>())
                        return (T)(object)element.GetInt32();
                    if (TypeUtils.IsType<T, int?>())
                        return (T)(object)element.GetInt32();
                    if (TypeUtils.IsType<T, decimal>())
                        return (T)(object)element.GetDecimal();
                    if (TypeUtils.IsType<T, decimal?>())
                        return (T)(object)element.GetDecimal();
                    break;
                }
            case JsonValueKind.String:
                return (T)(object)element.GetString().NullIsEmpty();
        }

        throw new NotImplementedException($"Mapping is missing for JsonElement type {element.ValueKind}");
    }
}
