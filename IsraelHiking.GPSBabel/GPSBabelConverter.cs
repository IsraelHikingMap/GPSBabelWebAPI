using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;

namespace IsraelHiking.GPSBabel
{
    public interface IGPSBabelConverter
    {
        byte[] Run(byte[] content, string inputFormat, string outputFormat, string parameters);
    }

    public class GPSBabelConverter : IGPSBabelConverter
    {
        public byte[] Run(byte[] content, string inputFormat, string outputFormat, string parameters)
        {
            var inputTempfileName = Path.GetTempFileName();
            File.WriteAllBytes(inputTempfileName, content);
            // file names are created to overcome utf-8 issues in file name.
            var outputTempfileName = Path.GetTempFileName();
            var arguments = "-i " + inputFormat + " -f \"" + inputTempfileName + "\" " + (parameters ?? string.Empty) + " -o " + outputFormat + " -F \"" +
                            outputTempfileName + "\"";
            var osDirectory = RuntimeInformation.OSDescription.ToLowerInvariant().Contains("win")
                ? "win"
                : "debian";
            var dllFolder = Path.GetDirectoryName(Assembly.GetAssembly(typeof(GPSBabelConverter)).Location);
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
            File.Delete(inputTempfileName);
            var outputContent = File.ReadAllBytes(outputTempfileName);
            File.Delete(outputTempfileName);
            return outputContent;
        }
    }
}
