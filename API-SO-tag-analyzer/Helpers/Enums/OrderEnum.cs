using System.Text.Json.Serialization;

namespace API_SO_tag_analyzer.Helpers.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum OrderEnum
    {
        Asc,

        /// <summary>
        /// Descending.
        /// </summary>
        Desc,
    }
}
