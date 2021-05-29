using CG.Blazor.Components;
using CG.Obsidian.Managers;
using CG.Obsidian.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace CG.Obsidian.Web.Pages.MimeTypes
{
    /// <summary>
    /// This class is the code-behind for the <see cref="MimeTypeEditDialog"/> razor view.
    /// </summary>
    public partial class MimeTypeEditDialog
    {
        // *******************************************************************
        // Fields.
        // *******************************************************************

        #region Fields

        /// <summary>
        /// This field contains the authentication state.
        /// </summary>
        private AuthenticationState _authState;

        #endregion

        // *******************************************************************
        // Properties.
        // *******************************************************************

        #region Properties

        /// <summary>
        /// This property contains a reference to dialog service.
        /// </summary>
        [Inject]
        private IDialogService DialogService { get; set; }

        /// <summary>
        /// This property contains a reference to an authentication state provider.
        /// </summary>
        [Inject]
        private AuthenticationStateProvider AuthenticationStateProvider { get; set; }

        /// <summary>
        /// This property contains a reference to the dialog.
        /// </summary>
        [CascadingParameter]
        MudDialogInstance MudDialog { get; set; }

        /// <summary>
        /// This property contains a reference to the model.
        /// </summary>
        [Parameter]
        public MimeType Model { get; set; }

        /// <summary>
        /// This property contains a reference to the dialog caption.
        /// </summary>
        [Parameter]
        public string Caption { get; set; }

        #endregion

        // *******************************************************************
        // Protected methods.
        // *******************************************************************

        #region Protected methods

        /// <summary>
        /// This method invoked when the component is ready to start, having received 
        /// its initial parameters from its parent in the render tree. 
        /// </summary>
        /// <returns>A task to perform the operation.</returns>
        protected override async Task OnInitializedAsync()
        {
            // Get the current authentication state.
            _authState = await AuthenticationStateProvider.GetAuthenticationStateAsync()
                .ConfigureAwait(false);

            // Give the base class a chance.s
            await base.OnInitializedAsync();
        }

        #endregion

        // *******************************************************************
        // Private methods.
        // *******************************************************************

        #region Private methods

        /// <summary>
        /// This method is called whenever the user cancels the dialog.
        /// </summary>
        private void Cancel()
        {
            MudDialog.Cancel();
        }

        // *******************************************************************

        /// <summary>
        /// This method is called whenever the user submits the dialog.
        /// </summary>
        /// <param name="context">The edit context to use for the operation.</param>
        private void OnValidSubmit(EditContext context)
        {
            if (context.Validate())
            {
                MudDialog.Close(DialogResult.Ok(Model.Id));
            }
        }

        // *******************************************************************

        /// <summary>
        /// This method is called whenever the user presses the edit button
        /// for an existing file extension.
        /// </summary>
        /// <param name="model">The model for the operation.</param>
        private async Task OnEditFileExtensionAsync(
            FileExtension model
            )
        {
            // Clone the model.
            var temp = model.QuickClone();

            // Make the form validations happy.
            temp.UpdatedBy = _authState.User.GetEmail();
            temp.UpdatedDate = DateTime.Now;

            // Pass the model to the dialog.
            var parameters = new DialogParameters
            {
                ["Model"] = temp,
                ["Caption"] = $"Edit '{temp.Extension}'"
            };

            // Create the dialog.
            var dialog = DialogService.Show<FileExtensionEditDialog>(
                "",
                parameters
                );

            // Show the dialog.
            var result = await dialog.Result.ConfigureAwait(false);

            // Did the user hit save?
            if (!result.Cancelled)
            {
                // Set the changes to the model.
                model.Extension = temp.Extension;
                model.UpdatedBy = temp.UpdatedBy;
                model.UpdatedDate = temp.UpdatedDate;
            }
        }

        // *******************************************************************

        /// <summary>
        /// This method is called whenever the user presses the delete button
        /// for an existing file extension.
        /// </summary>
        /// <param name="model">The model for the operation.</param>
        private async Task OnDeleteFileExtensionAsync(
            FileExtension model
            )
        {
            // Prompt the user first.
            bool? result = await DialogService.ShowMessageBox(
                "Warning",
                $"This will delete the file extension: '{model.Extension}'.",
                yesText: "OK!",
                cancelText: "Cancel"
                );

            // Did the user press ok?
            if (result != null && result.Value)
            {
                // Remove from the parent.
                Model.Extensions.Remove(model);
            }
        }

        // *******************************************************************

        /// <summary>
        /// This method is called whenever the user presses the add button
        /// for a new file extension.
        /// </summary>
        /// <param name="mimeType">The mime type for the operation.</param>
        private async Task OnAddFileExtensionAsync(
            MimeType mimeType
            )
        {
            // Create a model.
            var model = new FileExtension()
            {
                // Make the form validations happy.
                CreatedBy = _authState.User.GetEmail(),
                CreatedDate = DateTime.Now,
                    
                MimeTypeId = Model.Id // Associate with the parent.
            };

            // Pass the model to the dialog.
            var parameters = new DialogParameters
            {
                ["Model"] = model,
                ["Caption"] = $"Add Extension"
            };

            // Create the dialog.
            var dialog = DialogService.Show<FileExtensionEditDialog>(
                "",
                parameters
                );

            // Show the dialog.
            var result = await dialog.Result.ConfigureAwait(false);

            // Did the user hit save?
            if (!result.Cancelled)
            {
                // Add to the parent.
                Model.Extensions.Add(model);
            }
        }

        // *******************************************************************

        /// <summary>
        /// This method is called whenever the user presses the properties button
        /// for an existing file extension.
        /// </summary>
        /// <param name="model">The model for the operation.</param>
        private async Task OnFileExtensionsPropertiesAsync(
            FileExtension model
            )
        {
            // Pass in the model.
            var parameters = new DialogParameters
            {
                ["Model"] = model
            };

            // Create the dialog.
            var dialog = DialogService.Show<AuditDialog<FileExtension>>(
                "",
                parameters
                );

            // Show the dialog.
            _ = await dialog.Result.ConfigureAwait(false);
        }

        #endregion
    }
}
