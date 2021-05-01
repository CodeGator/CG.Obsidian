using CG.Obsidian.Web.Extensions;
using CG.Validations;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MudBlazor.Services;

namespace CG.Obsidian.Web
{
    /// <summary>
    /// This class contains startup logic for the server.
    /// </summary>
    public class Startup
    {
        // *******************************************************************
        // Properties.
        // *******************************************************************

        #region Properties

        /// <summary>
        /// This property contains the configuration for the server.
        /// </summary>
        public IConfiguration Configuration { get; }

        #endregion

        // *******************************************************************
        // Constructors.
        // *******************************************************************

        #region Constructors

        /// <summary>
        /// This constructor creates a new instance of the <see cref="Startup"/>
        /// class.
        /// </summary>
        /// <param name="configuration">The configuration to use for the operation.</param>
        public Startup(
            IConfiguration configuration
            )
        {
            // Validate the parameters before attempting to use them.
            Guard.Instance().ThrowIfNull(configuration, nameof(configuration));

            // Save the references.
            Configuration = configuration;
        }

        #endregion

        // *******************************************************************
        // Public methods.
        // *******************************************************************

        #region Public methods

        /// <summary>
        /// This method is called to configure services with the DI container.
        /// </summary>
        /// <param name="services">The service collection to use for the operation.</param>
        public void ConfigureServices(
            IServiceCollection services
            )
        {
            // Validate the parameters before attempting to use them.
            Guard.Instance().ThrowIfNull(services, nameof(services));
            
            // Add the custom logging.
            services.AddLogging(
                Configuration.GetSection("Logging")
                );

            // Add the custom identity.
            services.AddCustomIdentity(
                Configuration.GetSection("Identity")
                );

            // Add the custom Blazor services.
            services.AddCustomBlazor(
                Configuration
                );

            // Add custom CG.Obsidian services.
            services.AddCustomObsidian(
                Configuration.GetSection("CG.Obsidian")
                );

            // Add MudBlazor services.
            services.AddMudServices();

            // Add custom Swagger services.
            services.AddCustomSwagger(
                Configuration.GetSection("Swagger")
                );

            // Add custom health checks.
            services.AddCustomHealthChecks(
                Configuration
                );
        }

        // *******************************************************************

        /// <summary>
        /// This method is called to configure the web application.
        /// </summary>
        /// <param name="app">The application builder to use for the operation.</param>
        /// <param name="env">The runtime environment to use for the operation.</param>
        public void Configure(
            IApplicationBuilder app, 
            IWebHostEnvironment env
            )
        {
            // Validate the parameters before attempting to use them.
            Guard.Instance().ThrowIfNull(app, nameof(app))
                .ThrowIfNull(env, nameof(env));

            // Use custom Blazor logic.
            app.UseCustomBlazor(env);

            // Use custom CG.Obsidian logic.
            app.UseCustomObsidian(env);

            // Use custom swagger.
            app.UseCustomSwagger(env);

            // Use custom health checks.
            app.UseCustomHealthChecks(env);
        }

        #endregion
    }
}
