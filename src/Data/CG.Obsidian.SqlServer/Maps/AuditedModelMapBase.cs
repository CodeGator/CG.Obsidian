using CG.Business.Models;
using CG.Obsidian.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace CG.Obsidian.SqlServer.Maps
{
    /// <summary>
    /// This class is a base entity mapping for an <see cref="AuditedModelBase"/>
    /// derived type.
    /// </summary>
    /// <typeparam name="T">The type of associated model.</typeparam>
    internal abstract class AuditedModelMapBase<T> : IEntityTypeConfiguration<T>
        where T : AuditedModelBase
    {
        // *******************************************************************
        // Public methods.
        // *******************************************************************

        #region Public methods

        /// <summary>
        /// This method configures the <see cref="AuditedModelBase"/> portion 
        /// of the specified entity.
        /// </summary>
        /// <param name="builder">The builder to use for the operation.</param>
        public virtual void Configure(
            EntityTypeBuilder<T> builder
            )
        {
            // Setup the property.
            builder.Property(e => e.CreatedBy)
                .HasMaxLength(50)
                .IsRequired();

            // Setup the property.
            builder.Property(e => e.CreatedDate)
                .IsRequired()
                .HasDefaultValue(DateTime.Now);

            // Setup the property.
            builder.Property(e => e.UpdatedBy)
                .HasMaxLength(50);

            // Setup the property.
            builder.Property(e => e.UpdatedDate);
        }

        #endregion
    }
}
