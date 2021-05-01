using CG.Business.Managers;
using CG.Obsidian.Models;
using CG.Obsidian.Stores;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace CG.Obsidian.Managers
{
    /// <summary>
    /// This interface represents an obbject that manages operations
    /// related to MIME types.
    /// </summary>
    public interface IMimeTypeManager : IManager
    {
        /// <summary>
        /// This property contains a reference to an object stores
        /// and retrieves <see cref="Models.MimeType"/> objects.
        /// </summary>
        IMimeTypeStore MimeTypes { get; }

        /// <summary>
        /// This property contains a reference to an object stores
        /// and retrieves <see cref="Models.FileExtension"/> objects.
        /// </summary>
        IFileExtensionStore FileExtensions { get; }

        /// <summary>
        /// This method returns a list of mime type(s) that are associated
        /// with the specified file extension.
        /// </summary>
        /// <param name="extension">The file extension to use for the operation.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>A task to perform the operation, that returns the results
        /// of the search.</returns>
        Task<IEnumerable<MimeType>> FindByExtensionAsync(
            string extension,
            CancellationToken cancellationToken = default
            );
    }
}
