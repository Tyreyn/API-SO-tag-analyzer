using System.Text.Json.Serialization;

namespace API_SO_tag_analyzer.Helpers.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum OrderByEnum
    {
        Name,
        Value,
    }
}
