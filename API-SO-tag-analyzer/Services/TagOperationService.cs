namespace API_SO_tag_analyzer.Services
{
    using API_SO_tag_analyzer.Data;
    using Serilog;

    /// <summary>
    /// Provides calculation methods over tags.
    /// </summary>
    public class TagOperationService
    {
        /// <summary>
        /// Json file service.
        /// </summary>
        private readonly JsonFileService jsonFileService;

        /// <summary>
        /// The logger object.
        /// </summary>
        private readonly ILogger logger;

        /// <summary>
        /// The tag storage object.
        /// </summary>
        private TagStorage? tagStorage;

        /// <summary>
        /// Initializes a new instance of the <see cref="TagOperationService"/> class.
        /// </summary>
        /// <param name="jsonFileService">
        /// Json file service.
        /// </param>
        /// <param name="logger">
        /// The logger object.
        /// </param>
        public TagOperationService(JsonFileService jsonFileService, ILogger logger)
        {
            this.jsonFileService = jsonFileService;
            this.logger = logger;
            this.logger.Information("Starting TagOperationService");
        }

        /// <summary>
        /// Download from stackoverflow API and save tags to file.
        /// </summary>
        /// <returns>
        /// Always true.
        /// </returns>
        public async Task InitializeTagStorage()
        {
            this.logger.Information("Loading TagStorage from file");
            this.tagStorage = await this.jsonFileService.ReadFromFileAsync<TagStorage>();
        }

        /// <summary>
        /// Sums up all occurrences of tags.
        /// </summary>
        /// <returns>
        /// Sum of all occurrences tags.
        /// </returns>
        /// <exception cref="NullReferenceException">
        /// Throws exception when tagstorage file is not saved.
        /// </exception>
        public async Task<long?> SumAllTagOccurences()
        {
            this.logger.Information("Summation of the number of occurrences");
            long? sum = 0;
            if (this.tagStorage != null)
            {
                foreach (TagItem item in this.tagStorage.Items)
                {
                    sum += item.Count;
                }
            }
            else
            {
                this.logger.Fatal("Load tagstorage! Use InitializeTagStorage method.");
                throw new NullReferenceException("Load tagstorage! Use InitializeTagStorage method.");
            }

            this.logger.Information("Sum: {0}", sum);
            return sum;
        }

        /// <summary>
        /// Calculate percentage of occurrences tag.
        /// </summary>
        /// <param name="sum">
        /// Sum of all occurrences tags.
        /// </param>
        /// <returns>
        /// Dictionary:
        /// Key: name of tag
        /// Value: percentage of occurrences.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Throws exception when tag storage is not initialized.
        /// </exception>
        public async Task<Dictionary<string, double>> CalculatePercentageOfOccurrences(long? sum)
        {
            Dictionary<string, double> tagItemDictionary = new Dictionary<string, double>();
            if (this.tagStorage != null)
            {
                foreach (TagItem item in this.tagStorage.Items)
                {
                    double percentage = Math.Round(((double)item.Count / (double)sum) * 100, 4);
                    this.logger.Information($"{item.Name} percentage {percentage}");
                    tagItemDictionary.Add(item.Name, percentage);
                }
            }
            else
            {
                this.logger.Fatal("Load tagstorage! Use InitializeTagStorage method.");
                throw new ArgumentNullException("Load tagstorage! Use InitializeTagStorage method.");
            }

            return tagItemDictionary;
        }
    }
}
