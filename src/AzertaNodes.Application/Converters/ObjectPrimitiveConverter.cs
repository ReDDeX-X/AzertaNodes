using System.Text.Json;
using System.Text.Json.Serialization;

namespace AzertaNodes.Application.Converters
{
    public class ObjectPrimitiveConverter : JsonConverter<object>
    {
        public override object Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return reader.TokenType switch
            {
                JsonTokenType.String => reader.GetString()!,
                JsonTokenType.Number => reader.TryGetInt32(out int i) ? i : reader.GetDouble(),
                JsonTokenType.True => true,
                JsonTokenType.False => false,
                _ => JsonDocument.ParseValue(ref reader).RootElement.Clone()
            };
        }
        public override void Write(Utf8JsonWriter writer, object value, JsonSerializerOptions options)
        {
            JsonSerializer.Serialize(writer, value, value.GetType(), options);
        }
    }
}
