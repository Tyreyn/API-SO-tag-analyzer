namespace API_SO_tag_analyzer.Data
{
    public class Collective
    {
        public List<string> tags { get; set; }
        public List<ExternalLink> external_links { get; set; }
        public string description { get; set; }
        public string link { get; set; }
        public string name { get; set; }
        public string slug { get; set; }

    }
}
