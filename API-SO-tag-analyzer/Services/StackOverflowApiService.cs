using API_SO_tag_analyzer.Data;
using Microsoft.AspNetCore.Http.HttpResults;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Serilog;
using System.Net;
using System.Net.Http.Headers;


namespace API_SO_tag_analyzer.Services
{
    public class StackOverflowApiService
    {
        private const string SoApiUrl = "https://api.stackexchange.com/";

        private JsonFileService jsonFileService { get; set; }

        private readonly Serilog.ILogger logger;

        public StackOverflowApiService(JsonFileService jsonFileService, Serilog.ILogger logger)
        {
            this.jsonFileService = jsonFileService;
            this.logger = logger;
            this.logger.Information("Starting TagsController");
            this.PrepareTagsStorage();
        }

        public async Task<string> GetTagsAsync(
            int startingPageNumber = 1,
            string requestParamUrl = "/2.3/tags?page={0}&pagesize=100&order=asc&sort=name&site=stackoverflow")
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

        public async Task PrepareTagsStorage(int maxItemCount = 1000, int startingPage = 1)
        {
            this.logger.Information("Prepare {0} tags storage", maxItemCount);
            int pageNumber = startingPage;
            JObject jsonMergedResponse = JObject.FromObject(new TagStorage());
            while (jsonMergedResponse["items"].Count() < maxItemCount)
            {
                this.logger.Information("Getting 100 tags from page {0}", pageNumber);
                string response = await this.GetTagsAsync(pageNumber);
                JObject jsonResponse = JObject.Parse(response);
                jsonMergedResponse.Merge(jsonResponse);
                pageNumber++;
            }

            this.jsonFileService.WriteTagsToFileAsync(jsonMergedResponse).GetAwaiter();
        }
    }
}
