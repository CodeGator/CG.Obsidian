using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace CG.Obsidian.SqlServer.Maps
{
    /// <summary>
    /// This class represents the entity mapping for the <see cref="Models.MimeType"/>
    /// object.
    /// </summary>
    internal class MimeTypeMap : AuditedModelMapBase<Models.MimeType>
    {
        // *******************************************************************
        // Public methods.
        // *******************************************************************

        #region Public methods

        /// <summary>
        /// This method configures the <see cref="Models.MimeType"/> entity.
        /// </summary>
        /// <param name="builder">The builder to use for the operation.</param>
        public override void Configure(
            EntityTypeBuilder<Models.MimeType> builder
            )
        {
            // Setup the table.
            builder.ToTable("MimeTypes", "Obsidian");
                        
            // Setup the property.
            builder.Property(e => e.Id)
                .IsRequired()
                .ValueGeneratedOnAdd();

            // Setup the primary key.
            builder.HasKey(e => new { e.Id });

            // Setup the property.
            builder.Property(e => e.Type)
                .IsRequired()
                .HasMaxLength(127);

            // Setup the property.
            builder.Property(e => e.SubType)
                .IsRequired()
                .HasMaxLength(127);

            // Setup the property.
            builder.Property(e => e.Description)
                .IsRequired()
                .HasMaxLength(128);

            // Setup the property.
            builder.HasMany(e => e.Extensions)
                .WithOne()
                .HasForeignKey(e => e.MimeTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            // Setup the index.
            builder.HasIndex(e => new 
            { 
                e.Type,
                e.SubType
            }).IsUnique();

            // Give the base class a chance.
            base.Configure(builder);
        }

        #endregion
    }
}
