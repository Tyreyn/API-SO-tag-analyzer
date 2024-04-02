namespace API_SO_tag_analyzer.Services
{
    using System.Net;
    using System.Net.Http.Headers;
    using API_SO_tag_analyzer.Data;
    using API_SO_tag_analyzer.Data.Model;
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
            this.logger.Information("Starting StackOverflowApiService");
            this.PrepareTagsStorage().GetAwaiter();
        }

        /// <summary>
        /// Gets or setsjJson file service.
        /// </summary>
        private JsonFileService JsonFileService { get; set; }

        /// <summary>
        /// Request tags from api.
        /// </summary>
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
            string requestParamUrl = "tags?page=1&pagesize=100&order=asc&sort=name&site=stackoverflow")
        {
            HttpClientHandler handler = new HttpClientHandler()
            {
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate,
            };
            string respStr = string.Empty;
            HttpClient httpClient = new HttpClient(handler);
            httpClient.BaseAddress = new Uri(SoApiUrl);
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            this.logger.Information("Calling {0}{1}", SoApiUrl, requestParamUrl);
            var response = await httpClient.GetAsync(
                requestParamUrl,
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
        /// <param name="newTagParamsModel">
        /// Request paramaters for tag.
        /// </param>
        /// <returns>
        /// Always true.
        /// </returns>
        public async Task PrepareTagsStorage(NewTagParamsModel? newTagParamsModel = null)
        {
            if (newTagParamsModel == null)
            {
                newTagParamsModel = new NewTagParamsModel();
            }

            this.logger.Information("Prepare {0} tags storage", newTagParamsModel.MaxItems);
            int pageNumber = newTagParamsModel.StartingPage;
            JObject jsonMergedResponse = new JObject();
            do
            {
                string requestString = $"tags?page={pageNumber}&pagesize=100&order={newTagParamsModel.Order}&sort={newTagParamsModel.Sort}&site=stackoverflow";
                string response = await this.GetTagsAsync(requestParamUrl: requestString);
                JObject jsonResponse = JObject.Parse(response);
                jsonMergedResponse.Merge(jsonResponse);
                pageNumber++;
                if (pageNumber >= 25)
                {
                    this.logger.Information("Can't access api on this page without special token");
                    break;
                }
            }
            while (jsonMergedResponse["items"].Count() < newTagParamsModel.MaxItems);

            await this.JsonFileService.WriteTagsToFileAsync(jsonMergedResponse);
        }
    }
}
