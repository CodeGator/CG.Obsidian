using CG.Blazor.Components;
using CG.Obsidian.Managers;
using CG.Obsidian.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace CG.Obsidian.Web.Pages.MimeTypes
{
    /// <summary>
    /// This class is the code-behind for the <see cref="Index"/> razor view.
    /// </summary>
    public partial class Index
    {
        // *******************************************************************
        // Fields.
        // *******************************************************************

        #region Fields

        /// <summary>
        /// This field contains a reference to the grid.
        /// </summary>
        private MudTable<MimeType> _ref;

        /// <summary>
        /// This field contains a summary error message.
        /// </summary>
        private string _error;

        /// <summary>
        /// This field contains an information message.
        /// </summary>
        private string _info;

        /// <summary>
        /// This field contains a search string.
        /// </summary>
        private string _searchString = "";

        /// <summary>
        /// This field contains the authentication state.
        /// </summary>
        private AuthenticationState _authState;

        /// <summary>
        /// This fields contains a reference to breadcrumbs for the view.
        /// </summary>
        private readonly List<BreadcrumbItem> _crumbs = new()
        {
            new BreadcrumbItem("Home", href: "/"),
            new BreadcrumbItem("MimeTypes", href: "/mimetypes")
        };

        #endregion

        // *******************************************************************
        // Properties.
        // *******************************************************************

        #region Properties

        /// <summary>
        /// This property contains a reference to a mime type manager.
        /// </summary>
        [Inject]
        private IMimeTypeManager MimeTypeManager { get; set; }

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
        /// This method is called whenever the user presses the add button
        /// </summary>
        private async Task OnAddMimeTypeAsync()
        {
            try
            {
                // Reset any error / information.
                _error = "";
                _info = "";

                // Create a model.
                var model = new MimeType()
                {
                    // Make the form validations happy.
                    CreatedBy = _authState.User.GetEmail(),
                    CreatedDate = DateTime.Now
                };

                // Pass the model to the dialog.
                var parameters = new DialogParameters
                {
                    ["Model"] = model,
                    ["Caption"] = "Add Mime Type"
                };

                // Create the dialog.
                var dialog = DialogService.Show<MimeTypeEditDialog>(
                    "",
                    parameters
                    );

                // Show the dialog.
                var result = await dialog.Result.ConfigureAwait(false);

                // Did the user hit save?
                if (!result.Cancelled)
                {
                    // Defer to the store.
                    _ = await MimeTypeManager.MimeTypes.AddAsync(
                        model
                        ).ConfigureAwait(false);

                    // Tell the world what we did.
                    _info = $"Mime Type: '{model.Description}' was created";
                }
            }
            catch (Exception ex)
            {
                // Save the error.
                _error = ex.Message;
            }

            // Update the UI.
            await InvokeAsync(() => StateHasChanged()).ConfigureAwait(false);
            await InvokeAsync(() => _ref.ReloadServerData());
        }

        // *******************************************************************

        /// <summary>
        /// This method is called whenever the user presses the properties button
        /// for a model.
        /// </summary>
        private async Task OnPropertiesAsync(
            MimeType model
            )
        {
            // Pass in the model.
            var parameters = new DialogParameters
            {
                ["Model"] = model
            };

            // Create the dialog.
            var dialog = DialogService.Show<AuditDialog<MimeType>>(
                "",
                parameters
                );

            // Show the dialog.
            _ = await dialog.Result.ConfigureAwait(false);
        }

        // *******************************************************************

        /// <summary>
        /// This method is called whenever the user presses the delete button
        /// for an existing mime type.
        /// </summary>
        /// <param name="model">The model for the operation.</param>
        private async Task OnDeleteMimeTypeAsync(
            MimeType model
            )
        {
            try
            {
                // Reset any error.
                _error = "";
                _info = "";

                // First, check if we have data hanging off the application.
                if (model.Extensions.Count() > 0)
                {
                    // Prompt the user first.
                    await DialogService.ShowMessageBox(
                        "Problem",
                        $"The mime type '{model.Description}' has one or more file extensions associated with it " +
                        $"that MUST be manually removed before the mime type can be deleted.",
                        yesText: "OK!"
                        );

                    // Nothing left to do.
                    return;
                }

                // Prompt the user first.
                bool? result = await DialogService.ShowMessageBox(
                    "Warning",
                    $"This will delete the mime type: '{model.Description}'.",
                    yesText: "OK!",
                    cancelText: "Cancel"
                    );

                // Did the user press ok?
                if (result != null && result.Value)
                {
                    // Defer to the store.
                    await MimeTypeManager.MimeTypes.DeleteAsync(
                        model.Id
                        ).ConfigureAwait(false);

                    // Tell the world what we did.
                    _info = $"Mime type: '{model.Description}' was deleted";
                }
            }
            catch (Exception ex)
            {
                // Tell the world what happened.
                _error = ex.Message;
            }

            // Update the UI.
            await InvokeAsync(() => StateHasChanged()).ConfigureAwait(false);
            await InvokeAsync(() => _ref.ReloadServerData());
        }

        // *******************************************************************

        /// <summary>
        /// This method is called whenever the user presses the edit button
        /// for an existing mime type.
        /// </summary>
        /// <param name="model">The model for the operation.</param>
        private async Task OnEditMimeTypeAsync(
            MimeType model
            )
        {
            try
            {
                // Reset any error / information.
                _error = "";
                _info = "";

                // Clone the model.
                var temp = model.QuickClone();

                // Make the form validations happy.
                temp.UpdatedBy = _authState.User.GetEmail();
                temp.UpdatedDate = DateTime.Now;

                // Pass the clone to the dialog, so we won't have changed anything
                //   if the user eventually presses cancel.
                var parameters = new DialogParameters
                {
                    ["Model"] = temp,
                    ["Caption"] = $"Edit '{temp.Description}'"
                };

                // Create the dialog.
                var dialog = DialogService.Show<MimeTypeEditDialog>(
                    "",
                    parameters
                    );

                // Show the dialog.
                var result = await dialog.Result.ConfigureAwait(false);

                // Did the user hit save?
                if (!result.Cancelled)
                {
                    // Set the changes to the model.
                    model.Description = temp.Description;
                    model.Type = temp.Type;
                    model.SubType = temp.SubType;
                    model.UpdatedBy = temp.UpdatedBy;
                    model.UpdatedDate = temp.UpdatedDate;
                    model.Extensions = temp.Extensions;

                    // Defer to the store for the mime type changes.
                    _ = await MimeTypeManager.MimeTypes.UpdateAsync(
                        model
                        ).ConfigureAwait(false);

                    // Tell the world what we did.
                    _info = $"Mime Type: '{model.Description}' was updated";
                }
            }
            catch (Exception ex)
            {
                // Tell the world what happened.
                _error = ex.Message;
            }

            // Update the UI.
            await InvokeAsync(() => StateHasChanged()).ConfigureAwait(false);
            await InvokeAsync(() => _ref.ReloadServerData());
        }

        // *******************************************************************

        /// <summary>
        /// This method is called to filter the grid.
        /// </summary>
        /// <param name="text">The string to use for the operation.</param>
        private void OnSearch(string text)
        {
            _searchString = text;
            _ref.ReloadServerData().Wait();
        }

        // *******************************************************************

        /// <summary>
        /// This method is called to get the data for the grid.
        /// </summary>
        /// <param name="state">The state to use for the operation.</param>
        /// <returns>A task to perform the operation, that returns the <see cref="TableData{T}"/>
        /// object required by the <see cref="MudTable{T}"/> grid.</returns>
        private Task<TableData<MimeType>> ServerReload(
            TableState state
            )
        {
            try
            {
                // Defer to the manager for the raw query.
                var data = MimeTypeManager.MimeTypes.AsQueryable();

                // Should we filter/
                if (!string.IsNullOrEmpty(_searchString))
                {
                    // Add the filter.
                    data = data.Where(x =>
                        x.Type.Contains(_searchString) ||
                        x.SubType.Contains(_searchString) ||
                        x.Description.Contains(_searchString)
                        );
                }

                // Get the total number of results.
                var totalItems = data.Count();

                // Sort, as needed.
                switch (state.SortLabel)
                {
                    case "Type":
                        data = data.OrderByDirection(
                            state.SortDirection, o => o.Type
                            ).AsQueryable();
                        break;
                    case "SubType":
                        data = data.OrderByDirection(
                            state.SortDirection, o => o.SubType
                            ).AsQueryable();
                        break;
                    case "Description":
                        data = data.OrderByDirection(
                            state.SortDirection, o => o.Description
                            ).AsQueryable();
                        break;
                }

                // Perform the paging.
                data = data.Skip(state.Page * state.PageSize)
                    .Take(state.PageSize);

                // Warp the results.
                var retData = new TableData<MimeType>()
                {
                    TotalItems = totalItems,
                    Items = data.ToList()
                };

                // Return the results.
                return Task.FromResult(retData);
            }
            catch (Exception ex)
            {
                // Tell the world what happened.
                _error = ex.Message;

                // Return empty results.
                return Task.FromResult(new TableData<MimeType>());
            }
        }

        #endregion
    }
}
