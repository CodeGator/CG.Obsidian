using CG.Business.Managers;
using CG.Obsidian.Models;
using CG.Obsidian.Stores;
using CG.Validations;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;

namespace CG.Obsidian.Managers
{
    /// <summary>
    /// This class is a default implementation of the <see cref="IMimeTypeManager"/>
    /// interface.
    /// </summary>
    public class MimeTypeManager : ManagerBase, IMimeTypeManager
    {
        // *******************************************************************
        // Properties.
        // *******************************************************************

        #region Properties

        /// <inheritdoc/>
        public IMimeTypeStore MimeTypes { get; }

        /// <inheritdoc/>
        public IFileExtensionStore FileExtensions { get; }

        /// <summary>
        /// This property contains a reference to a logger.
        /// </summary>
        protected ILogger<MimeTypeManager> Logger { get; }

        #endregion

        // *******************************************************************
        // Constructors.
        // *******************************************************************

        #region Constructors

        /// <summary>
        /// This constructor creates a new instance of the <see cref="MimeTypeManager"/>
        /// class.
        /// </summary>
        /// <param name="mimeTypeStore">The mime type store to use with the 
        /// manager.</param>
        /// <param name="fileExtensionStore">The file extension store to use 
        /// with the manager.</param>
        /// <param name="logger">The logger to use with the manager.</param>
        public MimeTypeManager(
            IMimeTypeStore mimeTypeStore,
            IFileExtensionStore fileExtensionStore,
            ILogger<MimeTypeManager> logger
            )
        {
            // Validate the parameters before attempting to use them.
            Guard.Instance().ThrowIfNull(mimeTypeStore, nameof(mimeTypeStore))
                .ThrowIfNull(fileExtensionStore, nameof(fileExtensionStore))
                .ThrowIfNull(logger, nameof(logger));

            // Save the referrences.
            MimeTypes = mimeTypeStore;
            FileExtensions = fileExtensionStore;
            Logger = logger;
        }

        #endregion

        // *******************************************************************
        // Public methods.
        // *******************************************************************

        #region Public methods

        /// <inheritdoc/>
        public virtual Task<IEnumerable<MimeType>> FindByExtensionAsync(
            string extension,
            CancellationToken cancellationToken = default
            )
        {
            try
            {
                // Validate the parameters before attempting to use them.
                Guard.Instance().ThrowIfNullOrEmpty(extension, nameof(extension));

                // Make sure we have the prepended '.' character.
                var ext = extension.StartsWith('.') 
                    ? extension.Trim() 
                    : $".{extension.Trim()}";

                // Find matching mime types, by extension.
                var list = MimeTypes.AsQueryable()
                    .Where(x => x.Extensions.Any(y => y.Extension == ext))
                    .Distinct()
                    .OrderBy(x => x.Type)
                    .ThenBy(x => x.SubType)
                    .ToList();
                
                // Return the results.
                return Task.FromResult(
                    list.AsEnumerable()
                    );
            }
            catch (Exception ex)
            {
                // Provide better context for the error.
                throw new ManagerException(
                    message: $"Failed to query for mime type(s) by file extension!",
                    innerException: ex
                    ).SetCallerInfo()
                     .SetOriginator(nameof(MimeTypeManager))
                     .SetMethodArguments(("extension", extension))
                     .SetDateTime();
            }
        }

        #endregion
    }
}
