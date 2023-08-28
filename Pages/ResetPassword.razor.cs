using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Radzen;

namespace SimplyMTD.Pages
{
    public partial class ResetPassword
    {
        [Inject]
        protected IJSRuntime JSRuntime { get; set; }

        [Inject]
        protected NavigationManager NavigationManager { get; set; }

        [Inject]
        protected DialogService DialogService { get; set; }

        [Inject]
        protected TooltipService TooltipService { get; set; }

        [Inject]
        protected ContextMenuService ContextMenuService { get; set; }

        [Inject]
        protected NotificationService NotificationService { get; set; }

        protected SimplyMTD.Models.ApplicationUser user;
        protected bool isBusy;
        protected bool errorVisible;
        protected string error;

        [Inject]
        protected SecurityService Security { get; set; }

        protected override async Task OnInitializedAsync()
        {
            user = new SimplyMTD.Models.ApplicationUser();
        }

        protected async Task FormSubmit()
        {
            try
            {
                isBusy = true;

                await Security.ResetPassword(user.Email);

                DialogService.Close(true);
            }
            catch (Exception ex)
            {
                errorVisible = true;
                error = ex.Message;
            }

            isBusy = false;
        }

        protected async Task CancelClick()
        {
            DialogService.Close(false);
        }
    }
}