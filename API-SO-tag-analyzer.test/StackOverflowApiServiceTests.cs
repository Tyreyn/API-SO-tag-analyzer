using API_SO_tag_analyzer.Data;
using API_SO_tag_analyzer.Services;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema.Generation;
using Newtonsoft.Json.Schema;
using Serilog.Sinks.InMemory;
using Serilog;
using System.Diagnostics;

namespace API_SO_tag_analyzer.test
{
    [TestFixture, Order(1)]
    public class StackOverflowApiServiceTests
    {
        private string jsonFilePath;

        private static readonly InMemorySink LogSink = new();

        private readonly ILogger logger;

        private StackOverflowApiService StackOverflowApiService;

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
            logger.Information("Setup test class");
            jsonFilePath = Path.Combine(Directory.GetCurrentDirectory(), "so-response.json");
            JsonFileService JsonFileService = new JsonFileService(jsonFilePath, this.logger);
            StackOverflowApiService = new StackOverflowApiService(JsonFileService, this.logger);
        }

        [Test, Order(0)]
        public void CheckConnectionWithDefaultParams()
        {
            try
            {
                StackOverflowApiService.GetTagsAsync().GetAwaiter();
                this.logger.Information("The connection was carried out successfully");
            }
            catch (Exception ex)
            {
                this.logger.Fatal(ex.ToString());
                Assert.Fail(ex.ToString());
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
