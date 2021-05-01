using CG.Blazor.Identity.Options;
using System;
using System.ComponentModel.DataAnnotations;

namespace CG.Obsidian.Web.Options
{
    /// <summary>
    /// This class represents configuration options for identity activities.
    /// </summary>
    public class IdentityOptions : TokenServiceOptions
    {
        // *******************************************************************
        // Properties.
        // *******************************************************************

        #region Properties

        /// <summary>
        /// This property contains configuration settings for authenticating the 
        /// internal REST API.
        /// </summary>
        [Required]
        public ApiAuthOptions API { get; set; }

        #endregion
    }
}
