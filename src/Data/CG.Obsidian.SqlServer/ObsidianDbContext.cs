using CG.Obsidian.Models;
using CG.Obsidian.SqlServer.Maps;
using CG.Validations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;

namespace CG.Obsidian.SqlServer
{
    /// <summary>
    /// This class is a data-context for the CG.Obsidian library.
    /// </summary>
    public class ObsidianDbContext : DbContext
    {
        // *******************************************************************
        // Properties.
        // *******************************************************************

        #region Properties

        /// <summary>
        /// This property contains a collection of mime types.
        /// </summary>
        public virtual DbSet<MimeType> MimeTypes { get; set; }

        /// <summary>
        /// This property contains a collection of file extension types.
        /// </summary>
        public virtual DbSet<FileExtension> FileExtensions { get; set; }
        
        #endregion

        // *******************************************************************
        // Constructors.
        // *******************************************************************

        #region Constructors

        /// <summary>
        /// This constructor creates a new instance of the <see cref="ObsidianDbContext"/>
        /// class.
        /// </summary>
        /// <param name="options">The options to use with the data-context.</param>
        public ObsidianDbContext(
            DbContextOptions<ObsidianDbContext> options
            ) : base(options)
        {

        }

        #endregion

        // *******************************************************************
        // Protected methods.
        // *******************************************************************

        #region Protected methods

        /// <summary>
        /// This method is called to create the model for the data-context.
        /// </summary>
        /// <param name="modelBuilder">The builder to use for the operation.</param>
        protected override void OnModelCreating(
            ModelBuilder modelBuilder
            )
        {
            // Build up the data model.
            modelBuilder.ApplyConfiguration(new MimeTypeMap());
            modelBuilder.ApplyConfiguration(new FileExtensionMap());
            
            // Give the base class a chance.
            base.OnModelCreating(modelBuilder);
        }
                
        #endregion
    }
}
