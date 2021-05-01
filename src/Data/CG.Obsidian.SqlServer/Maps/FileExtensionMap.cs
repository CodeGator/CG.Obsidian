using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace CG.Obsidian.SqlServer.Maps
{
    /// <summary>
    /// This class represents the entity mapping for the <see cref="Models.FileExtension"/>
    /// object.
    /// </summary>
    internal class FileExtensionMap : AuditedModelMapBase<Models.FileExtension>
    {
        // *******************************************************************
        // Public methods.
        // *******************************************************************

        #region Public methods

        /// <summary>
        /// This method configures the <see cref="Models.FileExtension"/> entity.
        /// </summary>
        /// <param name="builder">The builder to use for the operation.</param>
        public override void Configure(
            EntityTypeBuilder<Models.FileExtension> builder
            )
        {
            // Setup the table.
            builder.ToTable("FileExtensions", "Obsidian");

            // Setup the property.
            builder.Property(e => e.Id)
                .IsRequired()
                .ValueGeneratedOnAdd();

            // Setup the property.
            builder.Property(e => e.MimeTypeId)
                .IsRequired();

            // Setup the primary key.
            builder.HasKey(e => new { e.Id, e.MimeTypeId });

            // Setup the property.
            builder.Property(e => e.Extension)
                .IsRequired()
                .HasMaxLength(260);

            // Setup the index.
            builder.HasIndex(e => new 
            { 
                e.Extension
            }).IsUnique();

            // Give the base class a chance.
            base.Configure(builder);
        }

        #endregion
    }
}
