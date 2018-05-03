using System.IO;
using GPSBabelWebAPI;
using IsraelHiking.GPSBabel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace GpsBabelWebApi
{
    /// <summary>
    /// The controller to used to wrap GPSBabel command line.
    /// </summary>
    [Route("")]
    public class GPSBabelController : Controller
    {
        private readonly IGPSBabelConverter _gpsBabelConverter;

        public GPSBabelController()
        {
            _gpsBabelConverter = new GPSBabelConverter();
        }


        // POST api/values
        /// <summary>
        /// Using this request you can convert files using GPSBabel
        /// </summary>
        /// <param name="file">The input file to convert</param>
        /// <param name="inputFormat">The input file's format in GPSBabel commandline language</param>
        /// <param name="outputFormat">The requrired output format in GPSBabel commandline language</param>
        /// <param name="parameters">Additional parameters to use between input and output parameters</param>
        /// <returns>A converted binary data representing the converted file</returns>
        [HttpPost]
        [SwaggerOperationFilter(typeof(RequiredFileUploadParams))]
        public IActionResult Post([FromForm]IFormFile file, [FromForm]string inputFormat, [FromForm]string outputFormat, [FromForm] string parameters)
        {
            if (file == null)
            {
                return BadRequest("You must send a file.");
            }
            var stream = file.OpenReadStream();
            if (inputFormat == outputFormat && string.IsNullOrEmpty(parameters))
            {
                return new FileStreamResult(stream, file.ContentType);
            }
            var memoryStream = new MemoryStream();
            file.CopyTo(memoryStream);
            var outputContent = _gpsBabelConverter.Run(memoryStream.ToArray(), inputFormat, outputFormat, parameters);
            return File(outputContent, "application/octet-stream");
        }
    }
}
