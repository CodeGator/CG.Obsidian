using CG.Obsidian.Models;
using CG.Obsidian.Repositories;
using CG.Business.Stores;
using CG.Validations;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text.Json;

namespace CG.Obsidian.Stores
{
    /// <summary>
    /// This class is a default implementation of the <see cref="IFileExtensionStore"/>
    /// interface.
    /// </summary>
    public class FileExtensionStore : StoreBase, IFileExtensionStore
    {
        // *******************************************************************
        // Properties.
        // *******************************************************************

        #region Properties

        /// <summary>
        /// This property contains a referene to a repository.
        /// </summary>
        protected IFileExtensionRepository Repository { get; }

        /// <summary>
        /// This property contains a reference to a logger.
        /// </summary>
        protected ILogger<FileExtensionStore> Logger { get; }

        #endregion

        // *******************************************************************
        // Constructors.
        // *******************************************************************

        #region Constructors

        /// <summary>
        /// This constructor creates a new instance of the <see cref="FileExtensionStore"/>
        /// class.
        /// </summary>
        /// <param name="repository">The repository to use with the store.</param>
        /// <param name="logger">The logger to use with the store.</param>
        public FileExtensionStore(
            IFileExtensionRepository repository,
            ILogger<FileExtensionStore> logger
            )
        {
            // Validate the parameters before attempting to use them.
            Guard.Instance().ThrowIfNull(repository, nameof(repository))
                .ThrowIfNull(logger, nameof(logger));

            // Save the references.
            Repository = repository;
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
                // Defer to the repository.
                var query = Repository.AsQueryable();

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
                throw new StoreException(
                    message: $"Failed to query for file extensions!",
                    innerException: ex
                    ).SetCallerInfo()
                     .SetOriginator(nameof(FileExtensionStore))
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

                // Ensure the extension starts with '.'
                model.Extension = model.Extension.StartsWith(".")
                    ? model.Extension
                    : $".{model.Extension}";

                // Defer to the repository.
                var addedModel = await Repository.AddAsync(
                    model,
                    cancellationToken
                    ).ConfigureAwait(false);

                // Return the newly added model.
                return addedModel;
            }
            catch (Exception ex)
            {
                // Tell the world what happened.
                Logger.LogError(
                    ex,
                    "Failed to query for file extensions!",
                    (model != null ? JsonSerializer.Serialize(model) : "null")
                    );

                // Provide better context for the error.
                throw new StoreException(
                    message: $"Failed to add a new file extension!",
                    innerException: ex
                    ).SetCallerInfo()
                     .SetOriginator(nameof(FileExtensionStore))
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

                // Ensure the extension starts with '.'
                model.Extension = model.Extension.StartsWith(".")
                    ? model.Extension
                    : $".{model.Extension}";

                // Defer to the repository.
                var updatedModel = await Repository.UpdateAsync(
                    model,
                    cancellationToken
                    ).ConfigureAwait(false);

                // Return the newly updated model.
                return updatedModel;
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
                throw new StoreException(
                    message: $"Failed to update an existing file extension!",
                    innerException: ex
                    ).SetCallerInfo()
                     .SetOriginator(nameof(FileExtensionStore))
                     .SetMethodArguments(("model", model))
                     .SetDateTime();
            }
        }

        // *******************************************************************

        /// <inheritdoc />
        public virtual async Task DeleteAsync(
            int id,
            int mimeTypeId,
            CancellationToken cancellationToken = default
            )
        {
            try
            {
                // Validate the parameters before attempting to use them.
                Guard.Instance().ThrowIfZero(id, nameof(id))
                    .ThrowIfZero(mimeTypeId, nameof(mimeTypeId));

                // Look for the corresponding model.
                var model = Repository.AsQueryable().FirstOrDefault(x => 
                    x.Id == id && x.MimeTypeId == mimeTypeId
                    );

                // Did we fail?
                if (null == model)
                {
                    // Panic!
                    throw new KeyNotFoundException(
                        message: $"Keys: Id: {id}, MimeTypeId: {model.MimeTypeId}"
                        );
                }

                // Defer to the repository.
                await Repository.DeleteAsync(
                    model,
                    cancellationToken
                    ).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                // Tell the world what happened.
                Logger.LogError(
                    ex,
                    "Failed to delete an existing file extension!",
                    id,
                    mimeTypeId
                    );

                // Provide better context for the error.
                throw new StoreException(
                    message: $"Failed to delete an existing file extension!",
                    innerException: ex
                    ).SetCallerInfo()
                     .SetOriginator(nameof(FileExtensionStore))
                     .SetMethodArguments(
                        ("id", id), 
                        ("mimeTypeId", mimeTypeId)
                        ).SetDateTime();
            }
        }

        #endregion
    }
}
