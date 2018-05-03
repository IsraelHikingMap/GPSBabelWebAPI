using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace GpsBabelWebApi
{
    /// <summary>
    /// Runs the application
    /// </summary>
    public class Program
    {
        /// <summary>
        /// Main entry point for application
        /// </summary>
        /// <param name="args"></param>
        public static void Main(string[] args)
        {
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .UseUrls("http://0.0.0.0:11984")
                .Build()
                .Run();
        }           
    }
}
