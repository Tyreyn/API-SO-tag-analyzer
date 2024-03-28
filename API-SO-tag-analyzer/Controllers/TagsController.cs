namespace API_SO_tag_analyzer.Controllers
{
    using API_SO_tag_analyzer.Services;
    using Microsoft.AspNetCore.Mvc;
    using Serilog;

    /// <summary>
    /// Tags API Controller.
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class TagsController : ControllerBase
    {
        /// <summary>
        /// The logger object.
        /// </summary>
        private readonly ILogger<TagsController> logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="TagsController"/> class.
        /// </summary>
        /// <param name="jsonFileService">
        /// Json file service.
        /// </param>
        /// <param name="stackOverflowApiService">
        /// Stack Overflow API service.
        /// </param>
        /// <param name="logger">
        /// The logger object.
        /// </param>
        public TagsController(
            JsonFileService jsonFileService,
            StackOverflowApiService stackOverflowApiService,
            ILogger<TagsController> logger)
        {
            this.JsonFileService = jsonFileService;
            this.StackOverflowApiService = stackOverflowApiService;
            Log.Information("Starting TagsController");
            this.logger = logger;
        }

        /// <summary>
        /// Gets or sets Stack Overflow API service.
        /// </summary>
        private StackOverflowApiService StackOverflowApiService { get; set; }

        /// <summary>
        /// Gets or sets json file service.
        /// </summary>
        private JsonFileService JsonFileService { get; set; }

        [HttpGet("fetch")]
        public async Task<IActionResult> GetAndSaveNewTags()
        {
            this.logger.LogInformation("Get and save new tags");
            return Ok();
        }
    }
}
