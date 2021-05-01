using CG.Business.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CG.Obsidian.Models
{
    /// <summary>
    /// This class is a model that represents a MIME type.
    /// </summary>
    public class MimeType : AuditedModelBase
    {
        // *******************************************************************
        // Properties.
        // *******************************************************************

        #region Properties

        /// <summary>
        /// This property contains a unique identifier for the model.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// This property contains the MIME type.
        /// </summary>
        [Required]
        [MaxLength(127)]
        public string Type { get; set; }

        /// <summary>
        /// This property contains the MIME sub-type.
        /// </summary>
        [Required]
        [MaxLength(127)]
        public string SubType { get; set; }

        /// <summary>
        /// This property contains a description of the mime type.
        /// </summary>
        [MaxLength(128)]
        public string Description { get; set; }

        /// <summary>
        /// This property contains a list of associated file extensions.
        /// </summary>
        public IList<FileExtension> Extensions { get; set; }

        #endregion

        // *******************************************************************
        // Constructors.
        // *******************************************************************

        #region Constructors

        /// <summary>
        /// This concstructor creates a new instance of the <see cref="MimeType"/>
        /// class.
        /// </summary>
        public MimeType()
        {
            // Create default values.
            Extensions = new List<FileExtension>();
        }

        #endregion
    }
}
