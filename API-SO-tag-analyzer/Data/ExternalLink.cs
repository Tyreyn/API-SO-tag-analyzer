namespace API_SO_tag_analyzer.Data
{
    using Newtonsoft.Json;

    /// <summary>
    /// External link model.
    /// </summary>
    public class ExternalLink
    {
        /// <summary>
        /// Gets or sets type variable.
        /// </summary>
        [JsonProperty("type")]
        public string? Type { get; set; }

        /// <summary>
        /// Gets or sets link variable.
        /// </summary>
        [JsonProperty("link")]
        public string? Link { get; set; }
    }
}
