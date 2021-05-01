using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace CG.Obsidian.Web
{
    /// <summary>
    /// This class contains the main program logic.
    /// </summary>
    public class Program
    {
        // *******************************************************************
        // Public methods.
        // *******************************************************************

        #region Public methods

        /// <summary>
        /// This method is the main entry point for the process.
        /// </summary>
        /// <param name="args">Optional command line arguments</param>
        public static void Main(string[] args)
        {
            // Create the builder, then the host, then run the host.
            CreateHostBuilder(args).Build().Run();
        }

        // *******************************************************************

        /// <summary>
        /// This method returns a host builder.
        /// </summary>
        /// <param name="args">Optional command line arguments</param>
        /// <returns>An <see cref="IHostBuilder"/> instance.</returns>
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });

        #endregion
    }
}
