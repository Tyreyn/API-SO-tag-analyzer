using API_SO_tag_analyzer.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.InMemory;
using System.Diagnostics;

namespace API_SO_tag_analyzer.test
{

    [TestFixture]
    public class JsonFileServiceTests
    {
        private string jsonFilePath;

        private static readonly InMemorySink LogSink = new InMemorySink();

        private readonly ILogger logger;

        private readonly string apiTag = "{ \"items\":[{\"has_synonyms\":true, \"is_moderator_only\":false,"
                                         + " \"is_required\":false, \"count\":2528829, \"name\":\"javascript\"}],"
                                         + "\"has_more\":true,\"quota_max\":300,\"quota_remaining\":95}";

        public JsonFileServiceTests()
        {
            this.logger = new LoggerConfiguration()
            .MinimumLevel.Verbose()
            .WriteTo.Sink(LogSink)
            .CreateLogger();
        }

        [SetUp]
        public void Setup()
        {
            logger.Information("Setup test class");
            jsonFilePath = Path.Combine(Directory.GetCurrentDirectory(), "so-response.json");
        }

        [Test]
        public void CheckIfTagIsSavedCorrectly()
        {
            // Arrange
            JsonFileService jsonFileService = new JsonFileService(jsonFilePath, logger);
            JObject apiTestTagJObject = JObject.Parse(apiTag);

            // Act
            jsonFileService.WriteTagsToFileAsync(apiTestTagJObject).GetAwaiter();

            // Assert
            bool fileExits = File.Exists(jsonFilePath);
            logger.Information(fileExits ? "File exists" : "File does not exists");
            Assert.IsTrue(fileExits);

            JObject readApiTagJObject = JObject.Parse(File.ReadAllText(jsonFilePath));
            if (JToken.DeepEquals(apiTestTagJObject, readApiTagJObject))
            {
                logger.Error("Tag was saved correctly!");
                Assert.IsTrue(true);
            }
            else
            {
                logger.Error("Tag was not saved correctly!");
                Assert.Fail();
            }
        }

        [TearDown]
        public void TearDown()
        {
            if (File.Exists(jsonFilePath))
            {
                File.Delete(jsonFilePath);
            }

            var logOutput = LogSink.LogEvents.Aggregate("", (current, logEvent) => current + (logEvent.RenderMessage() + "\n"));
            Console.WriteLine(logOutput);
            Trace.Flush();

        }

    }
}