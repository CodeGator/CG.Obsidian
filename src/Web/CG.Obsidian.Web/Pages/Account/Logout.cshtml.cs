using CG.Obsidian.Web.Options;
using CG.Validations;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;

#pragma warning disable CS1998

namespace CG.Obsidian.Web.Pages.Account
{
    /// <summary>
    /// This class is the code-behind for the Logout page.
    /// </summary>
    public class LogoutModel : PageModel
    {
        // *******************************************************************
        // Fields.
        // *******************************************************************

        #region Fields

        /// <summary>
        /// This field contains a reference to the identity options.
        /// </summary>
        private IdentityOptions _identityOptions;

        #endregion

        // *******************************************************************
        // Constructors.
        // *******************************************************************

        #region Constructors

        /// <summary>
        /// This constructor creates a new instance of the <see cref="LogoutModel"/>
        /// class.
        /// </summary>
        /// <param name="identityOptions">The identity options to use for the page.</param>
        public LogoutModel(
            IOptions<IdentityOptions> identityOptions
            )
        {
            // Validate the parameters before attempting to use them.
            Guard.Instance().ThrowIfNull(identityOptions, nameof(identityOptions));

            // Save the references.
            _identityOptions = identityOptions.Value;
        }

        #endregion

        // *******************************************************************
        // Public methods.
        // *******************************************************************

        #region Public methods

        /// <summary>
        /// This method is called whenever the page is sent a POST verb.
        /// </summary>
        /// <returns>A task to perform the operation, that returns an <see cref="IActionResult"/>
        /// object.</returns>
        public async Task<IActionResult> OnPostAsync()
        {
            return SignOut(
                new AuthenticationProperties
                {
                    RedirectUri = _identityOptions.Authority
                },
                OpenIdConnectDefaults.AuthenticationScheme,
                CookieAuthenticationDefaults.AuthenticationScheme
                );
        }

        #endregion
    }
}