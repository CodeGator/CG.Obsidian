using CG.Linq.EFCore;
using CG.Obsidian.Repositories;
using CG.Obsidian.SqlServer;
using CG.Obsidian.SqlServer.Repositories;
using CG.Obsidian.SqlServer.Repositories.Options;
using CG.Validations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// This class contains extension methods related to the <see cref="IServiceCollection"/>
    /// type, for the CG.Obsidian sql server library.
    /// </summary>
    public static partial class ServiceCollectionExtensions
    {
        // *******************************************************************
        // Public methods.
        // *******************************************************************

        #region Public methods

        /// <summary>
        /// This method adds SQL Server repositories for the CG.Obsidian library.
        /// </summary>
        /// <param name="serviceCollection">The service collection to use for
        /// the operation.</param>
        /// <param name="configuration">The configuration to use for the operation.</param>
        /// <param name="serviceLifetime">The service lifetime to use for the operation.</param>
        /// <returns>The value of the <paramref name="serviceCollection"/> parameter,
        /// for chaining calls together.</returns>
        public static IServiceCollection AddSqlServerRepositories(
            this IServiceCollection serviceCollection,
            IConfiguration configuration,
            ServiceLifetime serviceLifetime = ServiceLifetime.Scoped
            )
        {
            // Validate the parameters before attempting to use them.
            Guard.Instance().ThrowIfNull(serviceCollection, nameof(serviceCollection))
                .ThrowIfNull(configuration, nameof(configuration));

            // Register the EFCORE options.
            serviceCollection.ConfigureOptions<ObsidianRepositoryOptions>(
                configuration,
                out var repositoryOptions
                );

            // Register the data-context.
            serviceCollection.AddTransient<ObsidianDbContext>(serviceProvider =>
            {
                // Get the options from the DI container.
                var options = serviceProvider.GetRequiredService<IOptions<ObsidianRepositoryOptions>>();

                // Create the options builder.
                var builder = new DbContextOptionsBuilder<ObsidianDbContext>();

                // Should we add explicit SQL logging?
                if (options.Value.LogAllQueries)
                {
                    // Add logging.
                    builder.UseLoggerFactory(
                        serviceProvider.GetRequiredService<ILoggerFactory>()
                        );
                }

#if DEBUG
                // Log everything in development.    
                builder.EnableDetailedErrors()
                    .EnableSensitiveDataLogging(); 
#endif

                // Configure the options.
                builder.UseSqlServer(options.Value.ConnectionString);

                // Create the data-context.
                var context = new ObsidianDbContext(builder.Options);

                // Return the data-context.
                return context;
            });

            // Register the data-context factory.
            serviceCollection.AddDbContextFactory<ObsidianDbContext>(builder =>
            {
                // Configure the options.
                builder.UseSqlServer(repositoryOptions.ConnectionString);
            });

            // Register the repositories.
            serviceCollection.Add<IMimeTypeRepository, MimeTypeRepository>(serviceLifetime);
            serviceCollection.Add<IFileExtensionRepository, FileExtensionRepository>(serviceLifetime);

            // Return the service collection.
            return serviceCollection;
        }

        #endregion
    }
}
