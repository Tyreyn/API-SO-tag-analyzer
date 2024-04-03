using API_SO_tag_analyzer.Services;
using Serilog.Sinks.InMemory;
using Serilog;
using System.Diagnostics;
using API_SO_tag_analyzer.Controllers;
using Microsoft.AspNetCore.Mvc;
using API_SO_tag_analyzer.Data.Model;

namespace API_SO_tag_analyzer.test
{
    [TestFixture, Order(3)]
    public class TagsControllerTests
    {

        private string jsonFilePath;

        private static readonly InMemorySink LogSink = new();

        private readonly ILogger logger;

        private StackOverflowApiService stackOverflowApiService;

        private JsonFileService jsonFileService;

        private TagsController tagsController;

        private TagOperationService tagOperationService;

        public TagsControllerTests()
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
            this.tagOperationService = new TagOperationService(this.jsonFileService, this.logger);
            this.tagsController = new TagsController(this.jsonFileService, this.stackOverflowApiService, this.logger, this.tagOperationService);
        }

        [Test, Order(0)]
        public async Task GettingPercentageOfTagOccurrencesWithDefaultParams()
        {
            // Arrange
            if (!File.Exists(jsonFilePath))
            {
                await this.stackOverflowApiService.PrepareTagsStorage();
            }

            // Act
            OkObjectResult response = this.tagsController.GetPercentageOfTagOccurrences().Result as OkObjectResult;
            List<KeyValuePair<string, double>> resultList = response.Value as List<KeyValuePair<string, double>>;

            // Assert
            var strings = resultList.Select(kvp => string.Format("@{0}={1}%", kvp.Key, kvp.Value));
            string path = string.Join("\n", strings);
            this.logger.Information(path);
            Assert.That(response != null);
        }

        [Test, Order(1)]
        public async Task CheckThatGetAndSaveNewTagsHandleWrongParams()
        {
            // Arrange
            // Act
            BadRequestResult response = this.tagsController.GetAndSaveNewTags(maxItems: 0, startingPage: 0).Result as BadRequestResult;

            // Assert
            this.logger.Information(string.Format("Status code: {0}", response.StatusCode));
            Assert.That(response.StatusCode == 400);
        }

        [Test, Order(10)]
        public async Task CheckThatGetPercentageOfTagOccurrencesMethodHandleNoFile()
        {
            // Arrange
            if (File.Exists(jsonFilePath))
            {
                File.Delete(jsonFilePath);
            }

            // Act
            ObjectResult response = this.tagsController.GetPercentageOfTagOccurrences().Result as ObjectResult;

            // Assert
            this.logger.Information(string.Format("Status code: {0}", response.StatusCode));
            Assert.That(response.StatusCode == 500);
        }

        [TearDown]
        public void TearDown()
        {
            this.logger.Information("Cleaning after test");
            var logOutput = LogSink.LogEvents.Aggregate("", (current, logEvent) => current + (logEvent.RenderMessage() + "\n"));
            Console.WriteLine(logOutput);
            LogSink.Dispose();
            Trace.Flush();

        }
    }
}
