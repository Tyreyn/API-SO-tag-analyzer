namespace API_SO_tag_analyzer.Services
{
    using System.Net;
    using System.Net.Http.Headers;
    using API_SO_tag_analyzer.Data;
    using Newtonsoft.Json.Linq;

    /// <summary>
    /// Provides sending requests to API.
    /// </summary>
    public class StackOverflowApiService
    {
        /// <summary>
        /// Base URL to Stack Overflow API.
        /// </summary>
        private const string SoApiUrl = "https://api.stackexchange.com/2.3";

        /// <summary>
        /// The logger object.
        /// </summary>
        private readonly Serilog.ILogger logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="StackOverflowApiService"/> class.
        /// </summary>
        /// <param name="jsonFileService">
        /// Json file service.
        /// </param>
        /// <param name="logger">
        /// The logger object.
        /// </param>
        public StackOverflowApiService(JsonFileService jsonFileService, Serilog.ILogger logger)
        {
            this.JsonFileService = jsonFileService;
            this.logger = logger;
            this.logger.Information("Starting TagsController");
            this.PrepareTagsStorage().GetAwaiter();
        }

        /// <summary>
        /// Gets or setsjJson file service.
        /// </summary>
        private JsonFileService JsonFileService { get; set; }

        /// <summary>
        /// Request tags from api.
        /// </summary>
        /// <param name="startingPageNumber">
        /// Starting page number from which download will be started.
        /// </param>
        /// <param name="requestParamUrl">
        /// Request parameters url.
        /// </param>
        /// <returns>
        /// Response string.
        /// </returns>
        /// <exception cref="Exception">
        /// Throws when there is problem with connecting to API.
        /// </exception>
        public async Task<string> GetTagsAsync(
            int startingPageNumber = 1,
            string requestParamUrl = "tags?page={0}&pagesize=100&order=asc&sort=name&site=stackoverflow")
        {
            HttpClientHandler handler = new HttpClientHandler()
            {
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate,
            };
            string respStr = string.Empty;
            HttpClient httpClient = new HttpClient(handler);
            httpClient.BaseAddress = new Uri(SoApiUrl);
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var response = await httpClient.GetAsync(
                string.Format(requestParamUrl, startingPageNumber),
                HttpCompletionOption.ResponseContentRead);

            if (response.IsSuccessStatusCode)
            {
                using (Stream stream = await response.Content.ReadAsStreamAsync())
                using (StreamReader streamReader = new StreamReader(stream))
                {
                    respStr = await streamReader.ReadToEndAsync();
                }
            }
            else
            {
                string problemMessage = "Problem with connecting to API";
                this.logger.Fatal(problemMessage);
                throw new Exception(problemMessage);
            }

            return respStr;
        }

        /// <summary>
        /// Request and save json api response to file.
        /// </summary>
        /// <param name="maxItemCount">
        /// Item count to be downloaded.
        /// </param>
        /// <param name="startingPage">
        /// Starting page from which start to download.
        /// </param>
        /// <returns>
        /// Always true.
        /// </returns>
        public async Task PrepareTagsStorage(int maxItemCount = 1000, int startingPage = 1)
        {
            this.logger.Information("Prepare {0} tags storage", maxItemCount);
            int pageNumber = startingPage;

            // Change string to JObject to be able merging requests.
            JObject jsonMergedResponse = JObject.FromObject(new TagStorage());

            while (jsonMergedResponse["items"]?.Count() < maxItemCount)
            {
                this.logger.Information("Getting 100 tags from page {0}", pageNumber);
                string response = await this.GetTagsAsync(pageNumber);
                JObject jsonResponse = JObject.Parse(response);
                jsonMergedResponse.Merge(jsonResponse);
                pageNumber++;
            }

            await this.JsonFileService.WriteTagsToFileAsync(jsonMergedResponse);
        }
    }
}
