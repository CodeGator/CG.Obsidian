using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;

namespace CG.Obsidian.Web.Pages.Account
{
    /// <summary>
    /// This class is the code-behind for the Login page.
    /// </summary>
    public class LoginModel : PageModel
    {
        // *******************************************************************
        // Public methods.
        // *******************************************************************

        #region Public methods

        /// <summary>
        /// This method is called whenever the page is sent a GET verb.
        /// </summary>
        /// <param name="redirectUri">The uri for a redirect.</param>
        /// <returns>A task to perform the operation.</returns>
        public async Task OnGetAsync(
            string redirectUri
            )
        {
            if (string.IsNullOrWhiteSpace(redirectUri))
            {
                redirectUri = Url.Content("~/");
            }

            if (HttpContext.User.Identity.IsAuthenticated)
            {
                Response.Redirect(redirectUri);
            }

            await HttpContext.ChallengeAsync(
               OpenIdConnectDefaults.AuthenticationScheme,
               new AuthenticationProperties { RedirectUri = redirectUri }
               );
        }

        #endregion
    }
}
