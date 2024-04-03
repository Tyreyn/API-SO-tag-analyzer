using API_SO_tag_analyzer.Data;
using API_SO_tag_analyzer.Services;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema.Generation;
using Newtonsoft.Json.Schema;
using Serilog.Sinks.InMemory;
using Serilog;
using System.Diagnostics;
using System.Net.Http.Headers;
using API_SO_tag_analyzer.Data.Model;

namespace API_SO_tag_analyzer.test
{
    [TestFixture, Order(1)]
    public class StackOverflowApiServiceTests
    {


        private const string SoApiUrl = "https://api.stackexchange.com/2.3";

        private string jsonFilePath;

        private static readonly InMemorySink LogSink = new();

        private readonly ILogger logger;

        private StackOverflowApiService stackOverflowApiService;

        private JsonFileService jsonFileService;

        public StackOverflowApiServiceTests()
        {
            this.logger = new LoggerConfiguration()
            .MinimumLevel.Verbose()
            .WriteTo.Sink(LogSink)
            .CreateLogger();
        }

        [SetUp]
        public void Setup()
        {
            this.logger.Information("Starting test {0}", TestContext.CurrentContext.Test.MethodName);
            logger.Information("Setup test class");
            jsonFilePath = Path.Combine(Directory.GetCurrentDirectory(), "so-response.json");
            this.jsonFileService = new JsonFileService(jsonFilePath, this.logger);
            this.stackOverflowApiService = new StackOverflowApiService(this.jsonFileService, this.logger, false);
        }

        [Test, Order(0)]
        public void CheckConnectionWithDefaultParams()
        {
            // Arrange
            string requestParamUrl = "tags?page=1&pagesize=1&order=asc&sort=name&site=stackoverflow";
            HttpClient httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri(SoApiUrl);
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            this.logger.Information("Calling {0}", SoApiUrl);

            try
            {
                // Act
                HttpResponseMessage response = httpClient.GetAsync(requestParamUrl).GetAwaiter().GetResult();
                response.EnsureSuccessStatusCode();
                // Assert
                this.logger.Information("The connection was carried out successfully");
            }
            catch (HttpRequestException ex)
            {
                this.logger.Fatal(ex.ToString());
                Assert.Fail(ex.ToString());
            }
        }

        [Test, Order(1)]
        public async Task CheckHandlingWrongRequestParameters()
        {
            // Arrange
            NewTagParamsModel newWrongTagParamsModel = new NewTagParamsModel
            {
                Order = "AAA",
                MaxItems = 0,
                StartingPage = 0,
                Sort = "BBB"
            };

            try
            {
                // Act
                await this.stackOverflowApiService.PrepareTagsStorage(newWrongTagParamsModel);

                // Assert
                Assert.Fail("Wrong parameters are not handled properly");
            }
            catch (Exception ex)
            {
                this.logger.Information(string.Format("Wrong parameters are handled properly {0}", ex.ToString()));
            }
        }

        [Test, Order(2)]
        public async Task CheckIfDownloadedTagsWillBeSavedCorrectlyWhenPageBeyondScope()
        {
            // Arrange
            NewTagParamsModel newTagParamsModel = new NewTagParamsModel
            {
                StartingPage = 23,
            };

            await this.stackOverflowApiService.PrepareTagsStorage(newTagParamsModel);

            // Act
            TagStorage readApiTag = this.jsonFileService.ReadFromFileAsync<TagStorage>().Result;
            int itemCount = readApiTag.Items.Count();
            this.logger.Information("Downloaded and saved items: {0}", itemCount);

            // Assert
            if (itemCount == 200)
            {
                this.logger.Information("Tags were successfully downloaded and saved");
            }
            else
            {
                Assert.Fail("Tags were incorrectly downloaded and saved");
            }
        }

        [TearDown]
        public void TearDown()
        {
            this.logger.Information("Cleaning after test");
            if (File.Exists(jsonFilePath))
            {
                this.logger.Information("Delete json file");
                File.Delete(jsonFilePath);
            }

            var logOutput = LogSink.LogEvents.Aggregate("", (current, logEvent) => current + (logEvent.RenderMessage() + "\n"));
            Console.WriteLine(logOutput);
            LogSink.Dispose();
            Trace.Flush();

        }
    }
}
