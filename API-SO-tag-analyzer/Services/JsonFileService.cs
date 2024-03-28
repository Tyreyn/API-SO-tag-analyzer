namespace API_SO_tag_analyzer.Services
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using ILogger = Serilog.ILogger;

    /// <summary>
    /// Provides saving and reading json string from file.
    /// </summary>
    public class JsonFileService
    {
        /// <summary>
        /// The path to data storing file.
        /// </summary>
        private readonly string filePath;

        /// <summary>
        /// The logger object.
        /// </summary>
        private readonly ILogger logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonFileService"/> class.
        /// </summary>
        /// <param name="filePath">
        /// The path to data storing file.
        /// </param>
        /// <param name="logger">
        /// The logger object.
        /// </param>
        public JsonFileService(string filePath, ILogger logger)
        {
            this.filePath = filePath;
            this.logger = logger;
            this.logger.Information("Starting JsonFileService");
        }

        /// <summary>
        /// Read from file object.
        /// </summary>
        /// <typeparam name="T">
        /// Object type to be read.
        /// </typeparam>
        /// <returns>
        /// Loaded object.
        /// </returns>
        /// <exception cref="FileLoadException">
        /// Throws when there is problem with loading file.
        /// </exception>
        /// <exception cref="FileNotFoundException">
        /// Throws when file is not found.
        /// </exception>
        public async Task<T> ReadFromFileAsync<T>()
        {
            this.logger.Debug("Loading saved {0} from file", typeof(T));
            if (File.Exists(this.filePath))
            {
                string readJsonString = await File.ReadAllTextAsync(this.filePath);
                T? tagStorage = JsonConvert.DeserializeObject<T>(readJsonString);
                if (tagStorage != null)
                {
                    this.logger.Debug("{0} loaded successfully", typeof(T));
                    return tagStorage;
                }
                else
                {
                    throw new FileLoadException("There is no saved tags");
                }
            }
            else
            {
                this.logger.Fatal("File with tags was not found");
                throw new FileNotFoundException("File with tags was not found");
            }
        }

        /// <summary>
        /// Write JObject to data storage file.
        /// </summary>
        /// <param name="tagsToSave">
        /// JObject tag to save.
        /// </param>
        /// <returns>
        /// Always returns true.
        /// </returns>
        public async Task WriteTagsToFileAsync(JObject tagsToSave)
        {
            this.logger.Information("Starting saving tags to file {0}", this.filePath);

            using (StreamWriter streamWriter = new StreamWriter(this.filePath))
            using (JsonWriter jsonWriter = new JsonTextWriter(streamWriter))
            {
                jsonWriter.Formatting = Formatting.Indented;
                await tagsToSave.WriteToAsync(jsonWriter);
            }

            this.logger.Information("tags to file {0} ended", this.filePath);
        }
    }
}
