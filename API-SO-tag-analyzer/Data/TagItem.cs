namespace API_SO_tag_analyzer.Data
{
    using Newtonsoft.Json;

    /// <summary>
    /// Tag item model.
    /// </summary>
    public class TagItem
    {
        /// <summary>
        /// Gets or sets has synonyms variable.
        /// </summary>
        [JsonProperty("has_synonyms")]
        public bool? HasSynonyms { get; set; }

        /// <summary>
        /// Gets or sets is moderator only variable.
        /// </summary>
        [JsonProperty("is_moderator_only")]
        public bool? IsModeratorOnly { get; set; }

        /// <summary>
        /// Gets or sets is required variable.
        /// </summary>
        [JsonProperty("is_required")]
        public bool? IsRequired { get; set; }

        /// <summary>
        /// Gets or sets count variable.
        /// </summary>
        [JsonProperty("count")]
        public long? Count { get; set; }

        /// <summary>
        /// Gets or sets name variable.
        /// </summary>
        [JsonProperty("name")]
        public string? Name { get; set; }

        /// <summary>
        /// Gets or sets collectives array.
        /// </summary>
        [JsonProperty("collectives", NullValueHandling = NullValueHandling.Ignore, Required = Required.Default)]
        public Collective[]? Collectives { get; set; }
    }
}
