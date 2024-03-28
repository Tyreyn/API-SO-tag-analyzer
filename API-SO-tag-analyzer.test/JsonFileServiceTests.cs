using API_SO_tag_analyzer.Data;
using API_SO_tag_analyzer.Services;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using Newtonsoft.Json.Schema.Generation;
using Serilog;
using Serilog.Sinks.InMemory;
using System.Diagnostics;

namespace API_SO_tag_analyzer.test
{

    [TestFixture, Order(0)]
    public class JsonFileServiceTests
    {
        private string jsonFilePath;

        private static readonly InMemorySink LogSink = new();

        private readonly ILogger logger;

        private JsonFileService jsonFileService;

        private readonly string apiTestTag = "{ \"items\":[{\"has_synonyms\":true, \"is_moderator_only\":false,"
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
            jsonFileService = new JsonFileService(jsonFilePath, logger);
        }

        [Test, Order(2)]
        public void CheckIfTagIsSavedCorrectly()
        {
            // Arrange
            this.logger.Information("Arrange testing JObject");
            JObject apiTestTagJObject = JObject.Parse(apiTestTag);

            // Act
            jsonFileService.WriteTagsToFileAsync(apiTestTagJObject).GetAwaiter();

            // Assert
            bool fileExits = File.Exists(jsonFilePath);
            logger.Information(fileExits ? "File exists" : "File does not exists");
            Assert.That(fileExits);

            JObject readApiTagJObject = jsonFileService.ReadFromFileAsync<JObject>().Result;
            if (JToken.DeepEquals(apiTestTagJObject, readApiTagJObject))
            {
                logger.Error("Tag was saved correctly!");
                Assert.Pass();
            }
            else
            {
                logger.Error("Tag was not saved correctly!");
                Assert.Fail();
            }
        }

        [Test, Order(0)]
        public void ValidateTagStorageModel_CorrectString()
        {
            // Arrange
            JSchemaGenerator generator = new JSchemaGenerator();
            JSchema schema = generator.Generate(typeof(TagStorage));
            IList<string> messages;
            JObject validTagJObject = JObject.Parse(this.apiTestTag);

            // Act
            bool valid = validTagJObject.IsValid(schema, out messages);

            // Assert
            if (valid)
            {
                this.logger.Information("Tested json string is valid"
                    + string.Join("\n", messages));
                Assert.Pass();
            }
            else
            {
                this.logger.Fatal("Tested json string is not valid");
                Assert.Fail(string.Join(Environment.NewLine, messages));
            }
        }

        [Test, Order(1)]
        public void ValidateTagStorageModel_WrongString()
        {
            // Arrange
            string wrongApiString = "{ \"asda\":[{\"has_synonyms\":true, \"is\":false,"
                                         + " \"is_required\":false, \"count\":\"asd\", \"name\":\"javascript\"}],"
                                         + "\"wrong_has\":true,\"quota_max\":\"testtest1\",\"quota_remaining\":95}";
            JSchemaGenerator generator = new JSchemaGenerator();
            JSchema schema = generator.Generate(typeof(TagStorage));
            IList<string> messages;
            JObject invalidTagJObject = JObject.Parse(wrongApiString);

            // Act
            bool valid = invalidTagJObject.IsValid(schema, out messages);

            // Assert
            if (!valid)
            {
                this.logger.Information("Tested json string is not valid \n"
                    + string.Join(Environment.NewLine, messages));
                Assert.Pass();
            }
            else
            {
                this.logger.Fatal("Tested json string is valid \n");
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
            LogSink.Dispose();
            Trace.Flush();

        }
    }
}