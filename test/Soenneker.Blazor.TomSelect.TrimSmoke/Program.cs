using System.Text.Json;
using Soenneker.Blazor.TomSelect;

Check(!JsonSerializer.IsReflectionEnabledByDefault, "JSON reflection disabled");

var configuration = JsonSerializer.Deserialize("{}", LibraryJsonContext.Default.TomSelectConfiguration)!;
var payload = JsonSerializer.SerializeToElement(configuration, LibraryJsonContext.Default.TomSelectConfiguration);
Check(payload.ValueKind == JsonValueKind.Object, "configuration wire object");
var options = LibraryJsonContext.WithContext(SmokeJsonContext.Default);
var custom = JsonSerializer.SerializeToElement<object>(new SmokePayload { Value = "custom" }, (System.Text.Json.Serialization.Metadata.JsonTypeInfo<object>)options.GetTypeInfo(typeof(object)));
Check(custom.GetProperty("value").GetString() == "custom", "application-generated metadata");
var items = JsonSerializer.Deserialize("""[{"value":"one","text":"First","item":{"id":7}}]""", LibraryJsonContext.Default.IEnumerableTomSelectOption)!;
Check(JsonSerializer.SerializeToElement(items, LibraryJsonContext.Default.IEnumerableTomSelectOption)[0].GetProperty("item").GetProperty("id").GetInt32() == 7, "option item graph");

IEnumerable<string> values = Enumerable.Range(1, 2).Select(value => $"item-{value}");
var valuesPayload = JsonSerializer.SerializeToElement(values, LibraryJsonContext.Default.IEnumerableString);
Check(valuesPayload.GetArrayLength() == 2 && valuesPayload[0].GetString() == "item-1" && valuesPayload[1].GetString() == "item-2", "iterator-backed item values");
var emptyValues = JsonSerializer.SerializeToElement(Enumerable.Empty<string>(), LibraryJsonContext.Default.IEnumerableString);
Check(emptyValues.GetArrayLength() == 0, "empty item values");
var customOption = new Soenneker.Blazor.TomSelect.Dtos.TomSelectOption
{
    Value = "custom",
    Text = "Custom",
    Item = new SmokePayload { Value = "nested" }
};
var optionTypeInfo = (System.Text.Json.Serialization.Metadata.JsonTypeInfo<Soenneker.Blazor.TomSelect.Dtos.TomSelectOption>)options.GetTypeInfo(typeof(Soenneker.Blazor.TomSelect.Dtos.TomSelectOption));
var optionPayload = JsonSerializer.SerializeToElement(customOption, optionTypeInfo);
Check(optionPayload.GetProperty("item").GetProperty("value").GetString() == "nested", "nested application-generated metadata");
var roundTrip = optionPayload.Deserialize(optionTypeInfo)!;
Check(roundTrip.Value == "custom" && roundTrip.Text == "Custom", "typed DTO deserialization");

Console.WriteLine("Trimmed JSON smoke checks passed.");

static void Check(bool condition, string name)
{
    if (!condition) throw new InvalidOperationException(name);
}
