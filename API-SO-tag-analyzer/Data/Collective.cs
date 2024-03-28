namespace API_SO_tag_analyzer.Data
{
    using Newtonsoft.Json;

    /// <summary>
    /// Collective model.
    /// </summary>
    public class Collective
    {
        /// <summary>
        /// Gets or sets tags model array.
        /// </summary>
        [JsonProperty("tags")]
        public string[]? Tags { get; set; }

        /// <summary>
        /// Gets or sets external links model array.
        /// </summary>
        [JsonProperty("external_links")]
        public ExternalLink[]? ExternalLinks { get; set; }

        /// <summary>
        /// Gets or sets description variable.
        /// </summary>
        [JsonProperty("description")]
        public string? Description { get; set; }

        /// <summary>
        /// Gets or sets link variable.
        /// </summary>
        [JsonProperty("link")]
        public string? Link { get; set; }

        /// <summary>
        /// Gets or sets name variable.
        /// </summary>
        [JsonProperty("name")]
        public string? Name { get; set; }

        /// <summary>
        /// Gets or sets slug variable.
        /// </summary>
        [JsonProperty("slug")]
        public string? Slug { get; set; }
    }
}
