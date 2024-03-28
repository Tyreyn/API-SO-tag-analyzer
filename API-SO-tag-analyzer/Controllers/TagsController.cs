using API_SO_tag_analyzer.Data;
using API_SO_tag_analyzer.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Serilog;

namespace API_SO_tag_analyzer.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TagsController : ControllerBase
    {

        private StackOverflowApiService stackOverflowApiService { get; set; }

        private JsonFileService jsonFileService { get; set; }

        private readonly ILogger<TagsController> logger;

        public TagsController(JsonFileService jsonFileService, StackOverflowApiService stackOverflowApiService, ILogger<TagsController> logger)
        {
            this.jsonFileService = jsonFileService;
            this.stackOverflowApiService = stackOverflowApiService;
            Log.Information("Starting TagsController");
            this.logger = logger;
        }

        [HttpGet("fetch")]
        public async Task<IActionResult> GetAndSaveNewTags()
        {
            this.logger.LogInformation("Get and save new tags");
            return Ok();
        }
    }
}
