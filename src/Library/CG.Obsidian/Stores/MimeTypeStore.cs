using CG.Obsidian.Models;
using CG.Business.Stores;
using CG.Validations;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using System.Collections.Generic;
using CG.Obsidian.Repositories;
using System.Text.Json;

namespace CG.Obsidian.Stores
{
    /// <summary>
    /// This class is a default implementation of the <see cref="IMimeTypeStore"/>
    /// interface.
    /// </summary>
    public class MimeTypeStore : StoreBase, IMimeTypeStore
    {
        // *******************************************************************
        // Properties.
        // *******************************************************************

        #region Properties

        /// <summary>
        /// This property contains a referene to a repository.
        /// </summary>
        protected IMimeTypeRepository Repository { get; }

        /// <summary>
        /// This property contains a reference to a logger.
        /// </summary>
        protected ILogger<MimeTypeStore> Logger { get; }

        #endregion

        // *******************************************************************
        // Constructors.
        // *******************************************************************

        #region Constructors

        /// <summary>
        /// This constructor creates a new instance of the <see cref="MimeTypeStore"/>
        /// class.
        /// </summary>
        /// <param name="repository">The repository to use with the store.</param>
        /// <param name="logger">The logger to use with the store.</param>
        public MimeTypeStore(
            IMimeTypeRepository repository,
            ILogger<MimeTypeStore> logger
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
        public virtual IQueryable<MimeType> AsQueryable()
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
                    "Failed to query for mime types!"
                    );

                // Provide better context for the error.
                throw new StoreException(
                    message: $"Failed to query for mime types!",
                    innerException: ex
                    ).SetCallerInfo()
                     .SetOriginator(nameof(MimeTypeStore))
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

                // Loop through the extensions (if any).
                for (var x = 0; x < model.Extensions.Count; x++)
                {
                    // Ensure the extension starts with '.'
                    model.Extensions[x].Extension = 
                        model.Extensions[x].Extension.StartsWith(".")
                        ? model.Extensions[x].Extension 
                        : $".{model.Extensions[x].Extension}";
                }

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
                    "Failed to add a new mime type!",
                    (model != null ? JsonSerializer.Serialize(model) : "null")
                    );

                // Provide better context for the error.
                throw new StoreException(
                    message: $"Failed to add a new mime type!",
                    innerException: ex
                    ).SetCallerInfo()
                     .SetOriginator(nameof(MimeTypeStore))
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

                // Loop through the extensions (if any).
                for (var x = 0; x < model.Extensions.Count; x++)
                {
                    // Ensure the extension starts with '.'
                    model.Extensions[x].Extension =
                        model.Extensions[x].Extension.StartsWith(".")
                        ? model.Extensions[x].Extension
                        : $".{model.Extensions[x].Extension}";
                }

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
                    "Failed to update an existing mime type!",
                    (model != null ? JsonSerializer.Serialize(model) : "null")
                    );

                // Provide better context for the error.
                throw new StoreException(
                    message: $"Failed to update an existing mime type!",
                    innerException: ex
                    ).SetCallerInfo()
                     .SetOriginator(nameof(MimeTypeStore))
                     .SetMethodArguments(("model", model))
                     .SetDateTime();
            }
        }

        // *******************************************************************

        /// <inheritdoc />
        public virtual async Task DeleteAsync(
            int id,
            CancellationToken cancellationToken = default
            )
        {
            try
            {
                // Validate the parameters before attempting to use them.
                Guard.Instance().ThrowIfZero(id, nameof(id));

                // Look for the corresponding model.
                var model = Repository.AsQueryable().FirstOrDefault(x => 
                    x.Id == id 
                    );

                // Did we fail?
                if (null == model)
                {
                    // Panic!
                    throw new KeyNotFoundException(
                        message: $"Key: {id}"
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
                // Let the world know what happened.
                Logger.LogError(
                    ex,
                    $"Failed to delete an existing mime type!",
                    id
                    );

                // Provide better context for the error.
                throw new StoreException(
                    message: $"Failed to delete an existing mime type!",
                    innerException: ex
                    ).SetCallerInfo()
                     .SetOriginator(nameof(MimeTypeStore))
                     .SetMethodArguments(("id", id))
                     .SetDateTime();
            }
        }

        #endregion
    }
}
