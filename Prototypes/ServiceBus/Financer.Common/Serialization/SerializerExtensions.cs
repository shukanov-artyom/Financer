using System.Text.Json;

namespace Financer.Common.Serialization
{
    public static class SerializerExtensions
    {
        public static async Task<T> DeserializeAsync<T>(this Stream stream)
        {
            return await JsonSerializer.DeserializeAsync<T>(stream,
                    new JsonSerializerOptions() { PropertyNamingPolicy = SnakeCaseNamingPolicy.Instance });
        }

        public static T Deserialize<T>(this string str)
        {
            return JsonSerializer.Deserialize<T>(str,
                    new JsonSerializerOptions() { PropertyNamingPolicy = SnakeCaseNamingPolicy.Instance });
        }

        public static string Serialize<T>(this T item)
        {
            return JsonSerializer.Serialize(item, new JsonSerializerOptions() 
                { PropertyNamingPolicy = SnakeCaseNamingPolicy.Instance });
        }
    }
}
