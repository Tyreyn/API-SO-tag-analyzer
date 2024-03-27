namespace API_SO_tag_analyzer.Data
{
    public class TagStorage
    {
        public List<TagItem> items { get; set; }
        public bool has_more { get; set; }
        public int quota_max { get; set; }
        public int quota_remaining { get; set; }
    }
}
