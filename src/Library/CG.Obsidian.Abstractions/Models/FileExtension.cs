using CG.Business.Models;
using System;
using System.ComponentModel.DataAnnotations;

namespace CG.Obsidian.Models
{
    /// <summary>
    /// This class is a model that represents a file extension 
    /// assocaited with a MIME type.
    /// </summary>
    public class FileExtension : AuditedModelBase
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
        /// This property contains the identifier for the associated mime type.
        /// </summary>
        public int MimeTypeId { get; set; }

        /// <summary>
        /// This property contains a file extension.
        /// </summary>
        [Required]
        [MaxLength(260)]
        public string Extension { get; set; }

        #endregion
    }
}
