namespace API_SO_tag_analyzer.Controllers
{
    using Swashbuckle.AspNetCore.Annotations;
    using API_SO_tag_analyzer.Services;
    using Microsoft.AspNetCore.Mvc;
    using Serilog;
    using Microsoft.OpenApi.Models;
    using API_SO_tag_analyzer.Data.Model;
    using API_SO_tag_analyzer.Helpers.Enums;

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
        private readonly ILogger logger;

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
            ILogger logger,
            TagOperationService tagOperationService)
        {
            this.logger = logger;
            this.logger.Information("Starting TagsController");
            this.JsonFileService = jsonFileService;
            this.StackOverflowApiService = stackOverflowApiService;
            this.TagOperationService = tagOperationService;
        }

        /// <summary>
        /// Gets or sets Stack Overflow API service.
        /// </summary>
        private StackOverflowApiService StackOverflowApiService { get; set; }

        /// <summary>
        /// Gets or sets json file service.
        /// </summary>
        private JsonFileService JsonFileService { get; set; }

        private TagOperationService TagOperationService { get; set; }

        /// <summary>
        /// Download and save tags to file.
        /// </summary>
        /// <param name="maxItems">sasas</param>
        /// <param name="startingPage"></param>
        /// <param name="order"></param>
        /// <param name="sort"></param>
        /// <returns></returns>
        [HttpGet("{maxItems}/{startingPage:int:range(1,15)}/{order:regex(^(Asc|Desc)$)}/{sort:regex(^(Popular|Activity|Name)$)}")]
        public async Task<IActionResult> GetAndSaveNewTags(
            int maxItems = 1000,
            int startingPage = 1,
            OrderEnum order = OrderEnum.Asc,
            SortEnum sort = SortEnum.Popular)
        {
            int pageNumber = startingPage;
            NewTagParamsModel newTagParams = new NewTagParamsModel
            {
                maxItems = maxItems,
                startingPage = startingPage,
                order = OrderEnum.Asc.ToString().ToLower(),
                sort = SortEnum.Popular.ToString().ToLower(),
            };
            await this.StackOverflowApiService.PrepareTagsStorage(newTagParams);
            return Ok("New tags saved correctly");
        }

        [HttpGet("{orderBy}/{order}")]
        public async Task<IActionResult> GetPercentageOfTagOccurences(OrderByEnum orderBy = OrderByEnum.Value, [FromRoute]OrderEnum order = OrderEnum.Desc)
        {
            await this.TagOperationService.InitializeTagStorage();
            long? sum = await this.TagOperationService.SumAllTagOccurences();
            Dictionary<string, double> tagDictionary = await this.TagOperationService.CalculatePercentageOfOccurrences(sum);

            if (orderBy == OrderByEnum.Name)
            {
                var sortedTagDictionary = order == OrderEnum.Asc ?
                    tagDictionary.OrderBy(item => item.Key).ToList() :
                    tagDictionary.OrderByDescending(item => item.Key).ToList();
                return Ok(sortedTagDictionary);

            }
            else
            {
                var sortedTagDictionary = order == OrderEnum.Asc ?
                    tagDictionary.OrderBy(item => item.Value).ToList() :
                    tagDictionary.OrderByDescending(item => item.Value).ToList();
                return Ok(sortedTagDictionary);

            }

        }
    }
}
