using CG.Business.Repositories;
using CG.Linq.EFCore;
using CG.Linq.EFCore.Repositories;
using CG.Obsidian.Models;
using CG.Obsidian.Repositories;
using CG.Obsidian.SqlServer.Repositories.Options;
using CG.Validations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace CG.Obsidian.SqlServer.Repositories
{
    /// <summary>
    /// This class is a SQL-Server implementation of the <see cref="IFileExtensionRepository"/>
    /// interface.
    /// </summary>
    public class FileExtensionRepository :
        EFCoreRepositoryBase<ObsidianDbContext, IOptions<ObsidianRepositoryOptions>>, 
        IFileExtensionRepository
    {
        // *******************************************************************
        // Properties.
        // *******************************************************************

        #region Properties

        /// <summary>
        /// This property contains a reference to a logger.
        /// </summary>
        protected ILogger<FileExtensionRepository> Logger { get; set; }

        #endregion

        // *******************************************************************
        // Constructors.
        // *******************************************************************

        #region Constructors

        /// <summary>
        /// This constructor creates a new instance of the <see cref="FileExtensionRepository"/>
        /// class.
        /// </summary>
        /// <param name="options">The options with the repository.</param>
        /// <param name="factory">The data-context factory to use with the repository.</param>
        /// <param name="logger">The logger to use with the repository.</param>
        public FileExtensionRepository(
            IOptions<ObsidianRepositoryOptions> options,
            IDbContextFactory<ObsidianDbContext> factory,
            ILogger<FileExtensionRepository> logger
            ) : base(options, factory)
        {
            // Validate the parameters before attempting to use them.
            Guard.Instance().ThrowIfNull(logger, nameof(logger));

            // Save the references.
            Logger = logger;
        }        

        #endregion

        // *******************************************************************
        // Public methods.
        // *******************************************************************

        #region Public methods

        /// <inheritdoc/>
        public virtual IQueryable<FileExtension> AsQueryable()
        {
            try
            {
                // Create the context.
                var context = Factory.CreateDbContext();

                // Defer to the context.
                var query = context.FileExtensions
                    .OrderBy(x => x.Extension);

                // Return the results.
                return query;
            }
            catch (Exception ex)
            {
                // Tell the world what happened.
                Logger.LogError(
                    ex,
                    "Failed to query for file extensions!"
                    );

                // Provide better context for the error.
                throw new RepositoryException(
                    message: $"Failed to query for file extensions!",
                    innerException: ex
                    ).SetCallerInfo()
                     .SetOriginator(nameof(FileExtensionRepository))
                     .SetDateTime();
            }
        }

        // *******************************************************************

        /// <inheritdoc />
        public virtual async Task<FileExtension> AddAsync(
            FileExtension model,
            CancellationToken cancellationToken = default
            )
        {
            try
            {
                // Validate the parameters before attempting to use them.
                Guard.Instance().ThrowIfNull(model, nameof(model));

                // Create a context.
                var context = Factory.CreateDbContext();

                // Add to the data-context.
                var entity = await context.FileExtensions.AddAsync(
                    model,
                    cancellationToken
                    ).ConfigureAwait(false);

                // Save the changes.
                await context.SaveChangesAsync(
                    cancellationToken
                    ).ConfigureAwait(false);

                // Return the newly added model.
                return entity.Entity;
            }
            catch (Exception ex)
            {
                // Tell the world what happened.
                Logger.LogError(
                    ex,
                    "Failed to add a new file extension!",
                    (model != null ? JsonSerializer.Serialize(model) : "null")
                    );

                // Provide better context for the error.
                throw new RepositoryException(
                    message: $"Failed to add a new file extension!",
                    innerException: ex
                    ).SetCallerInfo()
                     .SetOriginator(nameof(FileExtensionRepository))
                     .SetMethodArguments(("model", model))
                     .SetDateTime();
            }
        }

        // *******************************************************************

        /// <inheritdoc />
        public virtual async Task<FileExtension> UpdateAsync(
            FileExtension model,
            CancellationToken cancellationToken = default
            )
        {
            try
            {
                // Validate the parameters before attempting to use them.
                Guard.Instance().ThrowIfNull(model, nameof(model));

                // Create a context.
                var context = Factory.CreateDbContext();

                // Find the model in the data-context.
                var originalModel = context.FileExtensions.Find(
                    model.Id, 
                    model.MimeTypeId
                    );

                // Did we fail?
                if (null == originalModel)
                {
                    // Panic!
                    throw new KeyNotFoundException(
                        message: $"Keys: Id: {model.Id}, MimeTypeId: {model.MimeTypeId}"
                        );
                }

                // Update the editable properties.
                originalModel.UpdatedDate = model.UpdatedDate;
                originalModel.UpdatedBy = model.UpdatedBy;
                originalModel.Extension = model.Extension;

                // Save the changes.
                await context.SaveChangesAsync(
                    cancellationToken
                    ).ConfigureAwait(false);

                // Return the newly updated model.
                return originalModel;
            }
            catch (Exception ex)
            {
                // Tell the world what happened.
                Logger.LogError(
                    ex,
                    "Failed to update an existing file extension!",
                    (model != null ? JsonSerializer.Serialize(model) : "null")
                    );

                // Provide better context for the error.
                throw new RepositoryException(
                    message: $"Failed to update an existing file extension!",
                    innerException: ex
                    ).SetCallerInfo()
                     .SetOriginator(nameof(FileExtensionRepository))
                     .SetMethodArguments(("model", model))
                     .SetDateTime();
            }
        }

        // *******************************************************************

        /// <inheritdoc />
        public virtual async Task DeleteAsync(
            FileExtension model,
            CancellationToken cancellationToken = default
            )
        {
            try
            {
                // Validate the parameters before attempting to use them.
                Guard.Instance().ThrowIfNull(model, nameof(model));

                // Create a context.
                var context = Factory.CreateDbContext();

                // Defer to the data-context.
                context.FileExtensions.Remove(model);

                // Save the changes.
                await context.SaveChangesAsync(
                    cancellationToken
                    ).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                // Tell the world what happened.
                Logger.LogError(
                    ex,
                    "Failed to delete an existing file extension!",
                    (model != null ? JsonSerializer.Serialize(model) : "null")
                    );

                // Provide better context for the error.
                throw new RepositoryException(
                    message: $"Failed to delete an existing file extension!",
                    innerException: ex
                    ).SetCallerInfo()
                     .SetOriginator(nameof(FileExtensionRepository))
                     .SetMethodArguments(("model", model))
                     .SetDateTime();
            }
        }

        #endregion
    }
}
