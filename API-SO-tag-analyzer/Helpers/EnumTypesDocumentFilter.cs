using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace API_SO_tag_analyzer.Helpers
{
    /// <summary>
    /// Enums types document filter.
    /// </summary>
    public class EnumTypesDocumentFilter : IDocumentFilter
    {
        /// <summary>
        /// Apply filters on application start.
        /// </summary>
        /// <param name="swaggerDoc">
        /// OpenApi object.
        /// </param>
        /// <param name="context">
        /// Document filter context.
        /// </param>
        public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
        {
            foreach (var path in swaggerDoc.Paths.Values)
            {
                foreach (var operation in path.Operations.Values)
                {
                    foreach (var parameter in operation.Parameters)
                    {
                        var schemaReferenceId = parameter.Schema.Reference?.Id;

                        if (string.IsNullOrEmpty(schemaReferenceId))
                        {
                            continue;
                        }

                        var schema = context.SchemaRepository.Schemas[schemaReferenceId];

                        if (schema.Enum == null || schema.Enum.Count == 0)
                        {
                            continue;
                        }

                        parameter.Description += "<p>Variants:</p>";

                        int cutStart = schema.Description.IndexOf("<ul>");

                        int cutEnd = schema.Description.IndexOf("</ul>") + 5;

                        parameter.Description += schema.Description
                            .Substring(cutStart, cutEnd - cutStart);
                    }
                }
            }
        }
    }
}
