using CG.Obsidian.Models;
using CG.Business.Stores;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;

namespace CG.Obsidian.Stores
{
    /// <summary>
    /// This interface represents an object that stores and retrieves 
    /// <see cref="MimeType"/> objects.
    /// </summary>
    public interface IMimeTypeStore : IStore
    {
        /// <summary>
        /// This method returns a queryable sequence of <see cref="MimeType"/>
        /// objects.
        /// </summary>
        /// <returns>A queryable sequence of <see cref="MimeType"/> 
        /// objects.</returns>
        IQueryable<MimeType> AsQueryable();

        /// <summary>
        /// This method adds a new <see cref="MimeType"/> object
        /// to the store.
        /// </summary>
        /// <param name="model">The model to use for the operation.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>A task to perform the operation that returns the newly
        /// added <see cref="MimeType"/> object.</returns>
        Task<MimeType> AddAsync(
            MimeType model,
            CancellationToken cancellationToken = default
            );

        /// <summary>
        /// This method updates an existing <see cref="MimeType"/> object
        /// in the store.
        /// </summary>
        /// <param name="model">The model to use for the operation.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>A task to perform the operation that returns the newly
        /// updated <see cref="MimeType"/> object.</returns>
        Task<MimeType> UpdateAsync(
            MimeType model,
            CancellationToken cancellationToken = default
            );

        /// <summary>
        /// This method deletes an existing <see cref="MimeType"/> object
        /// from the store.
        /// </summary>
        /// <param name="id">The identifier for the operation.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>A task to perform the operation that returns the newly
        /// updated <see cref="MimeType"/> object.</returns>
        Task DeleteAsync(
            int id,
            CancellationToken cancellationToken = default
            );
    }
}
