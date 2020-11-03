using System.Diagnostics;
using System.IO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GpsBabelWebApi
{
    /// <summary>
    /// The controller to used to wrap GPSBabel command line.
    /// </summary>
    [Route("")]
    public class GPSBabelController : Controller
    {
        public GPSBabelController()
        {
            
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
        public IActionResult Post(IFormFile file, [FromForm]string inputFormat, [FromForm]string outputFormat, [FromForm] string parameters)
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
            var outputContent = Convert(memoryStream.ToArray(), inputFormat, outputFormat, parameters);
            return File(outputContent, "application/octet-stream");
        }

        private byte[] Convert(byte[] content, string inputFormat, string outputFormat, string parameters)
        {
            var inputTempfileName = Path.GetTempFileName();
            System.IO.File.WriteAllBytes(inputTempfileName, content);
            // file names are created to overcome utf-8 issues in file name.
            var outputTempfileName = Path.GetTempFileName();
            var arguments = "-i " + inputFormat + " -f \"" + inputTempfileName + "\" " + (parameters ?? string.Empty) + " -o " + outputFormat + " -F \"" +
                            outputTempfileName + "\"";
            using var process = Process.Start(new ProcessStartInfo
            {
                UseShellExecute = false,
                CreateNoWindow = true,
                Arguments = arguments,
                WorkingDirectory = Path.GetTempPath(),
                FileName = "gpsbabel"
            });
            process.WaitForExit(100000);
            System.IO.File.Delete(inputTempfileName);
            var outputContent = System.IO.File.ReadAllBytes(outputTempfileName);
            System.IO.File.Delete(outputTempfileName);
            return outputContent;
        }
    }
}
