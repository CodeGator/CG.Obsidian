using CG.Obsidian.Managers;
using CG.Validations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;

namespace CG.Obsidian.Web.Controllers
{
    /// <summary>
    /// This class is a REST controller for mime type operations.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class MimeTypesController : ControllerBase
    {
        // *******************************************************************
        // Fields.
        // *******************************************************************

        #region Fields

        /// <summary>
        /// This field contains a reference to a mime type manager.
        /// </summary>
        private readonly IMimeTypeManager _mimeTypeManager;

        /// <summary>
        /// This field contains a reference to a logger.
        /// </summary>
        private readonly ILogger<MimeTypesController> _logger;

        #endregion

        // *******************************************************************
        // Constructors.
        // *******************************************************************

        #region Constructors

        /// <summary>
        /// This constructor creates a new instance of the <see cref="MimeTypesController"/>
        /// class.
        /// </summary>
        /// <param name="mimeTypeManager">The mime type manager to use with 
        /// the controller.</param>
        /// <param name="logger">The logger to use with the controller.</param>
        public MimeTypesController(
            IMimeTypeManager mimeTypeManager,
            ILogger<MimeTypesController> logger
            )
        {
            // Validate the parameters before attempting to use them.
            Guard.Instance().ThrowIfNull(mimeTypeManager, nameof(mimeTypeManager))
                .ThrowIfNull(logger, nameof(logger));

            // Save the references.
            _mimeTypeManager = mimeTypeManager;
            _logger = logger;
        }

        #endregion

        // *******************************************************************
        // Public methods.
        // *******************************************************************

        #region Public methods

        /// <summary>
        /// This method returns a list of mime types that are associated with
        /// the specified file extension.
        /// </summary>
        /// <param name="extension">The extension to use for the operation.</param>
        /// <returns>A task to perform the operation that returns a <see cref="IActionResult"/>
        /// instance, with the results of the operation.</returns>
        [AllowAnonymous]
        [HttpPost]
        [Produces(MediaTypeNames.Application.Json)]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public virtual async Task<IActionResult> FindByExtensionAsync(
            [FromBody] string extension
            )
        {
            try
            {
                // Is the model invalid?
                if (false == ModelState.IsValid)
                {
                    // Tell the caller what happened.
                    return BadRequest();
                }

                // Defer to the manager.
                var mimeTypes = await _mimeTypeManager.FindByExtensionAsync(
                    extension
                    ).ConfigureAwait(false);

                // We only want mime type strings, not the actual models.
                var results = mimeTypes.Select(
                    x => $"{x.Type}/{x.SubType}"
                    ).ToList();

                // Return the results.
                return Ok(results);
            }
            catch (Exception ex)
            {
                // Tell the world what happened.
                _logger.LogError(
                    ex,
                    "Failed to query for mime type(s) by file extension!",
                    extension
                    );

                // Return a summary of the problem.
                return Problem(
                    title: $"{nameof(MimeTypesController)} error!",
                    detail: ex.Message,
                    statusCode: StatusCodes.Status500InternalServerError
                    );
            }
        }

        #endregion
    }
}
