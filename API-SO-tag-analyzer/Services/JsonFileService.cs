using API_SO_tag_analyzer.Data;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Serilog;


namespace API_SO_tag_analyzer.Services
{
    public class JsonFileService
    {
        private readonly string filePath;



        public JsonFileService(string filePath)
        {
            this.filePath = filePath;
            Log.Information("Starting JsonFileService");
        }

        //public async Task<TagStorage> ReadFromFileAsync()
        //{
        //    try
        //    {
        //        using (StreamReader file = File.OpenText(this.filePath))
        //        {
        //            var tagStorage = (TagStorage)JsonSerializer.Deserialize(file, typeof(TagStorage));
        //            return tagStorage;
        //        }
        //    }
        //    catch (FileNotFoundException)
        //    {
        //        return new TagStorage { Tags = new List<string>() };
        //    }
        //}

        public async Task WriteTagsToFileAsync(JObject tagsToSave)
        {
            Log.Information("Starting saving tags to file {0}", this.filePath);
            JsonSerializer serializer = new JsonSerializer();
            serializer.NullValueHandling = NullValueHandling.Ignore;

            using (StreamWriter streamWriter = new StreamWriter(this.filePath))
            using (JsonWriter jsonWriter = new JsonTextWriter(streamWriter))
            {
                jsonWriter.Formatting = Formatting.Indented;
                await tagsToSave.WriteToAsync(jsonWriter);
            }
        }

        private async Task<T> DeserializeResponse<T>(string httpContent)
        {
            return JsonConvert.DeserializeObject<T>(httpContent);
        }
    }
}
