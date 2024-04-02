namespace API_SO_tag_analyzer.Data.Model
{
    /// <summary>
    /// API request parameters model.
    /// </summary>
    public class NewTagParamsModel
    {
        /// <summary>
        /// Max items to download.
        /// </summary>
        public int MaxItems = 1000;

        /// <summary>
        /// Starting page from which start to download tags.
        /// </summary>
        public int StartingPage = 1;

        /// <summary>
        /// Describing the order of retrieved data.
        /// Ascending, descending.
        /// </summary>
        public string Order = "desc";

        /// <summary>
        /// Fescribing the sorting of retrieved data.
        /// Name, popular, activity.
        /// </summary>
        public string Sort = "popular";
    }
}
