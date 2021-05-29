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
    /// This class is a SQL-Server implementation of the <see cref="IMimeTypeRepository"/>
    /// interface.
    /// </summary>
    public class MimeTypeRepository :
        EFCoreRepositoryBase<ObsidianDbContext, IOptions<ObsidianRepositoryOptions>>, 
        IMimeTypeRepository
    {
        // *******************************************************************
        // Properties.
        // *******************************************************************

        #region Properties

        /// <summary>
        /// This property contains a reference to a logger.
        /// </summary>
        protected ILogger<MimeTypeRepository> Logger { get; set; }

        #endregion

        // *******************************************************************
        // Constructors.
        // *******************************************************************

        #region Constructors

        /// <summary>
        /// This constructor creates a new instance of the <see cref="MimeTypeRepository"/>
        /// class.
        /// </summary>
        /// <param name="options">The options for the repository.</param>
        /// <param name="factory">The data-context factory for the repository.</param>
        /// <param name="logger">The logger to use with the repository.</param>
        public MimeTypeRepository(
            IOptions<ObsidianRepositoryOptions> options,
            IDbContextFactory<ObsidianDbContext> factory,
            ILogger<MimeTypeRepository> logger
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
        public virtual IQueryable<MimeType> AsQueryable()
        {
            try
            {
                // Create the context.
                var context = Factory.CreateDbContext();

                // Defer to the context.
                var query = context.MimeTypes
                    .Include(x => x.Extensions)
                    .OrderBy(x => x.Type)
                    .ThenBy(x => x.SubType);

                // Return the results.
                return query;
            }
            catch (Exception ex)
            {
                // Tell the world what happened.
                Logger.LogError(
                    ex,
                    "Failed to query for mime types!"
                    );

                // Provide better context for the error.
                throw new RepositoryException(
                    message: $"Failed to query for mime types!",
                    innerException: ex
                    ).SetCallerInfo()
                     .SetOriginator(nameof(MimeTypeRepository))
                     .SetDateTime();
            }
        }

        // *******************************************************************

        /// <inheritdoc />
        public virtual async Task<MimeType> AddAsync(
            MimeType model,
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
                var entity = await context.MimeTypes.AddAsync(
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
                    "Failed to add a new mime type!",
                    (model != null ? JsonSerializer.Serialize(model) : "null")
                    );

                // Provide better context for the error.
                throw new RepositoryException(
                    message: $"Failed to add a new mime type!",
                    innerException: ex
                    ).SetCallerInfo()
                     .SetOriginator(nameof(MimeTypeRepository))
                     .SetMethodArguments(("model", model))
                     .SetDateTime();
            }
        }

        // *******************************************************************

        /// <inheritdoc />
        public virtual async Task<MimeType> UpdateAsync(
            MimeType model,
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
                var originalModel = context.MimeTypes
                    .Where(x => x.Id == model.Id)
                    .Include(x => x.Extensions)
                    .FirstOrDefault();

                // Did we fail?
                if (null == originalModel)
                {
                    // Panic!
                    throw new KeyNotFoundException(
                        message: $"Key: {model.Id}"
                        );
                }

                // Update the editable properties.
                originalModel.UpdatedDate = model.UpdatedDate;
                originalModel.UpdatedBy = model.UpdatedBy;
                originalModel.Type = model.Type;
                originalModel.SubType = model.SubType;
                originalModel.Description = model.Description;
                originalModel.Extensions = model.Extensions;
                
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
                    "Failed to update an existing mime type!",
                    (model != null ? JsonSerializer.Serialize(model) : "null")
                    );

                // Provide better context for the error.
                throw new RepositoryException(
                    message: $"Failed to update an existing mime type!",
                    innerException: ex
                    ).SetCallerInfo()
                     .SetOriginator(nameof(MimeTypeRepository))
                     .SetMethodArguments(("model", model))
                     .SetDateTime();
            }
        }

        // *******************************************************************

        /// <inheritdoc />
        public virtual async Task DeleteAsync(
            MimeType model,
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
                context.MimeTypes.Remove(model);

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
                    "Failed to delete an existing mime type!",
                    (model != null ? JsonSerializer.Serialize(model) : "null")
                    );

                // Provide better context for the error.
                throw new RepositoryException(
                    message: $"Failed to delete an existing mime type!",
                    innerException: ex
                    ).SetCallerInfo()
                     .SetOriginator(nameof(MimeTypeRepository))
                     .SetMethodArguments(("model", model))
                     .SetDateTime();
            }
        }

        #endregion
    }
}
