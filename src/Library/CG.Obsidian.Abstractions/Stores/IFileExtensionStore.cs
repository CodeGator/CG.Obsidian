﻿using CG.Obsidian.Models;
using CG.Business.Stores;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;

namespace CG.Obsidian.Stores
{
    /// <summary>
    /// This interface represents an object that stores and retrieves 
    /// <see cref="FileExtension"/> objects.
    /// </summary>
    public interface IFileExtensionStore : IStore
    {
        /// <summary>
        /// This method returns a queryable sequence of <see cref="FileExtension"/>
        /// objects.
        /// </summary>
        /// <returns>A queryable sequence of <see cref="FileExtension"/> 
        /// objects.</returns>
        IQueryable<FileExtension> AsQueryable();

        /// <summary>
        /// This method adds a new <see cref="FileExtension"/> object
        /// to the store.
        /// </summary>
        /// <param name="model">The model to use for the operation.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>A task to perform the operation that returns the newly
        /// added <see cref="FileExtension"/> object.</returns>
        Task<FileExtension> AddAsync(
            FileExtension model,
            CancellationToken cancellationToken = default
            );

        /// <summary>
        /// This method updates an existing <see cref="FileExtension"/> object
        /// in the store.
        /// </summary>
        /// <param name="model">The model to use for the operation.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>A task to perform the operation that returns the newly
        /// updated <see cref="FileExtension"/> object.</returns>
        Task<FileExtension> UpdateAsync(
            FileExtension model,
            CancellationToken cancellationToken = default
            );

        /// <summary>
        /// This method deletes an existing <see cref="FileExtension"/> object
        /// from the store.
        /// </summary>
        /// <param name="id">The identifier for the operation.</param>
        /// <param name="mimeTypeId">The mime type identifier to use for the operation.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>A task to perform the operation that returns the newly
        /// updated <see cref="FileExtension"/> object.</returns>
        Task DeleteAsync(
            int id,
            int mimeTypeId,
            CancellationToken cancellationToken = default
            );
    }
}
