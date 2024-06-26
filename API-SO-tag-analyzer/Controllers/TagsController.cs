using API_SO_tag_analyzer.Data.Model;
using API_SO_tag_analyzer.Helpers.Enums;
using API_SO_tag_analyzer.Services;
using Microsoft.AspNetCore.Mvc;
using ILogger = Serilog.ILogger;

namespace API_SO_tag_analyzer.Controllers
{
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
        /// <param name="tagOperationService">
        /// Tag operation service.
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

        /// <summary>
        /// Gets or sets tag operation service.
        /// </summary>
        private TagOperationService TagOperationService { get; set; }

        /// <summary>
        /// Download and save tags to file.
        /// </summary>
        /// <param name="maxItems">
        /// Max items to download.
        /// </param>
        /// <param name="startingPage">
        /// Starting page from which start to download tags.
        /// </param>
        /// <param name="order">
        /// Describing the order of retrieved data.
        /// </param>
        /// <param name="sort">
        /// Fescribing the sorting of retrieved data.
        /// </param>
        /// <returns>
        /// True, if downloaded and saved tags correctly.
        /// </returns>
        [HttpGet("{maxItems}/{startingPage:int:range(1,15)}/{order:regex(^(Asc|Desc)$)}/{sort:regex(^(Popular|Activity|Name)$)}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetAndSaveNewTags(
            int? maxItems = 1000,
            int? startingPage = 1,
            Order? order = Order.Asc,
            Sorting? sort = Sorting.Popular)
        {
            if (maxItems == null ||
                startingPage == null ||
                maxItems <= 0 ||
                startingPage <= 0 ||
                startingPage >= 15 ||
                order == null ||
                sort == null)
            {
                return this.BadRequest();
            }

            NewTagParamsModel newTagParams = new NewTagParamsModel
            {
                MaxItems = (int)maxItems,
                StartingPage = (int)startingPage,
                Order = order.ToString().ToLower(),
                Sort = sort.ToString().ToLower(),
            };

            await this.StackOverflowApiService.PrepareTagsStorage(newTagParams);
            return this.Ok("New tags saved correctly");
        }

        /// <summary>
        /// Get percentage of all occurrences tags.
        /// </summary>
        /// <param name="orderBy">
        /// Order by enum.
        /// </param>
        /// <param name="order">
        /// Order enum.
        /// </param>
        /// <returns>
        /// True, if correctly calculated.
        /// </returns>
        [HttpGet("{orderBy}/{order}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetPercentageOfTagOccurrences(
            OrderBy? orderBy = OrderBy.Value,
            Order? order = Order.Desc)
        {
            if (orderBy == null ||
                order == null)
            {
                return this.BadRequest();
            }

            try
            {
                await this.TagOperationService.InitializeTagStorage();
            }
            catch (Exception ex)
            {
                return this.Problem(ex.ToString());
            }

            long? sum = await this.TagOperationService.SumAllTagOccurences();
            Dictionary<string, double> tagDictionary = await this.TagOperationService.CalculatePercentageOfOccurrences(sum);

            if (orderBy == OrderBy.Name)
            {
                var sortedTagDictionary = order == Order.Asc ?
                    tagDictionary.OrderBy(item => item.Key).ToList() :
                    tagDictionary.OrderByDescending(item => item.Key).ToList();
                return this.Ok(sortedTagDictionary);
            }
            else
            {
                var sortedTagDictionary = order == Order.Asc ?
                    tagDictionary.OrderBy(item => item.Value).ToList() :
                    tagDictionary.OrderByDescending(item => item.Value).ToList();
                return this.Ok(sortedTagDictionary);
            }
        }
    }
}
