using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GpsBabelWebApi
{
    [Route("")]
    public class ExecuteController : Controller
    {
        // GET api/values
        [HttpGet]
        public IActionResult Get()
        {
            var content = @"<!DOCTYPE html>
<html lang='en' ng-app='APP'>
<head>
    <meta charset='UTF-8'>
    <title>angular file upload</title>
</head>
<body>
    This is a simple example so you can test the API - when sending a POST form data make sure to send 'inputFormat' and 'outputFormat'.
    <form method='post' action='/' enctype='multipart/form-data'>
        Input Format: <input type='text' name='inputFormat' palceholder='Input Format' /><br/>
        Output Format: <input type='text' name='outputFormat' palceholder='Output Format' /><br/>
        Additional Parameters: <input type='text' name='parameters' palceholder='Additional Parameters' /><br/>
        <input type='file' name='file'><br/>
        <input type='submit'>
    </form>
 </body>
</html>";
            var contentResult = new ContentResult
            {
                Content = content,
                ContentType = "text/html"
            };
            return contentResult;
        }

        // POST api/values
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
            var inputTempfileName = Path.GetTempFileName();
            // file names are created to overcome utf-8 issues in file name.
            var outputTempfileName = Path.GetTempFileName();
            using (var fileStream = System.IO.File.OpenWrite(inputTempfileName))
            {
                stream.CopyTo(fileStream);
            }

            var arguments = "-i " + inputFormat + " -f \"" + inputTempfileName + "\" " + (parameters ?? string.Empty) + " -o " + outputFormat + " -F \"" +
                            outputTempfileName + "\"";
            var osDirectory = RuntimeInformation.OSDescription.ToLowerInvariant().Contains("win")
                ? "win"
                : "debian";
            var dllFolder = Path.GetDirectoryName(Assembly.GetAssembly(typeof(ExecuteController)).Location);
            var workingDirectory = Path.Combine(dllFolder, "runtimes", osDirectory, "native");
            using (var process = Process.Start(new ProcessStartInfo
            {
                UseShellExecute = false,
                CreateNoWindow = true,
                Arguments = arguments,
                WorkingDirectory = workingDirectory,
                FileName = Path.Combine(workingDirectory, "gpsbabel.exe")
            }))
            {
                process.WaitForExit(100000);
            }
            System.IO.File.Delete(inputTempfileName);
            var outputContent = System.IO.File.ReadAllBytes(outputTempfileName);
            System.IO.File.Delete(outputTempfileName);
            return File(outputContent, "application/octet-stream");
        }
    }
}
