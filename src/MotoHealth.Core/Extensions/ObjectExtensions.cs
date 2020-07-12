using System.Text.Json;

namespace MotoHealth.Core.Extensions
{
    public static class ObjectExtensions
    {
        public static TObject Clone<TObject>(this TObject value)
            where TObject : notnull
        {
            var objectType = value.GetType();

            var serialized = JsonSerializer.SerializeToUtf8Bytes(value, objectType);
            return (TObject)JsonSerializer.Deserialize(serialized, objectType);
        }
    }
}