using System.Text.Json;
using Soenneker.Blazor.TomSelect;

var configuration = JsonSerializer.Deserialize("{}", LibraryJsonContext.Default.TomSelectConfiguration)!;
var payload = JsonSerializer.SerializeToElement(configuration, LibraryJsonContext.Default.TomSelectConfiguration);
Check(payload.ValueKind == JsonValueKind.Object, "configuration wire object");
var options = LibraryJsonContext.WithContext(SmokeJsonContext.Default);
var custom = JsonSerializer.SerializeToElement<object>(new SmokePayload { Value = "custom" }, (System.Text.Json.Serialization.Metadata.JsonTypeInfo<object>)options.GetTypeInfo(typeof(object)));
Check(custom.GetProperty("value").GetString() == "custom", "application-generated metadata");
var items = JsonSerializer.Deserialize("""[{"value":"one","text":"First","item":{"id":7}}]""", LibraryJsonContext.Default.IEnumerableTomSelectOption)!;
Check(JsonSerializer.SerializeToElement(items, LibraryJsonContext.Default.IEnumerableTomSelectOption)[0].GetProperty("item").GetProperty("id").GetInt32() == 7, "option item graph");

Console.WriteLine("Trimmed JSON smoke checks passed.");

static void Check(bool condition, string name)
{
    if (!condition) throw new InvalidOperationException(name);
}
