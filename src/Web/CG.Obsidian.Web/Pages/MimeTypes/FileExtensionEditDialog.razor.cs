using CG.Obsidian.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;
using System;
using System.Threading.Tasks;

namespace CG.Obsidian.Web.Pages.MimeTypes
{
    /// <summary>
    /// This class is the code-behind for the <see cref="FileExtensionEditDialog"/> razor view.
    /// </summary>
    public partial class FileExtensionEditDialog
    {
        // *******************************************************************
        // Properties.
        // *******************************************************************

        #region Properties

        /// <summary>
        /// This property contains a reference to the dialog.
        /// </summary>
        [CascadingParameter]
        MudDialogInstance MudDialog { get; set; }

        /// <summary>
        /// This property contains a reference to the model.
        /// </summary>
        [Parameter]
        public FileExtension Model { get; set; }

        /// <summary>
        /// This property contains a reference to the dialog caption.
        /// </summary>
        [Parameter]
        public string Caption { get; set; }

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

        #endregion
    }
}
