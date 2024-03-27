namespace API_SO_tag_analyzer.Data
{
    public class TagItem
    {
        public bool has_synonyms { get; set; }
        public bool is_moderator_only { get; set; }
        public bool is_required { get; set; }
        public int count { get; set; }
        public string name { get; set; }
        public List<Collective> collectives { get; set; }


    }
}
