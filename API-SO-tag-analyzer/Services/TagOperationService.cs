namespace API_SO_tag_analyzer.Services
{
    using API_SO_tag_analyzer.Data;
    using Microsoft.AspNetCore.Http.HttpResults;
    using Serilog;
    using System.Runtime.CompilerServices;

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

        private TagStorage tagStorage;

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

        public async Task InitializeTagStorage()
        {
            this.logger.Information("Loading TagStorage from file");
            this.tagStorage = this.jsonFileService.ReadFromFileAsync<TagStorage>().GetAwaiter().GetResult();
        }

        public async Task<long?> SumAllTagOccurences()
        {
            this.logger.Information("Summation of the number of occurrences");
            long? sum = 0;
            if (this.tagStorage != null) {
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

            return sum;
        }

        public async Task<Dictionary<string, double>> CalculatePercentageOfOccurrences(long? sum)
        {
            Dictionary<string, double> tagItemDictionary = new Dictionary<string, double>();
            if (this.tagStorage != null)
            {
                foreach (TagItem item in this.tagStorage.Items)
                {
                    tagItemDictionary.Add(item.Name, Math.Round(((double)item.Count / (double)sum) * 100, 6));
                }
            }
            else
            {
                this.logger.Fatal("Load tagstorage! Use InitializeTagStorage method.");
                throw new NullReferenceException("Load tagstorage! Use InitializeTagStorage method.");
            }

            return tagItemDictionary;
        }

    }
}
