using System.Text.Json;

namespace CluedIn.Contrib.Extensions;

public static class JsonExtensions
{
    /// <summary>
    ///     Converts a JsonDocument into a Dictionary&gt;string, string&lt;,
    ///     where each key represents a property path,
    ///     and value is a stingified JSON object value.
    ///     So a JSON object { "a": { "b": 1 } } is converted to { "a.b", "1" }.
    ///     Arrays are flattened with indexed: { "a": [ { "b": "b0" }, { "b": "b1" } ] } ->
    ///     { { "a.0.b", "b0" }, { "a.1.b", "b1" } }
    /// </summary>
    /// <param name="jsonDocument"></param>
    /// <returns></returns>
    public static Dictionary<string, string> Flatten(this JsonDocument jsonDocument)
    {
        var result = new Dictionary<string, string>();

        FlattenJsonElement(jsonDocument.RootElement.Clone(), result, string.Empty);

        return result;
    }

    private static void FlattenJsonElement(JsonElement element, IDictionary<string, string> dict, string prefix)
    {
        // ReSharper disable once SwitchStatementHandlesSomeKnownEnumValuesWithDefault
        switch (element.ValueKind)
        {
            case JsonValueKind.Object:
                foreach (var property in element.EnumerateObject())
                {
                    var propName = string.IsNullOrEmpty(prefix) ? property.Name : $"{prefix}.{property.Name}";
                    FlattenJsonElement(property.Value, dict, propName);
                }

                break;
            case JsonValueKind.Array:
                var index = 0;
                foreach (var item in element.EnumerateArray())
                {
                    FlattenJsonElement(item, dict, $"{prefix}.{index}");
                    index++;
                }

                break;
            default:
                dict[prefix] = element.ToString();
                break;
        }
    }
}
