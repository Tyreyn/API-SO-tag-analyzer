namespace API_SO_tag_analyzer.Data
{
    using Newtonsoft.Json;

    /// <summary>
    /// Tag storage model.
    /// </summary>
    public class TagStorage
    {
        /// <summary>
        /// Gets or sets items model array.
        /// </summary>
        [JsonProperty("items", Required = Required.Always)]
        public TagItem[]? Items { get; set; }

        /// <summary>
        /// Gets or sets has more variable.
        /// </summary>
        [JsonProperty("has_more", Required = Required.Always)]
        public bool? HasMore { get; set; }

        /// <summary>
        /// Gets or sets quota max variable.
        /// </summary>
        [JsonProperty("quota_max", Required = Required.Always)]
        public long? QuotaMax { get; set; }

        /// <summary>
        /// Gets or sets quota remaining variable.
        /// </summary>
        [JsonProperty("quota_remaining", Required = Required.Always)]
        public long? QuotaRemaining { get; set; }
    }
}
