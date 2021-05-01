using CG.DataProtection;
using CG.Options;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CG.Obsidian.Web.Options
{
    /// <summary>
    /// This class contains configuration settings for authenticating the
    /// internal REST API.
    /// </summary>
    public class ApiAuthOptions : OptionsBase
    {
        // *******************************************************************
        // Properties.
        // *******************************************************************

        #region Properties

        /// <summary>
        /// This property contains the name for an API.
        /// </summary>
        [Required]
        public string ApiName { get; set; }

        /// <summary>
        /// This property contains a client secret for the API.
        /// </summary>
        [Required]
        [ProtectedProperty(Optional = true)]
        public string ApiSecret { get; set; }

        #endregion
    }
}
