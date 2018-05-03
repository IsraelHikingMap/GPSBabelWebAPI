using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Http;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace GPSBabelWebAPI
{
    /// <summary>
    /// Allows required file upload for swagger API 
    /// </summary>
    public class RequiredFileUploadParams : IOperationFilter
    {   
        private const string FORM_DATA_MIME_TYPE = "multipart/form-data";
        private static readonly string[] FormFilePropertyNames =
            typeof(IFormFile).GetTypeInfo().DeclaredProperties.Select(x => x.Name).ToArray();

        /// <summary>
        /// Applys the required file upload button to the schema
        /// </summary>
        /// <param name="operation"></param>
        /// <param name="context"></param>
        public void Apply(Operation operation, OperationFilterContext context)
        {
            if (context.ApiDescription.ParameterDescriptions.Any(x => x.ModelMetadata.ContainerType == typeof(IFormFile)))
            {
                var formFileParameters = operation
                    .Parameters
                    .OfType<NonBodyParameter>()
                    .Where(x => FormFilePropertyNames.Contains(x.Name))
                    .ToArray();
                var index = operation.Parameters.IndexOf(formFileParameters.First());
                foreach (var formFileParameter in formFileParameters)
                {
                    operation.Parameters.Remove(formFileParameter);
                }

                var formFileParameterName = context
                    .ApiDescription
                    .ActionDescriptor
                    .Parameters
                    .Where(x => x.ParameterType == typeof(IFormFile))
                    .Select(x => x.Name)
                    .First();
                var parameter = new NonBodyParameter()
                {
                    Name = formFileParameterName,
                    In = "formData",
                    Description = "The file to upload.",
                    Required = true,
                    Type = "file"
                };
                operation.Parameters.Insert(index, parameter);

                if (!operation.Consumes.Contains(FORM_DATA_MIME_TYPE))
                {
                    operation.Consumes.Add(FORM_DATA_MIME_TYPE);
                }
            }
        }
    }
}
