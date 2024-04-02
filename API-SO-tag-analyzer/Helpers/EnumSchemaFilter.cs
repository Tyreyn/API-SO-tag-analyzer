using System.Xml.Linq;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace API_SO_tag_analyzer.Helpers
{
    /// <summary>
    /// Enum schema filter.
    /// </summary>
    public class EnumSchemaFilter : ISchemaFilter
    {
        /// <summary>
        /// Output xml documentation.
        /// </summary>
        private readonly XDocument xmlComments;

        /// <summary>
        /// Initializes a new instance of the <see cref="EnumSchemaFilter"/> class.
        /// </summary>
        /// <param name="xmlPath">
        /// Path to xml document.
        /// </param>
        public EnumSchemaFilter(string xmlPath)
        {
            if (File.Exists(xmlPath))
            {
                this.xmlComments = XDocument.Load(xmlPath);
            }
        }

        /// <summary>
        /// Apply filters on application start.
        /// </summary>
        /// <param name="schema">
        /// OpenApi object.
        /// </param>
        /// <param name="context">
        /// Document filter context.
        /// </param>
        public void Apply(OpenApiSchema schema, SchemaFilterContext context)
        {
            if (this.xmlComments == null)
            {
                return;
            }

            if (schema.Enum != null && schema.Enum.Count > 0 &&
                context.Type != null && context.Type.IsEnum)
            {
                schema.Description += "<p>Members:</p><ul>";

                var fullTypeName = context.Type.FullName;

                foreach (var enumMemberName in schema.Enum.OfType<OpenApiString>().
                         Select(v => v.Value))
                {
                    var fullEnumMemberName = $"F:{fullTypeName}.{enumMemberName}";

                    var enumMemberComments = this.xmlComments.Descendants("member")
                                                         .FirstOrDefault(m => m.Attribute("name").Value
                                                                               .Equals(fullEnumMemberName, StringComparison.OrdinalIgnoreCase));

                    if (enumMemberComments == null)
                    {
                        continue;
                    }

                    var summary = enumMemberComments.Descendants("summary").FirstOrDefault();

                    if (summary == null)
                    {
                        continue;
                    }

                    schema.Description += $"<li><i>{enumMemberName}</i> - {summary.Value.Trim()}</li> ";
                }

                schema.Description += "</ul>";
            }
        }
    }
}