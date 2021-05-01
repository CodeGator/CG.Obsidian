using CG.Linq.EFCore.Repositories.Options;
using System;

namespace CG.Obsidian.SqlServer.Repositories.Options
{
    /// <summary>
    /// This class represents repository configuration options
    /// for all the <see cref="CG.Obsidian"/> specific repositories.
    /// </summary>
    public class ObsidianRepositoryOptions : EFCoreRepositoryOptions
    {
        // *******************************************************************
        // Properties.
        // *******************************************************************

        #region Properties

        /// <summary>
        /// This property indicates whether the repositories should log their
        /// internal SQL to the logger.
        /// </summary>
        public bool LogAllQueries { get; set; }

        #endregion
    }
}
