using System.Text.Json;

namespace AzertaNodes.Core.Entities.Node
{
    public class NodeExecutionResult
    {
        public object? Value { get; private set; }
        public bool IsSkipped { get; private set; }
        public Type? ValueType => Value?.GetType();

        private NodeExecutionResult(object? value, bool isSkipped)
        {
            Value = value;
            IsSkipped = isSkipped;
        }

        public static NodeExecutionResult Create(object? value)
        {
            if (value is null) return CreateNullResult();
            if (value is JsonElement jsonElement) value = ExtractValue(jsonElement);
            if (value is NodeExecutionResult) throw new NodeSelfCreationException("Cannot create a NodeExecutionResult from another NodeExecutionResult.");
            return new NodeExecutionResult(value, false);
        }

        public static NodeExecutionResult CreateNullResult()
        {
            return new NodeExecutionResult(null, false);
        }
        public static NodeExecutionResult Skipped()
        {
            return new NodeExecutionResult(null, true);
        }
        public T? GetValue<T>()
        {
            if (IsSkipped) throw new InvalidOperationException("Cannot get value from a skipped NodeValue.");
            if (Value == null) return default;
            if (Value is T typedValue) return typedValue;
            throw new NodeInvalidValueException($"NodeValue contains a value of type {ValueType}, which cannot be cast to {typeof(T)}.");
        }

        private static object? ExtractValue(JsonElement element)
        {
            return element.ValueKind switch
            {
                JsonValueKind.String => element.GetString(),
                JsonValueKind.Number => element.TryGetInt64(out long l) ? l : element.GetDouble(),
                JsonValueKind.True => true,
                JsonValueKind.False => false,
                JsonValueKind.Null => null,
                JsonValueKind.Object or JsonValueKind.Array => element.Clone(),
                _ => element.GetRawText()
            };
        }
    }

    public class NodeSelfCreationException(string message) : Exception(message)
    {

    }
    public class NodeInvalidValueException(string message) : Exception(message)
    {

    }
}
