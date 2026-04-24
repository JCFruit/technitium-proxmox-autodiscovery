using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ProxmoxAutodiscovery;

public static class DeserializationHelper
{
    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        Converters =
        {
            new IpNetworkConverter()
        }
    };
    
    public static T Deserialize<T>(string json)
    {
        var value = JsonSerializer.Deserialize<T>(json, SerializerOptions);
        if (value == null)
            throw new InvalidOperationException("Json deserialization resulted in null object.");
        
        Validator.ValidateObject(value, new ValidationContext(value), validateAllProperties: true);
        
        return value;
    }
    
    private sealed class IpNetworkConverter : JsonConverter<IPNetwork>
    {
        public override IPNetwork Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var str = reader.GetString();
            if (!string.IsNullOrEmpty(str))
                return IPNetwork.Parse(str);
                
            return default;
        }

        public override void Write(Utf8JsonWriter writer, IPNetwork value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString());
        }
    }
}
