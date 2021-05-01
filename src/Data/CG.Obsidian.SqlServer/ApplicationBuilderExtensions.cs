using CG.Obsidian.SqlServer;
using CG.Obsidian.SqlServer.Repositories.Options;
using CG.Validations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Microsoft.AspNetCore.Builder
{
    /// <summary>
    /// This class contains extension methods related to the <see cref="IApplicationBuilder"/>
    /// type.
    /// </summary>
    /// <remarks>
    /// This class contains only those extension methods that are related to the logic
    /// within the <see cref="CG.Beryl.SqlServer"/> library itself. 
    /// </remarks>
    public static partial class ApplicationBuilderExtensions
    {
        // *******************************************************************
        // Public methods.
        // *******************************************************************

        #region Public methods

        /// <summary>
        /// This method wires up any startup logic required to support the 
        /// repositories and underyling SQL Server for the CG.Beryl library.
        /// </summary>
        /// <param name="applicationBuilder">The application builder to use for
        /// the operation.</param>
        /// <param name="configurationSection">The configuration section name
        /// that corresponds with the repositories.</param>
        /// <returns>The value of the <paramref name="applicationBuilder"/> parameter,
        /// for chaining calls together.</returns>
        public static IApplicationBuilder UseSqlServerRepositories(
            this IApplicationBuilder applicationBuilder,
            string configurationSection
            )
        {
            // Validate the parameters before attempting to use them.
            Guard.Instance().ThrowIfNull(applicationBuilder, nameof(applicationBuilder))
                .ThrowIfNullOrEmpty(configurationSection, nameof(configurationSection));

            // Startup EFCore.
            applicationBuilder.UseEFCore<ObsidianDbContext, ObsidianRepositoryOptions>(
                (context, wasDropped, wasMigrated) =>
            {
                // Create a logger.
                var logger = applicationBuilder.ApplicationServices.GetRequiredService<ILogger<ObsidianDbContext>>();

                // Let the world know what happened.
                logger.LogInformation(
                    $"Initialized: '{context.ContextId}', dropped: '{wasDropped}', migrated: '{wasMigrated}'"
                    );

                // Add seed data to the data-context.
                context.ApplySeedData(
                    logger
                    );
            });

            // Return the application builder.
            return applicationBuilder;
        }

        #endregion
    }
}
